using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x0200123A RID: 4666
	public class CubicPlanetZone : PlanetZone
	{
		// Token: 0x06007646 RID: 30278 RVA: 0x00266180 File Offset: 0x00264380
		private void UpdateConstraint()
		{
			float num = Mathf.Abs(this.constraints.x * 0.5f);
			float num2 = Mathf.Abs(this.constraints.y * 0.5f);
			float num3 = Mathf.Abs(this.constraints.z * 0.5f);
			this.minConstraints.x = num * -1f;
			this.minConstraints.y = num2 * -1f;
			this.minConstraints.z = num3 * -1f;
			this.maxConstraints.x = num;
			this.maxConstraints.y = num2;
			this.maxConstraints.z = num3;
		}

		// Token: 0x06007647 RID: 30279 RVA: 0x0026622C File Offset: 0x0026442C
		protected override void Awake()
		{
			base.Awake();
			this.CalculateDependentVars();
			this.UpdateConstraint();
		}

		// Token: 0x06007648 RID: 30280 RVA: 0x00266240 File Offset: 0x00264440
		private void CalculateDependentVars()
		{
			this.inverseRotation = Quaternion.Inverse(base.transform.rotation);
		}

		// Token: 0x06007649 RID: 30281 RVA: 0x00266258 File Offset: 0x00264458
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return worldPosition - this.GetPointOnBounds(in worldPosition);
		}

		// Token: 0x0600764A RID: 30282 RVA: 0x0026626C File Offset: 0x0026446C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetPointOnBounds(in Vector3 point)
		{
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Vector3 vector = this.inverseRotation * (point - position);
			float x = vector.x;
			float y = vector.y;
			float z = vector.z;
			if (x <= this.maxConstraints.x && x >= this.minConstraints.x && y <= this.maxConstraints.y && y >= this.minConstraints.y && z <= this.maxConstraints.z && z >= this.minConstraints.z)
			{
				Vector3 vector2 = new Vector3((x > 0f) ? this.maxConstraints.x : this.minConstraints.x, (y > 0f) ? this.maxConstraints.y : this.minConstraints.y, (z > 0f) ? this.maxConstraints.z : this.minConstraints.z);
				float num = Mathf.Abs(vector2.x - x);
				float num2 = Mathf.Abs(vector2.y - y);
				float num3 = Mathf.Abs(vector2.z - z);
				Vector3 vector3 = new Vector3((num <= num2 && num <= num3) ? 1f : 0f, (num2 <= num && num2 <= num3) ? 1f : 0f, (num3 <= num && num3 <= num2) ? 1f : 0f);
				Vector3 vector4 = (in vector).MultiplyBy(in vector3);
				Vector3 vector5 = (in vector2).MultiplyBy(in vector3);
				float magnitude = vector4.magnitude;
				float magnitude2 = vector5.magnitude;
				float num4 = 1f / magnitude2;
				float num5 = magnitude * num4;
				num5 *= 0.99f;
				return position + transform.rotation * vector.Clamp(this.minConstraints * num5, this.maxConstraints * num5);
			}
			return position + transform.rotation * vector.Clamp(this.minConstraints, this.maxConstraints);
		}

		// Token: 0x0600764B RID: 30283 RVA: 0x0026649E File Offset: 0x0026469E
		public void CopyProperties(CubicPlanetZoneSettings settings)
		{
			base.CopyProperties(settings);
			this.constraints = settings.constraints;
			this.CalculateDependentVars();
			this.UpdateConstraint();
		}

		// Token: 0x040085D6 RID: 34262
		[Header("box constraint for where gravity center can be")]
		[SerializeField]
		protected Vector3 constraints;

		// Token: 0x040085D7 RID: 34263
		[SerializeField]
		protected Vector3 minConstraints;

		// Token: 0x040085D8 RID: 34264
		[SerializeField]
		protected Vector3 maxConstraints;

		// Token: 0x040085D9 RID: 34265
		protected Quaternion inverseRotation;
	}
}
