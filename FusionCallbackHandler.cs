using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000432 RID: 1074
public class FusionCallbackHandler : SimulationBehaviour, INetworkRunnerCallbacks, IPublicFacingInterface
{
	// Token: 0x06001976 RID: 6518 RVA: 0x0008F134 File Offset: 0x0008D334
	public void Setup(NetworkSystemFusion parentController)
	{
		this.parent = parentController;
		this.parent.runner.AddCallbacks(new INetworkRunnerCallbacks[] { this });
	}

	// Token: 0x06001977 RID: 6519 RVA: 0x0008F157 File Offset: 0x0008D357
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.RemoveCallbacks();
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x0008F168 File Offset: 0x0008D368
	private async void RemoveCallbacks()
	{
		await Task.Delay(500);
		this.parent.runner.RemoveCallbacks(new INetworkRunnerCallbacks[] { this });
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x0008F19F File Offset: 0x0008D39F
	public void OnConnectedToServer(NetworkRunner runner)
	{
		this.parent.OnJoinedSession();
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x0008F1AC File Offset: 0x0008D3AC
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		this.parent.OnJoinFailed(reason);
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x0008F1BC File Offset: 0x0008D3BC
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
		this.parent.CustomAuthenticationResponse(data);
		Debug.Log("Received custom auth response:");
		foreach (KeyValuePair<string, object> keyValuePair in data)
		{
			Debug.Log(keyValuePair.Key + ":" + (keyValuePair.Value as string));
		}
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x0008F23C File Offset: 0x0008D43C
	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		this.parent.OnDisconnectedFromSession();
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x0008F249 File Offset: 0x0008D449
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.parent.MigrateHost(runner, hostMigrationToken);
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x0008F258 File Offset: 0x0008D458
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		NetworkedInput input2 = NetInput.GetInput();
		input.Set<NetworkedInput>(input2);
	}

	// Token: 0x06001980 RID: 6528 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06001981 RID: 6529 RVA: 0x0008F274 File Offset: 0x0008D474
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerJoined(player);
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x0008F282 File Offset: 0x0008D482
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerLeft(player);
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x0008F290 File Offset: 0x0008D490
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		this.parent.OnRunnerShutDown();
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x06001989 RID: 6537 RVA: 0x0008F2A0 File Offset: 0x0008D4A0
	[Rpc(Channel = RpcChannel.Reliable)]
	public unsafe static void RPC_OnEventRaisedReliable(NetworkRunner runner, byte eventCode, byte[] byteData, bool hasOps, byte[] netOptsData, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage != SimulationStages.Resimulate)
			{
				int num = 8;
				num += 4;
				num += (byteData.Length * 1 + 4 + 3) & -4;
				num += 4;
				num += (netOptsData.Length * 1 + 4 + 3) & -4;
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)"));
						int num2 = 8;
						ptr2[num2] = eventCode;
						num2 += (1 + 3) & -4;
						*(int*)(ptr2 + num2) = byteData.Length;
						num2 += 4;
						num2 = ((Native.CopyFromArray<byte>((void*)(ptr2 + num2), byteData) + 3) & -4) + num2;
						ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), hasOps);
						num2 += 4;
						*(int*)(ptr2 + num2) = netOptsData.Length;
						num2 += 4;
						num2 = ((Native.CopyFromArray<byte>((void*)(ptr2 + num2), netOptsData) + 3) & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
					goto IL_0010;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_0010:
		object obj = byteData.ByteDeserialize();
		NetEventOptions netEventOptions = null;
		if (hasOps)
		{
			netEventOptions = (NetEventOptions)netOptsData.ByteDeserialize();
		}
		if (!FusionCallbackHandler.CanRecieveEvent(runner, netEventOptions, info))
		{
			return;
		}
		NetworkSystem.Instance.RaiseEvent(eventCode, obj, info.Source.PlayerId);
	}

	// Token: 0x0600198A RID: 6538 RVA: 0x0008F4C4 File Offset: 0x0008D6C4
	[Rpc(Channel = RpcChannel.Unreliable)]
	public unsafe static void RPC_OnEventRaisedUnreliable(NetworkRunner runner, byte eventCode, byte[] byteData, bool hasOps, byte[] netOptsData, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage != SimulationStages.Resimulate)
			{
				int num = 8;
				num += 4;
				num += (byteData.Length * 1 + 4 + 3) & -4;
				num += 4;
				num += (netOptsData.Length * 1 + 4 + 3) & -4;
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)"));
						int num2 = 8;
						ptr2[num2] = eventCode;
						num2 += (1 + 3) & -4;
						*(int*)(ptr2 + num2) = byteData.Length;
						num2 += 4;
						num2 = ((Native.CopyFromArray<byte>((void*)(ptr2 + num2), byteData) + 3) & -4) + num2;
						ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), hasOps);
						num2 += 4;
						*(int*)(ptr2 + num2) = netOptsData.Length;
						num2 += 4;
						num2 = ((Native.CopyFromArray<byte>((void*)(ptr2 + num2), netOptsData) + 3) & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetUnreliable();
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, RpcChannel.Unreliable, RpcHostMode.SourceIsServer);
					goto IL_0010;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_0010:
		object obj = byteData.ByteDeserialize();
		NetEventOptions netEventOptions = null;
		if (hasOps)
		{
			netEventOptions = (NetEventOptions)netOptsData.ByteDeserialize();
		}
		if (!FusionCallbackHandler.CanRecieveEvent(runner, netEventOptions, info))
		{
			return;
		}
		NetworkSystem.Instance.RaiseEvent(eventCode, obj, info.Source.PlayerId);
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x0008F6F0 File Offset: 0x0008D8F0
	private static bool CanRecieveEvent(NetworkRunner runner, NetEventOptions opts, RpcInfo info)
	{
		if (opts != null)
		{
			if (opts.Reciever != NetEventOptions.RecieverTarget.all)
			{
				if (opts.Reciever == NetEventOptions.RecieverTarget.master && !NetworkSystem.Instance.IsMasterClient)
				{
					return false;
				}
				if (info.Source == runner.LocalPlayer)
				{
					return false;
				}
			}
			if (opts.TargetActors != null && !opts.TargetActors.Contains(runner.LocalPlayer.PlayerId))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x0600198D RID: 6541 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x06001992 RID: 6546 RVA: 0x0008F764 File Offset: 0x0008D964
	[NetworkRpcStaticWeavedInvoker("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnEventRaisedReliable@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		byte b = ptr[num];
		num += (1 + 3) & -4;
		byte b2 = b;
		byte[] array = new byte[*(int*)(ptr + num)];
		num += 4;
		num = ((Native.CopyToArray<byte>(array, (void*)(ptr + num)) + 3) & -4) + num;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
		num += 4;
		bool flag2 = flag;
		byte[] array2 = new byte[*(int*)(ptr + num)];
		num += 4;
		num = ((Native.CopyToArray<byte>(array2, (void*)(ptr + num)) + 3) & -4) + num;
		RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionCallbackHandler.RPC_OnEventRaisedReliable(runner, b2, array, flag2, array2, rpcInfo);
	}

	// Token: 0x06001993 RID: 6547 RVA: 0x0008F874 File Offset: 0x0008DA74
	[NetworkRpcStaticWeavedInvoker("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnEventRaisedUnreliable@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		byte b = ptr[num];
		num += (1 + 3) & -4;
		byte b2 = b;
		byte[] array = new byte[*(int*)(ptr + num)];
		num += 4;
		num = ((Native.CopyToArray<byte>(array, (void*)(ptr + num)) + 3) & -4) + num;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
		num += 4;
		bool flag2 = flag;
		byte[] array2 = new byte[*(int*)(ptr + num)];
		num += 4;
		num = ((Native.CopyToArray<byte>(array2, (void*)(ptr + num)) + 3) & -4) + num;
		RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(runner, b2, array, flag2, array2, rpcInfo);
	}

	// Token: 0x0400247B RID: 9339
	private NetworkSystemFusion parent;
}
