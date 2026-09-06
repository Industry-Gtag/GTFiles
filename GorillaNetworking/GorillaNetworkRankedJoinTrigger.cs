using System;
using GorillaGameModes;

namespace GorillaNetworking
{
	// Token: 0x020010F6 RID: 4342
	public class GorillaNetworkRankedJoinTrigger : GorillaNetworkJoinTrigger
	{
		// Token: 0x06006CED RID: 27885 RVA: 0x00233736 File Offset: 0x00231936
		public override string GetFullDesiredGameModeString()
		{
			return new GameModeString
			{
				zone = this.networkZone,
				gameType = base.GetDesiredGameType()
			}.ToString();
		}

		// Token: 0x06006CEE RID: 27886 RVA: 0x0023375A File Offset: 0x0023195A
		public override void OnBoxTriggered()
		{
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			PhotonNetworkController.Instance.ClearDeferredJoin();
			PhotonNetworkController.Instance.AttemptToJoinRankedPublicRoom(this, JoinType.Solo);
		}
	}
}
