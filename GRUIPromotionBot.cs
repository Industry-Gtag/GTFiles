using System;
using System.Text;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000833 RID: 2099
public class GRUIPromotionBot : MonoBehaviourTick
{
	// Token: 0x060035F2 RID: 13810 RVA: 0x00129B90 File Offset: 0x00127D90
	public string FormattedUserInfo()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			return "ERROR";
		}
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(grplayer.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item3;
		int item2 = gradePointDetails.Item4;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
		string titleNameAndGrade = GhostReactorProgression.GetTitleNameAndGrade(grplayer.CurrentProgression.redeemedPoints);
		int num = 1000 + grplayer.ShiftCreditCapIncreases * 100;
		int num2 = grplayer.CurrentProgression.points - grplayer.CurrentProgression.redeemedPoints + item2;
		string text = ((player != null) ? player.SanitizedNickName : "RANDO MONKE");
		this.cachedStringBuilder.Clear();
		this.cachedStringBuilder.Append("<color=#808080>EMPLOYEE:</color>     " + text + "\n");
		this.cachedStringBuilder.Append("<color=#808080>TITLE:</color>        " + titleNameAndGrade + "\n");
		this.cachedStringBuilder.Append(string.Format("<color=#808080>XP:</color>           {0}/{1}\n", num2, item));
		if (grplayer == GRPlayer.GetLocal())
		{
			this.cachedStringBuilder.Append(string.Format("<color=#808080>CREDITS:</color>      <color=#00ff00>⑭ {0}</color>\n", grplayer.ShiftCredits));
			this.cachedStringBuilder.Append(string.Format("<color=#808080>CREDIT LIMIT:</color> <color=#00a000>⑭ {0}</color>\n", num));
			if (this.reactor != null && this.reactor.toolProgression != null)
			{
				int numberOfResearchPoints = this.reactor.toolProgression.GetNumberOfResearchPoints();
				this.cachedStringBuilder.Append(string.Format("<color=#808080>JUICE:</color>        <color=purple>⑮ {0}</color>\n", numberOfResearchPoints));
			}
			if (ProgressionManager.Instance != null)
			{
				int shinyRocksTotal = ProgressionManager.Instance.GetShinyRocksTotal();
				this.cachedStringBuilder.Append(string.Format("<color=#808080>SHINY ROCKS:</color>  <color=white>⑯ {0}</color>\n", shinyRocksTotal));
			}
		}
		return this.cachedStringBuilder.ToString();
	}

	// Token: 0x060035F3 RID: 13811 RVA: 0x00129D84 File Offset: 0x00127F84
	public bool ActivePlayerEligibleForPromotion()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			return false;
		}
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(grplayer.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item3;
		int item2 = gradePointDetails.Item4;
		return item - item2 < grplayer.CurrentProgression.points - grplayer.CurrentProgression.redeemedPoints;
	}

	// Token: 0x060035F4 RID: 13812 RVA: 0x00129DE4 File Offset: 0x00127FE4
	public void Init(GhostReactor _reactor)
	{
		this.reactor = _reactor;
		this.currentPlayerActorNumber = -1;
		this.currentState = GRUIPromotionBot.PromotionBotState.WaitingForLogin;
	}

	// Token: 0x060035F5 RID: 13813 RVA: 0x00129DFB File Offset: 0x00127FFB
	public void Refresh()
	{
		this.RefreshPlayerData();
	}

	// Token: 0x060035F6 RID: 13814 RVA: 0x00129E04 File Offset: 0x00128004
	public override void Tick()
	{
		if (this.reactor == null || this.reactor.grManager == null || !this.reactor.grManager.IsAuthority())
		{
			return;
		}
		float time = Time.time;
		if (this.currentPlayerActorNumber != -1 && (this.timeLastDistanceCheck > time || time > this.timeLastDistanceCheck + this.timeBetweenDistanceChecks))
		{
			GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
			if (grplayer == null || (base.transform.position - grplayer.transform.position).sqrMagnitude > this.distanceForAutoLogout * this.distanceForAutoLogout)
			{
				this.SwitchState(GRUIPromotionBot.PromotionBotState.WaitingForLogin, false);
			}
		}
	}

	// Token: 0x060035F7 RID: 13815 RVA: 0x00129EBC File Offset: 0x001280BC
	public bool CheckIsActivePlayer()
	{
		Object @object = GRPlayer.Get(VRRig.LocalRig);
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		return @object == grplayer;
	}

	// Token: 0x060035F8 RID: 13816 RVA: 0x00129EE8 File Offset: 0x001280E8
	public void UpPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		if (promotionBotState != GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease)
		{
			if (promotionBotState == GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits)
			{
				this.SwitchState(GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease, false);
				return;
			}
		}
		else
		{
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePromotion, false);
		}
	}

	// Token: 0x060035F9 RID: 13817 RVA: 0x00129F20 File Offset: 0x00128120
	public void DownPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		if (promotionBotState == GRUIPromotionBot.PromotionBotState.ChoosePromotion)
		{
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease, false);
			return;
		}
		if (promotionBotState != GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease)
		{
			return;
		}
		this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits, false);
	}

	// Token: 0x060035FA RID: 13818 RVA: 0x00129F58 File Offset: 0x00128158
	public void YesPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		switch (this.currentState)
		{
		case GRUIPromotionBot.PromotionBotState.ChoosePromotion:
			this.AttemptPromotion();
			return;
		case GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease:
			this.AttemptPurchaseShiftCreditIncrease();
			return;
		case GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits:
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits, false);
			return;
		case GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits:
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x060035FB RID: 13819 RVA: 0x00129FB0 File Offset: 0x001281B0
	public void NoPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		if (promotionBotState - GRUIPromotionBot.PromotionBotState.ChoosePromotion > 2)
		{
			if (promotionBotState == GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits)
			{
				this.AttemptPurchaseShiftCreditRefillToMax();
				return;
			}
		}
		else
		{
			this.SwitchState(GRUIPromotionBot.PromotionBotState.WaitingForLogin, false);
		}
	}

	// Token: 0x060035FC RID: 13820 RVA: 0x00129FE8 File Offset: 0x001281E8
	public void SwitchState(GRUIPromotionBot.PromotionBotState newState, bool fromRPC = false)
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer2 == null)
		{
			return;
		}
		this.RefreshPlayerData();
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		this.currentState = newState;
		this.SetScreenVisibility();
		this.SetMenuText(newState);
		switch (newState)
		{
		case GRUIPromotionBot.PromotionBotState.ChoosePromotion:
			if (this.ActivePlayerEligibleForPromotion())
			{
				this.descriptionText.text = "<color=#c0c0c0>     YOU ARE ELIGIBLE FOR A PROMOTION!\n     PRESS 'YES' TO CONTINUE</color>";
			}
			else
			{
				this.descriptionText.text = "<color=#c04040>     YOU ARE NOT ELIGIBLE FOR A PROMOTION\n     EARN MORE XP BY COMPLETING SHIFT GOALS</color>";
			}
			break;
		case GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease:
			if (grplayer.ShiftCreditCapIncreases != grplayer.ShiftCreditCapIncreasesMax)
			{
				this.descriptionText.text = "<color=#c0c0c0>     INCREASE CREDIT LIMIT BY <color=#00ff00>⑭ 100</color>\n     FOR <color=purple>⑮ 2</color> JUICE?</color>";
			}
			else
			{
				this.descriptionText.text = "<color=#c0c0c0>     CREDIT LIMIT CAN'T BE INCREASED AT THIS TIME\n</color>";
			}
			break;
		case GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits:
			if (grplayer == null)
			{
				this.descriptionText.text = "No active player";
			}
			else
			{
				int purchaseToCreditCapAmount = this.GetPurchaseToCreditCapAmount();
				if (purchaseToCreditCapAmount > 0)
				{
					this.descriptionText.text = string.Format("<color=#c0c0c0>     PURCHASE <color=#00ff00>+⑭{0}</color> CREDITS\n     FOR <color=white>100 SHINY ROCKS?</color>", purchaseToCreditCapAmount);
				}
				else
				{
					this.descriptionText.text = "<color=#c0c0c0>     YOU ARE AT FULL CREDITS";
				}
			}
			break;
		case GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits:
		{
			int purchaseToCreditCapAmount2 = this.GetPurchaseToCreditCapAmount();
			this.descriptionText.text = string.Format("<color=#c0c0c0>     CONFIRM PURCHASE <color=#00ff00>+⑭{0}</color>\n     FOR <color=white>100 SHINY ROCKS?</color>", purchaseToCreditCapAmount2);
			break;
		}
		}
		if (this.currentState == GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits)
		{
			this.yesText.text = "<size=0.4>CANCEL</size>";
			this.noText.text = "<size=0.4>CONFIRM</size>";
		}
		else
		{
			if (this.yesText.text != "YES")
			{
				this.yesText.text = "YES";
			}
			if (this.noText.text != "NO")
			{
				this.noText.text = "NO";
			}
		}
		if (this.reactor != null && this.reactor.grManager != null && !fromRPC && (grplayer == grplayer2 || this.reactor.grManager.IsAuthority()))
		{
			this.reactor.grManager.PromotionBotActivePlayerRequest((int)this.currentState);
		}
	}

	// Token: 0x060035FD RID: 13821 RVA: 0x0012A208 File Offset: 0x00128408
	public int GetPurchaseToCreditCapAmount()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		int shiftCredits = grplayer.ShiftCredits;
		int num = 1000 + grplayer.ShiftCreditCapIncreases * 100;
		return Math.Max(0, num - shiftCredits);
	}

	// Token: 0x060035FE RID: 13822 RVA: 0x0012A244 File Offset: 0x00128444
	public void CelebratePromotion()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		this.particlesGO.SetActive(false);
		this.particlesGO.SetActive(true);
		this.levelUpSound.Play();
		this.popSound.Play();
		PlayerGameEvents.MiscEvent(GRUIPromotionBot.EVENT_PROMOTED, 1);
		grplayer.SendRankUpTelemetry(GhostReactorProgression.GetTitleNameAndGrade(grplayer.CurrentProgression.redeemedPoints));
	}

	// Token: 0x060035FF RID: 13823 RVA: 0x0012A2B8 File Offset: 0x001284B8
	public void SetMenuText(GRUIPromotionBot.PromotionBotState menuState)
	{
		switch (menuState)
		{
		case GRUIPromotionBot.PromotionBotState.ChoosePromotion:
			this.menuText.text = "-> REQUEST PROMOTION\n   INCREASE CREDIT LIMIT\n   BRIBE ACCOUNTING FOR CREDITS\n";
			return;
		case GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease:
			this.menuText.text = "   REQUEST PROMOTION\n-> INCREASE CREDIT LIMIT\n   BRIBE ACCOUNTING FOR CREDITS\n";
			return;
		case GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits:
		case GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits:
			this.menuText.text = "   REQUEST PROMOTION\n   INCREASE CREDIT LIMIT\n-> BRIBE ACCOUNTING FOR CREDITS\n";
			return;
		default:
			return;
		}
	}

	// Token: 0x06003600 RID: 13824 RVA: 0x0012A310 File Offset: 0x00128510
	public void SetScreenVisibility()
	{
		this.startScreenText.gameObject.SetActive(this.currentState == GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.userInfo.gameObject.SetActive(this.currentState > GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.menuText.gameObject.SetActive(this.currentState > GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.descriptionText.gameObject.SetActive(this.currentState > GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.purchaseSuccessText.gameObject.SetActive(false);
	}

	// Token: 0x06003601 RID: 13825 RVA: 0x0012A392 File Offset: 0x00128592
	public void RefreshPlayerData()
	{
		this.userInfo.text = this.FormattedUserInfo();
	}

	// Token: 0x06003602 RID: 13826 RVA: 0x0012A3A8 File Offset: 0x001285A8
	public void OnPurchaseCallback(bool success)
	{
		if (success)
		{
			this.purchaseSuccessText.text = "<color=#80ff80>     PURCHASE SUCCEEDED!</color>";
			this.RefreshPlayerData();
			this.purchaseSuccessText.gameObject.SetActive(true);
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded == null)
			{
				return;
			}
			onSucceeded.Invoke();
			return;
		}
		else
		{
			this.purchaseSuccessText.text = "<color=#ff8080>     FAILED PURCHASE. NO CHARGE.</color>";
			this.RefreshPlayerData();
			this.purchaseSuccessText.gameObject.SetActive(true);
			UnityEvent onFailed = this.scanner.onFailed;
			if (onFailed == null)
			{
				return;
			}
			onFailed.Invoke();
			return;
		}
	}

	// Token: 0x06003603 RID: 13827 RVA: 0x00129DFB File Offset: 0x00127FFB
	public void OnJuiceUpdated()
	{
		this.RefreshPlayerData();
	}

	// Token: 0x06003604 RID: 13828 RVA: 0x0012A434 File Offset: 0x00128634
	public void OnGetShiftCredit(string mothershipId, int credit)
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && grplayer.mothershipId == mothershipId)
		{
			this.RefreshPlayerData();
		}
	}

	// Token: 0x06003605 RID: 13829 RVA: 0x0012A46C File Offset: 0x0012866C
	public void OnShinyRocksUpdated()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && grplayer.gamePlayer.IsLocal())
		{
			this.RefreshPlayerData();
		}
	}

	// Token: 0x06003606 RID: 13830 RVA: 0x0012A4A4 File Offset: 0x001286A4
	public void OnGetShiftCreditCapData(string mothershipId, int creditCap, int creditCapMax)
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && grplayer.mothershipId == mothershipId)
		{
			this.RefreshPlayerData();
		}
	}

	// Token: 0x06003607 RID: 13831 RVA: 0x0012A4DC File Offset: 0x001286DC
	public void AttemptPromotion()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer && grplayer.AttemptPromotion() && this.reactor != null && this.reactor.grManager != null)
		{
			this.CelebratePromotion();
			this.RefreshPlayerData();
			this.RefreshActivePlayerBadge();
			string titleName = GhostReactorProgression.GetTitleName(grplayer.CurrentProgression.redeemedPoints);
			int grade = GhostReactorProgression.GetGrade(grplayer.CurrentProgression.redeemedPoints);
			this.purchaseSuccessText.text = string.Format("CONGRATULATIONS, {0} {1}!", titleName, grade);
			this.purchaseSuccessText.gameObject.SetActive(true);
		}
	}

	// Token: 0x06003608 RID: 13832 RVA: 0x0012A590 File Offset: 0x00128790
	public void AttemptPurchaseShiftCreditIncrease()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			Debug.Log("AttemptPurchaseShiftCreditIncrease currentPlayer null");
			return;
		}
		if (grplayer.ShiftCreditCapIncreases == grplayer.ShiftCreditCapIncreasesMax)
		{
			return;
		}
		Debug.Log(string.Format("AttemptPurchaseShiftCreditIncrease currentPlayer ShiftCreditCapIncreases {0} ShiftCreditCapIncreasesMax {1}", grplayer.ShiftCreditCapIncreases, grplayer.ShiftCreditCapIncreasesMax));
		int num = 2;
		if (grplayer != null && grplayer.gamePlayer.IsLocal() && grplayer.ShiftCreditCapIncreases < grplayer.ShiftCreditCapIncreasesMax && this.reactor.toolProgression.GetNumberOfResearchPoints() >= num && ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.PurchaseShiftCreditCapIncrease();
		}
		this.RefreshPlayerData();
	}

	// Token: 0x06003609 RID: 13833 RVA: 0x0012A64C File Offset: 0x0012884C
	public void AttemptPurchaseShiftCreditRefillToMax()
	{
		if (this.GetPurchaseToCreditCapAmount() == 0)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			Debug.Log("AttemptPurchaseShiftCreditIncrease currentPlayer null");
			return;
		}
		int num = 1000;
		int num2 = 100;
		int num3 = num + grplayer.ShiftCreditCapIncreases * num2;
		Debug.Log(string.Format("AttemptPurchaseShiftCreditIncrease currentPlayer ShiftCredits {0} ShiftCreditMax {1}", grplayer.ShiftCredits, num3));
		if (grplayer != null && grplayer.gamePlayer.IsLocal() && grplayer.ShiftCredits < num3)
		{
			int num4 = 100;
			if (ProgressionManager.Instance != null && ProgressionManager.Instance.GetShinyRocksTotal() >= num4)
			{
				ProgressionManager.Instance.PurchaseShiftCredit();
			}
		}
		this.RefreshPlayerData();
		this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits, false);
	}

	// Token: 0x0600360A RID: 13834 RVA: 0x0012A708 File Offset: 0x00128908
	public void PlayerSwipedID()
	{
		if (this.reactor == null || this.reactor.grManager == null)
		{
			return;
		}
		if (this.currentPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded == null)
			{
				return;
			}
			onSucceeded.Invoke();
			return;
		}
		else if (this.currentPlayerActorNumber != -1 && GRPlayer.Get(this.currentPlayerActorNumber) != null)
		{
			UnityEvent onFailed = this.scanner.onFailed;
			if (onFailed == null)
			{
				return;
			}
			onFailed.Invoke();
			return;
		}
		else
		{
			this.reactor.grManager.PromotionBotActivePlayerRequest(6);
			UnityEvent onSucceeded2 = this.scanner.onSucceeded;
			if (onSucceeded2 == null)
			{
				return;
			}
			onSucceeded2.Invoke();
			return;
		}
	}

	// Token: 0x0600360B RID: 13835 RVA: 0x0012A7B8 File Offset: 0x001289B8
	public void RefreshActivePlayerBadge()
	{
		if (this.currentPlayerActorNumber == -1)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && this.currentPlayerActorNumber != -1)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(this.currentPlayerActorNumber);
			if (netPlayerByID != null && grplayer.badge != null)
			{
				grplayer.badge.RefreshText(netPlayerByID);
			}
		}
	}

	// Token: 0x0600360C RID: 13836 RVA: 0x0012A81C File Offset: 0x00128A1C
	public void SetActivePlayerStateChange(int actorNumber, int state)
	{
		if (state == 0)
		{
			this.RefreshActivePlayerBadge();
			actorNumber = -1;
		}
		bool flag = this.currentPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		bool flag2 = actorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		if (flag && !flag2)
		{
			if (ProgressionManager.Instance != null)
			{
				ProgressionManager.Instance.OnPurchaseShiftCredit -= this.OnPurchaseCallback;
				ProgressionManager.Instance.OnPurchaseShiftCreditCapIncrease -= this.OnPurchaseCallback;
				ProgressionManager.Instance.OnInventoryUpdated -= this.OnJuiceUpdated;
				ProgressionManager.Instance.OnGetShiftCredit -= this.OnGetShiftCredit;
				ProgressionManager.Instance.OnGetShiftCreditCapData -= this.OnGetShiftCreditCapData;
			}
			if (CosmeticsController.instance != null)
			{
				CosmeticsController instance = CosmeticsController.instance;
				instance.OnGetCurrency = (Action)Delegate.Remove(instance.OnGetCurrency, new Action(this.OnShinyRocksUpdated));
			}
		}
		else if (!flag && flag2)
		{
			if (ProgressionManager.Instance != null)
			{
				ProgressionManager.Instance.OnPurchaseShiftCredit += this.OnPurchaseCallback;
				ProgressionManager.Instance.OnPurchaseShiftCreditCapIncrease += this.OnPurchaseCallback;
				ProgressionManager.Instance.OnInventoryUpdated += this.OnJuiceUpdated;
				ProgressionManager.Instance.OnGetShiftCredit += this.OnGetShiftCredit;
				ProgressionManager.Instance.OnGetShiftCreditCapData += this.OnGetShiftCreditCapData;
			}
			if (CosmeticsController.instance != null)
			{
				CosmeticsController instance2 = CosmeticsController.instance;
				instance2.OnGetCurrency = (Action)Delegate.Combine(instance2.OnGetCurrency, new Action(this.OnShinyRocksUpdated));
			}
		}
		this.currentPlayerActorNumber = actorNumber;
		this.SwitchState((GRUIPromotionBot.PromotionBotState)state, true);
	}

	// Token: 0x0600360D RID: 13837 RVA: 0x0012A9E8 File Offset: 0x00128BE8
	public int GetCurrentPlayerActorNumber()
	{
		return this.currentPlayerActorNumber;
	}

	// Token: 0x0400468E RID: 18062
	private static string EVENT_PROMOTED = "GRPromoted";

	// Token: 0x0400468F RID: 18063
	private GhostReactor reactor;

	// Token: 0x04004690 RID: 18064
	public TMP_Text startScreenText;

	// Token: 0x04004691 RID: 18065
	public TMP_Text userInfo;

	// Token: 0x04004692 RID: 18066
	public TMP_Text menuText;

	// Token: 0x04004693 RID: 18067
	public TMP_Text descriptionText;

	// Token: 0x04004694 RID: 18068
	public TMP_Text yesText;

	// Token: 0x04004695 RID: 18069
	public TMP_Text noText;

	// Token: 0x04004696 RID: 18070
	public TMP_Text purchaseSuccessText;

	// Token: 0x04004697 RID: 18071
	public IDCardScanner scanner;

	// Token: 0x04004698 RID: 18072
	public GameObject particlesGO;

	// Token: 0x04004699 RID: 18073
	public AudioSource levelUpSound;

	// Token: 0x0400469A RID: 18074
	public AudioSource popSound;

	// Token: 0x0400469B RID: 18075
	private string defaultText = "-N/A-\n-N/A-\n-N/A-\n-N/A-\n-N/A-\n\n-N/A-";

	// Token: 0x0400469C RID: 18076
	private string promotionTextStr1 = "CONGRATULATIONS\n ";

	// Token: 0x0400469D RID: 18077
	private string promotionTextStr2 = ".\n\nYOU ARE NOW A GRADE ";

	// Token: 0x0400469E RID: 18078
	private string promotionTextStr3 = ".\n\nYOU MAY TAKE TWO UNPAID MINUTES TO CELEBRATE, THEN RETURN TO WORK.";

	// Token: 0x0400469F RID: 18079
	private string inertButtonText = "-";

	// Token: 0x040046A0 RID: 18080
	private string buttonReturnText = "-RETURN-";

	// Token: 0x040046A1 RID: 18081
	private string requestPromotionText = "REQUEST PROMOTION";

	// Token: 0x040046A2 RID: 18082
	public const string newLine = "\n";

	// Token: 0x040046A3 RID: 18083
	public int currentPlayerActorNumber;

	// Token: 0x040046A4 RID: 18084
	public GRUIPromotionBot.PromotionBotState currentState;

	// Token: 0x040046A5 RID: 18085
	public float timeOutTime;

	// Token: 0x040046A6 RID: 18086
	public float distanceForAutoLogout = 2.5f;

	// Token: 0x040046A7 RID: 18087
	private StringBuilder cachedStringBuilder = new StringBuilder(512);

	// Token: 0x040046A8 RID: 18088
	private float timeLastDistanceCheck;

	// Token: 0x040046A9 RID: 18089
	private float timeBetweenDistanceChecks = 0.5f;

	// Token: 0x02000834 RID: 2100
	public enum PromotionBotState
	{
		// Token: 0x040046AB RID: 18091
		WaitingForLogin,
		// Token: 0x040046AC RID: 18092
		ChoosePromotion,
		// Token: 0x040046AD RID: 18093
		ChooseCreditIncrease,
		// Token: 0x040046AE RID: 18094
		ChoosePurchaseCredits,
		// Token: 0x040046AF RID: 18095
		ConfirmPurchaseCredits,
		// Token: 0x040046B0 RID: 18096
		CelebratePromotion,
		// Token: 0x040046B1 RID: 18097
		TryingLogIn
	}
}
