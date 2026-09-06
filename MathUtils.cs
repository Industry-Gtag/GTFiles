using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000DB8 RID: 3512
public static class MathUtils
{
	// Token: 0x0600562F RID: 22063 RVA: 0x001C26C7 File Offset: 0x001C08C7
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Xlerp(float a, float b, float dt, float decay = 16f)
	{
		return b + (a - b) * Mathf.Exp(-decay * dt);
	}

	// Token: 0x06005630 RID: 22064 RVA: 0x001C26D8 File Offset: 0x001C08D8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Xlerp(Vector3 a, Vector3 b, float dt, float decay = 16f)
	{
		return b + (a - b) * Mathf.Exp(-decay * dt);
	}

	// Token: 0x06005631 RID: 22065 RVA: 0x001C26F5 File Offset: 0x001C08F5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float SafeDivide(this float f, float d, float eps = 1E-06f)
	{
		if (Math.Abs(d) < eps)
		{
			return 0f;
		}
		if (float.IsNaN(f))
		{
			return 0f;
		}
		return f / d;
	}

	// Token: 0x06005632 RID: 22066 RVA: 0x001C2718 File Offset: 0x001C0918
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 SafeDivide(this Vector3 v, float d)
	{
		v.x = v.x.SafeDivide(d, 1E-05f);
		v.y = v.y.SafeDivide(d, 1E-05f);
		v.z = v.z.SafeDivide(d, 1E-05f);
		return v;
	}

	// Token: 0x06005633 RID: 22067 RVA: 0x001C2770 File Offset: 0x001C0970
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 SafeDivide(this Vector3 v, Vector3 d)
	{
		v.x = v.x.SafeDivide(d.x, 1E-05f);
		v.y = v.y.SafeDivide(d.y, 1E-05f);
		v.z = v.z.SafeDivide(d.z, 1E-05f);
		return v;
	}

	// Token: 0x06005634 RID: 22068 RVA: 0x001C27D5 File Offset: 0x001C09D5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Saturate(this float f, float eps = 1E-06f)
	{
		return Math.Min(Math.Max(f, 0f), 1f - eps);
	}

	// Token: 0x06005635 RID: 22069 RVA: 0x001C27EE File Offset: 0x001C09EE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Sin(this Vector3 v)
	{
		v.x = Mathf.Sin(v.x);
		v.y = Mathf.Sin(v.y);
		v.z = Mathf.Sin(v.z);
		return v;
	}

	// Token: 0x06005636 RID: 22070 RVA: 0x001C2827 File Offset: 0x001C0A27
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Quantize(this float f, float step)
	{
		return MathF.Round(f / step) * step;
	}

	// Token: 0x06005637 RID: 22071 RVA: 0x001C2833 File Offset: 0x001C0A33
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(this Quaternion a, Quaternion b, float epsilon = 1E-06f)
	{
		return Math.Abs(Quaternion.Dot(a, b)) > 1f - epsilon;
	}

	// Token: 0x06005638 RID: 22072 RVA: 0x001C284C File Offset: 0x001C0A4C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3[] BoxCorners(Vector3 center, Vector3 size)
	{
		Vector3 vector = new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 vector3 = new Vector3(0f, 0f, size.z * 0.5f);
		return new Vector3[]
		{
			center + vector + vector2 + vector3,
			center + vector + vector2 - vector3,
			center - vector + vector2 - vector3,
			center - vector + vector2 + vector3,
			center + vector - vector2 + vector3,
			center + vector - vector2 - vector3,
			center - vector - vector2 - vector3,
			center - vector - vector2 + vector3
		};
	}

	// Token: 0x06005639 RID: 22073 RVA: 0x001C2988 File Offset: 0x001C0B88
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void BoxCornersNonAlloc(Vector3 center, Vector3 size, Vector3[] array, int index = 0)
	{
		Vector3 vector = new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 vector3 = new Vector3(0f, 0f, size.z * 0.5f);
		array[index] = center + vector + vector2 + vector3;
		array[index + 1] = center + vector + vector2 - vector3;
		array[index + 2] = center - vector + vector2 - vector3;
		array[index + 3] = center - vector + vector2 + vector3;
		array[index + 4] = center + vector - vector2 + vector3;
		array[index + 5] = center + vector - vector2 - vector3;
		array[index + 6] = center - vector - vector2 - vector3;
		array[index + 7] = center - vector - vector2 + vector3;
	}

	// Token: 0x0600563A RID: 22074 RVA: 0x001C2ACC File Offset: 0x001C0CCC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3[] OrientedBoxCorners(Vector3 center, Vector3 size, Quaternion angles)
	{
		Vector3 vector = angles * new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 vector2 = angles * new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 vector3 = angles * new Vector3(0f, 0f, size.z * 0.5f);
		return new Vector3[]
		{
			center + vector + vector2 + vector3,
			center + vector + vector2 - vector3,
			center - vector + vector2 - vector3,
			center - vector + vector2 + vector3,
			center + vector - vector2 + vector3,
			center + vector - vector2 - vector3,
			center - vector - vector2 - vector3,
			center - vector - vector2 + vector3
		};
	}

	// Token: 0x0600563B RID: 22075 RVA: 0x001C2C18 File Offset: 0x001C0E18
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void OrientedBoxCornersNonAlloc(Vector3 center, Vector3 size, Quaternion angles, Vector3[] array, int index = 0)
	{
		Vector3 vector = angles * new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 vector2 = angles * new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 vector3 = angles * new Vector3(0f, 0f, size.z * 0.5f);
		array[index] = center + vector + vector2 + vector3;
		array[index + 1] = center + vector + vector2 - vector3;
		array[index + 2] = center - vector + vector2 - vector3;
		array[index + 3] = center - vector + vector2 + vector3;
		array[index + 4] = center + vector - vector2 + vector3;
		array[index + 5] = center + vector - vector2 - vector3;
		array[index + 6] = center - vector - vector2 - vector3;
		array[index + 7] = center - vector - vector2 + vector3;
	}

	// Token: 0x0600563C RID: 22076 RVA: 0x001C2D6C File Offset: 0x001C0F6C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool OrientedBoxContains(Vector3 point, Vector3 boxCenter, Vector3 boxSize, Quaternion boxAngles)
	{
		Vector3 vector = Matrix4x4.TRS(boxCenter, boxAngles, Vector3.one).inverse.MultiplyPoint3x4(point);
		Vector3 vector2 = boxSize * 0.5f;
		vector.x = Mathf.Abs(vector.x);
		vector.y = Mathf.Abs(vector.y);
		vector.z = Mathf.Abs(vector.z);
		return (Mathf.Approximately(vector.x, vector2.x) && Mathf.Approximately(vector.y, vector2.y) && Mathf.Approximately(vector.z, vector2.z)) || (vector.x < vector2.x && vector.y < vector2.y && vector.z < vector2.z);
	}

	// Token: 0x0600563D RID: 22077 RVA: 0x001C2E44 File Offset: 0x001C1044
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int OrientedBoxSphereOverlap(Vector3 center, float radius, Vector3 boxCenter, Vector3 boxSize, Quaternion boxAngles)
	{
		Matrix4x4 matrix4x = Matrix4x4.Inverse(Matrix4x4.TRS(boxCenter, boxAngles, Vector3.one));
		Vector3 vector = boxSize * 0.5f;
		Vector3 vector2 = matrix4x.MultiplyPoint3x4(center);
		Vector3 vector3 = Vector3.right * radius;
		float magnitude = matrix4x.MultiplyVector(vector3).magnitude;
		Vector3 vector4 = -vector;
		Vector3 vector5 = vector2.Clamped(vector4, vector);
		if ((vector2 - vector5).sqrMagnitude > magnitude * magnitude)
		{
			return -1;
		}
		if (vector4.x + magnitude <= vector2.x && vector2.x <= vector.x - magnitude && vector.x - vector4.x > magnitude && vector4.y + magnitude <= vector2.y && vector2.y <= vector.y - magnitude && vector.y - vector4.y > magnitude && vector4.z + magnitude <= vector2.z && vector2.z <= vector.z - magnitude && vector.z - vector4.z > magnitude)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x0600563E RID: 22078 RVA: 0x001C2F6C File Offset: 0x001C116C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Clamp(ref Vector3 v, ref Vector3 min, ref Vector3 max)
	{
		float num = v.x;
		num = ((num > max.x) ? max.x : num);
		num = ((num < min.x) ? min.x : num);
		float num2 = v.y;
		num2 = ((num2 > max.y) ? max.y : num2);
		num2 = ((num2 < min.y) ? min.y : num2);
		float num3 = v.z;
		num3 = ((num3 > max.z) ? max.z : num3);
		num3 = ((num3 < min.z) ? min.z : num3);
		return new Vector3(num, num2, num3);
	}

	// Token: 0x0600563F RID: 22079 RVA: 0x001C3008 File Offset: 0x001C1208
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Bounds[] Subdivide(Bounds b, int x = 1, int y = 1, int z = 1)
	{
		if (x < 1)
		{
			x = 1;
		}
		if (y < 1)
		{
			y = 1;
		}
		if (z < 1)
		{
			z = 1;
		}
		int num = x * y * z;
		if (num == 1)
		{
			return new Bounds[] { b };
		}
		Vector3 size = b.size;
		float num2 = size.x * 0.5f;
		float num3 = size.y * 0.5f;
		float num4 = size.z * 0.5f;
		float num5 = size.x / (float)x;
		float num6 = size.y / (float)y;
		float num7 = size.z / (float)z;
		Vector3 vector = new Vector3(num5, num6, num7);
		Bounds[] array = new Bounds[num];
		for (int i = 0; i < num; i++)
		{
			int num8;
			int num9;
			int num10;
			SpatialUtils.FlatIndexToXYZ(i, x, y, out num8, out num9, out num10);
			float num11 = num5 * (float)num8;
			float num12 = num5 * (float)(num8 + 1);
			float num13 = (num11 + num12) * 0.5f - num2;
			float num14 = num6 * (float)num9;
			float num15 = num6 * (float)(num9 + 1);
			float num16 = (num14 + num15) * 0.5f - num3;
			float num17 = num7 * (float)num10;
			float num18 = num7 * (float)(num10 + 1);
			float num19 = (num17 + num18) * 0.5f - num4;
			array[i].center = new Vector3(num13, num16, num19);
			array[i].size = vector;
		}
		return array;
	}

	// Token: 0x06005640 RID: 22080 RVA: 0x001C314D File Offset: 0x001C134D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ClampToReal(this float f, float min, float max, float epsilon = 1E-06f)
	{
		if (float.IsNaN(f))
		{
			f = 0f;
		}
		if (float.IsNegativeInfinity(min))
		{
			min = float.MinValue;
		}
		if (float.IsPositiveInfinity(max))
		{
			max = float.MaxValue;
		}
		return f.ClampApprox(min, max, epsilon);
	}

	// Token: 0x06005641 RID: 22081 RVA: 0x001C3185 File Offset: 0x001C1385
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ClampApprox(this float f, float min, float max, float epsilon = 1E-06f)
	{
		if (f < min || f.Approx(min, epsilon))
		{
			return min;
		}
		if (f > max || f.Approx(max, epsilon))
		{
			return max;
		}
		return f;
	}

	// Token: 0x06005642 RID: 22082 RVA: 0x001C31A8 File Offset: 0x001C13A8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(this float a, float b, float epsilon = 1E-06f)
	{
		return Math.Abs(a - b) < epsilon;
	}

	// Token: 0x06005643 RID: 22083 RVA: 0x001C31B5 File Offset: 0x001C13B5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx1(this float a, float epsilon = 1E-06f)
	{
		return Math.Abs(a - 1f) < epsilon;
	}

	// Token: 0x06005644 RID: 22084 RVA: 0x001C31C6 File Offset: 0x001C13C6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx0(this float a, float epsilon = 1E-06f)
	{
		return Math.Abs(a) < epsilon;
	}

	// Token: 0x06005645 RID: 22085 RVA: 0x001C31D4 File Offset: 0x001C13D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetScaledRadius(float radius, Vector3 scale)
	{
		float num = Math.Abs(scale.x);
		float num2 = Math.Abs(scale.y);
		float num3 = Math.Abs(scale.z);
		return Math.Max(Math.Abs(Math.Max(num, Math.Max(num2, num3)) * radius), 0f);
	}

	// Token: 0x06005646 RID: 22086 RVA: 0x001C3224 File Offset: 0x001C1424
	public static float Linear(float value, float min, float max, float newMin, float newMax)
	{
		float num = (value - min) / (max - min) * (newMax - newMin) + newMin;
		if (num < newMin)
		{
			return newMin;
		}
		if (num > newMax)
		{
			return newMax;
		}
		return num;
	}

	// Token: 0x06005647 RID: 22087 RVA: 0x001C324F File Offset: 0x001C144F
	public static float LinearUnclamped(float value, float min, float max, float newMin, float newMax)
	{
		return (value - min) / (max - min) * (newMax - newMin) + newMin;
	}

	// Token: 0x06005648 RID: 22088 RVA: 0x001C3260 File Offset: 0x001C1460
	public static float GetCircleValue(float degrees)
	{
		if (degrees > 90f)
		{
			degrees -= 180f;
		}
		else if (degrees < -90f)
		{
			degrees += 180f;
		}
		if (degrees > 180f)
		{
			degrees -= 270f;
		}
		else if (degrees < -180f)
		{
			degrees += 270f;
		}
		return degrees / 90f;
	}

	// Token: 0x06005649 RID: 22089 RVA: 0x001C32BC File Offset: 0x001C14BC
	public static Vector3 WeightedMaxVector(Vector3 a, Vector3 b, float eps = 0.0001f)
	{
		float magnitude = a.magnitude;
		float magnitude2 = b.magnitude;
		if (magnitude < eps || magnitude2 < eps)
		{
			return Vector3.zero;
		}
		a / magnitude;
		b / magnitude2;
		Vector3 vector = a * (magnitude / (magnitude + magnitude2)) + b * (magnitude2 / (magnitude + magnitude2));
		float num = Mathf.Max(magnitude, magnitude2);
		return vector * num;
	}

	// Token: 0x0600564A RID: 22090 RVA: 0x001C3320 File Offset: 0x001C1520
	public static Vector3 MatchMagnitudeInDirection(Vector3 input, Vector3 target, float eps = 0.0001f)
	{
		Vector3 vector = input;
		float magnitude = target.magnitude;
		if (magnitude > eps)
		{
			Vector3 vector2 = target / magnitude;
			float num = Vector3.Dot(input, vector2);
			float num2 = magnitude - num;
			if (num2 > 0f)
			{
				vector = input + num2 * vector2;
			}
		}
		return vector;
	}

	// Token: 0x0600564B RID: 22091 RVA: 0x001C336C File Offset: 0x001C156C
	public static int CalculateAgeFromDateTime(DateTime Dob)
	{
		return new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
	}

	// Token: 0x0600564C RID: 22092 RVA: 0x001C33A0 File Offset: 0x001C15A0
	public static int PositiveModulo(this int x, int m)
	{
		int num = x % m;
		if (num >= 0)
		{
			return num;
		}
		return num + m;
	}

	// Token: 0x0600564D RID: 22093 RVA: 0x001C33BC File Offset: 0x001C15BC
	public static float PositiveModulo(this float x, float m)
	{
		float num = x % m;
		if ((num < 0f && m > 0f) || (num > 0f && m < 0f))
		{
			num += m;
		}
		return num;
	}

	// Token: 0x04006772 RID: 26482
	private const float kDecay = 16f;

	// Token: 0x04006773 RID: 26483
	public const float kFloatEpsilon = 1E-06f;
}
