using System;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000AC8 RID: 2760
public class CustomMapsTerminal : MonoBehaviour
{
	// Token: 0x170006A2 RID: 1698
	// (get) Token: 0x060046BE RID: 18110 RVA: 0x0017DC8E File Offset: 0x0017BE8E
	public static int LocalPlayerID
	{
		get
		{
			return NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x170006A3 RID: 1699
	// (get) Token: 0x060046BF RID: 18111 RVA: 0x0017DC9F File Offset: 0x0017BE9F
	public static long LocalModDetailsID
	{
		get
		{
			return CustomMapsTerminal.localModDetailsID;
		}
	}

	// Token: 0x170006A4 RID: 1700
	// (get) Token: 0x060046C0 RID: 18112 RVA: 0x0017DCA6 File Offset: 0x0017BEA6
	public static int CurrentScreen
	{
		get
		{
			return (int)CustomMapsTerminal.localCurrentScreen;
		}
	}

	// Token: 0x170006A5 RID: 1701
	// (get) Token: 0x060046C1 RID: 18113 RVA: 0x0017DCAD File Offset: 0x0017BEAD
	public static CustomMapsTerminal.ScreenType PreviousScreen
	{
		get
		{
			return CustomMapsTerminal.previousScreen;
		}
	}

	// Token: 0x170006A6 RID: 1702
	// (get) Token: 0x060046C2 RID: 18114 RVA: 0x0017DCB4 File Offset: 0x0017BEB4
	public static bool IsDriver
	{
		get
		{
			return CustomMapsTerminal.localDriverID == CustomMapsTerminal.LocalPlayerID;
		}
	}

	// Token: 0x060046C3 RID: 18115 RVA: 0x0017DCC2 File Offset: 0x0017BEC2
	private void Awake()
	{
		CustomMapsTerminal.instance = this;
		CustomMapsTerminal.hasInstance = true;
	}

	// Token: 0x060046C4 RID: 18116 RVA: 0x0017DCD0 File Offset: 0x0017BED0
	private void Start()
	{
		CustomMapsTerminal.localDriverID = -2;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
		this.controlAccessScreen.Show();
		this.detailsAccessScreen.Show();
		this.modListScreen.Hide();
		this.modDetailsScreen.Hide();
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnReturnedToSinglePlayer;
	}

	// Token: 0x060046C5 RID: 18117 RVA: 0x0017DD8C File Offset: 0x0017BF8C
	private void OnDestroy()
	{
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnReturnedToSinglePlayer;
	}

	// Token: 0x060046C6 RID: 18118 RVA: 0x0017DE08 File Offset: 0x0017C008
	public static void ShowDetailsScreen(Mod mod)
	{
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.localCurrentScreen;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
		CustomMapsTerminal.localModDetailsID = mod.Id;
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.controlAccessScreen.Hide();
		CustomMapsTerminal.instance.detailsAccessScreen.Hide();
		CustomMapsTerminal.instance.modDetailsScreen.Show();
		CustomMapsTerminal.instance.modDetailsScreen.SetModProfile(mod);
		CustomMapsTerminal.instance.modDisplayScreen.Show();
		CustomMapsTerminal.instance.modDisplayScreen.SetModProfile(mod);
		CustomMapsTerminal.instance.modSearchScreen.Hide();
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x060046C7 RID: 18119 RVA: 0x0017DEB4 File Offset: 0x0017C0B4
	public static void ReturnFromDetailsScreen()
	{
		CustomMapsTerminal.ScreenType screenType = CustomMapsTerminal.previousScreen;
		if (screenType == CustomMapsTerminal.ScreenType.ModDetails || screenType == CustomMapsTerminal.ScreenType.Invalid || screenType == CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		else
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.previousScreen;
		}
		switch (CustomMapsTerminal.localCurrentScreen)
		{
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Show();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.AvailableMods:
		case CustomMapsTerminal.ScreenType.InstalledMods:
		case CustomMapsTerminal.ScreenType.FavoriteMods:
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			CustomMapsTerminal.instance.modListScreen.Show();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Hide();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.SearchMods:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modSearchScreen.ReturnFromDetailsScreen();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Hide();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		}
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x060046C8 RID: 18120 RVA: 0x0017E030 File Offset: 0x0017C230
	public static void ShowSearchScreen()
	{
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.localCurrentScreen;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.SearchMods;
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.controlAccessScreen.Hide();
		CustomMapsTerminal.instance.detailsAccessScreen.SetDetailsScreenForDriver();
		CustomMapsTerminal.instance.detailsAccessScreen.Show();
		CustomMapsTerminal.instance.modDetailsScreen.Hide();
		CustomMapsTerminal.instance.modDisplayScreen.Hide();
		CustomMapsTerminal.instance.modSearchScreen.Show();
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x060046C9 RID: 18121 RVA: 0x0017E0BC File Offset: 0x0017C2BC
	public static void ReturnFromSearchScreen()
	{
		CustomMapsTerminal.ScreenType screenType = CustomMapsTerminal.previousScreen;
		if (screenType == CustomMapsTerminal.ScreenType.ModDetails || screenType == CustomMapsTerminal.ScreenType.Invalid || screenType == CustomMapsTerminal.ScreenType.TerminalControlPrompt || screenType == CustomMapsTerminal.ScreenType.SearchMods)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		else
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.previousScreen;
		}
		switch (CustomMapsTerminal.localCurrentScreen)
		{
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Show();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.AvailableMods:
		case CustomMapsTerminal.ScreenType.InstalledMods:
		case CustomMapsTerminal.ScreenType.FavoriteMods:
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			CustomMapsTerminal.instance.modListScreen.Show();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Hide();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		}
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x060046CA RID: 18122 RVA: 0x0017E1DA File Offset: 0x0017C3DA
	public static void SendTerminalStatus()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.instance.mapTerminalNetworkObject.SendTerminalStatus();
	}

	// Token: 0x060046CB RID: 18123 RVA: 0x0017E1F3 File Offset: 0x0017C3F3
	public static void ResetTerminalControl()
	{
		CustomMapsTerminal.localDriverID = -2;
		CustomMapsTerminal.instance.terminalControlButton.UnlockTerminalControl();
		CustomMapsTerminal.ShowTerminalControlScreen();
	}

	// Token: 0x060046CC RID: 18124 RVA: 0x0017E210 File Offset: 0x0017C410
	public static void HandleTerminalControlStatusChangeRequest(bool lockedStatus, int playerID)
	{
		if (lockedStatus && playerID == -2)
		{
			return;
		}
		if (CustomMapsTerminal.localDriverID == -2)
		{
			if (!lockedStatus)
			{
				return;
			}
		}
		else if (CustomMapsTerminal.localDriverID != playerID)
		{
			return;
		}
		CustomMapsTerminal.SetTerminalControlStatus(lockedStatus, playerID, true);
	}

	// Token: 0x060046CD RID: 18125 RVA: 0x0017E23C File Offset: 0x0017C43C
	public static void SetTerminalControlStatus(bool isLocked, int driverID = -2, bool sendRPC = false)
	{
		GTDev.Log<string>(string.Format("[CustomMapsTerminal::SetTerminalControlStatus] isLocked: {0} | driverID: {1} | playerId {2} | sendRPC: {3}", new object[]
		{
			isLocked,
			driverID,
			CustomMapsTerminal.LocalPlayerID,
			sendRPC
		}), null);
		if (isLocked)
		{
			CustomMapsTerminal.localDriverID = driverID;
			CustomMapsTerminal.instance.terminalControlButton.LockTerminalControl();
			if (CustomMapsTerminal.IsDriver)
			{
				CustomMapsTerminal.HideTerminalControlScreens();
			}
			else
			{
				CustomMapsTerminal.ShowTerminalControlScreen();
			}
		}
		else
		{
			CustomMapsTerminal.localDriverID = -2;
			CustomMapsTerminal.instance.terminalControlButton.UnlockTerminalControl();
			CustomMapsTerminal.ShowTerminalControlScreen();
		}
		if (sendRPC && NetworkSystem.Instance.IsMasterClient)
		{
			CustomMapsTerminal.instance.mapTerminalNetworkObject.SetTerminalControlStatus(isLocked, CustomMapsTerminal.localDriverID);
		}
	}

	// Token: 0x060046CE RID: 18126 RVA: 0x0017E2F4 File Offset: 0x0017C4F4
	public static void UpdateFromDriver(int currentScreen, long modDetailsID, int driverID)
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.localDriverID = driverID;
		CustomMapsTerminal.cachedModDetailsID = modDetailsID;
		CustomMapsTerminal.localModDetailsID = modDetailsID;
		CustomMapsTerminal.cachedCurrentScreen = (CustomMapsTerminal.ScreenType)currentScreen;
		CustomMapsTerminal.localCurrentScreen = (CustomMapsTerminal.ScreenType)currentScreen;
		Debug.Log(string.Format("[CustomMapsTerminal::UpdateFromDriver] currentScreen {0} modDetailsID {1}", CustomMapsTerminal.localCurrentScreen, CustomMapsTerminal.localModDetailsID));
		if (CustomMapsTerminal.localDriverID != -2)
		{
			CustomMapsTerminal.RefreshDriverNickName();
		}
		CustomMapsTerminal.ScreenType screenType = CustomMapsTerminal.localCurrentScreen;
		if (screenType <= CustomMapsTerminal.ScreenType.SearchMods)
		{
			CustomMapsTerminal.ShowTerminalControlScreen();
			return;
		}
		if (screenType != CustomMapsTerminal.ScreenType.ModDetails)
		{
			return;
		}
		CustomMapsTerminal.ShowTerminalControlScreen();
		if (CustomMapsTerminal.localModDetailsID <= 0L)
		{
			return;
		}
		CustomMapsTerminal.instance.detailsAccessScreen.Hide();
		CustomMapsTerminal.instance.modDisplayScreen.Show();
		CustomMapsTerminal.instance.modDisplayScreen.RetrieveModFromModIO(CustomMapsTerminal.localModDetailsID, false, null);
	}

	// Token: 0x060046CF RID: 18127 RVA: 0x0017E3B0 File Offset: 0x0017C5B0
	private void UpdateControlScreenForDriver()
	{
		GTDev.Log<string>(string.Format("[CustomMapsTerminal::UpdateScreenToMatchStatus] driverID: {0} ", CustomMapsTerminal.localDriverID) + string.Format("| currentScreen: {0} ", CustomMapsTerminal.localCurrentScreen) + string.Format("| previousScreen: {0} ", CustomMapsTerminal.previousScreen), null);
		switch (CustomMapsTerminal.localCurrentScreen)
		{
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			return;
		case CustomMapsTerminal.ScreenType.AvailableMods:
		case CustomMapsTerminal.ScreenType.InstalledMods:
		case CustomMapsTerminal.ScreenType.FavoriteMods:
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			this.controlAccessScreen.Hide();
			this.modSearchScreen.Hide();
			this.detailsAccessScreen.SetDetailsScreenForDriver();
			this.detailsAccessScreen.Show();
			this.modListScreen.Show();
			this.modDetailsScreen.Hide();
			this.modDisplayScreen.Hide();
			return;
		case CustomMapsTerminal.ScreenType.SearchMods:
			this.controlAccessScreen.Hide();
			this.modSearchScreen.Show();
			this.detailsAccessScreen.SetDetailsScreenForDriver();
			this.detailsAccessScreen.Show();
			this.modListScreen.Hide();
			this.modDetailsScreen.Hide();
			this.modDisplayScreen.Hide();
			return;
		case CustomMapsTerminal.ScreenType.ModDetails:
			this.controlAccessScreen.Hide();
			this.modSearchScreen.Hide();
			this.detailsAccessScreen.Hide();
			this.modListScreen.Hide();
			this.modDetailsScreen.Show();
			this.modDetailsScreen.RetrieveModFromModIO(CustomMapsTerminal.localModDetailsID, false, null);
			this.modDisplayScreen.Show();
			this.modDisplayScreen.RetrieveModFromModIO(CustomMapsTerminal.localModDetailsID, false, null);
			return;
		default:
			return;
		}
	}

	// Token: 0x060046D0 RID: 18128 RVA: 0x0017E530 File Offset: 0x0017C730
	private void ValidateLocalStatus()
	{
		if (CustomMapsTerminal.localDriverID == -2)
		{
			return;
		}
		if (CustomMapLoader.IsMapLoaded())
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localModDetailsID = CustomMapLoader.LoadedMapModId;
			CustomMapsTerminal.SendTerminalStatus();
			return;
		}
		if (CustomMapManager.IsLoading())
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localModDetailsID = CustomMapManager.LoadingMapId;
			CustomMapsTerminal.SendTerminalStatus();
			return;
		}
		if (CustomMapManager.GetRoomMapId() != ModId.Null)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localModDetailsID = CustomMapManager.GetRoomMapId()._id;
			CustomMapsTerminal.SendTerminalStatus();
		}
	}

	// Token: 0x060046D1 RID: 18129 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnModIOLoggedIn()
	{
	}

	// Token: 0x060046D2 RID: 18130 RVA: 0x0017E5B1 File Offset: 0x0017C7B1
	private void OnModIOLoggedOut()
	{
		if (CustomMapsTerminal.localCurrentScreen == CustomMapsTerminal.ScreenType.SubscribedMods)
		{
			if (this.modListScreen.isActiveAndEnabled)
			{
				this.modListScreen.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, false);
			}
			else
			{
				CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			}
		}
		if (CustomMapsTerminal.previousScreen == CustomMapsTerminal.ScreenType.SubscribedMods)
		{
			CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
	}

	// Token: 0x060046D3 RID: 18131 RVA: 0x0017E5EC File Offset: 0x0017C7EC
	public void HandleTerminalControlButtonPressed()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			CustomMapsTerminal.SetTerminalControlStatus(!this.terminalControlButton.IsLocked, CustomMapsTerminal.LocalPlayerID, false);
			return;
		}
		if (CustomMapsTerminal.localDriverID != -2 && !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (this.mapTerminalNetworkObject.HasAuthority)
		{
			CustomMapsTerminal.HandleTerminalControlStatusChangeRequest(!this.terminalControlButton.IsLocked, CustomMapsTerminal.LocalPlayerID);
			return;
		}
		this.mapTerminalNetworkObject.RequestTerminalControlStatusChange(!this.terminalControlButton.IsLocked);
	}

	// Token: 0x060046D4 RID: 18132 RVA: 0x0017E670 File Offset: 0x0017C870
	private static void ShowTerminalControlScreen()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (CustomMapsTerminal.localDriverID == -2)
		{
			CustomMapsTerminal.instance.controlAccessScreen.Reset();
			CustomMapsTerminal.instance.detailsAccessScreen.Reset();
		}
		else
		{
			CustomMapsTerminal.instance.controlAccessScreen.SetDriverName();
			CustomMapsTerminal.instance.detailsAccessScreen.SetDriverName();
		}
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.modDetailsScreen.Hide();
		CustomMapsTerminal.instance.modDisplayScreen.Hide();
		CustomMapsTerminal.instance.controlAccessScreen.Show();
		CustomMapsTerminal.instance.detailsAccessScreen.Show();
		CustomMapsTerminal.instance.modSearchScreen.Hide();
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.localCurrentScreen;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
	}

	// Token: 0x060046D5 RID: 18133 RVA: 0x0017E738 File Offset: 0x0017C938
	private static void HideTerminalControlScreens()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (CustomMapsTerminal.localCurrentScreen != CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			return;
		}
		if (CustomMapsTerminal.previousScreen > CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.previousScreen;
			if ((CustomMapsTerminal.localCurrentScreen == CustomMapsTerminal.ScreenType.SubscribedMods || CustomMapsTerminal.localCurrentScreen == CustomMapsTerminal.ScreenType.FavoriteMods) && !ModIOManager.IsLoggedIn())
			{
				CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			}
		}
		else if (CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.GetRoomMapId() != ModId.Null)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
		}
		else
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		CustomMapsTerminal.instance.UpdateControlScreenForDriver();
	}

	// Token: 0x060046D6 RID: 18134 RVA: 0x0017E7BD File Offset: 0x0017C9BD
	public static void RequestDriverNickNameRefresh()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
		CustomMapsTerminal.instance.mapTerminalNetworkObject.RefreshDriverNickName();
	}

	// Token: 0x060046D7 RID: 18135 RVA: 0x0017E7E4 File Offset: 0x0017C9E4
	public static void RefreshDriverNickName()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		CustomMapsTerminal.instance.terminalControllerLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(CustomMapsTerminal.localDriverID);
			CustomMapsTerminal.instance.terminalControllerText.text = netPlayerByID.DefaultName;
			if (GorillaComputer.instance.NametagsEnabled && flag)
			{
				RigContainer rigContainer;
				if (netPlayerByID.IsLocal)
				{
					CustomMapsTerminal.instance.terminalControllerText.text = netPlayerByID.NickName;
				}
				else if (VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					CustomMapsTerminal.instance.terminalControllerText.text = rigContainer.Rig.playerNameVisible;
				}
			}
		}
		else
		{
			CustomMapsTerminal.instance.terminalControllerText.text = ((GorillaComputer.instance.NametagsEnabled && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
		}
		CustomMapsTerminal.instance.terminalControllerText.gameObject.SetActive(true);
		CustomMapsTerminal.instance.modListScreen.RefreshDriverNickname(CustomMapsTerminal.instance.terminalControllerText.text);
	}

	// Token: 0x060046D8 RID: 18136 RVA: 0x0017E918 File Offset: 0x0017CB18
	private void OnReturnedToSinglePlayer()
	{
		if (CustomMapsTerminal.localDriverID != CustomMapsTerminal.cachedLocalPlayerID)
		{
			CustomMapsTerminal.ResetTerminalControl();
		}
		else
		{
			CustomMapsTerminal.localDriverID = CustomMapsTerminal.LocalPlayerID;
		}
		CustomMapsTerminal.cachedLocalPlayerID = -1;
	}

	// Token: 0x060046D9 RID: 18137 RVA: 0x0017E93D File Offset: 0x0017CB3D
	private void OnJoinedRoom()
	{
		CustomMapsTerminal.cachedLocalPlayerID = CustomMapsTerminal.LocalPlayerID;
		CustomMapsTerminal.ResetTerminalControl();
	}

	// Token: 0x060046DA RID: 18138 RVA: 0x0017E94E File Offset: 0x0017CB4E
	public static bool IsLocked()
	{
		return CustomMapsTerminal.localDriverID != -2;
	}

	// Token: 0x060046DB RID: 18139 RVA: 0x0017E95C File Offset: 0x0017CB5C
	public static int GetDriverID()
	{
		return CustomMapsTerminal.localDriverID;
	}

	// Token: 0x060046DC RID: 18140 RVA: 0x0017E963 File Offset: 0x0017CB63
	public static string GetDriverNickname()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return "";
		}
		return CustomMapsTerminal.instance.terminalControllerText.text;
	}

	// Token: 0x04005953 RID: 22867
	[SerializeField]
	private CustomMapsAccessScreen controlAccessScreen;

	// Token: 0x04005954 RID: 22868
	[SerializeField]
	private CustomMapsAccessScreen detailsAccessScreen;

	// Token: 0x04005955 RID: 22869
	[SerializeField]
	private CustomMapsListScreen modListScreen;

	// Token: 0x04005956 RID: 22870
	[SerializeField]
	private CustomMapsDetailsScreen modDetailsScreen;

	// Token: 0x04005957 RID: 22871
	[SerializeField]
	private CustomMapsDisplayScreen modDisplayScreen;

	// Token: 0x04005958 RID: 22872
	[SerializeField]
	private CustomMapsSearchScreen modSearchScreen;

	// Token: 0x04005959 RID: 22873
	[SerializeField]
	private VirtualStumpSerializer mapTerminalNetworkObject;

	// Token: 0x0400595A RID: 22874
	[SerializeField]
	private CustomMapsTerminalControlButton terminalControlButton;

	// Token: 0x0400595B RID: 22875
	[SerializeField]
	private TMP_Text terminalControllerLabelText;

	// Token: 0x0400595C RID: 22876
	[SerializeField]
	private TMP_Text terminalControllerText;

	// Token: 0x0400595D RID: 22877
	public const int NO_DRIVER_ID = -2;

	// Token: 0x0400595E RID: 22878
	private static CustomMapsTerminal instance;

	// Token: 0x0400595F RID: 22879
	private static bool hasInstance;

	// Token: 0x04005960 RID: 22880
	private static long localModDetailsID = -1L;

	// Token: 0x04005961 RID: 22881
	private static long cachedModDetailsID = -1L;

	// Token: 0x04005962 RID: 22882
	private static int localDriverID = -1;

	// Token: 0x04005963 RID: 22883
	private static int cachedLocalPlayerID = -1;

	// Token: 0x04005964 RID: 22884
	private static CustomMapsTerminal.ScreenType localCurrentScreen = CustomMapsTerminal.ScreenType.Invalid;

	// Token: 0x04005965 RID: 22885
	private static CustomMapsTerminal.ScreenType cachedCurrentScreen = CustomMapsTerminal.ScreenType.Invalid;

	// Token: 0x04005966 RID: 22886
	private static CustomMapsTerminal.ScreenType previousScreen = CustomMapsTerminal.ScreenType.Invalid;

	// Token: 0x02000AC9 RID: 2761
	public enum ScreenType
	{
		// Token: 0x04005968 RID: 22888
		Invalid = -1,
		// Token: 0x04005969 RID: 22889
		TerminalControlPrompt,
		// Token: 0x0400596A RID: 22890
		AvailableMods,
		// Token: 0x0400596B RID: 22891
		InstalledMods,
		// Token: 0x0400596C RID: 22892
		FavoriteMods,
		// Token: 0x0400596D RID: 22893
		SubscribedMods,
		// Token: 0x0400596E RID: 22894
		SearchMods,
		// Token: 0x0400596F RID: 22895
		ModDetails
	}
}
