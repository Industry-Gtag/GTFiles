using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001341 RID: 4929
	public class HandControlledCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06007BA5 RID: 31653 RVA: 0x00286628 File Offset: 0x00284828
		public void Awake()
		{
			this.myRig = base.GetComponentInParent<VRRig>();
			this.initialRotation = base.transform.localRotation;
			base.enabled = false;
			if (this.debugRelativePositionTransform1 != null)
			{
				Object.Destroy(this.debugRelativePositionTransform1.gameObject);
			}
			if (this.debugRelativePositionTransform2 != null)
			{
				Object.Destroy(this.debugRelativePositionTransform2.gameObject);
			}
		}

		// Token: 0x06007BA6 RID: 31654 RVA: 0x00286698 File Offset: 0x00284898
		private void SetControlIndicatorPoints()
		{
			if (this.myRig.isOfflineVRRig && this.controllingHand != null && this.controlIndicatorCurve != null && this.controlIndicatorCurve.points != null)
			{
				this.controlIndicatorCurve.points[0] = this.controllingHand.position;
				this.controlIndicatorCurve.points[1] = this.controlIndicatorCurve.points[0] + this.myRig.scaleFactor * this.controllingHand.up;
				this.controlIndicatorCurve.points[2] = base.transform.position;
			}
		}

		// Token: 0x06007BA7 RID: 31655 RVA: 0x0028675E File Offset: 0x0028495E
		private Vector3 GetRelativeHandPosition()
		{
			return this.controllingHand.TransformPoint(this.handPositionOffset) - this.myRig.bodyTransform.position;
		}

		// Token: 0x06007BA8 RID: 31656 RVA: 0x00286788 File Offset: 0x00284988
		public void StartControl(bool leftHand, float flexValue)
		{
			if (!base.enabled || !base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.lowAngleLimits = this.activeSettings.angleLimits;
			this.highAngleLimits = 360f * Vector3.one - this.lowAngleLimits;
			this.handRotationOffset = (leftHand ? this.leftHandRotation : this.rightHandRotation);
			this.controllingHand = (leftHand ? this.myRig.leftHand.rigTarget.transform : this.myRig.rightHand.rigTarget.transform);
			this.startHandRelativePosition = this.GetRelativeHandPosition();
			this.startHandInverseRotation = Quaternion.Inverse(this.controllingHand.rotation * this.handRotationOffset);
			this.isActive = true;
			this.SetControlIndicatorPoints();
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06007BA9 RID: 31657 RVA: 0x00286868 File Offset: 0x00284A68
		public void StopControl()
		{
			this.localEuler = base.transform.localRotation.eulerAngles;
			this.isActive = false;
			this.SetControlIndicatorPoints();
		}

		// Token: 0x06007BAA RID: 31658 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnEnable()
		{
		}

		// Token: 0x06007BAB RID: 31659 RVA: 0x0028689B File Offset: 0x00284A9B
		public void OnDisable()
		{
			base.transform.localRotation = this.initialRotation;
			this.StopControl();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007BAC RID: 31660 RVA: 0x002868BA File Offset: 0x00284ABA
		private float ReverseClampDegrees(float value, float low, float high)
		{
			value = Mathf.Repeat(value, 360f);
			if (value <= low || value >= high)
			{
				return value;
			}
			if (value >= 180f)
			{
				return high;
			}
			return low;
		}

		// Token: 0x17000C1F RID: 3103
		// (get) Token: 0x06007BAD RID: 31661 RVA: 0x002868DE File Offset: 0x00284ADE
		// (set) Token: 0x06007BAE RID: 31662 RVA: 0x002868E6 File Offset: 0x00284AE6
		public bool TickRunning { get; set; }

		// Token: 0x06007BAF RID: 31663 RVA: 0x002868F0 File Offset: 0x00284AF0
		public void Tick()
		{
			if (this.isActive)
			{
				HandControlledCosmetic.RotationControl rotationControl = this.activeSettings.rotationControl;
				if (rotationControl != HandControlledCosmetic.RotationControl.Angle)
				{
					if (rotationControl == HandControlledCosmetic.RotationControl.Translation)
					{
						Vector3 relativeHandPosition = this.GetRelativeHandPosition();
						Vector3 vector = new Vector3(relativeHandPosition.x, 0f, relativeHandPosition.z);
						float num = Vector3.SignedAngle(new Vector3(this.startHandRelativePosition.x, 0f, this.startHandRelativePosition.z), vector, Vector3.up);
						float num2 = 50f * (this.startHandRelativePosition.y - relativeHandPosition.y) / this.myRig.scaleFactor;
						float num3 = Vector3.Distance(this.startHandRelativePosition, relativeHandPosition) / this.myRig.scaleFactor;
						this.localEuler += Time.deltaTime * new Vector3(this.activeSettings.verticalSensitivity.Evaluate(num3) * num2, this.activeSettings.horizontalSensitivity.Evaluate(num3) * num, 0f);
						this.startHandRelativePosition = Vector3.MoveTowards(this.startHandRelativePosition, relativeHandPosition, Time.deltaTime * this.activeSettings.inputDecayCurve.Evaluate(num3));
					}
				}
				else
				{
					Quaternion quaternion = this.controllingHand.rotation * this.handRotationOffset;
					Quaternion quaternion2 = this.startHandInverseRotation * quaternion;
					this.localEuler += this.activeSettings.inputSensitivity * quaternion2.eulerAngles;
					float num4 = 1f - Mathf.Exp(-this.activeSettings.inputDecaySpeed * Time.deltaTime);
					this.startHandInverseRotation = Quaternion.Slerp(this.startHandInverseRotation, Quaternion.Inverse(quaternion), num4);
				}
				for (int i = 0; i < 3; i++)
				{
					this.localEuler[i] = this.ReverseClampDegrees(this.localEuler[i], this.lowAngleLimits[i], this.highAngleLimits[i]);
				}
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.Euler(this.localEuler), 1f - Mathf.Exp(-this.activeSettings.rotationSpeed * Time.deltaTime));
				return;
			}
			Quaternion quaternion3 = Quaternion.Slerp(base.transform.localRotation, this.initialRotation, 1f - Mathf.Exp(-this.inactiveSettings.rotationSpeed * Time.deltaTime));
			base.transform.localRotation = quaternion3;
			this.localEuler = quaternion3.eulerAngles;
		}

		// Token: 0x04008DAA RID: 36266
		[SerializeField]
		private HandControlledSettingsSO activeSettings;

		// Token: 0x04008DAB RID: 36267
		[SerializeField]
		private HandControlledSettingsSO inactiveSettings;

		// Token: 0x04008DAC RID: 36268
		[SerializeField]
		private Vector3 handPositionOffset;

		// Token: 0x04008DAD RID: 36269
		[SerializeField]
		private Quaternion rightHandRotation;

		// Token: 0x04008DAE RID: 36270
		[SerializeField]
		private Quaternion leftHandRotation;

		// Token: 0x04008DAF RID: 36271
		private Quaternion handRotationOffset;

		// Token: 0x04008DB0 RID: 36272
		[SerializeField]
		private BezierCurve controlIndicatorCurve;

		// Token: 0x04008DB1 RID: 36273
		[SerializeField]
		private Transform debugRelativePositionTransform1;

		// Token: 0x04008DB2 RID: 36274
		[SerializeField]
		private Transform debugRelativePositionTransform2;

		// Token: 0x04008DB3 RID: 36275
		private VRRig myRig;

		// Token: 0x04008DB4 RID: 36276
		private Transform controllingHand;

		// Token: 0x04008DB5 RID: 36277
		private Vector3 startHandRelativePosition;

		// Token: 0x04008DB6 RID: 36278
		private Vector3 lowAngleLimits;

		// Token: 0x04008DB7 RID: 36279
		private Vector3 highAngleLimits;

		// Token: 0x04008DB8 RID: 36280
		private Vector3 localEuler;

		// Token: 0x04008DB9 RID: 36281
		private Quaternion startHandInverseRotation;

		// Token: 0x04008DBA RID: 36282
		private Quaternion initialRotation;

		// Token: 0x04008DBB RID: 36283
		private bool isActive;

		// Token: 0x02001342 RID: 4930
		public enum RotationControl
		{
			// Token: 0x04008DBE RID: 36286
			Angle,
			// Token: 0x04008DBF RID: 36287
			Translation
		}
	}
}
