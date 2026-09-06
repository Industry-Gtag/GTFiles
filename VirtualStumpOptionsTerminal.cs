using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio;
using Modio.Customizations;
using Modio.Users;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000ACC RID: 2764
public class VirtualStumpOptionsTerminal : MonoBehaviour, IWssAuthPrompter
{
	// Token: 0x060046E5 RID: 18149 RVA: 0x0017EA6C File Offset: 0x0017CC6C
	public void Start()
	{
		this.optionList.gameObject.SetActive(true);
		this.mainScreenText.gameObject.SetActive(true);
		this.RefreshButtonState();
		this.UpdateOptionListForCurrentState();
		this.UpdateScreen();
		CustomMapsKeyboard customMapsKeyboard = this.keyboard;
		if (customMapsKeyboard != null)
		{
			customMapsKeyboard.OnKeyPressed.AddListener(new UnityAction<CustomMapKeyboardBinding>(this.OnKeyPressed));
		}
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoginStarted.AddListener(new UnityAction(this.OnModIOLoginStarted));
		ModIOManager.OnModIOLoginFailed.AddListener(new UnityAction<string>(this.OnModIOLoginFailed));
		ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
	}

	// Token: 0x060046E6 RID: 18150 RVA: 0x0017EB28 File Offset: 0x0017CD28
	public void OnDestroy()
	{
		CustomMapsKeyboard customMapsKeyboard = this.keyboard;
		if (customMapsKeyboard != null)
		{
			customMapsKeyboard.OnKeyPressed.RemoveListener(new UnityAction<CustomMapKeyboardBinding>(this.OnKeyPressed));
		}
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoginStarted.RemoveListener(new UnityAction(this.OnModIOLoginStarted));
		ModIOManager.OnModIOLoginFailed.RemoveListener(new UnityAction<string>(this.OnModIOLoginFailed));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
	}

	// Token: 0x060046E7 RID: 18151 RVA: 0x0017EBC5 File Offset: 0x0017CDC5
	public void OnEnable()
	{
		this.RefreshButtonState();
		this.UpdateOptionListForCurrentState();
		this.UpdateScreen();
	}

	// Token: 0x060046E8 RID: 18152 RVA: 0x0017EBDC File Offset: 0x0017CDDC
	private void OnKeyPressed(CustomMapKeyboardBinding pressedButton)
	{
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.cachedError = null;
			this.RefreshButtonState();
			this.UpdateScreen();
			return;
		}
		if (pressedButton == CustomMapKeyboardBinding.up)
		{
			int num = this.currentState - VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE;
			if (num < 0)
			{
				num = 1;
			}
			this.ChangeState((VirtualStumpOptionsTerminal.ETerminalState)num);
			this.UpdateOptionListForCurrentState();
			this.UpdateScreen();
			return;
		}
		if (pressedButton == CustomMapKeyboardBinding.down)
		{
			int num2 = (int)(this.currentState + 1);
			if (num2 >= 2)
			{
				num2 = 0;
			}
			this.ChangeState((VirtualStumpOptionsTerminal.ETerminalState)num2);
			this.UpdateOptionListForCurrentState();
			this.UpdateScreen();
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			this.OnKeyPressed_ModIOAccount(pressedButton);
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		this.OnKeyPressed_RoomSize(pressedButton);
	}

	// Token: 0x060046E9 RID: 18153 RVA: 0x0017EC78 File Offset: 0x0017CE78
	private void ChangeState(VirtualStumpOptionsTerminal.ETerminalState newState)
	{
		if (newState == this.currentState)
		{
			return;
		}
		this.currentState = newState;
		this.RefreshButtonState();
	}

	// Token: 0x060046EA RID: 18154 RVA: 0x0017EC94 File Offset: 0x0017CE94
	private void RefreshButtonState()
	{
		for (int i = 0; i < this.contextualButtons.Count; i++)
		{
			if (this.contextualButtons[i].IsNotNull())
			{
				this.contextualButtons[i].SetActive(false);
			}
		}
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.OKButton.SetActive(true);
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			for (int j = 0; j < this.buttonsToShow_MODIO.Count; j++)
			{
				if (this.buttonsToShow_MODIO[j].IsNotNull())
				{
					this.buttonsToShow_MODIO[j].SetActive(true);
				}
			}
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		for (int k = 0; k < this.buttonsToShow_ROOMSIZE.Count; k++)
		{
			if (this.buttonsToShow_ROOMSIZE[k].IsNotNull())
			{
				this.buttonsToShow_ROOMSIZE[k].SetActive(true);
			}
		}
	}

	// Token: 0x060046EB RID: 18155 RVA: 0x0017ED7C File Offset: 0x0017CF7C
	private void UpdateOptionListForCurrentState()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 2; i++)
		{
			stringBuilder.Append(this.optionStrings[i]);
			if (i == (int)this.currentState)
			{
				stringBuilder.Append(" <-");
			}
			stringBuilder.Append("\n");
		}
		this.optionList.text = stringBuilder.ToString();
	}

	// Token: 0x060046EC RID: 18156 RVA: 0x0017EDE0 File Offset: 0x0017CFE0
	private void UpdateScreen()
	{
		this.mainScreenText.text = "";
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.RefreshButtonState();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.cachedError);
			TMP_Text tmp_Text = this.mainScreenText;
			string text = "<color=\"red\">";
			StringBuilder stringBuilder2 = stringBuilder;
			tmp_Text.text = text + ((stringBuilder2 != null) ? stringBuilder2.ToString() : null);
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			this.mainScreenText.text = this.UpdateScreen_ModIOAccount();
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		this.mainScreenText.text = this.UpdateScreen_RoomSize();
	}

	// Token: 0x060046ED RID: 18157 RVA: 0x0017EE78 File Offset: 0x0017D078
	private void OnModIOLoginStarted()
	{
		this.UpdateScreen();
	}

	// Token: 0x060046EE RID: 18158 RVA: 0x0017EE80 File Offset: 0x0017D080
	private void OnModIOLoggedIn()
	{
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		this.processingAccountLink = false;
		this.UpdateScreen();
		AssociateMotherhsipAndModIOAccountsRequest associateMotherhsipAndModIOAccountsRequest = new AssociateMotherhsipAndModIOAccountsRequest();
		associateMotherhsipAndModIOAccountsRequest.ModIOId = ModIOManager.GetCurrentUserId();
		associateMotherhsipAndModIOAccountsRequest.ModIOToken = ModIOManager.GetCurrentAuthToken();
		associateMotherhsipAndModIOAccountsRequest.MothershipEnvId = MothershipClientApiUnity.EnvironmentId;
		associateMotherhsipAndModIOAccountsRequest.MothershipPlayerId = MothershipClientContext.MothershipId;
		associateMotherhsipAndModIOAccountsRequest.MothershipToken = MothershipClientContext.Token;
		base.StartCoroutine(ModIOManager.AssociateMothershipAndModIOAccounts(associateMotherhsipAndModIOAccountsRequest, delegate(AssociateMotherhsipAndModIOAccountsResponse response)
		{
		}));
	}

	// Token: 0x060046EF RID: 18159 RVA: 0x0017EF2D File Offset: 0x0017D12D
	private void OnModIOLoggedOut()
	{
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		this.processingAccountLink = false;
		this.UpdateScreen();
	}

	// Token: 0x060046F0 RID: 18160 RVA: 0x0017EF52 File Offset: 0x0017D152
	private void OnModIOLoginFailed(string error)
	{
		this.processingAccountLink = false;
		this.cachedError = error;
		this.UpdateScreen();
	}

	// Token: 0x060046F1 RID: 18161 RVA: 0x0017EE78 File Offset: 0x0017D078
	private void OnModIOUserChanged(User user)
	{
		this.UpdateScreen();
	}

	// Token: 0x060046F2 RID: 18162 RVA: 0x0017EF68 File Offset: 0x0017D168
	private void OnKeyPressed_ModIOAccount(CustomMapKeyboardBinding pressedButton)
	{
		if (pressedButton == CustomMapKeyboardBinding.option1)
		{
			this.StartAccountLinkingProcess();
		}
		if (pressedButton == CustomMapKeyboardBinding.option2)
		{
			GTDev.Log<string>(string.Format("[VirtualStumpOptionsTerminal::OnKeyPressed_ModIOAccount] logout {0}", ModIOManager.IsLoggedIn()), null);
			if (ModIOManager.IsLoggedIn())
			{
				ModIOManager.LogoutFromModIO();
			}
		}
		if (pressedButton == CustomMapKeyboardBinding.option3)
		{
			GTDev.Log<string>(string.Format("[VirtualStumpOptionsTerminal::OnKeyPressed_ModIOAccount] login {0}", ModIOManager.IsLoggedIn()), null);
			if (!ModIOManager.IsLoggedIn())
			{
				ModIOManager.CancelExternalAuthentication();
				try
				{
					ModIOManager.RequestPlatformLogin();
				}
				catch (Exception ex)
				{
					GTDev.Log<string>(string.Format("VirtualStumpOptionsTerminal::OnKeyPressed_ModIOAccount platform login error: {0}", ex), null);
					throw;
				}
			}
		}
	}

	// Token: 0x060046F3 RID: 18163 RVA: 0x0017F004 File Offset: 0x0017D204
	private async Task StartAccountLinkingProcess()
	{
		if (!this.processingAccountLink)
		{
			this.processingAccountLink = true;
			if (ModIOManager.IsAuthenticated(false))
			{
				if (ModIOManager.GetLastAuthMethod() == ModIOManager.ModIOAuthMethod.LinkedAccount)
				{
					ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
					ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
					ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
					ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
					this.processingAccountLink = false;
					return;
				}
				ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
				ModIOManager.LogoutFromModIO();
				ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
				ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
				ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
				ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
			}
			ModIOManager.SetAccountLinkPrompter(this);
			Error error = await ModIOManager.RequestAccountLinkCode();
			if (error)
			{
				Debug.LogError("[ModIOAccountLinkingTerminal::StartAccountLinkingProcess] Failed to log in to mod.io: " + error.GetMessage());
				this.cachedError = error.GetMessage() + "\n\nPRESS THE 'LINK MOD.IO ACCOUNT' BUTTON TO RETRY.";
				this.processingAccountLink = false;
				this.UpdateScreen();
			}
		}
		else
		{
			this.UpdateScreen();
		}
	}

	// Token: 0x060046F4 RID: 18164 RVA: 0x0017F047 File Offset: 0x0017D247
	public void ShowPrompt(string url, string code)
	{
		this.cachedLinkURL = url;
		this.cachedLinkCode = code;
		this.UpdateScreen();
	}

	// Token: 0x060046F5 RID: 18165 RVA: 0x0017F060 File Offset: 0x0017D260
	private string UpdateScreen_ModIOAccount()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (ModIOManager.IsLoggedIn())
		{
			stringBuilder.Append(this.loggedInAsString + "\n");
			stringBuilder.Append("   " + ModIOManager.GetCurrentUsername() + "\n\n");
			if (ModIOManager.GetLastAuthMethod() != ModIOManager.ModIOAuthMethod.LinkedAccount)
			{
				stringBuilder.Append(this.linkAccountPromptString + "\n");
			}
			else
			{
				stringBuilder.Append(this.alreadyLinkedAccountString + "\n");
			}
		}
		else if (ModIOManager.IsLoggingIn() && !this.processingAccountLink)
		{
			stringBuilder.Append(this.loggingInString);
		}
		else if (ModIOManager.IsLoggingOut())
		{
			stringBuilder.Append(this.loggingOutString);
		}
		else if (this.processingAccountLink)
		{
			stringBuilder.Append(this.linkAccountPromptString + "\n\n");
			stringBuilder.Append(this.urlLabelString + this.cachedLinkURL + "\n");
			stringBuilder.Append(this.linkCodeLabelString + this.cachedLinkCode + "\n");
		}
		else
		{
			stringBuilder.Append(this.notLoggedInString + "\n\n");
			stringBuilder.Append(this.loginPromptString);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060046F6 RID: 18166 RVA: 0x0017F1A8 File Offset: 0x0017D3A8
	private void OnKeyPressed_RoomSize(CustomMapKeyboardBinding pressedButton)
	{
		if (pressedButton == CustomMapKeyboardBinding.left)
		{
			this.DecrementRoomSize();
		}
		if (pressedButton == CustomMapKeyboardBinding.right)
		{
			this.IncrementRoomSize();
		}
		this.UpdateScreen();
	}

	// Token: 0x060046F7 RID: 18167 RVA: 0x0017F1C6 File Offset: 0x0017D3C6
	private void DecrementRoomSize()
	{
		RoomSystem.OverrideRoomSize(RoomSystem.GetOverridenRoomSize() - 1);
		this.UpdateScreen();
	}

	// Token: 0x060046F8 RID: 18168 RVA: 0x0017F1DB File Offset: 0x0017D3DB
	private void IncrementRoomSize()
	{
		RoomSystem.OverrideRoomSize(RoomSystem.GetOverridenRoomSize() + 1);
		this.UpdateScreen();
	}

	// Token: 0x060046F9 RID: 18169 RVA: 0x0017F1F0 File Offset: 0x0017D3F0
	private string UpdateScreen_RoomSize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.roomSizeDescriptionString + "\n\n");
		stringBuilder.Append(this.roomSizeLabelString + RoomSystem.GetOverridenRoomSize().ToString());
		return stringBuilder.ToString();
	}

	// Token: 0x04005973 RID: 22899
	[SerializeField]
	private TMP_Text optionList;

	// Token: 0x04005974 RID: 22900
	[SerializeField]
	private TMP_Text mainScreenText;

	// Token: 0x04005975 RID: 22901
	[SerializeField]
	private CustomMapsKeyboard keyboard;

	// Token: 0x04005976 RID: 22902
	[SerializeField]
	private List<string> optionStrings = new List<string> { "MOD.IO", "ROOM SIZE" };

	// Token: 0x04005977 RID: 22903
	[SerializeField]
	private string loggedInAsString = "LOGGED INTO MOD.IO AS: ";

	// Token: 0x04005978 RID: 22904
	[SerializeField]
	private string notLoggedInString = "LOGGED OUT OF MOD.IO";

	// Token: 0x04005979 RID: 22905
	[SerializeField]
	private string loginPromptString = "PRESS THE 'PLATFORM LOGIN' OR 'LINK MOD.IO ACCOUNT' BUTTON TO LOGIN";

	// Token: 0x0400597A RID: 22906
	[SerializeField]
	private string loggingInString = "LOGGING IN TO MOD.IO...";

	// Token: 0x0400597B RID: 22907
	[SerializeField]
	private string loggingOutString = "LOGGING OUT OF MOD.IO...";

	// Token: 0x0400597C RID: 22908
	[SerializeField]
	private string linkAccountPromptString = "IF YOU HAVE AN EXISTING MOD.IO ACCOUNT, YOU CAN LINK IT BY PRESSING THE 'LINK MOD.IO ACCOUNT' BUTTON.";

	// Token: 0x0400597D RID: 22909
	[SerializeField]
	private string alreadyLinkedAccountString = "YOU'VE ALREADY LINKED YOUR MOD.IO ACCOUNT.";

	// Token: 0x0400597E RID: 22910
	[SerializeField]
	private string accountLinkingPromptString = "PLEASE GO TO THIS URL IN YOUR BROWSER AND LOG IN TO YOUR MOD.IO ACCOUNT. ONCE LOGGED IN, ENTER THE FOLLOWING CODE TO PROCEED: ";

	// Token: 0x0400597F RID: 22911
	[SerializeField]
	private string urlLabelString = "URL: ";

	// Token: 0x04005980 RID: 22912
	[SerializeField]
	private string linkCodeLabelString = "CODE: ";

	// Token: 0x04005981 RID: 22913
	[SerializeField]
	private string roomSizeDescriptionString = "THIS SETTING WILL CHANGE THE MAXIMUM AMOUNT OF PLAYERS ALLOWED IN PRIVATE ROOMS YOU CREATE. WHEN JOINING A PUBLIC ROOM, THE MAP YOU'VE LOADED WILL CONTROL THE ROOM SIZE.";

	// Token: 0x04005982 RID: 22914
	[SerializeField]
	private string roomSizeLabelString = "MAX PLAYERS: ";

	// Token: 0x04005983 RID: 22915
	[SerializeField]
	private GameObject OKButton;

	// Token: 0x04005984 RID: 22916
	[SerializeField]
	private List<GameObject> contextualButtons = new List<GameObject>();

	// Token: 0x04005985 RID: 22917
	[SerializeField]
	private List<GameObject> buttonsToShow_MODIO = new List<GameObject>();

	// Token: 0x04005986 RID: 22918
	[SerializeField]
	private List<GameObject> buttonsToShow_ROOMSIZE = new List<GameObject>();

	// Token: 0x04005987 RID: 22919
	private bool processingAccountLink;

	// Token: 0x04005988 RID: 22920
	private string cachedLinkURL = "";

	// Token: 0x04005989 RID: 22921
	private string cachedLinkCode = "";

	// Token: 0x0400598A RID: 22922
	private string cachedError;

	// Token: 0x0400598B RID: 22923
	private VirtualStumpOptionsTerminal.ETerminalState currentState;

	// Token: 0x02000ACD RID: 2765
	private enum ETerminalState
	{
		// Token: 0x0400598D RID: 22925
		MODIO_ACCOUNT,
		// Token: 0x0400598E RID: 22926
		ROOM_SIZE,
		// Token: 0x0400598F RID: 22927
		NUM_STATES
	}
}
