using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class ArtilleryCrank : HoldableObject
{
	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000ACC RID: 2764 RVA: 0x0003A018 File Offset: 0x00038218
	public bool IsHeld
	{
		get
		{
			return this.isHeld;
		}
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000ACD RID: 2765 RVA: 0x0003A020 File Offset: 0x00038220
	public bool IsHeldLeftHand
	{
		get
		{
			return this.isHeldLeftHand;
		}
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000ACE RID: 2766 RVA: 0x0003A028 File Offset: 0x00038228
	public float CurrentAngle
	{
		get
		{
			return this.currentAngle;
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000ACF RID: 2767 RVA: 0x0003A030 File Offset: 0x00038230
	private int CrankIndex
	{
		get
		{
			if (this.crankType != ArtilleryCrankType.Pitch)
			{
				return 1;
			}
			return 0;
		}
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x0003A040 File Offset: 0x00038240
	private void Awake()
	{
		if (this.rotatingPart == null)
		{
			this.rotatingPart = base.transform;
		}
		Vector3 vector = this.rotatingPart.parent.InverseTransformPoint(this.rotatingPart.TransformPoint(Vector3.right));
		this.lastAngle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
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

	// Token: 0x06000AD1 RID: 2769 RVA: 0x0003A130 File Offset: 0x00038330
	private void LateUpdate()
	{
		if (!this.isHeld)
		{
			return;
		}
		if (!this.cannon.IsCrankHeldLocally(this.CrankIndex))
		{
			this.DropItemCleanup();
			return;
		}
		Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(this.isHeldLeftHand);
		Vector3 vector = this.rotatingPart.InverseTransformPoint(controllerTransform.position);
		Vector3 vector2 = (vector.xy().normalized * this.crankRadius).WithZ(Mathf.Clamp(vector.z, this.crankHandleMinZ, this.crankHandleMaxZ));
		Vector3 vector3 = this.rotatingPart.TransformPoint(vector2);
		if (this.maxHandSnapDistance > 0f && (controllerTransform.position - vector3).IsLongerThan(this.maxHandSnapDistance))
		{
			this.OnRelease(null, this.isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			return;
		}
		controllerTransform.position = vector3;
		float num = this.ComputeAngleFromWorldPos(controllerTransform.position);
		float num2 = Mathf.DeltaAngle(this.lastAngle, num);
		this.lastAngle = num;
		this.currentAngle = num;
		if (num2 != 0f)
		{
			this.cannon.OnCrankInput(this.CrankIndex, num2);
		}
		this.ApplyVisualAngle(num);
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x0003A270 File Offset: 0x00038470
	public void UpdateFromRemoteHand(VRRig rig, bool leftHand)
	{
		VRMap vrmap = (leftHand ? rig.leftHand : rig.rightHand);
		Vector3 vector = vrmap.GetExtrapolatedControllerPosition();
		vector -= vrmap.rigTarget.rotation * GTPlayer.Instance.GetHandOffset(leftHand) * rig.scaleFactor;
		float num = this.ComputeAngleFromWorldPos(vector);
		this.currentAngle = num;
		this.ApplyVisualAngle(num);
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0003A2D9 File Offset: 0x000384D9
	public void SetVisualAngle(float angle)
	{
		if (this.rotatingPart != null)
		{
			this.currentAngle = angle;
			this.ApplyVisualAngle(angle);
		}
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x0003A2F8 File Offset: 0x000384F8
	private float ComputeAngleFromWorldPos(Vector3 worldPos)
	{
		Vector3 vector = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (worldPos - this.rotatingPart.position);
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0003A353 File Offset: 0x00038553
	private void ApplyVisualAngle(float angle)
	{
		this.rotatingPart.localRotation = this.baseLocalAngle * Quaternion.AngleAxis(angle - this.crankAngleOffset, Vector3.forward);
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0003A380 File Offset: 0x00038580
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.isHeldLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (!this.cannon.OnCrankGrabbed(this.CrankIndex, this.isHeldLeftHand))
		{
			return;
		}
		this.isHeld = true;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(this.isHeldLeftHand);
		Vector3 vector = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (controllerTransform.position - this.rotatingPart.position);
		this.lastAngle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x0003A443 File Offset: 0x00038643
	public override void DropItemCleanup()
	{
		if (this.isHeld)
		{
			this.isHeld = false;
			this.cannon.OnCrankReleased(this.CrankIndex, this.currentAngle);
		}
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0003A46C File Offset: 0x0003866C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		if (this.isHeld)
		{
			this.isHeld = false;
			this.cannon.OnCrankReleased(this.CrankIndex, this.currentAngle);
		}
		return true;
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x0003A4C0 File Offset: 0x000386C0
	private void OnDrawGizmosSelected()
	{
		Transform transform = ((this.rotatingPart != null) ? this.rotatingPart : base.transform);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMinZ)), transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMaxZ)));
	}

	// Token: 0x04000D12 RID: 3346
	[SerializeField]
	private ArtilleryCannon cannon;

	// Token: 0x04000D13 RID: 3347
	[SerializeField]
	private ArtilleryCrankType crankType;

	// Token: 0x04000D14 RID: 3348
	[SerializeField]
	private float crankHandleX;

	// Token: 0x04000D15 RID: 3349
	[SerializeField]
	private float crankHandleY;

	// Token: 0x04000D16 RID: 3350
	[SerializeField]
	private float crankHandleMinZ;

	// Token: 0x04000D17 RID: 3351
	[SerializeField]
	private float crankHandleMaxZ;

	// Token: 0x04000D18 RID: 3352
	[SerializeField]
	private float maxHandSnapDistance;

	// Token: 0x04000D19 RID: 3353
	[SerializeField]
	private Transform rotatingPart;

	// Token: 0x04000D1A RID: 3354
	private float crankAngleOffset;

	// Token: 0x04000D1B RID: 3355
	private float crankRadius;

	// Token: 0x04000D1C RID: 3356
	private float lastAngle;

	// Token: 0x04000D1D RID: 3357
	private float currentAngle;

	// Token: 0x04000D1E RID: 3358
	private Quaternion baseLocalAngle;

	// Token: 0x04000D1F RID: 3359
	private Quaternion baseLocalAngleInverse;

	// Token: 0x04000D20 RID: 3360
	private bool isHeld;

	// Token: 0x04000D21 RID: 3361
	private bool isHeldLeftHand;
}
