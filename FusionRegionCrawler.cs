using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;

// Token: 0x02000439 RID: 1081
public class FusionRegionCrawler : MonoBehaviour, INetworkRunnerCallbacks, IPublicFacingInterface
{
	// Token: 0x1700028E RID: 654
	// (get) Token: 0x060019C1 RID: 6593 RVA: 0x0009040C File Offset: 0x0008E60C
	public int PlayerCountGlobal
	{
		get
		{
			return this.globalPlayerCount;
		}
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x00090414 File Offset: 0x0008E614
	public void Start()
	{
		this.regionRunner = base.gameObject.AddComponent<NetworkRunner>();
		this.regionRunner.AddCallbacks(new INetworkRunnerCallbacks[] { this });
		base.StartCoroutine(this.OccasionalUpdate());
	}

	// Token: 0x060019C3 RID: 6595 RVA: 0x00090449 File Offset: 0x0008E649
	public IEnumerator OccasionalUpdate()
	{
		while (this.refreshPlayerCountAutomatically)
		{
			yield return this.UpdatePlayerCount();
			yield return new WaitForSeconds(this.UpdateFrequency);
		}
		yield break;
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x00090458 File Offset: 0x0008E658
	public IEnumerator UpdatePlayerCount()
	{
		int tempGlobalPlayerCount = 0;
		StartGameArgs startGameArgs = default(StartGameArgs);
		foreach (string text in NetworkSystem.Instance.regionNames)
		{
			startGameArgs.CustomPhotonAppSettings = new FusionAppSettings();
			startGameArgs.CustomPhotonAppSettings.FixedRegion = text;
			this.waitingForSessionListUpdate = true;
			this.regionRunner.JoinSessionLobby(SessionLobby.ClientServer, startGameArgs.CustomPhotonAppSettings.FixedRegion, null, null, new bool?(false), default(CancellationToken), true);
			while (this.waitingForSessionListUpdate)
			{
				yield return new WaitForEndOfFrame();
			}
			foreach (SessionInfo sessionInfo in this.sessionInfoCache)
			{
				tempGlobalPlayerCount += sessionInfo.PlayerCount;
			}
			tempGlobalPlayerCount += this.tempSessionPlayerCount;
		}
		string[] array = null;
		this.globalPlayerCount = tempGlobalPlayerCount;
		FusionRegionCrawler.PlayerCountUpdated onPlayerCountUpdated = this.OnPlayerCountUpdated;
		if (onPlayerCountUpdated != null)
		{
			onPlayerCountUpdated(this.globalPlayerCount);
		}
		yield break;
	}

	// Token: 0x060019C5 RID: 6597 RVA: 0x00090467 File Offset: 0x0008E667
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		if (this.waitingForSessionListUpdate)
		{
			this.sessionInfoCache = sessionList;
			this.waitingForSessionListUpdate = false;
		}
	}

	// Token: 0x060019C6 RID: 6598 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x060019C8 RID: 6600 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	// Token: 0x060019C9 RID: 6601 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	// Token: 0x060019CB RID: 6603 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x060019CD RID: 6605 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x00002C2D File Offset: 0x00000E2D
	void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x04002487 RID: 9351
	public FusionRegionCrawler.PlayerCountUpdated OnPlayerCountUpdated;

	// Token: 0x04002488 RID: 9352
	private NetworkRunner regionRunner;

	// Token: 0x04002489 RID: 9353
	private List<SessionInfo> sessionInfoCache;

	// Token: 0x0400248A RID: 9354
	private bool waitingForSessionListUpdate;

	// Token: 0x0400248B RID: 9355
	private int globalPlayerCount;

	// Token: 0x0400248C RID: 9356
	private float UpdateFrequency = 10f;

	// Token: 0x0400248D RID: 9357
	private bool refreshPlayerCountAutomatically = true;

	// Token: 0x0400248E RID: 9358
	private int tempSessionPlayerCount;

	// Token: 0x0200043A RID: 1082
	// (Invoke) Token: 0x060019DB RID: 6619
	public delegate void PlayerCountUpdated(int playerCount);
}
