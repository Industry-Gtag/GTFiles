using System;

namespace UniLabs.Time
{
	// Token: 0x02000F0C RID: 3852
	public static class TimeUnitExtensions
	{
		// Token: 0x06005E3A RID: 24122 RVA: 0x001E16F4 File Offset: 0x001DF8F4
		public static string ToShortString(this TimeUnit timeUnit)
		{
			string text;
			switch (timeUnit)
			{
			case TimeUnit.None:
				text = "";
				break;
			case TimeUnit.Milliseconds:
				text = "ms";
				break;
			case TimeUnit.Seconds:
				text = "s";
				break;
			case TimeUnit.Minutes:
				text = "m";
				break;
			case TimeUnit.Hours:
				text = "h";
				break;
			case TimeUnit.Days:
				text = "D";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return text;
		}

		// Token: 0x06005E3B RID: 24123 RVA: 0x001E1764 File Offset: 0x001DF964
		public static string ToSeparatorString(this TimeUnit timeUnit)
		{
			string text;
			switch (timeUnit)
			{
			case TimeUnit.None:
				text = "";
				break;
			case TimeUnit.Milliseconds:
				text = "";
				break;
			case TimeUnit.Seconds:
				text = ".";
				break;
			case TimeUnit.Minutes:
				text = ":";
				break;
			case TimeUnit.Hours:
				text = ":";
				break;
			case TimeUnit.Days:
				text = ".";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return text;
		}

		// Token: 0x06005E3C RID: 24124 RVA: 0x001E17D4 File Offset: 0x001DF9D4
		public static double GetUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			int num;
			switch (timeUnit)
			{
			case TimeUnit.None:
				num = 0;
				break;
			case TimeUnit.Milliseconds:
				num = timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				num = timeSpan.Seconds;
				break;
			case TimeUnit.Minutes:
				num = timeSpan.Minutes;
				break;
			case TimeUnit.Hours:
				num = timeSpan.Hours;
				break;
			case TimeUnit.Days:
				num = timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return (double)num;
		}

		// Token: 0x06005E3D RID: 24125 RVA: 0x001E184C File Offset: 0x001DFA4C
		public static TimeSpan WithUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan timeSpan2;
			switch (timeUnit)
			{
			case TimeUnit.None:
				timeSpan2 = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				timeSpan2 = timeSpan.Add(TimeSpan.FromMilliseconds(value - (double)timeSpan.Milliseconds));
				break;
			case TimeUnit.Seconds:
				timeSpan2 = timeSpan.Add(TimeSpan.FromSeconds(value - (double)timeSpan.Seconds));
				break;
			case TimeUnit.Minutes:
				timeSpan2 = timeSpan.Add(TimeSpan.FromMinutes(value - (double)timeSpan.Minutes));
				break;
			case TimeUnit.Hours:
				timeSpan2 = timeSpan.Add(TimeSpan.FromHours(value - (double)timeSpan.Hours));
				break;
			case TimeUnit.Days:
				timeSpan2 = timeSpan.Add(TimeSpan.FromDays(value - (double)timeSpan.Days));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return timeSpan2;
		}

		// Token: 0x06005E3E RID: 24126 RVA: 0x001E1910 File Offset: 0x001DFB10
		public static double GetLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double num;
			switch (timeUnit)
			{
			case TimeUnit.None:
				num = 0.0;
				break;
			case TimeUnit.Milliseconds:
				num = (double)timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				num = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				num = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalMinutes;
				break;
			case TimeUnit.Hours:
				num = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalHours;
				break;
			case TimeUnit.Days:
				num = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return num;
		}

		// Token: 0x06005E3F RID: 24127 RVA: 0x001E19EC File Offset: 0x001DFBEC
		public static TimeSpan WithLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan timeSpan2;
			switch (timeUnit)
			{
			case TimeUnit.None:
				timeSpan2 = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, (int)value);
				break;
			case TimeUnit.Seconds:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				timeSpan2 = new TimeSpan(timeSpan.Days, 0, 0, 0).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				timeSpan2 = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return timeSpan2;
		}

		// Token: 0x06005E40 RID: 24128 RVA: 0x001E1AD8 File Offset: 0x001DFCD8
		public static double GetHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double num;
			switch (timeUnit)
			{
			case TimeUnit.None:
				num = 0.0;
				break;
			case TimeUnit.Milliseconds:
				num = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				num = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				num = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).TotalMinutes;
				break;
			case TimeUnit.Hours:
				num = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).TotalHours;
				break;
			case TimeUnit.Days:
				num = (double)timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return num;
		}

		// Token: 0x06005E41 RID: 24129 RVA: 0x001E1BB4 File Offset: 0x001DFDB4
		public static TimeSpan WithHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan timeSpan2;
			switch (timeUnit)
			{
			case TimeUnit.None:
				timeSpan2 = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				timeSpan2 = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				timeSpan2 = new TimeSpan(0, 0, 0, 0, timeSpan.Milliseconds).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				timeSpan2 = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				timeSpan2 = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				timeSpan2 = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromDays(value));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return timeSpan2;
		}

		// Token: 0x06005E42 RID: 24130 RVA: 0x001E1CB4 File Offset: 0x001DFEB4
		public static double GetSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double num;
			switch (timeUnit)
			{
			case TimeUnit.Milliseconds:
				num = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				num = timeSpan.TotalSeconds;
				break;
			case TimeUnit.Minutes:
				num = timeSpan.TotalMinutes;
				break;
			case TimeUnit.Hours:
				num = timeSpan.TotalHours;
				break;
			case TimeUnit.Days:
				num = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return num;
		}

		// Token: 0x06005E43 RID: 24131 RVA: 0x001E1D24 File Offset: 0x001DFF24
		public static TimeSpan FromSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan timeSpan2;
			switch (timeUnit)
			{
			case TimeUnit.None:
				timeSpan2 = TimeSpan.Zero;
				break;
			case TimeUnit.Milliseconds:
				timeSpan2 = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				timeSpan2 = TimeSpan.FromSeconds(value);
				break;
			case TimeUnit.Minutes:
				timeSpan2 = TimeSpan.FromMinutes(value);
				break;
			case TimeUnit.Hours:
				timeSpan2 = TimeSpan.FromHours(value);
				break;
			case TimeUnit.Days:
				timeSpan2 = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return timeSpan2;
		}

		// Token: 0x06005E44 RID: 24132 RVA: 0x001E1D9C File Offset: 0x001DFF9C
		public static TimeSpan SnapToUnit(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			TimeSpan timeSpan2;
			switch (timeUnit)
			{
			case TimeUnit.None:
				timeSpan2 = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				timeSpan2 = timeSpan;
				break;
			case TimeUnit.Seconds:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				break;
			case TimeUnit.Minutes:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);
				break;
			case TimeUnit.Hours:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0);
				break;
			case TimeUnit.Days:
				timeSpan2 = new TimeSpan(timeSpan.Days, 0, 0, 0);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return timeSpan2;
		}

		// Token: 0x02000F0D RID: 3853
		// (Invoke) Token: 0x06005E46 RID: 24134
		public delegate TimeSpan WithUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit, double value);

		// Token: 0x02000F0E RID: 3854
		// (Invoke) Token: 0x06005E4A RID: 24138
		public delegate double GetUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit);
	}
}
