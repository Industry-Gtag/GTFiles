using System;
using TMPro;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class QuestDisplay : MonoBehaviour
{
	// Token: 0x1700019A RID: 410
	// (get) Token: 0x0600105E RID: 4190 RVA: 0x00057D9E File Offset: 0x00055F9E
	public bool IsChanged
	{
		get
		{
			return this.quest.lastChange > this._lastUpdate;
		}
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x00057DB4 File Offset: 0x00055FB4
	public void UpdateDisplay()
	{
		this.text.text = this.quest.GetTextDescription();
		if (this.quest.isQuestComplete)
		{
			this.progressDisplay.SetVisible(false);
		}
		else if (this.quest.requiredOccurenceCount > 1)
		{
			this.progressDisplay.SetProgress(this.quest.occurenceCount, this.quest.requiredOccurenceCount);
			this.progressDisplay.SetVisible(true);
		}
		else
		{
			this.progressDisplay.SetVisible(false);
		}
		this.UpdateCompletionIndicator();
		this._lastUpdate = Time.frameCount;
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x00057E4C File Offset: 0x0005604C
	private void UpdateCompletionIndicator()
	{
		bool isQuestComplete = this.quest.isQuestComplete;
		bool flag = !isQuestComplete && this.quest.requiredOccurenceCount == 1;
		this.dailyIncompleteIndicator.SetActive(this.quest.isDailyQuest && flag);
		this.dailyCompleteIndicator.SetActive(this.quest.isDailyQuest && isQuestComplete);
		this.weeklyIncompleteIndicator.SetActive(!this.quest.isDailyQuest && flag);
		this.weeklyCompleteIndicator.SetActive(!this.quest.isDailyQuest && isQuestComplete);
	}

	// Token: 0x0400138B RID: 5003
	[SerializeField]
	private ProgressDisplay progressDisplay;

	// Token: 0x0400138C RID: 5004
	[SerializeField]
	private TMP_Text text;

	// Token: 0x0400138D RID: 5005
	[SerializeField]
	private TMP_Text statusText;

	// Token: 0x0400138E RID: 5006
	[SerializeField]
	private GameObject dailyIncompleteIndicator;

	// Token: 0x0400138F RID: 5007
	[SerializeField]
	private GameObject dailyCompleteIndicator;

	// Token: 0x04001390 RID: 5008
	[SerializeField]
	private GameObject weeklyIncompleteIndicator;

	// Token: 0x04001391 RID: 5009
	[SerializeField]
	private GameObject weeklyCompleteIndicator;

	// Token: 0x04001392 RID: 5010
	[NonSerialized]
	public RotatingQuest quest;

	// Token: 0x04001393 RID: 5011
	private int _lastUpdate = -1;
}
