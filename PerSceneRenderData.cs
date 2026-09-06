using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000362 RID: 866
public class PerSceneRenderData : MonoBehaviour
{
	// Token: 0x06001532 RID: 5426 RVA: 0x00070C40 File Offset: 0x0006EE40
	private void RefreshRenderer()
	{
		int sceneIndex = this.sceneIndex;
		new List<Renderer>();
		foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
		{
			if (renderer.gameObject.scene.buildIndex == sceneIndex)
			{
				this.representativeRenderer = renderer;
				return;
			}
		}
	}

	// Token: 0x1700021B RID: 539
	// (get) Token: 0x06001533 RID: 5427 RVA: 0x00070C94 File Offset: 0x0006EE94
	public string sceneName
	{
		get
		{
			return base.gameObject.scene.name;
		}
	}

	// Token: 0x1700021C RID: 540
	// (get) Token: 0x06001534 RID: 5428 RVA: 0x00070CB4 File Offset: 0x0006EEB4
	public int sceneIndex
	{
		get
		{
			return base.gameObject.scene.buildIndex;
		}
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x00070CD4 File Offset: 0x0006EED4
	private void Awake()
	{
		for (int i = 0; i < this.mRendererIndex; i++)
		{
			this.mRenderers[i] = this.gO[i].GetComponent<MeshRenderer>();
		}
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x00070D07 File Offset: 0x0006EF07
	private void OnEnable()
	{
		BetterDayNightManager.Register(this);
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x00070D0F File Offset: 0x0006EF0F
	private void OnDisable()
	{
		BetterDayNightManager.Unregister(this);
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x00070D18 File Offset: 0x0006EF18
	public void AddMeshToList(GameObject _gO, MeshRenderer mR)
	{
		try
		{
			if (mR.lightmapIndex != -1)
			{
				this.gO[this.mRendererIndex] = _gO;
				this.mRendererIndex++;
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x00070D64 File Offset: 0x0006EF64
	public bool CheckShouldRepopulate()
	{
		return this.representativeRenderer.lightmapIndex != this.lastLightmapIndex;
	}

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x0600153A RID: 5434 RVA: 0x00070D7C File Offset: 0x0006EF7C
	public bool IsLoadingLightmaps
	{
		get
		{
			return this.resourceRequests.Count != 0;
		}
	}

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x0600153B RID: 5435 RVA: 0x00070D8C File Offset: 0x0006EF8C
	public int LoadingLightmapsCount
	{
		get
		{
			return this.resourceRequests.Count;
		}
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x00070D9C File Offset: 0x0006EF9C
	private Texture2D GetLightmap(string timeOfDay)
	{
		if (this.singleLightmap != null)
		{
			return this.singleLightmap;
		}
		Texture2D texture2D;
		if (!this.lightmapsCache.TryGetValue(timeOfDay, out texture2D))
		{
			ResourceRequest request;
			if (this.resourceRequests.TryGetValue(timeOfDay, out request))
			{
				return null;
			}
			request = Resources.LoadAsync<Texture2D>(Path.Combine(this.lightmapsResourcePath, timeOfDay));
			this.resourceRequests.Add(timeOfDay, request);
			request.completed += delegate(AsyncOperation ao)
			{
				if (this == null)
				{
					return;
				}
				this.lightmapsCache.Add(timeOfDay, (Texture2D)request.asset);
				this.resourceRequests.Remove(timeOfDay);
				if (BetterDayNightManager.instance != null)
				{
					BetterDayNightManager.instance.RequestRepopulateLightmaps();
				}
			};
		}
		return texture2D;
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x00070E50 File Offset: 0x0006F050
	public void PopulateLightmaps(string fromTimeOfDay, string toTimeOfDay, LightmapData[] lightmaps)
	{
		LightmapData lightmapData = new LightmapData();
		lightmapData.lightmapColor = this.GetLightmap(fromTimeOfDay);
		lightmapData.lightmapDir = this.GetLightmap(toTimeOfDay);
		if (this.representativeRenderer == null)
		{
			this.RefreshRenderer();
		}
		if (this.representativeRenderer == null)
		{
			return;
		}
		if (lightmapData.lightmapColor != null && lightmapData.lightmapDir != null && this.representativeRenderer.lightmapIndex >= 0 && this.representativeRenderer.lightmapIndex < lightmaps.Length)
		{
			lightmaps[this.representativeRenderer.lightmapIndex] = lightmapData;
		}
		this.lastLightmapIndex = this.representativeRenderer.lightmapIndex;
		for (int i = 0; i < this.mRendererIndex; i++)
		{
			if (i < this.mRenderers.Length && this.gO[i] != null)
			{
				if (this.mRenderers[i] == null)
				{
					this.mRenderers[i] = this.gO[i].GetComponent<MeshRenderer>();
				}
				if (this.mRenderers[i] == null)
				{
					this.gO[i] = null;
				}
				else
				{
					this.mRenderers[i].lightmapIndex = this.lastLightmapIndex;
				}
			}
		}
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x00070F78 File Offset: 0x0006F178
	public void ReleaseLightmap(string oldTimeOfDay)
	{
		Texture2D texture2D;
		if (this.lightmapsCache.Remove(oldTimeOfDay, out texture2D))
		{
			Resources.UnloadAsset(texture2D);
		}
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x00070F9C File Offset: 0x0006F19C
	private void TryGetLightmapOrAsyncLoad(string momentName, Action<Texture2D> callback)
	{
		if (this.singleLightmap != null)
		{
			callback(this.singleLightmap);
		}
		Texture2D texture2D;
		if (this.lightmapsCache.TryGetValue(momentName, out texture2D))
		{
			callback(texture2D);
		}
		List<Action<Texture2D>> callbacks;
		if (!this._momentName_to_callbacks.TryGetValue(momentName, out callbacks))
		{
			callbacks = new List<Action<Texture2D>>(8);
			this._momentName_to_callbacks[momentName] = callbacks;
		}
		if (!callbacks.Contains(callback))
		{
			callbacks.Add(callback);
		}
		ResourceRequest request;
		if (this.resourceRequests.TryGetValue(momentName, out request))
		{
			return;
		}
		request = Resources.LoadAsync<Texture2D>(Path.Combine(this.lightmapsResourcePath, momentName));
		this.resourceRequests.Add(momentName, request);
		request.completed += delegate(AsyncOperation ao)
		{
			if (this == null || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			Texture2D texture2D2 = (Texture2D)request.asset;
			this.lightmapsCache.Add(momentName, texture2D2);
			this.resourceRequests.Remove(momentName);
			foreach (Action<Texture2D> action in callbacks)
			{
				if (action != null)
				{
					action(texture2D2);
				}
			}
			callbacks.Clear();
		};
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x000710B0 File Offset: 0x0006F2B0
	public bool IsLightmapWithNameLoaded(string lightmapName)
	{
		if (this.singleLightmap != null)
		{
			return true;
		}
		string text;
		string text2;
		this.GetFromAndToLightmapNames(out text, out text2);
		return !string.IsNullOrEmpty(lightmapName) && ((!string.IsNullOrEmpty(text) && text == lightmapName) || (!string.IsNullOrEmpty(text2) && text2 == lightmapName));
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x00071108 File Offset: 0x0006F308
	public bool IsLightmapsWithNamesLoaded(string fromLightmapName, string toLightmapName)
	{
		if (this.singleLightmap != null)
		{
			return true;
		}
		string text;
		string text2;
		this.GetFromAndToLightmapNames(out text, out text2);
		return !string.IsNullOrEmpty(fromLightmapName) && !string.IsNullOrEmpty(toLightmapName) && !string.IsNullOrEmpty(text) && text == fromLightmapName && !string.IsNullOrEmpty(text2) && text2 == toLightmapName;
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x00071164 File Offset: 0x0006F364
	public void GetFromAndToLightmapNames(out string fromLightmapName, out string toLightmapName)
	{
		if (this.singleLightmap != null)
		{
			fromLightmapName = null;
			toLightmapName = null;
			return;
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		if (this.representativeRenderer.lightmapIndex < 0 || this.representativeRenderer.lightmapIndex >= lightmaps.Length)
		{
			fromLightmapName = null;
			toLightmapName = null;
			return;
		}
		Texture2D lightmapColor = lightmaps[this.representativeRenderer.lightmapIndex].lightmapColor;
		Texture2D lightmapDir = lightmaps[this.representativeRenderer.lightmapIndex].lightmapDir;
		fromLightmapName = ((lightmapColor != null) ? lightmapColor.name : null);
		toLightmapName = ((lightmapDir != null) ? lightmapDir.name : null);
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x00071200 File Offset: 0x0006F400
	public static void g_StartAllScenesPopulateLightmaps(string fromLightmapName, string toLightmapName)
	{
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Clear();
		PerSceneRenderData[] array = Object.FindObjectsByType<PerSceneRenderData>(FindObjectsSortMode.None);
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.UnionWith(array);
		foreach (PerSceneRenderData perSceneRenderData in array)
		{
			perSceneRenderData.StartPopulateLightmaps(fromLightmapName, toLightmapName);
			perSceneRenderData.OnPopulateToAndFromLightmapsCompleted = (Action<PerSceneRenderData>)Delegate.Combine(perSceneRenderData.OnPopulateToAndFromLightmapsCompleted, new Action<PerSceneRenderData>(PerSceneRenderData._g_AllScenesPopulateLightmaps_OnOneCompleted));
		}
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x00071268 File Offset: 0x0006F468
	private static void _g_AllScenesPopulateLightmaps_OnOneCompleted(PerSceneRenderData perSceneRenderData)
	{
		int count = PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Remove(perSceneRenderData);
		int count2 = PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		if (count2 == 0 && count2 != count)
		{
			Action action = PerSceneRenderData.g_OnAllScenesPopulateLightmapsCompleted;
			if (action == null)
			{
				return;
			}
			action();
		}
	}

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06001545 RID: 5445 RVA: 0x000712AD File Offset: 0x0006F4AD
	public static int g_AllScenesPopulatingLightmapsLoadCount
	{
		get
		{
			return PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		}
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x000712BC File Offset: 0x0006F4BC
	public void StartPopulateLightmaps(string fromMomentName, string toMomentName)
	{
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Clear();
		this._populateLightmaps_fromMomentLightmap = null;
		this._populateLightmaps_toMomentLightmap = null;
		this._populateLightmaps_fromMomentName = fromMomentName;
		this._populateLightmaps_toMomentName = toMomentName;
		this.TryGetLightmapOrAsyncLoad(fromMomentName, new Action<Texture2D>(this._PopulateLightmaps_OnLoadLightmap));
		this.TryGetLightmapOrAsyncLoad(toMomentName, new Action<Texture2D>(this._PopulateLightmaps_OnLoadLightmap));
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x00071318 File Offset: 0x0006F518
	private void _PopulateLightmaps_OnLoadLightmap(Texture2D lightmapTex)
	{
		if (this == null || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this._populateLightmaps_fromMomentName != lightmapTex.name)
		{
			this._populateLightmaps_fromMomentLightmap = lightmapTex;
		}
		if (this._populateLightmaps_toMomentName != lightmapTex.name)
		{
			this._populateLightmaps_toMomentLightmap = lightmapTex;
		}
		if (this._populateLightmaps_fromMomentLightmap != null && this._populateLightmaps_toMomentLightmap != null)
		{
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			LightmapData lightmapData = new LightmapData
			{
				lightmapColor = this._populateLightmaps_fromMomentLightmap,
				lightmapDir = this._populateLightmaps_toMomentLightmap
			};
			if (this.representativeRenderer.lightmapIndex >= 0 && this.representativeRenderer.lightmapIndex < lightmaps.Length)
			{
				lightmaps[this.representativeRenderer.lightmapIndex] = lightmapData;
			}
			LightmapSettings.lightmaps = lightmaps;
			this.lastLightmapIndex = this.representativeRenderer.lightmapIndex;
			for (int i = 0; i < this.mRendererIndex; i++)
			{
				if (i < this.mRenderers.Length && this.mRenderers[i] != null)
				{
					this.mRenderers[i].lightmapIndex = this.lastLightmapIndex;
				}
			}
			Action<PerSceneRenderData> onPopulateToAndFromLightmapsCompleted = this.OnPopulateToAndFromLightmapsCompleted;
			if (onPopulateToAndFromLightmapsCompleted == null)
			{
				return;
			}
			onPopulateToAndFromLightmapsCompleted(this);
		}
	}

	// Token: 0x04001A15 RID: 6677
	public Renderer representativeRenderer;

	// Token: 0x04001A16 RID: 6678
	public string lightmapsResourcePath;

	// Token: 0x04001A17 RID: 6679
	public Texture2D singleLightmap;

	// Token: 0x04001A18 RID: 6680
	private int lastLightmapIndex = -1;

	// Token: 0x04001A19 RID: 6681
	public GameObject[] gO = new GameObject[5000];

	// Token: 0x04001A1A RID: 6682
	public MeshRenderer[] mRenderers = new MeshRenderer[5000];

	// Token: 0x04001A1B RID: 6683
	public int mRendererIndex;

	// Token: 0x04001A1C RID: 6684
	private readonly Dictionary<string, ResourceRequest> resourceRequests = new Dictionary<string, ResourceRequest>(8);

	// Token: 0x04001A1D RID: 6685
	private readonly Dictionary<string, Texture2D> lightmapsCache = new Dictionary<string, Texture2D>(8);

	// Token: 0x04001A1E RID: 6686
	private Dictionary<string, List<Action<Texture2D>>> _momentName_to_callbacks = new Dictionary<string, List<Action<Texture2D>>>(8);

	// Token: 0x04001A1F RID: 6687
	private static readonly HashSet<PerSceneRenderData> _g_allScenesPopulateLightmaps_renderDatasHashSet = new HashSet<PerSceneRenderData>(32);

	// Token: 0x04001A20 RID: 6688
	public static Action g_OnAllScenesPopulateLightmapsCompleted;

	// Token: 0x04001A21 RID: 6689
	private string _populateLightmaps_fromMomentName;

	// Token: 0x04001A22 RID: 6690
	private string _populateLightmaps_toMomentName;

	// Token: 0x04001A23 RID: 6691
	private Texture2D _populateLightmaps_fromMomentLightmap;

	// Token: 0x04001A24 RID: 6692
	private Texture2D _populateLightmaps_toMomentLightmap;

	// Token: 0x04001A25 RID: 6693
	public Action<PerSceneRenderData> OnPopulateToAndFromLightmapsCompleted;
}
