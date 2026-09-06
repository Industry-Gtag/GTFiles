using System;
using System.Globalization;
using GameObjectScheduling;
using UnityEngine;

// Token: 0x020000AD RID: 173
[CreateAssetMenu(fileName = "New Game Object Schedule Generator", menuName = "Game Object Scheduling/Game Object Schedule Generator")]
public class GameObjectScheduleGenerator : ScriptableObject
{
	// Token: 0x06000433 RID: 1075 RVA: 0x00018C38 File Offset: 0x00016E38
	private void GenerateSchedule()
	{
		DateTime dateTime;
		try
		{
			dateTime = DateTime.Parse(this.scheduleStart, CultureInfo.InvariantCulture);
		}
		catch
		{
			Debug.LogError("Don't understand Start Date " + this.scheduleStart);
			return;
		}
		DateTime dateTime2;
		try
		{
			dateTime2 = DateTime.Parse(this.scheduleEnd, CultureInfo.InvariantCulture);
		}
		catch
		{
			Debug.LogError("Don't understand End Date " + this.scheduleEnd);
			return;
		}
		if (this.scheduleType == GameObjectScheduleGenerator.ScheduleType.DailyShuffle)
		{
			GameObjectSchedule.GenerateDailyShuffle(dateTime, dateTime2, this.schedules);
		}
	}

	// Token: 0x04000494 RID: 1172
	[SerializeField]
	private GameObjectSchedule[] schedules;

	// Token: 0x04000495 RID: 1173
	[SerializeField]
	private string scheduleStart = "1/1/0001 00:00:00";

	// Token: 0x04000496 RID: 1174
	[SerializeField]
	private string scheduleEnd = "1/1/0001 00:00:00";

	// Token: 0x04000497 RID: 1175
	[SerializeField]
	private GameObjectScheduleGenerator.ScheduleType scheduleType;

	// Token: 0x020000AE RID: 174
	private enum ScheduleType
	{
		// Token: 0x04000499 RID: 1177
		DailyShuffle
	}
}
