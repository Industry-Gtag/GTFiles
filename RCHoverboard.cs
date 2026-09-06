using System;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaTag.Cosmetics;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200022D RID: 557
public class RCHoverboard : RCVehicle
{
	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000EBD RID: 3773 RVA: 0x000503BB File Offset: 0x0004E5BB
	// (set) Token: 0x06000EBE RID: 3774 RVA: 0x000503C3 File Offset: 0x0004E5C3
	private float _MaxForwardSpeed
	{
		get
		{
			return this.m_maxForwardSpeed;
		}
		set
		{
			this.m_maxForwardSpeed = value;
			this._forwardAccel = value / math.max(0.01f, this.m_forwardAccelTime);
		}
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06000EBF RID: 3775 RVA: 0x000503E4 File Offset: 0x0004E5E4
	// (set) Token: 0x06000EC0 RID: 3776 RVA: 0x000503EC File Offset: 0x0004E5EC
	private float _MaxTurnRate
	{
		get
		{
			return this.m_maxTurnRate;
		}
		set
		{
			this.m_maxTurnRate = value;
			this._turnAccel = value / math.max(1E-06f, this.m_turnAccelTime);
		}
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06000EC1 RID: 3777 RVA: 0x0005040D File Offset: 0x0004E60D
	// (set) Token: 0x06000EC2 RID: 3778 RVA: 0x00050415 File Offset: 0x0004E615
	private float _MaxTiltAngle
	{
		get
		{
			return this.m_maxTiltAngle;
		}
		set
		{
			this.m_maxTiltAngle = value;
			this._tiltAccel = value / math.max(1E-06f, this.m_tiltTime);
		}
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x00050438 File Offset: 0x0004E638
	protected override void Awake()
	{
		base.Awake();
		this._hasAudioSource = this.m_audioSource != null;
		this._hasHoverSound = this.m_hoverSound != null;
		this._MaxForwardSpeed = this.m_maxForwardSpeed;
		this._MaxTurnRate = this.m_maxTurnRate;
		this._MaxTiltAngle = this.m_maxTiltAngle;
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x00050494 File Offset: 0x0004E694
	protected override void AuthorityBeginDocked()
	{
		base.AuthorityBeginDocked();
		this._currentTurnRate = 0f;
		this._currentTiltAngle = 0f;
		float3 @float = this._ProjectOnPlane(base.transform.forward, math.up());
		this._currentTurnAngle = this._SignedAngle(new float3(0f, 0f, 1f), @float, new float3(0f, 1f, 0f));
		this._motorLevel = 0f;
		if (this._hasAudioSource)
		{
			this.m_audioSource.Stop();
			this.m_audioSource.volume = 0f;
		}
		if (this.connectedRemote == null)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x00050558 File Offset: 0x0004E758
	protected override void AuthorityUpdate(float dt)
	{
		base.AuthorityUpdate(dt);
		if (this.localState == RCVehicle.State.Mobilized)
		{
			float num = math.length(this.activeInput.joystick);
			this._motorLevel = math.saturate(num);
			if (this.hasNetworkSync)
			{
				this.networkSync.syncedState.dataA = (byte)((uint)(this._motorLevel * 255f));
				return;
			}
		}
		else
		{
			this._motorLevel = 0f;
		}
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x000505CC File Offset: 0x0004E7CC
	protected override void RemoteUpdate(float dt)
	{
		base.RemoteUpdate(dt);
		if (this.localState == RCVehicle.State.Mobilized && this.hasNetworkSync)
		{
			this._motorLevel = (float)this.networkSync.syncedState.dataA / 255f;
			return;
		}
		this._motorLevel = 0f;
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x0005061C File Offset: 0x0004E81C
	protected override void SharedUpdate(float dt)
	{
		base.SharedUpdate(dt);
		switch (this.localState)
		{
		case RCVehicle.State.Disabled:
		case RCVehicle.State.DockedLeft:
		case RCVehicle.State.DockedRight:
		case RCVehicle.State.Crashed:
			break;
		case RCVehicle.State.Mobilized:
			if (this._hasAudioSource && this._hasHoverSound)
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.m_audioSource.volume = 0f;
					this.m_audioSource.clip = this.m_hoverSound;
					this.m_audioSource.loop = true;
					this.m_audioSource.GTPlay();
					return;
				}
				float num = math.lerp(this.m_hoverSoundVolumeMinMax.x, this.m_hoverSoundVolumeMinMax.y, this._motorLevel);
				float num2 = this.m_hoverSoundVolumeMinMax.y / this.m_hoverSoundVolumeRampTime * dt;
				this.m_audioSource.volume = this._MoveTowards(this.m_audioSource.volume, num, num2);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x00050700 File Offset: 0x0004E900
	protected void FixedUpdate()
	{
		if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
		{
			return;
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = this.m_inputThrustForward.Get(this.activeInput) - this.m_inputThrustBack.Get(this.activeInput);
		float num2 = this.m_inputTurn.Get(this.activeInput);
		float num3 = this.m_inputJump.Get(this.activeInput);
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 10f, this.raycastLayers, QueryTriggerInteraction.Collide);
		bool flag2 = flag && raycastHit.distance <= this.m_hoverHeight + 0.1f;
		if (this.enableJumpInput && num3 > 0.001f && flag2 && !this._hasJumped)
		{
			this.rb.AddForce(Vector3.up * this.m_jumpForce, ForceMode.Impulse);
			this._hasJumped = true;
		}
		else if (num3 <= 0.001f)
		{
			this._hasJumped = false;
		}
		float num4 = num2 * this._MaxTurnRate;
		this._currentTurnRate = this._MoveTowards(this._currentTurnRate, num4, this._turnAccel * fixedDeltaTime);
		this._currentTurnAngle += this._currentTurnRate * fixedDeltaTime;
		float num5 = math.lerp(-this.m_maxTiltAngle, this.m_maxTiltAngle, math.unlerp(-1f, 1f, num));
		this._currentTiltAngle = this._MoveTowards(this._currentTiltAngle, num5, this._tiltAccel * fixedDeltaTime);
		base.transform.rotation = quaternion.EulerXYZ(math.radians(new float3(this._currentTiltAngle, this._currentTurnAngle, 0f)));
		float3 @float = base.transform.forward;
		float num6 = math.dot(@float, this.rb.linearVelocity);
		float num7 = num * this.m_maxForwardSpeed;
		float num8 = ((math.abs(num7) > 0.001f && ((num7 > 0f && num6 < num7) || (num7 < 0f && num6 > num7))) ? math.sign(num7) : 0f);
		this.rb.AddForce(@float * this._forwardAccel * num8 * this.rb.mass, ForceMode.Force);
		if (flag)
		{
			float num9 = math.saturate(this.m_hoverHeight - raycastHit.distance);
			float num10 = math.dot(this.rb.linearVelocity, Vector3.up);
			float num11 = num9 * this.m_hoverForce - num10 * this.m_hoverDamp;
			this.rb.AddForce(math.up() * num11, ForceMode.Force);
		}
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x000509CC File Offset: 0x0004EBCC
	protected void OnCollisionEnter(Collision collision)
	{
		GameObject gameObject = collision.collider.gameObject;
		bool flag = gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
		bool flag2 = gameObject.IsOnLayer(UnityLayer.GorillaHand);
		if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
		{
			Vector3 vector = Vector3.zero;
			if (flag2)
			{
				GorillaHandClimber component = gameObject.GetComponent<GorillaHandClimber>();
				if (component != null)
				{
					vector = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == XRNode.LeftHand).GetAverageVelocity(true, 0.15f, false);
				}
			}
			else if (collision.rigidbody != null)
			{
				vector = collision.rigidbody.linearVelocity;
			}
			if ((flag || vector.sqrMagnitude > 0.01f) && base.HasLocalAuthority)
			{
				this.AuthorityApplyImpact(vector, flag);
				if (this.networkSync != null)
				{
					this.networkSync.photonView.RPC("HitRCVehicleRPC", RpcTarget.Others, new object[] { vector, flag });
				}
			}
		}
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x00050AC0 File Offset: 0x0004ECC0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _MoveTowards(float current, float target, float maxDelta)
	{
		if (math.abs(target - current) > maxDelta)
		{
			return current + math.sign(target - current) * maxDelta;
		}
		return target;
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x00050ADC File Offset: 0x0004ECDC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _SignedAngle(float3 from, float3 to, float3 axis)
	{
		float3 @float = math.normalize(from);
		float3 float2 = math.normalize(to);
		float num = math.acos(math.dot(@float, float2));
		float num2 = math.sign(math.dot(math.cross(@float, float2), axis));
		return math.degrees(num) * num2;
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x00050B1D File Offset: 0x0004ED1D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float3 _ProjectOnPlane(float3 vector, float3 planeNormal)
	{
		return vector - math.dot(vector, planeNormal) * planeNormal;
	}

	// Token: 0x040011A4 RID: 4516
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputTurn = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.StickX, new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.1f, 0f, 0f, 1.25f, 0f, 0f),
		new Keyframe(0.9f, 1f, 1.25f, 0f, 0f, 0f),
		new Keyframe(1f, 1f, 0f, 0f, 0f, 0f)
	}));

	// Token: 0x040011A5 RID: 4517
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputThrustForward = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.Trigger, AnimationCurves.EaseInCirc);

	// Token: 0x040011A6 RID: 4518
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputThrustBack = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.StickBack, new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.9f, 0f, 0f, 9.9999f, 0.5825f, 0.3767f),
		new Keyframe(1f, 1f, 9.9999f, 1f, 0f, 0f)
	}));

	// Token: 0x040011A7 RID: 4519
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputJump = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.PrimaryFaceButton, AnimationCurves.Linear);

	// Token: 0x040011A8 RID: 4520
	[Tooltip("Desired hover height above ground from this transform's position.")]
	[SerializeField]
	private float m_hoverHeight = 0.2f;

	// Token: 0x040011A9 RID: 4521
	[Tooltip("Upward force to maintain hover when below hoverHeight.")]
	[SerializeField]
	private float m_hoverForce = 200f;

	// Token: 0x040011AA RID: 4522
	[Tooltip("Damping factor to smooth out vertical movement.")]
	[SerializeField]
	private float m_hoverDamp = 5f;

	// Token: 0x040011AB RID: 4523
	[SerializeField]
	private LayerMask raycastLayers = -1;

	// Token: 0x040011AC RID: 4524
	[SerializeField]
	private bool enableJumpInput = true;

	// Token: 0x040011AD RID: 4525
	[Tooltip("Upward impulse force for jump.")]
	[SerializeField]
	private float m_jumpForce = 3.5f;

	// Token: 0x040011AE RID: 4526
	private bool _hasJumped;

	// Token: 0x040011AF RID: 4527
	[SerializeField]
	[HideInInspector]
	private float m_maxForwardSpeed = 6f;

	// Token: 0x040011B0 RID: 4528
	[SerializeField]
	[Tooltip("Time (seconds) to reach max forward speed from zero.")]
	private float m_forwardAccelTime = 2f;

	// Token: 0x040011B1 RID: 4529
	[SerializeField]
	[HideInInspector]
	private float m_maxTurnRate = 720f;

	// Token: 0x040011B2 RID: 4530
	[Tooltip("Time (seconds) to reach max turning rate.")]
	[SerializeField]
	private float m_turnAccelTime = 0.75f;

	// Token: 0x040011B3 RID: 4531
	[SerializeField]
	[HideInInspector]
	private float m_maxTiltAngle = 30f;

	// Token: 0x040011B4 RID: 4532
	[Tooltip("Time (seconds) to reach max tilt angle.")]
	[SerializeField]
	private float m_tiltTime = 0.1f;

	// Token: 0x040011B5 RID: 4533
	[Tooltip("Audio source for any motor or hover sound.")]
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x040011B6 RID: 4534
	[Tooltip("Looping motor/hover sound clip.")]
	[SerializeField]
	private AudioClip m_hoverSound;

	// Token: 0x040011B7 RID: 4535
	[Tooltip("Volume range for the hover sound (x = min, y = max).")]
	[SerializeField]
	private float2 m_hoverSoundVolumeMinMax = new float2(0.1f, 0.5f);

	// Token: 0x040011B8 RID: 4536
	[Tooltip("Time it takes for the volume to reach max value.")]
	[SerializeField]
	private float m_hoverSoundVolumeRampTime = 1f;

	// Token: 0x040011B9 RID: 4537
	private bool _hasAudioSource;

	// Token: 0x040011BA RID: 4538
	private bool _hasHoverSound;

	// Token: 0x040011BB RID: 4539
	private float _forwardAccel;

	// Token: 0x040011BC RID: 4540
	private float _turnAccel;

	// Token: 0x040011BD RID: 4541
	private float _tiltAccel;

	// Token: 0x040011BE RID: 4542
	private float _currentTurnRate;

	// Token: 0x040011BF RID: 4543
	private float _currentTurnAngle;

	// Token: 0x040011C0 RID: 4544
	private float _currentTiltAngle;

	// Token: 0x040011C1 RID: 4545
	private float _motorLevel;

	// Token: 0x0200022E RID: 558
	private enum _EInputSource
	{
		// Token: 0x040011C3 RID: 4547
		None,
		// Token: 0x040011C4 RID: 4548
		StickX,
		// Token: 0x040011C5 RID: 4549
		StickForward,
		// Token: 0x040011C6 RID: 4550
		StickBack,
		// Token: 0x040011C7 RID: 4551
		Trigger,
		// Token: 0x040011C8 RID: 4552
		PrimaryFaceButton
	}

	// Token: 0x0200022F RID: 559
	[Serializable]
	private struct _SingleInputOption
	{
		// Token: 0x06000ECE RID: 3790 RVA: 0x00050D5E File Offset: 0x0004EF5E
		public _SingleInputOption(RCHoverboard._EInputSource source, AnimationCurve remapCurve)
		{
			this.source = new GTOption<StringEnum<RCHoverboard._EInputSource>>(source);
			this.remapCurve = new GTOption<AnimationCurve>(remapCurve);
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x00050D80 File Offset: 0x0004EF80
		public float Get(RCRemoteHoldable.RCInput input)
		{
			float num;
			switch (this.source.ResolvedValue.Value)
			{
			case RCHoverboard._EInputSource.None:
				num = 0f;
				break;
			case RCHoverboard._EInputSource.StickX:
				num = input.joystick.x;
				break;
			case RCHoverboard._EInputSource.StickForward:
				num = math.saturate(input.joystick.y);
				break;
			case RCHoverboard._EInputSource.StickBack:
				num = math.saturate(-input.joystick.y);
				break;
			case RCHoverboard._EInputSource.Trigger:
				num = input.trigger;
				break;
			case RCHoverboard._EInputSource.PrimaryFaceButton:
				num = (float)input.buttons;
				break;
			default:
				num = 0f;
				break;
			}
			float num2 = num;
			return this.remapCurve.ResolvedValue.Evaluate(math.abs(num2)) * math.sign(num2);
		}

		// Token: 0x040011C9 RID: 4553
		public GTOption<StringEnum<RCHoverboard._EInputSource>> source;

		// Token: 0x040011CA RID: 4554
		public GTOption<AnimationCurve> remapCurve;
	}
}
