using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000BD3 RID: 3027
public class KIDUI_MainScreen : MonoBehaviour
{
	// Token: 0x06004C44 RID: 19524 RVA: 0x00196791 File Offset: 0x00194991
	private void Awake()
	{
		KIDUI_MainScreen._featuresList.Clear();
		if (this._setupKidScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Setup K-ID Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._initialised)
		{
			return;
		}
		this.InitialiseMainScreen();
	}

	// Token: 0x06004C45 RID: 19525 RVA: 0x001967CA File Offset: 0x001949CA
	private void OnEnable()
	{
		KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.UpdatePermissionsAndFeaturesScreen));
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this.UpdatePermissionsAndFeaturesScreen();
	}

	// Token: 0x06004C46 RID: 19526 RVA: 0x001967F4 File Offset: 0x001949F4
	private void OnDisable()
	{
		KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.UpdatePermissionsAndFeaturesScreen));
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
		}
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004C47 RID: 19527 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnDestroy()
	{
	}

	// Token: 0x06004C48 RID: 19528 RVA: 0x0019682C File Offset: 0x00194A2C
	private void ConstructFeatureSettings()
	{
		for (int i = 0; i < this._displayOrder.Length; i++)
		{
			for (int j = 0; j < this._featureSetups.Count; j++)
			{
				if (this._featureSetups[j].linkedFeature == this._displayOrder[i])
				{
					this.CreateNewFeatureDisplay(this._featureSetups[j]);
					break;
				}
			}
		}
		this.UpdatePermissionsAndFeaturesScreen();
	}

	// Token: 0x06004C49 RID: 19529 RVA: 0x00196898 File Offset: 0x00194A98
	private void CreateNewFeatureDisplay(KIDUI_MainScreen.FeatureToggleSetup setup)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(setup.linkedFeature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID::UI::MAIN] Failed to retrieve permission data for feature; [" + setup.linkedFeature.ToString() + "]", Array.Empty<object>());
			return;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER)
		{
			if (permissionDataByFeature.Enabled)
			{
				return;
			}
			if (KIDManager.CheckFeatureOptIn(setup.linkedFeature, null).Item2)
			{
				return;
			}
		}
		if (setup.alwaysCheckFeatureSetting && KIDManager.CheckFeatureSettingEnabled(setup.linkedFeature))
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this._featurePrefab, this._featureRootTransform);
		KIDUIFeatureSetting component = gameObject.GetComponent<KIDUIFeatureSetting>();
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
		{
			Debug.LogFormat(string.Format("[KID::UI::MAIN_SCREEN] Adding new Locked Feature:  {0} Is enabled: {1}", setup.linkedFeature.ToString(), permissionDataByFeature.Enabled), Array.Empty<object>());
			component.CreateNewFeatureSettingGuardianManaged(setup, permissionDataByFeature.Enabled);
			if (!KIDUI_MainScreen._featuresList.ContainsKey(setup.linkedFeature))
			{
				KIDUI_MainScreen._featuresList.Add(setup.linkedFeature, new List<KIDUIFeatureSetting>());
			}
			KIDUI_MainScreen._featuresList[setup.linkedFeature].Add(component);
			return;
		}
		if (setup.requiresToggle)
		{
			component.CreateNewFeatureSettingWithToggle(setup, false, setup.alwaysCheckFeatureSetting);
		}
		else
		{
			component.CreateNewFeatureSettingWithoutToggle(setup, setup.alwaysCheckFeatureSetting);
		}
		if (!KIDUI_MainScreen._featuresList.ContainsKey(setup.linkedFeature))
		{
			KIDUI_MainScreen._featuresList.Add(setup.linkedFeature, new List<KIDUIFeatureSetting>());
		}
		KIDUI_MainScreen._featuresList[setup.linkedFeature].Add(component);
		this.ConstructAdditionalSetup(setup.linkedFeature, gameObject);
	}

	// Token: 0x06004C4A RID: 19530 RVA: 0x00196A34 File Offset: 0x00194C34
	private void ConstructAdditionalSetup(EKIDFeatures feature, GameObject featureObject)
	{
	}

	// Token: 0x06004C4B RID: 19531 RVA: 0x00196A3C File Offset: 0x00194C3C
	private void UpdatePermissionsAndFeaturesScreen()
	{
		int num = 0;
		Debug.LogFormat(string.Format("[KID::UI::MAIN] Updated Feature listings. To Update: [{0}]", KIDUI_MainScreen._featuresList.Count), Array.Empty<object>());
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair in KIDUI_MainScreen._featuresList)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(keyValuePair.Key);
				if (permissionDataByFeature == null)
				{
					Debug.LogErrorFormat("[KID::UI::MAIN] Failed to find permission data for feature: [" + keyValuePair.Key.ToString() + "]", Array.Empty<object>());
				}
				else if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
				{
					keyValuePair.Value[i].SetGuardianManagedState(permissionDataByFeature.Enabled);
				}
				else
				{
					bool flag = KIDManager.CheckFeatureOptIn(keyValuePair.Key, permissionDataByFeature).Item2;
					if (keyValuePair.Value[i].AlwaysCheckFeatureSetting)
					{
						flag = KIDManager.CheckFeatureSettingEnabled(keyValuePair.Key);
					}
					if (!keyValuePair.Value[i].GetHasToggle())
					{
						keyValuePair.Value[i].SetPlayerManagedState(permissionDataByFeature.Enabled, flag);
					}
				}
			}
		}
		int num2 = 0;
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair2 in KIDUI_MainScreen._featuresList)
		{
			for (int j = 0; j < keyValuePair2.Value.Count; j++)
			{
				num2++;
				Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(keyValuePair2.Key);
				if (keyValuePair2.Value[j].GetFeatureToggleState() || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PLAYER)
				{
					num++;
				}
			}
		}
		if (num >= num2)
		{
			if (!this._initialised)
			{
				this._titleFeaturePermissions.SetActive(false);
				this._titleGameFeatures.SetActive(true);
			}
			this._hasAllPermissions = true;
			this._getPermissionsButton.gameObject.SetActive(false);
			this._gettingPermissionsButton.gameObject.SetActive(false);
			this._requestPermissionsButton.gameObject.SetActive(false);
			this._permissionsTip.SetActive(false);
			this.SetButtonContainersVisibility(EGetPermissionsStatus.RequestedPermission);
		}
	}

	// Token: 0x06004C4C RID: 19532 RVA: 0x00196CC4 File Offset: 0x00194EC4
	private bool IsFeatureToggledOn(EKIDFeatures permissionFeature)
	{
		List<KIDUIFeatureSetting> list;
		if (!KIDUI_MainScreen._featuresList.TryGetValue(permissionFeature, out list))
		{
			return true;
		}
		KIDUIFeatureSetting kiduifeatureSetting = list.FirstOrDefault<KIDUIFeatureSetting>();
		if (kiduifeatureSetting == null)
		{
			Debug.LogErrorFormat(string.Format("[KID::UI::MAIN] Empty list for permission Name [{0}]", permissionFeature), Array.Empty<object>());
			return false;
		}
		return kiduifeatureSetting.GetFeatureToggleState();
	}

	// Token: 0x06004C4D RID: 19533 RVA: 0x00196D14 File Offset: 0x00194F14
	public void InitialiseMainScreen()
	{
		if (this._initialised)
		{
			Debug.Log("[KID::MAIN_SCREEN] Already Initialised");
			return;
		}
		this.ConstructFeatureSettings();
		this._declinedStatus.SetActive(false);
		this._timeoutStatus.SetActive(false);
		this._pendingStatus.SetActive(false);
		this._updatedStatus.SetActive(false);
		this._setupRequiredStatus.SetActive(false);
		this._missingStatus.SetActive(false);
		this._fullPlayerControlStatus.SetActive(false);
		this._initialised = true;
	}

	// Token: 0x06004C4E RID: 19534 RVA: 0x00196D98 File Offset: 0x00194F98
	public void ShowMainScreen(EMainScreenStatus showStatus, KIDUI_Controller.Metrics_ShowReason reason)
	{
		this.ShowMainScreen(showStatus);
		this._mainScreenOpenedReason = reason;
		string text = reason.ToString().Replace("_", "-").ToLower();
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_game_settings",
			CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment,
				KIDTelemetry.Open_MetricActionCustomTag
			},
			BodyData = new Dictionary<string, string> { { "screen_shown_reason", text } }
		};
		foreach (Permission permission in KIDManager.GetAllPermissionsData())
		{
			telemetryData.BodyData.Add(KIDTelemetry.GetPermissionManagedByBodyData(permission.Name), permission.ManagedBy.ToString().ToLower());
			telemetryData.BodyData.Add(KIDTelemetry.GetPermissionEnabledBodyData(permission.Name), permission.Enabled.ToString().ToLower());
		}
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
	}

	// Token: 0x06004C4F RID: 19535 RVA: 0x00196EE4 File Offset: 0x001950E4
	public void ShowMainScreen(EMainScreenStatus showStatus)
	{
		KIDUI_MainScreen.ShownSettingsScreen = true;
		base.gameObject.SetActive(true);
		this.ConfigurePermissionsButtons();
		this.UpdateScreenStatus(showStatus, false);
	}

	// Token: 0x06004C50 RID: 19536 RVA: 0x00196F08 File Offset: 0x00195108
	public void UpdateScreenStatus(EMainScreenStatus showStatus, bool sendMetrics = false)
	{
		if (sendMetrics && showStatus == EMainScreenStatus.Updated)
		{
			string text = this._mainScreenOpenedReason.ToString().Replace("_", "-").ToLower();
			TelemetryData telemetryData = new TelemetryData
			{
				EventName = "kid_game_settings",
				CustomTags = new string[]
				{
					"kid_setup",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment,
					KIDTelemetry.Updated_MetricActionCustomTag
				},
				BodyData = new Dictionary<string, string> { { "screen_shown_reason", text } }
			};
			foreach (Permission permission in KIDManager.GetAllPermissionsData())
			{
				telemetryData.BodyData.Add(KIDTelemetry.GetPermissionManagedByBodyData(permission.Name), permission.ManagedBy.ToString().ToLower());
				telemetryData.BodyData.Add(KIDTelemetry.GetPermissionEnabledBodyData(permission.Name), permission.Enabled.ToString().ToLower());
			}
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		}
		GameObject activeStatusObject = this.GetActiveStatusObject();
		this._declinedStatus.SetActive(false);
		this._timeoutStatus.SetActive(false);
		this._pendingStatus.SetActive(false);
		this._updatedStatus.SetActive(false);
		this._setupRequiredStatus.SetActive(false);
		this._missingStatus.SetActive(false);
		this._fullPlayerControlStatus.SetActive(false);
		switch (showStatus)
		{
		default:
			if (!this._hasAllPermissions)
			{
				this._missingStatus.SetActive(true);
			}
			else if (this._hasAllPermissions)
			{
				this._fullPlayerControlStatus.SetActive(true);
			}
			else
			{
				this._screenStatus = showStatus;
			}
			break;
		case EMainScreenStatus.Declined:
			this._declinedStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Pending:
			this._pendingStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Timedout:
			this._timeoutStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Setup:
			this._setupRequiredStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Previous:
			if (activeStatusObject != null)
			{
				activeStatusObject.SetActive(true);
			}
			else
			{
				this._updatedStatus.SetActive(true);
			}
			break;
		case EMainScreenStatus.FullControl:
			this._fullPlayerControlStatus.SetActive(true);
			break;
		}
		this.SetButtonContainersVisibility(KIDUI_MainScreen.GetPermissionState());
	}

	// Token: 0x06004C51 RID: 19537 RVA: 0x00044B04 File Offset: 0x00042D04
	public void HideMainScreen()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06004C52 RID: 19538 RVA: 0x001971A4 File Offset: 0x001953A4
	public async void OnAskForPermission()
	{
		this._requestPermissionsButton.interactable = false;
		this._getPermissionsButton.interactable = false;
		this._gettingPermissionsButton.interactable = false;
		await this._animatedEllipsis.StartAnimation();
		bool missingPermissionsPostUpdate = await this.UpdateAndCheckForMissingPermissions();
		this._requestPermissionsButton.interactable = true;
		this._getPermissionsButton.interactable = true;
		this._gettingPermissionsButton.interactable = true;
		await this._animatedEllipsis.StopAnimation();
		if (missingPermissionsPostUpdate)
		{
			base.gameObject.SetActive(false);
			if (KIDManager.CurrentSession.IsDefault)
			{
				this._setupKidScreen.OnStartSetup();
			}
			else
			{
				List<string> list = new List<string>(this.CollectPermissionsToUpgrade());
				await this._sendUpgradeEmailScreen.SendUpgradeEmail(list);
				if (KIDManager.CurrentSession.ManagedBy == Session.ManagedByEnum.PLAYER)
				{
					this._setupKidScreen.OnStartSetup();
				}
				KIDManager.WaitForAndUpdateNewSession(true);
			}
		}
	}

	// Token: 0x06004C53 RID: 19539 RVA: 0x001971DC File Offset: 0x001953DC
	public void OnSaveAndExit()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::KID_UI_MAINSCREEN] There is no session as such cannot opt into anything");
			KIDUI_Controller.Instance.CloseKIDScreens();
			return;
		}
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		for (int i = 0; i < allPermissionsData.Count; i++)
		{
			string name = allPermissionsData[i].Name;
			if (!(name == "multiplayer"))
			{
				if (!(name == "mods"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "voice-chat"))
						{
							if (!(name == "custom-username"))
							{
								Debug.LogError("[KID::UI::MainScreen] Unhandled permission when saving and exiting: [" + allPermissionsData[i].Name + "]");
							}
							else
							{
								this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Custom_Nametags, delegate(bool b, Permission p, bool hasOptedInPreviously)
								{
									GorillaComputer.instance.SetNametagSetting(b, p.ManagedBy, hasOptedInPreviously);
								});
							}
						}
						else
						{
							this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Voice_Chat, delegate(bool b, Permission p, bool hasOptedInPreviously)
							{
								GorillaComputer.instance.KID_SetVoiceChatSettingOnStart(b, p.ManagedBy, hasOptedInPreviously);
							});
						}
					}
				}
				else
				{
					this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Mods, null);
				}
			}
			else
			{
				this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Multiplayer, null);
			}
		}
		KIDManager.SendOptInPermissions();
		if (this._screenStatus != EMainScreenStatus.None)
		{
			string text = this._mainScreenOpenedReason.ToString().Replace("_", "-").ToLower();
			TelemetryData telemetryData = new TelemetryData
			{
				EventName = "kid_game_settings",
				CustomTags = new string[]
				{
					"kid_setup",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string>
				{
					{ "screen_shown_reason", text },
					{
						"kid_status",
						this._screenStatus.ToString().ToLower()
					},
					{ "button_pressed", "save_and_continue" }
				}
			};
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		}
		else
		{
			Debug.LogError("[KID::UI::MAIN_SCREEN] Trying to close k-ID Main Screen, but screen status is set to [None] - Invalid status, will not submit analytics");
		}
		KIDUI_Controller.Instance.CloseKIDScreens();
	}

	// Token: 0x06004C54 RID: 19540 RVA: 0x00197408 File Offset: 0x00195608
	public int GetFeatureListingCount()
	{
		int num = 0;
		foreach (List<KIDUIFeatureSetting> list in KIDUI_MainScreen._featuresList.Values)
		{
			num += list.Count;
		}
		return num;
	}

	// Token: 0x06004C55 RID: 19541 RVA: 0x00197464 File Offset: 0x00195664
	private async Task<bool> UpdateAndCheckForMissingPermissions()
	{
		bool hasUpdated = false;
		bool wasSuccess = false;
		float cutOffDuration = Time.realtimeSinceStartup + 15f;
		KIDManager.UpdateSession(delegate(bool success)
		{
			hasUpdated = true;
			wasSuccess = success;
		});
		do
		{
			await Task.Yield();
		}
		while (Time.realtimeSinceStartup < cutOffDuration && !hasUpdated);
		this.UpdatePermissionsAndFeaturesScreen();
		if (wasSuccess)
		{
			bool flag = false;
			foreach (Permission permission in KIDManager.CurrentSession.GetAllPermissions())
			{
				if (permission.ManagedBy == Permission.ManagedByEnum.GUARDIAN && !permission.Enabled)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.UpdateScreenStatus(EMainScreenStatus.FullControl, false);
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004C56 RID: 19542 RVA: 0x001974A8 File Offset: 0x001956A8
	private void OnLanguageChanged()
	{
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair in KIDUI_MainScreen._featuresList)
		{
			List<KIDUIFeatureSetting> value = keyValuePair.Value;
			if (value != null)
			{
				for (int i = 0; i < value.Count; i++)
				{
					if (value[i] != null)
					{
						value[i].RefreshTextOnLanguageChanged();
					}
				}
			}
		}
	}

	// Token: 0x06004C57 RID: 19543 RVA: 0x0019752C File Offset: 0x0019572C
	private void UpdateOptInSetting(Permission permissionData, EKIDFeatures feature, Action<bool, Permission, bool> onOptedIn)
	{
		bool item = KIDManager.CheckFeatureOptIn(feature, permissionData).Item2;
		bool flag = this.IsFeatureToggledOn(feature);
		Debug.Log(string.Format("[KID::UI::MainScreen] Update opt in for {0}. Has opted in: {1}. Toggled on: {2}", feature.ToString(), item, flag));
		KIDManager.SetFeatureOptIn(feature, flag);
		if (onOptedIn != null)
		{
			onOptedIn(flag, permissionData, item);
		}
	}

	// Token: 0x06004C58 RID: 19544 RVA: 0x00197589 File Offset: 0x00195789
	public void OnConfirmedEmailAddress(string emailAddress)
	{
		this._emailAddress = emailAddress;
		Debug.LogFormat("[KID::UI::Main] Email has been confirmed: " + this._emailAddress, Array.Empty<object>());
	}

	// Token: 0x06004C59 RID: 19545 RVA: 0x001975AC File Offset: 0x001957AC
	private IEnumerable<string> CollectPermissionsToUpgrade()
	{
		return from permission in KIDManager.GetAllPermissionsData()
			where permission.ManagedBy == Permission.ManagedByEnum.GUARDIAN && !permission.Enabled
			select permission.Name;
	}

	// Token: 0x06004C5A RID: 19546 RVA: 0x00197608 File Offset: 0x00195808
	private void ConfigurePermissionsButtons()
	{
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS");
		if (!this._getPermissionsButton.gameObject.activeSelf && !this._gettingPermissionsButton.gameObject.activeSelf)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - GET PERMISSIONS IS DISABLED");
			return;
		}
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - CHECK SESSION STATUS: Is Default: [" + KIDManager.CurrentSession.IsDefault.ToString() + "]");
		this.SetButtonContainersVisibility(KIDUI_MainScreen.GetPermissionState());
	}

	// Token: 0x06004C5B RID: 19547 RVA: 0x0019767C File Offset: 0x0019587C
	private void SetButtonContainersVisibility(EGetPermissionsStatus permissionStatus)
	{
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - PERMISSION STATE: [" + permissionStatus.ToString() + "]");
		this._defaultButtonsContainer.SetActive(permissionStatus == EGetPermissionsStatus.GetPermission);
		this._permissionsRequestingButtonContainer.SetActive(permissionStatus == EGetPermissionsStatus.RequestingPermission);
		this._permissionsRequestedButtonContainer.SetActive(permissionStatus == EGetPermissionsStatus.RequestedPermission);
	}

	// Token: 0x06004C5C RID: 19548 RVA: 0x001976D8 File Offset: 0x001958D8
	private GameObject GetActiveStatusObject()
	{
		foreach (GameObject gameObject in new List<GameObject> { this._declinedStatus, this._timeoutStatus, this._pendingStatus, this._updatedStatus, this._setupRequiredStatus, this._fullPlayerControlStatus })
		{
			if (gameObject.activeInHierarchy)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x06004C5D RID: 19549 RVA: 0x0019777C File Offset: 0x0019597C
	private static EGetPermissionsStatus GetPermissionState()
	{
		if (!KIDManager.CurrentSession.IsDefault)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW REQUESTED");
			return EGetPermissionsStatus.RequestedPermission;
		}
		if (PlayerPrefs.GetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 0) == 0)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW DEFAULT");
			return EGetPermissionsStatus.GetPermission;
		}
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW SWAPPED DEFAULT");
		return EGetPermissionsStatus.RequestingPermission;
	}

	// Token: 0x06004C5E RID: 19550 RVA: 0x001977BC File Offset: 0x001959BC
	private void OnFeatureToggleChanged(EKIDFeatures feature)
	{
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
			this.OnMultiplayerToggled();
			return;
		case EKIDFeatures.Custom_Nametags:
			this.OnCustomNametagsToggled();
			return;
		case EKIDFeatures.Voice_Chat:
			this.OnVoiceChatToggled();
			return;
		case EKIDFeatures.Mods:
			this.OnModToggleChanged();
			return;
		case EKIDFeatures.Groups:
			this.OnGroupToggleChanged();
			return;
		default:
			Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] Toggle NOT YET IMPLEMENTED for Feature: " + feature.ToString() + ".", Array.Empty<object>());
			return;
		}
	}

	// Token: 0x06004C5F RID: 19551 RVA: 0x0019782E File Offset: 0x00195A2E
	private void OnMultiplayerToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] MULTIPLAYER Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06004C60 RID: 19552 RVA: 0x0019783F File Offset: 0x00195A3F
	private void OnVoiceChatToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] VOICE CHAT Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06004C61 RID: 19553 RVA: 0x00197850 File Offset: 0x00195A50
	private void OnGroupToggleChanged()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] GROUPS Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06004C62 RID: 19554 RVA: 0x00197861 File Offset: 0x00195A61
	private void OnModToggleChanged()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] MODS Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06004C63 RID: 19555 RVA: 0x00197872 File Offset: 0x00195A72
	private void OnCustomNametagsToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] CUSTOM USERNAMES Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x04005F43 RID: 24387
	public const string OPT_IN_SUFFIX = "-opt-in";

	// Token: 0x04005F44 RID: 24388
	public static bool ShownSettingsScreen = false;

	// Token: 0x04005F45 RID: 24389
	[SerializeField]
	private GameObject _kidScreensGroup;

	// Token: 0x04005F46 RID: 24390
	[SerializeField]
	private KIDUI_SetupScreen _setupKidScreen;

	// Token: 0x04005F47 RID: 24391
	[SerializeField]
	private KIDUI_SendUpgradeEmailScreen _sendUpgradeEmailScreen;

	// Token: 0x04005F48 RID: 24392
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x04005F49 RID: 24393
	[Header("Permission Request Buttons")]
	[SerializeField]
	private KIDUIButton _getPermissionsButton;

	// Token: 0x04005F4A RID: 24394
	[SerializeField]
	private KIDUIButton _gettingPermissionsButton;

	// Token: 0x04005F4B RID: 24395
	[SerializeField]
	private KIDUIButton _requestPermissionsButton;

	// Token: 0x04005F4C RID: 24396
	[SerializeField]
	private GameObject _defaultButtonsContainer;

	// Token: 0x04005F4D RID: 24397
	[SerializeField]
	private GameObject _permissionsRequestingButtonContainer;

	// Token: 0x04005F4E RID: 24398
	[SerializeField]
	private GameObject _permissionsRequestedButtonContainer;

	// Token: 0x04005F4F RID: 24399
	private bool _hasAllPermissions;

	// Token: 0x04005F50 RID: 24400
	[Header("Dynamic Feature Settings Setup")]
	[SerializeField]
	private GameObject _featurePrefab;

	// Token: 0x04005F51 RID: 24401
	[SerializeField]
	private Transform _featureRootTransform;

	// Token: 0x04005F52 RID: 24402
	[SerializeField]
	private EKIDFeatures[] _displayOrder = new EKIDFeatures[4];

	// Token: 0x04005F53 RID: 24403
	[SerializeField]
	private List<KIDUI_MainScreen.FeatureToggleSetup> _featureSetups = new List<KIDUI_MainScreen.FeatureToggleSetup>();

	// Token: 0x04005F54 RID: 24404
	[Header("Additional Feature-Specific Setup")]
	[SerializeField]
	private GameObject _voiceChatLabel;

	// Token: 0x04005F55 RID: 24405
	[Header("Hide Permissions Tip")]
	[SerializeField]
	private GameObject _permissionsTip;

	// Token: 0x04005F56 RID: 24406
	[Header("Titles")]
	[SerializeField]
	private GameObject _titleFeaturePermissions;

	// Token: 0x04005F57 RID: 24407
	[SerializeField]
	private GameObject _titleGameFeatures;

	// Token: 0x04005F58 RID: 24408
	[Header("Game Status Setup")]
	[SerializeField]
	private GameObject _missingStatus;

	// Token: 0x04005F59 RID: 24409
	[SerializeField]
	private GameObject _updatedStatus;

	// Token: 0x04005F5A RID: 24410
	[SerializeField]
	private GameObject _declinedStatus;

	// Token: 0x04005F5B RID: 24411
	[SerializeField]
	private GameObject _pendingStatus;

	// Token: 0x04005F5C RID: 24412
	[SerializeField]
	private GameObject _timeoutStatus;

	// Token: 0x04005F5D RID: 24413
	[SerializeField]
	private GameObject _setupRequiredStatus;

	// Token: 0x04005F5E RID: 24414
	[SerializeField]
	private GameObject _fullPlayerControlStatus;

	// Token: 0x04005F5F RID: 24415
	private string _emailAddress;

	// Token: 0x04005F60 RID: 24416
	private bool _multiplayerEnabled;

	// Token: 0x04005F61 RID: 24417
	private bool _customNameEnabled;

	// Token: 0x04005F62 RID: 24418
	private bool _voiceChatEnabled;

	// Token: 0x04005F63 RID: 24419
	private bool _initialised;

	// Token: 0x04005F64 RID: 24420
	private KIDUI_Controller.Metrics_ShowReason _mainScreenOpenedReason;

	// Token: 0x04005F65 RID: 24421
	private EMainScreenStatus _screenStatus;

	// Token: 0x04005F66 RID: 24422
	private GameObject _eventSystemObj;

	// Token: 0x04005F67 RID: 24423
	private static Dictionary<EKIDFeatures, List<KIDUIFeatureSetting>> _featuresList = new Dictionary<EKIDFeatures, List<KIDUIFeatureSetting>>();

	// Token: 0x02000BD4 RID: 3028
	[Serializable]
	public struct FeatureToggleSetup
	{
		// Token: 0x04005F68 RID: 24424
		public EKIDFeatures linkedFeature;

		// Token: 0x04005F69 RID: 24425
		public string permissionName;

		// Token: 0x04005F6A RID: 24426
		public LocalizedString featureName;

		// Token: 0x04005F6B RID: 24427
		public bool requiresToggle;

		// Token: 0x04005F6C RID: 24428
		public bool alwaysCheckFeatureSetting;

		// Token: 0x04005F6D RID: 24429
		public LocalizedString enabledText;

		// Token: 0x04005F6E RID: 24430
		public LocalizedString disabledText;
	}
}
