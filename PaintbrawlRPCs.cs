using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000D2D RID: 3373
internal class PaintbrawlRPCs : RPCNetworkBase
{
	// Token: 0x06005377 RID: 21367 RVA: 0x001B7F01 File Offset: 0x001B6101
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.paintbrawlManager = (GorillaPaintbrawlManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x06005378 RID: 21368 RVA: 0x001B7F1C File Offset: 0x001B611C
	[PunRPC]
	public void RPC_ReportSlingshotHit(Player taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RPC_ReportSlingshotHit");
		if (!NetworkSystem.Instance.IsMasterClient || taggedPlayer == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(taggedPlayer);
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		this.paintbrawlManager.ReportSlingshotHit(player, hitLocation, projectileCount, photonMessageInfoWrapped);
	}

	// Token: 0x04006513 RID: 25875
	private GameModeSerializer serializer;

	// Token: 0x04006514 RID: 25876
	private GorillaPaintbrawlManager paintbrawlManager;
}
