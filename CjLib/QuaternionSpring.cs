using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001413 RID: 5139
	public struct QuaternionSpring
	{
		// Token: 0x17000C6D RID: 3181
		// (get) Token: 0x06008198 RID: 33176 RVA: 0x002A5ABF File Offset: 0x002A3CBF
		// (set) Token: 0x06008199 RID: 33177 RVA: 0x002A5ACD File Offset: 0x002A3CCD
		public Quaternion ValueQuat
		{
			get
			{
				return QuaternionUtil.FromVector4(this.ValueVec, true);
			}
			set
			{
				this.ValueVec = QuaternionUtil.ToVector4(value);
			}
		}

		// Token: 0x17000C6E RID: 3182
		// (get) Token: 0x0600819A RID: 33178 RVA: 0x002A5ADB File Offset: 0x002A3CDB
		// (set) Token: 0x0600819B RID: 33179 RVA: 0x002A5AE9 File Offset: 0x002A3CE9
		public Quaternion VelocityQuat
		{
			get
			{
				return QuaternionUtil.FromVector4(this.VelocityVec, false);
			}
			set
			{
				this.VelocityVec = QuaternionUtil.ToVector4(value);
			}
		}

		// Token: 0x0600819C RID: 33180 RVA: 0x002A5AF7 File Offset: 0x002A3CF7
		public void Reset()
		{
			this.ValueVec = QuaternionUtil.ToVector4(Quaternion.identity);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x0600819D RID: 33181 RVA: 0x002A5B14 File Offset: 0x002A3D14
		public void Reset(Vector4 initValue)
		{
			this.ValueVec = initValue;
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x0600819E RID: 33182 RVA: 0x002A5B28 File Offset: 0x002A3D28
		public void Reset(Vector4 initValue, Vector4 initVelocity)
		{
			this.ValueVec = initValue;
			this.VelocityVec = initVelocity;
		}

		// Token: 0x0600819F RID: 33183 RVA: 0x002A5B38 File Offset: 0x002A3D38
		public void Reset(Quaternion initValue)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x060081A0 RID: 33184 RVA: 0x002A5B51 File Offset: 0x002A3D51
		public void Reset(Quaternion initValue, Quaternion initVelocity)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = QuaternionUtil.ToVector4(initVelocity);
		}

		// Token: 0x060081A1 RID: 33185 RVA: 0x002A5B6C File Offset: 0x002A3D6C
		public Quaternion TrackDampingRatio(Quaternion targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				return QuaternionUtil.FromVector4(this.ValueVec, true);
			}
			Vector4 vector = QuaternionUtil.ToVector4(targetValue);
			if (Vector4.Dot(this.ValueVec, vector) < 0f)
			{
				vector = -vector;
			}
			Vector4 vector2 = vector - this.ValueVec;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector4 vector3 = num * this.ValueVec + deltaTime * this.VelocityVec + num4 * vector;
			Vector4 vector4 = this.VelocityVec + num3 * vector2;
			this.VelocityVec = vector4 * num5;
			this.ValueVec = vector3 * num5;
			if (this.VelocityVec.magnitude < MathUtil.Epsilon && vector2.magnitude < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = vector;
			}
			return QuaternionUtil.FromVector4(this.ValueVec, true);
		}

		// Token: 0x060081A2 RID: 33186 RVA: 0x002A5CA0 File Offset: 0x002A3EA0
		public Quaternion TrackHalfLife(Quaternion targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = QuaternionUtil.ToVector4(targetValue);
				return targetValue;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float num2 = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x060081A3 RID: 33187 RVA: 0x002A5CF0 File Offset: 0x002A3EF0
		public Quaternion TrackExponential(Quaternion targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = QuaternionUtil.ToVector4(targetValue);
				return targetValue;
			}
			float num = 0.6931472f / halfLife;
			float num2 = 1f;
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x04009272 RID: 37490
		public static readonly int Stride = 32;

		// Token: 0x04009273 RID: 37491
		public Vector4 ValueVec;

		// Token: 0x04009274 RID: 37492
		public Vector4 VelocityVec;
	}
}
