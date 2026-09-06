using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000DFE RID: 3582
public static class VectorMath
{
	// Token: 0x060057BD RID: 22461 RVA: 0x001C98B0 File Offset: 0x001C7AB0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Int Clamped(this Vector3Int v, int min, int max)
	{
		v.x = Math.Clamp(v.x, min, max);
		v.y = Math.Clamp(v.y, min, max);
		v.z = Math.Clamp(v.z, min, max);
		return v;
	}

	// Token: 0x060057BE RID: 22462 RVA: 0x001C98FD File Offset: 0x001C7AFD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetXYZ(this Vector3 v, float f)
	{
		v.x = f;
		v.y = f;
		v.z = f;
	}

	// Token: 0x060057BF RID: 22463 RVA: 0x001C9914 File Offset: 0x001C7B14
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Int Abs(this Vector3Int v)
	{
		v.x = Math.Abs(v.x);
		v.y = Math.Abs(v.y);
		v.z = Math.Abs(v.z);
		return v;
	}

	// Token: 0x060057C0 RID: 22464 RVA: 0x001C9950 File Offset: 0x001C7B50
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Abs(this Vector3 v)
	{
		v.x = Math.Abs(v.x);
		v.y = Math.Abs(v.y);
		v.z = Math.Abs(v.z);
		return v;
	}

	// Token: 0x060057C1 RID: 22465 RVA: 0x001C9989 File Offset: 0x001C7B89
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Min(this Vector3 v, Vector3 other)
	{
		return new Vector3(Math.Min(v.x, other.x), Math.Min(v.y, other.y), Math.Min(v.z, other.z));
	}

	// Token: 0x060057C2 RID: 22466 RVA: 0x001C99C3 File Offset: 0x001C7BC3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Max(this Vector3 v, Vector3 other)
	{
		return new Vector3(Math.Max(v.x, other.x), Math.Max(v.y, other.y), Math.Max(v.z, other.z));
	}

	// Token: 0x060057C3 RID: 22467 RVA: 0x001C99FD File Offset: 0x001C7BFD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Add(this Vector3 v, float amount)
	{
		return new Vector3(v.x + amount, v.y + amount, v.z + amount);
	}

	// Token: 0x060057C4 RID: 22468 RVA: 0x001C9A1C File Offset: 0x001C7C1C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Sub(this Vector3 v, float amount)
	{
		v.x -= amount;
		v.y -= amount;
		v.z -= amount;
		return v;
	}

	// Token: 0x060057C5 RID: 22469 RVA: 0x001C9A43 File Offset: 0x001C7C43
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Mul(this Vector3 v, float amount)
	{
		v.x *= amount;
		v.y *= amount;
		v.z *= amount;
		return v;
	}

	// Token: 0x060057C6 RID: 22470 RVA: 0x001C9A6C File Offset: 0x001C7C6C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Div(this Vector3 v, float amount)
	{
		float num = 1f / amount;
		v.x *= num;
		v.y *= num;
		v.z *= num;
		return v;
	}

	// Token: 0x060057C7 RID: 22471 RVA: 0x001C9AA8 File Offset: 0x001C7CA8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Max(this Vector3 v)
	{
		float num = Math.Max(Math.Max(v.x, v.y), v.z);
		v.x = num;
		v.y = num;
		v.z = num;
		return v;
	}

	// Token: 0x060057C8 RID: 22472 RVA: 0x001C9AEC File Offset: 0x001C7CEC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Max(this Vector3 v, float max)
	{
		float num = Math.Max(Math.Max(Math.Max(v.x, v.y), v.z), max);
		v.x = num;
		v.y = num;
		v.z = num;
		return v;
	}

	// Token: 0x060057C9 RID: 22473 RVA: 0x001C9B38 File Offset: 0x001C7D38
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 Max(this float3 v)
	{
		float num = Math.Max(v.x, Math.Max(v.y, v.z));
		v.x = num;
		v.y = num;
		v.z = num;
		return v;
	}

	// Token: 0x060057CA RID: 22474 RVA: 0x001C9B7B File Offset: 0x001C7D7B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsFinite(this Vector3 v)
	{
		return float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z);
	}

	// Token: 0x060057CB RID: 22475 RVA: 0x001C9BA4 File Offset: 0x001C7DA4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Clamped(this Vector3 v, Vector3 min, Vector3 max)
	{
		v.x = Math.Clamp(v.x, min.x, max.x);
		v.y = Math.Clamp(v.y, min.y, max.y);
		v.z = Math.Clamp(v.z, min.z, max.z);
		return v;
	}

	// Token: 0x060057CC RID: 22476 RVA: 0x001C9C0C File Offset: 0x001C7E0C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx0(this Vector3 v, float epsilon = 1E-05f)
	{
		float x = v.x;
		float y = v.y;
		float z = v.z;
		return x * x + y * y + z * z <= epsilon * epsilon;
	}

	// Token: 0x060057CD RID: 22477 RVA: 0x001C9C40 File Offset: 0x001C7E40
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx1(this Vector3 v, float epsilon = 1E-05f)
	{
		float num = v.x - 1f;
		float num2 = v.y - 1f;
		float num3 = v.z - 1f;
		return num * num + num2 * num2 + num3 * num3 <= epsilon * epsilon;
	}

	// Token: 0x060057CE RID: 22478 RVA: 0x001C9C88 File Offset: 0x001C7E88
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(this Vector3 a, Vector3 b, float epsilon = 1E-05f)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		return num * num + num2 * num2 + num3 * num3 <= epsilon * epsilon;
	}

	// Token: 0x060057CF RID: 22479 RVA: 0x001C9CD0 File Offset: 0x001C7ED0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(this Vector4 a, Vector4 b, float epsilon = 1E-05f)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		float num4 = a.w - b.w;
		return num * num + num2 * num2 + num3 * num3 + num4 * num4 <= epsilon * epsilon;
	}
}
