using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012D6 RID: 4822
	public class RCDragon : RCVehicle
	{
		// Token: 0x060078C3 RID: 30915 RVA: 0x00274594 File Offset: 0x00272794
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

		// Token: 0x060078C4 RID: 30916 RVA: 0x00274604 File Offset: 0x00272804
		protected override void Awake()
		{
			base.Awake();
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
			this.tiltAccel = this.maxHorizontalTiltAngle / this.horizontalTiltTime;
			this.shouldFlap = false;
			this.isFlapping = false;
			this.StopBreathFire();
			if (this.animation != null)
			{
				this.animation[this.wingFlapAnimName].speed = this.wingFlapAnimSpeed;
				this.animation[this.crashAnimName].speed = this.crashAnimSpeed;
				this.animation[this.mouthClosedAnimName].layer = 1;
				this.animation[this.mouthBreathFireAnimName].layer = 1;
			}
			this.nextFlapEventAnimTime = this.flapAnimEventTime;
		}

		// Token: 0x060078C5 RID: 30917 RVA: 0x002746F7 File Offset: 0x002728F7
		protected override void OnDisable()
		{
			base.OnDisable();
			this.audioSource.GTStop();
		}

		// Token: 0x060078C6 RID: 30918 RVA: 0x0027470C File Offset: 0x0027290C
		public void StartBreathFire()
		{
			if (!string.IsNullOrEmpty(this.mouthBreathFireAnimName))
			{
				this.animation.CrossFade(this.mouthBreathFireAnimName, 0.1f);
			}
			if (this.fireBreath != null)
			{
				this.fireBreath.SetActive(true);
			}
			this.PlayRandomSound(this.breathFireSound, this.breathFireVolume);
			this.fireBreathTimeRemaining = this.fireBreathDuration;
		}

		// Token: 0x060078C7 RID: 30919 RVA: 0x00274774 File Offset: 0x00272974
		public void StopBreathFire()
		{
			if (!string.IsNullOrEmpty(this.mouthClosedAnimName))
			{
				this.animation.CrossFade(this.mouthClosedAnimName, 0.1f);
			}
			if (this.fireBreath != null)
			{
				this.fireBreath.SetActive(false);
			}
			this.fireBreathTimeRemaining = -1f;
		}

		// Token: 0x060078C8 RID: 30920 RVA: 0x002747C9 File Offset: 0x002729C9
		public bool IsBreathingFire()
		{
			return this.fireBreathTimeRemaining >= 0f;
		}

		// Token: 0x060078C9 RID: 30921 RVA: 0x002747DB File Offset: 0x002729DB
		private void PlayRandomSound(List<AudioClip> clips, float volume)
		{
			if (clips == null || clips.Count == 0)
			{
				return;
			}
			this.PlaySound(clips[Random.Range(0, clips.Count)], volume);
		}

		// Token: 0x060078CA RID: 30922 RVA: 0x00274804 File Offset: 0x00272A04
		private void PlaySound(AudioClip clip, float volume)
		{
			if (this.audioSource == null || clip == null)
			{
				return;
			}
			this.audioSource.GTStop();
			this.audioSource.clip = null;
			this.audioSource.loop = false;
			this.audioSource.volume = volume;
			this.audioSource.GTPlayOneShot(clip, 1f);
		}

		// Token: 0x060078CB RID: 30923 RVA: 0x0027486C File Offset: 0x00272A6C
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
				if (!this.IsBreathingFire() && this.activeInput.buttons > 0)
				{
					this.StartBreathFire();
				}
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
				this.networkSync.syncedState.dataB = this.activeInput.buttons;
				this.networkSync.syncedState.dataC = (this.shouldFlap ? 1 : 0);
			}
		}

		// Token: 0x060078CC RID: 30924 RVA: 0x00274968 File Offset: 0x00272B68
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.localState == RCVehicle.State.Mobilized && this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
				if (!this.IsBreathingFire() && this.networkSync.syncedState.dataB > 0)
				{
					this.StartBreathFire();
				}
				this.shouldFlap = this.networkSync.syncedState.dataC > 0;
			}
		}

		// Token: 0x060078CD RID: 30925 RVA: 0x002749F0 File Offset: 0x00272BF0
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
					if (this.crashCollider != null)
					{
						this.crashCollider.enabled = false;
					}
					if (this.animation != null)
					{
						this.animation.Play(this.dockedAnimName);
					}
					if (this.IsBreathingFire())
					{
						this.StopBreathFire();
						return;
					}
				}
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized && this.crashCollider != null)
				{
					this.crashCollider.enabled = false;
				}
				if (this.animation != null)
				{
					if (!this.isFlapping && this.shouldFlap)
					{
						this.animation.CrossFade(this.wingFlapAnimName, 0.1f);
						this.nextFlapEventAnimTime = this.flapAnimEventTime;
					}
					else if (this.isFlapping && !this.shouldFlap)
					{
						this.animation.CrossFade(this.idleAnimName, 0.15f);
					}
					this.isFlapping = this.shouldFlap;
					if (this.isFlapping && !this.IsBreathingFire())
					{
						AnimationState animationState = this.animation[this.wingFlapAnimName];
						if (animationState.normalizedTime * animationState.length > this.nextFlapEventAnimTime)
						{
							this.PlayRandomSound(this.wingFlapSound, this.wingFlapVolume);
							this.nextFlapEventAnimTime = (Mathf.Floor(animationState.normalizedTime) + 1f) * animationState.length + this.flapAnimEventTime;
						}
					}
				}
				GTTime.TimeAsDouble();
				if (this.IsBreathingFire())
				{
					this.fireBreathTimeRemaining -= dt;
					if (this.fireBreathTimeRemaining <= 0f)
					{
						this.StopBreathFire();
					}
				}
				float num = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, num, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.PlaySound(this.crashSound, this.crashSoundVolume);
					if (this.crashCollider != null)
					{
						this.crashCollider.enabled = true;
					}
					if (this.animation != null)
					{
						this.animation.CrossFade(this.crashAnimName, 0.05f);
					}
					if (this.IsBreathingFire())
					{
						this.StopBreathFire();
						return;
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060078CE RID: 30926 RVA: 0x00274C8C File Offset: 0x00272E8C
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.shouldFlap = false;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = this.maxAscendSpeed * x;
				float num2 = this.maxHorizontalSpeed * x;
				float num3 = this.ascendAccel * x;
				float num4 = this.ascendWhileFlyingAccelBoost * x;
				float num5 = 0.5f * x;
				float num6 = 45f;
				Vector3 linearVelocity = this.rb.linearVelocity;
				Vector3 normalized = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z).normalized;
				this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
				this.tiltAngle = Vector3.SignedAngle(normalized, base.transform.forward, base.transform.right);
				float num7 = this.activeInput.joystick.x * this.maxTurnRate;
				this.turnRate = Mathf.MoveTowards(this.turnRate, num7, this.turnAccel * fixedDeltaTime);
				this.turnAngle += this.turnRate * fixedDeltaTime;
				float num8 = Vector3.Dot(normalized, linearVelocity);
				float num9 = Mathf.InverseLerp(-num2, num2, num8);
				float num10 = Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, num9);
				this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, num10, this.tiltAccel * fixedDeltaTime);
				base.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0f));
				Vector3 vector = new Vector3(linearVelocity.x, 0f, linearVelocity.z);
				Vector3 vector2 = Vector3.Lerp(normalized * this.activeInput.joystick.y * num2, vector, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime));
				this.rb.AddForce((vector2 - vector) * this.rb.mass, ForceMode.Impulse);
				float num11 = this.activeInput.trigger * num;
				if (num11 > 0.01f && linearVelocity.y < num11)
				{
					this.rb.AddForce(Vector3.up * num3 * this.rb.mass, ForceMode.Force);
				}
				bool flag = Mathf.Abs(num8) > num5;
				bool flag2 = Mathf.Abs(this.turnRate) > num6;
				if (flag || flag2)
				{
					this.rb.AddForce(Vector3.up * num4 * this.rb.mass, ForceMode.Force);
				}
				this.shouldFlap = num11 > 0.01f || flag || flag2;
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

		// Token: 0x060078CF RID: 30927 RVA: 0x00274FA0 File Offset: 0x002731A0
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

		// Token: 0x04008960 RID: 35168
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x04008961 RID: 35169
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x04008962 RID: 35170
		[SerializeField]
		private float ascendWhileFlyingAccelBoost;

		// Token: 0x04008963 RID: 35171
		[SerializeField]
		private float gravityCompensation = 0.9f;

		// Token: 0x04008964 RID: 35172
		[SerializeField]
		private float crashedGravityCompensation = 0.5f;

		// Token: 0x04008965 RID: 35173
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04008966 RID: 35174
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x04008967 RID: 35175
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x04008968 RID: 35176
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x04008969 RID: 35177
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x0400896A RID: 35178
		[SerializeField]
		private float horizontalTiltTime = 2f;

		// Token: 0x0400896B RID: 35179
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.1f, 0.8f);

		// Token: 0x0400896C RID: 35180
		[SerializeField]
		private float crashSoundVolume = 0.1f;

		// Token: 0x0400896D RID: 35181
		[SerializeField]
		private float breathFireVolume = 0.5f;

		// Token: 0x0400896E RID: 35182
		[SerializeField]
		private float wingFlapVolume = 0.1f;

		// Token: 0x0400896F RID: 35183
		[SerializeField]
		private Animation animation;

		// Token: 0x04008970 RID: 35184
		[SerializeField]
		private string wingFlapAnimName;

		// Token: 0x04008971 RID: 35185
		[SerializeField]
		private float wingFlapAnimSpeed = 1f;

		// Token: 0x04008972 RID: 35186
		[SerializeField]
		private string dockedAnimName;

		// Token: 0x04008973 RID: 35187
		[SerializeField]
		private string idleAnimName;

		// Token: 0x04008974 RID: 35188
		[SerializeField]
		private string crashAnimName;

		// Token: 0x04008975 RID: 35189
		[SerializeField]
		private float crashAnimSpeed = 1f;

		// Token: 0x04008976 RID: 35190
		[SerializeField]
		private string mouthClosedAnimName;

		// Token: 0x04008977 RID: 35191
		[SerializeField]
		private string mouthBreathFireAnimName;

		// Token: 0x04008978 RID: 35192
		private bool shouldFlap;

		// Token: 0x04008979 RID: 35193
		private bool isFlapping;

		// Token: 0x0400897A RID: 35194
		private float nextFlapEventAnimTime;

		// Token: 0x0400897B RID: 35195
		[SerializeField]
		private float flapAnimEventTime = 0.25f;

		// Token: 0x0400897C RID: 35196
		[SerializeField]
		private GameObject fireBreath;

		// Token: 0x0400897D RID: 35197
		[SerializeField]
		private float fireBreathDuration;

		// Token: 0x0400897E RID: 35198
		private float fireBreathTimeRemaining;

		// Token: 0x0400897F RID: 35199
		[SerializeField]
		private Collider crashCollider;

		// Token: 0x04008980 RID: 35200
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008981 RID: 35201
		[SerializeField]
		private List<AudioClip> breathFireSound;

		// Token: 0x04008982 RID: 35202
		[SerializeField]
		private List<AudioClip> wingFlapSound;

		// Token: 0x04008983 RID: 35203
		[SerializeField]
		private AudioClip crashSound;

		// Token: 0x04008984 RID: 35204
		private float turnRate;

		// Token: 0x04008985 RID: 35205
		private float turnAngle;

		// Token: 0x04008986 RID: 35206
		private float tiltAngle;

		// Token: 0x04008987 RID: 35207
		private float ascendAccel;

		// Token: 0x04008988 RID: 35208
		private float turnAccel;

		// Token: 0x04008989 RID: 35209
		private float tiltAccel;

		// Token: 0x0400898A RID: 35210
		private float horizontalAccel;

		// Token: 0x0400898B RID: 35211
		private float motorVolumeRampTime = 1f;

		// Token: 0x0400898C RID: 35212
		private float motorLevel;
	}
}
