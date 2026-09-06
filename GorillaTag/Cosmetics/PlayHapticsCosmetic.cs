using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001355 RID: 4949
	public class PlayHapticsCosmetic : MonoBehaviour
	{
		// Token: 0x06007C03 RID: 31747 RVA: 0x00288284 File Offset: 0x00286484
		private void Awake()
		{
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			this.myHeldItem = base.GetComponentInParent<IHeldItem>();
		}

		// Token: 0x06007C04 RID: 31748 RVA: 0x0028829E File Offset: 0x0028649E
		public void PlayHaptics()
		{
			GorillaTagger.Instance.StartVibration(this.leftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06007C05 RID: 31749 RVA: 0x002882BC File Offset: 0x002864BC
		public void PlayHapticsTransferableObject()
		{
			this.PlayHapticsHeldItem();
		}

		// Token: 0x06007C06 RID: 31750 RVA: 0x002882C4 File Offset: 0x002864C4
		public void PlayHapticsHeldItem()
		{
			bool flag;
			if (!(this.parentTransferable != null))
			{
				IHeldItem heldItem = this.myHeldItem;
				flag = heldItem != null && heldItem.IsMyItem();
			}
			else
			{
				flag = this.parentTransferable.IsMyItem();
			}
			if (!flag)
			{
				return;
			}
			bool flag2;
			if (!(this.parentTransferable != null))
			{
				IHeldItem heldItem2 = this.myHeldItem;
				flag2 = heldItem2 != null && heldItem2.InLeftHand();
			}
			else
			{
				flag2 = this.parentTransferable.InLeftHand();
			}
			bool flag3 = flag2;
			GorillaTagger.Instance.StartVibration(flag3, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06007C07 RID: 31751 RVA: 0x00288346 File Offset: 0x00286546
		public void PlayHaptics(bool isLeftHand)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06007C08 RID: 31752 RVA: 0x0028835F File Offset: 0x0028655F
		public void PlayHapticsBothHands(bool isLeftHand)
		{
			this.PlayHaptics(false);
			this.PlayHaptics(true);
		}

		// Token: 0x06007C09 RID: 31753 RVA: 0x00288346 File Offset: 0x00286546
		public void PlayHaptics(bool isLeftHand, float value)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06007C0A RID: 31754 RVA: 0x0028836F File Offset: 0x0028656F
		public void PlayHapticsBothHands(bool isLeftHand, float value)
		{
			this.PlayHaptics(false, value);
			this.PlayHaptics(true, value);
		}

		// Token: 0x06007C0B RID: 31755 RVA: 0x00288346 File Offset: 0x00286546
		public void PlayHaptics(bool isLeftHand, Collider other)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06007C0C RID: 31756 RVA: 0x00288381 File Offset: 0x00286581
		public void PlayHapticsBothHands(bool isLeftHand, Collider other)
		{
			this.PlayHaptics(false, other);
			this.PlayHaptics(true, other);
		}

		// Token: 0x06007C0D RID: 31757 RVA: 0x00288346 File Offset: 0x00286546
		public void PlayHaptics(bool isLeftHand, Collision other)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06007C0E RID: 31758 RVA: 0x00288393 File Offset: 0x00286593
		public void PlayHapticsBothHands(bool isLeftHand, Collision other)
		{
			this.PlayHaptics(false, other);
			this.PlayHaptics(true, other);
		}

		// Token: 0x06007C0F RID: 31759 RVA: 0x002883A8 File Offset: 0x002865A8
		public void PlayHapticsByButtonValue(bool isLeftHand, float strength)
		{
			float num = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, strength);
			GorillaTagger.Instance.StartVibration(isLeftHand, num, this.hapticDuration);
		}

		// Token: 0x06007C10 RID: 31760 RVA: 0x002883DA File Offset: 0x002865DA
		public void PlayHapticsByButtonValueBothHands(bool isLeftHand, float strength)
		{
			this.PlayHapticsByButtonValue(false, strength);
			this.PlayHapticsByButtonValue(true, strength);
		}

		// Token: 0x06007C11 RID: 31761 RVA: 0x002883EC File Offset: 0x002865EC
		public void PlayHapticsByVelocity(bool isLeftHand, float velocity)
		{
			float num = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand).GetAverageVelocity(true, 0.15f, false).magnitude;
			num = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, num);
			GorillaTagger.Instance.StartVibration(isLeftHand, num, this.hapticDuration);
		}

		// Token: 0x06007C12 RID: 31762 RVA: 0x0028843E File Offset: 0x0028663E
		public void PlayHapticsByVelocityBothHands(bool isLeftHand, float velocity)
		{
			this.PlayHapticsByVelocity(false, velocity);
			this.PlayHapticsByVelocity(true, velocity);
		}

		// Token: 0x04008E1B RID: 36379
		[SerializeField]
		private float hapticDuration;

		// Token: 0x04008E1C RID: 36380
		[SerializeField]
		private float hapticStrength;

		// Token: 0x04008E1D RID: 36381
		[SerializeField]
		private float minHapticStrengthThreshold;

		// Token: 0x04008E1E RID: 36382
		[SerializeField]
		private float maxHapticStrengthThreshold;

		// Token: 0x04008E1F RID: 36383
		[Tooltip("Only check this box if you are not setting the left/hand right from the subscriber")]
		[SerializeField]
		private bool leftHand;

		// Token: 0x04008E20 RID: 36384
		private TransferrableObject parentTransferable;

		// Token: 0x04008E21 RID: 36385
		private IHeldItem myHeldItem;
	}
}
