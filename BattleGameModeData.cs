using System;
using Fusion;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020005C9 RID: 1481
[NetworkBehaviourWeaved(61)]
public class BattleGameModeData : FusionGameModeData
{
	// Token: 0x170003EA RID: 1002
	// (get) Token: 0x0600254B RID: 9547 RVA: 0x000C7A2F File Offset: 0x000C5C2F
	// (set) Token: 0x0600254C RID: 9548 RVA: 0x000C7A59 File Offset: 0x000C5C59
	[Networked]
	[NetworkedWeaved(0, 61)]
	private unsafe PaintbrawlData PaintbrawlData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BattleGameModeData.PaintbrawlData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(PaintbrawlData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BattleGameModeData.PaintbrawlData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(PaintbrawlData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x0600254D RID: 9549 RVA: 0x000C7A84 File Offset: 0x000C5C84
	// (set) Token: 0x0600254E RID: 9550 RVA: 0x000C7A91 File Offset: 0x000C5C91
	public override object Data
	{
		get
		{
			return this.PaintbrawlData;
		}
		set
		{
			this.PaintbrawlData = (PaintbrawlData)value;
		}
	}

	// Token: 0x0600254F RID: 9551 RVA: 0x000C7A9F File Offset: 0x000C5C9F
	public override void Spawned()
	{
		this.serializer = base.GetComponent<GameModeSerializer>();
		this.battleTarget = (GorillaPaintbrawlManager)this.serializer.GameModeInstance;
	}

	// Token: 0x06002550 RID: 9552 RVA: 0x000C7AC4 File Offset: 0x000C5CC4
	[Rpc]
	public unsafe void RPC_ReportSlinshotHit(int taggedPlayerID, Vector3 hitLocation, int projectileCount, RpcInfo rpcInfo = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void BattleGameModeData::RPC_ReportSlinshotHit(System.Int32,UnityEngine.Vector3,System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					int num = 8;
					num += 4;
					num += 12;
					num += 4;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void BattleGameModeData::RPC_ReportSlinshotHit(System.Int32,UnityEngine.Vector3,System.Int32,Fusion.RpcInfo)", num);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
							int num2 = 8;
							*(int*)(ptr2 + num2) = taggedPlayerID;
							num2 += 4;
							*(Vector3*)(ptr2 + num2) = hitLocation;
							num2 += 12;
							*(int*)(ptr2 + num2) = projectileCount;
							num2 += 4;
							ptr->Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							rpcInfo = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0012;
						}
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(rpcInfo);
		MonkeAgent.IncrementRPCCall(photonMessageInfoWrapped, "RPC_ReportSlinshotHit");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(taggedPlayerID);
		this.battleTarget.ReportSlingshotHit(player, hitLocation, projectileCount, photonMessageInfoWrapped);
	}

	// Token: 0x06002552 RID: 9554 RVA: 0x000C7CAC File Offset: 0x000C5EAC
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.PaintbrawlData = this._PaintbrawlData;
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x000C7CC4 File Offset: 0x000C5EC4
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._PaintbrawlData = this.PaintbrawlData;
	}

	// Token: 0x06002554 RID: 9556 RVA: 0x000C7CD8 File Offset: 0x000C5ED8
	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportSlinshotHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int num3 = num2;
		Vector3 vector = *(Vector3*)(ptr + num);
		num += 12;
		Vector3 vector2 = vector;
		int num4 = *(int*)(ptr + num);
		num += 4;
		int num5 = num4;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((BattleGameModeData)behaviour).RPC_ReportSlinshotHit(num3, vector2, num5, rpcInfo);
	}

	// Token: 0x040030E0 RID: 12512
	[WeaverGenerated]
	[DefaultForProperty("PaintbrawlData", 0, 61)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private PaintbrawlData _PaintbrawlData;

	// Token: 0x040030E1 RID: 12513
	private GorillaPaintbrawlManager battleTarget;

	// Token: 0x040030E2 RID: 12514
	private GameModeSerializer serializer;
}
