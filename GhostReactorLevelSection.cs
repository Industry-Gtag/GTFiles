using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000715 RID: 1813
public class GhostReactorLevelSection : MonoBehaviour
{
	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x06002DA7 RID: 11687 RVA: 0x000F7FA7 File Offset: 0x000F61A7
	public Transform Anchor
	{
		get
		{
			return this.anchorTransform;
		}
	}

	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x06002DA8 RID: 11688 RVA: 0x000F7FAF File Offset: 0x000F61AF
	public List<Transform> Anchors
	{
		get
		{
			return this.anchors;
		}
	}

	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x06002DA9 RID: 11689 RVA: 0x000F7FB7 File Offset: 0x000F61B7
	public GhostReactorLevelSection.SectionType Type
	{
		get
		{
			return this.sectionType;
		}
	}

	// Token: 0x17000470 RID: 1136
	// (get) Token: 0x06002DAA RID: 11690 RVA: 0x000F7FBF File Offset: 0x000F61BF
	public BoxCollider BoundingCollider
	{
		get
		{
			return this.boundingCollider;
		}
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x000F7FC8 File Offset: 0x000F61C8
	private void Awake()
	{
		this.spawnPointGroupLookup = new GhostReactorLevelSection.SpawnPointGroup[11];
		for (int i = 0; i < this.spawnPointGroups.Count; i++)
		{
			this.spawnPointGroups[i].SpawnPointIndexes = new List<int>();
			int type = (int)this.spawnPointGroups[i].type;
			if (type < this.spawnPointGroupLookup.Length)
			{
				this.spawnPointGroupLookup[type] = this.spawnPointGroups[i];
			}
		}
		this.hazardousMaterials = new List<GRHazardousMaterial>(32);
		base.GetComponentsInChildren<GRHazardousMaterial>(this.hazardousMaterials);
		for (int j = 0; j < this.patrolPaths.Count; j++)
		{
			if (this.patrolPaths[j] == null)
			{
				Debug.LogErrorFormat("Why does {0} have a null patrol path at index {1}", new object[]
				{
					base.gameObject.name,
					j
				});
			}
			else
			{
				this.patrolPaths[j].index = j;
			}
		}
		this.prePlacedGameEntities = new List<GameEntity>(128);
		base.GetComponentsInChildren<GameEntity>(this.prePlacedGameEntities);
		for (int k = 0; k < this.prePlacedGameEntities.Count; k++)
		{
			this.prePlacedGameEntities[k].gameObject.SetActive(false);
		}
		this.renderers = new List<Renderer>(512);
		this.hidden = false;
		base.GetComponentsInChildren<Renderer>(false, this.renderers);
		for (int l = this.renderers.Count - 1; l >= 0; l--)
		{
			if (this.renderers[l] == null || !this.renderers[l].enabled)
			{
				this.renderers.RemoveAt(l);
			}
		}
		if (this.boundingCollider == null)
		{
			Debug.LogWarningFormat("Missing Bounding Collider for section {0}", new object[] { base.gameObject.name });
		}
	}

	// Token: 0x06002DAC RID: 11692 RVA: 0x000F81A8 File Offset: 0x000F63A8
	public static void RandomizeIndices(List<int> list, int count, ref SRand randomGenerator)
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		randomGenerator.Shuffle<int>(list);
	}

	// Token: 0x06002DAD RID: 11693 RVA: 0x000F81D8 File Offset: 0x000F63D8
	public void InitLevelSection(int sectionIndex, GhostReactor reactor)
	{
		this.index = sectionIndex;
		for (int i = 0; i < this.hazardousMaterials.Count; i++)
		{
			this.hazardousMaterials[i].Init(reactor);
		}
	}

	// Token: 0x06002DAE RID: 11694 RVA: 0x000F8214 File Offset: 0x000F6414
	public void SpawnSectionEntities(ref SRand randomGenerator, GameEntityManager gameEntityManager, GhostReactor reactor, List<GhostReactorSpawnConfig> spawnConfigs, float respawnCount)
	{
		if (spawnConfigs == null)
		{
			spawnConfigs = this.spawnConfigs;
		}
		if (spawnConfigs != null && spawnConfigs.Count > 0)
		{
			GhostReactorSpawnConfig ghostReactorSpawnConfig = spawnConfigs[randomGenerator.NextInt(spawnConfigs.Count)];
			Debug.LogFormat("Spawn Ghost Reactor Level Section {0} {1}", new object[]
			{
				base.gameObject.name,
				ghostReactorSpawnConfig.name
			});
			for (int i = 0; i < this.spawnPointGroups.Count; i++)
			{
				this.spawnPointGroups[i].CurrentIndex = 0;
				this.spawnPointGroups[i].NeedsRandomization = true;
			}
			for (int j = 0; j < ghostReactorSpawnConfig.entitySpawnGroups.Count; j++)
			{
				int num = ghostReactorSpawnConfig.entitySpawnGroups[j].spawnCount;
				if (num > 0)
				{
					int spawnPointType = (int)ghostReactorSpawnConfig.entitySpawnGroups[j].spawnPointType;
					if (spawnPointType < this.spawnPointGroupLookup.Length)
					{
						GhostReactorLevelSection.SpawnPointGroup spawnPointGroup = this.spawnPointGroupLookup[spawnPointType];
						if (spawnPointGroup != null)
						{
							if (spawnPointGroup.NeedsRandomization)
							{
								spawnPointGroup.NeedsRandomization = false;
								GhostReactorLevelSection.RandomizeIndices(spawnPointGroup.SpawnPointIndexes, spawnPointGroup.spawnPoints.Count, ref randomGenerator);
							}
							num = Mathf.Min(num, spawnPointGroup.spawnPoints.Count);
							for (int k = 0; k < num; k++)
							{
								int currentIndex = spawnPointGroup.CurrentIndex;
								GREntitySpawnPoint nextSpawnPoint = spawnPointGroup.GetNextSpawnPoint();
								nextSpawnPoint == null;
								GameEntity entity = ghostReactorSpawnConfig.entitySpawnGroups[j].entity;
								if (ghostReactorSpawnConfig.entitySpawnGroups[j].randomEntity != null)
								{
									ghostReactorSpawnConfig.entitySpawnGroups[j].randomEntity.TryForRandomItem(reactor, ref randomGenerator, out entity, 0);
								}
								if (!(entity == null))
								{
									int staticHash = entity.name.GetStaticHash();
									long num2 = -1L;
									if (nextSpawnPoint.applyScale)
									{
										num2 = BitPackUtils.PackWorldPosForNetwork(nextSpawnPoint.transform.localScale);
									}
									else if (spawnPointGroup.type == GhostReactorSpawnConfig.SpawnPointType.Enemy || spawnPointGroup.type == GhostReactorSpawnConfig.SpawnPointType.Pest || nextSpawnPoint.patrolPath != null)
									{
										int num3 = 255;
										if (nextSpawnPoint.patrolPath != null)
										{
											num3 = nextSpawnPoint.patrolPath.index;
										}
										int num4 = (int)respawnCount;
										if (randomGenerator.NextFloat() < respawnCount - (float)num4)
										{
											num4++;
										}
										GhostReactor.EnemyEntityCreateData enemyEntityCreateData;
										enemyEntityCreateData.respawnCount = num4;
										enemyEntityCreateData.sectionIndex = this.index;
										enemyEntityCreateData.patrolIndex = num3;
										num2 = enemyEntityCreateData.Pack();
									}
									GameEntityCreateData gameEntityCreateData = new GameEntityCreateData
									{
										entityTypeId = staticHash,
										position = nextSpawnPoint.transform.position,
										rotation = nextSpawnPoint.transform.rotation,
										createData = num2,
										createdByEntityId = -1,
										slotIndex = -1
									};
									GhostReactorLevelSection.tempCreateEntitiesList.Add(gameEntityCreateData);
									if (GhostReactorLevelSection.tempCreateEntitiesList.Count > 25)
									{
										gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
										GhostReactorLevelSection.tempCreateEntitiesList.Clear();
									}
								}
							}
						}
					}
				}
			}
			for (int l = 0; l < this.prePlacedGameEntities.Count; l++)
			{
				if (!this.prePlacedGameEntities[l].isBuiltIn)
				{
					int staticHash2 = this.prePlacedGameEntities[l].gameObject.name.GetStaticHash();
					if (!gameEntityManager.FactoryHasEntity(staticHash2))
					{
						Debug.LogErrorFormat("Cannot Find Entity in Factory {0} {1} Trying to spawn in {2}", new object[]
						{
							this.prePlacedGameEntities[l].gameObject.name,
							staticHash2,
							base.gameObject.name
						});
					}
					else
					{
						GameEntityCreateData gameEntityCreateData2 = new GameEntityCreateData
						{
							entityTypeId = staticHash2,
							position = this.prePlacedGameEntities[l].transform.position,
							rotation = this.prePlacedGameEntities[l].transform.rotation,
							createData = 0L,
							createdByEntityId = -1,
							slotIndex = -1
						};
						GhostReactorLevelSection.tempCreateEntitiesList.Add(gameEntityCreateData2);
						if (GhostReactorLevelSection.tempCreateEntitiesList.Count > 25)
						{
							gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
							GhostReactorLevelSection.tempCreateEntitiesList.Clear();
						}
					}
				}
			}
		}
	}

	// Token: 0x06002DAF RID: 11695 RVA: 0x000F8660 File Offset: 0x000F6860
	public void RespawnEntity(ref SRand randomGenerator, GameEntityManager gameEntityManager, int entityId, long entityCreateData, GameEntityId createdByEntityId)
	{
		if (0 > this.spawnPointGroupLookup.Length)
		{
			return;
		}
		GhostReactorLevelSection.SpawnPointGroup spawnPointGroup = this.spawnPointGroupLookup[0];
		int count = spawnPointGroup.spawnPoints.Count;
		if (count > 3)
		{
			this.rotatingIndexForRespawn = (this.rotatingIndexForRespawn + randomGenerator.NextInt(1, 1 + spawnPointGroup.spawnPoints.Count / 2)) % spawnPointGroup.spawnPoints.Count;
		}
		else if (count > 1)
		{
			this.rotatingIndexForRespawn = (this.rotatingIndexForRespawn + 1) % count;
		}
		else
		{
			this.rotatingIndexForRespawn = 0;
		}
		GREntitySpawnPoint grentitySpawnPoint = spawnPointGroup.spawnPoints[this.rotatingIndexForRespawn];
		GhostReactor.EnemyEntityCreateData enemyEntityCreateData = GhostReactor.EnemyEntityCreateData.Unpack(entityCreateData);
		enemyEntityCreateData.patrolIndex = ((grentitySpawnPoint.patrolPath != null) ? grentitySpawnPoint.patrolPath.index : 255);
		long num = enemyEntityCreateData.Pack();
		gameEntityManager.RequestCreateItem(entityId, grentitySpawnPoint.transform.position, grentitySpawnPoint.transform.rotation, num, createdByEntityId);
	}

	// Token: 0x06002DB0 RID: 11696 RVA: 0x000F874C File Offset: 0x000F694C
	public GRPatrolPath GetPatrolPath(int patrolPathIndex)
	{
		if (patrolPathIndex >= 0 && patrolPathIndex < this.patrolPaths.Count)
		{
			return this.patrolPaths[patrolPathIndex];
		}
		return null;
	}

	// Token: 0x06002DB1 RID: 11697 RVA: 0x000F8770 File Offset: 0x000F6970
	public void Hide(bool hide)
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (!(this.renderers[i] == null))
			{
				this.renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x06002DB2 RID: 11698 RVA: 0x000F87BC File Offset: 0x000F69BC
	public void UpdateDisable(Vector3 playerPos)
	{
		if (this.boundingCollider == null)
		{
			return;
		}
		float distSq = this.GetDistSq(playerPos);
		float num = 1024f;
		float num2 = 1296f;
		if (this.hidden && distSq < num)
		{
			this.hidden = false;
			this.Hide(false);
			return;
		}
		if (!this.hidden && distSq > num2)
		{
			this.hidden = true;
			this.Hide(true);
		}
	}

	// Token: 0x06002DB3 RID: 11699 RVA: 0x000F8824 File Offset: 0x000F6A24
	public float GetDistSq(Vector3 pos)
	{
		return (this.boundingCollider.ClosestPoint(pos) - pos).sqrMagnitude;
	}

	// Token: 0x06002DB4 RID: 11700 RVA: 0x000F884B File Offset: 0x000F6A4B
	public Transform GetAnchor(int anchorIndex)
	{
		return this.anchors[anchorIndex];
	}

	// Token: 0x04003A77 RID: 14967
	private const float SHOW_DIST = 32f;

	// Token: 0x04003A78 RID: 14968
	private const float HIDE_DIST = 36f;

	// Token: 0x04003A79 RID: 14969
	private const int MAX_CREATE_PER_RPC = 25;

	// Token: 0x04003A7A RID: 14970
	[SerializeField]
	private GhostReactorLevelSection.SectionType sectionType;

	// Token: 0x04003A7B RID: 14971
	[SerializeField]
	[Tooltip("Single Anchor Transform used for End Caps and Blockers")]
	private Transform anchorTransform;

	// Token: 0x04003A7C RID: 14972
	[SerializeField]
	[Tooltip("A List of Anchors used as in and out connections for Hubs")]
	private List<Transform> anchors = new List<Transform>();

	// Token: 0x04003A7D RID: 14973
	[SerializeField]
	private List<GhostReactorLevelSection.SpawnPointGroup> spawnPointGroups;

	// Token: 0x04003A7E RID: 14974
	[SerializeField]
	private List<GhostReactorSpawnConfig> spawnConfigs;

	// Token: 0x04003A7F RID: 14975
	[SerializeField]
	private List<GRPatrolPath> patrolPaths;

	// Token: 0x04003A80 RID: 14976
	[SerializeField]
	private BoxCollider boundingCollider;

	// Token: 0x04003A81 RID: 14977
	private List<Renderer> renderers;

	// Token: 0x04003A82 RID: 14978
	private bool hidden;

	// Token: 0x04003A83 RID: 14979
	private List<GRHazardousMaterial> hazardousMaterials;

	// Token: 0x04003A84 RID: 14980
	[HideInInspector]
	public GhostReactorLevelSectionConnector sectionConnector;

	// Token: 0x04003A85 RID: 14981
	[HideInInspector]
	public int hubAnchorIndex;

	// Token: 0x04003A86 RID: 14982
	private int index;

	// Token: 0x04003A87 RID: 14983
	private GhostReactorLevelSection.SpawnPointGroup[] spawnPointGroupLookup;

	// Token: 0x04003A88 RID: 14984
	private List<GameEntity> prePlacedGameEntities;

	// Token: 0x04003A89 RID: 14985
	public static List<GameEntityCreateData> tempCreateEntitiesList = new List<GameEntityCreateData>(32);

	// Token: 0x04003A8A RID: 14986
	private int rotatingIndexForRespawn;

	// Token: 0x02000716 RID: 1814
	public enum SectionType
	{
		// Token: 0x04003A8C RID: 14988
		Hub,
		// Token: 0x04003A8D RID: 14989
		EndCap,
		// Token: 0x04003A8E RID: 14990
		Blocker
	}

	// Token: 0x02000717 RID: 1815
	[Serializable]
	public class SpawnPointGroup
	{
		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06002DB7 RID: 11703 RVA: 0x000F887A File Offset: 0x000F6A7A
		// (set) Token: 0x06002DB8 RID: 11704 RVA: 0x000F8882 File Offset: 0x000F6A82
		public bool NeedsRandomization
		{
			get
			{
				return this.needsRandomization;
			}
			set
			{
				this.needsRandomization = value;
			}
		}

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06002DB9 RID: 11705 RVA: 0x000F888B File Offset: 0x000F6A8B
		// (set) Token: 0x06002DBA RID: 11706 RVA: 0x000F8893 File Offset: 0x000F6A93
		public int CurrentIndex
		{
			get
			{
				return this.currentIndex;
			}
			set
			{
				this.currentIndex = value;
			}
		}

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06002DBB RID: 11707 RVA: 0x000F889C File Offset: 0x000F6A9C
		// (set) Token: 0x06002DBC RID: 11708 RVA: 0x000F88A4 File Offset: 0x000F6AA4
		public List<int> SpawnPointIndexes
		{
			get
			{
				return this.spawnPointIndexes;
			}
			set
			{
				this.spawnPointIndexes = value;
			}
		}

		// Token: 0x06002DBD RID: 11709 RVA: 0x000F88AD File Offset: 0x000F6AAD
		public GREntitySpawnPoint GetNextSpawnPoint()
		{
			GREntitySpawnPoint grentitySpawnPoint = this.spawnPoints[this.spawnPointIndexes[this.currentIndex]];
			this.currentIndex = (this.currentIndex + 1) % this.spawnPointIndexes.Count;
			return grentitySpawnPoint;
		}

		// Token: 0x04003A8F RID: 14991
		public GhostReactorSpawnConfig.SpawnPointType type;

		// Token: 0x04003A90 RID: 14992
		public List<GREntitySpawnPoint> spawnPoints;

		// Token: 0x04003A91 RID: 14993
		private List<int> spawnPointIndexes;

		// Token: 0x04003A92 RID: 14994
		private bool needsRandomization;

		// Token: 0x04003A93 RID: 14995
		private int currentIndex;
	}
}
