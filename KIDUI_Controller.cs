using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using UnityEngine;

// Token: 0x02000BCB RID: 3019
public class KIDUI_Controller : MonoBehaviour
{
	// Token: 0x17000749 RID: 1865
	// (get) Token: 0x06004C14 RID: 19476 RVA: 0x0019598A File Offset: 0x00193B8A
	public static KIDUI_Controller Instance
	{
		get
		{
			return KIDUI_Controller._instance;
		}
	}

	// Token: 0x1700074A RID: 1866
	// (get) Token: 0x06004C15 RID: 19477 RVA: 0x00195991 File Offset: 0x00193B91
	public static bool IsKIDUIActive
	{
		get
		{
			return !(KIDUI_Controller.Instance == null) && KIDUI_Controller.Instance._isKidUIActive;
		}
	}

	// Token: 0x1700074B RID: 1867
	// (get) Token: 0x06004C16 RID: 19478 RVA: 0x001959AC File Offset: 0x00193BAC
	private static string EtagOnCloseBlackScreenPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr))
			{
				KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr = "closeBlackScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr;
		}
	}

	// Token: 0x06004C17 RID: 19479 RVA: 0x001959DA File Offset: 0x00193BDA
	private void Awake()
	{
		KIDUI_Controller._instance = this;
		Debug.LogFormat("[KID::UI::CONTROLLER] Controller Initialised", Array.Empty<object>());
	}

	// Token: 0x06004C18 RID: 19480 RVA: 0x001959F1 File Offset: 0x00193BF1
	private void OnDestroy()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x06004C19 RID: 19481 RVA: 0x00195A14 File Offset: 0x00193C14
	public async Task StartKIDScreens(CancellationToken cancellationToken)
	{
		Debug.LogFormat("[KID::UI::CONTROLLER] Starting k-ID Screens", Array.Empty<object>());
		bool flag = await this.ShouldShowKIDScreen(cancellationToken);
		if (!cancellationToken.IsCancellationRequested)
		{
			if (!flag)
			{
				Debug.LogFormat("[KID::UI::CONTROLLER] Should NOT Show k-ID Screens", Array.Empty<object>());
			}
			else
			{
				PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.KID, "");
				Debug.LogFormat("[KID::UI::CONTROLLER] Showing k-ID Screens", Array.Empty<object>());
				while (HandRayController.Instance == null)
				{
					await Task.Yield();
				}
				HandRayController.Instance.EnableHandRays();
				PrivateUIRoom.AddUI(base.transform);
				EMainScreenStatus screenStatusFromSession = this.GetScreenStatusFromSession();
				this._mainKIDScreen.ShowMainScreen(screenStatusFromSession, this._showReason);
				this._isKidUIActive = true;
				KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Combine(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
			}
		}
	}

	// Token: 0x06004C1A RID: 19482 RVA: 0x00195A60 File Offset: 0x00193C60
	public void CloseKIDScreens()
	{
		this.SaveEtagOnCloseScreen();
		this._isKidUIActive = false;
		this._mainKIDScreen.HideMainScreen();
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
		}
		PrivateUIRoom.RemoveUI(base.transform);
		HandRayController.Instance.DisableHandRays();
		Object.DestroyImmediate(base.gameObject);
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x06004C1B RID: 19483 RVA: 0x00195AD8 File Offset: 0x00193CD8
	public void UpdateScreenStatus()
	{
		EMainScreenStatus screenStatusFromSession = this.GetScreenStatusFromSession();
		KIDUI_MainScreen mainKIDScreen = this._mainKIDScreen;
		if (mainKIDScreen == null)
		{
			return;
		}
		mainKIDScreen.UpdateScreenStatus(screenStatusFromSession, true);
	}

	// Token: 0x06004C1C RID: 19484 RVA: 0x00195B00 File Offset: 0x00193D00
	public void NotifyOfEmailResult(bool success)
	{
		if (this._confirmScreen == null)
		{
			Debug.LogError("[KID::UI_CONTROLLER] _confirmScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		if (success)
		{
			PlayerPrefs.SetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 1);
			PlayerPrefs.Save();
		}
		Debug.Log("[KID::UI_CONTROLLER] Notifying user about email result. Showing confirm screen.");
		this._confirmScreen.NotifyOfResult(success);
	}

	// Token: 0x06004C1D RID: 19485 RVA: 0x00195B50 File Offset: 0x00193D50
	private EMainScreenStatus GetScreenStatusFromSession()
	{
		EMainScreenStatus emainScreenStatus;
		switch (KIDManager.CurrentSession.SessionStatus)
		{
		case SessionStatus.PASS:
			if (this.ShouldShowScreenOnPermissionChange())
			{
				emainScreenStatus = EMainScreenStatus.Updated;
			}
			else if (KIDManager.PreviousStatus == SessionStatus.CHALLENGE_SESSION_UPGRADE)
			{
				emainScreenStatus = EMainScreenStatus.Declined;
			}
			else
			{
				emainScreenStatus = EMainScreenStatus.Missing;
			}
			break;
		case SessionStatus.PROHIBITED:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Status is PROHIBITED but is trying to show k-ID screens");
			emainScreenStatus = EMainScreenStatus.Declined;
			break;
		case SessionStatus.CHALLENGE:
		case SessionStatus.CHALLENGE_SESSION_UPGRADE:
		case SessionStatus.PENDING_AGE_APPEAL:
			if (string.IsNullOrEmpty(PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "")))
			{
				emainScreenStatus = EMainScreenStatus.Setup;
			}
			else
			{
				emainScreenStatus = EMainScreenStatus.Pending;
			}
			break;
		default:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Unknown status");
			emainScreenStatus = EMainScreenStatus.None;
			break;
		}
		return emainScreenStatus;
	}

	// Token: 0x06004C1E RID: 19486 RVA: 0x00195BDC File Offset: 0x00193DDC
	private async Task<bool> ShouldShowKIDScreen(CancellationToken cancellationToken)
	{
		bool flag;
		if (KIDManager.CurrentSession == null)
		{
			this._showReason = KIDUI_Controller.Metrics_ShowReason.No_Session;
			flag = true;
		}
		else
		{
			if (!KIDManager.CurrentSession.IsValidSession)
			{
				while (!KIDManager.CurrentSession.IsValidSession)
				{
					Debug.Log("[KID::UI::CONTROLLER] K-ID Session not found yet");
					await Task.Delay(100, cancellationToken);
				}
			}
			Debug.Log("[KID::UI::CONTROLLER] K-ID Session has been found and is proceeding ");
			if (KIDManager.HasAllPermissions())
			{
				flag = false;
			}
			else
			{
				for (int i = 0; i < this._inaccessibleSettings.Count; i++)
				{
					Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(this._inaccessibleSettings[i]);
					if (permissionDataByFeature == null)
					{
						Debug.LogErrorFormat(string.Format("[KID::UI::CONTROLLER] Failed to get Permission with name [{0}]", this._inaccessibleSettings[i]), Array.Empty<object>());
						return true;
					}
					if (permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED && !KIDManager.CheckFeatureSettingEnabled(this._inaccessibleSettings[i]))
					{
						this._showReason = KIDUI_Controller.Metrics_ShowReason.Inaccessible;
						if (KIDManager.CurrentSession.IsDefault)
						{
							this._showReason = KIDUI_Controller.Metrics_ShowReason.Default_Session;
						}
						return true;
					}
				}
				List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
				for (int j = 0; j < allPermissionsData.Count; j++)
				{
					if (allPermissionsData[j].ManagedBy == Permission.ManagedByEnum.GUARDIAN && !allPermissionsData[j].Enabled)
					{
						this._showReason = KIDUI_Controller.Metrics_ShowReason.Guardian_Disabled;
						if (KIDManager.CurrentSession.IsDefault)
						{
							this._showReason = KIDUI_Controller.Metrics_ShowReason.Default_Session;
						}
						return true;
					}
				}
				this._mainKIDScreen.InitialiseMainScreen();
				if (this._mainKIDScreen.GetFeatureListingCount() == 0)
				{
					Debug.Log("[KID::CONTROLLER] Nothing to show on k-ID UI. Skipping");
					flag = false;
				}
				else if (this.ShouldShowScreenOnPermissionChange())
				{
					this._showReason = KIDUI_Controller.Metrics_ShowReason.Permissions_Changed;
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
		}
		return flag;
	}

	// Token: 0x06004C1F RID: 19487 RVA: 0x00195C27 File Offset: 0x00193E27
	private bool ShouldShowScreenOnPermissionChange()
	{
		this._lastEtagOnClose = this.GetLastBlackScreenEtag();
		string lastEtagOnClose = this._lastEtagOnClose;
		TMPSession currentSession = KIDManager.CurrentSession;
		return lastEtagOnClose != (((currentSession != null) ? currentSession.Etag : null) ?? string.Empty);
	}

	// Token: 0x06004C20 RID: 19488 RVA: 0x00195C5A File Offset: 0x00193E5A
	private string GetLastBlackScreenEtag()
	{
		return PlayerPrefs.GetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, "");
	}

	// Token: 0x06004C21 RID: 19489 RVA: 0x00195C6B File Offset: 0x00193E6B
	private void SaveEtagOnCloseScreen()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.Log("[KID::MANAGER] Trying to save Pre-Game Screen ETAG, but [CurrentSession] is null");
			return;
		}
		PlayerPrefs.SetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, KIDManager.CurrentSession.Etag);
		PlayerPrefs.Save();
	}

	// Token: 0x06004C22 RID: 19490 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005F11 RID: 24337
	private const string CLOSE_BLACK_SCREEN_ETAG_PLAYER_PREF_PREFIX = "closeBlackScreen-";

	// Token: 0x04005F12 RID: 24338
	private const string FIRST_TIME_POST_CHANGE_PLAYER_PREF = "hasShownFirstTimePostChange-";

	// Token: 0x04005F13 RID: 24339
	private static KIDUI_Controller _instance;

	// Token: 0x04005F14 RID: 24340
	[SerializeField]
	private KIDUI_MainScreen _mainKIDScreen;

	// Token: 0x04005F15 RID: 24341
	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	// Token: 0x04005F16 RID: 24342
	[SerializeField]
	private List<string> _PermissionsWithToggles = new List<string>();

	// Token: 0x04005F17 RID: 24343
	[SerializeField]
	private List<EKIDFeatures> _inaccessibleSettings = new List<EKIDFeatures>
	{
		EKIDFeatures.Multiplayer,
		EKIDFeatures.Mods
	};

	// Token: 0x04005F18 RID: 24344
	private KIDUI_Controller.Metrics_ShowReason _showReason;

	// Token: 0x04005F19 RID: 24345
	private bool _isKidUIActive;

	// Token: 0x04005F1A RID: 24346
	private static string etagOnCloseBlackScreenPlayerPrefStr;

	// Token: 0x04005F1B RID: 24347
	private string _lastEtagOnClose;

	// Token: 0x02000BCC RID: 3020
	public enum Metrics_ShowReason
	{
		// Token: 0x04005F1D RID: 24349
		None,
		// Token: 0x04005F1E RID: 24350
		Inaccessible,
		// Token: 0x04005F1F RID: 24351
		Guardian_Disabled,
		// Token: 0x04005F20 RID: 24352
		Permissions_Changed,
		// Token: 0x04005F21 RID: 24353
		Default_Session,
		// Token: 0x04005F22 RID: 24354
		No_Session
	}
}
