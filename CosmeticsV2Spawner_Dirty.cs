using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Text;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaNetworking.Store;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x02000308 RID: 776
public class CosmeticsV2Spawner_Dirty : IDelayedExecListener
{
	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x060013BF RID: 5055 RVA: 0x0006A5A3 File Offset: 0x000687A3
	// (set) Token: 0x060013C0 RID: 5056 RVA: 0x0006A5AA File Offset: 0x000687AA
	public static bool isPrepared { get; private set; }

	// Token: 0x060013C1 RID: 5057 RVA: 0x0006A5B2 File Offset: 0x000687B2
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		if (contextId >= 0 && contextId < 1000000)
		{
			CosmeticsV2Spawner_Dirty._RetryDownload(contextId);
			return;
		}
		if (contextId == -Mathf.Abs("_Step5_InitializeVRRigsAndCosmeticsControllerFinalize".GetHashCode()))
		{
			CosmeticsV2Spawner_Dirty._Step5_InitializeVRRigsAndCosmeticsControllerFinalize();
		}
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x0006A5E0 File Offset: 0x000687E0
	public static void PrepareLoadOpInfos()
	{
		if (CosmeticsV2Spawner_Dirty.isPrepared)
		{
			return;
		}
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (CosmeticsV2Spawner_Dirty._instance == null)
		{
			CosmeticsV2Spawner_Dirty._instance = new CosmeticsV2Spawner_Dirty();
		}
		CosmeticsV2Spawner_Dirty.k_stopwatch.Restart();
		CosmeticsV2Spawner_Dirty.g_gorillaPlayer = Object.FindAnyObjectByType<GTPlayer>();
		foreach (SnowballMaker snowballMaker in CosmeticsV2Spawner_Dirty.g_gorillaPlayer.GetComponentsInChildren<SnowballMaker>(true))
		{
			if (snowballMaker.isLeftHand)
			{
				CosmeticsV2Spawner_Dirty._gSnowballMakerLeft = snowballMaker;
			}
			else
			{
				CosmeticsV2Spawner_Dirty._gSnowballMakerRight = snowballMaker;
			}
		}
		if (!CosmeticsController.hasInstance)
		{
			Debug.LogError("(Should never happen) Cannot instantiate prefabs before cosmetics controller instance is available.");
			return;
		}
		if (!CosmeticsController.instance.v2_allCosmeticsInfoAssetRef.IsValid())
		{
			Debug.LogError("(Should never happen) Cannot load prefabs before v2_allCosmeticsInfoAssetRef is loaded.");
			return;
		}
		AllCosmeticsArraySO allCosmeticsArraySO = CosmeticsController.instance.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
		if (allCosmeticsArraySO == null)
		{
			Debug.LogError("(Should never happen) v2_allCosmeticsInfoAssetRef is valid but null.");
			return;
		}
		Transform[] array;
		string text;
		if (!GTHardCodedBones.TryGetBoneXforms(VRRig.LocalRig, out array, out text))
		{
			Debug.LogError("CosmeticsV2Spawner_Dirty: Error getting bone Transforms from local VRRig: " + text, VRRig.LocalRig);
			return;
		}
		CosmeticsV2Spawner_Dirty._gVRRigDatas.Add(new CosmeticsV2Spawner_Dirty.VRRigData(VRRig.LocalRig, array));
		CosmeticsV2Spawner_Dirty._gVRRigDatasIndexByRig[VRRig.LocalRig] = 0;
		int num = 0;
		foreach (VRRig vrrig in VRRigCache.Instance.GetAllRigs())
		{
			Transform[] array2;
			if (!GTHardCodedBones.TryGetBoneXforms(vrrig, out array2, out text))
			{
				Debug.LogError("CosmeticsV2Spawner_Dirty: Error getting bone Transforms from cached VRRig: " + text, VRRig.LocalRig);
				return;
			}
			CosmeticsV2Spawner_Dirty._gVRRigDatasIndexByRig[vrrig] = CosmeticsV2Spawner_Dirty._gVRRigDatas.Count;
			CosmeticsV2Spawner_Dirty._gVRRigDatas.Add(new CosmeticsV2Spawner_Dirty.VRRigData(vrrig, array2));
		}
		CosmeticsV2Spawner_Dirty._gDeactivatedSpawnParent = GlobalDeactivatedSpawnRoot.GetOrCreate();
		GTDelayedExec.Add(CosmeticsV2Spawner_Dirty._instance, 2f, -100);
		CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft = new Dictionary<int, string>();
		CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight = new Dictionary<int, string>();
		CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringLeft = new Dictionary<int, string>();
		CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringRight = new Dictionary<int, string>();
		CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts = new Dictionary<string, List<CosmeticsV2Spawner_Dirty.LoadOpInfo>>[20];
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (GTDirectAssetRef<CosmeticSO> gtdirectAssetRef in allCosmeticsArraySO.sturdyAssetRefs)
		{
			CosmeticInfoV2 info = gtdirectAssetRef.obj.info;
			if (info.hasHoldableParts)
			{
				for (int k = 0; k < CosmeticsV2Spawner_Dirty._gVRRigDatas.Count; k++)
				{
					for (int l = 0; l < info.holdableParts.Length; l++)
					{
						CosmeticPart cosmeticPart = info.holdableParts[l];
						if (!cosmeticPart.prefabAssetRef.RuntimeKeyIsValid())
						{
							if (k == 0)
							{
								GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in wearable parts, skipping load", null);
							}
						}
						else
						{
							CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart, l, info, k, ref num2);
						}
					}
				}
			}
			if (info.hasFunctionalParts)
			{
				for (int m = 0; m < CosmeticsV2Spawner_Dirty._gVRRigDatas.Count; m++)
				{
					for (int n = 0; n < info.functionalParts.Length; n++)
					{
						CosmeticPart cosmeticPart2 = info.functionalParts[n];
						if (!cosmeticPart2.prefabAssetRef.RuntimeKeyIsValid())
						{
							if (m == 0)
							{
								GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in functional parts, skipping load", null);
							}
						}
						else
						{
							CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart2, n, info, m, ref num2);
						}
					}
				}
			}
			if (info.hasFirstPersonViewParts)
			{
				for (int num5 = 0; num5 < info.firstPersonViewParts.Length; num5++)
				{
					CosmeticPart cosmeticPart3 = info.firstPersonViewParts[num5];
					if (!cosmeticPart3.prefabAssetRef.RuntimeKeyIsValid())
					{
						GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in first person parts, skipping load", null);
					}
					else
					{
						CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart3, num5, info, num, ref num3);
					}
				}
			}
			if (info.hasLocalRigParts)
			{
				for (int num6 = 0; num6 < info.localRigParts.Length; num6++)
				{
					CosmeticPart cosmeticPart4 = info.localRigParts[num6];
					if (!cosmeticPart4.prefabAssetRef.RuntimeKeyIsValid())
					{
						GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in local rig parts, skipping load", null);
					}
					else
					{
						CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart4, num6, info, num, ref num4);
					}
				}
			}
		}
		CosmeticsV2Spawner_Dirty._Step4_PopulateAllArrays();
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x0006A9FC File Offset: 0x00068BFC
	private static void AddEachAttachInfoToLoadOpInfosList(CosmeticPart part, int partIndex, CosmeticInfoV2 cosmeticInfo, int vrRigIndex, ref int partCount)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		for (int i = 0; i < part.attachAnchors.Length; i++)
		{
			CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo = new CosmeticsV2Spawner_Dirty.LoadOpInfo(part.attachAnchors[i], part, partIndex, cosmeticInfo, vrRigIndex);
			if (cosmeticInfo.isThrowable)
			{
				if (GTHardCodedBones.GetHandednessFromBone(loadOpInfo.attachInfo.parentBone) == EHandedness.Right)
				{
					for (int j = 0; j < cosmeticInfo.throwableMaterialGrabIndices.Length; j++)
					{
						int num = cosmeticInfo.throwableMaterialGrabIndices[j];
						if (!CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight.ContainsKey(num))
						{
							CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight[num] = cosmeticInfo.playFabID;
						}
					}
					CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringRight.TryAdd(cosmeticInfo.throwableIndex, cosmeticInfo.playFabID);
				}
				else
				{
					for (int k = 0; k < cosmeticInfo.throwableMaterialGrabIndices.Length; k++)
					{
						int num2 = cosmeticInfo.throwableMaterialGrabIndices[k];
						if (!CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft.ContainsKey(num2))
						{
							CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft[num2] = cosmeticInfo.playFabID;
						}
					}
					CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringLeft.TryAdd(cosmeticInfo.throwableIndex, cosmeticInfo.playFabID);
				}
			}
			CosmeticsV2Spawner_Dirty._g_loadOpInfos.Add(loadOpInfo);
			if (CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[vrRigIndex] == null)
			{
				CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[vrRigIndex] = new Dictionary<string, List<CosmeticsV2Spawner_Dirty.LoadOpInfo>>();
			}
			if (!CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[vrRigIndex].ContainsKey(cosmeticInfo.playFabID))
			{
				CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[vrRigIndex].Add(cosmeticInfo.playFabID, new List<CosmeticsV2Spawner_Dirty.LoadOpInfo>());
			}
			CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[vrRigIndex][cosmeticInfo.playFabID].Add(loadOpInfo);
			partCount++;
			if (part.partType == ECosmeticPartType.Holdable && i == 0)
			{
				break;
			}
		}
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x0006AB88 File Offset: 0x00068D88
	public static bool GetPlayfabIdFromThrowableIndex(bool isLeft, int throwableIndex, out string playfabId)
	{
		if (CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringLeft == null || CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringRight == null)
		{
			playfabId = "null";
			return false;
		}
		if (isLeft && CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringLeft.TryGetValue(throwableIndex, out playfabId))
		{
			return true;
		}
		if (!isLeft && CosmeticsV2Spawner_Dirty.throwableIndexPlayfabIdStringRight.TryGetValue(throwableIndex, out playfabId))
		{
			return true;
		}
		playfabId = "null";
		return false;
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x0006ABDC File Offset: 0x00068DDC
	public static bool GetThrowableIDFromMaterialIndex(bool isLeft, int matIndex, out string throwableId)
	{
		if (CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft == null || CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight == null)
		{
			throwableId = "null";
			return false;
		}
		if (isLeft && CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft.TryGetValue(matIndex, out throwableId))
		{
			return true;
		}
		if (!isLeft && CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight.TryGetValue(matIndex, out throwableId))
		{
			return true;
		}
		throwableId = "null";
		return false;
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x0006AC30 File Offset: 0x00068E30
	public static void ProcessLoadOpInfos(VRRig rig, string playfabId, CosmeticItemRegistry registry)
	{
		CosmeticsV2Spawner_Dirty.<>c__DisplayClass37_0 CS$<>8__locals1 = new CosmeticsV2Spawner_Dirty.<>c__DisplayClass37_0();
		CS$<>8__locals1.registry = registry;
		if (!CosmeticsV2Spawner_Dirty.processedIdsByRig.ContainsKey(rig))
		{
			CosmeticsV2Spawner_Dirty.processedIdsByRig.Add(rig, new HashSet<string>());
		}
		else if (CosmeticsV2Spawner_Dirty.processedIdsByRig[rig].Contains(playfabId))
		{
			return;
		}
		CosmeticsV2Spawner_Dirty.processedIdsByRig[rig].Add(playfabId);
		if (!CosmeticsV2Spawner_Dirty.currentGOBatchByRegistry.ContainsKey(CS$<>8__locals1.registry))
		{
			CosmeticsV2Spawner_Dirty.currentGOBatchByRegistry[CS$<>8__locals1.registry] = new List<GameObject>();
		}
		if (!CosmeticsV2Spawner_Dirty.sides.ContainsKey(CS$<>8__locals1.registry))
		{
			CosmeticsV2Spawner_Dirty.sides[CS$<>8__locals1.registry] = new List<StringEnum<ECosmeticSelectSide>>();
		}
		if (!CosmeticsV2Spawner_Dirty.overrides.ContainsKey(CS$<>8__locals1.registry))
		{
			CosmeticsV2Spawner_Dirty.overrides[CS$<>8__locals1.registry] = new List<bool>();
		}
		List<CosmeticsV2Spawner_Dirty.LoadOpInfo> list = CosmeticsV2Spawner_Dirty._g_loadOpInfosForRigAndCosmeticIDDicts[CosmeticsV2Spawner_Dirty._gVRRigDatasIndexByRig[rig]][playfabId];
		for (int i = 0; i < list.Count; i++)
		{
			CosmeticsV2Spawner_Dirty.<>c__DisplayClass37_1 CS$<>8__locals2 = new CosmeticsV2Spawner_Dirty.<>c__DisplayClass37_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals2.currentIndex = CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Count;
			CosmeticsV2Spawner_Dirty._ProcessLoadOpInfo(CS$<>8__locals2.currentIndex, list[i]);
			CosmeticsV2Spawner_Dirty._g_loadOpInfos[CS$<>8__locals2.currentIndex].loadOp.Completed += CS$<>8__locals2.<ProcessLoadOpInfos>g__AddToRegistryWhenCompleted|2;
		}
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x0006AD88 File Offset: 0x00068F88
	private static void _ProcessLoadOpInfo(int currentIndex, CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo)
	{
		try
		{
			loadOpInfo.loadOp = loadOpInfo.part.prefabAssetRef.InstantiateAsync(CosmeticsV2Spawner_Dirty._gDeactivatedSpawnParent, false);
			loadOpInfo.isStarted = true;
			CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Add(loadOpInfo.loadOp, currentIndex);
			loadOpInfo.loadOp.Completed += CosmeticsV2Spawner_Dirty._Step3_HandleLoadOpCompleted;
			CosmeticsV2Spawner_Dirty._g_loadOpInfos[currentIndex] = loadOpInfo;
		}
		catch (InvalidKeyException ex)
		{
			Debug.LogError("CosmeticsV2Spawner_Dirty: Missing Addressable for " + string.Format("\"{0}\" part index {1}. Skipping. {2}", loadOpInfo.cosmeticInfoV2.displayName, loadOpInfo.partIndex, ex.Message));
			loadOpInfo.isStarted = true;
			loadOpInfo.resultGObj = null;
			CosmeticsV2Spawner_Dirty._g_loadOpInfos[currentIndex] = loadOpInfo;
			CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted++;
		}
		catch (ArgumentException ex2)
		{
			Debug.LogError("CosmeticsV2Spawner_Dirty: Invalid Addressable key/config for " + string.Format("\"{0}\" part index {1}. Skipping. {2}", loadOpInfo.cosmeticInfoV2.displayName, loadOpInfo.partIndex, ex2.Message));
			loadOpInfo.isStarted = true;
			loadOpInfo.resultGObj = null;
			CosmeticsV2Spawner_Dirty._g_loadOpInfos[currentIndex] = loadOpInfo;
			CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted++;
		}
	}

	// Token: 0x060013C8 RID: 5064 RVA: 0x0006AED0 File Offset: 0x000690D0
	private static void _Step3_HandleLoadOpCompleted(AsyncOperationHandle<GameObject> loadOp)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		int num;
		if (!CosmeticsV2Spawner_Dirty._g_loadOp_to_index.TryGetValue(loadOp, out num))
		{
			throw new Exception("(this should never happen) could not find LoadOpInfo in `_g_loadOpInfos`.");
		}
		CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo = CosmeticsV2Spawner_Dirty._g_loadOpInfos[num];
		if (loadOp.Status == AsyncOperationStatus.Failed)
		{
			Debug.LogWarning("CosmeticsV2Spawner_Dirty: Failed to load part " + string.Format("\"{0}\" (key: {1}). Skipping.", loadOpInfo.cosmeticInfoV2.displayName, loadOpInfo.part.prefabAssetRef.RuntimeKey));
			CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted++;
			CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Remove(loadOp);
			return;
		}
		CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted++;
		ECosmeticSelectSide ecosmeticSelectSide = loadOpInfo.attachInfo.selectSide;
		string text = loadOpInfo.cosmeticInfoV2.playFabID;
		if (ecosmeticSelectSide != ECosmeticSelectSide.Both)
		{
			string playFabID = loadOpInfo.cosmeticInfoV2.playFabID;
			string text2;
			if (ecosmeticSelectSide != ECosmeticSelectSide.Left)
			{
				if (ecosmeticSelectSide != ECosmeticSelectSide.Right)
				{
					text2 = "";
				}
				else
				{
					text2 = " RIGHT.";
				}
			}
			else
			{
				text2 = " LEFT.";
			}
			text = ZString.Concat<string, string>(playFabID, text2);
		}
		loadOpInfo.resultGObj = loadOp.Result;
		loadOpInfo.resultGObj.SetActive(false);
		Transform transform = loadOpInfo.resultGObj.transform;
		Transform transform2 = transform;
		CosmeticPart[] holdableParts = loadOpInfo.cosmeticInfoV2.holdableParts;
		if (holdableParts != null && holdableParts.Length > 0)
		{
			TransferrableObject componentInChildren = loadOpInfo.resultGObj.GetComponentInChildren<TransferrableObject>(true);
			if (componentInChildren && componentInChildren.gameObject != loadOpInfo.resultGObj)
			{
				transform2 = componentInChildren.transform;
				transform2.gameObject.SetActive(false);
				loadOpInfo.resultGObj.SetActive(true);
			}
		}
		if (loadOpInfo.cosmeticInfoV2.isThrowable)
		{
			SnowballThrowable componentInChildren2 = loadOpInfo.resultGObj.GetComponentInChildren<SnowballThrowable>(true);
			if (componentInChildren2 && componentInChildren2.gameObject != loadOpInfo.resultGObj)
			{
				transform2 = componentInChildren2.transform;
				transform2.gameObject.SetActive(false);
				loadOpInfo.resultGObj.SetActive(true);
			}
		}
		transform2.name = text;
		CosmeticsV2Spawner_Dirty.VRRigData vrrigData = ((loadOpInfo.vrRigIndex != -1) ? CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex] : default(CosmeticsV2Spawner_Dirty.VRRigData));
		if (loadOpInfo.cosmeticInfoV2.category != CosmeticsController.CosmeticCategory.Collectable)
		{
			Transform transform3;
			switch (loadOpInfo.part.partType)
			{
			case ECosmeticPartType.Holdable:
				transform3 = ((loadOpInfo.attachInfo.parentBone != GTHardCodedBones.EBone.body_AnchorFront_StowSlot) ? vrrigData.parentOfDeactivatedHoldables : vrrigData.boneXforms[(int)loadOpInfo.attachInfo.parentBone]);
				goto IL_02DD;
			case ECosmeticPartType.Functional:
				transform3 = vrrigData.boneXforms[(int)loadOpInfo.attachInfo.parentBone];
				goto IL_02DD;
			case ECosmeticPartType.FirstPerson:
				transform3 = CosmeticsV2Spawner_Dirty.g_gorillaPlayer.CosmeticsHeadTarget;
				goto IL_02DD;
			case ECosmeticPartType.LocalRig:
				transform3 = vrrigData.boneXforms[(int)loadOpInfo.attachInfo.parentBone];
				goto IL_02DD;
			}
			throw new ArgumentOutOfRangeException("partType", "unhandled part type.");
			IL_02DD:
			Transform transform4 = transform3;
			if (transform4)
			{
				transform.SetParent(transform4, false);
				transform.localPosition = loadOpInfo.attachInfo.offset.pos;
				Transform transform5 = transform;
				XformOffset offset = loadOpInfo.attachInfo.offset;
				transform5.localRotation = offset.rot;
				transform.localScale = loadOpInfo.attachInfo.offset.scale;
			}
			else
			{
				Debug.LogError(string.Concat(new string[]
				{
					string.Format("Bone transform not found for cosmetic part type {0}. Cosmetic: ", loadOpInfo.part.partType),
					"\"",
					loadOpInfo.cosmeticInfoV2.displayName,
					"\",",
					string.Format("part: \"{0}\"", loadOpInfo.part.prefabAssetRef.RuntimeKey)
				}));
			}
		}
		switch (loadOpInfo.part.partType)
		{
		case ECosmeticPartType.Holdable:
		{
			vrrigData.vrRig_cosmetics.Add(transform2.gameObject);
			HoldableObject componentInChildren3 = loadOpInfo.resultGObj.GetComponentInChildren<HoldableObject>(true);
			SnowballThrowable snowballThrowable = componentInChildren3 as SnowballThrowable;
			if (snowballThrowable != null)
			{
				CosmeticsV2Spawner_Dirty.AddPartToThrowableLists(loadOpInfo, snowballThrowable);
				goto IL_0656;
			}
			TransferrableObject transferrableObject = componentInChildren3 as TransferrableObject;
			if (transferrableObject == null)
			{
				if (componentInChildren3 != null)
				{
					throw new Exception("Encountered unexpected HoldableObject derived type on cosmetic part: \"" + loadOpInfo.cosmeticInfoV2.displayName + "\"");
				}
				goto IL_0656;
			}
			else
			{
				string text3 = loadOpInfo.cosmeticInfoV2.playFabID;
				int[] array;
				if (CosmeticsLegacyV1Info.TryGetBodyDockAllObjectsIndexes(text3, out array))
				{
					if (loadOpInfo.partIndex < array.Length && loadOpInfo.partIndex >= 0)
					{
						transferrableObject.myIndex = array[loadOpInfo.partIndex];
					}
				}
				else if (text3.Length >= 5 && text3[0] == 'L')
				{
					if (text3[1] != 'M')
					{
						throw new Exception("(this should never happen) A TransferrableObject cosmetic added sometime after 2024-06 does not use the expected PlayFabID format where the string starts with \"LM\" and ends with \".\". Path: " + transform2.GetPathQ());
					}
					string text4 = text3;
					text3 = ((text4[text4.Length - 1] == '.') ? text3 : (text3 + "."));
					int num2 = 224;
					transferrableObject.myIndex = num2 + CosmeticIDUtils.PlayFabIdToIndexInCategory(text3);
				}
				else
				{
					transferrableObject.myIndex = -2;
					if (!(text3 == "STICKABLE TARGET"))
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Cosmetic \"",
							loadOpInfo.cosmeticInfoV2.displayName,
							"\" cannot derive `TransferrableObject.myIndex` from playFabId \"",
							text3,
							"\" and so will not be included in `BodyDockPositions.allObjects` array."
						}));
					}
				}
				ProjectileWeapon projectileWeapon = transferrableObject as ProjectileWeapon;
				if (projectileWeapon != null && loadOpInfo.cosmeticInfoV2.playFabID == "Slingshot")
				{
					vrrigData.vrRig.projectileWeapon = projectileWeapon;
				}
				if (transferrableObject.myIndex <= 0 || transferrableObject.myIndex >= vrrigData.bdPositions_allObjects_length)
				{
					goto IL_0656;
				}
				vrrigData.bdPositionsComp._allObjects[transferrableObject.myIndex] = transferrableObject;
				if (!vrrigData.vrRig.isOfflineVRRig)
				{
					vrrigData.bdPositionsComp.RefreshTransferrableItems();
					goto IL_0656;
				}
				goto IL_0656;
			}
			break;
		}
		case ECosmeticPartType.Functional:
			vrrigData.vrRig_cosmetics.Add(transform2.gameObject);
			goto IL_0656;
		case ECosmeticPartType.FirstPerson:
		case ECosmeticPartType.LocalRig:
			vrrigData.vrRig_override.Add(transform2.gameObject);
			goto IL_0656;
		}
		throw new ArgumentOutOfRangeException("Unexpected ECosmeticPartType value encountered: " + string.Format("{0}, ", loadOpInfo.part.partType) + string.Format("int: {0}.", (int)loadOpInfo.part.partType));
		IL_0656:
		if (loadOpInfo.vrRigIndex > -1)
		{
			CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex] = vrrigData;
		}
		CosmeticRefRegistry cosmeticReferences = CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex].vrRig.cosmeticReferences;
		CosmeticRefTarget[] componentsInChildren = loadOpInfo.resultGObj.GetComponentsInChildren<CosmeticRefTarget>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			cosmeticReferences.Register(componentsInChildren[i].id, componentsInChildren[i].gameObject);
		}
		CosmeticsV2Spawner_Dirty._g_loadOpInfos[num] = loadOpInfo;
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x0006B5B4 File Offset: 0x000697B4
	private static void _RetryDownload(int loadOpIndex)
	{
		if (loadOpIndex < 0 || loadOpIndex >= CosmeticsV2Spawner_Dirty._g_loadOpInfos.Count)
		{
			Debug.LogError("(should never happen) Unexpected! While trying to recover from a failed download, the value " + string.Format("{0}={1} was out of range of ", "loadOpIndex", loadOpIndex) + string.Format("{0}.Count={1}.", "_g_loadOpInfos", CosmeticsV2Spawner_Dirty._g_loadOpInfos.Count));
			return;
		}
		CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo = CosmeticsV2Spawner_Dirty._g_loadOpInfos[loadOpIndex];
		if (!CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Remove(loadOpInfo.loadOp))
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"(should never happen) Unexpected! Could not find the loadOp to remove it in the _g_loadOp_to_index. If you see this message then comparison does not work the way I thought and we need a different way to store/retrieve loadOpInfos. Happened while trying to retry failed download prefab part of cosmetic \"",
				loadOpInfo.cosmeticInfoV2.displayName,
				"\" with guid \"",
				loadOpInfo.part.prefabAssetRef.AssetGUID,
				"\"."
			}));
		}
		Debug.Log(string.Concat(new string[]
		{
			"Retrying prefab part of cosmetic \"",
			loadOpInfo.cosmeticInfoV2.displayName,
			"\" with guid \"",
			loadOpInfo.part.prefabAssetRef.AssetGUID,
			"\"."
		}));
		loadOpInfo.loadOp = loadOpInfo.part.prefabAssetRef.InstantiateAsync(CosmeticsV2Spawner_Dirty._gDeactivatedSpawnParent, false);
		CosmeticsV2Spawner_Dirty._g_loadOpInfos[loadOpIndex] = loadOpInfo;
		CosmeticsV2Spawner_Dirty._g_loadOp_to_index[loadOpInfo.loadOp] = loadOpIndex;
		loadOpInfo.loadOp.Completed += CosmeticsV2Spawner_Dirty._Step3_HandleLoadOpCompleted;
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x0006B718 File Offset: 0x00069918
	private static void AddPartToThrowableLists(CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo, SnowballThrowable throwable)
	{
		CosmeticsV2Spawner_Dirty.VRRigData vrrigData = CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex];
		EHandedness handednessFromBone = GTHardCodedBones.GetHandednessFromBone(loadOpInfo.attachInfo.parentBone);
		bool flag = vrrigData.vrRig == CosmeticsV2Spawner_Dirty._gVRRigDatas[0].vrRig;
		throwable.SpawnOffset = loadOpInfo.attachInfo.offset;
		switch (handednessFromBone)
		{
		case EHandedness.None:
			throw new ArgumentException(string.Concat(new string[]
			{
				"Encountered throwable cosmetic \"",
				loadOpInfo.cosmeticInfoV2.displayName,
				"\" where handedness ",
				string.Format("could not be determined from bone `{0}`. ", loadOpInfo.attachInfo.parentBone),
				"Path: \"",
				throwable.transform.GetPath(),
				"\""
			}));
		case EHandedness.Left:
			CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<GameObject>(vrrigData.bdPositions_leftHandThrowables, throwable.gameObject, throwable.throwableMakerIndex);
			if (flag)
			{
				CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<SnowballThrowable>(CosmeticsV2Spawner_Dirty._gSnowballMakerLeft_throwables, throwable, throwable.throwableMakerIndex);
			}
			vrrigData.bdPositionsComp.leftHandThrowables = vrrigData.bdPositions_leftHandThrowables.ToArray();
			CosmeticsV2Spawner_Dirty._gSnowballMakerLeft.SetupThrowables(CosmeticsV2Spawner_Dirty._gSnowballMakerLeft_throwables.ToArray());
			return;
		case EHandedness.Right:
			CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<GameObject>(vrrigData.bdPositions_rightHandThrowables, throwable.gameObject, throwable.throwableMakerIndex);
			if (flag)
			{
				CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<SnowballThrowable>(CosmeticsV2Spawner_Dirty._gSnowballMakerRight_throwables, throwable, throwable.throwableMakerIndex);
			}
			vrrigData.bdPositionsComp.rightHandThrowables = vrrigData.bdPositions_rightHandThrowables.ToArray();
			CosmeticsV2Spawner_Dirty._gSnowballMakerRight.SetupThrowables(CosmeticsV2Spawner_Dirty._gSnowballMakerRight_throwables.ToArray());
			return;
		default:
			throw new ArgumentOutOfRangeException("Unexpected ECosmeticSelectSide value encountered: " + string.Format("{0}, ", handednessFromBone) + string.Format("int: {0}.", (int)handednessFromBone));
		}
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x0006B8D8 File Offset: 0x00069AD8
	private static void ResizeAndSetAtIndex<T>(List<T> list, T item, int index)
	{
		if (index >= list.Count)
		{
			int num = index - list.Count + 1;
			for (int i = 0; i < num; i++)
			{
				list.Add(default(T));
			}
		}
		list[index] = item;
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x0006B91C File Offset: 0x00069B1C
	private static void _Step4_PopulateAllArrays()
	{
		foreach (CosmeticsV2Spawner_Dirty.VRRigData vrrigData in CosmeticsV2Spawner_Dirty._gVRRigDatas)
		{
			vrrigData.bdPositionsComp._allObjects = new TransferrableObject[2000];
		}
		GTDelayedExec.Add(CosmeticsV2Spawner_Dirty._instance, 1f, -Mathf.Abs("_Step5_InitializeVRRigsAndCosmeticsControllerFinalize".GetHashCode()));
	}

	// Token: 0x060013CD RID: 5069 RVA: 0x0006B99C File Offset: 0x00069B9C
	private static void _Step5_InitializeVRRigsAndCosmeticsControllerFinalize()
	{
		CosmeticsController.instance.UpdateWardrobeModelsAndButtons();
		try
		{
			Action onPostInstantiateAllPrefabs = CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs;
			if (onPostInstantiateAllPrefabs != null)
			{
				onPostInstantiateAllPrefabs();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		try
		{
			CosmeticsController.instance.InitializeCosmeticStands();
		}
		catch (Exception ex2)
		{
			Debug.LogException(ex2);
		}
		try
		{
			CosmeticsController.instance.UpdateWornCosmetics();
			CosmeticsV2Spawner_Dirty.<_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|44_0();
		}
		catch (Exception ex3)
		{
			Debug.LogException(ex3);
		}
		foreach (CosmeticsV2Spawner_Dirty.VRRigData vrrigData in CosmeticsV2Spawner_Dirty._gVRRigDatas)
		{
			try
			{
				if (vrrigData.bdPositionsComp.isActiveAndEnabled)
				{
					vrrigData.bdPositionsComp.RefreshTransferrableItems();
				}
			}
			catch (Exception ex4)
			{
				Debug.LogException(ex4, vrrigData.vrRig);
			}
		}
		try
		{
			StoreController.instance.InitalizeCosmeticStands();
		}
		catch (Exception ex5)
		{
			Debug.LogException(ex5);
		}
		CosmeticsV2Spawner_Dirty.isPrepared = true;
		CosmeticsV2Spawner_Dirty.k_stopwatch.Stop();
		Debug.Log("_Step5_InitializeVRRigsAndCosmeticsControllerFinalize" + string.Format(": Done preparing cosmetics system in {0:0.0000} seconds.", (double)CosmeticsV2Spawner_Dirty.k_stopwatch.ElapsedMilliseconds / 1000.0));
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x0006BAFC File Offset: 0x00069CFC
	public static CosmeticsV2Spawner_Dirty.VRRigData RigDataForRig(VRRig rig)
	{
		return CosmeticsV2Spawner_Dirty._gVRRigDatas[CosmeticsV2Spawner_Dirty._gVRRigDatasIndexByRig[rig]];
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x0006BBA8 File Offset: 0x00069DA8
	[CompilerGenerated]
	internal static void <ProcessLoadOpInfos>g__PostCompletionProcess|37_0()
	{
		foreach (KeyValuePair<CosmeticItemRegistry, List<GameObject>> keyValuePair in CosmeticsV2Spawner_Dirty.currentGOBatchByRegistry)
		{
			List<GameObject> value = keyValuePair.Value;
			CosmeticItemRegistry key = keyValuePair.Key;
			for (int i = 0; i < value.Count; i++)
			{
				if (!(value[i] == null))
				{
					try
					{
						if (CosmeticsV2Spawner_Dirty.overrides[key][i])
						{
							key.InitializeCosmetic(value[i], true);
						}
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"CosmeticsV2Spawner_Dirty.PostCompletionProcess: InitializeCosmetic (override) threw for \"",
							value[i].name,
							"\" on rig \"",
							(key.Rig != null) ? key.Rig.name : "<null rig>",
							"\". Skipping this cosmetic so the rest of the batch still initializes. Exception follows:"
						}), value[i]);
						Debug.LogException(ex, value[i]);
					}
				}
			}
			for (int j = 0; j < value.Count; j++)
			{
				if (!(value[j] == null))
				{
					try
					{
						if (!CosmeticsV2Spawner_Dirty.overrides[key][j])
						{
							key.InitializeCosmetic(value[j], false);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"CosmeticsV2Spawner_Dirty.PostCompletionProcess: InitializeCosmetic threw for \"",
							value[j].name,
							"\" on rig \"",
							(key.Rig != null) ? key.Rig.name : "<null rig>",
							"\". Skipping this cosmetic so the rest of the batch still initializes. Exception follows:"
						}), value[j]);
						Debug.LogException(ex2, value[j]);
					}
				}
			}
			for (int k = 0; k < value.Count; k++)
			{
				if (!(value[k] == null))
				{
					ISpawnable[] componentsInChildren = value[k].GetComponentsInChildren<ISpawnable>(true);
					for (int l = 0; l < componentsInChildren.Length; l++)
					{
						if (!componentsInChildren[l].IsSpawned)
						{
							try
							{
								componentsInChildren[l].IsSpawned = true;
								componentsInChildren[l].CosmeticSelectedSide = CosmeticsV2Spawner_Dirty.sides[key][k];
								componentsInChildren[l].OnSpawn(key.Rig);
							}
							catch (Exception ex3)
							{
								Debug.LogException(ex3);
							}
						}
					}
				}
			}
			value.Clear();
			CosmeticsV2Spawner_Dirty.sides[key].Clear();
			CosmeticsV2Spawner_Dirty.overrides[key].Clear();
			key.RefreshRig();
			if (key.Rig != null && key.Rig.myBodyDockPositions != null)
			{
				key.Rig.myBodyDockPositions.RefreshTransferrableItems();
			}
		}
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x0006BEF0 File Offset: 0x0006A0F0
	[CompilerGenerated]
	internal static GameObject <ProcessLoadOpInfos>g__ObjectToInitialize|37_1(CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo)
	{
		if (loadOpInfo.resultGObj == null)
		{
			return null;
		}
		Transform transform = loadOpInfo.resultGObj.transform;
		CosmeticPart[] holdableParts = loadOpInfo.cosmeticInfoV2.holdableParts;
		if (holdableParts != null && holdableParts.Length > 0)
		{
			TransferrableObject componentInChildren = loadOpInfo.resultGObj.GetComponentInChildren<TransferrableObject>(true);
			if (componentInChildren && componentInChildren.gameObject != loadOpInfo.resultGObj)
			{
				transform = componentInChildren.transform;
				transform.gameObject.SetActive(false);
				loadOpInfo.resultGObj.SetActive(true);
			}
		}
		if (loadOpInfo.cosmeticInfoV2.isThrowable)
		{
			SnowballThrowable componentInChildren2 = loadOpInfo.resultGObj.GetComponentInChildren<SnowballThrowable>(true);
			if (componentInChildren2 && componentInChildren2.gameObject != loadOpInfo.resultGObj)
			{
				transform = componentInChildren2.transform;
				transform.gameObject.SetActive(false);
				loadOpInfo.resultGObj.SetActive(true);
			}
		}
		return transform.gameObject;
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x0006BFD0 File Offset: 0x0006A1D0
	[CompilerGenerated]
	internal static async void <_Step5_InitializeVRRigsAndCosmeticsControllerFinalize>g__StartupRerun|44_0()
	{
		await Awaitable.WaitForSecondsAsync(2f, default(CancellationToken));
		CosmeticsController.instance.UpdateWornCosmetics();
	}

	// Token: 0x04001847 RID: 6215
	private static CosmeticsV2Spawner_Dirty _instance;

	// Token: 0x04001848 RID: 6216
	public static Action OnPostInstantiateAllPrefabs;

	// Token: 0x0400184A RID: 6218
	[OnEnterPlay_SetNull]
	private static Transform _gDeactivatedSpawnParent;

	// Token: 0x0400184B RID: 6219
	[OnEnterPlay_Set(0)]
	private static int _g_loadOpsCountCompleted = 0;

	// Token: 0x0400184C RID: 6220
	private const int _k_maxActiveLoadOps = 1000000;

	// Token: 0x0400184D RID: 6221
	private const int _k_maxTotalLoadOps = 1000000;

	// Token: 0x0400184E RID: 6222
	private const int _k_delayedStatusCheckContextId = -100;

	// Token: 0x0400184F RID: 6223
	[OnEnterPlay_Clear]
	private static readonly List<CosmeticsV2Spawner_Dirty.LoadOpInfo> _g_loadOpInfos = new List<CosmeticsV2Spawner_Dirty.LoadOpInfo>(100000);

	// Token: 0x04001850 RID: 6224
	[OnEnterPlay_Clear]
	private static Dictionary<string, List<CosmeticsV2Spawner_Dirty.LoadOpInfo>>[] _g_loadOpInfosForRigAndCosmeticIDDicts;

	// Token: 0x04001851 RID: 6225
	[OnEnterPlay_Clear]
	private static readonly Dictionary<AsyncOperationHandle<GameObject>, int> _g_loadOp_to_index = new Dictionary<AsyncOperationHandle<GameObject>, int>(100000);

	// Token: 0x04001852 RID: 6226
	[OnEnterPlay_SetNull]
	private static SnowballMaker _gSnowballMakerLeft;

	// Token: 0x04001853 RID: 6227
	[OnEnterPlay_Clear]
	private static readonly List<SnowballThrowable> _gSnowballMakerLeft_throwables = new List<SnowballThrowable>(20);

	// Token: 0x04001854 RID: 6228
	[OnEnterPlay_SetNull]
	private static SnowballMaker _gSnowballMakerRight;

	// Token: 0x04001855 RID: 6229
	[OnEnterPlay_Clear]
	private static readonly List<SnowballThrowable> _gSnowballMakerRight_throwables = new List<SnowballThrowable>(20);

	// Token: 0x04001856 RID: 6230
	[OnEnterPlay_SetNull]
	private static GTPlayer g_gorillaPlayer;

	// Token: 0x04001857 RID: 6231
	private static Stopwatch k_stopwatch = new Stopwatch();

	// Token: 0x04001858 RID: 6232
	[OnEnterPlay_Clear]
	public static readonly List<CosmeticsV2Spawner_Dirty.VRRigData> _gVRRigDatas = new List<CosmeticsV2Spawner_Dirty.VRRigData>(20);

	// Token: 0x04001859 RID: 6233
	private static Dictionary<VRRig, int> _gVRRigDatasIndexByRig = new Dictionary<VRRig, int>();

	// Token: 0x0400185A RID: 6234
	[OnEnterPlay_Clear]
	private static Dictionary<int, string> materialIndexToSnowballThrowablePlayfabIdStringLeft;

	// Token: 0x0400185B RID: 6235
	[OnEnterPlay_Clear]
	private static Dictionary<int, string> materialIndexToSnowballThrowablePlayfabIdStringRight;

	// Token: 0x0400185C RID: 6236
	[OnEnterPlay_Clear]
	private static Dictionary<int, string> throwableIndexPlayfabIdStringRight;

	// Token: 0x0400185D RID: 6237
	[OnEnterPlay_Clear]
	private static Dictionary<int, string> throwableIndexPlayfabIdStringLeft;

	// Token: 0x0400185E RID: 6238
	private static Dictionary<VRRig, HashSet<string>> processedIdsByRig = new Dictionary<VRRig, HashSet<string>>();

	// Token: 0x0400185F RID: 6239
	private static Dictionary<CosmeticItemRegistry, List<GameObject>> currentGOBatchByRegistry = new Dictionary<CosmeticItemRegistry, List<GameObject>>();

	// Token: 0x04001860 RID: 6240
	private static Dictionary<CosmeticItemRegistry, List<StringEnum<ECosmeticSelectSide>>> sides = new Dictionary<CosmeticItemRegistry, List<StringEnum<ECosmeticSelectSide>>>();

	// Token: 0x04001861 RID: 6241
	private static Dictionary<CosmeticItemRegistry, List<bool>> overrides = new Dictionary<CosmeticItemRegistry, List<bool>>();

	// Token: 0x02000309 RID: 777
	private struct LoadOpInfo
	{
		// Token: 0x060013D4 RID: 5076 RVA: 0x0006C000 File Offset: 0x0006A200
		public LoadOpInfo(CosmeticAttachInfo attachInfo, CosmeticPart part, int partIndex, CosmeticInfoV2 cosmeticInfoV2, int vrRigIndex)
		{
			this.isStarted = false;
			this.loadOp = default(AsyncOperationHandle<GameObject>);
			this.resultGObj = null;
			this.attachInfo = attachInfo;
			this.part = part;
			this.partIndex = partIndex;
			this.cosmeticInfoV2 = cosmeticInfoV2;
			this.vrRigIndex = vrRigIndex;
		}

		// Token: 0x04001862 RID: 6242
		public bool isStarted;

		// Token: 0x04001863 RID: 6243
		public AsyncOperationHandle<GameObject> loadOp;

		// Token: 0x04001864 RID: 6244
		public GameObject resultGObj;

		// Token: 0x04001865 RID: 6245
		public readonly CosmeticAttachInfo attachInfo;

		// Token: 0x04001866 RID: 6246
		public readonly CosmeticPart part;

		// Token: 0x04001867 RID: 6247
		public readonly int partIndex;

		// Token: 0x04001868 RID: 6248
		public readonly CosmeticInfoV2 cosmeticInfoV2;

		// Token: 0x04001869 RID: 6249
		public readonly int vrRigIndex;
	}

	// Token: 0x0200030A RID: 778
	public struct VRRigData
	{
		// Token: 0x060013D5 RID: 5077 RVA: 0x0006C04C File Offset: 0x0006A24C
		public VRRigData(VRRig vrRig, Transform[] boneXforms)
		{
			this.vrRig = vrRig;
			this.boneXforms = boneXforms;
			if (!vrRig.transform.TryFindByPath("./**/Holdables", out this.parentOfDeactivatedHoldables, false))
			{
				Debug.LogError("Could not find parent for deactivated holdables. Falling back to VRRig transform: \"" + vrRig.transform.GetPath() + "\"");
			}
			this.bdPositionsComp = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			this.vrRig_cosmetics = new List<GameObject>(500);
			this.vrRig_override = new List<GameObject>(500);
			this.bdPositions_leftHandThrowables = new List<GameObject>(20);
			this.bdPositions_rightHandThrowables = new List<GameObject>(20);
			this.bdPositions_allObjects_length = 2000;
		}

		// Token: 0x0400186A RID: 6250
		public readonly VRRig vrRig;

		// Token: 0x0400186B RID: 6251
		public readonly Transform[] boneXforms;

		// Token: 0x0400186C RID: 6252
		public readonly BodyDockPositions bdPositionsComp;

		// Token: 0x0400186D RID: 6253
		public readonly List<GameObject> vrRig_cosmetics;

		// Token: 0x0400186E RID: 6254
		public readonly List<GameObject> vrRig_override;

		// Token: 0x0400186F RID: 6255
		public readonly Transform parentOfDeactivatedHoldables;

		// Token: 0x04001870 RID: 6256
		public int bdPositions_allObjects_length;

		// Token: 0x04001871 RID: 6257
		public readonly List<GameObject> bdPositions_leftHandThrowables;

		// Token: 0x04001872 RID: 6258
		public readonly List<GameObject> bdPositions_rightHandThrowables;
	}
}
