using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000198 RID: 408
public class BatteryChargerCrank : HoldableObject
{
	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000AF6 RID: 2806 RVA: 0x0003ABAE File Offset: 0x00038DAE
	public bool IsHeld
	{
		get
		{
			return this.isHeld;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000AF7 RID: 2807 RVA: 0x0003ABB6 File Offset: 0x00038DB6
	public bool IsHeldLeftHand
	{
		get
		{
			return this.isHeldLeftHand;
		}
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000AF8 RID: 2808 RVA: 0x0003ABBE File Offset: 0x00038DBE
	public float CurrentAngle
	{
		get
		{
			return this.currentAngle;
		}
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000AF9 RID: 2809 RVA: 0x0003ABC6 File Offset: 0x00038DC6
	internal int CrankIndex
	{
		get
		{
			return this.crankIndex;
		}
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0003ABD0 File Offset: 0x00038DD0
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

	// Token: 0x06000AFB RID: 2811 RVA: 0x0003ACBD File Offset: 0x00038EBD
	private void Start()
	{
		this.crankIndex = this.charger.RegisterCrank(this);
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x0003ACD4 File Offset: 0x00038ED4
	private void LateUpdate()
	{
		if (!this.isHeld || this.crankIndex < 0)
		{
			return;
		}
		if (!this.charger.IsCrankHeldLocally(this.crankIndex))
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
			this.charger.OnCrankInput(this.crankIndex, num2);
			GorillaTagger.Instance.DoVibration(this.isHeldLeftHand ? XRNode.LeftHand : XRNode.RightHand, Mathf.Abs(num2 / 30f) * this.vibrationAmplitude, Time.deltaTime);
		}
		this.UpdateCrankSound(num2);
		this.ApplyVisualAngle(num);
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0003AE54 File Offset: 0x00039054
	public void UpdateFromRemoteHand(VRRig rig, bool leftHand)
	{
		VRMap vrmap = (leftHand ? rig.leftHand : rig.rightHand);
		Vector3 vector = vrmap.GetExtrapolatedControllerPosition();
		vector -= vrmap.rigTarget.rotation * GTPlayer.Instance.GetHandOffset(leftHand) * rig.scaleFactor;
		float num = this.ComputeAngleFromWorldPos(vector);
		this.currentAngle = num;
		this.ApplyVisualAngle(num);
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0003AEBD File Offset: 0x000390BD
	public void SetVisualAngle(float angle)
	{
		if (this.rotatingPart != null)
		{
			this.currentAngle = angle;
			this.ApplyVisualAngle(angle);
		}
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0003AEDC File Offset: 0x000390DC
	private float ComputeAngleFromWorldPos(Vector3 worldPos)
	{
		Vector3 vector = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (worldPos - this.rotatingPart.position);
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0003AF37 File Offset: 0x00039137
	private void ApplyVisualAngle(float angle)
	{
		this.rotatingPart.localRotation = this.baseLocalAngle * Quaternion.AngleAxis(angle - this.crankAngleOffset, Vector3.forward);
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x0003AF64 File Offset: 0x00039164
	private void UpdateCrankSound(float crankAmount)
	{
		if (this.crankSound == null)
		{
			return;
		}
		float num = Mathf.Abs(crankAmount / 30f) * this.vibrationAmplitude;
		this.smoothCrankSpeed = Mathf.Lerp(this.smoothCrankSpeed, num, 10f * Time.deltaTime);
		if (this.smoothCrankSpeed > 0.01f)
		{
			if (!this.crankSound.isPlaying)
			{
				this.crankSound.Play();
			}
			float num2 = Mathf.Clamp01(this.smoothCrankSpeed);
			this.crankSound.pitch = Mathf.Lerp(this.crankSoundMinPitch, this.crankSoundMaxPitch, num2);
			return;
		}
		if (this.crankSound.isPlaying)
		{
			this.crankSound.Stop();
			this.smoothCrankSpeed = 0f;
		}
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x0003B023 File Offset: 0x00039223
	private void StopCrankSound()
	{
		if (this.crankSound != null && this.crankSound.isPlaying)
		{
			this.crankSound.Stop();
		}
		this.smoothCrankSpeed = 0f;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x0003B058 File Offset: 0x00039258
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (this.crankIndex < 0)
		{
			return;
		}
		this.isHeldLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (!this.charger.OnCrankGrabbed(this.crankIndex, this.isHeldLeftHand))
		{
			return;
		}
		this.isHeld = true;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(this.isHeldLeftHand);
		Vector3 vector = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (controllerTransform.position - this.rotatingPart.position);
		this.lastAngle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0003B125 File Offset: 0x00039325
	public override void DropItemCleanup()
	{
		if (this.isHeld)
		{
			this.isHeld = false;
			this.StopCrankSound();
			this.charger.OnCrankReleased(this.crankIndex, this.currentAngle);
		}
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x0003B154 File Offset: 0x00039354
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
			this.charger.OnCrankReleased(this.crankIndex, this.currentAngle);
		}
		return true;
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x0003B1A8 File Offset: 0x000393A8
	private void OnDrawGizmosSelected()
	{
		Transform transform = ((this.rotatingPart != null) ? this.rotatingPart : base.transform);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMinZ)), transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMaxZ)));
	}

	// Token: 0x04000D38 RID: 3384
	[SerializeField]
	private BatteryCharger charger;

	// Token: 0x04000D39 RID: 3385
	[SerializeField]
	private float crankHandleX;

	// Token: 0x04000D3A RID: 3386
	[SerializeField]
	private float crankHandleY;

	// Token: 0x04000D3B RID: 3387
	[SerializeField]
	private float crankHandleMinZ;

	// Token: 0x04000D3C RID: 3388
	[SerializeField]
	private float crankHandleMaxZ;

	// Token: 0x04000D3D RID: 3389
	[SerializeField]
	private float maxHandSnapDistance;

	// Token: 0x04000D3E RID: 3390
	[SerializeField]
	private Transform rotatingPart;

	// Token: 0x04000D3F RID: 3391
	[SerializeField]
	private float vibrationAmplitude = 0.3f;

	// Token: 0x04000D40 RID: 3392
	[SerializeField]
	private AudioSource crankSound;

	// Token: 0x04000D41 RID: 3393
	[SerializeField]
	private float crankSoundMinPitch = 0.6f;

	// Token: 0x04000D42 RID: 3394
	[SerializeField]
	private float crankSoundMaxPitch = 1.4f;

	// Token: 0x04000D43 RID: 3395
	private float crankAngleOffset;

	// Token: 0x04000D44 RID: 3396
	private float crankRadius;

	// Token: 0x04000D45 RID: 3397
	private float lastAngle;

	// Token: 0x04000D46 RID: 3398
	private float currentAngle;

	// Token: 0x04000D47 RID: 3399
	private float smoothCrankSpeed;

	// Token: 0x04000D48 RID: 3400
	private Quaternion baseLocalAngle;

	// Token: 0x04000D49 RID: 3401
	private Quaternion baseLocalAngleInverse;

	// Token: 0x04000D4A RID: 3402
	private int crankIndex = -1;

	// Token: 0x04000D4B RID: 3403
	private bool isHeld;

	// Token: 0x04000D4C RID: 3404
	private bool isHeldLeftHand;
}
