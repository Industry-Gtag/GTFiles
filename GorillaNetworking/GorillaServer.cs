using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010FE RID: 4350
	public class GorillaServer : MonoBehaviour, ISerializationCallbackReceiver
	{
		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x06006CF7 RID: 27895 RVA: 0x0023378D File Offset: 0x0023198D
		public bool FeatureFlagsReady
		{
			get
			{
				return this.featureFlags.ready;
			}
		}

		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x06006CF8 RID: 27896 RVA: 0x0023379A File Offset: 0x0023199A
		private global::PlayFab.CloudScriptModels.EntityKey playerEntity
		{
			get
			{
				return new global::PlayFab.CloudScriptModels.EntityKey
				{
					Id = PlayFabSettings.staticPlayer.EntityId,
					Type = PlayFabSettings.staticPlayer.EntityType
				};
			}
		}

		// Token: 0x06006CF9 RID: 27897 RVA: 0x002337C1 File Offset: 0x002319C1
		public void Start()
		{
			this.featureFlags.FetchFeatureFlags();
		}

		// Token: 0x06006CFA RID: 27898 RVA: 0x002337CE File Offset: 0x002319CE
		private void Awake()
		{
			if (GorillaServer.Instance == null)
			{
				GorillaServer.Instance = this;
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06006CFB RID: 27899 RVA: 0x002337F0 File Offset: 0x002319F0
		public void ReturnCurrentVersion(ReturnCurrentVersionRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnCurrentVersion result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnCurrentVersion error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnCurrentVersionV2",
				FunctionParameter = request
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006CFC RID: 27900 RVA: 0x00233848 File Offset: 0x00231A48
		public void TryDistributeCurrency(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "TryDistributeCurrency result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "TryDistributeCurrency error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "TryDistributeCurrencyV2",
				FunctionParameter = new { }
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006CFD RID: 27901 RVA: 0x002338A1 File Offset: 0x00231AA1
		public void ReconcileBundleRewards(Action<string> successCallback, Action<string> errorCallback)
		{
			successCallback = this.DebugWrapCb<string>(successCallback, "ReconcileBundleRewards result");
			errorCallback = this.DebugWrapCb<string>(errorCallback, "ReconcileBundleRewards error");
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				errorCallback("Not logged in to Mothership");
				return;
			}
			base.StartCoroutine(this.SendReconcileBundleRewards(successCallback, errorCallback));
		}

		// Token: 0x06006CFE RID: 27902 RVA: 0x002338E1 File Offset: 0x00231AE1
		private IEnumerator SendReconcileBundleRewards(Action<string> successCallback, Action<string> errorCallback)
		{
			GorillaServer.<SendReconcileBundleRewards>d__15 <SendReconcileBundleRewards>d__ = new GorillaServer.<SendReconcileBundleRewards>d__15(0);
			<SendReconcileBundleRewards>d__.successCallback = successCallback;
			<SendReconcileBundleRewards>d__.errorCallback = errorCallback;
			return <SendReconcileBundleRewards>d__;
		}

		// Token: 0x06006CFF RID: 27903 RVA: 0x002338F8 File Offset: 0x00231AF8
		public void AddOrRemoveDLCOwnership(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "AddOrRemoveDLCOwnership result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "AddOrRemoveDLCOwnership error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "AddOrRemoveDLCOwnershipV2",
				FunctionParameter = new { }
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006D00 RID: 27904 RVA: 0x00233954 File Offset: 0x00231B54
		public void BroadcastMyRoom(BroadcastMyRoomRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "BroadcastMyRoom result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "BroadcastMyRoom error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "BroadcastMyRoomV2",
				FunctionParameter = request
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006D01 RID: 27905 RVA: 0x002339AC File Offset: 0x00231BAC
		public void UpdateUserCosmetics()
		{
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = this.playerEntity;
			executeFunctionRequest.FunctionName = "UpdatePersonalCosmeticsList";
			executeFunctionRequest.FunctionParameter = new { };
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
				if (CosmeticsController.instance != null)
				{
					CosmeticsController.instance.CheckCosmeticsSharedGroup();
				}
			}, delegate(PlayFabError error)
			{
			}, null, null);
		}

		// Token: 0x06006D02 RID: 27906 RVA: 0x00233A34 File Offset: 0x00231C34
		public void GetAcceptedAgreements(GetAcceptedAgreementsRequest request, Action<Dictionary<string, string>> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<Dictionary<string, string>>(successCallback, "GetAcceptedAgreements result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetAcceptedAgreements json error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetAcceptedAgreements",
				FunctionParameter = string.Join(",", request.AgreementKeys),
				GeneratePlayStreamEvent = new bool?(false)
			}, delegate(ExecuteFunctionResult result)
			{
				try
				{
					string text = Convert.ToString(result.FunctionResult);
					successCallback(JsonConvert.DeserializeObject<Dictionary<string, string>>(text));
				}
				catch (Exception ex)
				{
					errorCallback(new PlayFabError
					{
						ErrorMessage = string.Format("Invalid format for GetAcceptedAgreements ({0})", ex),
						Error = PlayFabErrorCode.JsonParseError
					});
				}
			}, errorCallback, null, null);
		}

		// Token: 0x06006D03 RID: 27907 RVA: 0x00233ADC File Offset: 0x00231CDC
		public void SubmitAcceptedAgreements(SubmitAcceptedAgreementsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "SubmitAcceptedAgreements result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "SubmitAcceptedAgreements error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "SubmitAcceptedAgreements",
				FunctionParameter = request.Agreements,
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006D04 RID: 27908 RVA: 0x00233B44 File Offset: 0x00231D44
		public void UploadGorillanalytics(object uploadData)
		{
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = this.playerEntity;
			executeFunctionRequest.FunctionName = "Gorillanalytics";
			executeFunctionRequest.FunctionParameter = uploadData;
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
			}, delegate(PlayFabError error)
			{
			}, null, null);
		}

		// Token: 0x06006D05 RID: 27909 RVA: 0x00233BC8 File Offset: 0x00231DC8
		public void CheckForBadName(CheckForBadNameRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "CheckForBadName result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "CheckForBadName error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "CheckForBadName",
				FunctionParameter = new
				{
					name = request.name,
					forRoom = request.forRoom.ToString(),
					forTroop = request.forTroop.ToString()
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006D06 RID: 27910 RVA: 0x00233C4C File Offset: 0x00231E4C
		public void GetRandomName(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "GetRandomName result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetRandomName error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetRandomName",
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006D07 RID: 27911 RVA: 0x00233CA8 File Offset: 0x00231EA8
		public void ReturnQueueStats(ReturnQueueStatsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnQueueStats result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnQueueStats error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnQueueStats",
				FunctionParameter = new
				{
					QueueName = request.queueName
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006D08 RID: 27912 RVA: 0x00233D14 File Offset: 0x00231F14
		public void ReturnVstumpMapStats(ReturnVstumpMapStatsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnVstumpMapStats result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnVstumpMapStats error");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnVstumpMapStats",
				FunctionParameter = new
				{
					MapIds = request.mapIds
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006D09 RID: 27913 RVA: 0x00233D7F File Offset: 0x00231F7F
		private Action<T> DebugWrapCb<T>(Action<T> cb, string label)
		{
			return delegate(T arg)
			{
				bool flag = this.debug;
				cb(arg);
			};
		}

		// Token: 0x06006D0A RID: 27914 RVA: 0x00233DA0 File Offset: 0x00231FA0
		private ExecuteFunctionResult toFunctionResult(global::PlayFab.ClientModels.ExecuteCloudScriptResult csResult)
		{
			FunctionExecutionError functionExecutionError = null;
			if (csResult.Error != null)
			{
				functionExecutionError = new FunctionExecutionError
				{
					Error = csResult.Error.Error,
					Message = csResult.Error.Message,
					StackTrace = csResult.Error.StackTrace
				};
			}
			return new ExecuteFunctionResult
			{
				CustomData = csResult.CustomData,
				Error = functionExecutionError,
				ExecutionTimeMilliseconds = Convert.ToInt32(Math.Round(csResult.ExecutionTimeSeconds * 1000.0)),
				FunctionName = csResult.FunctionName,
				FunctionResult = csResult.FunctionResult,
				FunctionResultTooLarge = csResult.FunctionResultTooLarge
			};
		}

		// Token: 0x06006D0B RID: 27915 RVA: 0x00233E4C File Offset: 0x0023204C
		public void OnBeforeSerialize()
		{
			this.FeatureFlagsTitleDataKey = this.featureFlags.TitleDataKey;
			this.DefaultDeployFeatureFlagsEnabled.Clear();
			foreach (KeyValuePair<string, bool> keyValuePair in this.featureFlags.defaults)
			{
				if (keyValuePair.Value)
				{
					this.DefaultDeployFeatureFlagsEnabled.Add(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06006D0C RID: 27916 RVA: 0x00233ED4 File Offset: 0x002320D4
		public void OnAfterDeserialize()
		{
			this.featureFlags.TitleDataKey = this.FeatureFlagsTitleDataKey;
			foreach (string text in this.DefaultDeployFeatureFlagsEnabled)
			{
				this.featureFlags.defaults.AddOrUpdate(text, true);
			}
		}

		// Token: 0x06006D0D RID: 27917 RVA: 0x00233F44 File Offset: 0x00232144
		public bool CheckIsInKIDOptInCohort()
		{
			return this.featureFlags.IsEnabled("2025-04-KIDOptIn");
		}

		// Token: 0x06006D0E RID: 27918 RVA: 0x00233F56 File Offset: 0x00232156
		public bool CheckIsInKIDRequiredCohort()
		{
			return this.featureFlags.IsEnabled("2025-04-KIDRequired");
		}

		// Token: 0x06006D0F RID: 27919 RVA: 0x00233F68 File Offset: 0x00232168
		public bool CheckOptedInKID()
		{
			return KIDManager.HasOptedInToKID;
		}

		// Token: 0x06006D10 RID: 27920 RVA: 0x00233F6F File Offset: 0x0023216F
		public bool CheckIsTZE_Enabled()
		{
			return this.featureFlags.IsEnabled("2025-10-TelemetryZoneEventSampling");
		}

		// Token: 0x06006D11 RID: 27921 RVA: 0x00233F81 File Offset: 0x00232181
		public bool CheckIsMothershipTelemetryEnabled()
		{
			return this.featureFlags.IsEnabled("2025-09-MothershipAnalyticsSampleRate");
		}

		// Token: 0x06006D12 RID: 27922 RVA: 0x00233F94 File Offset: 0x00232194
		public bool CheckIsVStumpGrabbablesFixEnabled()
		{
			if (this.cachedVStumpGrabbablesFix.Item1)
			{
				return this.cachedVStumpGrabbablesFix.Item2;
			}
			bool flag = this.featureFlags.IsEnabled("2026-04-VStumpGrabbablesFix");
			if (this.featureFlags.ready)
			{
				this.cachedVStumpGrabbablesFix.Item2 = flag;
				this.cachedVStumpGrabbablesFix.Item1 = true;
			}
			return flag;
		}

		// Token: 0x06006D13 RID: 27923 RVA: 0x00233FF4 File Offset: 0x002321F4
		public bool CheckIsSuppressZonesInVStumpEnabled()
		{
			if (this.cachedSuppressZonesInVStump.Item1)
			{
				return this.cachedSuppressZonesInVStump.Item2;
			}
			bool flag = this.featureFlags.IsEnabled("2026-04-SuppressZonesInVStump");
			if (this.featureFlags.ready)
			{
				this.cachedSuppressZonesInVStump.Item2 = flag;
				this.cachedSuppressZonesInVStump.Item1 = true;
			}
			return flag;
		}

		// Token: 0x06006D14 RID: 27924 RVA: 0x00234051 File Offset: 0x00232251
		public bool CheckRoomControlsEnabled()
		{
			return this.featureFlags.IsEnabled("2026-05-RoomControlsEnabled");
		}

		// Token: 0x06006D15 RID: 27925 RVA: 0x00234063 File Offset: 0x00232263
		public bool CheckRoomControlsEnabledForUser(string playFabId)
		{
			return this.featureFlags.IsEnabledForUser("2026-05-RoomControlsEnabled", playFabId);
		}

		// Token: 0x06006D16 RID: 27926 RVA: 0x00234076 File Offset: 0x00232276
		public bool CheckRoomControlsEnabledForAnyone()
		{
			return this.featureFlags.IsEnabledForAnyone("2026-05-RoomControlsEnabled");
		}

		// Token: 0x04007D4F RID: 32079
		public static volatile GorillaServer Instance;

		// Token: 0x04007D50 RID: 32080
		public string FeatureFlagsTitleDataKey = "DeployFeatureFlags";

		// Token: 0x04007D51 RID: 32081
		public List<string> DefaultDeployFeatureFlagsEnabled = new List<string>();

		// Token: 0x04007D52 RID: 32082
		private TitleDataFeatureFlags featureFlags = new TitleDataFeatureFlags();

		// Token: 0x04007D53 RID: 32083
		private bool debug;

		// Token: 0x04007D54 RID: 32084
		private JsonSerializerSettings serializationSettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			MissingMemberHandling = MissingMemberHandling.Ignore,
			ObjectCreationHandling = ObjectCreationHandling.Replace,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			TypeNameHandling = TypeNameHandling.Auto
		};

		// Token: 0x04007D55 RID: 32085
		[TupleElementNames(new string[] { "valid", "value" })]
		private ValueTuple<bool, bool> cachedVStumpGrabbablesFix;

		// Token: 0x04007D56 RID: 32086
		[TupleElementNames(new string[] { "valid", "value" })]
		private ValueTuple<bool, bool> cachedSuppressZonesInVStump;
	}
}
