using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001411 RID: 5137
	public struct Vector3Spring
	{
		// Token: 0x0600818A RID: 33162 RVA: 0x002A5717 File Offset: 0x002A3917
		public void Reset()
		{
			this.Value = Vector3.zero;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x0600818B RID: 33163 RVA: 0x002A572F File Offset: 0x002A392F
		public void Reset(Vector3 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x0600818C RID: 33164 RVA: 0x002A5743 File Offset: 0x002A3943
		public void Reset(Vector3 initValue, Vector3 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x0600818D RID: 33165 RVA: 0x002A5754 File Offset: 0x002A3954
		public Vector3 TrackDampingRatio(Vector3 targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				return this.Value;
			}
			Vector3 vector = targetValue - this.Value;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector3 vector2 = num * this.Value + deltaTime * this.Velocity + num4 * targetValue;
			Vector3 vector3 = this.Velocity + num3 * vector;
			this.Velocity = vector3 * num5;
			this.Value = vector2 * num5;
			if (this.Velocity.magnitude < MathUtil.Epsilon && vector.magnitude < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
			}
			return this.Value;
		}

		// Token: 0x0600818E RID: 33166 RVA: 0x002A5850 File Offset: 0x002A3A50
		public Vector3 TrackHalfLife(Vector3 targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float num2 = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x0600818F RID: 33167 RVA: 0x002A589C File Offset: 0x002A3A9C
		public Vector3 TrackExponential(Vector3 targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = 0.6931472f / halfLife;
			float num2 = 1f;
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x0400926A RID: 37482
		public static readonly int Stride = 32;

		// Token: 0x0400926B RID: 37483
		public Vector3 Value;

		// Token: 0x0400926C RID: 37484
		private float m_padding0;

		// Token: 0x0400926D RID: 37485
		public Vector3 Velocity;

		// Token: 0x0400926E RID: 37486
		private float m_padding1;
	}
}
