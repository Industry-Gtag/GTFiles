using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020003BB RID: 955
public class ZoneManagement : MonoBehaviour
{
	// Token: 0x14000031 RID: 49
	// (add) Token: 0x060016FC RID: 5884 RVA: 0x00085B1C File Offset: 0x00083D1C
	// (remove) Token: 0x060016FD RID: 5885 RVA: 0x00085B50 File Offset: 0x00083D50
	public static event ZoneManagement.ZoneChangeEvent OnZoneChange;

	// Token: 0x1700023C RID: 572
	// (get) Token: 0x060016FE RID: 5886 RVA: 0x00085B83 File Offset: 0x00083D83
	// (set) Token: 0x060016FF RID: 5887 RVA: 0x00085B8B File Offset: 0x00083D8B
	public bool hasInstance { get; private set; }

	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06001700 RID: 5888 RVA: 0x00085B94 File Offset: 0x00083D94
	// (set) Token: 0x06001701 RID: 5889 RVA: 0x00085B9C File Offset: 0x00083D9C
	public bool Initialized { get; private set; }

	// Token: 0x06001702 RID: 5890 RVA: 0x00085BA5 File Offset: 0x00083DA5
	private void Awake()
	{
		if (ZoneManagement.instance == null)
		{
			this.Initialize();
			return;
		}
		if (ZoneManagement.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001703 RID: 5891 RVA: 0x00085BD3 File Offset: 0x00083DD3
	public static void SetActiveZone(GTZone zone)
	{
		ZoneManagement.SetActiveZones(new GTZone[] { zone });
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x00085BE4 File Offset: 0x00083DE4
	public static void SetActiveZones(GTZone[] zones)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		if (zones == null || zones.Length == 0)
		{
			return;
		}
		ZoneManagement.instance.SetZones(zones);
		Action action = ZoneManagement.instance.onZoneChanged;
		if (action != null)
		{
			action();
		}
		if (ZoneManagement.OnZoneChange != null)
		{
			ZoneManagement.OnZoneChange(ZoneManagement.instance.zones);
		}
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x00085C48 File Offset: 0x00083E48
	public static bool IsInZone(GTZone zone)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneData zoneData = ZoneManagement.instance.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	// Token: 0x06001706 RID: 5894 RVA: 0x00085C80 File Offset: 0x00083E80
	public static bool IsZoneLoaded(GTZone zone)
	{
		if (!ZoneManagement.instance)
		{
			ZoneManagement.FindInstance();
		}
		ZoneData zoneData = ZoneManagement.instance.GetZoneData(zone);
		return zoneData != null && zoneData.active && SceneManager.GetSceneByName(zoneData.sceneName).isLoaded;
	}

	// Token: 0x06001707 RID: 5895 RVA: 0x00085CCA File Offset: 0x00083ECA
	public GameObject GetPrimaryGameObject(GTZone zone)
	{
		return this.GetZoneData(zone).rootGameObjects[0];
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x00085CDA File Offset: 0x00083EDA
	public static void AddSceneToForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Add(sceneName);
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x00085CFF File Offset: 0x00083EFF
	public static void RemoveSceneFromForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Remove(sceneName);
	}

	// Token: 0x0600170A RID: 5898 RVA: 0x00085D24 File Offset: 0x00083F24
	public static void FindInstance()
	{
		ZoneManagement zoneManagement = Object.FindAnyObjectByType<ZoneManagement>();
		if (zoneManagement == null)
		{
			throw new NullReferenceException("Unable to find ZoneManagement object in scene.");
		}
		Debug.LogWarning("ZoneManagement accessed before MonoBehaviour awake function called; consider delaying zone management functions to avoid FindObject lookup.");
		zoneManagement.Initialize();
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x00085D50 File Offset: 0x00083F50
	public bool IsSceneLoaded(GTZone gtZone)
	{
		foreach (ZoneData zoneData in this.zones)
		{
			if (zoneData.zone == gtZone && (zoneData.sceneName == "" || this.scenesLoaded.Contains(zoneData.sceneName)))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x00085DA8 File Offset: 0x00083FA8
	public bool IsZoneActive(GTZone zone)
	{
		ZoneData zoneData = this.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	// Token: 0x0600170D RID: 5901 RVA: 0x00085DC8 File Offset: 0x00083FC8
	public HashSet<string> GetAllLoadedScenes()
	{
		return this.scenesLoaded;
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x00085DD0 File Offset: 0x00083FD0
	public bool IsSceneLoaded(string sceneName)
	{
		return this.scenesLoaded.Contains(sceneName);
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x00085DE0 File Offset: 0x00083FE0
	private void Initialize()
	{
		ZoneManagement.instance = this;
		this.hasInstance = true;
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		List<GameObject> list = new List<GameObject>(8);
		for (int i = 0; i < this.zones.Length; i++)
		{
			list.Clear();
			ZoneData zoneData = this.zones[i];
			if (zoneData != null && zoneData.rootGameObjects != null)
			{
				hashSet.UnionWith(zoneData.rootGameObjects);
				for (int j = 0; j < zoneData.rootGameObjects.Length; j++)
				{
					GameObject gameObject = zoneData.rootGameObjects[j];
					if (!(gameObject == null))
					{
						list.Add(gameObject);
					}
				}
				hashSet.UnionWith(list);
			}
		}
		this.allObjects = hashSet.ToArray<GameObject>();
		this.objectActivationState = new bool[this.allObjects.Length];
		ZoneManagement.AddSceneToForceStayLoaded("City");
		this.Initialized = true;
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x00085EAC File Offset: 0x000840AC
	private void SetZones(GTZone[] newActiveZones)
	{
		for (int i = 0; i < this.objectActivationState.Length; i++)
		{
			this.objectActivationState[i] = false;
		}
		this.activeZones.Clear();
		for (int j = 0; j < newActiveZones.Length; j++)
		{
			this.activeZones.Add(newActiveZones[j]);
		}
		this.scenesRequested.Clear();
		this.scenesRequested.Add("GorillaTag");
		float num = 0f;
		for (int k = 0; k < this.zones.Length; k++)
		{
			ZoneData zoneData = this.zones[k];
			if (zoneData != null)
			{
				if (zoneData.rootGameObjects == null || !newActiveZones.Contains(zoneData.zone))
				{
					zoneData.active = false;
				}
				else
				{
					zoneData.active = true;
					num = Mathf.Max(num, zoneData.CameraFarClipPlane);
					if (!string.IsNullOrEmpty(zoneData.sceneName))
					{
						this.scenesRequested.Add(zoneData.sceneName);
					}
					foreach (GameObject gameObject in zoneData.rootGameObjects)
					{
						if (!(gameObject == null))
						{
							for (int m = 0; m < this.allObjects.Length; m++)
							{
								if (gameObject == this.allObjects[m])
								{
									this.objectActivationState[m] = true;
									break;
								}
							}
						}
					}
				}
			}
		}
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		this.mainCamera.farClipPlane = num;
		int loadedSceneCount = SceneManager.loadedSceneCount;
		for (int n = 0; n < loadedSceneCount; n++)
		{
			this.scenesLoaded.Add(SceneManager.GetSceneAt(n).name);
		}
		foreach (string text in this.scenesRequested)
		{
			if (this.scenesLoaded.Add(text))
			{
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(text, LoadSceneMode.Additive);
				this._scenes_to_loadOps[text] = asyncOperation;
				asyncOperation.completed += this.HandleOnSceneLoadCompleted;
			}
		}
		this.scenesToUnload.Clear();
		foreach (string text2 in this.scenesLoaded)
		{
			if (!this.scenesRequested.Contains(text2) && !this.sceneForceStayLoaded.Contains(text2))
			{
				this.scenesToUnload.Add(text2);
			}
		}
		foreach (string text3 in this.scenesToUnload)
		{
			this.scenesLoaded.Remove(text3);
			if (SceneManager.GetSceneByName(text3).IsValid())
			{
				AsyncOperation asyncOperation2 = SceneManager.UnloadSceneAsync(text3);
				this._scenes_to_unloadOps[text3] = asyncOperation2;
			}
		}
		for (int num2 = 0; num2 < this.objectActivationState.Length; num2++)
		{
			if (!(this.allObjects[num2] == null))
			{
				this.allObjects[num2].SetActive(this.objectActivationState[num2]);
			}
		}
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x00086208 File Offset: 0x00084408
	private void HandleOnSceneLoadCompleted(AsyncOperation thisLoadOp)
	{
		foreach (KeyValuePair<string, AsyncOperation> keyValuePair in this._scenes_to_loadOps)
		{
			string text;
			AsyncOperation asyncOperation;
			keyValuePair.Deconstruct(out text, out asyncOperation);
			string text2 = text;
			AsyncOperation asyncOperation2 = asyncOperation;
			if (asyncOperation2 == null)
			{
				Debug.LogError("ERROR!!!  HandleOnSceneLoadCompleted: Why is `loadOp` null in `_scenes_to_loadOps` for scene \"" + text2 + "\"?????");
			}
			else if (!asyncOperation2.isDone)
			{
				return;
			}
		}
		foreach (KeyValuePair<string, AsyncOperation> keyValuePair in this._scenes_to_unloadOps)
		{
			string text;
			AsyncOperation asyncOperation;
			keyValuePair.Deconstruct(out text, out asyncOperation);
			string text3 = text;
			AsyncOperation asyncOperation3 = asyncOperation;
			if (asyncOperation3 == null)
			{
				Debug.LogError("ERROR!!!  HandleOnSceneLoadCompleted: Why is `unloadOps` null in `_scenes_to_unloadOps` for scene \"" + text3 + "\"?????");
			}
			else if (!asyncOperation3.isDone)
			{
				return;
			}
		}
		Action onSceneLoadsCompleted = this.OnSceneLoadsCompleted;
		if (onSceneLoadsCompleted == null)
		{
			return;
		}
		onSceneLoadsCompleted();
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x00086314 File Offset: 0x00084514
	public bool AnyActiveLoadOps()
	{
		return this._scenes_to_loadOps.Values.Any((AsyncOperation op) => !op.isDone);
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x00086348 File Offset: 0x00084548
	private ZoneData GetZoneData(GTZone zone)
	{
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].zone == zone)
			{
				return this.zones[i];
			}
		}
		return null;
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x00086382 File Offset: 0x00084582
	public string GetSceneNameForZone(GTZone zone)
	{
		ZoneData zoneData = this.GetZoneData(zone);
		if (zoneData == null)
		{
			return null;
		}
		return zoneData.sceneName;
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x00086396 File Offset: 0x00084596
	public static bool IsValidZoneInt(int zoneInt)
	{
		return zoneInt >= 11 && zoneInt <= 24;
	}

	// Token: 0x0400223B RID: 8763
	private const string preLog = "[GT/ZoneManagement]  ";

	// Token: 0x0400223C RID: 8764
	private const string preErr = "ERROR!!!  ";

	// Token: 0x0400223D RID: 8765
	private const string preErrBeta = "(beta only log)  ";

	// Token: 0x0400223F RID: 8767
	public static ZoneManagement instance;

	// Token: 0x04002242 RID: 8770
	[SerializeField]
	private ZoneData[] zones;

	// Token: 0x04002243 RID: 8771
	private GameObject[] allObjects;

	// Token: 0x04002244 RID: 8772
	private bool[] objectActivationState;

	// Token: 0x04002245 RID: 8773
	public Action onZoneChanged;

	// Token: 0x04002246 RID: 8774
	public Action OnSceneLoadsCompleted;

	// Token: 0x04002247 RID: 8775
	public List<GTZone> activeZones = new List<GTZone>(20);

	// Token: 0x04002248 RID: 8776
	private HashSet<string> scenesLoaded = new HashSet<string>();

	// Token: 0x04002249 RID: 8777
	private HashSet<string> scenesRequested = new HashSet<string>();

	// Token: 0x0400224A RID: 8778
	private HashSet<string> sceneForceStayLoaded = new HashSet<string>(8);

	// Token: 0x0400224B RID: 8779
	private List<string> scenesToUnload = new List<string>();

	// Token: 0x0400224C RID: 8780
	private Dictionary<string, AsyncOperation> _scenes_to_loadOps = new Dictionary<string, AsyncOperation>(32);

	// Token: 0x0400224D RID: 8781
	private Dictionary<string, AsyncOperation> _scenes_to_unloadOps = new Dictionary<string, AsyncOperation>(32);

	// Token: 0x0400224E RID: 8782
	private Camera mainCamera;

	// Token: 0x020003BC RID: 956
	// (Invoke) Token: 0x06001718 RID: 5912
	public delegate void ZoneChangeEvent(ZoneData[] zones);
}
