using System;
using GorillaGameModes;

namespace GorillaNetworking
{
	// Token: 0x020010ED RID: 4333
	public class CustomMapNetworkJoinTrigger : GorillaNetworkJoinTrigger
	{
		// Token: 0x06006CB8 RID: 27832 RVA: 0x002325FC File Offset: 0x002307FC
		public override string GetFullDesiredGameModeString()
		{
			return new GameModeString
			{
				zone = this.networkZone,
				queue = GorillaComputer.instance.currentQueue,
				gameType = base.GetDesiredGameType(),
				modId = CustomMapLoader.LoadedMapModId.ToString(),
				modFileId = CustomMapLoader.LoadedMapModFileId.ToString()
			}.ToString();
		}

		// Token: 0x06006CB9 RID: 27833 RVA: 0x00232669 File Offset: 0x00230869
		public override byte GetRoomSize(bool subscribed)
		{
			return CustomMapLoader.GetRoomSizeForCurrentlyLoadedMap();
		}
	}
}
