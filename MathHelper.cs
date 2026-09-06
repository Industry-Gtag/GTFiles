using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public static class MathHelper
{
	// Token: 0x06000CCA RID: 3274 RVA: 0x000469DD File Offset: 0x00044BDD
	public static float RoundTo(this float value, float increment)
	{
		return Mathf.Floor(value / increment + 0.5f) * increment;
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x000469F0 File Offset: 0x00044BF0
	public static Vector3 RoundTo(this Vector3 value, float increment)
	{
		value.x = Mathf.Floor(value.x / increment + 0.5f) * increment;
		value.y = Mathf.Floor(value.y / increment + 0.5f) * increment;
		value.z = Mathf.Floor(value.z / increment + 0.5f) * increment;
		return value;
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x00046A54 File Offset: 0x00044C54
	public static Vector3 SnapToInt(this Vector3 value)
	{
		value.x = Mathf.Floor(value.x + 0.5f);
		value.y = Mathf.Floor(value.y + 0.5f);
		value.z = Mathf.Floor(value.z + 0.5f);
		return value;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x00046AAC File Offset: 0x00044CAC
	public static Quaternion RoundTo(this Quaternion value, float increment)
	{
		Vector3 eulerAngles = value.eulerAngles;
		eulerAngles.x = Mathf.Floor(eulerAngles.x / increment + 0.5f) * increment;
		eulerAngles.y = Mathf.Floor(eulerAngles.y / increment + 0.5f) * increment;
		eulerAngles.z = Mathf.Floor(eulerAngles.z / increment + 0.5f) * increment;
		value.eulerAngles = eulerAngles;
		return value;
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x00046B20 File Offset: 0x00044D20
	public static Quaternion SnapToCardinal(this Quaternion value)
	{
		Vector3 eulerAngles = value.eulerAngles;
		eulerAngles.x = (eulerAngles.z = 0f);
		eulerAngles.y = Mathf.Floor(eulerAngles.y / 90f + 0.5f) * 90f;
		return Quaternion.Euler(eulerAngles);
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x00046B78 File Offset: 0x00044D78
	public static bool IsInBounds(this int3 a, int3 min, int3 max)
	{
		return min.x <= a.x && max.x >= a.x && min.y <= a.y && max.y >= a.y && min.z <= a.z && max.z >= a.z;
	}
}
