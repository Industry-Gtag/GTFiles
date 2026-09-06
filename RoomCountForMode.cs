using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000D4D RID: 3405
[Serializable]
internal class RoomCountForMode
{
	// Token: 0x1700080A RID: 2058
	// (get) Token: 0x06005460 RID: 21600 RVA: 0x001BC335 File Offset: 0x001BA535
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x1700080B RID: 2059
	// (get) Token: 0x06005461 RID: 21601 RVA: 0x001BC33D File Offset: 0x001BA53D
	public GameModeType Mode
	{
		get
		{
			return this.mode;
		}
	}

	// Token: 0x040065DF RID: 26079
	[SerializeField]
	private GameModeType mode;

	// Token: 0x040065E0 RID: 26080
	[SerializeField]
	private int count;
}
