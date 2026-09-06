using System;
using TMPro;
using UnityEngine;

// Token: 0x020008B3 RID: 2227
public class GorillaTagCompetitiveScoreboardLine : MonoBehaviour
{
	// Token: 0x06003A70 RID: 14960 RVA: 0x0013D998 File Offset: 0x0013BB98
	public void SetPlayer(string playerName, Sprite icon)
	{
		this.playerNameDisplay.text = playerName;
		this.rankSprite.sprite = icon;
	}

	// Token: 0x06003A71 RID: 14961 RVA: 0x0013D9B4 File Offset: 0x0013BBB4
	public void SetScore(float untaggedTime, int tagCount)
	{
		int num = Mathf.FloorToInt(untaggedTime);
		int num2 = num / 60;
		int num3 = num % 60;
		this.untaggedTimeDisplay.text = string.Format("{0}:{1:D2}", num2, num3);
		this.tagCountDisplay.text = tagCount.ToString();
	}

	// Token: 0x06003A72 RID: 14962 RVA: 0x0013DA03 File Offset: 0x0013BC03
	public void SetPredictedResult(GorillaTagCompetitiveScoreboard.PredictedResult result)
	{
		this.resultSprite.sprite = this.resultSprites[(int)result];
	}

	// Token: 0x06003A73 RID: 14963 RVA: 0x0013DA18 File Offset: 0x0013BC18
	public void DisplayPredictedResults(bool bShow)
	{
		this.resultSprite.gameObject.SetActive(bShow);
	}

	// Token: 0x06003A74 RID: 14964 RVA: 0x0013DA2B File Offset: 0x0013BC2B
	public void SetInfected(bool infected)
	{
		this.playerNameDisplay.color = (infected ? Color.red : Color.white);
	}

	// Token: 0x04004A66 RID: 19046
	public SpriteRenderer rankSprite;

	// Token: 0x04004A67 RID: 19047
	public TMP_Text playerNameDisplay;

	// Token: 0x04004A68 RID: 19048
	public TMP_Text untaggedTimeDisplay;

	// Token: 0x04004A69 RID: 19049
	public TMP_Text tagCountDisplay;

	// Token: 0x04004A6A RID: 19050
	public SpriteRenderer resultSprite;

	// Token: 0x04004A6B RID: 19051
	public Sprite[] resultSprites;
}
