using System;
using GorillaNetworking;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012FC RID: 4860
	public class NetworkedWearable : MonoBehaviour, ISpawnable, ITickSystemTick
	{
		// Token: 0x060079D9 RID: 31193 RVA: 0x0027C4C4 File Offset: 0x0027A6C4
		private void Awake()
		{
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw)
			{
				this.isTwoHanded = false;
			}
			this.wearableSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, true);
			this.leftSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, true);
			this.rightSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, false);
		}

		// Token: 0x060079DA RID: 31194 RVA: 0x0027C51A File Offset: 0x0027A71A
		private void OnEnable()
		{
			if (!this.IsSpawned)
			{
				return;
			}
			if (this.isLocal && !this.listenForChangesLocal)
			{
				this.SetWearableStateBool(this.startTrue);
				return;
			}
			if (!this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x060079DB RID: 31195 RVA: 0x0027C550 File Offset: 0x0027A750
		public void ToggleWearableStateBool()
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling ToggleWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling ToggleWearableStateBool on two handed object " + base.gameObject.name + ". please use ToggleLeftWearableStateBool or ToggleRightWearableStateBool instead", null);
				this.ToggleLeftWearableStateBool();
				return;
			}
			this.value = !this.value;
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.wearableSlot, this.value);
			this.OnWearableStateChanged();
		}

		// Token: 0x060079DC RID: 31196 RVA: 0x0027C628 File Offset: 0x0027A828
		public void SetWearableStateBool(bool newState)
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling SetWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling SetWearableStateBool on two handed object " + base.gameObject.name + ". please use SetLeftWearableStateBool or SetRightWearableStateBool instead", null);
				this.SetLeftWearableStateBool(newState);
				return;
			}
			if (this.value != newState)
			{
				this.value = newState;
				this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.wearableSlot, this.value);
				this.OnWearableStateChanged();
			}
		}

		// Token: 0x060079DD RID: 31197 RVA: 0x0027C704 File Offset: 0x0027A904
		public void ToggleLeftWearableStateBool()
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling ToggleLeftWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling ToggleLeftWearableStateBool on one handed object " + base.gameObject.name + ". Please use ToggleWearableStateBool instead", null);
				this.ToggleWearableStateBool();
				return;
			}
			this.leftHandValue = !this.leftHandValue;
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.leftSlot, this.leftHandValue);
			this.OnLeftStateChanged();
		}

		// Token: 0x060079DE RID: 31198 RVA: 0x0027C7DC File Offset: 0x0027A9DC
		public void ToggleRightWearableStateBool()
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling ToggleRightWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling ToggleRightWearableStateBool on one handed object " + base.gameObject.name + ". Please use ToggleWearableStateBool instead", null);
				this.ToggleWearableStateBool();
				return;
			}
			this.rightHandValue = !this.rightHandValue;
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.rightSlot, this.rightHandValue);
			this.OnRightStateChanged();
		}

		// Token: 0x060079DF RID: 31199 RVA: 0x0027C8B4 File Offset: 0x0027AAB4
		public void SetLeftWearableStateBool(bool newState)
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling SetLeftWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling SetLeftWearableStateBool on one handed object " + base.gameObject.name + ". Please use SetWearableStateBool instead", null);
				this.SetWearableStateBool(newState);
				return;
			}
			if (this.leftHandValue != newState)
			{
				this.leftHandValue = newState;
				this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.leftSlot, this.leftHandValue);
				this.OnLeftStateChanged();
			}
		}

		// Token: 0x060079E0 RID: 31200 RVA: 0x0027C990 File Offset: 0x0027AB90
		public void SetRightWearableStateBool(bool newState)
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling SetRightWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling SetRightWearableStateBool on one handed object " + base.gameObject.name + ". Please use SetWearableStateBool instead", null);
				this.SetWearableStateBool(newState);
				return;
			}
			if (this.rightHandValue != newState)
			{
				this.rightHandValue = newState;
				this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.rightSlot, this.rightHandValue);
				this.OnRightStateChanged();
			}
		}

		// Token: 0x060079E1 RID: 31201 RVA: 0x0027CA6A File Offset: 0x0027AC6A
		public void OnDisable()
		{
			if (this.isLocal && !this.listenForChangesLocal)
			{
				this.SetWearableStateBool(false);
				return;
			}
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x060079E2 RID: 31202 RVA: 0x0027CA92 File Offset: 0x0027AC92
		private void OnWearableStateChanged()
		{
			if (this.value)
			{
				UnityEvent onWearableStateTrue = this.OnWearableStateTrue;
				if (onWearableStateTrue == null)
				{
					return;
				}
				onWearableStateTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onWearableStateFalse = this.OnWearableStateFalse;
				if (onWearableStateFalse == null)
				{
					return;
				}
				onWearableStateFalse.Invoke();
				return;
			}
		}

		// Token: 0x060079E3 RID: 31203 RVA: 0x0027CABD File Offset: 0x0027ACBD
		private void OnLeftStateChanged()
		{
			if (this.leftHandValue)
			{
				UnityEvent onLeftWearableStateTrue = this.OnLeftWearableStateTrue;
				if (onLeftWearableStateTrue == null)
				{
					return;
				}
				onLeftWearableStateTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onLeftWearableStateFalse = this.OnLeftWearableStateFalse;
				if (onLeftWearableStateFalse == null)
				{
					return;
				}
				onLeftWearableStateFalse.Invoke();
				return;
			}
		}

		// Token: 0x060079E4 RID: 31204 RVA: 0x0027CAE8 File Offset: 0x0027ACE8
		private void OnRightStateChanged()
		{
			if (this.rightHandValue)
			{
				UnityEvent onRightWearableStateTrue = this.OnRightWearableStateTrue;
				if (onRightWearableStateTrue == null)
				{
					return;
				}
				onRightWearableStateTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onRightWearableStateFalse = this.OnRightWearableStateFalse;
				if (onRightWearableStateFalse == null)
				{
					return;
				}
				onRightWearableStateFalse.Invoke();
				return;
			}
		}

		// Token: 0x17000BCE RID: 3022
		// (get) Token: 0x060079E5 RID: 31205 RVA: 0x0027CB13 File Offset: 0x0027AD13
		// (set) Token: 0x060079E6 RID: 31206 RVA: 0x0027CB1B File Offset: 0x0027AD1B
		public bool IsSpawned { get; set; }

		// Token: 0x17000BCF RID: 3023
		// (get) Token: 0x060079E7 RID: 31207 RVA: 0x0027CB24 File Offset: 0x0027AD24
		// (set) Token: 0x060079E8 RID: 31208 RVA: 0x0027CB2C File Offset: 0x0027AD2C
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x060079E9 RID: 31209 RVA: 0x0027CB38 File Offset: 0x0027AD38
		public void OnSpawn(VRRig rig)
		{
			if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.CosmeticSelectedSide == ECosmeticSelectSide.Both)
			{
				GTDev.LogWarning<string>(string.Format("NetworkedWearable: Cosmetic {0} with category {1} has select side Both, assuming left side!", base.gameObject.name, this.assignedSlot), null);
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				GTDev.LogError<string>(string.Format("NetworkedWearable: Cosmetic {0} spawned with invalid category {1}!", base.gameObject.name, this.assignedSlot), null);
			}
			this.myRig = rig;
			this.isLocal = rig.isLocal;
			this.wearableSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, this.CosmeticSelectedSide != ECosmeticSelectSide.Right);
		}

		// Token: 0x060079EA RID: 31210 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnDespawn()
		{
		}

		// Token: 0x17000BD0 RID: 3024
		// (get) Token: 0x060079EB RID: 31211 RVA: 0x0027CBE0 File Offset: 0x0027ADE0
		// (set) Token: 0x060079EC RID: 31212 RVA: 0x0027CBE8 File Offset: 0x0027ADE8
		public bool TickRunning { get; set; }

		// Token: 0x060079ED RID: 31213 RVA: 0x0027CBF4 File Offset: 0x0027ADF4
		public void Tick()
		{
			if ((!this.isLocal || this.listenForChangesLocal) && this.IsSpawned)
			{
				if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
				{
					bool flag = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int)this.leftSlot);
					if (this.leftHandValue != flag)
					{
						this.leftHandValue = flag;
						this.OnLeftStateChanged();
					}
					flag = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int)this.rightSlot);
					if (this.rightHandValue != flag)
					{
						this.rightHandValue = flag;
						this.OnRightStateChanged();
						return;
					}
				}
				else
				{
					bool flag2 = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int)this.wearableSlot);
					if (this.value != flag2)
					{
						this.value = flag2;
						this.OnWearableStateChanged();
					}
				}
			}
		}

		// Token: 0x060079EE RID: 31214 RVA: 0x0027CCB8 File Offset: 0x0027AEB8
		public static bool IsCategoryValid(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
			case CosmeticsController.CosmeticCategory.Badge:
			case CosmeticsController.CosmeticCategory.Face:
			case CosmeticsController.CosmeticCategory.Paw:
			case CosmeticsController.CosmeticCategory.Fur:
			case CosmeticsController.CosmeticCategory.Shirt:
			case CosmeticsController.CosmeticCategory.Pants:
				return true;
			}
			return false;
		}

		// Token: 0x060079EF RID: 31215 RVA: 0x0027CCF0 File Offset: 0x0027AEF0
		private VRRig.WearablePackedStateSlots CosmeticCategoryToWearableSlot(CosmeticsController.CosmeticCategory category, bool isLeft)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return VRRig.WearablePackedStateSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return VRRig.WearablePackedStateSlots.Badge;
			case CosmeticsController.CosmeticCategory.Face:
				return VRRig.WearablePackedStateSlots.Face;
			case CosmeticsController.CosmeticCategory.Paw:
				if (!isLeft)
				{
					return VRRig.WearablePackedStateSlots.RightHand;
				}
				return VRRig.WearablePackedStateSlots.LeftHand;
			case CosmeticsController.CosmeticCategory.Fur:
				return VRRig.WearablePackedStateSlots.Fur;
			case CosmeticsController.CosmeticCategory.Shirt:
				return VRRig.WearablePackedStateSlots.Shirt;
			case CosmeticsController.CosmeticCategory.Pants:
				return VRRig.WearablePackedStateSlots.Pants1;
			}
			GTDev.LogWarning<string>(string.Format("NetworkedWearable: {0} item cannot set wearable state", category), null);
			return VRRig.WearablePackedStateSlots.Hat;
		}

		// Token: 0x04008B46 RID: 35654
		[Tooltip("Whether the wearable state is toggled on by default.")]
		[SerializeField]
		private bool startTrue;

		// Token: 0x04008B47 RID: 35655
		[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
		[SerializeField]
		private CosmeticsController.CosmeticCategory assignedSlot;

		// Token: 0x04008B48 RID: 35656
		[FormerlySerializedAs("IsTwoHanded")]
		[SerializeField]
		private bool isTwoHanded;

		// Token: 0x04008B49 RID: 35657
		private const string listenInfo = "listenForChangesLocal should be false in most cases";

		// Token: 0x04008B4A RID: 35658
		private const string listenDetails = "listenForChangesLocal should be false in most cases\nIf you have a first person part and a local rig part that both need to react to a state change\ncall the Toggle/Set functions to change the state from one prefab and check \nlistenForChangesLocal on the other prefab ";

		// Token: 0x04008B4B RID: 35659
		[SerializeField]
		private bool listenForChangesLocal;

		// Token: 0x04008B4C RID: 35660
		private VRRig.WearablePackedStateSlots wearableSlot;

		// Token: 0x04008B4D RID: 35661
		private VRRig.WearablePackedStateSlots leftSlot = VRRig.WearablePackedStateSlots.LeftHand;

		// Token: 0x04008B4E RID: 35662
		private VRRig.WearablePackedStateSlots rightSlot = VRRig.WearablePackedStateSlots.RightHand;

		// Token: 0x04008B4F RID: 35663
		private VRRig myRig;

		// Token: 0x04008B50 RID: 35664
		private bool isLocal;

		// Token: 0x04008B51 RID: 35665
		private bool value;

		// Token: 0x04008B52 RID: 35666
		private bool leftHandValue;

		// Token: 0x04008B53 RID: 35667
		private bool rightHandValue;

		// Token: 0x04008B54 RID: 35668
		[SerializeField]
		protected UnityEvent OnWearableStateTrue;

		// Token: 0x04008B55 RID: 35669
		[SerializeField]
		protected UnityEvent OnWearableStateFalse;

		// Token: 0x04008B56 RID: 35670
		[SerializeField]
		protected UnityEvent OnLeftWearableStateTrue;

		// Token: 0x04008B57 RID: 35671
		[SerializeField]
		protected UnityEvent OnLeftWearableStateFalse;

		// Token: 0x04008B58 RID: 35672
		[SerializeField]
		protected UnityEvent OnRightWearableStateTrue;

		// Token: 0x04008B59 RID: 35673
		[SerializeField]
		protected UnityEvent OnRightWearableStateFalse;
	}
}
