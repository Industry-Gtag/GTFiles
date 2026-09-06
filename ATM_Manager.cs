using System;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaNetworking.Store;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200056C RID: 1388
public class ATM_Manager : MonoBehaviour, IBuildValidation
{
	// Token: 0x170003BF RID: 959
	// (get) Token: 0x06002342 RID: 9026 RVA: 0x000BCFA9 File Offset: 0x000BB1A9
	public ATM_Manager.ATMStages CurrentATMStage
	{
		get
		{
			return this.currentATMStage;
		}
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x000BCFB4 File Offset: 0x000BB1B4
	public void Awake()
	{
		if (ATM_Manager.instance)
		{
			Object.Destroy(this);
		}
		else
		{
			ATM_Manager.instance = this;
		}
		string text = "CREATOR CODE: ";
		string text2;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE", out text2, text))
		{
			Debug.LogError("[LOCALIZATION::ATM_MANAGER] Failed to get key for [ATM_CREATOR_CODE]");
		}
		for (int i = this.atmUIs.Count - 1; i >= 0; i--)
		{
			if (this.atmUIs[i] == null)
			{
				this.atmUIs.RemoveAt(i);
			}
			else
			{
				this.atmUIs[i].SetCreatorCodeTitle(text2);
			}
		}
		this.SwitchToStage(ATM_Manager.ATMStages.Unavailable);
		this.smallDisplays = new List<CreatorCodeSmallDisplay>();
		this.ATM_TERMINAL_ID = string.Empty;
		for (int j = 0; j < this.nexusGroups.Length; j++)
		{
			string atm_TERMINAL_ID = this.ATM_TERMINAL_ID;
			NexusGroupId nexusGroupId = this.nexusGroups[j];
			this.ATM_TERMINAL_ID = atm_TERMINAL_ID + ((nexusGroupId != null) ? nexusGroupId.ToString() : null);
		}
		this.HookupToCreatorCodes();
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000BD0A8 File Offset: 0x000BB2A8
	public void Start()
	{
		Debug.Log("ATM COUNT: " + this.atmUIs.Count.ToString());
		Debug.Log("SMALL DISPLAY COUNT: " + this.smallDisplays.Count.ToString());
		GameEvents.OnGorrillaATMKeyButtonPressedEvent.AddListener(new UnityAction<GorillaATMKeyBindings>(this.PressButton));
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x000BD110 File Offset: 0x000BB310
	public void HookupToCreatorCodes()
	{
		CreatorCodes.InitializedEvent += this.CreatorCodesInitialized;
		CreatorCodes.OnCreatorCodeChangedEvent += this.OnCreatorCodeChanged;
		CreatorCodes.OnCreatorCodeFailureEvent += this.OnOnCreatorCodeFailureEvent;
		if (CreatorCodes.Intialized)
		{
			this.CreatorCodesInitialized();
		}
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x000BD160 File Offset: 0x000BB360
	public void CreatorCodesInitialized()
	{
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeField(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x000BD208 File Offset: 0x000BB408
	public void OnCreatorCodeChanged(string id)
	{
		if (id != this.ATM_TERMINAL_ID)
		{
			return;
		}
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeField(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
		string text = "CREATOR CODE:";
		CreatorCodes.CreatorCodeStatus currentCreatorCodeStatus = CreatorCodes.getCurrentCreatorCodeStatus(this.ATM_TERMINAL_ID);
		if (currentCreatorCodeStatus != CreatorCodes.CreatorCodeStatus.Validating)
		{
			if (currentCreatorCodeStatus == CreatorCodes.CreatorCodeStatus.Valid)
			{
				text += " VALID";
			}
		}
		else
		{
			text += " VALIDATING";
		}
		foreach (ATM_UI atm_UI2 in this.atmUIs)
		{
			atm_UI2.SetCreatorCodeTitle(text);
		}
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x000BD330 File Offset: 0x000BB530
	private void OnOnCreatorCodeFailureEvent(string id)
	{
		if (id != this.ATM_TERMINAL_ID)
		{
			return;
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeTitle("CREATOR CODE: INVALID");
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE_INVALID", out text, atm_UI.atmText.text);
			atm_UI.SetCreatorCodeTitle(text);
		}
		Debug.Log("ATM CODE FAILURE");
	}

	// Token: 0x06002349 RID: 9033 RVA: 0x000BD3C0 File Offset: 0x000BB5C0
	public void OnCreatorCodeInvalid(string id)
	{
		if (id != this.ATM_TERMINAL_ID)
		{
			return;
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeTitle("CREATOR CODE: INVALID");
		}
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x000BD424 File Offset: 0x000BB624
	private void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x000BD443 File Offset: 0x000BB643
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x000BD456 File Offset: 0x000BB656
	private void OnLanguageChanged()
	{
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x000BD464 File Offset: 0x000BB664
	public void PressButton(GorillaATMKeyBindings buttonPressed)
	{
		if (this.currentATMStage == ATM_Manager.ATMStages.Confirm && CreatorCodes.getCurrentCreatorCodeStatus(this.ATM_TERMINAL_ID) != CreatorCodes.CreatorCodeStatus.Validating)
		{
			string text = "CREATOR CODE: ";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE", out text2, text);
			foreach (ATM_UI atm_UI in this.atmUIs)
			{
				atm_UI.SetCreatorCodeTitle(text2);
			}
			if (buttonPressed == GorillaATMKeyBindings.delete)
			{
				CreatorCodes.DeleteCharacter(this.ATM_TERMINAL_ID);
				return;
			}
			string atm_TERMINAL_ID = this.ATM_TERMINAL_ID;
			string text3;
			if (buttonPressed >= GorillaATMKeyBindings.delete)
			{
				text3 = buttonPressed.ToString();
			}
			else
			{
				int num = (int)buttonPressed;
				text3 = num.ToString();
			}
			CreatorCodes.AppendKey(atm_TERMINAL_ID, text3);
		}
	}

	// Token: 0x0600234E RID: 9038 RVA: 0x000BD520 File Offset: 0x000BB720
	public async void ProcessATMState(ATM_UI atm_ui, string currencyButton)
	{
		switch (this.currentATMStage)
		{
		case ATM_Manager.ATMStages.Unavailable:
		case ATM_Manager.ATMStages.Purchasing:
			break;
		case ATM_Manager.ATMStages.Begin:
			this.SwitchToStage(ATM_Manager.ATMStages.Menu);
			break;
		case ATM_Manager.ATMStages.Menu:
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				if (!(currencyButton == "one"))
				{
					if (currencyButton == "four")
					{
						this.SwitchToStage(ATM_Manager.ATMStages.Begin);
					}
				}
				else
				{
					this.SwitchToStage(ATM_Manager.ATMStages.Balance);
				}
			}
			else if (!(currencyButton == "one"))
			{
				if (!(currencyButton == "two"))
				{
					if (currencyButton == "back")
					{
						this.SwitchToStage(ATM_Manager.ATMStages.Begin);
					}
				}
				else
				{
					this.SwitchToStage(ATM_Manager.ATMStages.Choose);
				}
			}
			else
			{
				this.SwitchToStage(ATM_Manager.ATMStages.Balance);
			}
			break;
		case ATM_Manager.ATMStages.Balance:
			if (currencyButton == "back")
			{
				this.SwitchToStage(ATM_Manager.ATMStages.Menu);
			}
			break;
		case ATM_Manager.ATMStages.Choose:
			if (!(currencyButton == "one"))
			{
				if (!(currencyButton == "two"))
				{
					if (!(currencyButton == "three"))
					{
						if (!(currencyButton == "four"))
						{
							if (currencyButton == "back")
							{
								this.SwitchToStage(ATM_Manager.ATMStages.Menu);
							}
						}
						else
						{
							this.numShinyRocksToBuy = 11000;
							this.shinyRocksCost = 39.99f;
							CosmeticsController.instance.itemToPurchase = "11000SHINYROCKS";
							CosmeticsController.instance.buyingBundle = false;
							this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
						}
					}
					else
					{
						this.numShinyRocksToBuy = 5000;
						this.shinyRocksCost = 19.99f;
						CosmeticsController.instance.itemToPurchase = "5000SHINYROCKS";
						CosmeticsController.instance.buyingBundle = false;
						this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
					}
				}
				else
				{
					this.numShinyRocksToBuy = 2200;
					this.shinyRocksCost = 9.99f;
					CosmeticsController.instance.itemToPurchase = "2200SHINYROCKS";
					CosmeticsController.instance.buyingBundle = false;
					this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
				}
			}
			else
			{
				this.numShinyRocksToBuy = 1000;
				this.shinyRocksCost = 4.99f;
				CosmeticsController.instance.itemToPurchase = "1000SHINYROCKS";
				CosmeticsController.instance.buyingBundle = false;
				this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
			}
			break;
		case ATM_Manager.ATMStages.Confirm:
			if (!(currencyButton == "one"))
			{
				if (currencyButton == "back")
				{
					this.SwitchToStage(ATM_Manager.ATMStages.Choose);
				}
			}
			else
			{
				if (atm_ui != null)
				{
					CosmeticsController.instance.PurchaseLocation = atm_ui.PurchaseLocation;
				}
				if (atm_ui != null && this.atmUIToMemberCode.ContainsKey(atm_ui))
				{
					this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
					CosmeticsController.instance.SetValidatedCreatorCode(this.atmUIToMemberCode[atm_ui].Item1, this.atmUIToMemberCode[atm_ui].Item2, string.Empty);
					CosmeticsController.instance.SteamPurchase();
				}
				else if (CreatorCodes.getCurrentCreatorCodeStatus(this.ATM_TERMINAL_ID) == CreatorCodes.CreatorCodeStatus.Empty)
				{
					CosmeticsController.instance.SteamPurchase();
					this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
				}
				else
				{
					this.CreatorCodeValidating();
					NexusManager.MemberCode memberCode = await CreatorCodes.CheckValidationCoroutineJIT(this.ATM_TERMINAL_ID, CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID), this.nexusGroups);
					if (memberCode != null)
					{
						this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
						CosmeticsController.instance.SetValidatedCreatorCode(memberCode.memberCode, memberCode.groupId.Code, this.ATM_TERMINAL_ID);
						CosmeticsController.instance.SteamPurchase();
					}
					else
					{
						this.OnCreatorCodeInvalid(this.ATM_TERMINAL_ID);
					}
				}
			}
			break;
		default:
			this.SwitchToStage(ATM_Manager.ATMStages.Menu);
			break;
		}
	}

	// Token: 0x0600234F RID: 9039 RVA: 0x000BD568 File Offset: 0x000BB768
	public void AddATM(ATM_UI newATM, Tuple<string, string> creatorCode)
	{
		this.atmUIs.RemoveAll((ATM_UI atm) => !atm);
		this.atmUIs.Add(newATM);
		if (creatorCode != null)
		{
			this.atmUIToMemberCode.Add(newATM, creatorCode);
		}
		else
		{
			newATM.SetCreatorCodeField(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x06002350 RID: 9040 RVA: 0x000BD5DB File Offset: 0x000BB7DB
	public void RemoveATM(ATM_UI atmToRemove)
	{
		this.atmUIs.Remove(atmToRemove);
	}

	// Token: 0x06002351 RID: 9041 RVA: 0x000BD5EC File Offset: 0x000BB7EC
	public void CreatorCodeValidating()
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeTitle("CREATOR CODE: VALIDATING");
		}
	}

	// Token: 0x06002352 RID: 9042 RVA: 0x000BD644 File Offset: 0x000BB844
	public void CreatorCodeValid()
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.SetCreatorCodeTitle("CREATOR CODE: VALIDATING");
		}
		if (this.currentATMStage == ATM_Manager.ATMStages.Confirm)
		{
			this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
		}
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x000BD6AC File Offset: 0x000BB8AC
	public void SwitchToStage(ATM_Manager.ATMStages newStage)
	{
		this.currentATMStage = newStage;
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			if (atm_UI && atm_UI.atmText)
			{
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				switch (newStage)
				{
				case ATM_Manager.ATMStages.Unavailable:
					atm_UI.atmText.text = "ATM NOT AVAILABLE! PLEASE TRY AGAIN LATER!";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_NOT_AVAILABLE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.Begin:
					atm_UI.atmText.text = "WELCOME! PRESS ANY BUTTON TO BEGIN.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_STARTUP", out text, atm_UI.atmText.text);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_BEGIN", out text5, "BEGIN");
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = text5;
					atm_UI.ATM_RightColumnArrowText[3].enabled = true;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.Menu:
					if (PlayFabAuthenticator.instance.GetSafety())
					{
						atm_UI.atmText.text = "CHECK YOUR BALANCE.";
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_CHECK_YOUR_BALANCE", out text, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_BALANCE", out text2, atm_UI.atmText.text);
						atm_UI.atmText.text = text;
						atm_UI.ATM_RightColumnButtonText[0].text = text2;
						atm_UI.ATM_RightColumnArrowText[0].enabled = true;
						atm_UI.ATM_RightColumnButtonText[1].text = "";
						atm_UI.ATM_RightColumnArrowText[1].enabled = false;
						atm_UI.ATM_RightColumnButtonText[2].text = "";
						atm_UI.ATM_RightColumnArrowText[2].enabled = false;
						atm_UI.ATM_RightColumnButtonText[3].text = "";
						atm_UI.ATM_RightColumnArrowText[3].enabled = false;
						atm_UI.HideCreatorCode();
					}
					else
					{
						atm_UI.atmText.text = "CHECK YOUR BALANCE OR PURCHASE MORE SHINY ROCKS.";
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_MAIN_SCREEN", out text, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_BALANCE", out text2, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE", out text3, atm_UI.atmText.text);
						atm_UI.atmText.text = text;
						atm_UI.ATM_RightColumnButtonText[0].text = text2;
						atm_UI.ATM_RightColumnArrowText[0].enabled = true;
						atm_UI.ATM_RightColumnButtonText[1].text = text3;
						atm_UI.ATM_RightColumnArrowText[1].enabled = true;
						atm_UI.ATM_RightColumnButtonText[2].text = "";
						atm_UI.ATM_RightColumnArrowText[2].enabled = false;
						atm_UI.ATM_RightColumnButtonText[3].text = "";
						atm_UI.ATM_RightColumnArrowText[3].enabled = false;
						atm_UI.HideCreatorCode();
					}
					break;
				case ATM_Manager.ATMStages.Balance:
					atm_UI.atmText.text = "CURRENT BALANCE:\n\n" + CosmeticsController.instance.CurrencyBalance.ToString();
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CURRENT_BALANCE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text + "\n\n" + CosmeticsController.instance.CurrencyBalance.ToString();
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.Choose:
				{
					string text6 = "{numShinyRocksToBuy} - {currencySymbol}{shinyRocksCost}";
					string text7 = "{numShinyRocksToBuy} - {currencySymbol}{shinyRocksCost}\r\n({discount}% BONUS!";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_FIRST", out text2, text6);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text3, text7);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text4, text7);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text5, text7);
					text2 = text2.Replace("{numShinyRocksToBuy}", "1000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "4.99");
					text3 = text3.Replace("{numShinyRocksToBuy}", "2200").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "9.99")
						.Replace("{discount}", "10");
					text4 = text4.Replace("{numShinyRocksToBuy}", "5000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "19.99")
						.Replace("{discount}", "25");
					text5 = text5.Replace("{numShinyRocksToBuy}", "11000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "39.99")
						.Replace("{discount}", "37");
					atm_UI.atmText.text = "CHOOSE AN AMOUNT OF SHINY ROCKS TO PURCHASE.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CHOOSE_PURCHASE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = text2;
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = text3;
					atm_UI.ATM_RightColumnArrowText[1].enabled = true;
					atm_UI.ATM_RightColumnButtonText[2].text = text4;
					atm_UI.ATM_RightColumnArrowText[2].enabled = true;
					atm_UI.ATM_RightColumnButtonText[3].text = text5;
					atm_UI.ATM_RightColumnArrowText[3].enabled = true;
					atm_UI.HideCreatorCode();
					break;
				}
				case ATM_Manager.ATMStages.Confirm:
					atm_UI.atmText.text = string.Concat(new string[]
					{
						"YOU HAVE CHOSEN TO PURCHASE ",
						this.numShinyRocksToBuy.ToString(),
						" SHINY ROCKS FOR $",
						this.shinyRocksCost.ToString(),
						". CONFIRM TO LAUNCH A STEAM WINDOW TO COMPLETE YOUR PURCHASE."
					});
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_CONFIRMATION_STEAM", out text, atm_UI.atmText.text);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CONFIRM", out text2, "CONFIRM");
					text = text.Replace("{numShinyRocksToBuy}", this.numShinyRocksToBuy.ToString());
					text = text.Replace("{currencySymbol}", "$");
					text = text.Replace("{shinyRocksCost}", this.shinyRocksCost.ToString());
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = text2;
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.ShowCreatorCode();
					break;
				case ATM_Manager.ATMStages.Purchasing:
					atm_UI.atmText.text = "PURCHASING IN STEAM...";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASING", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.Success:
					atm_UI.atmText.text = "SUCCESS! NEW SHINY ROCKS BALANCE: " + (CosmeticsController.instance.CurrencyBalance + this.numShinyRocksToBuy).ToString();
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_SUCCESS_NEW_BALANCE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text + (CosmeticsController.instance.CurrencyBalance + this.numShinyRocksToBuy).ToString();
					if (CreatorCodes.getCurrentCreatorCodeStatus(this.ATM_TERMINAL_ID) == CreatorCodes.CreatorCodeStatus.Valid)
					{
						string name = CreatorCodes.supportedMember.name;
						if (!string.IsNullOrEmpty(name))
						{
							TMP_Text atmText = atm_UI.atmText;
							atmText.text = atmText.text + "\n\nTHIS PURCHASE SUPPORTED\n" + name + "!";
							foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
							{
								creatorCodeSmallDisplay.SuccessfulPurchase(name);
							}
						}
					}
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.Failure:
					atm_UI.atmText.text = "PURCHASE CANCELLED. NO FUNDS WERE SPENT.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_CANCELLED", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.HideCreatorCode();
					break;
				case ATM_Manager.ATMStages.SafeAccount:
					atm_UI.atmText.text = "Out Of Order.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASING_DISABLED_OUT_OF_ORDER", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.HideCreatorCode();
					break;
				}
			}
		}
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x000BE250 File Offset: 0x000BC450
	public void SetATMText(string newText)
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.atmText.text = newText;
		}
	}

	// Token: 0x06002355 RID: 9045 RVA: 0x000BE2A8 File Offset: 0x000BC4A8
	public void PressCurrencyPurchaseButton(ATM_UI atm_ui, string currencyPurchaseSize)
	{
		this.ProcessATMState(atm_ui, currencyPurchaseSize);
	}

	// Token: 0x06002356 RID: 9046 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void LeaveSystemMenu()
	{
	}

	// Token: 0x06002357 RID: 9047 RVA: 0x000BE2B2 File Offset: 0x000BC4B2
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.nexusGroups.Length == 0)
		{
			Debug.LogError("You have to set at least one nexusGroup in " + base.name + " or things will not work!");
			return false;
		}
		return true;
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x000BE2DC File Offset: 0x000BC4DC
	internal void SetTemporaryCreatorCode(string code)
	{
		if (code == null)
		{
			CreatorCodes.ResetCreatorCode(this.ATM_TERMINAL_ID);
			CreatorCodes.AppendKey(this.ATM_TERMINAL_ID, this._tempCreatorCodeOveride);
			this._tempCreatorCodeOveride = null;
			return;
		}
		if (this._tempCreatorCodeOveride == null)
		{
			this._tempCreatorCodeOveride = CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID);
		}
		CreatorCodes.ResetCreatorCode(this.ATM_TERMINAL_ID);
		CreatorCodes.AppendKey(this.ATM_TERMINAL_ID, code);
	}

	// Token: 0x04002E4E RID: 11854
	private const string ATM_STARTUP_KEY = "ATM_STARTUP";

	// Token: 0x04002E4F RID: 11855
	private const string ATM_SCREEN_KEY = "ATM_SCREEN";

	// Token: 0x04002E50 RID: 11856
	private const string ATM_NOT_AVAILABLE_KEY = "ATM_NOT_AVAILABLE";

	// Token: 0x04002E51 RID: 11857
	private const string ATM_BEGIN_KEY = "ATM_BEGIN";

	// Token: 0x04002E52 RID: 11858
	private const string ATM_MAIN_SCREEN_KEY = "ATM_MAIN_SCREEN";

	// Token: 0x04002E53 RID: 11859
	private const string ATM_CHECK_YOUR_BALANCE_KEY = "ATM_CHECK_YOUR_BALANCE";

	// Token: 0x04002E54 RID: 11860
	private const string ATM_PURCHASING_DISABLED_OUT_OF_ORDER_KEY = "ATM_PURCHASING_DISABLED_OUT_OF_ORDER";

	// Token: 0x04002E55 RID: 11861
	private const string ATM_CURRENT_BALANCE_KEY = "ATM_CURRENT_BALANCE";

	// Token: 0x04002E56 RID: 11862
	private const string ATM_MODDED_CLIENT_KEY = "ATM_MODDED_CLIENT";

	// Token: 0x04002E57 RID: 11863
	private const string ATM_CHOOSE_PURCHASE_KEY = "ATM_CHOOSE_PURCHASE";

	// Token: 0x04002E58 RID: 11864
	private const string ATM_PURCHASE_CONFIRMATION_KEY = "ATM_PURCHASE_CONFIRMATION";

	// Token: 0x04002E59 RID: 11865
	private const string ATM_PURCHASE_CONFIRMATION_STEAM_KEY = "ATM_PURCHASE_CONFIRMATION_STEAM";

	// Token: 0x04002E5A RID: 11866
	private const string ATM_PURCHASING_KEY = "ATM_PURCHASING";

	// Token: 0x04002E5B RID: 11867
	private const string ATM_SUCCESS_NEW_BALANCE_KEY = "ATM_SUCCESS_NEW_BALANCE";

	// Token: 0x04002E5C RID: 11868
	private const string ATM_PURCHASE_CANCELLED_KEY = "ATM_PURCHASE_CANCELLED";

	// Token: 0x04002E5D RID: 11869
	private const string ATM_LOCKED_KEY = "ATM_LOCKED";

	// Token: 0x04002E5E RID: 11870
	private const string ATM_RETURN_KEY = "ATM_RETURN";

	// Token: 0x04002E5F RID: 11871
	private const string ATM_BACK_KEY = "ATM_BACK";

	// Token: 0x04002E60 RID: 11872
	private const string ATM_CONFIRM_KEY = "ATM_CONFIRM";

	// Token: 0x04002E61 RID: 11873
	private const string ATM_IAP_NOT_AVAILABLE_KEY = "ATM_IAP_NOT_AVAILABLE";

	// Token: 0x04002E62 RID: 11874
	private const string ATM_BALANCE_KEY = "ATM_BALANCE";

	// Token: 0x04002E63 RID: 11875
	private const string ATM_PURCHASE_KEY = "ATM_PURCHASE";

	// Token: 0x04002E64 RID: 11876
	private const string ATM_CREATOR_CODE_KEY = "ATM_CREATOR_CODE";

	// Token: 0x04002E65 RID: 11877
	private const string ATM_CREATOR_CODE_VALIDATING_KEY = "ATM_CREATOR_CODE_VALIDATING";

	// Token: 0x04002E66 RID: 11878
	private const string ATM_CREATOR_CODE_VALID_KEY = "ATM_CREATOR_CODE_VALID";

	// Token: 0x04002E67 RID: 11879
	private const string ATM_CREATOR_CODE_INVALID_KEY = "ATM_CREATOR_CODE_INVALID";

	// Token: 0x04002E68 RID: 11880
	private const string ATM_PURCHASE_OPTION_FIRST_KEY = "ATM_PURCHASE_OPTION_FIRST";

	// Token: 0x04002E69 RID: 11881
	private const string ATM_PURCHASE_OPTION_SECOND_KEY = "ATM_PURCHASE_OPTION_SECOND";

	// Token: 0x04002E6A RID: 11882
	private const string ATM_PURCHASE_OPTION_THIRD_KEY = "ATM_PURCHASE_OPTION_THIRD";

	// Token: 0x04002E6B RID: 11883
	private const string ATM_PURCHASE_OPTION_FOURTH_KEY = "ATM_PURCHASE_OPTION_FOURTH";

	// Token: 0x04002E6C RID: 11884
	[OnEnterPlay_SetNull]
	public static volatile ATM_Manager instance;

	// Token: 0x04002E6D RID: 11885
	private const int MAX_CODE_LENGTH = 10;

	// Token: 0x04002E6E RID: 11886
	public List<ATM_UI> atmUIs = new List<ATM_UI>();

	// Token: 0x04002E6F RID: 11887
	public Dictionary<ATM_UI, Tuple<string, string>> atmUIToMemberCode = new Dictionary<ATM_UI, Tuple<string, string>>();

	// Token: 0x04002E70 RID: 11888
	[HideInInspector]
	public List<CreatorCodeSmallDisplay> smallDisplays;

	// Token: 0x04002E71 RID: 11889
	private ATM_Manager.ATMStages currentATMStage;

	// Token: 0x04002E72 RID: 11890
	public int numShinyRocksToBuy;

	// Token: 0x04002E73 RID: 11891
	public float shinyRocksCost;

	// Token: 0x04002E74 RID: 11892
	public bool alreadyBegan;

	// Token: 0x04002E75 RID: 11893
	[SerializeField]
	private NexusGroupId[] nexusGroups;

	// Token: 0x04002E76 RID: 11894
	private string _tempCreatorCodeOveride;

	// Token: 0x04002E77 RID: 11895
	private string ATM_TERMINAL_ID = "atm_terminal_id";

	// Token: 0x0200056D RID: 1389
	public enum ATMStages
	{
		// Token: 0x04002E79 RID: 11897
		Unavailable,
		// Token: 0x04002E7A RID: 11898
		Begin,
		// Token: 0x04002E7B RID: 11899
		Menu,
		// Token: 0x04002E7C RID: 11900
		Balance,
		// Token: 0x04002E7D RID: 11901
		Choose,
		// Token: 0x04002E7E RID: 11902
		Confirm,
		// Token: 0x04002E7F RID: 11903
		Purchasing,
		// Token: 0x04002E80 RID: 11904
		Success,
		// Token: 0x04002E81 RID: 11905
		Failure,
		// Token: 0x04002E82 RID: 11906
		SafeAccount
	}
}
