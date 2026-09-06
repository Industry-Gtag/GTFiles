using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000712 RID: 1810
public class GhostReactorLevelGenerator : MonoBehaviourTick
{
	// Token: 0x1700046C RID: 1132
	// (get) Token: 0x06002D94 RID: 11668 RVA: 0x000F66D3 File Offset: 0x000F48D3
	public List<GhostReactorLevelGeneratorV2.TreeLevelConfig> TreeLevels
	{
		get
		{
			return this.GetTreeLevels();
		}
	}

	// Token: 0x06002D95 RID: 11669 RVA: 0x000F66DC File Offset: 0x000F48DC
	private void Awake()
	{
		GameObject gameObject = new GameObject("TestColliderA");
		this.testColliderA = gameObject.AddComponent<BoxCollider>();
		this.testColliderA.isTrigger = true;
		gameObject.transform.SetParent(base.transform);
		gameObject.gameObject.SetActive(false);
		GameObject gameObject2 = new GameObject("TestColliderB");
		this.testColliderB = gameObject2.AddComponent<BoxCollider>();
		this.testColliderB.isTrigger = true;
		gameObject2.transform.SetParent(base.transform);
		gameObject2.gameObject.SetActive(false);
		this.nextVisCheckNodeIndex = 0;
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x000F6770 File Offset: 0x000F4970
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06002D97 RID: 11671 RVA: 0x000F677C File Offset: 0x000F497C
	public override void Tick()
	{
		Vector3 position = VRRig.LocalRig.transform.position;
		int num = Mathf.Min(1, this.nodeList.Count);
		for (int i = 0; i < num; i++)
		{
			if (this.nextVisCheckNodeIndex >= this.nodeList.Count)
			{
				this.nextVisCheckNodeIndex = 0;
			}
			if (this.nodeList[this.nextVisCheckNodeIndex] != null)
			{
				if (this.nodeList[this.nextVisCheckNodeIndex].sectionInstance != null)
				{
					this.nodeList[this.nextVisCheckNodeIndex].sectionInstance.UpdateDisable(position);
				}
				if (this.nodeList[this.nextVisCheckNodeIndex].connectorInstance != null)
				{
					this.nodeList[this.nextVisCheckNodeIndex].connectorInstance.UpdateDisable(position);
				}
				GhostReactorLevelGenerator.Node[] children = this.nodeList[this.nextVisCheckNodeIndex].children;
				for (int j = 0; j < children.Length; j++)
				{
					if (children[j] != null)
					{
						if (children[j].sectionInstance != null)
						{
							children[j].sectionInstance.UpdateDisable(position);
						}
						if (children[j].connectorInstance != null)
						{
							children[j].connectorInstance.UpdateDisable(position);
						}
					}
				}
				this.nextVisCheckNodeIndex++;
			}
		}
	}

	// Token: 0x06002D98 RID: 11672 RVA: 0x000F68E0 File Offset: 0x000F4AE0
	private List<GhostReactorLevelGeneratorV2.TreeLevelConfig> GetTreeLevels()
	{
		if (this.depthConfigs == null || this.depthConfigs.Count == 0)
		{
			return null;
		}
		List<GhostReactorLevelGeneratorV2.TreeLevelConfig> treeLevels = this.depthConfigs[Mathf.Clamp(this.reactor.GetDepthLevel(), 0, this.depthConfigs.Count - 1)].options[this.reactor.GetDepthConfigIndex()].levelConfig.treeLevels;
		List<GhostReactorLevelGeneratorV2.TreeLevelConfig> list = new List<GhostReactorLevelGeneratorV2.TreeLevelConfig>();
		foreach (GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevelConfig in treeLevels)
		{
			if (GhostReactorLevelGenerator.TreeLevelIsEnabledNow(treeLevelConfig))
			{
				list.Add(treeLevelConfig);
			}
		}
		return list;
	}

	// Token: 0x06002D99 RID: 11673 RVA: 0x000F699C File Offset: 0x000F4B9C
	private static bool TreeLevelIsEnabledNow(GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevel)
	{
		if (string.IsNullOrEmpty(treeLevel.EnableAfterDatetime) && string.IsNullOrEmpty(treeLevel.DisableAfterDatetime))
		{
			return true;
		}
		if (!string.IsNullOrEmpty(treeLevel.EnableAfterDatetime) && !string.IsNullOrEmpty(treeLevel.DisableAfterDatetime))
		{
			throw new ArgumentException("Both enable and disable after datetime are set--this should never happen!");
		}
		DateTime dateTime = GorillaComputer.instance.GetServerTime().ToUniversalTime();
		if (!string.IsNullOrEmpty(treeLevel.EnableAfterDatetime))
		{
			DateTime dateTime2 = DateTime.Parse(treeLevel.EnableAfterDatetime).ToUniversalTime();
			return dateTime > dateTime2;
		}
		DateTime dateTime3 = DateTime.Parse(treeLevel.DisableAfterDatetime).ToUniversalTime();
		return dateTime < dateTime3;
	}

	// Token: 0x06002D9A RID: 11674 RVA: 0x000F6A44 File Offset: 0x000F4C44
	private bool TestForCollision(GhostReactorLevelSection section, Vector3 position, Quaternion rotation, int selfi, int selfj, int selfk)
	{
		this.testColliderA.gameObject.SetActive(true);
		this.testColliderB.gameObject.SetActive(true);
		this.testColliderA.transform.position = position + rotation * section.BoundingCollider.transform.localPosition;
		this.testColliderA.transform.rotation = rotation * section.BoundingCollider.transform.localRotation;
		this.testColliderA.transform.localScale = section.BoundingCollider.transform.localScale;
		this.testColliderA.size = section.BoundingCollider.size;
		this.testColliderA.center = section.BoundingCollider.center;
		for (int i = 0; i < this.nonOverlapZones.Count; i++)
		{
			Vector3 vector;
			float num;
			if (this.testColliderA.bounds.Intersects(this.nonOverlapZones[i].bounds) && Physics.ComputePenetration(this.testColliderA, this.testColliderA.transform.position, this.testColliderA.transform.rotation, this.nonOverlapZones[i], this.nonOverlapZones[i].transform.position, this.nonOverlapZones[i].transform.rotation, out vector, out num))
			{
				this.testColliderA.gameObject.SetActive(false);
				this.testColliderB.gameObject.SetActive(false);
				return true;
			}
		}
		for (int j = 0; j < this.nodeTree.Count; j++)
		{
			for (int k = 0; k < this.nodeTree[j].Count; k++)
			{
				if (j != selfi || k != selfj || selfk != -1)
				{
					GhostReactorLevelGenerator.Node node = this.nodeTree[j][k];
					for (int l = 0; l < node.children.Length; l++)
					{
						if (j != selfi || k != selfj || l != selfk)
						{
							GhostReactorLevelGenerator.Node node2 = node.children[l];
							if (node2 != null && node2.sectionInstance != null && node2.sectionInstance.BoundingCollider != null && (node2.type == GhostReactorLevelGenerator.NodeType.Blocker || node2.type == GhostReactorLevelGenerator.NodeType.EndCap))
							{
								GhostReactorLevelSection sectionInstance = node2.sectionInstance;
								this.testColliderB.transform.position = sectionInstance.transform.position + sectionInstance.transform.rotation * sectionInstance.BoundingCollider.transform.localPosition;
								this.testColliderB.transform.rotation = sectionInstance.transform.rotation * sectionInstance.BoundingCollider.transform.localRotation;
								this.testColliderB.transform.localScale = sectionInstance.BoundingCollider.transform.localScale;
								this.testColliderB.size = sectionInstance.BoundingCollider.size;
								this.testColliderB.center = sectionInstance.BoundingCollider.center;
								Vector3 vector2;
								float num2;
								if (this.testColliderA.bounds.Intersects(this.testColliderB.bounds) && Physics.ComputePenetration(this.testColliderA, this.testColliderA.transform.position, this.testColliderA.transform.rotation, this.testColliderB, this.testColliderB.transform.position, this.testColliderB.transform.rotation, out vector2, out num2))
								{
									this.testColliderA.gameObject.SetActive(false);
									this.testColliderB.gameObject.SetActive(false);
									return true;
								}
							}
						}
					}
					if ((j != selfi || k != selfj) && node.sectionInstance != null && node.sectionInstance.BoundingCollider != null)
					{
						GhostReactorLevelSection sectionInstance2 = node.sectionInstance;
						this.testColliderB.transform.position = sectionInstance2.transform.position + sectionInstance2.transform.rotation * sectionInstance2.BoundingCollider.transform.localPosition;
						this.testColliderB.transform.rotation = sectionInstance2.transform.rotation * sectionInstance2.BoundingCollider.transform.localRotation;
						this.testColliderB.transform.localScale = sectionInstance2.BoundingCollider.transform.localScale;
						this.testColliderB.size = sectionInstance2.BoundingCollider.size;
						this.testColliderB.center = sectionInstance2.BoundingCollider.center;
						Vector3 vector3;
						float num3;
						if (this.testColliderA.bounds.Intersects(this.testColliderB.bounds) && Physics.ComputePenetration(this.testColliderA, this.testColliderA.transform.position, this.testColliderA.transform.rotation, this.testColliderB, this.testColliderB.transform.position, this.testColliderB.transform.rotation, out vector3, out num3))
						{
							this.testColliderA.gameObject.SetActive(false);
							this.testColliderB.gameObject.SetActive(false);
							return true;
						}
					}
				}
			}
		}
		this.testColliderA.gameObject.SetActive(false);
		this.testColliderB.gameObject.SetActive(false);
		return false;
	}

	// Token: 0x06002D9B RID: 11675 RVA: 0x000F7005 File Offset: 0x000F5205
	private void DebugGenerate()
	{
		this.Generate(this.seed);
	}

	// Token: 0x06002D9C RID: 11676 RVA: 0x000F7014 File Offset: 0x000F5214
	public void Generate(int inputSeed)
	{
		this.ClearLevelSections();
		if (!Application.isPlaying)
		{
			return;
		}
		this.seed = inputSeed;
		this.randomGenerator = new SRand(this.seed);
		if (this.TreeLevels.Count < 1)
		{
			return;
		}
		this.spawnedHubHashSet.Clear();
		for (int i = 0; i < this.TreeLevels.Count; i++)
		{
			this.nodeTree.Add(new List<GhostReactorLevelGenerator.Node>());
			GameObject gameObject = new GameObject(string.Format("Tree Level {0}", i));
			gameObject.transform.parent = base.transform;
			gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			this.treeParents.Add(gameObject.transform);
		}
		GhostReactorLevelGenerator.Node node = new GhostReactorLevelGenerator.Node();
		node.type = GhostReactorLevelGenerator.NodeType.Hub;
		node.configIndex = -1;
		node.attachAnchorIndex = -1;
		node.parentAnchorIndex = -1;
		node.children = new GhostReactorLevelGenerator.Node[this.mainHub.Anchors.Count];
		node.sectionInstance = this.mainHub;
		node.anchorCount = this.mainHub.Anchors.Count;
		node.anchorOrder = new List<int>();
		this.RandomizeIndices(ref node.anchorOrder, node.anchorCount);
		this.nodeTree[0].Add(node);
		this.nodeList.Add(node);
		for (int j = 0; j < this.TreeLevels.Count; j++)
		{
			List<GhostReactorLevelSection> hubs = this.TreeLevels[j].hubs;
			List<GhostReactorLevelSectionConnector> connectors = this.TreeLevels[j].connectors;
			if (hubs.Count >= 1 && connectors.Count >= 1)
			{
				this.RandomizeIndices(ref this.hubOrder, hubs.Count);
				this.RandomizeIndices(ref this.connectorOrder, connectors.Count);
				int num = 0;
				int num2 = 0;
				int num3 = Mathf.Max(this.TreeLevels[j].maxHubs - this.TreeLevels[j].minHubs, 0);
				int num4 = Mathf.Max(this.TreeLevels[j].minHubs, 0) + this.randomGenerator.NextInt(num3 + 1);
				for (int k = 0; k < num4; k++)
				{
					if (j <= 0 || this.nodeTree[j].Count >= 1)
					{
						int num5 = this.hubOrder[num % this.hubOrder.Count];
						num++;
						int num6 = this.connectorOrder[num2 % this.connectorOrder.Count];
						num2++;
						int num7 = ((j == 0) ? (-1) : (k % this.nodeTree[j].Count));
						GhostReactorLevelGenerator.Node node2 = ((num7 != -1) ? this.nodeTree[j][num7] : node);
						for (int l = 0; l < node2.anchorOrder.Count; l++)
						{
							int num8 = node2.anchorOrder[l];
							bool flag = this.spawnedHubHashSet.Contains(hubs[num5].gameObject.name);
							if (node2.children[num8] == null && node2.attachAnchorIndex != num8 && !flag)
							{
								Quaternion quaternion = node2.sectionInstance.Anchors[num8].rotation * this.flip180;
								Vector3 position = node2.sectionInstance.Anchors[num8].position;
								GhostReactorLevelSectionConnector ghostReactorLevelSectionConnector = connectors[num6];
								Quaternion quaternion2 = Quaternion.Inverse(ghostReactorLevelSectionConnector.hubAnchor.localRotation) * quaternion;
								Vector3 vector = quaternion2 * -ghostReactorLevelSectionConnector.hubAnchor.localPosition + position;
								Vector3 vector2 = quaternion2 * ghostReactorLevelSectionConnector.sectionAnchor.localPosition + vector;
								Quaternion quaternion3 = quaternion2 * ghostReactorLevelSectionConnector.sectionAnchor.localRotation;
								GhostReactorLevelSection ghostReactorLevelSection = hubs[num5];
								bool flag2 = false;
								if (ghostReactorLevelSection.Anchors.Count > 0)
								{
									this.RandomizeIndices(ref this.entryAnchorOrder, ghostReactorLevelSection.Anchors.Count);
									for (int m = 0; m < this.entryAnchorOrder.Count; m++)
									{
										int num9 = this.entryAnchorOrder[m];
										Transform transform = ghostReactorLevelSection.Anchors[num9];
										Quaternion quaternion4 = Quaternion.Inverse(transform.localRotation) * quaternion3;
										Vector3 vector3 = quaternion4 * -transform.localPosition + vector2;
										if (!this.TestForCollision(ghostReactorLevelSection, vector3, quaternion4, j, k, num8))
										{
											GhostReactorLevelGenerator.Node node3 = new GhostReactorLevelGenerator.Node();
											node3.type = GhostReactorLevelGenerator.NodeType.Hub;
											node3.configIndex = num5;
											node3.children = new GhostReactorLevelGenerator.Node[ghostReactorLevelSection.Anchors.Count];
											node3.parentAnchorIndex = num8;
											node3.attachAnchorIndex = num9;
											node3.anchorCount = ghostReactorLevelSection.Anchors.Count;
											node3.anchorOrder = new List<int>();
											this.RandomizeIndices(ref node3.anchorOrder, node3.anchorCount);
											GhostReactorLevelSectionConnector component = Object.Instantiate<GameObject>(ghostReactorLevelSectionConnector.gameObject, vector, quaternion2, this.treeParents[j]).GetComponent<GhostReactorLevelSectionConnector>();
											node3.connectorInstance = component;
											GhostReactorLevelSection component2 = Object.Instantiate<GameObject>(ghostReactorLevelSection.gameObject, vector3, quaternion4, this.treeParents[j]).GetComponent<GhostReactorLevelSection>();
											node3.sectionInstance = component2;
											node2.children[node3.parentAnchorIndex] = node3;
											this.nodeTree[j + 1].Add(node3);
											this.nodeList.Add(node3);
											this.spawnedHubHashSet.Add(ghostReactorLevelSection.gameObject.name);
											flag2 = true;
											break;
										}
									}
								}
								if (flag2)
								{
									break;
								}
							}
						}
					}
				}
			}
		}
		for (int n = 0; n < this.nodeTree.Count; n++)
		{
			List<GhostReactorLevelSection> endCaps = this.TreeLevels[n].endCaps;
			List<GhostReactorLevelSection> blockers = this.TreeLevels[n].blockers;
			this.RandomizeIndices(ref this.blockerOrder, blockers.Count);
			this.RandomizeIndices(ref this.endCapOrder, endCaps.Count);
			int num10 = 0;
			int num11 = 0;
			for (int num12 = 0; num12 < this.nodeTree[n].Count; num12++)
			{
				GhostReactorLevelGenerator.Node node4 = this.nodeTree[n][num12];
				int num13 = Mathf.Max(this.TreeLevels[n].maxCaps - this.TreeLevels[n].minCaps, 0);
				int num14 = Mathf.Max(this.TreeLevels[n].minCaps, 0) + this.randomGenerator.NextInt(num13 + 1);
				for (int num15 = 0; num15 < node4.children.Length; num15++)
				{
					if (node4.children[num15] == null && node4.attachAnchorIndex != num15)
					{
						bool flag3 = false;
						if (num14 > 0 && this.endCapOrder.Count > 0)
						{
							int num16 = this.endCapOrder[num11 % this.endCapOrder.Count];
							num11++;
							num14--;
							Quaternion quaternion5 = node4.sectionInstance.Anchors[num15].rotation * this.flip180;
							Vector3 position2 = node4.sectionInstance.Anchors[num15].position;
							GhostReactorLevelSection ghostReactorLevelSection2 = endCaps[num16];
							Quaternion quaternion6 = Quaternion.Inverse(ghostReactorLevelSection2.Anchor.localRotation) * quaternion5;
							Vector3 vector4 = quaternion6 * -ghostReactorLevelSection2.Anchor.localPosition + position2;
							if (!this.TestForCollision(ghostReactorLevelSection2, vector4, quaternion6, n, num12, num15))
							{
								GhostReactorLevelGenerator.Node node5 = new GhostReactorLevelGenerator.Node();
								node5.type = GhostReactorLevelGenerator.NodeType.EndCap;
								node5.configIndex = num16;
								node5.parentAnchorIndex = num15;
								GhostReactorLevelSection component3 = Object.Instantiate<GameObject>(ghostReactorLevelSection2.gameObject, vector4, quaternion6, this.treeParents[n]).GetComponent<GhostReactorLevelSection>();
								node5.sectionInstance = component3;
								node4.children[num15] = node5;
								flag3 = true;
							}
						}
						if (!flag3 && this.blockerOrder.Count > 0)
						{
							int num17 = this.blockerOrder[num10 % this.blockerOrder.Count];
							num10++;
							GhostReactorLevelGenerator.Node node6 = new GhostReactorLevelGenerator.Node();
							node6.type = GhostReactorLevelGenerator.NodeType.Blocker;
							node6.configIndex = num17;
							node6.parentAnchorIndex = num15;
							Quaternion quaternion7 = node4.sectionInstance.Anchors[num15].rotation * this.flip180;
							Vector3 position3 = node4.sectionInstance.Anchors[num15].position;
							GhostReactorLevelSection ghostReactorLevelSection3 = blockers[node6.configIndex];
							Quaternion quaternion8 = Quaternion.Inverse(ghostReactorLevelSection3.Anchor.localRotation) * quaternion7;
							Vector3 vector5 = quaternion8 * -ghostReactorLevelSection3.Anchor.localPosition + position3;
							GhostReactorLevelSection component4 = Object.Instantiate<GameObject>(ghostReactorLevelSection3.gameObject, vector5, quaternion8, this.treeParents[n]).GetComponent<GhostReactorLevelSection>();
							node6.sectionInstance = component4;
							node4.children[num15] = node6;
						}
					}
				}
			}
		}
		for (int num18 = 0; num18 < this.nodeList.Count; num18++)
		{
			if (this.nodeList[num18].connectorInstance != null)
			{
				this.nodeList[num18].connectorInstance.Init(this.reactor.grManager);
			}
			this.nodeList[num18].sectionInstance.InitLevelSection(num18, this.reactor);
		}
	}

	// Token: 0x06002D9D RID: 11677 RVA: 0x000F7A0D File Offset: 0x000F5C0D
	private void DebugClear()
	{
		this.ClearLevelSections();
	}

	// Token: 0x06002D9E RID: 11678 RVA: 0x000F7A18 File Offset: 0x000F5C18
	public void ClearLevelSections()
	{
		for (int i = 0; i < this.nodeList.Count; i++)
		{
			if (!(this.nodeList[i].sectionInstance == this.mainHub))
			{
				if (this.nodeList[i].connectorInstance != null)
				{
					Object.Destroy(this.nodeList[i].connectorInstance.gameObject);
				}
				Object.Destroy(this.nodeList[i].sectionInstance.gameObject);
			}
		}
		this.nodeList.Clear();
		for (int j = 0; j < this.nodeTree.Count; j++)
		{
			this.nodeTree[j].Clear();
		}
		this.nodeTree.Clear();
		for (int k = 0; k < this.treeParents.Count; k++)
		{
			Object.Destroy(this.treeParents[k].gameObject);
		}
		this.treeParents.Clear();
	}

	// Token: 0x06002D9F RID: 11679 RVA: 0x000F7B1C File Offset: 0x000F5D1C
	public void SpawnEntitiesInEachSection(float respawnCount)
	{
		for (int i = 0; i < this.nodeTree.Count; i++)
		{
			List<GhostReactorSpawnConfig> list = ((i < 1) ? this.mainHubSpawnConfigs : this.TreeLevels[i - 1].sectionSpawnConfigs);
			List<GhostReactorSpawnConfig> endCapSpawnConfigs = this.TreeLevels[i].endCapSpawnConfigs;
			for (int j = 0; j < this.nodeTree[i].Count; j++)
			{
				GhostReactorLevelGenerator.Node node = this.nodeTree[i][j];
				if (node != null && node.sectionInstance != null && node.type == GhostReactorLevelGenerator.NodeType.Hub)
				{
					node.sectionInstance.SpawnSectionEntities(ref this.randomGenerator, this.reactor.grManager.gameEntityManager, this.reactor, list, respawnCount);
				}
				for (int k = 0; k < node.children.Length; k++)
				{
					GhostReactorLevelGenerator.Node node2 = node.children[k];
					if (node2 != null && node2.sectionInstance != null && node2.type == GhostReactorLevelGenerator.NodeType.EndCap)
					{
						node2.sectionInstance.SpawnSectionEntities(ref this.randomGenerator, this.reactor.grManager.gameEntityManager, this.reactor, endCapSpawnConfigs, respawnCount);
					}
				}
			}
		}
		if (GhostReactorLevelSection.tempCreateEntitiesList.Count > 0)
		{
			this.reactor.grManager.gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
			GhostReactorLevelSection.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x06002DA0 RID: 11680 RVA: 0x000F7C90 File Offset: 0x000F5E90
	public void RespawnEntity(int entityId, long entityCreateData, GameEntityId createdByEntityId)
	{
		int sectionIndex = GhostReactor.EnemyEntityCreateData.Unpack(entityCreateData).sectionIndex;
		if (sectionIndex >= 0 && sectionIndex < this.nodeList.Count)
		{
			this.nodeList[sectionIndex].sectionInstance.RespawnEntity(ref this.randomGenerator, this.reactor.grManager.gameEntityManager, entityId, entityCreateData, createdByEntityId);
		}
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x000F7CEC File Offset: 0x000F5EEC
	public GRPatrolPath GetPatrolPath(long createData)
	{
		GhostReactor.EnemyEntityCreateData enemyEntityCreateData = GhostReactor.EnemyEntityCreateData.Unpack(createData);
		int sectionIndex = enemyEntityCreateData.sectionIndex;
		int patrolIndex = enemyEntityCreateData.patrolIndex;
		if (sectionIndex < 0 || sectionIndex >= this.nodeList.Count)
		{
			return null;
		}
		return this.nodeList[sectionIndex].sectionInstance.GetPatrolPath(patrolIndex);
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x000F7D38 File Offset: 0x000F5F38
	private void RandomizeIndices(ref List<int> list, int count)
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		this.randomGenerator.Shuffle<int>(list);
	}

	// Token: 0x06002DA3 RID: 11683 RVA: 0x000F7D70 File Offset: 0x000F5F70
	public bool GetExitFromCurrentSection(Vector3 pos, out Vector3 exitPos, out Quaternion exitRot, List<Vector3> connectorCorners)
	{
		exitPos = Vector3.zero;
		exitRot = Quaternion.identity;
		GhostReactorLevelGenerator.Node currentNode = this.GetCurrentNode(pos);
		if (currentNode == null || currentNode.parentAnchorIndex < 0)
		{
			return false;
		}
		Transform anchor = currentNode.sectionInstance.GetAnchor(currentNode.attachAnchorIndex);
		exitPos = anchor.transform.position;
		exitRot = anchor.transform.rotation;
		GRLevelAnchor component = anchor.GetComponent<GRLevelAnchor>();
		if (component != null && component.navigablePoint != null)
		{
			exitPos = component.navigablePoint.position;
			exitRot = component.navigablePoint.rotation;
		}
		connectorCorners.Clear();
		if (currentNode.connectorInstance != null)
		{
			for (int i = 0; i < currentNode.connectorInstance.pathNodes.Count; i++)
			{
				connectorCorners.Add(currentNode.connectorInstance.pathNodes[i].position);
			}
		}
		return true;
	}

	// Token: 0x06002DA4 RID: 11684 RVA: 0x000F7E6C File Offset: 0x000F606C
	private GhostReactorLevelGenerator.Node GetCurrentNode(Vector3 pos)
	{
		float num = float.MaxValue;
		GhostReactorLevelGenerator.Node node = null;
		for (int i = 0; i < this.nodeTree.Count; i++)
		{
			List<GhostReactorLevelGenerator.Node> list = this.nodeTree[i];
			for (int j = 0; j < list.Count; j++)
			{
				GhostReactorLevelGenerator.Node node2 = list[j];
				if (!(node2.sectionInstance == null))
				{
					float distSq = node2.sectionInstance.GetDistSq(pos);
					if (distSq < num)
					{
						num = distSq;
						node = node2;
					}
				}
			}
		}
		return node;
	}

	// Token: 0x04003A53 RID: 14931
	public List<GhostReactorLevelDepthConfig> depthConfigs;

	// Token: 0x04003A54 RID: 14932
	[SerializeField]
	private GhostReactorLevelSection mainHub = new GhostReactorLevelSection();

	// Token: 0x04003A55 RID: 14933
	[SerializeField]
	private List<GhostReactorSpawnConfig> mainHubSpawnConfigs;

	// Token: 0x04003A56 RID: 14934
	[SerializeField]
	private List<Collider> nonOverlapZones = new List<Collider>();

	// Token: 0x04003A57 RID: 14935
	public int seed = 2343;

	// Token: 0x04003A58 RID: 14936
	private List<List<GhostReactorLevelGenerator.Node>> nodeTree = new List<List<GhostReactorLevelGenerator.Node>>();

	// Token: 0x04003A59 RID: 14937
	private List<GhostReactorLevelGenerator.Node> nodeList = new List<GhostReactorLevelGenerator.Node>();

	// Token: 0x04003A5A RID: 14938
	private HashSet<string> spawnedHubHashSet = new HashSet<string>();

	// Token: 0x04003A5B RID: 14939
	private List<int> hubOrder = new List<int>();

	// Token: 0x04003A5C RID: 14940
	private List<int> connectorOrder = new List<int>();

	// Token: 0x04003A5D RID: 14941
	private List<int> endCapOrder = new List<int>();

	// Token: 0x04003A5E RID: 14942
	private List<int> blockerOrder = new List<int>();

	// Token: 0x04003A5F RID: 14943
	private List<int> entryAnchorOrder = new List<int>();

	// Token: 0x04003A60 RID: 14944
	private List<Transform> treeParents = new List<Transform>();

	// Token: 0x04003A61 RID: 14945
	private string generationOutput = "";

	// Token: 0x04003A62 RID: 14946
	private SRand randomGenerator;

	// Token: 0x04003A63 RID: 14947
	private BoxCollider testColliderA;

	// Token: 0x04003A64 RID: 14948
	private BoxCollider testColliderB;

	// Token: 0x04003A65 RID: 14949
	private GhostReactor reactor;

	// Token: 0x04003A66 RID: 14950
	[NonSerialized]
	public int depthConfigIndex;

	// Token: 0x04003A67 RID: 14951
	private Quaternion flip180 = Quaternion.AngleAxis(180f, Vector3.up);

	// Token: 0x04003A68 RID: 14952
	private const int MAX_VIS_CHECKS_PER_FRAME = 1;

	// Token: 0x04003A69 RID: 14953
	public int nextVisCheckNodeIndex;

	// Token: 0x02000713 RID: 1811
	public enum NodeType
	{
		// Token: 0x04003A6B RID: 14955
		Hub,
		// Token: 0x04003A6C RID: 14956
		EndCap,
		// Token: 0x04003A6D RID: 14957
		Blocker
	}

	// Token: 0x02000714 RID: 1812
	public class Node
	{
		// Token: 0x04003A6E RID: 14958
		public GhostReactorLevelGenerator.NodeType type;

		// Token: 0x04003A6F RID: 14959
		public int configIndex;

		// Token: 0x04003A70 RID: 14960
		public int parentAnchorIndex;

		// Token: 0x04003A71 RID: 14961
		public int attachAnchorIndex;

		// Token: 0x04003A72 RID: 14962
		public int anchorCount;

		// Token: 0x04003A73 RID: 14963
		public List<int> anchorOrder;

		// Token: 0x04003A74 RID: 14964
		public GhostReactorLevelSection sectionInstance;

		// Token: 0x04003A75 RID: 14965
		public GhostReactorLevelSectionConnector connectorInstance;

		// Token: 0x04003A76 RID: 14966
		public GhostReactorLevelGenerator.Node[] children;
	}
}
