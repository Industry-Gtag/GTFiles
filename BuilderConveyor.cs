using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

// Token: 0x0200062C RID: 1580
public class BuilderConveyor : MonoBehaviour
{
	// Token: 0x06002749 RID: 10057 RVA: 0x000CFAB8 File Offset: 0x000CDCB8
	private void Start()
	{
		this.InitIfNeeded();
	}

	// Token: 0x0600274A RID: 10058 RVA: 0x000CFAB8 File Offset: 0x000CDCB8
	public void Setup()
	{
		this.InitIfNeeded();
	}

	// Token: 0x0600274B RID: 10059 RVA: 0x000CFAC0 File Offset: 0x000CDCC0
	private void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.nextPieceToSpawn = 0;
		this.grabbedPieceTypes = new Queue<int>(10);
		this.grabbedPieceMaterials = new Queue<int>(10);
		this.setSelector.Setup(this._includeCategories);
		this.currentDisplayGroup = this.setSelector.GetSelectedGroup();
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentDisplayGroup.pieceSubsets)
		{
			if (this._includeCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		double timeAsDouble = Time.timeAsDouble;
		this.nextSpawnTime = timeAsDouble + (double)this.spawnDelay;
		this.setSelector.OnSelectedGroup.AddListener(new UnityAction<int>(this.OnSelectedSetChange));
		this.initialized = true;
		this.splineLength = this.spline.Splines[0].GetLength();
		this.maxItemsOnSpline = Mathf.RoundToInt(this.splineLength / (this.conveyorMoveSpeed * this.spawnDelay)) + 5;
		this.nativeSpline = new NativeSpline(this.spline.Splines[0], this.spline.transform.localToWorldMatrix, Allocator.Persistent);
	}

	// Token: 0x0600274C RID: 10060 RVA: 0x000CFC34 File Offset: 0x000CDE34
	public int GetMaxItemsOnConveyor()
	{
		return Mathf.RoundToInt(this.splineLength / (this.conveyorMoveSpeed * this.spawnDelay)) + 5;
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x000CFC51 File Offset: 0x000CDE51
	public float GetFrameMovement()
	{
		return this.conveyorMoveSpeed / this.splineLength;
	}

	// Token: 0x0600274E RID: 10062 RVA: 0x000CFC60 File Offset: 0x000CDE60
	private void OnDestroy()
	{
		if (this.setSelector != null)
		{
			this.setSelector.OnSelectedGroup.RemoveListener(new UnityAction<int>(this.OnSelectedSetChange));
		}
		this.nativeSpline.Dispose();
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x000CFC97 File Offset: 0x000CDE97
	public void OnSelectedSetChange(int displayGroupID)
	{
		if (this.table.GetTableState() != BuilderTable.TableState.Ready)
		{
			return;
		}
		this.table.RequestShelfSelection(this.shelfID, displayGroupID, true);
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x000CFCBC File Offset: 0x000CDEBC
	public void SetSelection(int displayGroupID)
	{
		this.setSelector.SetSelection(displayGroupID);
		this.currentDisplayGroup = this.setSelector.GetSelectedGroup();
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentDisplayGroup.pieceSubsets)
		{
			if (this._includeCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		this.nextPieceToSpawn = 0;
		this.loopCount = 0;
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x000CFD68 File Offset: 0x000CDF68
	public int GetSelectedDisplayGroupID()
	{
		return this.setSelector.GetSelectedGroup().GetDisplayGroupIdentifier();
	}

	// Token: 0x06002752 RID: 10066 RVA: 0x000CFD7C File Offset: 0x000CDF7C
	public void UpdateConveyor()
	{
		if (!this.initialized)
		{
			this.Setup();
		}
		for (int i = this.piecesOnConveyor.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.piecesOnConveyor[i];
			if (builderPiece.state != BuilderPiece.State.OnConveyor)
			{
				if (PhotonNetwork.LocalPlayer.IsMasterClient && builderPiece.state != BuilderPiece.State.None)
				{
					this.grabbedPieceTypes.Enqueue(builderPiece.pieceType);
					this.grabbedPieceMaterials.Enqueue(builderPiece.materialType);
				}
				builderPiece.shelfOwner = -1;
				this.piecesOnConveyor.RemoveAt(i);
				this.table.conveyorManager.RemovePieceFromJob(builderPiece);
			}
		}
	}

	// Token: 0x06002753 RID: 10067 RVA: 0x000CFE20 File Offset: 0x000CE020
	public void RemovePieceFromConveyor(Transform pieceTransform)
	{
		foreach (BuilderPiece builderPiece in this.piecesOnConveyor)
		{
			if (builderPiece.transform == pieceTransform)
			{
				this.piecesOnConveyor.Remove(builderPiece);
				builderPiece.shelfOwner = -1;
				this.table.RequestRecyclePiece(builderPiece, false, -1);
				break;
			}
		}
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x000CFEA0 File Offset: 0x000CE0A0
	private Vector3 EvaluateSpline(float t)
	{
		float num;
		this._evaluateCurve = this.nativeSpline.GetCurve(this.nativeSpline.SplineToCurveT(t, out num));
		return CurveUtility.EvaluatePosition(this._evaluateCurve, num);
	}

	// Token: 0x06002755 RID: 10069 RVA: 0x000CFEE0 File Offset: 0x000CE0E0
	public void UpdateShelfSliced()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		if (this.shouldVerifySetSelection)
		{
			BuilderPieceSet.BuilderDisplayGroup selectedGroup = this.setSelector.GetSelectedGroup();
			if (selectedGroup == null || !BuilderSetManager.instance.DoesAnyPlayerInRoomOwnPieceSet(selectedGroup.setID))
			{
				int defaultGroupID = this.setSelector.GetDefaultGroupID();
				if (defaultGroupID != -1)
				{
					this.OnSelectedSetChange(defaultGroupID);
				}
			}
			this.shouldVerifySetSelection = false;
		}
		if (this.waitForResourceChange)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble >= this.nextSpawnTime)
		{
			this.SpawnNextPiece();
			this.nextSpawnTime = timeAsDouble + (double)this.spawnDelay;
		}
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x000CFF70 File Offset: 0x000CE170
	public void VerifySetSelection()
	{
		this.shouldVerifySetSelection = true;
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x000CFF79 File Offset: 0x000CE179
	public void OnAvailableResourcesChange()
	{
		this.waitForResourceChange = false;
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x000CFF82 File Offset: 0x000CE182
	public Transform GetSpawnTransform()
	{
		return this.spawnTransform;
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000CFF8C File Offset: 0x000CE18C
	public void OnShelfPieceCreated(BuilderPiece piece, float timeOffset)
	{
		float num = timeOffset * this.conveyorMoveSpeed / this.splineLength;
		if (num > 1f)
		{
			Debug.LogWarningFormat("Piece {0} add to shelf time {1}", new object[] { piece.pieceId, num });
		}
		int count = this.piecesOnConveyor.Count;
		this.piecesOnConveyor.Add(piece);
		float num2 = Mathf.Clamp(num, 0f, 1f);
		Vector3 vector = this.EvaluateSpline(num2);
		Quaternion quaternion = this.spawnTransform.rotation * Quaternion.Euler(piece.desiredShelfRotationOffset);
		Vector3 vector2 = vector + this.spawnTransform.rotation * piece.desiredShelfOffset;
		piece.transform.SetPositionAndRotation(vector2, quaternion);
		if (num <= 1f)
		{
			this.table.conveyorManager.AddPieceToJob(piece, num2, this.shelfID);
		}
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x000D006D File Offset: 0x000CE26D
	public void OnShelfPieceRecycled(BuilderPiece piece)
	{
		this.piecesOnConveyor.Remove(piece);
		if (piece != null)
		{
			this.table.conveyorManager.RemovePieceFromJob(piece);
		}
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x000D0096 File Offset: 0x000CE296
	public void OnClearTable()
	{
		this.piecesOnConveyor.Clear();
		this.grabbedPieceTypes.Clear();
		this.grabbedPieceMaterials.Clear();
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x000D00BC File Offset: 0x000CE2BC
	public void ResetConveyorState()
	{
		for (int i = this.piecesOnConveyor.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.piecesOnConveyor[i];
			if (!(builderPiece == null))
			{
				BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
				{
					type = BuilderTable.BuilderCommandType.Recycle,
					pieceId = builderPiece.pieceId,
					localPosition = builderPiece.transform.position,
					localRotation = builderPiece.transform.rotation,
					player = NetworkSystem.Instance.LocalPlayer,
					isLeft = false,
					parentPieceId = -1
				};
				this.table.ExecutePieceRecycled(builderCommand);
			}
		}
		this.OnClearTable();
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000D0174 File Offset: 0x000CE374
	private void SpawnNextPiece()
	{
		int num;
		int num2;
		this.FindNextAffordablePieceType(out num, out num2);
		if (num == -1)
		{
			return;
		}
		this.table.RequestCreateConveyorPiece(num, num2, this.shelfID);
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x000D01A4 File Offset: 0x000CE3A4
	private void FindNextAffordablePieceType(out int pieceType, out int materialType)
	{
		if (this.grabbedPieceTypes.Count > 0)
		{
			pieceType = this.grabbedPieceTypes.Dequeue();
			materialType = this.grabbedPieceMaterials.Dequeue();
			return;
		}
		pieceType = -1;
		materialType = -1;
		if (this.piecesInSet.Count <= 0)
		{
			return;
		}
		for (int i = this.nextPieceToSpawn; i < this.piecesInSet.Count; i++)
		{
			BuilderPiece piecePrefab = this.piecesInSet[i].piecePrefab;
			if (this.table.HasEnoughResources(piecePrefab))
			{
				if (i + 1 >= this.piecesInSet.Count)
				{
					this.loopCount++;
					this.loopCount = Mathf.Max(0, this.loopCount);
				}
				this.nextPieceToSpawn = (i + 1) % this.piecesInSet.Count;
				pieceType = piecePrefab.name.GetStaticHash();
				materialType = this.GetMaterialType(this.piecesInSet[i]);
				return;
			}
		}
		this.loopCount++;
		this.loopCount = Mathf.Max(0, this.loopCount);
		for (int j = 0; j < this.nextPieceToSpawn; j++)
		{
			BuilderPiece piecePrefab2 = this.piecesInSet[j].piecePrefab;
			if (this.table.HasEnoughResources(piecePrefab2))
			{
				this.nextPieceToSpawn = (j + 1) % this.piecesInSet.Count;
				pieceType = piecePrefab2.name.GetStaticHash();
				materialType = this.GetMaterialType(this.piecesInSet[j]);
				return;
			}
		}
		this.waitForResourceChange = true;
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x000D0328 File Offset: 0x000CE528
	private int GetMaterialType(BuilderPieceSet.PieceInfo info)
	{
		if (info.piecePrefab.materialOptions != null && info.overrideSetMaterial && info.pieceMaterialTypes.Length != 0)
		{
			int num = this.loopCount % info.pieceMaterialTypes.Length;
			string text = info.pieceMaterialTypes[num];
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogErrorFormat("Empty Material Override for piece {0} in set {1}", new object[]
				{
					info.piecePrefab.name,
					this.currentDisplayGroup.displayName
				});
				return -1;
			}
			return text.GetHashCode();
		}
		else
		{
			if (string.IsNullOrEmpty(this.currentDisplayGroup.defaultMaterial))
			{
				return -1;
			}
			return this.currentDisplayGroup.defaultMaterial.GetHashCode();
		}
	}

	// Token: 0x040032EA RID: 13034
	[Header("Set Selection")]
	[SerializeField]
	private BuilderSetSelector setSelector;

	// Token: 0x040032EB RID: 13035
	public List<BuilderPieceSet.BuilderPieceCategory> _includeCategories;

	// Token: 0x040032EC RID: 13036
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x040032ED RID: 13037
	public int shelfID = -1;

	// Token: 0x040032EE RID: 13038
	[Header("Conveyor Properties")]
	[SerializeField]
	private Transform spawnTransform;

	// Token: 0x040032EF RID: 13039
	[SerializeField]
	private SplineContainer spline;

	// Token: 0x040032F0 RID: 13040
	private float conveyorMoveSpeed = 0.2f;

	// Token: 0x040032F1 RID: 13041
	private float spawnDelay = 1.5f;

	// Token: 0x040032F2 RID: 13042
	private double nextSpawnTime;

	// Token: 0x040032F3 RID: 13043
	private int nextPieceToSpawn;

	// Token: 0x040032F4 RID: 13044
	private BuilderPieceSet.BuilderDisplayGroup currentDisplayGroup;

	// Token: 0x040032F5 RID: 13045
	private int loopCount;

	// Token: 0x040032F6 RID: 13046
	private List<BuilderPieceSet.PieceInfo> piecesInSet = new List<BuilderPieceSet.PieceInfo>(10);

	// Token: 0x040032F7 RID: 13047
	private Queue<int> grabbedPieceTypes;

	// Token: 0x040032F8 RID: 13048
	private Queue<int> grabbedPieceMaterials;

	// Token: 0x040032F9 RID: 13049
	private List<BuilderPiece> piecesOnConveyor = new List<BuilderPiece>(10);

	// Token: 0x040032FA RID: 13050
	private Vector3 moveDirection;

	// Token: 0x040032FB RID: 13051
	private bool waitForResourceChange;

	// Token: 0x040032FC RID: 13052
	private bool initialized;

	// Token: 0x040032FD RID: 13053
	private float splineLength = 1f;

	// Token: 0x040032FE RID: 13054
	private int maxItemsOnSpline;

	// Token: 0x040032FF RID: 13055
	private global::UnityEngine.Splines.BezierCurve _evaluateCurve;

	// Token: 0x04003300 RID: 13056
	public NativeSpline nativeSpline;

	// Token: 0x04003301 RID: 13057
	private bool shouldVerifySetSelection;
}
