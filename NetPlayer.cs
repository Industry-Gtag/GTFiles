using System;
using System.Collections.Generic;
using Fusion;
using GorillaTag;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000457 RID: 1111
[Serializable]
public abstract class NetPlayer : ObjectPoolEvents
{
	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x06001A83 RID: 6787
	public abstract bool IsValid { get; }

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06001A84 RID: 6788
	public abstract int ActorNumber { get; }

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x06001A85 RID: 6789
	public abstract string UserId { get; }

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x06001A86 RID: 6790
	public abstract bool IsMasterClient { get; }

	// Token: 0x170002AC RID: 684
	// (get) Token: 0x06001A87 RID: 6791
	public abstract bool IsLocal { get; }

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x06001A88 RID: 6792
	public abstract bool IsNull { get; }

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x06001A89 RID: 6793
	public abstract string NickName { get; }

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x06001A8A RID: 6794 RVA: 0x0009479C File Offset: 0x0009299C
	// (set) Token: 0x06001A8B RID: 6795 RVA: 0x000947A4 File Offset: 0x000929A4
	public virtual string SanitizedNickName { get; set; } = string.Empty;

	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06001A8C RID: 6796
	public abstract string DefaultName { get; }

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x06001A8D RID: 6797
	public abstract bool InRoom { get; }

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x06001A8E RID: 6798 RVA: 0x000947AD File Offset: 0x000929AD
	// (set) Token: 0x06001A8F RID: 6799 RVA: 0x000947B5 File Offset: 0x000929B5
	public virtual float JoinedTime { get; private set; }

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x06001A90 RID: 6800 RVA: 0x000947BE File Offset: 0x000929BE
	// (set) Token: 0x06001A91 RID: 6801 RVA: 0x000947C6 File Offset: 0x000929C6
	public virtual float LeftTime { get; private set; }

	// Token: 0x06001A92 RID: 6802
	public abstract bool Equals(NetPlayer myPlayer, NetPlayer other);

	// Token: 0x06001A93 RID: 6803 RVA: 0x000947CF File Offset: 0x000929CF
	public virtual void OnReturned()
	{
		this.LeftTime = Time.time;
		HashSet<int> singleCallRPCStatus = this.SingleCallRPCStatus;
		if (singleCallRPCStatus != null)
		{
			singleCallRPCStatus.Clear();
		}
		this.SanitizedNickName = string.Empty;
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x000947F8 File Offset: 0x000929F8
	public virtual void OnTaken()
	{
		this.JoinedTime = Time.time;
		HashSet<int> singleCallRPCStatus = this.SingleCallRPCStatus;
		if (singleCallRPCStatus == null)
		{
			return;
		}
		singleCallRPCStatus.Clear();
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x00094815 File Offset: 0x00092A15
	public virtual bool CheckSingleCallRPC(NetPlayer.SingleCallRPC RPCType)
	{
		return this.SingleCallRPCStatus.Contains((int)RPCType);
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x00094823 File Offset: 0x00092A23
	public virtual void ReceivedSingleCallRPC(NetPlayer.SingleCallRPC RPCType)
	{
		this.SingleCallRPCStatus.Add((int)RPCType);
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x00094832 File Offset: 0x00092A32
	public Player GetPlayerRef()
	{
		return (this as PunNetPlayer).PlayerRef;
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x0009483F File Offset: 0x00092A3F
	public string ToStringFull()
	{
		return string.Format("#{0: 0:00} '{1}', Not sure what to do with inactive yet, Or custom props?", this.ActorNumber, this.NickName);
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x0009485C File Offset: 0x00092A5C
	public static implicit operator NetPlayer(Player player)
	{
		Utils.Log("Using an implicit cast from Player to NetPlayer. Please make sure this was intended as this has potential to cause errors when switching between network backends");
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x0009487F File Offset: 0x00092A7F
	public static implicit operator NetPlayer(PlayerRef player)
	{
		Utils.Log("Using an implicit cast from PlayerRef to NetPlayer. Please make sure this was intended as this has potential to cause errors when switching between network backends");
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x000948A2 File Offset: 0x00092AA2
	public static NetPlayer Get(Player player)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x000948BB File Offset: 0x00092ABB
	public static NetPlayer Get(PlayerRef player)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x000948D4 File Offset: 0x00092AD4
	public static NetPlayer Get(int actorNr)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(actorNr) : null) ?? null;
	}

	// Token: 0x0400255C RID: 9564
	private HashSet<int> SingleCallRPCStatus = new HashSet<int>(5);

	// Token: 0x02000458 RID: 1112
	public enum SingleCallRPC
	{
		// Token: 0x0400255E RID: 9566
		CMS_RequestRoomInitialization,
		// Token: 0x0400255F RID: 9567
		CMS_RequestTriggerHistory,
		// Token: 0x04002560 RID: 9568
		CMS_SyncTriggerHistory,
		// Token: 0x04002561 RID: 9569
		CMS_SyncTriggerCounts,
		// Token: 0x04002562 RID: 9570
		RankedSendScoreToLateJoiner,
		// Token: 0x04002563 RID: 9571
		Count
	}
}
