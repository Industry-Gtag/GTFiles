using System;

namespace GorillaGameModes
{
	// Token: 0x02000F32 RID: 3890
	[Serializable]
	public struct ZoneGameModes
	{
		// Token: 0x04006DE1 RID: 28129
		public GTZone[] zone;

		// Token: 0x04006DE2 RID: 28130
		public GameModeType[] modes;

		// Token: 0x04006DE3 RID: 28131
		public GameModeType[] privateModes;
	}
}
