using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200140E RID: 5134
	public class MathUtil
	{
		// Token: 0x06008177 RID: 33143 RVA: 0x002A5240 File Offset: 0x002A3440
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06008178 RID: 33144 RVA: 0x002A5257 File Offset: 0x002A3457
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06008179 RID: 33145 RVA: 0x002A5270 File Offset: 0x002A3470
		public static float CatmullRom(float p0, float p1, float p2, float p3, float t)
		{
			float num = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
		}

		// Token: 0x04009255 RID: 37461
		public static readonly float Pi = 3.1415927f;

		// Token: 0x04009256 RID: 37462
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x04009257 RID: 37463
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x04009258 RID: 37464
		public static readonly float ThirdPi = 1.0471976f;

		// Token: 0x04009259 RID: 37465
		public static readonly float QuarterPi = 0.7853982f;

		// Token: 0x0400925A RID: 37466
		public static readonly float FifthPi = 0.62831855f;

		// Token: 0x0400925B RID: 37467
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x0400925C RID: 37468
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x0400925D RID: 37469
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x0400925E RID: 37470
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x0400925F RID: 37471
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x04009260 RID: 37472
		public static readonly float Epsilon = 1E-09f;

		// Token: 0x04009261 RID: 37473
		public static readonly float EpsilonComp = 1f - MathUtil.Epsilon;

		// Token: 0x04009262 RID: 37474
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x04009263 RID: 37475
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
