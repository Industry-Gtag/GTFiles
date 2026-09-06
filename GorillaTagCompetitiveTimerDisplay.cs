using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020008C9 RID: 2249
public class GorillaTagCompetitiveTimerDisplay : MonoBehaviour
{
	// Token: 0x06003AC4 RID: 15044 RVA: 0x0013EE94 File Offset: 0x0013D094
	private void Awake()
	{
		this.prevTime = -1;
		if (this.waitingForPlayersBackground)
		{
			this.waitingForPlayersBackground.SetActive(true);
			this.currentBackground = this.waitingForPlayersBackground;
		}
		if (this.startCountdownBackground)
		{
			this.startCountdownBackground.SetActive(false);
		}
		if (this.playingBackground)
		{
			this.playingBackground.SetActive(false);
		}
		if (this.postRoundBackground)
		{
			this.postRoundBackground.SetActive(false);
		}
		this.timerDisplay.gameObject.SetActive(false);
		if (this.timerDisplay2)
		{
			this.timerDisplay2.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003AC5 RID: 15045 RVA: 0x0013EF48 File Offset: 0x0013D148
	private void OnEnable()
	{
		GorillaTagCompetitiveManager.onStateChanged += this.HandleOnGameStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime += this.HandleOnTimeChanged;
		GorillaTagCompetitiveManager gorillaTagCompetitiveManager = GorillaGameManager.instance as GorillaTagCompetitiveManager;
		if (gorillaTagCompetitiveManager != null)
		{
			this.HandleOnGameStateChanged(gorillaTagCompetitiveManager.GetCurrentGameState());
		}
		this.myRig = base.GetComponentInParent<VRRig>();
		this.DisplayStandardTimer(false);
	}

	// Token: 0x06003AC6 RID: 15046 RVA: 0x0013EFAA File Offset: 0x0013D1AA
	private void OnDisable()
	{
		GorillaTagCompetitiveManager.onStateChanged -= this.HandleOnGameStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime -= this.HandleOnTimeChanged;
	}

	// Token: 0x06003AC7 RID: 15047 RVA: 0x0013EFD0 File Offset: 0x0013D1D0
	private void HandleOnGameStateChanged(GorillaTagCompetitiveManager.GameState newState)
	{
		this.SetNewBackground(newState);
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			this.DisplayStandardTimer(false);
			this.resultsDisplay.gameObject.SetActive(false);
			return;
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
		case GorillaTagCompetitiveManager.GameState.Playing:
			this.DisplayStandardTimer(true);
			return;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			this.DoPostRoundShow();
			return;
		default:
			return;
		}
	}

	// Token: 0x06003AC8 RID: 15048 RVA: 0x0013F024 File Offset: 0x0013D224
	private void DisplayStandardTimer(bool bShow)
	{
		if (bShow)
		{
			this.resultsDisplay.gameObject.SetActive(false);
		}
		this.timerDisplay.gameObject.SetActive(bShow);
		if (this.timerDisplay2 != null)
		{
			this.timerDisplay2.gameObject.SetActive(bShow);
		}
	}

	// Token: 0x06003AC9 RID: 15049 RVA: 0x0013F078 File Offset: 0x0013D278
	private void DoPostRoundShow()
	{
		GorillaTagCompetitiveManager gorillaTagCompetitiveManager = GorillaGameManager.instance as GorillaTagCompetitiveManager;
		if (gorillaTagCompetitiveManager == null)
		{
			return;
		}
		this.DisplayStandardTimer(false);
		this.resultsDisplay.gameObject.SetActive(true);
		List<VRRig> list = new List<VRRig>();
		List<RankedMultiplayerScore.PlayerScoreInRound> sortedScores = gorillaTagCompetitiveManager.GetScoring().GetSortedScores();
		float num = gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(sortedScores[0].NumTags, sortedScores[0].PointsOnDefense);
		int num2 = 0;
		while (num2 < sortedScores.Count && num2 < 3)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(sortedScores[num2].PlayerId, out rigContainer))
			{
				float num3 = gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(sortedScores[num2].NumTags, sortedScores[num2].PointsOnDefense);
				if (num2 == 0 || num3.Approx(num, 0.01f))
				{
					list.Add(rigContainer.Rig);
				}
				switch (num2)
				{
				case 0:
					if (this.tintableCelebration != null)
					{
						Color playerColor = rigContainer.Rig.playerColor;
						float num4;
						float num5;
						float num6;
						Color.RGBToHSV(playerColor, out num4, out num5, out num6);
						Color color = Color.HSVToRGB(num4, num5, (num6 < 0.5f) ? (num6 + 0.5f) : (num6 - 0.5f));
						this.tintableCelebration.main.startColor = new ParticleSystem.MinMaxGradient(playerColor, color);
						this.tintableCelebration.gameObject.SetActive(true);
					}
					if (this.goldCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.goldCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				case 1:
					if (this.silverCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.silverCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				case 2:
					if (this.bronzeCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.bronzeCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				}
			}
			num2++;
		}
		for (int i = 0; i < this.postRoundTimerText.Length; i++)
		{
			this.postRoundTimerText[i].text = ((list.Count > 1) ? "SHARED WIN" : "WINNER");
		}
		string text = string.Empty;
		for (int j = 0; j < list.Count; j++)
		{
			text = text + list[j].playerText1.text.ToUpper() + "\n";
		}
		this.resultsDisplay.text = text.Trim();
		if (this.timerDisplay2 != null)
		{
			this.timerDisplay2.text = this.resultsDisplay.text;
		}
	}

	// Token: 0x06003ACA RID: 15050 RVA: 0x0013F3A0 File Offset: 0x0013D5A0
	private void HandleOnTimeChanged(float time)
	{
		int num = Mathf.CeilToInt(time);
		num = Mathf.Max(num, 1);
		if (this.prevTime != num)
		{
			this.prevTime = num;
			if (this.currentState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				int num2 = this.prevTime / 60;
				int num3 = this.prevTime % 60;
				this.timerDisplay.text = string.Format("{0}:{1:D2}", num2, num3);
				if (this.timerDisplay2)
				{
					this.timerDisplay2.text = string.Format("{0}:{1:D2}", num2, num3);
					return;
				}
			}
			else if (this.currentState != GorillaTagCompetitiveManager.GameState.PostRound)
			{
				this.timerDisplay.text = this.prevTime.ToString("#00");
				if (this.timerDisplay2)
				{
					this.timerDisplay2.text = this.prevTime.ToString("#00");
				}
			}
		}
	}

	// Token: 0x06003ACB RID: 15051 RVA: 0x0013F488 File Offset: 0x0013D688
	private void SetNewBackground(GorillaTagCompetitiveManager.GameState newState)
	{
		if (this.currentBackground != null)
		{
			this.currentBackground.SetActive(false);
		}
		this.currentState = newState;
		GameObject gameObject = this.SelectBackground(newState);
		this.GetTextColor(newState);
		this.currentBackground = null;
		if (gameObject != null)
		{
			this.currentBackground = gameObject;
			this.currentBackground.SetActive(true);
		}
	}

	// Token: 0x06003ACC RID: 15052 RVA: 0x0013F4E9 File Offset: 0x0013D6E9
	private GameObject SelectBackground(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			return this.waitingForPlayersBackground;
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
			return this.startCountdownBackground;
		case GorillaTagCompetitiveManager.GameState.Playing:
			return this.playingBackground;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			return this.postRoundBackground;
		default:
			return null;
		}
	}

	// Token: 0x06003ACD RID: 15053 RVA: 0x0013F522 File Offset: 0x0013D722
	private Color GetTextColor(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
			return this.timerColorStart;
		case GorillaTagCompetitiveManager.GameState.Playing:
			return this.timerColorPlaying;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			return this.timerColorPostRound;
		default:
			return Color.white;
		}
	}

	// Token: 0x04004AC1 RID: 19137
	public TextMeshPro timerDisplay;

	// Token: 0x04004AC2 RID: 19138
	public TextMeshPro timerDisplay2;

	// Token: 0x04004AC3 RID: 19139
	public TextMeshPro resultsDisplay;

	// Token: 0x04004AC4 RID: 19140
	public GameObject waitingForPlayersBackground;

	// Token: 0x04004AC5 RID: 19141
	public GameObject startCountdownBackground;

	// Token: 0x04004AC6 RID: 19142
	public Color timerColorStart = Color.white;

	// Token: 0x04004AC7 RID: 19143
	public GameObject playingBackground;

	// Token: 0x04004AC8 RID: 19144
	public Color timerColorPlaying = Color.white;

	// Token: 0x04004AC9 RID: 19145
	public GameObject postRoundBackground;

	// Token: 0x04004ACA RID: 19146
	public Color timerColorPostRound = Color.white;

	// Token: 0x04004ACB RID: 19147
	public TextMeshPro[] postRoundTimerText;

	// Token: 0x04004ACC RID: 19148
	private GorillaTagCompetitiveManager.GameState currentState;

	// Token: 0x04004ACD RID: 19149
	private GameObject currentBackground;

	// Token: 0x04004ACE RID: 19150
	private int prevTime = -1;

	// Token: 0x04004ACF RID: 19151
	[SerializeField]
	private ParticleSystem tintableCelebration;

	// Token: 0x04004AD0 RID: 19152
	[SerializeField]
	private ParticleSystem goldCelebration;

	// Token: 0x04004AD1 RID: 19153
	[SerializeField]
	private ParticleSystem silverCelebration;

	// Token: 0x04004AD2 RID: 19154
	[SerializeField]
	private ParticleSystem bronzeCelebration;

	// Token: 0x04004AD3 RID: 19155
	private VRRig myRig;

	// Token: 0x04004AD4 RID: 19156
	[SerializeField]
	private AudioSource celebrationAudio;
}
