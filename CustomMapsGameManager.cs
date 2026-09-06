using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A75 RID: 2677
public class CustomMapsGameManager : MonoBehaviour, IGameEntityZoneComponent
{
	// Token: 0x060044E7 RID: 17639 RVA: 0x0017043F File Offset: 0x0016E63F
	private void Awake()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			Object.Destroy(this);
			return;
		}
		CustomMapsGameManager.instance = this;
		this.customMapsAgents = new Dictionary<int, AIAgent>(Constants.aiAgentLimit);
		CustomMapsGameManager.tempCreateEntitiesList = new List<GameEntityCreateData>(Constants.aiAgentLimit);
	}

	// Token: 0x060044E8 RID: 17640 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Start()
	{
	}

	// Token: 0x060044E9 RID: 17641 RVA: 0x0017047C File Offset: 0x0016E67C
	public void CreatePlacedEntities(List<MapEntity> entities)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("CustomMapsManager::CreateAIAgents not the authority", null);
			return;
		}
		int gameAgentCount = this.gameAgentManager.GetGameAgentCount();
		if (gameAgentCount >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>("[CustomMapsGameManager::CreateAIAgents] Failed to create agent. Max Agent count " + string.Format("({0}) has been reached!", Constants.aiAgentLimit), null);
			return;
		}
		CustomMapsGameManager.tempCreateEntitiesList.Clear();
		int num = ((Constants.aiAgentLimit - gameAgentCount < 0) ? 0 : (Constants.aiAgentLimit - gameAgentCount));
		int num2 = Mathf.Min(entities.Count, num);
		if (num2 < entities.Count)
		{
			GTDev.LogWarning<string>(string.Format("[CustomMapsGameManager::CreateAIAgents] Only creating {0} out of the ", num2) + string.Format("requested {0} agents. Max Agent count ({1}) has been reached.!", entities.Count, Constants.aiAgentLimit), null);
		}
		for (int i = 0; i < num2; i++)
		{
			if (entities[i].IsNull())
			{
				Debug.Log(string.Format("[CustomMapsGameManager::CreateAIAgents] Requested entity to create is null! {0}/{1}", i, entities.Count));
			}
			else
			{
				int num3 = ((entities[i] is AIAgent) ? "CustomMapsAIAgent".GetStaticHash() : "CustomMapsGrabbableEntity".GetStaticHash());
				if (!this.gameEntityManager.FactoryHasEntity(num3))
				{
					Debug.LogErrorFormat("[CustomMapsManager::CreateAIAgents] Cannot Find Entity in Factory {0} {1}", new object[]
					{
						entities[i].gameObject.name,
						num3
					});
				}
				else
				{
					GameEntityCreateData gameEntityCreateData = new GameEntityCreateData
					{
						entityTypeId = num3,
						position = entities[i].transform.position,
						rotation = entities[i].transform.rotation,
						createData = entities[i].GetPackedCreateData(),
						createdByEntityId = -1,
						slotIndex = -1
					};
					CustomMapsGameManager.tempCreateEntitiesList.Add(gameEntityCreateData);
				}
			}
		}
		if (CustomMapsGameManager.tempCreateEntitiesList.Count > 0)
		{
			this.gameEntityManager.RequestCreateItems(CustomMapsGameManager.tempCreateEntitiesList);
			CustomMapsGameManager.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x060044EA RID: 17642 RVA: 0x0017068F File Offset: 0x0016E88F
	public void TEST_Spawning()
	{
		GTDev.Log<string>("CustomMapsGameManager::TEST_Spawn starting spawn", null);
		base.StartCoroutine(this.TEST_Spawn());
	}

	// Token: 0x060044EB RID: 17643 RVA: 0x001706A9 File Offset: 0x0016E8A9
	private IEnumerator TEST_Spawn()
	{
		while (this.spawnCount < 10)
		{
			yield return new WaitForSeconds(5f);
			GTDev.Log<string>("CustomMapsGameManager::TEST_Spawn spawning enemy", null);
			this.TEST_index = ((this.TEST_index == 5) ? 3 : 5);
			this.SpawnEnemyFromPoint("79e43963", this.TEST_index);
			this.spawnCount++;
		}
		yield break;
	}

	// Token: 0x060044EC RID: 17644 RVA: 0x001706B8 File Offset: 0x0016E8B8
	public GameEntityId SpawnEnemyFromPoint(string spawnPointId, int enemyTypeId)
	{
		AISpawnPoint aispawnPoint;
		if (!AISpawnManager.instance.GetSpawnPoint(spawnPointId, out aispawnPoint))
		{
			GTDev.LogError<string>("CustomMapsGameManager::SpawnEnemyFromPoint cannot find spawn point", null);
			return GameEntityId.Invalid;
		}
		return this.SpawnEnemyAtLocation(enemyTypeId, aispawnPoint.transform.position, aispawnPoint.transform.rotation);
	}

	// Token: 0x060044ED RID: 17645 RVA: 0x00170704 File Offset: 0x0016E904
	public GameEntityId SpawnEnemyAtLocation(int enemyTypeId, Vector3 position, Quaternion rotation)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed: Not Authority", null);
			return GameEntityId.Invalid;
		}
		if (this.gameEntityManager.GetGameEntities().Count >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>(string.Format("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed: Max Agents ({0}) reached.", Constants.aiAgentLimit), null);
			return GameEntityId.Invalid;
		}
		int staticHash = "CustomMapsAIAgent".GetStaticHash();
		if (!this.gameEntityManager.FactoryHasEntity(staticHash))
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed cannot find entity type", null);
			return GameEntityId.Invalid;
		}
		return this.gameEntityManager.RequestCreateItem(staticHash, position, rotation, (long)enemyTypeId);
	}

	// Token: 0x060044EE RID: 17646 RVA: 0x001707A0 File Offset: 0x0016E9A0
	public void SpawnEnemyClient(int enemyTypeId, int agentId)
	{
		if (this.gameEntityManager.IsAuthority())
		{
			return;
		}
		if (enemyTypeId == -1)
		{
			return;
		}
		AIAgent aiagent;
		if (AISpawnManager.HasInstance && AISpawnManager.instance.SpawnEnemy(enemyTypeId, out aiagent))
		{
			aiagent.transform.parent = AISpawnManager.instance.transform;
			this.customMapsAgents[agentId] = aiagent;
			return;
		}
		MapEntity mapEntity;
		if (MapSpawnManager.instance.SpawnEntity(enemyTypeId, out mapEntity))
		{
			aiagent = (AIAgent)mapEntity;
			aiagent.transform.parent = AISpawnManager.instance.transform;
			this.customMapsAgents[agentId] = aiagent;
			return;
		}
	}

	// Token: 0x060044EF RID: 17647 RVA: 0x00170834 File Offset: 0x0016EA34
	public GameEntityId SpawnGrabbableAtLocation(int enemyTypeId, Vector3 position, Quaternion rotation)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnGrabbableAtLocation] Failed: Not Authority", null);
			return GameEntityId.Invalid;
		}
		if (this.gameEntityManager.GetGameEntities().Count >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>(string.Format("[CustomMapsGameManager::SpawnGrabbableAtLocation] Failed: Max Entities ({0}) reached.", Constants.aiAgentLimit), null);
			return GameEntityId.Invalid;
		}
		int staticHash = "CustomMapsGrabbableEntity".GetStaticHash();
		if (!this.gameEntityManager.FactoryHasEntity(staticHash))
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnGrabbableAtLocation] Failed cannot find entity type", null);
			return GameEntityId.Invalid;
		}
		return this.gameEntityManager.RequestCreateItem(staticHash, position, rotation, (long)enemyTypeId);
	}

	// Token: 0x060044F0 RID: 17648 RVA: 0x000FCC07 File Offset: 0x000FAE07
	public long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData)
	{
		return createData;
	}

	// Token: 0x060044F1 RID: 17649 RVA: 0x00002076 File Offset: 0x00000276
	public bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr)
	{
		return false;
	}

	// Token: 0x060044F2 RID: 17650 RVA: 0x001708D0 File Offset: 0x0016EAD0
	public bool ValidateCreateMultipleItems(int zoneId, byte[] compressedStateData, int EntityCount)
	{
		return EntityCount <= Constants.aiAgentLimit;
	}

	// Token: 0x060044F3 RID: 17651 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool ValidateCreateItemBatchSize(int size)
	{
		return true;
	}

	// Token: 0x060044F4 RID: 17652 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool ValidateCreateItem(int nedId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int createdByEntityNetId)
	{
		return true;
	}

	// Token: 0x060044F5 RID: 17653 RVA: 0x001708DD File Offset: 0x0016EADD
	private bool IsAuthority()
	{
		return this.gameEntityManager.IsAuthority();
	}

	// Token: 0x060044F6 RID: 17654 RVA: 0x001708EA File Offset: 0x0016EAEA
	private bool IsDriver()
	{
		return CustomMapsTerminal.IsDriver;
	}

	// Token: 0x060044F7 RID: 17655 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnZoneCreate()
	{
	}

	// Token: 0x060044F8 RID: 17656 RVA: 0x001708F1 File Offset: 0x0016EAF1
	public void OnZoneInit()
	{
		if (this.hasCreatedPlacedEntitiesForZone)
		{
			return;
		}
		this.hasCreatedPlacedEntitiesForZone = true;
		if (CustomMapsGameManager.agentsToCreateOnZoneInit.IsNullOrEmpty<MapEntity>())
		{
			return;
		}
		if (!this.gameEntityManager.IsAuthority())
		{
			return;
		}
		this.CreatePlacedEntities(CustomMapsGameManager.agentsToCreateOnZoneInit);
	}

	// Token: 0x060044F9 RID: 17657 RVA: 0x00170929 File Offset: 0x0016EB29
	public void OnZoneClear(ZoneClearReason reason)
	{
		this.hasCreatedPlacedEntitiesForZone = false;
	}

	// Token: 0x060044FA RID: 17658 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool ShouldClearZone()
	{
		return true;
	}

	// Token: 0x060044FB RID: 17659 RVA: 0x00170932 File Offset: 0x0016EB32
	public bool IsZoneReady()
	{
		return CustomMapLoader.CanLoadEntities && NetworkSystem.Instance.InRoom;
	}

	// Token: 0x060044FC RID: 17660 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnCreateGameEntity(GameEntity entity)
	{
	}

	// Token: 0x060044FD RID: 17661 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void SetupCollisions(GameObject go)
	{
	}

	// Token: 0x060044FE RID: 17662 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void SerializeZoneData(BinaryWriter writer)
	{
	}

	// Token: 0x060044FF RID: 17663 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void DeserializeZoneData(BinaryReader reader)
	{
	}

	// Token: 0x06004500 RID: 17664 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x06004501 RID: 17665 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x06004502 RID: 17666 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void SerializeZonePlayerData(BinaryWriter writer, int actorNumber)
	{
	}

	// Token: 0x06004503 RID: 17667 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void DeserializeZonePlayerData(BinaryReader reader, int actorNumber)
	{
	}

	// Token: 0x06004504 RID: 17668 RVA: 0x00170947 File Offset: 0x0016EB47
	public static GameEntityManager GetEntityManager()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			return CustomMapsGameManager.instance.gameEntityManager;
		}
		return null;
	}

	// Token: 0x06004505 RID: 17669 RVA: 0x00170961 File Offset: 0x0016EB61
	public static GameAgentManager GetAgentManager()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			return CustomMapsGameManager.instance.gameAgentManager;
		}
		return null;
	}

	// Token: 0x06004506 RID: 17670 RVA: 0x0017097C File Offset: 0x0016EB7C
	public static CustomMapsAIBehaviourController GetBehaviorControllerForEntity(GameEntityId entityId)
	{
		GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
		if (entityManager.IsNull())
		{
			return null;
		}
		GameEntity gameEntity = entityManager.GetGameEntity(entityId);
		if (gameEntity.IsNull())
		{
			return null;
		}
		return gameEntity.gameObject.GetComponent<CustomMapsAIBehaviourController>();
	}

	// Token: 0x06004507 RID: 17671 RVA: 0x001709B6 File Offset: 0x0016EBB6
	public static void AddAgentsToCreate(List<MapEntity> entitiesToCreate)
	{
		if (CustomMapsGameManager.instance.IsNull())
		{
			return;
		}
		if (entitiesToCreate.IsNullOrEmpty<MapEntity>())
		{
			return;
		}
		CustomMapsGameManager.agentsToCreateOnZoneInit.AddRange(entitiesToCreate);
	}

	// Token: 0x06004508 RID: 17672 RVA: 0x001709D9 File Offset: 0x0016EBD9
	public static void ClearAgentsToCreate()
	{
		CustomMapsGameManager.agentsToCreateOnZoneInit.Clear();
	}

	// Token: 0x06004509 RID: 17673 RVA: 0x001709E5 File Offset: 0x0016EBE5
	public void OnPlayerHit(GameEntityId hitByEntityId, GRPlayer player, Vector3 hitPosition)
	{
		this.ghostReactorManager.RequestEnemyHitPlayer(GhostReactor.EnemyType.CustomMapsEnemy, hitByEntityId, player, hitPosition);
	}

	// Token: 0x040056E9 RID: 22249
	public GameEntityManager gameEntityManager;

	// Token: 0x040056EA RID: 22250
	public GameAgentManager gameAgentManager;

	// Token: 0x040056EB RID: 22251
	public GhostReactorManager ghostReactorManager;

	// Token: 0x040056EC RID: 22252
	public static CustomMapsGameManager instance;

	// Token: 0x040056ED RID: 22253
	private const string AGENT_PREFAB_NAME = "CustomMapsAIAgent";

	// Token: 0x040056EE RID: 22254
	private const string GRABBABLE_PREFAB_NAME = "CustomMapsGrabbableEntity";

	// Token: 0x040056EF RID: 22255
	private Dictionary<int, AIAgent> customMapsAgents;

	// Token: 0x040056F0 RID: 22256
	private static List<GameEntityCreateData> tempCreateEntitiesList = new List<GameEntityCreateData>(128);

	// Token: 0x040056F1 RID: 22257
	private static List<MapEntity> agentsToCreateOnZoneInit = new List<MapEntity>(128);

	// Token: 0x040056F2 RID: 22258
	private bool hasCreatedPlacedEntitiesForZone;

	// Token: 0x040056F3 RID: 22259
	private int TEST_index;

	// Token: 0x040056F4 RID: 22260
	private int spawnCount;
}
