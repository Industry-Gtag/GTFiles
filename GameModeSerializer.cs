using System;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000845 RID: 2117
[NetworkBehaviourWeaved(1)]
internal class GameModeSerializer : GorillaSerializerMasterOnly, IStateAuthorityChanged, IPublicFacingInterface
{
	// Token: 0x170004C4 RID: 1220
	// (get) Token: 0x06003657 RID: 13911 RVA: 0x0012C624 File Offset: 0x0012A824
	// (set) Token: 0x06003658 RID: 13912 RVA: 0x0012C64A File Offset: 0x0012A84A
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe int gameModeKeyInt
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameModeSerializer.gameModeKeyInt. Networked properties can only be accessed when Spawned() has been called.");
			}
			return this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameModeSerializer.gameModeKeyInt. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = value;
		}
	}

	// Token: 0x170004C5 RID: 1221
	// (get) Token: 0x06003659 RID: 13913 RVA: 0x0012C671 File Offset: 0x0012A871
	public GorillaGameManager GameModeInstance
	{
		get
		{
			return this.gameModeInstance;
		}
	}

	// Token: 0x0600365A RID: 13914 RVA: 0x0012C67C File Offset: 0x0012A87C
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
		if (player != null)
		{
			MonkeAgent.IncrementRPCCall(wrappedInfo, "OnSpawnSetupCheck");
		}
		GameModeSerializer activeNetworkHandler = global::GorillaGameModes.GameMode.ActiveNetworkHandler;
		if (player != null && player.InRoom)
		{
			if (!player.IsMasterClient)
			{
				GTDev.LogError<string>("SPAWN FAIL NOT MASTER :" + player.UserId + player.NickName, null);
				MonkeAgent.instance.SendReport("trying to inappropriately create game managers", player.UserId, player.NickName);
				return false;
			}
			if (!this.netView.IsRoomView)
			{
				GTDev.LogError<string>("SPAWN FAIL ROOM VIEW" + player.UserId + player.NickName, null);
				MonkeAgent.instance.SendReport("creating game manager as player object", player.UserId, player.NickName);
				return false;
			}
			if (activeNetworkHandler.IsNotNull() && activeNetworkHandler != this)
			{
				GTDev.LogError<string>("DUPLICATE CHECK" + player.UserId + player.NickName, null);
				MonkeAgent.instance.SendReport("trying to create multiple game managers", player.UserId, player.NickName);
				return false;
			}
		}
		else if ((activeNetworkHandler.IsNotNull() && activeNetworkHandler != this) || !this.netView.IsRoomView)
		{
			GTDev.LogError<string>("ACTIVE HANDLER CHECK FAIL" + ((player != null) ? player.UserId : null) + ((player != null) ? player.NickName : null), null);
			GTDev.LogError<string>("existing game manager! destroying newly created manager", null);
			return false;
		}
		object[] instantiationData = wrappedInfo.punInfo.photonView.InstantiationData;
		if (instantiationData != null && instantiationData.Length >= 1)
		{
			object obj = instantiationData[0];
			if (obj is int)
			{
				int num = (int)obj;
				this.gameModeKey = (GameModeType)num;
				this.gameModeInstance = global::GorillaGameModes.GameMode.GetGameModeInstance(this.gameModeKey);
				if (this.gameModeInstance.IsNull() || !this.gameModeInstance.ValidGameMode())
				{
					return false;
				}
				this.serializeTarget = this.gameModeInstance;
				base.transform.parent = VRRigCache.Instance.NetworkParent;
				return true;
			}
		}
		GTDev.LogError<string>("missing instantiation data", null);
		return false;
	}

	// Token: 0x0600365B RID: 13915 RVA: 0x0012C88A File Offset: 0x0012AA8A
	internal void Init(int gameModeType)
	{
		Debug.Log("<color=red>Init called</color>");
		this.gameModeKeyInt = gameModeType;
	}

	// Token: 0x0600365C RID: 13916 RVA: 0x0012C89D File Offset: 0x0012AA9D
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
		this.netView.GetView.AddCallbackTarget(this);
		global::GorillaGameModes.GameMode.SetupGameModeRemote(this);
	}

	// Token: 0x0600365D RID: 13917 RVA: 0x0012C8B6 File Offset: 0x0012AAB6
	protected override void OnBeforeDespawn()
	{
		global::GorillaGameModes.GameMode.RemoveNetworkLink(this);
	}

	// Token: 0x0600365E RID: 13918 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x0600365F RID: 13919 RVA: 0x0012C8BE File Offset: 0x0012AABE
	[PunRPC]
	internal void RPC_ReportTag(int taggedPlayer, PhotonMessageInfo info)
	{
		this.ReportTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003660 RID: 13920 RVA: 0x0012C8D7 File Offset: 0x0012AAD7
	[PunRPC]
	internal void RPC_ReportHit(PhotonMessageInfo info)
	{
		this.ReportHit(new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003661 RID: 13921 RVA: 0x0012C8E8 File Offset: 0x0012AAE8
	[Rpc(RpcSources.All, RpcTargets.All)]
	internal unsafe void RPC_ReportTag(int taggedPlayer, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameModeSerializer::RPC_ReportTag(System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					int num = 8;
					num += 4;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameModeSerializer::RPC_ReportTag(System.Int32,Fusion.RpcInfo)", num);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
							int num2 = 8;
							*(int*)(ptr2 + num2) = taggedPlayer;
							num2 += 4;
							ptr->Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0012;
						}
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		this.ReportTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003662 RID: 13922 RVA: 0x0012CA54 File Offset: 0x0012AC54
	[Rpc(RpcSources.All, RpcTargets.All)]
	internal unsafe void RPC_ReportHit(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameModeSerializer::RPC_ReportHit(Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					int num = 8;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameModeSerializer::RPC_ReportHit(Fusion.RpcInfo)", num);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2);
							int num2 = 8;
							ptr->Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0012;
						}
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		this.ReportHit(new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003663 RID: 13923 RVA: 0x0012CB94 File Offset: 0x0012AD94
	private void ReportTag(NetPlayer taggedPlayer, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "ReportTag");
		NetPlayer sender = info.Sender;
		this.gameModeInstance.ReportTag(taggedPlayer, sender);
	}

	// Token: 0x06003664 RID: 13924 RVA: 0x0012CBC0 File Offset: 0x0012ADC0
	private void ReportHit(PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "ReportContactWithLavaRPC");
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.customMaps);
		bool flag2 = false;
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			InfectionLavaController infectionLavaController = null;
			if (rigContainer.Rig.zoneEntity != null)
			{
				infectionLavaController = InfectionLavaController.GetControllerForZone(rigContainer.Rig.zoneEntity.currentZone);
			}
			flag2 = infectionLavaController != null && infectionLavaController.LavaCurrentlyActivated && (infectionLavaController.SurfaceCenter - rigContainer.Rig.syncPos).sqrMagnitude < 2500f && infectionLavaController.LavaPlane.GetDistanceToPoint(rigContainer.Rig.syncPos) < 5f;
		}
		if (flag || flag2)
		{
			this.GameModeInstance.HitPlayer(info.Sender);
		}
	}

	// Token: 0x06003665 RID: 13925 RVA: 0x0012CC98 File Offset: 0x0012AE98
	[PunRPC]
	internal void RPC_BroadcastRoundComplete(PhotonMessageInfo info)
	{
		this.BroadcastRoundComplete(info);
	}

	// Token: 0x06003666 RID: 13926 RVA: 0x0012CCA6 File Offset: 0x0012AEA6
	private void BroadcastRoundComplete(PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "BroadcastRoundComplete");
		if (info.Sender.IsMasterClient)
		{
			this.gameModeInstance.HandleRoundComplete();
		}
	}

	// Token: 0x06003667 RID: 13927 RVA: 0x0012CCCB File Offset: 0x0012AECB
	[PunRPC]
	internal void RPC_BroadcastTag(int taggedPlayer, int taggingPlayer, PhotonMessageInfo info)
	{
		this.BroadcastTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), NetworkSystem.Instance.GetPlayer(taggingPlayer), info);
	}

	// Token: 0x06003668 RID: 13928 RVA: 0x0012CCEC File Offset: 0x0012AEEC
	private void BroadcastTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "BroadcastTag");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (taggedPlayer == null || taggingPlayer == null)
		{
			return;
		}
		if (!this.broadcastTagCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.gameModeInstance.HandleTagBroadcast(taggedPlayer, taggingPlayer);
	}

	// Token: 0x06003669 RID: 13929 RVA: 0x0012CD39 File Offset: 0x0012AF39
	protected override void FusionDataRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
		Debug.Log(this.gameModeData.GetType().Name);
	}

	// Token: 0x0600366A RID: 13930 RVA: 0x0012CD50 File Offset: 0x0012AF50
	protected override void FusionDataRPC(string method, RpcTarget target, params object[] parameters)
	{
		base.FusionDataRPC(method, target, parameters);
	}

	// Token: 0x0600366B RID: 13931 RVA: 0x0012CD5B File Offset: 0x0012AF5B
	void IStateAuthorityChanged.StateAuthorityChanged()
	{
		GameModeSerializer.FusionGameModeOwnerChanged(NetworkSystem.Instance.GetPlayer(base.Object.StateAuthority));
	}

	// Token: 0x0600366D RID: 13933 RVA: 0x0012CD9B File Offset: 0x0012AF9B
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.gameModeKeyInt = this._gameModeKeyInt;
	}

	// Token: 0x0600366E RID: 13934 RVA: 0x0012CDB3 File Offset: 0x0012AFB3
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._gameModeKeyInt = this.gameModeKeyInt;
	}

	// Token: 0x0600366F RID: 13935 RVA: 0x0012CDC8 File Offset: 0x0012AFC8
	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportTag@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int num3 = num2;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((GameModeSerializer)behaviour).RPC_ReportTag(num3, rpcInfo);
	}

	// Token: 0x06003670 RID: 13936 RVA: 0x0012CE28 File Offset: 0x0012B028
	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((GameModeSerializer)behaviour).RPC_ReportHit(rpcInfo);
	}

	// Token: 0x0400471D RID: 18205
	[WeaverGenerated]
	[DefaultForProperty("gameModeKeyInt", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private int _gameModeKeyInt;

	// Token: 0x0400471E RID: 18206
	private GameModeType gameModeKey;

	// Token: 0x0400471F RID: 18207
	private GorillaGameManager gameModeInstance;

	// Token: 0x04004720 RID: 18208
	private FusionGameModeData gameModeData;

	// Token: 0x04004721 RID: 18209
	private Type currentGameDataType;

	// Token: 0x04004722 RID: 18210
	private CallLimiter broadcastTagCallLimit = new CallLimiter(12, 5f, 0.5f);

	// Token: 0x04004723 RID: 18211
	public static Action<NetPlayer> FusionGameModeOwnerChanged;
}
