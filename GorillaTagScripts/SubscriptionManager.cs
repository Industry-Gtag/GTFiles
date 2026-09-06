using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;

namespace GorillaTagScripts
{
	// Token: 0x02000FAD RID: 4013
	public class SubscriptionManager : MonoBehaviour
	{
		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x060063E9 RID: 25577 RVA: 0x00201E32 File Offset: 0x00200032
		public static bool LocalSubscriptionDataInitialized
		{
			get
			{
				return SubscriptionManager._localSubscriptionDataInitialized;
			}
		}

		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x060063EA RID: 25578 RVA: 0x00201E39 File Offset: 0x00200039
		public static bool LocalSubscriptionDataResolved
		{
			get
			{
				return SubscriptionManager._localSubscriptionDataResolved;
			}
		}

		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x060063EB RID: 25579 RVA: 0x00201E40 File Offset: 0x00200040
		// (set) Token: 0x060063EC RID: 25580 RVA: 0x00201E4F File Offset: 0x0020004F
		public static bool SubsOnlyMatchmaking
		{
			get
			{
				return PlayerPrefs.GetInt("subsOnlyMatchmaking") == 1;
			}
			set
			{
				PlayerPrefs.SetInt("subsOnlyMatchmaking", value ? 1 : 0);
				PlayerPrefs.Save();
			}
		}

		// Token: 0x060063ED RID: 25581 RVA: 0x00201E67 File Offset: 0x00200067
		public static string GetSubsFeatureKey(SubscriptionManager.SubscriptionFeatures feature)
		{
			return SubscriptionManager.SUBS_KEYS[(int)feature];
		}

		// Token: 0x060063EE RID: 25582 RVA: 0x00201E70 File Offset: 0x00200070
		private async void Awake()
		{
			if (SubscriptionManager.Instance == null)
			{
				SubscriptionManager.Instance = this;
				SubscriptionManager.SUBS_KEYS = new string[3];
				for (int i = 0; i < SubscriptionManager.SUBS_KEYS.Length; i++)
				{
					SubscriptionManager.SubscriptionFeatures subscriptionFeatures = (SubscriptionManager.SubscriptionFeatures)i;
					string text = string.Empty;
					if (subscriptionFeatures != SubscriptionManager.SubscriptionFeatures.GoldenName)
					{
						if (subscriptionFeatures != SubscriptionManager.SubscriptionFeatures.IOBT)
						{
							text = subscriptionFeatures.ToString().ToUpper() + "_ENABLE_KEY";
						}
						else
						{
							text = "IOBT_ENABLE_KEY";
						}
					}
					else
					{
						text = "GOLDEN_NAME_KEY";
					}
					SubscriptionManager.SUBS_KEYS[i] = "SMKEYPREFIX" + text;
				}
				while (NetworkSystem.Instance == null && NetworkSystem.Instance.AllNetPlayers.Length == 0)
				{
					await Awaitable.WaitForSecondsAsync(0.1f, default(CancellationToken));
				}
				SubscriptionManager.DEFAULT_SEND_RATE = PhotonNetwork.SendRate;
				this.UpdatePlayerSubsDetails(NetworkSystem.Instance.LocalPlayer, null, null);
				SubscriptionManager.ForceRecheck();
			}
			else
			{
				Debug.LogError("Failed attempt to instantiate a second SubscriptionManager. Don't do that!");
				Object.DestroyImmediate(base.gameObject);
			}
		}

		// Token: 0x060063EF RID: 25583 RVA: 0x00201EA7 File Offset: 0x002000A7
		protected void OnEnable()
		{
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoinedRoom);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeft);
			SubscriptionManager.InitializePersonalSubscriptionData();
		}

		// Token: 0x060063F0 RID: 25584 RVA: 0x00201EE4 File Offset: 0x002000E4
		public static async void InitializePersonalSubscriptionData()
		{
			while (!KIDManager.InitialisationComplete)
			{
				await Awaitable.WaitForSecondsAsync(0.5f, default(CancellationToken));
			}
			while (!MothershipClientApiUnity.IsClientLoggedIn())
			{
				await Awaitable.WaitForSecondsAsync(0.5f, default(CancellationToken));
			}
			SubscriptionManager.GetMySubscriptionsAndTheirBenefitsRequest requestBody = new SubscriptionManager.GetMySubscriptionsAndTheirBenefitsRequest
			{
				Refresh = true,
				MothershipId = MothershipClientContext.MothershipId,
				MothershipToken = MothershipClientContext.Token,
				MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				MothershipDeploymentId = MothershipClientApiUnity.DeploymentId
			};
			int retryCount = 0;
			for (;;)
			{
				using (UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.IapApiBaseUrl + "/api/GetMySubscriptionsAndTheirBenefits", "POST"))
				{
					string text = JsonConvert.SerializeObject(requestBody);
					request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(text));
					request.downloadHandler = new DownloadHandlerBuffer();
					request.SetRequestHeader("Content-Type", "application/json");
					request.timeout = 15;
					await request.SendWebRequest();
					if (request.result == UnityWebRequest.Result.Success)
					{
						SubscriptionManager.GetMySubscriptionsAndTheirBenefitsResponse getMySubscriptionsAndTheirBenefitsResponse;
						try
						{
							getMySubscriptionsAndTheirBenefitsResponse = JsonConvert.DeserializeObject<SubscriptionManager.GetMySubscriptionsAndTheirBenefitsResponse>(request.downloadHandler.text);
						}
						catch (Exception)
						{
							Debug.LogError("[SubscriptionResults] Error deserializing subscription data: " + request.downloadHandler.text);
							SubscriptionManager.<InitializePersonalSubscriptionData>g__MarkResolved|37_1();
							break;
						}
						if (getMySubscriptionsAndTheirBenefitsResponse != null && getMySubscriptionsAndTheirBenefitsResponse.Subscriptions != null)
						{
							for (int i = 0; i < getMySubscriptionsAndTheirBenefitsResponse.Subscriptions.Count; i++)
							{
								if (!(getMySubscriptionsAndTheirBenefitsResponse.Subscriptions[i].Sku != "fan_club"))
								{
									SubscriptionManager.GorillaTagSubscription gorillaTagSubscription = getMySubscriptionsAndTheirBenefitsResponse.Subscriptions[i];
									int num = gorillaTagSubscription.TotalLifetimeSeconds / 86400;
									DateTime localDateTime = gorillaTagSubscription.MostRecentBillingCycleStartDate.LocalDateTime;
									DateTime localDateTime2 = gorillaTagSubscription.MostRecentBillingCycleEndDate.LocalDateTime;
									int num2 = Mathf.RoundToInt((float)(localDateTime2 - localDateTime).Days / 30f);
									SubscriptionManager.localSubscriptionDetails = new SubscriptionManager.SubscriptionDetails
									{
										active = (DateTimeOffset.UtcNow < (gorillaTagSubscription.ExpirationTime ?? gorillaTagSubscription.MostRecentBillingCycleEndDate)),
										daysAccrued = num,
										tier = 1,
										autoRenew = !gorillaTagSubscription.IsCancelling,
										autoRenewMonths = num2,
										subscriptionActiveUntilDate = localDateTime2
									};
									SubscriptionManager.Instance.subData[NetworkSystem.Instance.LocalPlayer] = SubscriptionManager.localSubscriptionDetails;
									SubscriptionManager.<InitializePersonalSubscriptionData>g__MarkInitialized|37_0();
									return;
								}
							}
						}
						SubscriptionManager.localSubscriptionDetails = default(SubscriptionManager.SubscriptionDetails);
						SubscriptionManager.Instance.subData[NetworkSystem.Instance.LocalPlayer] = SubscriptionManager.localSubscriptionDetails;
						SubscriptionManager.<InitializePersonalSubscriptionData>g__MarkInitialized|37_0();
					}
					else
					{
						Debug.LogError(string.Format("[SubscriptionResults] Error fetching subscription data: {0} (Status: {1})", request.downloadHandler.text, request.responseCode));
						bool flag = request.result != UnityWebRequest.Result.ProtocolError;
						if (!flag)
						{
							long responseCode = request.responseCode;
							if (responseCode >= 500L)
							{
								if (responseCode >= 600L)
								{
									goto IL_048B;
								}
							}
							else if (responseCode != 408L && responseCode != 429L)
							{
								goto IL_048B;
							}
							bool flag2 = true;
							goto IL_048E;
							IL_048B:
							flag2 = false;
							IL_048E:
							flag = flag2;
						}
						if (flag)
						{
							if (retryCount < SubscriptionManager.maxRetries)
							{
								await Awaitable.WaitForSecondsAsync(Random.Range(0.5f, Mathf.Pow(2f, (float)(++retryCount))), default(CancellationToken));
								continue;
							}
							Debug.LogError("[SubscriptionResults] Maximum retries attempted");
						}
						SubscriptionManager.<InitializePersonalSubscriptionData>g__MarkResolved|37_1();
					}
				}
				break;
			}
		}

		// Token: 0x060063F1 RID: 25585 RVA: 0x00201F13 File Offset: 0x00200113
		protected void OnDisable()
		{
			RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoinedRoom);
			RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeft);
		}

		// Token: 0x060063F2 RID: 25586 RVA: 0x00201F4C File Offset: 0x0020014C
		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails(VRRig rig)
		{
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.rigs.ContainsKey(rig))
			{
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return SubscriptionManager.GetSubscriptionDetails(SubscriptionManager.Instance.rigs[rig]);
		}

		// Token: 0x060063F3 RID: 25587 RVA: 0x00201F98 File Offset: 0x00200198
		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails(NetPlayer np)
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(np, out subscriptionDetails))
			{
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return subscriptionDetails;
		}

		// Token: 0x060063F4 RID: 25588 RVA: 0x00201FD1 File Offset: 0x002001D1
		public static bool IsPlayerSubscribed(VRRig rig)
		{
			return SubscriptionManager.GetSubscriptionDetails(rig).active;
		}

		// Token: 0x060063F5 RID: 25589 RVA: 0x00201FDE File Offset: 0x002001DE
		public static bool IsPlayerSubscribed(NetPlayer np)
		{
			return SubscriptionManager.GetSubscriptionDetails(np).active;
		}

		// Token: 0x060063F6 RID: 25590 RVA: 0x00201FEC File Offset: 0x002001EC
		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out subscriptionDetails))
			{
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return subscriptionDetails;
		}

		// Token: 0x060063F7 RID: 25591 RVA: 0x00202030 File Offset: 0x00200230
		public static SubscriptionManager.SubscriptionStatus LocalSubscriptionStatus()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out subscriptionDetails))
			{
				return SubscriptionManager.SubscriptionStatus.Unknown;
			}
			if (!subscriptionDetails.active)
			{
				return SubscriptionManager.SubscriptionStatus.Inactive;
			}
			return SubscriptionManager.SubscriptionStatus.Active;
		}

		// Token: 0x060063F8 RID: 25592 RVA: 0x00202074 File Offset: 0x00200274
		public static SubscriptionManager.SubscriptionDetails LocalSubscriptionDetails()
		{
			return SubscriptionManager.localSubscriptionDetails;
		}

		// Token: 0x060063F9 RID: 25593 RVA: 0x0020207C File Offset: 0x0020027C
		public static bool IsLocalSubscribed()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails;
			return !(SubscriptionManager.Instance == null) && !(VRRig.LocalRig == null) && VRRig.LocalRig.creator != null && SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out subscriptionDetails) && subscriptionDetails.active;
		}

		// Token: 0x060063FA RID: 25594 RVA: 0x002020D4 File Offset: 0x002002D4
		public static void ForceRecheck()
		{
			SubscriptionManager.Instance.OnPlayerJoinedRoom(null);
		}

		// Token: 0x060063FB RID: 25595 RVA: 0x002020E1 File Offset: 0x002002E1
		private void OnPlayerJoinedRoom(NetPlayer npl)
		{
			if (SubscriptionManager.OnSubscriptionData != null)
			{
				SubscriptionManager.OnSubscriptionData();
			}
			if (NetworkSystem.Instance.AllNetPlayers.Length > SubscriptionManager.PERF_CHANGE_ROOMSIZE)
			{
				GorillaTagger.Instance.ToggleForcedPerformanceRefresh();
				PhotonNetwork.SendRate = 20;
			}
		}

		// Token: 0x060063FC RID: 25596 RVA: 0x00202118 File Offset: 0x00200318
		private void UpdatePlayerSubsDetails(NetPlayer player, bool? isSubscribed = null, int? daysAccrued = null)
		{
			if (player == null)
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(player.ActorNumber, out rigContainer))
			{
				this.rigs[rigContainer.Rig] = player;
			}
			if (player != NetworkSystem.Instance.LocalPlayer)
			{
				bool flag = player == VRRig.LocalRig.creator;
			}
			bool flag2 = false;
			int num = 0;
			int num2 = 0;
			if (isSubscribed != null)
			{
				flag2 = isSubscribed.Value;
				num2 = (flag2 ? 1 : 0);
				num = daysAccrued.GetValueOrDefault();
			}
			SubscriptionManager.SubscriptionDetails subscriptionDetails = new SubscriptionManager.SubscriptionDetails
			{
				active = flag2,
				tier = num2,
				daysAccrued = num
			};
			this.subData[player] = subscriptionDetails;
		}

		// Token: 0x060063FD RID: 25597 RVA: 0x002021C8 File Offset: 0x002003C8
		private void OnPlayerLeft(NetPlayer pl)
		{
			if (this.subData.ContainsKey(pl))
			{
				this.subData.Remove(pl);
			}
			NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
			if (allNetPlayers.Length <= SubscriptionManager.PERF_CHANGE_ROOMSIZE)
			{
				GorillaTagger.Instance.ToggleDefaultPerformanceRefresh();
				PhotonNetwork.SendRate = SubscriptionManager.DEFAULT_SEND_RATE;
			}
			NetPlayer lowestNetPlayer = this.GetLowestNetPlayer(allNetPlayers);
			if (lowestNetPlayer != null && lowestNetPlayer == NetworkSystem.Instance.LocalPlayer)
			{
				byte currentRoomExpectedSize = RoomSystem.GetCurrentRoomExpectedSize();
				PhotonNetwork.CurrentRoom.MaxPlayers = currentRoomExpectedSize;
			}
		}

		// Token: 0x060063FE RID: 25598 RVA: 0x00202244 File Offset: 0x00200444
		private NetPlayer GetLowestNetPlayer(NetPlayer[] players)
		{
			NetPlayer netPlayer = null;
			int num = int.MaxValue;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].ActorNumber < num)
				{
					num = players[i].ActorNumber;
					netPlayer = players[i];
				}
			}
			return netPlayer;
		}

		// Token: 0x060063FF RID: 25599 RVA: 0x00202280 File Offset: 0x00200480
		private void OnGetViewerPurchasesStartup(Message msg)
		{
			if (msg.IsError)
			{
				if (this.attempts < 3)
				{
					this.attempts++;
					IAP.GetViewerPurchases().OnComplete(new Message<PurchaseList>.Callback(this.OnGetViewerPurchasesStartup));
				}
				return;
			}
			if (msg.GetPurchaseList() == null)
			{
				return;
			}
			if (SubscriptionManager._localSubscriptionDataInitialized)
			{
				return;
			}
			bool flag = false;
			foreach (Purchase purchase in msg.GetPurchaseList())
			{
				if (purchase.Type == ProductType.SUBSCRIPTION && purchase.Sku.Contains("fan_club"))
				{
					flag = true;
					SubscriptionManager.localSubscriptionDetails = new SubscriptionManager.SubscriptionDetails
					{
						active = (DateTime.Now < purchase.ExpirationTime),
						subscriptionActiveUntilDate = purchase.ExpirationTime
					};
				}
			}
			if (!flag)
			{
				SubscriptionManager.localSubscriptionDetails = new SubscriptionManager.SubscriptionDetails
				{
					active = false
				};
			}
		}

		// Token: 0x06006400 RID: 25600 RVA: 0x00202378 File Offset: 0x00200578
		public static void SetSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures feature, int settingValue)
		{
			string subsFeatureKey = SubscriptionManager.GetSubsFeatureKey(feature);
			PlayerPrefs.SetInt(subsFeatureKey, settingValue);
			SubscriptionManager.subSettings[subsFeatureKey] = settingValue;
			PlayerPrefs.Save();
		}

		// Token: 0x06006401 RID: 25601 RVA: 0x002023A4 File Offset: 0x002005A4
		public static int GetSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures feature)
		{
			string subsFeatureKey = SubscriptionManager.GetSubsFeatureKey(feature);
			int num;
			if (SubscriptionManager.subSettings.TryGetValue(subsFeatureKey, out num))
			{
				return num;
			}
			SubscriptionManager.subSettings[subsFeatureKey] = PlayerPrefs.GetInt(subsFeatureKey, 1);
			return SubscriptionManager.subSettings[subsFeatureKey];
		}

		// Token: 0x06006402 RID: 25602 RVA: 0x002023E6 File Offset: 0x002005E6
		public static bool GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures feature)
		{
			return SubscriptionManager.GetSubscriptionSettingValue(feature) >= 1;
		}

		// Token: 0x06006403 RID: 25603 RVA: 0x002023F4 File Offset: 0x002005F4
		public static bool IsSubscriptionFeatureAvailable(SubscriptionManager.SubscriptionFeatures feature)
		{
			if (feature != SubscriptionManager.SubscriptionFeatures.IOBT)
			{
				return feature != SubscriptionManager.SubscriptionFeatures.HandTracking || global::UnityEngine.Application.platform == RuntimePlatform.Android;
			}
			if (global::UnityEngine.Application.platform != RuntimePlatform.Android)
			{
				return false;
			}
			OVRPlugin.SystemHeadset systemHeadsetType = OVRPlugin.GetSystemHeadsetType();
			return systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Quest_3 || systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Quest_3S || systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Link_Quest_3 || systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Link_Quest_3S;
		}

		// Token: 0x06006404 RID: 25604 RVA: 0x00202447 File Offset: 0x00200647
		public static bool CheckSubscriptionFeaturePermission(SubscriptionManager.SubscriptionFeatures feature)
		{
			if (feature != SubscriptionManager.SubscriptionFeatures.IOBT)
			{
				return feature != SubscriptionManager.SubscriptionFeatures.HandTracking || OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.BodyTracking);
			}
			return OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.BodyTracking);
		}

		// Token: 0x06006405 RID: 25605 RVA: 0x00002C2D File Offset: 0x00000E2D
		[RuntimeInitializeOnLoadMethod]
		private static void OnLoad()
		{
		}

		// Token: 0x06006406 RID: 25606 RVA: 0x00202464 File Offset: 0x00200664
		public static void UpdatePlayerSubscriptionData(NetPlayer player, bool isSubscribed, int daysAccrued = 0)
		{
			if (SubscriptionManager.Instance == null)
			{
				Debug.LogWarning("SubscriptionManager: Instance is null, cannot update player subscription data");
				return;
			}
			if (player == null)
			{
				Debug.LogWarning("SubscriptionManager: NetPlayer is null, cannot update subscription data");
				return;
			}
			SubscriptionManager.Instance.UpdatePlayerSubsDetails(player, new bool?(isSubscribed), new int?(daysAccrued));
			if (SubscriptionManager.OnSubscriptionData != null)
			{
				SubscriptionManager.OnSubscriptionData();
			}
		}

		// Token: 0x06006409 RID: 25609 RVA: 0x00202507 File Offset: 0x00200707
		[CompilerGenerated]
		internal static void <InitializePersonalSubscriptionData>g__MarkInitialized|37_0()
		{
			SubscriptionManager._localSubscriptionDataInitialized = true;
			Action onLocalSubscriptionData = SubscriptionManager.OnLocalSubscriptionData;
			if (onLocalSubscriptionData != null)
			{
				onLocalSubscriptionData();
			}
			SubscriptionManager.<InitializePersonalSubscriptionData>g__MarkResolved|37_1();
		}

		// Token: 0x0600640A RID: 25610 RVA: 0x00202524 File Offset: 0x00200724
		[CompilerGenerated]
		internal static void <InitializePersonalSubscriptionData>g__MarkResolved|37_1()
		{
			SubscriptionManager._localSubscriptionDataResolved = true;
			Action onLocalSubscriptionDataResolved = SubscriptionManager.OnLocalSubscriptionDataResolved;
			if (onLocalSubscriptionDataResolved == null)
			{
				return;
			}
			onLocalSubscriptionDataResolved();
		}

		// Token: 0x04007296 RID: 29334
		public const string FAN_CLUB_BASE_SKU = "fan_club";

		// Token: 0x04007297 RID: 29335
		public const string FAN_CLUB_STEAM_SKU = "40494";

		// Token: 0x04007298 RID: 29336
		public const string SUBSCRIBER_NAME_COLOR_HEX = "#ffc600";

		// Token: 0x04007299 RID: 29337
		public static Color SUBSCRIBER_NAME_COLOR = Color.gold;

		// Token: 0x0400729A RID: 29338
		public const int PERF_SEND_RATE = 20;

		// Token: 0x0400729B RID: 29339
		public static int DEFAULT_SEND_RATE = 30;

		// Token: 0x0400729C RID: 29340
		public static int PERF_CHANGE_ROOMSIZE = 10;

		// Token: 0x0400729D RID: 29341
		private static SubscriptionManager Instance;

		// Token: 0x0400729E RID: 29342
		public static Action OnSubscriptionData;

		// Token: 0x0400729F RID: 29343
		public static Action OnLocalSubscriptionData;

		// Token: 0x040072A0 RID: 29344
		public static Action OnLocalSubscriptionDataResolved;

		// Token: 0x040072A1 RID: 29345
		private Dictionary<NetPlayer, SubscriptionManager.SubscriptionDetails> subData = new Dictionary<NetPlayer, SubscriptionManager.SubscriptionDetails>();

		// Token: 0x040072A2 RID: 29346
		private Dictionary<VRRig, NetPlayer> rigs = new Dictionary<VRRig, NetPlayer>();

		// Token: 0x040072A3 RID: 29347
		private static SubscriptionManager.SubscriptionDetails localSubscriptionDetails;

		// Token: 0x040072A4 RID: 29348
		private static bool _localSubscriptionDataInitialized;

		// Token: 0x040072A5 RID: 29349
		private static bool _localSubscriptionDataResolved;

		// Token: 0x040072A6 RID: 29350
		public const string SUB_PREFIX = "SMKEYPREFIX";

		// Token: 0x040072A7 RID: 29351
		public static string[] SUBS_KEYS;

		// Token: 0x040072A8 RID: 29352
		private static int maxRetries = 3;

		// Token: 0x040072A9 RID: 29353
		private int attempts;

		// Token: 0x040072AA RID: 29354
		private static Dictionary<string, int> subSettings = new Dictionary<string, int>();

		// Token: 0x02000FAE RID: 4014
		public enum SubscriptionStatus
		{
			// Token: 0x040072AC RID: 29356
			Active,
			// Token: 0x040072AD RID: 29357
			Inactive,
			// Token: 0x040072AE RID: 29358
			Unknown
		}

		// Token: 0x02000FAF RID: 4015
		public enum SubscriptionTerm
		{
			// Token: 0x040072B0 RID: 29360
			MONTHLY,
			// Token: 0x040072B1 RID: 29361
			QUARTERLY,
			// Token: 0x040072B2 RID: 29362
			SEMIANNUAL,
			// Token: 0x040072B3 RID: 29363
			ANNUAL
		}

		// Token: 0x02000FB0 RID: 4016
		public enum SubscriptionFeatures
		{
			// Token: 0x040072B5 RID: 29365
			GoldenName,
			// Token: 0x040072B6 RID: 29366
			IOBT,
			// Token: 0x040072B7 RID: 29367
			HandTracking,
			// Token: 0x040072B8 RID: 29368
			SubscriptionFeatureCount
		}

		// Token: 0x02000FB1 RID: 4017
		public struct SubscriptionDetails
		{
			// Token: 0x040072B9 RID: 29369
			public bool active;

			// Token: 0x040072BA RID: 29370
			public int daysAccrued;

			// Token: 0x040072BB RID: 29371
			public bool[] subscriptionFeatureSettings;

			// Token: 0x040072BC RID: 29372
			public int tier;

			// Token: 0x040072BD RID: 29373
			public DateTime subscriptionActiveUntilDate;

			// Token: 0x040072BE RID: 29374
			public bool autoRenew;

			// Token: 0x040072BF RID: 29375
			public int autoRenewMonths;
		}

		// Token: 0x02000FB2 RID: 4018
		[Serializable]
		private class GorillaTagSubscription
		{
			// Token: 0x040072C0 RID: 29376
			public string SubscriptionId;

			// Token: 0x040072C1 RID: 29377
			public DateTimeOffset EarliestStartDate;

			// Token: 0x040072C2 RID: 29378
			public DateTimeOffset CurrentStartDate;

			// Token: 0x040072C3 RID: 29379
			public DateTimeOffset MostRecentBillingCycleStartDate;

			// Token: 0x040072C4 RID: 29380
			public DateTimeOffset MostRecentBillingCycleEndDate;

			// Token: 0x040072C5 RID: 29381
			public DateTimeOffset? ExpirationTime;

			// Token: 0x040072C6 RID: 29382
			public int TotalLifetimeSeconds;

			// Token: 0x040072C7 RID: 29383
			public bool IsActive;

			// Token: 0x040072C8 RID: 29384
			public bool IsCancelling;

			// Token: 0x040072C9 RID: 29385
			public string Sku;

			// Token: 0x040072CA RID: 29386
			public string PlayerId;

			// Token: 0x040072CB RID: 29387
			public string TrialType;

			// Token: 0x040072CC RID: 29388
			public string ExternalServiceName;

			// Token: 0x040072CD RID: 29389
			public string ExternalSubscriptionId;

			// Token: 0x040072CE RID: 29390
			public string SubscriptionCatalogItemId;
		}

		// Token: 0x02000FB3 RID: 4019
		[Serializable]
		private class GrantedSubscriptionBenefit
		{
			// Token: 0x040072CF RID: 29391
			public string BenefitId;

			// Token: 0x040072D0 RID: 29392
			public DateTimeOffset GrantedTime;

			// Token: 0x040072D1 RID: 29393
			public string PlayFabItemId;
		}

		// Token: 0x02000FB4 RID: 4020
		[Serializable]
		private class GetMySubscriptionsAndTheirBenefitsRequest
		{
			// Token: 0x040072D2 RID: 29394
			public bool Refresh;

			// Token: 0x040072D3 RID: 29395
			public bool? SkipBenefitsCheck;

			// Token: 0x040072D4 RID: 29396
			public bool? SkipSharedGroupDataUpdate;

			// Token: 0x040072D5 RID: 29397
			public string MothershipId;

			// Token: 0x040072D6 RID: 29398
			public string MothershipToken;

			// Token: 0x040072D7 RID: 29399
			public string MothershipEnvId;

			// Token: 0x040072D8 RID: 29400
			public string MothershipDeploymentId;
		}

		// Token: 0x02000FB5 RID: 4021
		[Serializable]
		private class GetMySubscriptionsAndTheirBenefitsResponse
		{
			// Token: 0x040072D9 RID: 29401
			public List<SubscriptionManager.GorillaTagSubscription> Subscriptions;

			// Token: 0x040072DA RID: 29402
			public Dictionary<string, List<SubscriptionManager.GrantedSubscriptionBenefit>> PreviouslyGrantedBenefitsBySubscriptionSku;

			// Token: 0x040072DB RID: 29403
			public Dictionary<string, List<SubscriptionManager.GrantedSubscriptionBenefit>> NewlyGrantedBenefitsBySubscriptionSku;

			// Token: 0x040072DC RID: 29404
			public bool? SharedGroupDataUpdateSucceeded;
		}
	}
}
