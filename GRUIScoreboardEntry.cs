using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000837 RID: 2103
public class GRUIScoreboardEntry : MonoBehaviour
{
	// Token: 0x06003618 RID: 13848 RVA: 0x0012AC39 File Offset: 0x00128E39
	public void Setup(VRRig vrRig, int playerActorId, GRUIScoreboard.ScoreboardScreen screenType)
	{
		this.playerActorId = playerActorId;
		this.Refresh(vrRig, screenType);
	}

	// Token: 0x06003619 RID: 13849 RVA: 0x0012AC4C File Offset: 0x00128E4C
	private void Refresh(VRRig vrRig, GRUIScoreboard.ScoreboardScreen screenType)
	{
		GRPlayer grplayer = GRPlayer.Get(vrRig);
		if (!(vrRig != null) || !(grplayer != null))
		{
			this.playerNameLabel.text = "";
			this.playerCurrencyLabel.text = "";
			this.playerTitleLabel.text = "";
			this.playerCutLabel.text = "";
			this.currencySet = 0;
			return;
		}
		if (!this.playerNameLabel.text.Equals(vrRig.playerNameVisible))
		{
			this.playerNameLabel.text = vrRig.playerNameVisible;
		}
		if (screenType != GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			if (screenType == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
			{
				this.defaultUIParent.SetActive(false);
				this.shiftCutParent.SetActive(true);
				if (GhostReactor.instance.shiftManager != null && (GhostReactor.instance.shiftManager.ShiftActive || GhostReactor.instance.shiftManager.ShiftTotalEarned >= 0))
				{
					int num = Mathf.FloorToInt(grplayer.ShiftPlayTime / 60f);
					int num2 = Mathf.FloorToInt(grplayer.ShiftPlayTime - (float)(num * 60));
					this.playerTimeLabel.text = string.Format("{0:00}:{1:00}", num, num2);
					this.playerPercentageLabel.text = "%" + Mathf.Floor(grplayer.ShiftPlayTime / GhostReactor.instance.shiftManager.TotalPlayTime * 100f).ToString();
				}
				else
				{
					this.playerTimeLabel.text = "n/a";
					this.playerPercentageLabel.text = "n/a";
				}
				this.playerTitleLabel.text = this.titleSet;
			}
		}
		else
		{
			this.defaultUIParent.SetActive(true);
			this.shiftCutParent.SetActive(false);
			if (grplayer.ShiftCredits != this.currencySet)
			{
				this.currencySet = grplayer.ShiftCredits;
				this.playerCurrencyLabel.text = this.currencySet.ToString();
			}
			string titleNameAndGrade = GhostReactorProgression.GetTitleNameAndGrade(grplayer.CurrentProgression.redeemedPoints);
			if (titleNameAndGrade != this.titleSet)
			{
				this.titleSet = titleNameAndGrade;
				this.playerTitleLabel.text = this.titleSet;
			}
		}
		if (GhostReactor.instance.shiftManager == null || GhostReactor.instance.shiftManager.ShiftActive)
		{
			this.playerCutLabel.text = "-";
			return;
		}
		this.playerCutLabel.text = grplayer.LastShiftCut.ToString();
	}

	// Token: 0x040046BB RID: 18107
	[SerializeField]
	private TMP_Text playerNameLabel;

	// Token: 0x040046BC RID: 18108
	[SerializeField]
	private TMP_Text playerCutLabel;

	// Token: 0x040046BD RID: 18109
	public GameObject defaultUIParent;

	// Token: 0x040046BE RID: 18110
	[SerializeField]
	private TMP_Text playerTitleLabel;

	// Token: 0x040046BF RID: 18111
	[SerializeField]
	private TMP_Text playerCurrencyLabel;

	// Token: 0x040046C0 RID: 18112
	public GameObject shiftCutParent;

	// Token: 0x040046C1 RID: 18113
	[SerializeField]
	private TMP_Text playerTimeLabel;

	// Token: 0x040046C2 RID: 18114
	[SerializeField]
	private TMP_Text playerPercentageLabel;

	// Token: 0x040046C3 RID: 18115
	private int playerActorId = -1;

	// Token: 0x040046C4 RID: 18116
	private int currencySet = -1;

	// Token: 0x040046C5 RID: 18117
	private string titleSet = "";
}
