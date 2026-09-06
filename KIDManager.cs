using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using Modio;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000B62 RID: 2914
public class KIDManager : MonoBehaviour
{
	// Token: 0x17000719 RID: 1817
	// (get) Token: 0x06004A02 RID: 18946 RVA: 0x0018A4AB File Offset: 0x001886AB
	public static KIDManager Instance
	{
		get
		{
			return KIDManager._instance;
		}
	}

	// Token: 0x1700071A RID: 1818
	// (get) Token: 0x06004A03 RID: 18947 RVA: 0x0018A4B2 File Offset: 0x001886B2
	// (set) Token: 0x06004A04 RID: 18948 RVA: 0x0018A4B9 File Offset: 0x001886B9
	public static bool InitialisationComplete { get; private set; } = false;

	// Token: 0x1700071B RID: 1819
	// (get) Token: 0x06004A05 RID: 18949 RVA: 0x0018A4C1 File Offset: 0x001886C1
	// (set) Token: 0x06004A06 RID: 18950 RVA: 0x0018A4C8 File Offset: 0x001886C8
	public static bool InitialisationSuccessful { get; private set; } = false;

	// Token: 0x1700071C RID: 1820
	// (get) Token: 0x06004A07 RID: 18951 RVA: 0x0018A4D0 File Offset: 0x001886D0
	// (set) Token: 0x06004A08 RID: 18952 RVA: 0x0018A4D7 File Offset: 0x001886D7
	public static TMPSession CurrentSession { get; private set; }

	// Token: 0x1700071D RID: 1821
	// (get) Token: 0x06004A09 RID: 18953 RVA: 0x0018A4DF File Offset: 0x001886DF
	// (set) Token: 0x06004A0A RID: 18954 RVA: 0x0018A4E6 File Offset: 0x001886E6
	public static SessionStatus PreviousStatus { get; private set; }

	// Token: 0x1700071E RID: 1822
	// (get) Token: 0x06004A0B RID: 18955 RVA: 0x0018A4EE File Offset: 0x001886EE
	// (set) Token: 0x06004A0C RID: 18956 RVA: 0x0018A4F5 File Offset: 0x001886F5
	public static GetRequirementsData _ageGateRequirements { get; private set; }

	// Token: 0x1700071F RID: 1823
	// (get) Token: 0x06004A0D RID: 18957 RVA: 0x0018A4FD File Offset: 0x001886FD
	public static bool KidTitleDataReady
	{
		get
		{
			return KIDManager._titleDataReady;
		}
	}

	// Token: 0x17000720 RID: 1824
	// (get) Token: 0x06004A0E RID: 18958 RVA: 0x0018A504 File Offset: 0x00188704
	public static bool KidEnabled
	{
		get
		{
			return KIDManager.KidTitleDataReady && KIDManager._useKid;
		}
	}

	// Token: 0x17000721 RID: 1825
	// (get) Token: 0x06004A0F RID: 18959 RVA: 0x0018A514 File Offset: 0x00188714
	public static bool KidEnabledAndReady
	{
		get
		{
			return KIDManager.KidEnabled && KIDManager.InitialisationSuccessful;
		}
	}

	// Token: 0x17000722 RID: 1826
	// (get) Token: 0x06004A10 RID: 18960 RVA: 0x0018A524 File Offset: 0x00188724
	public static bool HasSession
	{
		get
		{
			return KIDManager.CurrentSession != null && KIDManager.CurrentSession.SessionId != Guid.Empty;
		}
	}

	// Token: 0x17000723 RID: 1827
	// (get) Token: 0x06004A11 RID: 18961 RVA: 0x0018A543 File Offset: 0x00188743
	public static string PreviousStatusPlayerPrefRef
	{
		get
		{
			return "previous-status-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x17000724 RID: 1828
	// (get) Token: 0x06004A12 RID: 18962 RVA: 0x0018A55B File Offset: 0x0018875B
	// (set) Token: 0x06004A13 RID: 18963 RVA: 0x0018A562 File Offset: 0x00188762
	public static bool HasOptedInToKID { get; private set; }

	// Token: 0x17000725 RID: 1829
	// (get) Token: 0x06004A14 RID: 18964 RVA: 0x0018A56A File Offset: 0x0018876A
	private static string KIDSetupPlayerPref
	{
		get
		{
			return "KID-Setup-";
		}
	}

	// Token: 0x17000726 RID: 1830
	// (get) Token: 0x06004A15 RID: 18965 RVA: 0x0018A571 File Offset: 0x00188771
	// (set) Token: 0x06004A16 RID: 18966 RVA: 0x0018A578 File Offset: 0x00188778
	public static string DbgLocale { get; set; }

	// Token: 0x17000727 RID: 1831
	// (get) Token: 0x06004A17 RID: 18967 RVA: 0x0018A580 File Offset: 0x00188780
	public static string DebugKIDLocalePlayerPrefRef
	{
		get
		{
			return KIDManager._debugKIDLocalePlayerPrefRef;
		}
	}

	// Token: 0x17000728 RID: 1832
	// (get) Token: 0x06004A18 RID: 18968 RVA: 0x0018A587 File Offset: 0x00188787
	public static string GetEmailForUserPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDManager.parentEmailForUserPlayerPrefRef))
			{
				KIDManager.parentEmailForUserPlayerPrefRef = "k-id_EmailAddress" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDManager.parentEmailForUserPlayerPrefRef;
		}
	}

	// Token: 0x17000729 RID: 1833
	// (get) Token: 0x06004A19 RID: 18969 RVA: 0x0018A5B5 File Offset: 0x001887B5
	public static string GetChallengedBeforePlayerPrefRef
	{
		get
		{
			return "k-id_ChallengedBefore" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06004A1A RID: 18970 RVA: 0x0018A5D0 File Offset: 0x001887D0
	private void Awake()
	{
		if (KIDManager._instance != null)
		{
			Debug.LogError("Trying to create new instance of [KIDManager], but one already exists. Destroying object [" + base.gameObject.name + "].");
			Object.Destroy(base.gameObject);
			return;
		}
		KIDManager._instance = this;
		KIDManager.DbgLocale = PlayerPrefs.GetString(KIDManager._debugKIDLocalePlayerPrefRef, "");
	}

	// Token: 0x06004A1B RID: 18971 RVA: 0x0018A630 File Offset: 0x00188830
	private async void Start()
	{
		TaskAwaiter<bool> taskAwaiter = KIDManager.UseKID().GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<bool> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		KIDManager._useKid = taskAwaiter.GetResult();
		TaskAwaiter<int> taskAwaiter3 = KIDManager.CheckKIDPhase().GetAwaiter();
		if (!taskAwaiter3.IsCompleted)
		{
			await taskAwaiter3;
			TaskAwaiter<int> taskAwaiter4;
			taskAwaiter3 = taskAwaiter4;
			taskAwaiter4 = default(TaskAwaiter<int>);
		}
		KIDManager._kIDPhase = taskAwaiter3.GetResult();
		TaskAwaiter<DateTime?> taskAwaiter5 = KIDManager.CheckKIDNewPlayerDateTime().GetAwaiter();
		if (!taskAwaiter5.IsCompleted)
		{
			await taskAwaiter5;
			TaskAwaiter<DateTime?> taskAwaiter6;
			taskAwaiter5 = taskAwaiter6;
			taskAwaiter6 = default(TaskAwaiter<DateTime?>);
		}
		KIDManager._kIDNewPlayerDateTime = taskAwaiter5.GetResult();
		KIDManager._titleDataReady = true;
	}

	// Token: 0x06004A1C RID: 18972 RVA: 0x0018A65F File Offset: 0x0018885F
	private void OnDestroy()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	// Token: 0x06004A1D RID: 18973 RVA: 0x0018A66C File Offset: 0x0018886C
	public static string GetActiveAccountStatusNiceString()
	{
		switch (KIDManager.GetActiveAccountStatus())
		{
		case AgeStatusType.DIGITALMINOR:
			return "Digital Minor";
		case AgeStatusType.DIGITALYOUTH:
			return "Digital Youth";
		case AgeStatusType.LEGALADULT:
			return "Legal Adult";
		default:
			return "UNKNOWN";
		}
	}

	// Token: 0x06004A1E RID: 18974 RVA: 0x0018A6AC File Offset: 0x001888AC
	public static AgeStatusType GetActiveAccountStatus()
	{
		if (KIDManager.CurrentSession != null)
		{
			return KIDManager.CurrentSession.AgeStatus;
		}
		if (!PlayFabAuthenticator.instance.GetSafety())
		{
			return AgeStatusType.LEGALADULT;
		}
		return AgeStatusType.DIGITALMINOR;
	}

	// Token: 0x06004A1F RID: 18975 RVA: 0x0018A6D1 File Offset: 0x001888D1
	public static List<Permission> GetAllPermissionsData()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::MANAGER] There is no current session. Unless the age-gate has not yet finished there should always be a session even if it is the default session");
			return new List<Permission>();
		}
		return KIDManager.CurrentSession.GetAllPermissions();
	}

	// Token: 0x06004A20 RID: 18976 RVA: 0x0018A6F4 File Offset: 0x001888F4
	public static bool TryGetAgeStatusTypeFromAge(int age, out AgeStatusType ageType)
	{
		if (KIDManager._ageGateRequirements == null)
		{
			Debug.LogError("[KID::MANAGER] [_ageGateRequirements] is not set - need to Get AgeGate Requirements first");
			ageType = AgeStatusType.DIGITALMINOR;
			return false;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.DigitalConsentAge)
		{
			ageType = AgeStatusType.DIGITALMINOR;
			return true;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.CivilAge)
		{
			ageType = AgeStatusType.DIGITALYOUTH;
			return true;
		}
		ageType = AgeStatusType.LEGALADULT;
		return true;
	}

	// Token: 0x06004A21 RID: 18977 RVA: 0x0018A74C File Offset: 0x0018894C
	[return: TupleElementNames(new string[] { "requiresOptIn", "hasOptedInPreviously" })]
	public static ValueTuple<bool, bool> CheckFeatureOptIn(EKIDFeatures feature, Permission permissionData = null)
	{
		if (permissionData == null)
		{
			permissionData = KIDManager.GetPermissionDataByFeature(feature);
			if (permissionData == null)
			{
				Debug.LogError("[KID::MANAGER] Unable to retrieve permission data for feature [" + feature.ToStandardisedString() + "]");
				return new ValueTuple<bool, bool>(false, false);
			}
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		bool flag = true;
		if (KIDManager.CurrentSession != null)
		{
			flag = KIDManager.CurrentSession.HasOptedInToPermission(feature);
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
		{
			return new ValueTuple<bool, bool>(false, flag);
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.PLAYER && permissionData.Enabled)
		{
			return new ValueTuple<bool, bool>(false, true);
		}
		return new ValueTuple<bool, bool>(true, flag);
	}

	// Token: 0x06004A22 RID: 18978 RVA: 0x0018A7E0 File Offset: 0x001889E0
	public static void SetFeatureOptIn(EKIDFeatures feature, bool optedIn)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID] Trying to set Feature Opt in for feature [" + feature.ToStandardisedString() + "] but permission data could not be found. Assumed is opt-in", Array.Empty<object>());
			return;
		}
		if (KIDManager.CurrentSession == null)
		{
			Debug.Log("[KID::MANAGER] CurrentSession is null, cannot set feature opt-in. Returning.");
			return;
		}
		switch (permissionDataByFeature.ManagedBy)
		{
		case Permission.ManagedByEnum.PLAYER:
			KIDManager.CurrentSession.OptInToPermission(feature, optedIn);
			return;
		case Permission.ManagedByEnum.GUARDIAN:
			KIDManager.CurrentSession.OptInToPermission(feature, permissionDataByFeature.Enabled);
			return;
		case Permission.ManagedByEnum.PROHIBITED:
			KIDManager.CurrentSession.OptInToPermission(feature, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06004A23 RID: 18979 RVA: 0x0018A870 File Offset: 0x00188A70
	public static bool CheckFeatureSettingEnabled(EKIDFeatures feature)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogError("[KID::MANAGER] Unable to permissions for feature [" + feature.ToStandardisedString() + "]");
			return false;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return false;
		}
		bool item = KIDManager.CheckFeatureOptIn(feature, null).Item2;
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
		case EKIDFeatures.Mods:
			return item;
		case EKIDFeatures.Custom_Nametags:
			return item && GorillaComputer.instance.NametagsEnabled;
		case EKIDFeatures.Voice_Chat:
			return item && GorillaComputer.instance.CheckVoiceChatEnabled();
		case EKIDFeatures.Groups:
			return permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.GUARDIAN || permissionDataByFeature.Enabled;
		default:
			Debug.LogError("[KID::MANAGER] Tried finding feature setting for [" + feature.ToStandardisedString() + "] but failed.");
			return false;
		}
	}

	// Token: 0x06004A24 RID: 18980 RVA: 0x0018A92C File Offset: 0x00188B2C
	private static async Task<GetPlayerData_Data> TryGetPlayerData(bool forceRefresh)
	{
		return await KIDManager.Server_GetPlayerData(forceRefresh, null);
	}

	// Token: 0x06004A25 RID: 18981 RVA: 0x0018A970 File Offset: 0x00188B70
	private static async Task<GetRequirementsData> TryGetRequirements()
	{
		return await KIDManager.Server_GetRequirements();
	}

	// Token: 0x06004A26 RID: 18982 RVA: 0x0018A9AC File Offset: 0x00188BAC
	private static async Task<VerifyAgeData> TryVerifyAgeResponse()
	{
		PlayerPlatform playerPlatform = PlayerPlatform.Steam;
		VerifyAgeRequest verifyAgeRequest = new VerifyAgeRequest();
		verifyAgeRequest.Age = new int?(KIDAgeGate.UserAge);
		verifyAgeRequest.Platform = new PlayerPlatform?(playerPlatform);
		Debug.Log(string.Format("[KID::MANAGER] Sending verify age request for age: [{0}]", KIDAgeGate.UserAge));
		return await KIDManager.Server_VerifyAge(verifyAgeRequest, null);
	}

	// Token: 0x06004A27 RID: 18983 RVA: 0x0018A9E8 File Offset: 0x00188BE8
	[return: TupleElementNames(new string[] { "success", "exception" })]
	private static async Task<ValueTuple<bool, string>> TrySendChallengeEmailRequest()
	{
		do
		{
			await Task.Yield();
		}
		while (string.IsNullOrEmpty(KIDManager._emailAddress));
		ValueTuple<bool, string> valueTuple = await KIDManager.Server_SendChallengeEmail(new SendChallengeEmailRequest
		{
			Email = KIDManager._emailAddress,
			Locale = (string.IsNullOrEmpty(KIDManager.DbgLocale) ? CultureInfo.CurrentCulture.Name : KIDManager.DbgLocale)
		});
		bool item = valueTuple.Item1;
		string item2 = valueTuple.Item2;
		if (item)
		{
			KIDManager.OnEmailResultReceived onEmailResultReceived = KIDManager.onEmailResultReceived;
			if (onEmailResultReceived != null)
			{
				onEmailResultReceived(true);
			}
		}
		else
		{
			KIDManager.OnEmailResultReceived onEmailResultReceived2 = KIDManager.onEmailResultReceived;
			if (onEmailResultReceived2 != null)
			{
				onEmailResultReceived2(false);
			}
		}
		return new ValueTuple<bool, string>(item, item2);
	}

	// Token: 0x06004A28 RID: 18984 RVA: 0x0018AA24 File Offset: 0x00188C24
	private static async Task<bool> TrySendOptInPermissions()
	{
		string[] optedInPermissions = KIDManager.CurrentSession.GetOptedInPermissions();
		bool flag;
		if (optedInPermissions == null)
		{
			Debug.LogError("[KID::MANAGER::OptInRefactor] Tried to set opt-in permissions but no permissions were provided");
			flag = false;
		}
		else
		{
			Debug.Log("[KID::MANAGER::OptInRefactor] Setting Opt-in Permissions: " + string.Join(", ", optedInPermissions));
			flag = await KIDManager.Server_SetOptInPermissions(new SetOptInPermissionsRequest
			{
				OptInPermissions = optedInPermissions
			}, null);
		}
		return flag;
	}

	// Token: 0x06004A29 RID: 18985 RVA: 0x0018AA60 File Offset: 0x00188C60
	public static async Task<ValueTuple<bool, string>> TrySendUpgradeSessionChallengeEmail()
	{
		ValueTuple<bool, string> valueTuple = await KIDManager.Server_SendChallengeEmail(new SendChallengeEmailRequest());
		bool item = valueTuple.Item1;
		string item2 = valueTuple.Item2;
		return new ValueTuple<bool, string>(item, item2);
	}

	// Token: 0x06004A2A RID: 18986 RVA: 0x0018AA9C File Offset: 0x00188C9C
	public static async Task<bool> TrySetHasConfirmedStatus()
	{
		return await KIDManager.Server_SetConfirmedStatus();
	}

	// Token: 0x06004A2B RID: 18987 RVA: 0x0018AAD8 File Offset: 0x00188CD8
	public static async Task<UpgradeSessionData> TryUpgradeSession(List<string> requestedPermissions)
	{
		global::UpgradeSessionRequest upgradeSessionRequest = new global::UpgradeSessionRequest();
		upgradeSessionRequest.Permissions = requestedPermissions.Select((string name) => new RequestedPermission(name)).ToList<RequestedPermission>();
		UpgradeSessionData upgradeSessionData = await KIDManager.Server_UpgradeSession(upgradeSessionRequest);
		UpgradeSessionData upgradeSessionData2;
		if (upgradeSessionData == null)
		{
			Debug.LogError("[KID::MANAGER] Failed to upgrade session. Data is null.");
			upgradeSessionData2 = null;
		}
		else
		{
			KIDManager.UpdatePermissions(upgradeSessionData.session);
			upgradeSessionData2 = upgradeSessionData;
		}
		return upgradeSessionData2;
	}

	// Token: 0x06004A2C RID: 18988 RVA: 0x0018AB1C File Offset: 0x00188D1C
	public static async Task<AttemptAgeUpdateData> TryAttemptAgeUpdate(int age)
	{
		PlayerPlatform playerPlatform = PlayerPlatform.Steam;
		AttemptAgeUpdateRequest attemptAgeUpdateRequest = new AttemptAgeUpdateRequest();
		attemptAgeUpdateRequest.Age = age;
		attemptAgeUpdateRequest.Platform = playerPlatform;
		Debug.Log(string.Format("[KID::MANAGER] Sending age update request for age: [{0}]", age));
		return await KIDManager.Server_AttemptAgeUpdate(attemptAgeUpdateRequest, null);
	}

	// Token: 0x06004A2D RID: 18989 RVA: 0x0018AB60 File Offset: 0x00188D60
	public static async Task<bool> TryAppealAge(string email, int newAge)
	{
		string text = (string.IsNullOrEmpty(KIDManager.DbgLocale) ? CultureInfo.CurrentCulture.Name : KIDManager.DbgLocale);
		AppealAgeRequest appealAgeRequest = new AppealAgeRequest();
		appealAgeRequest.Age = newAge;
		appealAgeRequest.Email = email;
		appealAgeRequest.Locale = text;
		Debug.Log(string.Format("[KID::MANAGER] Sending age appeal request for age: [{0}] at email [{1}]", newAge, email));
		return await KIDManager.Server_AppealAge(appealAgeRequest, null);
	}

	// Token: 0x06004A2E RID: 18990 RVA: 0x0018ABAC File Offset: 0x00188DAC
	public static async Task UpdateSession(Action<bool> getDataCompleted = null)
	{
		GetPlayerData_Data getPlayerData_Data = await KIDManager.TryGetPlayerData(true);
		if (getPlayerData_Data == null)
		{
			if (getDataCompleted != null)
			{
				getDataCompleted(false);
			}
			Debug.LogError("[KID::MANAGER] Failed to retrieve session");
		}
		else if (getPlayerData_Data.responseType == GetSessionResponseType.ERROR)
		{
			if (getDataCompleted != null)
			{
				getDataCompleted(false);
			}
			Debug.LogError("[KID::MANAGER] Failed to get session. Resulted in error. Cannot update session");
		}
		else
		{
			if (getDataCompleted != null)
			{
				getDataCompleted(true);
			}
			KIDManager.UpdatePermissions(getPlayerData_Data.session);
		}
	}

	// Token: 0x06004A2F RID: 18991 RVA: 0x0018ABF0 File Offset: 0x00188DF0
	private static async Task<bool> CheckWarningScreensOptedIn()
	{
		bool flag;
		if (GorillaServer.Instance.CheckOptedInKID())
		{
			flag = true;
		}
		else
		{
			PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.KID, "");
			WarningButtonResult warningButtonResult = await WarningScreens.StartWarningScreen(KIDManager._requestCancellationSource.Token);
			if (warningButtonResult == WarningButtonResult.None)
			{
				if (KIDManager._requestCancellationSource.IsCancellationRequested)
				{
					flag = false;
				}
				else
				{
					GorillaServer.Instance.CheckIsInKIDOptInCohort();
					GorillaServer.Instance.CheckIsInKIDRequiredCohort();
					flag = false;
				}
			}
			else if (warningButtonResult == WarningButtonResult.CloseWarning)
			{
				bool isCancellationRequested = KIDManager._requestCancellationSource.IsCancellationRequested;
				flag = false;
			}
			else
			{
				if (warningButtonResult == WarningButtonResult.OptIn)
				{
					TaskAwaiter<bool> taskAwaiter = KIDManager.Server_OptIn().GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						Debug.LogError("[KID::MANAGER] PHASE ONE (A) -- FAILURE - Opting in to k-ID failed!");
						return false;
					}
					if (CosmeticsController.instance != null)
					{
						CosmeticsController.instance.GetCurrencyBalance();
					}
					await WarningScreens.StartOptInFollowUpScreen(KIDManager._requestCancellationSource.Token);
				}
				flag = true;
			}
		}
		return flag;
	}

	// Token: 0x06004A30 RID: 18992 RVA: 0x0018AC2B File Offset: 0x00188E2B
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void InitialiseBootFlow()
	{
		if (PlayerPrefs.GetInt(KIDManager.KIDSetupPlayerPref, 0) != 0)
		{
			return;
		}
		PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.KID, "");
	}

	// Token: 0x06004A31 RID: 18993 RVA: 0x0018AC48 File Offset: 0x00188E48
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static async void InitialiseKID()
	{
		bool snapTurnDisabled = false;
		float? cachedTapHapticsStrength = null;
		object obj = null;
		int num = 0;
		try
		{
			TaskAwaiter<bool> taskAwaiter = KIDManager.WaitForAuthentication().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			bool flag = !taskAwaiter.GetResult();
			UGCPermissionManager.UsePlayFabSafety();
			if (!flag && KIDManager._useKid)
			{
				GorillaSnapTurn.DisableSnapTurn();
				snapTurnDisabled = true;
				if (GorillaTagger.Instance != null)
				{
					cachedTapHapticsStrength = new float?(GorillaTagger.Instance.tapHapticStrength);
					GorillaTagger.Instance.tapHapticStrength = 0f;
				}
				GetPlayerData_Data newSessionData = await KIDManager.TryGetPlayerData(true);
				if (!KIDManager._requestCancellationSource.IsCancellationRequested)
				{
					if (newSessionData == null)
					{
						Debug.LogError("[KID::MANAGER] [newSessionData] returned NULL. Something went wrong, we should always get a [GetPlayerData_Data]. Disabling k-ID");
					}
					else if (newSessionData.responseType == GetSessionResponseType.ERROR)
					{
						Debug.LogError("[KID::MANAGER] Failed to retrieve Player Data, response type: [" + newSessionData.responseType.ToString() + "]. Unable to proceed. Will default to Using Safeties");
					}
					else
					{
						KIDManager.HasOptedInToKID = newSessionData.responseType != GetSessionResponseType.NOT_FOUND;
						bool flag2 = await KIDManager.CheckWarningScreensOptedIn();
						if (!KIDManager._requestCancellationSource.IsCancellationRequested && flag2)
						{
							KIDManager.PreviousStatus = (SessionStatus)PlayerPrefs.GetInt(KIDManager.PreviousStatusPlayerPrefRef, 0);
							TMPSession newSession = newSessionData.session;
							TMPSession session = newSessionData.session;
							if (session != null)
							{
								AgeStatusType ageStatus = session.AgeStatus;
							}
							KIDManager._ageGateRequirements = await KIDManager.TryGetRequirements();
							KIDAgeGate.SetAgeGateConfig(KIDManager._ageGateRequirements);
							if (KIDManager._ageGateRequirements != null)
							{
								GetRequirementsResponse ageGateRequirements = KIDManager._ageGateRequirements.AgeGateRequirements;
							}
							SessionStatus? sessionStatus = newSessionData.status;
							SessionStatus sessionStatus2 = SessionStatus.PROHIBITED;
							if (!((sessionStatus.GetValueOrDefault() == sessionStatus2) & (sessionStatus != null)))
							{
								sessionStatus = newSessionData.status;
								sessionStatus2 = SessionStatus.PENDING_AGE_APPEAL;
								if (!((sessionStatus.GetValueOrDefault() == sessionStatus2) & (sessionStatus != null)))
								{
									TMPSession session2 = newSessionData.session;
									bool flag3;
									if (session2 == null)
									{
										flag3 = true;
									}
									else
									{
										AgeStatusType ageStatus2 = session2.AgeStatus;
										flag3 = false;
									}
									if (flag3)
									{
										PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.KID, "");
										ValueTuple<AgeStatusType, TMPSession> valueTuple = await KIDManager.AgeGateFlow(newSessionData);
										AgeStatusType item = valueTuple.Item1;
										TMPSession item2 = valueTuple.Item2;
										if (KIDManager._requestCancellationSource.IsCancellationRequested)
										{
											goto IL_074A;
										}
										newSession = item2;
									}
									if (LegalAgreements.instance != null)
									{
										await LegalAgreements.instance.StartLegalAgreements();
										if (KIDManager._requestCancellationSource.IsCancellationRequested)
										{
											goto IL_074A;
										}
									}
									if (!KIDManager.UpdatePermissions(newSession) || KIDManager.CurrentSession == null)
									{
										goto IL_074A;
									}
									if (KIDManager.CurrentSession.IsDefault)
									{
										KIDManager.WaitForAndUpdateNewSession(true);
									}
									if (KIDManager._requestCancellationSource.IsCancellationRequested)
									{
										goto IL_074A;
									}
									UGCPermissionManager.UseKID();
									Error error = await ModIOManager.ShowTermsOfUseAtGameLoad();
									if (error)
									{
										Debug.LogError(string.Format("[KID::MANAGER] Failed to show Mod.io Terms of Use at game load: {0}", error));
									}
									if (!KIDManager._requestCancellationSource.IsCancellationRequested)
									{
										await KIDUI_Controller.Instance.StartKIDScreens(KIDManager._requestCancellationSource.Token);
										while (!KIDManager._requestCancellationSource.IsCancellationRequested)
										{
											await Task.Yield();
											if (!KIDUI_Controller.IsKIDUIActive)
											{
												if (KIDManager._requestCancellationSource.IsCancellationRequested)
												{
													break;
												}
												if (KIDManager.CurrentSession == null)
												{
													Debug.LogError("[KID::MANAGER] PHASE SEVEN -- FAILURE -- CurrentSession is NULL, should at least have a default session!");
													Debug.Log(string.Format("[KID::MANAGER] Safeties is: [{0}", PlayFabAuthenticator.instance.GetSafety()));
													break;
												}
												if (!newSessionData.HasConfirmedSetup)
												{
													await KIDMessagingController.StartKIDConfirmationScreen(KIDManager._requestCancellationSource.Token);
												}
												PlayerPrefs.SetInt(KIDManager.PreviousStatusPlayerPrefRef, (int)KIDManager.PreviousStatus);
												PlayerPrefs.Save();
												KIDManager.InitialisationSuccessful = true;
												newSessionData = null;
												newSession = null;
												goto IL_075F;
											}
										}
										goto IL_074A;
									}
									goto IL_074A;
								}
							}
							PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.KID, "");
							KIDUI_AgeAppealController.Instance.StartAgeAppealScreens(newSessionData.status);
						}
					}
				}
			}
			IL_074A:
			num = 1;
		}
		catch (object obj)
		{
		}
		IL_075F:
		KIDManager.InitialisationComplete = true;
		if (!KIDManager.InitialisationSuccessful)
		{
			if (cachedTapHapticsStrength != null && GorillaTagger.Instance != null)
			{
				GorillaTagger.Instance.tapHapticStrength = cachedTapHapticsStrength.Value;
			}
			if (snapTurnDisabled)
			{
				GorillaSnapTurn.LoadSettingsFromCache();
			}
			if (LegalAgreements.instance != null)
			{
				await LegalAgreements.instance.StartLegalAgreements();
			}
			PrivateUIRoom.StopForcedOverlay(PrivateUIRoom.OverlaySource.KID);
		}
		object obj2 = obj;
		if (obj2 != null)
		{
			Exception ex = obj2 as Exception;
			if (ex == null)
			{
				throw obj2;
			}
			ExceptionDispatchInfo.Capture(ex).Throw();
		}
		if (num != 1)
		{
			obj = null;
			UGCPermissionManager.UseKID();
			if (KIDManager.CurrentSession == null)
			{
				PlayFabAuthenticator.instance.GetSafety();
			}
			if (cachedTapHapticsStrength != null && GorillaTagger.Instance != null)
			{
				GorillaTagger.Instance.tapHapticStrength = cachedTapHapticsStrength.Value;
			}
			if (snapTurnDisabled)
			{
				GorillaSnapTurn.LoadSettingsFromCache();
			}
			PrivateUIRoom.StopForcedOverlay(PrivateUIRoom.OverlaySource.KID);
		}
	}

	// Token: 0x06004A32 RID: 18994 RVA: 0x0018AC78 File Offset: 0x00188E78
	private static bool UpdatePermissions(TMPSession newSession)
	{
		if (newSession == null || !newSession.IsValidSession)
		{
			Debug.LogError("[KID::MANAGER] A NULL or Invalid Session was received!");
			return false;
		}
		KIDManager.CurrentSession = newSession;
		if (KIDUI_Controller.IsKIDUIActive)
		{
			KIDManager.PreviousStatus = KIDManager.CurrentSession.SessionStatus;
			PlayerPrefs.SetInt(KIDManager.PreviousStatusPlayerPrefRef, (int)KIDManager.PreviousStatus);
			PlayerPrefs.Save();
		}
		if (!KIDManager.CurrentSession.IsDefault)
		{
			PlayerPrefs.SetInt(KIDManager.KIDSetupPlayerPref, 1);
			PlayerPrefs.Save();
		}
		KIDManager.OnSessionUpdated();
		if (KIDUI_Controller.Instance)
		{
			KIDUI_Controller.Instance.UpdateScreenStatus();
		}
		return true;
	}

	// Token: 0x06004A33 RID: 18995 RVA: 0x0018AD04 File Offset: 0x00188F04
	private static void ClearSession()
	{
		KIDManager.CurrentSession = null;
		KIDManager.DeleteStoredPermissions();
	}

	// Token: 0x06004A34 RID: 18996 RVA: 0x00002C2D File Offset: 0x00000E2D
	private static void DeleteStoredPermissions()
	{
	}

	// Token: 0x06004A35 RID: 18997 RVA: 0x0018AD11 File Offset: 0x00188F11
	public static CancellationTokenSource ResetCancellationToken()
	{
		KIDManager._requestCancellationSource.Dispose();
		KIDManager._requestCancellationSource = new CancellationTokenSource();
		return KIDManager._requestCancellationSource;
	}

	// Token: 0x06004A36 RID: 18998 RVA: 0x0018AD2C File Offset: 0x00188F2C
	public static Permission GetPermissionDataByFeature(EKIDFeatures feature)
	{
		if (KIDManager.CurrentSession == null)
		{
			if (!PlayFabAuthenticator.instance.GetSafety())
			{
				return new Permission(feature.ToStandardisedString(), true, Permission.ManagedByEnum.PLAYER);
			}
			return new Permission(feature.ToStandardisedString(), false, Permission.ManagedByEnum.GUARDIAN);
		}
		else
		{
			Permission permission;
			if (!KIDManager.CurrentSession.TryGetPermission(feature, out permission))
			{
				Debug.LogError("[KID::MANAGER] Failed to retreive permission from session for [" + feature.ToStandardisedString() + "]. Assuming disabled permission");
				return new Permission(feature.ToStandardisedString(), false, Permission.ManagedByEnum.GUARDIAN);
			}
			return permission;
		}
	}

	// Token: 0x06004A37 RID: 18999 RVA: 0x0018A65F File Offset: 0x0018885F
	public static void CancelToken()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	// Token: 0x06004A38 RID: 19000 RVA: 0x0018ADA8 File Offset: 0x00188FA8
	public static async Task<bool> UseKID()
	{
		bool flag;
		if (KIDManager._titleDataReady)
		{
			flag = KIDManager._useKid;
		}
		else
		{
			int state = 0;
			bool isEnabled = false;
			PlayFabTitleDataCache.Instance.GetTitleData("KIDData", delegate(string res)
			{
				state = 1;
				isEnabled = KIDManager.GetIsEnabled(res);
			}, delegate(PlayFabError err)
			{
				state = -1;
				Debug.LogError("[KID_MANAGER::UseKID] Something went wrong trying to get title data for key: [KIDData]. Error:\n" + err.ErrorMessage);
			}, false);
			do
			{
				await Task.Yield();
			}
			while (state == 0);
			if (PlayFabAuthenticator.instance.postAuthSetSafety & isEnabled)
			{
				PlayFabAuthenticator.instance.DefaultSafetiesByAgeCategory();
			}
			flag = isEnabled;
		}
		return flag;
	}

	// Token: 0x06004A39 RID: 19001 RVA: 0x0018ADE4 File Offset: 0x00188FE4
	public static async Task<int> CheckKIDPhase()
	{
		int num;
		if (KIDManager._titleDataReady)
		{
			num = KIDManager._kIDPhase;
		}
		else
		{
			int state = 0;
			int phase = 0;
			PlayFabTitleDataCache.Instance.GetTitleData("KIDData", delegate(string res)
			{
				state = 1;
				phase = KIDManager.GetPhase(res);
			}, delegate(PlayFabError err)
			{
				state = -1;
				Debug.LogError("[KID_MANAGER] Something went wrong trying to get title data for key: [KIDData]. Error:\n" + err.ErrorMessage);
			}, false);
			do
			{
				await Task.Yield();
			}
			while (state == 0);
			num = phase;
		}
		return num;
	}

	// Token: 0x06004A3A RID: 19002 RVA: 0x0018AE20 File Offset: 0x00189020
	public static async Task<DateTime?> CheckKIDNewPlayerDateTime()
	{
		DateTime? dateTime;
		if (KIDManager._titleDataReady)
		{
			dateTime = KIDManager._kIDNewPlayerDateTime;
		}
		else
		{
			int state = 0;
			DateTime? newPlayerDateTime = null;
			PlayFabTitleDataCache.Instance.GetTitleData("KIDData", delegate(string res)
			{
				state = 1;
				newPlayerDateTime = KIDManager.GetNewPlayerDateTime(res);
			}, delegate(PlayFabError err)
			{
				state = -1;
				Debug.LogError("[KID_MANAGER] Something went wrong trying to get title data for key: [KIDData]. Error:\n" + err.ErrorMessage);
			}, false);
			do
			{
				await Task.Yield();
			}
			while (state == 0);
			dateTime = newPlayerDateTime;
		}
		return dateTime;
	}

	// Token: 0x06004A3B RID: 19003 RVA: 0x0018AE5C File Offset: 0x0018905C
	private static bool GetIsEnabled(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return false;
		}
		bool flag;
		if (!bool.TryParse(kidtitleData.KIDEnabled, out flag))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDEnabled': [KIDEnabled] to bool.");
			return false;
		}
		return flag;
	}

	// Token: 0x06004A3C RID: 19004 RVA: 0x0018AEA4 File Offset: 0x001890A4
	private static int GetPhase(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return 0;
		}
		return kidtitleData.KIDPhase;
	}

	// Token: 0x06004A3D RID: 19005 RVA: 0x0018AED4 File Offset: 0x001890D4
	private static DateTime? GetNewPlayerDateTime(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		DateTime dateTime;
		if (!DateTime.TryParse(kidtitleData.KIDNewPlayerIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDNewPlayerIsoTimestamp': [KIDNewPlayerIsoTimestamp] to DateTime.");
			return null;
		}
		return new DateTime?(dateTime);
	}

	// Token: 0x06004A3E RID: 19006 RVA: 0x0018AF38 File Offset: 0x00189138
	public static bool IsAdult()
	{
		return KIDManager.CurrentSession.IsValidSession && KIDManager.CurrentSession.AgeStatus == AgeStatusType.LEGALADULT;
	}

	// Token: 0x06004A3F RID: 19007 RVA: 0x0018AF58 File Offset: 0x00189158
	public static bool HasAllPermissions()
	{
		List<Permission> allPermissions = KIDManager.CurrentSession.GetAllPermissions();
		for (int i = 0; i < allPermissions.Count; i++)
		{
			if (allPermissions[i].ManagedBy == Permission.ManagedByEnum.GUARDIAN || !allPermissions[i].Enabled)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004A40 RID: 19008 RVA: 0x0018AFA4 File Offset: 0x001891A4
	public static async Task<bool> SetKIDOptIn()
	{
		return await KIDManager.Server_OptIn();
	}

	// Token: 0x06004A41 RID: 19009 RVA: 0x0018AFE0 File Offset: 0x001891E0
	[return: TupleElementNames(new string[] { "success", "message" })]
	public static async Task<ValueTuple<bool, string>> SetAndSendEmail(string email)
	{
		KIDManager._emailAddress = email;
		return await KIDManager.TrySendChallengeEmailRequest();
	}

	// Token: 0x06004A42 RID: 19010 RVA: 0x0018B024 File Offset: 0x00189224
	public static async Task<bool> SendOptInPermissions()
	{
		return await KIDManager.TrySendOptInPermissions();
	}

	// Token: 0x06004A43 RID: 19011 RVA: 0x0018B060 File Offset: 0x00189260
	public static bool HasPermissionToUseFeature(EKIDFeatures feature)
	{
		if (!KIDManager.KidEnabledAndReady)
		{
			return !PlayFabAuthenticator.instance.GetSafety();
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		return (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
	}

	// Token: 0x06004A44 RID: 19012 RVA: 0x0018B0AC File Offset: 0x001892AC
	private static async Task<bool> WaitForAuthentication()
	{
		while (!PlayFabClientAPI.IsClientLoggedIn())
		{
			bool flag;
			if (KIDManager._requestCancellationSource.IsCancellationRequested)
			{
				flag = false;
			}
			else
			{
				if (!PlayFabAuthenticator.instance || !PlayFabAuthenticator.instance.loginFailed)
				{
					await Task.Yield();
					continue;
				}
				flag = false;
			}
			return flag;
		}
		while (!GorillaServer.Instance.FeatureFlagsReady)
		{
			if (KIDManager._requestCancellationSource.IsCancellationRequested)
			{
				return false;
			}
			await Task.Yield();
		}
		while (!KIDManager._titleDataReady)
		{
			if (KIDManager._requestCancellationSource.IsCancellationRequested)
			{
				return false;
			}
			await Task.Yield();
		}
		return true;
	}

	// Token: 0x06004A45 RID: 19013 RVA: 0x0018B0E8 File Offset: 0x001892E8
	[return: TupleElementNames(new string[] { "ageStatus", "resp" })]
	private static async Task<ValueTuple<AgeStatusType, TMPSession>> AgeGateFlow(GetPlayerData_Data newPlayerData)
	{
		TMPSession tmpsession = newPlayerData.session;
		TMPSession session = newPlayerData.session;
		AgeStatusType? ageStatusType = ((session != null) ? new AgeStatusType?(session.AgeStatus) : null);
		if (ageStatusType == null)
		{
			VerifyAgeData verifyAgeData = await KIDManager.ProcessAgeGate();
			if (verifyAgeData == null)
			{
				return new ValueTuple<AgeStatusType, TMPSession>(AgeStatusType.DIGITALMINOR, null);
			}
			tmpsession = verifyAgeData.Session;
			ageStatusType = new AgeStatusType?(tmpsession.AgeStatus);
			bool isDefault = tmpsession.IsDefault;
		}
		if (ageStatusType == null)
		{
			Debug.LogError("[KID::MANAGER] PHASE THREE (A) -- FAILURE - Age Gate completed, but age status is null. Defaulting to MINOR");
			ageStatusType = new AgeStatusType?(AgeStatusType.DIGITALMINOR);
		}
		return new ValueTuple<AgeStatusType, TMPSession>(ageStatusType.Value, tmpsession);
	}

	// Token: 0x06004A46 RID: 19014 RVA: 0x0018B12C File Offset: 0x0018932C
	private static async Task<VerifyAgeData> ProcessAgeGate()
	{
		await KIDAgeGate.BeginAgeGate();
		VerifyAgeData verifyAgeData;
		if (KIDManager._requestCancellationSource.IsCancellationRequested)
		{
			verifyAgeData = null;
		}
		else
		{
			VerifyAgeData verifyResponse = await KIDManager.TryVerifyAgeResponse();
			if (KIDManager._requestCancellationSource.IsCancellationRequested)
			{
				verifyAgeData = null;
			}
			else if (verifyResponse.Status == SessionStatus.PROHIBITED || verifyResponse.Status == SessionStatus.PENDING_AGE_APPEAL)
			{
				KIDUI_AgeAppealController.Instance.StartAgeAppealScreens(new SessionStatus?(verifyResponse.Status));
				GetPlayerData_Data getPlayerData_Data = await KIDManager.TryGetPlayerData(true);
				for (;;)
				{
					SessionStatus? sessionStatus = getPlayerData_Data.status;
					SessionStatus sessionStatus2 = SessionStatus.PROHIBITED;
					if (!((sessionStatus.GetValueOrDefault() == sessionStatus2) & (sessionStatus != null)))
					{
						sessionStatus = getPlayerData_Data.status;
						sessionStatus2 = SessionStatus.PENDING_AGE_APPEAL;
						if (!((sessionStatus.GetValueOrDefault() == sessionStatus2) & (sessionStatus != null)))
						{
							break;
						}
					}
					await Task.Delay(30000);
					getPlayerData_Data = await KIDManager.TryGetPlayerData(true);
				}
				verifyAgeData = verifyResponse;
			}
			else
			{
				verifyAgeData = verifyResponse;
			}
		}
		return verifyAgeData;
	}

	// Token: 0x06004A47 RID: 19015 RVA: 0x0018B167 File Offset: 0x00189367
	public static string GetOptInKey(EKIDFeatures feature)
	{
		return feature.ToStandardisedString() + "-opt-in-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
	}

	// Token: 0x06004A48 RID: 19016 RVA: 0x0018B188 File Offset: 0x00189388
	private static async Task<GetPlayerData_Data> Server_GetPlayerData(bool forceRefresh, Action failureCallback)
	{
		string text = string.Format("sessionRefresh={0}", forceRefresh ? "true" : "false");
		ValueTuple<long, GetPlayerDataResponse, string> valueTuple = await KIDManager.KIDServerWebRequest<GetPlayerDataResponse, KIDRequestData>("GetPlayerData", "GET", null, text, 3, null);
		long item = valueTuple.Item1;
		GetPlayerDataResponse item2 = valueTuple.Item2;
		GetSessionResponseType getSessionResponseType = GetSessionResponseType.ERROR;
		if (item != 200L)
		{
			if (item != 204L)
			{
				if (item == 404L)
				{
					getSessionResponseType = GetSessionResponseType.LOST;
				}
			}
			else
			{
				getSessionResponseType = GetSessionResponseType.NOT_FOUND;
			}
		}
		else
		{
			getSessionResponseType = GetSessionResponseType.OK;
		}
		GetPlayerData_Data getPlayerData_Data = new GetPlayerData_Data(getSessionResponseType, item2);
		if (item < 200L || item >= 300L)
		{
			if (failureCallback != null)
			{
				failureCallback();
			}
		}
		return getPlayerData_Data;
	}

	// Token: 0x06004A49 RID: 19017 RVA: 0x0018B1D4 File Offset: 0x001893D4
	private static async Task<bool> Server_SetConfirmedStatus()
	{
		long num = await KIDManager.KIDServerWebRequestNoResponse<KIDRequestData>("SetConfirmedStatus", "POST", null, 2, null);
		bool flag;
		if (num == 200L)
		{
			flag = true;
		}
		else
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] SetConfirmedStatus request failed. Code: {0}", num));
			flag = false;
		}
		return flag;
	}

	// Token: 0x06004A4A RID: 19018 RVA: 0x0018B210 File Offset: 0x00189410
	private static async Task<UpgradeSessionData> Server_UpgradeSession(global::UpgradeSessionRequest request)
	{
		ValueTuple<long, global::UpgradeSessionResponse, string> valueTuple = await KIDManager.KIDServerWebRequest<global::UpgradeSessionResponse, global::UpgradeSessionRequest>("UpgradeSession", "POST", request, null, 2, null);
		long item = valueTuple.Item1;
		global::UpgradeSessionResponse item2 = valueTuple.Item2;
		if (item != 200L)
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] Upgrade session request failed. Code: {0}", item));
		}
		UpgradeSessionData upgradeSessionData;
		if (item2 == null)
		{
			Debug.LogError("[KID::SERVER_ROUTER] Upgrade session response is NULL. This is unexpected.");
			upgradeSessionData = null;
		}
		else
		{
			upgradeSessionData = new UpgradeSessionData(item2);
		}
		return upgradeSessionData;
	}

	// Token: 0x06004A4B RID: 19019 RVA: 0x0018B254 File Offset: 0x00189454
	private static async Task<VerifyAgeData> Server_VerifyAge(VerifyAgeRequest request, Action failureCallback)
	{
		ValueTuple<long, VerifyAgeResponse, string> valueTuple = await KIDManager.KIDServerWebRequest<VerifyAgeResponse, VerifyAgeRequest>("VerifyAge", "POST", request, null, 2, null);
		long item = valueTuple.Item1;
		VerifyAgeData verifyAgeData = new VerifyAgeData(valueTuple.Item2);
		if (item < 200L || item >= 300L)
		{
			if (failureCallback != null)
			{
				failureCallback();
			}
		}
		return verifyAgeData;
	}

	// Token: 0x06004A4C RID: 19020 RVA: 0x0018B2A0 File Offset: 0x001894A0
	private static async Task<AttemptAgeUpdateData> Server_AttemptAgeUpdate(AttemptAgeUpdateRequest request, Action failureCallback)
	{
		ValueTuple<long, AttemptAgeUpdateResponse, string> valueTuple = await KIDManager.KIDServerWebRequest<AttemptAgeUpdateResponse, AttemptAgeUpdateRequest>("AttemptAgeUpdate", "POST", request, null, 2, null);
		long item = valueTuple.Item1;
		AttemptAgeUpdateResponse item2 = valueTuple.Item2;
		if (item != 200L)
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] Attempt age update request failed. Code: {0}", item));
		}
		return new AttemptAgeUpdateData(item2.Status);
	}

	// Token: 0x06004A4D RID: 19021 RVA: 0x0018B2E4 File Offset: 0x001894E4
	private static async Task<bool> Server_AppealAge(AppealAgeRequest request, Action failureCallback)
	{
		bool success = false;
		long num = await KIDManager.KIDServerWebRequestNoResponse<AppealAgeRequest>("AppealAge", "POST", request, 2, null);
		if (num == 200L)
		{
			success = true;
		}
		else
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] Appeal age request failed. Code: {0}", num));
		}
		return success;
	}

	// Token: 0x06004A4E RID: 19022 RVA: 0x0018B328 File Offset: 0x00189528
	private static async Task<ValueTuple<bool, string>> Server_SendChallengeEmail(SendChallengeEmailRequest request)
	{
		bool success = false;
		ValueTuple<long, object, string> valueTuple = await KIDManager.KIDServerWebRequest<object, SendChallengeEmailRequest>("SendChallengeEmail", "POST", request, null, 2, null);
		long item = valueTuple.Item1;
		string item2 = valueTuple.Item3;
		ValueTuple<bool, string> valueTuple2;
		if (item >= 200L && item < 300L)
		{
			success = true;
			valueTuple2 = new ValueTuple<bool, string>(success, string.Empty);
		}
		else
		{
			Debug.Log(string.Format("[KID::SERVER_ROUTER::Server_SendChallengeEmail] Send challenge email request failed. Code: [{0}] ErrorMessage: [{1}]", item, item2));
			string text = "Oops, something went wrong.";
			ErrorContent errorContent = new ErrorContent
			{
				Error = "Unhandled",
				Message = "This error is unhandled"
			};
			try
			{
				errorContent = JsonConvert.DeserializeObject<ErrorContent>(item2);
			}
			catch (Exception)
			{
				Debug.LogError("Could not deserialize error message");
			}
			if (item <= 403L)
			{
				if (item != 400L)
				{
					if (item == 403L)
					{
						text = "This account has been banned. Please contact Customer Support.";
						Debug.LogError("[KID::SERVER_ROUTER::Server_SendChallengeEmail] Account is banned for player ID: [" + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "]");
					}
				}
				else if (errorContent.Error.ToLower().Contains("BadRequest-InvalidEmail".ToLower()))
				{
					text = "This email doesn't seem right. Please check and try again.";
					Debug.LogError("[KID::SERVER_ROUTER::Server_SendChallengeEmail] Invalid email format: [" + request.Email + "]");
				}
				else if (errorContent.Error.ToLower().Contains("BadRequest-PlayerDataNotFound".ToLower()))
				{
					text = "Something went wrong. Please reboot the game and try again.";
					Debug.LogError("[KID::SERVER_ROUTER::Server_SendChallengeEmail] Player data not found for player ID: [" + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "]");
				}
				else if (errorContent.Error.ToLower().Contains("BadRequest-ChallengeNotFound".ToLower()))
				{
					text = "Couldn't find your challenge. If this keeps happening, contact Customer Support.";
					Debug.LogError("[KID::SERVER_ROUTER::Server_SendChallengeEmail] Challenge not found for player ID: [" + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "]");
				}
			}
			else if (item != 429L)
			{
				if (item == 500L)
				{
					if (errorContent.Error.ToLower().Contains("InternalServerError-FailedToRetrievePlayerData".ToLower()))
					{
						text = "We couldn't find your player data. Please reboot the game and try again.";
						Debug.LogError("[KID::SERVER_ROUTER::Server_SendChallengeEmail] Failed to retrieve player data for player ID: [" + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "]");
					}
					else if (errorContent.Error.ToLower().Contains("InternalServerError-UnhandledException".ToLower()))
					{
						text = "Something went wrong. Please reboot the game and try again.";
						Debug.LogError("[KID::SERVER_ROUTER::Server_SendChallengeEmail] Unhandled exception for player ID: [" + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "]");
					}
					else if (errorContent.Error.ToLower().Contains("InternalServerError-SendEmail".ToLower()))
					{
						text = "Something went wrong while sending the email. Please reboot the game and try again.";
						Debug.LogError("[KID::SERVER_ROUTER::Server_SendChallengeEmail] Failed to send email for player ID: [" + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "]");
					}
				}
			}
			else
			{
				text = "You've sent too many! Please wait a moment and try again.";
				Debug.LogError("[KID::SERVER_ROUTER::Server_SendChallengeEmail] Too many requests for player ID: [" + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "]");
			}
			valueTuple2 = new ValueTuple<bool, string>(success, text);
		}
		return valueTuple2;
	}

	// Token: 0x06004A4F RID: 19023 RVA: 0x0018B36C File Offset: 0x0018956C
	private static async Task<bool> Server_SetOptInPermissions(SetOptInPermissionsRequest request, Action failureCallback)
	{
		bool success = false;
		Debug.Log("[KID::SERVER_ROUTER::OptInRefactor] Setting opt-in permissions with request: " + JsonConvert.SerializeObject(request));
		long num = await KIDManager.KIDServerWebRequestNoResponse<SetOptInPermissionsRequest>("SetOptInPermissions", "POST", request, 2, null);
		if (num >= 200L && num < 300L)
		{
			success = true;
		}
		else
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] SetOptInPermissions request failed. Code: {0}", num));
			if (failureCallback != null)
			{
				failureCallback();
			}
		}
		Debug.Log(string.Format("[KID::SERVER_ROUTER::OptInRefactor] SetOptInPermissions request completed with success: {0} - code: {1}", success, num));
		return success;
	}

	// Token: 0x06004A50 RID: 19024 RVA: 0x0018B3B8 File Offset: 0x001895B8
	private static async Task<bool> Server_OptIn()
	{
		long num = await KIDManager.KIDServerWebRequestNoResponse<KIDRequestData>("OptIn", "POST", null, 2, null);
		bool flag;
		if (num == 200L)
		{
			flag = true;
		}
		else
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] Opt in request failed. Code: {0}", num));
			flag = false;
		}
		return flag;
	}

	// Token: 0x06004A51 RID: 19025 RVA: 0x0018B3F4 File Offset: 0x001895F4
	private static async Task<GetRequirementsData> Server_GetRequirements()
	{
		ValueTuple<long, GetRequirementsResponse, string> valueTuple = await KIDManager.KIDServerWebRequest<GetRequirementsResponse, KIDRequestData>("GetRequirements", "GET", null, null, 3, null);
		long item = valueTuple.Item1;
		GetRequirementsResponse item2 = valueTuple.Item2;
		GetRequirementsData getRequirementsData = new GetRequirementsData
		{
			AgeGateRequirements = item2
		};
		GetRequirementsData getRequirementsData2;
		if (item == 200L)
		{
			getRequirementsData2 = getRequirementsData;
		}
		else
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] Get Age-gate Requirements FAILED. Code: {0}", item));
			getRequirementsData2 = getRequirementsData;
		}
		return getRequirementsData2;
	}

	// Token: 0x06004A52 RID: 19026 RVA: 0x0018B430 File Offset: 0x00189630
	[return: TupleElementNames(new string[] { "code", "responseModel", "errorMessage" })]
	private static async Task<ValueTuple<long, T, string>> KIDServerWebRequest<T, Q>(string endpoint, string operationType, Q requestData, string queryParams = null, int maxRetries = 2, Func<long, bool> responseCodeIsRetryable = null) where T : class where Q : KIDRequestData
	{
		int retryCount = 0;
		string URL = "/api/" + endpoint;
		if (!string.IsNullOrEmpty(queryParams))
		{
			URL = URL + "?" + queryParams;
		}
		Debug.Log("[KID::MANAGER::SERVER_ROUTER] URL: " + URL);
		ValueTuple<long, T, string> valueTuple;
		for (;;)
		{
			using (UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.KidApiBaseUrl + URL, operationType))
			{
				byte[] array = Array.Empty<byte>();
				string json = "";
				if (requestData != null)
				{
					json = JsonConvert.SerializeObject(requestData);
					array = Encoding.UTF8.GetBytes(json);
				}
				request.uploadHandler = new UploadHandlerRaw(array);
				request.downloadHandler = new DownloadHandlerBuffer();
				request.SetRequestHeader("Content-Type", "application/json");
				request.SetRequestHeader("X-Authorization", PlayFabSettings.staticPlayer.ClientSessionTicket);
				request.SetRequestHeader("X-PlayerId", PlayFabSettings.staticPlayer.PlayFabId);
				request.SetRequestHeader("X-Mothership-Token", MothershipClientContext.Token);
				request.SetRequestHeader("X-Mothership-Player-Id", MothershipClientContext.MothershipId);
				request.SetRequestHeader("X-Mothership-Env-Id", MothershipClientApiUnity.EnvironmentId);
				request.SetRequestHeader("X-Mothership-Deployment-Id", MothershipClientApiUnity.DeploymentId);
				if (!PlayFabAuthenticatorSettings.KidApiBaseUrl.Contains("gtag-cf.com"))
				{
					request.SetRequestHeader("CF-IPCountry", RegionInfo.CurrentRegion.TwoLetterISORegionName);
				}
				request.timeout = 15;
				UnityWebRequest unityWebRequest = await request.SendWebRequest();
				if (unityWebRequest.result == UnityWebRequest.Result.Success)
				{
					if (typeof(T) == typeof(object))
					{
						valueTuple = new ValueTuple<long, T, string>(unityWebRequest.responseCode, default(T), unityWebRequest.error);
						break;
					}
					try
					{
						T t = JsonConvert.DeserializeObject<T>(unityWebRequest.downloadHandler.text);
						valueTuple = new ValueTuple<long, T, string>(unityWebRequest.responseCode, t, unityWebRequest.error);
						break;
					}
					catch (Exception)
					{
						Debug.LogError("[KID::SERVER_ROUTER] Failed to convert to class type [T] via JSON:\n[" + unityWebRequest.downloadHandler.text + "]");
						valueTuple = new ValueTuple<long, T, string>(unityWebRequest.responseCode, default(T), unityWebRequest.error);
						break;
					}
				}
				bool flag = request.result != UnityWebRequest.Result.ProtocolError;
				if (!flag)
				{
					bool flag2;
					if (responseCodeIsRetryable != null)
					{
						flag2 = responseCodeIsRetryable(request.responseCode);
					}
					else
					{
						long responseCode = request.responseCode;
						if (responseCode >= 500L)
						{
							if (responseCode >= 600L)
							{
								goto IL_035E;
							}
						}
						else if (responseCode != 408L && responseCode != 429L)
						{
							goto IL_035E;
						}
						bool flag3 = true;
						goto IL_0361;
						IL_035E:
						flag3 = false;
						IL_0361:
						flag2 = flag3;
					}
					flag = flag2;
				}
				if (flag)
				{
					if (retryCount < maxRetries)
					{
						float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(++retryCount)));
						Debug.LogWarning(string.Concat(new string[] { "[KID::SERVER_ROUTER] Tried sending request [", operationType, " - ", endpoint, "] but it failed:\n", unityWebRequest.error, "\n\nRequest:\n", json }));
						Debug.LogWarning(string.Format("[KID::SERVER_ROUTER] Retrying {0}... Retry attempt #{1}, waiting for {2} seconds", endpoint, retryCount, num));
						await Task.Delay(TimeSpan.FromSeconds((double)num));
						continue;
					}
					Debug.LogError(string.Concat(new string[] { "[KID::SERVER_ROUTER] Tried sending request [", operationType, " - ", endpoint, "] but it failed:\n", unityWebRequest.error, "\n\nRequest:\n", json }));
					Debug.LogError("[KID::SERVER_ROUTER] Maximum retries attempted. Please check your network connection.");
				}
				if (request.result == UnityWebRequest.Result.ProtocolError)
				{
					Debug.LogError(string.Format("[KID::SERVER_ROUTER] HTTP {0} ERROR: {1}\nMessage: {2}", request.responseCode, request.error, request.downloadHandler.text));
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.LogError("[KID::SERVER_ROUTER] NETWORK ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
					if (KIDUI_Controller.Instance != null)
					{
						KIDMessagingController.ShowConnectionErrorScreen();
					}
				}
				else
				{
					Debug.LogError("[KID::SERVER_ROUTER] ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
				}
				valueTuple = new ValueTuple<long, T, string>(unityWebRequest.responseCode, default(T), unityWebRequest.downloadHandler.text);
			}
			break;
		}
		return valueTuple;
	}

	// Token: 0x06004A53 RID: 19027 RVA: 0x0018B4A0 File Offset: 0x001896A0
	private static async Task<long> KIDServerWebRequestNoResponse<Q>(string endpoint, string operationType, Q requestData, int maxRetries = 2, Func<long, bool> responseCodeIsRetryable = null) where Q : KIDRequestData
	{
		TaskAwaiter<ValueTuple<long, object, string>> taskAwaiter = KIDManager.KIDServerWebRequest<object, Q>(endpoint, operationType, requestData, null, maxRetries, responseCodeIsRetryable).GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<ValueTuple<long, object, string>> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<ValueTuple<long, object, string>>);
		}
		return taskAwaiter.GetResult().Item1;
	}

	// Token: 0x06004A54 RID: 19028 RVA: 0x0018B504 File Offset: 0x00189704
	public static void RegisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Combine(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	// Token: 0x06004A55 RID: 19029 RVA: 0x0018B51B File Offset: 0x0018971B
	public static void UnregisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Remove(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	// Token: 0x06004A56 RID: 19030 RVA: 0x0018B532 File Offset: 0x00189732
	public static void RegisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	// Token: 0x06004A57 RID: 19031 RVA: 0x0018B549 File Offset: 0x00189749
	public static void UnregisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	// Token: 0x06004A58 RID: 19032 RVA: 0x0018B560 File Offset: 0x00189760
	public static void RegisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	// Token: 0x06004A59 RID: 19033 RVA: 0x0018B577 File Offset: 0x00189777
	public static void UnregisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	// Token: 0x06004A5A RID: 19034 RVA: 0x0018B58E File Offset: 0x0018978E
	public static void RegisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	// Token: 0x06004A5B RID: 19035 RVA: 0x0018B5A5 File Offset: 0x001897A5
	public static void UnregisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	// Token: 0x06004A5C RID: 19036 RVA: 0x0018B5BC File Offset: 0x001897BC
	public static void RegisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	// Token: 0x06004A5D RID: 19037 RVA: 0x0018B5D3 File Offset: 0x001897D3
	public static void UnregisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	// Token: 0x06004A5E RID: 19038 RVA: 0x0018B5EA File Offset: 0x001897EA
	public static void RegisterSessionUpdatedCallback_UGC(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_UGC = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_UGC, callback);
	}

	// Token: 0x06004A5F RID: 19039 RVA: 0x0018B604 File Offset: 0x00189804
	public static async Task<bool> WaitForAndUpdateNewSession(bool forceRefresh)
	{
		bool flag;
		if (KIDManager._isUpdatingNewSession)
		{
			flag = false;
		}
		else
		{
			KIDManager._isUpdatingNewSession = true;
			float updateTimeout = Time.realtimeSinceStartup + 600f;
			GetPlayerData_Data getPlayerData_Data = await KIDManager.TryGetPlayerData(forceRefresh);
			TMPSession tmpsession = ((getPlayerData_Data != null) ? getPlayerData_Data.session : null);
			bool flag2 = KIDManager.HasSessionChanged(tmpsession);
			while (Time.realtimeSinceStartup < updateTimeout && (tmpsession == null || tmpsession.Age == 0 || !flag2))
			{
				await Task.Delay(30000);
				if (KIDManager._requestCancellationSource.IsCancellationRequested)
				{
					KIDManager._isUpdatingNewSession = false;
					return false;
				}
				getPlayerData_Data = await KIDManager.TryGetPlayerData(forceRefresh);
				tmpsession = ((getPlayerData_Data != null) ? getPlayerData_Data.session : null);
				flag2 = KIDManager.HasSessionChanged(tmpsession);
				if (flag2)
				{
					break;
				}
				if (getPlayerData_Data == null)
				{
					Debug.LogError("[KID::MANAGER] UpdateNewSession -- LOOP - Tried getting Player Data but returned NULL");
				}
				else if (getPlayerData_Data.responseType == GetSessionResponseType.ERROR)
				{
					Debug.LogError("[KID::MANAGER] UpdateNewSession -- LOOP - Tried getting a new Session but playerData returned with ERROR");
				}
				else if (tmpsession == null)
				{
					Debug.LogError("[KID::MANAGER] UpdateNewSession -- LOOP - Found Player Data, but SESSION was NULL");
				}
			}
			KIDManager._isUpdatingNewSession = false;
			if (getPlayerData_Data == null || getPlayerData_Data.responseType != GetSessionResponseType.OK || tmpsession == null)
			{
				flag = false;
			}
			else
			{
				flag = KIDManager.UpdatePermissions(tmpsession);
			}
		}
		return flag;
	}

	// Token: 0x06004A60 RID: 19040 RVA: 0x0018B648 File Offset: 0x00189848
	private static bool HasSessionChanged(TMPSession newSession)
	{
		if (newSession == null)
		{
			return false;
		}
		if (KIDManager.CurrentSession == null)
		{
			return true;
		}
		if (!newSession.IsValidSession)
		{
			return false;
		}
		if (newSession.IsDefault)
		{
			Debug.LogError(string.Format("[KID::MANAGER] DEBUG - New Session Is Default! Age: [{0}]", newSession.Age));
			return false;
		}
		return KIDManager.CurrentSession.IsDefault || !newSession.Etag.Equals(KIDManager.CurrentSession.Etag);
	}

	// Token: 0x06004A61 RID: 19041 RVA: 0x0018B6BC File Offset: 0x001898BC
	private static void OnSessionUpdated()
	{
		Action onSessionUpdated_AnyPermission = KIDManager._onSessionUpdated_AnyPermission;
		if (onSessionUpdated_AnyPermission != null)
		{
			onSessionUpdated_AnyPermission();
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		int count = allPermissionsData.Count;
		for (int i = 0; i < count; i++)
		{
			Permission permission = allPermissionsData[i];
			string name = permission.Name;
			if (!(name == "voice-chat"))
			{
				if (!(name == "custom-username"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "multiplayer"))
						{
							if (!(name == "mods"))
							{
								Debug.Log("[KID] Tried updating permission with name [" + permission.Name + "] but did not match any of the set cases. Unable to process");
							}
							else if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_UGC = KIDManager._onSessionUpdated_UGC;
								if (onSessionUpdated_UGC != null)
								{
									onSessionUpdated_UGC(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
						}
						else
						{
							if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_Multiplayer = KIDManager._onSessionUpdated_Multiplayer;
								if (onSessionUpdated_Multiplayer != null)
								{
									onSessionUpdated_Multiplayer(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
							bool enabled = permission.Enabled;
						}
					}
					else
					{
						if (KIDManager.HasPermissionChanged(permission))
						{
							Action<bool, Permission.ManagedByEnum> onSessionUpdated_PrivateRooms = KIDManager._onSessionUpdated_PrivateRooms;
							if (onSessionUpdated_PrivateRooms != null)
							{
								onSessionUpdated_PrivateRooms(permission.Enabled, permission.ManagedBy);
							}
							KIDManager._previousPermissionSettings[permission.Name] = permission;
						}
						flag2 = permission.Enabled;
					}
				}
				else
				{
					if (KIDManager.HasPermissionChanged(permission))
					{
						Action<bool, Permission.ManagedByEnum> onSessionUpdated_CustomUsernames = KIDManager._onSessionUpdated_CustomUsernames;
						if (onSessionUpdated_CustomUsernames != null)
						{
							onSessionUpdated_CustomUsernames(permission.Enabled, permission.ManagedBy);
						}
						KIDManager._previousPermissionSettings[permission.Name] = permission;
					}
					flag3 = permission.Enabled;
				}
			}
			else
			{
				if (KIDManager.HasPermissionChanged(permission))
				{
					Action<bool, Permission.ManagedByEnum> onSessionUpdated_VoiceChat = KIDManager._onSessionUpdated_VoiceChat;
					if (onSessionUpdated_VoiceChat != null)
					{
						onSessionUpdated_VoiceChat(permission.Enabled, permission.ManagedBy);
					}
					KIDManager._previousPermissionSettings[permission.Name] = permission;
				}
				flag = permission.Enabled;
			}
		}
		GorillaTelemetry.PostKidEvent(flag2, flag, flag3, KIDManager.CurrentSession.AgeStatus, GTKidEventType.permission_update);
	}

	// Token: 0x06004A62 RID: 19042 RVA: 0x0018B8F0 File Offset: 0x00189AF0
	private static bool HasPermissionChanged(Permission newValue)
	{
		Permission permission;
		if (KIDManager._previousPermissionSettings.TryGetValue(newValue.Name, out permission))
		{
			return permission.Enabled != newValue.Enabled || permission.ManagedBy != newValue.ManagedBy;
		}
		KIDManager._previousPermissionSettings.Add(newValue.Name, newValue);
		return true;
	}

	// Token: 0x04005C8B RID: 23691
	public const string MULTIPLAYER_PERMISSION_NAME = "multiplayer";

	// Token: 0x04005C8C RID: 23692
	public const string UGC_PERMISSION_NAME = "mods";

	// Token: 0x04005C8D RID: 23693
	public const string PRIVATE_ROOM_PERMISSION_NAME = "join-groups";

	// Token: 0x04005C8E RID: 23694
	public const string VOICE_CHAT_PERMISSION_NAME = "voice-chat";

	// Token: 0x04005C8F RID: 23695
	public const string CUSTOM_USERNAME_PERMISSION_NAME = "custom-username";

	// Token: 0x04005C90 RID: 23696
	public const string PREVIOUS_STATUS_PREF_KEY_PREFIX = "previous-status-";

	// Token: 0x04005C91 RID: 23697
	public const string KID_DATA_KEY = "KIDData";

	// Token: 0x04005C92 RID: 23698
	private const string KID_EMAIL_KEY = "k-id_EmailAddress";

	// Token: 0x04005C93 RID: 23699
	private const int SECONDS_BETWEEN_UPDATE_ATTEMPTS = 30;

	// Token: 0x04005C94 RID: 23700
	private const string KID_SETUP_FLAG = "KID-Setup-";

	// Token: 0x04005C95 RID: 23701
	[OnEnterPlay_SetNull]
	private static KIDManager _instance;

	// Token: 0x04005C9A RID: 23706
	private static string _emailAddress;

	// Token: 0x04005C9B RID: 23707
	private static CancellationTokenSource _requestCancellationSource = new CancellationTokenSource();

	// Token: 0x04005C9C RID: 23708
	private static bool _titleDataReady = false;

	// Token: 0x04005C9D RID: 23709
	private static bool _useKid = false;

	// Token: 0x04005C9E RID: 23710
	private static int _kIDPhase = 0;

	// Token: 0x04005C9F RID: 23711
	private static DateTime? _kIDNewPlayerDateTime = null;

	// Token: 0x04005CA3 RID: 23715
	private static string _debugKIDLocalePlayerPrefRef = "KID_SPOOF_LOCALE";

	// Token: 0x04005CA4 RID: 23716
	private static string parentEmailForUserPlayerPrefRef;

	// Token: 0x04005CA5 RID: 23717
	[OnEnterPlay_SetNull]
	private static Action _sessionUpdatedCallback = null;

	// Token: 0x04005CA6 RID: 23718
	[OnEnterPlay_SetNull]
	private static Action _onKIDInitialisationComplete = null;

	// Token: 0x04005CA7 RID: 23719
	public static KIDManager.OnEmailResultReceived onEmailResultReceived;

	// Token: 0x04005CA8 RID: 23720
	private const string KID_GET_SESSION = "GetPlayerData";

	// Token: 0x04005CA9 RID: 23721
	private const string KID_VERIFY_AGE = "VerifyAge";

	// Token: 0x04005CAA RID: 23722
	private const string KID_UPGRADE_SESSION = "UpgradeSession";

	// Token: 0x04005CAB RID: 23723
	private const string KID_SEND_CHALLENGE_EMAIL = "SendChallengeEmail";

	// Token: 0x04005CAC RID: 23724
	private const string KID_ATTEMPT_AGE_UPDATE = "AttemptAgeUpdate";

	// Token: 0x04005CAD RID: 23725
	private const string KID_APPEAL_AGE = "AppealAge";

	// Token: 0x04005CAE RID: 23726
	private const string KID_OPT_IN = "OptIn";

	// Token: 0x04005CAF RID: 23727
	private const string KID_GET_REQUIREMENTS = "GetRequirements";

	// Token: 0x04005CB0 RID: 23728
	private const string KID_SET_CONFIRMED_STATUS = "SetConfirmedStatus";

	// Token: 0x04005CB1 RID: 23729
	private const string KID_SET_OPT_IN_PERMISSIONS = "SetOptInPermissions";

	// Token: 0x04005CB2 RID: 23730
	private const string KID_FORCE_REFRESH = "sessionRefresh";

	// Token: 0x04005CB3 RID: 23731
	private const int MAX_RETRIES_FOR_CRITICAL_KID_SERVER_REQUESTS = 3;

	// Token: 0x04005CB4 RID: 23732
	private const int MAX_RETRIES_FOR_NORMAL_KID_SERVER_REQUESTS = 2;

	// Token: 0x04005CB5 RID: 23733
	public const string KID_PERMISSION__VOICE_CHAT = "voice-chat";

	// Token: 0x04005CB6 RID: 23734
	public const string KID_PERMISSION__CUSTOM_NAMES = "custom-username";

	// Token: 0x04005CB7 RID: 23735
	public const string KID_PERMISSION__PRIVATE_ROOMS = "join-groups";

	// Token: 0x04005CB8 RID: 23736
	public const string KID_PERMISSION__MULTIPLAYER = "multiplayer";

	// Token: 0x04005CB9 RID: 23737
	public const string KID_PERMISSION__UGC = "mods";

	// Token: 0x04005CBA RID: 23738
	private const float MAX_SESSION_UPDATE_TIME = 600f;

	// Token: 0x04005CBB RID: 23739
	private const int TIME_BETWEEN_SESSION_UPDATE_ATTEMPTS = 30;

	// Token: 0x04005CBC RID: 23740
	[OnEnterPlay_SetNull]
	private static Action _onSessionUpdated_AnyPermission;

	// Token: 0x04005CBD RID: 23741
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_VoiceChat;

	// Token: 0x04005CBE RID: 23742
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_CustomUsernames;

	// Token: 0x04005CBF RID: 23743
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_PrivateRooms;

	// Token: 0x04005CC0 RID: 23744
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_Multiplayer;

	// Token: 0x04005CC1 RID: 23745
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_UGC;

	// Token: 0x04005CC2 RID: 23746
	private static bool _isUpdatingNewSession = false;

	// Token: 0x04005CC3 RID: 23747
	[OnEnterPlay_SetNull]
	private static Dictionary<string, Permission> _previousPermissionSettings = new Dictionary<string, Permission>();

	// Token: 0x02000B63 RID: 2915
	// (Invoke) Token: 0x06004A66 RID: 19046
	public delegate void OnEmailResultReceived(bool result);
}
