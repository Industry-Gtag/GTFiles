using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Sockets;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000393 RID: 915
internal class RequestableOwnershipGuardHandler : IPunOwnershipCallbacks, IInRoomCallbacks, INetworkRunnerCallbacks, IPublicFacingInterface
{
	// Token: 0x0600161C RID: 5660 RVA: 0x0008121D File Offset: 0x0007F41D
	static RequestableOwnershipGuardHandler()
	{
		PhotonNetwork.AddCallbackTarget(RequestableOwnershipGuardHandler.callbackInstance);
	}

	// Token: 0x0600161D RID: 5661 RVA: 0x00081247 File Offset: 0x0007F447
	internal static void RegisterView(NetworkView view, RequestableOwnershipGuard guard)
	{
		if (view == null || RequestableOwnershipGuardHandler.gaurdedViews.Contains(view))
		{
			return;
		}
		RequestableOwnershipGuardHandler.gaurdedViews.Add(view);
		RequestableOwnershipGuardHandler.guardingLookup.Add(view, guard);
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x00081278 File Offset: 0x0007F478
	internal static void RemoveView(NetworkView view)
	{
		if (view == null)
		{
			return;
		}
		RequestableOwnershipGuardHandler.gaurdedViews.Remove(view);
		RequestableOwnershipGuardHandler.guardingLookup.Remove(view);
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x0008129C File Offset: 0x0007F49C
	internal static void RegisterViews(NetworkView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGuardHandler.RegisterView(views[i], guard);
		}
	}

	// Token: 0x06001620 RID: 5664 RVA: 0x000812C4 File Offset: 0x0007F4C4
	public static void RemoveViews(NetworkView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGuardHandler.RemoveView(views[i]);
		}
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x000812EC File Offset: 0x0007F4EC
	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		NetworkView networkView = RequestableOwnershipGuardHandler.gaurdedViews.FirstOrDefault((NetworkView p) => p.GetView == targetView);
		RequestableOwnershipGuard requestableOwnershipGuard;
		if (networkView.IsNull() || !RequestableOwnershipGuardHandler.guardingLookup.TryGetValue(networkView, out requestableOwnershipGuard) || requestableOwnershipGuard.IsNull())
		{
			return;
		}
		NetPlayer currentOwner = requestableOwnershipGuard.currentOwner;
		Player player = ((currentOwner != null) ? currentOwner.GetPlayerRef() : null);
		int num = ((player != null) ? player.ActorNumber : 0);
		if (num == 0 || previousOwner != player)
		{
			GTDev.LogWarning<string>("Ownership transferred but the previous owner didn't initiate the request, Switching back", null);
			targetView.OwnerActorNr = num;
			targetView.ControllerActorNr = num;
		}
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x0008138B File Offset: 0x0007F58B
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		this.OnHostChangedShared();
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x0008138B File Offset: 0x0007F58B
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.OnHostChangedShared();
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x00081394 File Offset: 0x0007F594
	private void OnHostChangedShared()
	{
		foreach (NetworkView networkView in RequestableOwnershipGuardHandler.gaurdedViews)
		{
			RequestableOwnershipGuard requestableOwnershipGuard;
			if (!RequestableOwnershipGuardHandler.guardingLookup.TryGetValue(networkView, out requestableOwnershipGuard))
			{
				break;
			}
			if (networkView.Owner != null && requestableOwnershipGuard.currentOwner != null && !object.Equals(networkView.Owner, requestableOwnershipGuard.currentOwner))
			{
				networkView.OwnerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
				networkView.ControllerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
			}
		}
	}

	// Token: 0x06001625 RID: 5669 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06001626 RID: 5670 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x06001627 RID: 5671 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06001628 RID: 5672 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06001629 RID: 5673 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x0600162A RID: 5674 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x0600162B RID: 5675 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnConnectedToServer(NetworkRunner runner)
	{
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x0600163C RID: 5692 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x04002072 RID: 8306
	private static HashSet<NetworkView> gaurdedViews = new HashSet<NetworkView>();

	// Token: 0x04002073 RID: 8307
	private static readonly RequestableOwnershipGuardHandler callbackInstance = new RequestableOwnershipGuardHandler();

	// Token: 0x04002074 RID: 8308
	private static Dictionary<NetworkView, RequestableOwnershipGuard> guardingLookup = new Dictionary<NetworkView, RequestableOwnershipGuard>();
}
