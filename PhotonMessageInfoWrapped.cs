using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000456 RID: 1110
public struct PhotonMessageInfoWrapped
{
	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x06001A7B RID: 6779 RVA: 0x0009467D File Offset: 0x0009287D
	public double SentServerTime
	{
		get
		{
			return this.sentTick / 1000.0;
		}
	}

	// Token: 0x06001A7C RID: 6780 RVA: 0x00094691 File Offset: 0x00092891
	public PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		Player sender = info.Sender;
		this.senderID = ((sender != null) ? sender.ActorNumber : (-1));
		this.Sender = NetPlayer.Get(info.Sender);
		this.sentTick = info.SentServerTimestamp;
		this.punInfo = info;
	}

	// Token: 0x06001A7D RID: 6781 RVA: 0x000946D0 File Offset: 0x000928D0
	public PhotonMessageInfoWrapped(RpcInfo info)
	{
		this.senderID = info.Source.PlayerId;
		this.Sender = NetPlayer.Get(info.Source);
		this.sentTick = info.Tick.Raw;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x06001A7E RID: 6782 RVA: 0x0009471D File Offset: 0x0009291D
	public PhotonMessageInfoWrapped(int playerID, int tick)
	{
		this.senderID = playerID;
		this.Sender = NetworkSystem.Instance.GetPlayer(this.senderID);
		this.sentTick = tick;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x0009474F File Offset: 0x0009294F
	private PhotonMessageInfoWrapped(NetPlayer sender)
	{
		this.Sender = sender;
		this.senderID = sender.ActorNumber;
		this.sentTick = PhotonNetwork.ServerTimestamp;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x0009477B File Offset: 0x0009297B
	public static implicit operator PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		return new PhotonMessageInfoWrapped(info);
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x00094783 File Offset: 0x00092983
	public static implicit operator PhotonMessageInfoWrapped(RpcInfo info)
	{
		return new PhotonMessageInfoWrapped(info);
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x0009478B File Offset: 0x0009298B
	public static PhotonMessageInfoWrapped GetLocalDefault()
	{
		return new PhotonMessageInfoWrapped(NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x04002555 RID: 9557
	public readonly int senderID;

	// Token: 0x04002556 RID: 9558
	public readonly int sentTick;

	// Token: 0x04002557 RID: 9559
	public readonly PhotonMessageInfo punInfo;

	// Token: 0x04002558 RID: 9560
	public readonly NetPlayer Sender;
}
