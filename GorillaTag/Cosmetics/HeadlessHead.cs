using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012FB RID: 4859
	public class HeadlessHead : HoldableObject
	{
		// Token: 0x060079CD RID: 31181 RVA: 0x0027BFD4 File Offset: 0x0027A1D4
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
			if (this.ownerRig == null)
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = this.ownerRig.isOfflineVRRig;
			this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.wearablePackedStateSlot];
			this.baseLocalPosition = base.transform.localPosition;
			this.hasFirstPersonRenderer = this.firstPersonRenderer != null;
		}

		// Token: 0x060079CE RID: 31182 RVA: 0x0027C058 File Offset: 0x0027A258
		protected void OnEnable()
		{
			if (this.ownerRig == null)
			{
				Debug.LogError("HeadlessHead \"" + base.transform.GetPath() + "\": Deactivating because ownerRig is null.", this);
				base.gameObject.SetActive(false);
				return;
			}
			this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.NoHead);
		}

		// Token: 0x060079CF RID: 31183 RVA: 0x0027C0B1 File Offset: 0x0027A2B1
		private void OnDisable()
		{
			this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.Default);
		}

		// Token: 0x060079D0 RID: 31184 RVA: 0x0027C0C4 File Offset: 0x0027A2C4
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

		// Token: 0x060079D1 RID: 31185 RVA: 0x0027C0E2 File Offset: 0x0027A2E2
		protected virtual void LateUpdateLocal()
		{
			this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, (this.isHeld ? 1 : 0) + (this.isHeldLeftHand ? 2 : 0));
		}

		// Token: 0x060079D2 RID: 31186 RVA: 0x0027C120 File Offset: 0x0027A320
		protected virtual void LateUpdateReplicated()
		{
			int num = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
			this.isHeld = num != 0;
			this.isHeldLeftHand = (num & 2) != 0;
		}

		// Token: 0x060079D3 RID: 31187 RVA: 0x0027C16C File Offset: 0x0027A36C
		protected virtual void LateUpdateShared()
		{
			if (this.isHeld != this.wasHeld || this.isHeldLeftHand != this.wasHeldLeftHand)
			{
				this.blendingFromPosition = base.transform.position;
				this.blendingFromRotation = base.transform.rotation;
				this.blendFraction = 0f;
			}
			Quaternion quaternion;
			Vector3 vector;
			if (this.isHeldLeftHand)
			{
				quaternion = this.ownerRig.leftHandTransform.rotation * this.rotationFromLeftHand;
				vector = this.ownerRig.leftHandTransform.TransformPoint(this.offsetFromLeftHand) - quaternion * this.holdAnchorPoint.transform.localPosition;
			}
			else if (this.isHeld)
			{
				quaternion = this.ownerRig.rightHandTransform.rotation * this.rotationFromRightHand;
				vector = this.ownerRig.rightHandTransform.TransformPoint(this.offsetFromRightHand) - quaternion * this.holdAnchorPoint.transform.localPosition;
			}
			else
			{
				quaternion = base.transform.parent.rotation;
				vector = base.transform.parent.TransformPoint(this.baseLocalPosition);
			}
			if (this.blendFraction < 1f)
			{
				this.blendFraction += Time.deltaTime / this.blendDuration;
				quaternion = Quaternion.Lerp(this.blendingFromRotation, quaternion, this.blendFraction);
				vector = Vector3.Lerp(this.blendingFromPosition, vector, this.blendFraction);
			}
			base.transform.rotation = quaternion;
			base.transform.position = vector;
			if (this.hasFirstPersonRenderer)
			{
				float x = base.transform.lossyScale.x;
				this.firstPersonRenderer.enabled = (this.firstPersonHideCenter.transform.position - GTPlayer.Instance.headCollider.transform.position).IsLongerThan(this.firstPersonHiddenRadius * x);
			}
			this.wasHeld = this.isHeld;
			this.wasHeldLeftHand = this.isHeldLeftHand;
		}

		// Token: 0x060079D4 RID: 31188 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
		{
		}

		// Token: 0x060079D5 RID: 31189 RVA: 0x0027C373 File Offset: 0x0027A573
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			this.isHeld = true;
			this.isHeldLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
			EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		}

		// Token: 0x060079D6 RID: 31190 RVA: 0x0027C3A7 File Offset: 0x0027A5A7
		public override void DropItemCleanup()
		{
			this.isHeld = false;
			this.isHeldLeftHand = false;
		}

		// Token: 0x060079D7 RID: 31191 RVA: 0x0027C3B8 File Offset: 0x0027A5B8
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
			{
				return false;
			}
			if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
			{
				return false;
			}
			EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
			this.isHeld = false;
			this.isHeldLeftHand = false;
			return true;
		}

		// Token: 0x04008B30 RID: 35632
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.Face;

		// Token: 0x04008B31 RID: 35633
		[SerializeField]
		private Vector3 offsetFromLeftHand = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04008B32 RID: 35634
		[SerializeField]
		private Vector3 offsetFromRightHand = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04008B33 RID: 35635
		[SerializeField]
		private Quaternion rotationFromLeftHand = Quaternion.Euler(14.063973f, 52.56744f, 10.067408f);

		// Token: 0x04008B34 RID: 35636
		[SerializeField]
		private Quaternion rotationFromRightHand = Quaternion.Euler(14.063973f, 52.56744f, 10.067408f);

		// Token: 0x04008B35 RID: 35637
		private Vector3 baseLocalPosition;

		// Token: 0x04008B36 RID: 35638
		private VRRig ownerRig;

		// Token: 0x04008B37 RID: 35639
		private bool isLocal;

		// Token: 0x04008B38 RID: 35640
		private bool isHeld;

		// Token: 0x04008B39 RID: 35641
		private bool isHeldLeftHand;

		// Token: 0x04008B3A RID: 35642
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x04008B3B RID: 35643
		[SerializeField]
		private MeshRenderer firstPersonRenderer;

		// Token: 0x04008B3C RID: 35644
		[SerializeField]
		private float firstPersonHiddenRadius;

		// Token: 0x04008B3D RID: 35645
		[SerializeField]
		private Transform firstPersonHideCenter;

		// Token: 0x04008B3E RID: 35646
		[SerializeField]
		private Transform holdAnchorPoint;

		// Token: 0x04008B3F RID: 35647
		private bool hasFirstPersonRenderer;

		// Token: 0x04008B40 RID: 35648
		private Vector3 blendingFromPosition;

		// Token: 0x04008B41 RID: 35649
		private Quaternion blendingFromRotation;

		// Token: 0x04008B42 RID: 35650
		private float blendFraction;

		// Token: 0x04008B43 RID: 35651
		private bool wasHeld;

		// Token: 0x04008B44 RID: 35652
		private bool wasHeldLeftHand;

		// Token: 0x04008B45 RID: 35653
		[SerializeField]
		private float blendDuration = 0.3f;
	}
}
