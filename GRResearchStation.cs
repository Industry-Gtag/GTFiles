using System;
using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020007E0 RID: 2016
public class GRResearchStation : MonoBehaviour
{
	// Token: 0x06003369 RID: 13161 RVA: 0x001194A4 File Offset: 0x001176A4
	public void Init(GRToolProgressionManager tree, GhostReactor ghostReactor)
	{
		this.toolProgressionManager = tree;
		this.toolProgressionManager.OnProgressionUpdated += this.ResearchTreeUpdated;
		this.reactor = ghostReactor;
		this.totalTools = 0;
		this.selectedToolIndex = 0;
		this._levelString = this.LevelText.text;
		this._costString = this.CostText.text;
		this._researchPointsString = this.ResearchPointsTex.text;
		this._requiredLevelString = this.RequiredLevelText.text;
		this.UpdateUI();
		this.SelectTool(0);
	}

	// Token: 0x0600336A RID: 13162 RVA: 0x00119538 File Offset: 0x00117738
	private void SelectTool(int index)
	{
		if (this.toolProgressionManager == null || this.totalTools == 0)
		{
			return;
		}
		if (index < this.totalTools && index > -1)
		{
			this.selectedToolIndex = index;
			this.selectedToolUpgrades = this.toolProgressionManager.GetToolUpgrades(this.supportedTools[this.selectedToolIndex]);
			this.SelectUpgrade(0);
			this.UpdateUI();
		}
	}

	// Token: 0x0600336B RID: 13163 RVA: 0x0011959F File Offset: 0x0011779F
	public void ResearchTreeUpdated()
	{
		this.supportedTools = this.toolProgressionManager.GetSupportedTools();
		this.totalTools = this.supportedTools.Count;
		this.SelectTool(this.selectedToolIndex);
		this.UpdateUI();
	}

	// Token: 0x0600336C RID: 13164 RVA: 0x001195D5 File Offset: 0x001177D5
	public void UpdateUI()
	{
		this.UpdateToolName();
		this.UpdateUpgradeTitles();
		this.UpdateLocked();
		this.UpdateRequiredLevel();
		this.UpdateCost();
		this.UpdateResearchPoints(this.toolProgressionManager.GetNumberOfResearchPoints());
	}

	// Token: 0x0600336D RID: 13165 RVA: 0x00119608 File Offset: 0x00117808
	public void SelectUpgrade(int UpgradeIndex)
	{
		if (this.toolProgressionManager == null)
		{
			return;
		}
		this.selectedUpgradeIndex = UpgradeIndex;
		if (this.selectedToolUpgrades.Count > this.selectedUpgradeIndex)
		{
			this.currentlySelectedToolUpgrade = this.selectedToolUpgrades[this.selectedUpgradeIndex];
			this.currentlySelectedUpgradeMetadata = this.currentlySelectedToolUpgrade.partMetadata;
			this.SetUpgradeTextColors(this.selectedUpgradeIndex);
			this.UpdateDescriptionText(this.currentlySelectedUpgradeMetadata.description);
		}
		this.UpdateUI();
	}

	// Token: 0x0600336E RID: 13166 RVA: 0x0011968C File Offset: 0x0011788C
	private void SetUpgradeTextColors(int index)
	{
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			this.UpgradeButton[i].isOn = false;
			this.UpgradeButton[i].UpdateColor();
		}
		this.UpgradeButton[index].isOn = true;
		this.UpgradeButton[index].UpdateColor();
	}

	// Token: 0x0600336F RID: 13167 RVA: 0x001196E4 File Offset: 0x001178E4
	private void UpdateUpgradeTitles()
	{
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this.totalTools >= this.selectedToolIndex && this.selectedToolUpgrades.Count > i)
			{
				this.UpgradeTitlesText[i].text = this.selectedToolUpgrades[i].partMetadata.name;
			}
			else
			{
				this.UpgradeTitlesText[i].text = null;
			}
		}
	}

	// Token: 0x06003370 RID: 13168 RVA: 0x00119754 File Offset: 0x00117954
	public void UpdateLocked()
	{
		if (this.currentlySelectedToolUpgrade.unlocked)
		{
			this.UnlockedText.color = this.unlockedToolColor;
			this.UnlockedText.text = "UNLOCKED";
		}
		else
		{
			this.UnlockedText.color = this.lockedToolColor;
			this.UnlockedText.text = "LOCKED";
		}
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this.totalTools >= this.selectedToolIndex && this.selectedToolUpgrades.Count > i)
			{
				bool unlocked = this.selectedToolUpgrades[i].unlocked;
				this.UpgradeTitlesText[i].color = (unlocked ? this.unlockedToolColor : this.lockedToolColor);
				this.LockedImage[i].gameObject.SetActive(!unlocked);
			}
			else
			{
				this.UpgradeTitlesText[i].color = Color.black;
				this.LockedImage[i].gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06003371 RID: 13169 RVA: 0x00119854 File Offset: 0x00117A54
	public void UpdateRequiredLevel()
	{
		int requiredEmployeeLevel = this.toolProgressionManager.GetRequiredEmployeeLevel(this.currentlySelectedToolUpgrade.requiredEmployeeLevel);
		string titleNameFromLevel = GhostReactorProgression.GetTitleNameFromLevel(requiredEmployeeLevel);
		int num = 0;
		GRPlayer grplayer = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);
		if (grplayer != null)
		{
			num = GhostReactorProgression.GetTitleLevel(grplayer.CurrentProgression.redeemedPoints);
		}
		string titleNameFromLevel2 = GhostReactorProgression.GetTitleNameFromLevel(num);
		this.RequiredLevelText.text = string.Format(this._requiredLevelString, titleNameFromLevel);
		this.LevelText.text = string.Format(this._levelString, titleNameFromLevel2);
		this.RequiredLevelText.color = ((num >= requiredEmployeeLevel) ? this.unlockedToolColor : this.lockedToolColor);
	}

	// Token: 0x06003372 RID: 13170 RVA: 0x001198FF File Offset: 0x00117AFF
	public void UpdateDescriptionText(string description)
	{
		this.DescriptionText.text = description;
	}

	// Token: 0x06003373 RID: 13171 RVA: 0x00119910 File Offset: 0x00117B10
	public void UpdateCost()
	{
		if (this.selectedToolUpgrades != null && this.selectedToolUpgrades.Count > 0 && this.selectedToolUpgrades.Count > this.selectedUpgradeIndex)
		{
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			int researchCost = this.selectedToolUpgrades[this.selectedUpgradeIndex].researchCost;
			this.CostText.text = string.Format(this._costString, researchCost);
			this.CostText.color = ((numberOfResearchPoints >= researchCost) ? this.unlockedToolColor : this.lockedToolColor);
		}
	}

	// Token: 0x06003374 RID: 13172 RVA: 0x001199A2 File Offset: 0x00117BA2
	public void UpdateToolName()
	{
		if (this.supportedTools.Count > 0)
		{
			this.ToolNameText.text = GRUtils.GetToolName(this.supportedTools[this.selectedToolIndex]);
		}
	}

	// Token: 0x06003375 RID: 13173 RVA: 0x001199D3 File Offset: 0x00117BD3
	public void UpdateResearchPoints(int ResearchPoints)
	{
		this.ResearchPointsTex.text = string.Format(this._researchPointsString, ResearchPoints);
	}

	// Token: 0x06003376 RID: 13174 RVA: 0x001199F1 File Offset: 0x00117BF1
	public void MFDButton0Pressed()
	{
		this.SelectUpgrade(0);
	}

	// Token: 0x06003377 RID: 13175 RVA: 0x001199FA File Offset: 0x00117BFA
	public void MFDButton1Pressed()
	{
		this.SelectUpgrade(1);
	}

	// Token: 0x06003378 RID: 13176 RVA: 0x00119A03 File Offset: 0x00117C03
	public void MFDButton2Pressed()
	{
		this.SelectUpgrade(2);
	}

	// Token: 0x06003379 RID: 13177 RVA: 0x00119A0C File Offset: 0x00117C0C
	public void MFDButton3Pressed()
	{
		this.SelectUpgrade(3);
	}

	// Token: 0x0600337A RID: 13178 RVA: 0x00119A15 File Offset: 0x00117C15
	public void MFDButton4Pressed()
	{
		this.SelectUpgrade(4);
	}

	// Token: 0x0600337B RID: 13179 RVA: 0x00119A1E File Offset: 0x00117C1E
	public void MFDButton5Pressed()
	{
		this.SelectUpgrade(5);
	}

	// Token: 0x0600337C RID: 13180 RVA: 0x00119A27 File Offset: 0x00117C27
	public void NextToolButtonPressed()
	{
		this.selectedToolIndex = (this.selectedToolIndex + 1) % this.totalTools;
		this.SelectTool(this.selectedToolIndex);
	}

	// Token: 0x0600337D RID: 13181 RVA: 0x00119A4A File Offset: 0x00117C4A
	public void PreviousToolButtonPressed()
	{
		this.selectedToolIndex = (this.selectedToolIndex - 1).PositiveModulo(this.totalTools);
		this.SelectTool(this.selectedToolIndex);
	}

	// Token: 0x0600337E RID: 13182 RVA: 0x00119A71 File Offset: 0x00117C71
	public void UpgradeButtonPressed()
	{
		UnityEvent onSucceeded = this.scanner.onSucceeded;
		if (onSucceeded != null)
		{
			onSucceeded.Invoke();
		}
		GhostReactorProgression.instance.UnlockProgressionTreeNode(this.toolProgressionManager.GetTreeId(), this.currentlySelectedToolUpgrade.id, this.reactor);
	}

	// Token: 0x0600337F RID: 13183 RVA: 0x00119AAF File Offset: 0x00117CAF
	public void ResearchCompleted(bool success, string researchID)
	{
		this.UpdateUI();
	}

	// Token: 0x040042B9 RID: 17081
	public Color selectedUpgradeColor = Color.yellow;

	// Token: 0x040042BA RID: 17082
	public Color unselectedUpgradeColor = Color.black;

	// Token: 0x040042BB RID: 17083
	public Color lockedToolColor = Color.red;

	// Token: 0x040042BC RID: 17084
	public Color unlockedToolColor = Color.green;

	// Token: 0x040042BD RID: 17085
	private int selectedUpgradeIndex;

	// Token: 0x040042BE RID: 17086
	[SerializeField]
	private IDCardScanner scanner;

	// Token: 0x040042BF RID: 17087
	[SerializeField]
	private TMP_Text BonusText;

	// Token: 0x040042C0 RID: 17088
	[SerializeField]
	private TMP_Text CostText;

	// Token: 0x040042C1 RID: 17089
	[SerializeField]
	private TMP_Text DescriptionText;

	// Token: 0x040042C2 RID: 17090
	[SerializeField]
	private TMP_Text LevelText;

	// Token: 0x040042C3 RID: 17091
	[SerializeField]
	private TMP_Text ResearchPointsTex;

	// Token: 0x040042C4 RID: 17092
	[SerializeField]
	private TMP_Text RequiredLevelText;

	// Token: 0x040042C5 RID: 17093
	[SerializeField]
	private TMP_Text ToolNameText;

	// Token: 0x040042C6 RID: 17094
	[SerializeField]
	private TMP_Text UnlockedText;

	// Token: 0x040042C7 RID: 17095
	[SerializeField]
	private TMP_Text[] UpgradePointerText;

	// Token: 0x040042C8 RID: 17096
	[SerializeField]
	private TMP_Text[] UpgradeTitlesText;

	// Token: 0x040042C9 RID: 17097
	[SerializeField]
	private Image[] LockedImage;

	// Token: 0x040042CA RID: 17098
	[SerializeField]
	private GorillaPressableButton[] UpgradeButton;

	// Token: 0x040042CB RID: 17099
	private string _costString;

	// Token: 0x040042CC RID: 17100
	private string _levelString;

	// Token: 0x040042CD RID: 17101
	private string _researchPointsString;

	// Token: 0x040042CE RID: 17102
	private string _requiredLevelString;

	// Token: 0x040042CF RID: 17103
	private int selectedToolIndex;

	// Token: 0x040042D0 RID: 17104
	private int totalTools;

	// Token: 0x040042D1 RID: 17105
	[NonSerialized]
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x040042D2 RID: 17106
	[NonSerialized]
	private List<GRTool.GRToolType> supportedTools = new List<GRTool.GRToolType>();

	// Token: 0x040042D3 RID: 17107
	[NonSerialized]
	private List<GRToolProgressionTree.GRToolProgressionNode> selectedToolUpgrades = new List<GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x040042D4 RID: 17108
	[NonSerialized]
	private GRToolProgressionTree.GRToolProgressionNode currentlySelectedToolUpgrade = new GRToolProgressionTree.GRToolProgressionNode();

	// Token: 0x040042D5 RID: 17109
	[NonSerialized]
	private GRToolProgressionManager.ToolProgressionMetaData currentlySelectedUpgradeMetadata = new GRToolProgressionManager.ToolProgressionMetaData();

	// Token: 0x040042D6 RID: 17110
	[NonSerialized]
	private GhostReactor reactor;
}
