using System;
using System.Collections;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000AD2 RID: 2770
internal class VirtualStumpSerializer : GorillaSerializer
{
	// Token: 0x170006A7 RID: 1703
	// (get) Token: 0x06004707 RID: 18183 RVA: 0x0012D775 File Offset: 0x0012B975
	internal bool HasAuthority
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x06004708 RID: 18184 RVA: 0x0017F7DC File Offset: 0x0017D9DC
	protected void Start()
	{
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnLeftRoom;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
	}

	// Token: 0x06004709 RID: 18185 RVA: 0x0017F84C File Offset: 0x0017DA4C
	private void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		int driverID = CustomMapsTerminal.GetDriverID();
		if (leavingPlayer.ActorNumber == driverID)
		{
			CustomMapsTerminal.SetTerminalControlStatus(false, -2, true);
		}
	}

	// Token: 0x0600470A RID: 18186 RVA: 0x0017F87E File Offset: 0x0017DA7E
	private void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			VirtualStumpSerializer.roomInitialized = true;
			return;
		}
		Debug.Log("[VirtualStumpSerializer::OnJoinedRoom] Requesting Room Initialization...");
		VirtualStumpSerializer.waitingForRoomInitialization = true;
		base.SendRPC("RequestRoomInitialization_RPC", false, Array.Empty<object>());
	}

	// Token: 0x0600470B RID: 18187 RVA: 0x0017F8B4 File Offset: 0x0017DAB4
	private void OnLeftRoom()
	{
		Debug.Log("[VirtualStumpSerializer::OnLeftRoom]...");
		VirtualStumpSerializer.roomInitialized = false;
	}

	// Token: 0x0600470C RID: 18188 RVA: 0x0017F8C6 File Offset: 0x0017DAC6
	public static bool IsWaitingForRoomInit()
	{
		return VirtualStumpSerializer.waitingForRoomInitialization || !VirtualStumpSerializer.roomInitialized;
	}

	// Token: 0x0600470D RID: 18189 RVA: 0x0017F8DC File Offset: 0x0017DADC
	[PunRPC]
	private void RequestRoomInitialization_RPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestRoomInitialization_RPC");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestRoomInitialization))
		{
			return;
		}
		player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestRoomInitialization);
		long id = CustomMapManager.GetRoomMapId()._id;
		base.SendRPC("InitializeRoom_RPC", info.Sender, new object[]
		{
			CustomMapsTerminal.CurrentScreen,
			CustomMapsTerminal.GetDriverID(),
			CustomMapsTerminal.LocalModDetailsID,
			id
		});
	}

	// Token: 0x0600470E RID: 18190 RVA: 0x0017F978 File Offset: 0x0017DB78
	[PunRPC]
	private void InitializeRoom_RPC(int currentScreen, int driverID, long modDetailsID, long loadedMapModID, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "InitializeRoom_RPC");
		if (!info.Sender.IsMasterClient || !VirtualStumpSerializer.waitingForRoomInitialization)
		{
			return;
		}
		if (driverID != -2 && NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		CustomMapsTerminal.UpdateFromDriver(currentScreen, modDetailsID, driverID);
		if (loadedMapModID > 0L)
		{
			CustomMapManager.SetRoomMap(loadedMapModID);
		}
		VirtualStumpSerializer.roomInitialized = true;
		VirtualStumpSerializer.waitingForRoomInitialization = false;
		Debug.Log("[VStumpSerializer.InitializeRPC] Room initialization finished.");
	}

	// Token: 0x0600470F RID: 18191 RVA: 0x0017F9E8 File Offset: 0x0017DBE8
	public void LoadMapSynced(long modId, GTMapLoadSource loadSource = GTMapLoadSource.none)
	{
		CustomMapManager.SetRoomMap(modId);
		CustomMapManager.LoadMap(new ModId(modId), loadSource);
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			base.SendRPC("SetRoomMap_RPC", true, new object[] { modId });
		}
	}

	// Token: 0x06004710 RID: 18192 RVA: 0x0017FA3B File Offset: 0x0017DC3B
	public void UnloadMapSynced()
	{
		CustomMapManager.UnloadMap(true);
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			base.SendRPC("UnloadMap_RPC", true, Array.Empty<object>());
		}
	}

	// Token: 0x06004711 RID: 18193 RVA: 0x0017FA70 File Offset: 0x0017DC70
	[PunRPC]
	private void SetRoomMap_RPC(long modId, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "SetRoomMap_RPC");
		if (modId <= 0L)
		{
			return;
		}
		if (info.Sender.ActorNumber != this.photonView.OwnerActorNr && info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		if (modId != this.detailsScreen.currentMapMod.Id._id)
		{
			return;
		}
		CustomMapManager.SetRoomMap(modId);
	}

	// Token: 0x06004712 RID: 18194 RVA: 0x0017FAD8 File Offset: 0x0017DCD8
	[PunRPC]
	private void UnloadMap_RPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "UnloadMap_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		if (!CustomMapManager.AreAllPlayersInVirtualStump())
		{
			return;
		}
		CustomMapManager.UnloadMap(true);
	}

	// Token: 0x06004713 RID: 18195 RVA: 0x0017FB07 File Offset: 0x0017DD07
	public void RequestTerminalControlStatusChange(bool lockedStatus)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RequestTerminalControlStatusChange_RPC", false, new object[] { lockedStatus });
	}

	// Token: 0x06004714 RID: 18196 RVA: 0x0017FB34 File Offset: 0x0017DD34
	[PunRPC]
	private void RequestTerminalControlStatusChange_RPC(bool lockedStatus, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestTerminalControlStatusChange_RPC");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[19].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (!player.IsNull && CustomMapManager.IsRemotePlayerInVirtualStump(info.Sender.UserId))
		{
			CustomMapsTerminal.HandleTerminalControlStatusChangeRequest(lockedStatus, info.Sender.ActorNumber);
		}
	}

	// Token: 0x06004715 RID: 18197 RVA: 0x0017FBC9 File Offset: 0x0017DDC9
	public void SetTerminalControlStatus(bool locked, int playerID)
	{
		if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		base.SendRPC("SetTerminalControlStatus_RPC", true, new object[] { locked, playerID });
	}

	// Token: 0x06004716 RID: 18198 RVA: 0x0017FC08 File Offset: 0x0017DE08
	[PunRPC]
	private void SetTerminalControlStatus_RPC(bool locked, int driverID, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "SetTerminalControlStatus_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (driverID != -2 && NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[16].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.SetTerminalControlStatus(locked, driverID, false);
	}

	// Token: 0x06004717 RID: 18199 RVA: 0x0017FC8E File Offset: 0x0017DE8E
	public void SendTerminalStatus()
	{
		if (!NetworkSystem.Instance.InRoom || !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (this.statusUpdateCoroutine != null)
		{
			base.StopCoroutine(this.statusUpdateCoroutine);
		}
		this.statusUpdateCoroutine = base.StartCoroutine(this.WaitToSendStatus());
	}

	// Token: 0x06004718 RID: 18200 RVA: 0x0017FCCA File Offset: 0x0017DECA
	private IEnumerator WaitToSendStatus()
	{
		yield return new WaitForSeconds(0.5f);
		base.SendRPC("UpdateScreen_RPC", true, new object[]
		{
			CustomMapsTerminal.CurrentScreen,
			CustomMapsTerminal.LocalModDetailsID,
			CustomMapsTerminal.GetDriverID()
		});
		yield break;
	}

	// Token: 0x06004719 RID: 18201 RVA: 0x0017FCDC File Offset: 0x0017DEDC
	[PunRPC]
	private void UpdateScreen_RPC(int currentScreen, long modDetailsID, int driverID, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "UpdateScreen_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID() || !CustomMapManager.IsRemotePlayerInVirtualStump(info.Sender.UserId))
		{
			return;
		}
		if (currentScreen < -1 || currentScreen > 6)
		{
			return;
		}
		if (NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[17].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.UpdateFromDriver(currentScreen, modDetailsID, driverID);
	}

	// Token: 0x0600471A RID: 18202 RVA: 0x0017FD81 File Offset: 0x0017DF81
	public void RefreshDriverNickName()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RefreshDriverNickName_RPC", true, Array.Empty<object>());
	}

	// Token: 0x0600471B RID: 18203 RVA: 0x0017FDA4 File Offset: 0x0017DFA4
	[PunRPC]
	private void RefreshDriverNickName_RPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RefreshDriverNickName_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[18].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
	}

	// Token: 0x0400599B RID: 22939
	[SerializeField]
	private VirtualStumpBarrierSFX barrierSFX;

	// Token: 0x0400599C RID: 22940
	[SerializeField]
	private CustomMapsDisplayScreen detailsScreen;

	// Token: 0x0400599D RID: 22941
	private static bool waitingForRoomInitialization;

	// Token: 0x0400599E RID: 22942
	private static bool roomInitialized;

	// Token: 0x0400599F RID: 22943
	private bool sendModList;

	// Token: 0x040059A0 RID: 22944
	private bool forceNewSearch;

	// Token: 0x040059A1 RID: 22945
	private bool waitToSendStatus;

	// Token: 0x040059A2 RID: 22946
	private bool sendNewStatus;

	// Token: 0x040059A3 RID: 22947
	private const float STATUS_UPDATE_INTERVAL = 0.5f;

	// Token: 0x040059A4 RID: 22948
	private Coroutine statusUpdateCoroutine;
}
