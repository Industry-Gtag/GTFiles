using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GorillaExtensions;
using JetBrains.Annotations;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking
{
	// Token: 0x0200110E RID: 4366
	public class PlayFabAuthenticator : MonoBehaviour
	{
		// Token: 0x17000A66 RID: 2662
		// (get) Token: 0x06006D81 RID: 28033 RVA: 0x002361EC File Offset: 0x002343EC
		public GorillaComputer gorillaComputer
		{
			get
			{
				return GorillaComputer.instance;
			}
		}

		// Token: 0x17000A67 RID: 2663
		// (get) Token: 0x06006D82 RID: 28034 RVA: 0x002361F5 File Offset: 0x002343F5
		// (set) Token: 0x06006D83 RID: 28035 RVA: 0x002361FD File Offset: 0x002343FD
		public bool IsReturningPlayer { get; private set; }

		// Token: 0x17000A68 RID: 2664
		// (get) Token: 0x06006D84 RID: 28036 RVA: 0x00236206 File Offset: 0x00234406
		// (set) Token: 0x06006D85 RID: 28037 RVA: 0x0023620E File Offset: 0x0023440E
		public bool postAuthSetSafety { get; private set; }

		// Token: 0x06006D86 RID: 28038 RVA: 0x00236218 File Offset: 0x00234418
		private void Awake()
		{
			if (PlayFabAuthenticator.instance == null)
			{
				PlayFabAuthenticator.instance = this;
			}
			else if (PlayFabAuthenticator.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			if (PlayFabAuthenticator.instance.photonAuthenticator == null)
			{
				PlayFabAuthenticator.instance.photonAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<PhotonAuthenticator>();
			}
			this.platform = ScriptableObject.CreateInstance<PlatformTagJoin>();
			PlayFabSettings.CompressApiData = false;
			new byte[1];
			if (this.screenDebugMode)
			{
				this.debugText.text = "";
			}
			Debug.Log("doing steam thing");
			if (PlayFabAuthenticator.instance.steamAuthenticator == null)
			{
				PlayFabAuthenticator.instance.steamAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
			}
			this.platform.PlatformTag = "Steam";
			PlayFabSettings.TitleId = PlayFabAuthenticatorSettings.TitleId;
			PlayFabSettings.DisableFocusTimeCollection = true;
			this.BeginLoginFlow();
		}

		// Token: 0x06006D87 RID: 28039 RVA: 0x0023631C File Offset: 0x0023451C
		public void BeginLoginFlow()
		{
			if (!MothershipClientApiUnity.IsEnabled())
			{
				this.AuthenticateWithPlayFab();
				return;
			}
			if (PlayFabAuthenticator.instance.mothershipAuthenticator == null)
			{
				PlayFabAuthenticator.instance.mothershipAuthenticator = MothershipAuthenticator.Instance ?? PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<MothershipAuthenticator>();
				MothershipAuthenticator mothershipAuthenticator = PlayFabAuthenticator.instance.mothershipAuthenticator;
				mothershipAuthenticator.OnLoginSuccess = (Action)Delegate.Combine(mothershipAuthenticator.OnLoginSuccess, new Action(delegate
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}));
				MothershipAuthenticator mothershipAuthenticator2 = PlayFabAuthenticator.instance.mothershipAuthenticator;
				mothershipAuthenticator2.OnLoginFailure = (Action<string, string, string>)Delegate.Combine(mothershipAuthenticator2.OnLoginFailure, new Action<string, string, string>(delegate(string errorMessage, string errorCode, string traceId)
				{
					this.SetLoginFailed();
					this.ShowMothershipAuthErrorMessage(errorMessage, errorCode, traceId);
				}));
				PlayFabAuthenticator.instance.mothershipAuthenticator.BeginLoginFlow();
			}
		}

		// Token: 0x06006D88 RID: 28040 RVA: 0x002363F4 File Offset: 0x002345F4
		private void SetLoginFailed()
		{
			this.loginFailed = true;
			NetworkSystem networkSystem = NetworkSystem.Instance;
			if (networkSystem == null)
			{
				return;
			}
			networkSystem.FinishAuthenticating();
		}

		// Token: 0x06006D89 RID: 28041 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void Start()
		{
		}

		// Token: 0x06006D8A RID: 28042 RVA: 0x0023640C File Offset: 0x0023460C
		private void OnEnable()
		{
			NetworkSystem.Instance.OnCustomAuthenticationResponse += this.OnCustomAuthenticationResponse;
		}

		// Token: 0x06006D8B RID: 28043 RVA: 0x00236424 File Offset: 0x00234624
		private void OnDisable()
		{
			NetworkSystem.Instance.OnCustomAuthenticationResponse -= this.OnCustomAuthenticationResponse;
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			SteamAuthTicket steamAuthTicket2 = this.steamAuthTicketForPlayFab;
			if (steamAuthTicket2 == null)
			{
				return;
			}
			steamAuthTicket2.Dispose();
		}

		// Token: 0x06006D8C RID: 28044 RVA: 0x0023645D File Offset: 0x0023465D
		public void RefreshSteamAuthTicketForPhoton(Action<string> successCallback, Action<EResult> failureCallback)
		{
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			this.steamAuthTicketForPhoton = this.steamAuthenticator.GetAuthTicketForWebApi(this.steamAuthIdForPhoton, successCallback, failureCallback);
		}

		// Token: 0x06006D8D RID: 28045 RVA: 0x00236490 File Offset: 0x00234690
		private void OnCustomAuthenticationResponse(Dictionary<string, object> response)
		{
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			object obj;
			if (response.TryGetValue("SteamAuthIdForPhoton", out obj))
			{
				string text = obj as string;
				if (text != null)
				{
					this.steamAuthIdForPhoton = text;
					return;
				}
			}
			this.steamAuthIdForPhoton = null;
		}

		// Token: 0x06006D8E RID: 28046 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void GetNonceForPlayFab()
		{
		}

		// Token: 0x06006D8F RID: 28047 RVA: 0x002364D8 File Offset: 0x002346D8
		private void OnPlayFabAuthResponse(PlayFabAuthenticator.PlayfabAuthResponseData response)
		{
			Debug.Log("[PLAYFAB] Response Received. Response is: [" + (((response != null) ? response.PlayFabId : null) ?? "NULL") + "]");
			if (response != null)
			{
				PlayFabSettings.staticPlayer = new PlayFabAuthenticationContext(response.SessionTicket, response.EntityToken, response.PlayFabId, response.EntityId, response.EntityType);
				this._playFabPlayerIdCache = response.PlayFabId;
				this._sessionTicket = response.SessionTicket;
				DateTime dateTime;
				if (DateTime.TryParse(response.AccountCreationIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime))
				{
					base.StartCoroutine(this.VerifyKidAuthenticated(dateTime));
				}
				this.AdvanceLogin();
				return;
			}
			Debug.LogError("Error: Could not authenticate with PlayFab");
			this.SetLoginFailed();
		}

		// Token: 0x06006D90 RID: 28048 RVA: 0x00236590 File Offset: 0x00234790
		public void AuthenticateWithPlayFab()
		{
			Debug.Log("authenticating with playFab!");
			GorillaServer gorillaServer = GorillaServer.Instance;
			if (gorillaServer != null && gorillaServer.FeatureFlagsReady)
			{
				if (KIDManager.KidEnabled)
				{
					Debug.Log("[KID] Is Enabled - Enabling safeties by platform and age category");
					this.DefaultSafetiesByAgeCategory();
				}
			}
			else
			{
				this.postAuthSetSafety = true;
			}
			if (SteamManager.Initialized)
			{
				this.userID = SteamUser.GetSteamID().ToString();
				Debug.Log("trying to auth with steam");
				this.steamAuthTicketForPlayFab = this.steamAuthenticator.GetAuthTicket(delegate(string ticket)
				{
					Debug.Log("Got steam auth session ticket!");
					PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
					{
						CreateAccount = new bool?(true),
						SteamTicket = ticket
					}, new Action<LoginResult>(this.OnLoginWithSteamResponse), new Action<PlayFabError>(this.OnPlayFabError), null, null);
				}, delegate(EResult result)
				{
					base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
				});
				return;
			}
			base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
		}

		// Token: 0x06006D91 RID: 28049 RVA: 0x00236642 File Offset: 0x00234842
		private IEnumerator VerifyKidAuthenticated(DateTime accountCreationDateTime)
		{
			Task<DateTime?> getNewPlayerDateTimeTask = KIDManager.CheckKIDNewPlayerDateTime();
			yield return new WaitUntil(() => getNewPlayerDateTimeTask.IsCompleted);
			DateTime? result = getNewPlayerDateTimeTask.Result;
			if (result != null && KIDManager.KidEnabled)
			{
				this.IsReturningPlayer = accountCreationDateTime < result;
			}
			yield break;
		}

		// Token: 0x06006D92 RID: 28050 RVA: 0x00236658 File Offset: 0x00234858
		private IEnumerator DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame()
		{
			yield return null;
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
				this.gorillaComputer.screenText.Set("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
				Debug.Log("Couldn't authenticate steam account");
			}
			else
			{
				Debug.LogError("PlayFabAuthenticator: gorillaComputer is null, so could not set GeneralFailureMessage notifying user that the steam account could not be authenticated.", this);
			}
			yield break;
		}

		// Token: 0x06006D93 RID: 28051 RVA: 0x00236668 File Offset: 0x00234868
		private void OnLoginWithSteamResponse(LoginResult obj)
		{
			this._playFabPlayerIdCache = obj.PlayFabId;
			this._sessionTicket = obj.SessionTicket;
			base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
			{
				Platform = this.platform.ToString(),
				SessionTicket = this._sessionTicket,
				PlayFabId = this._playFabPlayerIdCache,
				TitleId = PlayFabSettings.TitleId,
				MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
				MothershipToken = MothershipClientContext.Token,
				MothershipId = MothershipClientContext.MothershipId
			}, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
		}

		// Token: 0x06006D94 RID: 28052 RVA: 0x0023670C File Offset: 0x0023490C
		private void OnCachePlayFabIdRequest([CanBeNull] PlayFabAuthenticator.CachePlayFabIdResponse response)
		{
			if (response != null)
			{
				this.steamAuthIdForPhoton = response.SteamAuthIdForPhoton;
				DateTime dateTime;
				if (DateTime.TryParse(response.AccountCreationIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime))
				{
					base.StartCoroutine(this.VerifyKidAuthenticated(dateTime));
				}
				Debug.Log("Successfully cached PlayFab Id.  Continuing!");
				this.AdvanceLogin();
				return;
			}
			Debug.LogError("Could not cache PlayFab Id.  Cannot continue.");
		}

		// Token: 0x06006D95 RID: 28053 RVA: 0x0023676A File Offset: 0x0023496A
		private void AdvanceLogin()
		{
			this.LogMessage("PlayFab authenticated ... Getting Nonce");
			this.RefreshSteamAuthTicketForPhoton(delegate(string ticket)
			{
				this._nonce = ticket;
				Debug.Log("Got nonce!  Authenticating...");
				this.AuthenticateWithPhoton();
			}, delegate(EResult result)
			{
				Debug.LogWarning("Failed to get nonce!");
				this.AuthenticateWithPhoton();
			});
		}

		// Token: 0x06006D96 RID: 28054 RVA: 0x00236798 File Offset: 0x00234998
		private void AuthenticateWithPhoton()
		{
			this.photonAuthenticator.SetCustomAuthenticationParameters(new Dictionary<string, object>
			{
				{
					"AppId",
					PlayFabSettings.TitleId
				},
				{
					"AppVersion",
					NetworkSystemConfig.AppVersion ?? "-1"
				},
				{ "Ticket", this._sessionTicket },
				{ "Nonce", this._nonce },
				{
					"MothershipEnvId",
					MothershipClientApiUnity.EnvironmentId
				},
				{
					"MothershipDeploymentId",
					MothershipClientApiUnity.DeploymentId
				},
				{
					"MothershipToken",
					MothershipClientContext.Token
				}
			});
			this.GetPlayerDisplayName(this._playFabPlayerIdCache);
			GorillaServer.Instance.AddOrRemoveDLCOwnership(delegate(ExecuteFunctionResult result)
			{
				Debug.Log("got results! updating!");
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
				}
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error retrieving user data:");
				Debug.Log(error.GenerateErrorReport());
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
				}
			});
			if (CosmeticsController.instance != null)
			{
				Debug.Log("initializing cosmetics");
				CosmeticsController.instance.Initialize();
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.OnConnectedToMasterStuff();
			}
			else
			{
				base.StartCoroutine(this.ComputerOnConnectedToMaster());
			}
			if (RankedProgressionManager.Instance != null)
			{
				RankedProgressionManager.Instance.LoadStats();
			}
			if (PhotonNetworkController.Instance != null)
			{
				Debug.Log("Finish authenticating");
				NetworkSystem.Instance.FinishAuthenticating();
			}
		}

		// Token: 0x06006D97 RID: 28055 RVA: 0x0023690D File Offset: 0x00234B0D
		private IEnumerator ComputerOnConnectedToMaster()
		{
			WaitForEndOfFrame frameYield = new WaitForEndOfFrame();
			while (this.gorillaComputer == null)
			{
				yield return frameYield;
			}
			this.gorillaComputer.OnConnectedToMasterStuff();
			yield break;
		}

		// Token: 0x06006D98 RID: 28056 RVA: 0x0023691C File Offset: 0x00234B1C
		private void OnPlayFabError(PlayFabError obj)
		{
			this.LogMessage(obj.ErrorMessage);
			Debug.Log("OnPlayFabError(): " + obj.ErrorMessage);
			this.SetLoginFailed();
			if (obj.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (obj.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
					if (keyValuePair2.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
					return;
				}
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage(this.gorillaComputer.unableToConnect);
			}
		}

		// Token: 0x06006D99 RID: 28057 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void LogMessage(string message)
		{
		}

		// Token: 0x06006D9A RID: 28058 RVA: 0x00236B50 File Offset: 0x00234D50
		private void GetPlayerDisplayName(string playFabId)
		{
			GetPlayerProfileRequest getPlayerProfileRequest = new GetPlayerProfileRequest();
			getPlayerProfileRequest.PlayFabId = playFabId;
			getPlayerProfileRequest.ProfileConstraints = new PlayerProfileViewConstraints
			{
				ShowDisplayName = true
			};
			PlayFabClientAPI.GetPlayerProfile(getPlayerProfileRequest, delegate(GetPlayerProfileResult result)
			{
				this._displayName = result.PlayerProfile.DisplayName;
			}, delegate(PlayFabError error)
			{
				Debug.LogError(error.GenerateErrorReport());
			}, null, null);
		}

		// Token: 0x06006D9B RID: 28059 RVA: 0x00236BB0 File Offset: 0x00234DB0
		public void SetDisplayName(string playerName)
		{
			if (this._displayName == null || (this._displayName.Length > 4 && this._displayName.Substring(0, this._displayName.Length - 4) != playerName && this._displayName != playerName))
			{
				PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
				{
					DisplayName = playerName
				}, delegate(UpdateUserTitleDisplayNameResult result)
				{
					this._displayName = playerName;
				}, delegate(PlayFabError error)
				{
					Debug.LogError("Error with name: " + playerName + ". Error is " + error.GenerateErrorReport());
				}, null, null);
			}
		}

		// Token: 0x06006D9C RID: 28060 RVA: 0x00236C50 File Offset: 0x00234E50
		public void ScreenDebug(string debugString)
		{
			Debug.Log(debugString);
			if (this.screenDebugMode)
			{
				Text text = this.debugText;
				text.text = text.text + debugString + "\n";
			}
		}

		// Token: 0x06006D9D RID: 28061 RVA: 0x00236C7C File Offset: 0x00234E7C
		public void ScreenDebugClear()
		{
			this.debugText.text = "";
		}

		// Token: 0x06006D9E RID: 28062 RVA: 0x00236C8E File Offset: 0x00234E8E
		public IEnumerator PlayfabAuthenticate(PlayFabAuthenticator.PlayfabAuthRequestData data, Action<PlayFabAuthenticator.PlayfabAuthResponseData> callback)
		{
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/PlayFabAuthentication", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 30;
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
			{
				PlayFabAuthenticator.PlayfabAuthResponseData playfabAuthResponseData = JsonUtility.FromJson<PlayFabAuthenticator.PlayfabAuthResponseData>(request.downloadHandler.text);
				callback(playfabAuthResponseData);
			}
			else
			{
				if (request.responseCode == 403L)
				{
					Debug.LogError(string.Format("HTTP {0}: {1}, with body: {2}", request.responseCode, request.error, request.downloadHandler.text));
					PlayFabAuthenticator.BanInfo banInfo = JsonUtility.FromJson<PlayFabAuthenticator.BanInfo>(request.downloadHandler.text);
					this.ShowBanMessage(banInfo);
					callback(null);
				}
				if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1} message:{2}", request.responseCode, request.error, request.downloadHandler.text));
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					retry = true;
					Debug.LogError("NETWORK ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
				}
				else
				{
					Debug.LogError("HTTP ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
					retry = true;
				}
			}
			if (retry)
			{
				if (this.playFabAuthRetryCount < this.playFabMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabAuthRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabAuthRetryCount + 1, num));
					this.playFabAuthRetryCount++;
					yield return new WaitForSecondsRealtime((float)num);
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback(null);
					this.ShowPlayFabAuthErrorMessage(request.downloadHandler.text);
				}
			}
			yield break;
		}

		// Token: 0x06006D9F RID: 28063 RVA: 0x00236CAB File Offset: 0x00234EAB
		private void ShowMothershipAuthErrorMessage(string errorMessage, string errorCode, string traceId)
		{
			base.StartCoroutine(this.ShowMothershipAuthErrorMessageCoroutine(errorMessage, errorCode, traceId));
		}

		// Token: 0x06006DA0 RID: 28064 RVA: 0x00236CBD File Offset: 0x00234EBD
		private IEnumerator ShowMothershipAuthErrorMessageCoroutine(string errorMessage, string errorCode, string traceId)
		{
			WaitForEndOfFrame frameYield = new WaitForEndOfFrame();
			while (this.gorillaComputer == null)
			{
				yield return frameYield;
			}
			try
			{
				StringBuilder stringBuilder = new StringBuilder("UNABLE TO AUTHENTICATE WITH MOTHERSHIP.\nREASON: " + errorMessage);
				StringBuilder stringBuilder2 = stringBuilder;
				if (!char.IsPunctuation(stringBuilder2[stringBuilder2.Length - 1]))
				{
					stringBuilder.Append('.');
				}
				if (!string.IsNullOrEmpty(errorCode))
				{
					stringBuilder.Append("\nERROR CODE: " + errorCode);
				}
				if (!string.IsNullOrEmpty(traceId))
				{
					stringBuilder.Append("\nTRACE ID: " + traceId);
				}
				this.gorillaComputer.GeneralFailureMessage(stringBuilder.ToString());
				yield break;
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Failed to show Mothership auth error message: {0}", ex));
				yield break;
			}
			yield break;
		}

		// Token: 0x06006DA1 RID: 28065 RVA: 0x00236CE4 File Offset: 0x00234EE4
		private void ShowPlayFabAuthErrorMessage(string errorJson)
		{
			try
			{
				PlayFabAuthenticator.ErrorInfo errorInfo = JsonUtility.FromJson<PlayFabAuthenticator.ErrorInfo>(errorJson);
				StringBuilder stringBuilder = new StringBuilder("UNABLE TO AUTHENTICATE WITH PLAYFAB.\nREASON: " + errorInfo.Message);
				StringBuilder stringBuilder2 = stringBuilder;
				if (!char.IsPunctuation(stringBuilder2[stringBuilder2.Length - 1]))
				{
					stringBuilder.Append('.');
				}
				this.gorillaComputer.GeneralFailureMessage(stringBuilder.ToString());
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Failed to show PlayFab auth error message: {0}", ex));
			}
		}

		// Token: 0x06006DA2 RID: 28066 RVA: 0x00236D64 File Offset: 0x00234F64
		private void ShowBanMessage(PlayFabAuthenticator.BanInfo banInfo)
		{
			try
			{
				if (banInfo.BanExpirationTime != null && banInfo.BanMessage != null)
				{
					if (banInfo.BanExpirationTime != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + banInfo.BanMessage + "\nHOURS LEFT: " + ((int)((DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
					}
					else
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + banInfo.BanMessage);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Failed to show ban message: {0}", ex));
			}
		}

		// Token: 0x06006DA3 RID: 28067 RVA: 0x00236E28 File Offset: 0x00235028
		public IEnumerator CachePlayFabId(PlayFabAuthenticator.CachePlayFabIdRequest data, Action<PlayFabAuthenticator.CachePlayFabIdResponse> callback)
		{
			Debug.Log("Trying to cache playfab Id");
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/CachePlayFabId", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 30;
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
			{
				if (request.responseCode == 200L)
				{
					PlayFabAuthenticator.CachePlayFabIdResponse cachePlayFabIdResponse = JsonUtility.FromJson<PlayFabAuthenticator.CachePlayFabIdResponse>(request.downloadHandler.text);
					callback(cachePlayFabIdResponse);
				}
			}
			else if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
			{
				retry = true;
				Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
			}
			else
			{
				retry = request.result != UnityWebRequest.Result.ConnectionError || true;
			}
			if (retry)
			{
				if (this.playFabCacheRetryCount < this.playFabCacheMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabCacheRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabCacheRetryCount + 1, num));
					this.playFabCacheRetryCount++;
					yield return new WaitForSecondsRealtime((float)num);
					base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
					{
						Platform = this.platform.ToString(),
						SessionTicket = this._sessionTicket,
						PlayFabId = this._playFabPlayerIdCache,
						TitleId = PlayFabSettings.TitleId,
						MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
						MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
						MothershipToken = MothershipClientContext.Token,
						MothershipId = MothershipClientContext.MothershipId
					}, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback(null);
					this.ShowPlayFabAuthErrorMessage(request.downloadHandler.text);
				}
			}
			yield break;
		}

		// Token: 0x06006DA4 RID: 28068 RVA: 0x00236E45 File Offset: 0x00235045
		public void DefaultSafetiesByAgeCategory()
		{
			Debug.Log("[KID::PLAYFAB_AUTHENTICATOR] Defaulting Safety Settings to Disabled because age category data unavailable on this platform");
			this.SetSafety(false, true, false);
		}

		// Token: 0x06006DA5 RID: 28069 RVA: 0x00236E5C File Offset: 0x0023505C
		public void SetSafety(bool isSafety, bool isAutoSet, bool setPlayfab = false)
		{
			this.postAuthSetSafety = false;
			Action<bool> onSafetyUpdate = this.OnSafetyUpdate;
			if (onSafetyUpdate != null)
			{
				onSafetyUpdate(isSafety);
			}
			Debug.Log("[KID] Setting safety to: [" + isSafety.ToString() + "]");
			this.isSafeAccount = isSafety;
			this.safetyType = PlayFabAuthenticator.SafetyType.None;
			if (!isSafety)
			{
				if (isAutoSet)
				{
					PlayerPrefs.SetInt("autoSafety", 0);
				}
				else
				{
					PlayerPrefs.SetInt("optSafety", 0);
				}
				PlayerPrefs.Save();
				return;
			}
			if (isAutoSet)
			{
				PlayerPrefs.SetInt("autoSafety", 1);
				this.safetyType = PlayFabAuthenticator.SafetyType.Auto;
				return;
			}
			PlayerPrefs.SetInt("optSafety", 1);
			this.safetyType = PlayFabAuthenticator.SafetyType.OptIn;
		}

		// Token: 0x06006DA6 RID: 28070 RVA: 0x00236EF7 File Offset: 0x002350F7
		public string GetPlayFabSessionTicket()
		{
			return this._sessionTicket;
		}

		// Token: 0x06006DA7 RID: 28071 RVA: 0x00236EFF File Offset: 0x002350FF
		public string GetPlayFabPlayerId()
		{
			return this._playFabPlayerIdCache;
		}

		// Token: 0x06006DA8 RID: 28072 RVA: 0x00236F07 File Offset: 0x00235107
		public bool GetSafety()
		{
			return this.isSafeAccount;
		}

		// Token: 0x06006DA9 RID: 28073 RVA: 0x00236F0F File Offset: 0x0023510F
		public PlayFabAuthenticator.SafetyType GetSafetyType()
		{
			return this.safetyType;
		}

		// Token: 0x06006DAA RID: 28074 RVA: 0x00236F17 File Offset: 0x00235117
		public string GetUserID()
		{
			return this.userID;
		}

		// Token: 0x04007DD4 RID: 32212
		public static volatile PlayFabAuthenticator instance;

		// Token: 0x04007DD5 RID: 32213
		private const int PlayFabAuthRequestTimeout = 30;

		// Token: 0x04007DD6 RID: 32214
		private string _playFabPlayerIdCache;

		// Token: 0x04007DD7 RID: 32215
		private string _sessionTicket;

		// Token: 0x04007DD8 RID: 32216
		private string _displayName;

		// Token: 0x04007DD9 RID: 32217
		private string _nonce;

		// Token: 0x04007DDA RID: 32218
		public string userID;

		// Token: 0x04007DDB RID: 32219
		private string userToken;

		// Token: 0x04007DDC RID: 32220
		public PlatformTagJoin platform;

		// Token: 0x04007DDD RID: 32221
		private bool isSafeAccount;

		// Token: 0x04007DDE RID: 32222
		public Action<bool> OnSafetyUpdate;

		// Token: 0x04007DDF RID: 32223
		private PlayFabAuthenticator.SafetyType safetyType;

		// Token: 0x04007DE0 RID: 32224
		private byte[] m_Ticket;

		// Token: 0x04007DE1 RID: 32225
		private uint m_pcbTicket;

		// Token: 0x04007DE2 RID: 32226
		public Text debugText;

		// Token: 0x04007DE3 RID: 32227
		public bool screenDebugMode;

		// Token: 0x04007DE4 RID: 32228
		public bool loginFailed;

		// Token: 0x04007DE5 RID: 32229
		[FormerlySerializedAs("loginDisplayID")]
		public GameObject emptyObject;

		// Token: 0x04007DE6 RID: 32230
		private int playFabAuthRetryCount;

		// Token: 0x04007DE7 RID: 32231
		private int playFabMaxRetries = 5;

		// Token: 0x04007DE8 RID: 32232
		private int playFabCacheRetryCount;

		// Token: 0x04007DE9 RID: 32233
		private int playFabCacheMaxRetries = 5;

		// Token: 0x04007DEA RID: 32234
		public MetaAuthenticator metaAuthenticator;

		// Token: 0x04007DEB RID: 32235
		public SteamAuthenticator steamAuthenticator;

		// Token: 0x04007DEC RID: 32236
		public MothershipAuthenticator mothershipAuthenticator;

		// Token: 0x04007DED RID: 32237
		public PhotonAuthenticator photonAuthenticator;

		// Token: 0x04007DEE RID: 32238
		[SerializeField]
		private bool dbg_isReturningPlayer;

		// Token: 0x04007DF0 RID: 32240
		private SteamAuthTicket steamAuthTicketForPlayFab;

		// Token: 0x04007DF1 RID: 32241
		private SteamAuthTicket steamAuthTicketForPhoton;

		// Token: 0x04007DF2 RID: 32242
		private string steamAuthIdForPhoton;

		// Token: 0x0200110F RID: 4367
		public enum SafetyType
		{
			// Token: 0x04007DF5 RID: 32245
			None,
			// Token: 0x04007DF6 RID: 32246
			Auto,
			// Token: 0x04007DF7 RID: 32247
			OptIn
		}

		// Token: 0x02001110 RID: 4368
		[Serializable]
		public class CachePlayFabIdRequest
		{
			// Token: 0x04007DF8 RID: 32248
			public string Platform;

			// Token: 0x04007DF9 RID: 32249
			public string SessionTicket;

			// Token: 0x04007DFA RID: 32250
			public string PlayFabId;

			// Token: 0x04007DFB RID: 32251
			public string TitleId;

			// Token: 0x04007DFC RID: 32252
			public string MothershipEnvId;

			// Token: 0x04007DFD RID: 32253
			public string MothershipDeploymentId;

			// Token: 0x04007DFE RID: 32254
			public string MothershipToken;

			// Token: 0x04007DFF RID: 32255
			public string MothershipId;
		}

		// Token: 0x02001111 RID: 4369
		[Serializable]
		public class PlayfabAuthRequestData
		{
			// Token: 0x04007E00 RID: 32256
			public string AppId;

			// Token: 0x04007E01 RID: 32257
			public string Nonce;

			// Token: 0x04007E02 RID: 32258
			public string OculusId;

			// Token: 0x04007E03 RID: 32259
			public string Platform;

			// Token: 0x04007E04 RID: 32260
			public string AgeCategory;

			// Token: 0x04007E05 RID: 32261
			public string MothershipEnvId;

			// Token: 0x04007E06 RID: 32262
			public string MothershipDeploymentId;

			// Token: 0x04007E07 RID: 32263
			public string MothershipToken;

			// Token: 0x04007E08 RID: 32264
			public string MothershipId;
		}

		// Token: 0x02001112 RID: 4370
		[Serializable]
		public class PlayfabAuthResponseData
		{
			// Token: 0x04007E09 RID: 32265
			public string SessionTicket;

			// Token: 0x04007E0A RID: 32266
			public string EntityToken;

			// Token: 0x04007E0B RID: 32267
			public string PlayFabId;

			// Token: 0x04007E0C RID: 32268
			public string EntityId;

			// Token: 0x04007E0D RID: 32269
			public string EntityType;

			// Token: 0x04007E0E RID: 32270
			public string AccountCreationIsoTimestamp;
		}

		// Token: 0x02001113 RID: 4371
		[Serializable]
		public class CachePlayFabIdResponse
		{
			// Token: 0x04007E0F RID: 32271
			public string PlayFabId;

			// Token: 0x04007E10 RID: 32272
			public string SteamAuthIdForPhoton;

			// Token: 0x04007E11 RID: 32273
			public string AccountCreationIsoTimestamp;
		}

		// Token: 0x02001114 RID: 4372
		private class ErrorInfo
		{
			// Token: 0x04007E12 RID: 32274
			public string Message;

			// Token: 0x04007E13 RID: 32275
			public string Error;
		}

		// Token: 0x02001115 RID: 4373
		private class BanInfo
		{
			// Token: 0x04007E14 RID: 32276
			public string BanMessage;

			// Token: 0x04007E15 RID: 32277
			public string BanExpirationTime;
		}
	}
}
