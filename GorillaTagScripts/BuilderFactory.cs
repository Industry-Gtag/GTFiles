using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F57 RID: 3927
	public class BuilderFactory : MonoBehaviour
	{
		// Token: 0x06006084 RID: 24708 RVA: 0x001E9B11 File Offset: 0x001E7D11
		private void Awake()
		{
			this.InitIfNeeded();
		}

		// Token: 0x06006085 RID: 24709 RVA: 0x001E9B1C File Offset: 0x001E7D1C
		public void InitIfNeeded()
		{
			if (this.initialized)
			{
				return;
			}
			this.buildItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnBuildItem));
			this.currPieceTypeIndex = 0;
			this.prevItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnPrevItem));
			this.nextItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnNextItem));
			this.currPieceMaterialIndex = 0;
			this.prevMaterialButton.Setup(new Action<BuilderOptionButton, bool>(this.OnPrevMaterial));
			this.nextMaterialButton.Setup(new Action<BuilderOptionButton, bool>(this.OnNextMaterial));
			this.pieceTypeToIndex = new Dictionary<int, int>(256);
			this.initialized = true;
			if (this.resourceCostUIs != null)
			{
				for (int i = 0; i < this.resourceCostUIs.Count; i++)
				{
					if (this.resourceCostUIs[i] != null)
					{
						this.resourceCostUIs[i].gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06006086 RID: 24710 RVA: 0x001E9C14 File Offset: 0x001E7E14
		public void Setup(BuilderTable tableOwner)
		{
			this.table = tableOwner;
			this.InitIfNeeded();
			List<BuilderPiece> list = this.pieceList;
			this.pieceTypes = new List<int>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				string name = list[i].name;
				int staticHash = name.GetStaticHash();
				int num;
				if (this.pieceTypeToIndex.TryAdd(staticHash, i))
				{
					this.pieceTypes.Add(staticHash);
				}
				else if (this.pieceTypeToIndex.TryGetValue(staticHash, out num))
				{
					string text = "BuilderFactory: ERROR!! " + string.Format("Could not add pieceType \"{0}\" with hash {1} ", name, staticHash) + "to 'pieceTypeToIndex' Dictionary because because it was already added!";
					if (num < 0 || num >= list.Count)
					{
						text += " Also the index to the conflicting piece is out of range of the pieceList!";
					}
					else
					{
						BuilderPiece builderPiece = list[num];
						if (builderPiece != null)
						{
							if (name == builderPiece.name)
							{
								text += "The conflicting piece has the same name (as expected).";
							}
							else
							{
								text = text + "Also the conflicting pieceType has the same hash but different name \"" + builderPiece.name + "\"!";
							}
						}
						else
						{
							text += "And (should never happen) the piece at that slot is null!";
						}
					}
					Debug.LogError(text, this);
				}
			}
			int num2 = this.pieceTypes.Count;
			foreach (BuilderPieceSet builderPieceSet in BuilderSetManager.instance.GetAllPieceSets())
			{
				foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in builderPieceSet.subsets)
				{
					foreach (BuilderPieceSet.PieceInfo pieceInfo in builderPieceSubset.pieceInfos)
					{
						int staticHash2 = pieceInfo.piecePrefab.name.GetStaticHash();
						if (!this.pieceTypeToIndex.ContainsKey(staticHash2))
						{
							this.pieceList.Add(pieceInfo.piecePrefab);
							this.pieceTypes.Add(staticHash2);
							this.pieceTypeToIndex.Add(staticHash2, num2);
							num2++;
						}
					}
				}
			}
		}

		// Token: 0x06006087 RID: 24711 RVA: 0x001E9E78 File Offset: 0x001E8078
		public void Show()
		{
			this.RefreshUI();
		}

		// Token: 0x06006088 RID: 24712 RVA: 0x001E9E80 File Offset: 0x001E8080
		public BuilderPiece GetPiecePrefab(int pieceType)
		{
			int num;
			if (this.pieceTypeToIndex.TryGetValue(pieceType, out num))
			{
				return this.pieceList[num];
			}
			Debug.LogErrorFormat("No Prefab found for type {0}", new object[] { pieceType });
			return null;
		}

		// Token: 0x06006089 RID: 24713 RVA: 0x001E9EC4 File Offset: 0x001E80C4
		public void OnBuildItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > this.currPieceTypeIndex)
			{
				int selectedMaterialType = this.GetSelectedMaterialType();
				this.table.RequestCreatePiece(this.pieceTypes[this.currPieceTypeIndex], this.spawnLocation.position, this.spawnLocation.rotation, selectedMaterialType);
				if (this.audioSource != null && this.buildPieceSound != null)
				{
					this.audioSource.GTPlayOneShot(this.buildPieceSound, 1f);
				}
			}
		}

		// Token: 0x0600608A RID: 24714 RVA: 0x001E9F58 File Offset: 0x001E8158
		public void OnPrevItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				for (int i = 0; i < this.pieceTypes.Count; i++)
				{
					this.currPieceTypeIndex = (this.currPieceTypeIndex - 1 + this.pieceTypes.Count) % this.pieceTypes.Count;
					if (this.CanBuildPieceType(this.pieceTypes[this.currPieceTypeIndex]))
					{
						break;
					}
				}
				this.RefreshUI();
			}
		}

		// Token: 0x0600608B RID: 24715 RVA: 0x001E9FD8 File Offset: 0x001E81D8
		public void OnNextItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				for (int i = 0; i < this.pieceTypes.Count; i++)
				{
					this.currPieceTypeIndex = (this.currPieceTypeIndex + 1 + this.pieceTypes.Count) % this.pieceTypes.Count;
					if (this.CanBuildPieceType(this.pieceTypes[this.currPieceTypeIndex]))
					{
						break;
					}
				}
				this.RefreshUI();
			}
		}

		// Token: 0x0600608C RID: 24716 RVA: 0x001EA058 File Offset: 0x001E8258
		public void OnPrevMaterial(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
				if (piecePrefab != null)
				{
					BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
					if (materialOptions != null && materialOptions.options.Count > 0)
					{
						for (int i = 0; i < materialOptions.options.Count; i++)
						{
							this.currPieceMaterialIndex = (this.currPieceMaterialIndex - 1 + materialOptions.options.Count) % materialOptions.options.Count;
							if (this.CanUseMaterialType(materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode()))
							{
								break;
							}
						}
					}
					this.RefreshUI();
				}
			}
		}

		// Token: 0x0600608D RID: 24717 RVA: 0x001EA128 File Offset: 0x001E8328
		public void OnNextMaterial(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
				if (piecePrefab != null)
				{
					BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
					if (materialOptions != null && materialOptions.options.Count > 0)
					{
						for (int i = 0; i < materialOptions.options.Count; i++)
						{
							this.currPieceMaterialIndex = (this.currPieceMaterialIndex + 1 + materialOptions.options.Count) % materialOptions.options.Count;
							if (this.CanUseMaterialType(materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode()))
							{
								break;
							}
						}
					}
					this.RefreshUI();
				}
			}
		}

		// Token: 0x0600608E RID: 24718 RVA: 0x001EA1F8 File Offset: 0x001E83F8
		private int GetSelectedMaterialType()
		{
			int num = -1;
			BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
			if (piecePrefab != null)
			{
				BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
				if (materialOptions != null && materialOptions.options != null && this.currPieceMaterialIndex >= 0 && this.currPieceMaterialIndex < materialOptions.options.Count)
				{
					num = materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode();
				}
			}
			return num;
		}

		// Token: 0x0600608F RID: 24719 RVA: 0x001EA27C File Offset: 0x001E847C
		private string GetSelectedMaterialName()
		{
			string text = "DEFAULT";
			BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
			if (piecePrefab != null)
			{
				BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
				if (materialOptions != null && materialOptions.options != null && this.currPieceMaterialIndex >= 0 && this.currPieceMaterialIndex < materialOptions.options.Count)
				{
					text = materialOptions.options[this.currPieceMaterialIndex].materialId;
				}
			}
			return text;
		}

		// Token: 0x06006090 RID: 24720 RVA: 0x001EA2FC File Offset: 0x001E84FC
		public bool CanBuildPieceType(int pieceType)
		{
			BuilderPiece piecePrefab = this.GetPiecePrefab(pieceType);
			return !(piecePrefab == null) && !piecePrefab.isBuiltIntoTable;
		}

		// Token: 0x06006091 RID: 24721 RVA: 0x00023F0C File Offset: 0x0002210C
		public bool CanUseMaterialType(int materalType)
		{
			return true;
		}

		// Token: 0x06006092 RID: 24722 RVA: 0x001EA328 File Offset: 0x001E8528
		public void RefreshUI()
		{
			if (this.pieceList != null && this.pieceList.Count > this.currPieceTypeIndex)
			{
				this.itemLabel.SetText(this.pieceList[this.currPieceTypeIndex].displayName);
			}
			else
			{
				this.itemLabel.SetText("No Items");
			}
			if (this.previewPiece != null)
			{
				this.table.builderPool.DestroyPiece(this.previewPiece);
				this.previewPiece = null;
			}
			if (this.currPieceTypeIndex < 0 || this.currPieceTypeIndex >= this.pieceTypes.Count)
			{
				return;
			}
			int num = this.pieceTypes[this.currPieceTypeIndex];
			this.previewPiece = this.table.builderPool.CreatePiece(num, false);
			this.previewPiece.SetTable(this.table);
			this.previewPiece.pieceType = num;
			string selectedMaterialName = this.GetSelectedMaterialName();
			this.materialLabel.SetText(selectedMaterialName);
			this.previewPiece.SetScale(this.table.pieceScale * 0.75f);
			this.previewPiece.SetupPiece(this.table.gridSize);
			int selectedMaterialType = this.GetSelectedMaterialType();
			this.previewPiece.SetMaterial(selectedMaterialType, true);
			this.previewPiece.transform.SetPositionAndRotation(this.previewMarker.position, this.previewMarker.rotation);
			this.previewPiece.SetState(BuilderPiece.State.Displayed, false);
			this.previewPiece.enabled = false;
			this.RefreshCostUI();
		}

		// Token: 0x06006093 RID: 24723 RVA: 0x001EA4B0 File Offset: 0x001E86B0
		private void RefreshCostUI()
		{
			List<BuilderResourceQuantity> list = null;
			if (this.previewPiece != null)
			{
				list = this.previewPiece.cost.quantities;
			}
			for (int i = 0; i < this.resourceCostUIs.Count; i++)
			{
				if (!(this.resourceCostUIs[i] == null))
				{
					bool flag = list != null && i < list.Count;
					this.resourceCostUIs[i].gameObject.SetActive(flag);
					if (flag)
					{
						this.resourceCostUIs[i].SetResourceCost(list[i], this.table);
					}
				}
			}
		}

		// Token: 0x06006094 RID: 24724 RVA: 0x001EA550 File Offset: 0x001E8750
		public void OnAvailableResourcesChange()
		{
			this.RefreshCostUI();
		}

		// Token: 0x06006095 RID: 24725 RVA: 0x001EA558 File Offset: 0x001E8758
		public void CreateRandomPiece()
		{
			Debug.LogError("Create Random Piece No longer implemented");
		}

		// Token: 0x04006F1F RID: 28447
		public Transform spawnLocation;

		// Token: 0x04006F20 RID: 28448
		private List<int> pieceTypes;

		// Token: 0x04006F21 RID: 28449
		public List<GameObject> itemList;

		// Token: 0x04006F22 RID: 28450
		[HideInInspector]
		public List<BuilderPiece> pieceList;

		// Token: 0x04006F23 RID: 28451
		public BuilderOptionButton buildItemButton;

		// Token: 0x04006F24 RID: 28452
		public TextMeshPro itemLabel;

		// Token: 0x04006F25 RID: 28453
		public BuilderOptionButton prevItemButton;

		// Token: 0x04006F26 RID: 28454
		public BuilderOptionButton nextItemButton;

		// Token: 0x04006F27 RID: 28455
		public TextMeshPro materialLabel;

		// Token: 0x04006F28 RID: 28456
		public BuilderOptionButton prevMaterialButton;

		// Token: 0x04006F29 RID: 28457
		public BuilderOptionButton nextMaterialButton;

		// Token: 0x04006F2A RID: 28458
		public AudioSource audioSource;

		// Token: 0x04006F2B RID: 28459
		public AudioClip buildPieceSound;

		// Token: 0x04006F2C RID: 28460
		public Transform previewMarker;

		// Token: 0x04006F2D RID: 28461
		public List<BuilderUIResource> resourceCostUIs;

		// Token: 0x04006F2E RID: 28462
		private BuilderPiece previewPiece;

		// Token: 0x04006F2F RID: 28463
		private int currPieceTypeIndex;

		// Token: 0x04006F30 RID: 28464
		private int currPieceMaterialIndex;

		// Token: 0x04006F31 RID: 28465
		private Dictionary<int, int> pieceTypeToIndex;

		// Token: 0x04006F32 RID: 28466
		private BuilderTable table;

		// Token: 0x04006F33 RID: 28467
		private bool initialized;
	}
}
