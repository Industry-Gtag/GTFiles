using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012F9 RID: 4857
	public class EdibleWearable : MonoBehaviour
	{
		// Token: 0x060079C5 RID: 31173 RVA: 0x0027BA50 File Offset: 0x00279C50
		protected void Awake()
		{
			this.edibleState = 0;
			this.previousEdibleState = 0;
			this.ownerRig = base.GetComponentInParent<VRRig>();
			this.isLocal = this.ownerRig != null && this.ownerRig.isOfflineVRRig;
			this.isHandSlot = this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand || this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.RightHand;
			this.isLeftHand = this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand;
			this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.wearablePackedStateSlot];
		}

		// Token: 0x060079C6 RID: 31174 RVA: 0x0027BADC File Offset: 0x00279CDC
		protected void OnEnable()
		{
			if (this.ownerRig == null)
			{
				Debug.LogError("EdibleWearable \"" + base.transform.GetPath() + "\": Deactivating because ownerRig is null.", this);
				base.gameObject.SetActive(false);
				return;
			}
			for (int i = 0; i < this.edibleStateInfos.Length; i++)
			{
				this.edibleStateInfos[i].gameObject.SetActive(i == this.edibleState);
			}
		}

		// Token: 0x060079C7 RID: 31175 RVA: 0x0027BB56 File Offset: 0x00279D56
		protected virtual void LateUpdate()
		{
			if (this.isLocal)
			{
				this.LateUpdateLocal();
			}
			else
			{
				this.LateUpdateReplicated();
			}
			this.LateUpdateShared();
		}

		// Token: 0x060079C8 RID: 31176 RVA: 0x0027BB74 File Offset: 0x00279D74
		protected virtual void LateUpdateLocal()
		{
			if (this.edibleState == this.edibleStateInfos.Length - 1)
			{
				if (!this.isNonRespawnable && Time.time > this.lastFullyEatenTime + this.respawnTime)
				{
					this.edibleState = 0;
					this.previousEdibleState = 0;
					this.OnEdibleHoldableStateChange();
				}
				if (this.isNonRespawnable && Time.time > this.lastFullyEatenTime)
				{
					this.edibleState = 0;
					this.previousEdibleState = 0;
					this.OnEdibleHoldableStateChange();
					GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer).netView.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[] { false, this.isLeftHand });
				}
			}
			else if (Time.time > this.lastEatTime + this.biteCooldown)
			{
				Vector3 vector = base.transform.TransformPoint(this.edibleBiteOffset);
				bool flag = false;
				float num = this.biteDistance * this.biteDistance;
				if (!GorillaParent.hasInstance)
				{
					return;
				}
				if ((GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - vector).sqrMagnitude < num)
				{
					flag = true;
				}
				foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
				{
					VRRig rig = rigContainer.Rig;
					if (!flag)
					{
						if (rig.head == null)
						{
							break;
						}
						if (rig.head.rigTarget.IsNull())
						{
							break;
						}
						if ((rig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - vector).sqrMagnitude < num)
						{
							flag = true;
						}
					}
				}
				if (flag && !this.wasInBiteZoneLastFrame && this.edibleState < this.edibleStateInfos.Length)
				{
					this.edibleState++;
					this.lastEatTime = Time.time;
					this.lastFullyEatenTime = Time.time;
				}
				this.wasInBiteZoneLastFrame = flag;
			}
			this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, this.edibleState);
		}

		// Token: 0x060079C9 RID: 31177 RVA: 0x0027BDB4 File Offset: 0x00279FB4
		protected virtual void LateUpdateReplicated()
		{
			this.edibleState = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
		}

		// Token: 0x060079CA RID: 31178 RVA: 0x0027BDE4 File Offset: 0x00279FE4
		protected virtual void LateUpdateShared()
		{
			int num = this.edibleState;
			if (num != this.previousEdibleState)
			{
				this.OnEdibleHoldableStateChange();
			}
			this.previousEdibleState = num;
		}

		// Token: 0x060079CB RID: 31179 RVA: 0x0027BE10 File Offset: 0x0027A010
		protected virtual void OnEdibleHoldableStateChange()
		{
			if (this.previousEdibleState >= 0 && this.previousEdibleState < this.edibleStateInfos.Length)
			{
				this.edibleStateInfos[this.previousEdibleState].gameObject.SetActive(false);
			}
			if (this.edibleState >= 0 && this.edibleState < this.edibleStateInfos.Length)
			{
				this.edibleStateInfos[this.edibleState].gameObject.SetActive(true);
			}
			if (this.edibleState > 0 && this.edibleState < this.edibleStateInfos.Length && this.audioSource != null)
			{
				this.audioSource.GTPlayOneShot(this.edibleStateInfos[this.edibleState].sound, this.volume);
			}
			if (this.edibleState == this.edibleStateInfos.Length && this.audioSource != null)
			{
				this.audioSource.GTPlayOneShot(this.edibleStateInfos[this.edibleState - 1].sound, this.volume);
			}
			float num = GorillaTagger.Instance.tapHapticStrength / 4f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (this.isLocal && this.isHandSlot)
			{
				GorillaTagger.Instance.StartVibration(this.isLeftHand, num, fixedDeltaTime);
			}
		}

		// Token: 0x04008B1A RID: 35610
		[Tooltip("Check when using non cosmetic edible items like honeycomb")]
		public bool isNonRespawnable;

		// Token: 0x04008B1B RID: 35611
		[Tooltip("Eating sounds are played through this AudioSource using PlayOneShot.")]
		public AudioSource audioSource;

		// Token: 0x04008B1C RID: 35612
		[Tooltip("Volume each bite should play at.")]
		public float volume = 0.08f;

		// Token: 0x04008B1D RID: 35613
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.LeftHand;

		// Token: 0x04008B1E RID: 35614
		[Tooltip("Time between bites.")]
		public float biteCooldown = 1f;

		// Token: 0x04008B1F RID: 35615
		[Tooltip("How long it takes to pop back to the uneaten state after being fully eaten.")]
		public float respawnTime = 7f;

		// Token: 0x04008B20 RID: 35616
		[Tooltip("Distance from mouth to item required to trigger a bite.")]
		public float biteDistance = 0.5f;

		// Token: 0x04008B21 RID: 35617
		[Tooltip("Offset from Gorilla's head to mouth.")]
		public Vector3 gorillaHeadMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04008B22 RID: 35618
		[Tooltip("Offset from edible's transform to the bite point.")]
		public Vector3 edibleBiteOffset = new Vector3(0f, 0f, 0f);

		// Token: 0x04008B23 RID: 35619
		public EdibleWearable.EdibleStateInfo[] edibleStateInfos;

		// Token: 0x04008B24 RID: 35620
		private VRRig ownerRig;

		// Token: 0x04008B25 RID: 35621
		private bool isLocal;

		// Token: 0x04008B26 RID: 35622
		private bool isHandSlot;

		// Token: 0x04008B27 RID: 35623
		private bool isLeftHand;

		// Token: 0x04008B28 RID: 35624
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x04008B29 RID: 35625
		private int edibleState;

		// Token: 0x04008B2A RID: 35626
		private int previousEdibleState;

		// Token: 0x04008B2B RID: 35627
		private float lastEatTime;

		// Token: 0x04008B2C RID: 35628
		private float lastFullyEatenTime;

		// Token: 0x04008B2D RID: 35629
		private bool wasInBiteZoneLastFrame;

		// Token: 0x020012FA RID: 4858
		[Serializable]
		public struct EdibleStateInfo
		{
			// Token: 0x04008B2E RID: 35630
			[Tooltip("Will be activated when this stage is reached.")]
			public GameObject gameObject;

			// Token: 0x04008B2F RID: 35631
			[Tooltip("Will be played when this stage is reached.")]
			public AudioClip sound;
		}
	}
}
