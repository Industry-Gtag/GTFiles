using System;
using DefaultNamespace;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200009E RID: 158
public class EvolvingCosmetic : MonoBehaviour, ICosmeticStateSync
{
	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060003E9 RID: 1001 RVA: 0x000177FB File Offset: 0x000159FB
	public int StateValue
	{
		get
		{
			return this.SelectedObjectIndex;
		}
	}

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x060003EA RID: 1002 RVA: 0x00017803 File Offset: 0x00015A03
	// (set) Token: 0x060003EB RID: 1003 RVA: 0x0001780B File Offset: 0x00015A0B
	public int SelectedObjectIndex { get; private set; } = -1;

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x060003EC RID: 1004 RVA: 0x00017814 File Offset: 0x00015A14
	public string PlayfabId
	{
		get
		{
			return base.gameObject.name;
		}
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x00017824 File Offset: 0x00015A24
	private void Awake()
	{
		int num;
		if (EvolvingCosmeticSaveData.Instance.SelectedIndices.TryGetValue(this.PlayfabId, out num) && this.IsIndexAvailable(num))
		{
			this.SelectedObjectIndex = num;
			this.ActivateSelectedIndex();
		}
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x00017860 File Offset: 0x00015A60
	private void OnEnable()
	{
		if (this.m_parentRig == null)
		{
			VRRig vrrig = base.GetComponentInParent<VRRig>();
			if (vrrig == null)
			{
				if (base.GetComponentInParent<GTPlayer>() == null)
				{
					return;
				}
				vrrig = VRRig.LocalRig;
			}
			this.m_parentRig = vrrig;
		}
		if (this.m_parentRig == null)
		{
			return;
		}
		if (this.m_parentRig.isLocal)
		{
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.UpdateDaysAccrued));
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.UpdateDaysAccrued));
		}
		this._daysAccrued = new int?(0);
		this.UnselectAll();
		this.m_parentRig.reliableState.RegisterCosmeticStateSyncTarget(this.GetStateSyncSlot(), this);
		this.UpdateDaysAccrued();
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x00017934 File Offset: 0x00015B34
	private void UpdateDaysAccrued()
	{
		SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.GetSubscriptionDetails(this.m_parentRig);
		switch (this.ageRule)
		{
		case EvolvingCosmetic.SubscriptionAgeRule.ItemAge:
			this._daysAccrued = new int?(this.m_parentRig.CheckCosmeticAge(base.name));
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.MinItemSubscriptionAge:
			this._daysAccrued = new int?(Mathf.Min(subscriptionDetails.daysAccrued, this.m_parentRig.CheckCosmeticAge(base.name)));
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.SubscriptionAge:
			this._daysAccrued = new int?(subscriptionDetails.daysAccrued);
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.MinItemSubscriptionAgeActive:
			if (subscriptionDetails.active)
			{
				this._daysAccrued = new int?(Mathf.Min(subscriptionDetails.daysAccrued, this.m_parentRig.CheckCosmeticAge(base.name)));
			}
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.SubscriptionAgeActive:
			if (subscriptionDetails.active)
			{
				this._daysAccrued = new int?(subscriptionDetails.daysAccrued);
			}
			break;
		}
		if (this._daysAccrued == null)
		{
			Debug.LogError("_daysAccrued was not set by end of OnEnable.");
			return;
		}
		int value = this._daysAccrued.Value;
		this.SelectedObjectIndex = this.FindAgeAwareIndex(value);
		this.ActivateSelectedIndex();
		UnityEvent<int> dispatchDaysOnEnable = this.DispatchDaysOnEnable;
		if (dispatchDaysOnEnable != null)
		{
			dispatchDaysOnEnable.Invoke(Mathf.Min(value, this.capDays));
		}
		if (this.maxDays > 0)
		{
			UnityEvent<float> dispatchDaysOnEnableNormalized = this.DispatchDaysOnEnableNormalized;
			if (dispatchDaysOnEnableNormalized == null)
			{
				return;
			}
			dispatchDaysOnEnableNormalized.Invoke(Mathf.Min((float)value / (float)this.maxDays, 1f) * (float)this.multiplier);
		}
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x00017AA4 File Offset: 0x00015CA4
	private int FindAgeAwareIndex(int daysAccrued)
	{
		if (this.ageAwareGameObjects.Length == 0)
		{
			return -1;
		}
		if (this.ageAwareGameObjects[0].minActiveDays > daysAccrued)
		{
			return -1;
		}
		for (int i = 0; i < this.ageAwareGameObjects.Length; i++)
		{
			if (daysAccrued <= this.ageAwareGameObjects[i].maxActiveDays)
			{
				return i;
			}
		}
		return this.ageAwareGameObjects.Length - 1;
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x00017B08 File Offset: 0x00015D08
	private void OnDisable()
	{
		if (this.m_parentRig.IsNull())
		{
			return;
		}
		if (this.m_parentRig.isLocal)
		{
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.UpdateDaysAccrued));
		}
		this.m_parentRig.reliableState.UnRegisterCosmeticStateSyncTarget(this.GetStateSyncSlot(), this);
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x00017B68 File Offset: 0x00015D68
	private void ActivateSelectedIndex()
	{
		if (this.SelectedObjectIndex < 0)
		{
			this.UnselectAll();
			return;
		}
		if (!this.IsSelectedIndexAvailable())
		{
			return;
		}
		for (int i = 0; i < this.ageAwareGameObjects.Length; i++)
		{
			this.ageAwareGameObjects[i].gameObject.SetActive(i == this.SelectedObjectIndex);
		}
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00017BC0 File Offset: 0x00015DC0
	private bool IsSelectedIndexAvailable()
	{
		return this.IsIndexAvailable(this.SelectedObjectIndex);
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x00017BD0 File Offset: 0x00015DD0
	private bool IsIndexAvailable(int index)
	{
		if (index < 0 || index >= this.ageAwareGameObjects.Length)
		{
			return false;
		}
		EvolvingCosmetic.AgeAwareGameObject ageAwareGameObject = this.ageAwareGameObjects[index];
		return this._daysAccrued.GetValueOrDefault() >= ageAwareGameObject.minActiveDays;
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x00017C14 File Offset: 0x00015E14
	public void GoBack()
	{
		if (!this.CanGoBack())
		{
			return;
		}
		int selectedObjectIndex = this.SelectedObjectIndex;
		this.SelectedObjectIndex = selectedObjectIndex - 1;
		this.ActivateSelectedIndex();
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00017C40 File Offset: 0x00015E40
	public void GoForward()
	{
		if (!this.CanGoForward())
		{
			return;
		}
		int selectedObjectIndex = this.SelectedObjectIndex;
		this.SelectedObjectIndex = selectedObjectIndex + 1;
		this.ActivateSelectedIndex();
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x00017C6C File Offset: 0x00015E6C
	public void MatchStage(EvolvingCosmetic other)
	{
		while (this.SelectedObjectIndex > other.SelectedObjectIndex)
		{
			int num;
			if (!this.CanGoBack())
			{
				IL_0042:
				while (this.SelectedObjectIndex < other.SelectedObjectIndex && this.CanGoForward())
				{
					num = this.SelectedObjectIndex;
					this.SelectedObjectIndex = num + 1;
				}
				this.ActivateSelectedIndex();
				return;
			}
			num = this.SelectedObjectIndex;
			this.SelectedObjectIndex = num - 1;
		}
		goto IL_0042;
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x00017CD0 File Offset: 0x00015ED0
	private void UnselectAll()
	{
		this.SelectedObjectIndex = -1;
		EvolvingCosmetic.AgeAwareGameObject[] array = this.ageAwareGameObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00017D0B File Offset: 0x00015F0B
	public bool CanGoBack()
	{
		return this.IsIndexAvailable(this.SelectedObjectIndex - 1);
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x00017D1B File Offset: 0x00015F1B
	public bool CanGoForward()
	{
		return this.IsIndexAvailable(this.SelectedObjectIndex + 1);
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00017D2B File Offset: 0x00015F2B
	public void OnStateUpdate(int state)
	{
		if (!this.IsIndexAvailable(state))
		{
			return;
		}
		this.SelectedObjectIndex = state;
		this.ActivateSelectedIndex();
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00017D44 File Offset: 0x00015F44
	private VRRigReliableState.StateSyncSlots GetStateSyncSlot()
	{
		CosmeticSO cosmeticSOFromDisplayName = CosmeticsController.instance.GetCosmeticSOFromDisplayName(this.PlayfabId);
		CosmeticsController.CosmeticCategory value = cosmeticSOFromDisplayName.info.category.Value;
		if (value <= CosmeticsController.CosmeticCategory.Face)
		{
			if (value == CosmeticsController.CosmeticCategory.Hat)
			{
				return VRRigReliableState.StateSyncSlots.Hat;
			}
			if (value == CosmeticsController.CosmeticCategory.Face)
			{
				return VRRigReliableState.StateSyncSlots.Face;
			}
		}
		else
		{
			if (value == CosmeticsController.CosmeticCategory.Shirt)
			{
				return VRRigReliableState.StateSyncSlots.Shirt;
			}
			if (value == CosmeticsController.CosmeticCategory.Pants)
			{
				return VRRigReliableState.StateSyncSlots.Pants;
			}
		}
		throw new Exception(string.Format("Unhandled CosmeticCategory {0}", cosmeticSOFromDisplayName.info.category.Value));
	}

	// Token: 0x0400045A RID: 1114
	[SerializeField]
	private EvolvingCosmetic.SubscriptionAgeRule ageRule;

	// Token: 0x0400045B RID: 1115
	[SerializeField]
	private EvolvingCosmetic.AgeAwareGameObject[] ageAwareGameObjects;

	// Token: 0x0400045C RID: 1116
	[SerializeField]
	private int capDays = 1;

	// Token: 0x0400045D RID: 1117
	[SerializeField]
	private UnityEvent<int> DispatchDaysOnEnable;

	// Token: 0x0400045E RID: 1118
	[SerializeField]
	private int maxDays = 1;

	// Token: 0x0400045F RID: 1119
	[SerializeField]
	private int multiplier = 1;

	// Token: 0x04000460 RID: 1120
	[SerializeField]
	private UnityEvent<float> DispatchDaysOnEnableNormalized;

	// Token: 0x04000462 RID: 1122
	private int? _daysAccrued;

	// Token: 0x04000463 RID: 1123
	private VRRig m_parentRig;

	// Token: 0x0200009F RID: 159
	private enum SubscriptionAgeRule
	{
		// Token: 0x04000465 RID: 1125
		ItemAge,
		// Token: 0x04000466 RID: 1126
		MinItemSubscriptionAge,
		// Token: 0x04000467 RID: 1127
		SubscriptionAge,
		// Token: 0x04000468 RID: 1128
		MinItemSubscriptionAgeActive,
		// Token: 0x04000469 RID: 1129
		SubscriptionAgeActive
	}

	// Token: 0x020000A0 RID: 160
	[Serializable]
	private struct AgeAwareGameObject
	{
		// Token: 0x0400046A RID: 1130
		public GameObject gameObject;

		// Token: 0x0400046B RID: 1131
		public int minActiveDays;

		// Token: 0x0400046C RID: 1132
		public int maxActiveDays;

		// Token: 0x0400046D RID: 1133
		public bool requireCurrentSubscription;
	}
}
