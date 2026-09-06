using System;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001239 RID: 4665
	public class ConsensusGravityZone : BasicGravityZone
	{
		// Token: 0x06007640 RID: 30272 RVA: 0x00265F55 File Offset: 0x00264155
		protected override void Awake()
		{
			base.Awake();
			this.zoneCollider = base.GetComponent<Collider>();
		}

		// Token: 0x06007641 RID: 30273 RVA: 0x00265F69 File Offset: 0x00264169
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return base.transform.TransformVector(new Vector3(Mathf.Sin(this.currentRot * 0.017453292f), Mathf.Cos(this.currentRot * 0.017453292f), 0f));
		}

		// Token: 0x06007642 RID: 30274 RVA: 0x00265FA4 File Offset: 0x002641A4
		private void FixedUpdate()
		{
			Vector3 vector = Vector3.zero;
			int num = 0;
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				Vector3 position = rigContainer.Rig.transform.position;
				if (this.zoneCollider.bounds.Contains(position))
				{
					vector += position;
					num++;
				}
			}
			if (num > 0)
			{
				Vector3 vector2 = vector / (float)num;
				Vector3 vector3 = base.transform.InverseTransformPoint(vector2);
				this.idealRot = Mathf.Atan2(vector3.x, vector3.y) * 57.29578f;
			}
			float num2 = (this.idealRot - this.currentRot) * this.weightForce - this.currentRot * this.centeringForce;
			this.rotSpeed += num2 * Time.fixedDeltaTime;
			this.rotSpeed *= this.drag;
			this.currentRot += this.rotSpeed * Time.fixedDeltaTime;
			if (this.currentRot < this.rotMin)
			{
				this.rotSpeed = 0f;
				this.currentRot = this.rotMin;
				return;
			}
			if (this.currentRot > this.rotMax)
			{
				this.rotSpeed = 0f;
				this.currentRot = this.rotMax;
			}
		}

		// Token: 0x06007643 RID: 30275 RVA: 0x00023F0C File Offset: 0x0002210C
		protected override bool GetRotationIntent(in Vector3 offsetFromGravity)
		{
			return true;
		}

		// Token: 0x06007644 RID: 30276 RVA: 0x00266110 File Offset: 0x00264310
		public void CopyProperties(ConsensusGravityZoneSettings settings)
		{
			base.CopyProperties(settings);
			this.weightForce = settings.weightForce;
			this.centeringForce = settings.centeringForce;
			this.drag = settings.drag;
			this.rotMin = settings.rotMin;
			this.rotMax = settings.rotMax;
		}

		// Token: 0x040085CD RID: 34253
		private Collider zoneCollider;

		// Token: 0x040085CE RID: 34254
		private float currentRot;

		// Token: 0x040085CF RID: 34255
		private float idealRot;

		// Token: 0x040085D0 RID: 34256
		private float rotSpeed;

		// Token: 0x040085D1 RID: 34257
		[SerializeField]
		private float weightForce;

		// Token: 0x040085D2 RID: 34258
		[SerializeField]
		private float centeringForce;

		// Token: 0x040085D3 RID: 34259
		[SerializeField]
		private float drag;

		// Token: 0x040085D4 RID: 34260
		[SerializeField]
		private float rotMin = -45f;

		// Token: 0x040085D5 RID: 34261
		[SerializeField]
		private float rotMax = 45f;
	}
}
