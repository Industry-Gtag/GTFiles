using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000D4B RID: 3403
[Serializable]
internal class RoomCount : PrivateRoomCount
{
	// Token: 0x0600545A RID: 21594 RVA: 0x001BC2C4 File Offset: 0x001BA4C4
	public int GetRoomCount(GTZone zone)
	{
		for (int i = 0; i < this.zoneCountOverrides.Length; i++)
		{
			if (this.zoneCountOverrides[i].Zone == zone)
			{
				return this.zoneCountOverrides[i].Count;
			}
		}
		return this.count;
	}

	// Token: 0x0600545B RID: 21595 RVA: 0x001BC308 File Offset: 0x001BA508
	public override int GetRoomCount(GTZone zone, GameModeType mode)
	{
		return Mathf.Min(this.GetRoomCount(zone), base.GetRoomCount(mode));
	}

	// Token: 0x040065DC RID: 26076
	[SerializeField]
	private RoomCountForZone[] zoneCountOverrides;
}
