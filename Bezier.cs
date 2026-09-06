using System;
using UnityEngine;

// Token: 0x02000DDA RID: 3546
public static class Bezier
{
	// Token: 0x060056E7 RID: 22247 RVA: 0x001C5D9C File Offset: 0x001C3F9C
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return num * num * p0 + 2f * num * t * p1 + t * t * p2;
	}

	// Token: 0x060056E8 RID: 22248 RVA: 0x001C5DE4 File Offset: 0x001C3FE4
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
	}

	// Token: 0x060056E9 RID: 22249 RVA: 0x001C5E18 File Offset: 0x001C4018
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
	}

	// Token: 0x060056EA RID: 22250 RVA: 0x001C5E84 File Offset: 0x001C4084
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return 3f * num * num * (p1 - p0) + 6f * num * t * (p2 - p1) + 3f * t * t * (p3 - p2);
	}
}
