using System;
using Photon.Pun;

// Token: 0x02000D2B RID: 3371
internal class GorillaTagCompetitiveRPCs : RPCNetworkBase
{
	// Token: 0x0600536E RID: 21358 RVA: 0x001B7AE1 File Offset: 0x001B5CE1
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.tagCompManager = (GorillaTagCompetitiveManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x0600536F RID: 21359 RVA: 0x001B7AFC File Offset: 0x001B5CFC
	[PunRPC]
	public void SendScoresToLateJoinerRPC(int[] playerId, int[] numTags, float[] pointsOnDefense, float[] joinTime, bool[] infected, float[] taggedTime, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "SendScoresToLateJoinerRPC");
		if (info.Sender == null || !info.Sender.IsMasterClient)
		{
			return;
		}
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (photonMessageInfoWrapped.Sender.CheckSingleCallRPC(NetPlayer.SingleCallRPC.RankedSendScoreToLateJoiner))
		{
			return;
		}
		photonMessageInfoWrapped.Sender.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.RankedSendScoreToLateJoiner);
		if (playerId == null || numTags == null || pointsOnDefense == null || joinTime == null || infected == null || taggedTime == null)
		{
			return;
		}
		int num = playerId.Length;
		if (num > 10)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = i + 1; j < num; j++)
			{
				if (playerId[i] == playerId[j])
				{
					return;
				}
			}
		}
		if (numTags.Length != num || pointsOnDefense.Length != num || joinTime.Length != num || infected.Length != num || taggedTime.Length != num)
		{
			return;
		}
		for (int k = 0; k < num; k++)
		{
			if (NetworkSystem.Instance.GetNetPlayerByID(playerId[k]) == null)
			{
				return;
			}
			if (numTags[k] < 0 || numTags[k] >= 15)
			{
				return;
			}
			if (pointsOnDefense[k] < 0f)
			{
				return;
			}
			float num2 = joinTime[k];
			if (float.IsNaN(num2) || float.IsInfinity(num2) || num2 < 0f || num2 > this.tagCompManager.GetRoundDuration() + 15f)
			{
				return;
			}
			float num3 = taggedTime[k];
			if (float.IsNaN(num3) || float.IsInfinity(num3) || num3 < 0f || num3 > this.tagCompManager.GetRoundDuration() + 15f)
			{
				return;
			}
		}
		this.tagCompManager.GetScoring().ReceivedScoresForLateJoiner(playerId, numTags, pointsOnDefense, joinTime, infected, taggedTime);
	}

	// Token: 0x0400650C RID: 25868
	private GameModeSerializer serializer;

	// Token: 0x0400650D RID: 25869
	private GorillaTagCompetitiveManager tagCompManager;
}
