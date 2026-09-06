using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200095F RID: 2399
public class ProgressionManager : MonoBehaviour
{
	// Token: 0x170005D3 RID: 1491
	// (get) Token: 0x06003F03 RID: 16131 RVA: 0x00152F70 File Offset: 0x00151170
	// (set) Token: 0x06003F04 RID: 16132 RVA: 0x00152F77 File Offset: 0x00151177
	public static ProgressionManager Instance { get; private set; }

	// Token: 0x14000073 RID: 115
	// (add) Token: 0x06003F05 RID: 16133 RVA: 0x00152F80 File Offset: 0x00151180
	// (remove) Token: 0x06003F06 RID: 16134 RVA: 0x00152FB8 File Offset: 0x001511B8
	public event Action OnTreeUpdated;

	// Token: 0x14000074 RID: 116
	// (add) Token: 0x06003F07 RID: 16135 RVA: 0x00152FF0 File Offset: 0x001511F0
	// (remove) Token: 0x06003F08 RID: 16136 RVA: 0x00153028 File Offset: 0x00151228
	public event Action OnInventoryUpdated;

	// Token: 0x14000075 RID: 117
	// (add) Token: 0x06003F09 RID: 16137 RVA: 0x00153060 File Offset: 0x00151260
	// (remove) Token: 0x06003F0A RID: 16138 RVA: 0x00153098 File Offset: 0x00151298
	public event Action<string, int> OnTrackRead;

	// Token: 0x14000076 RID: 118
	// (add) Token: 0x06003F0B RID: 16139 RVA: 0x001530D0 File Offset: 0x001512D0
	// (remove) Token: 0x06003F0C RID: 16140 RVA: 0x00153108 File Offset: 0x00151308
	public event Action<string, int> OnTrackSet;

	// Token: 0x14000077 RID: 119
	// (add) Token: 0x06003F0D RID: 16141 RVA: 0x00153140 File Offset: 0x00151340
	// (remove) Token: 0x06003F0E RID: 16142 RVA: 0x00153178 File Offset: 0x00151378
	public event Action<string, string> OnNodeUnlocked;

	// Token: 0x14000078 RID: 120
	// (add) Token: 0x06003F0F RID: 16143 RVA: 0x001531B0 File Offset: 0x001513B0
	// (remove) Token: 0x06003F10 RID: 16144 RVA: 0x001531E8 File Offset: 0x001513E8
	public event Action<string, int> OnGetShiftCredit;

	// Token: 0x14000079 RID: 121
	// (add) Token: 0x06003F11 RID: 16145 RVA: 0x00153220 File Offset: 0x00151420
	// (remove) Token: 0x06003F12 RID: 16146 RVA: 0x00153258 File Offset: 0x00151458
	public event Action<string, int, int> OnGetShiftCreditCapData;

	// Token: 0x1400007A RID: 122
	// (add) Token: 0x06003F13 RID: 16147 RVA: 0x00153290 File Offset: 0x00151490
	// (remove) Token: 0x06003F14 RID: 16148 RVA: 0x001532C8 File Offset: 0x001514C8
	public event Action<bool> OnPurchaseShiftCreditCapIncrease;

	// Token: 0x1400007B RID: 123
	// (add) Token: 0x06003F15 RID: 16149 RVA: 0x00153300 File Offset: 0x00151500
	// (remove) Token: 0x06003F16 RID: 16150 RVA: 0x00153338 File Offset: 0x00151538
	public event Action<bool> OnPurchaseShiftCredit;

	// Token: 0x1400007C RID: 124
	// (add) Token: 0x06003F17 RID: 16151 RVA: 0x00153370 File Offset: 0x00151570
	// (remove) Token: 0x06003F18 RID: 16152 RVA: 0x001533A8 File Offset: 0x001515A8
	public event Action<bool> OnChaosDepositSuccess;

	// Token: 0x1400007D RID: 125
	// (add) Token: 0x06003F19 RID: 16153 RVA: 0x001533E0 File Offset: 0x001515E0
	// (remove) Token: 0x06003F1A RID: 16154 RVA: 0x00153418 File Offset: 0x00151618
	public event Action<ProgressionManager.JuicerStatusResponse> OnJucierStatusUpdated;

	// Token: 0x1400007E RID: 126
	// (add) Token: 0x06003F1B RID: 16155 RVA: 0x00153450 File Offset: 0x00151650
	// (remove) Token: 0x06003F1C RID: 16156 RVA: 0x00153488 File Offset: 0x00151688
	public event Action<bool> OnPurchaseOverdrive;

	// Token: 0x1400007F RID: 127
	// (add) Token: 0x06003F1D RID: 16157 RVA: 0x001534C0 File Offset: 0x001516C0
	// (remove) Token: 0x06003F1E RID: 16158 RVA: 0x001534F8 File Offset: 0x001516F8
	public event Action<ProgressionManager.DockWristStatusResponse> OnDockWristStatusUpdated;

	// Token: 0x14000080 RID: 128
	// (add) Token: 0x06003F1F RID: 16159 RVA: 0x00153530 File Offset: 0x00151730
	// (remove) Token: 0x06003F20 RID: 16160 RVA: 0x00153568 File Offset: 0x00151768
	public event Action<ProgressionManager.GhostReactorStatsResponse> OnGhostReactorStatsUpdated;

	// Token: 0x14000081 RID: 129
	// (add) Token: 0x06003F21 RID: 16161 RVA: 0x001535A0 File Offset: 0x001517A0
	// (remove) Token: 0x06003F22 RID: 16162 RVA: 0x001535D8 File Offset: 0x001517D8
	public event Action<ProgressionManager.GhostReactorInventoryResponse> OnGhostReactorInventoryUpdated;

	// Token: 0x06003F23 RID: 16163 RVA: 0x0015360D File Offset: 0x0015180D
	private void Awake()
	{
		if (ProgressionManager.Instance == null)
		{
			ProgressionManager.Instance = this;
		}
	}

	// Token: 0x06003F24 RID: 16164 RVA: 0x00153624 File Offset: 0x00151824
	public async void RefreshProgressionTree()
	{
		double unscaledTimeAsDouble = Time.unscaledTimeAsDouble;
		ProgressionManager.debug_lastRefreshTreeAttemptTime = unscaledTimeAsDouble;
		if (this._treeRefreshInFlight)
		{
			ProgressionManager.debug_refreshTreeDroppedByThrottle++;
		}
		else if (unscaledTimeAsDouble - this._lastTreeRefreshTime < 2.0)
		{
			ProgressionManager.debug_refreshTreeDroppedByThrottle++;
		}
		else
		{
			this._lastTreeRefreshTime = unscaledTimeAsDouble;
			this._treeRefreshInFlight = true;
			ProgressionManager.debug_refreshTreeCount++;
			await ProgressionUtil.WaitForMothershipSessionToken();
			MothershipClientApiUnity.GetPlayerProgressionTreesData(delegate(GetProgressionTreesForPlayerResponse response)
			{
				this._treeRefreshInFlight = false;
				this.OnGetTrees(response);
			}, delegate(MothershipError err, int code)
			{
				this._treeRefreshInFlight = false;
				ProgressionManager.GetMothershipFailure(err, code);
			});
		}
	}

	// Token: 0x06003F25 RID: 16165 RVA: 0x0015365C File Offset: 0x0015185C
	public async void RefreshUserInventory()
	{
		double unscaledTimeAsDouble = Time.unscaledTimeAsDouble;
		ProgressionManager.debug_lastRefreshInventoryAttemptTime = unscaledTimeAsDouble;
		if (this._inventoryRefreshInFlight)
		{
			ProgressionManager.debug_refreshInventoryDroppedByThrottle++;
		}
		else if (unscaledTimeAsDouble - this._lastInventoryRefreshTime < 2.0)
		{
			ProgressionManager.debug_refreshInventoryDroppedByThrottle++;
		}
		else
		{
			this._lastInventoryRefreshTime = unscaledTimeAsDouble;
			this._inventoryRefreshInFlight = true;
			ProgressionManager.debug_refreshInventoryCount++;
			await ProgressionUtil.WaitForMothershipSessionToken();
			MothershipClientApiUnity.GetUserInventory(delegate(MothershipGetInventoryResponse response)
			{
				this._inventoryRefreshInFlight = false;
				this.OnGetInventory(response);
			}, delegate(MothershipError err, int code)
			{
				this._inventoryRefreshInFlight = false;
				ProgressionManager.GetMothershipFailure(err, code);
			});
			await ProgressionUtil.WaitForPlayFabSessionTicket();
			this.RefreshShinyRocksTotal();
		}
	}

	// Token: 0x06003F26 RID: 16166 RVA: 0x00153694 File Offset: 0x00151894
	public UserHydratedProgressionTreeResponse GetTree(string treeName)
	{
		UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse;
		this._trees.TryGetValue(treeName, out userHydratedProgressionTreeResponse);
		return userHydratedProgressionTreeResponse;
	}

	// Token: 0x06003F27 RID: 16167 RVA: 0x001536B1 File Offset: 0x001518B1
	public bool GetInventoryItem(string inventoryKey, out ProgressionManager.MothershipItemSummary item)
	{
		return this._inventory.TryGetValue((inventoryKey != null) ? inventoryKey.Trim() : null, out item);
	}

	// Token: 0x06003F28 RID: 16168 RVA: 0x001536CC File Offset: 0x001518CC
	public int GetNodeCost(string treeName, string nodeId, string currencyKey)
	{
		UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse;
		if (!this._trees.TryGetValue(treeName, out userHydratedProgressionTreeResponse) || userHydratedProgressionTreeResponse == null || string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(currencyKey))
		{
			return 0;
		}
		foreach (UserHydratedNodeDefinition userHydratedNodeDefinition in userHydratedProgressionTreeResponse.Nodes)
		{
			if (userHydratedNodeDefinition.id == nodeId && userHydratedNodeDefinition.cost != null && userHydratedNodeDefinition.cost.items != null)
			{
				using (HydratedInventoryChangeMap.HydratedInventoryChangeMapEnumerator enumerator2 = userHydratedNodeDefinition.cost.items.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<string, MothershipHydratedInventoryChange> keyValuePair = enumerator2.Current;
						string key = keyValuePair.Key;
						if (string.Equals((key != null) ? key.Trim() : null, currencyKey.Trim(), StringComparison.Ordinal))
						{
							return keyValuePair.Value.Delta;
						}
					}
					break;
				}
			}
		}
		return 0;
	}

	// Token: 0x06003F29 RID: 16169 RVA: 0x001537D0 File Offset: 0x001519D0
	public async void GetProgression(string trackId)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoGetProgression(new ProgressionManager.GetProgressionRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			TrackId = trackId
		}));
	}

	// Token: 0x06003F2A RID: 16170 RVA: 0x00153810 File Offset: 0x00151A10
	public async void SetProgression(string trackId, int progress)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoSetProgression(new ProgressionManager.SetProgressionRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			TrackId = trackId,
			Progress = progress
		}));
	}

	// Token: 0x06003F2B RID: 16171 RVA: 0x00153858 File Offset: 0x00151A58
	public async void UnlockNode(string treeId, string nodeId)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoUnlockNode(new ProgressionManager.UnlockNodeRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			TreeId = treeId,
			NodeId = nodeId
		}));
	}

	// Token: 0x06003F2C RID: 16172 RVA: 0x001538A0 File Offset: 0x00151AA0
	public async void IncrementSIResource(string resourceName, Action<string> OnSuccess = null, Action<string> OnFailure = null)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoIncrementSIResource(new ProgressionManager.IncrementSIResourceRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipToken = MothershipClientContext.Token,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			ResourceType = resourceName
		}, OnSuccess, OnFailure));
	}

	// Token: 0x06003F2D RID: 16173 RVA: 0x001538F0 File Offset: 0x00151AF0
	public async void CompleteSIQuest(int questID, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoQuestCompleteReward(new ProgressionManager.SetSIQuestCompleteRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipToken = MothershipClientContext.Token,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			QuestID = questID
		}, OnSuccess, OnFailure));
	}

	// Token: 0x06003F2E RID: 16174 RVA: 0x00153940 File Offset: 0x00151B40
	public async void CompleteSIBonus(Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoBonusCompleteReward(new ProgressionManager.SetSIBonusCompleteRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipToken = MothershipClientContext.Token,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId
		}, OnSuccess, OnFailure));
	}

	// Token: 0x06003F2F RID: 16175 RVA: 0x00153988 File Offset: 0x00151B88
	public async void CollectSIIdol(Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoIdolCollectReward(new ProgressionManager.SetSIIdolCollectRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipToken = MothershipClientContext.Token,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId
		}, OnSuccess, OnFailure));
	}

	// Token: 0x06003F30 RID: 16176 RVA: 0x001539D0 File Offset: 0x00151BD0
	public async void GetActiveSIQuests(Action<List<RotatingQuest>> OnSuccess = null, Action<string> OnFailure = null)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoGetActiveSIQuests(new ProgressionManager.GetActiveSIQuestsRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipToken = MothershipClientContext.Token,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId
		}, OnSuccess, OnFailure));
	}

	// Token: 0x06003F31 RID: 16177 RVA: 0x00153A18 File Offset: 0x00151C18
	public async void GetSIQuestStatus(Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoGetSIQuestsStatus(new ProgressionManager.GetSIQuestsStatusRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipToken = MothershipClientContext.Token,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId
		}, OnSuccess, OnFailure));
	}

	// Token: 0x06003F32 RID: 16178 RVA: 0x00153A60 File Offset: 0x00151C60
	public async void PurchaseTechPoints(int amount, Action OnSuccess = null, Action<string> OnFailure = null)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoPurchaseTechPoints(new ProgressionManager.PurchaseTechPointsRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipToken = MothershipClientContext.Token,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			TechPointsAmount = amount
		}, OnSuccess, OnFailure));
	}

	// Token: 0x06003F33 RID: 16179 RVA: 0x00153AB0 File Offset: 0x00151CB0
	public async void PurchaseResources(Action<ProgressionManager.UserInventory> OnSuccess = null, Action<string> OnFailure = null)
	{
		await ProgressionUtil.WaitForMothershipSessionToken();
		base.StartCoroutine(this.DoPurchaseResources(new ProgressionManager.PurchaseResourcesRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipToken = MothershipClientContext.Token,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId
		}, OnSuccess, OnFailure));
	}

	// Token: 0x06003F34 RID: 16180 RVA: 0x00153AF7 File Offset: 0x00151CF7
	public void PurchaseShiftCreditCapIncrease()
	{
		this.PurchaseShiftCreditCapIncreaseInternal(false);
	}

	// Token: 0x06003F35 RID: 16181 RVA: 0x00153B00 File Offset: 0x00151D00
	private void PurchaseShiftCreditCapIncreaseInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoPurchaseShiftCreditCapIncrease(new ProgressionManager.PurchaseShiftCreditCapIncreaseRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003F36 RID: 16182 RVA: 0x00153B52 File Offset: 0x00151D52
	public void PurchaseShiftCredit()
	{
		this.PurchaseShiftCreditInternal(false);
	}

	// Token: 0x06003F37 RID: 16183 RVA: 0x00153B5C File Offset: 0x00151D5C
	private void PurchaseShiftCreditInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoPurchaseShiftCredit(new ProgressionManager.PurchaseShiftCreditRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003F38 RID: 16184 RVA: 0x00153BB0 File Offset: 0x00151DB0
	public void GetShiftCredit(string mothershipId)
	{
		base.StartCoroutine(this.DoGetShiftCredit(new ProgressionManager.GetShiftCreditRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			TargetMothershipId = mothershipId
		}));
	}

	// Token: 0x06003F39 RID: 16185 RVA: 0x00153C02 File Offset: 0x00151E02
	public void GetJuicerStatus()
	{
		this.GetJuicerStatusInternal(false);
	}

	// Token: 0x06003F3A RID: 16186 RVA: 0x00153C0C File Offset: 0x00151E0C
	private void GetJuicerStatusInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoGetJuicerStatus(new ProgressionManager.GetJuicerStatusRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003F3B RID: 16187 RVA: 0x00153C5E File Offset: 0x00151E5E
	public void DepositCore(ProgressionManager.CoreType coreType)
	{
		this.DepositCoreInternal(coreType, false);
	}

	// Token: 0x06003F3C RID: 16188 RVA: 0x00153C68 File Offset: 0x00151E68
	private void DepositCoreInternal(ProgressionManager.CoreType coreType, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoDepositCore(new ProgressionManager.DepositCoreRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			CoreBeingDeposited = coreType,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003F3D RID: 16189 RVA: 0x00153CC1 File Offset: 0x00151EC1
	public void PurchaseOverdrive()
	{
		this.PurchaseOverdriveInternal(false);
	}

	// Token: 0x06003F3E RID: 16190 RVA: 0x00153CCC File Offset: 0x00151ECC
	private void PurchaseOverdriveInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoPurchaseOverdrive(new ProgressionManager.PurchaseOverdriveRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003F3F RID: 16191 RVA: 0x00153D1E File Offset: 0x00151F1E
	public void SubtractShiftCredit(int creditsToSubtract)
	{
		this.SubtractShiftCreditInternal(creditsToSubtract, false);
	}

	// Token: 0x06003F40 RID: 16192 RVA: 0x00153D28 File Offset: 0x00151F28
	private void SubtractShiftCreditInternal(int creditsToSubtract, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoSubtractShiftCredit(new ProgressionManager.SubtractShiftCreditRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ShiftCreditToRemove = creditsToSubtract,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003F41 RID: 16193 RVA: 0x00153D81 File Offset: 0x00151F81
	public void AdvanceDockWristUpgradeLevel(ProgressionManager.WristDockUpgradeType upgrade)
	{
		this.AdvanceDockWristUpgradeLevelInternal(upgrade, false);
	}

	// Token: 0x06003F42 RID: 16194 RVA: 0x00153D8C File Offset: 0x00151F8C
	private void AdvanceDockWristUpgradeLevelInternal(ProgressionManager.WristDockUpgradeType upgrade, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoAdvanceDockWristUpgradeLevel(new ProgressionManager.AdvanceDockWristUpgradeRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			Upgrade = upgrade,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003F43 RID: 16195 RVA: 0x00153DE5 File Offset: 0x00151FE5
	public void GetDockWristUpgradeStatus()
	{
		base.StartCoroutine(this.DoGetDockWristUpgradeStatus(new ProgressionManager.DockWristUpgradeStatusRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token
		}));
	}

	// Token: 0x06003F44 RID: 16196 RVA: 0x00153E28 File Offset: 0x00152028
	public void PurchaseDrillUpgrade(ProgressionManager.DrillUpgradeLevel upgrade)
	{
		base.StartCoroutine(this.DoPurchaseDrillUpgrade(new ProgressionManager.PurchaseDrillUpgradeRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			Upgrade = upgrade
		}));
	}

	// Token: 0x06003F45 RID: 16197 RVA: 0x00153E7C File Offset: 0x0015207C
	public void RecycleTool(GRTool.GRToolType toolBeingRecycled, int numberOfPlayers)
	{
		base.StartCoroutine(this.DoRecycleTool(new ProgressionManager.RecycleToolRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ToolBeingRecycled = toolBeingRecycled,
			NumberOfPlayers = numberOfPlayers
		}));
	}

	// Token: 0x06003F46 RID: 16198 RVA: 0x00153ED8 File Offset: 0x001520D8
	public void StartOfShift(string shiftId, int coresRequired, int numberOfPlayers, int depth)
	{
		base.StartCoroutine(this.DoStartOfShift(new ProgressionManager.StartOfShiftRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ShiftId = shiftId,
			CoresRequired = coresRequired,
			NumberOfPlayers = numberOfPlayers,
			Depth = depth
		}));
	}

	// Token: 0x06003F47 RID: 16199 RVA: 0x00153F40 File Offset: 0x00152140
	public void EndOfShiftReward(string shiftId)
	{
		this.EndOfShiftRewardInternal(shiftId, false);
	}

	// Token: 0x06003F48 RID: 16200 RVA: 0x00153F4C File Offset: 0x0015214C
	private void EndOfShiftRewardInternal(string shiftId, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoEndOfShiftReward(new ProgressionManager.EndOfShiftRewardRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ShiftId = shiftId,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003F49 RID: 16201 RVA: 0x00153FA5 File Offset: 0x001521A5
	public void GetGhostReactorStats()
	{
		base.StartCoroutine(this.DoGetGhostReactorStats(new ProgressionManager.GhostReactorStatsRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token
		}));
	}

	// Token: 0x06003F4A RID: 16202 RVA: 0x00153FE5 File Offset: 0x001521E5
	public void GetGhostReactorInventory()
	{
		base.StartCoroutine(this.DoGetGhostReactorInventory(new ProgressionManager.GhostReactorInventoryRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token
		}));
	}

	// Token: 0x06003F4B RID: 16203 RVA: 0x00154025 File Offset: 0x00152225
	public void SetGhostReactorInventory(string jsonInventory)
	{
		this.SetGhostReactorInventoryInternal(jsonInventory, false);
	}

	// Token: 0x06003F4C RID: 16204 RVA: 0x00154030 File Offset: 0x00152230
	private void SetGhostReactorInventoryInternal(string jsonInventory, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoSetGhostReactorInventory(new ProgressionManager.SetGhostReactorInventoryRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			InventoryJson = jsonInventory,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003F4D RID: 16205 RVA: 0x00154089 File Offset: 0x00152289
	private IEnumerator HandleWebRequestRetries<T>(ProgressionManager.RequestType requestType, T data, Action<T> actionToTake, Action failureActionToTake = null)
	{
		if (!this.retryCounters.ContainsKey(requestType))
		{
			this.retryCounters[requestType] = 0;
		}
		if (this.retryCounters[requestType] < this.maxRetriesOnFail)
		{
			float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.retryCounters[requestType] + 1)));
			Debug.LogWarning(string.Format("PM: Retrying ... attempt #{0}, waiting {1}s", this.retryCounters[requestType] + 1, num));
			Dictionary<ProgressionManager.RequestType, int> dictionary = this.retryCounters;
			int num2 = dictionary[requestType];
			dictionary[requestType] = num2 + 1;
			yield return new WaitForSecondsRealtime(num);
			actionToTake(data);
		}
		else
		{
			Debug.LogError("PM: Maximum retries attempted.");
			this.retryCounters[requestType] = 0;
			if (failureActionToTake != null)
			{
				failureActionToTake();
			}
		}
		yield break;
	}

	// Token: 0x06003F4E RID: 16206 RVA: 0x001540B8 File Offset: 0x001522B8
	private bool HandleWebRequestFailures(UnityWebRequest request, bool retryOnConflict = false)
	{
		bool flag = false;
		Debug.LogError(string.Format("PM: HandleWebRequestFailures Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
		if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			flag = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_006A;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_006A;
			}
			bool flag2 = true;
			goto IL_006C;
			IL_006A:
			flag2 = false;
			IL_006C:
			if (flag2 || (retryOnConflict && request.responseCode == 409L))
			{
				flag = true;
				Debug.LogError(string.Format("PM: HTTP {0} error: {1}", request.responseCode, request.error));
			}
		}
		return flag;
	}

	// Token: 0x06003F4F RID: 16207 RVA: 0x00154168 File Offset: 0x00152368
	private IEnumerator DoGetProgression(ProgressionManager.GetProgressionRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetProgressionRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetProgression);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			int num = int.Parse(request.downloadHandler.text);
			this._tracks[data.TrackId] = num;
			Debug.Log("PM: GetProgression Success: track is " + data.TrackId + " and progress is " + num.ToString());
			this.retryCounters[ProgressionManager.RequestType.GetProgression] = 0;
			Action<string, int> onTrackRead = this.OnTrackRead;
			if (onTrackRead != null)
			{
				onTrackRead(data.TrackId, num);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<string>(ProgressionManager.RequestType.GetProgression, data.TrackId, delegate(string x)
		{
			this.GetProgression(x);
		}, null);
		yield break;
	}

	// Token: 0x06003F50 RID: 16208 RVA: 0x0015417E File Offset: 0x0015237E
	private IEnumerator DoSetProgression(ProgressionManager.SetProgressionRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetProgressionRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.SetProgression);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.GetProgressionResponse getProgressionResponse = JsonConvert.DeserializeObject<ProgressionManager.GetProgressionResponse>(request.downloadHandler.text);
			this._tracks[data.TrackId] = getProgressionResponse.Progress;
			this.retryCounters[ProgressionManager.RequestType.SetProgression] = 0;
			Action<string, int> onTrackSet = this.OnTrackSet;
			if (onTrackSet != null)
			{
				onTrackSet(data.TrackId, getProgressionResponse.Progress);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ValueTuple<string, int>>(ProgressionManager.RequestType.SetProgression, new ValueTuple<string, int>(data.TrackId, data.Progress), delegate([TupleElementNames(new string[] { "TrackId", "Progress" })] ValueTuple<string, int> x)
		{
			this.SetProgression(x.Item1, x.Item2);
		}, null);
		yield break;
	}

	// Token: 0x06003F51 RID: 16209 RVA: 0x00154194 File Offset: 0x00152394
	private IEnumerator DoUnlockNode(ProgressionManager.UnlockNodeRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.UnlockNodeRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.UnlockProgressionTreeNode);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.UnlockProgressionTreeNode] = 0;
			this.RefreshProgressionTree();
			this.RefreshUserInventory();
			Action<string, string> onNodeUnlocked = this.OnNodeUnlocked;
			if (onNodeUnlocked != null)
			{
				onNodeUnlocked(data.TreeId, data.NodeId);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ValueTuple<string, string>>(ProgressionManager.RequestType.UnlockProgressionTreeNode, new ValueTuple<string, string>(data.TreeId, data.NodeId), delegate([TupleElementNames(new string[] { "TreeId", "NodeId" })] ValueTuple<string, string> x)
		{
			this.UnlockNode(x.Item1, x.Item2);
		}, null);
		yield break;
	}

	// Token: 0x06003F52 RID: 16210 RVA: 0x001541AA File Offset: 0x001523AA
	private IEnumerator DoIncrementSIResource(ProgressionManager.IncrementSIResourceRequest data, Action<string> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.IncrementSIResourceRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.IncrementSIResource);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.IncrementSIResourceResponse incrementSIResourceResponse = JsonConvert.DeserializeObject<ProgressionManager.IncrementSIResourceResponse>(request.downloadHandler.text);
			Action<string> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(incrementSIResourceResponse.ResourceType);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.IncrementSIResourceRequest>(ProgressionManager.RequestType.IncrementSIResource, data, delegate(ProgressionManager.IncrementSIResourceRequest x)
		{
			this.IncrementSIResource(data.ResourceType, OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003F53 RID: 16211 RVA: 0x001541CE File Offset: 0x001523CE
	private IEnumerator DoQuestCompleteReward(ProgressionManager.SetSIQuestCompleteRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetSIQuestCompleteRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.CompleteSIQuest);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetSIQuestCompleteRequest>(ProgressionManager.RequestType.CompleteSIQuest, data, delegate(ProgressionManager.SetSIQuestCompleteRequest x)
		{
			this.CompleteSIQuest(data.QuestID, OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003F54 RID: 16212 RVA: 0x001541F2 File Offset: 0x001523F2
	private IEnumerator DoBonusCompleteReward(ProgressionManager.SetSIBonusCompleteRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetSIBonusCompleteRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.CompleteSIBonus);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetSIBonusCompleteRequest>(ProgressionManager.RequestType.CompleteSIBonus, data, delegate(ProgressionManager.SetSIBonusCompleteRequest x)
		{
			this.CompleteSIBonus(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003F55 RID: 16213 RVA: 0x00154216 File Offset: 0x00152416
	private IEnumerator DoIdolCollectReward(ProgressionManager.SetSIIdolCollectRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetSIIdolCollectRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.CollectSIIdol);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetSIIdolCollectRequest>(ProgressionManager.RequestType.CollectSIIdol, data, delegate(ProgressionManager.SetSIIdolCollectRequest x)
		{
			this.CollectSIIdol(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003F56 RID: 16214 RVA: 0x0015423A File Offset: 0x0015243A
	private IEnumerator DoGetActiveSIQuests(ProgressionManager.GetActiveSIQuestsRequest data, Action<List<RotatingQuest>> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetActiveSIQuestsRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.GetActiveSIQuests);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetActiveSIQuestsResponse getActiveSIQuestsResponse = JsonConvert.DeserializeObject<ProgressionManager.GetActiveSIQuestsResponse>(request.downloadHandler.text);
			Action<List<RotatingQuest>> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(getActiveSIQuestsResponse.Result.Quests);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetActiveSIQuestsRequest>(ProgressionManager.RequestType.GetActiveSIQuests, data, delegate(ProgressionManager.GetActiveSIQuestsRequest x)
		{
			this.GetActiveSIQuests(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003F57 RID: 16215 RVA: 0x0015425E File Offset: 0x0015245E
	private IEnumerator DoGetSIQuestsStatus(ProgressionManager.GetSIQuestsStatusRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetSIQuestsStatusRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.GetSIQuestsStatus);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetSIQuestsStatusRequest>(ProgressionManager.RequestType.GetSIQuestsStatus, data, delegate(ProgressionManager.GetSIQuestsStatusRequest x)
		{
			this.GetSIQuestStatus(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003F58 RID: 16216 RVA: 0x00154282 File Offset: 0x00152482
	private IEnumerator DoPurchaseTechPoints(ProgressionManager.PurchaseTechPointsRequest data, Action OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseTechPointsRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.PurchaseTechPoints);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			Action onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess();
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseTechPointsRequest>(ProgressionManager.RequestType.PurchaseTechPoints, data, delegate(ProgressionManager.PurchaseTechPointsRequest x)
		{
			this.PurchaseTechPoints(data.TechPointsAmount, OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003F59 RID: 16217 RVA: 0x001542A6 File Offset: 0x001524A6
	private IEnumerator DoPurchaseResources(ProgressionManager.PurchaseResourcesRequest data, Action<ProgressionManager.UserInventory> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseResourcesRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.PurchaseResources);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.UserInventoryResponse userInventoryResponse = JsonConvert.DeserializeObject<ProgressionManager.UserInventoryResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserInventory> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(userInventoryResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseResourcesRequest>(ProgressionManager.RequestType.PurchaseResources, data, delegate(ProgressionManager.PurchaseResourcesRequest x)
		{
			this.PurchaseResources(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003F5A RID: 16218 RVA: 0x001542CA File Offset: 0x001524CA
	private IEnumerator DoPurchaseShiftCreditCapIncrease(ProgressionManager.PurchaseShiftCreditCapIncreaseRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseShiftCreditCapIncreaseRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.PurchaseShiftCreditCapIncreaseResponse purchaseShiftCreditCapIncreaseResponse = JsonConvert.DeserializeObject<ProgressionManager.PurchaseShiftCreditCapIncreaseResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease] = 0;
			this.RefreshShinyRocksTotal();
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData(purchaseShiftCreditCapIncreaseResponse.TargetMothershipId, purchaseShiftCreditCapIncreaseResponse.CurrentShiftCreditCapIncreases, purchaseShiftCreditCapIncreaseResponse.CurrentShiftCreditCapIncreasesMax);
			}
			Action<bool> onPurchaseShiftCreditCapIncrease = this.OnPurchaseShiftCreditCapIncrease;
			if (onPurchaseShiftCreditCapIncrease != null)
			{
				onPurchaseShiftCreditCapIncrease(true);
			}
			yield break;
		}
		if (request.responseCode == 400L && request.downloadHandler.text == "User Already Has Purchased Max Shift Credit Cap")
		{
			Action<bool> onPurchaseShiftCreditCapIncrease2 = this.OnPurchaseShiftCreditCapIncrease;
			if (onPurchaseShiftCreditCapIncrease2 != null)
			{
				onPurchaseShiftCreditCapIncrease2(false);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseShiftCreditCapIncreaseRequest>(ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease, data, delegate(ProgressionManager.PurchaseShiftCreditCapIncreaseRequest x)
		{
			this.PurchaseShiftCreditCapIncreaseInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003F5B RID: 16219 RVA: 0x001542E0 File Offset: 0x001524E0
	private IEnumerator DoPurchaseShiftCredit(ProgressionManager.PurchaseShiftCreditRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseShiftCreditRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseShiftCredit);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.PurchaseShiftCreditResponse purchaseShiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.PurchaseShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.PurchaseShiftCredit] = 0;
			this.RefreshShinyRocksTotal();
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit(purchaseShiftCreditResponse.TargetMothershipId, purchaseShiftCreditResponse.CurrentShiftCredits);
			}
			Action<bool> onPurchaseShiftCredit = this.OnPurchaseShiftCredit;
			if (onPurchaseShiftCredit != null)
			{
				onPurchaseShiftCredit(true);
			}
			GRPlayer local = GRPlayer.GetLocal();
			if (local != null)
			{
				local.SendCreditsRefilledTelemetry(100, purchaseShiftCreditResponse.CurrentShiftCredits);
			}
			yield break;
		}
		if (request.responseCode == 400L && request.downloadHandler.text == "User Already at Max Shift Credit")
		{
			Action<bool> onPurchaseShiftCredit2 = this.OnPurchaseShiftCredit;
			if (onPurchaseShiftCredit2 != null)
			{
				onPurchaseShiftCredit2(false);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseShiftCreditRequest>(ProgressionManager.RequestType.PurchaseShiftCredit, data, delegate(ProgressionManager.PurchaseShiftCreditRequest x)
		{
			this.PurchaseShiftCreditInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003F5C RID: 16220 RVA: 0x001542F6 File Offset: 0x001524F6
	private IEnumerator DoGetShiftCredit(ProgressionManager.GetShiftCreditRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetShiftCreditRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetShiftCredit);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetShiftCredit] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetShiftCreditRequest>(ProgressionManager.RequestType.GetShiftCredit, data, delegate(ProgressionManager.GetShiftCreditRequest x)
		{
			this.GetShiftCredit(x.TargetMothershipId);
		}, null);
		yield break;
	}

	// Token: 0x06003F5D RID: 16221 RVA: 0x0015430C File Offset: 0x0015250C
	private IEnumerator DoGetJuicerStatus(ProgressionManager.GetJuicerStatusRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetJuicerStatusRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetJuicerStatus);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.GetJuicerStatus] = 0;
			ProgressionManager.JuicerStatusResponse juicerStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.JuicerStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.JuicerStatusResponse> onJucierStatusUpdated = this.OnJucierStatusUpdated;
			if (onJucierStatusUpdated != null)
			{
				onJucierStatusUpdated(juicerStatusResponse);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetJuicerStatusRequest>(ProgressionManager.RequestType.GetJuicerStatus, data, delegate(ProgressionManager.GetJuicerStatusRequest x)
		{
			this.GetJuicerStatusInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003F5E RID: 16222 RVA: 0x00154322 File Offset: 0x00152522
	private IEnumerator DoDepositCore(ProgressionManager.DepositCoreRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.DepositCoreRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.DepositCore);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.DepositCore] = 0;
			if (data.CoreBeingDeposited == ProgressionManager.CoreType.ChaosSeed)
			{
				Action<bool> onChaosDepositSuccess = this.OnChaosDepositSuccess;
				if (onChaosDepositSuccess != null)
				{
					onChaosDepositSuccess(true);
				}
				this.GetJuicerStatus();
			}
			else
			{
				ProgressionManager.DepositCoreResponse depositCoreResponse = JsonConvert.DeserializeObject<ProgressionManager.DepositCoreResponse>(request.downloadHandler.text);
				Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
				if (onGetShiftCredit != null)
				{
					onGetShiftCredit(data.MothershipId, depositCoreResponse.CurrentShiftCredits);
				}
			}
			yield break;
		}
		if (request.responseCode == 400L && request.downloadHandler.text == "DepositGRCore already at seed cap")
		{
			if (data.CoreBeingDeposited == ProgressionManager.CoreType.ChaosSeed)
			{
				Action<bool> onChaosDepositSuccess2 = this.OnChaosDepositSuccess;
				if (onChaosDepositSuccess2 != null)
				{
					onChaosDepositSuccess2(false);
				}
				this.GetJuicerStatus();
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.DepositCoreRequest>(ProgressionManager.RequestType.DepositCore, data, delegate(ProgressionManager.DepositCoreRequest x)
		{
			this.DepositCoreInternal(x.CoreBeingDeposited, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003F5F RID: 16223 RVA: 0x00154338 File Offset: 0x00152538
	private IEnumerator DoPurchaseOverdrive(ProgressionManager.PurchaseOverdriveRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseOverdriveRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseOverdrive);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.PurchaseOverdrive] = 0;
			this.GetJuicerStatus();
			this.RefreshShinyRocksTotal();
			Action<bool> onPurchaseOverdrive = this.OnPurchaseOverdrive;
			if (onPurchaseOverdrive != null)
			{
				onPurchaseOverdrive(true);
			}
			yield break;
		}
		if (request.responseCode == 400L && (request.downloadHandler.text == "User Already At Overdrive Cap" || request.downloadHandler.text == "User would exceed Overdrive Cap"))
		{
			Action<bool> onPurchaseOverdrive2 = this.OnPurchaseOverdrive;
			if (onPurchaseOverdrive2 != null)
			{
				onPurchaseOverdrive2(false);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseOverdriveRequest>(ProgressionManager.RequestType.PurchaseOverdrive, data, delegate(ProgressionManager.PurchaseOverdriveRequest x)
		{
			this.PurchaseOverdriveInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003F60 RID: 16224 RVA: 0x0015434E File Offset: 0x0015254E
	private IEnumerator DoSubtractShiftCredit(ProgressionManager.SubtractShiftCreditRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SubtractShiftCreditRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.SubtractShiftCredit);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.SubtractShiftCredit] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit(data.MothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SubtractShiftCreditRequest>(ProgressionManager.RequestType.SubtractShiftCredit, data, delegate(ProgressionManager.SubtractShiftCreditRequest x)
		{
			this.SubtractShiftCreditInternal(data.ShiftCreditToRemove, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003F61 RID: 16225 RVA: 0x00154364 File Offset: 0x00152564
	private IEnumerator DoAdvanceDockWristUpgradeLevel(ProgressionManager.AdvanceDockWristUpgradeRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.AdvanceDockWristUpgradeRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.AdvanceDockWristUpgrade);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.DockWristStatusResponse dockWristStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.DockWristStatusResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.AdvanceDockWristUpgrade] = 0;
			Action<ProgressionManager.DockWristStatusResponse> onDockWristStatusUpdated = this.OnDockWristStatusUpdated;
			if (onDockWristStatusUpdated != null)
			{
				onDockWristStatusUpdated(dockWristStatusResponse);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.AdvanceDockWristUpgradeRequest>(ProgressionManager.RequestType.AdvanceDockWristUpgrade, data, delegate(ProgressionManager.AdvanceDockWristUpgradeRequest x)
		{
			this.AdvanceDockWristUpgradeLevelInternal(data.Upgrade, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003F62 RID: 16226 RVA: 0x0015437A File Offset: 0x0015257A
	private IEnumerator DoGetDockWristUpgradeStatus(ProgressionManager.DockWristUpgradeStatusRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.DockWristUpgradeStatusRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetDockWristUpgradeStatus);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.DockWristStatusResponse dockWristStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.DockWristStatusResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetDockWristUpgradeStatus] = 0;
			Action<ProgressionManager.DockWristStatusResponse> onDockWristStatusUpdated = this.OnDockWristStatusUpdated;
			if (onDockWristStatusUpdated != null)
			{
				onDockWristStatusUpdated(dockWristStatusResponse);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.DockWristUpgradeStatusRequest>(ProgressionManager.RequestType.GetDockWristUpgradeStatus, data, delegate(ProgressionManager.DockWristUpgradeStatusRequest x)
		{
			this.GetDockWristUpgradeStatus();
		}, null);
		yield break;
	}

	// Token: 0x06003F63 RID: 16227 RVA: 0x00154390 File Offset: 0x00152590
	private IEnumerator DoPurchaseDrillUpgrade(ProgressionManager.PurchaseDrillUpgradeRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseDrillUpgradeRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseDrillUpgrade);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.PurchaseDrillUpgrade] = 0;
			this.RefreshUserInventory();
			Action<string, string> onNodeUnlocked = this.OnNodeUnlocked;
			if (onNodeUnlocked != null)
			{
				onNodeUnlocked("", "");
			}
			if (data.Upgrade == ProgressionManager.DrillUpgradeLevel.Base)
			{
				GRPlayer local = GRPlayer.GetLocal();
				if (local != null)
				{
					local.SendPodUpgradeTelemetry(ProgressionManager.DrillUpgradeLevel.Base.ToString(), 0, 2500, 0);
				}
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseDrillUpgradeRequest>(ProgressionManager.RequestType.PurchaseDrillUpgrade, data, delegate(ProgressionManager.PurchaseDrillUpgradeRequest x)
		{
			this.PurchaseDrillUpgrade(data.Upgrade);
		}, null);
		yield break;
	}

	// Token: 0x06003F64 RID: 16228 RVA: 0x001543A6 File Offset: 0x001525A6
	private IEnumerator DoRecycleTool(ProgressionManager.RecycleToolRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.RecycleToolRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.RecycleTool);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.RecycleTool] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit(data.MothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.RecycleToolRequest>(ProgressionManager.RequestType.RecycleTool, data, delegate(ProgressionManager.RecycleToolRequest x)
		{
			this.RecycleTool(data.ToolBeingRecycled, data.NumberOfPlayers);
		}, null);
		yield break;
	}

	// Token: 0x06003F65 RID: 16229 RVA: 0x001543BC File Offset: 0x001525BC
	private IEnumerator DoStartOfShift(ProgressionManager.StartOfShiftRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.StartOfShiftRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.StartOfShift);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.StartOfShift] = 0;
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.StartOfShiftRequest>(ProgressionManager.RequestType.StartOfShift, data, delegate(ProgressionManager.StartOfShiftRequest x)
		{
			this.StartOfShift(data.ShiftId, data.CoresRequired, data.NumberOfPlayers, data.Depth);
		}, null);
		yield break;
	}

	// Token: 0x06003F66 RID: 16230 RVA: 0x001543D2 File Offset: 0x001525D2
	private IEnumerator DoEndOfShiftReward(ProgressionManager.EndOfShiftRewardRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.EndOfShiftRewardRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.EndOfShiftReward);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.EndOfShiftReward] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit(data.MothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (request.responseCode == 400L && request.error == "EndOfShiftReward Unknown Shift or Mothership Failure.")
		{
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.EndOfShiftRewardRequest>(ProgressionManager.RequestType.EndOfShiftReward, data, delegate(ProgressionManager.EndOfShiftRewardRequest x)
		{
			this.EndOfShiftRewardInternal(data.ShiftId, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003F67 RID: 16231 RVA: 0x001543E8 File Offset: 0x001525E8
	private IEnumerator DoGetGhostReactorStats(ProgressionManager.GhostReactorStatsRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GhostReactorStatsRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetGhostReactorStats);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.GhostReactorStatsResponse ghostReactorStatsResponse = JsonConvert.DeserializeObject<ProgressionManager.GhostReactorStatsResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetGhostReactorStats] = 0;
			Action<ProgressionManager.GhostReactorStatsResponse> onGhostReactorStatsUpdated = this.OnGhostReactorStatsUpdated;
			if (onGhostReactorStatsUpdated != null)
			{
				onGhostReactorStatsUpdated(ghostReactorStatsResponse);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GhostReactorStatsRequest>(ProgressionManager.RequestType.GetGhostReactorStats, data, delegate(ProgressionManager.GhostReactorStatsRequest x)
		{
			this.GetGhostReactorStats();
		}, null);
		yield break;
	}

	// Token: 0x06003F68 RID: 16232 RVA: 0x001543FE File Offset: 0x001525FE
	private IEnumerator DoGetGhostReactorInventory(ProgressionManager.GhostReactorInventoryRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GhostReactorInventoryRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetGhostReactorInventory);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.GhostReactorInventoryResponse ghostReactorInventoryResponse = JsonConvert.DeserializeObject<ProgressionManager.GhostReactorInventoryResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetGhostReactorInventory] = 0;
			Action<ProgressionManager.GhostReactorInventoryResponse> onGhostReactorInventoryUpdated = this.OnGhostReactorInventoryUpdated;
			if (onGhostReactorInventoryUpdated != null)
			{
				onGhostReactorInventoryUpdated(ghostReactorInventoryResponse);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GhostReactorInventoryRequest>(ProgressionManager.RequestType.GetGhostReactorInventory, data, delegate(ProgressionManager.GhostReactorInventoryRequest x)
		{
			this.GetGhostReactorInventory();
		}, null);
		yield break;
	}

	// Token: 0x06003F69 RID: 16233 RVA: 0x00154414 File Offset: 0x00152614
	private IEnumerator DoSetGhostReactorInventory(ProgressionManager.SetGhostReactorInventoryRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetGhostReactorInventoryRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.SetGhostReactorInventory);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.SetGhostReactorInventory] = 0;
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetGhostReactorInventoryRequest>(ProgressionManager.RequestType.SetGhostReactorInventory, data, delegate(ProgressionManager.SetGhostReactorInventoryRequest x)
		{
			this.SetGhostReactorInventoryInternal(data.InventoryJson, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003F6A RID: 16234 RVA: 0x0015442A File Offset: 0x0015262A
	private bool IsSuccessResponse(long code)
	{
		return code >= 200L && code < 300L;
	}

	// Token: 0x06003F6B RID: 16235 RVA: 0x00154440 File Offset: 0x00152640
	private UnityWebRequest FormatWebRequest<T>(string url, T pendingRequest, ProgressionManager.RequestType type)
	{
		string text = "";
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(pendingRequest));
		switch (type)
		{
		case ProgressionManager.RequestType.GetProgression:
			text = "/api/GetProgression";
			break;
		case ProgressionManager.RequestType.SetProgression:
			text = "/api/SetProgression";
			break;
		case ProgressionManager.RequestType.UnlockProgressionTreeNode:
			text = "/api/UnlockProgressionTreeNode";
			break;
		case ProgressionManager.RequestType.IncrementSIResource:
			text = "/api/IncrementSIResource";
			break;
		case ProgressionManager.RequestType.CompleteSIQuest:
			text = "/api/SetSIQuestComplete";
			break;
		case ProgressionManager.RequestType.CompleteSIBonus:
			text = "/api/SetSIBonusComplete";
			break;
		case ProgressionManager.RequestType.CollectSIIdol:
			text = "/api/SetSIIdolCollect";
			break;
		case ProgressionManager.RequestType.GetActiveSIQuests:
			text = "/api/GetActiveSIQuests";
			break;
		case ProgressionManager.RequestType.GetSIQuestsStatus:
			text = "/api/GetSIQuestsStatus";
			break;
		case ProgressionManager.RequestType.ResetSIQuestsStatus:
			text = "/api/ResetSIQuestsStatus";
			break;
		case ProgressionManager.RequestType.PurchaseTechPoints:
			text = "/api/PurchaseTechPoints";
			break;
		case ProgressionManager.RequestType.PurchaseResources:
			text = "/api/PurchaseResources";
			break;
		case ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease:
			text = "/api/PurchaseShiftCreditCapIncrease";
			break;
		case ProgressionManager.RequestType.PurchaseShiftCredit:
			text = "/api/PurchaseShiftCredit";
			break;
		case ProgressionManager.RequestType.GetJuicerStatus:
			text = "/api/GetJuicerStatus";
			break;
		case ProgressionManager.RequestType.DepositCore:
			text = "/api/DepositGRCore";
			break;
		case ProgressionManager.RequestType.PurchaseOverdrive:
			text = "/api/PurchaseOverdrive";
			break;
		case ProgressionManager.RequestType.GetShiftCredit:
			text = "/api/GetShiftCredit";
			break;
		case ProgressionManager.RequestType.SubtractShiftCredit:
			text = "/api/SubtractShiftCredit";
			break;
		case ProgressionManager.RequestType.AdvanceDockWristUpgrade:
			text = "/api/AdvanceDockWristUpgrade";
			break;
		case ProgressionManager.RequestType.GetDockWristUpgradeStatus:
			text = "/api/GetDockWristUpgradeStatus";
			break;
		case ProgressionManager.RequestType.PurchaseDrillUpgrade:
			text = "/api/PurchaseDrillUpgrade";
			break;
		case ProgressionManager.RequestType.RecycleTool:
			text = "/api/RecycleTool";
			break;
		case ProgressionManager.RequestType.StartOfShift:
			text = "/api/StartOfShift";
			break;
		case ProgressionManager.RequestType.EndOfShiftReward:
			text = "/api/EndOfShiftReward";
			break;
		case ProgressionManager.RequestType.GetGhostReactorStats:
			text = "/api/GetGhostReactorStats";
			break;
		case ProgressionManager.RequestType.GetGhostReactorInventory:
			text = "/api/GetGhostReactorInventory";
			break;
		case ProgressionManager.RequestType.SetGhostReactorInventory:
			text = "/api/SetGhostReactorInventory";
			break;
		}
		UnityWebRequest unityWebRequest = new UnityWebRequest(url + text, "POST");
		unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
		unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
		unityWebRequest.SetRequestHeader("Content-Type", "application/json");
		return unityWebRequest;
	}

	// Token: 0x06003F6C RID: 16236 RVA: 0x00154620 File Offset: 0x00152820
	private void OnGetTrees(GetProgressionTreesForPlayerResponse response)
	{
		if (((response != null) ? response.Results : null) == null)
		{
			return;
		}
		this._trees.Clear();
		foreach (UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse in response.Results)
		{
			UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse2 = new UserHydratedProgressionTreeResponse();
			userHydratedProgressionTreeResponse2.Tree = userHydratedProgressionTreeResponse.Tree;
			userHydratedProgressionTreeResponse2.Track = userHydratedProgressionTreeResponse.Track;
			userHydratedProgressionTreeResponse2.Nodes = userHydratedProgressionTreeResponse.Nodes;
			this._trees[userHydratedProgressionTreeResponse.Tree.name] = userHydratedProgressionTreeResponse2;
		}
		Action onTreeUpdated = this.OnTreeUpdated;
		if (onTreeUpdated == null)
		{
			return;
		}
		onTreeUpdated();
	}

	// Token: 0x06003F6D RID: 16237 RVA: 0x001546D4 File Offset: 0x001528D4
	private void OnGetInventory(MothershipGetInventoryResponse response)
	{
		if (((response != null) ? response.Results : null) == null)
		{
			return;
		}
		this._inventory.Clear();
		foreach (KeyValuePair<string, MothershipPlayerInventorySummary> keyValuePair in response.Results)
		{
			MothershipPlayerInventorySummary value = keyValuePair.Value;
			if (((value != null) ? value.entitlements : null) != null)
			{
				foreach (MothershipInventoryItemSummary mothershipInventoryItemSummary in keyValuePair.Value.entitlements)
				{
					string name = mothershipInventoryItemSummary.name;
					string text = ((name != null) ? name.Trim() : null);
					this._inventory[text] = new ProgressionManager.MothershipItemSummary
					{
						EntitlementId = mothershipInventoryItemSummary.entitlement_id,
						InGameId = mothershipInventoryItemSummary.in_game_id,
						Name = mothershipInventoryItemSummary.name,
						Quantity = mothershipInventoryItemSummary.quantity
					};
				}
			}
		}
		Action onInventoryUpdated = this.OnInventoryUpdated;
		if (onInventoryUpdated == null)
		{
			return;
		}
		onInventoryUpdated();
	}

	// Token: 0x06003F6E RID: 16238 RVA: 0x001547FC File Offset: 0x001529FC
	public int GetShinyRocksTotal()
	{
		if (CosmeticsController.instance != null)
		{
			return CosmeticsController.instance.CurrencyBalance;
		}
		return 0;
	}

	// Token: 0x06003F6F RID: 16239 RVA: 0x0015481B File Offset: 0x00152A1B
	public void RefreshShinyRocksTotal()
	{
		if (CosmeticsController.instance != null)
		{
			CosmeticsController.instance.GetCurrencyBalance();
		}
	}

	// Token: 0x06003F70 RID: 16240 RVA: 0x00154838 File Offset: 0x00152A38
	public static void GetMothershipFailure(MothershipError callError, int errorCode)
	{
		Debug.LogError("Progression: GetMothershipFailure: " + callError.MothershipErrorCode + ":" + callError.Message);
	}

	// Token: 0x04004F61 RID: 20321
	private readonly Dictionary<string, UserHydratedProgressionTreeResponse> _trees = new Dictionary<string, UserHydratedProgressionTreeResponse>();

	// Token: 0x04004F62 RID: 20322
	private readonly Dictionary<string, ProgressionManager.MothershipItemSummary> _inventory = new Dictionary<string, ProgressionManager.MothershipItemSummary>();

	// Token: 0x04004F63 RID: 20323
	private readonly Dictionary<string, int> _tracks = new Dictionary<string, int>();

	// Token: 0x04004F64 RID: 20324
	private Dictionary<ProgressionManager.RequestType, int> retryCounters = new Dictionary<ProgressionManager.RequestType, int>();

	// Token: 0x04004F65 RID: 20325
	private int maxRetriesOnFail = 4;

	// Token: 0x04004F66 RID: 20326
	private const double k_minRefreshIntervalSeconds = 2.0;

	// Token: 0x04004F67 RID: 20327
	private double _lastTreeRefreshTime = double.NegativeInfinity;

	// Token: 0x04004F68 RID: 20328
	private double _lastInventoryRefreshTime = double.NegativeInfinity;

	// Token: 0x04004F69 RID: 20329
	private bool _treeRefreshInFlight;

	// Token: 0x04004F6A RID: 20330
	private bool _inventoryRefreshInFlight;

	// Token: 0x04004F6B RID: 20331
	public static int debug_refreshTreeCount;

	// Token: 0x04004F6C RID: 20332
	public static int debug_refreshInventoryCount;

	// Token: 0x04004F6D RID: 20333
	public static int debug_refreshTreeDroppedByThrottle;

	// Token: 0x04004F6E RID: 20334
	public static int debug_refreshInventoryDroppedByThrottle;

	// Token: 0x04004F6F RID: 20335
	public static double debug_lastRefreshTreeAttemptTime;

	// Token: 0x04004F70 RID: 20336
	public static double debug_lastRefreshInventoryAttemptTime;

	// Token: 0x02000960 RID: 2400
	public struct MothershipItemSummary
	{
		// Token: 0x04004F71 RID: 20337
		public string Name;

		// Token: 0x04004F72 RID: 20338
		public string EntitlementId;

		// Token: 0x04004F73 RID: 20339
		public string InGameId;

		// Token: 0x04004F74 RID: 20340
		public int Quantity;
	}

	// Token: 0x02000961 RID: 2401
	private enum RequestType
	{
		// Token: 0x04004F76 RID: 20342
		GetProgression,
		// Token: 0x04004F77 RID: 20343
		SetProgression,
		// Token: 0x04004F78 RID: 20344
		UnlockProgressionTreeNode,
		// Token: 0x04004F79 RID: 20345
		IncrementSIResource,
		// Token: 0x04004F7A RID: 20346
		CompleteSIQuest,
		// Token: 0x04004F7B RID: 20347
		CompleteSIBonus,
		// Token: 0x04004F7C RID: 20348
		CollectSIIdol,
		// Token: 0x04004F7D RID: 20349
		GetActiveSIQuests,
		// Token: 0x04004F7E RID: 20350
		GetSIQuestsStatus,
		// Token: 0x04004F7F RID: 20351
		ResetSIQuestsStatus,
		// Token: 0x04004F80 RID: 20352
		PurchaseTechPoints,
		// Token: 0x04004F81 RID: 20353
		PurchaseResources,
		// Token: 0x04004F82 RID: 20354
		PurchaseShiftCreditCapIncrease,
		// Token: 0x04004F83 RID: 20355
		PurchaseShiftCredit,
		// Token: 0x04004F84 RID: 20356
		RegisterToGRShift,
		// Token: 0x04004F85 RID: 20357
		GetJuicerStatus,
		// Token: 0x04004F86 RID: 20358
		DepositCore,
		// Token: 0x04004F87 RID: 20359
		PurchaseOverdrive,
		// Token: 0x04004F88 RID: 20360
		GetShiftCredit,
		// Token: 0x04004F89 RID: 20361
		SubtractShiftCredit,
		// Token: 0x04004F8A RID: 20362
		AdvanceDockWristUpgrade,
		// Token: 0x04004F8B RID: 20363
		GetDockWristUpgradeStatus,
		// Token: 0x04004F8C RID: 20364
		PurchaseDrillUpgrade,
		// Token: 0x04004F8D RID: 20365
		RecycleTool,
		// Token: 0x04004F8E RID: 20366
		StartOfShift,
		// Token: 0x04004F8F RID: 20367
		EndOfShiftReward,
		// Token: 0x04004F90 RID: 20368
		GetGhostReactorStats,
		// Token: 0x04004F91 RID: 20369
		GetGhostReactorInventory,
		// Token: 0x04004F92 RID: 20370
		SetGhostReactorInventory
	}

	// Token: 0x02000962 RID: 2402
	public enum WristDockUpgradeType
	{
		// Token: 0x04004F94 RID: 20372
		None,
		// Token: 0x04004F95 RID: 20373
		Upgrade1,
		// Token: 0x04004F96 RID: 20374
		Upgrade2,
		// Token: 0x04004F97 RID: 20375
		Upgrade3
	}

	// Token: 0x02000963 RID: 2403
	public enum DrillUpgradeLevel
	{
		// Token: 0x04004F99 RID: 20377
		None,
		// Token: 0x04004F9A RID: 20378
		Base,
		// Token: 0x04004F9B RID: 20379
		Upgrade1,
		// Token: 0x04004F9C RID: 20380
		Upgrade2,
		// Token: 0x04004F9D RID: 20381
		Upgrade3
	}

	// Token: 0x02000964 RID: 2404
	public enum CoreType
	{
		// Token: 0x04004F9F RID: 20383
		None,
		// Token: 0x04004FA0 RID: 20384
		Core,
		// Token: 0x04004FA1 RID: 20385
		SuperCore,
		// Token: 0x04004FA2 RID: 20386
		ChaosSeed
	}

	// Token: 0x02000965 RID: 2405
	[Serializable]
	private class GetProgressionRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004FA3 RID: 20387
		public string TrackId;
	}

	// Token: 0x02000966 RID: 2406
	[Serializable]
	private class GetProgressionResponse
	{
		// Token: 0x04004FA4 RID: 20388
		public string Track;

		// Token: 0x04004FA5 RID: 20389
		public int Progress;

		// Token: 0x04004FA6 RID: 20390
		public int StatusCode;

		// Token: 0x04004FA7 RID: 20391
		public string Error;
	}

	// Token: 0x02000967 RID: 2407
	[Serializable]
	private class SetProgressionRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004FA8 RID: 20392
		public string TrackId;

		// Token: 0x04004FA9 RID: 20393
		public int Progress;
	}

	// Token: 0x02000968 RID: 2408
	[Serializable]
	private class SetProgressionResponse
	{
		// Token: 0x04004FAA RID: 20394
		public string Track;

		// Token: 0x04004FAB RID: 20395
		public int Progress;

		// Token: 0x04004FAC RID: 20396
		public int StatusCode;

		// Token: 0x04004FAD RID: 20397
		public string Error;
	}

	// Token: 0x02000969 RID: 2409
	[Serializable]
	private class UnlockNodeRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004FAE RID: 20398
		public string TreeId;

		// Token: 0x04004FAF RID: 20399
		public string NodeId;
	}

	// Token: 0x0200096A RID: 2410
	[Serializable]
	private class UnlockNodeResponse
	{
		// Token: 0x04004FB0 RID: 20400
		public UserHydratedProgressionTreeResponse Tree;

		// Token: 0x04004FB1 RID: 20401
		public int StatusCode;

		// Token: 0x04004FB2 RID: 20402
		public string Error;
	}

	// Token: 0x0200096B RID: 2411
	[Serializable]
	private class IncrementSIResourceRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004FB3 RID: 20403
		public string ResourceType;
	}

	// Token: 0x0200096C RID: 2412
	[Serializable]
	private class IncrementSIResourceResponse : ProgressionManager.UserInventoryResponse
	{
		// Token: 0x04004FB4 RID: 20404
		public string ResourceType;
	}

	// Token: 0x0200096D RID: 2413
	[Serializable]
	private class GetActiveSIQuestsRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x0200096E RID: 2414
	[Serializable]
	private class GetActiveSIQuestsResponse
	{
		// Token: 0x04004FB5 RID: 20405
		public ProgressionManager.GetActiveSIQuestsResult Result;

		// Token: 0x04004FB6 RID: 20406
		public int StatusCode;

		// Token: 0x04004FB7 RID: 20407
		public string Error;
	}

	// Token: 0x0200096F RID: 2415
	[Serializable]
	public class GetActiveSIQuestsResult
	{
		// Token: 0x04004FB8 RID: 20408
		public List<RotatingQuest> Quests;
	}

	// Token: 0x02000970 RID: 2416
	[Serializable]
	private class GetSIQuestsStatusRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000971 RID: 2417
	[Serializable]
	private class ResetSIQuestsStatusRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000972 RID: 2418
	[Serializable]
	private class PurchaseTechPointsRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004FB9 RID: 20409
		public int TechPointsAmount;
	}

	// Token: 0x02000973 RID: 2419
	private class PurchaseResourcesRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000974 RID: 2420
	[Serializable]
	private class GetSIQuestsStatusResponse
	{
		// Token: 0x04004FBA RID: 20410
		public ProgressionManager.UserQuestsStatusResponse Result;
	}

	// Token: 0x02000975 RID: 2421
	[Serializable]
	private class UserInventoryResponse
	{
		// Token: 0x04004FBB RID: 20411
		public ProgressionManager.UserInventory Result;
	}

	// Token: 0x02000976 RID: 2422
	[Serializable]
	public class UserInventory
	{
		// Token: 0x04004FBC RID: 20412
		public Dictionary<string, int> Inventory;
	}

	// Token: 0x02000977 RID: 2423
	[Serializable]
	private class SetSIQuestCompleteRequest : ProgressionManager.RewardRequest
	{
		// Token: 0x04004FBD RID: 20413
		public int QuestID;
	}

	// Token: 0x02000978 RID: 2424
	[Serializable]
	private class SetSIBonusCompleteRequest : ProgressionManager.RewardRequest
	{
	}

	// Token: 0x02000979 RID: 2425
	[Serializable]
	private class SetSIIdolCollectRequest : ProgressionManager.RewardRequest
	{
	}

	// Token: 0x0200097A RID: 2426
	[Serializable]
	private class RewardRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x0200097B RID: 2427
	[Serializable]
	private class MothershipRequest
	{
		// Token: 0x04004FBE RID: 20414
		public string MothershipId;

		// Token: 0x04004FBF RID: 20415
		public string MothershipToken;

		// Token: 0x04004FC0 RID: 20416
		public string MothershipEnvId;

		// Token: 0x04004FC1 RID: 20417
		public string MothershipDeploymentId;
	}

	// Token: 0x0200097C RID: 2428
	[Serializable]
	private class MothershipUserDataWriteRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004FC2 RID: 20418
		public bool SkipUserDataCache;
	}

	// Token: 0x0200097D RID: 2429
	[Serializable]
	public class UserQuestsStatusResponse
	{
		// Token: 0x04004FC3 RID: 20419
		public int TodayClaimableQuests;

		// Token: 0x04004FC4 RID: 20420
		public int TodayClaimableBonus;

		// Token: 0x04004FC5 RID: 20421
		public int TodayClaimableIdol;
	}

	// Token: 0x0200097E RID: 2430
	[Serializable]
	private class PurchaseShiftCreditCapIncreaseRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x0200097F RID: 2431
	[Serializable]
	private class PurchaseShiftCreditCapIncreaseResponse
	{
		// Token: 0x04004FC6 RID: 20422
		public int StatusCode;

		// Token: 0x04004FC7 RID: 20423
		public string Error;

		// Token: 0x04004FC8 RID: 20424
		public int CurrentShiftCreditCapIncreases;

		// Token: 0x04004FC9 RID: 20425
		public int CurrentShiftCreditCapIncreasesMax;

		// Token: 0x04004FCA RID: 20426
		public string TargetMothershipId;
	}

	// Token: 0x02000980 RID: 2432
	[Serializable]
	private class PurchaseShiftCreditRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x02000981 RID: 2433
	[Serializable]
	private class PurchaseShiftCreditResponse
	{
		// Token: 0x04004FCB RID: 20427
		public int StatusCode;

		// Token: 0x04004FCC RID: 20428
		public string Error;

		// Token: 0x04004FCD RID: 20429
		public int CurrentShiftCredits;

		// Token: 0x04004FCE RID: 20430
		public string TargetMothershipId;
	}

	// Token: 0x02000982 RID: 2434
	[Serializable]
	private class GetShiftCreditRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004FCF RID: 20431
		public string TargetMothershipId;
	}

	// Token: 0x02000983 RID: 2435
	[Serializable]
	public class ShiftCreditResponse
	{
		// Token: 0x04004FD0 RID: 20432
		public int StatusCode;

		// Token: 0x04004FD1 RID: 20433
		public string Error;

		// Token: 0x04004FD2 RID: 20434
		public int CurrentShiftCredits;

		// Token: 0x04004FD3 RID: 20435
		public int CurrentShiftCreditCapIncreases;

		// Token: 0x04004FD4 RID: 20436
		public int CurrentShiftCreditCapIncreasesMax;

		// Token: 0x04004FD5 RID: 20437
		public string TargetMothershipId;
	}

	// Token: 0x02000984 RID: 2436
	[Serializable]
	private class GetJuicerStatusRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x02000985 RID: 2437
	[Serializable]
	private class DepositCoreRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004FD6 RID: 20438
		public ProgressionManager.CoreType CoreBeingDeposited;
	}

	// Token: 0x02000986 RID: 2438
	[Serializable]
	private class DepositCoreResponse
	{
		// Token: 0x04004FD7 RID: 20439
		public int StatusCode;

		// Token: 0x04004FD8 RID: 20440
		public string Error;

		// Token: 0x04004FD9 RID: 20441
		public int CurrentShiftCredits;
	}

	// Token: 0x02000987 RID: 2439
	[Serializable]
	private class PurchaseOverdriveRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x02000988 RID: 2440
	[Serializable]
	public class JuicerStatusResponse
	{
		// Token: 0x04004FDA RID: 20442
		public string MothershipId;

		// Token: 0x04004FDB RID: 20443
		public int StatusCode;

		// Token: 0x04004FDC RID: 20444
		public string Error;

		// Token: 0x04004FDD RID: 20445
		public int CurrentCoreCount;

		// Token: 0x04004FDE RID: 20446
		public int CoreProcessingTimeSec;

		// Token: 0x04004FDF RID: 20447
		public float CoreProcessingPercent;

		// Token: 0x04004FE0 RID: 20448
		public int OverdriveSupply;

		// Token: 0x04004FE1 RID: 20449
		public int OverdriveCap;

		// Token: 0x04004FE2 RID: 20450
		public int CoresProcessedByOverdrive;

		// Token: 0x04004FE3 RID: 20451
		public bool RefreshJuice;
	}

	// Token: 0x02000989 RID: 2441
	[Serializable]
	private class SubtractShiftCreditRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004FE4 RID: 20452
		public int ShiftCreditToRemove;
	}

	// Token: 0x0200098A RID: 2442
	[Serializable]
	private class AdvanceDockWristUpgradeRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004FE5 RID: 20453
		public ProgressionManager.WristDockUpgradeType Upgrade;
	}

	// Token: 0x0200098B RID: 2443
	[Serializable]
	private class DockWristUpgradeStatusRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x0200098C RID: 2444
	[Serializable]
	public class DockWristStatusResponse
	{
		// Token: 0x04004FE6 RID: 20454
		public int CurrentUpgrade1Level;

		// Token: 0x04004FE7 RID: 20455
		public int CurrentUpgrade2Level;

		// Token: 0x04004FE8 RID: 20456
		public int CurrentUpgrade3Level;

		// Token: 0x04004FE9 RID: 20457
		public int Upgrade1LevelMax;

		// Token: 0x04004FEA RID: 20458
		public int Upgrade2LevelMax;

		// Token: 0x04004FEB RID: 20459
		public int Upgrade3LevelMax;
	}

	// Token: 0x0200098D RID: 2445
	[Serializable]
	private class PurchaseDrillUpgradeRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004FEC RID: 20460
		public ProgressionManager.DrillUpgradeLevel Upgrade;
	}

	// Token: 0x0200098E RID: 2446
	[Serializable]
	private class PurchaseDrillUpgradeResponse
	{
		// Token: 0x04004FED RID: 20461
		public int StatusCode;

		// Token: 0x04004FEE RID: 20462
		public string Error;
	}

	// Token: 0x0200098F RID: 2447
	[Serializable]
	private class RecycleToolRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004FEF RID: 20463
		public GRTool.GRToolType ToolBeingRecycled;

		// Token: 0x04004FF0 RID: 20464
		public int NumberOfPlayers;
	}

	// Token: 0x02000990 RID: 2448
	[Serializable]
	private class StartOfShiftRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004FF1 RID: 20465
		public string ShiftId;

		// Token: 0x04004FF2 RID: 20466
		public int CoresRequired;

		// Token: 0x04004FF3 RID: 20467
		public int NumberOfPlayers;

		// Token: 0x04004FF4 RID: 20468
		public int Depth;
	}

	// Token: 0x02000991 RID: 2449
	[Serializable]
	private class EndOfShiftRewardRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004FF5 RID: 20469
		public string ShiftId;
	}

	// Token: 0x02000992 RID: 2450
	[Serializable]
	private class GhostReactorStatsRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000993 RID: 2451
	[Serializable]
	public class GhostReactorStatsResponse
	{
		// Token: 0x04004FF6 RID: 20470
		public string MothershipId;

		// Token: 0x04004FF7 RID: 20471
		public int MaxDepthReached;
	}

	// Token: 0x02000994 RID: 2452
	[Serializable]
	private class GhostReactorInventoryRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000995 RID: 2453
	[Serializable]
	public class GhostReactorInventoryResponse
	{
		// Token: 0x04004FF8 RID: 20472
		public string MothershipId;

		// Token: 0x04004FF9 RID: 20473
		public string InventoryJson;
	}

	// Token: 0x02000996 RID: 2454
	[Serializable]
	private class SetGhostReactorInventoryRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004FFA RID: 20474
		public string InventoryJson;
	}

	// Token: 0x02000997 RID: 2455
	[Serializable]
	public class SetGhostReactorInventoryResponse
	{
		// Token: 0x04004FFB RID: 20475
		public string MothershipId;
	}
}
