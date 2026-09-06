using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x0200046A RID: 1130
public static class NetworkSystemRaiseEvent
{
	// Token: 0x06001B76 RID: 7030 RVA: 0x000957AA File Offset: 0x000939AA
	public static void RaiseEvent(byte code, object data)
	{
		PhotonNetwork.RaiseEvent(code, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x000957C0 File Offset: 0x000939C0
	public static void RaiseEvent(byte code, object data, NetEventOptions options, bool reliable)
	{
		PhotonNetwork.RaiseEvent(code, data, new RaiseEventOptions
		{
			TargetActors = options.TargetActors,
			Receivers = (ReceiverGroup)options.Reciever,
			Flags = options.Flags
		}, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
	}

	// Token: 0x040025AC RID: 9644
	public static readonly NetEventOptions neoOthers = new NetEventOptions
	{
		Reciever = NetEventOptions.RecieverTarget.others
	};

	// Token: 0x040025AD RID: 9645
	public static readonly NetEventOptions neoMaster = new NetEventOptions
	{
		Reciever = NetEventOptions.RecieverTarget.master
	};

	// Token: 0x040025AE RID: 9646
	public static readonly NetEventOptions neoTarget = new NetEventOptions
	{
		TargetActors = new int[1]
	};
}
