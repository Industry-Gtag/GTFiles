using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000723 RID: 1827
public class GhostReactorShiftManager : MonoBehaviourTick
{
	// Token: 0x17000477 RID: 1143
	// (get) Token: 0x06002E70 RID: 11888 RVA: 0x000FD694 File Offset: 0x000FB894
	public int ShiftTotalEarned
	{
		get
		{
			return this.shiftTotalEarned;
		}
	}

	// Token: 0x17000478 RID: 1144
	// (get) Token: 0x06002E71 RID: 11889 RVA: 0x000FD69C File Offset: 0x000FB89C
	public bool ShiftActive
	{
		get
		{
			return this.shiftStarted;
		}
	}

	// Token: 0x17000479 RID: 1145
	// (get) Token: 0x06002E72 RID: 11890 RVA: 0x000FD6A4 File Offset: 0x000FB8A4
	public double ShiftStartNetworkTime
	{
		get
		{
			return this.shiftStartNetworkTime;
		}
	}

	// Token: 0x1700047A RID: 1146
	// (get) Token: 0x06002E73 RID: 11891 RVA: 0x000FD6AC File Offset: 0x000FB8AC
	public bool LocalPlayerInside
	{
		get
		{
			return this.localPlayerInside;
		}
	}

	// Token: 0x1700047B RID: 1147
	// (get) Token: 0x06002E74 RID: 11892 RVA: 0x000FD6B4 File Offset: 0x000FB8B4
	public float TotalPlayTime
	{
		get
		{
			return this.totalPlayTime;
		}
	}

	// Token: 0x1700047C RID: 1148
	// (get) Token: 0x06002E75 RID: 11893 RVA: 0x000FD6BC File Offset: 0x000FB8BC
	public string ShiftId
	{
		get
		{
			return this.gameIdGuid;
		}
	}

	// Token: 0x06002E76 RID: 11894 RVA: 0x000FD6C4 File Offset: 0x000FB8C4
	public void SetShiftId(string shiftId)
	{
		this.gameIdGuid = shiftId;
	}

	// Token: 0x1700047D RID: 1149
	// (get) Token: 0x06002E77 RID: 11895 RVA: 0x000FD6CD File Offset: 0x000FB8CD
	// (set) Token: 0x06002E78 RID: 11896 RVA: 0x000FD6D5 File Offset: 0x000FB8D5
	public GhostReactorShiftManager.State ShiftState { get; private set; }

	// Token: 0x06002E79 RID: 11897 RVA: 0x000FD6DE File Offset: 0x000FB8DE
	public void Init(GhostReactorManager grManager)
	{
		this.grManager = grManager;
		this.SetState(GhostReactorShiftManager.State.WaitingForConnect, true);
		this.depthDisplay.Setup();
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x000FD6FC File Offset: 0x000FB8FC
	public void RefreshShiftStatsDisplay()
	{
		this.shiftStatsText.text = string.Concat(new string[]
		{
			"\n\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.CoresCollected).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.SentientCoresCollected).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths).ToString("D2")
		});
		this.depthDisplay.RefreshObjectives();
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x000FD7BA File Offset: 0x000FB9BA
	public void StartShiftButtonPressed()
	{
		this.RequestShiftStart();
	}

	// Token: 0x06002E7C RID: 11900 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void RequestShiftStart()
	{
	}

	// Token: 0x06002E7D RID: 11901 RVA: 0x000FD7C2 File Offset: 0x000FB9C2
	public void EndShift()
	{
		this.grManager.SendRequestShiftEndRPC();
	}

	// Token: 0x06002E7E RID: 11902 RVA: 0x000FD7CF File Offset: 0x000FB9CF
	public void ClearEntities()
	{
		Debug.LogError("Need to re-implement whatever this was doing");
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x000FD7DC File Offset: 0x000FB9DC
	public void RefreshShiftTimer()
	{
		if (this.shiftTimerText != null)
		{
			this.shiftTimerText.text = Mathf.FloorToInt(this.shiftDurationMinutes).ToString("D2") + ":00";
		}
	}

	// Token: 0x06002E80 RID: 11904 RVA: 0x000FD824 File Offset: 0x000FBA24
	public void UpdateLogoAnimations(List<TMP_Text> frames)
	{
		float num = 300f;
		float num2 = 0.5f;
		double time = PhotonNetwork.Time;
		if (frames.Count < 4)
		{
			return;
		}
		if (this.lastReactorLogoAnimationTime + (double)num < time || time < this.lastReactorLogoAnimationTime)
		{
			this.isPlayingLogoAnimation = true;
			this.lastReactorLogoAnimationTime = time;
		}
		if (this.isPlayingLogoAnimation)
		{
			if (this.lastReactorLogoAnimationTime + (double)num2 < time)
			{
				this.isPlayingLogoAnimation = false;
			}
			float num3 = Mathf.Clamp01((float)(time - this.lastReactorLogoAnimationTime) / num2) * 3.1415925f;
			int num4 = (int)(3.5f - Mathf.Abs(Mathf.Cos(num3) * 3f));
			if (!this.isPlayingLogoAnimation)
			{
				num4 = 0;
			}
			if (this.lastReactorLogoAnimFrame != num4)
			{
				frames[this.lastReactorLogoAnimFrame].gameObject.SetActive(false);
				frames[num4].gameObject.SetActive(true);
				this.lastReactorLogoAnimFrame = num4;
			}
			return;
		}
	}

	// Token: 0x06002E81 RID: 11905 RVA: 0x000FD908 File Offset: 0x000FBB08
	public void UpdateReactorDisplayMainShared(float countDownTotal)
	{
		if (this.reactorTextMain == null)
		{
			return;
		}
		double time = PhotonNetwork.Time;
		float num = 0.5f;
		if (this.lastReactorDisplayUpdate < time && this.lastReactorDisplayUpdate + (double)num > time)
		{
			return;
		}
		this.lastReactorDisplayUpdate = time;
		this.cachedStringBuilder.Clear();
		int num2 = Mathf.FloorToInt(countDownTotal / 60f);
		int num3 = Mathf.FloorToInt(countDownTotal % 60f);
		switch (this.ShiftState)
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000 + 1000));
			this.cachedStringBuilder.AppendLine("STAND BY");
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + 1000);
			break;
		case GhostReactorShiftManager.State.ShiftActive:
		{
			int shiftStat = this.shiftStats.GetShiftStat(GRShiftStatType.CoresCollected);
			int num4 = this.coresRequiredToDelveDeeper;
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + 1000);
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000 + 1000));
			this.cachedStringBuilder.AppendLine("ANOMALY COLLAPSE IN " + num2.ToString("D2") + ":" + num3.ToString("D2"));
			if (shiftStat >= num4)
			{
				this.cachedStringBuilder.Append("\nPOWER REQUIREMENTS MET\n");
			}
			else
			{
				this.cachedStringBuilder.Append(string.Format("\nCORES REQUIRED ({0}/{1})\n", shiftStat, num4));
			}
			int num5 = (int)((float)shiftStat / (float)num4 * 30f);
			if (shiftStat > 1 && num5 == 0)
			{
				num5 = 1;
			}
			int num6 = num5 / 3;
			int num7 = num5 - num6 * 3;
			for (int i = 0; i < 10; i++)
			{
				if (i < num6)
				{
					this.cachedStringBuilder.Append("▐█");
				}
				else if (i > num6 || num7 == 0)
				{
					this.cachedStringBuilder.Append(" ░");
				}
				else if (num7 == 1)
				{
					this.cachedStringBuilder.Append("▐░");
				}
				else
				{
					this.cachedStringBuilder.Append("▐▌");
				}
			}
			this.cachedStringBuilder.Append("\n");
			if (shiftStat > 0)
			{
				this.cachedStringBuilder.Append(string.Format("\nTOTAL BONUS EARNED: +⑭{0}", shiftStat * 5));
			}
			break;
		}
		case GhostReactorShiftManager.State.PreparingToDrill:
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000));
			this.cachedStringBuilder.AppendLine("STAND BY");
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + 1000);
			break;
		case GhostReactorShiftManager.State.Drilling:
		{
			int num8 = (int)((time - this.stateStartTime) / (double)this.GetDrillingDuration() * 1000.0);
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000 + num8));
			this.cachedStringBuilder.AppendLine("DRILLING");
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + num8);
			break;
		}
		}
		this.reactorTextMain.text = this.cachedStringBuilder.ToString();
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x000FDD04 File Offset: 0x000FBF04
	public void OnShiftStarted(string gameId, double shiftStartTime, bool wasPlayerInAtStart, bool isFirstShift)
	{
		this.gameIdGuid = gameId;
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (!this.shiftStarted && grplayer != null)
		{
			float num = (float)(PhotonNetwork.Time - shiftStartTime);
			grplayer.ResetTelemetryTracking(this.gameIdGuid, num);
			grplayer.IncrementShiftsPlayed(1);
			grplayer.SendFloorStartedTelemetry(num, wasPlayerInAtStart, this.reactor.GetDepthLevel(), this.reactor.GetCurrLevelGenConfig().name, "");
			if (grplayer.isFirstShift)
			{
				grplayer.SendGameStartedTelemetry(num, wasPlayerInAtStart, this.reactor.GetDepthLevel());
				grplayer.gameStartTime = (float)PhotonNetwork.Time;
			}
		}
		this.shiftStarted = true;
		this.shiftJustStarted = true;
		this.shiftStartNetworkTime = shiftStartTime;
		this.frontGate.OpenGate();
		this.ringTransform.gameObject.SetActive(false);
		this.anomalyLoop1.Stop();
		this.anomalyLoop2.Stop();
		this.anomalyLoop3.Stop();
		this.anomalyAlert.Stop();
		this.gateBlockerTransform.gameObject.SetActive(false);
		this.prevCountDownTotal = this.shiftDurationMinutes * 60f;
		this.shiftTotalEarned = -1;
		this.authorizedToDelveDeeper = false;
		this.ResetJoinTimes();
		this.reactor.RefreshScoreboards();
		this.reactor.RefreshDepth();
		this.isRoomClosed = false;
		if (grplayer != null)
		{
			grplayer.RefreshPlayerVisuals();
		}
	}

	// Token: 0x06002E83 RID: 11907 RVA: 0x000FDE60 File Offset: 0x000FC060
	public void OnShiftEnded(double shiftEndTime, bool isShiftActuallyEnding, ZoneClearReason zoneClearReason = ZoneClearReason.JoinZone)
	{
		if (this.shiftStarted)
		{
			GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
			if (component != null)
			{
				component.SendFloorEndedTelemetry(isShiftActuallyEnding, (float)this.shiftStartNetworkTime, zoneClearReason, this.reactor.GetDepthLevel(), this.reactor.GetCurrLevelGenConfig().name, "", this.authorizedToDelveDeeper, ((this.reactor.GetDepthLevel() + 1) / 5).ToString(), this.authorizedToDelveDeeper ? (10 * this.reactor.GetDepthLevel()) : 0);
			}
		}
		this.shiftStarted = false;
		this.shiftEndNetworkTime = shiftEndTime;
		this.RefreshShiftTimer();
		this.frontGate.CloseGate();
		this.ringTransform.gameObject.SetActive(false);
		this.anomalyLoop1.Stop();
		this.anomalyLoop2.Stop();
		this.anomalyLoop3.Stop();
		this.anomalyAlert.Stop();
		this.TeleportLocalPlayerIfOutOfBounds();
		if (this.shiftEndNetworkTime > 0.0 && this.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) > this.shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths))
		{
			PlayerGameEvents.MiscEvent("GRShiftGoodKD", 1);
		}
		if (PhotonNetwork.InRoom && !NetworkSystem.Instance.SessionIsPrivate && this.grManager.IsAuthority())
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("ghostReactorShiftStarted", "false");
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
			this.isRoomClosed = false;
		}
	}

	// Token: 0x06002E84 RID: 11908 RVA: 0x000FDFD4 File Offset: 0x000FC1D4
	public override void Tick()
	{
		if (this.grManager == null)
		{
			return;
		}
		double num = PhotonNetwork.Time - this.shiftStartNetworkTime;
		float num2 = 60f * this.shiftDurationMinutes - (float)num;
		if (this.grManager.IsAuthority())
		{
			this.AuthorityUpdate(num2);
		}
		num2 = Mathf.Clamp(num2, 0f, 60f * this.shiftDurationMinutes);
		this.SharedUpdate(num2);
		this.prevCountDownTotal = num2;
	}

	// Token: 0x06002E85 RID: 11909 RVA: 0x000FE048 File Offset: 0x000FC248
	private void AuthorityUpdate(float countDownTotal)
	{
		if (PhotonNetwork.InRoom && this.grManager.IsAuthority())
		{
			if (this.shiftStarted && !NetworkSystem.Instance.SessionIsPrivate && !this.isRoomClosed && 60f * this.shiftDurationMinutes - countDownTotal >= this.roomCloseTimeSeconds)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("ghostReactorShiftStarted", "true");
				PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
				this.isRoomClosed = true;
			}
			if (this.shiftStarted && countDownTotal <= 0f)
			{
				this.grManager.RequestShiftEnd();
			}
			this.UpdateStateAuthority();
		}
	}

	// Token: 0x06002E86 RID: 11910 RVA: 0x000FE0EC File Offset: 0x000FC2EC
	private void SharedUpdate(float countDownTotal)
	{
		this.UpdateStateShared();
		this.UpdateReactorDisplayMainShared(countDownTotal);
		if (this.lastLeaderboardRefreshTime + (double)this.leaderboardUpdateFrequency < (double)Time.time || (double)Time.time < this.lastLeaderboardRefreshTime)
		{
			this.RefreshShiftLeaderboard();
			this.lastLeaderboardRefreshTime = (double)Time.time;
		}
		if (this.shiftStarted)
		{
			if (this.debugFastForwarding)
			{
				float num = this.debugFastForwardRate * Time.deltaTime;
				this.shiftStartNetworkTime -= (double)num;
			}
			int num2 = Mathf.FloorToInt(countDownTotal / 60f);
			int num3 = Mathf.FloorToInt(countDownTotal % 60f);
			this.shiftTimerText.text = num2.ToString("D2") + ":" + num3.ToString("D2");
			for (int i = 0; i < this.warnings.Count; i++)
			{
				if (countDownTotal < (float)this.warnings[i].time && this.prevCountDownTotal >= (float)this.warnings[i].time && !this.shiftJustStarted)
				{
					this.warnings[i].sound.Play(this.announceAudioSource);
					break;
				}
			}
			if (this.ShiftState == GhostReactorShiftManager.State.ShiftActive && countDownTotal > 0f && countDownTotal < this.anomalyAlertCountdownTimeToStartPlayingInMinutes * 60f && !this.anomalyAlert.isPlaying)
			{
				this.anomalyAlert.Play();
			}
			if (this.localPlayerInside)
			{
				if (countDownTotal >= 0f && countDownTotal < this.ringClosingDuration * 60f)
				{
					this.ringTransform.gameObject.SetActive(true);
					float num4 = Mathf.Lerp(this.ringClosingMinRadius, this.ringClosingMaxRadius, countDownTotal / (this.ringClosingDuration * 60f));
					this.ringTransform.localScale = new Vector3(num4, 1f, num4);
					Vector3 position = VRRig.LocalRig.bodyTransform.position;
					Vector3 vector = position - this.ringTransform.position;
					vector.y = 0f;
					Vector3 normalized = vector.normalized;
					float num5 = 0.5235988f;
					Vector3 vector2 = this.ringTransform.position + normalized * num4;
					Quaternion quaternion = Quaternion.AngleAxis(num5, Vector3.up);
					Quaternion quaternion2 = Quaternion.AngleAxis(-num5, Vector3.up);
					Vector3 vector3 = this.ringTransform.position + quaternion * normalized * num4;
					Vector3 vector4 = this.ringTransform.position + quaternion2 * normalized * num4;
					vector2.y = position.y;
					vector3.y = position.y;
					vector4.y = position.y;
					this.anomalyLoop1.transform.position = vector2;
					this.anomalyLoop2.transform.position = vector3;
					this.anomalyLoop3.transform.position = vector4;
					if (!this.anomalyLoop1.isPlaying)
					{
						this.anomalyLoop1.Play();
					}
					if (!this.anomalyLoop2.isPlaying)
					{
						this.anomalyLoop2.Play();
					}
					if (!this.anomalyLoop3.isPlaying)
					{
						this.anomalyLoop3.Play();
					}
					if (vector.sqrMagnitude > num4 * num4)
					{
						this.TeleportLocalPlayerIfOutOfBounds();
					}
				}
			}
			else if (this.ringTransform.gameObject.activeSelf)
			{
				this.ringTransform.gameObject.SetActive(false);
			}
			this.shiftJustStarted = false;
			return;
		}
		if (!this.shiftStarted)
		{
			this.TeleportLocalPlayerIfOutOfBounds();
		}
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x000FE478 File Offset: 0x000FC678
	private void TeleportLocalPlayerIfOutOfBounds()
	{
		if (this.localPlayerInside || (this.localPlayerOverlapping && Vector3.Dot(GTPlayer.Instance.headCollider.transform.position - this.gatePlaneTransform.position, this.gatePlaneTransform.forward) < 0f))
		{
			this.grManager.ReportLocalPlayerHit();
			GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
			component.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, this.grManager);
			GTPlayer.Instance.TeleportTo(this.playerTeleportTransform, true, true);
			this.localPlayerInside = false;
			this.localPlayerOverlapping = false;
			component.caughtByAnomaly = true;
		}
	}

	// Token: 0x06002E88 RID: 11912 RVA: 0x000FE51C File Offset: 0x000FC71C
	public void RevealJudgment(int evaluation)
	{
		if (evaluation <= 0)
		{
			this.shiftJugmentText.text = "DON'T QUIT YOUR DAY JOB.";
			return;
		}
		switch (evaluation)
		{
		case 1:
			this.shiftJugmentText.text = "YOU'RE LEARNING. GOOD.";
			return;
		case 2:
			this.shiftJugmentText.text = "YOU MIGHT EARN A PROMOTION.";
			return;
		case 3:
			this.shiftJugmentText.text = "YOU DID A MANAGER-TIER JOB.";
			return;
		case 4:
			this.shiftJugmentText.text = "NICE. YOU GET EXTRA SHIFTS.";
			return;
		default:
			this.shiftJugmentText.text = "YOU WORK FOR US NOW.";
			if (this.wrongStumpGoo != null)
			{
				this.wrongStumpGoo.SetActive(true);
			}
			return;
		}
	}

	// Token: 0x06002E89 RID: 11913 RVA: 0x000FE5C6 File Offset: 0x000FC7C6
	public void ResetJudgment()
	{
		this.shiftJugmentText.text = "";
		if (this.wrongStumpGoo != null)
		{
			this.wrongStumpGoo.SetActive(false);
		}
	}

	// Token: 0x06002E8A RID: 11914 RVA: 0x000FE5F4 File Offset: 0x000FC7F4
	public void ResetJoinTimes()
	{
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		for (int i = 0; i < count; i++)
		{
			GRPlayer.Get(this.reactor.vrRigs[i]).shiftJoinTime = this.shiftStartNetworkTime;
		}
	}

	// Token: 0x06002E8B RID: 11915 RVA: 0x000FE64C File Offset: 0x000FC84C
	public void CalculatePlayerPercentages()
	{
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (this.reactor.vrRigs[i] != null && grplayer != null)
			{
				if (this.reactor.vrRigs[i].OwningNetPlayer == null)
				{
					grplayer.ShiftPlayTime = 0.1f;
				}
				else if (this.shiftStarted)
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(PhotonNetwork.Time - grplayer.shiftJoinTime));
				}
				else
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(this.shiftEndNetworkTime - grplayer.shiftJoinTime));
				}
				this.totalPlayTime += grplayer.ShiftPlayTime;
			}
		}
	}

	// Token: 0x06002E8C RID: 11916 RVA: 0x000FE754 File Offset: 0x000FC954
	public void CalculateShiftTotal()
	{
		this.shiftTotalEarned = 0;
		int count = this.reactor.vrRigs.Count;
		double num = 0.0;
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (this.reactor.vrRigs[i] != null && grplayer != null)
			{
				this.shiftTotalEarned += grplayer.ShiftCredits;
				if (this.reactor.vrRigs[i].OwningNetPlayer == null)
				{
					grplayer.ShiftPlayTime = 0.1f;
				}
				else
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(PhotonNetwork.Time - grplayer.shiftJoinTime));
				}
				num += (double)grplayer.ShiftPlayTime;
			}
		}
		this.shiftTotalEarned = Mathf.Clamp(this.shiftTotalEarned, 0, this.shiftSanityMaximumEarned);
		num = (double)Mathf.Clamp((float)num, 0.1f, this.shiftDurationMinutes * 10f * 60f);
		for (int j = 0; j < count; j++)
		{
			GRPlayer grplayer2 = GRPlayer.Get(this.reactor.vrRigs[j]);
			if (this.reactor.vrRigs[j] != null && grplayer2 != null && this.depthDisplay != null)
			{
				int rewardXP = this.depthDisplay.GetRewardXP();
				if (this.authorizedToDelveDeeper)
				{
					grplayer2.LastShiftCut = rewardXP;
					grplayer2.CollectShiftCut();
				}
			}
		}
		this.reactor.RefreshScoreboards();
		this.reactor.promotionBot.Refresh();
		this.reactor.RefreshDepth();
	}

	// Token: 0x06002E8D RID: 11917 RVA: 0x000FE90C File Offset: 0x000FCB0C
	private void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.localPlayerOverlapping = true;
		}
	}

	// Token: 0x06002E8E RID: 11918 RVA: 0x000FE928 File Offset: 0x000FCB28
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			bool flag = Vector3.Dot(other.transform.position - this.gatePlaneTransform.position, this.gatePlaneTransform.forward) < 0f;
			this.localPlayerInside = flag;
			this.localPlayerOverlapping = false;
		}
	}

	// Token: 0x06002E8F RID: 11919 RVA: 0x000FE988 File Offset: 0x000FCB88
	public void OnButtonDelveDeeper()
	{
		if (this.ShiftActive)
		{
			bool flag = this.authorizedToDelveDeeper;
			return;
		}
	}

	// Token: 0x06002E90 RID: 11920 RVA: 0x000FE99A File Offset: 0x000FCB9A
	public void OnButtonDEBUGResetDepth()
	{
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DEBUG_ResetDepth);
	}

	// Token: 0x06002E91 RID: 11921 RVA: 0x000FE9A9 File Offset: 0x000FCBA9
	public void OnButtonDEBUGDelveDeeper()
	{
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DEBUG_DelveDeeper);
	}

	// Token: 0x06002E92 RID: 11922 RVA: 0x000FE9B8 File Offset: 0x000FCBB8
	public void OnButtonDEBUGDelveShallower()
	{
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DEBUG_DelveShallower);
	}

	// Token: 0x06002E93 RID: 11923 RVA: 0x000FE9C7 File Offset: 0x000FCBC7
	public void RequestState(GhostReactorShiftManager.State newState)
	{
		if (!this.grManager.IsAuthority())
		{
			return;
		}
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DelveState, (int)newState);
	}

	// Token: 0x06002E94 RID: 11924 RVA: 0x000FE9E4 File Offset: 0x000FCBE4
	public void SetState(GhostReactorShiftManager.State newState, bool force = false)
	{
		if (this.ShiftState == newState && !force)
		{
			return;
		}
		GhostReactorShiftManager.State shiftState = this.ShiftState;
		if (shiftState != GhostReactorShiftManager.State.ReadyForShift)
		{
			if (shiftState == GhostReactorShiftManager.State.Drilling)
			{
				this.reactor.shiftManager.depthDisplay.StopDelveDeeperFX();
			}
		}
		else if (this.startShiftButton != null)
		{
			this.startShiftButton.SetActive(false);
		}
		this.ShiftState = newState;
		this.stateStartTime = PhotonNetwork.Time;
		switch (this.ShiftState)
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
			this.announceBell.Play(this.announceBellAudioSource);
			this.announceTip.Play(this.announceAudioSource);
			goto IL_021F;
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
			break;
		case GhostReactorShiftManager.State.ReadyForShift:
			goto IL_021F;
		case GhostReactorShiftManager.State.ShiftActive:
		{
			this.announceStartShift.Play(this.announceAudioSource);
			using (List<VRRig>.Enumerator enumerator = this.reactor.vrRigs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VRRig vrrig = enumerator.Current;
					GRPlayer component = vrrig.GetComponent<GRPlayer>();
					if (component != null)
					{
						component.startingShiftCreditCache = component.ShiftCredits;
					}
				}
				goto IL_021F;
			}
			break;
		}
		case GhostReactorShiftManager.State.PostShift:
			if (this.authorizedToDelveDeeper)
			{
				this.announceCompleteShift.Play(this.announceAudioSource);
				if (!string.IsNullOrEmpty(this.ShiftId))
				{
					ProgressionManager.Instance.EndOfShiftReward(this.ShiftId);
					int count = this.reactor.vrRigs.Count;
					for (int i = 0; i < count; i++)
					{
						GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
						if (grplayer != null)
						{
							grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, (float)this.shiftRewardCredits);
						}
					}
				}
				Debug.LogError("ShiftId is null or empty, skipping reward of end of shift.");
				goto IL_021F;
			}
			this.announceFailShift.Play(this.announceAudioSource);
			goto IL_021F;
		case GhostReactorShiftManager.State.PreparingToDrill:
			this.announcePrepareDrill.Play(this.announceAudioSource);
			goto IL_021F;
		case GhostReactorShiftManager.State.Drilling:
			this.reactor.DelveToNextDepth();
			this.reactor.shiftManager.depthDisplay.StartDelveDeeperFX();
			goto IL_021F;
		default:
			goto IL_021F;
		}
		this.announceBell.Play(this.announceBellAudioSource);
		this.announceTip.Play(this.announceAudioSource);
		IL_021F:
		this.RefreshDepthDisplay();
	}

	// Token: 0x06002E95 RID: 11925 RVA: 0x000FEC28 File Offset: 0x000FCE28
	public GhostReactorShiftManager.State GetState()
	{
		return this.ShiftState;
	}

	// Token: 0x06002E96 RID: 11926 RVA: 0x000FEC30 File Offset: 0x000FCE30
	public bool IsSoaking()
	{
		return GhostReactorSoak.instance != null && GhostReactorSoak.instance.IsSoaking();
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x000FEC45 File Offset: 0x000FCE45
	private int GetPreShiftDuration()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.preShiftDuration;
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x000FEC57 File Offset: 0x000FCE57
	private int GetPreShiftDurationFirstArrive()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.preShiftDurationFirstArrive;
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x000FEC69 File Offset: 0x000FCE69
	private int GetPostShiftDuration()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.postShiftDuration;
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x000FEC7B File Offset: 0x000FCE7B
	private int GetPreparingToDrillDuration()
	{
		this.IsSoaking();
		return 5;
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x000FEC85 File Offset: 0x000FCE85
	public int GetDrillingDuration()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.drillDuration;
	}

	// Token: 0x06002E9C RID: 11932 RVA: 0x000FEC98 File Offset: 0x000FCE98
	private void UpdateStateAuthority()
	{
		if (!this.grManager.IsAuthority())
		{
			return;
		}
		double time = PhotonNetwork.Time;
		switch (this.ShiftState)
		{
		case GhostReactorShiftManager.State.WaitingForConnect:
			if (this.reactor.grManager.IsZoneReady())
			{
				this.RequestState(GhostReactorShiftManager.State.WaitingForFirstShiftStart);
				return;
			}
			break;
		case GhostReactorShiftManager.State.WaitingForShiftStart:
			if (time - this.stateStartTime > (double)this.GetPreShiftDuration())
			{
				this.reactor.grManager.RequestShiftStartAuthority(false);
				return;
			}
			break;
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
			if (time - this.stateStartTime > (double)this.GetPreShiftDurationFirstArrive())
			{
				this.reactor.grManager.RequestShiftStartAuthority(true);
				return;
			}
			break;
		case GhostReactorShiftManager.State.ReadyForShift:
		case GhostReactorShiftManager.State.ShiftActive:
			break;
		case GhostReactorShiftManager.State.PostShift:
			if (time - this.stateStartTime > (double)this.GetPostShiftDuration())
			{
				if (this.authorizedToDelveDeeper)
				{
					this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DelveDeeper);
					this.RequestState(GhostReactorShiftManager.State.PreparingToDrill);
					return;
				}
				this.RequestState(GhostReactorShiftManager.State.WaitingForShiftStart);
				return;
			}
			break;
		case GhostReactorShiftManager.State.PreparingToDrill:
			if (time - this.stateStartTime > (double)this.GetPreparingToDrillDuration())
			{
				this.RequestState(GhostReactorShiftManager.State.Drilling);
				return;
			}
			break;
		case GhostReactorShiftManager.State.Drilling:
			if (time - this.stateStartTime > (double)this.GetDrillingDuration())
			{
				this.RequestState(GhostReactorShiftManager.State.WaitingForShiftStart);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x000FEDB8 File Offset: 0x000FCFB8
	private void UpdateStateShared()
	{
		double time = PhotonNetwork.Time;
		switch (this.ShiftState)
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
		{
			int num = this.GetPreShiftDuration() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			num = Mathf.Max(0, num);
			this.shiftTimerText.text = ":" + num.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
		{
			int num2 = this.GetPreShiftDurationFirstArrive() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			num2 = Mathf.Max(0, num2);
			this.shiftTimerText.text = ":" + num2.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.ReadyForShift:
		case GhostReactorShiftManager.State.ShiftActive:
			break;
		case GhostReactorShiftManager.State.PostShift:
		{
			int num3 = this.GetPostShiftDuration() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			num3 = Mathf.Max(0, num3);
			this.shiftTimerText.text = ":" + num3.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.PreparingToDrill:
		{
			int num4 = 5 - Mathf.FloorToInt((float)(time - this.stateStartTime));
			num4 = Mathf.Max(0, num4);
			this.shiftTimerText.text = ":" + num4.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.Drilling:
		{
			int num5 = this.GetDrillingDuration() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			num5 = Mathf.Max(0, num5);
			this.shiftTimerText.text = ":" + num5.ToString("D2");
			this.UpdateLogoAnimations(this.depthDisplay.logoFrames);
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06002E9E RID: 11934 RVA: 0x000FEF4C File Offset: 0x000FD14C
	public void RefreshDepthDisplay()
	{
		GhostReactorLevelGenConfig currLevelGenConfig = this.reactor.GetCurrLevelGenConfig();
		int num = this.reactor.GetDepthLevel() + 1;
		int num2 = num / 4 + 1 + ((num % 5 == 4) ? 2 : 0);
		this.shiftRewardCoresForMothership = currLevelGenConfig.coresRequired + num2;
		this.coresRequiredToDelveDeeper = ((currLevelGenConfig.coresRequired > 0) ? ((int)(this.reactor.difficultyScalingForCurrentFloor * (float)currLevelGenConfig.coresRequired) + num2) : 0);
		this.killsRequiredToDelveDeeper = currLevelGenConfig.minEnemyKills;
		this.shiftRewardCredits = currLevelGenConfig.coresRequired * 5;
		this.sentientCoresRequiredToDelveDeeper = (int)(this.reactor.difficultyScalingForCurrentFloor * (float)currLevelGenConfig.sentientCoresRequired);
		this.shiftDurationMinutes = (float)(currLevelGenConfig.shiftDuration / 60);
		if (this.IsSoaking())
		{
			this.shiftDurationMinutes = (float)Random.Range(1, 3);
		}
		this.maxPlayerDeaths = currLevelGenConfig.maxPlayerDeaths;
		if (this.depthDisplay != null)
		{
			this.depthDisplay.RefreshDisplay();
		}
		this.RefreshShiftTimer();
	}

	// Token: 0x06002E9F RID: 11935 RVA: 0x000FF039 File Offset: 0x000FD239
	public void RefreshShiftLeaderboard()
	{
		if (this.nextRefreshLeaderboardSafety)
		{
			this.RefreshShiftLeaderboard_Safety();
		}
		else
		{
			this.RefreshShiftLeaderboard_Efficiency();
		}
		this.nextRefreshLeaderboardSafety = !this.nextRefreshLeaderboardSafety;
	}

	// Token: 0x06002EA0 RID: 11936 RVA: 0x000FF060 File Offset: 0x000FD260
	public void RefreshShiftLeaderboard_Safety()
	{
		if (this.shiftLeaderboardSafety == null)
		{
			return;
		}
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		this.leaderboardDisplay.Clear();
		this.leaderboardDisplay.Append("<color=#c0c0c0c0><size=-0.4>SAFETY          GHOSTS   WORKPLACE  TEAM    CHAOS\nREPORT          BANISHED INCIDENTS  ASSISTS EXPOSURE\n----------------------------------------------------</size></color>\n");
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (!(this.reactor.vrRigs[i] == null) && !(grplayer == null) && !(grplayer.gamePlayer == null))
			{
				string playerNameVisible = grplayer.gamePlayer.rig.playerNameVisible;
				int num = (int)grplayer.synchronizedSessionStats[4];
				int num2 = (int)grplayer.synchronizedSessionStats[5];
				int num3 = (int)grplayer.synchronizedSessionStats[6];
				float num4 = grplayer.synchronizedSessionStats[7];
				int num5 = (int)num4 / 60;
				int num6 = (int)num4 % 60;
				this.leaderboardDisplay.Append((i % 2 == 0) ? "<color=#e0e0ff>" : "<color=#a0a0ff>");
				this.leaderboardDisplay.Append(string.Format("{0,-12}{1,5}{2,7}{3,7}{4,10}", new object[]
				{
					playerNameVisible,
					num2,
					num,
					num3,
					string.Format("{0,3}:{1:00}", num5, num6)
				}));
				this.leaderboardDisplay.Append("</color>\n");
			}
		}
		this.shiftLeaderboardSafety.text = this.leaderboardDisplay.ToString();
	}

	// Token: 0x06002EA1 RID: 11937 RVA: 0x000FF1FC File Offset: 0x000FD3FC
	public void RefreshShiftLeaderboard_Efficiency()
	{
		if (this.shiftLeaderboardEfficiency == null)
		{
			return;
		}
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		this.leaderboardDisplay.Clear();
		this.leaderboardDisplay.Append("<color=#c0c0c0c0><size=-0.4>KEY PERFORMANCE   CORES   EARNED   SPENT    DISTANCE\nINDICATORS        FOUND   CREDITS  CREDITS  TRAVELED\n----------------------------------------------------</size></color>\n");
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (!(this.reactor.vrRigs[i] == null) && !(grplayer == null) && !(grplayer.gamePlayer == null))
			{
				string playerNameVisible = grplayer.gamePlayer.rig.playerNameVisible;
				int num = (int)grplayer.synchronizedSessionStats[0];
				int num2 = (int)grplayer.synchronizedSessionStats[1];
				int num3 = (int)grplayer.synchronizedSessionStats[2];
				int num4 = (int)grplayer.synchronizedSessionStats[3];
				this.leaderboardDisplay.Append((i % 2 == 0) ? "<color=#e0e0ff>" : "<color=#a0a0ff>");
				this.leaderboardDisplay.Append(string.Format("{0,-12}{1,6}{2,7}{3,7}{4,8}", new object[] { playerNameVisible, num, num2, num3, num4 }));
				this.leaderboardDisplay.Append("</color>\n");
			}
		}
		this.shiftLeaderboardEfficiency.text = this.leaderboardDisplay.ToString();
	}

	// Token: 0x04003B0C RID: 15116
	private const string EVENT_GOOD_KD = "GRShiftGoodKD";

	// Token: 0x04003B0D RID: 15117
	[SerializeField]
	public GhostReactor reactor;

	// Token: 0x04003B0E RID: 15118
	[SerializeField]
	private GRMetalEnergyGate frontGate;

	// Token: 0x04003B0F RID: 15119
	[SerializeField]
	private GameObject startShiftButton;

	// Token: 0x04003B10 RID: 15120
	[SerializeField]
	private TMP_Text shiftTimerText;

	// Token: 0x04003B11 RID: 15121
	[SerializeField]
	private TMP_Text shiftStatsText;

	// Token: 0x04003B12 RID: 15122
	[SerializeField]
	private TMP_Text shiftJugmentText;

	// Token: 0x04003B13 RID: 15123
	[SerializeField]
	private TMP_Text reactorTextMain;

	// Token: 0x04003B14 RID: 15124
	[SerializeField]
	private GameObject wrongStumpGoo;

	// Token: 0x04003B15 RID: 15125
	[SerializeField]
	private float shiftDurationMinutes = 20f;

	// Token: 0x04003B16 RID: 15126
	[SerializeField]
	private Transform playerTeleportTransform;

	// Token: 0x04003B17 RID: 15127
	[SerializeField]
	private Transform gatePlaneTransform;

	// Token: 0x04003B18 RID: 15128
	[SerializeField]
	private Transform gateBlockerTransform;

	// Token: 0x04003B19 RID: 15129
	[SerializeField]
	private AudioSource anomalyLoop1;

	// Token: 0x04003B1A RID: 15130
	[SerializeField]
	private AudioSource anomalyLoop2;

	// Token: 0x04003B1B RID: 15131
	[SerializeField]
	private AudioSource anomalyLoop3;

	// Token: 0x04003B1C RID: 15132
	[SerializeField]
	private AudioSource anomalyAlert;

	// Token: 0x04003B1D RID: 15133
	[SerializeField]
	private float anomalyAlertCountdownTimeToStartPlayingInMinutes = 3f;

	// Token: 0x04003B1E RID: 15134
	[SerializeField]
	private float roomCloseTimeSeconds = 60f;

	// Token: 0x04003B1F RID: 15135
	private bool isRoomClosed;

	// Token: 0x04003B20 RID: 15136
	[SerializeField]
	private int preShiftDuration = 10;

	// Token: 0x04003B21 RID: 15137
	private int preShiftDurationFirstArrive = 60;

	// Token: 0x04003B22 RID: 15138
	private int postShiftDuration = 10;

	// Token: 0x04003B23 RID: 15139
	[SerializeField]
	public int drillDuration = 50;

	// Token: 0x04003B24 RID: 15140
	private bool bIsStartingFloorAuthorityOnly;

	// Token: 0x04003B25 RID: 15141
	[Header("Drill Announcements")]
	[SerializeField]
	private AudioSource announceAudioSource;

	// Token: 0x04003B26 RID: 15142
	[SerializeField]
	private AudioSource announceBellAudioSource;

	// Token: 0x04003B27 RID: 15143
	public AbilitySound announcePrepareShift;

	// Token: 0x04003B28 RID: 15144
	public AbilitySound announceStartShift;

	// Token: 0x04003B29 RID: 15145
	public AbilitySound announceCompleteShift;

	// Token: 0x04003B2A RID: 15146
	public AbilitySound announceFailShift;

	// Token: 0x04003B2B RID: 15147
	public AbilitySound announcePrepareDrill;

	// Token: 0x04003B2C RID: 15148
	public AbilitySound announceTip;

	// Token: 0x04003B2D RID: 15149
	public AbilitySound announceBell;

	// Token: 0x04003B2E RID: 15150
	[Header("Warning")]
	public List<GhostReactorShiftManager.WarningPres> warnings;

	// Token: 0x04003B2F RID: 15151
	[SerializeField]
	private AudioClip warningAudio;

	// Token: 0x04003B30 RID: 15152
	[SerializeField]
	[Tooltip("Must be ordered from largest time (first played) to smallest time (last played)")]
	private List<int> warningClipPlayTimes = new List<int>();

	// Token: 0x04003B31 RID: 15153
	[Header("Ring")]
	[SerializeField]
	private Transform ringTransform;

	// Token: 0x04003B32 RID: 15154
	[SerializeField]
	private float ringClosingDuration = 3f;

	// Token: 0x04003B33 RID: 15155
	[SerializeField]
	private float ringClosingMaxRadius = 100f;

	// Token: 0x04003B34 RID: 15156
	[SerializeField]
	private float ringClosingMinRadius = 7f;

	// Token: 0x04003B35 RID: 15157
	[Header("Debug")]
	[SerializeField]
	private float debugFastForwardRate = 30f;

	// Token: 0x04003B36 RID: 15158
	[SerializeField]
	private bool debugFastForwarding;

	// Token: 0x04003B37 RID: 15159
	private bool shiftStarted;

	// Token: 0x04003B38 RID: 15160
	private bool shiftJustStarted;

	// Token: 0x04003B39 RID: 15161
	private double shiftStartNetworkTime;

	// Token: 0x04003B3A RID: 15162
	private double shiftEndNetworkTime;

	// Token: 0x04003B3B RID: 15163
	private float prevCountDownTotal;

	// Token: 0x04003B3C RID: 15164
	[SerializeField]
	private int shiftTotalEarned = -1;

	// Token: 0x04003B3D RID: 15165
	[SerializeField]
	private int shiftSanityMaximumEarned = 10000;

	// Token: 0x04003B3E RID: 15166
	public GhostReactorShiftDepthDisplay depthDisplay;

	// Token: 0x04003B3F RID: 15167
	public bool authorizedToDelveDeeper;

	// Token: 0x04003B40 RID: 15168
	public int shiftRewardCoresForMothership;

	// Token: 0x04003B41 RID: 15169
	public int coresRequiredToDelveDeeper;

	// Token: 0x04003B42 RID: 15170
	public int sentientCoresRequiredToDelveDeeper;

	// Token: 0x04003B43 RID: 15171
	public List<GREnemyCount> killsRequiredToDelveDeeper;

	// Token: 0x04003B44 RID: 15172
	public int maxPlayerDeaths;

	// Token: 0x04003B45 RID: 15173
	public int shiftRewardCredits;

	// Token: 0x04003B46 RID: 15174
	private bool localPlayerInside;

	// Token: 0x04003B47 RID: 15175
	private bool localPlayerOverlapping;

	// Token: 0x04003B48 RID: 15176
	private float totalPlayTime;

	// Token: 0x04003B49 RID: 15177
	private string gameIdGuid = "";

	// Token: 0x04003B4A RID: 15178
	public GRShiftStat shiftStats = new GRShiftStat();

	// Token: 0x04003B4B RID: 15179
	[NonSerialized]
	private GhostReactorManager grManager;

	// Token: 0x04003B4C RID: 15180
	[SerializeField]
	private TMP_Text shiftLeaderboardEfficiency;

	// Token: 0x04003B4D RID: 15181
	[SerializeField]
	private TMP_Text shiftLeaderboardSafety;

	// Token: 0x04003B4E RID: 15182
	private double lastLeaderboardRefreshTime;

	// Token: 0x04003B4F RID: 15183
	private float leaderboardUpdateFrequency = 0.5f;

	// Token: 0x04003B51 RID: 15185
	public double stateStartTime;

	// Token: 0x04003B52 RID: 15186
	private double lastReactorLogoAnimationTime;

	// Token: 0x04003B53 RID: 15187
	private int lastReactorLogoAnimFrame;

	// Token: 0x04003B54 RID: 15188
	private bool isPlayingLogoAnimation;

	// Token: 0x04003B55 RID: 15189
	private double lastReactorDisplayUpdate;

	// Token: 0x04003B56 RID: 15190
	private StringBuilder cachedStringBuilder = new StringBuilder(256);

	// Token: 0x04003B57 RID: 15191
	private bool nextRefreshLeaderboardSafety;

	// Token: 0x04003B58 RID: 15192
	private StringBuilder leaderboardDisplay = new StringBuilder(1024);

	// Token: 0x02000724 RID: 1828
	[Serializable]
	public class WarningPres
	{
		// Token: 0x04003B59 RID: 15193
		public int time;

		// Token: 0x04003B5A RID: 15194
		public AbilitySound sound;
	}

	// Token: 0x02000725 RID: 1829
	public enum State
	{
		// Token: 0x04003B5C RID: 15196
		WaitingForConnect,
		// Token: 0x04003B5D RID: 15197
		WaitingForShiftStart,
		// Token: 0x04003B5E RID: 15198
		WaitingForFirstShiftStart,
		// Token: 0x04003B5F RID: 15199
		ReadyForShift,
		// Token: 0x04003B60 RID: 15200
		ShiftActive,
		// Token: 0x04003B61 RID: 15201
		PostShift,
		// Token: 0x04003B62 RID: 15202
		PreparingToDrill,
		// Token: 0x04003B63 RID: 15203
		Drilling
	}
}
