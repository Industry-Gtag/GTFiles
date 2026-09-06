using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

// Token: 0x02000127 RID: 295
public class GadgetNode : TechTreeNodeBase
{
	// Token: 0x1700007D RID: 125
	// (get) Token: 0x0600074D RID: 1869 RVA: 0x0002959F File Offset: 0x0002779F
	private static bool InEditor
	{
		get
		{
			return NodeInspectorBridge.InNodeEditor;
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x0600074E RID: 1870 RVA: 0x000295A8 File Offset: 0x000277A8
	public bool IsValid
	{
		get
		{
			EAssetReleaseTier eassetReleaseTier = this.releaseTier;
			return eassetReleaseTier != EAssetReleaseTier.Disabled && eassetReleaseTier <= EAssetReleaseTier.PublicRC;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x0600074F RID: 1871 RVA: 0x000295C8 File Offset: 0x000277C8
	public bool IsDispensableGadget
	{
		get
		{
			return this.unlockedGadgetPrefab;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000750 RID: 1872 RVA: 0x000295D5 File Offset: 0x000277D5
	private bool ShowGadgetPrefab
	{
		get
		{
			return !GadgetNode.InEditor || this.IsDispensableGadget;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000751 RID: 1873 RVA: 0x000295E6 File Offset: 0x000277E6
	private bool ShowExcludedGameModes
	{
		get
		{
			return !GadgetNode.InEditor || this.excludedGameModes > (ESuperGameModes)0;
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000752 RID: 1874 RVA: 0x000295FA File Offset: 0x000277FA
	private bool ShowReleaseTier
	{
		get
		{
			return !GadgetNode.InEditor || this.releaseTier != EAssetReleaseTier.PublicRC;
		}
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x00029614 File Offset: 0x00027814
	public void ConfigureFrom(SITechTreeNode sourceNode)
	{
		this.releaseTier = sourceNode.EdReleaseTier;
		this.upgradeType = sourceNode.upgradeType;
		this.nickName = sourceNode.nickName;
		this.description = sourceNode.description;
		this.unlockedGadgetPrefab = sourceNode.unlockedGadgetPrefab;
		this.excludedGameModes = sourceNode.excludedGameModes;
		this.nodeCost = sourceNode.nodeCost.ToArray<SIResource.ResourceCost>();
		this.costOverride = sourceNode.costOverride;
		base.name = this.nickName;
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00029694 File Offset: 0x00027894
	public void AssignParentUpgrades(SIUpgradeType[] prerequisites)
	{
		NodePort port = base.GetPort("input");
		port.ClearConnections();
		for (int i = 0; i < prerequisites.Length; i++)
		{
			SIUpgradeType id = prerequisites[i];
			GadgetNode gadgetNode = this.graph.nodes.FirstOrDefault(delegate(Node n)
			{
				GadgetNode gadgetNode2 = n as GadgetNode;
				return gadgetNode2 != null && gadgetNode2.upgradeType == id;
			}) as GadgetNode;
			if (gadgetNode != null)
			{
				NodePort port2 = gadgetNode.GetPort("output");
				port.Connect(port2);
			}
		}
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00029718 File Offset: 0x00027918
	public List<SIUpgradeType> GetParentUpgradeTypes()
	{
		List<SIUpgradeType> list = new List<SIUpgradeType>();
		foreach (Node node in from n in base.GetPort("input").GetConnections()
			select n.node)
		{
			GadgetNode gadgetNode = node as GadgetNode;
			if (gadgetNode != null)
			{
				list.Add(gadgetNode.upgradeType);
			}
		}
		return list;
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x000297A8 File Offset: 0x000279A8
	public SITechTreeNode GenerateTechTreeNode()
	{
		return new SITechTreeNode
		{
			upgradeType = this.upgradeType,
			nickName = this.nickName,
			description = this.description,
			unlockedGadgetPrefab = this.unlockedGadgetPrefab,
			nodeCost = this.nodeCost.ToArray<SIResource.ResourceCost>(),
			excludedGameModes = this.excludedGameModes,
			EdReleaseTier = this.releaseTier,
			parentUpgrades = this.GetParentUpgradeTypes().ToArray()
		};
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x00029824 File Offset: 0x00027A24
	public int GetDepth()
	{
		int num = 0;
		NodePort inputPort = base.GetInputPort("input");
		IEnumerable<NodePort> enumerable = ((inputPort != null) ? inputPort.GetConnections() : null);
		foreach (NodePort nodePort in (enumerable ?? Enumerable.Empty<NodePort>()))
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				num = Mathf.Max(num, gadgetNode.GetDepth() + 1);
			}
		}
		return num;
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x000298A8 File Offset: 0x00027AA8
	public int GetTreeDepth()
	{
		int num = this.GetDepth();
		foreach (NodePort nodePort in base.GetOutputPort("output").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				num = Mathf.Max(num, gadgetNode.GetTreeDepth());
			}
		}
		return num;
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00029920 File Offset: 0x00027B20
	public List<GadgetNode> GetTreeNodes()
	{
		List<GadgetNode> list = new List<GadgetNode>();
		this.GetTreeNodes(list);
		return list;
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x0002993C File Offset: 0x00027B3C
	public void GetTreeNodes(List<GadgetNode> nodes)
	{
		nodes.Add(this);
		foreach (NodePort nodePort in base.GetOutputPort("output").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				gadgetNode.GetTreeNodes(nodes);
			}
		}
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x000299B0 File Offset: 0x00027BB0
	public List<GadgetNode> GetParentNodes()
	{
		List<GadgetNode> list = new List<GadgetNode>();
		foreach (NodePort nodePort in base.GetPort("input").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				list.Add(gadgetNode);
			}
		}
		return list;
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00029A24 File Offset: 0x00027C24
	public List<GadgetNode> GetChildNodes()
	{
		List<GadgetNode> list = new List<GadgetNode>();
		foreach (NodePort nodePort in base.GetOutputPort("output").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				list.Add(gadgetNode);
			}
		}
		return list;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x00029A98 File Offset: 0x00027C98
	public int GetTreeWidth()
	{
		List<GadgetNode> childNodes = this.GetChildNodes();
		if (childNodes.Count == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GadgetNode gadgetNode in childNodes)
		{
			num += gadgetNode.GetTreeWidth();
		}
		return num;
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00029AFC File Offset: 0x00027CFC
	public bool CostEquals(SIResource.ResourceCost[] cost)
	{
		if (cost.Length != this.nodeCost.Length)
		{
			return false;
		}
		for (int i = 0; i < cost.Length; i++)
		{
			if (!cost[i].Equals(this.nodeCost[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400098D RID: 2445
	[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
	public TechTreeNodeBase.Empty input;

	// Token: 0x0400098E RID: 2446
	[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
	public TechTreeNodeBase.Empty output;

	// Token: 0x0400098F RID: 2447
	public SIUpgradeType upgradeType;

	// Token: 0x04000990 RID: 2448
	public string nickName;

	// Token: 0x04000991 RID: 2449
	[TextArea]
	public string description;

	// Token: 0x04000992 RID: 2450
	public SIResource.ResourceCost[] nodeCost;

	// Token: 0x04000993 RID: 2451
	public bool costOverride;

	// Token: 0x04000994 RID: 2452
	[Header("Prefab")]
	public GameEntity unlockedGadgetPrefab;

	// Token: 0x04000995 RID: 2453
	public ESuperGameModes excludedGameModes;

	// Token: 0x04000996 RID: 2454
	public EAssetReleaseTier releaseTier = (EAssetReleaseTier)(-1);
}
