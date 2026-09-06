using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011F1 RID: 4593
	public static class GTColor
	{
		// Token: 0x060074B0 RID: 29872 RVA: 0x0025E890 File Offset: 0x0025CA90
		public static Color RandomHSV(GTColor.HSVRanges ranges)
		{
			return Color.HSVToRGB(Random.Range(ranges.h.x, ranges.h.y), Random.Range(ranges.s.x, ranges.s.y), Random.Range(ranges.v.x, ranges.v.y));
		}

		// Token: 0x020011F2 RID: 4594
		[Serializable]
		public struct HSVRanges
		{
			// Token: 0x060074B1 RID: 29873 RVA: 0x0025E8F3 File Offset: 0x0025CAF3
			public HSVRanges(float hMin = 0f, float hMax = 1f, float sMin = 0f, float sMax = 1f, float vMin = 0f, float vMax = 1f)
			{
				this.h = new Vector2(hMin, hMax);
				this.s = new Vector2(sMin, sMax);
				this.v = new Vector2(vMin, vMax);
			}

			// Token: 0x04008466 RID: 33894
			public Vector2 h;

			// Token: 0x04008467 RID: 33895
			public Vector2 s;

			// Token: 0x04008468 RID: 33896
			public Vector2 v;
		}
	}
}
