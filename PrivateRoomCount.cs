using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000D4A RID: 3402
[Serializable]
internal class PrivateRoomCount
{
	// Token: 0x06005456 RID: 21590 RVA: 0x001BC26A File Offset: 0x001BA46A
	public int GetRoomCount()
	{
		return this.count;
	}

	// Token: 0x06005457 RID: 21591 RVA: 0x001BC274 File Offset: 0x001BA474
	public int GetRoomCount(GameModeType mode)
	{
		for (int i = 0; i < this.modeCountOverrides.Length; i++)
		{
			if (this.modeCountOverrides[i].Mode == mode)
			{
				return this.modeCountOverrides[i].Count;
			}
		}
		return this.count;
	}

	// Token: 0x06005458 RID: 21592 RVA: 0x001BC2B8 File Offset: 0x001BA4B8
	public virtual int GetRoomCount(GTZone zone, GameModeType mode)
	{
		return this.GetRoomCount(mode);
	}

	// Token: 0x040065DA RID: 26074
	[SerializeField]
	protected int count;

	// Token: 0x040065DB RID: 26075
	[SerializeField]
	protected RoomCountForMode[] modeCountOverrides;
}
