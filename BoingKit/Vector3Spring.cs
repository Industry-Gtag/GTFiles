using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001461 RID: 5217
	public struct Vector3Spring
	{
		// Token: 0x0600838C RID: 33676 RVA: 0x002B0CBB File Offset: 0x002AEEBB
		public void Reset()
		{
			this.Value = Vector3.zero;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x0600838D RID: 33677 RVA: 0x002B0CD3 File Offset: 0x002AEED3
		public void Reset(Vector3 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x0600838E RID: 33678 RVA: 0x002B0CE7 File Offset: 0x002AEEE7
		public void Reset(Vector3 initValue, Vector3 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x0600838F RID: 33679 RVA: 0x002B0CF8 File Offset: 0x002AEEF8
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

		// Token: 0x06008390 RID: 33680 RVA: 0x002B0DF4 File Offset: 0x002AEFF4
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

		// Token: 0x06008391 RID: 33681 RVA: 0x002B0E40 File Offset: 0x002AF040
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

		// Token: 0x0400947C RID: 38012
		public static readonly int Stride = 32;

		// Token: 0x0400947D RID: 38013
		public Vector3 Value;

		// Token: 0x0400947E RID: 38014
		private float m_padding0;

		// Token: 0x0400947F RID: 38015
		public Vector3 Velocity;

		// Token: 0x04009480 RID: 38016
		private float m_padding1;
	}
}
