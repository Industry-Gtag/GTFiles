using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class SITechTreeSO : ScriptableObject
{
	// Token: 0x170000BE RID: 190
	// (get) Token: 0x0600095B RID: 2395 RVA: 0x0003260A File Offset: 0x0003080A
	// (set) Token: 0x0600095C RID: 2396 RVA: 0x00032612 File Offset: 0x00030812
	public List<SITechTreePage> TreePages { get; private set; }

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x0600095D RID: 2397 RVA: 0x0003261B File Offset: 0x0003081B
	// (set) Token: 0x0600095E RID: 2398 RVA: 0x00032623 File Offset: 0x00030823
	public int TreePageCount { get; private set; }

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x0600095F RID: 2399 RVA: 0x0003262C File Offset: 0x0003082C
	// (set) Token: 0x06000960 RID: 2400 RVA: 0x00032634 File Offset: 0x00030834
	public int[] TreeNodeCounts { get; private set; }

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06000961 RID: 2401 RVA: 0x0003263D File Offset: 0x0003083D
	// (set) Token: 0x06000962 RID: 2402 RVA: 0x00032645 File Offset: 0x00030845
	public List<GraphNode<SITechTreeNode>> AllNodes { get; private set; }

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06000963 RID: 2403 RVA: 0x0003264E File Offset: 0x0003084E
	// (set) Token: 0x06000964 RID: 2404 RVA: 0x00032656 File Offset: 0x00030856
	public bool Initialized { get; private set; }

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06000965 RID: 2405 RVA: 0x0003265F File Offset: 0x0003085F
	public List<GameEntity> SpawnableEntities
	{
		get
		{
			this.EnsureInitialized();
			return this._spawnableEntities;
		}
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x0003266D File Offset: 0x0003086D
	public bool TryGetNode(SIUpgradeType upgradeType, out GraphNode<SITechTreeNode> node)
	{
		return this._nodeLookup.TryGetValue(upgradeType, out node);
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x0003267C File Offset: 0x0003087C
	public bool TryGetUpgradeTypeByEntityTypeId(int entityTypeId, out SIUpgradeType upgradeType)
	{
		return this._upgradeTypeByEntityTypeId.TryGetValue(entityTypeId, out upgradeType);
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x0003268B File Offset: 0x0003088B
	public bool IsSpawnableEntityTypeId(int entityTypeId)
	{
		this.EnsureInitialized();
		return this._spawnableEntityTypeIds.Contains(entityTypeId);
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x000326A0 File Offset: 0x000308A0
	public bool IsValidPage(SITechTreePageId id)
	{
		foreach (SITechTreePage sitechTreePage in this.TreePages)
		{
			if (sitechTreePage.pageId == id && sitechTreePage.IsValid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x00032704 File Offset: 0x00030904
	public SITechTreePage GetTreePage(SITechTreePageId id)
	{
		SITechTreePage sitechTreePage;
		if (!this.TryGetTreePage(id, out sitechTreePage))
		{
			return null;
		}
		return sitechTreePage;
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x00032720 File Offset: 0x00030920
	public bool TryGetTreePage(SITechTreePageId id, out SITechTreePage treePage)
	{
		foreach (SITechTreePage sitechTreePage in this.TreePages)
		{
			if (sitechTreePage.pageId == id && sitechTreePage.IsValid)
			{
				treePage = sitechTreePage;
				return true;
			}
		}
		treePage = null;
		return false;
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x0003278C File Offset: 0x0003098C
	public bool IsValidNode(int pageId, int nodeId)
	{
		return this.IsValidNode(SIUpgradeTypeSystem.GetUpgradeType(pageId, nodeId));
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x0003279B File Offset: 0x0003099B
	public bool IsValidNode(SIUpgradeType upgradeType)
	{
		return this._nodeLookup.ContainsKey(upgradeType);
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x000327A9 File Offset: 0x000309A9
	public SITechTreeNode GetTreeNode(int pageId, int nodeId)
	{
		return this.GetTreeNode(SIUpgradeTypeSystem.GetUpgradeType(pageId, nodeId));
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x000327B8 File Offset: 0x000309B8
	public SITechTreeNode GetTreeNode(SIUpgradeType upgradeType)
	{
		GraphNode<SITechTreeNode> graphNode;
		if (this._nodeLookup.TryGetValue(upgradeType, out graphNode))
		{
			return graphNode.Value;
		}
		return null;
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x000327DD File Offset: 0x000309DD
	public void EnsureInitialized()
	{
		if (!this.Initialized)
		{
			this.InitTechTree();
		}
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x000327F0 File Offset: 0x000309F0
	private void InitTechTree()
	{
		Debug.Log("[SI] SITechTreeSO.InitTechTree");
		this.ClearTechTree();
		this.TreePages = new List<SITechTreePage>();
		this._spawnableEntities = new List<GameEntity>();
		int num = 0;
		foreach (SITechTreePage sitechTreePage in this.treePages)
		{
			if (sitechTreePage.IsValid)
			{
				sitechTreePage.BuildGraph();
				foreach (GraphNode<SITechTreeNode> graphNode in sitechTreePage.Roots)
				{
					foreach (GraphNode<SITechTreeNode> graphNode2 in graphNode.TraversePreOrder())
					{
						if (!this._nodeLookup.ContainsKey(graphNode2.Value.upgradeType))
						{
							this._nodeLookup.Add(graphNode2.Value.upgradeType, graphNode2);
						}
					}
				}
				foreach (SITechTreeNode sitechTreeNode in sitechTreePage.DispensableGadgets)
				{
					num++;
					this.AddSpawnableGadget(sitechTreeNode.unlockedGadgetPrefab);
				}
				if (sitechTreePage.Roots.Count > 0)
				{
					this.TreePages.Add(sitechTreePage);
				}
			}
		}
		if (this._upgradeTypeByEntityTypeId.IsCreated)
		{
			this._upgradeTypeByEntityTypeId.Clear();
		}
		else
		{
			this._upgradeTypeByEntityTypeId = new NativeHashMap<int, SIUpgradeType>(num, Allocator.Persistent);
		}
		foreach (SITechTreePage sitechTreePage2 in this.treePages)
		{
			if (sitechTreePage2.IsValid)
			{
				foreach (SITechTreeNode sitechTreeNode2 in sitechTreePage2.DispensableGadgets)
				{
					int staticHash = sitechTreeNode2.unlockedGadgetPrefab.gameObject.name.GetStaticHash();
					this._upgradeTypeByEntityTypeId.TryAdd(staticHash, sitechTreeNode2.upgradeType);
				}
			}
		}
		this.AllNodes = new List<GraphNode<SITechTreeNode>>(this._nodeLookup.Values);
		this.TreePageCount = ((SIUpgradeType[])Enum.GetValues(typeof(SIUpgradeType))).Select((SIUpgradeType v) => v.GetPageId()).Max() + 1;
		this.TreeNodeCounts = new int[this.TreePageCount];
		foreach (SIUpgradeType siupgradeType in (SIUpgradeType[])Enum.GetValues(typeof(SIUpgradeType)))
		{
			int pageId = siupgradeType.GetPageId();
			int nodeId = siupgradeType.GetNodeId();
			this.TreeNodeCounts[pageId] = Mathf.Max(this.TreeNodeCounts[pageId], nodeId + 1);
		}
		this.Initialized = true;
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x00032AE8 File Offset: 0x00030CE8
	private void AddSpawnableGadget(GameEntity entity)
	{
		this._spawnableEntities.Add(entity);
		this._spawnableEntityTypeIds.Add(entity.gameObject.name.GetStaticHash());
		IPrefabRequirements component = entity.GetComponent<IPrefabRequirements>();
		if (component != null)
		{
			foreach (GameEntity gameEntity in component.RequiredPrefabs)
			{
				this._spawnableEntities.Add(gameEntity);
				this._spawnableEntityTypeIds.Add(gameEntity.gameObject.name.GetStaticHash());
			}
		}
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x00032B88 File Offset: 0x00030D88
	private void ClearTechTree()
	{
		SITechTreePage[] array = this.treePages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ClearGraph();
		}
		this._nodeLookup.Clear();
		this._spawnableEntityTypeIds.Clear();
		if (this._upgradeTypeByEntityTypeId.IsCreated)
		{
			this._upgradeTypeByEntityTypeId.Dispose();
		}
		this.Initialized = false;
	}

	// Token: 0x04000B6D RID: 2925
	private const string preLog = "[SITechTreeSO]  ";

	// Token: 0x04000B6E RID: 2926
	private const string preErr = "[SITechTreeSO]  ERROR!!!  ";

	// Token: 0x04000B6F RID: 2927
	private const int RESOURCE_CAP = 20;

	// Token: 0x04000B70 RID: 2928
	[SerializeField]
	private SITechTreePage[] treePages;

	// Token: 0x04000B71 RID: 2929
	private readonly Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>> _nodeLookup = new Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>>();

	// Token: 0x04000B72 RID: 2930
	private NativeHashMap<int, SIUpgradeType> _upgradeTypeByEntityTypeId;

	// Token: 0x04000B73 RID: 2931
	private readonly HashSet<int> _spawnableEntityTypeIds = new HashSet<int>();

	// Token: 0x04000B79 RID: 2937
	private List<GameEntity> _spawnableEntities;
}
