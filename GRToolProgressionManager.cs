using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000814 RID: 2068
public class GRToolProgressionManager : MonoBehaviourTick
{
	// Token: 0x1400005D RID: 93
	// (add) Token: 0x060034F3 RID: 13555 RVA: 0x00122A84 File Offset: 0x00120C84
	// (remove) Token: 0x060034F4 RID: 13556 RVA: 0x00122ABC File Offset: 0x00120CBC
	public event Action OnProgressionUpdated;

	// Token: 0x060034F5 RID: 13557 RVA: 0x00122AF1 File Offset: 0x00120CF1
	public void SetPendingTreeToProcess()
	{
		this.pendingTreeToProcess = true;
	}

	// Token: 0x060034F6 RID: 13558 RVA: 0x00122AFA File Offset: 0x00120CFA
	public void UpdateInventory()
	{
		this.pendingUpdateInventory = true;
	}

	// Token: 0x060034F7 RID: 13559 RVA: 0x00122B04 File Offset: 0x00120D04
	public void Init(GhostReactor ghostReactor)
	{
		this.reactor = ghostReactor;
		this.PopulateToolPartMetadata();
		this.PopulateEmployeeLevelMetadata();
		if (this.researchStations != null)
		{
			foreach (GRResearchStation grresearchStation in this.researchStations)
			{
				grresearchStation.Init(this, ghostReactor);
			}
		}
		if (this.toolUpgradeStations != null)
		{
			foreach (GRToolUpgradeStation grtoolUpgradeStation in this.toolUpgradeStations)
			{
				grtoolUpgradeStation.Init(this, ghostReactor);
			}
		}
		this.toolProgressionTree.Init(this.reactor, this);
	}

	// Token: 0x060034F8 RID: 13560 RVA: 0x00122BCC File Offset: 0x00120DCC
	public override void Tick()
	{
		if (this.sendUpdate)
		{
			Action onProgressionUpdated = this.OnProgressionUpdated;
			if (onProgressionUpdated != null)
			{
				onProgressionUpdated();
			}
			this.sendUpdate = false;
		}
		if (this.pendingTreeToProcess)
		{
			this.toolProgressionTree.RefreshProgressionTree();
			this.pendingTreeToProcess = false;
		}
		if (this.pendingUpdateInventory)
		{
			this.toolProgressionTree.RefreshUserInventory();
			this.pendingUpdateInventory = false;
		}
	}

	// Token: 0x060034F9 RID: 13561 RVA: 0x00122C2D File Offset: 0x00120E2D
	public void SendMothershipUpdated()
	{
		this.sendUpdate = true;
	}

	// Token: 0x060034FA RID: 13562 RVA: 0x00122C38 File Offset: 0x00120E38
	public GRToolProgressionManager.ToolProgressionMetaData GetPartMetadata(GRToolProgressionManager.ToolParts part)
	{
		GRToolProgressionManager.ToolProgressionMetaData toolProgressionMetaData;
		this.partMetadata.TryGetValue(part, out toolProgressionMetaData);
		return toolProgressionMetaData;
	}

	// Token: 0x060034FB RID: 13563 RVA: 0x00122C58 File Offset: 0x00120E58
	private void PopulateToolPartMetadata()
	{
		this.PopulateClubPartMetadata();
		this.PopulateFlashPartMetadata();
		this.PopulateCollectorPartMetadata();
		this.PopulateLanternPartMetadata();
		this.PopulateShieldGunPartMetadata();
		this.PopulateDirectionalShieldPartMetadata();
		this.PopulateEnergyEfficiencyPartMetadata();
		this.PopulateRevivePartMetadata();
		this.PopulateDockWristPartMetadata();
		this.PopulateDropPodPartMetadata();
		this.PopulateHocketStickMetadata();
	}

	// Token: 0x060034FC RID: 13564 RVA: 0x00122CA8 File Offset: 0x00120EA8
	private void PopulateEmployeeLevelMetadata()
	{
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.None] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "None",
			level = 0
		};
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.Intern] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "Intern",
			level = 2
		};
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.PartTime] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "Part Time",
			level = 3
		};
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.FullTime] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "Full Time",
			level = 4
		};
	}

	// Token: 0x060034FD RID: 13565 RVA: 0x00122D5C File Offset: 0x00120F5C
	private void PopulateClubPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Baton] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Charge Baton",
			description = "50,000 volts of ghost-zapping power",
			annotation = "Impact Power: ❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.BatonDamage1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Lead Core",
			description = "Conductive lead sheath",
			annotation = "Attaches to Charge Baton. Impact Power: ❶❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.BatonDamage2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Osmium Core",
			description = "More mass for more win",
			annotation = "Attaches to Charge Baton. Impact Power: ❶❶❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.BatonDamage3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Electrified Spikes",
			description = "Impales, shocks, and crushes simultaneously",
			annotation = "Attaches to Charge Baton. Impact Power: ❶❶❶❶❶"
		};
	}

	// Token: 0x060034FE RID: 13566 RVA: 0x00122E5C File Offset: 0x0012105C
	private void PopulateFlashPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Flash] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Spectral Flash",
			description = "Makes strong ghosts vulnerable",
			annotation = "Damages ghost armor."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.FlashDamage1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Spectral Lens",
			description = "Safety through momentary paralysis",
			annotation = "Attaches to Spectral Flash. Stuns enemies."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.FlashDamage2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Parabolic Focuser",
			description = "When you want ghosts to feel it",
			annotation = "Attaches to Spectral Flash. Stuns enemies. Disintegrates armor."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.FlashDamage3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Beta Wave Amplifier",
			description = "Exposure with explosive results",
			annotation = "Attaches to Spectral Flash. Stuns enemies. Shatters armor."
		};
	}

	// Token: 0x060034FF RID: 13567 RVA: 0x00122F5C File Offset: 0x0012115C
	private void PopulateCollectorPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Collector] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 50,
			name = "Collector",
			description = "Every team needs a sucker",
			annotation = "Collects essence and recharges tools"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.CollectorBonus1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Vortex Intake",
			description = "Harvests ambient essence",
			annotation = "Attaches to Collector.  Recharges over time."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.CollectorBonus2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Cyclone Intake",
			description = "Creates a wormhole to a twin universe",
			annotation = "Attaches to Collector. 2x collection bonus."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.CollectorBonus3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Hurricane Intake",
			description = "A Category 5 commitment to teamwork",
			annotation = "Attaches to Collector. 2x collection bonus.  Area recharge."
		};
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x00123060 File Offset: 0x00121260
	private void PopulateLanternPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Lantern] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 50,
			name = "Lantern",
			description = "Creates the gentle glow of safety",
			annotation = "Illuminates dark areas."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.LanternIntensity1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Kinetic Power",
			description = "Saves batteries to optimize shareholder value",
			annotation = "Attaches to Lantern. Doesn't need recharge."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.LanternIntensity2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Flare Discharge",
			description = "Blaze the trail for your team",
			annotation = "Attaches to Lantern. Drops long-lasting flares."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.LanternIntensity3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Gamma Burster",
			description = "See through walls. Do not aim at important body parts",
			annotation = "Attaches to Lantern. X-ray ghost vision."
		};
	}

	// Token: 0x06003501 RID: 13569 RVA: 0x00123164 File Offset: 0x00121364
	private void PopulateShieldGunPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGun] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Forcefield Gun",
			description = "Corporate armor for fragile assets",
			annotation = "Gives forcefields."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGunStrength1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Truebright Nozzle",
			description = "Nuclear protection",
			annotation = "Attaches to Forcefield Gun. Increases light."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGunStrength2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Stealth Nozzle",
			description = "Protection they'll never see coming",
			annotation = "Attaches to Forcefield Gun. Gives temporary stealth."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGunStrength3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Medic Nozzle",
			description = "Restores productivity through impact therapy",
			annotation = "Attaches to Forcefield Gun. Heals to full."
		};
	}

	// Token: 0x06003502 RID: 13570 RVA: 0x00123268 File Offset: 0x00121468
	private void PopulateDirectionalShieldPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShield] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Umbrella Shield",
			description = "Protects company property",
			annotation = "Blocks attacks."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShieldSize1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Sling Shield",
			description = "Deflects danger and liability",
			annotation = "Attaches to Umbrella Shield. Reflects projectiles."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShieldSize2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Harmshadow",
			description = "The best defense is a good offense",
			annotation = "Attaches to Umbrella Shield. Impact Power: ❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShieldSize3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Total Defense Array",
			description = "The only safety device with a kill count",
			annotation = "Attaches to Shield. Reflects projectiles. Impact power: ❶❶"
		};
	}

	// Token: 0x06003503 RID: 13571 RVA: 0x0012336C File Offset: 0x0012156C
	private void PopulateEnergyEfficiencyPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Flash",
			description = "Lead Core Does things!"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Regulator",
			description = "Do more with less",
			annotation = "Attaches to most tools. Efficiency: +❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Optimizer",
			description = "Half the juice, double the morale",
			annotation = "Attaches to most tools. Efficiency: +❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Peak Power",
			description = "Efficiency that borders on spiritual enlightenment",
			annotation = "Attaches to most tools. Efficiency: +❶❶❶"
		};
	}

	// Token: 0x06003504 RID: 13572 RVA: 0x00123463 File Offset: 0x00121663
	private void PopulateRevivePartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Revive] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Revive",
			description = "Turns fatal injuries into teachable moments",
			annotation = "Brings defeated employees back to life."
		};
	}

	// Token: 0x06003505 RID: 13573 RVA: 0x001234A0 File Offset: 0x001216A0
	private void PopulateDockWristPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.DockWrist] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 500,
			name = "Wrist Dock",
			description = "Wearable storage that maximizes output per limb",
			annotation = "Extra storage slot"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.StatusWatch] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Ecto Watch",
			description = "Keep track of your location and statistics",
			annotation = "Compass and stat tracker"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.RattyBackpack] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 300,
			name = "Ratty Backpack",
			description = "Torn up backpack we found laying around. Can store one item.",
			annotation = "Worn on the back. It's a backpack."
		};
	}

	// Token: 0x06003506 RID: 13574 RVA: 0x00123568 File Offset: 0x00121768
	private void PopulateDropPodPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodBasic] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Starter Pod",
			description = "Descend with confidence in a personal drop pod!\nSupports drops to 5000m\nUpgradable for deeper drops",
			annotation = "DropPodBasic"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodChassis1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Reinforced Pod Chassis",
			description = "Upgrade your drop pod to support drops to 10000m",
			annotation = "DropPodChassis1"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodChassis2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Iron Pod Chassis",
			description = "Upgrade your drop pod to support drops to 15000m",
			annotation = "DropPodChassis2"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodChassis3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Steel Pod Chassis",
			description = "Upgrade your drop pod to support drops to 20000m",
			annotation = "DropPodChassis3"
		};
	}

	// Token: 0x06003507 RID: 13575 RVA: 0x0012366A File Offset: 0x0012186A
	private void PopulateHocketStickMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.HockeyStick] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 10,
			name = "Hockey Stick",
			description = "A Used Hockey Stick",
			annotation = "Hit things with it?"
		};
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x001236A7 File Offset: 0x001218A7
	public int GetRequiredEmployeeLevel(GRToolProgressionTree.EmployeeLevelRequirement employeeLevel)
	{
		return this.employeeLevelMetadata[employeeLevel].level;
	}

	// Token: 0x06003509 RID: 13577 RVA: 0x001236BA File Offset: 0x001218BA
	public string GetEmployeeLevelDisplayName(GRToolProgressionTree.EmployeeLevelRequirement employeeLevel)
	{
		return this.employeeLevelMetadata[employeeLevel].name;
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x001236CD File Offset: 0x001218CD
	public int GetNumberOfResearchPoints()
	{
		return this.toolProgressionTree.GetNumberOfResearchPoints();
	}

	// Token: 0x0600350B RID: 13579 RVA: 0x001236DA File Offset: 0x001218DA
	public List<GRTool.GRToolType> GetSupportedTools()
	{
		return this.toolProgressionTree.GetSupportedTools();
	}

	// Token: 0x0600350C RID: 13580 RVA: 0x001236E7 File Offset: 0x001218E7
	public List<GRToolProgressionTree.GRToolProgressionNode> GetToolUpgrades(GRTool.GRToolType tool)
	{
		return this.toolProgressionTree.GetToolUpgrades(tool);
	}

	// Token: 0x0600350D RID: 13581 RVA: 0x001236F8 File Offset: 0x001218F8
	public int GetRecycleShiftCredit(GRTool.GRToolType tool)
	{
		if (tool == GRTool.GRToolType.HockeyStick)
		{
			return (int)(10f / (float)this.reactor.vrRigs.Count);
		}
		GRToolProgressionTree.GRToolProgressionNode toolNode = this.toolProgressionTree.GetToolNode(tool);
		if (toolNode != null)
		{
			return (int)((float)(toolNode.partMetadata.shiftCreditCost / 2) / (float)this.reactor.vrRigs.Count);
		}
		return 0;
	}

	// Token: 0x0600350E RID: 13582 RVA: 0x00123756 File Offset: 0x00121956
	public bool GetShiftCreditCost(GRToolProgressionManager.ToolParts part, out int shiftCreditCost)
	{
		shiftCreditCost = 0;
		if (this.partMetadata.ContainsKey(part))
		{
			shiftCreditCost += this.partMetadata[part].shiftCreditCost;
			return true;
		}
		return false;
	}

	// Token: 0x0600350F RID: 13583 RVA: 0x00123784 File Offset: 0x00121984
	public void AttemptToUnlockPart(GRToolProgressionManager.ToolParts part)
	{
		bool flag;
		if (!this.IsPartUnlocked(part, out flag))
		{
			return;
		}
		if (!flag)
		{
			int numberOfResearchPoints = this.GetNumberOfResearchPoints();
			int num;
			if (!this.GetPartUnlockJuiceCost(part, out num))
			{
				return;
			}
			if (numberOfResearchPoints < num)
			{
				return;
			}
			GRToolProgressionTree.EmployeeLevelRequirement employeeLevelRequirement;
			if (!this.GetPartUnlockEmployeeRequiredLevel(part, out employeeLevelRequirement))
			{
				return;
			}
			int requiredEmployeeLevel = this.GetRequiredEmployeeLevel(this.GetCurrentEmployeeLevel());
			int requiredEmployeeLevel2 = this.GetRequiredEmployeeLevel(employeeLevelRequirement);
			if (requiredEmployeeLevel < requiredEmployeeLevel2)
			{
				return;
			}
			this.toolProgressionTree.AttemptToUnlockPart(part);
		}
	}

	// Token: 0x06003510 RID: 13584 RVA: 0x001237EC File Offset: 0x001219EC
	public bool IsPartUnlocked(GRToolProgressionManager.ToolParts part, out bool unlocked)
	{
		unlocked = false;
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		unlocked = partNode.unlocked;
		return true;
	}

	// Token: 0x06003511 RID: 13585 RVA: 0x00123818 File Offset: 0x00121A18
	public bool GetPartUnlockEmployeeRequiredLevel(GRToolProgressionManager.ToolParts part, out GRToolProgressionTree.EmployeeLevelRequirement level)
	{
		level = GRToolProgressionTree.EmployeeLevelRequirement.None;
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		level = partNode.requiredEmployeeLevel;
		return true;
	}

	// Token: 0x06003512 RID: 13586 RVA: 0x00123844 File Offset: 0x00121A44
	public bool GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts part, out int juiceCost)
	{
		juiceCost = 0;
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		juiceCost = partNode.researchCost;
		return true;
	}

	// Token: 0x06003513 RID: 13587 RVA: 0x00123870 File Offset: 0x00121A70
	public bool GetPartUnlockRequiredParentParts(GRToolProgressionManager.ToolParts part, out List<GRToolProgressionManager.ToolParts> requiredParts)
	{
		requiredParts = new List<GRToolProgressionManager.ToolParts>();
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		foreach (GRToolProgressionTree.GRToolProgressionNode grtoolProgressionNode in partNode.parents)
		{
			requiredParts.Add(grtoolProgressionNode.type);
		}
		return true;
	}

	// Token: 0x06003514 RID: 13588 RVA: 0x001238E4 File Offset: 0x00121AE4
	public bool GetPlayerShiftCredit(out int playerShiftCredit)
	{
		playerShiftCredit = 0;
		if (VRRig.LocalRig != null)
		{
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			if (grplayer != null)
			{
				playerShiftCredit = grplayer.ShiftCredits;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003515 RID: 13589 RVA: 0x00123920 File Offset: 0x00121B20
	public GRToolProgressionTree.EmployeeLevelRequirement GetCurrentEmployeeLevel()
	{
		return this.toolProgressionTree.GetCurrentEmploymentLevel();
	}

	// Token: 0x06003516 RID: 13590 RVA: 0x0012392D File Offset: 0x00121B2D
	public string GetTreeId()
	{
		return this.toolProgressionTree.GetTreeId();
	}

	// Token: 0x06003517 RID: 13591 RVA: 0x0012393C File Offset: 0x00121B3C
	public int GetDropPodLevel()
	{
		bool flag;
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodBasic, out flag) && flag)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06003518 RID: 13592 RVA: 0x0012395C File Offset: 0x00121B5C
	public int GetDropPodChasisLevel()
	{
		bool flag;
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis3, out flag) && flag)
		{
			return 3;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis2, out flag) && flag)
		{
			return 2;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis1, out flag) && flag)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06003519 RID: 13593 RVA: 0x0012399C File Offset: 0x00121B9C
	public ProgressionManager.DrillUpgradeLevel GetDrillLevel()
	{
		bool flag;
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis3, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Upgrade3;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis2, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Upgrade2;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis1, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Upgrade1;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodBasic, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Base;
		}
		return ProgressionManager.DrillUpgradeLevel.None;
	}

	// Token: 0x0600351A RID: 13594 RVA: 0x001239EC File Offset: 0x00121BEC
	public int GetJuiceCostForDrillUpgrade(ProgressionManager.DrillUpgradeLevel upgradeLevel)
	{
		int num = 0;
		switch (upgradeLevel)
		{
		case ProgressionManager.DrillUpgradeLevel.Base:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodBasic, out num);
			break;
		case ProgressionManager.DrillUpgradeLevel.Upgrade1:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodChassis1, out num);
			break;
		case ProgressionManager.DrillUpgradeLevel.Upgrade2:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodChassis2, out num);
			break;
		case ProgressionManager.DrillUpgradeLevel.Upgrade3:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodChassis3, out num);
			break;
		}
		return num;
	}

	// Token: 0x0600351B RID: 13595 RVA: 0x00123A48 File Offset: 0x00121C48
	public int GetSRCostForDrillUpgradeLevel(ProgressionManager.DrillUpgradeLevel level)
	{
		switch (level)
		{
		case ProgressionManager.DrillUpgradeLevel.Base:
			return 3600;
		case ProgressionManager.DrillUpgradeLevel.Upgrade1:
			return 0;
		case ProgressionManager.DrillUpgradeLevel.Upgrade2:
			return 0;
		case ProgressionManager.DrillUpgradeLevel.Upgrade3:
			return 0;
		default:
			return 0;
		}
	}

	// Token: 0x040044FD RID: 17661
	[NonSerialized]
	private Dictionary<GRToolProgressionTree.EmployeeLevelRequirement, GRToolProgressionManager.EmployeeMetadata> employeeLevelMetadata = new Dictionary<GRToolProgressionTree.EmployeeLevelRequirement, GRToolProgressionManager.EmployeeMetadata>();

	// Token: 0x040044FE RID: 17662
	[NonSerialized]
	private Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionManager.ToolProgressionMetaData> partMetadata = new Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionManager.ToolProgressionMetaData>();

	// Token: 0x040044FF RID: 17663
	[NonSerialized]
	private GRToolProgressionTree toolProgressionTree = new GRToolProgressionTree();

	// Token: 0x04004500 RID: 17664
	[NonSerialized]
	private GhostReactor reactor;

	// Token: 0x04004501 RID: 17665
	[SerializeField]
	private List<GRResearchStation> researchStations;

	// Token: 0x04004502 RID: 17666
	[SerializeField]
	private List<GRToolUpgradeStation> toolUpgradeStations;

	// Token: 0x04004503 RID: 17667
	[NonSerialized]
	private bool pendingTreeToProcess;

	// Token: 0x04004504 RID: 17668
	[NonSerialized]
	private bool pendingUpdateInventory;

	// Token: 0x04004506 RID: 17670
	private bool sendUpdate;

	// Token: 0x02000815 RID: 2069
	public class ToolProgressionMetaData
	{
		// Token: 0x04004507 RID: 17671
		public string name;

		// Token: 0x04004508 RID: 17672
		public string description;

		// Token: 0x04004509 RID: 17673
		public string annotation;

		// Token: 0x0400450A RID: 17674
		public int shiftCreditCost;
	}

	// Token: 0x02000816 RID: 2070
	public struct EmployeeMetadata
	{
		// Token: 0x0400450B RID: 17675
		public string name;

		// Token: 0x0400450C RID: 17676
		public int level;
	}

	// Token: 0x02000817 RID: 2071
	public enum ToolParts
	{
		// Token: 0x0400450E RID: 17678
		None,
		// Token: 0x0400450F RID: 17679
		Baton,
		// Token: 0x04004510 RID: 17680
		BatonDamage1,
		// Token: 0x04004511 RID: 17681
		BatonDamage2,
		// Token: 0x04004512 RID: 17682
		BatonDamage3,
		// Token: 0x04004513 RID: 17683
		Flash,
		// Token: 0x04004514 RID: 17684
		FlashDamage1,
		// Token: 0x04004515 RID: 17685
		FlashDamage2,
		// Token: 0x04004516 RID: 17686
		FlashDamage3,
		// Token: 0x04004517 RID: 17687
		Collector,
		// Token: 0x04004518 RID: 17688
		CollectorBonus1,
		// Token: 0x04004519 RID: 17689
		CollectorBonus2,
		// Token: 0x0400451A RID: 17690
		CollectorBonus3,
		// Token: 0x0400451B RID: 17691
		Lantern,
		// Token: 0x0400451C RID: 17692
		LanternIntensity1,
		// Token: 0x0400451D RID: 17693
		LanternIntensity2,
		// Token: 0x0400451E RID: 17694
		LanternIntensity3,
		// Token: 0x0400451F RID: 17695
		ShieldGun,
		// Token: 0x04004520 RID: 17696
		ShieldGunStrength1,
		// Token: 0x04004521 RID: 17697
		ShieldGunStrength2,
		// Token: 0x04004522 RID: 17698
		ShieldGunStrength3,
		// Token: 0x04004523 RID: 17699
		DirectionalShield,
		// Token: 0x04004524 RID: 17700
		DirectionalShieldSize1,
		// Token: 0x04004525 RID: 17701
		DirectionalShieldSize2,
		// Token: 0x04004526 RID: 17702
		DirectionalShieldSize3,
		// Token: 0x04004527 RID: 17703
		EnergyEff,
		// Token: 0x04004528 RID: 17704
		EnergyEff1,
		// Token: 0x04004529 RID: 17705
		EnergyEff2,
		// Token: 0x0400452A RID: 17706
		EnergyEff3,
		// Token: 0x0400452B RID: 17707
		DockWrist,
		// Token: 0x0400452C RID: 17708
		Revive,
		// Token: 0x0400452D RID: 17709
		DropPodBasic,
		// Token: 0x0400452E RID: 17710
		DropPodChassis1,
		// Token: 0x0400452F RID: 17711
		DropPodChassis2,
		// Token: 0x04004530 RID: 17712
		DropPodChassis3,
		// Token: 0x04004531 RID: 17713
		StatusWatch,
		// Token: 0x04004532 RID: 17714
		RattyBackpack,
		// Token: 0x04004533 RID: 17715
		HockeyStick
	}
}
