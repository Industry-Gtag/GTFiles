using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Networking;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001054 RID: 4180
	public class SharedBlocksManager : MonoBehaviour
	{
		// Token: 0x140000B1 RID: 177
		// (add) Token: 0x06006843 RID: 26691 RVA: 0x0021936C File Offset: 0x0021756C
		// (remove) Token: 0x06006844 RID: 26692 RVA: 0x002193A4 File Offset: 0x002175A4
		public event Action<string> OnGetTableConfiguration;

		// Token: 0x140000B2 RID: 178
		// (add) Token: 0x06006845 RID: 26693 RVA: 0x002193DC File Offset: 0x002175DC
		// (remove) Token: 0x06006846 RID: 26694 RVA: 0x00219414 File Offset: 0x00217614
		public event Action<string> OnGetTitleDataBuildComplete;

		// Token: 0x140000B3 RID: 179
		// (add) Token: 0x06006847 RID: 26695 RVA: 0x0021944C File Offset: 0x0021764C
		// (remove) Token: 0x06006848 RID: 26696 RVA: 0x00219484 File Offset: 0x00217684
		public event Action<int> OnSavePrivateScanSuccess;

		// Token: 0x140000B4 RID: 180
		// (add) Token: 0x06006849 RID: 26697 RVA: 0x002194BC File Offset: 0x002176BC
		// (remove) Token: 0x0600684A RID: 26698 RVA: 0x002194F4 File Offset: 0x002176F4
		public event Action<int, string> OnSavePrivateScanFailed;

		// Token: 0x140000B5 RID: 181
		// (add) Token: 0x0600684B RID: 26699 RVA: 0x0021952C File Offset: 0x0021772C
		// (remove) Token: 0x0600684C RID: 26700 RVA: 0x00219564 File Offset: 0x00217764
		public event Action<int, bool> OnFetchPrivateScanComplete;

		// Token: 0x140000B6 RID: 182
		// (add) Token: 0x0600684D RID: 26701 RVA: 0x0021959C File Offset: 0x0021779C
		// (remove) Token: 0x0600684E RID: 26702 RVA: 0x002195D4 File Offset: 0x002177D4
		public event Action<bool, SharedBlocksManager.SharedBlocksMap> OnFoundDefaultSharedBlocksMap;

		// Token: 0x140000B7 RID: 183
		// (add) Token: 0x0600684F RID: 26703 RVA: 0x0021960C File Offset: 0x0021780C
		// (remove) Token: 0x06006850 RID: 26704 RVA: 0x00219644 File Offset: 0x00217844
		public event Action<bool> OnGetPopularMapsComplete;

		// Token: 0x140000B8 RID: 184
		// (add) Token: 0x06006851 RID: 26705 RVA: 0x0021967C File Offset: 0x0021787C
		// (remove) Token: 0x06006852 RID: 26706 RVA: 0x002196B0 File Offset: 0x002178B0
		public static event Action OnRecentMapIdsUpdated;

		// Token: 0x140000B9 RID: 185
		// (add) Token: 0x06006853 RID: 26707 RVA: 0x002196E4 File Offset: 0x002178E4
		// (remove) Token: 0x06006854 RID: 26708 RVA: 0x00219718 File Offset: 0x00217918
		public static event Action OnSaveTimeUpdated;

		// Token: 0x170009E6 RID: 2534
		// (get) Token: 0x06006855 RID: 26709 RVA: 0x0021974B File Offset: 0x0021794B
		public List<SharedBlocksManager.SharedBlocksMap> LatestPopularMaps
		{
			get
			{
				return this.latestPopularMaps;
			}
		}

		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x06006856 RID: 26710 RVA: 0x00219753 File Offset: 0x00217953
		public string[] BuildData
		{
			get
			{
				return this.privateScanDataCache;
			}
		}

		// Token: 0x06006857 RID: 26711 RVA: 0x0021975B File Offset: 0x0021795B
		public bool IsWaitingOnRequest()
		{
			return this.saveScanInProgress || this.getScanInProgress;
		}

		// Token: 0x06006858 RID: 26712 RVA: 0x00219770 File Offset: 0x00217970
		private void Awake()
		{
			if (SharedBlocksManager.instance == null)
			{
				SharedBlocksManager.instance = this;
				for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
				{
					this.privateScanDataCache[i] = string.Empty;
					this.hasPulledPrivateScanMothership[i] = false;
				}
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06006859 RID: 26713 RVA: 0x002197C0 File Offset: 0x002179C0
		public async void Start()
		{
			SharedBlocksManager.saveDateKeys.Clear();
			for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
			{
				SharedBlocksManager.saveDateKeys.Add(this.GetPlayfabSlotTimeKey(i));
			}
			await this.WaitForPlayfabSessionToken();
			this.FetchConfigurationFromTitleData();
			this.LoadPlayerPrefs();
			if (NetworkSystem.Instance != null)
			{
				NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			}
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom)
			{
				this.RefreshPopularMapsForRandom();
			}
		}

		// Token: 0x0600685A RID: 26714 RVA: 0x002197F7 File Offset: 0x002179F7
		private void OnDestroy()
		{
			if (NetworkSystem.Instance != null)
			{
				NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
			}
		}

		// Token: 0x0600685B RID: 26715 RVA: 0x00219827 File Offset: 0x00217A27
		private void OnJoinedRoom()
		{
			Debug.Log("OnJoinedRoom inside SharedBlocksManager");
			this.RefreshPopularMapsForRandom();
		}

		// Token: 0x0600685C RID: 26716 RVA: 0x0021983C File Offset: 0x00217A3C
		private bool TryGetCachedSharedBlocksMapByMapID(string mapID, out SharedBlocksManager.SharedBlocksMap result)
		{
			foreach (SharedBlocksManager.SharedBlocksMap sharedBlocksMap in this.mapResponseCache)
			{
				if (sharedBlocksMap.MapID.Equals(mapID))
				{
					result = sharedBlocksMap;
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x0600685D RID: 26717 RVA: 0x002198A4 File Offset: 0x00217AA4
		private void AddMapToResponseCache(SharedBlocksManager.SharedBlocksMap map)
		{
			if (map == null)
			{
				return;
			}
			try
			{
				int num = this.mapResponseCache.FindIndex((SharedBlocksManager.SharedBlocksMap x) => x.MapID.Equals(map.MapID));
				if (num < 0)
				{
					this.mapResponseCache.Add(map);
				}
				else
				{
					this.mapResponseCache[num] = map;
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("SharedBlocksManager AddMapToResponseCache Exception " + ex.ToString(), null);
			}
			if (this.mapResponseCache.Count >= 5)
			{
				this.mapResponseCache.RemoveAt(0);
			}
		}

		// Token: 0x0600685E RID: 26718 RVA: 0x00219950 File Offset: 0x00217B50
		public static bool IsMapIDValid(string mapID)
		{
			if (mapID.IsNullOrEmpty())
			{
				return false;
			}
			if (mapID.Length != 8)
			{
				return false;
			}
			if (!Regex.IsMatch(mapID, "^[CFGHKMNPRTWXZ256789]+$"))
			{
				GTDev.LogError<string>("Invalid Characters in SharedBlocksManager IsMapIDValid map " + mapID, null);
				return false;
			}
			return true;
		}

		// Token: 0x0600685F RID: 26719 RVA: 0x00219988 File Offset: 0x00217B88
		public static LinkedList<string> GetRecentUpVotes()
		{
			return SharedBlocksManager.recentUpVotes;
		}

		// Token: 0x06006860 RID: 26720 RVA: 0x0021998F File Offset: 0x00217B8F
		public static List<string> GetLocalMapIDs()
		{
			return SharedBlocksManager.localMapIds;
		}

		// Token: 0x06006861 RID: 26721 RVA: 0x00219998 File Offset: 0x00217B98
		private static void SetPublishTimeForSlot(int slotID, DateTime time)
		{
			SharedBlocksManager.LocalPublishInfo localPublishInfo;
			if (SharedBlocksManager.localPublishData.TryGetValue(slotID, out localPublishInfo))
			{
				localPublishInfo.publishTime = time.ToBinary();
				SharedBlocksManager.localPublishData[slotID] = localPublishInfo;
				return;
			}
			SharedBlocksManager.LocalPublishInfo localPublishInfo2 = new SharedBlocksManager.LocalPublishInfo
			{
				mapID = null,
				publishTime = time.ToBinary()
			};
			SharedBlocksManager.localPublishData.Add(slotID, localPublishInfo2);
		}

		// Token: 0x06006862 RID: 26722 RVA: 0x002199FC File Offset: 0x00217BFC
		private static void SetMapIDAndPublishTimeForSlot(int slotID, string mapID, DateTime time)
		{
			SharedBlocksManager.LocalPublishInfo localPublishInfo = new SharedBlocksManager.LocalPublishInfo
			{
				mapID = mapID,
				publishTime = time.ToBinary()
			};
			SharedBlocksManager.localPublishData.AddOrUpdate(slotID, localPublishInfo);
		}

		// Token: 0x06006863 RID: 26723 RVA: 0x00219A38 File Offset: 0x00217C38
		public static SharedBlocksManager.LocalPublishInfo GetPublishInfoForSlot(int slot)
		{
			SharedBlocksManager.LocalPublishInfo localPublishInfo;
			if (SharedBlocksManager.localPublishData.TryGetValue(slot, out localPublishInfo))
			{
				return localPublishInfo;
			}
			return new SharedBlocksManager.LocalPublishInfo
			{
				mapID = null,
				publishTime = DateTime.MinValue.ToBinary()
			};
		}

		// Token: 0x06006864 RID: 26724 RVA: 0x00219A78 File Offset: 0x00217C78
		private void LoadPlayerPrefs()
		{
			string recentVotesPrefsKey = this.serializationConfig.recentVotesPrefsKey;
			string localMapsPrefsKey = this.serializationConfig.localMapsPrefsKey;
			string @string = PlayerPrefs.GetString(recentVotesPrefsKey, null);
			string string2 = PlayerPrefs.GetString(localMapsPrefsKey, null);
			if (!@string.IsNullOrEmpty())
			{
				try
				{
					SharedBlocksManager.recentUpVotes = JsonConvert.DeserializeObject<LinkedList<string>>(@string);
					while (SharedBlocksManager.recentUpVotes.Count > 10)
					{
						SharedBlocksManager.recentUpVotes.RemoveLast();
					}
					goto IL_0082;
				}
				catch (Exception ex)
				{
					GTDev.LogWarning<string>("SharedBlocksManager failed to deserialize Recent Up Votes " + ex.Message, null);
					SharedBlocksManager.recentUpVotes.Clear();
					goto IL_0082;
				}
			}
			SharedBlocksManager.recentUpVotes.Clear();
			IL_0082:
			if (!string2.IsNullOrEmpty())
			{
				SharedBlocksManager.localPublishData.Clear();
				SharedBlocksManager.localMapIds.Clear();
				try
				{
					SharedBlocksManager.localPublishData = JsonConvert.DeserializeObject<Dictionary<int, SharedBlocksManager.LocalPublishInfo>>(string2);
				}
				catch (Exception ex2)
				{
					GTDev.LogWarning<string>("SharedBlocksManager failed to deserialize localMapIDs " + ex2.Message, null);
					this.GetPlayfabLastSaveTime();
				}
				foreach (KeyValuePair<int, SharedBlocksManager.LocalPublishInfo> keyValuePair in SharedBlocksManager.localPublishData)
				{
					if (!keyValuePair.Value.mapID.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(keyValuePair.Value.mapID))
					{
						SharedBlocksManager.localMapIds.Add(keyValuePair.Value.mapID);
					}
				}
				Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
				if (onSaveTimeUpdated != null)
				{
					onSaveTimeUpdated();
				}
			}
			else
			{
				SharedBlocksManager.localMapIds.Clear();
				this.GetPlayfabLastSaveTime();
			}
			Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
			if (onRecentMapIdsUpdated == null)
			{
				return;
			}
			onRecentMapIdsUpdated();
		}

		// Token: 0x06006865 RID: 26725 RVA: 0x00219C1C File Offset: 0x00217E1C
		private void SaveRecentVotesToPlayerPrefs()
		{
			PlayerPrefs.SetString(this.serializationConfig.recentVotesPrefsKey, JsonConvert.SerializeObject(SharedBlocksManager.recentUpVotes));
			PlayerPrefs.Save();
		}

		// Token: 0x06006866 RID: 26726 RVA: 0x00219C3D File Offset: 0x00217E3D
		private void SaveLocalMapIdsToPlayerPrefs()
		{
			PlayerPrefs.SetString(this.serializationConfig.localMapsPrefsKey, JsonConvert.SerializeObject(SharedBlocksManager.localPublishData));
			PlayerPrefs.Save();
		}

		// Token: 0x06006867 RID: 26727 RVA: 0x00219C60 File Offset: 0x00217E60
		public void RequestVote(string mapID, bool up, Action<bool, string> callback)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestVote Client Not Logged into Mothership", null);
				if (callback != null)
				{
					callback(false, 1.ToString());
				}
				return;
			}
			if (this.voteInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestVote already in progress", null);
				return;
			}
			this.voteInProgress = true;
			base.StartCoroutine(this.PostVote(new SharedBlocksManager.VoteRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				mapId = mapID,
				vote = (up ? 1 : (-1))
			}, callback));
		}

		// Token: 0x06006868 RID: 26728 RVA: 0x00219CF6 File Offset: 0x00217EF6
		private IEnumerator PostVote(SharedBlocksManager.VoteRequest data, Action<bool, string> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/MapVote", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				string mapId = data.mapId;
				if (data.vote == -1)
				{
					if (SharedBlocksManager.recentUpVotes.Remove(mapId))
					{
						this.SaveRecentVotesToPlayerPrefs();
						Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
						if (onRecentMapIdsUpdated != null)
						{
							onRecentMapIdsUpdated();
						}
					}
				}
				else if (!SharedBlocksManager.recentUpVotes.Contains(mapId))
				{
					if (SharedBlocksManager.recentUpVotes.Count >= 10)
					{
						SharedBlocksManager.recentUpVotes.RemoveLast();
					}
					SharedBlocksManager.recentUpVotes.AddFirst(mapId);
					this.SaveRecentVotesToPlayerPrefs();
					Action onRecentMapIdsUpdated2 = SharedBlocksManager.OnRecentMapIdsUpdated;
					if (onRecentMapIdsUpdated2 != null)
					{
						onRecentMapIdsUpdated2();
					}
				}
				this.voteInProgress = false;
				if (callback != null)
				{
					callback(true, "");
				}
			}
			else
			{
				GTDev.LogError<string>(string.Format("PostVote Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text, null);
				if (request.result != UnityWebRequest.Result.ProtocolError)
				{
					retry = true;
				}
				else
				{
					long responseCode = request.responseCode;
					if (responseCode >= 500L)
					{
						if (responseCode >= 600L)
						{
							goto IL_0207;
						}
					}
					else if (responseCode != 408L && responseCode != 429L)
					{
						goto IL_0207;
					}
					bool flag = true;
					goto IL_020A;
					IL_0207:
					flag = false;
					IL_020A:
					if (flag)
					{
						retry = true;
					}
					else
					{
						this.voteInProgress = false;
						if (callback != null)
						{
							callback(false, "REQUEST ERROR");
						}
					}
				}
			}
			if (retry)
			{
				if (this.voteRetryCount < this.maxRetriesOnFail)
				{
					float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.voteRetryCount + 1)));
					this.voteRetryCount++;
					yield return new WaitForSecondsRealtime(num);
					this.voteInProgress = false;
					this.RequestVote(data.mapId, data.vote == 1, callback);
				}
				else
				{
					this.voteRetryCount = 0;
					this.voteInProgress = false;
					if (callback != null)
					{
						callback(false, "CONNECTION ERROR");
					}
				}
			}
			yield break;
		}

		// Token: 0x06006869 RID: 26729 RVA: 0x00219D14 File Offset: 0x00217F14
		public void RefreshPopularMapsForRandom()
		{
			if (!ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocksShared))
			{
				return;
			}
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				return;
			}
			if (this.getTopMapsInProgress)
			{
				return;
			}
			if (!this.hasCachedTopMaps || Time.realtimeSinceStartupAsDouble > this.lastGetTopMapsTime + 60.0 || this.latestPopularMaps == null || this.latestPopularMaps.Count <= 0)
			{
				this.RequestGetConfiguredTopMaps();
				return;
			}
			Action<bool> onGetPopularMapsComplete = this.OnGetPopularMapsComplete;
			if (onGetPopularMapsComplete == null)
			{
				return;
			}
			onGetPopularMapsComplete(true);
		}

		// Token: 0x0600686A RID: 26730 RVA: 0x00219D8F File Offset: 0x00217F8F
		public bool RequestGetConfiguredTopMaps()
		{
			return this.RequestGetTopMaps(this.startingMapConfig.pageNumber, this.startingMapConfig.pageSize, this.startingMapConfig.sortMethod);
		}

		// Token: 0x0600686B RID: 26731 RVA: 0x00219DB8 File Offset: 0x00217FB8
		private void RequestPublishMap(string userMetadataKey)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestPublishMap Client Not Logged into Mothership", null);
				this.PublishMapComplete(false, userMetadataKey, string.Empty, 0L);
				return;
			}
			if (this.publishRequestInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestPublishMap Publish Request in progress", null);
				return;
			}
			this.publishRequestInProgress = true;
			base.StartCoroutine(this.PostPublishMapRequest(new SharedBlocksManager.PublishMapRequestData
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				userdataMetadataKey = userMetadataKey,
				playerNickname = GorillaTagger.Instance.offlineVRRig.playerNameVisible
			}, new SharedBlocksManager.PublishMapRequestCallback(this.PublishMapComplete)));
		}

		// Token: 0x0600686C RID: 26732 RVA: 0x00219E60 File Offset: 0x00218060
		private void PublishMapComplete(bool success, string key, [CanBeNull] string mapID, long response)
		{
			this.publishRequestInProgress = false;
			if (success)
			{
				int num = this.serializationConfig.scanSlotMothershipKeys.IndexOf(key);
				if (num >= 0)
				{
					SharedBlocksManager.LocalPublishInfo localPublishInfo;
					if (SharedBlocksManager.localPublishData.TryGetValue(num, out localPublishInfo))
					{
						SharedBlocksManager.localMapIds.Remove(localPublishInfo.mapID);
					}
					SharedBlocksManager.SetMapIDAndPublishTimeForSlot(num, mapID, DateTime.Now);
					this.SaveLocalMapIdsToPlayerPrefs();
				}
				if (!SharedBlocksManager.localMapIds.Contains(mapID))
				{
					SharedBlocksManager.localMapIds.Add(mapID);
					Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
					if (onRecentMapIdsUpdated != null)
					{
						onRecentMapIdsUpdated();
					}
				}
				SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = mapID,
					MapData = this.privateScanDataCache[num],
					CreatorNickName = GorillaTagger.Instance.offlineVRRig.playerNameVisible,
					UpdateTime = DateTime.Now
				};
				this.AddMapToResponseCache(sharedBlocksMap);
				Action<int> onSavePrivateScanSuccess = this.OnSavePrivateScanSuccess;
				if (onSavePrivateScanSuccess != null)
				{
					onSavePrivateScanSuccess(this.currentSaveScanIndex);
				}
			}
			else
			{
				Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
				if (onSavePrivateScanFailed != null)
				{
					onSavePrivateScanFailed(this.currentSaveScanIndex, "ERROR PUBLISHING: " + response.ToString());
				}
			}
			this.currentSaveScanIndex = -1;
			this.currentSaveScanData = string.Empty;
		}

		// Token: 0x0600686D RID: 26733 RVA: 0x00219F7F File Offset: 0x0021817F
		private IEnumerator PostPublishMapRequest(SharedBlocksManager.PublishMapRequestData data, SharedBlocksManager.PublishMapRequestCallback callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/Publish", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				GTDev.Log<string>("PostPublishMapRequest Success: raw response: " + request.downloadHandler.text, null);
				try
				{
					string text = request.downloadHandler.text;
					bool flag = !text.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(text);
					if (callback != null)
					{
						callback(flag, data.userdataMetadataKey, text, request.responseCode);
					}
					goto IL_021D;
				}
				catch (Exception ex)
				{
					GTDev.LogError<string>("SharedBlocksManager PostPublishMapRequest " + ex.Message, null);
					if (callback != null)
					{
						callback(false, data.userdataMetadataKey, null, request.responseCode);
					}
					goto IL_021D;
				}
			}
			if (request.result != UnityWebRequest.Result.ProtocolError)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_01E0;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_01E0;
				}
				bool flag2 = true;
				goto IL_01E3;
				IL_01E0:
				flag2 = false;
				IL_01E3:
				if (flag2)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback(false, data.userdataMetadataKey, string.Empty, request.responseCode);
				}
			}
			IL_021D:
			if (retry)
			{
				if (this.postPublishMapRetryCount < this.maxRetriesOnFail)
				{
					float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.postPublishMapRetryCount + 1)));
					this.postPublishMapRetryCount++;
					yield return new WaitForSecondsRealtime(num);
					this.publishRequestInProgress = false;
					this.RequestPublishMap(data.userdataMetadataKey);
				}
				else
				{
					this.postPublishMapRetryCount = 0;
					if (callback != null)
					{
						callback(false, data.userdataMetadataKey, string.Empty, request.responseCode);
					}
				}
			}
			yield break;
		}

		// Token: 0x0600686E RID: 26734 RVA: 0x00219F9C File Offset: 0x0021819C
		public void RequestMapDataFromID(string mapID, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestMapDataFromID Client Not Logged into Mothership", null);
				if (callback != null)
				{
					callback(null);
				}
				return;
			}
			SharedBlocksManager.SharedBlocksMap sharedBlocksMap;
			if (this.TryGetCachedSharedBlocksMapByMapID(mapID, out sharedBlocksMap))
			{
				if (callback != null)
				{
					callback(sharedBlocksMap);
				}
				return;
			}
			if (this.getMapDataFromIDInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestMapDataFromID Fetch already in progress", null);
				return;
			}
			this.getMapDataFromIDInProgress = true;
			base.StartCoroutine(this.GetMapDataFromID(new SharedBlocksManager.GetMapDataFromIDRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				mapId = mapID
			}, callback));
		}

		// Token: 0x0600686F RID: 26735 RVA: 0x0021A032 File Offset: 0x00218232
		private IEnumerator GetMapDataFromID(SharedBlocksManager.GetMapDataFromIDRequest data, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/GetMapData", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				string text = request.downloadHandler.text;
				this.GetMapDataFromIDComplete(data.mapId, text, callback);
			}
			else if (request.result != UnityWebRequest.Result.ProtocolError)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_014E;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_014E;
				}
				bool flag = true;
				goto IL_0151;
				IL_014E:
				flag = false;
				IL_0151:
				if (flag)
				{
					retry = true;
				}
				else
				{
					this.GetMapDataFromIDComplete(data.mapId, null, callback);
				}
			}
			if (retry)
			{
				if (this.getMapDataFromIDRetryCount < this.maxRetriesOnFail)
				{
					float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.getMapDataFromIDRetryCount + 1)));
					this.getMapDataFromIDRetryCount++;
					yield return new WaitForSecondsRealtime(num);
					this.getMapDataFromIDInProgress = false;
					this.RequestMapDataFromID(data.mapId, callback);
				}
				else
				{
					this.getMapDataFromIDRetryCount = 0;
					this.GetMapDataFromIDComplete(data.mapId, null, callback);
				}
			}
			yield break;
		}

		// Token: 0x06006870 RID: 26736 RVA: 0x0021A050 File Offset: 0x00218250
		private void GetMapDataFromIDComplete(string mapID, [CanBeNull] string response, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			this.getMapDataFromIDInProgress = false;
			if (response == null)
			{
				if (callback != null)
				{
					callback(null);
					return;
				}
			}
			else
			{
				SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = mapID,
					MapData = response
				};
				this.AddMapToResponseCache(sharedBlocksMap);
				if (callback != null)
				{
					callback(sharedBlocksMap);
				}
			}
		}

		// Token: 0x06006871 RID: 26737 RVA: 0x0021A098 File Offset: 0x00218298
		public bool RequestGetTopMaps(int pageNum, int pageSize, string sort)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps Client Not Logged into Mothership", null);
				return false;
			}
			if (this.getTopMapsInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps already in progress", null);
				return false;
			}
			this.getTopMapsInProgress = true;
			this.lastGetTopMapsTime = Time.realtimeSinceStartupAsDouble;
			base.StartCoroutine(this.GetTopMaps(new SharedBlocksManager.GetMapsRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				page = pageNum,
				pageSize = pageSize,
				sort = sort,
				ShowInactive = false
			}, new Action<List<SharedBlocksManager.SharedBlocksMapMetaData>>(this.GetTopMapsComplete)));
			return true;
		}

		// Token: 0x06006872 RID: 26738 RVA: 0x0021A13C File Offset: 0x0021833C
		private IEnumerator GetTopMaps(SharedBlocksManager.GetMapsRequest data, Action<List<SharedBlocksManager.SharedBlocksMapMetaData>> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/GetMaps", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				try
				{
					List<SharedBlocksManager.SharedBlocksMapMetaData> list = JsonConvert.DeserializeObject<List<SharedBlocksManager.SharedBlocksMapMetaData>>(request.downloadHandler.text);
					if (callback != null)
					{
						callback(list);
					}
					goto IL_0187;
				}
				catch (Exception)
				{
					if (callback != null)
					{
						callback(null);
					}
					goto IL_0187;
				}
			}
			if (request.result != UnityWebRequest.Result.ProtocolError)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_0165;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_0165;
				}
				bool flag = true;
				goto IL_0168;
				IL_0165:
				flag = false;
				IL_0168:
				if (flag)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback(null);
				}
			}
			IL_0187:
			if (retry)
			{
				if (this.getTopMapsRetryCount < this.maxRetriesOnFail)
				{
					float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.getTopMapsRetryCount + 1)));
					this.getTopMapsRetryCount++;
					yield return new WaitForSecondsRealtime(num);
					this.getTopMapsInProgress = false;
					this.RequestGetTopMaps(data.page, data.pageSize, data.sort);
				}
				else
				{
					this.getTopMapsRetryCount = 0;
					if (callback != null)
					{
						callback(null);
					}
				}
			}
			yield break;
		}

		// Token: 0x06006873 RID: 26739 RVA: 0x0021A15C File Offset: 0x0021835C
		private void GetTopMapsComplete([CanBeNull] List<SharedBlocksManager.SharedBlocksMapMetaData> maps)
		{
			this.getTopMapsInProgress = false;
			if (maps != null)
			{
				this.latestPopularMaps.Clear();
				foreach (SharedBlocksManager.SharedBlocksMapMetaData sharedBlocksMapMetaData in maps)
				{
					if (sharedBlocksMapMetaData != null && SharedBlocksManager.IsMapIDValid(sharedBlocksMapMetaData.mapId))
					{
						DateTime dateTime = DateTime.MinValue;
						DateTime dateTime2 = DateTime.MinValue;
						try
						{
							dateTime = DateTime.Parse(sharedBlocksMapMetaData.createdTime);
							dateTime2 = DateTime.Parse(sharedBlocksMapMetaData.updatedTime);
						}
						catch (Exception ex)
						{
							GTDev.LogWarning<string>("SharedBlocksManager GetTopMaps bad update or create time" + ex.Message, null);
						}
						SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
						{
							MapID = sharedBlocksMapMetaData.mapId,
							CreatorID = null,
							CreatorNickName = sharedBlocksMapMetaData.nickname,
							CreateTime = dateTime,
							UpdateTime = dateTime2,
							MapData = null
						};
						this.latestPopularMaps.Add(sharedBlocksMap);
					}
				}
				this.hasCachedTopMaps = true;
				Action<bool> onGetPopularMapsComplete = this.OnGetPopularMapsComplete;
				if (onGetPopularMapsComplete == null)
				{
					return;
				}
				onGetPopularMapsComplete(true);
				return;
			}
			else
			{
				Action<bool> onGetPopularMapsComplete2 = this.OnGetPopularMapsComplete;
				if (onGetPopularMapsComplete2 == null)
				{
					return;
				}
				onGetPopularMapsComplete2(false);
				return;
			}
		}

		// Token: 0x06006874 RID: 26740 RVA: 0x0021A298 File Offset: 0x00218498
		private void RequestUpdateMapActive(string userMetadataKey, bool active)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestUpdateMapActive Client Not Logged into Mothership", null);
				return;
			}
			if (this.updateMapActiveInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestUpdateMapActive already in progress", null);
				return;
			}
			this.updateMapActiveInProgress = true;
			base.StartCoroutine(this.PostUpdateMapActive(new SharedBlocksManager.UpdateMapActiveRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				userdataMetadataKey = userMetadataKey,
				setActive = active
			}, new Action<bool>(this.OnUpdatedMapActiveComplete)));
		}

		// Token: 0x06006875 RID: 26741 RVA: 0x0021A320 File Offset: 0x00218520
		private IEnumerator PostUpdateMapActive(SharedBlocksManager.UpdateMapActiveRequest data, Action<bool> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/UpdateMapActive", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				if (callback != null)
				{
					callback(true);
				}
			}
			else if (request.result != UnityWebRequest.Result.ProtocolError)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_0132;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_0132;
				}
				bool flag = true;
				goto IL_0135;
				IL_0132:
				flag = false;
				IL_0135:
				if (flag)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback(false);
				}
			}
			if (retry)
			{
				if (this.updateMapActiveRetryCount < this.maxRetriesOnFail)
				{
					float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.updateMapActiveRetryCount + 1)));
					this.updateMapActiveRetryCount++;
					yield return new WaitForSecondsRealtime(num);
					this.updateMapActiveInProgress = false;
					this.RequestUpdateMapActive(data.userdataMetadataKey, data.setActive);
				}
				else
				{
					this.updateMapActiveRetryCount = 0;
					if (callback != null)
					{
						callback(false);
					}
				}
			}
			yield break;
		}

		// Token: 0x06006876 RID: 26742 RVA: 0x0021A33D File Offset: 0x0021853D
		private void OnUpdatedMapActiveComplete(bool success)
		{
			this.updateMapActiveInProgress = false;
		}

		// Token: 0x06006877 RID: 26743 RVA: 0x0021A348 File Offset: 0x00218548
		private async Task WaitForPlayfabSessionToken()
		{
			while (!PlayFabAuthenticator.instance || PlayFabAuthenticator.instance.GetPlayFabPlayerId().IsNullOrEmpty() || PlayFabAuthenticator.instance.GetPlayFabSessionTicket().IsNullOrEmpty() || PlayFabAuthenticator.instance.userID.IsNullOrEmpty())
			{
				await Task.Yield();
				await Task.Delay(1000);
			}
		}

		// Token: 0x06006878 RID: 26744 RVA: 0x0021A383 File Offset: 0x00218583
		public void RequestTableConfiguration()
		{
			if (this.fetchedTableConfig)
			{
				Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
				if (onGetTableConfiguration == null)
				{
					return;
				}
				onGetTableConfiguration(this.tableConfigResponse);
			}
		}

		// Token: 0x06006879 RID: 26745 RVA: 0x0021A3A3 File Offset: 0x002185A3
		private void FetchConfigurationFromTitleData()
		{
			PlayFabTitleDataCache.Instance.GetTitleData(this.serializationConfig.tableConfigurationKey, new Action<string>(this.OnGetConfigurationSuccess), new Action<PlayFabError>(this.OnGetConfigurationFail), false);
		}

		// Token: 0x0600687A RID: 26746 RVA: 0x0021A3D3 File Offset: 0x002185D3
		private void OnGetConfigurationSuccess(string dataRecord)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetConfigurationSuccess", null);
			this.tableConfigResponse = dataRecord;
			this.fetchedTableConfig = true;
			Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
			if (onGetTableConfiguration == null)
			{
				return;
			}
			onGetTableConfiguration(this.tableConfigResponse);
		}

		// Token: 0x0600687B RID: 26747 RVA: 0x0021A404 File Offset: 0x00218604
		private void OnGetConfigurationFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnGetConfigurationFail " + ((error != null) ? error.ToString() : null), null);
			if (this.fetchTableConfigRetryCount < this.maxRetriesOnFail)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.fetchTableConfigRetryCount + 1)));
				this.fetchTableConfigRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(num, new Action(this.FetchConfigurationFromTitleData)));
				return;
			}
			this.tableConfigResponse = string.Empty;
			this.fetchedTableConfig = true;
			Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
			if (onGetTableConfiguration == null)
			{
				return;
			}
			onGetTableConfiguration(this.tableConfigResponse);
		}

		// Token: 0x0600687C RID: 26748 RVA: 0x0021A4AB File Offset: 0x002186AB
		private IEnumerator RetryAfterWaitTime(float waitTime, Action function)
		{
			yield return new WaitForSecondsRealtime(waitTime);
			if (function != null)
			{
				function();
			}
			yield break;
		}

		// Token: 0x0600687D RID: 26749 RVA: 0x0021A4C4 File Offset: 0x002186C4
		public void FetchTitleDataBuild()
		{
			if (!this.fetchTitleDataBuildComplete)
			{
				if (!this.fetchTitleDataBuildInProgress)
				{
					this.fetchTitleDataBuildInProgress = true;
					PlayFabTitleDataCache.Instance.GetTitleData(this.serializationConfig.titleDataKey, new Action<string>(this.OnGetTitleDataBuildSuccess), new Action<PlayFabError>(this.OnGetTitleDataBuildFail), false);
				}
				return;
			}
			Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
			if (onGetTitleDataBuildComplete == null)
			{
				return;
			}
			onGetTitleDataBuildComplete(this.titleDataBuildCache);
		}

		// Token: 0x0600687E RID: 26750 RVA: 0x0021A530 File Offset: 0x00218730
		private void OnGetTitleDataBuildSuccess(string dataRecord)
		{
			this.fetchTitleDataBuildInProgress = false;
			GTDev.Log<string>("SharedBlocksManager OnGetTitleDataBuildSuccess", null);
			if (!dataRecord.IsNullOrEmpty())
			{
				this.titleDataBuildCache = dataRecord;
				this.fetchTitleDataBuildComplete = true;
				Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
				if (onGetTitleDataBuildComplete == null)
				{
					return;
				}
				onGetTitleDataBuildComplete(this.titleDataBuildCache);
				return;
			}
			else
			{
				this.titleDataBuildCache = string.Empty;
				this.fetchTitleDataBuildComplete = true;
				Action<string> onGetTitleDataBuildComplete2 = this.OnGetTitleDataBuildComplete;
				if (onGetTitleDataBuildComplete2 == null)
				{
					return;
				}
				onGetTitleDataBuildComplete2(this.titleDataBuildCache);
				return;
			}
		}

		// Token: 0x0600687F RID: 26751 RVA: 0x0021A5A4 File Offset: 0x002187A4
		private void OnGetTitleDataBuildFail(PlayFabError error)
		{
			this.fetchTitleDataBuildInProgress = false;
			GTDev.LogWarning<string>("SharedBlocksManager FetchTitleDataBuildFail " + ((error != null) ? error.ToString() : null), null);
			if (this.fetchTitleDataRetryCount < this.maxRetriesOnFail)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.fetchTitleDataRetryCount + 1)));
				this.fetchTitleDataRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(num, new Action(this.FetchTitleDataBuild)));
				return;
			}
			this.titleDataBuildCache = string.Empty;
			this.fetchTitleDataBuildComplete = true;
			Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
			if (onGetTitleDataBuildComplete == null)
			{
				return;
			}
			onGetTitleDataBuildComplete(this.titleDataBuildCache);
		}

		// Token: 0x06006880 RID: 26752 RVA: 0x0021A652 File Offset: 0x00218852
		private string GetPlayfabKeyForSlot(int slot)
		{
			return this.serializationConfig.playfabScanKey + slot.ToString("D2");
		}

		// Token: 0x06006881 RID: 26753 RVA: 0x0021A670 File Offset: 0x00218870
		private string GetPlayfabSlotTimeKey(int slot)
		{
			return this.serializationConfig.playfabScanKey + slot.ToString("D2") + this.serializationConfig.timeAppend;
		}

		// Token: 0x06006882 RID: 26754 RVA: 0x0021A69C File Offset: 0x0021889C
		private void GetPlayfabLastSaveTime()
		{
			if (!this.hasQueriedSaveTime)
			{
				global::PlayFab.ClientModels.GetUserDataRequest getUserDataRequest = new global::PlayFab.ClientModels.GetUserDataRequest
				{
					PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
					Keys = SharedBlocksManager.saveDateKeys
				};
				try
				{
					PlayFabClientAPI.GetUserData(getUserDataRequest, new Action<GetUserDataResult>(this.OnGetLastSaveTimeSuccess), new Action<PlayFabError>(this.OnGetLastSaveTimeFailure), null, null);
				}
				catch (PlayFabException ex)
				{
					this.OnGetLastSaveTimeFailure(new PlayFabError
					{
						Error = PlayFabErrorCode.Unknown,
						ErrorMessage = ex.Message
					});
				}
				this.hasQueriedSaveTime = true;
				return;
			}
			Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
			if (onSaveTimeUpdated == null)
			{
				return;
			}
			onSaveTimeUpdated();
		}

		// Token: 0x06006883 RID: 26755 RVA: 0x0021A740 File Offset: 0x00218940
		private void OnGetLastSaveTimeSuccess(GetUserDataResult result)
		{
			bool flag = false;
			for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
			{
				UserDataRecord userDataRecord;
				if (result.Data.TryGetValue(this.GetPlayfabSlotTimeKey(i), out userDataRecord))
				{
					flag = true;
					DateTime lastUpdated = userDataRecord.LastUpdated;
					SharedBlocksManager.SetPublishTimeForSlot(i, lastUpdated + DateTimeOffset.Now.Offset);
				}
			}
			if (flag)
			{
				this.SaveLocalMapIdsToPlayerPrefs();
			}
			Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
			if (onSaveTimeUpdated == null)
			{
				return;
			}
			onSaveTimeUpdated();
		}

		// Token: 0x06006884 RID: 26756 RVA: 0x0021A7B0 File Offset: 0x002189B0
		private void OnGetLastSaveTimeFailure(PlayFabError error)
		{
			string text = ((error != null) ? error.ErrorMessage : null) ?? "Null";
			GTDev.LogError<string>("SharedBlocksManager GetLastSaveTimeFailure " + text, null);
		}

		// Token: 0x06006885 RID: 26757 RVA: 0x0021A7E4 File Offset: 0x002189E4
		private void FetchBuildFromPlayfab()
		{
			if (this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex])
			{
				Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete != null)
				{
					onFetchPrivateScanComplete(this.currentGetScanIndex, true);
				}
				this.currentGetScanIndex = -1;
				this.getScanInProgress = false;
				return;
			}
			global::PlayFab.ClientModels.GetUserDataRequest getUserDataRequest = new global::PlayFab.ClientModels.GetUserDataRequest
			{
				PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				Keys = new List<string> { this.GetPlayfabKeyForSlot(this.currentGetScanIndex) }
			};
			base.StartCoroutine(this.SendPlayfabUserDataRequest(getUserDataRequest, new Action<GetUserDataResult>(this.OnFetchBuildFromPlayfabSuccess), new Action<PlayFabError>(this.OnFetchBuildFromPlayfabFail)));
		}

		// Token: 0x06006886 RID: 26758 RVA: 0x0021A882 File Offset: 0x00218A82
		private IEnumerator SendPlayfabUserDataRequest(global::PlayFab.ClientModels.GetUserDataRequest request, Action<GetUserDataResult> resultCallback, Action<PlayFabError> errorCallback)
		{
			while (!PlayFabSettings.staticPlayer.IsClientLoggedIn())
			{
				yield return new WaitForSecondsRealtime(5f);
			}
			try
			{
				PlayFabClientAPI.GetUserData(request, resultCallback, errorCallback, null, null);
				yield break;
			}
			catch (PlayFabException ex)
			{
				if (errorCallback != null)
				{
					errorCallback(new PlayFabError
					{
						Error = PlayFabErrorCode.Unknown,
						ErrorMessage = ex.Message
					});
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x06006887 RID: 26759 RVA: 0x0021A8A0 File Offset: 0x00218AA0
		private void OnFetchBuildFromPlayfabSuccess(GetUserDataResult result)
		{
			this.getScanInProgress = false;
			GTDev.Log<string>("SharedBlocksManager OnFetchBuildsFromPlayfabSuccess", null);
			UserDataRecord userDataRecord;
			if (result != null && result.Data != null && result.Data.TryGetValue(this.GetPlayfabKeyForSlot(this.currentGetScanIndex), out userDataRecord))
			{
				this.privateScanDataCache[this.currentGetScanIndex] = userDataRecord.Value;
				this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
				if (!userDataRecord.Value.IsNullOrEmpty())
				{
					this.RequestSavePrivateScan(this.currentGetScanIndex, userDataRecord.Value);
				}
			}
			else
			{
				this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
				this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
			}
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(this.currentGetScanIndex, true);
			}
			this.currentGetScanIndex = -1;
		}

		// Token: 0x06006888 RID: 26760 RVA: 0x0021A968 File Offset: 0x00218B68
		private void OnFetchBuildFromPlayfabFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnFetchBuildsFromPlayfabFail " + (((error != null) ? error.ErrorMessage : null) ?? "Null"), null);
			if (error != null && error.Error == PlayFabErrorCode.ConnectionError && this.fetchPlayfabBuildsRetryCount < this.maxRetriesOnFail)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.fetchPlayfabBuildsRetryCount + 1)));
				this.fetchPlayfabBuildsRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(num, new Action(this.FetchBuildFromPlayfab)));
				return;
			}
			this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
			this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
			this.getScanInProgress = false;
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(this.currentGetScanIndex, false);
			}
			this.currentGetScanIndex = -1;
		}

		// Token: 0x06006889 RID: 26761 RVA: 0x0021AA44 File Offset: 0x00218C44
		public bool TryGetRandomPopularMap(out SharedBlocksManager.SharedBlocksMap map)
		{
			map = null;
			if (this.latestPopularMaps == null || this.latestPopularMaps.Count == 0)
			{
				return false;
			}
			GTDev.Log<string>(string.Format("[SharedBlocksManager] Selecting from {0} popular maps", this.latestPopularMaps.Count), null);
			for (int i = 0; i < Mathf.Min(10, this.latestPopularMaps.Count); i++)
			{
				int num = Random.Range(0, this.latestPopularMaps.Count);
				GTDev.Log<string>(string.Format("[SharedBlocksManager] Random pick index: {0}", num), null);
				SharedBlocksManager.SharedBlocksMap sharedBlocksMap = this.latestPopularMaps[num];
				if (sharedBlocksMap != null && SharedBlocksManager.IsMapIDValid(sharedBlocksMap.MapID))
				{
					map = sharedBlocksMap;
					return true;
				}
			}
			for (int j = 0; j < this.latestPopularMaps.Count; j++)
			{
				SharedBlocksManager.SharedBlocksMap sharedBlocksMap2 = this.latestPopularMaps[j];
				if (sharedBlocksMap2 != null && SharedBlocksManager.IsMapIDValid(sharedBlocksMap2.MapID))
				{
					map = sharedBlocksMap2;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600688A RID: 26762 RVA: 0x0021AB30 File Offset: 0x00218D30
		private async Task WaitForMothership()
		{
			while (!MothershipClientContext.IsClientLoggedIn())
			{
				await Task.Yield();
				await Task.Delay(1000);
			}
		}

		// Token: 0x0600688B RID: 26763 RVA: 0x0021AB6C File Offset: 0x00218D6C
		public void RequestSavePrivateScan(int scanIndex, string scanData)
		{
			if (scanIndex < 0 || scanIndex >= this.serializationConfig.scanSlotMothershipKeys.Count)
			{
				GTDev.LogError<string>(string.Format("SharedBlocksManager RequestSaveScanToMothership: scan index {0} out of bounds", scanIndex), null);
				return;
			}
			this.currentSaveScanIndex = scanIndex;
			this.currentSaveScanData = scanData;
			if (!this.hasPulledPrivateScanMothership[scanIndex])
			{
				this.PullMothershipPrivateScanThenPush(scanIndex);
				return;
			}
			this.privateScanDataCache[scanIndex] = scanData;
			this.RequestSetMothershipUserData(this.serializationConfig.scanSlotMothershipKeys[scanIndex], scanData);
		}

		// Token: 0x0600688C RID: 26764 RVA: 0x0021ABE8 File Offset: 0x00218DE8
		private void PullMothershipPrivateScanThenPush(int scanIndex)
		{
			if (this.getScanInProgress && this.currentGetScanIndex != scanIndex)
			{
				GTDev.LogWarning<string>("SharedBLocksManager PullMothershipPrivateScanThenPush GetScan in progress", null);
				Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
				if (onSavePrivateScanFailed != null)
				{
					onSavePrivateScanFailed(scanIndex, "ERROR SAVING: BUSY");
				}
				this.currentSaveScanIndex = -1;
				this.currentSaveScanData = string.Empty;
				return;
			}
			this.OnFetchPrivateScanComplete += this.PushMothershipPrivateScan;
			this.RequestFetchPrivateScan(scanIndex);
		}

		// Token: 0x0600688D RID: 26765 RVA: 0x0021AC54 File Offset: 0x00218E54
		private void PushMothershipPrivateScan(int scan, bool success)
		{
			if (scan == this.currentSaveScanIndex)
			{
				this.OnFetchPrivateScanComplete -= this.PushMothershipPrivateScan;
				this.privateScanDataCache[this.currentSaveScanIndex] = this.currentSaveScanData;
				this.RequestSetMothershipUserData(this.serializationConfig.scanSlotMothershipKeys[this.currentSaveScanIndex], this.currentSaveScanData);
			}
		}

		// Token: 0x0600688E RID: 26766 RVA: 0x0021ACB4 File Offset: 0x00218EB4
		private void RequestSetMothershipUserData(string keyName, string value)
		{
			if (this.saveScanInProgress)
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: request already in progress");
				return;
			}
			this.saveScanInProgress = true;
			try
			{
				if (!MothershipClientApiUnity.SetUserDataValue(keyName, value, new Action<SetUserDataResponse>(this.OnSetMothershipUserDataSuccess), new Action<MothershipError, int>(this.OnSetMothershipUserDataFail), ""))
				{
					Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: SetUserDataValue Fail");
					this.OnSetMothershipDataComplete(false);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: exception " + ex.Message);
				this.OnSetMothershipDataComplete(false);
			}
		}

		// Token: 0x0600688F RID: 26767 RVA: 0x0021AD44 File Offset: 0x00218F44
		private void OnSetMothershipUserDataSuccess(SetUserDataResponse response)
		{
			GTDev.Log<string>("SharedBlocksManager OnSetMothershipUserDataSuccess", null);
			this.OnSetMothershipDataComplete(true);
			response.Dispose();
		}

		// Token: 0x06006890 RID: 26768 RVA: 0x0021AD60 File Offset: 0x00218F60
		private void OnSetMothershipUserDataFail(MothershipError error, int status)
		{
			string text = ((error == null) ? status.ToString() : error.Message);
			GTDev.LogError<string>("SharedBlocksManager OnSetMothershipUserDataFail: " + text, null);
			this.OnSetMothershipDataComplete(false);
			if (error != null)
			{
				error.Dispose();
			}
		}

		// Token: 0x06006891 RID: 26769 RVA: 0x0021ADA4 File Offset: 0x00218FA4
		private void OnSetMothershipDataComplete(bool success)
		{
			this.saveScanInProgress = false;
			if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveScanIndex))
			{
				this.currentSaveScanIndex = -1;
				this.currentSaveScanData = string.Empty;
				return;
			}
			if (success)
			{
				this.RequestPublishMap(this.serializationConfig.scanSlotMothershipKeys[this.currentSaveScanIndex]);
				return;
			}
			Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
			if (onSavePrivateScanFailed != null)
			{
				onSavePrivateScanFailed(this.currentSaveScanIndex, "ERROR SAVING");
			}
			this.currentSaveScanIndex = -1;
			this.currentSaveScanData = string.Empty;
		}

		// Token: 0x06006892 RID: 26770 RVA: 0x0021AE26 File Offset: 0x00219026
		public bool TryGetPrivateScanResponse(int scanSlot, out string scanData)
		{
			if (scanSlot < 0 || scanSlot >= this.privateScanDataCache.Length || !this.hasPulledPrivateScanMothership[scanSlot])
			{
				scanData = string.Empty;
				return false;
			}
			scanData = this.privateScanDataCache[scanSlot];
			return true;
		}

		// Token: 0x06006893 RID: 26771 RVA: 0x0021AE58 File Offset: 0x00219058
		public void RequestFetchPrivateScan(int slot)
		{
			if (!BuilderScanKiosk.IsSaveSlotValid(slot))
			{
				GTDev.LogError<string>(string.Format("SharedBlocksManager RequestSaveScan: slot {0} OOB", slot), null);
				slot = Mathf.Clamp(slot, 0, BuilderScanKiosk.NUM_SAVE_SLOTS - 1);
			}
			if (this.hasPulledPrivateScanMothership[slot])
			{
				bool flag = this.privateScanDataCache[slot].Length > 0;
				Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete == null)
				{
					return;
				}
				onFetchPrivateScanComplete(slot, flag);
				return;
			}
			else
			{
				if (this.getScanInProgress)
				{
					Debug.LogError("SharedBlocksManager RequestFetchPrivateScan: request already in progress");
					if (slot != this.currentGetScanIndex)
					{
						Action<int, bool> onFetchPrivateScanComplete2 = this.OnFetchPrivateScanComplete;
						if (onFetchPrivateScanComplete2 == null)
						{
							return;
						}
						onFetchPrivateScanComplete2(slot, false);
					}
					return;
				}
				this.currentGetScanIndex = slot;
				this.getScanInProgress = true;
				try
				{
					if (!MothershipClientApiUnity.GetUserDataValue(this.serializationConfig.scanSlotMothershipKeys[slot], new Action<MothershipUserData>(this.OnGetMothershipPrivateScanSuccess), new Action<MothershipError, int>(this.OnGetMothershipPrivateScanFail), ""))
					{
						Debug.LogError("SharedBlocksManager RequestFetchPrivateScan failed ");
						this.currentGetScanIndex = -1;
						this.getScanInProgress = false;
						Action<int, bool> onFetchPrivateScanComplete3 = this.OnFetchPrivateScanComplete;
						if (onFetchPrivateScanComplete3 != null)
						{
							onFetchPrivateScanComplete3(slot, false);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("SharedBlocksManager RequestFetchPrivateScan exception " + ex.Message);
					this.currentGetScanIndex = -1;
					this.getScanInProgress = false;
					Action<int, bool> onFetchPrivateScanComplete4 = this.OnFetchPrivateScanComplete;
					if (onFetchPrivateScanComplete4 != null)
					{
						onFetchPrivateScanComplete4(slot, false);
					}
				}
				return;
			}
		}

		// Token: 0x06006894 RID: 26772 RVA: 0x0021AFA8 File Offset: 0x002191A8
		private void OnGetMothershipPrivateScanSuccess(MothershipUserData response)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetMothershipPrivateScanSuccess", null);
			bool flag = response != null && response.value != null && response.value.Length > 0;
			int num = this.currentGetScanIndex;
			if (response != null)
			{
				this.privateScanDataCache[this.currentGetScanIndex] = response.value;
				this.hasPulledPrivateScanMothership[this.currentGetScanIndex] = true;
				if (flag)
				{
					SharedBlocksManager.LocalPublishInfo publishInfoForSlot = SharedBlocksManager.GetPublishInfoForSlot(this.currentGetScanIndex);
					if (publishInfoForSlot.mapID != null)
					{
						SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
						{
							MapID = publishInfoForSlot.mapID,
							MapData = this.privateScanDataCache[this.currentGetScanIndex],
							CreatorNickName = GorillaTagger.Instance.offlineVRRig.playerNameVisible,
							UpdateTime = DateTime.Now
						};
						this.AddMapToResponseCache(sharedBlocksMap);
					}
					this.currentGetScanIndex = -1;
					this.getScanInProgress = false;
					Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
					if (onFetchPrivateScanComplete != null)
					{
						onFetchPrivateScanComplete(num, true);
					}
				}
				else
				{
					this.FetchBuildFromPlayfab();
				}
			}
			else
			{
				this.currentGetScanIndex = -1;
				this.getScanInProgress = false;
				Action<int, bool> onFetchPrivateScanComplete2 = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete2 != null)
				{
					onFetchPrivateScanComplete2(num, false);
				}
			}
			if (response != null)
			{
				response.Dispose();
			}
		}

		// Token: 0x06006895 RID: 26773 RVA: 0x0021B0C8 File Offset: 0x002192C8
		private void OnGetMothershipPrivateScanFail(MothershipError error, int status)
		{
			string text = ((error == null) ? status.ToString() : error.Message);
			GTDev.LogError<string>("SharedBlocksManager OnGetMothershipPrivateScanFail: " + text, null);
			int num = this.currentGetScanIndex;
			if (BuilderScanKiosk.IsSaveSlotValid(this.currentGetScanIndex))
			{
				this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
				this.hasPulledPrivateScanMothership[this.currentGetScanIndex] = true;
			}
			this.getScanInProgress = false;
			this.currentGetScanIndex = -1;
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(num, false);
			}
			if (error != null)
			{
				error.Dispose();
			}
		}

		// Token: 0x040077BE RID: 30654
		public static SharedBlocksManager instance;

		// Token: 0x040077C8 RID: 30664
		[SerializeField]
		private BuilderTableSerializationConfig serializationConfig;

		// Token: 0x040077C9 RID: 30665
		private int maxRetriesOnFail = 3;

		// Token: 0x040077CA RID: 30666
		public const int MAP_ID_LENGTH = 8;

		// Token: 0x040077CB RID: 30667
		private const string MAP_ID_PATTERN = "^[CFGHKMNPRTWXZ256789]+$";

		// Token: 0x040077CC RID: 30668
		public const float MINIMUM_REFRESH_DELAY = 60f;

		// Token: 0x040077CD RID: 30669
		public const int VOTE_HISTORY_LENGTH = 10;

		// Token: 0x040077CE RID: 30670
		private const int NUM_CACHED_MAP_RESULTS = 5;

		// Token: 0x040077CF RID: 30671
		private SharedBlocksManager.StartingMapConfig startingMapConfig = new SharedBlocksManager.StartingMapConfig
		{
			pageNumber = 0,
			pageSize = 50,
			sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
			useMapID = false,
			mapID = null
		};

		// Token: 0x040077D0 RID: 30672
		private bool hasQueriedSaveTime;

		// Token: 0x040077D1 RID: 30673
		private static List<string> saveDateKeys = new List<string>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x040077D2 RID: 30674
		private bool fetchedTableConfig;

		// Token: 0x040077D3 RID: 30675
		private int fetchTableConfigRetryCount;

		// Token: 0x040077D4 RID: 30676
		private string tableConfigResponse;

		// Token: 0x040077D5 RID: 30677
		private bool fetchTitleDataBuildInProgress;

		// Token: 0x040077D6 RID: 30678
		private bool fetchTitleDataBuildComplete;

		// Token: 0x040077D7 RID: 30679
		private int fetchTitleDataRetryCount;

		// Token: 0x040077D8 RID: 30680
		private string titleDataBuildCache = string.Empty;

		// Token: 0x040077D9 RID: 30681
		private bool[] hasPulledPrivateScanPlayfab = new bool[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x040077DA RID: 30682
		private int fetchPlayfabBuildsRetryCount;

		// Token: 0x040077DB RID: 30683
		private readonly int publicSlotIndex = BuilderScanKiosk.NUM_SAVE_SLOTS;

		// Token: 0x040077DC RID: 30684
		private string[] privateScanDataCache = new string[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x040077DD RID: 30685
		private bool[] hasPulledPrivateScanMothership = new bool[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x040077DE RID: 30686
		private bool hasPulledDevScan;

		// Token: 0x040077DF RID: 30687
		private string devScanDataCache;

		// Token: 0x040077E0 RID: 30688
		private bool saveScanInProgress;

		// Token: 0x040077E1 RID: 30689
		private int currentSaveScanIndex = -1;

		// Token: 0x040077E2 RID: 30690
		private string currentSaveScanData = string.Empty;

		// Token: 0x040077E3 RID: 30691
		private bool getScanInProgress;

		// Token: 0x040077E4 RID: 30692
		private int currentGetScanIndex = -1;

		// Token: 0x040077E5 RID: 30693
		private int voteRetryCount;

		// Token: 0x040077E6 RID: 30694
		private bool voteInProgress;

		// Token: 0x040077E7 RID: 30695
		private bool publishRequestInProgress;

		// Token: 0x040077E8 RID: 30696
		private int postPublishMapRetryCount;

		// Token: 0x040077E9 RID: 30697
		private bool getMapDataFromIDInProgress;

		// Token: 0x040077EA RID: 30698
		private int getMapDataFromIDRetryCount;

		// Token: 0x040077EB RID: 30699
		private bool getTopMapsInProgress;

		// Token: 0x040077EC RID: 30700
		private int getTopMapsRetryCount;

		// Token: 0x040077ED RID: 30701
		private bool hasCachedTopMaps;

		// Token: 0x040077EE RID: 30702
		private double lastGetTopMapsTime = double.MinValue;

		// Token: 0x040077EF RID: 30703
		private bool updateMapActiveInProgress;

		// Token: 0x040077F0 RID: 30704
		private int updateMapActiveRetryCount;

		// Token: 0x040077F1 RID: 30705
		private List<SharedBlocksManager.SharedBlocksMap> latestPopularMaps = new List<SharedBlocksManager.SharedBlocksMap>();

		// Token: 0x040077F2 RID: 30706
		private static LinkedList<string> recentUpVotes = new LinkedList<string>();

		// Token: 0x040077F3 RID: 30707
		private static Dictionary<int, SharedBlocksManager.LocalPublishInfo> localPublishData = new Dictionary<int, SharedBlocksManager.LocalPublishInfo>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x040077F4 RID: 30708
		private static List<string> localMapIds = new List<string>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x040077F5 RID: 30709
		private List<SharedBlocksManager.SharedBlocksMap> mapResponseCache = new List<SharedBlocksManager.SharedBlocksMap>(5);

		// Token: 0x040077F6 RID: 30710
		private SharedBlocksManager.SharedBlocksMap defaultMap;

		// Token: 0x040077F7 RID: 30711
		private bool hasDefaultMap;

		// Token: 0x040077F8 RID: 30712
		private double defaultMapCacheTime = double.MinValue;

		// Token: 0x040077F9 RID: 30713
		private bool getDefaultMapInProgress;

		// Token: 0x02001055 RID: 4181
		[Serializable]
		public class SharedBlocksMap
		{
			// Token: 0x170009E8 RID: 2536
			// (get) Token: 0x06006898 RID: 26776 RVA: 0x0021B285 File Offset: 0x00219485
			// (set) Token: 0x06006899 RID: 26777 RVA: 0x0021B28D File Offset: 0x0021948D
			public string MapID { get; set; }

			// Token: 0x170009E9 RID: 2537
			// (get) Token: 0x0600689A RID: 26778 RVA: 0x0021B296 File Offset: 0x00219496
			// (set) Token: 0x0600689B RID: 26779 RVA: 0x0021B29E File Offset: 0x0021949E
			public string CreatorID { get; set; }

			// Token: 0x170009EA RID: 2538
			// (get) Token: 0x0600689C RID: 26780 RVA: 0x0021B2A7 File Offset: 0x002194A7
			// (set) Token: 0x0600689D RID: 26781 RVA: 0x0021B2AF File Offset: 0x002194AF
			public string CreatorNickName { get; set; }

			// Token: 0x170009EB RID: 2539
			// (get) Token: 0x0600689E RID: 26782 RVA: 0x0021B2B8 File Offset: 0x002194B8
			// (set) Token: 0x0600689F RID: 26783 RVA: 0x0021B2C0 File Offset: 0x002194C0
			public DateTime CreateTime { get; set; }

			// Token: 0x170009EC RID: 2540
			// (get) Token: 0x060068A0 RID: 26784 RVA: 0x0021B2C9 File Offset: 0x002194C9
			// (set) Token: 0x060068A1 RID: 26785 RVA: 0x0021B2D1 File Offset: 0x002194D1
			public DateTime UpdateTime { get; set; }

			// Token: 0x170009ED RID: 2541
			// (get) Token: 0x060068A2 RID: 26786 RVA: 0x0021B2DA File Offset: 0x002194DA
			// (set) Token: 0x060068A3 RID: 26787 RVA: 0x0021B2E2 File Offset: 0x002194E2
			public string MapData { get; set; }
		}

		// Token: 0x02001056 RID: 4182
		[Serializable]
		public struct LocalPublishInfo
		{
			// Token: 0x04007800 RID: 30720
			public string mapID;

			// Token: 0x04007801 RID: 30721
			public long publishTime;
		}

		// Token: 0x02001057 RID: 4183
		[Serializable]
		private class SharedBlocksRequestBase
		{
			// Token: 0x04007802 RID: 30722
			public string mothershipId;

			// Token: 0x04007803 RID: 30723
			public string mothershipToken;

			// Token: 0x04007804 RID: 30724
			public string mothershipEnvId;
		}

		// Token: 0x02001058 RID: 4184
		[Serializable]
		private class VoteRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04007805 RID: 30725
			public string mapId;

			// Token: 0x04007806 RID: 30726
			public int vote;
		}

		// Token: 0x02001059 RID: 4185
		[Serializable]
		private class PublishMapRequestData : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04007807 RID: 30727
			public string userdataMetadataKey;

			// Token: 0x04007808 RID: 30728
			public string playerNickname;
		}

		// Token: 0x0200105A RID: 4186
		public enum MapSortMethod
		{
			// Token: 0x0400780A RID: 30730
			Top,
			// Token: 0x0400780B RID: 30731
			NewlyCreated,
			// Token: 0x0400780C RID: 30732
			RecentlyUpdated
		}

		// Token: 0x0200105B RID: 4187
		public struct StartingMapConfig
		{
			// Token: 0x0400780D RID: 30733
			public int pageNumber;

			// Token: 0x0400780E RID: 30734
			public int pageSize;

			// Token: 0x0400780F RID: 30735
			public string sortMethod;

			// Token: 0x04007810 RID: 30736
			public bool useMapID;

			// Token: 0x04007811 RID: 30737
			public string mapID;
		}

		// Token: 0x0200105C RID: 4188
		[Serializable]
		private class GetMapsRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04007812 RID: 30738
			public int page;

			// Token: 0x04007813 RID: 30739
			public int pageSize;

			// Token: 0x04007814 RID: 30740
			public string sort;

			// Token: 0x04007815 RID: 30741
			public bool ShowInactive;
		}

		// Token: 0x0200105D RID: 4189
		[Serializable]
		private class GetMapDataFromIDRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04007816 RID: 30742
			public string mapId;
		}

		// Token: 0x0200105E RID: 4190
		[Serializable]
		private class GetMapIDFromPlayerRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04007817 RID: 30743
			public string requestId;

			// Token: 0x04007818 RID: 30744
			public string requestUserDataMetaKey;
		}

		// Token: 0x0200105F RID: 4191
		[Serializable]
		private class GetMapIDFromPlayerResponse
		{
			// Token: 0x04007819 RID: 30745
			public SharedBlocksManager.SharedBlocksMapMetaData result;

			// Token: 0x0400781A RID: 30746
			public int statusCode;

			// Token: 0x0400781B RID: 30747
			public string error;
		}

		// Token: 0x02001060 RID: 4192
		[Serializable]
		private class SharedBlocksMapMetaData
		{
			// Token: 0x0400781C RID: 30748
			public string mapId;

			// Token: 0x0400781D RID: 30749
			public string mothershipId;

			// Token: 0x0400781E RID: 30750
			public string userDataMetadataKey;

			// Token: 0x0400781F RID: 30751
			public string nickname;

			// Token: 0x04007820 RID: 30752
			public string createdTime;

			// Token: 0x04007821 RID: 30753
			public string updatedTime;

			// Token: 0x04007822 RID: 30754
			public int voteCount;

			// Token: 0x04007823 RID: 30755
			public bool isActive;
		}

		// Token: 0x02001061 RID: 4193
		[Serializable]
		private struct GetMapDataFromPlayerRequestData
		{
			// Token: 0x04007824 RID: 30756
			public string CreatorID;

			// Token: 0x04007825 RID: 30757
			public string MapScan;

			// Token: 0x04007826 RID: 30758
			public SharedBlocksManager.BlocksMapRequestCallback Callback;
		}

		// Token: 0x02001062 RID: 4194
		[Serializable]
		private class UpdateMapActiveRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04007827 RID: 30759
			public string userdataMetadataKey;

			// Token: 0x04007828 RID: 30760
			public bool setActive;
		}

		// Token: 0x02001063 RID: 4195
		// (Invoke) Token: 0x060068AF RID: 26799
		public delegate void PublishMapRequestCallback(bool success, string key, string mapID, long responseCode);

		// Token: 0x02001064 RID: 4196
		// (Invoke) Token: 0x060068B3 RID: 26803
		public delegate void BlocksMapRequestCallback(SharedBlocksManager.SharedBlocksMap response);
	}
}
