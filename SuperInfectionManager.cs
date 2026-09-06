using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Pool;

// Token: 0x02000183 RID: 387
[DefaultExecutionOrder(0)]
public class SuperInfectionManager : MonoBehaviour, IGameEntityZoneComponent, IFactoryItemProvider
{
	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000A56 RID: 2646 RVA: 0x0003770E File Offset: 0x0003590E
	public bool HasSIZonePlatform
	{
		get
		{
			return this.zoneSuperInfectionRef.TargetID != 0;
		}
	}

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000A57 RID: 2647 RVA: 0x0003771E File Offset: 0x0003591E
	public bool HasActiveTryOnDispenser
	{
		get
		{
			return this.tryOnDispenserCount > 0;
		}
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x00037729 File Offset: 0x00035929
	internal void RegisterTryOnDispenser()
	{
		this.tryOnDispenserCount++;
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x00037739 File Offset: 0x00035939
	internal void UnregisterTryOnDispenser()
	{
		this.tryOnDispenserCount = Mathf.Max(this.tryOnDispenserCount - 1, 0);
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0003774F File Offset: 0x0003594F
	private void Awake()
	{
		GameEntityManager gameEntityManager = this.gameEntityManager;
		gameEntityManager.OnEntityRemoved = (Action<GameEntity>)Delegate.Combine(gameEntityManager.OnEntityRemoved, new Action<GameEntity>(this.OnEntityRemoved));
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x00037778 File Offset: 0x00035978
	public void OnEnableZoneSuperInfection(SuperInfection zone)
	{
		this.zoneSuperInfection = zone;
		if (this.PendingZoneInit)
		{
			this.PendingZoneInit = false;
			this.OnZoneInit();
		}
		if (this.gameEntityManager.PendingTableData)
		{
			this.gameEntityManager.ResolveTableData();
		}
		if (!this.hasInitialized)
		{
			List<GameEntity> list = new List<GameEntity>();
			foreach (SIResourceRegion siresourceRegion in this.zoneSuperInfection.resourceRegions)
			{
				list.Add(siresourceRegion.resourcePrefab.GetComponent<GameEntity>());
			}
			this.gameEntityManager.AddToFactory(list);
		}
		this.hasInitialized = true;
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0003780C File Offset: 0x00035A0C
	private void OnEnable()
	{
		if (!SuperInfectionManager.siManagerByZone.TryAdd(this.gameEntityManager.zone, this))
		{
			Debug.LogError("[GT/SuperInfectionManager]  ERROR!!!  " + string.Format("Tried to add a duplicate Manager for zone `{0}`. Did you forget to change the ", this.gameEntityManager.zone) + "zone on the GameEntityManager on GameObject at path: " + base.transform.GetPathQ(), this);
			return;
		}
		GameMode.OnStartGameMode += this._OnStartGameMode;
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x0003787D File Offset: 0x00035A7D
	private void OnDisable()
	{
		SuperInfectionManager.siManagerByZone.Remove(this.gameEntityManager.zone);
		GameMode.OnStartGameMode -= this._OnStartGameMode;
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x000378A8 File Offset: 0x00035AA8
	private void _OnStartGameMode(GameModeType newGameModeType)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			return;
		}
		List<GameEntityId> list;
		using (CollectionPool<List<GameEntityId>, GameEntityId>.Get(out list))
		{
			ESuperGameModes esuperGameModes = (ESuperGameModes)(1 << (int)newGameModeType);
			foreach (GameEntity gameEntity in this.gameEntityManager.GetGameEntities())
			{
				SIGadget sigadget;
				SITechTreePage sitechTreePage;
				if (!(gameEntity == null) && gameEntity.TryGetComponent<SIGadget>(out sigadget) && this.techTreeSO.TryGetTreePage(sigadget.PageId, out sitechTreePage) && (sitechTreePage.excludedGameModes & esuperGameModes) != (ESuperGameModes)0)
				{
					list.Add(gameEntity.id);
				}
			}
			if (list.Count > 0)
			{
				this.gameEntityManager.RequestDestroyItems(list);
			}
		}
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x00037990 File Offset: 0x00035B90
	public static SuperInfectionManager GetSIManagerForZone(GTZone targetZone)
	{
		SuperInfectionManager superInfectionManager;
		if (SuperInfectionManager.siManagerByZone.TryGetValue(targetZone, out superInfectionManager))
		{
			return superInfectionManager;
		}
		return null;
	}

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000A60 RID: 2656 RVA: 0x00002076 File Offset: 0x00000276
	public bool IsSupercharged
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnZoneCreate()
	{
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x000379B0 File Offset: 0x00035BB0
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		if (!this.gameEntityManager.IsAuthority())
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].WriteDataPUN(stream, info);
		}
		for (int j = 0; j < this.zoneSuperInfection.siDeposits.Length; j++)
		{
			this.zoneSuperInfection.siDeposits[j].WriteDataPUN(stream, info);
		}
		this.zoneSuperInfection.questBoard.WriteDataPUN(stream, info);
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x00037A44 File Offset: 0x00035C44
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		if (!this.gameEntityManager.IsAuthorityPlayer(info.Sender))
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].ReadDataPUN(stream, info);
		}
		for (int j = 0; j < this.zoneSuperInfection.siDeposits.Length; j++)
		{
			this.zoneSuperInfection.siDeposits[j].ReadDataPUN(stream, info);
		}
		this.zoneSuperInfection.questBoard.ReadDataPUN(stream, info);
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x00037AE0 File Offset: 0x00035CE0
	void IGameEntityZoneComponent.SerializeZoneData(BinaryWriter writer)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].SerializeZoneData(writer);
		}
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x00037B28 File Offset: 0x00035D28
	void IGameEntityZoneComponent.DeserializeZoneData(BinaryReader reader)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].DeserializeZoneData(reader);
		}
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x00037B70 File Offset: 0x00035D70
	void IGameEntityZoneComponent.SerializeZonePlayerData(BinaryWriter writer, int actorNumber)
	{
		SIPlayer siplayer = SIPlayer.Get(actorNumber);
		siplayer.SerializeNetworkState(writer, siplayer.gamePlayer.rig.OwningNetPlayer);
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x00037B9C File Offset: 0x00035D9C
	void IGameEntityZoneComponent.DeserializeZonePlayerData(BinaryReader reader, int actorNumber)
	{
		SIPlayer siplayer = SIPlayer.Get(actorNumber);
		SIPlayer.DeserializeNetworkStateAndBurn(reader, siplayer, this);
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x00037BB8 File Offset: 0x00035DB8
	public bool IsZoneReady()
	{
		if (!this.HasSIZonePlatform)
		{
			return NetworkSystem.Instance.InRoom && VRRig.LocalRig.zoneEntity.currentZone == this.gameEntityManager.zone;
		}
		return NetworkSystem.Instance.InRoom && SuperInfectionManager.IsSuperGameMode() && this.zoneSuperInfection.IsNotNull() && VRRig.LocalRig.zoneEntity.currentZone == this.gameEntityManager.zone && SIProgression.Instance != null && SIProgression.Instance._treeReady;
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x00037C50 File Offset: 0x00035E50
	public bool ShouldClearZone()
	{
		if (!this.HasSIZonePlatform)
		{
			return false;
		}
		if (GameMode.ActiveGameMode != null)
		{
			GameModeType gameModeType = GameMode.ActiveGameMode.GameType();
			return gameModeType != GameModeType.SuperInfect && gameModeType != GameModeType.SuperCasual;
		}
		return false;
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x00037C90 File Offset: 0x00035E90
	public static bool IsSuperGameMode()
	{
		return GameMode.CurrentGameModeType == GameModeType.SuperInfect || GameMode.CurrentGameModeType == GameModeType.SuperCasual;
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00037CA8 File Offset: 0x00035EA8
	public void OnCreateGameEntity(GameEntity entity)
	{
		SIGadget component = entity.GetComponent<SIGadget>();
		bool flag = (entity.createData & long.MinValue) != 0L;
		if (component != null)
		{
			SIPlayer siplayer = SIPlayer.Get((int)(entity.createData & (long)((ulong)(-1))));
			if (siplayer != null)
			{
				int num = 0;
				for (int i = siplayer.activePlayerGadgets.Count - 1; i >= 0; i--)
				{
					GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siplayer.activePlayerGadgets[i]);
					if (gameEntityFromNetId == null)
					{
						siplayer.activePlayerGadgets.RemoveAt(i);
					}
					else
					{
						num++;
						if (num >= siplayer.TotalGadgetLimit)
						{
							this.gameEntityManager.DestroyItemLocal(gameEntityFromNetId.id);
							break;
						}
					}
				}
				if (!siplayer.activePlayerGadgets.Contains(entity.GetNetId()))
				{
					siplayer.activePlayerGadgets.Add(entity.GetNetId());
				}
			}
			SIUpgradeSet siupgradeSet = new SIUpgradeSet((int)((entity.createData & 9223372032559808512L) >> 32));
			siupgradeSet = component.FilterUpgradeNodes(siupgradeSet);
			component.ApplyUpgradeNodes(siupgradeSet);
			component.RefreshUpgradeVisuals(siupgradeSet);
			if (this.zoneSuperInfection != null)
			{
				this.zoneSuperInfection.AddGadget(component);
			}
			if (flag)
			{
				entity.shouldDestroyOnZoneExit = true;
				GameEntityDelayedDestroy gameEntityDelayedDestroy = entity.gameObject.AddComponent<GameEntityDelayedDestroy>();
				gameEntityDelayedDestroy.Configure(SIGadgetDispenser.g_tryOnOptions);
				gameEntityDelayedDestroy.ResetTimer();
			}
		}
		List<SuperInfectionSnapPoint> list;
		using (CollectionPool<List<SuperInfectionSnapPoint>, SuperInfectionSnapPoint>.Get(out list))
		{
			entity.GetComponentsInChildren<SuperInfectionSnapPoint>(true, list);
			foreach (SuperInfectionSnapPoint superInfectionSnapPoint in list)
			{
				this.RegisterSnapPoint(superInfectionSnapPoint);
			}
		}
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x00037E80 File Offset: 0x00036080
	public void OnZoneClear(ZoneClearReason reason)
	{
		SuperInfection superInfection = this.zoneSuperInfection;
		if (superInfection != null)
		{
			superInfection.OnZoneClear(reason);
		}
		SIPlayer localPlayer = SIPlayer.LocalPlayer;
		if (localPlayer != null)
		{
			localPlayer.Reset();
		}
		SIPlayer.ClearPlayerCache();
		this.allSnapPoints.Clear();
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x00037EB4 File Offset: 0x000360B4
	public void OnZoneInit()
	{
		if (this.zoneSuperInfection == null && this.HasSIZonePlatform)
		{
			this.PendingZoneInit = true;
			return;
		}
		if (!SuperInfectionManager.IsSuperGameMode())
		{
			return;
		}
		SuperInfectionManager.activeSuperInfectionManager = this;
		if (this.gameEntityManager.IsAuthority() && this.zoneSuperInfection != null)
		{
			this.TestSpawnGadget();
		}
		if (this.zoneSuperInfection != null)
		{
			this.zoneSuperInfection.OnZoneInit();
		}
		if (SIPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber) != null)
		{
			this.progression.Init();
			if (this.progression.ClientReady)
			{
				SIPlayer.SetAndBroadcastProgression();
			}
			else
			{
				this.progression.OnClientReady += this.<OnZoneInit>g__WhenReady|51_0;
			}
		}
		this.allSnapPoints.Clear();
		foreach (GameEntity gameEntity in this.gameEntityManager.GetGameEntities())
		{
			if (!(gameEntity == null))
			{
				List<SuperInfectionSnapPoint> list;
				using (CollectionPool<List<SuperInfectionSnapPoint>, SuperInfectionSnapPoint>.Get(out list))
				{
					gameEntity.GetComponentsInChildren<SuperInfectionSnapPoint>(true, list);
					foreach (SuperInfectionSnapPoint superInfectionSnapPoint in list)
					{
						this.RegisterSnapPoint(superInfectionSnapPoint);
					}
				}
			}
		}
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x00038038 File Offset: 0x00036238
	public void RegisterSnapPoint(SuperInfectionSnapPoint snapPoint)
	{
		List<SuperInfectionSnapPoint> list;
		if (!this.allSnapPoints.TryGetValue(snapPoint.jointType, out list))
		{
			list = (this.allSnapPoints[snapPoint.jointType] = new List<SuperInfectionSnapPoint>());
		}
		list.Add(snapPoint);
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x0003807C File Offset: 0x0003627C
	public void UnregisterSnapPoint(SuperInfectionSnapPoint snapPoint)
	{
		if (this.allSnapPoints.ContainsKey(snapPoint.jointType))
		{
			this.allSnapPoints[snapPoint.jointType].Remove(snapPoint);
			if (this.allSnapPoints[snapPoint.jointType].Count == 0)
			{
				this.allSnapPoints.Remove(snapPoint.jointType);
			}
		}
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x000380DE File Offset: 0x000362DE
	public IEnumerable<SuperInfectionSnapPoint> GetPoints(SnapJointType jointType)
	{
		foreach (KeyValuePair<SnapJointType, List<SuperInfectionSnapPoint>> keyValuePair in this.allSnapPoints)
		{
			if ((keyValuePair.Key & jointType) != SnapJointType.None)
			{
				foreach (SuperInfectionSnapPoint superInfectionSnapPoint in keyValuePair.Value)
				{
					yield return superInfectionSnapPoint;
				}
				List<SuperInfectionSnapPoint>.Enumerator enumerator2 = default(List<SuperInfectionSnapPoint>.Enumerator);
			}
		}
		Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>.Enumerator enumerator = default(Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x000380F8 File Offset: 0x000362F8
	public SuperInfectionSnapPoint FindNearestSnapPoint(SnapJointType jointType, Vector3 origin, float maxDist, bool includeOccupied = false)
	{
		SuperInfectionSnapPoint superInfectionSnapPoint = null;
		float num = maxDist * maxDist;
		foreach (SuperInfectionSnapPoint superInfectionSnapPoint2 in this.GetPoints(jointType))
		{
			if (!(superInfectionSnapPoint2 == null) && superInfectionSnapPoint2.isActiveAndEnabled && (includeOccupied || !superInfectionSnapPoint2.HasSnapped()))
			{
				float sqrMagnitude = (superInfectionSnapPoint2.transform.position - origin).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					superInfectionSnapPoint = superInfectionSnapPoint2;
					num = sqrMagnitude;
				}
			}
		}
		return superInfectionSnapPoint;
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x0003818C File Offset: 0x0003638C
	public void CallRPC(SuperInfectionManager.ClientToAuthorityRPC clientToAuthorityRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIClientToAuthorityRPC", this.gameEntityManager.GetAuthorityPlayer(), new object[]
			{
				(int)clientToAuthorityRPC,
				data
			});
		}
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x000381C8 File Offset: 0x000363C8
	public void CallRPC(SuperInfectionManager.ClientToClientRPC clientToClientRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIClientToClientRPC", RpcTarget.Others, new object[]
			{
				(int)clientToClientRPC,
				data
			});
		}
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x000381FA File Offset: 0x000363FA
	public void CallRPC(SuperInfectionManager.AuthorityToClientRPC authorityToClientRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIAuthorityToClientRPC", RpcTarget.Others, new object[]
			{
				(int)authorityToClientRPC,
				data
			});
		}
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0003822C File Offset: 0x0003642C
	public void CallRPC(SuperInfectionManager.AuthorityToClientRPC authorityToClientRPC, int actorNr, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIAuthorityToClientRPC", NetworkSystem.Instance.GetNetPlayerByID(actorNr).GetPlayerRef(), new object[]
			{
				(int)authorityToClientRPC,
				data
			});
		}
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x00038278 File Offset: 0x00036478
	[PunRPC]
	public void SIClientToAuthorityRPC(int clientToAuthorityRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.clientToAuthorityRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessClientToAuthorityRPC(clientToAuthorityRPCEnum, data, info);
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x000382D4 File Offset: 0x000364D4
	public void ProcessClientToAuthorityRPC(int clientToAuthorityRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (clientToAuthorityRPCEnum)
		{
		case 0:
		{
			if (this.zoneSuperInfection == null)
			{
				return;
			}
			if (data.Length != 4)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			int num2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num2))
			{
				return;
			}
			int num3;
			if (!GameEntityManager.ValidateDataType<int>(data[2], out num3))
			{
				return;
			}
			int num4;
			if (!GameEntityManager.ValidateDataType<int>(data[3], out num4))
			{
				return;
			}
			if (num4 < 0 || num4 >= this.zoneSuperInfection.siTerminals.Length)
			{
				return;
			}
			if (!Enum.IsDefined(typeof(SITouchscreenButton.SITouchscreenButtonType), (SITouchscreenButton.SITouchscreenButtonType)num))
			{
				return;
			}
			if (!Enum.IsDefined(typeof(SICombinedTerminal.TerminalSubFunction), (SICombinedTerminal.TerminalSubFunction)num3))
			{
				return;
			}
			this.zoneSuperInfection.siTerminals[num4].TouchscreenButtonPressed((SITouchscreenButton.SITouchscreenButtonType)num, num2, info.Sender.ActorNumber, (SICombinedTerminal.TerminalSubFunction)num3);
			return;
		}
		case 1:
		{
			if (this.zoneSuperInfection == null)
			{
				return;
			}
			if (data.Length != 1)
			{
				return;
			}
			int num5;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num5))
			{
				return;
			}
			if (num5 < 0 || num5 >= this.zoneSuperInfection.siTerminals.Length)
			{
				return;
			}
			SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
			if (siplayer == null)
			{
				return;
			}
			SICombinedTerminal sicombinedTerminal = this.zoneSuperInfection.siTerminals[num5];
			if (!siplayer.gamePlayer.rig.IsPositionInRange(sicombinedTerminal.transform.position, 3f))
			{
				return;
			}
			sicombinedTerminal.PlayerHandScanned(info.Sender.ActorNumber);
			return;
		}
		case 2:
		{
			if (this.zoneSuperInfection == null)
			{
				return;
			}
			if (data.Length != 2)
			{
				return;
			}
			int num6;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num6))
			{
				return;
			}
			int num7;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num7))
			{
				return;
			}
			if (num7 < 0 || num7 >= this.zoneSuperInfection.siDeposits.Length)
			{
				return;
			}
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(num6);
			if (gameEntityFromNetId == null)
			{
				return;
			}
			SIResourceDeposit siresourceDeposit = this.zoneSuperInfection.siDeposits[num7];
			if ((gameEntityFromNetId.transform.position - siresourceDeposit.transform.position).IsLongerThan(3f))
			{
				return;
			}
			SIResource component = gameEntityFromNetId.GetComponent<SIResource>();
			if (component != null)
			{
				siresourceDeposit.ResourceDeposited(component);
				return;
			}
			break;
		}
		case 3:
		{
			if (data.Length != 2)
			{
				return;
			}
			int num8;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num8))
			{
				return;
			}
			int num9;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num9))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(num8);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessClientToAuthorityRPC(info, num9, null);
				return;
			}
			break;
		}
		case 4:
		{
			if (data.Length != 3)
			{
				return;
			}
			int num10;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num10))
			{
				return;
			}
			int num11;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num11))
			{
				return;
			}
			object[] array;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out array))
			{
				return;
			}
			GameEntity gameEntityFromNetId3 = this.gameEntityManager.GetGameEntityFromNetId(num10);
			if (!gameEntityFromNetId3)
			{
				return;
			}
			SIGadget component3 = gameEntityFromNetId3.GetComponent<SIGadget>();
			if (component3)
			{
				component3.ProcessClientToAuthorityRPC(info, num11, array);
			}
			break;
		}
		case 5:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x000385CC File Offset: 0x000367CC
	[PunRPC]
	public void SIAuthorityToClientRPC(int authorityToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidClientRPC(info.Sender))
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.authorityToClientRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessAuthorityToClientRPC(authorityToClientRPCEnum, data, info);
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x00038628 File Offset: 0x00036828
	public void ProcessAuthorityToClientRPC(int authorityToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (authorityToClientRPCEnum)
		{
		case 3:
		{
			if (data.Length != 2)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			int num2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num2))
			{
				return;
			}
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(num);
			if (!gameEntityFromNetId)
			{
				return;
			}
			SIGadget component = gameEntityFromNetId.GetComponent<SIGadget>();
			if (component)
			{
				component.ProcessAuthorityToClientRPC(info, num2, null);
				return;
			}
			break;
		}
		case 4:
		{
			if (data.Length != 3)
			{
				return;
			}
			int num3;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num3))
			{
				return;
			}
			int num4;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num4))
			{
				return;
			}
			object[] array;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out array))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(num3);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessAuthorityToClientRPC(info, num4, array);
				return;
			}
			break;
		}
		case 5:
		{
			if (data.Length != 1)
			{
				return;
			}
			Vector3 vector;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[0], out vector))
			{
				return;
			}
			if (SIPlayer.LocalPlayer)
			{
				SIPlayer.LocalPlayer.TriggerIdolDepositedCelebration(vector);
				return;
			}
			break;
		}
		case 6:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0003873C File Offset: 0x0003693C
	[PunRPC]
	public void SIClientToClientRPC(int clientToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.clientToClientRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessClientToClientRPC(clientToClientRPCEnum, data, info);
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x00038784 File Offset: 0x00036984
	public void ProcessClientToClientRPC(int clientToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (clientToClientRPCEnum)
		{
		case 0:
		{
			SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
			if (siplayer == null)
			{
				return;
			}
			if (data.Length != 8)
			{
				return;
			}
			int[] array;
			if (!GameEntityManager.ValidateDataType<int[]>(data[0], out array))
			{
				return;
			}
			int[] array2;
			if (!GameEntityManager.ValidateDataType<int[]>(data[1], out array2))
			{
				return;
			}
			bool[][] array3;
			if (!GameEntityManager.ValidateDataType<bool[][]>(data[2], out array3))
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[3], out num))
			{
				return;
			}
			int num2;
			if (!GameEntityManager.ValidateDataType<int>(data[4], out num2))
			{
				return;
			}
			int num3;
			if (!GameEntityManager.ValidateDataType<int>(data[5], out num3))
			{
				return;
			}
			int[] array4;
			if (!GameEntityManager.ValidateDataType<int[]>(data[6], out array4))
			{
				return;
			}
			int[] array5;
			if (!GameEntityManager.ValidateDataType<int[]>(data[7], out array5))
			{
				return;
			}
			siplayer.UpdateProgression(array, array2, array3, num, num2, num3, array4, array5);
			if (this.zoneSuperInfection != null)
			{
				this.zoneSuperInfection.RefreshStations(info.Sender.ActorNumber);
				return;
			}
			break;
		}
		case 1:
		{
			if (data.Length != 5)
			{
				return;
			}
			if (SIPlayer.Get(info.Sender.ActorNumber) == null)
			{
				return;
			}
			int num4;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num4))
			{
				return;
			}
			Vector3 vector;
			if (GameEntityManager.ValidateDataType<Vector3>(data[1], out vector))
			{
				float num5 = 10000f;
				if ((in vector).IsValid(in num5))
				{
					Vector3 vector2;
					if (GameEntityManager.ValidateDataType<Vector3>(data[2], out vector2))
					{
						num5 = 10000f;
						if ((in vector2).IsValid(in num5))
						{
							Vector3 vector3;
							if (GameEntityManager.ValidateDataType<Vector3>(data[3], out vector3))
							{
								num5 = 10000f;
								if ((in vector3).IsValid(in num5))
								{
									Quaternion quaternion;
									if (!GameEntityManager.ValidateDataType<Quaternion>(data[4], out quaternion) || !(in quaternion).IsValid())
									{
										return;
									}
									GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(num4);
									if (gameEntityFromNetId == null)
									{
										return;
									}
									if (gameEntityFromNetId.heldByActorNumber != info.Sender.ActorNumber && gameEntityFromNetId.snappedByActorNumber != info.Sender.ActorNumber)
									{
										return;
									}
									SIGadgetDashYoyo component = gameEntityFromNetId.GetComponent<SIGadgetDashYoyo>();
									if (component == null)
									{
										return;
									}
									component.RemoteThrowYoYoTarget(vector, vector2, vector3, quaternion);
									return;
								}
							}
							return;
						}
					}
					return;
				}
			}
			return;
		}
		case 2:
		{
			if (data.Length != 2)
			{
				return;
			}
			int num6;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num6))
			{
				return;
			}
			int num7;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num7))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(num6);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessClientToClientRPC(info, num7, null);
				return;
			}
			break;
		}
		case 3:
		{
			if (data.Length != 3)
			{
				return;
			}
			int num8;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num8))
			{
				return;
			}
			int num9;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num9))
			{
				return;
			}
			object[] array6;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out array6))
			{
				return;
			}
			GameEntity gameEntityFromNetId3 = this.gameEntityManager.GetGameEntityFromNetId(num8);
			if (!gameEntityFromNetId3)
			{
				return;
			}
			SIGadget component3 = gameEntityFromNetId3.GetComponent<SIGadget>();
			if (component3)
			{
				component3.ProcessClientToClientRPC(info, num9, array6);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x00038A34 File Offset: 0x00036C34
	[ContextMenu("Spawn Debug Object")]
	private void TestSpawnGadget()
	{
		this.testSpawner.Spawn(this.gameEntityManager);
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x00038A47 File Offset: 0x00036C47
	public IEnumerable<GameEntity> GetFactoryItems()
	{
		return this.techTreeSO.SpawnableEntities;
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x00038A54 File Offset: 0x00036C54
	private void OnEntityRemoved(GameEntity entity)
	{
		SIGadget sigadget;
		entity.TryGetComponent<SIGadget>(out sigadget);
		if (this.zoneSuperInfection != null && sigadget != null)
		{
			this.zoneSuperInfection.RemoveGadget(sigadget);
		}
		if (sigadget == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get((int)(entity.createData & (long)((ulong)(-1))));
		if (siplayer != null && siplayer.activePlayerGadgets.Contains(entity.GetNetId()))
		{
			siplayer.activePlayerGadgets.Remove(entity.GetNetId());
		}
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x00038AD5 File Offset: 0x00036CD5
	public long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData)
	{
		if (entity.GetComponent<SIGadget>() == null)
		{
			return createData;
		}
		return (createData & -4294967296L) | ((long)SIPlayer.LocalPlayer.ActorNr & (long)((ulong)(-1)));
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x00038B04 File Offset: 0x00036D04
	public bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr)
	{
		if (this.techTreeSO.IsSpawnableEntityTypeId(entityTypeId) && !SuperInfectionManager.IsSuperGameMode())
		{
			return false;
		}
		SIPlayer.Get(actorNr);
		if ((createData & -9223372036854775808L) != 0L)
		{
			return false;
		}
		GameObject gameObject = this.gameEntityManager.FactoryPrefabById(entityTypeId);
		if (gameObject == null)
		{
			return false;
		}
		if (gameObject.GetComponent<SIGadget>() == null)
		{
			return false;
		}
		SIPlayer siplayer = SIPlayer.Get(actorNr);
		if (siplayer == null)
		{
			return false;
		}
		SIPlayer siplayer2 = SIPlayer.Get((int)(createData & (long)((ulong)(-1))));
		if (siplayer != siplayer2)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < siplayer.activePlayerGadgets.Count; i++)
		{
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siplayer.activePlayerGadgets[i]);
			if (((gameEntityFromNetId != null) ? gameEntityFromNetId.GetComponent<SIGadget>() : null) != null)
			{
				num++;
			}
		}
		if (num > siplayer.TotalGadgetLimit)
		{
			return false;
		}
		SIUpgradeType siupgradeType;
		if (this.techTreeSO.TryGetUpgradeTypeByEntityTypeId(entityTypeId, out siupgradeType))
		{
			bool flag = siplayer.CurrentProgression.IsUnlocked(siupgradeType);
			bool flag2 = SuperInfectionManager._ValidatePlayerHasGadgetUpgrades(createData, siplayer, siupgradeType);
			new SIUpgradeSet((int)((createData & 9223372032559808512L) >> 32));
			siplayer.GetUpgrades((SITechTreePageId)siupgradeType.GetPageId());
			if (!flag || !flag2)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x00002076 File Offset: 0x00000276
	public bool ValidateCreateMultipleItems(int zoneId, byte[] compressedStateData, int EntityCount)
	{
		return false;
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x00038C44 File Offset: 0x00036E44
	public bool ValidateCreateItem(int nedId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int createdByEntityNetId)
	{
		this.gameEntityManager.IsAuthority();
		if (this.techTreeSO.IsSpawnableEntityTypeId(entityTypeId) && !SuperInfectionManager.IsSuperGameMode())
		{
			return false;
		}
		SIUpgradeType siupgradeType;
		if (!this.techTreeSO.TryGetUpgradeTypeByEntityTypeId(entityTypeId, out siupgradeType))
		{
			return true;
		}
		if ((createData & -9223372036854775808L) != 0L)
		{
			return this.HasActiveTryOnDispenser;
		}
		SIPlayer siplayer = SIPlayer.Get((int)(createData & (long)((ulong)(-1))));
		if (siplayer == null)
		{
			return false;
		}
		bool flag = siplayer.CurrentProgression.IsUnlocked(siupgradeType);
		bool flag2 = SuperInfectionManager._ValidatePlayerHasGadgetUpgrades(createData, siplayer, siupgradeType);
		if (!flag || !flag2)
		{
			new SIUpgradeSet((int)((createData & 9223372032559808512L) >> 32));
			siplayer.GetUpgrades((SITechTreePageId)siupgradeType.GetPageId());
		}
		return flag && flag2;
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x00038CF8 File Offset: 0x00036EF8
	private static bool _ValidatePlayerHasGadgetUpgrades(long createData, SIPlayer siPlayer, SIUpgradeType upgradeType)
	{
		SIUpgradeSet siupgradeSet = new SIUpgradeSet((int)((createData & 9223372032559808512L) >> 32));
		SIUpgradeSet upgrades = siPlayer.GetUpgrades((SITechTreePageId)upgradeType.GetPageId());
		return (siupgradeSet.GetBits() & ~upgrades.GetBits()) == 0;
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool ValidateCreateItemBatchSize(int size)
	{
		return true;
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x00038D3C File Offset: 0x00036F3C
	public void ClearPlayerGadgets(SIPlayer siPlayer)
	{
		for (int i = siPlayer.activePlayerGadgets.Count - 1; i >= 0; i--)
		{
			if (i < siPlayer.activePlayerGadgets.Count && siPlayer.activePlayerGadgets[i] >= 0)
			{
				GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siPlayer.activePlayerGadgets[i]);
				if (!(gameEntityFromNetId == null) && !(gameEntityFromNetId.id == GameEntityId.Invalid))
				{
					this.gameEntityManager.RequestDestroyItem(gameEntityFromNetId.id);
				}
			}
		}
		siPlayer.activePlayerGadgets.Clear();
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x00038E04 File Offset: 0x00037004
	[CompilerGenerated]
	private void <OnZoneInit>g__WhenReady|51_0()
	{
		this.progression.OnClientReady -= this.<OnZoneInit>g__WhenReady|51_0;
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x04000C7F RID: 3199
	private const string preLog = "[GT/SuperInfectionManager]  ";

	// Token: 0x04000C80 RID: 3200
	private const string preErr = "[GT/SuperInfectionManager]  ERROR!!!  ";

	// Token: 0x04000C81 RID: 3201
	public GameEntityManager gameEntityManager;

	// Token: 0x04000C82 RID: 3202
	public TestSpawnGadget testSpawner;

	// Token: 0x04000C83 RID: 3203
	public PhotonView photonView;

	// Token: 0x04000C84 RID: 3204
	public XSceneRef zoneSuperInfectionRef;

	// Token: 0x04000C85 RID: 3205
	[NonSerialized]
	public SuperInfection zoneSuperInfection;

	// Token: 0x04000C86 RID: 3206
	[SerializeField]
	private SITechTreeSO techTreeSO;

	// Token: 0x04000C87 RID: 3207
	[SerializeField]
	private SIProgression progression;

	// Token: 0x04000C88 RID: 3208
	[DebugReadout]
	public static SuperInfectionManager activeSuperInfectionManager;

	// Token: 0x04000C89 RID: 3209
	public static Dictionary<GTZone, SuperInfectionManager> siManagerByZone = new Dictionary<GTZone, SuperInfectionManager>();

	// Token: 0x04000C8A RID: 3210
	private static List<VRRig> tempRigs = new List<VRRig>(20);

	// Token: 0x04000C8B RID: 3211
	private static List<VRRig> tempRigs2 = new List<VRRig>(20);

	// Token: 0x04000C8C RID: 3212
	private readonly Dictionary<SnapJointType, List<SuperInfectionSnapPoint>> allSnapPoints = new Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>();

	// Token: 0x04000C8D RID: 3213
	private const float rpcProximityCheckRange = 3f;

	// Token: 0x04000C8E RID: 3214
	private bool PendingZoneInit;

	// Token: 0x04000C8F RID: 3215
	private int tryOnDispenserCount;

	// Token: 0x04000C90 RID: 3216
	private bool hasInitialized;

	// Token: 0x04000C91 RID: 3217
	private const int roomFXTypeCount = 5;

	// Token: 0x02000184 RID: 388
	public enum ClientToAuthorityRPC
	{
		// Token: 0x04000C93 RID: 3219
		CombinedTerminalButtonPress,
		// Token: 0x04000C94 RID: 3220
		CombinedTerminalHandScan,
		// Token: 0x04000C95 RID: 3221
		ResourceDepositDeposited,
		// Token: 0x04000C96 RID: 3222
		CallEntityRPC,
		// Token: 0x04000C97 RID: 3223
		CallEntityRPCData,
		// Token: 0x04000C98 RID: 3224
		RequestStartRoomFX
	}

	// Token: 0x02000185 RID: 389
	public enum RoomFXType
	{
		// Token: 0x04000C9A RID: 3226
		Underwater,
		// Token: 0x04000C9B RID: 3227
		LunarMode,
		// Token: 0x04000C9C RID: 3228
		ConstLowG,
		// Token: 0x04000C9D RID: 3229
		Bouncy,
		// Token: 0x04000C9E RID: 3230
		Supercharge
	}

	// Token: 0x02000186 RID: 390
	public enum AuthorityToClientRPC
	{
		// Token: 0x04000CA0 RID: 3232
		TechPointGranted,
		// Token: 0x04000CA1 RID: 3233
		ResourceDepositTechPointGranted,
		// Token: 0x04000CA2 RID: 3234
		ResourceDepositTechPointRejected,
		// Token: 0x04000CA3 RID: 3235
		CallEntityRPC,
		// Token: 0x04000CA4 RID: 3236
		CallEntityRPCData,
		// Token: 0x04000CA5 RID: 3237
		TriggerMonkeIdolDepositCelebration,
		// Token: 0x04000CA6 RID: 3238
		StartRoomFX
	}

	// Token: 0x02000187 RID: 391
	public enum ClientToClientRPC
	{
		// Token: 0x04000CA8 RID: 3240
		BroadcastProgression,
		// Token: 0x04000CA9 RID: 3241
		LaunchDashYoyo,
		// Token: 0x04000CAA RID: 3242
		CallEntityRPC,
		// Token: 0x04000CAB RID: 3243
		CallEntityRPCData
	}
}
