using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200132B RID: 4907
	public class EnvironmentProximityReactorManager : NetworkSceneObject
	{
		// Token: 0x17000C0B RID: 3083
		// (get) Token: 0x06007B29 RID: 31529 RVA: 0x00283B47 File Offset: 0x00281D47
		public static EnvironmentProximityReactorManager Instance
		{
			get
			{
				return EnvironmentProximityReactorManager.instance;
			}
		}

		// Token: 0x06007B2A RID: 31530 RVA: 0x00283B50 File Offset: 0x00281D50
		private void Awake()
		{
			if (EnvironmentProximityReactorManager.instance != null && EnvironmentProximityReactorManager.instance != this)
			{
				GTDev.LogWarning<string>("[EnvironmentProximityReactorManager] Duplicate instance of the Environment Reactor Manager, destroying.", null);
				global::UnityEngine.Object.Destroy(this);
				return;
			}
			EnvironmentProximityReactorManager.instance = this;
			foreach (EnvironmentProximityReactor environmentProximityReactor in EnvironmentProximityReactorManager.registry)
			{
				if (environmentProximityReactor != null)
				{
					this.RegisterInstance(environmentProximityReactor);
				}
			}
			EnvironmentProximityReactorManager.registry.Clear();
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoined);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeft);
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
			CosmeticsProximityReactorManager.OnCosmeticRegistered += this.OnCosmeticRegistered;
		}

		// Token: 0x06007B2B RID: 31531 RVA: 0x00283C4C File Offset: 0x00281E4C
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			if (EnvironmentProximityReactorManager.instance == this)
			{
				EnvironmentProximityReactorManager.instance = null;
			}
			if (NetworkSystem.Instance != null)
			{
				RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoined);
				RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeft);
				RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
			}
			CosmeticsProximityReactorManager.OnCosmeticRegistered -= this.OnCosmeticRegistered;
		}

		// Token: 0x06007B2C RID: 31532 RVA: 0x00283CE4 File Offset: 0x00281EE4
		private void OnPlayerLeft(NetPlayer player)
		{
			this.pendingEvents.Remove(player.ActorNumber);
			for (int i = 0; i < this.reactors.Count; i++)
			{
				EnvironmentProximityReactor environmentProximityReactor = this.reactors[i];
				if (environmentProximityReactor != null)
				{
					environmentProximityReactor.RemoveSharedActor(player.ActorNumber);
				}
			}
		}

		// Token: 0x06007B2D RID: 31533 RVA: 0x00283D38 File Offset: 0x00281F38
		private void OnLeftRoom()
		{
			this.pendingEvents.Clear();
			for (int i = 0; i < this.reactors.Count; i++)
			{
				EnvironmentProximityReactor environmentProximityReactor = this.reactors[i];
				if (environmentProximityReactor != null)
				{
					environmentProximityReactor.ClearRemoteActors();
				}
			}
		}

		// Token: 0x06007B2E RID: 31534 RVA: 0x00283D80 File Offset: 0x00281F80
		private void OnPlayerJoined(NetPlayer newPlayer)
		{
			if (newPlayer.IsLocal || !NetworkSystem.Instance.InRoom)
			{
				return;
			}
			for (int i = 0; i < this.reactors.Count; i++)
			{
				if (this.reactors[i] != null)
				{
					this.reactors[i].SyncStateTo(newPlayer, this);
				}
			}
		}

		// Token: 0x06007B2F RID: 31535 RVA: 0x00283DE0 File Offset: 0x00281FE0
		public void BroadcastProximityStateTo(NetPlayer target, int reactorId, int blockIndex, bool isBelow)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			this.photonView.RPC("ProximityStateRPC", ((PunNetPlayer)target).PlayerRef, new object[] { reactorId, blockIndex, isBelow });
		}

		// Token: 0x06007B30 RID: 31536 RVA: 0x00283E38 File Offset: 0x00282038
		private bool CheckPlayerRateLimit(NetPlayer sender)
		{
			RigContainer rigContainer;
			return VRRigCache.Instance.TryGetVrrig(sender, out rigContainer) && rigContainer.Rig.fxSettings.callSettings[24].CallLimitSettings.CheckCallTime(Time.unscaledTime);
		}

		// Token: 0x06007B31 RID: 31537 RVA: 0x00283E78 File Offset: 0x00282078
		private bool SenderHasValidCosmetic(int reactorId, int blockIndex, PhotonMessageInfoWrapped info)
		{
			if (CosmeticsProximityReactorManager.Instance == null)
			{
				return false;
			}
			EnvironmentProximityReactor environmentProximityReactor = null;
			for (int i = 0; i < this.reactors.Count; i++)
			{
				if (this.reactors[i] != null && this.reactors[i].reactorId == reactorId)
				{
					environmentProximityReactor = this.reactors[i];
					break;
				}
			}
			if (environmentProximityReactor == null || blockIndex >= environmentProximityReactor.blocks.Count)
			{
				return false;
			}
			EnvironmentProximityReactor.InteractionBlock interactionBlock = environmentProximityReactor.blocks[blockIndex];
			IReadOnlyList<CosmeticsProximityReactor> cosmetics = CosmeticsProximityReactorManager.Instance.Cosmetics;
			for (int j = 0; j < cosmetics.Count; j++)
			{
				CosmeticsProximityReactor cosmeticsProximityReactor = cosmetics[j];
				if (!(cosmeticsProximityReactor == null))
				{
					VRRig ownerRig = cosmeticsProximityReactor.GetOwnerRig();
					if (!(ownerRig == null) && ownerRig.Creator == info.Sender && interactionBlock.CanTriggerFrom(cosmeticsProximityReactor))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06007B32 RID: 31538 RVA: 0x00283F6C File Offset: 0x0028216C
		private bool SenderIsInRange(int reactorId, int blockIndex, PhotonMessageInfoWrapped info)
		{
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				return false;
			}
			EnvironmentProximityReactor environmentProximityReactor = null;
			for (int i = 0; i < this.reactors.Count; i++)
			{
				if (this.reactors[i] != null && this.reactors[i].reactorId == reactorId)
				{
					environmentProximityReactor = this.reactors[i];
					break;
				}
			}
			if (environmentProximityReactor == null || blockIndex >= environmentProximityReactor.blocks.Count)
			{
				return false;
			}
			float num = environmentProximityReactor.blocks[blockIndex].proximityThreshold + this.distanceBuffer;
			return rigContainer.Rig.IsPositionInRange(environmentProximityReactor.transform.position, num);
		}

		// Token: 0x06007B33 RID: 31539 RVA: 0x00284028 File Offset: 0x00282228
		private void OnCosmeticRegistered(CosmeticsProximityReactor cosmetic)
		{
			VRRig ownerRig = cosmetic.GetOwnerRig();
			if (ownerRig == null || ownerRig.Creator == null)
			{
				return;
			}
			int actorNumber = ownerRig.Creator.ActorNumber;
			List<EnvironmentProximityReactorManager.PendingProximityEvent> list;
			if (!this.pendingEvents.TryGetValue(actorNumber, out list))
			{
				return;
			}
			float unscaledTime = Time.unscaledTime;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				EnvironmentProximityReactorManager.PendingProximityEvent pendingProximityEvent = list[i];
				if (unscaledTime - pendingProximityEvent.receivedTime > 10f)
				{
					list.RemoveAt(i);
				}
				else if (this.SenderHasValidCosmetic(pendingProximityEvent.reactorId, pendingProximityEvent.blockIndex, pendingProximityEvent.info))
				{
					if (!this.SenderIsInRange(pendingProximityEvent.reactorId, pendingProximityEvent.blockIndex, pendingProximityEvent.info))
					{
						list.RemoveAt(i);
					}
					else
					{
						this.ApplyProximityEventToReactor(pendingProximityEvent.reactorId, pendingProximityEvent.blockIndex, pendingProximityEvent.isBelow, actorNumber);
						list.RemoveAt(i);
					}
				}
			}
			if (list.Count == 0)
			{
				this.pendingEvents.Remove(actorNumber);
			}
		}

		// Token: 0x06007B34 RID: 31540 RVA: 0x00284130 File Offset: 0x00282330
		private void Update()
		{
			if (this.pendingEvents.Count == 0)
			{
				return;
			}
			float unscaledTime = Time.unscaledTime;
			foreach (KeyValuePair<int, List<EnvironmentProximityReactorManager.PendingProximityEvent>> keyValuePair in this.pendingEvents)
			{
				List<EnvironmentProximityReactorManager.PendingProximityEvent> value = keyValuePair.Value;
				for (int i = value.Count - 1; i >= 0; i--)
				{
					if (unscaledTime - value[i].receivedTime > 10f)
					{
						value.RemoveAt(i);
					}
				}
			}
			foreach (KeyValuePair<int, List<EnvironmentProximityReactorManager.PendingProximityEvent>> keyValuePair2 in this.pendingEvents)
			{
				if (keyValuePair2.Value.Count == 0)
				{
					this.pendingEvents.Remove(keyValuePair2.Key);
					break;
				}
			}
		}

		// Token: 0x06007B35 RID: 31541 RVA: 0x0028422C File Offset: 0x0028242C
		private void TryCacheProximityEvent(int reactorId, int blockIndex, bool isBelow, PhotonMessageInfoWrapped info)
		{
			int actorNumber = info.Sender.ActorNumber;
			List<EnvironmentProximityReactorManager.PendingProximityEvent> list;
			if (!this.pendingEvents.TryGetValue(actorNumber, out list))
			{
				list = new List<EnvironmentProximityReactorManager.PendingProximityEvent>();
				this.pendingEvents[actorNumber] = list;
			}
			else if (list.Count >= this.m_maxCachedEvents || list.Exists((EnvironmentProximityReactorManager.PendingProximityEvent e) => e.reactorId == reactorId && e.blockIndex == blockIndex))
			{
				return;
			}
			list.Add(new EnvironmentProximityReactorManager.PendingProximityEvent
			{
				reactorId = reactorId,
				blockIndex = blockIndex,
				isBelow = isBelow,
				info = info,
				receivedTime = Time.unscaledTime
			});
		}

		// Token: 0x06007B36 RID: 31542 RVA: 0x002842E8 File Offset: 0x002824E8
		private void ApplyProximityEventToReactor(int reactorId, int blockIndex, bool isBelow, int senderActorNumber)
		{
			for (int i = 0; i < this.reactors.Count; i++)
			{
				EnvironmentProximityReactor environmentProximityReactor = this.reactors[i];
				if (!(environmentProximityReactor == null) && environmentProximityReactor.reactorId == reactorId)
				{
					environmentProximityReactor.ApplySharedProximity(blockIndex, isBelow, senderActorNumber);
					return;
				}
			}
		}

		// Token: 0x06007B37 RID: 31543 RVA: 0x00284335 File Offset: 0x00282535
		private void RegisterInstance(EnvironmentProximityReactor reactor)
		{
			if (reactor == null)
			{
				return;
			}
			if (this.idSet.Add(reactor.reactorId))
			{
				this.reactors.Add(reactor);
			}
		}

		// Token: 0x06007B38 RID: 31544 RVA: 0x00284360 File Offset: 0x00282560
		private void UnregisterInstance(EnvironmentProximityReactor reactor)
		{
			if (reactor == null)
			{
				return;
			}
			if (!this.idSet.Remove(reactor.reactorId))
			{
				return;
			}
			this.reactors.Remove(reactor);
		}

		// Token: 0x06007B39 RID: 31545 RVA: 0x0028438D File Offset: 0x0028258D
		public static void Register(EnvironmentProximityReactor reactor)
		{
			if (EnvironmentProximityReactorManager.instance != null)
			{
				EnvironmentProximityReactorManager.instance.RegisterInstance(reactor);
				return;
			}
			EnvironmentProximityReactorManager.registry.Add(reactor);
		}

		// Token: 0x06007B3A RID: 31546 RVA: 0x002843B4 File Offset: 0x002825B4
		public static void Unregister(EnvironmentProximityReactor reactor)
		{
			if (EnvironmentProximityReactorManager.instance != null)
			{
				EnvironmentProximityReactorManager.instance.UnregisterInstance(reactor);
				return;
			}
			EnvironmentProximityReactorManager.registry.Remove(reactor);
		}

		// Token: 0x06007B3B RID: 31547 RVA: 0x002843DC File Offset: 0x002825DC
		public void BroadcastProximityState(int reactorId, int blockIndex, bool isBelow)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			this.photonView.RPC("ProximityStateRPC", RpcTarget.Others, new object[] { reactorId, blockIndex, isBelow });
		}

		// Token: 0x06007B3C RID: 31548 RVA: 0x00284428 File Offset: 0x00282628
		[PunRPC]
		public void ProximityStateRPC(int reactorId, int blockIndex, bool isBelow, PhotonMessageInfo info)
		{
			this.ApplyProximityStateShared(reactorId, blockIndex, isBelow, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06007B3D RID: 31549 RVA: 0x0028443C File Offset: 0x0028263C
		[Rpc]
		public unsafe static void RPC_ProximityState(NetworkRunner runner, int reactorId, int blockIndex, bool isBelow, RpcInfo info = default(RpcInfo))
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
					num += 4;
					num += 4;
					if (SimulationMessage.CanAllocateUserPayload(num))
					{
						if (runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTag.Cosmetics.EnvironmentProximityReactorManager::RPC_ProximityState(Fusion.NetworkRunner,System.Int32,System.Int32,System.Boolean,Fusion.RpcInfo)"));
							int num2 = 8;
							*(int*)(ptr2 + num2) = reactorId;
							num2 += 4;
							*(int*)(ptr2 + num2) = blockIndex;
							num2 += 4;
							ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), isBelow);
							num2 += 4;
							ptr->Offset = num2 * 8;
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
						info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_0010;
					}
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.Cosmetics.EnvironmentProximityReactorManager::RPC_ProximityState(Fusion.NetworkRunner,System.Int32,System.Int32,System.Boolean,Fusion.RpcInfo)", num);
				}
				return;
			}
			IL_0010:
			if (EnvironmentProximityReactorManager.instance == null)
			{
				return;
			}
			EnvironmentProximityReactorManager.instance.ApplyProximityStateShared(reactorId, blockIndex, isBelow, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06007B3E RID: 31550 RVA: 0x002845B0 File Offset: 0x002827B0
		private void ApplyProximityStateShared(int reactorId, int blockIndex, bool isBelow, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "ApplyProximityStateShared");
			if (blockIndex < 0)
			{
				return;
			}
			if (!this.idSet.Contains(reactorId))
			{
				MonkeAgent.instance.SendReport("Sent invalid reactorId in ProximityStateRPC", info.Sender.UserId, info.Sender.NickName);
				return;
			}
			if (!this.CheckPlayerRateLimit(info.Sender))
			{
				return;
			}
			if (!isBelow)
			{
				List<EnvironmentProximityReactorManager.PendingProximityEvent> list;
				if (this.pendingEvents.TryGetValue(info.Sender.ActorNumber, out list))
				{
					list.RemoveAll((EnvironmentProximityReactorManager.PendingProximityEvent e) => e.reactorId == reactorId && e.blockIndex == blockIndex);
				}
				this.ApplyProximityEventToReactor(reactorId, blockIndex, false, info.Sender.ActorNumber);
				return;
			}
			if (!this.SenderHasValidCosmetic(reactorId, blockIndex, info))
			{
				this.TryCacheProximityEvent(reactorId, blockIndex, isBelow, info);
				return;
			}
			if (!this.SenderIsInRange(reactorId, blockIndex, info))
			{
				MonkeAgent.instance.SendReport("Sent ProximityStateRPC from out of range", info.Sender.UserId, info.Sender.NickName);
				return;
			}
			this.ApplyProximityEventToReactor(reactorId, blockIndex, isBelow, info.Sender.ActorNumber);
		}

		// Token: 0x06007B41 RID: 31553 RVA: 0x00284758 File Offset: 0x00282958
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTag.Cosmetics.EnvironmentProximityReactorManager::RPC_ProximityState(Fusion.NetworkRunner,System.Int32,System.Int32,System.Boolean,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_ProximityState@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int num3 = num2;
			int num4 = *(int*)(ptr + num);
			num += 4;
			int num5 = num4;
			bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
			num += 4;
			bool flag2 = flag;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			EnvironmentProximityReactorManager.RPC_ProximityState(runner, num3, num5, flag2, rpcInfo);
		}

		// Token: 0x04008D0D RID: 36109
		private static EnvironmentProximityReactorManager instance;

		// Token: 0x04008D0E RID: 36110
		[SerializeField]
		private List<EnvironmentProximityReactor> reactors = new List<EnvironmentProximityReactor>();

		// Token: 0x04008D0F RID: 36111
		private readonly HashSet<int> idSet = new HashSet<int>();

		// Token: 0x04008D10 RID: 36112
		private readonly Dictionary<int, List<EnvironmentProximityReactorManager.PendingProximityEvent>> pendingEvents = new Dictionary<int, List<EnvironmentProximityReactorManager.PendingProximityEvent>>();

		// Token: 0x04008D11 RID: 36113
		private float distanceBuffer = 3f;

		// Token: 0x04008D12 RID: 36114
		private const float cosmeticSyncTimeout = 10f;

		// Token: 0x04008D13 RID: 36115
		[SerializeField]
		private int m_maxCachedEvents = 10;

		// Token: 0x04008D14 RID: 36116
		private static readonly HashSet<EnvironmentProximityReactor> registry = new HashSet<EnvironmentProximityReactor>();

		// Token: 0x0200132C RID: 4908
		private struct PendingProximityEvent
		{
			// Token: 0x04008D15 RID: 36117
			public int reactorId;

			// Token: 0x04008D16 RID: 36118
			public int blockIndex;

			// Token: 0x04008D17 RID: 36119
			public bool isBelow;

			// Token: 0x04008D18 RID: 36120
			public PhotonMessageInfoWrapped info;

			// Token: 0x04008D19 RID: 36121
			public float receivedTime;
		}
	}
}
