using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000F09 RID: 3849
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanDrawerSettingsAttribute : Attribute
	{
		// Token: 0x06005E34 RID: 24116 RVA: 0x001E1621 File Offset: 0x001DF821
		public TimeSpanDrawerSettingsAttribute()
		{
		}

		// Token: 0x06005E35 RID: 24117 RVA: 0x001E1637 File Offset: 0x001DF837
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, TimeUnit lowestUnit)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = lowestUnit;
		}

		// Token: 0x06005E36 RID: 24118 RVA: 0x001E165B File Offset: 0x001DF85B
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, bool drawMilliseconds = false)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x06005E37 RID: 24119 RVA: 0x001E1685 File Offset: 0x001DF885
		public TimeSpanDrawerSettingsAttribute(bool drawMilliseconds)
		{
			this.HighestUnit = TimeUnit.Days;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x04006D70 RID: 28016
		public TimeUnit HighestUnit = TimeUnit.Days;

		// Token: 0x04006D71 RID: 28017
		public TimeUnit LowestUnit = TimeUnit.Seconds;
	}
}
