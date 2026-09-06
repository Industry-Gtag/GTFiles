using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001416 RID: 5142
	public class VectorUtil
	{
		// Token: 0x060081B7 RID: 33207 RVA: 0x002A619C File Offset: 0x002A439C
		public static Vector3 Rotate2D(Vector3 v, float deg)
		{
			Vector3 vector = v;
			float num = Mathf.Cos(MathUtil.Deg2Rad * deg);
			float num2 = Mathf.Sin(MathUtil.Deg2Rad * deg);
			vector.x = num * v.x - num2 * v.y;
			vector.y = num2 * v.x + num * v.y;
			return vector;
		}

		// Token: 0x060081B8 RID: 33208 RVA: 0x002A61F6 File Offset: 0x002A43F6
		public static Vector3 NormalizeSafe(Vector3 v, Vector3 fallback)
		{
			if (v.sqrMagnitude <= MathUtil.Epsilon)
			{
				return fallback;
			}
			return v.normalized;
		}

		// Token: 0x060081B9 RID: 33209 RVA: 0x002A6210 File Offset: 0x002A4410
		public static Vector3 FindOrthogonal(Vector3 v)
		{
			if (Mathf.Abs(v.x) >= MathUtil.Sqrt3Inv)
			{
				return Vector3.Normalize(new Vector3(v.y, -v.x, 0f));
			}
			return Vector3.Normalize(new Vector3(0f, v.z, -v.y));
		}

		// Token: 0x060081BA RID: 33210 RVA: 0x002A6268 File Offset: 0x002A4468
		public static void FormOrthogonalBasis(Vector3 v, out Vector3 a, out Vector3 b)
		{
			a = VectorUtil.FindOrthogonal(v);
			b = Vector3.Cross(a, v);
		}

		// Token: 0x060081BB RID: 33211 RVA: 0x002A6288 File Offset: 0x002A4488
		public static Vector3 Integrate(Vector3 x, Vector3 v, float dt)
		{
			return x + v * dt;
		}

		// Token: 0x060081BC RID: 33212 RVA: 0x002A6298 File Offset: 0x002A4498
		public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
		{
			float num = Vector3.Dot(a, b);
			if (num > 0.99999f)
			{
				return Vector3.Lerp(a, b, t);
			}
			if (num < -0.99999f)
			{
				Vector3 vector = VectorUtil.FindOrthogonal(a);
				return Quaternion.AngleAxis(180f * t, vector) * a;
			}
			float num2 = MathUtil.AcosSafe(num);
			return (Mathf.Sin((1f - t) * num2) * a + Mathf.Sin(t * num2) * b) / Mathf.Sin(num2);
		}

		// Token: 0x060081BD RID: 33213 RVA: 0x002A631C File Offset: 0x002A451C
		public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			float num = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
		}
	}
}
