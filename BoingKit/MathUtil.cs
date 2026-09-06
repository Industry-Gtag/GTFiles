using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200145C RID: 5212
	public class MathUtil
	{
		// Token: 0x0600835E RID: 33630 RVA: 0x002A5240 File Offset: 0x002A3440
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x0600835F RID: 33631 RVA: 0x002A5257 File Offset: 0x002A3457
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06008360 RID: 33632 RVA: 0x002B0348 File Offset: 0x002AE548
		public static float InvSafe(float x)
		{
			return 1f / Mathf.Max(MathUtil.Epsilon, x);
		}

		// Token: 0x06008361 RID: 33633 RVA: 0x002B035C File Offset: 0x002AE55C
		public static float PointLineDist(Vector2 point, Vector2 linePos, Vector2 lineDir)
		{
			Vector2 vector = point - linePos;
			return (vector - Vector2.Dot(vector, lineDir) * lineDir).magnitude;
		}

		// Token: 0x06008362 RID: 33634 RVA: 0x002B038C File Offset: 0x002AE58C
		public static float PointSegmentDist(Vector2 point, Vector2 segmentPosA, Vector2 segmentPosB)
		{
			Vector2 vector = segmentPosB - segmentPosA;
			float num = 1f / vector.magnitude;
			Vector2 vector2 = vector * num;
			float num2 = Vector2.Dot(point - segmentPosA, vector2) * num;
			return (segmentPosA + Mathf.Clamp(num2, 0f, 1f) * vector - point).magnitude;
		}

		// Token: 0x06008363 RID: 33635 RVA: 0x002B03F4 File Offset: 0x002AE5F4
		public static float Seek(float current, float target, float maxDelta)
		{
			float num = target - current;
			num = Mathf.Sign(num) * Mathf.Min(maxDelta, Mathf.Abs(num));
			return current + num;
		}

		// Token: 0x06008364 RID: 33636 RVA: 0x002B041C File Offset: 0x002AE61C
		public static Vector2 Seek(Vector2 current, Vector2 target, float maxDelta)
		{
			Vector2 vector = target - current;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return target;
			}
			vector = Mathf.Min(maxDelta, magnitude) * vector.normalized;
			return current + vector;
		}

		// Token: 0x06008365 RID: 33637 RVA: 0x002B045E File Offset: 0x002AE65E
		public static float Remainder(float a, float b)
		{
			return a - a / b * b;
		}

		// Token: 0x06008366 RID: 33638 RVA: 0x002B045E File Offset: 0x002AE65E
		public static int Remainder(int a, int b)
		{
			return a - a / b * b;
		}

		// Token: 0x06008367 RID: 33639 RVA: 0x002B0467 File Offset: 0x002AE667
		public static float Modulo(float a, float b)
		{
			return Mathf.Repeat(a, b);
		}

		// Token: 0x06008368 RID: 33640 RVA: 0x002B0470 File Offset: 0x002AE670
		public static int Modulo(int a, int b)
		{
			int num = a % b;
			if (num < 0)
			{
				return num + b;
			}
			return num;
		}

		// Token: 0x04009467 RID: 37991
		public static readonly float Pi = 3.1415927f;

		// Token: 0x04009468 RID: 37992
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x04009469 RID: 37993
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x0400946A RID: 37994
		public static readonly float QuaterPi = 0.7853982f;

		// Token: 0x0400946B RID: 37995
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x0400946C RID: 37996
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x0400946D RID: 37997
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x0400946E RID: 37998
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x0400946F RID: 37999
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x04009470 RID: 38000
		public static readonly float Epsilon = 1E-06f;

		// Token: 0x04009471 RID: 38001
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x04009472 RID: 38002
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
