using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000F11 RID: 3857
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UTimeSpanRange
	{
		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x06005E70 RID: 24176 RVA: 0x001E2193 File Offset: 0x001E0393
		// (set) Token: 0x06005E71 RID: 24177 RVA: 0x001E21A0 File Offset: 0x001E03A0
		public TimeSpan Start
		{
			get
			{
				return this._Start;
			}
			set
			{
				this._Start = value;
			}
		}

		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x06005E72 RID: 24178 RVA: 0x001E21AE File Offset: 0x001E03AE
		// (set) Token: 0x06005E73 RID: 24179 RVA: 0x001E21BB File Offset: 0x001E03BB
		public TimeSpan End
		{
			get
			{
				return this._End;
			}
			set
			{
				this._End = value;
			}
		}

		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x06005E74 RID: 24180 RVA: 0x001E21C9 File Offset: 0x001E03C9
		public TimeSpan Duration
		{
			get
			{
				return this.End - this.Start;
			}
		}

		// Token: 0x06005E75 RID: 24181 RVA: 0x001E21DC File Offset: 0x001E03DC
		public bool IsInRange(TimeSpan time)
		{
			return time >= this.Start && time <= this.End;
		}

		// Token: 0x06005E76 RID: 24182 RVA: 0x00002050 File Offset: 0x00000250
		[JsonConstructor]
		public UTimeSpanRange()
		{
		}

		// Token: 0x06005E77 RID: 24183 RVA: 0x001E21FA File Offset: 0x001E03FA
		public UTimeSpanRange(TimeSpan start)
		{
			this._Start = start;
			this._End = start;
		}

		// Token: 0x06005E78 RID: 24184 RVA: 0x001E221A File Offset: 0x001E041A
		public UTimeSpanRange(TimeSpan start, TimeSpan end)
		{
			this._Start = start;
			this._End = end;
		}

		// Token: 0x06005E79 RID: 24185 RVA: 0x001E223A File Offset: 0x001E043A
		private void OnStartChanged()
		{
			if (this._Start.CompareTo(this._End) > 0)
			{
				this._End.TimeSpan = this._Start.TimeSpan;
			}
		}

		// Token: 0x06005E7A RID: 24186 RVA: 0x001E2266 File Offset: 0x001E0466
		private void OnEndChanged()
		{
			if (this._End.CompareTo(this._Start) < 0)
			{
				this._Start.TimeSpan = this._End.TimeSpan;
			}
		}

		// Token: 0x04006D82 RID: 28034
		[JsonProperty("Start")]
		[SerializeField]
		private UTimeSpan _Start;

		// Token: 0x04006D83 RID: 28035
		[JsonProperty("End")]
		[SerializeField]
		private UTimeSpan _End;
	}
}
