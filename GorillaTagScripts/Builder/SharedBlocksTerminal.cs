using System;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001073 RID: 4211
	public class SharedBlocksTerminal : MonoBehaviour
	{
		// Token: 0x170009FC RID: 2556
		// (get) Token: 0x06006909 RID: 26889 RVA: 0x0021CBC0 File Offset: 0x0021ADC0
		public SharedBlocksManager.SharedBlocksMap SelectedMap
		{
			get
			{
				return this.selectedMap;
			}
		}

		// Token: 0x170009FD RID: 2557
		// (get) Token: 0x0600690A RID: 26890 RVA: 0x0021CBC8 File Offset: 0x0021ADC8
		public bool IsTerminalLocked
		{
			get
			{
				return this.isTerminalLocked;
			}
		}

		// Token: 0x170009FE RID: 2558
		// (get) Token: 0x0600690B RID: 26891 RVA: 0x0021CBD0 File Offset: 0x0021ADD0
		private int playersInLobby
		{
			get
			{
				return this.lobbyTrigger.playerIDsCurrentlyTouching.Count;
			}
		}

		// Token: 0x170009FF RID: 2559
		// (get) Token: 0x0600690C RID: 26892 RVA: 0x0021CBE2 File Offset: 0x0021ADE2
		public bool IsDriver
		{
			get
			{
				return this.localState.driverID == NetworkSystem.Instance.LocalPlayer.ActorNumber;
			}
		}

		// Token: 0x0600690D RID: 26893 RVA: 0x0021CC00 File Offset: 0x0021AE00
		public BuilderTable GetTable()
		{
			return this.linkedTable;
		}

		// Token: 0x17000A00 RID: 2560
		// (get) Token: 0x0600690E RID: 26894 RVA: 0x0021CC08 File Offset: 0x0021AE08
		public int GetDriverID
		{
			get
			{
				return this.localState.driverID;
			}
		}

		// Token: 0x0600690F RID: 26895 RVA: 0x0021CC18 File Offset: 0x0021AE18
		public static string MapIDToDisplayedString(string mapID)
		{
			if (mapID.IsNullOrEmpty())
			{
				return "____-____";
			}
			int num = 4;
			SharedBlocksTerminal.sb.Clear();
			if (mapID.Length > num)
			{
				SharedBlocksTerminal.sb.Append(mapID.Substring(0, num));
				SharedBlocksTerminal.sb.Append("-");
				SharedBlocksTerminal.sb.Append(mapID.Substring(num));
				int num2 = 9 - SharedBlocksTerminal.sb.Length;
				SharedBlocksTerminal.sb.Append('_', num2);
			}
			else
			{
				SharedBlocksTerminal.sb.Append(mapID.Substring(0));
				int num3 = num - SharedBlocksTerminal.sb.Length;
				SharedBlocksTerminal.sb.Append('_', num3);
				SharedBlocksTerminal.sb.Append("-____");
			}
			return SharedBlocksTerminal.sb.ToString();
		}

		// Token: 0x06006910 RID: 26896 RVA: 0x0021CCE4 File Offset: 0x0021AEE4
		public void Init(BuilderTable table)
		{
			if (this.hasInitialized)
			{
				return;
			}
			this.localState = new SharedBlocksTerminal.SharedBlocksTerminalState
			{
				state = SharedBlocksTerminal.TerminalState.NoStatus,
				driverID = -2
			};
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.AddListener(new UnityAction<SharedBlocksKeyboardBindings>(this.PressButton));
			this.terminalControlButton.onPressButton.AddListener(new UnityAction(this.OnTerminalControlPressed));
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			this.RefreshActiveScreen();
			this.linkedTable = table;
			table.linkedTerminal = this;
			this.linkedTable.OnMapLoaded.AddListener(new UnityAction<string>(this.OnSharedBlocksMapLoaded));
			this.linkedTable.OnMapLoadFailed.AddListener(new UnityAction<string>(this.OnSharedBlocksMapLoadFailed));
			this.linkedTable.OnMapCleared.AddListener(new UnityAction(this.OnSharedBlocksMapLoadStart));
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnReturnedToSinglePlayer;
			this.hasInitialized = true;
		}

		// Token: 0x06006911 RID: 26897 RVA: 0x0021CE00 File Offset: 0x0021B000
		private void Start()
		{
			BuilderTable builderTable;
			if (!this.hasInitialized && BuilderTable.TryGetBuilderTableForZone(this.tableZone, out builderTable))
			{
				this.Init(builderTable);
				return;
			}
			Debug.LogWarning("Could not find builder table for zone " + this.tableZone.ToString());
		}

		// Token: 0x06006912 RID: 26898 RVA: 0x0021CE4C File Offset: 0x0021B04C
		private void LateUpdate()
		{
			if (this.localState.driverID == -2)
			{
				return;
			}
			if (GorillaComputer.instance == null)
			{
				return;
			}
			if (this.useNametags == GorillaComputer.instance.NametagsEnabled)
			{
				return;
			}
			this.useNametags = GorillaComputer.instance.NametagsEnabled;
			this.RefreshDriverNickname();
		}

		// Token: 0x06006913 RID: 26899 RVA: 0x0021CEA8 File Offset: 0x0021B0A8
		private void OnDestroy()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.RemoveListener(new UnityAction<SharedBlocksKeyboardBindings>(this.PressButton));
			if (this.terminalControlButton != null)
			{
				this.terminalControlButton.onPressButton.RemoveListener(new UnityAction(this.OnTerminalControlPressed));
			}
			if (NetworkSystem.Instance != null)
			{
				NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
				NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnReturnedToSinglePlayer;
			}
			if (SharedBlocksManager.instance != null)
			{
				SharedBlocksManager.instance.OnGetPopularMapsComplete -= this.OnRandomMapsLoadedForRandomButton;
			}
			this.pendingRandomAfterMapsLoaded = false;
			this.randomMapsRequestInProgress = false;
			if (this.linkedTable != null)
			{
				this.linkedTable.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnSharedBlocksMapLoaded));
				this.linkedTable.OnMapLoadFailed.RemoveListener(new UnityAction<string>(this.OnSharedBlocksMapLoadFailed));
				this.linkedTable.OnMapCleared.RemoveListener(new UnityAction(this.OnSharedBlocksMapLoadStart));
			}
		}

		// Token: 0x06006914 RID: 26900 RVA: 0x0021CFD8 File Offset: 0x0021B1D8
		private void RefreshActiveScreen()
		{
			if (this.localState.driverID == -2)
			{
				if (this.currentScreen != this.noDriverScreen)
				{
					if (this.currentScreen != null)
					{
						this.currentScreen.Hide();
					}
					this.currentScreen = this.noDriverScreen;
					this.currentScreen.Show();
				}
				this.statusMessageText.gameObject.SetActive(false);
				return;
			}
			if (this.currentScreen != this.searchScreen)
			{
				if (this.currentScreen != null)
				{
					this.currentScreen.Hide();
				}
				this.currentScreen = this.searchScreen;
				this.currentScreen.Show();
			}
		}

		// Token: 0x06006915 RID: 26901 RVA: 0x0021D08C File Offset: 0x0021B28C
		private void SetTerminalState(SharedBlocksTerminal.TerminalState state)
		{
			this.localState.state = state;
			string text = "";
			if (this.localState.driverID == -2)
			{
				this.statusMessageText.gameObject.SetActive(false);
				return;
			}
			switch (state)
			{
			case SharedBlocksTerminal.TerminalState.NoStatus:
				this.statusMessageText.gameObject.SetActive(false);
				return;
			case SharedBlocksTerminal.TerminalState.Searching:
			{
				string text2 = "SEARCHING...";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_SEARCH", out text, text2))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_SEARCH]");
				}
				this.SetStatusText(text);
				return;
			}
			case SharedBlocksTerminal.TerminalState.NotFound:
			{
				string text2 = "MAP NOT FOUND";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND", out text, text2))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND]");
				}
				this.SetStatusText(text);
				return;
			}
			case SharedBlocksTerminal.TerminalState.Found:
			{
				string text2 = "MAP FOUND. PRESS 'ENTER' TO LOAD";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND", out text, text2))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND]");
				}
				this.SetStatusText(text);
				return;
			}
			case SharedBlocksTerminal.TerminalState.Loading:
			{
				string text2 = "LOADING...";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOADING", out text, text2))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_LOADING]");
				}
				this.SetStatusText(text);
				return;
			}
			case SharedBlocksTerminal.TerminalState.LoadSuccess:
			{
				string text2 = "LOAD SUCCESS";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS", out text, text2))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS]");
				}
				this.SetStatusText(text);
				return;
			}
			case SharedBlocksTerminal.TerminalState.LoadFail:
			{
				string text2 = "LOAD FAILED";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED", out text, text2))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED]");
				}
				this.SetStatusText(text);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06006916 RID: 26902 RVA: 0x0021D1F3 File Offset: 0x0021B3F3
		public void SelectMapIDAndOpenInfo(string mapID)
		{
			if (this.awaitingWebRequest)
			{
				return;
			}
			this.selectedMap = null;
			this.awaitingWebRequest = true;
			this.requestedMapID = mapID;
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.Searching);
			SharedBlocksManager.instance.RequestMapDataFromID(mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.OnPlayerMapRequestComplete));
		}

		// Token: 0x06006917 RID: 26903 RVA: 0x0021D234 File Offset: 0x0021B434
		private void OnPlayerMapRequestComplete(SharedBlocksManager.SharedBlocksMap response)
		{
			if (this.awaitingWebRequest)
			{
				this.awaitingWebRequest = false;
				this.requestedMapID = null;
				if (this.IsDriver)
				{
					if (response == null || response.MapID == null)
					{
						this.SetTerminalState(SharedBlocksTerminal.TerminalState.NotFound);
						return;
					}
					this.selectedMap = response;
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Found);
				}
			}
		}

		// Token: 0x06006918 RID: 26904 RVA: 0x0021D280 File Offset: 0x0021B480
		private bool CanChangeMapState(bool load, out string disallowedReason)
		{
			disallowedReason = "";
			if (!NetworkSystem.Instance.InRoom)
			{
				disallowedReason = "MUST BE IN A ROOM BEFORE  " + (load ? "" : "UN") + "LOADING A MAP.";
				string text = (load ? "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_LOAD" : "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_UNLOAD");
				string text2;
				if (!LocalisationManager.TryGetKeyForCurrentLocale(text, out text2, disallowedReason))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [" + text + "]");
				}
				disallowedReason = text2;
				return false;
			}
			this.RefreshLobbyCount();
			if (!this.AreAllPlayersInLobby())
			{
				disallowedReason = "ALL PLAYERS IN THE ROOM MUST BE INSIDE THE LOBBY BEFORE " + (load ? "" : "UN") + "LOADING A MAP.";
				string text3 = (load ? "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_LOAD" : "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_UNLOAD");
				string text4;
				if (!LocalisationManager.TryGetKeyForCurrentLocale(text3, out text4, disallowedReason))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [" + text3 + "]");
				}
				disallowedReason = text4;
				return false;
			}
			return true;
		}

		// Token: 0x06006919 RID: 26905 RVA: 0x0021D35B File Offset: 0x0021B55B
		public void SetStatusText(string text)
		{
			this.statusMessageText.text = text;
			this.statusMessageText.gameObject.SetActive(true);
		}

		// Token: 0x0600691A RID: 26906 RVA: 0x0021D37A File Offset: 0x0021B57A
		private bool IsLocalPlayerInLobby()
		{
			return base.isActiveAndEnabled && this.lobbyTrigger.playerIDsCurrentlyTouching.Contains(VRRig.LocalRig.creator.UserId);
		}

		// Token: 0x0600691B RID: 26907 RVA: 0x0021D3AA File Offset: 0x0021B5AA
		public bool AreAllPlayersInLobby()
		{
			return base.isActiveAndEnabled && this.playersInLobby == this.playersInRoom;
		}

		// Token: 0x0600691C RID: 26908 RVA: 0x0021D3C4 File Offset: 0x0021B5C4
		public string GetLobbyText()
		{
			string text = "PLAYERS IN ROOM {0}\nPLAYERS IN LOBBY {1}";
			string text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT", out text2, text))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT]");
			}
			return string.Format(text2, this.playersInRoom, this.playersInLobby);
		}

		// Token: 0x0600691D RID: 26909 RVA: 0x0021D40C File Offset: 0x0021B60C
		public void RefreshLobbyCount()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom)
			{
				this.playersInRoom = NetworkSystem.Instance.RoomPlayerCount;
				return;
			}
			this.playersInRoom = 0;
		}

		// Token: 0x0600691E RID: 26910 RVA: 0x0021D440 File Offset: 0x0021B640
		public void PressButton(SharedBlocksKeyboardBindings buttonPressed)
		{
			if (this.localState.driverID == -2)
			{
				return;
			}
			if (!this.IsDriver)
			{
				string text;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER", out text, "NOT TERMINAL CONTROLLER"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER]");
				}
				this.SetStatusText(text);
				return;
			}
			if (this.localState.state == SharedBlocksTerminal.TerminalState.Searching || this.localState.state == SharedBlocksTerminal.TerminalState.Loading)
			{
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.up)
			{
				this.OnUpButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.down)
			{
				this.OnDownButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.delete)
			{
				this.OnDeleteButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.enter)
			{
				this.OnSelectButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.random)
			{
				this.OnRandomizeButtonPressed();
				return;
			}
			if (buttonPressed >= SharedBlocksKeyboardBindings.zero && buttonPressed <= SharedBlocksKeyboardBindings.nine)
			{
				this.OnNumberPressed((int)buttonPressed);
				return;
			}
			if (buttonPressed >= SharedBlocksKeyboardBindings.A && buttonPressed <= SharedBlocksKeyboardBindings.Z)
			{
				this.OnLetterPressed(buttonPressed.ToString());
			}
		}

		// Token: 0x0600691F RID: 26911 RVA: 0x0021D511 File Offset: 0x0021B711
		private void OnUpButtonPressed()
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnUpPressed();
			}
		}

		// Token: 0x06006920 RID: 26912 RVA: 0x0021D52C File Offset: 0x0021B72C
		private void OnDownButtonPressed()
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnDownPressed();
			}
		}

		// Token: 0x06006921 RID: 26913 RVA: 0x0021D547 File Offset: 0x0021B747
		private void OnSelectButtonPressed()
		{
			if (this.localState.state == SharedBlocksTerminal.TerminalState.Found)
			{
				this.OnLoadMapPressed(false);
				return;
			}
			if (this.currentScreen != null)
			{
				this.currentScreen.OnSelectPressed();
			}
		}

		// Token: 0x06006922 RID: 26914 RVA: 0x0021D578 File Offset: 0x0021B778
		private void OnRandomizeButtonPressed()
		{
			if (!this.IsDriver)
			{
				return;
			}
			if (this.awaitingWebRequest || this.isLoadingMap || this.localState.state == SharedBlocksTerminal.TerminalState.Searching || this.localState.state == SharedBlocksTerminal.TerminalState.Loading)
			{
				return;
			}
			if (Time.time < this.lastRandomLoadTime + this.loadRandomMapCooldown)
			{
				this.SetStatusText("PLEASE WAIT BEFORE LOADING ANOTHER RANDOM MAP.");
				return;
			}
			SharedBlocksManager.SharedBlocksMap sharedBlocksMap;
			if (!SharedBlocksManager.instance.TryGetRandomPopularMap(out sharedBlocksMap))
			{
				this.LoadPopularMapsThenRandomize();
				return;
			}
			this.LoadRandomMap(sharedBlocksMap);
		}

		// Token: 0x06006923 RID: 26915 RVA: 0x0021D5F7 File Offset: 0x0021B7F7
		private void OnDeleteButtonPressed()
		{
			if (this.localState.state != SharedBlocksTerminal.TerminalState.Loading && this.localState.state != SharedBlocksTerminal.TerminalState.Searching)
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			}
			if (this.currentScreen != null)
			{
				this.currentScreen.OnDeletePressed();
			}
		}

		// Token: 0x06006924 RID: 26916 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void OnBackButtonPressed()
		{
		}

		// Token: 0x06006925 RID: 26917 RVA: 0x0021D635 File Offset: 0x0021B835
		private void OnNumberPressed(int number)
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnNumberPressed(number);
			}
		}

		// Token: 0x06006926 RID: 26918 RVA: 0x0021D651 File Offset: 0x0021B851
		private void OnLetterPressed(string letter)
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnLetterPressed(letter);
			}
		}

		// Token: 0x06006927 RID: 26919 RVA: 0x0021D670 File Offset: 0x0021B870
		private void OnTerminalControlPressed()
		{
			if (this.isTerminalLocked)
			{
				if (this.IsDriver)
				{
					if (NetworkSystem.Instance.InRoom)
					{
						this.linkedTable.builderNetworking.RequestBlocksTerminalControl(false);
						return;
					}
					this.SetTerminalDriver(-2);
					return;
				}
			}
			else
			{
				if (NetworkSystem.Instance.InRoom)
				{
					this.linkedTable.builderNetworking.RequestBlocksTerminalControl(true);
					return;
				}
				this.SetTerminalDriver(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			}
		}

		// Token: 0x06006928 RID: 26920 RVA: 0x0021D6E8 File Offset: 0x0021B8E8
		public void OnLoadMapPressed(bool isRandom = false)
		{
			if (!this.IsDriver)
			{
				string text;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER", out text, "NOT TERMINAL CONTROLLER"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER]");
				}
				this.SetStatusText(text);
				return;
			}
			if (this.currentScreen == null || this.selectedMap == null)
			{
				string text2;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION", out text2, "NO MAP SELECTED"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION]");
				}
				this.SetStatusText(text2);
				return;
			}
			if (this.awaitingWebRequest || this.isLoadingMap)
			{
				string text3;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS", out text3, "BLOCKS LOAD ALREADY IN PROGRESS"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS]");
				}
				this.SetStatusText("BLOCKS LOAD ALREADY IN PROGRESS");
				return;
			}
			string text4;
			if (!this.CanChangeMapState(true, out text4))
			{
				this.SetStatusText(text4);
				return;
			}
			if (this.linkedTable != null)
			{
				if (Time.time > this.lastLoadTime + this.loadMapCooldown || isRandom)
				{
					string text5;
					if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOADING", out text5, "LOADING BLOCKS ..."))
					{
						Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_LOADING]");
					}
					if (isRandom)
					{
						GorillaTelemetry.EnqueueTelemetryEvent("sharedblocks_random_button_pressed", new Dictionary<string, object>(), null);
					}
					GorillaTelemetry.EnqueueTelemetryEvent("sharedblocks_map_loaded", new Dictionary<string, object>
					{
						{ "isRandom", isRandom },
						{
							"mapID",
							this.selectedMap.MapID
						}
					}, null);
					this.SetStatusText((!isRandom) ? text5 : "LOADING RANDOM MAP ...");
					this.isLoadingMap = true;
					this.lastLoadTime = Time.time;
					this.linkedTable.LoadSharedMap(this.selectedMap);
					return;
				}
				int num = Mathf.RoundToInt(this.lastLoadTime + this.loadMapCooldown - Time.time);
				string text6 = string.Format("PLEASE WAIT {0} SECONDS BEFORE LOADING ANOTHER MAP", num);
				string text7;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_WAIT", out text7, text6))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_LOADING]");
				}
				text7 = text7.Replace("{time}", num.ToString());
				this.SetStatusText(text7);
			}
		}

		// Token: 0x06006929 RID: 26921 RVA: 0x0021D8D8 File Offset: 0x0021BAD8
		private void LoadPopularMapsThenRandomize()
		{
			SharedBlocksManager instance = SharedBlocksManager.instance;
			if (instance == null)
			{
				this.SetStatusText("RANDOM MAPS NOT READY.");
				return;
			}
			this.pendingRandomAfterMapsLoaded = true;
			if (!this.randomMapsRequestInProgress)
			{
				this.randomMapsRequestInProgress = true;
				instance.OnGetPopularMapsComplete -= this.OnRandomMapsLoadedForRandomButton;
				instance.OnGetPopularMapsComplete += this.OnRandomMapsLoadedForRandomButton;
			}
			this.SetStatusText("LOADING RANDOM MAP LIST.");
			instance.RefreshPopularMapsForRandom();
		}

		// Token: 0x0600692A RID: 26922 RVA: 0x0021D94C File Offset: 0x0021BB4C
		private void OnRandomMapsLoadedForRandomButton(bool success)
		{
			SharedBlocksManager instance = SharedBlocksManager.instance;
			if (instance != null)
			{
				instance.OnGetPopularMapsComplete -= this.OnRandomMapsLoadedForRandomButton;
			}
			this.randomMapsRequestInProgress = false;
			if (!this.pendingRandomAfterMapsLoaded)
			{
				return;
			}
			this.pendingRandomAfterMapsLoaded = false;
			if (!this.IsDriver)
			{
				return;
			}
			if (this.awaitingWebRequest || this.isLoadingMap || this.localState.state == SharedBlocksTerminal.TerminalState.Searching || this.localState.state == SharedBlocksTerminal.TerminalState.Loading)
			{
				return;
			}
			SharedBlocksManager.SharedBlocksMap sharedBlocksMap;
			if (!success || instance == null || !instance.TryGetRandomPopularMap(out sharedBlocksMap))
			{
				this.SetStatusText("NO RANDOM MAPS AVAILABLE.");
				return;
			}
			this.LoadRandomMap(sharedBlocksMap);
		}

		// Token: 0x0600692B RID: 26923 RVA: 0x0021D9F0 File Offset: 0x0021BBF0
		private void LoadRandomMap(SharedBlocksManager.SharedBlocksMap randomMap)
		{
			if (randomMap == null || !SharedBlocksManager.IsMapIDValid(randomMap.MapID))
			{
				this.SetStatusText("NO VALID RANDOM MAP AVAILABLE.");
				return;
			}
			this.lastRandomLoadTime = Time.time;
			this.selectedMap = randomMap;
			Debug.Log("Selected random map: " + randomMap.MapID);
			if (this.searchScreen != null)
			{
				this.searchScreen.SetMapCode(this.selectedMap.MapID);
			}
			this.OnLoadMapPressed(true);
		}

		// Token: 0x0600692C RID: 26924 RVA: 0x0021DA6B File Offset: 0x0021BC6B
		public bool IsPlayerDriver(Player player)
		{
			return player.ActorNumber == this.localState.driverID;
		}

		// Token: 0x0600692D RID: 26925 RVA: 0x0021DA80 File Offset: 0x0021BC80
		public bool ValidateTerminalControlRequest(bool locked, int playerNumber)
		{
			if (locked && playerNumber == -2)
			{
				return false;
			}
			if (this.localState.driverID == -2)
			{
				return locked;
			}
			return this.localState.driverID == playerNumber;
		}

		// Token: 0x0600692E RID: 26926 RVA: 0x0021DAAB File Offset: 0x0021BCAB
		private void OnDriverNameChanged()
		{
			this.RefreshDriverNickname();
		}

		// Token: 0x0600692F RID: 26927 RVA: 0x0021DAB4 File Offset: 0x0021BCB4
		public void SetTerminalDriver(int playerNum)
		{
			if (playerNum != -2)
			{
				if (this.localState.driverID != -2 && this.localState.driverID != playerNum)
				{
					GTDev.LogWarning<string>(string.Format("Shared BlocksTerminal SetTerminalDriver cannot set {0} as driver while {1} is driver", playerNum, this.localState.driverID), null);
					return;
				}
				this.localState.driverID = playerNum;
				NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerNum);
				RigContainer rigContainer;
				if (netPlayerByID != null && VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					this.driverRig = rigContainer.Rig;
					this.driverRig.OnPlayerNameVisibleChanged += this.OnDriverNameChanged;
				}
				this.isTerminalLocked = true;
				this.UpdateTerminalButton();
				this.RefreshActiveScreen();
				this.searchScreen.SetInputTextEnabled(this.IsDriver);
				if (this.IsDriver && this.awaitingWebRequest)
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Searching);
					this.searchScreen.SetMapCode(this.requestedMapID);
				}
				else if (this.isLoadingMap)
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Loading);
					this.searchScreen.SetMapCode(this.linkedTable.GetPendingMap());
				}
				else
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
				}
			}
			else
			{
				if (this.driverRig != null)
				{
					this.driverRig.OnPlayerNameVisibleChanged -= this.OnDriverNameChanged;
					this.driverRig = null;
				}
				this.localState.driverID = -2;
				this.isTerminalLocked = false;
				this.UpdateTerminalButton();
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
				this.RefreshActiveScreen();
			}
			this.RefreshDriverNickname();
		}

		// Token: 0x06006930 RID: 26928 RVA: 0x0021DC34 File Offset: 0x0021BE34
		private void RefreshDriverNickname()
		{
			StringVariable stringVariable = this._currentDriverLoc.StringReference["playerName"] as StringVariable;
			if (this.localState.driverID == -2)
			{
				this.currentDriverLabel.gameObject.SetActive(false);
				stringVariable.Value = "";
				this.currentDriverText.text = "";
				this.currentDriverText.gameObject.SetActive(false);
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
			if (NetworkSystem.Instance.InRoom)
			{
				NetPlayer player = NetworkSystem.Instance.GetPlayer(this.localState.driverID);
				if (player != null && this.useNametags && flag)
				{
					RigContainer rigContainer;
					if (player.IsLocal)
					{
						stringVariable.Value = player.NickName;
						this.currentDriverText.text = player.NickName;
					}
					else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
					{
						stringVariable.Value = rigContainer.Rig.playerNameVisible;
						this.currentDriverText.text = rigContainer.Rig.playerNameVisible;
					}
					else
					{
						stringVariable.Value = player.DefaultName;
						this.currentDriverText.text = player.DefaultName;
					}
				}
				else
				{
					stringVariable.Value = "";
					this.currentDriverText.text = "";
				}
			}
			else
			{
				stringVariable.Value = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
				this.currentDriverText.text = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
			}
			this.currentDriverLabel.gameObject.SetActive(true);
		}

		// Token: 0x06006931 RID: 26929 RVA: 0x0021DE02 File Offset: 0x0021C002
		public bool ValidateLoadMapRequest(string mapID, int playerNum)
		{
			return playerNum == this.localState.driverID && this.AreAllPlayersInLobby() && SharedBlocksManager.IsMapIDValid(mapID);
		}

		// Token: 0x06006932 RID: 26930 RVA: 0x0021DE24 File Offset: 0x0021C024
		private void OnJoinedRoom()
		{
			this.cachedLocalPlayerID = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			this.ResetTerminalControl();
		}

		// Token: 0x06006933 RID: 26931 RVA: 0x0021DE41 File Offset: 0x0021C041
		private void OnReturnedToSinglePlayer()
		{
			if (this.localState.driverID != this.cachedLocalPlayerID)
			{
				this.ResetTerminalControl();
			}
			else
			{
				this.localState.driverID = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			}
			this.cachedLocalPlayerID = -1;
		}

		// Token: 0x06006934 RID: 26932 RVA: 0x0021DE80 File Offset: 0x0021C080
		public void ResetTerminalControl()
		{
			this.localState.driverID = -2;
			this.isTerminalLocked = false;
			this.selectedMap = null;
			if (SharedBlocksManager.instance != null)
			{
				SharedBlocksManager.instance.OnGetPopularMapsComplete -= this.OnRandomMapsLoadedForRandomButton;
			}
			this.pendingRandomAfterMapsLoaded = false;
			this.randomMapsRequestInProgress = false;
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			this.RefreshActiveScreen();
			this.UpdateTerminalButton();
		}

		// Token: 0x06006935 RID: 26933 RVA: 0x0021DEEC File Offset: 0x0021C0EC
		private void UpdateTerminalButton()
		{
			this.terminalControlButton.isOn = this.isTerminalLocked;
			this.terminalControlButton.UpdateColor();
		}

		// Token: 0x06006936 RID: 26934 RVA: 0x0021DF0C File Offset: 0x0021C10C
		private void OnSharedBlocksMapLoaded(string mapID)
		{
			if (!this.IsDriver)
			{
				this.searchScreen.SetMapCode(mapID);
			}
			if (SharedBlocksManager.IsMapIDValid(mapID))
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadSuccess);
			}
			else if (this.localState.state != SharedBlocksTerminal.TerminalState.LoadFail)
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadFail);
			}
			this.isLoadingMap = false;
		}

		// Token: 0x06006937 RID: 26935 RVA: 0x0021DF5A File Offset: 0x0021C15A
		private void OnSharedBlocksMapLoadFailed(string message)
		{
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadFail);
			this.SetStatusText(message);
			this.isLoadingMap = false;
		}

		// Token: 0x06006938 RID: 26936 RVA: 0x0021DF74 File Offset: 0x0021C174
		private void OnSharedBlocksMapLoadStart()
		{
			if (this.linkedTable == null)
			{
				return;
			}
			if (!this.IsDriver)
			{
				this.searchScreen.SetMapCode(this.linkedTable.GetPendingMap());
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.Loading);
				this.isLoadingMap = true;
				this.lastLoadTime = Time.time;
			}
		}

		// Token: 0x04007870 RID: 30832
		public const string SHARE_BLOCKS_TERMINAL_PROMPT_KEY = "SHARE_BLOCKS_TERMINAL_PROMPT";

		// Token: 0x04007871 RID: 30833
		public const string SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_KEY = "SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON";

		// Token: 0x04007872 RID: 30834
		public const string SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_AVAILABLE_KEY = "SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_AVAILABLE";

		// Token: 0x04007873 RID: 30835
		public const string SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_LOCKED_KEY = "SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_LOCKED";

		// Token: 0x04007874 RID: 30836
		public const string SHARE_BLOCKS_TERMINAL_STATUS_SEARCH_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_SEARCH";

		// Token: 0x04007875 RID: 30837
		public const string SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND";

		// Token: 0x04007876 RID: 30838
		public const string SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND";

		// Token: 0x04007877 RID: 30839
		public const string SHARE_BLOCKS_TERMINAL_STATUS_LOADING_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_LOADING";

		// Token: 0x04007878 RID: 30840
		public const string SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS";

		// Token: 0x04007879 RID: 30841
		public const string SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED";

		// Token: 0x0400787A RID: 30842
		public const string SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER";

		// Token: 0x0400787B RID: 30843
		public const string SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION";

		// Token: 0x0400787C RID: 30844
		public const string SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS";

		// Token: 0x0400787D RID: 30845
		public const string SHARE_BLOCKS_TERMINAL_STATUS_WAIT_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_WAIT";

		// Token: 0x0400787E RID: 30846
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_LOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_LOAD";

		// Token: 0x0400787F RID: 30847
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_UNLOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_UNLOAD";

		// Token: 0x04007880 RID: 30848
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_LOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_LOAD";

		// Token: 0x04007881 RID: 30849
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_UNLOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_UNLOAD";

		// Token: 0x04007882 RID: 30850
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL";

		// Token: 0x04007883 RID: 30851
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE";

		// Token: 0x04007884 RID: 30852
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH";

		// Token: 0x04007885 RID: 30853
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_VOTES_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_VOTES";

		// Token: 0x04007886 RID: 30854
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL";

		// Token: 0x04007887 RID: 30855
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT";

		// Token: 0x04007888 RID: 30856
		public const string SHARE_BLOCKS_TERMINAL_ERROR_TITLE_KEY = "SHARE_BLOCKS_TERMINAL_ERROR_TITLE";

		// Token: 0x04007889 RID: 30857
		public const string SHARE_BLOCKS_TERMINAL_ERROR_INSTRUCTIONS_KEY = "SHARE_BLOCKS_TERMINAL_ERROR_INSTRUCTIONS";

		// Token: 0x0400788A RID: 30858
		public const string SHARE_BLOCKS_TERMINAL_ERROR_BACK_KEY = "SHARE_BLOCKS_TERMINAL_ERROR_BACK";

		// Token: 0x0400788B RID: 30859
		public const string SHARE_BLOCKS_TERMINAL_INFO_TITLE_KEY = "SHARE_BLOCKS_TERMINAL_INFO_TITLE";

		// Token: 0x0400788C RID: 30860
		public const string SHARE_BLOCKS_TERMINAL_INFO_DATA_KEY = "SHARE_BLOCKS_TERMINAL_INFO_DATA";

		// Token: 0x0400788D RID: 30861
		public const string SHARE_BLOCKS_TERMINAL_INFO_ENTER_KEY = "SHARE_BLOCKS_TERMINAL_INFO_ENTER";

		// Token: 0x0400788E RID: 30862
		public const string SHARE_BLOCKS_TERMINAL_OTHER_DRIVER_KEY = "SHARE_BLOCKS_TERMINAL_OTHER_DRIVER";

		// Token: 0x0400788F RID: 30863
		public const string SHARE_BLOCKS_TERMINAL_CONTROLLER_LABEL_KEY = "SHARE_BLOCKS_TERMINAL_CONTROLLER_LABEL";

		// Token: 0x04007890 RID: 30864
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT";

		// Token: 0x04007891 RID: 30865
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH";

		// Token: 0x04007892 RID: 30866
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID";

		// Token: 0x04007893 RID: 30867
		[SerializeField]
		private GTZone tableZone = GTZone.monkeBlocksShared;

		// Token: 0x04007894 RID: 30868
		[SerializeField]
		private TMP_Text currentMapSelectionText;

		// Token: 0x04007895 RID: 30869
		[SerializeField]
		private TMP_Text statusMessageText;

		// Token: 0x04007896 RID: 30870
		[SerializeField]
		private TMP_Text currentDriverText;

		// Token: 0x04007897 RID: 30871
		[SerializeField]
		private TMP_Text currentDriverLabel;

		// Token: 0x04007898 RID: 30872
		[SerializeField]
		private LocalizedText _currentDriverLoc;

		// Token: 0x04007899 RID: 30873
		[SerializeField]
		private SharedBlocksScreen noDriverScreen;

		// Token: 0x0400789A RID: 30874
		[SerializeField]
		private SharedBlocksScreenSearch searchScreen;

		// Token: 0x0400789B RID: 30875
		[SerializeField]
		private GorillaPressableButton terminalControlButton;

		// Token: 0x0400789C RID: 30876
		[SerializeField]
		private float loadMapCooldown = 30f;

		// Token: 0x0400789D RID: 30877
		[SerializeField]
		private float loadRandomMapCooldown = 3f;

		// Token: 0x0400789E RID: 30878
		[SerializeField]
		private GorillaFriendCollider lobbyTrigger;

		// Token: 0x0400789F RID: 30879
		private SharedBlocksManager.SharedBlocksMap selectedMap;

		// Token: 0x040078A0 RID: 30880
		private SharedBlocksScreen currentScreen;

		// Token: 0x040078A1 RID: 30881
		private BuilderTable linkedTable;

		// Token: 0x040078A2 RID: 30882
		public const int NO_DRIVER_ID = -2;

		// Token: 0x040078A3 RID: 30883
		private bool awaitingWebRequest;

		// Token: 0x040078A4 RID: 30884
		private string requestedMapID;

		// Token: 0x040078A5 RID: 30885
		public const string POINTER = "> ";

		// Token: 0x040078A6 RID: 30886
		public Action<bool> OnMapLoadComplete;

		// Token: 0x040078A7 RID: 30887
		private bool isTerminalLocked;

		// Token: 0x040078A8 RID: 30888
		private SharedBlocksTerminal.SharedBlocksTerminalState localState;

		// Token: 0x040078A9 RID: 30889
		private int cachedLocalPlayerID = -1;

		// Token: 0x040078AA RID: 30890
		private bool isLoadingMap;

		// Token: 0x040078AB RID: 30891
		private bool pendingRandomAfterMapsLoaded;

		// Token: 0x040078AC RID: 30892
		private bool randomMapsRequestInProgress;

		// Token: 0x040078AD RID: 30893
		private float lastLoadTime;

		// Token: 0x040078AE RID: 30894
		private float lastRandomLoadTime;

		// Token: 0x040078AF RID: 30895
		private bool useNametags;

		// Token: 0x040078B0 RID: 30896
		private bool hasInitialized;

		// Token: 0x040078B1 RID: 30897
		private static StringBuilder sb = new StringBuilder();

		// Token: 0x040078B2 RID: 30898
		private VRRig driverRig;

		// Token: 0x040078B3 RID: 30899
		private static List<VRRig> tempRigs = new List<VRRig>(16);

		// Token: 0x040078B4 RID: 30900
		private int playersInRoom;

		// Token: 0x02001074 RID: 4212
		public enum ScreenType
		{
			// Token: 0x040078B6 RID: 30902
			NO_DRIVER,
			// Token: 0x040078B7 RID: 30903
			SEARCH,
			// Token: 0x040078B8 RID: 30904
			LOADING,
			// Token: 0x040078B9 RID: 30905
			ERROR,
			// Token: 0x040078BA RID: 30906
			SCAN_INFO,
			// Token: 0x040078BB RID: 30907
			OTHER_DRIVER
		}

		// Token: 0x02001075 RID: 4213
		public enum TerminalState
		{
			// Token: 0x040078BD RID: 30909
			NoStatus,
			// Token: 0x040078BE RID: 30910
			Searching,
			// Token: 0x040078BF RID: 30911
			NotFound,
			// Token: 0x040078C0 RID: 30912
			Found,
			// Token: 0x040078C1 RID: 30913
			Loading,
			// Token: 0x040078C2 RID: 30914
			LoadSuccess,
			// Token: 0x040078C3 RID: 30915
			LoadFail
		}

		// Token: 0x02001076 RID: 4214
		public class SharedBlocksTerminalState
		{
			// Token: 0x040078C4 RID: 30916
			public SharedBlocksTerminal.ScreenType currentScreen;

			// Token: 0x040078C5 RID: 30917
			public SharedBlocksTerminal.TerminalState state;

			// Token: 0x040078C6 RID: 30918
			public int driverID;
		}
	}
}
