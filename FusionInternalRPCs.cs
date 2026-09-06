using System;
using Fusion;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000434 RID: 1076
public class FusionInternalRPCs : SimulationBehaviour
{
	// Token: 0x06001996 RID: 6550 RVA: 0x0008FA62 File Offset: 0x0008DC62
	private void Awake()
	{
		FusionInternalRPCs.netSys = NetworkSystem.Instance as NetworkSystemFusion;
	}

	// Token: 0x06001997 RID: 6551 RVA: 0x0008FA74 File Offset: 0x0008DC74
	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe static void RPC_SendPlayerSyncProp(NetworkRunner runner, [RpcTarget] PlayerRef player, PlayerRef playerData, string propKey, string propValue)
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
				RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(player);
				if (rpcTargetStatus == RpcTargetStatus.Unreachable)
				{
					NetworkBehaviourUtils.NotifyRpcTargetUnreachable(player, "System.Void FusionInternalRPCs::RPC_SendPlayerSyncProp(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.PlayerRef,System.String,System.String)");
				}
				else
				{
					if (rpcTargetStatus == RpcTargetStatus.Self)
					{
						goto IL_0010;
					}
					int num = 8;
					num += 4;
					num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(propKey) + 3) & -4;
					num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(propValue) + 3) & -4;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void FusionInternalRPCs::RPC_SendPlayerSyncProp(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.PlayerRef,System.String,System.String)", num);
					}
					else
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionInternalRPCs::RPC_SendPlayerSyncProp(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.PlayerRef,System.String,System.String)"));
						int num2 = 8;
						*(PlayerRef*)(ptr2 + num2) = playerData;
						num2 += 4;
						num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(ptr2 + num2), propKey) + 3) & -4) + num2;
						num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(ptr2 + num2), propValue) + 3) & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(player);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
			}
			return;
		}
		IL_0010:
		Debug.Log("RPC Setting player prop: " + propKey + " - " + propValue);
	}

	// Token: 0x06001999 RID: 6553 RVA: 0x0008FC34 File Offset: 0x0008DE34
	[NetworkRpcStaticWeavedInvoker("System.Void FusionInternalRPCs::RPC_SendPlayerSyncProp(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.PlayerRef,System.String,System.String)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_SendPlayerSyncProp@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		PlayerRef target = message->Target;
		PlayerRef playerRef = *(PlayerRef*)(ptr + num);
		num += 4;
		PlayerRef playerRef2 = playerRef;
		string text;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(ptr + num), out text) + 3) & -4) + num;
		string text2;
		num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(ptr + num), out text2) + 3) & -4) + num;
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionInternalRPCs.RPC_SendPlayerSyncProp(runner, target, playerRef2, text, text2);
	}

	// Token: 0x04002480 RID: 9344
	private static NetworkSystemFusion netSys;
}
