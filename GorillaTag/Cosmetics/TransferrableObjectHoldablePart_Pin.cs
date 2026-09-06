using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001366 RID: 4966
	public class TransferrableObjectHoldablePart_Pin : TransferrableObjectHoldablePart
	{
		// Token: 0x06007C70 RID: 31856 RVA: 0x0028A6BD File Offset: 0x002888BD
		protected void OnEnable()
		{
			UnityEvent onEnableHoldable = this.OnEnableHoldable;
			if (onEnableHoldable == null)
			{
				return;
			}
			onEnableHoldable.Invoke();
		}

		// Token: 0x06007C71 RID: 31857 RVA: 0x0028A6D0 File Offset: 0x002888D0
		protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
		{
			if (rig.isOfflineVRRig)
			{
				Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(isHeldLeftHand);
				if (GTPlayer.Instance.GetInteractPointVelocityTracker(isHeldLeftHand).GetAverageVelocity(true, 0.15f, false).magnitude > this.breakStrengthThreshold || (controllerTransform.position - this.pin.transform.position).IsLongerThan(this.maxHandSnapDistance))
				{
					this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
					UnityEvent onBreak = this.OnBreak;
					if (onBreak != null)
					{
						onBreak.Invoke();
					}
					if (this.transferrableParentObject && this.transferrableParentObject.IsMyItem())
					{
						UnityEvent onBreakLocal = this.OnBreakLocal;
						if (onBreakLocal == null)
						{
							return;
						}
						onBreakLocal.Invoke();
					}
					return;
				}
				controllerTransform.position = this.pin.position;
			}
		}

		// Token: 0x04008EDE RID: 36574
		[SerializeField]
		private float breakStrengthThreshold = 0.8f;

		// Token: 0x04008EDF RID: 36575
		[SerializeField]
		private float maxHandSnapDistance = 0.5f;

		// Token: 0x04008EE0 RID: 36576
		[SerializeField]
		private Transform pin;

		// Token: 0x04008EE1 RID: 36577
		public UnityEvent OnBreak;

		// Token: 0x04008EE2 RID: 36578
		public UnityEvent OnBreakLocal;

		// Token: 0x04008EE3 RID: 36579
		public UnityEvent OnEnableHoldable;
	}
}
