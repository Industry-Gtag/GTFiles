using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000F10 RID: 3856
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UTimeSpan : ISerializationCallbackReceiver, IComparable<UTimeSpan>, IComparable<TimeSpan>
	{
		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x06005E5D RID: 24157 RVA: 0x001E1FDF File Offset: 0x001E01DF
		// (set) Token: 0x06005E5E RID: 24158 RVA: 0x001E1FE7 File Offset: 0x001E01E7
		[JsonProperty("TimeSpan")]
		public TimeSpan TimeSpan { get; set; }

		// Token: 0x06005E5F RID: 24159 RVA: 0x001E1FF0 File Offset: 0x001E01F0
		[JsonConstructor]
		public UTimeSpan()
		{
			this.TimeSpan = TimeSpan.Zero;
		}

		// Token: 0x06005E60 RID: 24160 RVA: 0x001E2003 File Offset: 0x001E0203
		public UTimeSpan(TimeSpan timeSpan)
		{
			this.TimeSpan = timeSpan;
		}

		// Token: 0x06005E61 RID: 24161 RVA: 0x001E2012 File Offset: 0x001E0212
		public UTimeSpan(long ticks)
			: this(new TimeSpan(ticks))
		{
		}

		// Token: 0x06005E62 RID: 24162 RVA: 0x001E2020 File Offset: 0x001E0220
		public UTimeSpan(int hours, int minutes, int seconds)
			: this(new TimeSpan(hours, minutes, seconds))
		{
		}

		// Token: 0x06005E63 RID: 24163 RVA: 0x001E2030 File Offset: 0x001E0230
		public UTimeSpan(int days, int hours, int minutes, int seconds)
			: this(new TimeSpan(days, hours, minutes, seconds))
		{
		}

		// Token: 0x06005E64 RID: 24164 RVA: 0x001E2042 File Offset: 0x001E0242
		public UTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds)
			: this(new TimeSpan(days, hours, minutes, seconds, milliseconds))
		{
		}

		// Token: 0x06005E65 RID: 24165 RVA: 0x001E2056 File Offset: 0x001E0256
		public static implicit operator TimeSpan(UTimeSpan uTimeSpan)
		{
			if (uTimeSpan == null)
			{
				return TimeSpan.Zero;
			}
			return uTimeSpan.TimeSpan;
		}

		// Token: 0x06005E66 RID: 24166 RVA: 0x001E2067 File Offset: 0x001E0267
		public static implicit operator UTimeSpan(TimeSpan timeSpan)
		{
			return new UTimeSpan(timeSpan);
		}

		// Token: 0x06005E67 RID: 24167 RVA: 0x001E2070 File Offset: 0x001E0270
		public int CompareTo(TimeSpan other)
		{
			return this.TimeSpan.CompareTo(other);
		}

		// Token: 0x06005E68 RID: 24168 RVA: 0x001E208C File Offset: 0x001E028C
		public int CompareTo(UTimeSpan other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return this.TimeSpan.CompareTo(other.TimeSpan);
		}

		// Token: 0x06005E69 RID: 24169 RVA: 0x001E20B8 File Offset: 0x001E02B8
		protected bool Equals(UTimeSpan other)
		{
			return this.TimeSpan.Equals(other.TimeSpan);
		}

		// Token: 0x06005E6A RID: 24170 RVA: 0x001E20D9 File Offset: 0x001E02D9
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UTimeSpan)obj)));
		}

		// Token: 0x06005E6B RID: 24171 RVA: 0x001E2108 File Offset: 0x001E0308
		public override int GetHashCode()
		{
			return this.TimeSpan.GetHashCode();
		}

		// Token: 0x06005E6C RID: 24172 RVA: 0x001E212C File Offset: 0x001E032C
		public void OnAfterDeserialize()
		{
			TimeSpan timeSpan;
			this.TimeSpan = (TimeSpan.TryParse(this._TimeSpan, CultureInfo.InvariantCulture, out timeSpan) ? timeSpan : TimeSpan.Zero);
		}

		// Token: 0x06005E6D RID: 24173 RVA: 0x001E215C File Offset: 0x001E035C
		public void OnBeforeSerialize()
		{
			this._TimeSpan = this.TimeSpan.ToString();
		}

		// Token: 0x06005E6E RID: 24174 RVA: 0x001E2183 File Offset: 0x001E0383
		[OnSerializing]
		internal void OnSerializingMethod(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x06005E6F RID: 24175 RVA: 0x001E218B File Offset: 0x001E038B
		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x04006D81 RID: 28033
		[HideInInspector]
		[SerializeField]
		private string _TimeSpan;
	}
}
