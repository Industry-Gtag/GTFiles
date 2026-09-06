using System;
using System.Collections.Generic;
using Cysharp.Text;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x0200051C RID: 1308
public class HeadModel : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x060020A0 RID: 8352 RVA: 0x000AF154 File Offset: 0x000AD354
	protected void Awake()
	{
		this.RefreshRenderer();
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x000AF15C File Offset: 0x000AD35C
	protected void RefreshRenderer()
	{
		this._mannequinRenderer = base.GetComponentInChildren<Renderer>(true);
	}

	// Token: 0x060020A2 RID: 8354 RVA: 0x000AF16B File Offset: 0x000AD36B
	public void SetCosmeticActive(string playFabId, bool forRightSide = false)
	{
		this._ClearCurrent();
		this._AddPreviewCosmetic(playFabId, forRightSide);
	}

	// Token: 0x060020A3 RID: 8355 RVA: 0x000AF17C File Offset: 0x000AD37C
	public void SetCosmeticActiveArray(string[] playFabIds, bool[] forRightSideArray)
	{
		this._ClearCurrent();
		for (int i = 0; i < playFabIds.Length; i++)
		{
			this._AddPreviewCosmetic(playFabIds[i], forRightSideArray[i]);
		}
	}

	// Token: 0x060020A4 RID: 8356 RVA: 0x000AF1AC File Offset: 0x000AD3AC
	private void _AddPreviewCosmetic(string playFabId, bool forRightSide)
	{
		CosmeticInfoV2 cosmeticInfoV;
		if (!CosmeticsController.instance.TryGetCosmeticInfoV2(playFabId, out cosmeticInfoV))
		{
			if (!(playFabId == "null") && !(playFabId == "NOTHING") && !(playFabId == "Slingshot"))
			{
				Debug.LogError(ZString.Concat<string, string, string>("HeadModel._AddPreviewCosmetic: Cosmetic id \"", playFabId, "\" not found in `CosmeticsController`."), this);
			}
			return;
		}
		if (cosmeticInfoV.hideWardrobeMannequin)
		{
			if (this._mannequinRenderer.IsNull())
			{
				this.RefreshRenderer();
			}
			if (this._mannequinRenderer.IsNotNull())
			{
				this._mannequinRenderer.enabled = false;
			}
		}
		foreach (CosmeticPart cosmeticPart in cosmeticInfoV.wardrobeParts)
		{
			if (!cosmeticPart.prefabAssetRef.RuntimeKeyIsValid())
			{
				GTDev.LogError<string>("Cosmetic " + cosmeticInfoV.displayName + " has missing object reference in wardrobe parts, skipping load", null);
			}
			else
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
						cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadOpOnCompleted;
						this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
						this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
					}
				}
			}
		}
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x000AF380 File Offset: 0x000AD580
	private void _HandleLoadOpOnCompleted(AsyncOperationHandle<GameObject> loadOp)
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
		cosmeticPartLoadInfo.xform.localPosition = cosmeticPartLoadInfo.attachInfo.offset.pos;
		cosmeticPartLoadInfo.xform.localRotation = cosmeticPartLoadInfo.attachInfo.offset.rot;
		cosmeticPartLoadInfo.xform.localScale = cosmeticPartLoadInfo.attachInfo.offset.scale;
		cosmeticPartLoadInfo.xform.gameObject.SetActive(true);
	}

	// Token: 0x060020A6 RID: 8358 RVA: 0x000AF47C File Offset: 0x000AD67C
	void IDelayedExecListener.OnDelayedAction(int partLoadInfosIndex)
	{
		if (partLoadInfosIndex < 0 || partLoadInfosIndex >= this._currentPartLoadInfos.Count)
		{
			return;
		}
		HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[partLoadInfosIndex];
		if (cosmeticPartLoadInfo.loadOp.Status != AsyncOperationStatus.Failed)
		{
			return;
		}
		cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadOpOnCompleted;
		cosmeticPartLoadInfo.loadOp = cosmeticPartLoadInfo.prefabAssetRef.InstantiateAsync(base.transform, false);
		this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = partLoadInfosIndex;
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x000AF504 File Offset: 0x000AD704
	protected void _ClearCurrent()
	{
		for (int i = 0; i < this._currentPartLoadInfos.Count; i++)
		{
			Object.Destroy(this._currentPartLoadInfos[i].loadOp.Result);
		}
		this._EnsureCapacityAndClear<AsyncOperationHandle, int>(this._loadOp_to_partInfoIndex);
		this._EnsureCapacityAndClear<HeadModel._CosmeticPartLoadInfo>(this._currentPartLoadInfos);
		if (this._mannequinRenderer.IsNull())
		{
			this.RefreshRenderer();
		}
		this._mannequinRenderer.enabled = true;
	}

	// Token: 0x060020A8 RID: 8360 RVA: 0x000AF57C File Offset: 0x000AD77C
	private void _EnsureCapacityAndClear<T>(List<T> list)
	{
		if (list.Count > list.Capacity)
		{
			list.Capacity = list.Count;
		}
		list.Clear();
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x000AF59E File Offset: 0x000AD79E
	private void _EnsureCapacityAndClear<T1, T2>(Dictionary<T1, T2> dict)
	{
		dict.EnsureCapacity(dict.Count);
		dict.Clear();
	}

	// Token: 0x04002B79 RID: 11129
	[DebugReadout]
	protected readonly List<HeadModel._CosmeticPartLoadInfo> _currentPartLoadInfos = new List<HeadModel._CosmeticPartLoadInfo>(1);

	// Token: 0x04002B7A RID: 11130
	[DebugReadout]
	private readonly Dictionary<AsyncOperationHandle, int> _loadOp_to_partInfoIndex = new Dictionary<AsyncOperationHandle, int>(1);

	// Token: 0x04002B7B RID: 11131
	private Renderer _mannequinRenderer;

	// Token: 0x04002B7C RID: 11132
	public GameObject[] cosmetics;

	// Token: 0x0200051D RID: 1309
	protected struct _CosmeticPartLoadInfo
	{
		// Token: 0x04002B7D RID: 11133
		public string playFabId;

		// Token: 0x04002B7E RID: 11134
		public GTAssetRef<GameObject> prefabAssetRef;

		// Token: 0x04002B7F RID: 11135
		public CosmeticAttachInfo attachInfo;

		// Token: 0x04002B80 RID: 11136
		public AsyncOperationHandle<GameObject> loadOp;

		// Token: 0x04002B81 RID: 11137
		public Transform xform;
	}
}
