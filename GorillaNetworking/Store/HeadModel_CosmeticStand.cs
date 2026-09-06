using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GorillaNetworking.Store
{
	// Token: 0x02001148 RID: 4424
	public class HeadModel_CosmeticStand : HeadModel
	{
		// Token: 0x17000A9E RID: 2718
		// (get) Token: 0x06006F0E RID: 28430 RVA: 0x0023C716 File Offset: 0x0023A916
		private string mountID
		{
			get
			{
				return "Mount_" + this.bustType.ToString();
			}
		}

		// Token: 0x06006F0F RID: 28431 RVA: 0x0023C734 File Offset: 0x0023A934
		public void LoadCosmeticParts(CosmeticSO cosmeticInfo, bool forRightSide = false)
		{
			this.ClearManuallySpawnedCosmeticParts();
			this.ClearCosmetics();
			if (cosmeticInfo == null)
			{
				Debug.LogWarning("Dynamic Cosmetics - LoadWardRobeParts -  No Cosmetic Info");
				return;
			}
			Debug.Log("Dynamic Cosmetics - Loading Wardrobe Parts for " + cosmeticInfo.info.playFabID);
			this.HandleLoadCosmeticParts(cosmeticInfo, forRightSide);
		}

		// Token: 0x06006F10 RID: 28432 RVA: 0x0023C784 File Offset: 0x0023A984
		private void ResetMannequinSkin()
		{
			SkinnedMeshRenderer skinnedMeshRenderer;
			if (this.mannequin.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
			{
				List<Material> list;
				using (ListPool<Material>.Get(out list))
				{
					list.Clear();
					list.EnsureCapacity(3);
					list.Add(this.defaultMannequinBody);
					list.Add(this.defaultMannequinChest);
					list.Add(this.defaultMannequinFace);
					skinnedMeshRenderer.SetSharedMaterials(list);
					return;
				}
			}
			MeshRenderer meshRenderer;
			if (this.mannequin.TryGetComponent<MeshRenderer>(out meshRenderer))
			{
				List<Material> list2;
				using (ListPool<Material>.Get(out list2))
				{
					list2.Clear();
					list2.EnsureCapacity(3);
					list2.Add(this.defaultMannequinBody);
					list2.Add(this.defaultMannequinChest);
					list2.Add(this.defaultMannequinFace);
					meshRenderer.SetSharedMaterials(list2);
				}
			}
		}

		// Token: 0x06006F11 RID: 28433 RVA: 0x0023C874 File Offset: 0x0023AA74
		private void HandleLoadCosmeticParts(CosmeticSO cosmeticInfo, bool forRightSide)
		{
			if (cosmeticInfo.info.category == CosmeticsController.CosmeticCategory.Set && !cosmeticInfo.info.hasStoreParts)
			{
				foreach (CosmeticSO cosmeticSO in cosmeticInfo.info.setCosmetics)
				{
					this.HandleLoadCosmeticParts(cosmeticSO, forRightSide);
				}
				return;
			}
			CosmeticPart[] array;
			if (cosmeticInfo.info.storeParts.Length != 0)
			{
				array = cosmeticInfo.info.storeParts;
			}
			else
			{
				if (cosmeticInfo.info.category == CosmeticsController.CosmeticCategory.Fur)
				{
					CosmeticPart[] array2 = cosmeticInfo.info.functionalParts;
					int i = 0;
					if (i < array2.Length)
					{
						CosmeticPart cosmeticPart = array2[i];
						GameObject gameObject = this.LoadAndInstantiatePrefab(cosmeticPart.prefabAssetRef, base.transform);
						gameObject.GetComponent<GorillaSkinToggle>().ApplyToMannequin(this.mannequin, false);
						Object.DestroyImmediate(gameObject);
						return;
					}
				}
				array = cosmeticInfo.info.wardrobeParts;
			}
			foreach (CosmeticPart cosmeticPart2 in array)
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart2.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = cosmeticInfo.info.playFabID,
							prefabAssetRef = cosmeticPart2.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							xform = null
						};
						GameObject gameObject2 = this.LoadAndInstantiatePrefab(cosmeticPart2.prefabAssetRef, base.transform);
						cosmeticPartLoadInfo.xform = gameObject2.transform;
						this._manuallySpawnedCosmeticParts.Add(gameObject2);
						gameObject2.SetActive(true);
						switch (this.bustType)
						{
						case HeadModel_CosmeticStand.BustType.Disabled:
							this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaHead:
						case HeadModel_CosmeticStand.BustType.GorillaTorso:
						case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
						case HeadModel_CosmeticStand.BustType.GuitarStand:
						case HeadModel_CosmeticStand.BustType.JewelryBox:
						case HeadModel_CosmeticStand.BustType.Table:
						case HeadModel_CosmeticStand.BustType.PinDisplay:
						case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
							this.PositionWardRobeItems(gameObject2, cosmeticPartLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaMannequin:
							this._manuallySpawnedCosmeticParts.Remove(gameObject2);
							Object.DestroyImmediate(gameObject2);
							break;
						default:
							this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
							break;
						}
					}
				}
			}
		}

		// Token: 0x06006F12 RID: 28434 RVA: 0x0023CAC4 File Offset: 0x0023ACC4
		public void LoadCosmeticPartsV2(string playFabId, bool forRightSide = false)
		{
			this.ClearManuallySpawnedCosmeticParts();
			this.ClearCosmetics();
			CosmeticInfoV2 cosmeticInfoV;
			if (!CosmeticsController.instance.TryGetCosmeticInfoV2(playFabId, out cosmeticInfoV))
			{
				if (!(playFabId == "null") && !(playFabId == "NOTHING") && !(playFabId == "Slingshot"))
				{
					Debug.LogError("HeadModel.playFabId: Cosmetic id \"" + playFabId + "\" not found in `CosmeticsController`.", this);
				}
				return;
			}
			this.HandleLoadingAllPieces(playFabId, forRightSide, cosmeticInfoV);
		}

		// Token: 0x06006F13 RID: 28435 RVA: 0x0023CB38 File Offset: 0x0023AD38
		private void HandleLoadingAllPieces(string playFabId, bool forRightSide, CosmeticInfoV2 cosmeticInfo)
		{
			CosmeticPart[] array;
			if (cosmeticInfo.storeParts.Length != 0)
			{
				array = cosmeticInfo.storeParts;
			}
			else
			{
				if (cosmeticInfo.category == CosmeticsController.CosmeticCategory.Fur)
				{
					this.HandleLoadingFur(playFabId, forRightSide, cosmeticInfo);
					return;
				}
				if (cosmeticInfo.category == CosmeticsController.CosmeticCategory.Set)
				{
					foreach (CosmeticSO cosmeticSO in cosmeticInfo.setCosmetics)
					{
						this.HandleLoadingAllPieces(playFabId, forRightSide, cosmeticSO.info);
					}
					return;
				}
				array = cosmeticInfo.wardrobeParts;
			}
			foreach (CosmeticPart cosmeticPart in array)
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = playFabId,
							prefabAssetRef = cosmeticPart.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							loadOp = cosmeticPart.prefabAssetRef.InstantiateAsync(base.transform, false),
							xform = null
						};
						cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadCosmeticPartsV2;
						this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
						this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
					}
				}
			}
		}

		// Token: 0x06006F14 RID: 28436 RVA: 0x0023CCD0 File Offset: 0x0023AED0
		private void _HandleLoadCosmeticPartsV2(AsyncOperationHandle<GameObject> loadOp)
		{
			int num;
			if (!this._loadOp_to_partInfoIndex.TryGetValue(loadOp, out num))
			{
				if (loadOp.Status == AsyncOperationStatus.Succeeded && loadOp.Result)
				{
					Object.Destroy(loadOp.Result);
				}
				return;
			}
			HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[num];
			if (loadOp.Status == AsyncOperationStatus.Failed)
			{
				Debug.Log("HeadModel: Failed to load a part for cosmetic \"" + cosmeticPartLoadInfo.playFabId + "\"! Waiting for 10 seconds before trying again.", this);
				GTDelayedExec.Add(this, 10f, num);
				return;
			}
			cosmeticPartLoadInfo.xform = loadOp.Result.transform;
			this._manuallySpawnedCosmeticParts.Add(cosmeticPartLoadInfo.xform.gameObject);
			switch (this.bustType)
			{
			case HeadModel_CosmeticStand.BustType.Disabled:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaHead:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorso:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaMannequin:
				this._manuallySpawnedCosmeticParts.Remove(cosmeticPartLoadInfo.xform.gameObject);
				Object.DestroyImmediate(cosmeticPartLoadInfo.xform.gameObject);
				break;
			case HeadModel_CosmeticStand.BustType.GuitarStand:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.JewelryBox:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.Table:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.PinDisplay:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			default:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			}
			cosmeticPartLoadInfo.xform.gameObject.SetActive(true);
		}

		// Token: 0x06006F15 RID: 28437 RVA: 0x0023CE48 File Offset: 0x0023B048
		private void HandleLoadingFur(string playFabId, bool forRightSide, CosmeticInfoV2 cosmeticInfo)
		{
			foreach (CosmeticPart cosmeticPart in cosmeticInfo.functionalParts)
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = playFabId,
							prefabAssetRef = cosmeticPart.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							loadOp = cosmeticPart.prefabAssetRef.InstantiateAsync(base.transform, false),
							xform = null
						};
						cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadCosmeticPartsV2Fur;
						this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
						this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
					}
				}
			}
		}

		// Token: 0x06006F16 RID: 28438 RVA: 0x0023CF68 File Offset: 0x0023B168
		private void _HandleLoadCosmeticPartsV2Fur(AsyncOperationHandle<GameObject> loadOp)
		{
			int num;
			if (!this._loadOp_to_partInfoIndex.TryGetValue(loadOp, out num))
			{
				if (loadOp.Status == AsyncOperationStatus.Succeeded && loadOp.Result)
				{
					Object.Destroy(loadOp.Result);
				}
				return;
			}
			HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[num];
			if (loadOp.Status == AsyncOperationStatus.Failed)
			{
				Debug.Log("HeadModel: Failed to load a part for cosmetic \"" + cosmeticPartLoadInfo.playFabId + "\"! Waiting for 10 seconds before trying again.", this);
				GTDelayedExec.Add(this, 10f, num);
				return;
			}
			cosmeticPartLoadInfo.xform = loadOp.Result.transform;
			cosmeticPartLoadInfo.xform.GetComponent<GorillaSkinToggle>().ApplyToMannequin(this.mannequin, false);
			Object.DestroyImmediate(cosmeticPartLoadInfo.xform.gameObject);
		}

		// Token: 0x06006F17 RID: 28439 RVA: 0x0023D028 File Offset: 0x0023B228
		public void SetStandType(HeadModel_CosmeticStand.BustType newBustType)
		{
			this.bustType = newBustType;
		}

		// Token: 0x06006F18 RID: 28440 RVA: 0x0023D034 File Offset: 0x0023B234
		private void PositionWardRobeItems(GameObject instantiateEdObject, HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Transform transform = instantiateEdObject.transform.FindChildRecursive(this.mountID);
			if (transform != null)
			{
				Debug.Log("Dynamic Cosmetics - Mount Found: " + this.mountID);
				instantiateEdObject.transform.position = base.transform.position;
				instantiateEdObject.transform.rotation = base.transform.rotation;
				instantiateEdObject.transform.localPosition = transform.localPosition;
				instantiateEdObject.transform.localRotation = transform.localRotation;
				return;
			}
			HeadModel_CosmeticStand.BustType bustType = this.bustType;
			if (bustType - HeadModel_CosmeticStand.BustType.GuitarStand <= 2 || bustType == HeadModel_CosmeticStand.BustType.TagEffectDisplay)
			{
				instantiateEdObject.transform.position = base.transform.position;
				instantiateEdObject.transform.rotation = base.transform.rotation;
				return;
			}
			this.PositionWithWardRobeOffsets(partLoadInfo);
		}

		// Token: 0x06006F19 RID: 28441 RVA: 0x0023D108 File Offset: 0x0023B308
		private void PositionWardRobeItems(HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Transform transform = partLoadInfo.xform.FindChildRecursive(this.mountID);
			if (transform != null)
			{
				Debug.Log("Dynamic Cosmetics - Mount Found: " + this.mountID);
				partLoadInfo.xform.position = base.transform.position;
				partLoadInfo.xform.rotation = base.transform.rotation;
				partLoadInfo.xform.localPosition = transform.localPosition;
				partLoadInfo.xform.localRotation = transform.localRotation;
				return;
			}
			HeadModel_CosmeticStand.BustType bustType = this.bustType;
			if (bustType - HeadModel_CosmeticStand.BustType.GuitarStand <= 2 || bustType == HeadModel_CosmeticStand.BustType.TagEffectDisplay)
			{
				partLoadInfo.xform.position = base.transform.position;
				partLoadInfo.xform.rotation = base.transform.rotation;
				return;
			}
			this.PositionWithWardRobeOffsets(partLoadInfo);
		}

		// Token: 0x06006F1A RID: 28442 RVA: 0x0023D1DC File Offset: 0x0023B3DC
		private void PositionWithWardRobeOffsets(HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Debug.Log("Dynamic Cosmetics - Mount Not Found: " + this.mountID);
			partLoadInfo.xform.localPosition = partLoadInfo.attachInfo.offset.pos;
			partLoadInfo.xform.localRotation = partLoadInfo.attachInfo.offset.rot;
			partLoadInfo.xform.localScale = partLoadInfo.attachInfo.offset.scale;
		}

		// Token: 0x06006F1B RID: 28443 RVA: 0x0023D250 File Offset: 0x0023B450
		public void ClearManuallySpawnedCosmeticParts()
		{
			foreach (GameObject gameObject in this._manuallySpawnedCosmeticParts)
			{
				Object.DestroyImmediate(gameObject);
			}
			this._manuallySpawnedCosmeticParts.Clear();
		}

		// Token: 0x06006F1C RID: 28444 RVA: 0x0023D2AC File Offset: 0x0023B4AC
		public void ClearCosmetics()
		{
			this.ResetMannequinSkin();
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x06006F1D RID: 28445 RVA: 0x00036275 File Offset: 0x00034475
		private GameObject LoadAndInstantiatePrefab(GTAssetRef<GameObject> prefabAssetRef, Transform parent)
		{
			return null;
		}

		// Token: 0x06006F1E RID: 28446 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void UpdateCosmeticsMountPositions(CosmeticSO findCosmeticInAllCosmeticsArraySO)
		{
		}

		// Token: 0x04007F18 RID: 32536
		[ReadOnly]
		public HeadModel_CosmeticStand.BustType bustType = HeadModel_CosmeticStand.BustType.JewelryBox;

		// Token: 0x04007F19 RID: 32537
		[SerializeField]
		[ReadOnly]
		private List<GameObject> _manuallySpawnedCosmeticParts = new List<GameObject>();

		// Token: 0x04007F1A RID: 32538
		public GameObject mannequin;

		// Token: 0x04007F1B RID: 32539
		public Material defaultMannequinFace;

		// Token: 0x04007F1C RID: 32540
		public Material defaultMannequinChest;

		// Token: 0x04007F1D RID: 32541
		public Material defaultMannequinBody;

		// Token: 0x04007F1E RID: 32542
		[DebugReadout]
		private readonly Dictionary<AsyncOperationHandle, int> _loadOp_to_partInfoIndex = new Dictionary<AsyncOperationHandle, int>(1);

		// Token: 0x02001149 RID: 4425
		public enum BustType
		{
			// Token: 0x04007F20 RID: 32544
			Disabled,
			// Token: 0x04007F21 RID: 32545
			GorillaHead,
			// Token: 0x04007F22 RID: 32546
			GorillaTorso,
			// Token: 0x04007F23 RID: 32547
			GorillaTorsoPost,
			// Token: 0x04007F24 RID: 32548
			GorillaMannequin,
			// Token: 0x04007F25 RID: 32549
			GuitarStand,
			// Token: 0x04007F26 RID: 32550
			JewelryBox,
			// Token: 0x04007F27 RID: 32551
			Table,
			// Token: 0x04007F28 RID: 32552
			PinDisplay,
			// Token: 0x04007F29 RID: 32553
			TagEffectDisplay
		}
	}
}
