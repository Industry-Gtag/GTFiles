using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000F0A RID: 3850
	[AttributeUsage(AttributeTargets.All)]
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanRangeAttribute : Attribute
	{
		// Token: 0x06005E38 RID: 24120 RVA: 0x001E16AF File Offset: 0x001DF8AF
		public TimeSpanRangeAttribute(string maxGetter, bool inline = false, TimeUnit snappingUnit = TimeUnit.Seconds)
		{
			this.MaxGetter = maxGetter;
			this.SnappingUnit = snappingUnit;
			this.Inline = inline;
		}

		// Token: 0x06005E39 RID: 24121 RVA: 0x001E16CC File Offset: 0x001DF8CC
		public TimeSpanRangeAttribute(string minGetter, string maxGetter, bool inline = false, TimeUnit snappingUnit = TimeUnit.Seconds)
		{
			this.MinGetter = minGetter;
			this.MaxGetter = maxGetter;
			this.SnappingUnit = snappingUnit;
			this.Inline = inline;
		}

		// Token: 0x04006D72 RID: 28018
		public string MinGetter;

		// Token: 0x04006D73 RID: 28019
		public string MaxGetter;

		// Token: 0x04006D74 RID: 28020
		public TimeUnit SnappingUnit;

		// Token: 0x04006D75 RID: 28021
		public bool Inline;

		// Token: 0x04006D76 RID: 28022
		public string DisableMinMaxIf;
	}
}
