using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001238 RID: 4664
	public class ChangingBasicGravityZone : BasicGravityZone
	{
		// Token: 0x06007636 RID: 30262 RVA: 0x00265D09 File Offset: 0x00263F09
		protected override void Awake()
		{
			base.Awake();
			this.m_thisCallbackUnique = this;
			this.m_strengthDirty = false;
			this.m_directionDity = false;
		}

		// Token: 0x06007637 RID: 30263 RVA: 0x00265D26 File Offset: 0x00263F26
		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.m_strengthDirty)
			{
				this.m_strengthDirty = false;
				this.gravityStrength = this.m_targetGravityStrength;
			}
			if (this.m_directionDity)
			{
				this.m_directionDity = false;
				this.m_gravityDirection = this.m_targetGravityDirection;
			}
		}

		// Token: 0x06007638 RID: 30264 RVA: 0x00265D64 File Offset: 0x00263F64
		public void Update()
		{
			if (this.lastValueWhenSet == this.ExternalTriggerSetGravityStrength)
			{
				this.lastExternalTriggerSetMatched = true;
				return;
			}
			if (!this.lastExternalTriggerSetMatched)
			{
				this.SetGravityStrength(this.ExternalSetGravityStrength);
				this.lastValueWhenSet = this.ExternalTriggerSetGravityStrength;
				this.lastExternalTriggerSetMatched = true;
				return;
			}
			this.ExternalTriggerSetGravityStrength = this.lastValueWhenSet;
			this.lastExternalTriggerSetMatched = false;
		}

		// Token: 0x06007639 RID: 30265 RVA: 0x00265DC2 File Offset: 0x00263FC2
		public void SetGravityStrength(float strength)
		{
			this.SetGravityStrength(strength, this.m_changeStrengthTime);
		}

		// Token: 0x0600763A RID: 30266 RVA: 0x00265DD1 File Offset: 0x00263FD1
		public void SetGravityDirection(Vector3 dir)
		{
			this.SetGravityDirection(dir, this.m_changeDirectionTime);
		}

		// Token: 0x0600763B RID: 30267 RVA: 0x00265DE0 File Offset: 0x00263FE0
		public void SetGravityStrength(float strength, float time)
		{
			this.m_targetGravityStrength = strength;
			if (time == 0f || !this.m_thisCallbackUnique.Registered)
			{
				this.gravityStrength = this.m_targetGravityStrength;
				this.m_strengthDirty = false;
				return;
			}
			this.m_lerpToGravitySpeed = (strength - this.gravityStrength) / time;
			this.m_strengthDirty = true;
		}

		// Token: 0x0600763C RID: 30268 RVA: 0x00265E34 File Offset: 0x00264034
		public void SetGravityDirection(Vector3 direction, float time)
		{
			this.m_targetGravityDirection = direction.normalized;
			if (time == 0f || !this.m_thisCallbackUnique.Registered)
			{
				this.m_gravityDirection = this.m_targetGravityDirection;
				this.m_directionDity = false;
				return;
			}
			float num = Vector3.Angle(this.m_gravityDirection, direction) * 0.017453292f;
			this.m_lerpToDirectionSpeed = num / time;
			this.m_directionDity = true;
		}

		// Token: 0x0600763D RID: 30269 RVA: 0x00265E9A File Offset: 0x0026409A
		public void SetRotationIntent(bool rotate)
		{
			this.rotateTarget = rotate;
		}

		// Token: 0x0600763E RID: 30270 RVA: 0x00265EA4 File Offset: 0x002640A4
		public override void CallBack()
		{
			if (this.m_strengthDirty)
			{
				this.gravityStrength = Mathf.MoveTowards(this.gravityStrength, this.m_targetGravityStrength, this.m_lerpToGravitySpeed * Time.fixedDeltaTime);
				if (Mathf.Approximately(this.gravityStrength, this.m_targetGravityStrength))
				{
					this.m_strengthDirty = false;
				}
			}
			if (this.m_directionDity)
			{
				this.m_gravityDirection = Vector3.RotateTowards(this.m_gravityDirection, this.m_targetGravityDirection, this.m_lerpToDirectionSpeed * Time.fixedDeltaTime, 0f);
				if (this.m_gravityDirection == this.m_targetGravityDirection)
				{
					this.m_directionDity = false;
				}
			}
			base.CallBack();
		}

		// Token: 0x040085C0 RID: 34240
		[Header("Change Value To Trigger Gravity Strength Change At Set Value (false to true and true to false both work, but value must change the frame you want it changed)")]
		public bool ExternalTriggerSetGravityStrength;

		// Token: 0x040085C1 RID: 34241
		public float ExternalSetGravityStrength;

		// Token: 0x040085C2 RID: 34242
		private bool lastExternalTriggerSetMatched = true;

		// Token: 0x040085C3 RID: 34243
		private bool lastValueWhenSet;

		// Token: 0x040085C4 RID: 34244
		private bool m_strengthDirty;

		// Token: 0x040085C5 RID: 34245
		private float m_targetGravityStrength;

		// Token: 0x040085C6 RID: 34246
		private float m_lerpToGravitySpeed;

		// Token: 0x040085C7 RID: 34247
		private bool m_directionDity;

		// Token: 0x040085C8 RID: 34248
		private Vector3 m_targetGravityDirection;

		// Token: 0x040085C9 RID: 34249
		private float m_lerpToDirectionSpeed;

		// Token: 0x040085CA RID: 34250
		[SerializeField]
		private float m_changeStrengthTime;

		// Token: 0x040085CB RID: 34251
		[SerializeField]
		private float m_changeDirectionTime;

		// Token: 0x040085CC RID: 34252
		private ICallbackUnique m_thisCallbackUnique;
	}
}
