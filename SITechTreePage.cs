using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000168 RID: 360
[Serializable]
public class SITechTreePage
{
	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x0600097E RID: 2430 RVA: 0x00032C97 File Offset: 0x00030E97
	// (set) Token: 0x0600097F RID: 2431 RVA: 0x00032C9F File Offset: 0x00030E9F
	public EAssetReleaseTier EdReleaseTier
	{
		get
		{
			return this.m_edReleaseTier;
		}
		set
		{
			this.m_edReleaseTier = value;
		}
	}

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x06000980 RID: 2432 RVA: 0x00032CA8 File Offset: 0x00030EA8
	public bool IsValid
	{
		get
		{
			EAssetReleaseTier edReleaseTier = this.m_edReleaseTier;
			if (edReleaseTier != EAssetReleaseTier.Disabled && edReleaseTier <= EAssetReleaseTier.PublicRC)
			{
				SITechTreeNode[] array = this.treeNodes;
				return array != null && array.Length != 0;
			}
			return false;
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x06000981 RID: 2433 RVA: 0x00032CD5 File Offset: 0x00030ED5
	public bool IsAllowed
	{
		get
		{
			return (this.excludedGameModes & (ESuperGameModes)GameMode.CurrentGameModeFlag) == (ESuperGameModes)0;
		}
	}

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x06000982 RID: 2434 RVA: 0x00032CE6 File Offset: 0x00030EE6
	// (set) Token: 0x06000983 RID: 2435 RVA: 0x00032CEE File Offset: 0x00030EEE
	public List<GraphNode<SITechTreeNode>> Roots { get; private set; }

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06000984 RID: 2436 RVA: 0x00032CF7 File Offset: 0x00030EF7
	// (set) Token: 0x06000985 RID: 2437 RVA: 0x00032CFF File Offset: 0x00030EFF
	public List<GraphNode<SITechTreeNode>> AllNodes { get; private set; }

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x06000986 RID: 2438 RVA: 0x00032D08 File Offset: 0x00030F08
	// (set) Token: 0x06000987 RID: 2439 RVA: 0x00032D10 File Offset: 0x00030F10
	public List<SITechTreeNode> DispensableGadgets { get; private set; }

	// Token: 0x06000988 RID: 2440 RVA: 0x00032D19 File Offset: 0x00030F19
	public void ClearGraph()
	{
		this.Roots = null;
		this.AllNodes = null;
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x00032D2C File Offset: 0x00030F2C
	public void BuildGraph()
	{
		SITechTreePage.<>c__DisplayClass27_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.Roots = new List<GraphNode<SITechTreeNode>>();
		this.AllNodes = new List<GraphNode<SITechTreeNode>>();
		this.DispensableGadgets = new List<SITechTreeNode>();
		if (!this.IsValid)
		{
			return;
		}
		CS$<>8__locals1.nodeLookup = new Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>>();
		foreach (SITechTreeNode sitechTreeNode in this.treeNodes)
		{
			if (sitechTreeNode.IsValid && (sitechTreeNode.parentUpgrades == null || sitechTreeNode.parentUpgrades.Length == 0))
			{
				this.Roots.Add(this.<BuildGraph>g__PopulateGraph|27_0(sitechTreeNode, this.excludedGameModes, ref CS$<>8__locals1));
			}
		}
		foreach (GraphNode<SITechTreeNode> graphNode in this.AllNodes)
		{
			if (graphNode.Value.IsDispensableGadget)
			{
				this.DispensableGadgets.Add(graphNode.Value);
			}
		}
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x00032E24 File Offset: 0x00031024
	public void PrintGraph()
	{
		foreach (GraphNode<SITechTreeNode> graphNode in this.Roots)
		{
			foreach (GraphNode<SITechTreeNode> graphNode2 in graphNode.TraversePreOrderDistinct(null))
			{
				Debug.Log(string.Concat(new string[]
				{
					"[SI] Graph node: ",
					graphNode2.Value.nickName,
					" [",
					SITechTreePage.<PrintGraph>g__NodeListText|28_0(graphNode2.Parents),
					"]"
				}));
			}
		}
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x00032F04 File Offset: 0x00031104
	[CompilerGenerated]
	private GraphNode<SITechTreeNode> <BuildGraph>g__PopulateGraph|27_0(SITechTreeNode node, ESuperGameModes parentExcludedGameModes, ref SITechTreePage.<>c__DisplayClass27_0 A_3)
	{
		node.excludedGameModes |= parentExcludedGameModes;
		GraphNode<SITechTreeNode> graphNode;
		if (!A_3.nodeLookup.TryGetValue(node.upgradeType, out graphNode))
		{
			graphNode = new GraphNode<SITechTreeNode>(node);
			A_3.nodeLookup.Add(node.upgradeType, graphNode);
			this.AllNodes.Add(graphNode);
		}
		SIUpgradeType upgradeType = node.upgradeType;
		foreach (SITechTreeNode sitechTreeNode in this.treeNodes)
		{
			if (sitechTreeNode.IsValid && sitechTreeNode.parentUpgrades != null)
			{
				SIUpgradeType[] parentUpgrades = sitechTreeNode.parentUpgrades;
				for (int j = 0; j < parentUpgrades.Length; j++)
				{
					if (parentUpgrades[j] == upgradeType)
					{
						GraphNode<SITechTreeNode> graphNode2 = this.<BuildGraph>g__PopulateGraph|27_0(sitechTreeNode, node.excludedGameModes, ref A_3);
						if (!graphNode.Children.Contains(graphNode2))
						{
							graphNode.AddChild(graphNode2);
						}
					}
				}
			}
		}
		return graphNode;
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x00032FDA File Offset: 0x000311DA
	[CompilerGenerated]
	internal static string <PrintGraph>g__NodeListText|28_0(List<GraphNode<SITechTreeNode>> nodes)
	{
		return string.Join("|", nodes.Select((GraphNode<SITechTreeNode> n) => n.Value.nickName));
	}

	// Token: 0x04000B85 RID: 2949
	[SerializeField]
	private EAssetReleaseTier m_edReleaseTier = (EAssetReleaseTier)(-1);

	// Token: 0x04000B86 RID: 2950
	public string nickName;

	// Token: 0x04000B87 RID: 2951
	public SITechTreePageId pageId;

	// Token: 0x04000B88 RID: 2952
	public Sprite icon;

	// Token: 0x04000B89 RID: 2953
	public ESuperGameModes excludedGameModes;

	// Token: 0x04000B8A RID: 2954
	[SerializeField]
	private SITechTreeNode[] treeNodes;

	// Token: 0x04000B8B RID: 2955
	public float costMultiplier = 1f;
}
