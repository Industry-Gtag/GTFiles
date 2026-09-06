using System;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012D3 RID: 4819
	public class RCBlimp : RCVehicle
	{
		// Token: 0x060078B5 RID: 30901 RVA: 0x00273774 File Offset: 0x00271974
		protected override void AuthorityBeginDocked()
		{
			base.AuthorityBeginDocked();
			this.turnRate = 0f;
			this.turnAngle = Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(base.transform.forward, Vector3.up), Vector3.up);
			this.motorLevel = 0f;
			if (this.connectedRemote == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060078B6 RID: 30902 RVA: 0x002737E4 File Offset: 0x002719E4
		protected override void Awake()
		{
			base.Awake();
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
			this.tiltAccel = this.maxHorizontalTiltAngle / this.horizontalTiltTime;
		}

		// Token: 0x060078B7 RID: 30903 RVA: 0x00273843 File Offset: 0x00271A43
		protected override void OnDisable()
		{
			base.OnDisable();
			this.audioSource.GTStop();
		}

		// Token: 0x060078B8 RID: 30904 RVA: 0x00273858 File Offset: 0x00271A58
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
			}
		}

		// Token: 0x060078B9 RID: 30905 RVA: 0x00273900 File Offset: 0x00271B00
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.localState == RCVehicle.State.Mobilized && this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
			}
		}

		// Token: 0x060078BA RID: 30906 RVA: 0x00273950 File Offset: 0x00271B50
		protected override void SharedUpdate(float dt)
		{
			base.SharedUpdate(dt);
			switch (this.localState)
			{
			case RCVehicle.State.Disabled:
				break;
			case RCVehicle.State.DockedLeft:
			case RCVehicle.State.DockedRight:
				if (this.localStatePrev != RCVehicle.State.DockedLeft && this.localStatePrev != RCVehicle.State.DockedRight)
				{
					this.audioSource.GTStop();
					this.blimpDeflateBlendWeight = 0f;
					this.blimpMesh.SetBlendShapeWeight(0, 0f);
					this.crashCollider.enabled = false;
				}
				this.leftPropellerSpinRate = Mathf.MoveTowards(this.leftPropellerSpinRate, 0.6f, 6.6666665f * dt);
				this.rightPropellerSpinRate = Mathf.MoveTowards(this.rightPropellerSpinRate, 0.6f, 6.6666665f * dt);
				this.leftPropellerAngle += this.leftPropellerSpinRate * 360f * dt;
				this.rightPropellerAngle += this.rightPropellerSpinRate * 360f * dt;
				this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.leftPropellerAngle, 0f, -90f));
				this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.rightPropellerAngle, 0f, 90f));
				return;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.audioSource.loop = true;
					this.audioSource.clip = this.motorSound;
					this.audioSource.volume = 0f;
					this.audioSource.GTPlay();
					this.blimpDeflateBlendWeight = 0f;
					this.blimpMesh.SetBlendShapeWeight(0, 0f);
					this.crashCollider.enabled = false;
				}
				float num = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, num, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				this.blimpDeflateBlendWeight = 0f;
				float num2 = this.activeInput.joystick.y * 5f;
				float num3 = this.activeInput.joystick.x * 5f;
				float num4 = Mathf.Clamp(num3 + num2 + 0.6f, -5f, 5f);
				float num5 = Mathf.Clamp(-num3 + num2 + 0.6f, -5f, 5f);
				this.leftPropellerSpinRate = Mathf.MoveTowards(this.leftPropellerSpinRate, num4, 6.6666665f * dt);
				this.rightPropellerSpinRate = Mathf.MoveTowards(this.rightPropellerSpinRate, num5, 6.6666665f * dt);
				this.leftPropellerAngle += this.leftPropellerSpinRate * 360f * dt;
				this.rightPropellerAngle += this.rightPropellerSpinRate * 360f * dt;
				this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.leftPropellerAngle, 0f, -90f));
				this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.rightPropellerAngle, 0f, 90f));
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.audioSource.GTStop();
					this.audioSource.clip = null;
					this.audioSource.loop = false;
					this.audioSource.volume = this.deflateSoundVolume;
					if (this.deflateSound != null)
					{
						this.audioSource.GTPlayOneShot(this.deflateSound, 1f);
					}
					this.leftPropellerSpinRate = 0f;
					this.rightPropellerSpinRate = 0f;
					this.leftPropellerAngle = 0f;
					this.rightPropellerAngle = 0f;
					this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
					this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
					this.crashCollider.enabled = true;
				}
				this.blimpDeflateBlendWeight = Mathf.Lerp(1f, this.blimpDeflateBlendWeight, Mathf.Exp(-this.deflateRate * dt));
				this.blimpMesh.SetBlendShapeWeight(0, this.blimpDeflateBlendWeight * 100f);
				return;
			default:
				return;
			}
		}

		// Token: 0x060078BB RID: 30907 RVA: 0x00273DA0 File Offset: 0x00271FA0
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			float x = base.transform.lossyScale.x;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = this.maxAscendSpeed * x;
				float num2 = this.maxHorizontalSpeed * x;
				float num3 = this.ascendAccel * x;
				Vector3 linearVelocity = this.rb.linearVelocity;
				Vector3 normalized = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z).normalized;
				this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
				this.tiltAngle = Vector3.SignedAngle(normalized, base.transform.forward, base.transform.right);
				float num4 = this.activeInput.joystick.x * this.maxTurnRate;
				this.turnRate = Mathf.MoveTowards(this.turnRate, num4, this.turnAccel * fixedDeltaTime);
				this.turnAngle += this.turnRate * fixedDeltaTime;
				float num5 = Vector3.Dot(normalized, linearVelocity);
				float num6 = Mathf.InverseLerp(-num2, num2, num5);
				float num7 = Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, num6);
				this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, num7, this.tiltAccel * fixedDeltaTime);
				base.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0f));
				Vector3 vector = new Vector3(linearVelocity.x, 0f, linearVelocity.z);
				Vector3 vector2 = Vector3.Lerp(normalized * this.activeInput.joystick.y * num2, vector, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime));
				this.rb.AddForce((vector2 - vector) * this.rb.mass, ForceMode.Impulse);
				float num8 = this.activeInput.trigger * num;
				if (num8 > 0.01f && linearVelocity.y < num8)
				{
					this.rb.AddForce(Vector3.up * num3 * this.rb.mass, ForceMode.Force);
				}
				if (this.rb.useGravity)
				{
					RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.gravityCompensation);
					return;
				}
			}
			else if (this.localState == RCVehicle.State.Crashed && this.rb.useGravity)
			{
				RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.crashedGravityCompensation);
			}
		}

		// Token: 0x060078BC RID: 30908 RVA: 0x00274030 File Offset: 0x00272230
		private void OnTriggerEnter(Collider other)
		{
			bool flag = other.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
			bool flag2 = other.gameObject.IsOnLayer(UnityLayer.GorillaHand);
			if (!other.isTrigger && base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginCrash();
				return;
			}
			if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
			{
				Vector3 vector = Vector3.zero;
				if (flag2)
				{
					GorillaHandClimber component = other.gameObject.GetComponent<GorillaHandClimber>();
					if (component != null)
					{
						vector = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == XRNode.LeftHand).GetAverageVelocity(true, 0.15f, false);
					}
				}
				else if (other.attachedRigidbody != null)
				{
					vector = other.attachedRigidbody.linearVelocity;
				}
				if (flag || vector.sqrMagnitude > 0.01f)
				{
					if (base.HasLocalAuthority)
					{
						this.AuthorityApplyImpact(vector, flag);
						return;
					}
					if (this.networkSync != null)
					{
						this.networkSync.photonView.RPC("HitRCVehicleRPC", RpcTarget.Others, new object[] { vector, flag });
					}
				}
			}
		}

		// Token: 0x04008932 RID: 35122
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x04008933 RID: 35123
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x04008934 RID: 35124
		[SerializeField]
		private float gravityCompensation = 0.9f;

		// Token: 0x04008935 RID: 35125
		[SerializeField]
		private float crashedGravityCompensation = 0.5f;

		// Token: 0x04008936 RID: 35126
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04008937 RID: 35127
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x04008938 RID: 35128
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x04008939 RID: 35129
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x0400893A RID: 35130
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x0400893B RID: 35131
		[SerializeField]
		private float horizontalTiltTime = 2f;

		// Token: 0x0400893C RID: 35132
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.1f, 0.8f);

		// Token: 0x0400893D RID: 35133
		[SerializeField]
		private float deflateSoundVolume = 0.1f;

		// Token: 0x0400893E RID: 35134
		[SerializeField]
		private Collider crashCollider;

		// Token: 0x0400893F RID: 35135
		[SerializeField]
		private Transform leftPropeller;

		// Token: 0x04008940 RID: 35136
		[SerializeField]
		private Transform rightPropeller;

		// Token: 0x04008941 RID: 35137
		[SerializeField]
		private SkinnedMeshRenderer blimpMesh;

		// Token: 0x04008942 RID: 35138
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008943 RID: 35139
		[SerializeField]
		private AudioClip motorSound;

		// Token: 0x04008944 RID: 35140
		[SerializeField]
		private AudioClip deflateSound;

		// Token: 0x04008945 RID: 35141
		private float turnRate;

		// Token: 0x04008946 RID: 35142
		private float turnAngle;

		// Token: 0x04008947 RID: 35143
		private float tiltAngle;

		// Token: 0x04008948 RID: 35144
		private float ascendAccel;

		// Token: 0x04008949 RID: 35145
		private float turnAccel;

		// Token: 0x0400894A RID: 35146
		private float tiltAccel;

		// Token: 0x0400894B RID: 35147
		private float horizontalAccel;

		// Token: 0x0400894C RID: 35148
		private float leftPropellerAngle;

		// Token: 0x0400894D RID: 35149
		private float rightPropellerAngle;

		// Token: 0x0400894E RID: 35150
		private float leftPropellerSpinRate;

		// Token: 0x0400894F RID: 35151
		private float rightPropellerSpinRate;

		// Token: 0x04008950 RID: 35152
		private float blimpDeflateBlendWeight;

		// Token: 0x04008951 RID: 35153
		private float deflateRate = Mathf.Exp(1f);

		// Token: 0x04008952 RID: 35154
		private const float propellerIdleAcc = 1f;

		// Token: 0x04008953 RID: 35155
		private const float propellerIdleSpinRate = 0.6f;

		// Token: 0x04008954 RID: 35156
		private const float propellerMaxAcc = 6.6666665f;

		// Token: 0x04008955 RID: 35157
		private const float propellerMaxSpinRate = 5f;

		// Token: 0x04008956 RID: 35158
		private float motorVolumeRampTime = 1f;

		// Token: 0x04008957 RID: 35159
		private float motorLevel;
	}
}
