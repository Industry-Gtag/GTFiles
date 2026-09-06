using System;
using TMPro;
using UnityEngine;

// Token: 0x0200036D RID: 877
public class RaceVisual : MonoBehaviour
{
	// Token: 0x17000220 RID: 544
	// (get) Token: 0x0600156E RID: 5486 RVA: 0x00071CBB File Offset: 0x0006FEBB
	// (set) Token: 0x0600156F RID: 5487 RVA: 0x00071CC3 File Offset: 0x0006FEC3
	public int raceId { get; private set; }

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06001570 RID: 5488 RVA: 0x00071CCC File Offset: 0x0006FECC
	// (set) Token: 0x06001571 RID: 5489 RVA: 0x00071CD4 File Offset: 0x0006FED4
	public bool TickRunning { get; set; }

	// Token: 0x06001572 RID: 5490 RVA: 0x00071CDD File Offset: 0x0006FEDD
	private void Awake()
	{
		this.checkpoints = base.GetComponent<RaceCheckpointManager>();
		this.finishLineText.text = "";
		this.SetScoreboardText("", "");
		this.SetRaceStartScoreboardText("", "");
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x00071D1B File Offset: 0x0006FF1B
	private void OnEnable()
	{
		RacingManager.instance.RegisterVisual(this);
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x00071D28 File Offset: 0x0006FF28
	public void Button_StartRace(int laps)
	{
		RacingManager.instance.Button_StartRace(this.raceId, laps);
	}

	// Token: 0x06001575 RID: 5493 RVA: 0x00071D3B File Offset: 0x0006FF3B
	public void ShowFinishLineText(string text)
	{
		this.finishLineText.text = text;
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x00071D49 File Offset: 0x0006FF49
	public void UpdateCountdown(int timeRemaining)
	{
		if (timeRemaining != this.lastDisplayedCountdown)
		{
			this.countdownText.text = timeRemaining.ToString();
			this.finishLineText.text = "";
			this.lastDisplayedCountdown = timeRemaining;
		}
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x00071D80 File Offset: 0x0006FF80
	public void SetScoreboardText(string mainText, string timesText)
	{
		foreach (RacingScoreboard racingScoreboard in this.raceScoreboards)
		{
			racingScoreboard.mainDisplay.text = mainText;
			racingScoreboard.timesDisplay.text = timesText;
		}
	}

	// Token: 0x06001578 RID: 5496 RVA: 0x00071DBC File Offset: 0x0006FFBC
	public void SetRaceStartScoreboardText(string mainText, string timesText)
	{
		this.raceStartScoreboard.mainDisplay.text = mainText;
		this.raceStartScoreboard.timesDisplay.text = timesText;
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x00071DE0 File Offset: 0x0006FFE0
	public void ActivateStartingWall(bool enable)
	{
		this.startingWall.SetActive(enable);
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x00071DEE File Offset: 0x0006FFEE
	public bool IsPlayerNearCheckpoint(VRRig player, int checkpoint)
	{
		return this.checkpoints.IsPlayerNearCheckpoint(player, checkpoint);
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x00071DFD File Offset: 0x0006FFFD
	public void OnCountdownStart(int laps, float goAfterInterval)
	{
		this.raceConsoleVisual.ShowRaceInProgress(laps);
		this.countdownSoundPlayer.Play();
		this.countdownSoundPlayer.time = this.countdownSoundGoTime - goAfterInterval;
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x00071E29 File Offset: 0x00070029
	public void OnRaceStart()
	{
		this.finishLineText.text = "GO!";
		this.checkpoints.OnRaceStart();
		this.lastDisplayedCountdown = 0;
		this.startingWall.SetActive(false);
		this.isRaceEndSoundEnabled = false;
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x00071E60 File Offset: 0x00070060
	public void OnRaceEnded()
	{
		this.finishLineText.text = "";
		this.lastDisplayedCountdown = 0;
		this.checkpoints.OnRaceEnd();
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x00071E84 File Offset: 0x00070084
	public void OnRaceReset()
	{
		this.raceConsoleVisual.ShowCanStartRace();
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x00071E91 File Offset: 0x00070091
	public void EnableRaceEndSound()
	{
		this.isRaceEndSoundEnabled = true;
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x00071E9A File Offset: 0x0007009A
	public void OnCheckpointPassed(int index, SoundBankPlayer checkpointSound)
	{
		if (index == 0 && this.isRaceEndSoundEnabled)
		{
			this.countdownSoundPlayer.PlayOneShot(this.raceEndSound);
		}
		else
		{
			checkpointSound.Play();
		}
		RacingManager.instance.OnCheckpointPassed(this.raceId, index);
	}

	// Token: 0x04001A4B RID: 6731
	[SerializeField]
	private TextMeshPro finishLineText;

	// Token: 0x04001A4C RID: 6732
	[SerializeField]
	private TextMeshPro countdownText;

	// Token: 0x04001A4D RID: 6733
	[SerializeField]
	private RacingScoreboard[] raceScoreboards;

	// Token: 0x04001A4E RID: 6734
	[SerializeField]
	private RacingScoreboard raceStartScoreboard;

	// Token: 0x04001A4F RID: 6735
	[SerializeField]
	private RaceConsoleVisual raceConsoleVisual;

	// Token: 0x04001A50 RID: 6736
	private float nextVisualRefreshTimestamp;

	// Token: 0x04001A51 RID: 6737
	private RaceCheckpointManager checkpoints;

	// Token: 0x04001A52 RID: 6738
	[SerializeField]
	private AudioClip raceEndSound;

	// Token: 0x04001A53 RID: 6739
	[SerializeField]
	private float countdownSoundGoTime;

	// Token: 0x04001A54 RID: 6740
	[SerializeField]
	private AudioSource countdownSoundPlayer;

	// Token: 0x04001A55 RID: 6741
	[SerializeField]
	private GameObject startingWall;

	// Token: 0x04001A56 RID: 6742
	private int lastDisplayedCountdown;

	// Token: 0x04001A57 RID: 6743
	private bool isRaceEndSoundEnabled;
}
