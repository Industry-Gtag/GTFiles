using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012D7 RID: 4823
	public class RCHelicopter : RCVehicle
	{
		// Token: 0x060078D1 RID: 30929 RVA: 0x0027519C File Offset: 0x0027339C
		protected override void AuthorityBeginDocked()
		{
			base.AuthorityBeginDocked();
			this.turnRate = 0f;
			this.verticalPropeller.localRotation = this.verticalPropellerBaseRotation;
			this.turnPropeller.localRotation = this.turnPropellerBaseRotation;
			if (this.connectedRemote == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060078D2 RID: 30930 RVA: 0x002751F8 File Offset: 0x002733F8
		protected override void Awake()
		{
			base.Awake();
			this.verticalPropellerBaseRotation = this.verticalPropeller.localRotation;
			this.turnPropellerBaseRotation = this.turnPropeller.localRotation;
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
		}

		// Token: 0x060078D3 RID: 30931 RVA: 0x00275268 File Offset: 0x00273468
		protected override void SharedUpdate(float dt)
		{
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = Mathf.Lerp(this.mainPropellerSpinRateRange.x, this.mainPropellerSpinRateRange.y, this.activeInput.trigger);
				this.verticalPropeller.Rotate(new Vector3(0f, num * dt, 0f), Space.Self);
				this.turnPropeller.Rotate(new Vector3(this.activeInput.joystick.x * this.backPropellerSpinRate * dt, 0f, 0f), Space.Self);
			}
		}

		// Token: 0x060078D4 RID: 30932 RVA: 0x002752F8 File Offset: 0x002734F8
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			float num = this.activeInput.joystick.x * this.maxTurnRate;
			this.turnRate = Mathf.MoveTowards(this.turnRate, num, this.turnAccel * fixedDeltaTime);
			float num2 = this.activeInput.joystick.y * this.maxHorizontalSpeed;
			float num3 = Mathf.Sign(this.activeInput.joystick.y) * Mathf.Lerp(0f, this.maxHorizontalTiltAngle, Mathf.Abs(this.activeInput.joystick.y));
			base.transform.rotation = Quaternion.Euler(new Vector3(num3, this.turnAccel, 0f));
			float num4 = Mathf.Abs(num2);
			Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
			float num5 = Vector3.Dot(normalized, linearVelocity);
			if (num4 > 0.01f && ((num2 > 0f && num2 > num5) || (num2 < 0f && num2 < num5)))
			{
				this.rb.AddForce(normalized * Mathf.Sign(num2) * this.horizontalAccel * fixedDeltaTime * this.rb.mass, ForceMode.Force);
			}
			float num6 = this.activeInput.trigger * this.maxAscendSpeed;
			if (num6 > 0.01f && linearVelocity.y < num6)
			{
				this.rb.AddForce(Vector3.up * this.ascendAccel * this.rb.mass, ForceMode.Force);
			}
			if (this.rb.useGravity)
			{
				this.rb.AddForce(-Physics.gravity * this.gravityCompensation * this.rb.mass, ForceMode.Force);
			}
		}

		// Token: 0x060078D5 RID: 30933 RVA: 0x002754F6 File Offset: 0x002736F6
		private void OnTriggerEnter(Collider other)
		{
			if (!other.isTrigger && base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginCrash();
			}
		}

		// Token: 0x0400898D RID: 35213
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x0400898E RID: 35214
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x0400898F RID: 35215
		[SerializeField]
		private float gravityCompensation = 0.5f;

		// Token: 0x04008990 RID: 35216
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04008991 RID: 35217
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x04008992 RID: 35218
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x04008993 RID: 35219
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x04008994 RID: 35220
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x04008995 RID: 35221
		[SerializeField]
		private Vector2 mainPropellerSpinRateRange = new Vector2(3f, 15f);

		// Token: 0x04008996 RID: 35222
		[SerializeField]
		private float backPropellerSpinRate = 5f;

		// Token: 0x04008997 RID: 35223
		[SerializeField]
		private Transform verticalPropeller;

		// Token: 0x04008998 RID: 35224
		[SerializeField]
		private Transform turnPropeller;

		// Token: 0x04008999 RID: 35225
		private Quaternion verticalPropellerBaseRotation;

		// Token: 0x0400899A RID: 35226
		private Quaternion turnPropellerBaseRotation;

		// Token: 0x0400899B RID: 35227
		private float turnRate;

		// Token: 0x0400899C RID: 35228
		private float ascendAccel;

		// Token: 0x0400899D RID: 35229
		private float turnAccel;

		// Token: 0x0400899E RID: 35230
		private float horizontalAccel;
	}
}
