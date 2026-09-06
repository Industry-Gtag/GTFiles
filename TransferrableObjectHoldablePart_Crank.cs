using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004D1 RID: 1233
public class TransferrableObjectHoldablePart_Crank : TransferrableObjectHoldablePart
{
	// Token: 0x06001E03 RID: 7683 RVA: 0x000A1A0F File Offset: 0x0009FC0F
	public void SetOnCrankedCallback(Action<float> onCrankedCallback)
	{
		this.onCrankedCallback = onCrankedCallback;
	}

	// Token: 0x06001E04 RID: 7684 RVA: 0x000A1A18 File Offset: 0x0009FC18
	private void Awake()
	{
		if (this.rotatingPart == null)
		{
			this.rotatingPart = base.transform;
		}
		Vector3 vector = this.rotatingPart.parent.InverseTransformPoint(this.rotatingPart.TransformPoint(Vector3.right));
		this.lastAngle = Mathf.Atan2(vector.y, vector.x);
		this.baseLocalAngle = this.rotatingPart.localRotation;
		this.baseLocalAngleInverse = Quaternion.Inverse(this.baseLocalAngle);
		this.crankRadius = new Vector2(this.crankHandleX, this.crankHandleY).magnitude;
		this.crankAngleOffset = Mathf.Atan2(this.crankHandleY, this.crankHandleX) * 57.29578f;
		if (this.crankHandleMaxZ < this.crankHandleMinZ)
		{
			float num = this.crankHandleMaxZ;
			float num2 = this.crankHandleMinZ;
			this.crankHandleMinZ = num;
			this.crankHandleMaxZ = num2;
		}
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x000A1B00 File Offset: 0x0009FD00
	protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
		Vector3 vector4;
		if (rig.isOfflineVRRig)
		{
			Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(isHeldLeftHand);
			Vector3 vector = this.rotatingPart.InverseTransformPoint(controllerTransform.position);
			Vector3 vector2 = (vector.xy().normalized * this.crankRadius).WithZ(Mathf.Clamp(vector.z, this.crankHandleMinZ, this.crankHandleMaxZ));
			Vector3 vector3 = this.rotatingPart.TransformPoint(vector2);
			if (this.maxHandSnapDistance > 0f && (controllerTransform.position - vector3).IsLongerThan(this.maxHandSnapDistance))
			{
				this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
				return;
			}
			controllerTransform.position = vector3;
			vector4 = controllerTransform.position;
		}
		else
		{
			VRMap vrmap = (isHeldLeftHand ? rig.leftHand : rig.rightHand);
			vector4 = vrmap.GetExtrapolatedControllerPosition();
			vector4 -= vrmap.rigTarget.rotation * GTPlayer.Instance.GetHandOffset(isHeldLeftHand) * rig.scaleFactor;
		}
		Vector3 vector5 = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (vector4 - this.rotatingPart.position);
		float num = Mathf.Atan2(vector5.y, vector5.x) * 57.29578f;
		float num2 = Mathf.DeltaAngle(this.lastAngle, num);
		this.lastAngle = num;
		if (num2 != 0f)
		{
			if (this.onCrankedCallback != null)
			{
				this.onCrankedCallback(num2);
			}
			for (int i = 0; i < this.thresholds.Length; i++)
			{
				this.thresholds[i].OnCranked(num2);
			}
		}
		this.rotatingPart.localRotation = this.baseLocalAngle * Quaternion.AngleAxis(num - this.crankAngleOffset, Vector3.forward);
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x000A1D00 File Offset: 0x0009FF00
	private void OnDrawGizmosSelected()
	{
		Transform transform = ((this.rotatingPart != null) ? this.rotatingPart : base.transform);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMinZ)), transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMaxZ)));
	}

	// Token: 0x0400285B RID: 10331
	[SerializeField]
	private float crankHandleX;

	// Token: 0x0400285C RID: 10332
	[SerializeField]
	private float crankHandleY;

	// Token: 0x0400285D RID: 10333
	[SerializeField]
	private float crankHandleMinZ;

	// Token: 0x0400285E RID: 10334
	[SerializeField]
	private float crankHandleMaxZ;

	// Token: 0x0400285F RID: 10335
	[SerializeField]
	private float maxHandSnapDistance;

	// Token: 0x04002860 RID: 10336
	private float crankAngleOffset;

	// Token: 0x04002861 RID: 10337
	private float crankRadius;

	// Token: 0x04002862 RID: 10338
	[SerializeField]
	private Transform rotatingPart;

	// Token: 0x04002863 RID: 10339
	private float lastAngle;

	// Token: 0x04002864 RID: 10340
	private Quaternion baseLocalAngle;

	// Token: 0x04002865 RID: 10341
	private Quaternion baseLocalAngleInverse;

	// Token: 0x04002866 RID: 10342
	private Action<float> onCrankedCallback;

	// Token: 0x04002867 RID: 10343
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank.CrankThreshold[] thresholds;

	// Token: 0x020004D2 RID: 1234
	[Serializable]
	private struct CrankThreshold
	{
		// Token: 0x06001E08 RID: 7688 RVA: 0x000A1D7B File Offset: 0x0009FF7B
		public void OnCranked(float deltaAngle)
		{
			this.currentAngle += deltaAngle;
			if (Mathf.Abs(this.currentAngle) > this.angleThreshold)
			{
				this.currentAngle = 0f;
				this.onReached.Invoke();
			}
		}

		// Token: 0x04002868 RID: 10344
		public float angleThreshold;

		// Token: 0x04002869 RID: 10345
		public UnityEvent onReached;

		// Token: 0x0400286A RID: 10346
		[HideInInspector]
		public float currentAngle;
	}
}
