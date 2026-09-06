using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GorillaExtensions;
using GorillaUtil;
using LitJson;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking
{
	// Token: 0x02001120 RID: 4384
	public class PlayFabTitleDataCache : MonoBehaviour
	{
		// Token: 0x17000A75 RID: 2677
		// (get) Token: 0x06006DE9 RID: 28137 RVA: 0x00237A4A File Offset: 0x00235C4A
		// (set) Token: 0x06006DEA RID: 28138 RVA: 0x00237A51 File Offset: 0x00235C51
		public static PlayFabTitleDataCache Instance { get; private set; }

		// Token: 0x17000A76 RID: 2678
		// (get) Token: 0x06006DEB RID: 28139 RVA: 0x00237A59 File Offset: 0x00235C59
		private static string FilePath
		{
			get
			{
				return Path.Combine(Application.persistentDataPath, "TitleDataCache.json");
			}
		}

		// Token: 0x06006DEC RID: 28140 RVA: 0x00237A6C File Offset: 0x00235C6C
		public void GetTitleData(string name, Action<string> callback, Action<PlayFabError> errorCallback, bool ignoreCache = false)
		{
			Dictionary<string, string> dictionary;
			string text;
			if (ignoreCache || this.isFirstLoad || !this.localizedTitleData.TryGetValue(LocalisationManager.CurrentLanguage.Identifier.Code, out dictionary) || !dictionary.TryGetValue(name, out text))
			{
				PlayFabTitleDataCache.DataRequest dataRequest = new PlayFabTitleDataCache.DataRequest
				{
					Name = name,
					Callback = callback,
					ErrorCallback = errorCallback
				};
				this.requests.Add(dataRequest);
				this.TryUpdateData();
				return;
			}
			callback.SafeInvoke(text);
			Action<string, string> onCachedValueRetieved = PlayFabTitleDataCache.OnCachedValueRetieved;
			if (onCachedValueRetieved == null)
			{
				return;
			}
			onCachedValueRetieved(name, text);
		}

		// Token: 0x06006DED RID: 28141 RVA: 0x00237AF7 File Offset: 0x00235CF7
		private void Awake()
		{
			if (PlayFabTitleDataCache.Instance != null)
			{
				Object.Destroy(this);
				return;
			}
			PlayFabTitleDataCache.Instance = this;
			Action<PlayFabTitleDataCache> action = PlayFabTitleDataCache.k_onnLoaded;
			if (action != null)
			{
				action(this);
			}
			PlayFabTitleDataCache.k_onnLoaded = null;
		}

		// Token: 0x06006DEE RID: 28142 RVA: 0x00237B2A File Offset: 0x00235D2A
		private void Start()
		{
			this.UpdateData();
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.TryUpdateData));
		}

		// Token: 0x06006DEF RID: 28143 RVA: 0x00237B43 File Offset: 0x00235D43
		private void OnDestroy()
		{
			LocalisationManager.UnregisterOnLanguageChanged(new Action(this.TryUpdateData));
		}

		// Token: 0x06006DF0 RID: 28144 RVA: 0x00237B56 File Offset: 0x00235D56
		private void TryUpdateData()
		{
			if (!this.isFirstLoad && this.updateDataCoroutine == null)
			{
				this.UpdateData();
			}
		}

		// Token: 0x06006DF1 RID: 28145 RVA: 0x00237B70 File Offset: 0x00235D70
		public CacheImport LoadDataFromFile()
		{
			CacheImport cacheImport;
			try
			{
				if (!File.Exists(PlayFabTitleDataCache.FilePath))
				{
					Debug.LogWarning("[PlayFabTitleDataCache::LoadDataFromFile] Title data file " + PlayFabTitleDataCache.FilePath + " does not exist!");
					cacheImport = null;
				}
				else
				{
					cacheImport = JsonMapper.ToObject<CacheImport>(File.ReadAllText(PlayFabTitleDataCache.FilePath)) ?? new CacheImport();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[PlayFabTitleDataCache::LoadDataFromFile] Error reading PlayFab title data from file: {0}", ex));
				cacheImport = null;
			}
			return cacheImport;
		}

		// Token: 0x06006DF2 RID: 28146 RVA: 0x00237BE8 File Offset: 0x00235DE8
		private static void SaveDataToFile(string filepath, Dictionary<string, Dictionary<string, string>> titleData)
		{
			try
			{
				string text = JsonMapper.ToJson(new CacheImport
				{
					DeploymentId = MothershipClientApiUnity.DeploymentId,
					TitleData = titleData
				});
				File.WriteAllText(filepath, text);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[PlayFabTitleDataCache::SaveDataToFile] Error writing PlayFab title data to file: {0}", ex));
			}
		}

		// Token: 0x06006DF3 RID: 28147 RVA: 0x00237C40 File Offset: 0x00235E40
		public void UpdateData()
		{
			this.updateDataCoroutine = base.StartCoroutine(this.UpdateDataCo());
		}

		// Token: 0x06006DF4 RID: 28148 RVA: 0x00237C54 File Offset: 0x00235E54
		private IEnumerator UpdateDataCo()
		{
			try
			{
				PlayFabTitleDataCache.<>c__DisplayClass27_0 CS$<>8__locals1 = new PlayFabTitleDataCache.<>c__DisplayClass27_0();
				CacheImport oldCache = this.LoadDataFromFile();
				CS$<>8__locals1.currentLocale = LocalisationManager.CurrentLanguage.Identifier.Code;
				Dictionary<string, string> titleData;
				if (!this.localizedTitleData.TryGetValue(CS$<>8__locals1.currentLocale, out titleData))
				{
					this.localizedTitleData[CS$<>8__locals1.currentLocale] = new Dictionary<string, string>();
					titleData = this.localizedTitleData[CS$<>8__locals1.currentLocale];
				}
				Dictionary<string, string> oldLocalizedCache;
				if (oldCache == null || oldCache.TitleData == null || !oldCache.TitleData.TryGetValue(CS$<>8__locals1.currentLocale, out oldLocalizedCache))
				{
					oldLocalizedCache = new Dictionary<string, string>();
				}
				yield return new WaitUntil(() => MothershipClientApiUnity.IsClientLoggedIn());
				bool wipeOldData = oldCache == null || oldCache.DeploymentId != MothershipClientApiUnity.DeploymentId;
				CS$<>8__locals1.newTitleData = null;
				CS$<>8__locals1.mothershipError = null;
				Stopwatch.StartNew();
				StringVector stringVector = new StringVector();
				if (!this.isFirstLoad)
				{
					foreach (PlayFabTitleDataCache.DataRequest dataRequest in this.requests)
					{
						stringVector.Add(dataRequest.Name);
					}
				}
				CS$<>8__locals1.finished = false;
				if (!MothershipClientApiUnity.ListMothershipTitleData(MothershipClientApiUnity.TitleId, MothershipClientApiUnity.EnvironmentId, MothershipClientApiUnity.DeploymentId, stringVector, delegate(ListClientMothershipTitleDataResponse response)
				{
					if (response != null && response.Results != null)
					{
						CS$<>8__locals1.newTitleData = new Dictionary<string, string>();
						for (int j = 0; j < response.Results.Count; j++)
						{
							MothershipTitleDataShort mothershipTitleDataShort = response.Results[j];
							if (!string.IsNullOrEmpty(mothershipTitleDataShort.key))
							{
								if (mothershipTitleDataShort.data.Contains("#EN_FALLBACK="))
								{
									Debug.LogWarning(string.Concat(new string[] { "[PlayFabTitleDataCache::UpdateDataCo] Key '", mothershipTitleDataShort.key, "' exists, but it doesn't have a translation for locale '", CS$<>8__locals1.currentLocale, "'. Falling back to English." }));
									mothershipTitleDataShort.data = mothershipTitleDataShort.data.Split("#EN_FALLBACK=", StringSplitOptions.None)[1];
								}
								CS$<>8__locals1.newTitleData[mothershipTitleDataShort.key] = mothershipTitleDataShort.data;
							}
						}
						CS$<>8__locals1.mothershipError = null;
					}
					else
					{
						CS$<>8__locals1.mothershipError = "Failed to fetch title data - response or results were null";
						Debug.LogError("[PlayFabTitleDataCache::UpdateDataCo] " + CS$<>8__locals1.mothershipError);
					}
					CS$<>8__locals1.finished = true;
				}, delegate(MothershipError error, int statusCode)
				{
					CS$<>8__locals1.mothershipError = string.Format("Error fetching title data: {0} (Status: {1})", ((error != null) ? error.Message : null) ?? "Unknown error", statusCode);
					Debug.LogError("[PlayFabTitleDataCache::UpdateDataCo] Mothership API error callback - " + CS$<>8__locals1.mothershipError);
					CS$<>8__locals1.finished = true;
				}))
				{
					CS$<>8__locals1.mothershipError = "Mothership API call was not sent.";
					Debug.LogError("[PlayFabTitleDataCache::UpdateDataCo] " + CS$<>8__locals1.mothershipError);
				}
				yield return new WaitUntil(() => CS$<>8__locals1.finished);
				if (CS$<>8__locals1.newTitleData != null)
				{
					if (wipeOldData)
					{
						this.localizedTitleData.Clear();
						this.localizedTitleData[CS$<>8__locals1.currentLocale] = new Dictionary<string, string>();
						titleData = this.localizedTitleData[CS$<>8__locals1.currentLocale];
					}
					if (!this.localesUpdated.ContainsKey(CS$<>8__locals1.currentLocale))
					{
						titleData.Clear();
					}
					foreach (KeyValuePair<string, string> keyValuePair in CS$<>8__locals1.newTitleData)
					{
						string text;
						string text2;
						keyValuePair.Deconstruct(out text, out text2);
						string text3 = text;
						string text4 = text2;
						titleData[text3] = text4;
						for (int i = this.requests.Count - 1; i >= 0; i--)
						{
							PlayFabTitleDataCache.DataRequest dataRequest2 = this.requests[i];
							if (dataRequest2.Name == text3)
							{
								try
								{
									Action<string> callback = dataRequest2.Callback;
									if (callback != null)
									{
										callback(text4);
									}
									Action<string, string> onValueRetieved = PlayFabTitleDataCache.OnValueRetieved;
									if (onValueRetieved != null)
									{
										onValueRetieved(text3, text4);
									}
								}
								catch (Exception ex)
								{
									Debug.LogError(string.Concat(new string[] { "[PlayFabTitleDataCache::UpdateDataCo] Error running callback for key: '", text3, "' value: '", text4, "' exception: ", ex.Message }));
								}
								this.requests.RemoveAt(i);
							}
						}
						string text5;
						if (oldLocalizedCache.TryGetValue(text3, out text5) && text5 != text4)
						{
							PlayFabTitleDataCache.DataUpdate onTitleDataUpdate = this.OnTitleDataUpdate;
							if (onTitleDataUpdate != null)
							{
								onTitleDataUpdate.Invoke(text3);
							}
						}
					}
					this.localesUpdated[CS$<>8__locals1.currentLocale] = true;
					PlayFabTitleDataCache.SaveDataToFile(PlayFabTitleDataCache.FilePath, this.localizedTitleData);
				}
				CS$<>8__locals1 = null;
				oldCache = null;
				titleData = null;
				oldLocalizedCache = null;
			}
			finally
			{
				this.ClearRequestWithError(null);
				this.isFirstLoad = false;
				this.updateDataCoroutine = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06006DF5 RID: 28149 RVA: 0x00237C64 File Offset: 0x00235E64
		private void ClearRequestWithError(PlayFabError e = null)
		{
			if (e == null)
			{
				e = new PlayFabError
				{
					ErrorMessage = "PlayFabError was null. Maybe an exception was encountered."
				};
			}
			foreach (PlayFabTitleDataCache.DataRequest dataRequest in this.requests)
			{
				dataRequest.ErrorCallback.SafeInvoke(e);
			}
			this.requests.Clear();
		}

		// Token: 0x06006DF6 RID: 28150 RVA: 0x00237CDC File Offset: 0x00235EDC
		public static void RegisterOnLoad(Action<PlayFabTitleDataCache> callback)
		{
			if (PlayFabTitleDataCache.Instance.IsNotNull())
			{
				callback(PlayFabTitleDataCache.Instance);
				return;
			}
			PlayFabTitleDataCache.k_onnLoaded = (Action<PlayFabTitleDataCache>)Delegate.Combine(PlayFabTitleDataCache.k_onnLoaded, callback);
		}

		// Token: 0x04007E40 RID: 32320
		private static Action<PlayFabTitleDataCache> k_onnLoaded;

		// Token: 0x04007E41 RID: 32321
		public static Action<string, string> OnValueRetieved;

		// Token: 0x04007E42 RID: 32322
		public static Action<string, string> OnCachedValueRetieved;

		// Token: 0x04007E43 RID: 32323
		public PlayFabTitleDataCache.DataUpdate OnTitleDataUpdate;

		// Token: 0x04007E44 RID: 32324
		private const string FileName = "TitleDataCache.json";

		// Token: 0x04007E45 RID: 32325
		private readonly List<PlayFabTitleDataCache.DataRequest> requests = new List<PlayFabTitleDataCache.DataRequest>();

		// Token: 0x04007E46 RID: 32326
		private Dictionary<string, Dictionary<string, string>> localizedTitleData = new Dictionary<string, Dictionary<string, string>>();

		// Token: 0x04007E47 RID: 32327
		private Dictionary<string, bool> localesUpdated = new Dictionary<string, bool>();

		// Token: 0x04007E48 RID: 32328
		private bool isFirstLoad = true;

		// Token: 0x04007E49 RID: 32329
		private Coroutine updateDataCoroutine;

		// Token: 0x04007E4A RID: 32330
		[SerializeField]
		private StringTable betaTitleDataOveride;

		// Token: 0x02001121 RID: 4385
		[Serializable]
		public sealed class DataUpdate : UnityEvent<string>
		{
		}

		// Token: 0x02001122 RID: 4386
		private class DataRequest
		{
			// Token: 0x17000A77 RID: 2679
			// (get) Token: 0x06006DF9 RID: 28153 RVA: 0x00237D43 File Offset: 0x00235F43
			// (set) Token: 0x06006DFA RID: 28154 RVA: 0x00237D4B File Offset: 0x00235F4B
			public string Name { get; set; }

			// Token: 0x17000A78 RID: 2680
			// (get) Token: 0x06006DFB RID: 28155 RVA: 0x00237D54 File Offset: 0x00235F54
			// (set) Token: 0x06006DFC RID: 28156 RVA: 0x00237D5C File Offset: 0x00235F5C
			public Action<string> Callback { get; set; }

			// Token: 0x17000A79 RID: 2681
			// (get) Token: 0x06006DFD RID: 28157 RVA: 0x00237D65 File Offset: 0x00235F65
			// (set) Token: 0x06006DFE RID: 28158 RVA: 0x00237D6D File Offset: 0x00235F6D
			public Action<PlayFabError> ErrorCallback { get; set; }
		}
	}
}
