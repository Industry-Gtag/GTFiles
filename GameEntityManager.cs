using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// Token: 0x020006D2 RID: 1746
[NetworkBehaviourWeaved(0)]
public class GameEntityManager : NetworkComponent, IRequestableOwnershipGuardCallbacks, ITickSystemTick
{
	// Token: 0x14000057 RID: 87
	// (add) Token: 0x06002B9A RID: 11162 RVA: 0x000E8AF8 File Offset: 0x000E6CF8
	// (remove) Token: 0x06002B9B RID: 11163 RVA: 0x000E8B30 File Offset: 0x000E6D30
	public event GameEntityManager.ZoneStartEvent onZoneStart;

	// Token: 0x14000058 RID: 88
	// (add) Token: 0x06002B9C RID: 11164 RVA: 0x000E8B68 File Offset: 0x000E6D68
	// (remove) Token: 0x06002B9D RID: 11165 RVA: 0x000E8BA0 File Offset: 0x000E6DA0
	public event GameEntityManager.ZoneClearEvent onZoneClear;

	// Token: 0x14000059 RID: 89
	// (add) Token: 0x06002B9E RID: 11166 RVA: 0x000E8BD8 File Offset: 0x000E6DD8
	// (remove) Token: 0x06002B9F RID: 11167 RVA: 0x000E8C10 File Offset: 0x000E6E10
	public event GameEntityManager.AuthorityChangeEvent OnAuthorityChanged;

	// Token: 0x1400005A RID: 90
	// (add) Token: 0x06002BA0 RID: 11168 RVA: 0x000E8C48 File Offset: 0x000E6E48
	// (remove) Token: 0x06002BA1 RID: 11169 RVA: 0x000E8C80 File Offset: 0x000E6E80
	public event GameEntityManager.ZoneActiveChangeEvent OnZoneActiveChanged;

	// Token: 0x1700045B RID: 1115
	// (get) Token: 0x06002BA2 RID: 11170 RVA: 0x000E8CB5 File Offset: 0x000E6EB5
	// (set) Token: 0x06002BA3 RID: 11171 RVA: 0x000E8CBC File Offset: 0x000E6EBC
	public static GameEntityManager activeManager { get; private set; }

	// Token: 0x1700045C RID: 1116
	// (get) Token: 0x06002BA4 RID: 11172 RVA: 0x000E8CC4 File Offset: 0x000E6EC4
	// (set) Token: 0x06002BA5 RID: 11173 RVA: 0x000E8CCC File Offset: 0x000E6ECC
	public bool TickRunning { get; set; }

	// Token: 0x1700045D RID: 1117
	// (get) Token: 0x06002BA6 RID: 11174 RVA: 0x000E8CD5 File Offset: 0x000E6ED5
	// (set) Token: 0x06002BA7 RID: 11175 RVA: 0x000E8CDD File Offset: 0x000E6EDD
	public bool PendingTableData { get; private set; }

	// Token: 0x06002BA8 RID: 11176 RVA: 0x000E8CE8 File Offset: 0x000E6EE8
	protected override void Awake()
	{
		base.Awake();
		this.entities = new List<GameEntity>(64);
		this.entitiesActiveCount = 0;
		this.gameEntityData = new List<GameEntityData>(64);
		this.netIdToIndex = new Dictionary<int, int>(16384);
		this.netIds = new NativeArray<int>(16384, Unity.Collections.Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.createdItemTypeCount = new Dictionary<int, int>();
		this.OnEntityRemoved = (Action<GameEntity>)Delegate.Combine(this.OnEntityRemoved, new Action<GameEntity>(CustomGameMode.OnGameEntityRemoved));
		this.zoneStateData = new GameEntityManager.ZoneStateData
		{
			zoneStateRequests = new List<GameEntityManager.ZoneStateRequest>(),
			zonePlayers = new List<Player>(),
			recievedStateBytes = new byte[15360],
			numRecievedStateBytes = 0
		};
		this.guard.AddCallbackTarget(this);
		this.netIdsForCreate = new List<int>();
		this.entityTypeIdsForCreate = new List<int>();
		this.packedPositionsForCreate = new List<long>();
		this.packedRotationsForCreate = new List<int>();
		this.createDataForCreate = new List<long>();
		this.createdByEntityNetIdForCreate = new List<int>();
		this.netIdsForDelete = new List<int>();
		this.netIdsForState = new List<int>();
		this.statesForState = new List<long>();
		this.zoneComponents = new List<IGameEntityZoneComponent>(8);
		if (this.ghostReactorManager != null)
		{
			this.zoneComponents.Add(this.ghostReactorManager);
		}
		if (this.customMapsManager != null)
		{
			this.zoneComponents.Add(this.customMapsManager);
		}
		if (this.superInfectionManager != null)
		{
			this.zoneComponents.Add(this.superInfectionManager);
		}
		this.BuildFactory();
		GameEntityManager.allManagers.Add(this);
		GameEntityManager.managersByZone[(int)this.zone] = this;
		if (base.transform.parent != null)
		{
			base.transform.SetParent(null, true);
		}
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06002BA9 RID: 11177 RVA: 0x000E8EC8 File Offset: 0x000E70C8
	internal void RegisterScenePlacedEntities()
	{
		if (this.scenePlacedEntitiesRegistered)
		{
			return;
		}
		this.scenePlacedEntitiesRegistered = true;
		string zoneSceneName = this.GetZoneSceneName();
		List<GameEntity> list;
		GameEntityManager.s_scenePlacedEntities.TryGetValue(zoneSceneName, out list);
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			this.RegisterSingleScenePlacedEntity(list[i]);
		}
	}

	// Token: 0x06002BAA RID: 11178 RVA: 0x000E8F1C File Offset: 0x000E711C
	private void RegisterSingleScenePlacedEntity(GameEntity entity)
	{
		if (entity == null)
		{
			return;
		}
		XSceneRefTarget xsceneRefTarget;
		int num;
		if (entity.TryGetComponent<XSceneRefTarget>(out xsceneRefTarget) && xsceneRefTarget.UniqueID > 0)
		{
			num = GameEntityManager.NetIdFromXSceneRefId(xsceneRefTarget.UniqueID);
		}
		else
		{
			num = GameEntityManager.ComputeNetIdFromHierarchyForCustomMaps(entity.transform);
		}
		int num2;
		if (this.netIdToIndex.TryGetValue(num, out num2))
		{
			GameEntity gameEntity = ((num2 >= 0 && num2 < this.entities.Count) ? this.entities[num2] : null);
			if (gameEntity == entity)
			{
				this.EnsureScenePlacedRecord(entity);
				return;
			}
			if (!(gameEntity == null))
			{
				Debug.LogError("[GT/GameEntityManager]  ERROR!!!  RegisterSingleScenePlacedEntity" + string.Format(": NetId collision for scene-placed entity '{0}' (netId={1})", entity.name, num) + string.Format(" with live entity '{0}' at index {1}. Skipping.", gameEntity.name, num2));
				return;
			}
			this.netIdToIndex.Remove(num);
		}
		if (!entity.scenePlacedInitialized)
		{
			entity.scenePlacedHomePosition = entity.transform.position;
			entity.scenePlacedHomeRotation = entity.transform.rotation;
			entity.scenePlacedHomeScale = entity.transform.lossyScale.x;
			if (!entity.gameObject.activeSelf)
			{
				entity.gameObject.SetActive(true);
			}
			entity.IsScenePlaced = true;
			entity.Create(this, num, -2147483647);
			entity.Init(0L, -1);
			this.AddGameEntity(num, entity);
			entity.scenePlacedInitialized = true;
		}
		else
		{
			GameEntityManager manager = entity.manager;
			if (manager != null && manager != this)
			{
				manager.RemoveGameEntity(entity);
				if (entity.builtInEntities != null)
				{
					for (int i = 0; i < entity.builtInEntities.Count; i++)
					{
						manager.RemoveGameEntity(entity.builtInEntities[i]);
					}
				}
			}
			entity.manager = this;
			this.AddGameEntity(num, entity);
			if (entity.builtInEntities != null)
			{
				bool flag = num < -1 && num != int.MinValue;
				for (int j = 0; j < entity.builtInEntities.Count; j++)
				{
					int num3 = (flag ? (num - 1 - j) : (num + 1 + j));
					entity.builtInEntities[j].manager = this;
					this.AddGameEntity(num3, entity.builtInEntities[j]);
				}
			}
			if (!entity.gameObject.activeSelf)
			{
				entity.gameObject.SetActive(true);
			}
		}
		GameEntityManager.s_scenePlacedHomeScenes[num] = entity.gameObject.scene.name;
		this.EnsureScenePlacedRecord(entity);
	}

	// Token: 0x06002BAB RID: 11179 RVA: 0x000E91A4 File Offset: 0x000E73A4
	private void EnsureScenePlacedRecord(GameEntity entity)
	{
		for (int i = 0; i < this.scenePlacedEntities.Count; i++)
		{
			if (this.scenePlacedEntities[i].entity == entity)
			{
				return;
			}
		}
		this.scenePlacedEntities.Add(new GameEntityManager.ScenePlacedRecord
		{
			entity = entity,
			position = entity.scenePlacedHomePosition,
			rotation = entity.scenePlacedHomeRotation,
			uniformScale = entity.scenePlacedHomeScale
		});
	}

	// Token: 0x06002BAC RID: 11180 RVA: 0x000E9224 File Offset: 0x000E7424
	private static void ResetScenePlacedTransform(GameEntity entity, in GameEntityManager.ScenePlacedRecord record)
	{
		bool flag = entity.transform.parent != null;
		bool flag2 = entity.IsHeld() || entity.snappedByActorNumber != -1;
		if (flag || flag2)
		{
			if (entity.manager != null)
			{
				entity.manager.ReleaseScenePlacedHold(entity);
			}
			else
			{
				GameEntityManager.DetachScenePlacedFromRig(entity);
			}
		}
		entity.transform.SetPositionAndRotation(record.position, record.rotation);
		entity.transform.localScale = Vector3.one * record.uniformScale;
		Rigidbody component = entity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.linearVelocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
	}

	// Token: 0x06002BAD RID: 11181 RVA: 0x000E92D8 File Offset: 0x000E74D8
	internal void ReleaseScenePlacedHold(GameEntity entity)
	{
		if (entity == null || !entity.IsScenePlaced)
		{
			return;
		}
		int heldByActorNumber = entity.heldByActorNumber;
		int snappedByActorNumber = entity.snappedByActorNumber;
		GamePlayer gamePlayer;
		if (heldByActorNumber != -1 && GamePlayer.TryGetGamePlayer(heldByActorNumber, out gamePlayer))
		{
			gamePlayer.ClearGrabbedIfHeld(entity.id, this);
			if (gamePlayer.IsLocal())
			{
				GamePlayerLocal instance = GamePlayerLocal.instance;
				if (instance != null)
				{
					instance.ClearGrabbedIfHeld(entity.id, this);
				}
			}
		}
		GamePlayer gamePlayer2;
		if (snappedByActorNumber != -1 && GamePlayer.TryGetGamePlayer(snappedByActorNumber, out gamePlayer2))
		{
			gamePlayer2.ClearSnappedIfSnapped(entity.id, this);
		}
		GameEntityManager.DetachScenePlacedFromRig(entity);
		GameEntityManager.MoveScenePlacedToHomeScene(entity);
		bool flag = entity.heldByActorNumber != -1 || entity.snappedByActorNumber != -1 || entity.attachedToEntityId != GameEntityId.Invalid;
		entity.heldByActorNumber = -1;
		entity.heldByHandIndex = -1;
		entity.snappedByActorNumber = -1;
		entity.snappedJoint = SnapJointType.None;
		entity.attachedToEntityId = GameEntityId.Invalid;
		if (flag)
		{
			Action onReleased = entity.OnReleased;
			if (onReleased == null)
			{
				return;
			}
			onReleased();
		}
	}

	// Token: 0x06002BAE RID: 11182 RVA: 0x000E93C8 File Offset: 0x000E75C8
	private static void DetachScenePlacedFromRig(GameEntity entity)
	{
		Transform parent = entity.transform.parent;
		if (parent == null)
		{
			return;
		}
		if (parent.GetComponentInParent<VRRig>() != null)
		{
			entity.transform.SetParent(null, true);
		}
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x000E9408 File Offset: 0x000E7608
	private static void MoveScenePlacedToHomeScene(GameEntity entity)
	{
		if (entity == null || !entity.IsScenePlaced || entity.manager == null)
		{
			return;
		}
		string text;
		if (!GameEntityManager.s_scenePlacedHomeScenes.TryGetValue(entity.GetNetId(), out text))
		{
			return;
		}
		Scene sceneByName = SceneManager.GetSceneByName(text);
		if (!sceneByName.IsValid() || !sceneByName.isLoaded)
		{
			return;
		}
		if (entity.gameObject.scene == sceneByName)
		{
			return;
		}
		SceneManager.MoveGameObjectToScene(entity.gameObject, sceneByName);
	}

	// Token: 0x06002BB0 RID: 11184 RVA: 0x000E9484 File Offset: 0x000E7684
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
		VRRigCache.OnRigDeactivated += this.OnRigDeactivated;
		VRRigCache.OnActiveRigsChanged += this.RefreshRigList;
		RoomSystem.JoinedRoomEvent += new Action(this.OnNetworkJoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnNetworkLeftRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnNetworkPlayerLeft);
		this.RefreshRigList();
	}

	// Token: 0x06002BB1 RID: 11185 RVA: 0x000E951C File Offset: 0x000E771C
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
		VRRigCache.OnRigDeactivated -= this.OnRigDeactivated;
		VRRigCache.OnActiveRigsChanged -= this.RefreshRigList;
		RoomSystem.JoinedRoomEvent -= new Action(this.OnNetworkJoinedRoom);
		RoomSystem.LeftRoomEvent -= new Action(this.OnNetworkLeftRoom);
		RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnNetworkPlayerLeft);
	}

	// Token: 0x06002BB2 RID: 11186 RVA: 0x000E95B0 File Offset: 0x000E77B0
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.netIds.Dispose();
		GameEntityManager.allManagers.Remove(this);
		GameEntityManager gameEntityManager;
		if (GameEntityManager.managersByZone.TryGetValue((int)this.zone, out gameEntityManager) && gameEntityManager == this)
		{
			GameEntityManager.managersByZone.Remove((int)this.zone);
		}
	}

	// Token: 0x06002BB3 RID: 11187 RVA: 0x000E9608 File Offset: 0x000E7808
	public static GameEntityManager GetManagerForZone(GTZone zone)
	{
		for (int i = 0; i < GameEntityManager.allManagers.Count; i++)
		{
			if (GameEntityManager.allManagers[i].zone == zone)
			{
				return GameEntityManager.allManagers[i];
			}
		}
		return null;
	}

	// Token: 0x06002BB4 RID: 11188 RVA: 0x000E964C File Offset: 0x000E784C
	public void Tick()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.UpdateZoneState();
		if (!this.IsZoneActive())
		{
			if (this.netIdsForCreate.Count > 0 || this.netIdsForDelete.Count > 0 || this.netIdsForState.Count > 0)
			{
				this.ClearPendingRPCBatches();
			}
			if (this.PendingTableData)
			{
				if (Time.frameCount - this.pendingTableDataSetFrame > 90)
				{
					this.ResolveTableData();
					return;
				}
				int num = Time.frameCount % 300;
			}
			return;
		}
		float time = Time.time;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity = this.entities[i];
			if (gameEntity != null && gameEntity.LastTickTime + gameEntity.MinTimeBetweenTicks < time && gameEntity.isActiveAndEnabled)
			{
				Action onTick = gameEntity.OnTick;
				if (onTick != null)
				{
					onTick();
				}
				gameEntity.LastTickTime = time;
			}
		}
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.scenePlacedEntities.Count > 0)
		{
			this.scenePlacedBoundsCheckTimer -= Time.deltaTime;
			if (this.scenePlacedBoundsCheckTimer <= 0f)
			{
				this.scenePlacedBoundsCheckTimer = 1f;
				for (int j = 0; j < this.scenePlacedEntities.Count; j++)
				{
					GameEntityManager.ScenePlacedRecord scenePlacedRecord = this.scenePlacedEntities[j];
					if (!(scenePlacedRecord.entity == null))
					{
						Vector3 position = scenePlacedRecord.entity.transform.position;
						if ((position - scenePlacedRecord.position).sqrMagnitude >= 0.0625f && !this.IsPositionInManagerBounds(position))
						{
							GameEntityManager.ResetScenePlacedTransform(scenePlacedRecord.entity, in scenePlacedRecord);
						}
					}
				}
			}
		}
		if (this.netIdsForCreate.Count > 0 && Time.time > this.lastCreateSent + this.createCooldown)
		{
			this.lastCreateSent = Time.time;
			this.photonView.RPC("CreateItemRPC", RpcTarget.Others, new object[]
			{
				this.netIdsForCreate.ToArray(),
				this.entityTypeIdsForCreate.ToArray(),
				this.packedPositionsForCreate.ToArray(),
				this.packedRotationsForCreate.ToArray(),
				this.createDataForCreate.ToArray(),
				this.createdByEntityNetIdForCreate.ToArray()
			});
			this.netIdsForCreate.Clear();
			this.entityTypeIdsForCreate.Clear();
			this.packedPositionsForCreate.Clear();
			this.packedRotationsForCreate.Clear();
			this.createDataForCreate.Clear();
			this.createdByEntityNetIdForCreate.Clear();
		}
		if (this.netIdsForDelete.Count > 0 && Time.time > this.lastDestroySent + this.destroyCooldown)
		{
			this.lastDestroySent = Time.time;
			this.photonView.RPC("DestroyItemRPC", RpcTarget.Others, new object[] { this.netIdsForDelete.ToArray() });
			this.netIdsForDelete.Clear();
		}
		if (this.netIdsForState.Count > 0 && Time.time > this.lastStateSent + this.stateCooldown)
		{
			this.lastStateSent = Time.time;
			this.photonView.RPC("ApplyStateRPC", RpcTarget.All, new object[]
			{
				this.netIdsForState.ToArray(),
				this.statesForState.ToArray()
			});
			this.netIdsForState.Clear();
			this.statesForState.Clear();
		}
	}

	// Token: 0x06002BB5 RID: 11189 RVA: 0x000E99A3 File Offset: 0x000E7BA3
	public GameEntityId AddGameEntity(GameEntity gameEntity)
	{
		return this.AddGameEntity(this.CreateNetId(1 + gameEntity.builtInEntities.Count), gameEntity);
	}

	// Token: 0x06002BB6 RID: 11190 RVA: 0x000E99C0 File Offset: 0x000E7BC0
	public GameEntityId AddGameEntity(int netId, GameEntity gameEntity)
	{
		if (netId == -1)
		{
			Debug.LogError("[GT/GameEntityManager]  ERROR!!!  AddGameEntity: Aborting. Invalid netId for GameEntity '" + ((gameEntity != null) ? gameEntity.name : null) + "'.", gameEntity);
			return GameEntityId.Invalid;
		}
		int num;
		if (this.netIdToIndex.TryGetValue(netId, out num) && num != -1)
		{
			GameEntity gameEntity2 = this.GetGameEntity(num);
			if (gameEntity2 != null)
			{
				if (gameEntity2 == gameEntity)
				{
					return gameEntity.id;
				}
				Debug.LogError(string.Concat(new string[]
				{
					"[GT/GameEntityManager]  ERROR!!!  AddGameEntity",
					string.Format(": NetId {0} collision: ", netId),
					"'",
					gameEntity2.name,
					"' replaced by '",
					gameEntity.name,
					"'. Destroying old entity to prevent zombie."
				}));
				this.DestroyItemLocal(gameEntity2.id);
			}
		}
		int num2 = this.FindNewEntityIndex();
		this.entities[num2] = gameEntity;
		this.entitiesActiveCount++;
		GameEntityData gameEntityData = default(GameEntityData);
		this.gameEntityData.Add(gameEntityData);
		gameEntity.id = new GameEntityId
		{
			index = num2
		};
		this.netIdToIndex[netId] = num2;
		this.netIds[num2] = netId;
		Action<GameEntity> onEntityAdded = this.OnEntityAdded;
		if (onEntityAdded != null)
		{
			onEntityAdded(gameEntity);
		}
		return gameEntity.id;
	}

	// Token: 0x06002BB7 RID: 11191 RVA: 0x000E9B14 File Offset: 0x000E7D14
	private int FindNewEntityIndex()
	{
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (this.entities[i] == null)
			{
				return i;
			}
		}
		this.entities.Add(null);
		return this.entities.Count - 1;
	}

	// Token: 0x06002BB8 RID: 11192 RVA: 0x000E9B68 File Offset: 0x000E7D68
	public void RemoveGameEntity(GameEntity entity)
	{
		this.netIdToIndex.Remove(entity.GetNetId());
		int index = entity.id.index;
		if (index < 0 || index >= this.entities.Count)
		{
			return;
		}
		if (this.entities[index] == entity)
		{
			this.entities[index] = null;
			this.entitiesActiveCount--;
		}
		else
		{
			for (int i = 0; i < this.entities.Count; i++)
			{
				if (this.entities[i] == entity)
				{
					this.entities[i] = null;
					this.entitiesActiveCount--;
					break;
				}
			}
		}
		Action<GameEntity> onEntityRemoved = this.OnEntityRemoved;
		if (onEntityRemoved == null)
		{
			return;
		}
		onEntityRemoved(entity);
	}

	// Token: 0x06002BB9 RID: 11193 RVA: 0x000E9C2F File Offset: 0x000E7E2F
	public List<GameEntity> GetGameEntities()
	{
		return this.entities;
	}

	// Token: 0x06002BBA RID: 11194 RVA: 0x000E9C38 File Offset: 0x000E7E38
	public bool IsValidNetId(int netId)
	{
		int num;
		return this.netIdToIndex.TryGetValue(netId, out num) && num >= 0 && num < this.entities.Count;
	}

	// Token: 0x06002BBB RID: 11195 RVA: 0x000E9C6C File Offset: 0x000E7E6C
	public int FindOpenIndex()
	{
		for (int i = 0; i < this.netIds.Length; i++)
		{
			if (this.netIds[i] != -1)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002BBC RID: 11196 RVA: 0x000E9CA4 File Offset: 0x000E7EA4
	public GameEntityId GetEntityIdFromNetId(int netId)
	{
		int num;
		if (this.netIdToIndex.TryGetValue(netId, out num))
		{
			return new GameEntityId
			{
				index = num
			};
		}
		return GameEntityId.Invalid;
	}

	// Token: 0x06002BBD RID: 11197 RVA: 0x000E9CD8 File Offset: 0x000E7ED8
	public int GetNetIdFromEntityId(GameEntityId id)
	{
		if (id.index < 0 || id.index >= this.netIds.Length)
		{
			return -1;
		}
		return this.netIds[id.index];
	}

	// Token: 0x06002BBE RID: 11198 RVA: 0x000E9D0C File Offset: 0x000E7F0C
	private void ClearPendingRPCBatches()
	{
		this.netIdsForCreate.Clear();
		this.entityTypeIdsForCreate.Clear();
		this.packedPositionsForCreate.Clear();
		this.packedRotationsForCreate.Clear();
		this.createDataForCreate.Clear();
		this.createdByEntityNetIdForCreate.Clear();
		this.netIdsForDelete.Clear();
		this.netIdsForState.Clear();
		this.statesForState.Clear();
	}

	// Token: 0x06002BBF RID: 11199 RVA: 0x000E9D7C File Offset: 0x000E7F7C
	public virtual bool IsAuthority()
	{
		return !NetworkSystem.Instance.InRoom || this.guard.isTrulyMine;
	}

	// Token: 0x06002BC0 RID: 11200 RVA: 0x000E9D97 File Offset: 0x000E7F97
	public bool IsAuthorityPlayer(NetPlayer player)
	{
		return player != null && this.IsAuthorityPlayer(player.GetPlayerRef());
	}

	// Token: 0x06002BC1 RID: 11201 RVA: 0x000E9DAC File Offset: 0x000E7FAC
	public bool IsAuthorityPlayer(Player player)
	{
		if (player != null && this.guard.actualOwner != null)
		{
			int actorNumber = player.ActorNumber;
			Player playerRef = this.guard.actualOwner.GetPlayerRef();
			int? num = ((playerRef != null) ? new int?(playerRef.ActorNumber) : null);
			return (actorNumber == num.GetValueOrDefault()) & (num != null);
		}
		return false;
	}

	// Token: 0x06002BC2 RID: 11202 RVA: 0x000E9E0C File Offset: 0x000E800C
	public bool IsZoneAuthority()
	{
		return this.IsAuthority();
	}

	// Token: 0x06002BC3 RID: 11203 RVA: 0x000E9E14 File Offset: 0x000E8014
	public bool HasAuthority()
	{
		return this.GetAuthorityPlayer() != null;
	}

	// Token: 0x06002BC4 RID: 11204 RVA: 0x000E9E1F File Offset: 0x000E801F
	public Player GetAuthorityPlayer()
	{
		if (this.guard.actualOwner != null)
		{
			return this.guard.actualOwner.GetPlayerRef();
		}
		return null;
	}

	// Token: 0x06002BC5 RID: 11205 RVA: 0x000E9E40 File Offset: 0x000E8040
	public virtual bool IsZoneActive()
	{
		if (GorillaComputer.instance != null && GorillaComputer.instance.IsPlayerInVirtualStump() && GameEntityManager.IsSuppressZonesInVStumpEnabled())
		{
			return CustomMapLoader.CanLoadEntities && this.zone == GTZone.customMaps && this.zoneStateData.state == GameEntityManager.ZoneState.Active;
		}
		return this.zoneStateData.state == GameEntityManager.ZoneState.Active;
	}

	// Token: 0x06002BC6 RID: 11206 RVA: 0x000E9EA4 File Offset: 0x000E80A4
	private static bool IsSuppressZonesInVStumpEnabled()
	{
		GorillaServer instance = GorillaServer.Instance;
		return instance != null && instance.CheckIsSuppressZonesInVStumpEnabled();
	}

	// Token: 0x06002BC7 RID: 11207 RVA: 0x000E9ECC File Offset: 0x000E80CC
	public virtual bool IsPositionInManagerBounds(Vector3 pos)
	{
		bool flag;
		if (this.boundsBoxCollider != null)
		{
			flag = this.boundsBoxCollider.bounds.Contains(pos);
		}
		else
		{
			ZoneGraphBSP instance = ZoneGraphBSP.Instance;
			if (instance != null && instance.HasCompiledTree())
			{
				ZoneDef zoneDef = instance.FindZoneAtPoint(pos);
				flag = zoneDef != null && zoneDef.zoneId == this.zone;
			}
			else
			{
				flag = true;
			}
		}
		return flag;
	}

	// Token: 0x06002BC8 RID: 11208 RVA: 0x000E9F40 File Offset: 0x000E8140
	public virtual bool IsValidClientRPC(Player sender)
	{
		bool flag = this.IsAuthorityPlayer(sender);
		bool flag2 = this.IsZoneActive();
		bool flag3 = sender.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		return flag && (flag2 || flag3);
	}

	// Token: 0x06002BC9 RID: 11209 RVA: 0x000E9F75 File Offset: 0x000E8175
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.IsValidClientRPC(sender) && this.IsValidNetId(entityNetId);
	}

	// Token: 0x06002BCA RID: 11210 RVA: 0x000E9F89 File Offset: 0x000E8189
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.IsValidClientRPC(sender, entityNetId) && this.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002BCB RID: 11211 RVA: 0x000E9F9E File Offset: 0x000E819E
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.IsValidClientRPC(sender) && this.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002BCC RID: 11212 RVA: 0x000E9FB2 File Offset: 0x000E81B2
	public bool IsValidAuthorityRPC(Player sender)
	{
		return this.IsAuthority() && (this.IsZoneActive() || sender.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
	}

	// Token: 0x06002BCD RID: 11213 RVA: 0x000E9FDA File Offset: 0x000E81DA
	public bool IsValidAuthorityRPC(Player sender, int entityNetId)
	{
		return this.IsValidAuthorityRPC(sender) && this.IsValidNetId(entityNetId);
	}

	// Token: 0x06002BCE RID: 11214 RVA: 0x000E9FEE File Offset: 0x000E81EE
	public bool IsValidAuthorityRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.IsValidAuthorityRPC(sender, entityNetId) && this.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x000EA003 File Offset: 0x000E8203
	public bool IsValidAuthorityRPC(Player sender, Vector3 pos)
	{
		return this.IsValidAuthorityRPC(sender) && this.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002BD0 RID: 11216 RVA: 0x000EA017 File Offset: 0x000E8217
	public bool IsValidEntity(GameEntityId id)
	{
		return this.GetGameEntity(id) != null;
	}

	// Token: 0x06002BD1 RID: 11217 RVA: 0x000EA026 File Offset: 0x000E8226
	public GameEntity GetGameEntity(GameEntityId id)
	{
		if (!id.IsValid())
		{
			return null;
		}
		return this.GetGameEntity(id.index);
	}

	// Token: 0x06002BD2 RID: 11218 RVA: 0x000EA040 File Offset: 0x000E8240
	public GameEntity GetGameEntityFromNetId(int netId)
	{
		int num;
		if (this.netIdToIndex.TryGetValue(netId, out num))
		{
			return this.GetGameEntity(num);
		}
		return null;
	}

	// Token: 0x06002BD3 RID: 11219 RVA: 0x000EA066 File Offset: 0x000E8266
	private GameEntity GetGameEntity(int index)
	{
		if (index == -1)
		{
			return null;
		}
		if (index < 0 || index >= this.entities.Count)
		{
			return null;
		}
		return this.entities[index];
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x000EA090 File Offset: 0x000E8290
	public T GetGameComponent<T>(GameEntityId id) where T : Component
	{
		GameEntity gameEntity = this.GetGameEntity(id);
		if (gameEntity == null)
		{
			return default(T);
		}
		return gameEntity.GetComponent<T>();
	}

	// Token: 0x06002BD5 RID: 11221 RVA: 0x000EA0C0 File Offset: 0x000E82C0
	public bool LocalValidateMigrationRecoveryItem(int entityTypeId, ref long createData)
	{
		GameObject gameObject = this.FactoryPrefabById(entityTypeId);
		if (gameObject == null)
		{
			return false;
		}
		GameEntity component = gameObject.GetComponent<GameEntity>();
		if (component != null)
		{
			for (int i = 0; i < this.zoneComponents.Count; i++)
			{
				createData = this.zoneComponents[i].ProcessMigratedGameEntityCreateData(component, createData);
			}
		}
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		for (int j = 0; j < this.zoneComponents.Count; j++)
		{
			if (!this.zoneComponents[j].ValidateMigratedGameEntity(0, entityTypeId, Vector3.zero, Quaternion.identity, createData, actorNumber))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002BD6 RID: 11222 RVA: 0x000EA168 File Offset: 0x000E8368
	public bool IsEntityValidToMigrate(GameEntity entity)
	{
		if (entity == null)
		{
			return false;
		}
		Vector3 position = VRRig.LocalRig.transform.position;
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		bool flag = true;
		int num = 0;
		while (num < this.zoneComponents.Count && flag)
		{
			flag &= this.zoneComponents[num].ValidateMigratedGameEntity(this.GetNetIdFromEntityId(entity.id), entity.typeId, position, Quaternion.identity, entity.createData, actorNumber);
			num++;
		}
		return flag;
	}

	// Token: 0x06002BD7 RID: 11223 RVA: 0x000EA1EC File Offset: 0x000E83EC
	private void BuildFactory()
	{
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder(true))
		{
			string text = "[GT/GameEntityManager]  BuildFactory: Entity names and typeIds for manager \"" + base.name + "\":";
			utf16ValueStringBuilder.AppendLine(text);
			foreach (IGameEntityZoneComponent gameEntityZoneComponent in this.zoneComponents)
			{
				IFactoryItemProvider factoryItemProvider = gameEntityZoneComponent as IFactoryItemProvider;
				if (factoryItemProvider != null)
				{
					foreach (GameEntity gameEntity in factoryItemProvider.GetFactoryItems())
					{
						if (!this.tempFactoryItems.Contains(gameEntity))
						{
							this.tempFactoryItems.Add(gameEntity);
						}
					}
				}
			}
			this.itemPrefabFactory = new Dictionary<int, GameObject>(1024);
			this.priceLookupByEntityId = new Dictionary<int, int>();
			for (int i = 0; i < this.tempFactoryItems.Count; i++)
			{
				GameObject gameObject = this.tempFactoryItems[i].gameObject;
				int staticHash = gameObject.name.GetStaticHash();
				if (gameObject.GetComponent<GRToolLantern>())
				{
					this.priceLookupByEntityId.Add(staticHash, 50);
				}
				else if (gameObject.GetComponent<GRToolCollector>())
				{
					this.priceLookupByEntityId.Add(staticHash, 50);
				}
				this.itemPrefabFactory.Add(staticHash, gameObject);
				utf16ValueStringBuilder.AppendFormat<string, int>("    - name=\"{0}\", typeId={1}\n", gameObject.name, staticHash);
				if (utf16ValueStringBuilder.Length > 5000)
				{
					utf16ValueStringBuilder.Append("... (continued in next log message) ...");
					utf16ValueStringBuilder.Clear();
					if (i + 1 < this.tempFactoryItems.Count)
					{
						utf16ValueStringBuilder.Append(text);
						utf16ValueStringBuilder.Append(" ... CONTINUED FROM PREVIOUS ...\n");
					}
				}
			}
		}
	}

	// Token: 0x06002BD8 RID: 11224 RVA: 0x000EA400 File Offset: 0x000E8600
	public void AddToFactory(IEnumerable<GameEntity> items)
	{
		foreach (GameEntity gameEntity in items)
		{
			GameObject gameObject = gameEntity.gameObject;
			int staticHash = gameObject.name.GetStaticHash();
			if (!this.itemPrefabFactory.ContainsKey(staticHash))
			{
				this.itemPrefabFactory.Add(staticHash, gameObject);
			}
		}
	}

	// Token: 0x06002BD9 RID: 11225 RVA: 0x000EA470 File Offset: 0x000E8670
	private int CreateNetId(int numToCreate)
	{
		int num = this.nextNetId;
		this.nextNetId += numToCreate;
		return num;
	}

	// Token: 0x06002BDA RID: 11226 RVA: 0x000EA488 File Offset: 0x000E8688
	private void RecalculateNextNetId()
	{
		int num = 0;
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (this.entities[i] != null)
			{
				int num2 = this.netIds[i];
				if (num2 >= 0)
				{
					int num3 = num2 + this.entities[i].builtInEntities.Count;
					if (num3 > num)
					{
						num = num3;
					}
				}
			}
		}
		this.nextNetId = num + 1;
	}

	// Token: 0x06002BDB RID: 11227 RVA: 0x000EA4FA File Offset: 0x000E86FA
	public GameEntityId RequestCreateItem(int entityTypeId, Vector3 position, Quaternion rotation, long createData)
	{
		return this.RequestCreateItem(entityTypeId, position, rotation, createData, GameEntityId.Invalid);
	}

	// Token: 0x06002BDC RID: 11228 RVA: 0x000EA50C File Offset: 0x000E870C
	public GameEntityId RequestCreateItem(int entityTypeId, Vector3 position, Quaternion rotation, long createData, GameEntityId createdByEntityId)
	{
		if (!this.IsZoneAuthority() || !this.IsZoneActive() || !this.IsPositionInManagerBounds(position))
		{
			return GameEntityId.Invalid;
		}
		int netIdFromEntityId = this.GetNetIdFromEntityId(createdByEntityId);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			if (!this.zoneComponents[i].ValidateCreateItem(0, entityTypeId, position, rotation, createData, netIdFromEntityId))
			{
				MonoBehaviour monoBehaviour = this.zoneComponents[i] as MonoBehaviour;
				if (monoBehaviour != null)
				{
					string name = monoBehaviour.name;
				}
				return GameEntityId.Invalid;
			}
		}
		long num = BitPackUtils.PackWorldPosForNetwork(position);
		int num2 = BitPackUtils.PackQuaternionForNetwork(rotation);
		int num3 = 1 + this.FactoryGetBuiltInEntityCountById(entityTypeId);
		int num4 = this.CreateNetId(num3);
		this.netIdsForCreate.Add(num4);
		this.entityTypeIdsForCreate.Add(entityTypeId);
		this.packedPositionsForCreate.Add(num);
		this.packedRotationsForCreate.Add(num2);
		this.createDataForCreate.Add(createData);
		this.createdByEntityNetIdForCreate.Add(netIdFromEntityId);
		return this.CreateAndInitItemLocal(num4, entityTypeId, position, rotation, createData, netIdFromEntityId);
	}

	// Token: 0x06002BDD RID: 11229 RVA: 0x000EA618 File Offset: 0x000E8818
	[PunRPC]
	public void CreateItemRPC(int[] netId, int[] entityTypeId, long[] packedPos, int[] packedRot, long[] createData, int[] createdByEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.CreateItem))
		{
			return;
		}
		if (netId == null || entityTypeId == null || packedPos == null || createData == null || createdByEntityNetId == null || netId.Length != entityTypeId.Length || netId.Length != packedPos.Length || netId.Length != packedRot.Length || netId.Length != createData.Length || netId.Length != createdByEntityNetId.Length)
		{
			return;
		}
		if (netId.Length > 1)
		{
			for (int i = 0; i < this.zoneComponents.Count; i++)
			{
				if (!this.zoneComponents[i].ValidateCreateItemBatchSize(netId.Length))
				{
					return;
				}
			}
		}
		for (int j = 0; j < netId.Length; j++)
		{
			if (!GameEntityManager.IsScenePlacedNetId(netId[j]))
			{
				Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPos[j]);
				Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(packedRot[j]);
				float num = 10000f;
				if (!(in vector).IsValid(in num) || !(in quaternion).IsValid() || !this.FactoryHasEntity(entityTypeId[j]) || !this.IsPositionInManagerBounds(vector))
				{
					return;
				}
				int num2 = netId[j];
				int num3 = entityTypeId[j];
				long num4 = createData[j];
				int num5 = createdByEntityNetId[j];
				bool flag = true;
				for (int k = 0; k < this.zoneComponents.Count; k++)
				{
					if (!this.zoneComponents[k].ValidateCreateItem(num2, num3, vector, quaternion, num4, num5))
					{
						flag = false;
					}
				}
				if (flag)
				{
					this.CreateAndInitItemLocal(num2, num3, vector, quaternion, num4, num5);
				}
			}
		}
	}

	// Token: 0x06002BDE RID: 11230 RVA: 0x000EA780 File Offset: 0x000E8980
	public void RequestCreateItems(List<GameEntityCreateData> entityData)
	{
		if (!this.IsZoneAuthority() || !this.IsZoneActive())
		{
			GTDev.LogError<string>(string.Format("[GameEntityManager::RequestCreateItems] Cannot create items. Zone Auth: {0} ", this.IsZoneAuthority()) + string.Format("| Zone Active: {0}", this.IsZoneActive()), null);
			return;
		}
		GameEntityManager.ClearByteBuffer(this.tempSerializeGameState);
		MemoryStream memoryStream = new MemoryStream(this.tempSerializeGameState);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(entityData.Count);
		for (int i = 0; i < entityData.Count; i++)
		{
			GameEntityCreateData gameEntityCreateData = entityData[i];
			int num = 1 + this.FactoryGetBuiltInEntityCountById(gameEntityCreateData.entityTypeId);
			int num2 = this.CreateNetId(num);
			long num3 = BitPackUtils.PackWorldPosForNetwork(gameEntityCreateData.position);
			int num4 = BitPackUtils.PackQuaternionForNetwork(gameEntityCreateData.rotation);
			binaryWriter.Write(num2);
			binaryWriter.Write(gameEntityCreateData.entityTypeId);
			binaryWriter.Write(num3);
			binaryWriter.Write(num4);
			binaryWriter.Write(gameEntityCreateData.createData);
			binaryWriter.Write(gameEntityCreateData.createdByEntityId);
		}
		long position = memoryStream.Position;
		byte[] array = GZipStream.CompressBuffer(this.tempSerializeGameState);
		this.photonView.RPC("CreateItemsRPC", RpcTarget.All, new object[]
		{
			(int)this.zone,
			array
		});
	}

	// Token: 0x06002BDF RID: 11231 RVA: 0x000EA8D0 File Offset: 0x000E8AD0
	[PunRPC]
	public void CreateItemsRPC(int zoneId, byte[] stateData, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || stateData == null || stateData.Length >= 15360 || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.CreateItems))
		{
			return;
		}
		try
		{
			using (MemoryStream memoryStream = new MemoryStream(GZipStream.UncompressBuffer(stateData)))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < this.zoneComponents.Count; i++)
					{
						if (!this.zoneComponents[i].ValidateCreateMultipleItems(zoneId, stateData, num))
						{
							return;
						}
					}
					for (int j = 0; j < num; j++)
					{
						int num2 = binaryReader.ReadInt32();
						int num3 = binaryReader.ReadInt32();
						long num4 = binaryReader.ReadInt64();
						int num5 = binaryReader.ReadInt32();
						long num6 = binaryReader.ReadInt64();
						int num7 = binaryReader.ReadInt32();
						Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(num4);
						Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(num5);
						float num8 = 10000f;
						if ((in vector).IsValid(in num8) && (in quaternion).IsValid() && this.FactoryHasEntity(num3) && this.IsPositionInManagerBounds(vector))
						{
							bool flag = true;
							for (int k = 0; k < this.zoneComponents.Count; k++)
							{
								flag &= this.zoneComponents[k].ValidateCreateItem(num2, num3, vector, quaternion, num6, num7);
							}
							if (flag)
							{
								this.CreateAndInitItemLocal(num2, num3, vector, quaternion, num6, num7);
							}
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06002BE0 RID: 11232 RVA: 0x000EAA98 File Offset: 0x000E8C98
	public void RequestMigrationRecovery(List<GameEntityCreateData> entityData)
	{
		if (entityData == null || entityData.Count == 0)
		{
			return;
		}
		GameEntityManager.ClearByteBuffer(this.tempSerializeGameState);
		using (MemoryStream memoryStream = new MemoryStream(this.tempSerializeGameState))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(entityData.Count);
				for (int i = 0; i < entityData.Count; i++)
				{
					GameEntityCreateData gameEntityCreateData = entityData[i];
					GameEntityManager._JoinWithItems_WriteOne(binaryWriter, gameEntityCreateData.entityTypeId, gameEntityCreateData.position, gameEntityCreateData.rotation, gameEntityCreateData.createData, gameEntityCreateData.createdByEntityId, gameEntityCreateData.slotIndex);
				}
				byte[] array = GZipStream.CompressBuffer(this.tempSerializeGameState);
				this.photonView.RPC("JoinWithItemsRPC", this.GetAuthorityPlayer(), new object[]
				{
					array,
					Array.Empty<int>(),
					PhotonNetwork.LocalPlayer.ActorNumber
				});
			}
		}
	}

	// Token: 0x06002BE1 RID: 11233 RVA: 0x000EABA0 File Offset: 0x000E8DA0
	public void JoinWithItems(List<GameEntity> entities)
	{
		if (entities.Count == 0)
		{
			return;
		}
		GameEntityManager.ClearByteBuffer(this.tempSerializeGameState);
		MemoryStream memoryStream = new MemoryStream(this.tempSerializeGameState);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		int num = 0;
		for (int i = 0; i < entities.Count; i++)
		{
			if (entities[i] != null)
			{
				num++;
			}
		}
		binaryWriter.Write(num);
		for (int j = 0; j < entities.Count; j++)
		{
			GameEntity gameEntity = entities[j];
			if (!(gameEntity == null))
			{
				long num2 = gameEntity.createData;
				for (int k = 0; k < this.zoneComponents.Count; k++)
				{
					num2 = this.zoneComponents[k].ProcessMigratedGameEntityCreateData(gameEntity, num2);
				}
				GameEntityManager._JoinWithItems_WriteOne(binaryWriter, gameEntity.typeId, gameEntity.transform.localPosition, gameEntity.transform.localRotation, num2, this.GetNetIdFromEntityId(gameEntity.createdByEntityId), gameEntity.slotIndex);
			}
		}
		long position = memoryStream.Position;
		byte[] array = GZipStream.CompressBuffer(this.tempSerializeGameState);
		this.photonView.RPC("JoinWithItemsRPC", this.GetAuthorityPlayer(), new object[]
		{
			array,
			Array.Empty<int>(),
			PhotonNetwork.LocalPlayer.ActorNumber
		});
	}

	// Token: 0x06002BE2 RID: 11234 RVA: 0x000EACF8 File Offset: 0x000E8EF8
	[PunRPC]
	public void PlayerLeftZoneRPC(PhotonMessageInfo info)
	{
		if (this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.PlayerLeftZone))
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
		if (gamePlayer == null)
		{
			return;
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			gamePlayer.DidJoinWithItems = false;
		}
		this._leavingItemScratch.Clear();
		foreach (GameEntityId gameEntityId in gamePlayer.IterateHeldAndSnappedItems(this))
		{
			this._leavingItemScratch.Add(gameEntityId);
		}
		for (int i = 0; i < this._leavingItemScratch.Count; i++)
		{
			GameEntityId gameEntityId2 = this._leavingItemScratch[i];
			GameEntity gameEntity = this.GetGameEntity(gameEntityId2);
			if (gameEntity != null && gameEntity.IsScenePlaced)
			{
				this.ReleaseScenePlacedHold(gameEntity);
				GameEntityManager.ScenePlacedRecord scenePlacedRecord;
				if (this.IsAuthority() && this.TryGetScenePlacedRecord(gameEntity, out scenePlacedRecord))
				{
					GameEntityManager.ResetScenePlacedTransform(gameEntity, in scenePlacedRecord);
				}
			}
			else
			{
				if (!this.netIdsForDelete.Contains(this.GetNetIdFromEntityId(gameEntityId2)))
				{
					this.netIdsForDelete.Add(this.GetNetIdFromEntityId(gameEntityId2));
				}
				this.DestroyItemLocal(gameEntityId2);
			}
		}
		this._leavingItemScratch.Clear();
		this.playerZoneJoinTimes.Remove(info.Sender.ActorNumber);
		Action onPlayerLeftZone = gamePlayer.OnPlayerLeftZone;
		if (onPlayerLeftZone == null)
		{
			return;
		}
		onPlayerLeftZone();
	}

	// Token: 0x06002BE3 RID: 11235 RVA: 0x000EAE60 File Offset: 0x000E9060
	private bool TryGetScenePlacedRecord(GameEntity entity, out GameEntityManager.ScenePlacedRecord record)
	{
		for (int i = 0; i < this.scenePlacedEntities.Count; i++)
		{
			if (this.scenePlacedEntities[i].entity == entity)
			{
				record = this.scenePlacedEntities[i];
				return true;
			}
		}
		record = default(GameEntityManager.ScenePlacedRecord);
		return false;
	}

	// Token: 0x06002BE4 RID: 11236 RVA: 0x000EAEB8 File Offset: 0x000E90B8
	[PunRPC]
	public void JoinWithItemsRPC(byte[] stateData, int[] netIds, int joiningActorNum, PhotonMessageInfo info)
	{
		bool isAuthority = this.IsAuthority();
		if (isAuthority)
		{
			if (!this.IsValidAuthorityRPC(info.Sender))
			{
				return;
			}
		}
		else if (!this.IsAuthorityPlayer(info.Sender))
		{
			return;
		}
		float num;
		bool flag = this.playerZoneJoinTimes.TryGetValue(joiningActorNum, out num) && Time.unscaledTime - num < 10f;
		GamePlayer joiningPlayer;
		bool flag2 = GamePlayer.TryGetGamePlayer(joiningActorNum, out joiningPlayer);
		bool flag3;
		if (!isAuthority)
		{
			Player authorityPlayer = this.GetAuthorityPlayer();
			int? num2 = ((authorityPlayer != null) ? new int?(authorityPlayer.ActorNumber) : null);
			int actorNumber = info.Sender.ActorNumber;
			flag3 = !((num2.GetValueOrDefault() == actorNumber) & (num2 != null));
		}
		else
		{
			flag3 = false;
		}
		bool flag4 = flag3;
		bool flag5 = isAuthority && info.Sender.ActorNumber != joiningActorNum;
		bool flag6 = stateData == null || stateData.Length >= 255;
		bool flag7 = flag2 && joiningPlayer.DidJoinWithItems && !flag;
		if (!flag2 || flag4 || flag5 || flag6 || flag7)
		{
			return;
		}
		if (!this.IsInZone())
		{
			return;
		}
		if (isAuthority)
		{
			joiningPlayer.DidJoinWithItems = true;
		}
		Action createItemsCallback = null;
		createItemsCallback = delegate
		{
			try
			{
				GamePlayer joiningPlayer2 = joiningPlayer;
				joiningPlayer2.OnPlayerInitialized = (Action)Delegate.Remove(joiningPlayer2.OnPlayerInitialized, createItemsCallback);
				using (MemoryStream memoryStream = new MemoryStream(GZipStream.UncompressBuffer(stateData)))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						int num3 = binaryReader.ReadInt32();
						if (num3 <= 4)
						{
							if (isAuthority || netIds.Length == num3)
							{
								if (isAuthority)
								{
									netIds = new int[num3];
								}
								for (int i = 0; i < num3; i++)
								{
									int num4;
									Vector3 vector;
									Quaternion quaternion;
									long num5;
									int num6;
									int num7;
									GameEntityManager._JoinWithItems_ReadOne(binaryReader, out num4, out vector, out quaternion, out num5, out num6, out num7);
									Transform transform;
									if (!joiningPlayer.TryGetSlotXform(num7, out transform))
									{
										Debug.LogError("[GT/GameEntityManager]  ERROR!!!  " + string.Format("JoinWithItemsRPC: No slot transform for item's slot, {0}.", num7));
									}
									else
									{
										if (isAuthority)
										{
											int num8 = 1 + this.FactoryGetBuiltInEntityCountById(num4);
											netIds[i] = this.CreateNetId(num8);
										}
										int num9 = netIds[i];
										Vector3 vector2 = transform.TransformPoint(vector);
										float num10 = 10000f;
										if ((in vector).IsValid(in num10) && (in quaternion).IsValid() && this.FactoryHasEntity(num4) && this.IsPositionInManagerBounds(vector2))
										{
											bool flag8 = true;
											int num11 = 0;
											while (num11 < this.zoneComponents.Count && flag8)
											{
												flag8 &= this.zoneComponents[num11].ValidateMigratedGameEntity(num9, num4, joiningPlayer.rig.transform.position, Quaternion.identity, num5, joiningActorNum);
												num11++;
											}
											if (flag8)
											{
												GameEntityId gameEntityId = this.CreateAndInitItemLocal(num9, num4, joiningPlayer.rig.transform.position, Quaternion.identity, num5, num6);
												bool flag9 = num7 == 0;
												SnapJointType snapIndexToJoint = GameSnappable.GetSnapIndexToJoint(num7);
												if (snapIndexToJoint != SnapJointType.None)
												{
													this.SnapEntityLocal(gameEntityId, flag9, vector, quaternion, (int)snapIndexToJoint, joiningPlayer.rig.Creator);
												}
												else
												{
													this.GrabEntityOnCreate(gameEntityId, flag9, vector, quaternion, joiningPlayer.rig.Creator);
												}
											}
										}
									}
								}
								if (isAuthority)
								{
									this.photonView.RPC("JoinWithItemsRPC", RpcTarget.Others, new object[] { stateData, netIds, joiningActorNum });
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		};
		if (joiningPlayer.AdditionalDataInitialized)
		{
			createItemsCallback();
			return;
		}
		GamePlayer joiningPlayer3 = joiningPlayer;
		joiningPlayer3.OnPlayerInitialized = (Action)Delegate.Combine(joiningPlayer3.OnPlayerInitialized, createItemsCallback);
	}

	// Token: 0x06002BE5 RID: 11237 RVA: 0x000EB082 File Offset: 0x000E9282
	private static void _JoinWithItems_WriteOne(BinaryWriter writer, int typeId, Vector3 localPos, Quaternion localRot, long createData, int createdByEntityId, int slotIndex)
	{
		writer.Write(typeId);
		writer.Write(BitPackUtils.PackWorldPosForNetwork(localPos));
		writer.Write(BitPackUtils.PackQuaternionForNetwork(localRot));
		writer.Write(createData);
		writer.Write(createdByEntityId);
		writer.Write((byte)(slotIndex + 1));
	}

	// Token: 0x06002BE6 RID: 11238 RVA: 0x000EB0C0 File Offset: 0x000E92C0
	private static void _JoinWithItems_ReadOne(BinaryReader reader, out int entityTypeId, out Vector3 localPos, out Quaternion localRot, out long createData, out int createdByEntityNetId, out int slotIndex)
	{
		entityTypeId = reader.ReadInt32();
		localPos = BitPackUtils.UnpackWorldPosFromNetwork(reader.ReadInt64());
		localRot = BitPackUtils.UnpackQuaternionFromNetwork(reader.ReadInt32());
		createData = reader.ReadInt64();
		createdByEntityNetId = reader.ReadInt32();
		slotIndex = (int)(reader.ReadByte() - 1);
	}

	// Token: 0x06002BE7 RID: 11239 RVA: 0x000EB114 File Offset: 0x000E9314
	public bool FactoryHasEntity(int entityTypeId)
	{
		GameObject gameObject;
		return this.itemPrefabFactory.TryGetValue(entityTypeId, out gameObject);
	}

	// Token: 0x06002BE8 RID: 11240 RVA: 0x000EB130 File Offset: 0x000E9330
	public GameObject FactoryPrefabById(int entityTypeId)
	{
		GameObject gameObject;
		if (this.itemPrefabFactory.TryGetValue(entityTypeId, out gameObject))
		{
			return gameObject;
		}
		return null;
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x000EB150 File Offset: 0x000E9350
	public GameEntity FactoryEntityById(int entityTypeId)
	{
		GameObject gameObject;
		if (this.itemPrefabFactory.TryGetValue(entityTypeId, out gameObject))
		{
			return gameObject.GetComponent<GameEntity>();
		}
		return null;
	}

	// Token: 0x06002BEA RID: 11242 RVA: 0x000EB178 File Offset: 0x000E9378
	public int FactoryGetBuiltInEntityCountById(int entityTypeId)
	{
		GameEntity gameEntity = this.FactoryEntityById(entityTypeId);
		if (gameEntity == null || gameEntity.builtInEntities == null)
		{
			return 0;
		}
		return gameEntity.builtInEntities.Count;
	}

	// Token: 0x06002BEB RID: 11243 RVA: 0x000EB1AB File Offset: 0x000E93AB
	public bool PriceLookup(int entityTypeId, out int price)
	{
		if (this.priceLookupByEntityId.TryGetValue(entityTypeId, out price))
		{
			return true;
		}
		price = -1;
		return false;
	}

	// Token: 0x06002BEC RID: 11244 RVA: 0x000EB1C4 File Offset: 0x000E93C4
	private void ValidateThatNetIdIsNotAlreadyUsed(int netId, int newTypeId)
	{
		for (int i = 0; i < this.netIds.Length; i++)
		{
			if (i < this.entities.Count && this.netIds[i] == netId)
			{
				this.entities[i] == null;
			}
		}
	}

	// Token: 0x06002BED RID: 11245 RVA: 0x000EB218 File Offset: 0x000E9418
	public GameEntityId CreateAndInitItemLocal(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int createdByEntityNetId)
	{
		GameEntity gameEntity = this.CreateItemLocal(netId, entityTypeId, position, rotation);
		if (gameEntity == null)
		{
			return GameEntityId.Invalid;
		}
		this.InitItemLocal(gameEntity, createData, createdByEntityNetId);
		return gameEntity.id;
	}

	// Token: 0x06002BEE RID: 11246 RVA: 0x000EB254 File Offset: 0x000E9454
	public GameEntity CreateItemLocal(int netId, int entityTypeId, Vector3 position, Quaternion rotation)
	{
		if (entityTypeId == -1)
		{
			return null;
		}
		this.nextNetId = Mathf.Max(netId + 1, this.nextNetId);
		GameObject gameObject;
		if (!this.itemPrefabFactory.TryGetValue(entityTypeId, out gameObject))
		{
			return null;
		}
		if (!this.createdItemTypeCount.ContainsKey(entityTypeId))
		{
			this.createdItemTypeCount[entityTypeId] = 0;
		}
		if (this.createdItemTypeCount[entityTypeId] > 100)
		{
			return null;
		}
		Dictionary<int, int> dictionary = this.createdItemTypeCount;
		int num = dictionary[entityTypeId];
		dictionary[entityTypeId] = num + 1;
		GameEntity componentInChildren = global::UnityEngine.Object.Instantiate<GameObject>(gameObject, position, rotation).GetComponentInChildren<GameEntity>();
		this.AddGameEntity(netId, componentInChildren);
		componentInChildren.Create(this, netId, entityTypeId);
		return componentInChildren;
	}

	// Token: 0x06002BEF RID: 11247 RVA: 0x000EB2F8 File Offset: 0x000E94F8
	public void InitItemLocal(GameEntity entity, long createData, int createdByEntityNetId)
	{
		entity.Init(createData, createdByEntityNetId);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			this.zoneComponents[i].OnCreateGameEntity(entity);
		}
	}

	// Token: 0x06002BF0 RID: 11248 RVA: 0x000EB338 File Offset: 0x000E9538
	public void RequestDestroyItem(GameEntityId entityId)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity != null && gameEntity.IsScenePlaced)
		{
			return;
		}
		int netIdFromEntityId = this.GetNetIdFromEntityId(entityId);
		if (!this.netIdsForDelete.Contains(netIdFromEntityId))
		{
			this.netIdsForDelete.Add(netIdFromEntityId);
		}
		int num = this.netIdsForState.IndexOf(netIdFromEntityId);
		if (num >= 0)
		{
			this.netIdsForState.RemoveAt(num);
			this.statesForState.RemoveAt(num);
		}
		this.DestroyItemLocal(entityId);
	}

	// Token: 0x06002BF1 RID: 11249 RVA: 0x000EB3BC File Offset: 0x000E95BC
	public void RequestDestroyItems(List<GameEntityId> entityIds)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < entityIds.Count; i++)
		{
			list.Add(this.GetNetIdFromEntityId(entityIds[i]));
		}
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("DestroyItemRPC", RpcTarget.All, new object[] { list.ToArray() });
		}
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x000EB424 File Offset: 0x000E9624
	[PunRPC]
	public void DestroyItemRPC(int[] entityNetId, PhotonMessageInfo info)
	{
		if (entityNetId == null || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.DestroyItem))
		{
			return;
		}
		for (int i = 0; i < entityNetId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, entityNetId[i]))
			{
				return;
			}
			if (!GameEntityManager.IsScenePlacedNetId(entityNetId[i]))
			{
				this.DestroyItemLocal(this.GetEntityIdFromNetId(entityNetId[i]));
			}
		}
	}

	// Token: 0x06002BF3 RID: 11251 RVA: 0x000EB47C File Offset: 0x000E967C
	public void DestroyItemLocal(GameEntityId entityId)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		if (!this.createdItemTypeCount.ContainsKey(gameEntity.typeId))
		{
			this.createdItemTypeCount[gameEntity.typeId] = 1;
		}
		Dictionary<int, int> dictionary = this.createdItemTypeCount;
		int typeId = gameEntity.typeId;
		int num = dictionary[typeId];
		dictionary[typeId] = num - 1;
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer))
		{
			if (gamePlayer.IsLocal())
			{
				GamePlayerLocal.instance.ClearGrabbedIfHeld(gameEntity.id, this);
			}
			gamePlayer.ClearGrabbedIfHeld(gameEntity.id, this);
		}
		GamePlayer gamePlayer2;
		if (GamePlayer.TryGetGamePlayer(gameEntity.snappedByActorNumber, out gamePlayer2))
		{
			gamePlayer2.ClearSnappedIfSnapped(gameEntity.id, this);
		}
		this.RemoveGameEntity(gameEntity);
		if (gameEntity.isBuiltIn || gameEntity.IsScenePlaced)
		{
			gameEntity.gameObject.SetActive(false);
			return;
		}
		global::UnityEngine.Object.Destroy(gameEntity.gameObject);
	}

	// Token: 0x06002BF4 RID: 11252 RVA: 0x000EB564 File Offset: 0x000E9764
	public void RequestState(GameEntityId entityId, long newState)
	{
		if (this.IsAuthority())
		{
			this.RequestStateAuthority(entityId, newState);
			return;
		}
		this.photonView.RPC("RequestStateRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			newState
		});
	}

	// Token: 0x06002BF5 RID: 11253 RVA: 0x000EB5B8 File Offset: 0x000E97B8
	private void RequestStateAuthority(GameEntityId entityId, long newState)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.GetNetIdFromEntityId(entityId);
		if (!this.IsValidNetId(netIdFromEntityId))
		{
			return;
		}
		if (this.netIdsForState.Contains(netIdFromEntityId))
		{
			this.statesForState[this.netIdsForState.IndexOf(netIdFromEntityId)] = newState;
			return;
		}
		this.netIdsForState.Add(netIdFromEntityId);
		this.statesForState.Add(newState);
	}

	// Token: 0x06002BF6 RID: 11254 RVA: 0x000EB620 File Offset: 0x000E9820
	[PunRPC]
	public void RequestStateRPC(int entityNetId, long newState, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !gamePlayer.netStateLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		if (gameEntity == null || gameEntity.IsNull())
		{
			return;
		}
		bool flag = false;
		GRToolClub component = gameEntity.GetComponent<GRToolClub>();
		GRToolCollector component2 = gameEntity.GetComponent<GRToolCollector>();
		GRToolRevive component3 = gameEntity.GetComponent<GRToolRevive>();
		GRToolLantern component4 = gameEntity.GetComponent<GRToolLantern>();
		GRToolFlash component5 = gameEntity.GetComponent<GRToolFlash>();
		GRToolDirectionalShield component6 = gameEntity.GetComponent<GRToolDirectionalShield>();
		GRToolShieldGun component7 = gameEntity.GetComponent<GRToolShieldGun>();
		if (component == null && component2 == null && component3 == null && component4 == null && component5 == null && component6 == null && component7 == null)
		{
			flag = this.IsAuthorityPlayer(info.Sender);
		}
		bool flag2 = gamePlayer.IsHoldingEntity(entityIdFromNetId, false) || gamePlayer.IsHoldingEntity(entityIdFromNetId, true);
		bool flag3 = gameEntity.lastHeldByActorNumber == info.Sender.ActorNumber;
		if (!flag && (flag2 || flag3))
		{
			if (component4 != null)
			{
				flag = component4.CanChangeState(newState);
			}
			if (component5 != null)
			{
				flag = component5.CanChangeState(newState);
			}
			if (component != null || component2 != null || component3 != null || component6 != null || component7 != null)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			bool flag4 = gameEntity.snappedByActorNumber == gamePlayer.rig.OwningNetPlayer.ActorNumber;
			if (gameEntity.canHoldingPlayerUpdateState && flag2)
			{
				flag = true;
			}
			else if (gameEntity.canLastHoldingPlayerUpdateState && flag3)
			{
				flag = true;
			}
			else if (gameEntity.canSnapPlayerUpdateState && flag4)
			{
				flag = true;
			}
		}
		IGameEntityCustomStateChange component8 = gameEntity.GetComponent<IGameEntityCustomStateChange>();
		if (component8 != null)
		{
			flag = component8.CanChangeState(newState, info.Sender.ActorNumber);
		}
		if (flag)
		{
			if (this.netIdsForState.Contains(entityNetId))
			{
				this.statesForState[this.netIdsForState.IndexOf(entityNetId)] = newState;
				return;
			}
			this.netIdsForState.Add(entityNetId);
			this.statesForState.Add(newState);
		}
	}

	// Token: 0x06002BF7 RID: 11255 RVA: 0x000EB848 File Offset: 0x000E9A48
	[PunRPC]
	public void ApplyStateRPC(int[] netId, long[] newState, PhotonMessageInfo info)
	{
		if (netId == null || newState == null || netId.Length != newState.Length || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ApplyState))
		{
			return;
		}
		for (int i = 0; i < netId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netId[i]))
			{
				return;
			}
			GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(netId[i]);
			GameEntity gameEntity = this.entities[entityIdFromNetId.index];
			if (gameEntity != null)
			{
				gameEntity.SetState(newState[i]);
			}
		}
	}

	// Token: 0x06002BF8 RID: 11256 RVA: 0x000EB8C0 File Offset: 0x000E9AC0
	public void RequestGrabEntity(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation)
	{
		bool inRoom = PhotonNetwork.InRoom;
		if (!this.IsAuthority() || !inRoom)
		{
			this.GrabEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		if (inRoom)
		{
			long num = BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation);
			this.photonView.RPC("RequestGrabEntityRPC", this.GetAuthorityPlayer(), new object[]
			{
				this.GetNetIdFromEntityId(gameEntityId),
				isLeftHand,
				num
			});
		}
	}

	// Token: 0x06002BF9 RID: 11257 RVA: 0x000EB93C File Offset: 0x000E9B3C
	[PunRPC]
	public void RequestGrabEntityRPC(int entityNetId, bool isLeftHand, long packedPosRot, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
		float num = 10000f;
		if (!(in vector).IsValid(in num) || !(in quaternion).IsValid() || vector.sqrMagnitude > 6400f)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !this.IsPlayerHandNearEntity(gamePlayer, entityNetId, isLeftHand, false, 16f) || this.IsValidEntity(gamePlayer.GetGameEntityId(isLeftHand)) || !gamePlayer.netGrabLimiter.CheckCallTime(Time.time) || gamePlayer.IsHoldingEntity(this, isLeftHand))
		{
			return;
		}
		GameEntity gameEntity = this.GetGameEntity(this.GetEntityIdFromNetId(entityNetId));
		if (gameEntity == null)
		{
			return;
		}
		if (!this.ValidateGrab(gameEntity, info.Sender.ActorNumber, isLeftHand))
		{
			return;
		}
		this.photonView.RPC("GrabEntityRPC", RpcTarget.All, new object[] { entityNetId, isLeftHand, packedPosRot, info.Sender });
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x06002BFA RID: 11258 RVA: 0x000EBA4C File Offset: 0x000E9C4C
	[PunRPC]
	public void GrabEntityRPC(int entityNetId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.GrabEntity))
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
		float num = 10000f;
		if (!(in vector).IsValid(in num) || !(in quaternion).IsValid() || vector.sqrMagnitude > 6400f)
		{
			return;
		}
		this.GrabEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, vector, quaternion, NetPlayer.Get(grabbedByPlayer));
	}

	// Token: 0x06002BFB RID: 11259 RVA: 0x000EBAC4 File Offset: 0x000E9CC4
	private void GrabEntityLocal(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(grabbedByPlayer.ActorNumber), out rigContainer))
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		if (gameEntity == null)
		{
			return;
		}
		if (grabbedByPlayer == null)
		{
			return;
		}
		int handIndex = GamePlayer.GetHandIndex(isLeftHand);
		if (grabbedByPlayer.IsLocal && gameEntity.heldByActorNumber == grabbedByPlayer.ActorNumber && gameEntity.heldByHandIndex == handIndex)
		{
			return;
		}
		GameEntityManager.TryDetachCompletely(gameEntity);
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(grabbedByPlayer.ActorNumber, out gamePlayer))
		{
			return;
		}
		GamePlayer gamePlayer2;
		if (GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer2))
		{
			int num = gamePlayer2.FindHandIndex(gameEntityId);
			bool flag = gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
			gamePlayer2.ClearGrabbedIfHeld(gameEntityId, this);
			if (num != -1 && flag)
			{
				GamePlayerLocal.instance.ClearGrabbed(num);
			}
		}
		Transform handTransform = gamePlayer.GetHandTransform(handIndex);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			if (grabbedByPlayer.IsLocal)
			{
				component.constraints = RigidbodyConstraints.FreezeAll;
				component.isKinematic = false;
			}
			else
			{
				component.constraints = RigidbodyConstraints.None;
				component.isKinematic = true;
			}
		}
		gameEntity.transform.SetParent(handTransform);
		gameEntity.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		gameEntity.transform.localScale = Vector3.one;
		gameEntity.heldByActorNumber = grabbedByPlayer.ActorNumber;
		gameEntity.heldByHandIndex = handIndex;
		gameEntity.lastHeldByActorNumber = gameEntity.heldByActorNumber;
		gamePlayer.SetGrabbed(gameEntityId, handIndex, this);
		if (grabbedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GamePlayerLocal.instance.SetGrabbed(gameEntityId, GamePlayer.GetHandIndex(isLeftHand));
			GamePlayerLocal.instance.PlayCatchFx(isLeftHand);
		}
		GameEntityManager.TryUnsnapLocal(gameEntity);
		gameEntity.PlayCatchFx();
		Action onGrabbed = gameEntity.OnGrabbed;
		if (onGrabbed != null)
		{
			onGrabbed();
		}
		CustomGameMode.OnEntityGrabbed(gameEntity, true);
	}

	// Token: 0x06002BFC RID: 11260 RVA: 0x000EBCB4 File Offset: 0x000E9EB4
	public void GrabEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		if (grabbedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GamePlayerLocal.instance.gamePlayer.DeleteGrabbedEntityLocal(GamePlayer.GetHandIndex(isLeftHand));
		}
		this.GrabEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, grabbedByPlayer);
	}

	// Token: 0x06002BFD RID: 11261 RVA: 0x000EBCF0 File Offset: 0x000E9EF0
	public GameEntityId TryGrabLocal(Vector3 handPosition, Vector3 fingerPosition, bool isLeftHand, out Vector3 closestPointOnBoundingBox, out bool fingerPositionUsed)
	{
		float num = 0.03f;
		float num2 = 0f;
		float num3 = 0.1f;
		float num4 = 0.25f;
		fingerPositionUsed = false;
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		Vector3 rigidbodyVelocity = GTPlayer.Instance.RigidbodyVelocity;
		GameEntity gameEntity = null;
		float num5 = float.MaxValue;
		Vector3 vector = handPosition;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity2 = this.entities[i];
			if (this.ValidateGrab(gameEntity2, actorNumber, isLeftHand))
			{
				float num6 = 0.75f;
				float magnitude = (handPosition - gameEntity2.transform.position).magnitude;
				if (magnitude <= num6 && (gameEntity2.snappedByActorNumber == -1 || gameEntity2.snappedByActorNumber != actorNumber || magnitude <= 0.1f))
				{
					Vector3 vector2 = gameEntity2.GetVelocity() - rigidbodyVelocity;
					float magnitude2 = vector2.magnitude;
					float num7 = Mathf.Clamp(magnitude2 * num3, 0f, num4);
					Vector3 vector3 = ((magnitude2 > 0.2f) ? (vector2.normalized * num7) : Vector3.zero);
					num2 = Mathf.Max(num, gameEntity2.pickupRangeFromSurface);
					GameEntity.RendererSet grabbableRenderers = gameEntity2.GetGrabbableRenderers();
					foreach (ValueTuple<MeshFilter, MeshRenderer> valueTuple in grabbableRenderers.renderers)
					{
						MeshFilter item = valueTuple.Item1;
						MeshRenderer item2 = valueTuple.Item2;
						if (item2.gameObject.activeInHierarchy && item2.enabled)
						{
							GameEntityManager._TryGrabLocal_TestBounds(handPosition, item2.transform, vector3, item.sharedMesh.bounds, num7, num2, gameEntity2, false, ref num5, ref gameEntity, ref vector, ref fingerPositionUsed);
							if (GameEntityManager.IsThinAlongDirection(item2.transform, item.sharedMesh.bounds, fingerPosition - handPosition, 0.1f))
							{
								GameEntityManager._TryGrabLocal_TestBounds(fingerPosition, item2.transform, vector3, item.sharedMesh.bounds, num7, num2, gameEntity2, true, ref num5, ref gameEntity, ref vector, ref fingerPositionUsed);
							}
						}
					}
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in grabbableRenderers.skinnedRenderers)
					{
						if (skinnedMeshRenderer.gameObject.activeInHierarchy && skinnedMeshRenderer.enabled)
						{
							GameEntityManager._TryGrabLocal_TestBounds(handPosition, skinnedMeshRenderer.rootBone, vector3, skinnedMeshRenderer.localBounds, num7, num2, gameEntity2, false, ref num5, ref gameEntity, ref vector, ref fingerPositionUsed);
							if (GameEntityManager.IsThinAlongDirection(skinnedMeshRenderer.rootBone, skinnedMeshRenderer.bounds, fingerPosition - handPosition, 0.1f))
							{
								GameEntityManager._TryGrabLocal_TestBounds(fingerPosition, skinnedMeshRenderer.rootBone, vector3, skinnedMeshRenderer.localBounds, num7, num2, gameEntity2, true, ref num5, ref gameEntity, ref vector, ref fingerPositionUsed);
							}
						}
					}
					if (grabbableRenderers.renderers.Count == 0 && grabbableRenderers.skinnedRenderers.Count == 0)
					{
						float num8 = magnitude;
						if (num8 < num5)
						{
							num5 = num8;
							gameEntity = gameEntity2;
							vector = gameEntity2.transform.position;
						}
					}
				}
			}
		}
		closestPointOnBoundingBox = vector;
		if (!(gameEntity != null))
		{
			return GameEntityId.Invalid;
		}
		if (num5 > Mathf.Max(num, gameEntity.pickupRangeFromSurface))
		{
			return GameEntityId.Invalid;
		}
		return gameEntity.id;
	}

	// Token: 0x06002BFE RID: 11262 RVA: 0x000EC04C File Offset: 0x000EA24C
	private static bool IsThinAlongDirection(Transform transform, Bounds bounds, Vector3 direction, float thinThreshold = 0.1f)
	{
		return GameEntityManager.GetBoundsThicknessAlongDirection(bounds, transform.InverseTransformDirection(direction)) <= thinThreshold;
	}

	// Token: 0x06002BFF RID: 11263 RVA: 0x000EC064 File Offset: 0x000EA264
	private static float GetBoundsThicknessAlongDirection(Bounds bounds, Vector3 localDirection)
	{
		localDirection.Normalize();
		Vector3 vector = new Vector3(Mathf.Abs(localDirection.x), Mathf.Abs(localDirection.y), Mathf.Abs(localDirection.z));
		return Vector3.Dot(bounds.extents, vector);
	}

	// Token: 0x06002C00 RID: 11264 RVA: 0x000EC0B0 File Offset: 0x000EA2B0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void _TryGrabLocal_TestBounds(Vector3 handPosition, Transform t, Vector3 slopProjection, Bounds bounds, float slopForSpeed, float maxAdjustedGrabDistance, GameEntity entity, bool isTestingAltPosition, ref float bestDist, ref GameEntity bestEntity, ref Vector3 closestPoint, ref bool usedAltPosition)
	{
		Vector3 vector = t.InverseTransformPoint(handPosition);
		Vector3 vector2 = t.InverseTransformPoint(handPosition + slopProjection);
		bounds.extents != bounds.extents;
		Vector3 vector3;
		float num;
		Vector3 vector4;
		float num2;
		if (GameEntityManager.SegmentHitsBounds(bounds, vector, vector2, out vector3, out num))
		{
			vector4 = ((num <= 0f) ? Vector3.zero : t.TransformVector(vector - vector3));
			num2 = vector4.magnitude - slopForSpeed;
		}
		else
		{
			vector4 = t.TransformVector(vector - bounds.ClosestPoint(vector));
			num2 = vector4.magnitude;
		}
		num2 = Mathf.Max(0f, num2 - maxAdjustedGrabDistance);
		vector4 = vector4.normalized * num2;
		if (num2 < bestDist)
		{
			bestDist = num2;
			bestEntity = entity;
			closestPoint = handPosition - vector4;
			usedAltPosition = isTestingAltPosition;
		}
	}

	// Token: 0x06002C01 RID: 11265 RVA: 0x000EC180 File Offset: 0x000EA380
	private void DrawDebugStar(Vector3 position, float radius)
	{
		for (int i = 0; i < 20; i++)
		{
			Debug.DrawLine(position, position + Random.onUnitSphere * radius, Color.red, 10f);
		}
	}

	// Token: 0x06002C02 RID: 11266 RVA: 0x000EC1BC File Offset: 0x000EA3BC
	private static bool SegmentHitsBounds(Bounds bounds, Vector3 a, Vector3 b, out Vector3 hitPoint, out float distance)
	{
		hitPoint = default(Vector3);
		distance = float.MaxValue;
		Vector3 vector = b - a;
		float magnitude = vector.magnitude;
		if (magnitude <= Mathf.Epsilon)
		{
			if (bounds.Contains(a))
			{
				distance = 0f;
				hitPoint = a;
				return true;
			}
			return false;
		}
		else
		{
			Ray ray = new Ray(a, vector / magnitude);
			if (bounds.IntersectRay(ray, out distance) && distance <= magnitude)
			{
				hitPoint = a + ray.direction * distance;
				return true;
			}
			return false;
		}
	}

	// Token: 0x06002C03 RID: 11267 RVA: 0x000EC24C File Offset: 0x000EA44C
	public bool GetEntitiesWithComponentInRadius<T>(Vector3 center, float radius, bool checkRootOnly, List<T> nearbyEntities)
	{
		float num = radius * radius;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity = this.entities[i];
			if (!(gameEntity == null))
			{
				T t;
				if (checkRootOnly)
				{
					t = gameEntity.GetComponent<T>();
				}
				else
				{
					t = gameEntity.GetComponentInChildren<T>();
				}
				if (t != null && (this.entities[i].transform.position - center).sqrMagnitude < num)
				{
					nearbyEntities.Add(t);
				}
			}
		}
		return nearbyEntities.Count > 0;
	}

	// Token: 0x06002C04 RID: 11268 RVA: 0x000EC2E0 File Offset: 0x000EA4E0
	public void LogGrabDiagnostics(Vector3 handPosition, bool isLeftHand, int handIndex)
	{
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		int num = 0;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity = this.entities[i];
			if (!(gameEntity == null) && (handPosition - gameEntity.transform.position).magnitude <= 0.75f)
			{
				num++;
				this.WhyGrabRejected(gameEntity, actorNumber, isLeftHand);
			}
		}
	}

	// Token: 0x06002C05 RID: 11269 RVA: 0x000EC358 File Offset: 0x000EA558
	private string WhyGrabRejected(GameEntity gameEntity, int playerActorNumber, bool isLeftHand)
	{
		if (gameEntity == null)
		{
			return "null";
		}
		if (!gameEntity.pickupable)
		{
			return "not pickupable";
		}
		if (gameEntity.onlyGrabActorNumber != -1 && gameEntity.onlyGrabActorNumber != playerActorNumber)
		{
			return string.Format("onlyGrabActor={0} (you={1})", gameEntity.onlyGrabActorNumber, playerActorNumber);
		}
		GamePlayer gamePlayer;
		if (gameEntity.heldByActorNumber != -1 && gameEntity.heldByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer))
		{
			return string.Format("heldByActor={0}", gameEntity.heldByActorNumber);
		}
		if (gameEntity.snappedByActorNumber != -1 && gameEntity.snappedByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity.snappedByActorNumber, out gamePlayer))
		{
			return string.Format("snappedByActor={0}", gameEntity.snappedByActorNumber);
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component != null && !component.CanGrabWithHand(isLeftHand))
		{
			return "GameSnappable disallows " + (isLeftHand ? "left" : "right") + " hand";
		}
		if (this.IsValidEntity(gameEntity.attachedToEntityId))
		{
			GameEntity gameEntity2 = this.GetGameEntity(gameEntity.attachedToEntityId);
			if (gameEntity2 != null)
			{
				if (gameEntity2.snappedByActorNumber != -1 && gameEntity2.snappedByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity2.snappedByActorNumber, out gamePlayer))
				{
					return string.Format("attachedTo '{0}' snappedByActor={1}", gameEntity2.name, gameEntity2.snappedByActorNumber);
				}
				GameSnappable component2 = gameEntity2.GetComponent<GameSnappable>();
				if (component2 != null && !component2.CanGrabWithHand(isLeftHand))
				{
					return string.Concat(new string[]
					{
						"attachedTo '",
						gameEntity2.name,
						"' GameSnappable disallows ",
						isLeftHand ? "left" : "right",
						" hand"
					});
				}
			}
		}
		return null;
	}

	// Token: 0x06002C06 RID: 11270 RVA: 0x000EC518 File Offset: 0x000EA718
	private bool ValidateGrab(GameEntity gameEntity, int playerActorNumber, bool isLeftHand)
	{
		if (gameEntity == null || !gameEntity.pickupable)
		{
			return false;
		}
		if (gameEntity.onlyGrabActorNumber != -1 && gameEntity.onlyGrabActorNumber != playerActorNumber)
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (gameEntity.heldByActorNumber != -1 && gameEntity.heldByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		if (gameEntity.snappedByActorNumber != -1 && gameEntity.snappedByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity.snappedByActorNumber, out gamePlayer))
		{
			return false;
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component != null && !component.CanGrabWithHand(isLeftHand))
		{
			return false;
		}
		if (this.IsValidEntity(gameEntity.attachedToEntityId))
		{
			GameEntity gameEntity2 = this.GetGameEntity(gameEntity.attachedToEntityId);
			if (gameEntity2 != null)
			{
				if (gameEntity2.snappedByActorNumber != -1 && gameEntity2.snappedByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity2.snappedByActorNumber, out gamePlayer))
				{
					return false;
				}
				GameSnappable component2 = gameEntity2.GetComponent<GameSnappable>();
				if (component2 != null && !component2.CanGrabWithHand(isLeftHand))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06002C07 RID: 11271 RVA: 0x000EC628 File Offset: 0x000EA828
	public T GetParentEntity<T>(Transform transform) where T : MonoBehaviour
	{
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	// Token: 0x06002C08 RID: 11272 RVA: 0x000EC668 File Offset: 0x000EA868
	public void RequestThrowEntity(GameEntityId entityId, bool isLeftHand, Vector3 headPosition, Vector3 velocity, Vector3 angVelocity)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		Vector3 vector = gameEntity.transform.position;
		Quaternion rotation = gameEntity.transform.rotation;
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			Vector3 vector2 = gameEntity.transform.TransformPoint(component.centerOfMass);
			Vector3 vector3 = vector2 - headPosition;
			float magnitude = vector3.magnitude;
			if (magnitude > 0f)
			{
				vector3 /= magnitude;
				RaycastHit raycastHit;
				if (Physics.SphereCast(headPosition, 0.05f, vector3, out raycastHit, magnitude + 0.1f, 513, QueryTriggerInteraction.Ignore))
				{
					component.GetComponentsInChildren<Collider>(this._collidersList);
					Vector3 vector4 = component.position + -raycastHit.normal * 1000f;
					float num = float.MaxValue;
					bool flag = false;
					Plane plane = new Plane(raycastHit.normal, raycastHit.point);
					foreach (Collider collider in this._collidersList)
					{
						if (collider.enabled && !collider.isTrigger)
						{
							Vector3 vector5 = collider.ClosestPoint(vector4);
							float num2 = Mathf.Abs(plane.GetDistanceToPoint(vector5));
							if (num2 < num)
							{
								num = num2;
								flag = true;
							}
						}
					}
					if (flag)
					{
						vector += raycastHit.normal * num;
					}
					else
					{
						float num3 = Mathf.Max(raycastHit.distance - 0.2f, 0f);
						Vector3 vector6 = headPosition + vector3 * num3;
						vector += vector6 - vector2;
					}
				}
			}
		}
		bool inRoom = PhotonNetwork.InRoom;
		if (!this.IsAuthority() || !inRoom)
		{
			this.ThrowEntityLocal(entityId, isLeftHand, vector, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		if (inRoom)
		{
			this.photonView.RPC("RequestThrowEntityRPC", this.GetAuthorityPlayer(), new object[]
			{
				this.GetNetIdFromEntityId(entityId),
				isLeftHand,
				vector,
				rotation,
				velocity,
				angVelocity
			});
		}
	}

	// Token: 0x06002C09 RID: 11273 RVA: 0x000EC8B8 File Offset: 0x000EAAB8
	[PunRPC]
	public void RequestThrowEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, PhotonMessageInfo info)
	{
		if (this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			float num = 10000f;
			if ((in position).IsValid(in num) && (in rotation).IsValid())
			{
				float num2 = 10000f;
				if ((in velocity).IsValid(in num2))
				{
					float num3 = 10000f;
					if ((in angVelocity).IsValid(in num3) && velocity.sqrMagnitude <= 1600f && this.IsPositionInManagerBounds(position))
					{
						GamePlayer gamePlayer;
						if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !GameEntityManager.IsPlayerHandNearPosition(gamePlayer, position, isLeftHand, false, 16f) || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId), isLeftHand) || !gamePlayer.netThrowLimiter.CheckCallTime(Time.time))
						{
							return;
						}
						this.photonView.RPC("ThrowEntityRPC", RpcTarget.All, new object[] { entityNetId, isLeftHand, position, rotation, velocity, angVelocity, info.Sender, info.SentServerTime });
						PhotonNetwork.SendAllOutgoingCommands();
						return;
					}
				}
			}
		}
	}

	// Token: 0x06002C0A RID: 11274 RVA: 0x000EC9DC File Offset: 0x000EABDC
	[PunRPC]
	public void ThrowEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player thrownByPlayer, double throwTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, position) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if ((in position).IsValid(in num) && (in rotation).IsValid())
			{
				float num2 = 10000f;
				if ((in velocity).IsValid(in num2))
				{
					float num3 = 10000f;
					if ((in angVelocity).IsValid(in num3) && velocity.sqrMagnitude <= 1600f)
					{
						NetPlayer netPlayer = NetPlayer.Get(thrownByPlayer);
						if (netPlayer.IsLocal && !this.IsAuthority())
						{
							return;
						}
						this.ThrowEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, position, rotation, velocity, angVelocity, netPlayer);
						return;
					}
				}
			}
		}
	}

	// Token: 0x06002C0B RID: 11275 RVA: 0x000ECA84 File Offset: 0x000EAC84
	private void ThrowEntityLocal(GameEntityId entityId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer thrownByPlayer)
	{
		if (entityId.index < 0 || entityId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[entityId.index];
		if (gameEntity == null)
		{
			return;
		}
		if (thrownByPlayer == null)
		{
			return;
		}
		gameEntity.transform.SetParent(null);
		if (gameEntity.IsScenePlaced)
		{
			GameEntityManager.MoveScenePlacedToHomeScene(gameEntity);
		}
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.constraints = RigidbodyConstraints.None;
			component.position = position;
			component.rotation = rotation;
			component.linearVelocity = velocity;
			component.angularVelocity = angVelocity;
		}
		gameEntity.heldByActorNumber = -1;
		gameEntity.heldByHandIndex = -1;
		gameEntity.attachedToEntityId = GameEntityId.Invalid;
		VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(thrownByPlayer);
		if (vrrig != null && gameEntity.gravityController != null)
		{
			gameEntity.gravityController.SetPersonalGravityDirection(vrrig.transform.up);
		}
		bool flag = thrownByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		int handIndex = GamePlayer.GetHandIndex(isLeftHand);
		RigContainer rigContainer;
		if (flag)
		{
			GamePlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
			GamePlayerLocal.instance.ClearGrabbed(handIndex);
			GamePlayerLocal.instance.PlayThrowFx(isLeftHand);
		}
		else if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(thrownByPlayer.ActorNumber), out rigContainer))
		{
			GamePlayer gamePlayerRef = rigContainer.Rig.GamePlayerRef;
			if (gamePlayerRef != null)
			{
				gamePlayerRef.ClearGrabbedIfHeld(entityId, this);
				gamePlayerRef.ClearSnappedIfSnapped(entityId, this);
			}
		}
		gameEntity.PlayThrowFx();
		Action onReleased = gameEntity.OnReleased;
		if (onReleased != null)
		{
			onReleased();
		}
		CustomGameMode.OnEntityGrabbed(gameEntity, false);
		GRBadge component2 = gameEntity.GetComponent<GRBadge>();
		if (component2 != null)
		{
			GRPlayer grplayer = GRPlayer.Get(thrownByPlayer.ActorNumber);
			if (grplayer != null)
			{
				grplayer.AttachBadge(component2);
			}
		}
	}

	// Token: 0x06002C0C RID: 11276 RVA: 0x000ECC68 File Offset: 0x000EAE68
	public void RequestSnapEntity(GameEntityId entityId, bool isLeftHand, SnapJointType jointType)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		Vector3 position = gameEntity.transform.position;
		Quaternion rotation = gameEntity.transform.rotation;
		if (!this.IsAuthority())
		{
			this.SnapEntityLocal(entityId, isLeftHand, position, rotation, (int)jointType, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		this.photonView.RPC("RequestSnapEntityRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			isLeftHand,
			position,
			rotation,
			(int)jointType
		});
	}

	// Token: 0x06002C0D RID: 11277 RVA: 0x000ECD0C File Offset: 0x000EAF0C
	[PunRPC]
	public void RequestSnapEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, int jointType, PhotonMessageInfo info)
	{
		if (this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			float num = 10000f;
			if ((in position).IsValid(in num) && (in rotation).IsValid() && this.IsPositionInManagerBounds(position))
			{
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
				if (gamePlayer == null || !GameEntityManager.IsPlayerHandNearPosition(gamePlayer, position, isLeftHand, false, 16f) || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId), isLeftHand) || !gamePlayer.netSnapLimiter.CheckCallTime(Time.time))
				{
					return;
				}
				this.photonView.RPC("SnapEntityRPC", RpcTarget.All, new object[] { entityNetId, isLeftHand, position, rotation, jointType, info.Sender, info.SentServerTime });
				PhotonNetwork.SendAllOutgoingCommands();
				return;
			}
		}
	}

	// Token: 0x06002C0E RID: 11278 RVA: 0x000ECDFC File Offset: 0x000EAFFC
	[PunRPC]
	public void SnapEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, int jointType, Player thrownByPlayer, double snapTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, position) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if ((in position).IsValid(in num) && (in rotation).IsValid())
			{
				if (!this.IsAuthority() && thrownByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					return;
				}
				this.SnapEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, position, rotation, jointType, NetPlayer.Get(thrownByPlayer));
				return;
			}
		}
	}

	// Token: 0x06002C0F RID: 11279 RVA: 0x000ECE7C File Offset: 0x000EB07C
	private void SnapEntityLocal(GameEntityId gameEntityId, bool isLeftHand, Vector3 position, Quaternion rotation, int jointType, NetPlayer snappedByPlayer)
	{
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntity == null)
		{
			return;
		}
		if (snappedByPlayer == null)
		{
			return;
		}
		if (snappedByPlayer.IsLocal && gameEntity.heldByActorNumber != snappedByPlayer.ActorNumber && gameEntity.lastHeldByActorNumber == snappedByPlayer.ActorNumber)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(snappedByPlayer.ActorNumber, out gamePlayer))
		{
			return;
		}
		GameEntityManager.TryDetachCompletely(gameEntity);
		SuperInfectionSnapPoint superInfectionSnapPoint;
		if (jointType == 64)
		{
			gameEntity.GetComponent<GameSnappable>();
			superInfectionSnapPoint = SuperInfectionSnapPointManager.FindSnapPoint(gamePlayer, (SnapJointType)jointType);
		}
		else
		{
			superInfectionSnapPoint = SuperInfectionSnapPointManager.FindSnapPoint(gamePlayer, (SnapJointType)jointType);
			int num = -1;
			if (jointType == 1)
			{
				num = 2;
			}
			if (jointType == 4)
			{
				num = 3;
			}
			if (num != -1)
			{
				gamePlayer.SetSnapped(gameEntityId, num, this);
			}
		}
		if (superInfectionSnapPoint == null)
		{
			return;
		}
		if (superInfectionSnapPoint.HasSnapped())
		{
			GameEntity snappedEntity = superInfectionSnapPoint.GetSnappedEntity();
			snappedEntity.transform.SetParent(null);
			snappedEntity.transform.SetLocalPositionAndRotation(position, rotation);
			Rigidbody component = snappedEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
				component.constraints = RigidbodyConstraints.None;
				component.position = position;
				component.rotation = rotation;
				component.linearVelocity = Vector3.up * 5f;
			}
			snappedEntity.heldByActorNumber = -1;
			snappedEntity.heldByHandIndex = -1;
			snappedEntity.snappedByActorNumber = -1;
			snappedEntity.snappedJoint = SnapJointType.None;
			snappedEntity.PlayThrowFx();
			Action onReleased = snappedEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased();
			}
		}
		superInfectionSnapPoint.Snapped(gameEntity);
		gameEntity.transform.SetParent(superInfectionSnapPoint.transform);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		gameEntity.transform.localScale = Vector3.one;
		Rigidbody component2 = gameEntity.GetComponent<Rigidbody>();
		if (component2 != null)
		{
			component2.isKinematic = true;
		}
		Vector3 zero = Vector3.zero;
		Quaternion identity = Quaternion.identity;
		GameSnappable component3 = gameEntity.GetComponent<GameSnappable>();
		if (component3 != null)
		{
			component3.GetSnapOffset((SnapJointType)jointType, out zero, out identity);
		}
		gameEntity.transform.localPosition = zero;
		gameEntity.transform.localRotation = identity;
		gameEntity.snappedByActorNumber = snappedByPlayer.ActorNumber;
		gameEntity.snappedJoint = (SnapJointType)jointType;
		if (component3 != null)
		{
			component3.OnSnap();
		}
		Action onSnapped = gameEntity.OnSnapped;
		if (onSnapped != null)
		{
			onSnapped();
		}
		gameEntity.PlaySnapFx();
	}

	// Token: 0x06002C10 RID: 11280 RVA: 0x000ED0CB File Offset: 0x000EB2CB
	public void SnapEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, int jointType, NetPlayer grabbedByPlayer)
	{
		this.SnapEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, jointType, grabbedByPlayer);
	}

	// Token: 0x06002C11 RID: 11281 RVA: 0x000ED0DC File Offset: 0x000EB2DC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void TryUnsnapLocal(GameEntity gameEntity)
	{
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(gameEntity.snappedByActorNumber, out gamePlayer))
		{
			gamePlayer.ClearSnappedIfSnapped(gameEntity.id, gameEntity.manager);
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component != null && component.snappedToJoint != null && component.snappedToJoint.jointType != SnapJointType.None)
		{
			SuperInfectionSnapPoint superInfectionSnapPoint = SuperInfectionSnapPointManager.FindSnapPoint(gamePlayer, component.snappedToJoint.jointType);
			if (superInfectionSnapPoint == null)
			{
				superInfectionSnapPoint = component.snappedToJoint;
			}
			component.OnUnsnap();
			superInfectionSnapPoint.Unsnapped();
			Action onUnsnapped = gameEntity.OnUnsnapped;
			if (onUnsnapped != null)
			{
				onUnsnapped();
			}
		}
		gameEntity.snappedByActorNumber = -1;
		gameEntity.snappedJoint = SnapJointType.None;
	}

	// Token: 0x06002C12 RID: 11282 RVA: 0x000ED184 File Offset: 0x000EB384
	public void RequestAttachEntity(GameEntityId entityId, GameEntityId attachToEntityId, int slotId, Vector3 localPosition, Quaternion localRotation)
	{
		if (this.GetGameEntity(entityId) == null)
		{
			return;
		}
		if (!this.IsAuthority())
		{
			this.AttachEntityLocal(entityId, attachToEntityId, slotId, localPosition, localRotation);
		}
		this.photonView.RPC("RequestAttachEntityRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			this.GetNetIdFromEntityId(attachToEntityId),
			slotId,
			localPosition,
			localRotation
		});
	}

	// Token: 0x06002C13 RID: 11283 RVA: 0x000ED210 File Offset: 0x000EB410
	public void RequestAttachEntityAuthority(GameEntityId entityId, GameEntityId attachToEntityId, int slotId, Vector3 localPosition, Quaternion localRotation)
	{
		if (this.GetGameEntity(entityId) == null)
		{
			return;
		}
		if (!this.IsAuthority())
		{
			return;
		}
		this.photonView.RPC("AttachEntityRPC", RpcTarget.All, new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			this.GetNetIdFromEntityId(attachToEntityId),
			slotId,
			localPosition,
			localRotation,
			null,
			PhotonNetwork.Time
		});
	}

	// Token: 0x06002C14 RID: 11284 RVA: 0x000ED298 File Offset: 0x000EB498
	[PunRPC]
	public void RequestAttachEntityRPC(int entityNetId, int attachToEntityNetId, int slotId, Vector3 localPosition, Quaternion localRotation, PhotonMessageInfo info)
	{
		bool flag = !this.IsValidNetId(attachToEntityNetId);
		if (this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			float num = 10000f;
			if ((in localPosition).IsValid(in num) && (in localRotation).IsValid())
			{
				if (!flag)
				{
					if (localPosition.sqrMagnitude > 4f || !this.IsEntityNearEntity(entityNetId, attachToEntityNetId, 16f))
					{
						return;
					}
				}
				else if (!this.IsPositionInManagerBounds(localPosition))
				{
					return;
				}
				GameEntity gameEntityFromNetId = this.GetGameEntityFromNetId(entityNetId);
				if (gameEntityFromNetId == null)
				{
					return;
				}
				GameDockable component = gameEntityFromNetId.GetComponent<GameDockable>();
				if (component == null)
				{
					return;
				}
				GameEntity gameEntityFromNetId2 = this.GetGameEntityFromNetId(attachToEntityNetId);
				if (gameEntityFromNetId2 != null)
				{
					GameDock component2 = gameEntityFromNetId2.GetComponent<GameDock>();
					if (component2 == null)
					{
						return;
					}
					if (!component2.CanDock(component))
					{
						return;
					}
				}
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
				if (gamePlayer == null || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId)) || !gamePlayer.netSnapLimiter.CheckCallTime(Time.time))
				{
					return;
				}
				this.photonView.RPC("AttachEntityRPC", RpcTarget.All, new object[] { entityNetId, attachToEntityNetId, slotId, localPosition, localRotation, info.Sender, info.SentServerTime });
				PhotonNetwork.SendAllOutgoingCommands();
				return;
			}
		}
	}

	// Token: 0x06002C15 RID: 11285 RVA: 0x000ED3FC File Offset: 0x000EB5FC
	[PunRPC]
	public void AttachEntityRPC(int entityNetId, int attachToEntityNetId, int slotId, Vector3 localPosition, Quaternion localRotation, Player attachedByPlayer, double snapTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId) && this.IsValidNetId(attachToEntityNetId) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if ((in localPosition).IsValid(in num) && (in localRotation).IsValid())
			{
				if (!this.IsAuthority() && attachedByPlayer != null && attachedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					return;
				}
				this.AttachEntityLocal(this.GetEntityIdFromNetId(entityNetId), this.GetEntityIdFromNetId(attachToEntityNetId), slotId, localPosition, localRotation);
				return;
			}
		}
	}

	// Token: 0x06002C16 RID: 11286 RVA: 0x000ED484 File Offset: 0x000EB684
	private void AttachEntityLocal(GameEntityId gameEntityId, GameEntityId attachToEntityId, int slotId, Vector3 localPosition, Quaternion localRotation)
	{
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntity == null)
		{
			return;
		}
		GameEntity gameEntity2 = this.entities[attachToEntityId.index];
		GameEntityManager.TryDetachCompletely(gameEntity);
		bool flag = gameEntity2 == null;
		Transform transform = ((gameEntity2 == null) ? null : gameEntity2.transform);
		gameEntity.transform.SetParent(transform);
		gameEntity.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		gameEntity.attachedToEntityId = (flag ? GameEntityId.Invalid : gameEntity2.id);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = !flag;
			component.constraints = RigidbodyConstraints.None;
		}
		GameDockable component2 = gameEntity.GetComponent<GameDockable>();
		if (gameEntity2 != null)
		{
			Action onAttached = gameEntity.OnAttached;
			if (onAttached != null)
			{
				onAttached();
			}
			GameDock component3 = gameEntity2.GetComponent<GameDock>();
			if (component3 != null)
			{
				component3.OnDock(gameEntity, gameEntity2);
				if (component2 != null)
				{
					component2.OnDock(gameEntity, gameEntity2);
				}
			}
		}
	}

	// Token: 0x06002C17 RID: 11287 RVA: 0x000ED5A4 File Offset: 0x000EB7A4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void TryDetachLocal(GameEntity gameEntity)
	{
		if (gameEntity.attachedToEntityId != GameEntityId.Invalid)
		{
			GameEntity gameEntity2 = gameEntity.manager.entities[gameEntity.attachedToEntityId.index];
			if (gameEntity2 != null)
			{
				GameDock component = gameEntity2.GetComponent<GameDock>();
				if (component != null)
				{
					component.OnUndock(gameEntity, gameEntity2);
					GameDockable component2 = gameEntity.GetComponent<GameDockable>();
					if (component2 != null)
					{
						component2.OnUndock(gameEntity, gameEntity2);
					}
				}
			}
		}
		if (gameEntity.attachedToEntityId != GameEntityId.Invalid)
		{
			Action onDetached = gameEntity.OnDetached;
			if (onDetached != null)
			{
				onDetached();
			}
		}
		gameEntity.attachedToEntityId = GameEntityId.Invalid;
	}

	// Token: 0x06002C18 RID: 11288 RVA: 0x000ED646 File Offset: 0x000EB846
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void TryDetachCompletely(GameEntity gameEntity)
	{
		if (gameEntity == null)
		{
			return;
		}
		GameEntityManager.TryRemoveFromHandLocal(gameEntity);
		GameEntityManager.TryUnsnapLocal(gameEntity);
		GameEntityManager.TryDetachLocal(gameEntity);
	}

	// Token: 0x06002C19 RID: 11289 RVA: 0x000ED664 File Offset: 0x000EB864
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void TryRemoveFromHandLocal(GameEntity gameEntity)
	{
		GameEntityId id = gameEntity.id;
		int heldByActorNumber = gameEntity.heldByActorNumber;
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(heldByActorNumber, out gamePlayer))
		{
			if (heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				GamePlayerLocal.instance.ClearGrabbedIfHeld(id, gameEntity.manager);
			}
			gamePlayer.ClearGrabbedIfHeld(id, gameEntity.manager);
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased();
			}
		}
		gameEntity.heldByActorNumber = -1;
		gameEntity.heldByHandIndex = -1;
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x000ED0CB File Offset: 0x000EB2CB
	public void AttachEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, int jointType, NetPlayer grabbedByPlayer)
	{
		this.SnapEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, jointType, grabbedByPlayer);
	}

	// Token: 0x06002C1B RID: 11291 RVA: 0x000ED6D8 File Offset: 0x000EB8D8
	public void RequestHit(GameHitData hit)
	{
		GameHittable gameComponent = this.GetGameComponent<GameHittable>(hit.hitEntityId);
		if (gameComponent == null)
		{
			return;
		}
		gameComponent.ApplyHit(hit);
		this.photonView.RPC("RequestHitRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(hit.hitEntityId),
			this.GetNetIdFromEntityId(hit.hitByEntityId),
			hit.hitTypeId,
			hit.hitEntityPosition,
			hit.hitPosition,
			hit.hitImpulse,
			hit.hittablePoint
		});
	}

	// Token: 0x06002C1C RID: 11292 RVA: 0x000ED790 File Offset: 0x000EB990
	[PunRPC]
	public void RequestHitRPC(int hittableNetId, int hitByNetId, int hitTypeId, Vector3 entityPosition, Vector3 hitPosition, Vector3 hitImpulse, int hittablePoint, PhotonMessageInfo info)
	{
		float num = 10000f;
		if ((in entityPosition).IsValid(in num))
		{
			float num2 = 10000f;
			if ((in hitPosition).IsValid(in num2))
			{
				float num3 = 10000f;
				if ((in hitImpulse).IsValid(in num3) && this.IsValidAuthorityRPC(info.Sender, hittableNetId, entityPosition) && this.IsPositionInManagerBounds(hitPosition))
				{
					GamePlayer gamePlayer;
					if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !gamePlayer.netImpulseLimiter.CheckCallTime(Time.time))
					{
						return;
					}
					GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(hittableNetId);
					GameHittable gameComponent = this.GetGameComponent<GameHittable>(entityIdFromNetId);
					if (gameComponent == null)
					{
						return;
					}
					GameHitData gameHitData = new GameHitData
					{
						hitTypeId = hitTypeId,
						hitEntityId = entityIdFromNetId,
						hitByEntityId = this.GetEntityIdFromNetId(hitByNetId),
						hitEntityPosition = entityPosition,
						hitPosition = hitPosition,
						hitImpulse = hitImpulse,
						hittablePoint = hittablePoint
					};
					if (!gameComponent.IsHitValid(gameHitData))
					{
						return;
					}
					base.SendRPC("ApplyHitRPC", RpcTarget.All, new object[] { hittableNetId, hitByNetId, hitTypeId, entityPosition, hitPosition, hitImpulse, hittablePoint, info.Sender });
					return;
				}
			}
		}
	}

	// Token: 0x06002C1D RID: 11293 RVA: 0x000ED8E4 File Offset: 0x000EBAE4
	[PunRPC]
	public void ApplyHitRPC(int hittableNetId, int hitByNetId, int hitTypeId, Vector3 entityPosition, Vector3 hitPosition, Vector3 hitImpulse, int hittablePoint, Player player, PhotonMessageInfo info)
	{
		float num = 10000f;
		if ((in hitPosition).IsValid(in num))
		{
			float num2 = 10000f;
			if ((in hitImpulse).IsValid(in num2) && this.IsValidClientRPC(info.Sender, hittableNetId, entityPosition) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.HitEntity) && player != null)
			{
				if (player.IsLocal)
				{
					return;
				}
				if (this.GetGameEntity(this.GetEntityIdFromNetId(hittableNetId)) == null)
				{
					return;
				}
				hitImpulse = Vector3.ClampMagnitude(hitImpulse, 100f);
				GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(hittableNetId);
				GameHitData gameHitData = new GameHitData
				{
					hitTypeId = hitTypeId,
					hitEntityId = entityIdFromNetId,
					hitByEntityId = this.GetEntityIdFromNetId(hitByNetId),
					hitEntityPosition = entityPosition,
					hitPosition = hitPosition,
					hitImpulse = hitImpulse,
					hitAmount = 0,
					hittablePoint = hittablePoint
				};
				GameEntity gameEntity = this.GetGameEntity(this.GetEntityIdFromNetId(hitByNetId));
				GameHittable gameComponent = this.GetGameComponent<GameHittable>(entityIdFromNetId);
				if (gameEntity != null)
				{
					GameHitter component = gameEntity.GetComponent<GameHitter>();
					if (component != null)
					{
						gameHitData.hitAmount = component.CalcHitAmount((GameHitType)hitTypeId, gameComponent, gameEntity);
					}
				}
				if (gameComponent != null)
				{
					gameComponent.ApplyHit(gameHitData);
				}
				return;
			}
		}
	}

	// Token: 0x06002C1E RID: 11294 RVA: 0x000EDA18 File Offset: 0x000EBC18
	public bool IsPlayerHandNearEntity(GamePlayer player, int entityNetId, bool isLeftHand, bool checkBothHands, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && GameEntityManager.IsPlayerHandNearPosition(player, gameEntity.transform.position, isLeftHand, checkBothHands, acceptableRadius);
	}

	// Token: 0x06002C1F RID: 11295 RVA: 0x000EDA58 File Offset: 0x000EBC58
	public static bool IsPlayerHandNearPosition(GamePlayer player, Vector3 worldPosition, bool isLeftHand, bool checkBothHands, float acceptableRadius = 16f)
	{
		bool flag = true;
		if (player != null && player.rig != null)
		{
			if (isLeftHand || checkBothHands)
			{
				flag = (worldPosition - player.rig.leftHandTransform.position).sqrMagnitude < acceptableRadius * acceptableRadius;
			}
			if (!isLeftHand || checkBothHands)
			{
				float sqrMagnitude = (worldPosition - player.rig.rightHandTransform.position).sqrMagnitude;
				flag = flag && sqrMagnitude < acceptableRadius * acceptableRadius;
			}
		}
		return flag;
	}

	// Token: 0x06002C20 RID: 11296 RVA: 0x000EDAE4 File Offset: 0x000EBCE4
	public bool IsEntityNearEntity(int entityNetId, int otherEntityNetId, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(otherEntityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && this.IsEntityNearPosition(entityNetId, gameEntity.transform.position, acceptableRadius);
	}

	// Token: 0x06002C21 RID: 11297 RVA: 0x000EDB20 File Offset: 0x000EBD20
	public bool IsEntityNearPosition(int entityNetId, Vector3 position, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && Vector3.SqrMagnitude(gameEntity.transform.position - position) < acceptableRadius * acceptableRadius;
	}

	// Token: 0x06002C22 RID: 11298 RVA: 0x0000DE5A File Offset: 0x0000C05A
	public static bool ValidateDataType<T>(object obj, out T dataAsType)
	{
		if (obj is T)
		{
			dataAsType = (T)((object)obj);
			return true;
		}
		dataAsType = default(T);
		return false;
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x000EDB64 File Offset: 0x000EBD64
	private void ClearZone(bool ignoreHeldGadgets = false)
	{
		GamePlayerLocal.instance.DebugSlotsReport(string.Format("Pre ClearZone zone={0}", this.zone));
		this.ClearPendingRPCBatches();
		if (ignoreHeldGadgets)
		{
			List<GameEntity> list = GamePlayerLocal.instance.gamePlayer.HeldAndSnappedEntities(null);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] == null || list[i].manager != this)
				{
					list.RemoveAt(i);
				}
				else if (list[i].shouldDestroyOnZoneExit)
				{
					list.RemoveAt(i);
				}
			}
			for (int j = this.entities.Count - 1; j >= 0; j--)
			{
				if (!(this.entities[j] == null) && !this.entities[j].IsScenePlaced && !list.Contains(this.entities[j]))
				{
					this.DestroyItemLocal(this.entities[j].id);
				}
			}
			GamePlayerLocal.instance.joinWithItemsSentForCurrentMigration = false;
			GamePlayerLocal.instance.gamePlayer.DidJoinWithItems = false;
			GamePlayerLocal.instance.DebugSlotsReport(string.Format("ClearZone post-preserve zone={0}", this.zone));
		}
		else
		{
			for (int k = 0; k < this.entities.Count; k++)
			{
				if (!(this.entities[k] == null) && !(this.entities[k].manager != this) && !this.entities[k].IsScenePlaced)
				{
					this.DestroyItemLocal(this.entities[k].id);
				}
			}
			GamePlayerLocal.instance.DebugSlotsReport(string.Format("ClearZone post-destroy zone={0}", this.zone));
			GamePlayer gamePlayerRef = VRRig.LocalRig.GamePlayerRef;
			if (gamePlayerRef != null)
			{
				gamePlayerRef.ClearZone(this);
			}
			GamePlayerLocal.instance.DebugSlotsReport(string.Format("ClearZone post-ClearZone(player) zone={0}", this.zone));
		}
		for (int l = 0; l < this.entities.Count; l++)
		{
			if (this.entities[l] != null && this.entities[l].manager != this)
			{
				int num = this.netIds[l];
				int num2;
				if (this.netIdToIndex.TryGetValue(num, out num2) && num2 == l)
				{
					this.netIdToIndex.Remove(num);
				}
				this.entities[l] = null;
			}
		}
		foreach (VRRig vrrig in VRRigCache.ActiveRigs)
		{
			GamePlayer gamePlayerRef2 = vrrig.GamePlayerRef;
			if (!gamePlayerRef2.IsLocal())
			{
				gamePlayerRef2.ClearZone(this);
			}
		}
		this.gameEntityData.Clear();
		this.entitiesActiveCount = 0;
		this.scenePlacedEntitiesRegistered = false;
		this.scenePlacedEntities.Clear();
		for (int m = 0; m < this.zoneComponents.Count; m++)
		{
			this.zoneComponents[m].OnZoneClear(this.zoneClearReason);
		}
		GamePlayerLocal.instance.DebugSlotsReport(string.Format("ClearZone END zone={0}", this.zone));
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x000EDEE0 File Offset: 0x000EC0E0
	public int SerializeGameState(int zoneId, byte[] bytes, int maxBytes)
	{
		MemoryStream memoryStream = new MemoryStream(bytes);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			this.zoneComponents[i].SerializeZoneData(binaryWriter);
		}
		GameEntityManager.tempEntitiesToSerialize.Clear();
		for (int j = 0; j < this.entities.Count; j++)
		{
			GameEntity gameEntity = this.entities[j];
			if (!(gameEntity == null))
			{
				int attachedPlayerActorNr = gameEntity.AttachedPlayerActorNr;
				if (attachedPlayerActorNr != -1)
				{
					bool flag = false;
					for (int k = 0; k < GameEntityManager.tempRigs.Count; k++)
					{
						if (GameEntityManager.tempRigs[k].Creator.ActorNumber == attachedPlayerActorNr)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						goto IL_00B7;
					}
				}
				GameEntityManager.tempEntitiesToSerialize.Add(gameEntity);
			}
			IL_00B7:;
		}
		binaryWriter.Write(GameEntityManager.tempEntitiesToSerialize.Count);
		for (int l = 0; l < GameEntityManager.tempEntitiesToSerialize.Count; l++)
		{
			GameEntity gameEntity2 = GameEntityManager.tempEntitiesToSerialize[l];
			if (!(gameEntity2 == null))
			{
				int netIdFromEntityId = this.GetNetIdFromEntityId(gameEntity2.id);
				binaryWriter.Write(netIdFromEntityId);
				binaryWriter.Write(gameEntity2.typeId);
				long num = BitPackUtils.PackWorldPosForNetwork(gameEntity2.transform.position);
				int num2 = BitPackUtils.PackQuaternionForNetwork(gameEntity2.transform.rotation);
				binaryWriter.Write(num);
				binaryWriter.Write(num2);
			}
		}
		for (int m = 0; m < GameEntityManager.tempEntitiesToSerialize.Count; m++)
		{
			GameEntity gameEntity3 = GameEntityManager.tempEntitiesToSerialize[m];
			if (!(gameEntity3 == null))
			{
				int netIdFromEntityId2 = this.GetNetIdFromEntityId(gameEntity3.id);
				binaryWriter.Write(netIdFromEntityId2);
				binaryWriter.Write(gameEntity3.createData);
				binaryWriter.Write(this.GetNetIdFromEntityId(gameEntity3.createdByEntityId));
				binaryWriter.Write(gameEntity3.GetState());
				int num3 = -1;
				GameEntity gameEntity4 = this.GetGameEntity(gameEntity3.attachedToEntityId);
				if (gameEntity4 != null)
				{
					num3 = this.GetNetIdFromEntityId(gameEntity4.id);
				}
				binaryWriter.Write(num3);
				if (num3 != -1)
				{
					long num4 = BitPackUtils.PackHandPosRotForNetwork(gameEntity3.transform.localPosition, gameEntity3.transform.localRotation);
					binaryWriter.Write(num4);
				}
				GameAgent component = gameEntity3.GetComponent<GameAgent>();
				bool flag2 = component != null;
				binaryWriter.Write(flag2);
				if (flag2)
				{
					Vector3 vector = Vector3.zero;
					if (component.navAgent != null)
					{
						vector = component.navAgent.destination;
					}
					long num5 = BitPackUtils.PackWorldPosForNetwork(vector);
					binaryWriter.Write(num5);
					NetPlayer targetPlayer = component.targetPlayer;
					int num6 = ((targetPlayer != null) ? targetPlayer.ActorNumber : (-1));
					binaryWriter.Write(num6);
				}
				byte b = (byte)gameEntity3.entitySerialize.Count;
				binaryWriter.Write(b);
				for (int n = 0; n < (int)b; n++)
				{
					gameEntity3.entitySerialize[n].OnGameEntitySerialize(binaryWriter);
				}
				for (int num7 = 0; num7 < this.zoneComponents.Count; num7++)
				{
					this.zoneComponents[num7].SerializeZoneEntityData(binaryWriter, gameEntity3);
				}
			}
		}
		int count = GameEntityManager.tempRigs.Count;
		binaryWriter.Write(count);
		for (int num8 = 0; num8 < GameEntityManager.tempRigs.Count; num8++)
		{
			VRRig vrrig = GameEntityManager.tempRigs[num8];
			NetPlayer owningNetPlayer = vrrig.OwningNetPlayer;
			binaryWriter.Write(owningNetPlayer.ActorNumber);
			GamePlayer gamePlayerRef = vrrig.GamePlayerRef;
			bool flag3 = gamePlayerRef != null;
			binaryWriter.Write(flag3);
			if (flag3)
			{
				gamePlayerRef.SerializeNetworkState(binaryWriter, owningNetPlayer, this);
				for (int num9 = 0; num9 < this.zoneComponents.Count; num9++)
				{
					this.zoneComponents[num9].SerializeZonePlayerData(binaryWriter, owningNetPlayer.ActorNumber);
				}
			}
		}
		return (int)memoryStream.Position;
	}

	// Token: 0x06002C25 RID: 11301 RVA: 0x000EE2C8 File Offset: 0x000EC4C8
	public unsafe void DeserializeTableState(byte[] bytes, int numBytes)
	{
		if (numBytes <= 0)
		{
			return;
		}
		GameEntityManager.tempAttachments.Clear();
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				for (int i = 0; i < this.zoneComponents.Count; i++)
				{
					this.zoneComponents[i].DeserializeZoneData(binaryReader);
				}
				int num = binaryReader.ReadInt32();
				int num2 = num;
				Span<bool> span = new Span<bool>(stackalloc byte[(UIntPtr)num2], num2);
				for (int j = 0; j < num; j++)
				{
					int num3 = binaryReader.ReadInt32();
					int num4 = binaryReader.ReadInt32();
					long num5 = binaryReader.ReadInt64();
					int num6 = binaryReader.ReadInt32();
					Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(num5);
					Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(num6);
					GameEntity gameEntityFromNetId = this.GetGameEntityFromNetId(num3);
					if (gameEntityFromNetId != null)
					{
						*span[j] = true;
						if (gameEntityFromNetId.IsScenePlaced)
						{
							gameEntityFromNetId.transform.SetPositionAndRotation(vector, quaternion);
						}
					}
					else if (GameEntityManager.IsScenePlacedNetId(num3))
					{
						*span[j] = true;
					}
					else
					{
						this.CreateItemLocal(num3, num4, vector, quaternion);
					}
				}
				int k = 0;
				while (k < num)
				{
					int num7 = binaryReader.ReadInt32();
					long num8 = binaryReader.ReadInt64();
					int num9 = binaryReader.ReadInt32();
					long num10 = binaryReader.ReadInt64();
					GameEntity gameEntityFromNetId2 = this.GetGameEntityFromNetId(num7);
					if (gameEntityFromNetId2 != null)
					{
						if (!(*span[k]))
						{
							this.InitItemLocal(gameEntityFromNetId2, num8, num9);
							gameEntityFromNetId2.SetState(num10);
						}
						else if (gameEntityFromNetId2.IsScenePlaced)
						{
							gameEntityFromNetId2.SetState(num10);
						}
					}
					int num11 = binaryReader.ReadInt32();
					if (num11 == -1)
					{
						goto IL_01DA;
					}
					long num12 = binaryReader.ReadInt64();
					if (!(gameEntityFromNetId2 == null))
					{
						Vector3 vector2;
						Quaternion quaternion2;
						BitPackUtils.UnpackHandPosRotFromNetwork(num12, out vector2, out quaternion2);
						GameEntityManager.tempAttachments.Add(new GameEntityManager.AttachmentData
						{
							entityNetId = num7,
							attachToEntityNetId = num11,
							localPosition = vector2,
							localRotation = quaternion2
						});
						goto IL_01DA;
					}
					IL_0290:
					k++;
					continue;
					IL_01DA:
					if (binaryReader.ReadBoolean())
					{
						long num13 = binaryReader.ReadInt64();
						int num14 = binaryReader.ReadInt32();
						Vector3 vector3 = BitPackUtils.UnpackWorldPosFromNetwork(num13);
						GameAgent component = gameEntityFromNetId2.GetComponent<GameAgent>();
						if (component != null)
						{
							if (component.IsOnNavMesh())
							{
								component.navAgent.destination = vector3;
							}
							component.targetPlayer = NetworkSystem.Instance.GetPlayer(num14);
						}
					}
					byte b = binaryReader.ReadByte();
					for (int l = 0; l < (int)b; l++)
					{
						gameEntityFromNetId2.entitySerialize[l].OnGameEntityDeserialize(binaryReader);
					}
					for (int m = 0; m < this.zoneComponents.Count; m++)
					{
						this.zoneComponents[m].DeserializeZoneEntityData(binaryReader, gameEntityFromNetId2);
					}
					goto IL_0290;
				}
				int num15 = binaryReader.ReadInt32();
				for (int n = 0; n < num15; n++)
				{
					int num16 = binaryReader.ReadInt32();
					if (binaryReader.ReadBoolean())
					{
						GamePlayer gamePlayer;
						GamePlayer.TryGetGamePlayer(num16, out gamePlayer);
						GamePlayer.DeserializeNetworkState(binaryReader, gamePlayer, this);
						for (int num17 = 0; num17 < this.zoneComponents.Count; num17++)
						{
							this.zoneComponents[num17].DeserializeZonePlayerData(binaryReader, num16);
						}
					}
				}
				for (int num18 = 0; num18 < GameEntityManager.tempAttachments.Count; num18++)
				{
					GameEntityManager.AttachmentData attachmentData = GameEntityManager.tempAttachments[num18];
					GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(attachmentData.entityNetId);
					GameEntityId entityIdFromNetId2 = this.GetEntityIdFromNetId(attachmentData.attachToEntityNetId);
					if (!(entityIdFromNetId == entityIdFromNetId2))
					{
						this.AttachEntityLocal(entityIdFromNetId, entityIdFromNetId2, 0, attachmentData.localPosition, attachmentData.localRotation);
					}
				}
			}
		}
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x000EE694 File Offset: 0x000EC894
	private void UpdateZoneState()
	{
		this.UpdateAuthority(GameEntityManager.tempRigs);
		if (this.IsAuthority())
		{
			this.UpdateClientsFromAuthority(GameEntityManager.tempRigs);
			this.UpdateZoneStateAuthority();
		}
		else
		{
			this.UpdateZoneStateClient();
		}
		for (int i = this.zoneStateData.zonePlayers.Count - 1; i >= 0; i--)
		{
			if (this.zoneStateData.zonePlayers[i] == null)
			{
				this.zoneStateData.zonePlayers.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x000EE710 File Offset: 0x000EC910
	private void UpdateAuthority(List<VRRig> allRigs)
	{
		if (!PhotonNetwork.InRoom && base.IsMine)
		{
			if (!this.IsAuthority())
			{
				this.guard.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
				return;
			}
		}
		else if (this.IsAuthority() && !this.IsInZone())
		{
			Player player = null;
			GTZone currentZone = VRRig.LocalRig.zoneEntity.currentZone;
			if (this.useRandomCheckForAuthority)
			{
				int num = 0;
				while (player == null)
				{
					if (num >= 10)
					{
						break;
					}
					num++;
					int num2 = Random.Range(0, allRigs.Count);
					VRRig vrrig = allRigs[num2];
					GamePlayer gamePlayer;
					if (GamePlayer.TryGetGamePlayer(vrrig, out gamePlayer) && !(gamePlayer.rig == null) && gamePlayer.rig.Creator != null && !gamePlayer.rig.isLocal)
					{
						GTZone currentZone2 = vrrig.zoneEntity.currentZone;
						if (currentZone2 == this.zone && currentZone2 != currentZone)
						{
							player = gamePlayer.rig.Creator.GetPlayerRef();
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < allRigs.Count; i++)
				{
					VRRig vrrig2 = allRigs[i];
					GamePlayer gamePlayer2;
					if (GamePlayer.TryGetGamePlayer(vrrig2, out gamePlayer2) && !(gamePlayer2.rig == null) && gamePlayer2.rig.Creator != null && !gamePlayer2.rig.isLocal)
					{
						GTZone currentZone3 = vrrig2.zoneEntity.currentZone;
						if (currentZone3 == this.zone && currentZone3 != currentZone)
						{
							player = gamePlayer2.rig.Creator.GetPlayerRef();
						}
					}
				}
			}
			if (player != null)
			{
				this.guard.TransferOwnership(player, "");
			}
		}
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x000EE8B8 File Offset: 0x000ECAB8
	private void UpdateClientsFromAuthority(List<VRRig> allRigs)
	{
		if (!this.IsInZone())
		{
			return;
		}
		for (int i = 0; i < this.zoneStateData.zoneStateRequests.Count; i++)
		{
			GameEntityManager.ZoneStateRequest zoneStateRequest = this.zoneStateData.zoneStateRequests[i];
			if (zoneStateRequest.player != null && zoneStateRequest.zone == this.zone)
			{
				this.SendZoneStateToPlayerOrTarget(zoneStateRequest.zone, zoneStateRequest.player, RpcTarget.MasterClient);
				zoneStateRequest.completed = true;
				this.zoneStateData.zoneStateRequests[i] = zoneStateRequest;
				this.zoneStateData.zoneStateRequests.RemoveAt(i);
				return;
			}
			this.zoneStateData.zoneStateRequests.RemoveAt(i);
			i--;
		}
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x000EE970 File Offset: 0x000ECB70
	public void TestSerializeTableState()
	{
		GameEntityManager.ClearByteBuffer(this.tempSerializeGameState);
		int num = this.SerializeGameState((int)this.zone, this.tempSerializeGameState, 15360);
		byte[] array = GZipStream.CompressBuffer(this.tempSerializeGameState);
		Debug.LogFormat("Test Serialize Game State Buffer Size Uncompressed {0}", new object[] { num });
		Debug.LogFormat("Test Serialize Game State Buffer Size Compressed {0}", new object[] { array.Length });
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x000EE9E0 File Offset: 0x000ECBE0
	public static void ClearByteBuffer(byte[] buffer)
	{
		int num = buffer.Length;
		for (int i = 0; i < num; i++)
		{
			buffer[i] = 0;
		}
	}

	// Token: 0x06002C2B RID: 11307 RVA: 0x000EEA04 File Offset: 0x000ECC04
	private void SendZoneStateToPlayerOrTarget(GTZone zone, Player player, RpcTarget target)
	{
		GameEntityManager.ClearByteBuffer(this.tempSerializeGameState);
		this.SerializeGameState((int)zone, this.tempSerializeGameState, 15360);
		byte[] array = GZipStream.CompressBuffer(this.tempSerializeGameState);
		byte[] array2 = new byte[512];
		int i = 0;
		int num = 0;
		int num2 = array.Length;
		while (i < num2)
		{
			int num3 = Mathf.Min(512, num2 - i);
			Array.Copy(array, i, array2, 0, num3);
			if (player != null)
			{
				this.photonView.RPC("SendTableDataRPC", player, new object[] { num, num2, array2 });
			}
			else
			{
				this.photonView.RPC("SendTableDataRPC", target, new object[] { num, num2, array2 });
			}
			i += num3;
			num++;
		}
	}

	// Token: 0x06002C2C RID: 11308 RVA: 0x000EEAE4 File Offset: 0x000ECCE4
	[PunRPC]
	public void SendTableDataRPC(int packetNum, int totalBytes, byte[] bytes, PhotonMessageInfo info)
	{
		if (!this.IsAuthorityPlayer(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.SendTableData) || bytes == null || bytes.Length >= 15360)
		{
			return;
		}
		if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingForState)
		{
			return;
		}
		if (packetNum == 0)
		{
			this.zoneStateData.numRecievedStateBytes = 0;
			for (int i = 0; i < this.zoneStateData.recievedStateBytes.Length; i++)
			{
				this.zoneStateData.recievedStateBytes[i] = 0;
			}
		}
		Array.Copy(bytes, 0, this.zoneStateData.recievedStateBytes, this.zoneStateData.numRecievedStateBytes, bytes.Length);
		this.zoneStateData.numRecievedStateBytes += bytes.Length;
		if (this.zoneStateData.numRecievedStateBytes >= totalBytes)
		{
			if (this.superInfectionManager != null && this.superInfectionManager.zoneSuperInfection == null && !this.scenePlacedEntitiesRegistered)
			{
				this.PendingTableData = true;
				this.pendingTableDataSetFrame = Time.frameCount;
				return;
			}
			this.ResolveTableData();
		}
	}

	// Token: 0x06002C2D RID: 11309 RVA: 0x000EEBE8 File Offset: 0x000ECDE8
	public void ResolveTableData()
	{
		this.PendingTableData = false;
		if (GameEntityManager.activeManager.IsNotNull() && GameEntityManager.activeManager != this)
		{
			GameEntityManager.activeManager.zoneClearReason = ZoneClearReason.MigrateGameEntityZone;
			GameEntityManager.activeManager.ClearZone(true);
		}
		this.ClearZone(true);
		this.RegisterScenePlacedEntities();
		try
		{
			byte[] array = GZipStream.UncompressBuffer(this.zoneStateData.recievedStateBytes);
			int num = array.Length;
			this.DeserializeTableState(array, num);
			this.RecalculateNextNetId();
			this.SetZoneState(GameEntityManager.ZoneState.Active);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			Debug.LogError("[GT/GameEntityManager]  ERROR!!!  ResolveTableData: See exception in previous message.");
		}
	}

	// Token: 0x06002C2E RID: 11310 RVA: 0x000EEC88 File Offset: 0x000ECE88
	private void UpdateZoneStateAuthority()
	{
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		if (gamePlayer == null || gamePlayer.rig == null || gamePlayer.rig.OwningNetPlayer == null)
		{
			return;
		}
		bool flag = this.IsInZone();
		if (!flag)
		{
			if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
				return;
			}
			if (this.entitiesActiveCount > 0 && this.ShouldClearZone())
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.ClearZone(false);
				return;
			}
		}
		GameEntityManager.ZoneState state = this.zoneStateData.state;
		if (state > GameEntityManager.ZoneState.WaitingForState)
		{
			return;
		}
		bool inRoom = PhotonNetwork.InRoom;
		bool flag2 = GameEntityManager.HasAnyScenePlacedInScene(this.GetZoneSceneName());
		bool flag3 = this.scenePlacedEntitiesRegistered;
		int num = (int)(this.zoneStateData.state | (GameEntityManager.ZoneState)((flag ? 1 : 0) << 4) | (GameEntityManager.ZoneState)((inRoom ? 1 : 0) << 5) | (GameEntityManager.ZoneState)((flag2 ? 1 : 0) << 6) | (GameEntityManager.ZoneState)((flag3 ? 1 : 0) << 7) | (GameEntityManager.ZoneState)(this.entities.Count << 8) | (GameEntityManager.ZoneState)(this.zoneComponents.Count << 20));
		if (num != this._lastUpdateZoneStateAuthLogSig)
		{
			this._lastUpdateZoneStateAuthLogSig = num;
		}
		if (flag && inRoom)
		{
			this.SetZoneState(GameEntityManager.ZoneState.Active);
			for (int i = 0; i < this.zoneComponents.Count; i++)
			{
				this.zoneComponents[i].OnZoneCreate();
			}
		}
	}

	// Token: 0x06002C2F RID: 11311 RVA: 0x000EEDDC File Offset: 0x000ECFDC
	private void UpdateZoneStateClient()
	{
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		if (gamePlayer == null || gamePlayer.rig == null || gamePlayer.rig.OwningNetPlayer == null)
		{
			return;
		}
		if (!this.IsInZone())
		{
			if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
				return;
			}
			if (this.entities.Count > 0 && this.ShouldClearZone())
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.ClearZone(true);
				return;
			}
		}
		GameEntityManager.ZoneState state = this.zoneStateData.state;
		if (state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			if (state != GameEntityManager.ZoneState.WaitingToRequestState)
			{
				return;
			}
			if (Time.timeAsDouble - this.zoneStateData.stateStartTime > 1.0)
			{
				this.RecalculateNextNetId();
				List<GameEntity> list = GamePlayerLocal.instance.gamePlayer.HeldAndSnappedEntities(null);
				this.SetZoneState(GameEntityManager.ZoneState.WaitingForState);
				this.photonView.RPC("RequestZoneStateRPC", this.GetAuthorityPlayer(), new object[] { (int)this.zone });
				this.JoinWithItems(list);
				GamePlayerLocal.instance.joinWithItemsSentForCurrentMigration = true;
			}
		}
		else if (this.HasAuthority() && this.IsInZone() && !this.IsAuthority())
		{
			this.SetZoneState(GameEntityManager.ZoneState.WaitingToRequestState);
			return;
		}
	}

	// Token: 0x06002C30 RID: 11312 RVA: 0x000EEF1C File Offset: 0x000ED11C
	protected virtual bool IsInZone()
	{
		if (GorillaComputer.instance.IsPlayerInVirtualStump() && GameEntityManager.IsSuppressZonesInVStumpEnabled())
		{
			return CustomMapLoader.CanLoadEntities && this.zone == GTZone.customMaps;
		}
		if (VRRig.LocalRig.zoneEntity.currentZone != this.zone)
		{
			return false;
		}
		bool flag = global::GorillaGameModes.GameMode.CurrentGameModeType == GameModeType.SuperCasual || global::GorillaGameModes.GameMode.CurrentGameModeType == GameModeType.SuperInfect;
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			if ((flag || !(this.zoneComponents[i] is SuperInfectionManager)) && !this.zoneComponents[i].IsZoneReady())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002C31 RID: 11313 RVA: 0x000EEFC4 File Offset: 0x000ED1C4
	private bool ShouldClearZone()
	{
		bool flag = false;
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			flag |= this.zoneComponents[i].ShouldClearZone();
		}
		return flag;
	}

	// Token: 0x06002C32 RID: 11314 RVA: 0x000EF000 File Offset: 0x000ED200
	private void SetZoneState(GameEntityManager.ZoneState newState)
	{
		if (newState == this.zoneStateData.state)
		{
			return;
		}
		bool flag = this.IsZoneActive();
		this.zoneStateData.state = newState;
		this.zoneStateData.stateStartTime = Time.timeAsDouble;
		switch (this.zoneStateData.state)
		{
		case GameEntityManager.ZoneState.WaitingToEnterZone:
		{
			bool flag2 = this.ShouldClearZone();
			bool flag3 = this.zoneClearReason == ZoneClearReason.MigrateGameEntityZone;
			bool flag4 = this.zoneClearReason == ZoneClearReason.Disconnect;
			bool flag5 = !flag2 && !flag3 && !flag4;
			if (flag4 && GameEntityManager.activeManager == this)
			{
				GameEntityManager.activeManager = null;
				GamePlayerLocal.instance.currGameEntityManager = null;
			}
			if (!this.IsAuthority())
			{
				this.photonView.RPC("PlayerLeftZoneRPC", this.GetAuthorityPlayer(), Array.Empty<object>());
			}
			this.ClearZone(flag5);
			break;
		}
		case GameEntityManager.ZoneState.WaitingForState:
		{
			this.zoneStateData.numRecievedStateBytes = 0;
			for (int i = 0; i < this.zoneStateData.recievedStateBytes.Length; i++)
			{
				this.zoneStateData.recievedStateBytes[i] = 0;
			}
			this.RegisterScenePlacedEntities();
			if (this.scenePlacedEntities.Count > 0 && GamePlayerLocal.instance != null && GamePlayerLocal.instance.currGameEntityManager != this)
			{
				GamePlayerLocal.instance.currGameEntityManager = this;
				GamePlayerLocal.instance.pendingFullMigration = true;
			}
			break;
		}
		case GameEntityManager.ZoneState.Active:
			if (GameEntityManager.activeManager == this)
			{
				for (int j = 0; j < this.zoneComponents.Count; j++)
				{
					try
					{
						this.zoneComponents[j].OnZoneInit();
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
				this.RegisterScenePlacedEntities();
			}
			else
			{
				GameEntityManager activeManager = GameEntityManager.activeManager;
				GameEntityManager.activeManager = this;
				for (int k = 0; k < this.zoneComponents.Count; k++)
				{
					try
					{
						this.zoneComponents[k].OnZoneInit();
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
					}
				}
				this.RegisterScenePlacedEntities();
				GamePlayerLocal.instance.MigrateToEntityManager(this);
				if (activeManager.IsNotNull())
				{
					activeManager.zoneClearReason = ZoneClearReason.MigrateGameEntityZone;
					activeManager.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
				}
			}
			break;
		}
		bool flag6 = this.IsZoneActive();
		if (flag6 != flag)
		{
			GameEntityManager.ZoneActiveChangeEvent onZoneActiveChanged = this.OnZoneActiveChanged;
			if (onZoneActiveChanged == null)
			{
				return;
			}
			onZoneActiveChanged(flag6);
		}
	}

	// Token: 0x06002C33 RID: 11315 RVA: 0x000EF26C File Offset: 0x000ED46C
	public void DebugSendState()
	{
		this.SetZoneState(GameEntityManager.ZoneState.WaitingToRequestState);
	}

	// Token: 0x06002C34 RID: 11316 RVA: 0x000EF278 File Offset: 0x000ED478
	[PunRPC]
	public void RequestZoneStateRPC(int zoneId, PhotonMessageInfo info)
	{
		int actorNumber = info.Sender.ActorNumber;
		if (!this.IsAuthority())
		{
			return;
		}
		if (zoneId != (int)this.zone || this.zoneStateData.zoneStateRequests == null)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer))
		{
			return;
		}
		if (!gamePlayer.newJoinZoneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.playerZoneJoinTimes[actorNumber] = Time.unscaledTime;
		for (int i = 0; i < this.zoneStateData.zoneStateRequests.Count; i++)
		{
			Player player = this.zoneStateData.zoneStateRequests[i].player;
			if (player != null && player.ActorNumber == actorNumber)
			{
				return;
			}
		}
		this.zoneStateData.zoneStateRequests.Add(new GameEntityManager.ZoneStateRequest
		{
			player = info.Sender,
			zone = this.zone,
			completed = false
		});
	}

	// Token: 0x06002C35 RID: 11317 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06002C36 RID: 11318 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002C37 RID: 11319 RVA: 0x000EF363 File Offset: 0x000ED563
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.superInfectionManager != null)
		{
			this.superInfectionManager.WriteDataPUN(stream, info);
		}
	}

	// Token: 0x06002C38 RID: 11320 RVA: 0x000EF380 File Offset: 0x000ED580
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.superInfectionManager != null)
		{
			this.superInfectionManager.ReadDataPUN(stream, info);
		}
	}

	// Token: 0x06002C39 RID: 11321 RVA: 0x000EF39D File Offset: 0x000ED59D
	private void OnNetworkJoinedRoom()
	{
		this.GetZoneSceneName();
		this.zoneClearReason = ZoneClearReason.JoinZone;
		this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
	}

	// Token: 0x06002C3A RID: 11322 RVA: 0x000EF3B4 File Offset: 0x000ED5B4
	private void OnNetworkLeftRoom()
	{
		this.zoneClearReason = ZoneClearReason.Disconnect;
		if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
		}
		else
		{
			if (GameEntityManager.activeManager == this)
			{
				GameEntityManager.activeManager = null;
				GamePlayerLocal.instance.currGameEntityManager = null;
			}
			this.ClearZone(false);
		}
		this.playerZoneJoinTimes.Clear();
	}

	// Token: 0x06002C3B RID: 11323 RVA: 0x000EF410 File Offset: 0x000ED610
	private void OnNetworkPlayerLeft(NetPlayer leavingPlayer)
	{
		int num = 0;
		foreach (GameEntity gameEntity in this.entities)
		{
			if (gameEntity != null && gameEntity.IsAttachedToPlayer(leavingPlayer))
			{
				num++;
			}
		}
		this.playerZoneJoinTimes.Remove(leavingPlayer.ActorNumber);
	}

	// Token: 0x06002C3C RID: 11324 RVA: 0x000EF488 File Offset: 0x000ED688
	public void OnRigDeactivated(RigContainer container)
	{
		GamePlayer gamePlayerRef = container.Rig.GamePlayerRef;
		if (this != GameEntityManager.activeManager)
		{
			int num = 0;
			foreach (GameEntity gameEntity in this.entities)
			{
				if (gameEntity != null && gameEntity.IsAttachedToPlayer(container.Rig.Creator))
				{
					num++;
				}
			}
			return;
		}
		if (gamePlayerRef != null)
		{
			List<GameEntityId> list = gamePlayerRef.HeldAndSnappedItems(this);
			this._leavingItemScratch.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				GameEntity gameEntity2 = this.GetGameEntity(list[i]);
				if (gameEntity2 != null && gameEntity2.IsScenePlaced)
				{
					this.ReleaseScenePlacedHold(gameEntity2);
					GameEntityManager.ScenePlacedRecord scenePlacedRecord;
					if (this.IsAuthority() && this.TryGetScenePlacedRecord(gameEntity2, out scenePlacedRecord))
					{
						GameEntityManager.ResetScenePlacedTransform(gameEntity2, in scenePlacedRecord);
					}
				}
				else if (this.IsAuthority())
				{
					this._leavingItemScratch.Add(list[i]);
				}
			}
			if (this.IsAuthority() && this._leavingItemScratch.Count > 0)
			{
				this.RequestDestroyItems(this._leavingItemScratch);
			}
			this._leavingItemScratch.Clear();
		}
		gamePlayerRef.ResetData();
	}

	// Token: 0x06002C3D RID: 11325 RVA: 0x000EF5E0 File Offset: 0x000ED7E0
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		GameEntityManager.AuthorityChangeEvent onAuthorityChanged = this.OnAuthorityChanged;
		if (onAuthorityChanged != null)
		{
			onAuthorityChanged(fromPlayer, toPlayer);
		}
		if (toPlayer == null || !toPlayer.IsLocal)
		{
			return;
		}
		if (fromPlayer == null || fromPlayer.InRoom)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(fromPlayer.ActorNumber, out gamePlayer))
		{
			return;
		}
		this._leavingItemScratch.Clear();
		foreach (GameEntityId gameEntityId in gamePlayer.IterateHeldAndSnappedItems(this))
		{
			this._leavingItemScratch.Add(gameEntityId);
		}
		for (int i = 0; i < this._leavingItemScratch.Count; i++)
		{
			GameEntityId gameEntityId2 = this._leavingItemScratch[i];
			GameEntity gameEntity = this.GetGameEntity(gameEntityId2);
			if (gameEntity != null && gameEntity.IsScenePlaced)
			{
				this.ReleaseScenePlacedHold(gameEntity);
				GameEntityManager.ScenePlacedRecord scenePlacedRecord;
				if (this.IsAuthority() && this.TryGetScenePlacedRecord(gameEntity, out scenePlacedRecord))
				{
					GameEntityManager.ResetScenePlacedTransform(gameEntity, in scenePlacedRecord);
				}
			}
			else
			{
				if (!this.netIdsForDelete.Contains(this.GetNetIdFromEntityId(gameEntityId2)))
				{
					this.netIdsForDelete.Add(this.GetNetIdFromEntityId(gameEntityId2));
				}
				this.DestroyItemLocal(gameEntityId2);
			}
		}
		this._leavingItemScratch.Clear();
		Action onPlayerLeftZone = gamePlayer.OnPlayerLeftZone;
		if (onPlayerLeftZone == null)
		{
			return;
		}
		onPlayerLeftZone();
	}

	// Token: 0x06002C3E RID: 11326 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06002C40 RID: 11328 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x06002C41 RID: 11329 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x06002C42 RID: 11330 RVA: 0x000EF734 File Offset: 0x000ED934
	public void RefreshRigList()
	{
		GameEntityManager.tempRigs.Clear();
		GameEntityManager.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GameEntityManager.tempRigs);
	}

	// Token: 0x06002C43 RID: 11331 RVA: 0x000EF75E File Offset: 0x000ED95E
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitSceneUnloadHandler()
	{
		SceneManager.sceneUnloaded += GameEntityManager.OnZoneSceneUnloaded;
	}

	// Token: 0x06002C44 RID: 11332 RVA: 0x000EF774 File Offset: 0x000ED974
	private static void OnZoneSceneUnloaded(Scene scene)
	{
		List<GameEntity> list;
		if (!GameEntityManager.s_scenePlacedEntities.TryGetValue(scene.name, out list))
		{
			return;
		}
		for (int i = list.Count - 1; i >= 0; i--)
		{
			GameEntity gameEntity = list[i];
			if (!(gameEntity == null))
			{
				global::UnityEngine.Object.Destroy(gameEntity.gameObject);
			}
		}
		GameEntityManager.s_scenePlacedEntities.Remove(scene.name);
	}

	// Token: 0x06002C45 RID: 11333 RVA: 0x000EF7D8 File Offset: 0x000ED9D8
	internal string GetZoneSceneName()
	{
		if (string.IsNullOrEmpty(this.cachedZoneSceneName))
		{
			this.cachedZoneSceneName = ((ZoneManagement.instance != null) ? ZoneManagement.instance.GetSceneNameForZone(this.zone) : null);
			if (string.IsNullOrEmpty(this.cachedZoneSceneName))
			{
				this.cachedZoneSceneName = base.gameObject.scene.name;
			}
		}
		return this.cachedZoneSceneName;
	}

	// Token: 0x06002C46 RID: 11334 RVA: 0x000EF844 File Offset: 0x000EDA44
	internal static bool HasAnyScenePlacedInScene(string sceneName)
	{
		List<GameEntity> list;
		return GameEntityManager.s_scenePlacedEntities.TryGetValue(sceneName, out list) && list.Count > 0;
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x000EF86C File Offset: 0x000EDA6C
	internal static void RegisterScenePlacedEntity(GameEntity entity)
	{
		string name = entity.gameObject.scene.name;
		List<GameEntity> list;
		if (!GameEntityManager.s_scenePlacedEntities.TryGetValue(name, out list))
		{
			list = new List<GameEntity>(8);
			GameEntityManager.s_scenePlacedEntities[name] = list;
		}
		if (!list.Contains(entity))
		{
			list.Add(entity);
			GameEntityManager.NotifyManagersOfLateScenePlacedEntity(entity, name);
		}
	}

	// Token: 0x06002C48 RID: 11336 RVA: 0x000EF8C8 File Offset: 0x000EDAC8
	private static void NotifyManagersOfLateScenePlacedEntity(GameEntity entity, string sceneName)
	{
		foreach (KeyValuePair<int, GameEntityManager> keyValuePair in GameEntityManager.managersByZone)
		{
			GameEntityManager value = keyValuePair.Value;
			if (!(value == null) && value.scenePlacedEntitiesRegistered && !(value.GetZoneSceneName() != sceneName))
			{
				value.RegisterSingleScenePlacedEntity(entity);
			}
		}
	}

	// Token: 0x06002C49 RID: 11337 RVA: 0x000EF944 File Offset: 0x000EDB44
	internal static void UnregisterScenePlacedEntity(GameEntity entity)
	{
		int num = ((entity.manager != null) ? entity.GetNetId() : 0);
		string text;
		string text2;
		if (num != 0 && GameEntityManager.s_scenePlacedHomeScenes.TryGetValue(num, out text))
		{
			text2 = text;
			GameEntityManager.s_scenePlacedHomeScenes.Remove(num);
		}
		else
		{
			text2 = entity.gameObject.scene.name;
		}
		List<GameEntity> list;
		if (GameEntityManager.s_scenePlacedEntities.TryGetValue(text2, out list))
		{
			list.Remove(entity);
			if (list.Count == 0)
			{
				GameEntityManager.s_scenePlacedEntities.Remove(text2);
			}
		}
	}

	// Token: 0x06002C4A RID: 11338 RVA: 0x000EF9CB File Offset: 0x000EDBCB
	internal static bool IsScenePlacedNetId(int netId)
	{
		return netId < -1 && netId != int.MinValue;
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x000EF9E0 File Offset: 0x000EDBE0
	public static int NetIdFromXSceneRefId(int uniqueId)
	{
		int num = -uniqueId;
		if (num == -1)
		{
			num = -2;
		}
		return num;
	}

	// Token: 0x06002C4C RID: 11340 RVA: 0x000EF9F8 File Offset: 0x000EDBF8
	internal static int ComputeNetIdFromHierarchyForCustomMaps(Transform t)
	{
		int num = t.gameObject.scene.name.GetStaticHash();
		Transform transform = t;
		while (transform != null)
		{
			num = StaticHash.Compute(num, transform.name.GetStaticHash());
			transform = transform.parent;
		}
		if (num > 0)
		{
			num = -num;
		}
		if (num == 0 || num == -1 || num == -2147483648)
		{
			num = -2;
		}
		return num;
	}

	// Token: 0x06002C4F RID: 11343 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002C50 RID: 11344 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04003863 RID: 14435
	private const string preLog = "[GT/GameEntityManager]  ";

	// Token: 0x04003864 RID: 14436
	private const string preErr = "[GT/GameEntityManager]  ERROR!!!  ";

	// Token: 0x04003865 RID: 14437
	private const string preErrBeta = "[GT/GameEntityManager]  ERROR!!!  (beta only log) ";

	// Token: 0x04003866 RID: 14438
	private const int MAX_STATE_BYTES = 15360;

	// Token: 0x04003867 RID: 14439
	private const int MAX_CHUNK_BYTES = 512;

	// Token: 0x04003868 RID: 14440
	private const int MAX_JOINWITHITEMS_BYTES = 255;

	// Token: 0x04003869 RID: 14441
	public const float MAX_LOCAL_MAGNITUDE_SQ = 6400f;

	// Token: 0x0400386A RID: 14442
	public const float MAX_DISTANCE_FROM_HAND = 16f;

	// Token: 0x0400386B RID: 14443
	public const float MAX_ENTITY_DIST = 16f;

	// Token: 0x0400386C RID: 14444
	public const float MAX_THROW_SPEED_SQ = 1600f;

	// Token: 0x0400386D RID: 14445
	public const int MAX_ENTITY_COUNT_PER_TYPE = 100;

	// Token: 0x0400386E RID: 14446
	public const int INVALID_ID = -1;

	// Token: 0x0400386F RID: 14447
	public const int INVALID_INDEX = -1;

	// Token: 0x04003870 RID: 14448
	private static List<GameEntityManager> allManagers = new List<GameEntityManager>(8);

	// Token: 0x04003871 RID: 14449
	internal static readonly Dictionary<int, GameEntityManager> managersByZone = new Dictionary<int, GameEntityManager>(8);

	// Token: 0x04003872 RID: 14450
	public GTZone zone;

	// Token: 0x04003873 RID: 14451
	public PhotonView photonView;

	// Token: 0x04003874 RID: 14452
	public RequestableOwnershipGuard guard;

	// Token: 0x04003875 RID: 14453
	public Player prevAuthorityPlayer;

	// Token: 0x04003876 RID: 14454
	[FormerlySerializedAs("zoneLimit")]
	public BoxCollider boundsBoxCollider;

	// Token: 0x04003877 RID: 14455
	public bool useRandomCheckForAuthority;

	// Token: 0x04003878 RID: 14456
	public GameAgentManager gameAgentManager;

	// Token: 0x04003879 RID: 14457
	public GhostReactorManager ghostReactorManager;

	// Token: 0x0400387A RID: 14458
	public CustomMapsGameManager customMapsManager;

	// Token: 0x0400387B RID: 14459
	public SuperInfectionManager superInfectionManager;

	// Token: 0x0400387C RID: 14460
	protected List<IGameEntityZoneComponent> zoneComponents;

	// Token: 0x0400387D RID: 14461
	private List<GameEntity> entities;

	// Token: 0x0400387E RID: 14462
	private int entitiesActiveCount;

	// Token: 0x0400387F RID: 14463
	private List<GameEntityData> gameEntityData;

	// Token: 0x04003880 RID: 14464
	public List<GameEntity> tempFactoryItems;

	// Token: 0x04003885 RID: 14469
	private Dictionary<int, GameObject> itemPrefabFactory;

	// Token: 0x04003886 RID: 14470
	private Dictionary<int, int> priceLookupByEntityId;

	// Token: 0x04003887 RID: 14471
	private List<GameEntity> tempEntities = new List<GameEntity>();

	// Token: 0x04003888 RID: 14472
	private List<int> netIdsForCreate;

	// Token: 0x04003889 RID: 14473
	private List<int> entityTypeIdsForCreate;

	// Token: 0x0400388A RID: 14474
	private List<int> packedRotationsForCreate;

	// Token: 0x0400388B RID: 14475
	private List<long> packedPositionsForCreate;

	// Token: 0x0400388C RID: 14476
	private List<long> createDataForCreate;

	// Token: 0x0400388D RID: 14477
	private List<int> createdByEntityNetIdForCreate;

	// Token: 0x0400388E RID: 14478
	private float createCooldown = 0.24f;

	// Token: 0x0400388F RID: 14479
	private float lastCreateSent;

	// Token: 0x04003890 RID: 14480
	private List<int> netIdsForDelete;

	// Token: 0x04003891 RID: 14481
	private float destroyCooldown = 0.25f;

	// Token: 0x04003892 RID: 14482
	private float lastDestroySent;

	// Token: 0x04003893 RID: 14483
	private List<int> netIdsForState;

	// Token: 0x04003894 RID: 14484
	private List<long> statesForState;

	// Token: 0x04003895 RID: 14485
	private float lastStateSent;

	// Token: 0x04003896 RID: 14486
	private float stateCooldown;

	// Token: 0x04003897 RID: 14487
	private Dictionary<int, int> netIdToIndex;

	// Token: 0x04003898 RID: 14488
	private NativeArray<int> netIds;

	// Token: 0x04003899 RID: 14489
	private Dictionary<int, int> createdItemTypeCount;

	// Token: 0x0400389A RID: 14490
	private const float ZONE_MIGRATION_RECOVERY_DURATION = 10f;

	// Token: 0x0400389B RID: 14491
	private readonly Dictionary<int, float> playerZoneJoinTimes = new Dictionary<int, float>(20);

	// Token: 0x0400389D RID: 14493
	private ZoneClearReason zoneClearReason;

	// Token: 0x0400389E RID: 14494
	[NonSerialized]
	public Action<GameEntity> OnEntityRemoved;

	// Token: 0x0400389F RID: 14495
	[NonSerialized]
	public Action<GameEntity> OnEntityAdded;

	// Token: 0x040038A2 RID: 14498
	private int pendingTableDataSetFrame;

	// Token: 0x040038A3 RID: 14499
	[DebugReadout]
	private GameEntityManager.ZoneStateData zoneStateData;

	// Token: 0x040038A4 RID: 14500
	private int nextNetId = 1;

	// Token: 0x040038A5 RID: 14501
	private string cachedZoneSceneName = string.Empty;

	// Token: 0x040038A6 RID: 14502
	public CallLimitersList<CallLimiter, GameEntityManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GameEntityManager.RPC>();

	// Token: 0x040038A7 RID: 14503
	private bool scenePlacedEntitiesRegistered;

	// Token: 0x040038A8 RID: 14504
	private float scenePlacedBoundsCheckTimer;

	// Token: 0x040038A9 RID: 14505
	private int _lastUpdateZoneStateAuthLogSig = int.MinValue;

	// Token: 0x040038AA RID: 14506
	private readonly List<GameEntityManager.ScenePlacedRecord> scenePlacedEntities = new List<GameEntityManager.ScenePlacedRecord>(16);

	// Token: 0x040038AB RID: 14507
	private readonly List<GameEntityId> _leavingItemScratch = new List<GameEntityId>(4);

	// Token: 0x040038AC RID: 14508
	private List<Collider> _collidersList = new List<Collider>(16);

	// Token: 0x040038AD RID: 14509
	private static List<VRRig> tempRigs = new List<VRRig>(32);

	// Token: 0x040038AE RID: 14510
	private static List<GameEntity> tempEntitiesToSerialize = new List<GameEntity>(512);

	// Token: 0x040038AF RID: 14511
	private static List<GameEntityManager.AttachmentData> tempAttachments = new List<GameEntityManager.AttachmentData>(512);

	// Token: 0x040038B0 RID: 14512
	private byte[] tempSerializeGameState = new byte[15360];

	// Token: 0x040038B1 RID: 14513
	[OnEnterPlay_Clear]
	private static readonly Dictionary<string, List<GameEntity>> s_scenePlacedEntities = new Dictionary<string, List<GameEntity>>();

	// Token: 0x040038B2 RID: 14514
	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, string> s_scenePlacedHomeScenes = new Dictionary<int, string>();

	// Token: 0x020006D3 RID: 1747
	// (Invoke) Token: 0x06002C52 RID: 11346
	public delegate void ZoneStartEvent(GTZone zoneId);

	// Token: 0x020006D4 RID: 1748
	// (Invoke) Token: 0x06002C56 RID: 11350
	public delegate void ZoneClearEvent(GTZone zoneId);

	// Token: 0x020006D5 RID: 1749
	// (Invoke) Token: 0x06002C5A RID: 11354
	public delegate void AuthorityChangeEvent(NetPlayer fromPlayer, NetPlayer toPlayer);

	// Token: 0x020006D6 RID: 1750
	// (Invoke) Token: 0x06002C5E RID: 11358
	public delegate void ZoneActiveChangeEvent(bool active);

	// Token: 0x020006D7 RID: 1751
	private enum ZoneState
	{
		// Token: 0x040038B4 RID: 14516
		WaitingToEnterZone,
		// Token: 0x040038B5 RID: 14517
		WaitingToRequestState,
		// Token: 0x040038B6 RID: 14518
		WaitingForState,
		// Token: 0x040038B7 RID: 14519
		Active
	}

	// Token: 0x020006D8 RID: 1752
	private struct ZoneStateRequest
	{
		// Token: 0x040038B8 RID: 14520
		public Player player;

		// Token: 0x040038B9 RID: 14521
		public GTZone zone;

		// Token: 0x040038BA RID: 14522
		public bool completed;
	}

	// Token: 0x020006D9 RID: 1753
	private class ZoneStateData
	{
		// Token: 0x040038BB RID: 14523
		public GameEntityManager.ZoneState state;

		// Token: 0x040038BC RID: 14524
		public double stateStartTime;

		// Token: 0x040038BD RID: 14525
		public List<GameEntityManager.ZoneStateRequest> zoneStateRequests;

		// Token: 0x040038BE RID: 14526
		public List<Player> zonePlayers;

		// Token: 0x040038BF RID: 14527
		[HideInInspector]
		public byte[] recievedStateBytes;

		// Token: 0x040038C0 RID: 14528
		[HideInInspector]
		public int numRecievedStateBytes;
	}

	// Token: 0x020006DA RID: 1754
	public enum RPC
	{
		// Token: 0x040038C2 RID: 14530
		CreateItem,
		// Token: 0x040038C3 RID: 14531
		CreateItems,
		// Token: 0x040038C4 RID: 14532
		DestroyItem,
		// Token: 0x040038C5 RID: 14533
		ApplyState,
		// Token: 0x040038C6 RID: 14534
		GrabEntity,
		// Token: 0x040038C7 RID: 14535
		ThrowEntity,
		// Token: 0x040038C8 RID: 14536
		SendTableData,
		// Token: 0x040038C9 RID: 14537
		HitEntity,
		// Token: 0x040038CA RID: 14538
		PlayerLeftZone
	}

	// Token: 0x020006DB RID: 1755
	private struct ScenePlacedRecord
	{
		// Token: 0x040038CB RID: 14539
		public GameEntity entity;

		// Token: 0x040038CC RID: 14540
		public Vector3 position;

		// Token: 0x040038CD RID: 14541
		public Quaternion rotation;

		// Token: 0x040038CE RID: 14542
		public float uniformScale;
	}

	// Token: 0x020006DC RID: 1756
	private struct AttachmentData
	{
		// Token: 0x040038CF RID: 14543
		public int entityNetId;

		// Token: 0x040038D0 RID: 14544
		public int attachToEntityNetId;

		// Token: 0x040038D1 RID: 14545
		public Vector3 localPosition;

		// Token: 0x040038D2 RID: 14546
		public Quaternion localRotation;
	}
}
