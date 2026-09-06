using System;
using System.Collections.Generic;
using GorillaNetworking;

// Token: 0x02000818 RID: 2072
public class GRToolProgressionTree
{
	// Token: 0x0600351E RID: 13598 RVA: 0x00123A9C File Offset: 0x00121C9C
	public GRToolProgressionTree()
	{
		this.InitializeToolMapping();
		this.InitializeClubPartMapping();
		this.InitializeFlashPartMapping();
		this.InitializeRevivePartMapping();
		this.InitializeCollectorPartMapping();
		this.InitializeLanternPartMapping();
		this.InitializeShieldGunPartMapping();
		this.InitializeDirectionalShieldPartMapping();
		this.InitializeEnergyEfficiencyPartMapping();
		this.InitializeDockWristPartMapping();
		this.InitializeDropPodPartMapping();
	}

	// Token: 0x0600351F RID: 13599 RVA: 0x00123B78 File Offset: 0x00121D78
	public void Init(GhostReactor ghostReactor, GRToolProgressionManager toolManager)
	{
		this.reactor = ghostReactor;
		this.manager = toolManager;
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.OnTreeUpdated += this.OnProgressionTreeUpdate;
			ProgressionManager.Instance.OnInventoryUpdated += this.OnInventoryUpdated;
		}
		this.RefreshProgressionTree();
		this.RefreshUserInventory();
	}

	// Token: 0x06003520 RID: 13600 RVA: 0x00123BD8 File Offset: 0x00121DD8
	public string GetTreeId()
	{
		return this.treeId;
	}

	// Token: 0x06003521 RID: 13601 RVA: 0x00123BE0 File Offset: 0x00121DE0
	public List<GRTool.GRToolType> GetSupportedTools()
	{
		List<GRTool.GRToolType> list = new List<GRTool.GRToolType>();
		foreach (GRTool.GRToolType grtoolType in this.toolTree.Keys)
		{
			list.Add(grtoolType);
		}
		return list;
	}

	// Token: 0x06003522 RID: 13602 RVA: 0x00123C40 File Offset: 0x00121E40
	public List<GRToolProgressionTree.GRToolProgressionNode> GetToolUpgrades(GRTool.GRToolType tool)
	{
		List<GRToolProgressionTree.GRToolProgressionNode> list = new List<GRToolProgressionTree.GRToolProgressionNode>();
		this.AddToolProgressionChildren(this.toolTree[tool], ref list);
		return list;
	}

	// Token: 0x06003523 RID: 13603 RVA: 0x00123C68 File Offset: 0x00121E68
	public GRToolProgressionTree.GRToolProgressionNode GetToolNode(GRTool.GRToolType tool)
	{
		if (this.toolTree.ContainsKey(tool))
		{
			return this.toolTree[tool];
		}
		return null;
	}

	// Token: 0x06003524 RID: 13604 RVA: 0x00123C86 File Offset: 0x00121E86
	public GRToolProgressionTree.GRToolProgressionNode GetPartNode(GRToolProgressionManager.ToolParts part)
	{
		if (this.partTree.ContainsKey(part))
		{
			return this.partTree[part];
		}
		return null;
	}

	// Token: 0x06003525 RID: 13605 RVA: 0x00123CA4 File Offset: 0x00121EA4
	public void RefreshProgressionTree()
	{
		ProgressionManager.Instance.RefreshProgressionTree();
	}

	// Token: 0x06003526 RID: 13606 RVA: 0x00123CB0 File Offset: 0x00121EB0
	public void RefreshUserInventory()
	{
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x06003527 RID: 13607 RVA: 0x00123CBC File Offset: 0x00121EBC
	private void OnProgressionTreeUpdate()
	{
		UserHydratedProgressionTreeResponse tree = ProgressionManager.Instance.GetTree(this.treeName);
		if (tree != null)
		{
			this.ProcessToolProgressionTree(tree);
		}
		GRToolProgressionManager grtoolProgressionManager = this.manager;
		if (grtoolProgressionManager == null)
		{
			return;
		}
		grtoolProgressionManager.SendMothershipUpdated();
	}

	// Token: 0x06003528 RID: 13608 RVA: 0x00123CF4 File Offset: 0x00121EF4
	private void OnInventoryUpdated()
	{
		ProgressionManager.MothershipItemSummary mothershipItemSummary;
		if (ProgressionManager.Instance.GetInventoryItem(this.researchPointsEntitlement, out mothershipItemSummary))
		{
			this.currentResearchPoints = mothershipItemSummary.Quantity;
		}
		ProgressionManager.MothershipItemSummary mothershipItemSummary2;
		ProgressionManager.MothershipItemSummary mothershipItemSummary3;
		ProgressionManager.MothershipItemSummary mothershipItemSummary4;
		if (ProgressionManager.Instance.GetInventoryItem(this.fullTimeEntitlement, out mothershipItemSummary2))
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.FullTime;
		}
		else if (ProgressionManager.Instance.GetInventoryItem(this.partTimeEntitlement, out mothershipItemSummary3))
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.PartTime;
		}
		else if (ProgressionManager.Instance.GetInventoryItem(this.internEntitlement, out mothershipItemSummary4))
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.Intern;
		}
		else
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.None;
		}
		GRToolProgressionManager grtoolProgressionManager = this.manager;
		if (grtoolProgressionManager == null)
		{
			return;
		}
		grtoolProgressionManager.SendMothershipUpdated();
	}

	// Token: 0x06003529 RID: 13609 RVA: 0x00123D8F File Offset: 0x00121F8F
	public GRToolProgressionTree.EmployeeLevelRequirement GetCurrentEmploymentLevel()
	{
		return this.currentEmploymentLevel;
	}

	// Token: 0x0600352A RID: 13610 RVA: 0x00123D98 File Offset: 0x00121F98
	private void AddToolProgressionChildren(GRToolProgressionTree.GRToolProgressionNode currentNode, ref List<GRToolProgressionTree.GRToolProgressionNode> list)
	{
		foreach (GRToolProgressionTree.GRToolProgressionNode grtoolProgressionNode in currentNode.children)
		{
			list.Add(grtoolProgressionNode);
			this.AddToolProgressionChildren(grtoolProgressionNode, ref list);
		}
	}

	// Token: 0x0600352B RID: 13611 RVA: 0x00123DF4 File Offset: 0x00121FF4
	public int GetNumberOfResearchPoints()
	{
		return this.currentResearchPoints;
	}

	// Token: 0x0600352C RID: 13612 RVA: 0x00123DFC File Offset: 0x00121FFC
	private void InitializeToolMapping()
	{
		this.toolMapping["ChargeBaton"] = GRTool.GRToolType.Club;
		this.toolMapping["FlashTool"] = GRTool.GRToolType.Flash;
		this.toolMapping["Revive"] = GRTool.GRToolType.Revive;
		this.toolMapping["Collector"] = GRTool.GRToolType.Collector;
		this.toolMapping["Lantern"] = GRTool.GRToolType.Lantern;
		this.toolMapping["ShieldGun"] = GRTool.GRToolType.ShieldGun;
		this.toolMapping["DirectionalShield"] = GRTool.GRToolType.DirectionalShield;
		this.toolMapping["DockWrist"] = GRTool.GRToolType.DockWrist;
		this.toolMapping["EnergyEfficiency"] = GRTool.GRToolType.EnergyEfficiency;
		this.toolMapping["DropPodBasic"] = GRTool.GRToolType.DropPod;
	}

	// Token: 0x0600352D RID: 13613 RVA: 0x00123EB8 File Offset: 0x001220B8
	private void InitializeClubPartMapping()
	{
		this.partMapping["ChargeBaton"] = GRToolProgressionManager.ToolParts.Baton;
		this.partMapping["BatonDamage1"] = GRToolProgressionManager.ToolParts.BatonDamage1;
		this.partMapping["BatonDamage2"] = GRToolProgressionManager.ToolParts.BatonDamage2;
		this.partMapping["BatonDamage3"] = GRToolProgressionManager.ToolParts.BatonDamage3;
	}

	// Token: 0x0600352E RID: 13614 RVA: 0x00123F0C File Offset: 0x0012210C
	private void InitializeFlashPartMapping()
	{
		this.partMapping["FlashTool"] = GRToolProgressionManager.ToolParts.Flash;
		this.partMapping["FlashDamage1"] = GRToolProgressionManager.ToolParts.FlashDamage1;
		this.partMapping["FlashDamage2"] = GRToolProgressionManager.ToolParts.FlashDamage2;
		this.partMapping["FlashDamage3"] = GRToolProgressionManager.ToolParts.FlashDamage3;
	}

	// Token: 0x0600352F RID: 13615 RVA: 0x00123F60 File Offset: 0x00122160
	private void InitializeCollectorPartMapping()
	{
		this.partMapping["Collector"] = GRToolProgressionManager.ToolParts.Collector;
		this.partMapping["CollectorBonus1"] = GRToolProgressionManager.ToolParts.CollectorBonus1;
		this.partMapping["CollectorBonus2"] = GRToolProgressionManager.ToolParts.CollectorBonus2;
		this.partMapping["CollectorBonus3"] = GRToolProgressionManager.ToolParts.CollectorBonus3;
	}

	// Token: 0x06003530 RID: 13616 RVA: 0x00123FB5 File Offset: 0x001221B5
	private void InitializeRevivePartMapping()
	{
		this.partMapping["Revive"] = GRToolProgressionManager.ToolParts.Revive;
	}

	// Token: 0x06003531 RID: 13617 RVA: 0x00123FCC File Offset: 0x001221CC
	private void InitializeLanternPartMapping()
	{
		this.partMapping["Lantern"] = GRToolProgressionManager.ToolParts.Lantern;
		this.partMapping["LanternIntensity1"] = GRToolProgressionManager.ToolParts.LanternIntensity1;
		this.partMapping["LanternIntensity2"] = GRToolProgressionManager.ToolParts.LanternIntensity2;
		this.partMapping["LanternIntensity3"] = GRToolProgressionManager.ToolParts.LanternIntensity3;
	}

	// Token: 0x06003532 RID: 13618 RVA: 0x00124024 File Offset: 0x00122224
	private void InitializeShieldGunPartMapping()
	{
		this.partMapping["ShieldGun"] = GRToolProgressionManager.ToolParts.ShieldGun;
		this.partMapping["ShieldGunStrength1"] = GRToolProgressionManager.ToolParts.ShieldGunStrength1;
		this.partMapping["ShieldGunStrength2"] = GRToolProgressionManager.ToolParts.ShieldGunStrength2;
		this.partMapping["ShieldGunStrength3"] = GRToolProgressionManager.ToolParts.ShieldGunStrength3;
	}

	// Token: 0x06003533 RID: 13619 RVA: 0x0012407C File Offset: 0x0012227C
	private void InitializeDirectionalShieldPartMapping()
	{
		this.partMapping["DirectionalShield"] = GRToolProgressionManager.ToolParts.DirectionalShield;
		this.partMapping["DirectionalShieldSize1"] = GRToolProgressionManager.ToolParts.DirectionalShieldSize1;
		this.partMapping["DirectionalShieldSize2"] = GRToolProgressionManager.ToolParts.DirectionalShieldSize2;
		this.partMapping["DirectionalShieldSize3"] = GRToolProgressionManager.ToolParts.DirectionalShieldSize3;
	}

	// Token: 0x06003534 RID: 13620 RVA: 0x001240D4 File Offset: 0x001222D4
	private void InitializeEnergyEfficiencyPartMapping()
	{
		this.partMapping["EnergyEfficiency"] = GRToolProgressionManager.ToolParts.EnergyEff;
		this.partMapping["EnergyEff1"] = GRToolProgressionManager.ToolParts.EnergyEff1;
		this.partMapping["EnergyEff2"] = GRToolProgressionManager.ToolParts.EnergyEff2;
		this.partMapping["EnergyEff3"] = GRToolProgressionManager.ToolParts.EnergyEff3;
	}

	// Token: 0x06003535 RID: 13621 RVA: 0x00124129 File Offset: 0x00122329
	private void InitializeDockWristPartMapping()
	{
		this.partMapping["DockWrist"] = GRToolProgressionManager.ToolParts.DockWrist;
		this.partMapping["StatusWatch"] = GRToolProgressionManager.ToolParts.StatusWatch;
		this.partMapping["RattyBackpack"] = GRToolProgressionManager.ToolParts.RattyBackpack;
	}

	// Token: 0x06003536 RID: 13622 RVA: 0x00124164 File Offset: 0x00122364
	private void InitializeDropPodPartMapping()
	{
		this.partMapping["DropPodBasic"] = GRToolProgressionManager.ToolParts.DropPodBasic;
		this.partMapping["DropPodChassis01"] = GRToolProgressionManager.ToolParts.DropPodChassis1;
		this.partMapping["DropPodChassis02"] = GRToolProgressionManager.ToolParts.DropPodChassis2;
		this.partMapping["DropPodChassis03"] = GRToolProgressionManager.ToolParts.DropPodChassis3;
	}

	// Token: 0x06003537 RID: 13623 RVA: 0x001241BC File Offset: 0x001223BC
	private void AddFakeNodes()
	{
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.Club))
		{
			this.toolTree[GRTool.GRToolType.Club] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "Baton",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.Baton,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.Baton),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.Baton))
		{
			this.partTree[GRToolProgressionManager.ToolParts.Baton] = this.toolTree[GRTool.GRToolType.Club];
		}
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.EnergyEfficiency))
		{
			this.toolTree[GRTool.GRToolType.EnergyEfficiency] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "EnergyEfficiency",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.EnergyEff,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.EnergyEff),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.EnergyEff))
		{
			this.partTree[GRToolProgressionManager.ToolParts.EnergyEff] = this.toolTree[GRTool.GRToolType.EnergyEfficiency];
		}
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.Collector))
		{
			this.toolTree[GRTool.GRToolType.Collector] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "Collector",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.Collector,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.Collector),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.Collector))
		{
			this.partTree[GRToolProgressionManager.ToolParts.Collector] = this.toolTree[GRTool.GRToolType.Collector];
		}
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.Lantern))
		{
			this.toolTree[GRTool.GRToolType.Lantern] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "Lantern",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.Lantern,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.Lantern),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.Lantern))
		{
			this.partTree[GRToolProgressionManager.ToolParts.Lantern] = this.toolTree[GRTool.GRToolType.Lantern];
		}
	}

	// Token: 0x06003538 RID: 13624 RVA: 0x001243EC File Offset: 0x001225EC
	private void ProcessNodes()
	{
		foreach (KeyValuePair<string, GRToolProgressionTree.GRToolProgressionRawNode> keyValuePair in this.nodeTree)
		{
			GRToolProgressionTree.GRToolProgressionRawNode value = keyValuePair.Value;
			foreach (string text in value.requiredByIds)
			{
				if (this.nodeTree.ContainsKey(text))
				{
					this.nodeTree[text].progressionNode.children.Add(value.progressionNode);
					value.progressionNode.parents.Add(this.nodeTree[text].progressionNode);
				}
			}
			value.progressionNode.requiredEmployeeLevel = this.GetEmployeeLevel(value.requiredEntitlements);
			string text2 = value.progressionNode.name.Trim();
			if (this.toolMapping.ContainsKey(text2))
			{
				GRTool.GRToolType grtoolType = this.toolMapping[text2];
				value.progressionNode.rootNode = true;
				if (!value.progressionNode.unlocked && this.autoUnlockNodeId == string.Empty && value.progressionNode.researchCost == 0 && value.progressionNode.requiredEmployeeLevel == GRToolProgressionTree.EmployeeLevelRequirement.None)
				{
					this.autoUnlockNodeId = value.progressionNode.id;
				}
				this.toolTree[grtoolType] = value.progressionNode;
			}
			this.partTree[value.progressionNode.type] = value.progressionNode;
		}
	}

	// Token: 0x06003539 RID: 13625 RVA: 0x001245B8 File Offset: 0x001227B8
	private void PopulateMetadata()
	{
		foreach (KeyValuePair<string, GRToolProgressionTree.GRToolProgressionRawNode> keyValuePair in this.nodeTree)
		{
			keyValuePair.Value.progressionNode.partMetadata = this.manager.GetPartMetadata(keyValuePair.Value.progressionNode.type);
		}
	}

	// Token: 0x0600353A RID: 13626 RVA: 0x00124634 File Offset: 0x00122834
	private GRToolProgressionTree.EmployeeLevelRequirement GetEmployeeLevel(List<string> rawRequiredEntitlements)
	{
		foreach (string text in rawRequiredEntitlements)
		{
			string text2 = text.Trim();
			if (text2 == "Intern")
			{
				return GRToolProgressionTree.EmployeeLevelRequirement.Intern;
			}
			if (text2 == "PartTime")
			{
				return GRToolProgressionTree.EmployeeLevelRequirement.PartTime;
			}
			if (text2 == "FullTime")
			{
				return GRToolProgressionTree.EmployeeLevelRequirement.FullTime;
			}
		}
		return GRToolProgressionTree.EmployeeLevelRequirement.None;
	}

	// Token: 0x0600353B RID: 13627 RVA: 0x001246B8 File Offset: 0x001228B8
	private void ProcessTreeNode(UserHydratedNodeDefinition treeNode)
	{
		GRToolProgressionTree.GRToolProgressionRawNode grtoolProgressionRawNode = new GRToolProgressionTree.GRToolProgressionRawNode();
		grtoolProgressionRawNode.progressionNode.id = treeNode.id;
		grtoolProgressionRawNode.progressionNode.name = treeNode.name;
		grtoolProgressionRawNode.progressionNode.unlocked = treeNode.unlocked;
		if (this.partMapping.ContainsKey(grtoolProgressionRawNode.progressionNode.name))
		{
			if (this.toolMapping.ContainsKey(grtoolProgressionRawNode.progressionNode.name))
			{
				grtoolProgressionRawNode.progressionNode.rootNode = true;
			}
			grtoolProgressionRawNode.progressionNode.type = this.partMapping[grtoolProgressionRawNode.progressionNode.name];
		}
		if (treeNode.cost != null && treeNode.cost.items != null)
		{
			foreach (KeyValuePair<string, MothershipHydratedInventoryChange> keyValuePair in treeNode.cost.items)
			{
				if (keyValuePair.Key.Trim() == this.researchPointsEntitlement)
				{
					grtoolProgressionRawNode.progressionNode.researchCost = keyValuePair.Value.Delta;
				}
			}
		}
		foreach (MothershipEntitlementCatalogItem mothershipEntitlementCatalogItem in treeNode.prerequisite_entitlements)
		{
			grtoolProgressionRawNode.requiredEntitlements.Add(mothershipEntitlementCatalogItem.name);
		}
		foreach (SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t swigtype_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t in treeNode.prerequisite_nodes.nodes)
		{
			ComplexPrerequisiteNodes complexPrerequisiteNodes = new ComplexPrerequisiteNodes();
			NodeReference nodeReference = new NodeReference();
			if (!MothershipApi.TryGetComplexPrerequisiteNodeFromVariant(swigtype_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t, complexPrerequisiteNodes) && MothershipApi.TryGetNodeReferenceFromVariant(swigtype_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t, nodeReference))
			{
				grtoolProgressionRawNode.requiredByIds.Add(nodeReference.node_id);
			}
		}
		if (this.pendingPartUnlock != GRToolProgressionManager.ToolParts.None && this.pendingPartUnlock == grtoolProgressionRawNode.progressionNode.type)
		{
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			if (this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodBasic || this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodChassis1 || this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodChassis2 || this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodChassis3)
			{
				if (this.pendingPartUnlock != GRToolProgressionManager.ToolParts.DropPodBasic)
				{
					grplayer.SendPodUpgradeTelemetry(grtoolProgressionRawNode.progressionNode.name, treeNode.prerequisite_entitlements.Count, 0, grtoolProgressionRawNode.progressionNode.researchCost);
				}
			}
			else
			{
				grplayer.SendToolUpgradeTelemetry("Research", grtoolProgressionRawNode.progressionNode.name, treeNode.prerequisite_entitlements.Count, grtoolProgressionRawNode.progressionNode.researchCost, 0, 0);
			}
			this.pendingPartUnlock = GRToolProgressionManager.ToolParts.None;
		}
		this.nodeTree[grtoolProgressionRawNode.progressionNode.id] = grtoolProgressionRawNode;
	}

	// Token: 0x0600353C RID: 13628 RVA: 0x00124974 File Offset: 0x00122B74
	private void ProcessToolProgressionTree(UserHydratedProgressionTreeResponse tree)
	{
		if (tree.Tree.name != this.treeName)
		{
			return;
		}
		this.toolTree = new Dictionary<GRTool.GRToolType, GRToolProgressionTree.GRToolProgressionNode>();
		this.nodeTree = new Dictionary<string, GRToolProgressionTree.GRToolProgressionRawNode>();
		this.treeId = tree.Tree.id;
		foreach (UserHydratedNodeDefinition userHydratedNodeDefinition in tree.Nodes)
		{
			this.ProcessTreeNode(userHydratedNodeDefinition);
		}
		this.PopulateMetadata();
		this.ProcessNodes();
		this.AddFakeNodes();
		if (this.autoUnlockNodeId != string.Empty)
		{
			string text = this.autoUnlockNodeId;
			this.autoUnlockNodeId = string.Empty;
			GhostReactorProgression.instance.UnlockProgressionTreeNode(this.treeId, text, this.reactor);
		}
		GRToolProgressionManager grtoolProgressionManager = this.manager;
		if (grtoolProgressionManager == null)
		{
			return;
		}
		grtoolProgressionManager.SendMothershipUpdated();
	}

	// Token: 0x0600353D RID: 13629 RVA: 0x00124A60 File Offset: 0x00122C60
	public void AttemptToUnlockPart(GRToolProgressionManager.ToolParts part)
	{
		if (this.partTree.ContainsKey(part))
		{
			this.pendingPartUnlock = part;
			GhostReactorProgression.instance.UnlockProgressionTreeNode(this.treeId, this.partTree[part].id, this.reactor);
		}
	}

	// Token: 0x04004534 RID: 17716
	private string treeName = "GRTools";

	// Token: 0x04004535 RID: 17717
	private string treeId = string.Empty;

	// Token: 0x04004536 RID: 17718
	private string researchPointsEntitlement = "GR_ResearchPoints";

	// Token: 0x04004537 RID: 17719
	private Dictionary<GRTool.GRToolType, GRToolProgressionTree.GRToolProgressionNode> toolTree = new Dictionary<GRTool.GRToolType, GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x04004538 RID: 17720
	private Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionTree.GRToolProgressionNode> partTree = new Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x04004539 RID: 17721
	private Dictionary<string, GRToolProgressionTree.GRToolProgressionRawNode> nodeTree = new Dictionary<string, GRToolProgressionTree.GRToolProgressionRawNode>();

	// Token: 0x0400453A RID: 17722
	private Dictionary<string, GRTool.GRToolType> toolMapping = new Dictionary<string, GRTool.GRToolType>();

	// Token: 0x0400453B RID: 17723
	private Dictionary<string, GRToolProgressionManager.ToolParts> partMapping = new Dictionary<string, GRToolProgressionManager.ToolParts>();

	// Token: 0x0400453C RID: 17724
	private string autoUnlockNodeId = string.Empty;

	// Token: 0x0400453D RID: 17725
	private int currentResearchPoints;

	// Token: 0x0400453E RID: 17726
	[NonSerialized]
	private GhostReactor reactor;

	// Token: 0x0400453F RID: 17727
	[NonSerialized]
	private GRToolProgressionManager manager;

	// Token: 0x04004540 RID: 17728
	[NonSerialized]
	private GRToolProgressionTree.EmployeeLevelRequirement currentEmploymentLevel;

	// Token: 0x04004541 RID: 17729
	private string internEntitlement = "Intern";

	// Token: 0x04004542 RID: 17730
	private string partTimeEntitlement = "PartTime";

	// Token: 0x04004543 RID: 17731
	private string fullTimeEntitlement = "FullTime";

	// Token: 0x04004544 RID: 17732
	private GRToolProgressionManager.ToolParts pendingPartUnlock;

	// Token: 0x02000819 RID: 2073
	public enum EmployeeLevelRequirement
	{
		// Token: 0x04004546 RID: 17734
		None,
		// Token: 0x04004547 RID: 17735
		Intern,
		// Token: 0x04004548 RID: 17736
		PartTime,
		// Token: 0x04004549 RID: 17737
		FullTime
	}

	// Token: 0x0200081A RID: 2074
	public class GRToolProgressionNode
	{
		// Token: 0x0400454A RID: 17738
		public string id;

		// Token: 0x0400454B RID: 17739
		public string name;

		// Token: 0x0400454C RID: 17740
		public bool unlocked;

		// Token: 0x0400454D RID: 17741
		public int researchCost;

		// Token: 0x0400454E RID: 17742
		public bool rootNode;

		// Token: 0x0400454F RID: 17743
		public GRToolProgressionManager.ToolParts type;

		// Token: 0x04004550 RID: 17744
		public GRToolProgressionManager.ToolProgressionMetaData partMetadata;

		// Token: 0x04004551 RID: 17745
		public List<GRToolProgressionTree.GRToolProgressionNode> children = new List<GRToolProgressionTree.GRToolProgressionNode>();

		// Token: 0x04004552 RID: 17746
		public List<GRToolProgressionTree.GRToolProgressionNode> parents = new List<GRToolProgressionTree.GRToolProgressionNode>();

		// Token: 0x04004553 RID: 17747
		public GRToolProgressionTree.EmployeeLevelRequirement requiredEmployeeLevel;
	}

	// Token: 0x0200081B RID: 2075
	private class GRToolProgressionRawNode
	{
		// Token: 0x04004554 RID: 17748
		public GRToolProgressionTree.GRToolProgressionNode progressionNode = new GRToolProgressionTree.GRToolProgressionNode();

		// Token: 0x04004555 RID: 17749
		public List<string> requiredByIds = new List<string>();

		// Token: 0x04004556 RID: 17750
		public List<string> requiredEntitlements = new List<string>();
	}
}
