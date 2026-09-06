using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x0200114D RID: 4429
	public class StandTypeData
	{
		// Token: 0x06006F3F RID: 28479 RVA: 0x0023DF54 File Offset: 0x0023C154
		public StandTypeData(string[] spawnData)
		{
			this.departmentID = spawnData[0];
			this.displayID = spawnData[1];
			this.standID = spawnData[2];
			this.bustType = spawnData[3];
			if (spawnData.Length == 5)
			{
				this.playFabID = spawnData[4];
			}
			Debug.Log(string.Concat(new string[] { "StoreStuff: StandTypeData: ", this.departmentID, "\n", this.displayID, "\n", this.standID, "\n", this.bustType, "\n", this.playFabID }));
		}

		// Token: 0x06006F40 RID: 28480 RVA: 0x0023E038 File Offset: 0x0023C238
		public StandTypeData(string departmentID, string displayID, string standID, HeadModel_CosmeticStand.BustType bustType, string playFabID)
		{
			this.departmentID = departmentID;
			this.displayID = displayID;
			this.standID = standID;
			this.bustType = bustType.ToString();
			this.playFabID = playFabID;
		}

		// Token: 0x04007F37 RID: 32567
		public string departmentID = "";

		// Token: 0x04007F38 RID: 32568
		public string displayID = "";

		// Token: 0x04007F39 RID: 32569
		public string standID = "";

		// Token: 0x04007F3A RID: 32570
		public string bustType = "";

		// Token: 0x04007F3B RID: 32571
		public string playFabID = "";

		// Token: 0x0200114E RID: 4430
		public enum EStandDataID
		{
			// Token: 0x04007F3D RID: 32573
			departmentID,
			// Token: 0x04007F3E RID: 32574
			displayID,
			// Token: 0x04007F3F RID: 32575
			standID,
			// Token: 0x04007F40 RID: 32576
			bustType,
			// Token: 0x04007F41 RID: 32577
			playFabID,
			// Token: 0x04007F42 RID: 32578
			Count
		}
	}
}
