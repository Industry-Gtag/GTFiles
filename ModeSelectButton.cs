using System;
using GameObjectScheduling;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x0200094D RID: 2381
public class ModeSelectButton : GorillaPressableButton
{
	// Token: 0x170005BA RID: 1466
	// (get) Token: 0x06003E73 RID: 15987 RVA: 0x001514D9 File Offset: 0x0014F6D9
	// (set) Token: 0x06003E74 RID: 15988 RVA: 0x001514E1 File Offset: 0x0014F6E1
	public PartyGameModeWarning WarningScreen
	{
		get
		{
			return this.warningScreen;
		}
		set
		{
			this.warningScreen = value;
		}
	}

	// Token: 0x06003E75 RID: 15989 RVA: 0x001514EA File Offset: 0x0014F6EA
	public override void Start()
	{
		base.Start();
		GorillaComputer.instance.currentGameMode.AddCallback(new Action<string>(this.OnGameModeChanged), true);
	}

	// Token: 0x06003E76 RID: 15990 RVA: 0x00151510 File Offset: 0x0014F710
	private void OnDestroy()
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			GorillaComputer.instance.currentGameMode.RemoveCallback(new Action<string>(this.OnGameModeChanged));
		}
	}

	// Token: 0x06003E77 RID: 15991 RVA: 0x00151536 File Offset: 0x0014F736
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		if (this.warningScreen.ShouldShowWarning)
		{
			this.warningScreen.Show();
			return;
		}
		GorillaComputer.instance.OnModeSelectButtonPress(this.gameMode, isLeftHand);
	}

	// Token: 0x06003E78 RID: 15992 RVA: 0x0015156B File Offset: 0x0014F76B
	public void OnGameModeChanged(string newGameMode)
	{
		this.buttonRenderer.material = ((newGameMode.ToLower() == this.gameMode.ToLower()) ? this.pressedMaterial : this.unpressedMaterial);
	}

	// Token: 0x06003E79 RID: 15993 RVA: 0x001515A0 File Offset: 0x0014F7A0
	public void SetInfo(string Mode, string ModeTitle, bool NewMode, CountdownTextDate CountdownTo)
	{
		this.gameModeTitle.text = ModeTitle;
		this.gameMode = Mode;
		this.newModeSplash.SetActive(NewMode);
		this.limitedCountdown.gameObject.SetActive(false);
		if (CountdownTo == null)
		{
			return;
		}
		this.limitedCountdown.Countdown = CountdownTo;
		this.limitedCountdown.gameObject.SetActive(true);
	}

	// Token: 0x06003E7A RID: 15994 RVA: 0x00151606 File Offset: 0x0014F806
	public void HideNewAndLimitedTimeInfo()
	{
		this.limitedCountdown.gameObject.SetActive(false);
		this.newModeSplash.SetActive(false);
	}

	// Token: 0x04004EEB RID: 20203
	[SerializeField]
	public string gameMode;

	// Token: 0x04004EEC RID: 20204
	[SerializeField]
	protected PartyGameModeWarning warningScreen;

	// Token: 0x04004EED RID: 20205
	[SerializeField]
	private TMP_Text gameModeTitle;

	// Token: 0x04004EEE RID: 20206
	[SerializeField]
	private GameObject newModeSplash;

	// Token: 0x04004EEF RID: 20207
	[SerializeField]
	private CountdownText limitedCountdown;
}
