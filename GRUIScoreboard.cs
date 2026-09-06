using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000835 RID: 2101
public class GRUIScoreboard : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003610 RID: 13840 RVA: 0x0012AA82 File Offset: 0x00128C82
	public void SliceUpdate()
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			this.Refresh(GhostReactor.instance.vrRigs);
		}
	}

	// Token: 0x06003611 RID: 13841 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003612 RID: 13842 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003613 RID: 13843 RVA: 0x0012AAA0 File Offset: 0x00128CA0
	public void Refresh(List<VRRig> vrRigs)
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			GhostReactor.instance.shiftManager.CalculatePlayerPercentages();
		}
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (!(this.entries[i] == null))
			{
				if (i < vrRigs.Count && vrRigs[i] != null && vrRigs[i].OwningNetPlayer != null)
				{
					this.entries[i].gameObject.SetActive(true);
					this.entries[i].Setup(vrRigs[i], vrRigs[i].OwningNetPlayer.ActorNumber, this.currentScreen);
				}
				else
				{
					this.entries[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06003614 RID: 13844 RVA: 0x0012AB80 File Offset: 0x00128D80
	public void SwitchToScreen(GRUIScoreboard.ScoreboardScreen screenType)
	{
		this.currentScreen = screenType;
		GRUIScoreboard.ScoreboardScreen scoreboardScreen = this.currentScreen;
		if (scoreboardScreen == GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			this.infoTextParent.SetActive(true);
			this.calcTextParent.SetActive(false);
			this.buttonText.text = "SHOW CUT CALC";
			return;
		}
		if (scoreboardScreen != GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			return;
		}
		this.infoTextParent.SetActive(false);
		this.calcTextParent.SetActive(true);
		this.buttonText.text = "SHOW INFO";
	}

	// Token: 0x06003615 RID: 13845 RVA: 0x0012ABF4 File Offset: 0x00128DF4
	public void SwitchState()
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			this.SwitchToScreen(GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation);
		}
		else
		{
			this.SwitchToScreen(GRUIScoreboard.ScoreboardScreen.DefaultInfo);
		}
		this.Refresh(GhostReactor.instance.vrRigs);
		GhostReactor.instance.UpdateRemoteScoreboardScreen(this.currentScreen);
	}

	// Token: 0x06003616 RID: 13846 RVA: 0x0012AC2E File Offset: 0x00128E2E
	public static bool ValidPage(GRUIScoreboard.ScoreboardScreen screen)
	{
		return screen == GRUIScoreboard.ScoreboardScreen.DefaultInfo || screen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation;
	}

	// Token: 0x040046B2 RID: 18098
	public List<GRUIScoreboardEntry> entries;

	// Token: 0x040046B3 RID: 18099
	public TMP_Text total;

	// Token: 0x040046B4 RID: 18100
	public TMP_Text buttonText;

	// Token: 0x040046B5 RID: 18101
	public GRUIScoreboard.ScoreboardScreen currentScreen;

	// Token: 0x040046B6 RID: 18102
	public GameObject infoTextParent;

	// Token: 0x040046B7 RID: 18103
	public GameObject calcTextParent;

	// Token: 0x02000836 RID: 2102
	public enum ScoreboardScreen
	{
		// Token: 0x040046B9 RID: 18105
		DefaultInfo,
		// Token: 0x040046BA RID: 18106
		ShiftCutCalculation
	}
}
