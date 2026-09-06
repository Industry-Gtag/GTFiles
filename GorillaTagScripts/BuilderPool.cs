using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F5C RID: 3932
	public class BuilderPool : MonoBehaviour, IGorillaSimpleBackgroundWorker
	{
		// Token: 0x060060B8 RID: 24760 RVA: 0x001EAC92 File Offset: 0x001E8E92
		private void Awake()
		{
			if (BuilderPool.instance == null)
			{
				BuilderPool.instance = this;
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x060060B9 RID: 24761 RVA: 0x001EACB0 File Offset: 0x001E8EB0
		public void Setup()
		{
			if (this.isSetup)
			{
				return;
			}
			this.piecePools = new List<List<BuilderPiece>>(512);
			this.piecePoolLookup = new Dictionary<int, int>(512);
			this.bumpGlowPool = new List<BuilderBumpGlow>(256);
			this.AddToGlowBumpPool(256);
			this.snapOverlapPool = new List<SnapOverlap>(4096);
			this.AddToSnapOverlapPool(4096);
			this.isSetup = true;
		}

		// Token: 0x060060BA RID: 24762 RVA: 0x001EAD24 File Offset: 0x001E8F24
		public void BuildFromShelves(List<BuilderShelf> shelves)
		{
			for (int i = 0; i < shelves.Count; i++)
			{
				BuilderShelf builderShelf = shelves[i];
				for (int j = 0; j < builderShelf.buildPieceSpawns.Count; j++)
				{
					BuilderShelf.BuildPieceSpawn buildPieceSpawn = builderShelf.buildPieceSpawns[j];
					this.AddToPool(buildPieceSpawn.buildPiecePrefab.name.GetStaticHash(), buildPieceSpawn.count);
				}
			}
		}

		// Token: 0x060060BB RID: 24763 RVA: 0x001EAD89 File Offset: 0x001E8F89
		public IEnumerator BuildFromPieceSets()
		{
			if (this.hasBuiltPieceSets)
			{
				yield break;
			}
			this.hasBuiltPieceSets = true;
			List<BuilderPieceSet> allPieceSets = BuilderSetManager.instance.GetAllPieceSets();
			foreach (BuilderPieceSet builderPieceSet in allPieceSets)
			{
				bool isStarterSet = BuilderSetManager.instance.GetStarterSetsConcat().Contains(builderPieceSet.playfabID);
				bool isFallbackSet = builderPieceSet.SetName.Equals("HIDDEN");
				foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in builderPieceSet.subsets)
				{
					foreach (BuilderPieceSet.PieceInfo pieceInfo in builderPieceSubset.pieceInfos)
					{
						int staticHash = pieceInfo.piecePrefab.name.GetStaticHash();
						int count;
						if (!this.piecePoolLookup.TryGetValue(staticHash, out count))
						{
							count = this.piecePools.Count;
							this.piecePools.Add(new List<BuilderPiece>(128));
							this.piecePoolLookup.Add(staticHash, count);
							if (!isFallbackSet)
							{
								int num = (isStarterSet ? 32 : 8);
								int i = 0;
								while (i < num)
								{
									if (this.piecesToAdd.Count == 0)
									{
										GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
									}
									i += 2;
									this.piecesToAdd.Enqueue(staticHash);
								}
							}
						}
						yield return null;
					}
					List<BuilderPieceSet.PieceInfo>.Enumerator enumerator3 = default(List<BuilderPieceSet.PieceInfo>.Enumerator);
				}
				List<BuilderPieceSet.BuilderPieceSubset>.Enumerator enumerator2 = default(List<BuilderPieceSet.BuilderPieceSubset>.Enumerator);
			}
			List<BuilderPieceSet>.Enumerator enumerator = default(List<BuilderPieceSet>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060060BC RID: 24764 RVA: 0x001EAD98 File Offset: 0x001E8F98
		public void SimpleWork()
		{
			int num = 2;
			if (this.piecesToAdd.Count > 0)
			{
				this.AddToPool(this.piecesToAdd.Dequeue(), num);
			}
			if (this.piecesToAdd.Count > 0)
			{
				GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
			}
		}

		// Token: 0x060060BD RID: 24765 RVA: 0x001EADDC File Offset: 0x001E8FDC
		private void AddToPool(int pieceType, int count)
		{
			int count2;
			if (!this.piecePoolLookup.TryGetValue(pieceType, out count2))
			{
				count2 = this.piecePools.Count;
				this.piecePools.Add(new List<BuilderPiece>(count * 8));
				this.piecePoolLookup.Add(pieceType, count2);
				Debug.LogWarningFormat("Creating Pool for piece {0} of size {1}. Is this piece not in a piece set?", new object[]
				{
					pieceType,
					count * 8
				});
			}
			BuilderPiece piecePrefab = BuilderSetManager.instance.GetPiecePrefab(pieceType);
			if (piecePrefab == null)
			{
				return;
			}
			List<BuilderPiece> list = this.piecePools[count2];
			for (int i = 0; i < count; i++)
			{
				BuilderPiece builderPiece = Object.Instantiate<BuilderPiece>(piecePrefab);
				builderPiece.OnCreatedByPool();
				builderPiece.gameObject.SetActive(false);
				list.Add(builderPiece);
			}
		}

		// Token: 0x060060BE RID: 24766 RVA: 0x001EAEA0 File Offset: 0x001E90A0
		public BuilderPiece CreatePiece(int pieceType, bool assertNotEmpty)
		{
			int count;
			if (!this.piecePoolLookup.TryGetValue(pieceType, out count))
			{
				if (assertNotEmpty)
				{
					Debug.LogErrorFormat("No Pool Found for {0} Adding 4", new object[] { pieceType });
				}
				count = this.piecePools.Count;
				this.AddToPool(pieceType, 4);
			}
			List<BuilderPiece> list = this.piecePools[count];
			if (list.Count == 0)
			{
				if (assertNotEmpty)
				{
					Debug.LogErrorFormat("Pool for {0} is Empty Adding 4", new object[] { pieceType });
				}
				this.AddToPool(pieceType, 4);
			}
			BuilderPiece builderPiece = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return builderPiece;
		}

		// Token: 0x060060BF RID: 24767 RVA: 0x001EAF44 File Offset: 0x001E9144
		public void DestroyPiece(BuilderPiece piece)
		{
			if (piece == null)
			{
				Debug.LogError("Why is a null piece being destroyed");
				return;
			}
			int num;
			if (!this.piecePoolLookup.TryGetValue(piece.pieceType, out num))
			{
				Debug.LogErrorFormat("No Pool Found for {0} Cannot return to pool", new object[] { piece.pieceType });
				return;
			}
			List<BuilderPiece> list = this.piecePools[num];
			if (list.Count == 128)
			{
				piece.OnReturnToPool();
				Object.Destroy(piece.gameObject);
				return;
			}
			piece.gameObject.SetActive(false);
			piece.transform.SetParent(null);
			piece.transform.SetPositionAndRotation(Vector3.up * 10000f, Quaternion.identity);
			piece.OnReturnToPool();
			list.Add(piece);
		}

		// Token: 0x060060C0 RID: 24768 RVA: 0x001EB00C File Offset: 0x001E920C
		private void AddToGlowBumpPool(int count)
		{
			if (this.bumpGlowPrefab == null)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				BuilderBumpGlow builderBumpGlow = Object.Instantiate<BuilderBumpGlow>(this.bumpGlowPrefab);
				builderBumpGlow.gameObject.SetActive(false);
				this.bumpGlowPool.Add(builderBumpGlow);
			}
		}

		// Token: 0x060060C1 RID: 24769 RVA: 0x001EB058 File Offset: 0x001E9258
		public BuilderBumpGlow CreateGlowBump()
		{
			if (this.bumpGlowPool.Count == 0)
			{
				this.AddToGlowBumpPool(4);
			}
			BuilderBumpGlow builderBumpGlow = this.bumpGlowPool[this.bumpGlowPool.Count - 1];
			this.bumpGlowPool.RemoveAt(this.bumpGlowPool.Count - 1);
			return builderBumpGlow;
		}

		// Token: 0x060060C2 RID: 24770 RVA: 0x001EB0AC File Offset: 0x001E92AC
		public void DestroyBumpGlow(BuilderBumpGlow bump)
		{
			if (bump == null)
			{
				return;
			}
			bump.gameObject.SetActive(false);
			bump.transform.SetPositionAndRotation(Vector3.up * 10000f, Quaternion.identity);
			this.bumpGlowPool.Add(bump);
		}

		// Token: 0x060060C3 RID: 24771 RVA: 0x001EB0FC File Offset: 0x001E92FC
		private void AddToSnapOverlapPool(int count)
		{
			this.snapOverlapPool.Capacity = this.snapOverlapPool.Capacity + count;
			for (int i = 0; i < count; i++)
			{
				this.snapOverlapPool.Add(new SnapOverlap
				{
					inPool = true
				});
			}
		}

		// Token: 0x060060C4 RID: 24772 RVA: 0x001EB144 File Offset: 0x001E9344
		public SnapOverlap CreateSnapOverlap(BuilderAttachGridPlane otherPlane, SnapBounds bounds)
		{
			if (this.snapOverlapPool.Count == 0)
			{
				this.AddToSnapOverlapPool(1024);
			}
			SnapOverlap snapOverlap = this.snapOverlapPool[this.snapOverlapPool.Count - 1];
			this.snapOverlapPool.RemoveAt(this.snapOverlapPool.Count - 1);
			snapOverlap.otherPlane = otherPlane;
			snapOverlap.bounds = bounds;
			snapOverlap.nextOverlap = null;
			snapOverlap.inPool = false;
			return snapOverlap;
		}

		// Token: 0x060060C5 RID: 24773 RVA: 0x001EB1B5 File Offset: 0x001E93B5
		public void DestroySnapOverlap(SnapOverlap snapOverlap)
		{
			if (snapOverlap.inPool)
			{
				return;
			}
			snapOverlap.otherPlane = null;
			snapOverlap.nextOverlap = null;
			snapOverlap.inPool = true;
			this.snapOverlapPool.Add(snapOverlap);
		}

		// Token: 0x060060C6 RID: 24774 RVA: 0x001EB1E4 File Offset: 0x001E93E4
		private void OnDestroy()
		{
			for (int i = 0; i < this.piecePools.Count; i++)
			{
				if (this.piecePools[i] != null)
				{
					foreach (BuilderPiece builderPiece in this.piecePools[i])
					{
						if (builderPiece != null)
						{
							Object.Destroy(builderPiece);
						}
					}
					this.piecePools[i].Clear();
				}
			}
			this.piecePoolLookup.Clear();
			foreach (BuilderBumpGlow builderBumpGlow in this.bumpGlowPool)
			{
				Object.Destroy(builderBumpGlow);
			}
			this.bumpGlowPool.Clear();
		}

		// Token: 0x04006F55 RID: 28501
		public List<List<BuilderPiece>> piecePools;

		// Token: 0x04006F56 RID: 28502
		public Dictionary<int, int> piecePoolLookup;

		// Token: 0x04006F57 RID: 28503
		[HideInInspector]
		public List<BuilderBumpGlow> bumpGlowPool;

		// Token: 0x04006F58 RID: 28504
		public BuilderBumpGlow bumpGlowPrefab;

		// Token: 0x04006F59 RID: 28505
		[HideInInspector]
		public List<SnapOverlap> snapOverlapPool;

		// Token: 0x04006F5A RID: 28506
		public static BuilderPool instance;

		// Token: 0x04006F5B RID: 28507
		private const int POOl_CAPACITY = 128;

		// Token: 0x04006F5C RID: 28508
		private const int INITIAL_INSTANCE_COUNT_STARTER = 32;

		// Token: 0x04006F5D RID: 28509
		private const int INITIAL_INSTANCE_COUNT_PREMIUM = 8;

		// Token: 0x04006F5E RID: 28510
		private bool isSetup;

		// Token: 0x04006F5F RID: 28511
		private bool hasBuiltPieceSets;

		// Token: 0x04006F60 RID: 28512
		private Queue<int> piecesToAdd = new Queue<int>();
	}
}
