using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x020013F6 RID: 5110
	[CreateAssetMenu(fileName = "New CountdownText Date", menuName = "Game Object Scheduling/CountdownText Date", order = 1)]
	public class CountdownTextDate : ScriptableObject
	{
		// Token: 0x040091E3 RID: 37347
		public string CountdownTo = "1/1/0001 00:00:00";

		// Token: 0x040091E4 RID: 37348
		public string FormatString = "{0} {1}";

		// Token: 0x040091E5 RID: 37349
		public string DefaultString = "";

		// Token: 0x040091E6 RID: 37350
		public int DaysThreshold = 365;
	}
}
