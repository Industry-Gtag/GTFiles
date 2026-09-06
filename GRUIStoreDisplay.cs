using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200083A RID: 2106
public class GRUIStoreDisplay : MonoBehaviour
{
	// Token: 0x06003627 RID: 13863 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Awake()
	{
	}

	// Token: 0x06003628 RID: 13864 RVA: 0x0012B389 File Offset: 0x00129589
	public void OnEnable()
	{
		this.RefreshUI();
	}

	// Token: 0x06003629 RID: 13865 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDisable()
	{
	}

	// Token: 0x0600362A RID: 13866 RVA: 0x0012B391 File Offset: 0x00129591
	public void Setup(int playerActorId, GhostReactor reactor)
	{
		this.reactor = reactor;
		this.toolProgressionManager = reactor.toolProgression;
		this.playerActorId = playerActorId;
		this.RefreshUI();
		this.toolProgressionManager.OnProgressionUpdated += this.onProgressionUpdated;
	}

	// Token: 0x0600362B RID: 13867 RVA: 0x0012B389 File Offset: 0x00129589
	private void onProgressionUpdated()
	{
		this.RefreshUI();
	}

	// Token: 0x0600362C RID: 13868 RVA: 0x0012B3CA File Offset: 0x001295CA
	private void RefreshUI()
	{
		this.RefreshItemInfo();
	}

	// Token: 0x0600362D RID: 13869 RVA: 0x0012B3D4 File Offset: 0x001295D4
	public void OnBuy(int playerActorNumber)
	{
		if (playerActorNumber != this.playerActorId)
		{
			return;
		}
		if (GRPlayer.Get(this.playerActorId) == null)
		{
			return;
		}
		if (!this.CanLocalPlayerPurchaseItem())
		{
			if (this.scanner != null)
			{
				UnityEvent onFailed = this.scanner.onFailed;
				if (onFailed == null)
				{
					return;
				}
				onFailed.Invoke();
			}
			return;
		}
		if (this.scanner != null)
		{
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded != null)
			{
				onSucceeded.Invoke();
			}
		}
		bool flag;
		if (!this.reactor.grManager.DebugIsToolStationHacked() && (!this.toolProgressionManager.IsPartUnlocked(this.slot.PurchaseID, out flag) || !flag))
		{
			if (this.slot.drillUpgradeLevel == ProgressionManager.DrillUpgradeLevel.Base)
			{
				if (ProgressionManager.Instance.GetShinyRocksTotal() >= 2500)
				{
					ProgressionManager.Instance.PurchaseDrillUpgrade(ProgressionManager.DrillUpgradeLevel.Base);
					return;
				}
			}
			else
			{
				this.toolProgressionManager.AttemptToUnlockPart(this.slot.PurchaseID);
			}
		}
	}

	// Token: 0x0600362E RID: 13870 RVA: 0x0012B4BF File Offset: 0x001296BF
	private bool CanLocalPlayerPurchaseItem()
	{
		return this.slot.canAfford;
	}

	// Token: 0x0600362F RID: 13871 RVA: 0x0012B4CC File Offset: 0x001296CC
	public void RefreshItemInfo()
	{
		bool flag = true;
		if (this.toolProgressionManager != null)
		{
			GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(this.slot.PurchaseID);
			if (partMetadata == null)
			{
				this.slot.Name.text = "ERROR";
				return;
			}
			string text = "ERROR";
			string text2 = "";
			Color white = Color.white;
			bool flag2 = true;
			int num = 10000;
			int num2;
			this.toolProgressionManager.GetPlayerShiftCredit(out num2);
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			this.slot.canAfford = false;
			this.slot.purchaseText = "LOCKED";
			if (this.slot.Description != null)
			{
				this.slot.Description.text = partMetadata.description;
			}
			bool flag3;
			if (this.toolProgressionManager.IsPartUnlocked(this.slot.PurchaseID, out flag3))
			{
				if (flag3)
				{
					if (this.slot.drillUpgradeLevel != ProgressionManager.DrillUpgradeLevel.None)
					{
						this.slot.Price.color = this.colorCanBuyCredits;
						this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
						this.slot.canAfford = true;
						this.slot.purchaseText = "Purchased";
						text = this.slot.purchaseText;
						this.slot.Price.text = text;
						return;
					}
					if (this.toolProgressionManager.GetShiftCreditCost(this.slot.PurchaseID, out num))
					{
						text = string.Format("⑭ {0}", num);
					}
					bool flag4 = num2 >= num;
					this.slot.Name.text = partMetadata.name;
					this.slot.Name.color = (flag ? this.colorSelectedItem : this.colorUnselectedItem);
					this.slot.Price.text = text;
					this.slot.Price.color = (flag4 ? this.colorCanBuyCredits : this.colorCantBuy);
					this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
					this.slot.canAfford = flag4;
					if (flag4)
					{
						this.slot.purchaseText = string.Format("BUY FOR\n⑭ {0}", num);
						return;
					}
					this.slot.purchaseText = string.Format("NEED\n⑭ {0}", num);
					return;
				}
				else
				{
					this.slot.Name.text = partMetadata.name;
					this.slot.Name.color = (flag ? this.colorUnresearchedItem : this.colorUnselectedUnresearchedItem);
					flag2 = true;
					GRToolProgressionTree.EmployeeLevelRequirement employeeLevelRequirement;
					if (this.toolProgressionManager.GetPartUnlockEmployeeRequiredLevel(this.slot.PurchaseID, out employeeLevelRequirement) && this.toolProgressionManager.GetCurrentEmployeeLevel() < employeeLevelRequirement)
					{
						this.toolProgressionManager.GetEmployeeLevelDisplayName(employeeLevelRequirement);
						text2 += string.Format("⑱ {0}\n", employeeLevelRequirement);
						flag2 = false;
					}
					this.cachedRequiredPartsList.Clear();
					if (this.toolProgressionManager.GetPartUnlockRequiredParentParts(this.slot.PurchaseID, out this.cachedRequiredPartsList))
					{
						foreach (GRToolProgressionManager.ToolParts toolParts in this.cachedRequiredPartsList)
						{
							bool flag5 = false;
							GRToolProgressionManager.ToolProgressionMetaData partMetadata2 = this.toolProgressionManager.GetPartMetadata(toolParts);
							if (partMetadata2 == null)
							{
								text2 += "⑱ ERROR\n";
								flag2 = false;
							}
							else if (!this.toolProgressionManager.IsPartUnlocked(toolParts, out flag5) || !flag5)
							{
								text2 = text2 + "⑱ " + partMetadata2.name + "\n";
								flag2 = false;
							}
						}
					}
					if (!flag2)
					{
						this.slot.Price.text = text2;
						this.slot.Price.color = this.colorCantBuy;
						this.slot.Price.fontSize = ((text2.Length <= 8) ? 2.25f : 1.6f);
						this.slot.canAfford = false;
						this.slot.purchaseText = "LOCKED";
						return;
					}
					if (this.slot.drillUpgradeLevel == ProgressionManager.DrillUpgradeLevel.Base)
					{
						this.slot.Price.color = this.colorCanBuyCredits;
						this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
						this.slot.canAfford = true;
						this.slot.purchaseText = string.Format("Cost {0}⑯ Shiny Rocks", 2500);
						text = this.slot.purchaseText;
						this.slot.Price.text = text;
						return;
					}
					if (this.toolProgressionManager.GetPartUnlockJuiceCost(this.slot.PurchaseID, out num))
					{
						text = string.Format("⑮ {0}", num);
					}
					bool flag4 = numberOfResearchPoints >= num;
					this.slot.Price.text = text;
					this.slot.Price.color = (flag4 ? this.colorCanBuyJuice : this.colorCantBuy);
					this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
					this.slot.canAfford = flag4;
					if (flag4)
					{
						this.slot.purchaseText = string.Format("RESEARCH\n⑮ {0}", num);
						return;
					}
					this.slot.purchaseText = string.Format("NEED\n⑮ {0}", num);
				}
			}
		}
	}

	// Token: 0x040046CE RID: 18126
	public IDCardScanner scanner;

	// Token: 0x040046CF RID: 18127
	public GRUIStoreDisplay.GRPurchaseSlot slot;

	// Token: 0x040046D0 RID: 18128
	private GhostReactor reactor;

	// Token: 0x040046D1 RID: 18129
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x040046D2 RID: 18130
	private int playerActorId;

	// Token: 0x040046D3 RID: 18131
	private Color colorPurchaseButtonCanAfford = GRToolUpgradePurchaseStationFull.ColorFromRGB32(0, 0, 0);

	// Token: 0x040046D4 RID: 18132
	private Color colorCanBuyCredits = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 229, 37);

	// Token: 0x040046D5 RID: 18133
	private Color colorCanBuyJuice = GRToolUpgradePurchaseStationFull.ColorFromRGB32(232, 65, 255);

	// Token: 0x040046D6 RID: 18134
	private Color colorCantBuy = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 38, 38);

	// Token: 0x040046D7 RID: 18135
	private Color colorSelectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(251, 240, 229);

	// Token: 0x040046D8 RID: 18136
	private Color colorUnselectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(147, 145, 140);

	// Token: 0x040046D9 RID: 18137
	private Color colorUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(230, 19, 17);

	// Token: 0x040046DA RID: 18138
	private Color colorUnselectedUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(133, 11, 10);

	// Token: 0x040046DB RID: 18139
	private List<GRToolProgressionManager.ToolParts> cachedRequiredPartsList = new List<GRToolProgressionManager.ToolParts>(5);

	// Token: 0x0200083B RID: 2107
	[Serializable]
	public class GRPurchaseSlot
	{
		// Token: 0x040046DC RID: 18140
		public TMP_Text Name;

		// Token: 0x040046DD RID: 18141
		public TMP_Text Price;

		// Token: 0x040046DE RID: 18142
		public TMP_Text Description;

		// Token: 0x040046DF RID: 18143
		public GRToolProgressionManager.ToolParts PurchaseID;

		// Token: 0x040046E0 RID: 18144
		[NonSerialized]
		public Material overrideMaterial;

		// Token: 0x040046E1 RID: 18145
		[NonSerialized]
		public bool canAfford;

		// Token: 0x040046E2 RID: 18146
		[NonSerialized]
		public string purchaseText = "";

		// Token: 0x040046E3 RID: 18147
		public ProgressionManager.DrillUpgradeLevel drillUpgradeLevel;
	}
}
