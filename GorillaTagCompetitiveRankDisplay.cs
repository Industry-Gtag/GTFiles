using System;
using TMPro;
using UnityEngine;

// Token: 0x020008AF RID: 2223
public class GorillaTagCompetitiveRankDisplay : MonoBehaviour
{
	// Token: 0x06003A5E RID: 14942 RVA: 0x0013D412 File Offset: 0x0013B612
	private void OnEnable()
	{
		VRRig.LocalRig.OnRankedSubtierChanged += this.HandleRankedSubtierChanged;
		this.HandleRankedSubtierChanged(0, 0);
	}

	// Token: 0x06003A5F RID: 14943 RVA: 0x0013D432 File Offset: 0x0013B632
	private void OnDisable()
	{
		VRRig.LocalRig.OnRankedSubtierChanged -= this.HandleRankedSubtierChanged;
	}

	// Token: 0x06003A60 RID: 14944 RVA: 0x0013D44C File Offset: 0x0013B64C
	public void HandleRankedSubtierChanged(int questSubTier, int pcSubTier)
	{
		float currentELO = RankedProgressionManager.Instance.GetCurrentELO();
		int progressionRankIndex = RankedProgressionManager.Instance.GetProgressionRankIndex(currentELO);
		this.UpdateRankIcons(progressionRankIndex);
		this.UpdateRankProgress(RankedProgressionManager.Instance.GetProgressionRankProgress());
	}

	// Token: 0x06003A61 RID: 14945 RVA: 0x0013D488 File Offset: 0x0013B688
	private void UpdateRankIcons(int currentRank)
	{
		this.currentRankSprite.sprite = RankedProgressionManager.Instance.GetProgressionRankIcon(currentRank);
		this.currentRank_Name.text = RankedProgressionManager.Instance.GetProgressionRankName().ToUpper();
		bool flag = currentRank < RankedProgressionManager.Instance.MaxRank;
		bool flag2 = currentRank > 0;
		this.nextRankSprite.gameObject.SetActive(flag);
		this.nextText.gameObject.SetActive(flag);
		this.nextRank_Name.gameObject.SetActive(flag);
		if (flag)
		{
			this.nextRankSprite.sprite = RankedProgressionManager.Instance.GetNextProgressionRankIcon(currentRank);
			this.nextRank_Name.text = RankedProgressionManager.Instance.GetNextProgressionRankName(currentRank).ToUpper();
		}
		this.prevRankSprite.gameObject.SetActive(flag2);
		this.prevText.gameObject.SetActive(flag2);
		this.prevRank_Name.gameObject.SetActive(flag2);
		if (flag2)
		{
			this.prevRankSprite.sprite = RankedProgressionManager.Instance.GetPrevProgressionRankIcon(currentRank);
			this.prevRank_Name.text = RankedProgressionManager.Instance.GetPrevProgressionRankName(currentRank).ToUpper();
		}
	}

	// Token: 0x06003A62 RID: 14946 RVA: 0x0013D5A8 File Offset: 0x0013B7A8
	private void UpdateRankProgress(float percent)
	{
		percent = Mathf.Clamp01(percent);
		Vector2 size = this.progressBar.size;
		size.x = this.progressBarSize * percent;
		this.progressBar.size = size;
	}

	// Token: 0x04004A49 RID: 19017
	[SerializeField]
	private SpriteRenderer progressBar;

	// Token: 0x04004A4A RID: 19018
	[SerializeField]
	private float progressBarSize = 100f;

	// Token: 0x04004A4B RID: 19019
	[SerializeField]
	private SpriteRenderer currentRankSprite;

	// Token: 0x04004A4C RID: 19020
	[SerializeField]
	private SpriteRenderer prevRankSprite;

	// Token: 0x04004A4D RID: 19021
	[SerializeField]
	private SpriteRenderer nextRankSprite;

	// Token: 0x04004A4E RID: 19022
	[SerializeField]
	private TextMeshPro currentRank_Name;

	// Token: 0x04004A4F RID: 19023
	[SerializeField]
	private TextMeshPro prevText;

	// Token: 0x04004A50 RID: 19024
	[SerializeField]
	private TextMeshPro nextText;

	// Token: 0x04004A51 RID: 19025
	[SerializeField]
	private TextMeshPro prevRank_Name;

	// Token: 0x04004A52 RID: 19026
	[SerializeField]
	private TextMeshPro nextRank_Name;
}
