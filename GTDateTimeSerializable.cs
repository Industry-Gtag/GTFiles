using System;
using System.Globalization;
using UnityEngine;

// Token: 0x0200030E RID: 782
[Serializable]
public struct GTDateTimeSerializable : ISerializationCallbackReceiver
{
	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x060013DB RID: 5083 RVA: 0x0006C2A8 File Offset: 0x0006A4A8
	// (set) Token: 0x060013DC RID: 5084 RVA: 0x0006C2B0 File Offset: 0x0006A4B0
	public DateTime dateTime
	{
		get
		{
			return this._dateTime;
		}
		set
		{
			this._dateTime = value;
			this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
		}
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x0006C2CA File Offset: 0x0006A4CA
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x0006C2E0 File Offset: 0x0006A4E0
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		DateTime dateTime;
		if (GTDateTimeSerializable.TryParseDateTime(this._dateTimeString, out dateTime))
		{
			this._dateTime = dateTime;
		}
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x0006C304 File Offset: 0x0006A504
	public GTDateTimeSerializable(int dummyValue)
	{
		DateTime now = DateTime.Now;
		this._dateTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
		this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
	}

	// Token: 0x060013E0 RID: 5088 RVA: 0x0006C34C File Offset: 0x0006A54C
	private static string FormatDateTime(DateTime dateTime)
	{
		return dateTime.ToString("yyyy-MM-dd HH:mm");
	}

	// Token: 0x060013E1 RID: 5089 RVA: 0x0006C35C File Offset: 0x0006A55C
	private static bool TryParseDateTime(string value, out DateTime result)
	{
		if (DateTime.TryParseExact(value, new string[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "yyyy-MM" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
		{
			DateTime dateTime = result;
			if (dateTime.Hour == 0 && dateTime.Minute == 0)
			{
				result = result.AddHours(11.0);
			}
			return true;
		}
		return false;
	}

	// Token: 0x04001879 RID: 6265
	[HideInInspector]
	[SerializeField]
	private string _dateTimeString;

	// Token: 0x0400187A RID: 6266
	private DateTime _dateTime;
}
