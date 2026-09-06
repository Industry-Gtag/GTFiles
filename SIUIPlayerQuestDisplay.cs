using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200017A RID: 378
public class SIUIPlayerQuestDisplay : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060009EC RID: 2540 RVA: 0x000355B0 File Offset: 0x000337B0
	public void RefreshDisplay()
	{
		SIPlayer siplayer = SIPlayer.Get(this.activePlayerActorNumber);
		bool flag = siplayer != null && siplayer.gamePlayer != null && siplayer.gamePlayer.rig != null && siplayer.gamePlayer.rig.Creator != null && this.activePlayerActorNumber > 0;
		if (!flag || !SIProgression.Instance.ClientReady)
		{
			if (this.activePlayer.activeSelf)
			{
				this.activePlayer.SetActive(false);
			}
			if (!this.waitingForPlayer.activeSelf)
			{
				this.waitingForPlayer.SetActive(true);
			}
			this.displayBackground.color = this.noPlayerColor;
			this.smallDisplayBackground.color = this.noPlayerColor;
			return;
		}
		if (this.activePlayer.activeSelf != flag)
		{
			this.activePlayer.SetActive(flag);
		}
		if (this.waitingForPlayer.activeSelf == flag)
		{
			this.waitingForPlayer.SetActive(!flag);
		}
		if (!flag)
		{
			this.displayBackground.color = this.noPlayerColor;
			this.smallDisplayBackground.color = this.noPlayerColor;
			return;
		}
		Color color = ((siplayer == SIPlayer.LocalPlayer) ? this.localPlayerColor : this.remotePlayerColor);
		this.displayBackground.color = color;
		this.smallDisplayBackground.color = color;
		string sanitizedNickName = siplayer.gamePlayer.rig.Creator.SanitizedNickName;
		if (this.lastNickName != sanitizedNickName)
		{
			this.playerName.text = sanitizedNickName;
		}
		this.lastNickName = sanitizedNickName;
		int num = siplayer.CurrentProgression.resourceArray[0];
		if (this.lastTechPoints != num)
		{
			this.playerTechPoints.text = string.Format("TECH POINTS: {0}", num);
		}
		this.lastTechPoints = num;
		bool flag2 = siplayer.HasLimitedResourceBeenDeposited(SIResource.LimitedDepositType.MonkeIdol);
		if (flag2 != this.monkeIdolIcon.enabled)
		{
			this.monkeIdolIcon.enabled = flag2;
		}
		int stashedQuests = siplayer.CurrentProgression.stashedQuests;
		if (this.lastStashedQuests != stashedQuests)
		{
			this.stashedQuestCount.text = string.Format("STASHED QUESTS: {0}/{1}", Mathf.Max(0, stashedQuests - 3), 6);
		}
		this.lastStashedQuests = stashedQuests;
		int stashedBonusPoints = siplayer.CurrentProgression.stashedBonusPoints;
		if (this.lastStashedBonusPoints != stashedBonusPoints)
		{
			this.stashedBonusPointCount.text = string.Format("STASHED BONUS: {0}/{1}", Mathf.Max(0, stashedBonusPoints - 1), 2);
		}
		this.lastStashedBonusPoints = stashedBonusPoints;
		int bonusProgress = siplayer.CurrentProgression.bonusProgress;
		if (this.lastBonusProgress != bonusProgress)
		{
			float num2 = Mathf.Clamp01((float)bonusProgress / 4f);
			this.sharedProgress.UpdateFillPercent(num2);
			this.sharedProgress.progressText.text = string.Format("{0:F0}%", num2 * 100f);
		}
		this.lastBonusProgress = bonusProgress;
		bool flag3 = siplayer.CurrentProgression.stashedBonusPoints > 0;
		if (this.bonusPointsInProgress.activeSelf != flag3)
		{
			this.bonusPointsInProgress.SetActive(flag3);
		}
		if (this.bonusPointsCompleted.activeSelf == flag3)
		{
			this.bonusPointsCompleted.SetActive(!flag3);
		}
		bool flag4 = siplayer.CurrentProgression.bonusProgress >= 4;
		if (this.collectBonusButton.activeSelf != flag4)
		{
			this.collectBonusButton.SetActive(flag4);
		}
		if (this.questEntries == null || siplayer.CurrentProgression.currentQuestIds == null || siplayer.CurrentProgression.currentQuestProgresses == null)
		{
			return;
		}
		for (int i = 0; i < this.questEntries.Length; i++)
		{
			this.ProcessQuestEntry(this.questEntries[i], siplayer.CurrentProgression.currentQuestIds[i], siplayer.CurrentProgression.currentQuestProgresses[i]);
		}
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x00035974 File Offset: 0x00033B74
	public void ProcessQuestEntry(SIUIPlayerQuestEntry entry, int questId, int questProgress)
	{
		if (SIProgression.Instance.questSourceList == null)
		{
			if (entry.questInfo.activeSelf)
			{
				entry.questInfo.SetActive(false);
			}
			if (!entry.noQuestAvailable.activeSelf)
			{
				entry.noQuestAvailable.SetActive(true);
			}
			if (entry.completeOverlay.activeSelf)
			{
				entry.completeOverlay.SetActive(false);
			}
			entry.lastQuestId = -1;
			entry.lastQuestProgress = -1;
			return;
		}
		RotatingQuest questById = SIProgression.Instance.questSourceList.GetQuestById(questId);
		bool flag = questId != -1 && questById != null;
		if (entry.completeOverlay.activeSelf && !flag)
		{
			entry.completeOverlay.SetActive(false);
		}
		if (entry.questInfo.activeSelf != flag)
		{
			entry.questInfo.SetActive(flag);
		}
		if (entry.noQuestAvailable.activeSelf == flag)
		{
			entry.noQuestAvailable.SetActive(!flag);
		}
		if (!flag)
		{
			entry.lastQuestId = -1;
			return;
		}
		if (questId != entry.lastQuestId)
		{
			entry.questDescription.text = questById.GetTextDescription();
		}
		if (entry.lastQuestProgress != questProgress || questId != entry.lastQuestId)
		{
			entry.progress.UpdateFillPercent((float)questProgress / (float)questById.requiredOccurenceCount);
			entry.progress.progressText.text = questProgress.ToString() + "/" + questById.requiredOccurenceCount.ToString();
		}
		if (entry.lastQuestId != -1 && entry.lastQuestId != questById.questID)
		{
			entry.newQuestTag.SetActive(true);
		}
		entry.lastQuestId = questById.questID;
		entry.lastQuestProgress = questProgress;
		bool flag2 = questProgress >= questById.requiredOccurenceCount;
		if (entry.completeOverlay.activeSelf != flag2)
		{
			entry.completeOverlay.SetActive(flag2);
		}
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x00035B2C File Offset: 0x00033D2C
	public void BonusPointCollectButtonPress()
	{
		if (this.activePlayerActorNumber == SIPlayer.LocalPlayer.ActorNr)
		{
			SIProgression.Instance.AttemptRedeemBonusPoint();
		}
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00035B4A File Offset: 0x00033D4A
	public void QuestPointCollectButtonPress(int questIndex)
	{
		if (this.activePlayerActorNumber == SIPlayer.LocalPlayer.ActorNr && SIPlayer.LocalPlayer.QuestAvailableToClaim(questIndex))
		{
			SIProgression.Instance.AttemptRedeemCompletedQuest(questIndex);
		}
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00035B76 File Offset: 0x00033D76
	void IGorillaSliceableSimple.SliceUpdate()
	{
		this.RefreshDisplay();
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x04000C2B RID: 3115
	public TextMeshProUGUI playerName;

	// Token: 0x04000C2C RID: 3116
	[FormerlySerializedAs("playerTestPoints")]
	public TextMeshProUGUI playerTechPoints;

	// Token: 0x04000C2D RID: 3117
	public TextMeshProUGUI stashedQuestCount;

	// Token: 0x04000C2E RID: 3118
	public TextMeshProUGUI stashedBonusPointCount;

	// Token: 0x04000C2F RID: 3119
	public Image displayBackground;

	// Token: 0x04000C30 RID: 3120
	public Image smallDisplayBackground;

	// Token: 0x04000C31 RID: 3121
	public Image monkeIdolIcon;

	// Token: 0x04000C32 RID: 3122
	public Color localPlayerColor;

	// Token: 0x04000C33 RID: 3123
	public Color remotePlayerColor;

	// Token: 0x04000C34 RID: 3124
	public Color noPlayerColor;

	// Token: 0x04000C35 RID: 3125
	public SIUIPlayerQuestEntry[] questEntries;

	// Token: 0x04000C36 RID: 3126
	public GameObject collectBonusButton;

	// Token: 0x04000C37 RID: 3127
	public GameObject bonusPointsInProgress;

	// Token: 0x04000C38 RID: 3128
	public GameObject bonusPointsCompleted;

	// Token: 0x04000C39 RID: 3129
	public SIUIProgressBar sharedProgress;

	// Token: 0x04000C3A RID: 3130
	public GameObject activePlayer;

	// Token: 0x04000C3B RID: 3131
	public GameObject waitingForPlayer;

	// Token: 0x04000C3C RID: 3132
	public int activePlayerActorNumber;

	// Token: 0x04000C3D RID: 3133
	private string lastNickName;

	// Token: 0x04000C3E RID: 3134
	private int lastStashedQuests = -1;

	// Token: 0x04000C3F RID: 3135
	private int lastStashedBonusPoints = -1;

	// Token: 0x04000C40 RID: 3136
	private int lastTechPoints = -1;

	// Token: 0x04000C41 RID: 3137
	private int lastBonusProgress = -1;
}
