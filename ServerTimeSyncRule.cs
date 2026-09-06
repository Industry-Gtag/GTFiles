using System;
using UnityEngine;

// Token: 0x02000E03 RID: 3587
[CreateAssetMenu(fileName = "ServerTimeSyncRule", menuName = "Scriptable Objects/ServerTimeSyncRule")]
public class ServerTimeSyncRule : ScriptableObject
{
	// Token: 0x060057DF RID: 22495 RVA: 0x001CA2D8 File Offset: 0x001C84D8
	public DateTime GetPrevious(DateTime dt)
	{
		DateTime dateTime = DateTime.MinValue;
		switch (this.unit)
		{
		case ServerTimeSyncRule.Unit.Hours:
			dateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
			dateTime = dateTime.AddHours((double)(-(double)(dt.Hour % this.value)));
			break;
		case ServerTimeSyncRule.Unit.Minutes:
			dateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
			dateTime = dateTime.AddMinutes((double)(-(double)(dt.Minute % this.value)));
			break;
		case ServerTimeSyncRule.Unit.Seconds:
			dateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
			dateTime = dateTime.AddSeconds((double)(-(double)(dt.Second % this.value)));
			break;
		}
		return dateTime;
	}

	// Token: 0x060057E0 RID: 22496 RVA: 0x001CA3DC File Offset: 0x001C85DC
	public DateTime GetNext(DateTime dt)
	{
		DateTime dateTime = DateTime.MaxValue;
		switch (this.unit)
		{
		case ServerTimeSyncRule.Unit.Hours:
			dateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
			dateTime = dateTime.AddHours((double)(this.value - dt.Hour % this.value));
			break;
		case ServerTimeSyncRule.Unit.Minutes:
			dateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
			dateTime = dateTime.AddMinutes((double)(this.value - dt.Minute % this.value));
			break;
		case ServerTimeSyncRule.Unit.Seconds:
			dateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
			dateTime = dateTime.AddSeconds((double)(this.value - dt.Second % this.value));
			break;
		}
		return dateTime;
	}

	// Token: 0x04006842 RID: 26690
	[SerializeField]
	private ServerTimeSyncRule.Unit unit;

	// Token: 0x04006843 RID: 26691
	[SerializeField]
	private int value;

	// Token: 0x02000E04 RID: 3588
	private enum Unit
	{
		// Token: 0x04006845 RID: 26693
		Hours,
		// Token: 0x04006846 RID: 26694
		Minutes,
		// Token: 0x04006847 RID: 26695
		Seconds
	}
}
