using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GorillaGameModes;
using GorillaTagScripts;
using GorillaTagScripts.VirtualStumpCustomMaps;
using KID.Model;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaNetworking
{
	// Token: 0x020010DB RID: 4315
	public class GorillaComputer : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x17000A48 RID: 2632
		// (get) Token: 0x06006BC7 RID: 27591 RVA: 0x0022BC48 File Offset: 0x00229E48
		public string versionMismatch
		{
			get
			{
				if (this._lastLocaleChecked_Version != null && this._lastLocaleChecked_Version == LocalisationManager.CurrentLanguage && !string.IsNullOrEmpty(this._cachedVersionMismatch))
				{
					return this._cachedVersionMismatch;
				}
				string text = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("VERSION_MISMATCH", out text2, text);
				this._lastLocaleChecked_Version = LocalisationManager.CurrentLanguage;
				this._cachedVersionMismatch = text2;
				return this._cachedVersionMismatch;
			}
		}

		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x06006BC8 RID: 27592 RVA: 0x0022BCB8 File Offset: 0x00229EB8
		public string unableToConnect
		{
			get
			{
				if (this._lastLocaleChecked_Connect != null && this._lastLocaleChecked_Connect == LocalisationManager.CurrentLanguage && !string.IsNullOrEmpty(this._cachedUnableToConnect))
				{
					return this._cachedUnableToConnect;
				}
				string text = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("CONNECTION_ISSUE", out text2, text);
				this._lastLocaleChecked_Connect = LocalisationManager.CurrentLanguage;
				this._cachedUnableToConnect = text2;
				return this._cachedUnableToConnect;
			}
		}

		// Token: 0x06006BC9 RID: 27593 RVA: 0x0022BD25 File Offset: 0x00229F25
		public DateTime GetServerTime()
		{
			return this.startupTime + TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
		}

		// Token: 0x06006BCA RID: 27594 RVA: 0x0022BD3D File Offset: 0x00229F3D
		public void AddSeverTime(int m)
		{
			this.startupTime = this.startupTime.AddMinutes((double)m);
		}

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x06006BCB RID: 27595 RVA: 0x0022BD52 File Offset: 0x00229F52
		// (set) Token: 0x06006BCC RID: 27596 RVA: 0x0022BD5A File Offset: 0x00229F5A
		public string[] allowedMapsToJoin
		{
			get
			{
				return this._allowedMapsToJoin;
			}
			set
			{
				this._allowedMapsToJoin = value;
			}
		}

		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x06006BCD RID: 27597 RVA: 0x0022BD63 File Offset: 0x00229F63
		// (set) Token: 0x06006BCE RID: 27598 RVA: 0x0022BD6B File Offset: 0x00229F6B
		public string version { get; private set; }

		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x06006BCF RID: 27599 RVA: 0x0022BD74 File Offset: 0x00229F74
		public string VStumpRoomPrepend
		{
			get
			{
				return this.virtualStumpRoomPrepend;
			}
		}

		// Token: 0x06006BD0 RID: 27600 RVA: 0x0022BD7C File Offset: 0x00229F7C
		private static bool IsValidVStumpModePrefix(char c)
		{
			return c == 'A' || c == 'B' || c == 'C';
		}

		// Token: 0x06006BD1 RID: 27601 RVA: 0x0022BD90 File Offset: 0x00229F90
		public bool IsVStumpRoomName(string roomName)
		{
			return !string.IsNullOrEmpty(roomName) && !string.IsNullOrEmpty(this.virtualStumpRoomPrepend) && (roomName.Length > 1 && GorillaComputer.IsValidVStumpModePrefix(roomName[0])) && roomName[1] == this.virtualStumpRoomPrepend[0];
		}

		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06006BD2 RID: 27602 RVA: 0x0022BDE2 File Offset: 0x00229FE2
		public string VStumpRoomFullPrepend
		{
			get
			{
				return this.virtualStumpRoomModePrefix + this.virtualStumpRoomPrepend;
			}
		}

		// Token: 0x06006BD3 RID: 27603 RVA: 0x0022BDF5 File Offset: 0x00229FF5
		public void SetVStumpRoomModePrefix(string prefix)
		{
			this.virtualStumpRoomModePrefix = ((prefix != null && prefix.Length == 1 && GorillaComputer.IsValidVStumpModePrefix(prefix[0])) ? prefix : "C");
		}

		// Token: 0x06006BD4 RID: 27604 RVA: 0x0022BE20 File Offset: 0x0022A020
		public string StripVStumpRoomPrefix(string roomName)
		{
			if (string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(this.virtualStumpRoomPrepend))
			{
				return roomName;
			}
			int num = roomName.IndexOf(this.virtualStumpRoomPrepend, StringComparison.Ordinal);
			if (num < 0)
			{
				return roomName;
			}
			return roomName.Substring(num + this.virtualStumpRoomPrepend.Length);
		}

		// Token: 0x06006BD5 RID: 27605 RVA: 0x0022BE6B File Offset: 0x0022A06B
		public string GetVStumpRoomDisplayName(string roomName)
		{
			if (!this.IsVStumpRoomName(roomName))
			{
				return roomName;
			}
			return roomName.Substring(1);
		}

		// Token: 0x17000A4E RID: 2638
		// (get) Token: 0x06006BD6 RID: 27606 RVA: 0x0022BE80 File Offset: 0x0022A080
		public GorillaComputer.ComputerState currentState
		{
			get
			{
				GorillaComputer.ComputerState computerState;
				this.stateStack.TryPeek(out computerState);
				return computerState;
			}
		}

		// Token: 0x17000A4F RID: 2639
		// (get) Token: 0x06006BD7 RID: 27607 RVA: 0x0022BE9C File Offset: 0x0022A09C
		public string NameTagPlayerPref
		{
			get
			{
				if (PlayFabAuthenticator.instance == null)
				{
					Debug.LogError("Trying to access PlayFab Authenticator Instance, but it is null. Will use a shared key for the nametag instead");
					return "nameTagsOn";
				}
				return "nameTagsOn-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
		}

		// Token: 0x17000A50 RID: 2640
		// (get) Token: 0x06006BD8 RID: 27608 RVA: 0x0022BED3 File Offset: 0x0022A0D3
		// (set) Token: 0x06006BD9 RID: 27609 RVA: 0x0022BEDB File Offset: 0x0022A0DB
		public bool NametagsEnabled { get; private set; }

		// Token: 0x17000A51 RID: 2641
		// (get) Token: 0x06006BDA RID: 27610 RVA: 0x0022BEE4 File Offset: 0x0022A0E4
		// (set) Token: 0x06006BDB RID: 27611 RVA: 0x0022BEEC File Offset: 0x0022A0EC
		public GorillaComputer.RedemptionResult RedemptionStatus
		{
			get
			{
				return this.redemptionResult;
			}
			set
			{
				this.redemptionResult = value;
				this.UpdateScreen();
			}
		}

		// Token: 0x17000A52 RID: 2642
		// (get) Token: 0x06006BDC RID: 27612 RVA: 0x0022BEFB File Offset: 0x0022A0FB
		// (set) Token: 0x06006BDD RID: 27613 RVA: 0x0022BF03 File Offset: 0x0022A103
		public string RedemptionCode
		{
			get
			{
				return this.redemptionCode;
			}
			set
			{
				this.redemptionCode = value;
			}
		}

		// Token: 0x17000A53 RID: 2643
		// (get) Token: 0x06006BDE RID: 27614 RVA: 0x0022BF0C File Offset: 0x0022A10C
		// (set) Token: 0x06006BDF RID: 27615 RVA: 0x0022BF14 File Offset: 0x0022A114
		public DateTimeOffset? RedemptionRestrictionTime { get; set; }

		// Token: 0x06006BE0 RID: 27616 RVA: 0x0022BF20 File Offset: 0x0022A120
		private void Awake()
		{
			if (GorillaComputer.instance == null)
			{
				GorillaComputer.instance = this;
				GorillaComputer.hasInstance = true;
			}
			else if (GorillaComputer.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this.version = Application.version;
			Debug.Log(string.Concat(new string[] { "==== GORILLA TAG - VERSION: ", this.version, ", BUILD NUMBER: ", this.buildCode, ", BUILD DATE: ", this.buildDate, " ====\r\n.\r\n.               _______\r\n.              /       \\\r\n.             /  _____  \\\r\n.            / / _   _ \\ \\\r\n.           [ | (O) (O) | ]\r\n.            | \\  . .  / |\r\n.     _______|  | _._ |  |_______\r\n.    /        \\  \\___/  /        \\\r\n.\r\n.\r\n" }));
			this._activeOrderList = this.OrderList;
			this.defaultUpdateCooldown = this.updateCooldown;
		}

		// Token: 0x06006BE1 RID: 27617 RVA: 0x0022BFD4 File Offset: 0x0022A1D4
		private void Start()
		{
			Debug.Log("Computer Init");
			this.Initialise();
		}

		// Token: 0x06006BE2 RID: 27618 RVA: 0x0022BFE6 File Offset: 0x0022A1E6
		public void OnEnable()
		{
			KIDManager.RegisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.RegisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006BE3 RID: 27619 RVA: 0x0022C011 File Offset: 0x0022A211
		public void OnDisable()
		{
			KIDManager.UnregisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.UnregisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006BE4 RID: 27620 RVA: 0x0022C03D File Offset: 0x0022A23D
		protected void OnDestroy()
		{
			if (GorillaComputer.instance == this)
			{
				GorillaComputer.hasInstance = false;
				GorillaComputer.instance = null;
			}
			KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		// Token: 0x06006BE5 RID: 27621 RVA: 0x0022C070 File Offset: 0x0022A270
		public void SliceUpdate()
		{
			if ((this.internetFailure && Time.realtimeSinceStartup < this.lastCheckedWifi + this.checkIfConnectedSeconds) || (!this.internetFailure && Time.realtimeSinceStartup < this.lastCheckedWifi + this.checkIfDisconnectedSeconds))
			{
				if (!this.internetFailure && this.isConnectedToMaster && Time.realtimeSinceStartup > this.lastUpdateTime + this.updateCooldown)
				{
					this.deltaTime = Time.realtimeSinceStartup - this.lastUpdateTime;
					this.lastUpdateTime = Time.realtimeSinceStartup;
					this.UpdateScreen();
				}
				return;
			}
			this.lastCheckedWifi = Time.realtimeSinceStartup;
			this.stateUpdated = false;
			if (!this.CheckInternetConnection())
			{
				string text = "NO WIFI OR LAN CONNECTION DETECTED.";
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("NO_CONNECTION", out text2, text);
				this.UpdateFailureText(text2);
				this.internetFailure = true;
				return;
			}
			if (this.internetFailure)
			{
				if (this.CheckInternetConnection())
				{
					this.internetFailure = false;
				}
				this.RestoreFromFailureState();
				this.UpdateScreen();
				return;
			}
			if (this.isConnectedToMaster && Time.realtimeSinceStartup > this.lastUpdateTime + this.updateCooldown)
			{
				this.deltaTime = Time.realtimeSinceStartup - this.lastUpdateTime;
				this.lastUpdateTime = Time.realtimeSinceStartup;
				this.UpdateScreen();
			}
		}

		// Token: 0x06006BE6 RID: 27622 RVA: 0x0022C1A0 File Offset: 0x0022A3A0
		private void Initialise()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.AddListener(new UnityAction<GorillaKeyboardBindings>(this.PressButton));
			RoomSystem.JoinedRoomEvent += new Action(GorillaComputer.OnFirstJoinedRoom_IncrementSessionCount);
			RoomSystem.JoinedRoomEvent += new Action(this.UpdateScreen);
			RoomSystem.LeftRoomEvent += new Action(this.UpdateScreen);
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.PlayerCountChangedCallback);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.PlayerCountChangedCallback);
			LocalisationManager.RegisterOnLanguageChanged(delegate
			{
				this.RefreshFunctionNames();
				this.UpdateGameModeText();
			});
			this.RefreshFunctionNames();
			this.InitialiseRoomScreens();
			this.InitialiseStrings();
			this.InitialiseAllRoomStates();
			this.UpdateScreen();
			byte[] array = new byte[] { Convert.ToByte(64) };
			this.virtualStumpRoomPrepend = Encoding.ASCII.GetString(array);
			this.initialized = true;
		}

		// Token: 0x06006BE7 RID: 27623 RVA: 0x0022C2A4 File Offset: 0x0022A4A4
		private void InitialiseRoomScreens()
		{
			this.screenText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.ScreenTextChangedEvent, GameEvents.ScreenTextMaterialsEvent);
			this.functionSelectText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.FunctionSelectTextChangedEvent, null);
		}

		// Token: 0x06006BE8 RID: 27624 RVA: 0x0022C2FC File Offset: 0x0022A4FC
		private void InitialiseStrings()
		{
			this.roomToJoin = "";
			this.redText = "";
			this.blueText = "";
			this.greenText = "";
			this.currentName = "";
			this.savedName = "";
		}

		// Token: 0x06006BE9 RID: 27625 RVA: 0x0022C34C File Offset: 0x0022A54C
		private void InitialiseAllRoomStates()
		{
			this.SwitchState(GorillaComputer.ComputerState.Startup, true);
			this.InitialiseLanguageScreen();
			this.InitializeNameState();
			this.InitializeRoomState();
			this.InitializeTurnState();
			this.InitializeStartupState();
			this.InitializeQueueState();
			this.InitializeMicState();
			this.InitializeGroupState();
			this.InitializeVoiceState();
			this.InitializeAutoMuteState();
			this.InitializeGameMode();
			this.InitializeVisualsState();
			this.InitializeCreditsState();
			this.InitializeTimeState();
			this.InitializeSupportState();
			this.InitializeTroopState();
			this.InitializeKIdState();
			this.InitializeRedeemState();
		}

		// Token: 0x06006BEA RID: 27626 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void InitializeStartupState()
		{
		}

		// Token: 0x06006BEB RID: 27627 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void InitializeRoomState()
		{
		}

		// Token: 0x06006BEC RID: 27628 RVA: 0x0022C3D0 File Offset: 0x0022A5D0
		private void InitializeColorState()
		{
			this.redValue = PlayerPrefs.GetFloat("redValue", 0f);
			this.greenValue = PlayerPrefs.GetFloat("greenValue", 0f);
			this.blueValue = PlayerPrefs.GetFloat("blueValue", 0f);
			this.blueText = Mathf.Floor(this.blueValue * 9f).ToString();
			this.redText = Mathf.Floor(this.redValue * 9f).ToString();
			this.greenText = Mathf.Floor(this.greenValue * 9f).ToString();
			this.colorCursorLine = 0;
			GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
		}

		// Token: 0x06006BED RID: 27629 RVA: 0x0022C49C File Offset: 0x0022A69C
		private void InitializeNameState()
		{
			int @int = PlayerPrefs.GetInt("nameTagsOn", -1);
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			switch (permissionDataByFeature.ManagedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (@int == -1)
				{
					this.NametagsEnabled = permissionDataByFeature.Enabled;
				}
				else
				{
					this.NametagsEnabled = @int > 0;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				this.NametagsEnabled = permissionDataByFeature.Enabled && @int > 0;
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.NametagsEnabled = false;
				break;
			}
			this.savedName = PlayerPrefs.GetString("playerName", "gorilla");
			NetworkSystem.Instance.SetMyNickName(this.savedName);
			this.currentName = this.savedName;
			VRRigCache.Instance.localRig.Rig.UpdateName();
			this.exactOneWeek = this.exactOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereOneWeek = this.anywhereOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereTwoWeek = this.anywhereTwoWeekFile.text.Split('\n', StringSplitOptions.None);
			for (int i = 0; i < this.exactOneWeek.Length; i++)
			{
				this.exactOneWeek[i] = this.exactOneWeek[i].ToLower().TrimEnd(new char[] { '\r', '\n' });
			}
			for (int j = 0; j < this.anywhereOneWeek.Length; j++)
			{
				this.anywhereOneWeek[j] = this.anywhereOneWeek[j].ToLower().TrimEnd(new char[] { '\r', '\n' });
			}
			for (int k = 0; k < this.anywhereTwoWeek.Length; k++)
			{
				this.anywhereTwoWeek[k] = this.anywhereTwoWeek[k].ToLower().TrimEnd(new char[] { '\r', '\n' });
			}
		}

		// Token: 0x06006BEE RID: 27630 RVA: 0x0022C668 File Offset: 0x0022A868
		private void InitializeTurnState()
		{
			GorillaSnapTurn.LoadSettingsFromPlayerPrefs();
		}

		// Token: 0x06006BEF RID: 27631 RVA: 0x0022C670 File Offset: 0x0022A870
		private void InitializeMicState()
		{
			this.pttType = PlayerPrefs.GetString("pttType", "OPEN MIC");
			if (this.pttType == "ALL CHAT")
			{
				this.pttType = "OPEN MIC";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
			}
		}

		// Token: 0x06006BF0 RID: 27632 RVA: 0x0022C6C4 File Offset: 0x0022A8C4
		private void InitializeAutoMuteState()
		{
			int @int = PlayerPrefs.GetInt("autoMute", 1);
			if (@int == 0)
			{
				this.autoMuteType = "OFF";
				return;
			}
			if (@int == 1)
			{
				this.autoMuteType = "MODERATE";
				return;
			}
			if (@int == 2)
			{
				this.autoMuteType = "AGGRESSIVE";
			}
		}

		// Token: 0x06006BF1 RID: 27633 RVA: 0x0022C70C File Offset: 0x0022A90C
		private void InitializeQueueState()
		{
			this.currentQueue = PlayerPrefs.GetString("currentQueue", "DEFAULT");
			this.allowedInCompetitive = PlayerPrefs.GetInt("allowedInCompetitive", 0) == 1;
			if (!this.allowedInCompetitive && this.currentQueue == "COMPETITIVE")
			{
				PlayerPrefs.SetString("currentQueue", "DEFAULT");
				PlayerPrefs.Save();
				this.currentQueue = "DEFAULT";
			}
		}

		// Token: 0x06006BF2 RID: 27634 RVA: 0x0022C77B File Offset: 0x0022A97B
		private void InitializeGroupState()
		{
			this.groupMapJoin = PlayerPrefs.GetString("groupMapJoin", "FOREST");
			this.groupMapJoinIndex = PlayerPrefs.GetInt("groupMapJoinIndex", 0);
			this._allowedMapsToJoin = this.friendJoinCollider.myAllowedMapsToJoin;
		}

		// Token: 0x06006BF3 RID: 27635 RVA: 0x0022C7B4 File Offset: 0x0022A9B4
		private void InitializeTroopState()
		{
			bool flag = false;
			this.troopToJoin = (this.troopName = PlayerPrefs.GetString("troopName", string.Empty));
			if (!this.rememberTroopQueueState)
			{
				bool flag2 = PlayerPrefs.GetInt("troopQueueActive", 0) == 1;
				bool flag3 = this.currentQueue != "DEFAULT" && this.currentQueue != "COMPETITIVE" && this.currentQueue != "MINIGAMES";
				if (flag2 || flag3)
				{
					this.currentQueue = "DEFAULT";
					PlayerPrefs.SetInt("troopQueueActive", 0);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
					PlayerPrefs.Save();
				}
			}
			this.troopQueueActive = PlayerPrefs.GetInt("troopQueueActive", 0) == 1;
			if (this.troopQueueActive && !this.IsValidTroopName(this.troopName))
			{
				this.troopQueueActive = false;
				PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
				this.currentQueue = "DEFAULT";
				PlayerPrefs.SetString("currentQueue", this.currentQueue);
				flag = true;
			}
			if (this.troopQueueActive)
			{
				base.StartCoroutine(this.HandleInitialTroopQueueState());
			}
			if (flag)
			{
				PlayerPrefs.Save();
			}
		}

		// Token: 0x06006BF4 RID: 27636 RVA: 0x0022C8DF File Offset: 0x0022AADF
		private IEnumerator HandleInitialTroopQueueState()
		{
			Debug.Log("HandleInitialTroopQueueState()");
			while (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				yield return null;
			}
			this.RequestTroopPopulation(false);
			while (this.currentTroopPopulation < 0)
			{
				yield return null;
			}
			if (this.currentTroopPopulation < 2)
			{
				Debug.Log("Low population - starting in DEFAULT queue");
				this.JoinDefaultQueue();
			}
			yield break;
		}

		// Token: 0x06006BF5 RID: 27637 RVA: 0x0022C8F0 File Offset: 0x0022AAF0
		private void InitializeVoiceState()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			string text = PlayerPrefs.GetString("voiceChatOn", "");
			string text2 = "FALSE";
			switch (permissionDataByFeature.ManagedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (string.IsNullOrEmpty(text))
				{
					text2 = (permissionDataByFeature.Enabled ? "TRUE" : "FALSE");
				}
				else
				{
					text2 = text;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				if (permissionDataByFeature.Enabled)
				{
					text = (string.IsNullOrEmpty(text) ? "FALSE" : text);
					text2 = text;
				}
				else
				{
					text2 = "FALSE";
				}
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				text2 = "FALSE";
				break;
			}
			this.voiceChatOn = PlayerPrefs.GetString("voiceChatOn", text2);
		}

		// Token: 0x06006BF6 RID: 27638 RVA: 0x0022C996 File Offset: 0x0022AB96
		public void InitializeGameMode(string gameMode)
		{
			this.leftHanded = PlayerPrefs.GetInt("leftHanded", 0) == 1;
			this.OnModeSelectButtonPress(gameMode, this.leftHanded);
			GameModePages.SetSelectedGameModeShared(gameMode);
			this.didInitializeGameMode = true;
		}

		// Token: 0x06006BF7 RID: 27639 RVA: 0x0022C9C8 File Offset: 0x0022ABC8
		private void InitializeGameMode()
		{
			if (this.didInitializeGameMode)
			{
				return;
			}
			GorillaComputer.sessionCount = PlayerPrefs.GetInt("sessionCount", -1);
			string text = PlayerPrefs.GetString("currentGameModePostSI");
			if (GorillaComputer.sessionCount == -1)
			{
				GorillaComputer.sessionCount = ((text.Length == 0) ? 0 : 100);
				PlayerPrefs.SetInt("sessionCount", GorillaComputer.sessionCount);
				text = GameModeType.Infection.ToString();
				PlayerPrefs.SetString("currentGameModePostSI", text);
				PlayerPrefs.Save();
			}
			else if (GorillaComputer.sessionCount == 3)
			{
				GorillaComputer.sessionCount++;
				PlayerPrefs.SetInt("sessionCount", GorillaComputer.sessionCount);
				if (!text.StartsWith("Super"))
				{
					text = ((text == GameModeType.Casual.ToString()) ? GameModeType.SuperCasual.ToString() : GameModeType.SuperInfect.ToString());
					PlayerPrefs.SetString("currentGameModePostSI", text);
				}
				PlayerPrefs.Save();
			}
			GameModeType gameModeType;
			try
			{
				gameModeType = Enum.Parse<GameModeType>(text, true);
			}
			catch
			{
				gameModeType = GameModeType.SuperInfect;
				text = GameModeType.SuperInfect.ToString();
			}
			if (!GameMode.GameModeZoneMapping.AllModes.Contains(gameModeType) || gameModeType == GameModeType.None || gameModeType == GameModeType.Count)
			{
				Debug.Log("[GT/GorillaComputer]  InitializeGameMode: Falling back to default game mode " + string.Format("\"{0}\" because stored game mode \"{1}\" is not available in any zone.", GameModeType.SuperInfect, gameModeType));
				PlayerPrefs.SetString("currentGameModePostSI", GameModeType.SuperInfect.ToString());
				PlayerPrefs.Save();
				text = GameModeType.SuperInfect.ToString();
			}
			this.leftHanded = PlayerPrefs.GetInt("leftHanded", 0) == 1;
			this.OnModeSelectButtonPress(text, this.leftHanded);
			GameModePages.SetSelectedGameModeShared(text);
		}

		// Token: 0x06006BF8 RID: 27640 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void InitializeCreditsState()
		{
		}

		// Token: 0x06006BF9 RID: 27641 RVA: 0x0022CB88 File Offset: 0x0022AD88
		private void InitializeTimeState()
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}

		// Token: 0x06006BFA RID: 27642 RVA: 0x0022CB97 File Offset: 0x0022AD97
		private void InitializeSupportState()
		{
			this.displaySupport = false;
		}

		// Token: 0x06006BFB RID: 27643 RVA: 0x0022CBA0 File Offset: 0x0022ADA0
		private void InitializeVisualsState()
		{
			this.disableParticles = PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE";
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
			this.instrumentVolume = PlayerPrefs.GetFloat("instrumentVolume", 0.1f);
			this.iobtMode = SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.IOBT);
		}

		// Token: 0x06006BFC RID: 27644 RVA: 0x0022CC00 File Offset: 0x0022AE00
		private void InitializeRedeemState()
		{
			this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
		}

		// Token: 0x06006BFD RID: 27645 RVA: 0x0022CC09 File Offset: 0x0022AE09
		private bool CheckInternetConnection()
		{
			return Application.internetReachability > NetworkReachability.NotReachable;
		}

		// Token: 0x06006BFE RID: 27646 RVA: 0x0022CC14 File Offset: 0x0022AE14
		public void OnConnectedToMasterStuff()
		{
			if (!this.isConnectedToMaster)
			{
				this.isConnectedToMaster = true;
				GorillaServer.Instance.ReturnCurrentVersion(new ReturnCurrentVersionRequest
				{
					CurrentVersion = NetworkSystemConfig.AppVersionStripped,
					UpdatedSynchTest = new int?(this.includeUpdatedServerSynchTest)
				}, new Action<ExecuteFunctionResult>(this.OnReturnCurrentVersion), new Action<PlayFabError>(GorillaComputer.OnErrorShared));
				if (this.startupMillis == 0L && !this.tryGetTimeAgain)
				{
					this.GetCurrentTime();
				}
				bool safety = PlayFabAuthenticator.instance.GetSafety();
				if (!KIDManager.KidEnabledAndReady && !KIDManager.HasSession)
				{
					this.SetComputerSettingsBySafety(safety, new GorillaComputer.ComputerState[]
					{
						GorillaComputer.ComputerState.Voice,
						GorillaComputer.ComputerState.AutoMute,
						GorillaComputer.ComputerState.Name,
						GorillaComputer.ComputerState.Group
					}, false);
				}
			}
		}

		// Token: 0x06006BFF RID: 27647 RVA: 0x0022CCC4 File Offset: 0x0022AEC4
		private void OnReturnCurrentVersion(ExecuteFunctionResult result)
		{
			JsonObject jsonObject = (JsonObject)result.FunctionResult;
			if (jsonObject == null)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			object obj;
			if (jsonObject.TryGetValue("SynchTime", out obj))
			{
				Debug.Log("message value is: " + (string)obj);
			}
			if (jsonObject.TryGetValue("Fail", out obj) && (bool)obj)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("ResultCode", out obj) && (ulong)obj != 0UL)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("QueueStats", out obj))
			{
				JsonObject jsonObject2 = (JsonObject)obj;
				string text = "QueueStats: ";
				JsonObject jsonObject3 = jsonObject2;
				Debug.Log(text + ((jsonObject3 != null) ? jsonObject3.ToString() : null));
				if (jsonObject2.TryGetValue("TopTroops", out obj))
				{
					this.topTroops.Clear();
					foreach (object obj2 in ((JsonArray)obj))
					{
						this.topTroops.Add(obj2.ToString());
					}
				}
				if (jsonObject2.TryGetValue("TopVstumpMapIds", out obj))
				{
					this.topVstumpMaps.Clear();
					foreach (object obj3 in ((JsonArray)obj))
					{
						this.topVstumpMaps.Add(obj3.ToString());
					}
				}
			}
			if (jsonObject.TryGetValue("BannedUsers", out obj))
			{
				this.usersBanned = int.Parse((string)obj);
			}
			this.UpdateScreen();
		}

		// Token: 0x06006C00 RID: 27648 RVA: 0x0022CE8C File Offset: 0x0022B08C
		public void PressButton(GorillaKeyboardBindings buttonPressed)
		{
			if (this.currentState == GorillaComputer.ComputerState.Startup)
			{
				this.ProcessStartupState(buttonPressed);
				this.UpdateScreen();
				return;
			}
			this.RequestTroopPopulation(false);
			bool flag = true;
			if (buttonPressed == GorillaKeyboardBindings.up)
			{
				flag = false;
				this.DecreaseState();
			}
			else if (buttonPressed == GorillaKeyboardBindings.down)
			{
				flag = false;
				this.IncreaseState();
			}
			if (flag)
			{
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Name:
					this.ProcessNameState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Turn:
					this.ProcessTurnState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Mic:
					this.ProcessMicState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Room:
					this.ProcessRoomState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Queue:
					this.ProcessQueueState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Group:
					this.ProcessGroupState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Voice:
					this.ProcessVoiceState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.ProcessAutoMuteState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Credits:
					this.ProcessCreditsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.ProcessVisualsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.ProcessNameWarningState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Support:
					this.ProcessSupportState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Troop:
					this.ProcessTroopState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.KID:
					this.ProcessKIdState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.ProcessRedemptionState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Language:
					this.ProcessLanguageState(buttonPressed);
					break;
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x06006C01 RID: 27649 RVA: 0x0022CFD4 File Offset: 0x0022B1D4
		public void OnModeSelectButtonPress(string gameMode, bool leftHand)
		{
			this.lastPressedGameMode = gameMode;
			this.lastPressedGameModeType = (GameModeType)GameMode.gameModeKeyByName.GetValueOrDefault(gameMode, 11);
			PlayerPrefs.SetString("currentGameModePostSI", gameMode);
			if (leftHand != this.leftHanded)
			{
				PlayerPrefs.SetInt("leftHanded", leftHand ? 1 : 0);
				this.leftHanded = leftHand;
			}
			PlayerPrefs.Save();
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				FriendshipGroupDetection.Instance.SendRequestPartyGameMode(gameMode);
				return;
			}
			this.SetGameModeWithoutButton(gameMode);
		}

		// Token: 0x06006C02 RID: 27650 RVA: 0x0022D04B File Offset: 0x0022B24B
		public void SetGameModeWithoutButton(string gameMode)
		{
			this.currentGameMode.Value = gameMode;
			this.UpdateGameModeText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
		}

		// Token: 0x06006C03 RID: 27651 RVA: 0x0022D06B File Offset: 0x0022B26B
		public void RegisterPrimaryJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.primaryTriggersByZone[trigger.networkZone] = trigger;
		}

		// Token: 0x06006C04 RID: 27652 RVA: 0x0022D080 File Offset: 0x0022B280
		private GorillaNetworkJoinTrigger GetSelectedMapJoinTrigger()
		{
			GorillaNetworkJoinTrigger gorillaNetworkJoinTrigger;
			this.primaryTriggersByZone.TryGetValue(this._allowedMapsToJoin[Mathf.Min(this._allowedMapsToJoin.Length - 1, this.groupMapJoinIndex)], out gorillaNetworkJoinTrigger);
			return gorillaNetworkJoinTrigger;
		}

		// Token: 0x06006C05 RID: 27653 RVA: 0x0022D0B8 File Offset: 0x0022B2B8
		public GorillaNetworkJoinTrigger GetJoinTriggerForZone(string zone)
		{
			GorillaNetworkJoinTrigger gorillaNetworkJoinTrigger;
			this.primaryTriggersByZone.TryGetValue(zone, out gorillaNetworkJoinTrigger);
			return gorillaNetworkJoinTrigger;
		}

		// Token: 0x06006C06 RID: 27654 RVA: 0x0022D0D8 File Offset: 0x0022B2D8
		public GorillaNetworkJoinTrigger GetJoinTriggerFromFullGameModeString(string gameModeString)
		{
			foreach (KeyValuePair<string, GorillaNetworkJoinTrigger> keyValuePair in this.primaryTriggersByZone)
			{
				if (gameModeString.StartsWith(keyValuePair.Key))
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		// Token: 0x06006C07 RID: 27655 RVA: 0x0022D140 File Offset: 0x0022B340
		public void OnGroupJoinButtonPress(int mapJoinIndex, GorillaFriendCollider chosenFriendJoinCollider)
		{
			Debug.Log("On Group button press. Map:" + mapJoinIndex.ToString() + " - collider: " + chosenFriendJoinCollider.name);
			if (mapJoinIndex >= this._allowedMapsToJoin.Length)
			{
				this.roomNotAllowed = true;
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
			if (!FriendshipGroupDetection.Instance.IsInParty)
			{
				if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
				{
					PhotonNetworkController.Instance.FriendIDList = new List<string>(chosenFriendJoinCollider.playerIDsCurrentlyTouching);
					foreach (string text in this.networkController.FriendIDList)
					{
						Debug.Log("Friend ID:" + text);
					}
					PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					RoomSystem.SendNearbyFollowCommand(chosenFriendJoinCollider, PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
					PhotonNetwork.SendAllOutgoingCommands();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.JoinWithNearby, null, false);
					this.currentStateIndex = 0;
					this.SwitchState(this.GetState(this.currentStateIndex), true);
				}
				return;
			}
			if (selectedMapJoinTrigger != null && selectedMapJoinTrigger.CanPartyJoin())
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.ForceJoinWithParty, null, false);
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			this.UpdateScreen();
		}

		// Token: 0x06006C08 RID: 27656 RVA: 0x0022D334 File Offset: 0x0022B534
		public void CompQueueUnlockButtonPress()
		{
			this.allowedInCompetitive = true;
			PlayerPrefs.SetInt("allowedInCompetitive", 1);
			PlayerPrefs.Save();
			if (RankedProgressionManager.Instance != null)
			{
				RankedProgressionManager.Instance.RequestUnlockCompetitiveQueue(true);
			}
		}

		// Token: 0x06006C09 RID: 27657 RVA: 0x0022D368 File Offset: 0x0022B568
		private void SwitchState(GorillaComputer.ComputerState newState, bool clearStack = true)
		{
			if (this.currentComputerState == GorillaComputer.ComputerState.Mic && this.currentComputerState != newState)
			{
				this.updateCooldown = this.defaultUpdateCooldown;
			}
			else if (newState == GorillaComputer.ComputerState.Mic)
			{
				this.updateCooldown = this.micUpdateCooldown;
			}
			if (this.previousComputerState != this.currentComputerState)
			{
				this.previousComputerState = this.currentComputerState;
			}
			this.currentComputerState = newState;
			if (this.LoadingRoutine != null)
			{
				base.StopCoroutine(this.LoadingRoutine);
			}
			if (clearStack)
			{
				this.stateStack.Clear();
			}
			this.stateStack.Push(newState);
		}

		// Token: 0x06006C0A RID: 27658 RVA: 0x0022D3F4 File Offset: 0x0022B5F4
		private void PopState()
		{
			this.currentComputerState = this.previousComputerState;
			if (this.stateStack.Count <= 1)
			{
				Debug.LogError("Can't pop into an empty stack");
				return;
			}
			this.stateStack.Pop();
			this.UpdateScreen();
		}

		// Token: 0x06006C0B RID: 27659 RVA: 0x0022D42D File Offset: 0x0022B62D
		private void SwitchToWarningState()
		{
			this.warningConfirmationInputString = string.Empty;
			this.SwitchState(GorillaComputer.ComputerState.NameWarning, false);
		}

		// Token: 0x06006C0C RID: 27660 RVA: 0x0022D443 File Offset: 0x0022B643
		private void SwitchToLoadingState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Loading, false);
		}

		// Token: 0x06006C0D RID: 27661 RVA: 0x0022D44E File Offset: 0x0022B64E
		private void ProcessStartupState(GorillaKeyboardBindings buttonPressed)
		{
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06006C0E RID: 27662 RVA: 0x0022D464 File Offset: 0x0022B664
		private void ProcessColorState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.enter:
				return;
			case GorillaKeyboardBindings.option1:
				this.colorCursorLine = 0;
				return;
			case GorillaKeyboardBindings.option2:
				this.colorCursorLine = 1;
				return;
			case GorillaKeyboardBindings.option3:
				this.colorCursorLine = 2;
				return;
			default:
			{
				int num = (int)buttonPressed;
				if (num < 10)
				{
					switch (this.colorCursorLine)
					{
					case 0:
						this.redText = num.ToString();
						this.redValue = (float)num / 9f;
						PlayerPrefs.SetFloat("redValue", this.redValue);
						break;
					case 1:
						this.greenText = num.ToString();
						this.greenValue = (float)num / 9f;
						PlayerPrefs.SetFloat("greenValue", this.greenValue);
						break;
					case 2:
						this.blueText = num.ToString();
						this.blueValue = (float)num / 9f;
						PlayerPrefs.SetFloat("blueValue", this.blueValue);
						break;
					}
					GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
					PlayerPrefs.Save();
					if (NetworkSystem.Instance.InRoom)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { this.redValue, this.greenValue, this.blueValue });
					}
				}
				return;
			}
			}
		}

		// Token: 0x06006C0F RID: 27663 RVA: 0x0022D5C4 File Offset: 0x0022B7C4
		public void ProcessNameState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (this.currentName.Length > 0 && this.NametagsEnabled)
					{
						this.currentName = this.currentName.Substring(0, this.currentName.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (this.currentName != this.savedName && this.currentName != "" && this.NametagsEnabled)
					{
						this.CheckAutoBanListForPlayerName(this.currentName);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.UpdateNametagSetting(!this.NametagsEnabled, true);
					return;
				default:
					if (this.NametagsEnabled && this.currentName.Length < 12 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
					{
						string text = this.currentName;
						string text2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							text2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							text2 = num.ToString();
						}
						this.currentName = text + text2;
					}
					break;
				}
			}
		}

		// Token: 0x06006C10 RID: 27664 RVA: 0x0022D6D8 File Offset: 0x0022B8D8
		private void ProcessRoomState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.delete:
				if (flag && ((this.playerInVirtualStump && this.roomToJoin.Length > this.VStumpRoomFullPrepend.Length) || (!this.playerInVirtualStump && this.roomToJoin.Length > 0)))
				{
					this.roomToJoin = this.roomToJoin.Substring(0, this.roomToJoin.Length - 1);
					return;
				}
				break;
			case GorillaKeyboardBindings.enter:
				if (flag && ((!this.playerInVirtualStump && this.roomToJoin != "") || (this.playerInVirtualStump && this.roomToJoin.Length > this.VStumpRoomFullPrepend.Length)))
				{
					this.CheckAutoBanListForRoomName(this.roomToJoin);
					return;
				}
				break;
			case GorillaKeyboardBindings.option1:
				if (!FriendshipGroupDetection.Instance.IsInParty)
				{
					NetworkSystem.Instance.ReturnToSinglePlayer();
					return;
				}
				if (FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider, false))
				{
					this.OnGroupJoinButtonPress(0, this.friendJoinCollider);
					return;
				}
				FriendshipGroupDetection.Instance.LeaveParty();
				this.DisconnectAfterDelay(1f);
				return;
			case GorillaKeyboardBindings.option2:
				this.RequestUpdatedPermissions();
				return;
			case GorillaKeyboardBindings.option3:
				break;
			default:
				if (flag && this.roomToJoin.Length < 10)
				{
					string text = this.roomToJoin;
					string text2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						text2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						text2 = num.ToString();
					}
					this.roomToJoin = text + text2;
				}
				break;
			}
		}

		// Token: 0x06006C11 RID: 27665 RVA: 0x0022D868 File Offset: 0x0022BA68
		private async void DisconnectAfterDelay(float seconds)
		{
			await Task.Delay((int)(1000f * seconds));
			await NetworkSystem.Instance.ReturnToSinglePlayer();
		}

		// Token: 0x06006C12 RID: 27666 RVA: 0x0022D8A0 File Offset: 0x0022BAA0
		private void ProcessTurnState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				GorillaSnapTurn.UpdateAndSaveTurnFactor((int)buttonPressed);
				return;
			}
			string text = string.Empty;
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				text = "SNAP";
				break;
			case GorillaKeyboardBindings.option2:
				text = "SMOOTH";
				break;
			case GorillaKeyboardBindings.option3:
				text = "NONE";
				break;
			}
			if (text.Length > 0)
			{
				GorillaSnapTurn.UpdateAndSaveTurnType(text);
			}
		}

		// Token: 0x06006C13 RID: 27667 RVA: 0x0022D900 File Offset: 0x0022BB00
		private void ProcessMicState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.pttType = "OPEN MIC";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option2:
				this.pttType = "PUSH TO TALK";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option3:
				this.pttType = "PUSH TO MUTE";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			default:
				return;
			}
		}

		// Token: 0x06006C14 RID: 27668 RVA: 0x0022D988 File Offset: 0x0022BB88
		private void ProcessQueueState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.JoinQueue("DEFAULT", false);
				return;
			case GorillaKeyboardBindings.option2:
				this.JoinQueue("MINIGAMES", false);
				return;
			case GorillaKeyboardBindings.option3:
				if (this.allowedInCompetitive)
				{
					this.JoinQueue("COMPETITIVE", false);
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x06006C15 RID: 27669 RVA: 0x0022D9E4 File Offset: 0x0022BBE4
		public void JoinTroop(string newTroopName)
		{
			if (this.IsValidTroopName(newTroopName))
			{
				this.currentTroopPopulation = -1;
				this.troopName = newTroopName;
				PlayerPrefs.SetString("troopName", this.troopName);
				if (this.troopQueueActive)
				{
					this.currentQueue = this.GetQueueNameForTroop(this.troopName);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
				}
				PlayerPrefs.Save();
				this.JoinTroopQueue();
			}
		}

		// Token: 0x06006C16 RID: 27670 RVA: 0x0022DA4D File Offset: 0x0022BC4D
		public void JoinTroopQueue()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.currentTroopPopulation = -1;
				this.JoinQueue(this.GetQueueNameForTroop(this.troopName), true);
				this.RequestTroopPopulation(true);
			}
		}

		// Token: 0x06006C17 RID: 27671 RVA: 0x0022DA80 File Offset: 0x0022BC80
		private void RequestTroopPopulation(bool forceUpdate = false)
		{
			if (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				return;
			}
			if (!this.hasRequestedInitialTroopPopulation || forceUpdate)
			{
				if (this.nextPopulationCheckTime > Time.realtimeSinceStartup)
				{
					return;
				}
				this.nextPopulationCheckTime = Time.realtimeSinceStartup + this.troopPopulationCheckCooldown;
				this.hasRequestedInitialTroopPopulation = true;
				GorillaServer.Instance.ReturnQueueStats(new ReturnQueueStatsRequest
				{
					queueName = this.troopName
				}, delegate(ExecuteFunctionResult result)
				{
					Debug.Log("Troop pop received");
					object obj;
					if (((JsonObject)result.FunctionResult).TryGetValue("PlayerCount", out obj))
					{
						this.currentTroopPopulation = int.Parse(obj.ToString());
						if (this.currentComputerState == GorillaComputer.ComputerState.Queue)
						{
							this.UpdateScreen();
							return;
						}
					}
					else
					{
						this.currentTroopPopulation = 0;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogError(string.Format("Error requesting troop population: {0}", error));
					this.currentTroopPopulation = -1;
				});
			}
		}

		// Token: 0x06006C18 RID: 27672 RVA: 0x0022DAFE File Offset: 0x0022BCFE
		public void JoinDefaultQueue()
		{
			this.JoinQueue("DEFAULT", false);
		}

		// Token: 0x06006C19 RID: 27673 RVA: 0x0022DB0C File Offset: 0x0022BD0C
		public void LeaveTroop()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.troopToJoin = this.troopName;
			}
			this.currentTroopPopulation = -1;
			this.troopName = string.Empty;
			PlayerPrefs.SetString("troopName", this.troopName);
			if (this.troopQueueActive)
			{
				this.JoinDefaultQueue();
			}
			PlayerPrefs.Save();
		}

		// Token: 0x06006C1A RID: 27674 RVA: 0x0022DB68 File Offset: 0x0022BD68
		public string GetCurrentTroop()
		{
			if (this.troopQueueActive)
			{
				return this.troopName;
			}
			return this.currentQueue;
		}

		// Token: 0x06006C1B RID: 27675 RVA: 0x0022DB7F File Offset: 0x0022BD7F
		public int GetCurrentTroopPopulation()
		{
			if (this.troopQueueActive)
			{
				return this.currentTroopPopulation;
			}
			return -1;
		}

		// Token: 0x06006C1C RID: 27676 RVA: 0x0022DB94 File Offset: 0x0022BD94
		private void JoinQueue(string queueName, bool isTroopQueue = false)
		{
			this.currentQueue = queueName;
			this.troopQueueActive = isTroopQueue;
			this.currentTroopPopulation = -1;
			PlayerPrefs.SetString("currentQueue", this.currentQueue);
			PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
			PlayerPrefs.Save();
		}

		// Token: 0x06006C1D RID: 27677 RVA: 0x0022DBE4 File Offset: 0x0022BDE4
		private void ProcessGroupState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.one:
				this.SetGroupMapJoin("FOREST", 0);
				break;
			case GorillaKeyboardBindings.two:
				this.SetGroupMapJoin("CANYON", 1);
				break;
			case GorillaKeyboardBindings.three:
				this.SetGroupMapJoin("CITY", 2);
				break;
			default:
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					this.OnGroupJoinButtonPress(Mathf.Min(this._allowedMapsToJoin.Length - 1, this.groupMapJoinIndex), this.friendJoinCollider);
				}
				break;
			}
			this.roomFull = false;
		}

		// Token: 0x06006C1E RID: 27678 RVA: 0x0022DC67 File Offset: 0x0022BE67
		private void SetGroupMapJoin(string groupMap, int groupMapIndex)
		{
			this.groupMapJoin = groupMap;
			this.groupMapJoinIndex = groupMapIndex;
			PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
			PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
			PlayerPrefs.Save();
		}

		// Token: 0x06006C1F RID: 27679 RVA: 0x0022DC9C File Offset: 0x0022BE9C
		private void ProcessTroopState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
			bool flag2 = this.IsValidTroopName(this.troopName);
			if (flag)
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (!flag2 && this.troopToJoin.Length > 0)
					{
						this.troopToJoin = this.troopToJoin.Substring(0, this.troopToJoin.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (!flag2)
					{
						this.CheckAutoBanListForTroopName(this.troopToJoin);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.JoinTroopQueue();
					return;
				case GorillaKeyboardBindings.option2:
					this.JoinDefaultQueue();
					return;
				case GorillaKeyboardBindings.option3:
					this.LeaveTroop();
					return;
				default:
					if (!flag2 && this.troopToJoin.Length < 12)
					{
						string text = this.troopToJoin;
						string text2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							text2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							text2 = num.ToString();
						}
						this.troopToJoin = text + text2;
						return;
					}
					break;
				}
			}
			else
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.option1:
					break;
				case GorillaKeyboardBindings.option2:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
					{
						this.ProcessScreen_SetupKID();
						return;
					}
					this.RequestUpdatedPermissions();
					return;
				case GorillaKeyboardBindings.option3:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06006C20 RID: 27680 RVA: 0x0022DDC5 File Offset: 0x0022BFC5
		private bool IsValidTroopName(string troop)
		{
			return !string.IsNullOrEmpty(troop) && troop.Length <= 12 && (this.allowedInCompetitive || troop != "COMPETITIVE");
		}

		// Token: 0x06006C21 RID: 27681 RVA: 0x00022887 File Offset: 0x00020A87
		private string GetQueueNameForTroop(string troop)
		{
			return troop;
		}

		// Token: 0x06006C22 RID: 27682 RVA: 0x0022DDF0 File Offset: 0x0022BFF0
		private void ProcessVoiceState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				if (buttonPressed != GorillaKeyboardBindings.option1)
				{
					if (buttonPressed == GorillaKeyboardBindings.option2)
					{
						this.SetVoice(false, true);
					}
				}
				else
				{
					this.SetVoice(true, true);
				}
			}
			else if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				if (buttonPressed == GorillaKeyboardBindings.option3)
				{
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
				}
			}
			else if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
			{
				this.ProcessScreen_SetupKID();
			}
			else
			{
				this.RequestUpdatedPermissions();
			}
			RigContainer.RefreshAllRigVoices();
		}

		// Token: 0x06006C23 RID: 27683 RVA: 0x0022DE60 File Offset: 0x0022C060
		private void ProcessAutoMuteState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.autoMuteType = "AGGRESSIVE";
				PlayerPrefs.SetInt("autoMute", 2);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option2:
				this.autoMuteType = "MODERATE";
				PlayerPrefs.SetInt("autoMute", 1);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option3:
				this.autoMuteType = "OFF";
				PlayerPrefs.SetInt("autoMute", 0);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			}
			this.UpdateScreen();
		}

		// Token: 0x06006C24 RID: 27684 RVA: 0x0022DEF0 File Offset: 0x0022C0F0
		private void ProcessVisualsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				this.instrumentVolume = (float)buttonPressed / 50f;
				PlayerPrefs.SetFloat("instrumentVolume", this.instrumentVolume);
				PlayerPrefs.Save();
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.delete:
				if (SubscriptionManager.IsSubscriptionFeatureAvailable(SubscriptionManager.SubscriptionFeatures.IOBT) && GorillaIK.playerIK != null)
				{
					GorillaIK.playerIK.ResetLeanOffset();
				}
				break;
			case GorillaKeyboardBindings.enter:
				break;
			case GorillaKeyboardBindings.option1:
				this.disableParticles = false;
				PlayerPrefs.SetString("disableParticles", "FALSE");
				PlayerPrefs.Save();
				GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
				return;
			case GorillaKeyboardBindings.option2:
				this.disableParticles = true;
				PlayerPrefs.SetString("disableParticles", "TRUE");
				PlayerPrefs.Save();
				GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
				return;
			case GorillaKeyboardBindings.option3:
				if (SubscriptionManager.IsSubscriptionFeatureAvailable(SubscriptionManager.SubscriptionFeatures.IOBT) && GorillaIK.playerIK != null && GorillaIK.playerIK.CanUpdateIK())
				{
					SubscriptionManager.SetSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures.IOBT, (!this.iobtMode) ? 1 : 0);
					this.iobtMode = SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.IOBT);
					GorillaIK.playerIK.DelayedUpdateIK(this.iobtMode);
					return;
				}
				break;
			default:
				if (buttonPressed != GorillaKeyboardBindings.Q)
				{
					return;
				}
				if (SubscriptionManager.IsSubscriptionFeatureAvailable(SubscriptionManager.SubscriptionFeatures.IOBT) && GorillaIK.playerIK != null)
				{
					GorillaIK.playerIK.CalibrateLeanOffset();
					return;
				}
				break;
			}
		}

		// Token: 0x06006C25 RID: 27685 RVA: 0x0022E03E File Offset: 0x0022C23E
		private void ProcessCreditsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.creditsView.ProcessButtonPress(buttonPressed);
			}
		}

		// Token: 0x06006C26 RID: 27686 RVA: 0x0022E051 File Offset: 0x0022C251
		private void ProcessSupportState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.displaySupport = true;
			}
		}

		// Token: 0x06006C27 RID: 27687 RVA: 0x0022E060 File Offset: 0x0022C260
		private void ProcessRedemptionState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.RedemptionStatus == GorillaComputer.RedemptionResult.Checking)
			{
				return;
			}
			if (buttonPressed != GorillaKeyboardBindings.delete)
			{
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					if (this.redemptionCode != "")
					{
						if (this.redemptionCode.Length < 8)
						{
							this.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
							return;
						}
						CodeRedemption.Instance.HandleCodeRedemption(this.redemptionCode);
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Checking;
						return;
					}
					else if (this.RedemptionStatus != GorillaComputer.RedemptionResult.Success)
					{
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
						return;
					}
				}
				else if (this.redemptionCode.Length < 8 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
				{
					string text = this.redemptionCode;
					string text2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						text2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						text2 = num.ToString();
					}
					this.redemptionCode = text + text2;
				}
			}
			else if (this.redemptionCode.Length > 0)
			{
				this.redemptionCode = this.redemptionCode.Substring(0, this.redemptionCode.Length - 1);
				return;
			}
		}

		// Token: 0x06006C28 RID: 27688 RVA: 0x0022E14C File Offset: 0x0022C34C
		private void ProcessNameWarningState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				this.PopState();
				return;
			}
			if (buttonPressed == GorillaKeyboardBindings.delete)
			{
				if (this.warningConfirmationInputString.Length > 0)
				{
					this.warningConfirmationInputString = this.warningConfirmationInputString.Substring(0, this.warningConfirmationInputString.Length - 1);
					return;
				}
			}
			else if (this.warningConfirmationInputString.Length < 3)
			{
				this.warningConfirmationInputString += buttonPressed.ToString();
			}
		}

		// Token: 0x06006C29 RID: 27689 RVA: 0x0022E1D8 File Offset: 0x0022C3D8
		public void UpdateScreen()
		{
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.WrongVersion)
			{
				this.UpdateFunctionScreen();
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Startup:
					this.StartupScreen();
					break;
				case GorillaComputer.ComputerState.Name:
					this.NameScreen();
					break;
				case GorillaComputer.ComputerState.Turn:
					this.TurnScreen();
					break;
				case GorillaComputer.ComputerState.Mic:
					this.MicScreen();
					break;
				case GorillaComputer.ComputerState.Room:
					this.RoomScreen();
					break;
				case GorillaComputer.ComputerState.Queue:
					this.QueueScreen();
					break;
				case GorillaComputer.ComputerState.Group:
					this.GroupScreen();
					break;
				case GorillaComputer.ComputerState.Voice:
					this.VoiceScreen();
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.AutomuteScreen();
					break;
				case GorillaComputer.ComputerState.Credits:
					this.CreditsScreen();
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.VisualsScreen();
					break;
				case GorillaComputer.ComputerState.Time:
					this.TimeScreen();
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.NameWarningScreen();
					break;
				case GorillaComputer.ComputerState.Loading:
					this.LoadingScreen();
					break;
				case GorillaComputer.ComputerState.Support:
					this.SupportScreen();
					break;
				case GorillaComputer.ComputerState.Troop:
					this.TroopScreen();
					break;
				case GorillaComputer.ComputerState.KID:
					this.KIdScreen();
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.RedemptionScreen();
					break;
				case GorillaComputer.ComputerState.Language:
					this.LanguageScreen();
					break;
				}
			}
			this.UpdateGameModeText();
		}

		// Token: 0x06006C2A RID: 27690 RVA: 0x0022E310 File Offset: 0x0022C510
		private void LoadingScreen()
		{
			GorillaComputer.<>c__DisplayClass418_0 CS$<>8__locals1 = new GorillaComputer.<>c__DisplayClass418_0();
			CS$<>8__locals1.<>4__this = this;
			string text = "LOADING";
			LocalisationManager.TryGetKeyForCurrentLocale("LOADING_SCREEN", out CS$<>8__locals1.result, text);
			this.screenText.Set(CS$<>8__locals1.result);
			this.LoadingRoutine = base.StartCoroutine(CS$<>8__locals1.<LoadingScreen>g__LoadingScreenLocal|0());
		}

		// Token: 0x06006C2B RID: 27691 RVA: 0x0022E368 File Offset: 0x0022C568
		private void NameWarningScreen()
		{
			string text = "<color=red>WARNING: PLEASE CHOOSE A BETTER NAME\n\nENTERING ANOTHER BAD NAME WILL RESULT IN A BAN</color>";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN", out text2, text);
			this.screenText.Set(text2);
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				text = "\n\nPRESS ANY KEY TO CONTINUE";
				LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN_CONFIRMATION", out text2, text);
				this.screenText.Append(text2);
				return;
			}
			text = "\n\nTYPE 'YES' TO CONFIRM:";
			LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN_TYPE_YES", out text2, text);
			this.screenText.Append(text2.TrailingSpace());
			this.screenText.Append(this.warningConfirmationInputString);
		}

		// Token: 0x06006C2C RID: 27692 RVA: 0x0022E404 File Offset: 0x0022C604
		private void SupportScreen()
		{
			this.screenText.Set("");
			if (this.displaySupport)
			{
				string text = PlayFabAuthenticator.instance.platform.ToString().ToUpper();
				string text2;
				if (text == "PC")
				{
					text2 = "OCULUS PC";
				}
				else
				{
					text2 = text;
				}
				text = text2;
				string text3;
				if (!(text == "OCULUS PC"))
				{
					if (!(text == "STEAM"))
					{
						if (!(text == "PSVR"))
						{
							if (!(text == "PICO"))
							{
								if (!(text == "QUEST"))
								{
									text3 = "UNKNOWN_PLATFORM";
								}
								else
								{
									text3 = "PLATFORM_QUEST";
								}
							}
							else
							{
								text3 = "PLATFORM_PICO";
							}
						}
						else
						{
							text3 = "PLATFORM_PSVR";
						}
					}
					else
					{
						text3 = "PLATFORM_STEAM";
					}
				}
				else
				{
					text3 = "PLATFORM_OCULUS_PC";
				}
				string text4;
				LocalisationManager.TryGetKeyForCurrentLocale(text3, out text4, text);
				text = text4;
				string text5 = "SUPPORT";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INTRO", out text4, text5);
				this.screenText.Append(text4);
				text5 = "\n\nPLAYER ID";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_PLAYERID", out text4, text5);
				this.screenText.Append(text4 + "  ");
				this.screenText.Append(PlayFabAuthenticator.instance.GetPlayFabPlayerId());
				text5 = "\nVERSION";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_VERSION", out text4, text5);
				this.screenText.Append(text4 + " ");
				this.screenText.Append(this.version.ToUpper());
				text5 = "\nPLATFORM";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_PLATFORM", out text4, text5);
				this.screenText.Append(text4 + " ");
				this.screenText.Append(text);
				text5 = "\nBUILD DATE";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_BUILD_DATE", out text4, text5);
				this.screenText.Append(text4 + " ");
				this.screenText.Append(this.buildDate);
				text5 = "\nSESSION ID";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_MOTHERSHIP_SESSION_ID", out text4, text5);
				string sessionId = MothershipClientApiUnity.SessionId;
				string text6 = sessionId;
				int num = sessionId.LastIndexOf('-');
				if (num >= 0)
				{
					string text7 = sessionId.Substring(0, num);
					string text8 = "\n            ";
					text2 = sessionId;
					int num2 = num + 1;
					text6 = text7 + text8 + text2.Substring(num2, text2.Length - num2);
				}
				this.screenText.Append(text4 + " ");
				this.screenText.Append(text6);
				if (KIDManager.KidEnabled)
				{
					text5 = "\nk-ID ACCOUNT TYPE:";
					LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_KID_ACCOUNT_TYPE", out text4, text5);
					this.screenText.Append(text4.TrailingSpace());
					this.screenText.Append(KIDManager.GetActiveAccountStatusNiceString().ToUpper());
					return;
				}
			}
			else
			{
				string text9 = "SUPPORT";
				string text10;
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INTRO", out text10, text9);
				this.screenText.Append(text10);
				text9 = "\n\nPRESS ENTER TO DISPLAY SUPPORT AND ACCOUNT INFORMATION";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INITIAL", out text10, text9);
				this.screenText.Append(text10);
				text9 = "\n\n\n\n<color=red>DO NOT SHARE ACCOUNT INFORMATION WITH ANYONE OTHER THAN ANOTHER AXIOM</color>";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INITIAL_WARNING", out text10, text9);
				this.screenText.Append(text10);
			}
		}

		// Token: 0x06006C2D RID: 27693 RVA: 0x0022E728 File Offset: 0x0022C928
		private void TimeScreen()
		{
			string text = "UPDATE TIME SETTINGS. (LOCALLY ONLY). \nPRESS OPTION 1 FOR NORMAL MODE. \nPRESS OPTION 2 FOR STATIC MODE. \nPRESS 1-10 TO CHANGE TIME OF DAY. \nCURRENT MODE: {currentSetting}.\nTIME OF DAY: {currentTimeOfDay}.\n";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("TIME_SCREEN", out text2, text);
			text2 = text2.Replace("{currentSetting}", BetterDayNightManager.instance.currentSetting.ToString().ToUpper()).Replace("{currentTimeOfDay}", BetterDayNightManager.instance.currentTimeOfDay.ToUpper());
			this.screenText.Set(text2);
		}

		// Token: 0x06006C2E RID: 27694 RVA: 0x0022E798 File Offset: 0x0022C998
		private void CreditsScreen()
		{
			this.screenText.Set(this.creditsView.GetScreenText());
		}

		// Token: 0x06006C2F RID: 27695 RVA: 0x0022E7B0 File Offset: 0x0022C9B0
		private void VisualsScreen()
		{
			string text = "UPDATE ITEMS SETTINGS.";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_INTRO", out text2, text);
			this.screenText.Set(text2.TrailingSpace());
			text = "PRESS OPTION 1 TO ENABLE ITEM PARTICLES. PRESS OPTION 2 TO DISABLE ITEM PARTICLES. PRESS 1-10 TO CHANGE INSTRUMENT VOLUME FOR OTHER PLAYERS.";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_OPTIONS", out text2, text);
			this.screenText.Append(text2);
			text = "\n\nITEM PARTICLES ON:";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_CURRENT", out text2, text);
			this.screenText.Append(text2.TrailingSpace());
			string text3 = (this.disableParticles ? "FALSE" : "TRUE");
			LocalisationManager.TryGetKeyForCurrentLocale(text3, out text2, text3);
			this.screenText.Append(text2);
			text = "\nINSTRUMENT VOLUME:";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_VOLUME", out text2, text);
			this.screenText.Append(text2.TrailingSpace());
			this.screenText.Append(Mathf.CeilToInt(this.instrumentVolume * 50f).ToString());
			if (!SubscriptionManager.IsSubscriptionFeatureAvailable(SubscriptionManager.SubscriptionFeatures.IOBT))
			{
				return;
			}
			text = "\n\nPRESS OPTION 3 TO TOGGLE IOBT:";
			text2 = text;
			this.screenText.Append(text2.TrailingSpace());
			this.iobtMode = SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.IOBT);
			text3 = (this.iobtMode ? "TRUE" : "FALSE");
			LocalisationManager.TryGetKeyForCurrentLocale(text3, out text2, text3);
			this.screenText.Append(text2);
			text = "\n-PRESS Q TO CALIBRATE / DELETE TO RESET";
			text2 = text;
			this.screenText.Append(text2);
		}

		// Token: 0x06006C30 RID: 27696 RVA: 0x0022E908 File Offset: 0x0022CB08
		private void VoiceScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				string text = "CHOOSE WHICH TYPE OF VOICE YOU WANT TO HEAR AND SPEAK.";
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_INTRO", out text2, text);
				this.screenText.Set(text2);
				text = "\nPRESS OPTION 1 = HUMAN VOICES.\nPRESS OPTION 2 = MONKE VOICES.";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_OPTIONS", out text2, text);
				this.screenText.Append(text2);
				text = "\n\nVOICE TYPE:";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_CURRENT", out text2, text);
				this.screenText.Append(text2.TrailingSpace());
				string text3 = ((this.voiceChatOn == "TRUE") ? "VOICE_OPTION_HUMAN" : ((this.voiceChatOn == "FALSE") ? "VOICE_OPTION_MONKE" : "VOICE_OPTION_OFF"));
				text = ((this.voiceChatOn == "TRUE") ? "HUMAN" : ((this.voiceChatOn == "FALSE") ? "MONKE" : "OFF"));
				LocalisationManager.TryGetKeyForCurrentLocale(text3, out text2, text);
				this.screenText.Append(text2);
				return;
			}
			if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.VoiceScreen_KIdProhibited();
				return;
			}
			this.VoiceScreen_Permission();
		}

		// Token: 0x06006C31 RID: 27697 RVA: 0x0022EA28 File Offset: 0x0022CC28
		private void AutomuteScreen()
		{
			string text = "AUTOMOD AUTOMATICALLY MUTES PLAYERS WHEN THEY JOIN YOUR ROOM IF A LOT OF OTHER PLAYERS HAVE MUTED THEM";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_INTRO", out text2, text);
			this.screenText.Set(text2);
			text = "\nPRESS OPTION 1 FOR AGGRESSIVE MUTING\nPRESS OPTION 2 FOR MODERATE MUTING\nPRESS OPTION 3 TO TURN AUTOMOD OFF";
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_OPTIONS", out text2, text);
			this.screenText.Append(text2);
			text = "\n\nCURRENT AUTOMOD LEVEL: ";
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_CURRENT", out text2, text);
			this.screenText.Append(text2.TrailingSpace());
			string text3 = "AUTOMOD_OFF";
			string text4 = this.autoMuteType;
			if (!(text4 == "OFF"))
			{
				if (!(text4 == "MODERATE"))
				{
					if (text4 == "AGGRESSIVE")
					{
						text3 = "AUTOMOD_AGGRESSIVE";
					}
				}
				else
				{
					text3 = "AUTOMOD_MODERATE";
				}
			}
			else
			{
				text3 = "AUTOMOD_OFF";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(text3, out text2, this.autoMuteType);
			this.screenText.Append(text2);
		}

		// Token: 0x06006C32 RID: 27698 RVA: 0x0022EB04 File Offset: 0x0022CD04
		private void GroupScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			string text = "";
			string text2 = ((this._allowedMapsToJoin.Length > 1) ? this.groupMapJoin : this._allowedMapsToJoin[0].ToUpper());
			string text3 = "";
			string text4;
			if (this._allowedMapsToJoin.Length > 1)
			{
				text4 = "\n\nUSE NUMBER KEYS TO SELECT DESTINATION\n1: FOREST, 2: CANYON, 3: CITY.";
				LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_DESTINATIONS", out text, text4);
				text3 = text;
			}
			text4 = "\n\nACTIVE ZONE WILL BE:";
			LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ACTIVE_ZONES", out text, text4);
			string text5 = text.TrailingSpace();
			text5 = text5 + text2 + text3;
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
				string text6 = "";
				if (!selectedMapJoinTrigger.CanPartyJoin())
				{
					text4 = "\n\n<color=red>CANNOT JOIN BECAUSE YOUR GROUP IS NOT HERE</color>";
					LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_CANNOT_JOIN", out text, text4);
					text6 = text;
				}
				text4 = "PRESS ENTER TO JOIN A PUBLIC GAME WITH YOUR FRIENDSHIP GROUP.";
				LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ENTER_PARTY", out text, text4);
				this.screenText.Set(text);
				text5 += text6;
				this.screenText.Append(text5);
				return;
			}
			text4 = "PRESS ENTER TO JOIN A PUBLIC GAME AND BRING EVERYONE IN THIS ROOM WITH YOU.";
			LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ENTER_NOPARTY", out text, text4);
			this.screenText.Set(text);
			this.screenText.Append(text5);
		}

		// Token: 0x06006C33 RID: 27699 RVA: 0x0022EC38 File Offset: 0x0022CE38
		private void MicScreen()
		{
			if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.MicScreen_KIdProhibited();
				return;
			}
			bool flag = false;
			string text = "";
			if (Microphone.devices.Length == 0)
			{
				flag = true;
				text = "NO MICROPHONE DETECTED";
			}
			if (flag)
			{
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_MIC_DISABLED", out text2, "MIC DISABLED: ");
				this.screenText.Set(text2 + text);
				return;
			}
			string text3 = "PRESS OPTION 1 = ALL CHAT.\nPRESS OPTION 2 = PUSH TO TALK.\nPRESS OPTION 3 = PUSH TO MUTE.";
			string text4;
			LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_OPTIONS", out text4, text3);
			this.screenText.Set(text4);
			text3 = "\n\nCURRENT MIC SETTING:";
			LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_CURRENT", out text4, text3);
			this.screenText.Append(text4.TrailingSpace());
			string text5 = "";
			string text6 = this.pttType;
			if (!(text6 == "PUSH TO MUTE"))
			{
				if (!(text6 == "PUSH TO TALK"))
				{
					if (!(text6 == "OPEN MIC"))
					{
						if (text6 == "ALL CHAT")
						{
							text5 = "OPEN_MIC";
						}
					}
					else
					{
						text5 = "OPEN_MIC";
					}
				}
				else
				{
					text5 = "PUSH_TO_TALK_MIC";
				}
			}
			else
			{
				text5 = "PUSH_TO_MUTE_MIC";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(text5, out text4, this.pttType);
			this.screenText.Append(text4);
			if (this.pttType == "PUSH TO MUTE")
			{
				text3 = "- MIC IS OPEN.\n- HOLD ANY FACE BUTTON TO MUTE.\n\n";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP", out text4, text3);
				this.screenText.Append(text4);
			}
			else if (this.pttType == "PUSH TO TALK")
			{
				text3 = "- MIC IS MUTED.\n- HOLD ANY FACE BUTTON TO TALK.\n\n";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_PUSH_TO_TALK_TOOLTIP", out text4, text3);
				this.screenText.Append(text4);
			}
			else
			{
				this.screenText.Append("\n\n\n");
			}
			if (this.speakerLoudness == null)
			{
				this.speakerLoudness = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
			}
			if (this.speakerLoudness != null)
			{
				float num = Mathf.Sqrt(this.speakerLoudness.LoudnessNormalized);
				if (num <= 0.01f)
				{
					this.micInputTestTimer += this.deltaTime;
				}
				else
				{
					this.micInputTestTimer = 0f;
				}
				if (this.pttType != "OPEN MIC")
				{
					bool flag2 = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
					bool flag3 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
					bool flag4 = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
					bool flag5 = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
					bool flag6 = flag2 || flag3 || flag4 || flag5;
					if (flag6 && this.pttType == "PUSH TO MUTE")
					{
						text3 = "INPUT TEST: ";
						LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text4, text3);
						this.screenText.Append(text4);
						return;
					}
					if (!flag6 && this.pttType == "PUSH TO TALK")
					{
						text3 = "INPUT TEST: ";
						LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text4, text3);
						this.screenText.Append(text4);
						return;
					}
				}
				if (this.micInputTestTimer >= this.micInputTestTimerThreshold)
				{
					text3 = "NO MIC INPUT DETECTED. CHECK MIC SETTINGS IN THE OPERATING SYSTEM.";
					LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_NO_MIC", out text4, text3);
					this.screenText.Append(text4);
					return;
				}
				text3 = "INPUT TEST: ";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text4, text3);
				this.screenText.Append(text4);
				for (int i = 0; i < Mathf.FloorToInt(num * 50f); i++)
				{
					this.screenText.Append("|");
				}
			}
		}

		// Token: 0x06006C34 RID: 27700 RVA: 0x0022EF88 File Offset: 0x0022D188
		private void QueueScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			string text = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES.";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN", out text2, text);
			this.screenText.Set(text2.TrailingSpace());
			if (this.allowedInCompetitive)
			{
				text = "COMPETITIVE IS FOR PLAYERS WHO WANT TO PLAY THE GAME AND TRY AS HARD AS THEY CAN.";
				LocalisationManager.TryGetKeyForCurrentLocale("COMPETITIVE_DESC", out text2, text);
				this.screenText.Append(text2.TrailingSpace());
				text = "PRESS OPTION 1 FOR DEFAULT, OPTION 2 FOR MINIGAMES, OR OPTION 3 FOR COMPETITIVE.";
				LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN_ALL_QUEUES", out text2, text);
				this.screenText.Append(text2);
			}
			else
			{
				text = "BEAT THE OBSTACLE COURSE IN CITY TO ALLOW COMPETITIVE PLAY.";
				LocalisationManager.TryGetKeyForCurrentLocale("BEAT_OBSTACLE_COURSE", out text2, text);
				this.screenText.Append(text2.TrailingSpace());
				text = "PRESS OPTION 1 FOR DEFAULT, OR OPTION 2 FOR MINIGAMES.";
				LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN_DEFAULT_QUEUES", out text2, text);
				this.screenText.Append(text2);
			}
			text = "\n\nCURRENT QUEUE:";
			LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_QUEUE", out text2, text);
			this.screenText.Append(text2.TrailingSpace());
			string text3 = this.currentQueue;
			string text4;
			if (!(text3 == "DEFAULT"))
			{
				if (text3 == "COMPETITIVE")
				{
					text4 = "COMPETITIVE_QUEUE";
					goto IL_0137;
				}
				if (text3 == "MINIGAMES")
				{
					text4 = "MINIGAMES_QUEUE";
					goto IL_0137;
				}
			}
			text4 = "DEFAULT_QUEUE";
			IL_0137:
			text = this.currentQueue;
			LocalisationManager.TryGetKeyForCurrentLocale(text4, out text2, text);
			this.screenText.Append(text2);
		}

		// Token: 0x06006C35 RID: 27701 RVA: 0x0022F0EC File Offset: 0x0022D2EC
		private void TroopScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			bool flag2 = this.IsValidTroopName(this.troopName);
			this.screenText.Set(string.Empty);
			string text = "";
			string text2;
			if (flag)
			{
				text2 = "PLAY WITH A PERSISTENT GROUP ACROSS MULTIPLE ROOMS.";
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_INTRO", out text, text2);
				this.screenText.Set(text);
				if (!flag2)
				{
					text2 = " PRESS ENTER TO JOIN OR CREATE A TROOP.";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_INSTRUCTIONS", out text, text2);
					this.screenText.Append(text);
				}
			}
			text2 = "\n\nCURRENT TROOP: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_CURRENT_TROOP", out text, text2);
			this.screenText.Append(text.TrailingSpace());
			if (flag2)
			{
				this.screenText.Append(this.troopName ?? "");
				if (flag)
				{
					bool flag3 = this.currentTroopPopulation > -1;
					if (this.troopQueueActive)
					{
						text2 = "\n  -IN TROOP QUEUE-";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_IN_QUEUE", out text, text2);
						this.screenText.Append(text);
						if (flag3)
						{
							text2 = "\n\nPLAYERS IN TROOP: ";
							LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_PLAYERS_IN_TROOP", out text, text2);
							this.screenText.Append(text.TrailingSpace());
							this.screenText.Append(Mathf.Max(1, this.currentTroopPopulation).ToString());
						}
						text2 = "\n\nPRESS OPTION 2 FOR DEFAULT QUEUE.";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_DEFAULT_QUEUE", out text, text2);
						this.screenText.Append(text);
					}
					else
					{
						text2 = "\n  -IN {currentQueue} QUEUE-";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_CURRENT_QUEUE", out text, text2);
						string text3 = this.currentQueue;
						string text4;
						if (!(text3 == "DEFAULT"))
						{
							if (text3 == "MINIGAMES")
							{
								text4 = "MINIGAMES_QUEUE";
								goto IL_0209;
							}
							if (text3 == "COMPETITIVE")
							{
								text4 = "COMPETITIVE_QUEUE";
								goto IL_0209;
							}
						}
						text4 = "DEFAULT_QUEUE";
						IL_0209:
						text2 = this.currentQueue;
						string text5;
						LocalisationManager.TryGetKeyForCurrentLocale(text4, out text5, text2);
						text = text.Replace("{currentQueue}", text5);
						this.screenText.Append(text);
						if (flag3)
						{
							text2 = "\n\nPLAYERS IN TROOP: ";
							LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_PLAYERS_IN_TROOP", out text, text2);
							this.screenText.Append(text.TrailingSpace());
							this.screenText.Append(Mathf.Max(1, this.currentTroopPopulation).ToString());
						}
						text2 = "\n\nPRESS OPTION 1 FOR TROOP QUEUE.";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_TROOP_QUEUE", out text, text2);
						this.screenText.Append(text);
					}
					text2 = "\nPRESS OPTION 3 TO LEAVE YOUR TROOP.";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_LEAVE", out text, text2);
					this.screenText.Append(text);
				}
			}
			else
			{
				text2 = "-NOT IN TROOP-";
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_NOT_IN_TROOP", out text, text2);
				this.screenText.Append(text);
			}
			if (flag)
			{
				if (!flag2)
				{
					text2 = "\n\nTROOP TO JOIN: ";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_JOIN_TROOP", out text, text2);
					this.screenText.Append(text.TrailingSpace());
					this.screenText.Append(this.troopToJoin);
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
				{
					this.TroopScreen_KIdProhibited();
					return;
				}
				this.TroopScreen_Permission();
			}
		}

		// Token: 0x06006C36 RID: 27702 RVA: 0x0022F44C File Offset: 0x0022D64C
		private void TurnScreen()
		{
			string text = "PRESS OPTION 1 TO USE SNAP TURN. PRESS OPTION 2 TO USE SMOOTH TURN. PRESS OPTION 3 TO USE NO ARTIFICIAL TURNING.";
			string text2 = "";
			string text3;
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN", out text3, text);
			text2 += text3.TrailingSpace();
			text = "PRESS THE NUMBER KEYS TO CHOOSE A TURNING SPEED.";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURNING_SPEED", out text3, text);
			text2 += text3;
			text = "\n CURRENT TURN TYPE: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURN_TYPE", out text3, text);
			text2 += text3;
			string text4 = "TURN_TYPE_NO_TURN";
			string turnType = GorillaSnapTurn.CachedSnapTurnRef.turnType;
			if (!(turnType == "SNAP"))
			{
				if (!(turnType == "SMOOTH"))
				{
					if (!(turnType == "NONE"))
					{
						Debug.LogError("[LOCALIZATION::GORILLA_COMPUTER::TURN] Could not match [" + GorillaSnapTurn.CachedSnapTurnRef.turnType + "] to any case. Defaulting to NO_TURN");
					}
					else
					{
						text4 = "TURN_TYPE_NO_TURN";
					}
				}
				else
				{
					text4 = "TURN_TYPE_SMOOTH_TURN";
				}
			}
			else
			{
				text4 = "TURN_TYPE_SNAP_TURN";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(text4, out text3, GorillaSnapTurn.CachedSnapTurnRef.turnType);
			text2 += text3;
			text = "\nCURRENT TURN SPEED: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURN_SPEED", out text3, text);
			text2 += text3;
			text2 += GorillaSnapTurn.CachedSnapTurnRef.turnFactor.ToString();
			this.screenText.Set(text2);
		}

		// Token: 0x06006C37 RID: 27703 RVA: 0x0022F58C File Offset: 0x0022D78C
		private void NameScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				string text = "PRESS ENTER TO CHANGE YOUR NAME TO THE ENTERED NEW NAME.\n\n";
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN", out text2, text);
				this.screenText.Set(text2);
				text = "CURRENT NAME: ";
				LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_NAME", out text2, text);
				this.screenText.Append(text2.TrailingSpace());
				this.screenText.Append(this.savedName);
				if (this.NametagsEnabled)
				{
					text = "NEW NAME: ";
					LocalisationManager.TryGetKeyForCurrentLocale("NEW_NAME", out text2, text);
					this.screenText.Append(text2.TrailingSpace());
					this.screenText.Append(this.currentName);
				}
				text = "PRESS OPTION 1 TO TOGGLE NAMETAGS.\nCURRENTLY NAMETAGS ARE: ";
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_TOGGLE_NAMETAGS", out text2, text);
				string text3 = (this.NametagsEnabled ? "ON_KEY" : "OFF_KEY");
				this.screenText.Append(text2.TrailingSpace());
				text = (this.NametagsEnabled ? "ON" : "OFF");
				LocalisationManager.TryGetKeyForCurrentLocale(text3, out text2, text);
				this.screenText.Append(text2);
				return;
			}
			if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.NameScreen_KIdProhibited();
				return;
			}
			this.NameScreen_Permission();
		}

		// Token: 0x06006C38 RID: 27704 RVA: 0x0022F6BC File Offset: 0x0022D8BC
		private void StartupScreen()
		{
			string text = string.Empty;
			if (KIDManager.GetActiveAccountStatus() == AgeStatusType.DIGITALMINOR)
			{
				text = "YOU ARE PLAYING ON A MANAGED ACCOUNT. SOME SETTINGS MAY BE DISABLED WITHOUT PARENT OR GUARDIAN APPROVAL\n\n";
				string text2;
				if (LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_MANAGED", out text2, text))
				{
					text = text2;
				}
			}
			string empty = string.Empty;
			string text3;
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_INTRO", out text3, "GORILLA OS\n\n");
			this.screenText.Set(text3);
			this.screenText.Append(text);
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_PLAYERS_ONLINE", out text3, "{playersOnline} PLAYERS ONLINE\n\n");
			this.screenText.Append(text3.Replace("{playersOnline}", HowManyMonke.ThisMany.ToString()));
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_USERS_BANNED", out text3, "{usersBanned} USERS BANNED YESTERDAY\n\n");
			this.screenText.Append(text3.Replace("{usersBanned}", this.usersBanned.ToString()));
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_PRESS_KEY", out text3, "PRESS ANY KEY TO BEGIN");
			this.screenText.Append(text3);
		}

		// Token: 0x06006C39 RID: 27705 RVA: 0x0022F7A4 File Offset: 0x0022D9A4
		private void ColourScreen()
		{
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_SELECT_INTRO", out text, "USE THE OPTIONS BUTTONS TO SELECT THE COLOR TO UPDATE, THEN PRESS 0-9 TO SET A NEW VALUE.");
			this.screenText.Set(text);
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_RED", out text, "RED");
			this.screenText.Append("\n\n");
			this.screenText.Append(text);
			this.screenText.Append(Mathf.FloorToInt(this.redValue * 9f).ToString() + ((this.colorCursorLine == 0) ? "<--" : ""));
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_GREEN", out text, "GREEN");
			this.screenText.Append("\n\n");
			this.screenText.Append(text);
			this.screenText.Append(Mathf.FloorToInt(this.greenValue * 9f).ToString() + ((this.colorCursorLine == 1) ? "<--" : ""));
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_BLUE", out text, "BLUE");
			this.screenText.Append("\n\n");
			this.screenText.Append(text);
			this.screenText.Append(Mathf.FloorToInt(this.blueValue * 9f).ToString() + ((this.colorCursorLine == 2) ? "<--" : ""));
		}

		// Token: 0x06006C3A RID: 27706 RVA: 0x0022F914 File Offset: 0x0022DB14
		private void RoomScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Multiplayer, null).Item2;
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer) && item;
			this.screenText.Set("");
			string text = "";
			string text2;
			if (flag)
			{
				text2 = "PRESS ENTER TO JOIN OR CREATE A CUSTOM ROOM WITH THE ENTERED CODE.";
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_INTRO", out text, text2);
				this.screenText.Append(text.TrailingSpace());
			}
			text2 = "PRESS OPTION 1 TO DISCONNECT FROM THE CURRENT ROOM.";
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_OPTION", out text, text2);
			this.screenText.Append(text.TrailingSpace());
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider, false))
				{
					text2 = "\nYOUR GROUP WILL TRAVEL WITH YOU.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_GROUP_TRAVEL", out text, text2);
					this.screenText.Append(text.TrailingSpace());
				}
				else
				{
					text2 = "\n<color=red>YOU WILL LEAVE YOUR PARTY UNLESS YOU GATHER THEM HERE FIRST!</color> ";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_PARTY_WARNING", out text, text2);
					this.screenText.Append(text);
				}
			}
			text2 = "\n\nCURRENT ROOM:";
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_TEXT_CURRENT_ROOM", out text, text2);
			this.screenText.Append(text.TrailingSpace());
			if (NetworkSystem.Instance.InRoom)
			{
				this.screenText.Append(this.GetVStumpRoomDisplayName(NetworkSystem.Instance.RoomName).TrailingSpace());
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
					string text3 = ((activeGameMode != null) ? activeGameMode.GameModeNameRoomLabel() : null);
					if (!string.IsNullOrEmpty(text3))
					{
						this.screenText.Append(text3 ?? "");
					}
				}
				text2 = "\n\nPLAYERS IN ROOM:";
				LocalisationManager.TryGetKeyForCurrentLocale("PLAYERS_IN_ROOM", out text, text2);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(NetworkSystem.Instance.RoomPlayerCount.ToString());
			}
			else
			{
				text2 = "-NOT IN ROOM-";
				LocalisationManager.TryGetKeyForCurrentLocale("NOT_IN_ROOM", out text, text2);
				this.screenText.Append(text);
				text2 = "\n\nPLAYERS ONLINE:";
				LocalisationManager.TryGetKeyForCurrentLocale("PLAYERS_ONLINE", out text, text2);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(HowManyMonke.ThisMany.ToString());
			}
			if (flag)
			{
				text2 = "\n\nROOM TO JOIN:";
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_TO_JOIN", out text, text2);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(this.GetVStumpRoomDisplayName(this.roomToJoin));
				if (this.roomFull)
				{
					text2 = "\n\nROOM FULL. JOIN ROOM FAILED.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_FULL", out text, text2);
					this.screenText.Append(text);
					return;
				}
				if (this.roomNotAllowed)
				{
					text2 = "\n\nCANNOT JOIN ROOM TYPE FROM HERE.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_JOIN_NOT_ALLOWED", out text, text2);
					this.screenText.Append(text);
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
				{
					this.RoomScreen_KIdProhibited();
					return;
				}
				this.RoomScreen_Permission();
			}
		}

		// Token: 0x06006C3B RID: 27707 RVA: 0x0022FC28 File Offset: 0x0022DE28
		private void RedemptionScreen()
		{
			string text = "TYPE REDEMPTION CODE AND PRESS ENTER";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_INTRO", out text2, text);
			this.screenText.Set(text2);
			text = "\n\nCODE: " + this.redemptionCode;
			LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_LABEL", out text2, text);
			this.screenText.Append(text2.TrailingSpace());
			this.screenText.Append(this.redemptionCode);
			switch (this.RedemptionStatus)
			{
			case GorillaComputer.RedemptionResult.Empty:
				break;
			case GorillaComputer.RedemptionResult.Invalid:
				text = "\n\nINVALID CODE";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_INVALID", out text2, text);
				this.screenText.Append(text2);
				return;
			case GorillaComputer.RedemptionResult.Checking:
				text = "\n\nVALIDATING...";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_VALIDATING", out text2, text);
				this.screenText.Append(text2);
				return;
			case GorillaComputer.RedemptionResult.AlreadyUsed:
				text = "\n\nCODE ALREADY CLAIMED";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_ALREADY_USED", out text2, text);
				this.screenText.Append(text2);
				return;
			case GorillaComputer.RedemptionResult.TooEarly:
				text = "CODE IS NOT REDEEMABLE UNTIL";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_TOO_EARLY", out text2, text);
				this.screenText.Append((this.RedemptionRestrictionTime != null) ? ("\n\n" + text2 + "\n" + this.RedemptionRestrictionTime.Value.ToLocalTime().ToString("f").ToUpper()) : ("\n\n" + text2 + "\n[MISSING]"));
				return;
			case GorillaComputer.RedemptionResult.TooLate:
				text = "CODE EXPIRED";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_TOO_LATE", out text2, text);
				this.screenText.Append((this.RedemptionRestrictionTime != null) ? ("\n\n" + text2 + "\n" + this.RedemptionRestrictionTime.Value.ToLocalTime().ToString("f").ToUpper()) : ("\n\n" + text2 + "\n[MISSING]"));
				return;
			case GorillaComputer.RedemptionResult.AlreadyGranted:
				text = "ITEM ALREADY GRANTED";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_ALREADY_GRANTED", out text2, text);
				this.screenText.Append("\n\n" + text2);
				return;
			case GorillaComputer.RedemptionResult.Success:
				text = "\n\nSUCCESSFULLY CLAIMED!";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_SUCCESS", out text2, text);
				this.screenText.Append(text2);
				break;
			default:
				return;
			}
		}

		// Token: 0x06006C3C RID: 27708 RVA: 0x0022FE6C File Offset: 0x0022E06C
		private void LimitedOnlineFunctionalityScreen()
		{
			string text = "NOT AVAILABLE IN RANKED PLAY";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("LIMITED_ONLINE_FUNC", out text2, text);
			this.screenText.Set(text2);
		}

		// Token: 0x06006C3D RID: 27709 RVA: 0x0022FE9C File Offset: 0x0022E09C
		private void UpdateGameModeText()
		{
			string text = "CURRENT MODE";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_MODE", out text2, text);
			this.currentGameModeText.Value = text2;
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				text = "-NOT IN ROOM-";
				LocalisationManager.TryGetKeyForCurrentLocale("NOT_IN_ROOM", out text2, text);
				WatchableStringSO watchableStringSO = this.currentGameModeText;
				watchableStringSO.Value += text2;
				return;
			}
			WatchableStringSO watchableStringSO2 = this.currentGameModeText;
			watchableStringSO2.Value = watchableStringSO2.Value + "\n" + GorillaGameManager.instance.GameModeName();
		}

		// Token: 0x06006C3E RID: 27710 RVA: 0x0022FF33 File Offset: 0x0022E133
		private void UpdateFunctionScreen()
		{
			this.functionSelectText.Set(this.GetOrderListForScreen(this.currentState));
		}

		// Token: 0x06006C3F RID: 27711 RVA: 0x0022FF4C File Offset: 0x0022E14C
		private void CheckAutoBanListForRoomName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.CheckForBadRoomName(nameToCheck);
		}

		// Token: 0x06006C40 RID: 27712 RVA: 0x0022FF5B File Offset: 0x0022E15B
		private void CheckAutoBanListForPlayerName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.CheckForBadPlayerName(nameToCheck);
		}

		// Token: 0x06006C41 RID: 27713 RVA: 0x0022FF6A File Offset: 0x0022E16A
		private void CheckAutoBanListForTroopName(string nameToCheck)
		{
			if (this.IsValidTroopName(this.troopToJoin))
			{
				this.SwitchToLoadingState();
				this.CheckForBadTroopName(nameToCheck);
			}
		}

		// Token: 0x06006C42 RID: 27714 RVA: 0x0022FF87 File Offset: 0x0022E187
		private void CheckForBadRoomName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = true,
				forTroop = false
			}, new Action<ExecuteFunctionResult>(this.OnRoomNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x06006C43 RID: 27715 RVA: 0x0022FFC7 File Offset: 0x0022E1C7
		private void CheckForBadPlayerName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = false,
				forTroop = false
			}, new Action<ExecuteFunctionResult>(this.OnPlayerNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x06006C44 RID: 27716 RVA: 0x00230007 File Offset: 0x0022E207
		private void CheckForBadTroopName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = false,
				forTroop = true
			}, new Action<ExecuteFunctionResult>(this.OnTroopNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x06006C45 RID: 27717 RVA: 0x00230048 File Offset: 0x0022E248
		private void OnRoomNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					if (FriendshipGroupDetection.Instance.IsInParty && !FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider, false))
					{
						FriendshipGroupDetection.Instance.LeaveParty();
					}
					if (this.playerInVirtualStump)
					{
						if (CustomMapManager.IsInFeaturedMode())
						{
							CustomMapManager.PrepareFeaturedMapReloadOnRoomChange();
						}
						CustomMapManager.UnloadMap(false);
					}
					this.networkController.AttemptToJoinSpecificRoom(this.roomToJoin, FriendshipGroupDetection.Instance.IsInParty ? JoinType.ForceJoinWithParty : JoinType.Solo);
					break;
				case 1:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.VStumpRoomFullPrepend : "");
					this.SwitchToWarningState();
					break;
				case 2:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.VStumpRoomFullPrepend : "");
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x06006C46 RID: 27718 RVA: 0x0023017C File Offset: 0x0022E37C
		private void OnPlayerNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					NetworkSystem.Instance.SetMyNickName(this.currentName);
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					break;
				case 1:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					this.SwitchToWarningState();
					break;
				case 2:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			this.SetLocalNameTagText(this.currentName);
			this.savedName = this.currentName;
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { this.redValue, this.greenValue, this.blueValue });
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x06006C47 RID: 27719 RVA: 0x002302B8 File Offset: 0x0022E4B8
		private void OnTroopNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					this.JoinTroop(this.troopToJoin);
					break;
				case 1:
					this.troopToJoin = string.Empty;
					this.SwitchToWarningState();
					break;
				case 2:
					this.troopToJoin = string.Empty;
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x06006C48 RID: 27720 RVA: 0x0023033F File Offset: 0x0022E53F
		private void OnErrorNameCheck(PlayFabError error)
		{
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
			GorillaComputer.OnErrorShared(error);
		}

		// Token: 0x06006C49 RID: 27721 RVA: 0x00230358 File Offset: 0x0022E558
		public bool CheckAutoBanListForName(string nameToCheck)
		{
			nameToCheck = nameToCheck.ToLower();
			nameToCheck = new string(Array.FindAll<char>(nameToCheck.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
			foreach (string text in this.anywhereTwoWeek)
			{
				if (nameToCheck.IndexOf(text) >= 0)
				{
					return false;
				}
			}
			foreach (string text2 in this.anywhereOneWeek)
			{
				if (nameToCheck.IndexOf(text2) >= 0 && !nameToCheck.Contains("fagol"))
				{
					return false;
				}
			}
			string[] array = this.exactOneWeek;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == nameToCheck)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06006C4A RID: 27722 RVA: 0x00230418 File Offset: 0x0022E618
		public void UpdateColor(float red, float green, float blue)
		{
			this.redValue = Mathf.Clamp(red, 0f, 1f);
			this.greenValue = Mathf.Clamp(green, 0f, 1f);
			this.blueValue = Mathf.Clamp(blue, 0f, 1f);
		}

		// Token: 0x06006C4B RID: 27723 RVA: 0x00230467 File Offset: 0x0022E667
		public void UpdateFailureText(string failMessage)
		{
			GorillaScoreboardTotalUpdater.instance.SetOfflineFailureText(failMessage);
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.EnableFailedState(failMessage);
			this.functionSelectText.EnableFailedState(failMessage);
		}

		// Token: 0x06006C4C RID: 27724 RVA: 0x00230498 File Offset: 0x0022E698
		private void RestoreFromFailureState()
		{
			GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.DisableFailedState();
			this.functionSelectText.DisableFailedState();
		}

		// Token: 0x06006C4D RID: 27725 RVA: 0x002304C6 File Offset: 0x0022E6C6
		public void GeneralFailureMessage(string failMessage)
		{
			this.isConnectedToMaster = false;
			NetworkSystem.Instance.SetWrongVersion();
			this.UpdateFailureText(failMessage);
			this.UpdateScreen();
		}

		// Token: 0x06006C4E RID: 27726 RVA: 0x002304E8 File Offset: 0x0022E6E8
		private static void OnErrorShared(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			else if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
			if (error.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						GorillaComputer.instance.GeneralFailureMessage(string.Concat(new string[]
						{
							"YOUR ACCOUNT ",
							PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
							" HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: ",
							keyValuePair.Key,
							"\nHOURS LEFT: ",
							((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()
						}));
						return;
					}
					GorillaComputer.instance.GeneralFailureMessage("YOUR ACCOUNT " + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + " HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (error.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value[0] != "Indefinite")
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						}
						else
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
						}
					}
				}
			}
		}

		// Token: 0x06006C4F RID: 27727 RVA: 0x00230740 File Offset: 0x0022E940
		private void DecreaseState()
		{
			this.currentStateIndex--;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex--;
			}
			if (this.currentStateIndex < 0)
			{
				this.currentStateIndex = this.FunctionsCount - 1;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06006C50 RID: 27728 RVA: 0x002307A4 File Offset: 0x0022E9A4
		private void IncreaseState()
		{
			this.currentStateIndex++;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex++;
			}
			if (this.currentStateIndex >= this.FunctionsCount)
			{
				this.currentStateIndex = 0;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06006C51 RID: 27729 RVA: 0x00230808 File Offset: 0x0022EA08
		public GorillaComputer.ComputerState GetState(int index)
		{
			GorillaComputer.ComputerState computerState;
			try
			{
				computerState = this._activeOrderList[index].State;
			}
			catch
			{
				computerState = this._activeOrderList[0].State;
			}
			return computerState;
		}

		// Token: 0x06006C52 RID: 27730 RVA: 0x00230850 File Offset: 0x0022EA50
		public int GetStateIndex(GorillaComputer.ComputerState state)
		{
			return this._activeOrderList.FindIndex((GorillaComputer.StateOrderItem s) => s.State == state);
		}

		// Token: 0x06006C53 RID: 27731 RVA: 0x00230884 File Offset: 0x0022EA84
		public string GetOrderListForScreen(GorillaComputer.ComputerState currentState)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int stateIndex = this.GetStateIndex(currentState);
			for (int i = 0; i < this.FunctionsCount; i++)
			{
				stringBuilder.Append(this.FunctionNames[i]);
				if (i == stateIndex)
				{
					stringBuilder.Append(this.Pointer);
				}
				if (i < this.FunctionsCount - 1)
				{
					stringBuilder.Append("\n");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06006C54 RID: 27732 RVA: 0x002308F1 File Offset: 0x0022EAF1
		private void GetCurrentTime()
		{
			this.tryGetTimeAgain = true;
			PlayFabClientAPI.GetTime(new GetTimeRequest(), new Action<GetTimeResult>(this.OnGetTimeSuccess), new Action<PlayFabError>(this.OnGetTimeFailure), null, null);
		}

		// Token: 0x06006C55 RID: 27733 RVA: 0x00230920 File Offset: 0x0022EB20
		private void OnGetTimeSuccess(GetTimeResult result)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(result.Time.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = result.Time - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated == null)
			{
				return;
			}
			onServerTimeUpdated();
		}

		// Token: 0x06006C56 RID: 27734 RVA: 0x00230988 File Offset: 0x0022EB88
		private void OnGetTimeFailure(PlayFabError error)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = DateTime.UtcNow - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated != null)
			{
				onServerTimeUpdated();
			}
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
		}

		// Token: 0x06006C57 RID: 27735 RVA: 0x00230A1B File Offset: 0x0022EC1B
		private void PlayerCountChangedCallback(NetPlayer player)
		{
			this.UpdateScreen();
		}

		// Token: 0x06006C58 RID: 27736 RVA: 0x00230A23 File Offset: 0x0022EC23
		private static void OnFirstJoinedRoom_IncrementSessionCount()
		{
			RoomSystem.JoinedRoomEvent -= new Action(GorillaComputer.OnFirstJoinedRoom_IncrementSessionCount);
			GorillaComputer.sessionCount++;
			PlayerPrefs.SetInt("sessionCount", GorillaComputer.sessionCount);
			PlayerPrefs.Save();
		}

		// Token: 0x06006C59 RID: 27737 RVA: 0x00230A60 File Offset: 0x0022EC60
		public void SetNameBySafety(bool isSafety)
		{
			if (!isSafety)
			{
				return;
			}
			PlayerPrefs.SetString("playerNameBackup", this.currentName);
			this.currentName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			this.savedName = this.currentName;
			NetworkSystem.Instance.SetMyNickName(this.currentName);
			this.SetLocalNameTagText(this.currentName);
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { this.redValue, this.greenValue, this.blueValue });
			}
		}

		// Token: 0x06006C5A RID: 27738 RVA: 0x00230B3A File Offset: 0x0022ED3A
		public void SetLocalNameTagText(string newName)
		{
			VRRig.LocalRig.SetNameTagText(newName);
		}

		// Token: 0x06006C5B RID: 27739 RVA: 0x00230B48 File Offset: 0x0022ED48
		public void SetComputerSettingsBySafety(bool isSafety, GorillaComputer.ComputerState[] toFilterOut, bool shouldHide)
		{
			this._activeOrderList = this.OrderList;
			if (!isSafety)
			{
				this._activeOrderList = this.OrderList;
				if (this._filteredStates.Count > 0 && toFilterOut.Length != 0)
				{
					for (int i = 0; i < toFilterOut.Length; i++)
					{
						if (this._filteredStates.Contains(toFilterOut[i]))
						{
							this._filteredStates.Remove(toFilterOut[i]);
						}
					}
				}
			}
			else if (shouldHide)
			{
				for (int j = 0; j < toFilterOut.Length; j++)
				{
					if (!this._filteredStates.Contains(toFilterOut[j]))
					{
						this._filteredStates.Add(toFilterOut[j]);
					}
				}
			}
			if (this._filteredStates.Count > 0)
			{
				int k = 0;
				int num = this._activeOrderList.Count;
				while (k < num)
				{
					if (this._filteredStates.Contains(this._activeOrderList[k].State))
					{
						this._activeOrderList.RemoveAt(k);
						k--;
						num--;
					}
					k++;
				}
			}
			this.FunctionsCount = this._activeOrderList.Count;
			this.FunctionNames.Clear();
			this._activeOrderList.ForEach(delegate(GorillaComputer.StateOrderItem s)
			{
				string name = s.GetName();
				if (name.Length > this.highestCharacterCount)
				{
					this.highestCharacterCount = name.Length;
				}
				this.FunctionNames.Add(name);
			});
			for (int l = 0; l < this.FunctionsCount; l++)
			{
				int num2 = this.highestCharacterCount - this.FunctionNames[l].Length;
				for (int m = 0; m < num2; m++)
				{
					List<string> functionNames = this.FunctionNames;
					int num3 = l;
					functionNames[num3] += " ";
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x06006C5C RID: 27740 RVA: 0x00230CDA File Offset: 0x0022EEDA
		public void KID_SetVoiceChatSettingOnStart(bool voiceChatEnabled, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				return;
			}
			this.SetVoice(voiceChatEnabled, !hasOptedInPreviously);
		}

		// Token: 0x06006C5D RID: 27741 RVA: 0x00230CEC File Offset: 0x0022EEEC
		private void SetVoice(bool setting, bool saveSetting = true)
		{
			this.voiceChatOn = (setting ? "TRUE" : "FALSE");
			if (setting && !KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, null).Item2)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Voice_Chat, true);
				KIDManager.SendOptInPermissions();
			}
			if (!saveSetting)
			{
				return;
			}
			PlayerPrefs.SetString("voiceChatOn", this.voiceChatOn);
			PlayerPrefs.Save();
		}

		// Token: 0x06006C5E RID: 27742 RVA: 0x00230D45 File Offset: 0x0022EF45
		public bool CheckVoiceChatEnabled()
		{
			return this.voiceChatOn == "TRUE";
		}

		// Token: 0x06006C5F RID: 27743 RVA: 0x00230D58 File Offset: 0x0022EF58
		private void SetVoiceChatBySafety(bool voiceChatEnabled, Permission.ManagedByEnum managedBy)
		{
			bool flag = !voiceChatEnabled;
			this.SetComputerSettingsBySafety(flag, new GorillaComputer.ComputerState[]
			{
				GorillaComputer.ComputerState.Voice,
				GorillaComputer.ComputerState.AutoMute,
				GorillaComputer.ComputerState.Mic
			}, false);
			string text = PlayerPrefs.GetString("voiceChatOn", "");
			if (KIDManager.KidEnabledAndReady)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
				if (permissionDataByFeature != null)
				{
					ValueTuple<bool, bool> valueTuple = KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, permissionDataByFeature);
					if (valueTuple.Item1 && !valueTuple.Item2)
					{
						text = "FALSE";
					}
				}
				else
				{
					Debug.LogErrorFormat("[KID] Could not find permission data for [" + EKIDFeatures.Voice_Chat.ToStandardisedString() + "]", Array.Empty<object>());
				}
			}
			switch (managedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (string.IsNullOrEmpty(text))
				{
					this.voiceChatOn = (voiceChatEnabled ? "TRUE" : "FALSE");
				}
				else
				{
					this.voiceChatOn = text;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).Enabled)
				{
					if (string.IsNullOrEmpty(text))
					{
						this.voiceChatOn = "TRUE";
					}
					else
					{
						this.voiceChatOn = text;
					}
				}
				else
				{
					this.voiceChatOn = "FALSE";
				}
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.voiceChatOn = "FALSE";
				break;
			}
			RigContainer.RefreshAllRigVoices();
			Debug.Log("[KID] On Session Update - Voice Chat Permission changed - Has enabled voiceChat? [" + voiceChatEnabled.ToString() + "]");
		}

		// Token: 0x06006C60 RID: 27744 RVA: 0x00230E84 File Offset: 0x0022F084
		public void SetNametagSetting(bool setting, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				return;
			}
			if (managedBy == Permission.ManagedByEnum.GUARDIAN)
			{
				int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, 1);
				setting = setting && @int == 1;
				this.UpdateNametagSetting(setting, false);
				return;
			}
			setting = PlayerPrefs.GetInt(this.NameTagPlayerPref, setting ? 1 : 0) == 1;
			this.UpdateNametagSetting(setting, !hasOptedInPreviously && setting);
		}

		// Token: 0x06006C61 RID: 27745 RVA: 0x00230EE0 File Offset: 0x0022F0E0
		public static void RegisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Combine(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		// Token: 0x06006C62 RID: 27746 RVA: 0x00230EF7 File Offset: 0x0022F0F7
		public static void UnregisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Remove(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		// Token: 0x06006C63 RID: 27747 RVA: 0x00230F10 File Offset: 0x0022F110
		private void UpdateNametagSetting(bool newSettingValue, bool saveSetting = true)
		{
			if (newSettingValue)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Custom_Nametags, true);
			}
			this.NametagsEnabled = newSettingValue;
			NetworkSystem.Instance.SetMyNickName(this.NametagsEnabled ? this.savedName : NetworkSystem.Instance.GetMyDefaultName());
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { this.redValue, this.greenValue, this.blueValue });
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action != null)
			{
				action(this.NametagsEnabled);
			}
			if (!saveSetting)
			{
				return;
			}
			int num = (this.NametagsEnabled ? 1 : 0);
			PlayerPrefs.SetInt(this.NameTagPlayerPref, num);
			PlayerPrefs.Save();
		}

		// Token: 0x06006C64 RID: 27748 RVA: 0x00230FDB File Offset: 0x0022F1DB
		public void SetInVirtualStump(bool inVirtualStump)
		{
			GrowingSnowballThrowable.ForceAOEEnabled = inVirtualStump;
			this.playerInVirtualStump = inVirtualStump;
			this.roomToJoin = (this.playerInVirtualStump ? (this.VStumpRoomFullPrepend + this.roomToJoin) : this.StripVStumpRoomPrefix(this.roomToJoin));
		}

		// Token: 0x06006C65 RID: 27749 RVA: 0x00231017 File Offset: 0x0022F217
		public bool IsPlayerInVirtualStump()
		{
			return this.playerInVirtualStump;
		}

		// Token: 0x06006C66 RID: 27750 RVA: 0x0023101F File Offset: 0x0022F21F
		public void SetLimitOnlineScreens(bool isLimited)
		{
			this.limitOnlineScreens = isLimited;
			this.UpdateScreen();
		}

		// Token: 0x06006C67 RID: 27751 RVA: 0x0023102E File Offset: 0x0022F22E
		private void InitializeKIdState()
		{
			KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		// Token: 0x06006C68 RID: 27752 RVA: 0x00231041 File Offset: 0x0022F241
		private void UpdateKidState()
		{
			this._currentScreentState = GorillaComputer.EKidScreenState.Ready;
		}

		// Token: 0x06006C69 RID: 27753 RVA: 0x0023104A File Offset: 0x0022F24A
		private void RequestUpdatedPermissions()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				return;
			}
			if (Time.realtimeSinceStartup < this._nextUpdateAttemptTime)
			{
				return;
			}
			this._waitingForUpdatedSession = true;
			this.UpdateSession();
		}

		// Token: 0x06006C6A RID: 27754 RVA: 0x00231078 File Offset: 0x0022F278
		private async void UpdateSession()
		{
			this._nextUpdateAttemptTime = Time.realtimeSinceStartup + this._updateAttemptCooldown;
			await KIDManager.UpdateSession(null);
			this._waitingForUpdatedSession = false;
		}

		// Token: 0x06006C6B RID: 27755 RVA: 0x002310AF File Offset: 0x0022F2AF
		private void OnSessionUpdate_GorillaComputer()
		{
			this.UpdateKidState();
			this.UpdateScreen();
		}

		// Token: 0x06006C6C RID: 27756 RVA: 0x002310BD File Offset: 0x0022F2BD
		private void ProcessScreen_SetupKID()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				Debug.LogError("[KID] Unable to start k-ID Flow. Kid is disabled");
				return;
			}
		}

		// Token: 0x06006C6D RID: 27757 RVA: 0x002310D4 File Offset: 0x0022F2D4
		private bool GuardianConsentMessage(string setupKIDButtonName, string featureDescription)
		{
			string text = "PARENT/GUARDIAN PERMISSION REQUIRED TO ";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("KID_PERMISSION_NEEDED", out text2, text);
			this.screenText.Append(text2);
			this.screenText.Append(featureDescription + "!");
			if (this._waitingForUpdatedSession)
			{
				text = "\n\nWAITING FOR PARENT/GUARDIAN CONSENT!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_WAITING_PERMISSION", out text2, text);
				this.screenText.Append(text2);
				return true;
			}
			if (Time.realtimeSinceStartup >= this._nextUpdateAttemptTime)
			{
				text = "\n\nPRESS OPTION 2 TO REFRESH PERMISSIONS!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_REFRESH_PERMISSIONS", out text2, text);
				this.screenText.Append(text2);
			}
			else
			{
				text = "CHECK AGAIN IN {time} SECONDS!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_CHECK_AGAIN_COOLDOWN", out text2, text);
				text2 = text2.Replace("{time}", ((int)(this._nextUpdateAttemptTime - Time.realtimeSinceStartup)).ToString());
				this.screenText.Append(text2);
			}
			return false;
		}

		// Token: 0x06006C6E RID: 27758 RVA: 0x002311B4 File Offset: 0x0022F3B4
		private void ProhibitedMessage(string verb)
		{
			"\n\nYOU ARE NOT ALLOWED TO " + verb + " IN YOUR JURISDICTION.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("KID_PROHIBITED_MESSAGE", out text, "SET CUSTOM NICKNAMES");
			text = text.Replace("{verb}", verb);
			this.screenText.Append(text);
		}

		// Token: 0x06006C6F RID: 27759 RVA: 0x00231200 File Offset: 0x0022F400
		private void RoomScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				string text = "YOU CANNOT USE THE PRIVATE ROOM FEATURE RIGHT NOW";
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_DISABLED", out text2, text);
				this.screenText.Set(text2);
				return;
			}
			this.screenText.Set("");
			string text3 = "CREATE OR JOIN PRIVATE ROOMS";
			string text4;
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_KID_PROHIBITED_VERB", out text4, text3);
			this.GuardianConsentMessage("OPTION 3", text4);
		}

		// Token: 0x06006C70 RID: 27760 RVA: 0x00231268 File Offset: 0x0022F468
		private void RoomScreen_KIdProhibited()
		{
			string text = "CREATE OR JOIN PRIVATE ROOMS";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_KID_PROHIBITED_VERB", out text2, text);
			this.ProhibitedMessage(text2);
		}

		// Token: 0x06006C71 RID: 27761 RVA: 0x00231290 File Offset: 0x0022F490
		private void VoiceScreen_Permission()
		{
			string text = "VOICE TYPE: \"MONKE\"\n\n";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_KID_CURRENT_VOICE", out text2, text);
			this.screenText.Set(text2);
			if (!KIDManager.KidEnabled)
			{
				text = "YOU CANNOT USE THE HUMAN VOICE TYPE FEATURE RIGHT NOW";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_DISABLED", out text2, text);
				this.screenText.Append(text2);
				return;
			}
			text = "ENABLE HUMAN VOICE CHAT";
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_GUARDIAN_FEATURE_DESC", out text2, text);
			this.GuardianConsentMessage("OPTION 3", text2);
		}

		// Token: 0x06006C72 RID: 27762 RVA: 0x00231308 File Offset: 0x0022F508
		private void VoiceScreen_KIdProhibited()
		{
			string text = "USE THE VOICE CHAT";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_KID_PROHIBITED_VERB", out text2, text);
			this.ProhibitedMessage(text2);
		}

		// Token: 0x06006C73 RID: 27763 RVA: 0x00231330 File Offset: 0x0022F530
		private void MicScreen_Permission()
		{
			this.screenText.Set("");
			string text = "ENABLE HUMAN VOICE CHAT";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_GUARDIAN_FEATURE_DESC", out text2, text);
			this.GuardianConsentMessage("OPTION 3", text2);
		}

		// Token: 0x06006C74 RID: 27764 RVA: 0x0023136E File Offset: 0x0022F56E
		private void MicScreen_KIdProhibited()
		{
			this.VoiceScreen_KIdProhibited();
		}

		// Token: 0x06006C75 RID: 27765 RVA: 0x00231378 File Offset: 0x0022F578
		private void NameScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				string text = "YOU CANNOT USE THE CUSTOM NICKNAME FEATURE RIGHT NOW";
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_DISABLED", out text2, text);
				this.screenText.Append(text2);
				return;
			}
			this.screenText.Set("");
			string text3;
			LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_KID_PROHIBITED_VERB", out text3, "SET CUSTOM NICKNAMES");
			this.GuardianConsentMessage("OPTION 3", text3);
		}

		// Token: 0x06006C76 RID: 27766 RVA: 0x002313DC File Offset: 0x0022F5DC
		private void NameScreen_KIdProhibited()
		{
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_KID_PROHIBITED_VERB", out text, "SET CUSTOM NICKNAMES");
			this.ProhibitedMessage(text);
		}

		// Token: 0x06006C77 RID: 27767 RVA: 0x00231404 File Offset: 0x0022F604
		private void OnKIDSessionUpdated_CustomNicknames(bool showCustomNames, Permission.ManagedByEnum managedBy)
		{
			bool flag = (showCustomNames || managedBy == Permission.ManagedByEnum.PLAYER) && managedBy != Permission.ManagedByEnum.PROHIBITED;
			this.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[] { GorillaComputer.ComputerState.Name }, false);
			int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, -1);
			bool flag2 = @int > 0;
			switch (managedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (showCustomNames)
				{
					this.NametagsEnabled = @int == -1 || flag2;
				}
				else
				{
					this.NametagsEnabled = @int != -1 && flag2;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				this.NametagsEnabled = showCustomNames && (flag2 || @int == -1);
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.NametagsEnabled = false;
				break;
			}
			if (this.NametagsEnabled)
			{
				NetworkSystem.Instance.SetMyNickName(this.savedName);
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action == null)
			{
				return;
			}
			action(this.NametagsEnabled);
		}

		// Token: 0x06006C78 RID: 27768 RVA: 0x002314D0 File Offset: 0x0022F6D0
		private void TroopScreen_Permission()
		{
			this.screenText.Set("");
			if (!KIDManager.KidEnabled)
			{
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_DISABLED", out text, "YOU CANNOT USE THE TROOPS FEATURE RIGHT NOW");
				this.screenText.Append(text);
				return;
			}
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_KID_DESC", out text2, "JOIN TROOPS");
			this.GuardianConsentMessage("OPTION 3", text2);
		}

		// Token: 0x06006C79 RID: 27769 RVA: 0x00231534 File Offset: 0x0022F734
		private void TroopScreen_KIdProhibited()
		{
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_KID_PROHIBITED_VERB", out text, "CREATE OR JOIN TROOPS");
			this.ProhibitedMessage(text);
		}

		// Token: 0x06006C7A RID: 27770 RVA: 0x0023155A File Offset: 0x0022F75A
		private void ProcessKIdState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.option1 && this._currentScreentState == GorillaComputer.EKidScreenState.Ready)
			{
				this.RequestUpdatedPermissions();
			}
		}

		// Token: 0x06006C7B RID: 27771 RVA: 0x0023156F File Offset: 0x0022F76F
		private void KIdScreen()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (!KIDManager.HasSession)
			{
				this.GuardianConsentMessage("OPTION 3", "");
				return;
			}
			this.KIdScreen_DisplayPermissions();
		}

		// Token: 0x06006C7C RID: 27772 RVA: 0x00231598 File Offset: 0x0022F798
		private void KIdScreen_DisplayPermissions()
		{
			AgeStatusType activeAccountStatus = KIDManager.GetActiveAccountStatus();
			string text = ((!KIDManager.InitialisationSuccessful) ? "NOT READY" : activeAccountStatus.ToString());
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("k-ID Account Status:\t" + text);
			if (activeAccountStatus == (AgeStatusType)0)
			{
				stringBuilder.AppendLine("\nPress 'OPTION 1' to get permissions!");
				this.screenText.Set(stringBuilder.ToString());
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				stringBuilder.AppendLine("\nWAITING FOR PARENT/GUARDIAN CONSENT!");
				this.screenText.Set(stringBuilder.ToString());
				return;
			}
			stringBuilder.AppendLine("\nPermissions:");
			List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
			int count = allPermissionsData.Count;
			int num = 1;
			for (int i = 0; i < count; i++)
			{
				if (this._interestedPermissionNames.Contains(allPermissionsData[i].Name))
				{
					string text2 = (allPermissionsData[i].Enabled ? "<color=#85ffa5>" : "<color=\"RED\">");
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"[",
						num.ToString(),
						"] ",
						text2,
						allPermissionsData[i].Name,
						"</color>"
					}));
					num++;
				}
			}
			stringBuilder.AppendLine("\nTO REFRESH PERMISSIONS PRESS OPTION 1!");
			this.screenText.Set(stringBuilder.ToString());
		}

		// Token: 0x06006C7D RID: 27773 RVA: 0x002316FD File Offset: 0x0022F8FD
		private string GetLocalisedLanguageScreen()
		{
			return this.GetLanguageScreenLocalisation();
		}

		// Token: 0x06006C7E RID: 27774 RVA: 0x00231708 File Offset: 0x0022F908
		private void GetLangaugesList(ref string langStr)
		{
			this._languagesDisplaySB.Clear();
			int num = 12;
			int num2 = 3;
			int num3 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<int, Locale> keyValuePair in LocalisationManager.GetAllBindings())
			{
				num3++;
				string text = LocalisationManager.LocaleToFriendlyString(keyValuePair.Value, false).ToUpper();
				string text2 = string.Format("{0}) {1}", keyValuePair.Key, text);
				stringBuilder.Append(text2);
				int remainingChars = this.GetRemainingChars(text, num);
				stringBuilder.Append(' ', remainingChars);
				if (num3 >= num2)
				{
					this._languagesDisplaySB.AppendLine(stringBuilder.ToString());
					stringBuilder.Clear();
					num3 = 0;
				}
			}
			this._languagesDisplaySB.AppendLine(stringBuilder.ToString());
			langStr = langStr + this._languagesDisplaySB.ToString() + "\n";
		}

		// Token: 0x06006C7F RID: 27775 RVA: 0x0023180C File Offset: 0x0022FA0C
		private int GetRemainingChars(string value, int maxLength)
		{
			int num;
			if (value == "日本語")
			{
				num = ((LocalisationManager.CurrentLanguage.Identifier.Code == "ja") ? 7 : 7);
			}
			else
			{
				num = Mathf.Clamp(maxLength - value.Length, 0, maxLength);
			}
			return num;
		}

		// Token: 0x06006C80 RID: 27776 RVA: 0x00231860 File Offset: 0x0022FA60
		private string GetLanguageScreenLocalisation()
		{
			string text = "";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_TITLE", out text2, "CHOOSE YOUR LANGUAGE\n");
			text += text2;
			this.GetLangaugesList(ref text);
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_INSTRUCTIONS", out text2, "PRESS NUMBER KEYS TO CHOOSE A LANGUAGE\n");
			text += text2;
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_CURRENT_LANGUAGE", out text2, "CURRENT LANGUAGE: ");
			text = text + text2.TrailingSpace() + LocalisationManager.LocaleToFriendlyString(null, false).ToUpper();
			return text;
		}

		// Token: 0x06006C81 RID: 27777 RVA: 0x002318DB File Offset: 0x0022FADB
		private void InitialiseLanguageScreen()
		{
			this._previousLocalisationSetting = LocalisationManager.CurrentLanguage;
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}

		// Token: 0x06006C82 RID: 27778 RVA: 0x002318F9 File Offset: 0x0022FAF9
		private void LanguageScreen()
		{
			this.screenText.Set(this.GetLocalisedLanguageScreen());
		}

		// Token: 0x06006C83 RID: 27779 RVA: 0x0023190C File Offset: 0x0022FB0C
		private void ProcessLanguageState(GorillaKeyboardBindings buttonPressed)
		{
			int num;
			if (!buttonPressed.FromNumberBindingToInt(out num))
			{
				return;
			}
			Locale locale;
			if (!LocalisationManager.TryGetLocaleBinding(num, out locale))
			{
				return;
			}
			LocalisationManager.Instance.OnLanguageButtonPressed(locale.Identifier.Code, true);
			this.RefreshFunctionNames();
		}

		// Token: 0x06006C84 RID: 27780 RVA: 0x00231950 File Offset: 0x0022FB50
		private void OnLanguageChanged()
		{
			if (this._previousLocalisationSetting == LocalisationManager.CurrentLanguage)
			{
				Debug.Log("[LOCALISATION::GORILLA_COMPUTER] Language changed, but no different to previous setting [" + this._previousLocalisationSetting.ToString() + "]");
				return;
			}
			this._previousLocalisationSetting = LocalisationManager.CurrentLanguage;
			this.RefreshFunctionNames();
		}

		// Token: 0x06006C85 RID: 27781 RVA: 0x002319A0 File Offset: 0x0022FBA0
		private void RefreshFunctionNames()
		{
			this.FunctionNames.Clear();
			this.FunctionsCount = this.OrderList.Count;
			this.highestCharacterCount = int.MinValue;
			this.OrderList.ForEach(delegate(GorillaComputer.StateOrderItem s)
			{
				string name = s.GetName();
				if (name.Length > this.highestCharacterCount)
				{
					this.highestCharacterCount = name.Length;
				}
				this.FunctionNames.Add(name);
			});
			for (int i = 0; i < this.FunctionsCount; i++)
			{
				int num = this.highestCharacterCount - this.FunctionNames[i].Length;
				for (int j = 0; j < num; j++)
				{
					List<string> functionNames = this.FunctionNames;
					int num2 = i;
					functionNames[num2] += " ";
				}
			}
		}

		// Token: 0x04007B5D RID: 31581
		private const string VERSION_MISMATCH_KEY = "VERSION_MISMATCH";

		// Token: 0x04007B5E RID: 31582
		private const string CONNECTION_ISSUE_KEY = "CONNECTION_ISSUE";

		// Token: 0x04007B5F RID: 31583
		private const string NO_CONNECTION_KEY = "NO_CONNECTION";

		// Token: 0x04007B60 RID: 31584
		private const string STARTUP_INTRO_KEY = "STARTUP_INTRO";

		// Token: 0x04007B61 RID: 31585
		private const string STARTUP_PLAYERS_ONLINE_KEY = "STARTUP_PLAYERS_ONLINE";

		// Token: 0x04007B62 RID: 31586
		private const string STARTUP_USERS_BANNED_KEY = "STARTUP_USERS_BANNED";

		// Token: 0x04007B63 RID: 31587
		private const string STARTUP_PRESS_KEY_KEY = "STARTUP_PRESS_KEY";

		// Token: 0x04007B64 RID: 31588
		private const string STARTUP_PRESS_KEY_SHORT_KEY = "STARTUP_PRESS_KEY_SHORT";

		// Token: 0x04007B65 RID: 31589
		private const string STARTUP_MANAGED_KEY = "STARTUP_MANAGED";

		// Token: 0x04007B66 RID: 31590
		private const string COLOR_SELECT_INTRO_KEY = "COLOR_SELECT_INTRO";

		// Token: 0x04007B67 RID: 31591
		private const string CURRENT_SELECTED_LANGUAGE_KEY = "CURRENT_SELECTED_LANGUAGE";

		// Token: 0x04007B68 RID: 31592
		private const string CHANGE_TO_KEY = "CHANGE_TO";

		// Token: 0x04007B69 RID: 31593
		private const string CONFIRM_LANGUAGE_KEY = "CONFIRM_LANGUAGE";

		// Token: 0x04007B6A RID: 31594
		private const string COLOR_RED_KEY = "COLOR_RED";

		// Token: 0x04007B6B RID: 31595
		private const string COLOR_GREEN_KEY = "COLOR_GREEN";

		// Token: 0x04007B6C RID: 31596
		private const string COLOR_BLUE_KEY = "COLOR_BLUE";

		// Token: 0x04007B6D RID: 31597
		private const string ROOM_INTRO_KEY = "ROOM_INTRO";

		// Token: 0x04007B6E RID: 31598
		private const string ROOM_OPTION_KEY = "ROOM_OPTION";

		// Token: 0x04007B6F RID: 31599
		private const string ROOM_TEXT_CURRENT_ROOM_KEY = "ROOM_TEXT_CURRENT_ROOM";

		// Token: 0x04007B70 RID: 31600
		private const string PLAYERS_IN_ROOM_KEY = "PLAYERS_IN_ROOM";

		// Token: 0x04007B71 RID: 31601
		private const string NOT_IN_ROOM_KEY = "NOT_IN_ROOM";

		// Token: 0x04007B72 RID: 31602
		private const string PLAYERS_ONLINE_KEY = "PLAYERS_ONLINE";

		// Token: 0x04007B73 RID: 31603
		private const string ROOM_TO_JOIN_KEY = "ROOM_TO_JOIN";

		// Token: 0x04007B74 RID: 31604
		private const string ROOM_FULL_KEY = "ROOM_FULL";

		// Token: 0x04007B75 RID: 31605
		private const string ROOM_JOIN_NOT_ALLOWED_KEY = "ROOM_JOIN_NOT_ALLOWED";

		// Token: 0x04007B76 RID: 31606
		private const string LANGUAGE_KEY = "LANGUAGE";

		// Token: 0x04007B77 RID: 31607
		private const string NAME_SCREEN_KEY = "NAME_SCREEN";

		// Token: 0x04007B78 RID: 31608
		private const string CURRENT_NAME_KEY = "CURRENT_NAME";

		// Token: 0x04007B79 RID: 31609
		private const string NEW_NAME_KEY = "NEW_NAME";

		// Token: 0x04007B7A RID: 31610
		private const string TURN_SCREEN_KEY = "TURN_SCREEN";

		// Token: 0x04007B7B RID: 31611
		private const string TURN_SCREEN_TURNING_SPEED_KEY = "TURN_SCREEN_TURNING_SPEED";

		// Token: 0x04007B7C RID: 31612
		private const string TURN_SCREEN_TURN_TYPE_KEY = "TURN_SCREEN_TURN_TYPE";

		// Token: 0x04007B7D RID: 31613
		private const string TURN_SCREEN_TURN_SPEED_KEY = "TURN_SCREEN_TURN_SPEED";

		// Token: 0x04007B7E RID: 31614
		private const string TURN_TYPE_SNAP_TURN_KEY = "TURN_TYPE_SNAP_TURN";

		// Token: 0x04007B7F RID: 31615
		private const string TURN_TYPE_SMOOTH_TURN_KEY = "TURN_TYPE_SMOOTH_TURN";

		// Token: 0x04007B80 RID: 31616
		private const string TURN_TYPE_NO_TURN_KEY = "TURN_TYPE_NO_TURN";

		// Token: 0x04007B81 RID: 31617
		private const string QUEUE_SCREEN_KEY = "QUEUE_SCREEN";

		// Token: 0x04007B82 RID: 31618
		private const string BEAT_OBSTACLE_COURSE_KEY = "BEAT_OBSTACLE_COURSE";

		// Token: 0x04007B83 RID: 31619
		private const string COMPETITIVE_DESC_KEY = "COMPETITIVE_DESC";

		// Token: 0x04007B84 RID: 31620
		private const string QUEUE_SCREEN_ALL_QUEUES_KEY = "QUEUE_SCREEN_ALL_QUEUES";

		// Token: 0x04007B85 RID: 31621
		private const string QUEUE_SCREEN_DEFAULT_QUEUES_KEY = "QUEUE_SCREEN_DEFAULT_QUEUES";

		// Token: 0x04007B86 RID: 31622
		private const string CURRENT_QUEUE_KEY = "CURRENT_QUEUE";

		// Token: 0x04007B87 RID: 31623
		private const string DEFAULT_QUEUE_KEY = "DEFAULT_QUEUE";

		// Token: 0x04007B88 RID: 31624
		private const string MINIGAMES_QUEUE_KEY = "MINIGAMES_QUEUE";

		// Token: 0x04007B89 RID: 31625
		private const string COMPETITIVE_QUEUE_KEY = "COMPETITIVE_QUEUE";

		// Token: 0x04007B8A RID: 31626
		private const string MIC_SCREEN_INTRO_KEY = "MIC_SCREEN_INTRO";

		// Token: 0x04007B8B RID: 31627
		private const string MIC_SCREEN_OPTIONS_KEY = "MIC_SCREEN_OPTIONS";

		// Token: 0x04007B8C RID: 31628
		private const string MIC_SCREEN_CURRENT_KEY = "MIC_SCREEN_CURRENT";

		// Token: 0x04007B8D RID: 31629
		private const string MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP_KEY = "MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP";

		// Token: 0x04007B8E RID: 31630
		private const string MIC_SCREEN_MIC_DISABLED_KEY = "MIC_SCREEN_MIC_DISABLED";

		// Token: 0x04007B8F RID: 31631
		private const string MIC_SCREEN_NO_MIC_KEY = "MIC_SCREEN_NO_MIC";

		// Token: 0x04007B90 RID: 31632
		private const string MIC_SCREEN_NO_PERMISSIONS_KEY = "MIC_SCREEN_NO_PERMISSIONS";

		// Token: 0x04007B91 RID: 31633
		private const string MIC_SCREEN_PUSH_TO_TALK_TOOLTIP_KEY = "MIC_SCREEN_PUSH_TO_TALK_TOOLTIP";

		// Token: 0x04007B92 RID: 31634
		private const string MIC_SCREEN_INPUT_TEST_LABEL_KEY = "MIC_SCREEN_INPUT_TEST_LABEL";

		// Token: 0x04007B93 RID: 31635
		private const string MIC_SCREEN_INPUT_TEST_NO_MIC_KEY = "MIC_SCREEN_INPUT_TEST_NO_MIC";

		// Token: 0x04007B94 RID: 31636
		private const string ALL_CHAT_MIC_KEY = "ALL_CHAT_MIC";

		// Token: 0x04007B95 RID: 31637
		private const string PUSH_TO_TALK_MIC_KEY = "PUSH_TO_TALK_MIC";

		// Token: 0x04007B96 RID: 31638
		private const string PUSH_TO_MUTE_MIC_KEY = "PUSH_TO_MUTE_MIC";

		// Token: 0x04007B97 RID: 31639
		private const string OPEN_MIC_KEY = "OPEN_MIC";

		// Token: 0x04007B98 RID: 31640
		private const string AUTOMOD_SCREEN_INTRO_KEY = "AUTOMOD_SCREEN_INTRO";

		// Token: 0x04007B99 RID: 31641
		private const string AUTOMOD_SCREEN_OPTIONS_KEY = "AUTOMOD_SCREEN_OPTIONS";

		// Token: 0x04007B9A RID: 31642
		private const string AUTOMOD_SCREEN_CURRENT_KEY = "AUTOMOD_SCREEN_CURRENT";

		// Token: 0x04007B9B RID: 31643
		private const string AUTOMOD_AGGRESSIVE_KEY = "AUTOMOD_AGGRESSIVE";

		// Token: 0x04007B9C RID: 31644
		private const string AUTOMOD_MODERATE_KEY = "AUTOMOD_MODERATE";

		// Token: 0x04007B9D RID: 31645
		private const string AUTOMOD_OFF_KEY = "AUTOMOD_OFF";

		// Token: 0x04007B9E RID: 31646
		private const string VOICE_CHAT_SCREEN_INTRO_OLD_KEY = "VOICE_CHAT_SCREEN_INTRO_OLD";

		// Token: 0x04007B9F RID: 31647
		private const string VOICE_CHAT_SCREEN_OPTIONS_OLD_KEY = "VOICE_CHAT_SCREEN_OPTIONS_OLD";

		// Token: 0x04007BA0 RID: 31648
		private const string VOICE_CHAT_SCREEN_CURRENT_OLD_KEY = "VOICE_CHAT_SCREEN_CURRENT_OLD";

		// Token: 0x04007BA1 RID: 31649
		private const string TRUE_KEY = "TRUE";

		// Token: 0x04007BA2 RID: 31650
		private const string FALSE_KEY = "FALSE";

		// Token: 0x04007BA3 RID: 31651
		private const string VOICE_CHAT_SCREEN_INTRO_KEY = "VOICE_CHAT_SCREEN_INTRO";

		// Token: 0x04007BA4 RID: 31652
		private const string VOICE_CHAT_SCREEN_OPTIONS_KEY = "VOICE_CHAT_SCREEN_OPTIONS";

		// Token: 0x04007BA5 RID: 31653
		private const string VOICE_CHAT_SCREEN_CURRENT_KEY = "VOICE_CHAT_SCREEN_CURRENT";

		// Token: 0x04007BA6 RID: 31654
		private const string VOICE_OPTION_HUMAN_KEY = "VOICE_OPTION_HUMAN";

		// Token: 0x04007BA7 RID: 31655
		private const string VOICE_OPTION_MONKE_KEY = "VOICE_OPTION_MONKE";

		// Token: 0x04007BA8 RID: 31656
		private const string VOICE_OPTION_OFF_KEY = "VOICE_OPTION_OFF";

		// Token: 0x04007BA9 RID: 31657
		private const string VISUALS_SCREEN_INTRO_KEY = "VISUALS_SCREEN_INTRO";

		// Token: 0x04007BAA RID: 31658
		private const string VISUALS_SCREEN_OPTIONS_KEY = "VISUALS_SCREEN_OPTIONS";

		// Token: 0x04007BAB RID: 31659
		private const string VISUALS_SCREEN_CURRENT_KEY = "VISUALS_SCREEN_CURRENT";

		// Token: 0x04007BAC RID: 31660
		private const string VISUALS_SCREEN_VOLUME_KEY = "VISUALS_SCREEN_VOLUME";

		// Token: 0x04007BAD RID: 31661
		private const string CREDITS_KEY = "CREDITS";

		// Token: 0x04007BAE RID: 31662
		private const string CREDITS_PRESS_ENTER_KEY = "CREDITS_PRESS_ENTER";

		// Token: 0x04007BAF RID: 31663
		private const string CREDITS_CONTINUED_KEY = "CREDITS_CONTINUED";

		// Token: 0x04007BB0 RID: 31664
		private const string TIME_SCREEN_KEY = "TIME_SCREEN";

		// Token: 0x04007BB1 RID: 31665
		private const string GROUP_SCREEN_LIMITED_OLD_KEY = "GROUP_SCREEN_LIMITED_OLD";

		// Token: 0x04007BB2 RID: 31666
		private const string GROUP_SCREEN_FULL_OLD_KEY = "GROUP_SCREEN_FULL_OLD";

		// Token: 0x04007BB3 RID: 31667
		private const string GROUP_SCREEN_SELECTION_OLD_KEY = "GROUP_SCREEN_SELECTION_OLD";

		// Token: 0x04007BB4 RID: 31668
		private const string PLATFORM_STEAM_KEY = "PLATFORM_STEAM";

		// Token: 0x04007BB5 RID: 31669
		private const string PLATFORM_QUEST_KEY = "PLATFORM_QUEST";

		// Token: 0x04007BB6 RID: 31670
		private const string PLATFORM_PSVR_KEY = "PLATFORM_PSVR";

		// Token: 0x04007BB7 RID: 31671
		private const string PLATFORM_PICO_KEY = "PLATFORM_PICO";

		// Token: 0x04007BB8 RID: 31672
		private const string PLATFORM_OCULUS_PC_KEY = "PLATFORM_OCULUS_PC";

		// Token: 0x04007BB9 RID: 31673
		private const string SUPPORT_SCREEN_INTRO_KEY = "SUPPORT_SCREEN_INTRO";

		// Token: 0x04007BBA RID: 31674
		private const string SUPPORT_SCREEN_DETAILS_PLAYER_ID_KEY = "SUPPORT_SCREEN_DETAILS_PLAYERID";

		// Token: 0x04007BBB RID: 31675
		private const string SUPPORT_SCREEN_DETAILS_VERSION_KEY = "SUPPORT_SCREEN_DETAILS_VERSION";

		// Token: 0x04007BBC RID: 31676
		private const string SUPPORT_SCREEN_DETAILS_PLATFORM_KEY = "SUPPORT_SCREEN_DETAILS_PLATFORM";

		// Token: 0x04007BBD RID: 31677
		private const string SUPPORT_SCREEN_DETAILS_BUILD_DATE_KEY = "SUPPORT_SCREEN_DETAILS_BUILD_DATE";

		// Token: 0x04007BBE RID: 31678
		private const string SUPPORT_SCREEN_DETAILS_MOTHERSHIP_SESSION_ID_KEY = "SUPPORT_SCREEN_DETAILS_MOTHERSHIP_SESSION_ID";

		// Token: 0x04007BBF RID: 31679
		private const string SUPPORT_SCREEN_INITIAL_KEY = "SUPPORT_SCREEN_INITIAL";

		// Token: 0x04007BC0 RID: 31680
		private const string SUPPORT_SCREEN_INITIAL_WARNING_KEY = "SUPPORT_SCREEN_INITIAL_WARNING";

		// Token: 0x04007BC1 RID: 31681
		private const string OCULUS_BUILD_CODE_KEY = "OCULUS_BUILD_CODE";

		// Token: 0x04007BC2 RID: 31682
		private const string LOADING_SCREEN_KEY = "LOADING_SCREEN";

		// Token: 0x04007BC3 RID: 31683
		private const string WARNING_SCREEN_KEY = "WARNING_SCREEN";

		// Token: 0x04007BC4 RID: 31684
		private const string WARNING_SCREEN_CONFIRMATION_KEY = "WARNING_SCREEN_CONFIRMATION";

		// Token: 0x04007BC5 RID: 31685
		private const string WARNING_SCREEN_TYPE_YES_KEY = "WARNING_SCREEN_TYPE_YES";

		// Token: 0x04007BC6 RID: 31686
		private const string FUNCTION_ROOM_KEY = "FUNCTION_ROOM";

		// Token: 0x04007BC7 RID: 31687
		private const string FUNCTION_NAME_KEY = "FUNCTION_NAME";

		// Token: 0x04007BC8 RID: 31688
		private const string FUNCTION_COLOR_KEY = "FUNCTION_COLOR";

		// Token: 0x04007BC9 RID: 31689
		private const string FUNCTION_TURN_KEY = "FUNCTION_TURN";

		// Token: 0x04007BCA RID: 31690
		private const string FUNCTION_MIC_KEY = "FUNCTION_MIC";

		// Token: 0x04007BCB RID: 31691
		private const string FUNCTION_QUEUE_KEY = "FUNCTION_QUEUE";

		// Token: 0x04007BCC RID: 31692
		private const string FUNCTION_GROUP_KEY = "FUNCTION_GROUP";

		// Token: 0x04007BCD RID: 31693
		private const string FUNCTION_VOICE_KEY = "FUNCTION_VOICE";

		// Token: 0x04007BCE RID: 31694
		private const string FUNCTION_AUTOMOD_KEY = "FUNCTION_AUTOMOD";

		// Token: 0x04007BCF RID: 31695
		private const string FUNCTION_ITEMS_KEY = "FUNCTION_ITEMS";

		// Token: 0x04007BD0 RID: 31696
		private const string FUNCTION_CREDITS_KEY = "FUNCTION_CREDITS";

		// Token: 0x04007BD1 RID: 31697
		private const string FUNCTION_LANGUAGE_KEY = "FUNCTION_LANGUAGE";

		// Token: 0x04007BD2 RID: 31698
		private const string FUNCTION_SUPPORT_KEY = "FUNCTION_SUPPORT";

		// Token: 0x04007BD3 RID: 31699
		private const string COMPUTER_KEYBOARD_DELETE_KEY = "COMPUTER_KEYBOARD_DELETE";

		// Token: 0x04007BD4 RID: 31700
		private const string COMPUTER_KEYBOARD_ENTER_KEY = "COMPUTER_KEYBOARD_ENTER";

		// Token: 0x04007BD5 RID: 31701
		private const string COMPUTER_KEYBOARD_OPTION1_KEY = "COMPUTER_KEYBOARD_OPTION1";

		// Token: 0x04007BD6 RID: 31702
		private const string COMPUTER_KEYBOARD_OPTION2_KEY = "COMPUTER_KEYBOARD_OPTION2";

		// Token: 0x04007BD7 RID: 31703
		private const string COMPUTER_KEYBOARD_OPTION3_KEY = "COMPUTER_KEYBOARD_OPTION3";

		// Token: 0x04007BD8 RID: 31704
		private const string WARNING_SCREEN_YES_INPUT_KEY = "WARNING_SCREEN_YES_INPUT";

		// Token: 0x04007BD9 RID: 31705
		private const string GROUP_SCREEN_ENTER_PARTY_KEY = "GROUP_SCREEN_ENTER_PARTY";

		// Token: 0x04007BDA RID: 31706
		private const string GROUP_SCREEN_ENTER_NOPARTY_KEY = "GROUP_SCREEN_ENTER_NOPARTY";

		// Token: 0x04007BDB RID: 31707
		private const string GROUP_SCREEN_CANNOT_JOIN_KEY = "GROUP_SCREEN_CANNOT_JOIN";

		// Token: 0x04007BDC RID: 31708
		private const string GROUP_SCREEN_ACTIVE_ZONES_KEY = "GROUP_SCREEN_ACTIVE_ZONES";

		// Token: 0x04007BDD RID: 31709
		private const string GROUP_SCREEN_DESTINATIONS_KEY = "GROUP_SCREEN_DESTINATIONS";

		// Token: 0x04007BDE RID: 31710
		private const string NAME_SCREEN_TOGGLE_NAMETAGS_KEY = "NAME_SCREEN_TOGGLE_NAMETAGS";

		// Token: 0x04007BDF RID: 31711
		private const string NAME_SCREEN_KID_PROHIBITED_VERB_KEY = "NAME_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x04007BE0 RID: 31712
		private const string NAME_SCREEN_DISABLED_KEY = "NAME_SCREEN_DISABLED";

		// Token: 0x04007BE1 RID: 31713
		private const string ON_KEY = "ON_KEY";

		// Token: 0x04007BE2 RID: 31714
		private const string OFF_KEY = "OFF_KEY";

		// Token: 0x04007BE3 RID: 31715
		private const string KID_PROHIBITED_MESSAGE_KEY = "KID_PROHIBITED_MESSAGE";

		// Token: 0x04007BE4 RID: 31716
		private const string KID_PERMISSION_NEEDED_KEY = "KID_PERMISSION_NEEDED";

		// Token: 0x04007BE5 RID: 31717
		private const string KID_WAITING_PERMISSION_KEY = "KID_WAITING_PERMISSION";

		// Token: 0x04007BE6 RID: 31718
		private const string KID_REFRESH_PERMISSIONS_KEY = "KID_REFRESH_PERMISSIONS";

		// Token: 0x04007BE7 RID: 31719
		private const string KID_CHECK_AGAIN_COOLDOWN_KEY = "KID_CHECK_AGAIN_COOLDOWN";

		// Token: 0x04007BE8 RID: 31720
		private const string STARTUP_TROOP_TEXT_KEY = "STARTUP_TROOP_TEXT";

		// Token: 0x04007BE9 RID: 31721
		private const string ROOM_GROUP_TRAVEL_KEY = "ROOM_GROUP_TRAVEL";

		// Token: 0x04007BEA RID: 31722
		private const string ROOM_PARTY_WARNING_KEY = "ROOM_PARTY_WARNING";

		// Token: 0x04007BEB RID: 31723
		private const string ROOM_GAME_LABEL_KEY = "ROOM_GAME_LABEL";

		// Token: 0x04007BEC RID: 31724
		private const string ROOM_SCREEN_KID_PROHIBITED_VERB_KEY = "ROOM_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x04007BED RID: 31725
		private const string ROOM_SCREEN_DISABLED_KEY = "ROOM_SCREEN_DISABLED";

		// Token: 0x04007BEE RID: 31726
		private const string REDEMPTION_INTRO_KEY = "REDEMPTION_INTRO";

		// Token: 0x04007BEF RID: 31727
		private const string REDEMPTION_CODE_LABEL_KEY = "REDEMPTION_CODE_LABEL";

		// Token: 0x04007BF0 RID: 31728
		private const string REDEMPTION_CODE_INVALID_KEY = "REDEMPTION_CODE_INVALID";

		// Token: 0x04007BF1 RID: 31729
		private const string REDEMPTION_CODE_VALIDATING_KEY = "REDEMPTION_CODE_VALIDATING";

		// Token: 0x04007BF2 RID: 31730
		private const string REDEMPTION_CODE_ALREADY_USED_KEY = "REDEMPTION_CODE_ALREADY_USED";

		// Token: 0x04007BF3 RID: 31731
		private const string REDEMPTION_CODE_TOO_EARLY_KEY = "REDEMPTION_CODE_TOO_EARLY";

		// Token: 0x04007BF4 RID: 31732
		private const string REDEMPTION_CODE_TOO_LATE_KEY = "REDEMPTION_CODE_TOO_LATE";

		// Token: 0x04007BF5 RID: 31733
		private const string REDEMPTION_CODE_ALREADY_GRANTED_KEY = "REDEMPTION_CODE_ALREADY_GRANTED";

		// Token: 0x04007BF6 RID: 31734
		private const string REDEMPTION_CODE_SUCCESS_KEY = "REDEMPTION_CODE_SUCCESS";

		// Token: 0x04007BF7 RID: 31735
		private const string LIMITED_ONLINE_FUNC_KEY = "LIMITED_ONLINE_FUNC";

		// Token: 0x04007BF8 RID: 31736
		private const string CURRENT_MODE_KEY = "CURRENT_MODE";

		// Token: 0x04007BF9 RID: 31737
		private const string SUPPORT_META_ACCOUNT_TYPE_KEY = "SUPPORT_META_ACCOUNT_TYPE";

		// Token: 0x04007BFA RID: 31738
		private const string SUPPORT_FINAL_QUEST_ONE_KEY = "SUPPORT_FINAL_QUEST_ONE";

		// Token: 0x04007BFB RID: 31739
		private const string SUPPORT_KID_ACCOUNT_TYPE_KEY = "SUPPORT_KID_ACCOUNT_TYPE";

		// Token: 0x04007BFC RID: 31740
		private const string VOICE_SCREEN_KID_PROHIBITED_VERB_KEY = "VOICE_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x04007BFD RID: 31741
		private const string VOICE_SCREEN_DISABLED_KEY = "VOICE_SCREEN_DISABLED";

		// Token: 0x04007BFE RID: 31742
		private const string MIC_SCREEN_GUARDIAN_FEATURE_DESC_KEY = "VOICE_SCREEN_GUARDIAN_FEATURE_DESC";

		// Token: 0x04007BFF RID: 31743
		private const string VOICE_SCREEN_KID_CURRENT_VOICE_KEY = "VOICE_SCREEN_KID_CURRENT_VOICE";

		// Token: 0x04007C00 RID: 31744
		private const string MIC_SCREEN_PUSH_KEY_INSTRUCTIONS_KEY = "MIC_SCREEN_PUSH_KEY_INSTRUCTIONS";

		// Token: 0x04007C01 RID: 31745
		private const string TROOP_SCREEN_INTRO_KEY = "TROOP_SCREEN_INTRO";

		// Token: 0x04007C02 RID: 31746
		private const string TROOP_SCREEN_INSTRUCTIONS_KEY = "TROOP_SCREEN_INSTRUCTIONS";

		// Token: 0x04007C03 RID: 31747
		private const string TROOP_SCREEN_CURRENT_TROOP_KEY = "TROOP_SCREEN_CURRENT_TROOP";

		// Token: 0x04007C04 RID: 31748
		private const string TROOP_SCREEN_IN_QUEUE_KEY = "TROOP_SCREEN_IN_QUEUE";

		// Token: 0x04007C05 RID: 31749
		private const string TROOP_SCREEN_PLAYERS_IN_TROOP_KEY = "TROOP_SCREEN_PLAYERS_IN_TROOP";

		// Token: 0x04007C06 RID: 31750
		private const string TROOP_SCREEN_DEFAULT_QUEUE_KEY = "TROOP_SCREEN_DEFAULT_QUEUE";

		// Token: 0x04007C07 RID: 31751
		private const string TROOP_SCREEN_CURRENT_QUEUE_KEY = "TROOP_SCREEN_CURRENT_QUEUE";

		// Token: 0x04007C08 RID: 31752
		private const string TROOP_SCREEN_TROOP_QUEUE_KEY = "TROOP_SCREEN_TROOP_QUEUE";

		// Token: 0x04007C09 RID: 31753
		private const string TROOP_SCREEN_LEAVE_KEY = "TROOP_SCREEN_LEAVE";

		// Token: 0x04007C0A RID: 31754
		private const string TROOP_SCREEN_NOT_IN_TROOP_KEY = "TROOP_SCREEN_NOT_IN_TROOP";

		// Token: 0x04007C0B RID: 31755
		private const string TROOP_SCREEN_JOIN_TROOP_KEY = "TROOP_SCREEN_JOIN_TROOP";

		// Token: 0x04007C0C RID: 31756
		private const string TROOP_SCREEN_KID_PROHIBITED_VERB_KEY = "TROOP_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x04007C0D RID: 31757
		private const string TROOP_SCREEN_DISABLED_KEY = "TROOP_SCREEN_DISABLED";

		// Token: 0x04007C0E RID: 31758
		private const string TROOP_SCREEN_KID_DESC_KEY = "TROOP_SCREEN_KID_DESC";

		// Token: 0x04007C0F RID: 31759
		private const bool HIDE_SCREENS = false;

		// Token: 0x04007C10 RID: 31760
		public const string NAMETAG_PLAYER_PREF_KEY = "nameTagsOn";

		// Token: 0x04007C11 RID: 31761
		[OnEnterPlay_SetNull]
		public static volatile GorillaComputer instance;

		// Token: 0x04007C12 RID: 31762
		[OnEnterPlay_Set(false)]
		public static bool hasInstance = false;

		// Token: 0x04007C13 RID: 31763
		[OnEnterPlay_SetNull]
		private static Action<bool> onNametagSettingChangedAction;

		// Token: 0x04007C14 RID: 31764
		public bool tryGetTimeAgain;

		// Token: 0x04007C15 RID: 31765
		public Material unpressedMaterial;

		// Token: 0x04007C16 RID: 31766
		public Material pressedMaterial;

		// Token: 0x04007C17 RID: 31767
		public string currentTextField;

		// Token: 0x04007C18 RID: 31768
		public float buttonFadeTime;

		// Token: 0x04007C19 RID: 31769
		public string offlineTextInitialString;

		// Token: 0x04007C1A RID: 31770
		public GorillaText screenText;

		// Token: 0x04007C1B RID: 31771
		public GorillaText functionSelectText;

		// Token: 0x04007C1C RID: 31772
		public GorillaText wallScreenText;

		// Token: 0x04007C1D RID: 31773
		private Locale _lastLocaleChecked_Version;

		// Token: 0x04007C1E RID: 31774
		private Locale _lastLocaleChecked_Connect;

		// Token: 0x04007C1F RID: 31775
		private string _cachedVersionMismatch = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";

		// Token: 0x04007C20 RID: 31776
		private string _cachedUnableToConnect = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";

		// Token: 0x04007C21 RID: 31777
		public Material wrongVersionMaterial;

		// Token: 0x04007C22 RID: 31778
		public MeshRenderer wallScreenRenderer;

		// Token: 0x04007C23 RID: 31779
		public MeshRenderer computerScreenRenderer;

		// Token: 0x04007C24 RID: 31780
		public long startupMillis;

		// Token: 0x04007C25 RID: 31781
		public DateTime startupTime;

		// Token: 0x04007C26 RID: 31782
		public GameModeType lastPressedGameModeType;

		// Token: 0x04007C27 RID: 31783
		public string lastPressedGameMode;

		// Token: 0x04007C28 RID: 31784
		public WatchableStringSO currentGameMode;

		// Token: 0x04007C29 RID: 31785
		public WatchableStringSO currentGameModeText;

		// Token: 0x04007C2A RID: 31786
		public int includeUpdatedServerSynchTest;

		// Token: 0x04007C2B RID: 31787
		public PhotonNetworkController networkController;

		// Token: 0x04007C2C RID: 31788
		public float updateCooldown = 1f;

		// Token: 0x04007C2D RID: 31789
		private float defaultUpdateCooldown;

		// Token: 0x04007C2E RID: 31790
		private float micUpdateCooldown = 0.01f;

		// Token: 0x04007C2F RID: 31791
		public float lastUpdateTime;

		// Token: 0x04007C30 RID: 31792
		private float deltaTime;

		// Token: 0x04007C31 RID: 31793
		public bool isConnectedToMaster;

		// Token: 0x04007C32 RID: 31794
		public bool internetFailure;

		// Token: 0x04007C33 RID: 31795
		private string[] _allowedMapsToJoin;

		// Token: 0x04007C34 RID: 31796
		public bool limitOnlineScreens;

		// Token: 0x04007C35 RID: 31797
		[Header("State vars")]
		public bool stateUpdated;

		// Token: 0x04007C36 RID: 31798
		public bool screenChanged;

		// Token: 0x04007C37 RID: 31799
		public bool initialized;

		// Token: 0x04007C38 RID: 31800
		public List<GorillaComputer.StateOrderItem> OrderList = new List<GorillaComputer.StateOrderItem>
		{
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Room),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Name),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Language, "Lang"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Turn),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Mic),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Queue),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Troop),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Group),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Voice),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.AutoMute, "Automod"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Visuals, "Items"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Credits),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Support)
		};

		// Token: 0x04007C39 RID: 31801
		public string Pointer = "<-";

		// Token: 0x04007C3A RID: 31802
		public int highestCharacterCount;

		// Token: 0x04007C3B RID: 31803
		public List<string> FunctionNames = new List<string>();

		// Token: 0x04007C3C RID: 31804
		public int FunctionsCount;

		// Token: 0x04007C3D RID: 31805
		[Header("Room vars")]
		public string roomToJoin;

		// Token: 0x04007C3E RID: 31806
		public bool roomFull;

		// Token: 0x04007C3F RID: 31807
		public bool roomNotAllowed;

		// Token: 0x04007C40 RID: 31808
		[Header("Mic vars")]
		public string pttType;

		// Token: 0x04007C41 RID: 31809
		private GorillaSpeakerLoudness speakerLoudness;

		// Token: 0x04007C42 RID: 31810
		private float micInputTestTimer;

		// Token: 0x04007C43 RID: 31811
		public float micInputTestTimerThreshold = 10f;

		// Token: 0x04007C44 RID: 31812
		[Header("Automute vars")]
		public string autoMuteType;

		// Token: 0x04007C45 RID: 31813
		[Header("Queue vars")]
		public string currentQueue;

		// Token: 0x04007C46 RID: 31814
		public bool allowedInCompetitive;

		// Token: 0x04007C47 RID: 31815
		[Header("Group Vars")]
		public string groupMapJoin;

		// Token: 0x04007C48 RID: 31816
		public int groupMapJoinIndex;

		// Token: 0x04007C49 RID: 31817
		public GorillaFriendCollider friendJoinCollider;

		// Token: 0x04007C4A RID: 31818
		[Header("Troop vars")]
		public string troopName;

		// Token: 0x04007C4B RID: 31819
		public bool troopQueueActive;

		// Token: 0x04007C4C RID: 31820
		public string troopToJoin;

		// Token: 0x04007C4D RID: 31821
		private bool rememberTroopQueueState;

		// Token: 0x04007C4E RID: 31822
		[Header("Join Triggers")]
		public Dictionary<string, GorillaNetworkJoinTrigger> primaryTriggersByZone = new Dictionary<string, GorillaNetworkJoinTrigger>();

		// Token: 0x04007C4F RID: 31823
		public string voiceChatOn;

		// Token: 0x04007C50 RID: 31824
		[Header("Mode select vars")]
		public ModeSelectButton[] modeSelectButtons;

		// Token: 0x04007C52 RID: 31826
		public string buildDate;

		// Token: 0x04007C53 RID: 31827
		public string buildCode;

		// Token: 0x04007C54 RID: 31828
		[Header("Cosmetics")]
		public bool disableParticles;

		// Token: 0x04007C55 RID: 31829
		public float instrumentVolume;

		// Token: 0x04007C56 RID: 31830
		public bool iobtMode;

		// Token: 0x04007C57 RID: 31831
		public bool perfMode;

		// Token: 0x04007C58 RID: 31832
		public bool isSubcribed;

		// Token: 0x04007C59 RID: 31833
		[Header("Credits")]
		public CreditsView creditsView;

		// Token: 0x04007C5A RID: 31834
		[Header("Handedness")]
		public bool leftHanded;

		// Token: 0x04007C5B RID: 31835
		[Header("Name state vars")]
		public string savedName;

		// Token: 0x04007C5C RID: 31836
		public string currentName;

		// Token: 0x04007C5D RID: 31837
		public TextAsset exactOneWeekFile;

		// Token: 0x04007C5E RID: 31838
		public TextAsset anywhereOneWeekFile;

		// Token: 0x04007C5F RID: 31839
		public TextAsset anywhereTwoWeekFile;

		// Token: 0x04007C60 RID: 31840
		private List<GorillaComputer.ComputerState> _filteredStates = new List<GorillaComputer.ComputerState>();

		// Token: 0x04007C61 RID: 31841
		private List<GorillaComputer.StateOrderItem> _activeOrderList = new List<GorillaComputer.StateOrderItem>();

		// Token: 0x04007C62 RID: 31842
		private Stack<GorillaComputer.ComputerState> stateStack = new Stack<GorillaComputer.ComputerState>();

		// Token: 0x04007C63 RID: 31843
		private GorillaComputer.ComputerState currentComputerState;

		// Token: 0x04007C64 RID: 31844
		private GorillaComputer.ComputerState previousComputerState;

		// Token: 0x04007C65 RID: 31845
		private int currentStateIndex;

		// Token: 0x04007C66 RID: 31846
		private int usersBanned;

		// Token: 0x04007C67 RID: 31847
		private float redValue;

		// Token: 0x04007C68 RID: 31848
		private string redText;

		// Token: 0x04007C69 RID: 31849
		private float blueValue;

		// Token: 0x04007C6A RID: 31850
		private string blueText;

		// Token: 0x04007C6B RID: 31851
		private float greenValue;

		// Token: 0x04007C6C RID: 31852
		private string greenText;

		// Token: 0x04007C6D RID: 31853
		private int colorCursorLine;

		// Token: 0x04007C6E RID: 31854
		private string warningConfirmationInputString = string.Empty;

		// Token: 0x04007C6F RID: 31855
		private bool displaySupport;

		// Token: 0x04007C70 RID: 31856
		private string[] exactOneWeek;

		// Token: 0x04007C71 RID: 31857
		private string[] anywhereOneWeek;

		// Token: 0x04007C72 RID: 31858
		private string[] anywhereTwoWeek;

		// Token: 0x04007C73 RID: 31859
		private GorillaComputer.RedemptionResult redemptionResult;

		// Token: 0x04007C74 RID: 31860
		private string redemptionCode = "";

		// Token: 0x04007C75 RID: 31861
		private bool playerInVirtualStump;

		// Token: 0x04007C76 RID: 31862
		private string virtualStumpRoomPrepend = "";

		// Token: 0x04007C77 RID: 31863
		private string virtualStumpRoomModePrefix = "C";

		// Token: 0x04007C78 RID: 31864
		private WaitForSeconds waitOneSecond = new WaitForSeconds(1f);

		// Token: 0x04007C79 RID: 31865
		private Coroutine LoadingRoutine;

		// Token: 0x04007C7A RID: 31866
		private List<string> topTroops = new List<string>();

		// Token: 0x04007C7B RID: 31867
		private bool hasRequestedInitialTroopPopulation;

		// Token: 0x04007C7C RID: 31868
		private int currentTroopPopulation = -1;

		// Token: 0x04007C7D RID: 31869
		private List<string> topVstumpMaps = new List<string>();

		// Token: 0x04007C80 RID: 31872
		private float lastCheckedWifi;

		// Token: 0x04007C81 RID: 31873
		private float checkIfDisconnectedSeconds = 10f;

		// Token: 0x04007C82 RID: 31874
		private float checkIfConnectedSeconds = 1f;

		// Token: 0x04007C83 RID: 31875
		private bool didInitializeGameMode;

		// Token: 0x04007C84 RID: 31876
		private static int sessionCount = -1;

		// Token: 0x04007C85 RID: 31877
		private const bool k_debug_shouldResetSessionCount = false;

		// Token: 0x04007C86 RID: 31878
		private const bool k_debug_shouldResetGameMode = false;

		// Token: 0x04007C87 RID: 31879
		private const string k_sessionCountKey = "sessionCount";

		// Token: 0x04007C88 RID: 31880
		internal const GameModeType k_defaultGameMode = GameModeType.SuperInfect;

		// Token: 0x04007C89 RID: 31881
		internal const GameModeType k_noobGameMode = GameModeType.Infection;

		// Token: 0x04007C8A RID: 31882
		private const int k_noobSessionCountThreshold = 4;

		// Token: 0x04007C8B RID: 31883
		private float troopPopulationCheckCooldown = 3f;

		// Token: 0x04007C8C RID: 31884
		private float nextPopulationCheckTime;

		// Token: 0x04007C8D RID: 31885
		public Action OnServerTimeUpdated;

		// Token: 0x04007C8E RID: 31886
		private const string ENABLED_COLOUR = "#85ffa5";

		// Token: 0x04007C8F RID: 31887
		private const string DISABLED_COLOUR = "\"RED\"";

		// Token: 0x04007C90 RID: 31888
		private const string FAMILY_PORTAL_URL = "k-id.com/code";

		// Token: 0x04007C91 RID: 31889
		private float _updateAttemptCooldown = 15f;

		// Token: 0x04007C92 RID: 31890
		private float _nextUpdateAttemptTime;

		// Token: 0x04007C93 RID: 31891
		private bool _waitingForUpdatedSession;

		// Token: 0x04007C94 RID: 31892
		private GorillaComputer.EKidScreenState _currentScreentState = GorillaComputer.EKidScreenState.Show_OTP;

		// Token: 0x04007C95 RID: 31893
		private string[] _interestedPermissionNames = new string[] { "custom-username", "voice-chat", "join-groups" };

		// Token: 0x04007C96 RID: 31894
		private const string LANG_SCREEN_TITLE_KEY = "LANG_SCREEN_TITLE";

		// Token: 0x04007C97 RID: 31895
		private const string LANG_SCREEN_INSTRUCTIONS_KEY = "LANG_SCREEN_INSTRUCTIONS";

		// Token: 0x04007C98 RID: 31896
		private const string LANG_SCREEN_CURRENT_LANGUAGE_KEY = "LANG_SCREEN_CURRENT_LANGUAGE";

		// Token: 0x04007C99 RID: 31897
		private StringBuilder _languagesDisplaySB = new StringBuilder();

		// Token: 0x04007C9A RID: 31898
		private Locale _previousLocalisationSetting;

		// Token: 0x020010DC RID: 4316
		public enum ComputerState
		{
			// Token: 0x04007C9C RID: 31900
			Startup,
			// Token: 0x04007C9D RID: 31901
			Color,
			// Token: 0x04007C9E RID: 31902
			Name,
			// Token: 0x04007C9F RID: 31903
			Turn,
			// Token: 0x04007CA0 RID: 31904
			Mic,
			// Token: 0x04007CA1 RID: 31905
			Room,
			// Token: 0x04007CA2 RID: 31906
			Queue,
			// Token: 0x04007CA3 RID: 31907
			Group,
			// Token: 0x04007CA4 RID: 31908
			Voice,
			// Token: 0x04007CA5 RID: 31909
			AutoMute,
			// Token: 0x04007CA6 RID: 31910
			Credits,
			// Token: 0x04007CA7 RID: 31911
			Visuals,
			// Token: 0x04007CA8 RID: 31912
			Time,
			// Token: 0x04007CA9 RID: 31913
			NameWarning,
			// Token: 0x04007CAA RID: 31914
			Loading,
			// Token: 0x04007CAB RID: 31915
			Support,
			// Token: 0x04007CAC RID: 31916
			Troop,
			// Token: 0x04007CAD RID: 31917
			KID,
			// Token: 0x04007CAE RID: 31918
			Redemption,
			// Token: 0x04007CAF RID: 31919
			Language
		}

		// Token: 0x020010DD RID: 4317
		private enum NameCheckResult
		{
			// Token: 0x04007CB1 RID: 31921
			Success,
			// Token: 0x04007CB2 RID: 31922
			Warning,
			// Token: 0x04007CB3 RID: 31923
			Ban
		}

		// Token: 0x020010DE RID: 4318
		public enum RedemptionResult
		{
			// Token: 0x04007CB5 RID: 31925
			Empty,
			// Token: 0x04007CB6 RID: 31926
			Invalid,
			// Token: 0x04007CB7 RID: 31927
			Checking,
			// Token: 0x04007CB8 RID: 31928
			AlreadyUsed,
			// Token: 0x04007CB9 RID: 31929
			TooEarly,
			// Token: 0x04007CBA RID: 31930
			TooLate,
			// Token: 0x04007CBB RID: 31931
			AlreadyGranted,
			// Token: 0x04007CBC RID: 31932
			Success
		}

		// Token: 0x020010DF RID: 4319
		[Serializable]
		public class StateOrderItem
		{
			// Token: 0x06006C8D RID: 27789 RVA: 0x00231D52 File Offset: 0x0022FF52
			public StateOrderItem()
			{
			}

			// Token: 0x06006C8E RID: 27790 RVA: 0x00231D70 File Offset: 0x0022FF70
			public StateOrderItem(GorillaComputer.ComputerState state)
			{
				this.State = state;
			}

			// Token: 0x06006C8F RID: 27791 RVA: 0x00231D95 File Offset: 0x0022FF95
			public StateOrderItem(GorillaComputer.ComputerState state, string overrideName)
			{
				this.State = state;
				this.OverrideName = overrideName;
			}

			// Token: 0x06006C90 RID: 27792 RVA: 0x00231DC4 File Offset: 0x0022FFC4
			public string GetName()
			{
				if (this._previousLocale == LocalizationSettings.SelectedLocale && !string.IsNullOrEmpty(this._cachedTranslation))
				{
					return this._cachedTranslation;
				}
				if (this.StringReference == null || this.StringReference.IsEmpty)
				{
					return this.GetPreLocalisedName();
				}
				this._previousLocale = LocalizationSettings.SelectedLocale;
				string localizedString = this.StringReference.GetLocalizedString();
				this._cachedTranslation = ((localizedString != null) ? localizedString.ToUpper() : null);
				if (string.IsNullOrEmpty(this._cachedTranslation))
				{
					if (LocalisationManager.ApplicationRunning)
					{
						string[] array = new string[5];
						array[0] = "[LOCALIZATION::STATE_ORDER_ITEM] Failed to get translation for selected locale [";
						int num = 1;
						Locale previousLocale = this._previousLocale;
						array[num] = ((previousLocale != null) ? previousLocale.LocaleName : null) ?? "NULL";
						array[2] = ", for item [";
						array[3] = this.State.GetName<GorillaComputer.ComputerState>();
						array[4] = "]";
						Debug.LogError(string.Concat(array));
					}
					this._cachedTranslation = "";
				}
				return this._cachedTranslation;
			}

			// Token: 0x06006C91 RID: 27793 RVA: 0x00231EB4 File Offset: 0x002300B4
			public string GetPreLocalisedName()
			{
				if (!string.IsNullOrEmpty(this.OverrideName))
				{
					return this.OverrideName.ToUpper();
				}
				return this.State.ToString().ToUpper();
			}

			// Token: 0x04007CBD RID: 31933
			public GorillaComputer.ComputerState State;

			// Token: 0x04007CBE RID: 31934
			[Tooltip("Case not important - ToUpper applied at runtime")]
			public string OverrideName = "";

			// Token: 0x04007CBF RID: 31935
			public LocalizedString StringReference;

			// Token: 0x04007CC0 RID: 31936
			private Locale _previousLocale;

			// Token: 0x04007CC1 RID: 31937
			private string _cachedTranslation = "";
		}

		// Token: 0x020010E0 RID: 4320
		private enum EKidScreenState
		{
			// Token: 0x04007CC3 RID: 31939
			Ready,
			// Token: 0x04007CC4 RID: 31940
			Show_OTP,
			// Token: 0x04007CC5 RID: 31941
			Show_Setup_Screen
		}
	}
}
