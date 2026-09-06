using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012D8 RID: 4824
	public class RCPlane : RCVehicle
	{
		// Token: 0x060078D7 RID: 30935 RVA: 0x002755A4 File Offset: 0x002737A4
		protected override void Awake()
		{
			base.Awake();
			this.pitchAccelMinMax.x = this.pitchVelocityTargetMinMax.x / this.pitchVelocityRampTimeMinMax.x;
			this.pitchAccelMinMax.y = this.pitchVelocityTargetMinMax.y / this.pitchVelocityRampTimeMinMax.y;
			this.rollAccel = this.rollVelocityTarget / this.rollVelocityRampTime;
			this.thrustAccel = this.thrustVelocityTarget / this.thrustAccelTime;
		}

		// Token: 0x060078D8 RID: 30936 RVA: 0x00275624 File Offset: 0x00273824
		protected override void AuthorityBeginMobilization()
		{
			base.AuthorityBeginMobilization();
			float x = base.transform.lossyScale.x;
			this.rb.linearVelocity = base.transform.forward * this.initialSpeed * x;
		}

		// Token: 0x060078D9 RID: 30937 RVA: 0x00275670 File Offset: 0x00273870
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = this.activeInput.trigger;
			}
			this.leftAileronLevel = 0f;
			this.rightAileronLevel = 0f;
			float magnitude = this.activeInput.joystick.magnitude;
			if (magnitude > 0.01f)
			{
				float num = Mathf.Abs(this.activeInput.joystick.x) / magnitude;
				float num2 = Mathf.Abs(this.activeInput.joystick.y) / magnitude;
				this.leftAileronLevel = Mathf.Clamp(num * this.activeInput.joystick.x + num2 * -this.activeInput.joystick.y, -1f, 1f);
				this.rightAileronLevel = Mathf.Clamp(num * this.activeInput.joystick.x + num2 * this.activeInput.joystick.y, -1f, 1f);
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
				this.networkSync.syncedState.dataB = (byte)Mathf.Clamp(Mathf.FloorToInt(this.leftAileronLevel * 126f), -126, 126);
				this.networkSync.syncedState.dataC = (byte)Mathf.Clamp(Mathf.FloorToInt(this.rightAileronLevel * 126f), -126, 126);
			}
		}

		// Token: 0x060078DA RID: 30938 RVA: 0x00275814 File Offset: 0x00273A14
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
				this.leftAileronLevel = Mathf.Clamp((float)this.networkSync.syncedState.dataB / 126f, -1f, 1f);
				this.rightAileronLevel = Mathf.Clamp((float)this.networkSync.syncedState.dataC / 126f, -1f, 1f);
			}
		}

		// Token: 0x060078DB RID: 30939 RVA: 0x002758B0 File Offset: 0x00273AB0
		protected override void SharedUpdate(float dt)
		{
			base.SharedUpdate(dt);
			switch (this.localState)
			{
			case RCVehicle.State.DockedLeft:
			case RCVehicle.State.DockedRight:
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0.6f, 6.6666665f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.audioSource.loop = true;
					this.audioSource.clip = this.motorSound;
					this.audioSource.volume = 0f;
					this.audioSource.GTPlay();
				}
				float num = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, num, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 5f, 6.6666665f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.audioSource.GTStop();
					this.audioSource.clip = null;
					this.audioSource.loop = false;
					this.audioSource.volume = this.crashSoundVolume;
					if (this.crashSound != null)
					{
						this.audioSource.GTPlayOneShot(this.crashSound, 1f);
					}
				}
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0f, 13.333333f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			}
			float num2 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.leftAileronLevel));
			float num3 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.rightAileronLevel));
			this.leftAileronAngle = Mathf.MoveTowards(this.leftAileronAngle, num2, this.aileronAngularAcc * Time.deltaTime);
			this.rightAileronAngle = Mathf.MoveTowards(this.rightAileronAngle, num3, this.aileronAngularAcc * Time.deltaTime);
			Quaternion quaternion = Quaternion.Euler(0f, -90f, 90f + this.leftAileronAngle);
			Quaternion quaternion2 = Quaternion.Euler(0f, 90f, -90f + this.rightAileronAngle);
			this.leftAileronLower.localRotation = quaternion;
			this.leftAileronUpper.localRotation = quaternion;
			this.rightAileronLower.localRotation = quaternion2;
			this.rightAileronUpper.localRotation = quaternion2;
		}

		// Token: 0x060078DC RID: 30940 RVA: 0x00275C18 File Offset: 0x00273E18
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			float num = this.thrustVelocityTarget * x;
			float num2 = this.thrustAccel * x;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.pitch = base.NormalizeAngle180(this.pitch);
			this.roll = base.NormalizeAngle180(this.roll);
			float num3 = this.pitch;
			float num4 = this.roll;
			if (this.activeInput.joystick.y >= 0f)
			{
				float num5 = this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.y;
				this.pitchVel = Mathf.MoveTowards(this.pitchVel, num5, this.pitchAccelMinMax.y * fixedDeltaTime);
				this.pitch += this.pitchVel * fixedDeltaTime;
			}
			else
			{
				float num6 = -this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.x;
				this.pitchVel = Mathf.MoveTowards(this.pitchVel, num6, this.pitchAccelMinMax.x * fixedDeltaTime);
				this.pitch += this.pitchVel * fixedDeltaTime;
			}
			float num7 = -this.activeInput.joystick.x * this.rollVelocityTarget;
			this.rollVel = Mathf.MoveTowards(this.rollVel, num7, this.rollAccel * fixedDeltaTime);
			this.roll += this.rollVel * fixedDeltaTime;
			Quaternion quaternion = Quaternion.Euler(new Vector3(this.pitch - num3, 0f, this.roll - num4));
			base.transform.rotation = base.transform.rotation * quaternion;
			this.rb.angularVelocity = Vector3.zero;
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			float num8 = Mathf.Max(Vector3.Dot(base.transform.forward, linearVelocity), 0f);
			float num9 = this.activeInput.trigger * num;
			float num10 = 0.1f * x;
			if (num9 > num10 && num9 > num8)
			{
				float num11 = Mathf.MoveTowards(num8, num9, num2 * fixedDeltaTime);
				this.rb.AddForce(base.transform.forward * (num11 - num8) * this.rb.mass, ForceMode.Impulse);
			}
			float num12 = 0.01f * x;
			float num13 = Vector3.Dot(linearVelocity / Mathf.Max(magnitude, num12), base.transform.forward);
			float num14 = this.liftVsAttackCurve.Evaluate(num13);
			float num15 = Mathf.Lerp(this.liftVsSpeedOutput.x, this.liftVsSpeedOutput.y, Mathf.InverseLerp(this.liftVsSpeedInput.x, this.liftVsSpeedInput.y, magnitude / x));
			float num16 = num14 * num15;
			Vector3 vector = Vector3.RotateTowards(linearVelocity, base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime) - linearVelocity;
			this.rb.AddForce(vector * num16 * this.rb.mass, ForceMode.Impulse);
			float num17 = Vector3.Dot(linearVelocity.normalized, base.transform.up);
			float num18 = this.dragVsAttackCurve.Evaluate(num17);
			this.rb.AddForce(-linearVelocity * this.maxDrag * num18 * this.rb.mass, ForceMode.Force);
			if (this.rb.useGravity)
			{
				float num19 = Mathf.Lerp(this.gravityCompensationRange.x, this.gravityCompensationRange.y, Mathf.InverseLerp(0f, num, num8 / x));
				RCVehicle.AddScaledGravityCompensationForce(this.rb, x, num19);
			}
		}

		// Token: 0x060078DD RID: 30941 RVA: 0x00276000 File Offset: 0x00274200
		private void OnCollisionEnter(Collision collision)
		{
			if (base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				for (int i = 0; i < collision.contactCount; i++)
				{
					ContactPoint contact = collision.GetContact(i);
					if (!this.nonCrashColliders.Contains(contact.thisCollider))
					{
						this.AuthorityBeginCrash();
					}
				}
				return;
			}
			bool flag = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
			bool flag2 = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaHand);
			if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
			{
				Vector3 vector = Vector3.zero;
				if (flag2)
				{
					GorillaHandClimber component = collision.collider.gameObject.GetComponent<GorillaHandClimber>();
					if (component != null)
					{
						vector = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == XRNode.LeftHand).GetAverageVelocity(true, 0.15f, false);
					}
				}
				else if (collision.rigidbody != null)
				{
					vector = collision.rigidbody.linearVelocity;
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

		// Token: 0x0400899F RID: 35231
		public Vector2 pitchVelocityTargetMinMax = new Vector2(-180f, 180f);

		// Token: 0x040089A0 RID: 35232
		public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-0.75f, 0.75f);

		// Token: 0x040089A1 RID: 35233
		public float rollVelocityTarget = 180f;

		// Token: 0x040089A2 RID: 35234
		public float rollVelocityRampTime = 0.75f;

		// Token: 0x040089A3 RID: 35235
		public float thrustVelocityTarget = 15f;

		// Token: 0x040089A4 RID: 35236
		public float thrustAccelTime = 2f;

		// Token: 0x040089A5 RID: 35237
		[SerializeField]
		private float pitchVelocityFollowRateAngle = 60f;

		// Token: 0x040089A6 RID: 35238
		[SerializeField]
		private float pitchVelocityFollowRateMagnitude = 5f;

		// Token: 0x040089A7 RID: 35239
		[SerializeField]
		private float maxDrag = 0.1f;

		// Token: 0x040089A8 RID: 35240
		[SerializeField]
		private Vector2 liftVsSpeedInput = new Vector2(0f, 4f);

		// Token: 0x040089A9 RID: 35241
		[SerializeField]
		private Vector2 liftVsSpeedOutput = new Vector2(0.5f, 1f);

		// Token: 0x040089AA RID: 35242
		[SerializeField]
		private AnimationCurve liftVsAttackCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040089AB RID: 35243
		[SerializeField]
		private AnimationCurve dragVsAttackCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040089AC RID: 35244
		[SerializeField]
		private Vector2 gravityCompensationRange = new Vector2(0.5f, 1f);

		// Token: 0x040089AD RID: 35245
		[SerializeField]
		private List<Collider> nonCrashColliders = new List<Collider>();

		// Token: 0x040089AE RID: 35246
		[SerializeField]
		private Transform propeller;

		// Token: 0x040089AF RID: 35247
		[SerializeField]
		private Transform leftAileronUpper;

		// Token: 0x040089B0 RID: 35248
		[SerializeField]
		private Transform leftAileronLower;

		// Token: 0x040089B1 RID: 35249
		[SerializeField]
		private Transform rightAileronUpper;

		// Token: 0x040089B2 RID: 35250
		[SerializeField]
		private Transform rightAileronLower;

		// Token: 0x040089B3 RID: 35251
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040089B4 RID: 35252
		[SerializeField]
		private AudioClip motorSound;

		// Token: 0x040089B5 RID: 35253
		[SerializeField]
		private AudioClip crashSound;

		// Token: 0x040089B6 RID: 35254
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.02f, 0.1f);

		// Token: 0x040089B7 RID: 35255
		[SerializeField]
		private float crashSoundVolume = 0.12f;

		// Token: 0x040089B8 RID: 35256
		private float motorVolumeRampTime = 1f;

		// Token: 0x040089B9 RID: 35257
		private float propellerAngle;

		// Token: 0x040089BA RID: 35258
		private float propellerSpinRate;

		// Token: 0x040089BB RID: 35259
		private const float propellerIdleAcc = 1f;

		// Token: 0x040089BC RID: 35260
		private const float propellerIdleSpinRate = 0.6f;

		// Token: 0x040089BD RID: 35261
		private const float propellerMaxAcc = 6.6666665f;

		// Token: 0x040089BE RID: 35262
		private const float propellerMaxSpinRate = 5f;

		// Token: 0x040089BF RID: 35263
		public float initialSpeed = 3f;

		// Token: 0x040089C0 RID: 35264
		private float pitch;

		// Token: 0x040089C1 RID: 35265
		private float pitchVel;

		// Token: 0x040089C2 RID: 35266
		private Vector2 pitchAccelMinMax;

		// Token: 0x040089C3 RID: 35267
		private float roll;

		// Token: 0x040089C4 RID: 35268
		private float rollVel;

		// Token: 0x040089C5 RID: 35269
		private float rollAccel;

		// Token: 0x040089C6 RID: 35270
		private float thrustAccel;

		// Token: 0x040089C7 RID: 35271
		private float motorLevel;

		// Token: 0x040089C8 RID: 35272
		private float leftAileronLevel;

		// Token: 0x040089C9 RID: 35273
		private float rightAileronLevel;

		// Token: 0x040089CA RID: 35274
		private Vector2 aileronAngularRange = new Vector2(-30f, 45f);

		// Token: 0x040089CB RID: 35275
		private float aileronAngularAcc = 120f;

		// Token: 0x040089CC RID: 35276
		private float leftAileronAngle;

		// Token: 0x040089CD RID: 35277
		private float rightAileronAngle;
	}
}
