using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200024E RID: 590
public class MonkeBusinessStation : MonoBehaviour
{
	// Token: 0x06000FC3 RID: 4035 RVA: 0x00055920 File Offset: 0x00053B20
	private void OnEnable()
	{
		this.FindQuestManager();
		ProgressionController.OnQuestSelectionChanged += this.OnQuestSelectionChanged;
		ProgressionController.OnProgressEvent += this.OnProgress;
		ProgressionController.RequestProgressUpdate();
		RoomSystem.OnMonkePointsRedeemedReceived = (Action<NetPlayer, int>)Delegate.Combine(RoomSystem.OnMonkePointsRedeemedReceived, new Action<NetPlayer, int>(this.OnRemotePointsRedeemed));
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		this.UpdateCountdownTimers();
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x0005599C File Offset: 0x00053B9C
	private void OnDisable()
	{
		ProgressionController.OnQuestSelectionChanged -= this.OnQuestSelectionChanged;
		ProgressionController.OnProgressEvent -= this.OnProgress;
		RoomSystem.OnMonkePointsRedeemedReceived = (Action<NetPlayer, int>)Delegate.Remove(RoomSystem.OnMonkePointsRedeemedReceived, new Action<NetPlayer, int>(this.OnRemotePointsRedeemed));
		RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x00055A06 File Offset: 0x00053C06
	private void FindQuestManager()
	{
		if (!this._questManager)
		{
			this._questManager = Object.FindAnyObjectByType<RotatingQuestsManager>();
		}
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x00055A20 File Offset: 0x00053C20
	private void UpdateCountdownTimers()
	{
		this._dailyCountdown.SetCountdownTime(this._questManager.DailyQuestCountdown);
		this._weeklyCountdown.SetCountdownTime(this._questManager.WeeklyQuestCountdown);
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x00055A4E File Offset: 0x00053C4E
	private void OnQuestSelectionChanged()
	{
		this.UpdateCountdownTimers();
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x00055A56 File Offset: 0x00053C56
	private void OnProgress()
	{
		this.UpdateQuestStatus();
		this.UpdateProgressDisplays();
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x00055A64 File Offset: 0x00053C64
	private void UpdateProgressDisplays()
	{
		ValueTuple<int, int, int> progressionData = ProgressionController.GetProgressionData();
		int item = progressionData.Item1;
		int item2 = progressionData.Item2;
		this._weeklyProgress.SetProgress(item, ProgressionController.WeeklyCap);
		if (!this._isUpdatingPointCount)
		{
			this._unclaimedPoints.text = item2.ToString();
			this._claimButton.isOn = item2 > 0;
		}
		bool flag = item2 > 0;
		this._claimablePointsObject.SetActive(flag);
		this._noClaimablePointsObject.SetActive(!flag);
		this._badgeMount.position = (flag ? this._claimablePointsBadgePosition.position : this._noClaimablePointsBadgePosition.position);
		this._claimButton.gameObject.SetActive(flag);
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x00055B18 File Offset: 0x00053D18
	private void UpdateQuestStatus()
	{
		if (this._lastQuestChange >= RotatingQuestsManager.LastQuestChange)
		{
			return;
		}
		this.FindQuestManager();
		if (this._quests.Count == 0 || this._lastQuestDailyID != RotatingQuestsManager.LastQuestDailyID)
		{
			this.BuildQuestList();
		}
		foreach (QuestDisplay questDisplay in this._quests)
		{
			if (questDisplay.IsChanged)
			{
				questDisplay.UpdateDisplay();
			}
		}
		this._lastQuestChange = Time.frameCount;
		this._lastQuestDailyID = RotatingQuestsManager.LastQuestDailyID;
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x00055BBC File Offset: 0x00053DBC
	public void RedeemProgress()
	{
		if (this._claimButton.isOn)
		{
			this._isUpdatingPointCount = true;
			ValueTuple<int, int, int> progressionData = ProgressionController.GetProgressionData();
			int item = progressionData.Item2;
			int item2 = progressionData.Item3;
			this._tempUnclaimedPoints = item;
			this._tempTotalPoints = item2;
			this._claimButton.isOn = false;
			ProgressionController.RedeemProgress();
			RoomSystem.SendMonkePointsRedeemed(this._tempUnclaimedPoints);
			base.StartCoroutine(this.PerformPointRedemptionSequence());
		}
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x00055C26 File Offset: 0x00053E26
	private IEnumerator PerformPointRedemptionSequence()
	{
		while (this._tempUnclaimedPoints > 0)
		{
			this._tempUnclaimedPoints--;
			this._tempTotalPoints++;
			this._unclaimedPoints.text = this._tempUnclaimedPoints.ToString();
			if (this._tempUnclaimedPoints == 0)
			{
				this._audioSource.PlayOneShot(this._claimPointFinalSFX);
			}
			else
			{
				this._audioSource.PlayOneShot(this._claimPointDefaultSFX);
			}
			yield return new WaitForSeconds(this._claimDelayPerPoint);
		}
		this._isUpdatingPointCount = false;
		this.UpdateProgressDisplays();
		yield break;
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x00055C38 File Offset: 0x00053E38
	private void OnRemotePointsRedeemed(NetPlayer sender, int redeemedPointCount)
	{
		if (sender == null)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(sender, out rigContainer))
		{
			return;
		}
		if (!FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 10, (double)Time.unscaledTime))
		{
			return;
		}
		Coroutine coroutine;
		if (this.perPlayerRedemptionSequence.TryGetValue(sender, out coroutine))
		{
			if (coroutine != null)
			{
				base.StopCoroutine(coroutine);
			}
			this.perPlayerRedemptionSequence.Remove(sender);
		}
		if (base.gameObject.activeInHierarchy)
		{
			Coroutine coroutine2 = base.StartCoroutine(this.PerformRemotePointRedemptionSequence(sender, redeemedPointCount));
			this.perPlayerRedemptionSequence.Add(sender, coroutine2);
		}
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x00055CC4 File Offset: 0x00053EC4
	private void OnPlayerLeftRoom(NetPlayer player)
	{
		if (player == null)
		{
			return;
		}
		Coroutine coroutine;
		if (this.perPlayerRedemptionSequence.TryGetValue(player, out coroutine))
		{
			if (coroutine != null)
			{
				base.StopCoroutine(coroutine);
			}
			this.perPlayerRedemptionSequence.Remove(player);
		}
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x00055CFC File Offset: 0x00053EFC
	private IEnumerator PerformRemotePointRedemptionSequence(NetPlayer player, int redeemedPointCount)
	{
		while (redeemedPointCount > 0)
		{
			int num = redeemedPointCount;
			redeemedPointCount = num - 1;
			if (redeemedPointCount == 0)
			{
				this._audioSource.PlayOneShot(this._claimPointFinalSFX);
			}
			else
			{
				this._audioSource.PlayOneShot(this._claimPointDefaultSFX);
			}
			yield return new WaitForSeconds(this._claimDelayPerPoint);
		}
		this.perPlayerRedemptionSequence.Remove(player);
		yield break;
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00055D1C File Offset: 0x00053F1C
	private void BuildQuestList()
	{
		this.DestroyQuestList();
		RotatingQuestsManager.RotatingQuestList quests = this._questManager.quests;
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in quests.DailyQuests)
		{
			foreach (RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				if (rotatingQuest.isQuestActive)
				{
					QuestDisplay questDisplay = Object.Instantiate<QuestDisplay>(this._questDisplayPrefab, this._dailyQuestContainer);
					questDisplay.quest = rotatingQuest;
					this._quests.Add(questDisplay);
				}
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in quests.WeeklyQuests)
		{
			foreach (RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				if (rotatingQuest2.isQuestActive)
				{
					QuestDisplay questDisplay2 = Object.Instantiate<QuestDisplay>(this._questDisplayPrefab, this._weeklyQuestContainer);
					questDisplay2.quest = rotatingQuest2;
					this._quests.Add(questDisplay2);
				}
			}
		}
		foreach (QuestDisplay questDisplay3 in this._quests)
		{
			questDisplay3.UpdateDisplay();
		}
		if (!this._hasBuiltQuestList)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._questContainerParent);
			this._hasBuiltQuestList = true;
			return;
		}
		LayoutRebuilder.MarkLayoutForRebuild(this._questContainerParent);
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x00055EF0 File Offset: 0x000540F0
	private void DestroyQuestList()
	{
		MonkeBusinessStation.<DestroyQuestList>g__DestroyChildren|41_0(this._dailyQuestContainer);
		MonkeBusinessStation.<DestroyQuestList>g__DestroyChildren|41_0(this._weeklyQuestContainer);
		this._quests.Clear();
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x00055F40 File Offset: 0x00054140
	[CompilerGenerated]
	internal static void <DestroyQuestList>g__DestroyChildren|41_0(Transform parent)
	{
		for (int i = parent.childCount - 1; i >= 0; i--)
		{
			Object.Destroy(parent.GetChild(i).gameObject);
		}
	}

	// Token: 0x040012FB RID: 4859
	[SerializeField]
	private RectTransform _questContainerParent;

	// Token: 0x040012FC RID: 4860
	[SerializeField]
	private RectTransform _dailyQuestContainer;

	// Token: 0x040012FD RID: 4861
	[SerializeField]
	private RectTransform _weeklyQuestContainer;

	// Token: 0x040012FE RID: 4862
	[SerializeField]
	private QuestDisplay _questDisplayPrefab;

	// Token: 0x040012FF RID: 4863
	[SerializeField]
	private List<QuestDisplay> _quests;

	// Token: 0x04001300 RID: 4864
	[SerializeField]
	private ProgressDisplay _weeklyProgress;

	// Token: 0x04001301 RID: 4865
	[SerializeField]
	private TMP_Text _unclaimedPoints;

	// Token: 0x04001302 RID: 4866
	[SerializeField]
	private GorillaPressableButton _claimButton;

	// Token: 0x04001303 RID: 4867
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x04001304 RID: 4868
	[SerializeField]
	private GameObject _claimablePointsObject;

	// Token: 0x04001305 RID: 4869
	[SerializeField]
	private GameObject _noClaimablePointsObject;

	// Token: 0x04001306 RID: 4870
	[SerializeField]
	private Transform _claimablePointsBadgePosition;

	// Token: 0x04001307 RID: 4871
	[SerializeField]
	private Transform _noClaimablePointsBadgePosition;

	// Token: 0x04001308 RID: 4872
	[SerializeField]
	private Transform _badgeMount;

	// Token: 0x04001309 RID: 4873
	[Space]
	[SerializeField]
	private float _claimDelayPerPoint = 0.12f;

	// Token: 0x0400130A RID: 4874
	[SerializeField]
	private AudioClip _claimPointDefaultSFX;

	// Token: 0x0400130B RID: 4875
	[SerializeField]
	private AudioClip _claimPointFinalSFX;

	// Token: 0x0400130C RID: 4876
	[Header("Quest Timers")]
	[SerializeField]
	private CountdownText _dailyCountdown;

	// Token: 0x0400130D RID: 4877
	[SerializeField]
	private CountdownText _weeklyCountdown;

	// Token: 0x0400130E RID: 4878
	private RotatingQuestsManager _questManager;

	// Token: 0x0400130F RID: 4879
	private int _lastQuestChange = -1;

	// Token: 0x04001310 RID: 4880
	private int _lastQuestDailyID = -1;

	// Token: 0x04001311 RID: 4881
	private bool _isUpdatingPointCount;

	// Token: 0x04001312 RID: 4882
	private int _tempUnclaimedPoints;

	// Token: 0x04001313 RID: 4883
	private int _tempTotalPoints;

	// Token: 0x04001314 RID: 4884
	private bool _hasBuiltQuestList;

	// Token: 0x04001315 RID: 4885
	private Dictionary<NetPlayer, Coroutine> perPlayerRedemptionSequence = new Dictionary<NetPlayer, Coroutine>();
}
