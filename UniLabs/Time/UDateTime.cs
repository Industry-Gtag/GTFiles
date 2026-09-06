using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000F0F RID: 3855
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UDateTime : ISerializationCallbackReceiver, IComparable<UDateTime>, IComparable<DateTime>
	{
		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x06005E4D RID: 24141 RVA: 0x001E1E53 File Offset: 0x001E0053
		// (set) Token: 0x06005E4E RID: 24142 RVA: 0x001E1E5B File Offset: 0x001E005B
		[JsonProperty("DateTime")]
		public DateTime DateTime { get; set; }

		// Token: 0x06005E4F RID: 24143 RVA: 0x001E1E64 File Offset: 0x001E0064
		[JsonConstructor]
		public UDateTime()
		{
			this.DateTime = DateTime.UnixEpoch;
		}

		// Token: 0x06005E50 RID: 24144 RVA: 0x001E1E77 File Offset: 0x001E0077
		public UDateTime(DateTime dateTime)
		{
			this.DateTime = dateTime;
		}

		// Token: 0x06005E51 RID: 24145 RVA: 0x001E1E86 File Offset: 0x001E0086
		public static implicit operator DateTime(UDateTime udt)
		{
			return udt.DateTime;
		}

		// Token: 0x06005E52 RID: 24146 RVA: 0x001E1E8E File Offset: 0x001E008E
		public static implicit operator UDateTime(DateTime dt)
		{
			return new UDateTime
			{
				DateTime = dt
			};
		}

		// Token: 0x06005E53 RID: 24147 RVA: 0x001E1E9C File Offset: 0x001E009C
		public int CompareTo(DateTime other)
		{
			return this.DateTime.CompareTo(other);
		}

		// Token: 0x06005E54 RID: 24148 RVA: 0x001E1EB8 File Offset: 0x001E00B8
		public int CompareTo(UDateTime other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return this.DateTime.CompareTo(other.DateTime);
		}

		// Token: 0x06005E55 RID: 24149 RVA: 0x001E1EE4 File Offset: 0x001E00E4
		protected bool Equals(UDateTime other)
		{
			return this.DateTime.Equals(other.DateTime);
		}

		// Token: 0x06005E56 RID: 24150 RVA: 0x001E1F05 File Offset: 0x001E0105
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UDateTime)obj)));
		}

		// Token: 0x06005E57 RID: 24151 RVA: 0x001E1F34 File Offset: 0x001E0134
		public override int GetHashCode()
		{
			return this.DateTime.GetHashCode();
		}

		// Token: 0x06005E58 RID: 24152 RVA: 0x001E1F50 File Offset: 0x001E0150
		public override string ToString()
		{
			return this.DateTime.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x06005E59 RID: 24153 RVA: 0x001E1F70 File Offset: 0x001E0170
		public void OnAfterDeserialize()
		{
			DateTime dateTime;
			this.DateTime = (DateTime.TryParse(this._DateTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime) ? dateTime : DateTime.MinValue);
		}

		// Token: 0x06005E5A RID: 24154 RVA: 0x001E1FA4 File Offset: 0x001E01A4
		public void OnBeforeSerialize()
		{
			this._DateTime = this.DateTime.ToString("o", CultureInfo.InvariantCulture);
		}

		// Token: 0x06005E5B RID: 24155 RVA: 0x001E1FCF File Offset: 0x001E01CF
		[OnSerializing]
		internal void OnSerializing(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x06005E5C RID: 24156 RVA: 0x001E1FD7 File Offset: 0x001E01D7
		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x04006D7F RID: 28031
		[HideInInspector]
		[SerializeField]
		private string _DateTime;
	}
}
