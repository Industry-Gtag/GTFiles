using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000699 RID: 1689
public class FishingRod : TransferrableObject
{
	// Token: 0x06002A12 RID: 10770 RVA: 0x000E2C70 File Offset: 0x000E0E70
	public override void OnActivate()
	{
		base.OnActivate();
		Transform transform = base.transform;
		Vector3 vector = transform.up + transform.forward * 640f;
		this.bobRigidbody.AddForce(vector, ForceMode.Impulse);
		this.line.tensionScale = 0.86f;
		this.ReelOut();
	}

	// Token: 0x06002A13 RID: 10771 RVA: 0x000E2CC9 File Offset: 0x000E0EC9
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.line.tensionScale = 1f;
		this.ReelStop();
	}

	// Token: 0x06002A14 RID: 10772 RVA: 0x000E2CE7 File Offset: 0x000E0EE7
	protected override void Start()
	{
		base.Start();
		this.rig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06002A15 RID: 10773 RVA: 0x000E2CFB File Offset: 0x000E0EFB
	public void SetBobFloat(bool enable)
	{
		if (!this.bobRigidbody)
		{
			return;
		}
		this._bobFloatPlaneY = this.bobRigidbody.position.y;
		this._bobFloating = enable;
	}

	// Token: 0x06002A16 RID: 10774 RVA: 0x000E2D28 File Offset: 0x000E0F28
	private void QuickReel()
	{
		if (this._lineResizing)
		{
			return;
		}
		this.bobCollider.enabled = false;
		this.ReelIn();
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x000E2D48 File Offset: 0x000E0F48
	public bool IsFreeHandGripping()
	{
		bool flag = base.InLeftHand();
		Transform transform = (flag ? this.rig.rightHandTransform : this.rig.leftHandTransform);
		float magnitude = (this.reelToSync.position - transform.position).magnitude;
		bool flag2 = this._grippingHand || magnitude <= 0.16f;
		this.disableStealing = flag2;
		if (!flag2)
		{
			return false;
		}
		VRMapThumb vrmapThumb = (flag ? this.rig.rightThumb : this.rig.leftThumb);
		VRMapIndex vrmapIndex = (flag ? this.rig.rightIndex : this.rig.leftIndex);
		VRMap vrmap = (flag ? this.rig.rightMiddle : this.rig.leftMiddle);
		float calcT = vrmapThumb.calcT;
		float calcT2 = vrmapIndex.calcT;
		float calcT3 = vrmap.calcT;
		bool flag3 = calcT >= 0.1f && calcT2 >= 0.2f && calcT3 >= 0.2f;
		this._grippingHand = (flag3 ? transform : null);
		return flag3;
	}

	// Token: 0x06002A18 RID: 10776 RVA: 0x000E2E61 File Offset: 0x000E1061
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (this._grippingHand)
		{
			this._grippingHand = null;
		}
		this.ResetLineLength(this.lineLengthMin * 1.32f);
		return true;
	}

	// Token: 0x06002A19 RID: 10777 RVA: 0x000E2E98 File Offset: 0x000E1098
	public void ReelIn()
	{
		this._manualReeling = false;
		FishingRod.SetHandleMotorUse(true, this.reelSpinRate, this.handleJoint, true);
		this._lineResizing = true;
		this._lineExpanding = false;
		float num = (float)this.line.segmentNumber + 0.0001f;
		this.line.segmentMinLength = (this._targetSegmentMin = this.lineLengthMin / num);
		this.line.segmentMaxLength = (this._targetSegmentMax = this.lineLengthMax / num);
	}

	// Token: 0x06002A1A RID: 10778 RVA: 0x000E2F18 File Offset: 0x000E1118
	public void ReelOut()
	{
		this._manualReeling = false;
		FishingRod.SetHandleMotorUse(true, this.reelSpinRate, this.handleJoint, false);
		this._lineResizing = true;
		this._lineExpanding = true;
		float num = (float)this.line.segmentNumber + 0.0001f;
		this.line.segmentMinLength = (this._targetSegmentMin = this.lineLengthMin / num);
		this.line.segmentMaxLength = (this._targetSegmentMax = this.lineLengthMax / num);
	}

	// Token: 0x06002A1B RID: 10779 RVA: 0x000E2F98 File Offset: 0x000E1198
	public void ReelStop()
	{
		if (this._manualReeling)
		{
			this._localRotDelta = 0f;
		}
		else
		{
			FishingRod.SetHandleMotorUse(false, 0f, this.handleJoint, false);
		}
		this.bobCollider.enabled = true;
		if (this.line)
		{
			this.line.resizeScale = 1f;
		}
		this._lineResizing = false;
		this._lineExpanding = false;
	}

	// Token: 0x06002A1C RID: 10780 RVA: 0x000E3004 File Offset: 0x000E1204
	private static void SetHandleMotorUse(bool useMotor, float spinRate, HingeJoint handleJoint, bool reverse)
	{
		JointMotor motor = handleJoint.motor;
		motor.force = (useMotor ? 1f : 0f) * spinRate;
		motor.targetVelocity = 16384f * (reverse ? (-1f) : 1f);
		handleJoint.motor = motor;
	}

	// Token: 0x06002A1D RID: 10781 RVA: 0x000E3054 File Offset: 0x000E1254
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		this._manualReeling = (this._isGrippingHandle = this.IsFreeHandGripping());
		if (ControllerInputPoller.instance && ControllerInputPoller.PrimaryButtonPress(base.InLeftHand() ? XRNode.LeftHand : XRNode.RightHand))
		{
			this.QuickReel();
		}
		if (this._lineResetting && this._sinceReset.HasElapsed(this.line.resizeSpeed))
		{
			this.bobCollider.enabled = true;
			this._lineResetting = false;
		}
		this.handleTransform.localPosition = this.reelFreezeLocalPosition;
	}

	// Token: 0x06002A1E RID: 10782 RVA: 0x000E30E7 File Offset: 0x000E12E7
	private void ResetLineLength(float length)
	{
		if (!this.line)
		{
			return;
		}
		this._lineResetting = true;
		this.bobCollider.enabled = false;
		this.line.ForceTotalLength(length);
		this._sinceReset = TimeSince.Now();
	}

	// Token: 0x06002A1F RID: 10783 RVA: 0x000E3124 File Offset: 0x000E1324
	private void FixedUpdate()
	{
		Transform transform = base.transform;
		this.handleRigidbody.useGravity = !this._manualReeling;
		if (this._bobFloating && this.bobRigidbody)
		{
			float y = this.bobRigidbody.position.y;
			float num = this.bobFloatForce * this.bobRigidbody.mass;
			float num2 = num * Mathf.Clamp01(this._bobFloatPlaneY - y);
			num += num2;
			if (y <= this._bobFloatPlaneY)
			{
				this.bobRigidbody.AddForce(0f, num, 0f);
			}
		}
		if (this._manualReeling)
		{
			if (this._isGrippingHandle && this._grippingHand)
			{
				this.reelTo.position = this._grippingHand.position;
			}
			Vector3 vector = this.reelFrom.InverseTransformPoint(this.reelTo.position);
			vector.x = 0f;
			vector.Normalize();
			vector *= 2f;
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, vector);
			quaternion = (base.InRightHand() ? quaternion : Quaternion.Inverse(quaternion));
			this._localRotDelta = FishingRod.GetSignedDeltaYZ(ref this._lastLocalRot, ref quaternion);
			this._lastLocalRot = quaternion;
			Quaternion quaternion2 = transform.rotation * quaternion;
			this.handleRigidbody.MoveRotation(quaternion2);
		}
		else
		{
			this.reelTo.localPosition = transform.InverseTransformPoint(this.reelToSync.position);
		}
		if (!this.line)
		{
			return;
		}
		if (this._manualReeling)
		{
			this._lineResizing = Mathf.Abs(this._localRotDelta) >= 0.001f;
			this._lineExpanding = Mathf.Sign(this._localRotDelta) >= 0f;
		}
		if (!this._lineResizing)
		{
			return;
		}
		float num3 = (this._manualReeling ? (Mathf.Abs(this._localRotDelta) * 0.66f * Time.fixedDeltaTime) : (this.lineResizeRate * this.lineCastFactor));
		this.line.resizeScale = this.lineCastFactor;
		float num4 = num3 * Time.fixedDeltaTime;
		float num5 = this.line.segmentTargetLength;
		if (this._manualReeling)
		{
			float num6 = 1f / ((float)this.line.segmentNumber + 0.0001f);
			float num7 = this.lineLengthMin * num6;
			float num8 = this.lineLengthMax * num6;
			num4 *= (this._lineExpanding ? 1f : (-1f));
			num4 *= (base.InRightHand() ? (-1f) : 1f);
			float num9 = num5 + num4;
			if (num9 > num7 && num9 < num8)
			{
				num5 += num4;
			}
		}
		else if (this._lineExpanding)
		{
			if (num5 < this._targetSegmentMax)
			{
				num5 += num4;
			}
			else
			{
				this._lineResizing = false;
			}
		}
		else if (num5 > this._targetSegmentMin)
		{
			num5 -= num4;
		}
		else
		{
			this._lineResizing = false;
		}
		if (this._lineResizing)
		{
			this.line.segmentTargetLength = num5;
			return;
		}
		this.ReelStop();
	}

	// Token: 0x06002A20 RID: 10784 RVA: 0x000E341C File Offset: 0x000E161C
	private static float GetSignedDeltaYZ(ref Quaternion a, ref Quaternion b)
	{
		Vector3 forward = Vector3.forward;
		Vector3 vector = a * forward;
		Vector3 vector2 = b * forward;
		float num = Mathf.Atan2(vector.y, vector.z) * 57.29578f;
		float num2 = Mathf.Atan2(vector2.y, vector2.z) * 57.29578f;
		return Mathf.DeltaAngle(num, num2);
	}

	// Token: 0x040036BF RID: 14015
	public Transform handleTransform;

	// Token: 0x040036C0 RID: 14016
	public HingeJoint handleJoint;

	// Token: 0x040036C1 RID: 14017
	public Rigidbody handleRigidbody;

	// Token: 0x040036C2 RID: 14018
	public BoxCollider handleCollider;

	// Token: 0x040036C3 RID: 14019
	public Rigidbody bobRigidbody;

	// Token: 0x040036C4 RID: 14020
	public Collider bobCollider;

	// Token: 0x040036C5 RID: 14021
	public VerletLine line;

	// Token: 0x040036C6 RID: 14022
	public GorillaVelocityEstimator tipTracker;

	// Token: 0x040036C7 RID: 14023
	public Rigidbody tipBody;

	// Token: 0x040036C8 RID: 14024
	[NonSerialized]
	public VRRig rig;

	// Token: 0x040036C9 RID: 14025
	[Space]
	public Vector3 reelFreezeLocalPosition;

	// Token: 0x040036CA RID: 14026
	public Transform reelFrom;

	// Token: 0x040036CB RID: 14027
	public Transform reelTo;

	// Token: 0x040036CC RID: 14028
	public Transform reelToSync;

	// Token: 0x040036CD RID: 14029
	[Space]
	public float reelSpinRate = 1f;

	// Token: 0x040036CE RID: 14030
	public float lineResizeRate = 1f;

	// Token: 0x040036CF RID: 14031
	public float lineCastFactor = 3f;

	// Token: 0x040036D0 RID: 14032
	public float lineLengthMin = 0.1f;

	// Token: 0x040036D1 RID: 14033
	public float lineLengthMax = 8f;

	// Token: 0x040036D2 RID: 14034
	[Space]
	[NonSerialized]
	private bool _bobFloating;

	// Token: 0x040036D3 RID: 14035
	public float bobFloatForce = 8f;

	// Token: 0x040036D4 RID: 14036
	public float bobStaticDrag = 3.2f;

	// Token: 0x040036D5 RID: 14037
	public float bobDynamicDrag = 1.1f;

	// Token: 0x040036D6 RID: 14038
	[NonSerialized]
	private float _bobFloatPlaneY;

	// Token: 0x040036D7 RID: 14039
	[Space]
	[NonSerialized]
	private float _targetSegmentMin;

	// Token: 0x040036D8 RID: 14040
	[NonSerialized]
	private float _targetSegmentMax;

	// Token: 0x040036D9 RID: 14041
	[Space]
	[NonSerialized]
	private bool _manualReeling;

	// Token: 0x040036DA RID: 14042
	[NonSerialized]
	private bool _lineResizing;

	// Token: 0x040036DB RID: 14043
	[NonSerialized]
	private bool _lineExpanding;

	// Token: 0x040036DC RID: 14044
	[NonSerialized]
	private bool _lineResetting;

	// Token: 0x040036DD RID: 14045
	[NonSerialized]
	private TimeSince _sinceReset;

	// Token: 0x040036DE RID: 14046
	[Space]
	[NonSerialized]
	private Quaternion _lastLocalRot = Quaternion.identity;

	// Token: 0x040036DF RID: 14047
	[NonSerialized]
	private float _localRotDelta;

	// Token: 0x040036E0 RID: 14048
	[NonSerialized]
	private bool _isGrippingHandle;

	// Token: 0x040036E1 RID: 14049
	[NonSerialized]
	private Transform _grippingHand;

	// Token: 0x040036E2 RID: 14050
	private TimeSince _sinceGripLoss;
}
