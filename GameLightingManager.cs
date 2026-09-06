using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020006EA RID: 1770
public class GameLightingManager : MonoBehaviourTick, IGorillaSliceableSimple
{
	// Token: 0x17000460 RID: 1120
	// (get) Token: 0x06002C8E RID: 11406 RVA: 0x000F0A0A File Offset: 0x000EEC0A
	public bool IsDynamicLightingEnabled
	{
		get
		{
			return this.customVertexLightingEnabled;
		}
	}

	// Token: 0x06002C8F RID: 11407 RVA: 0x000F0A12 File Offset: 0x000EEC12
	private static uint PackHalf2(float a, float b)
	{
		return (uint)((int)Mathf.FloatToHalf(a) | ((int)Mathf.FloatToHalf(b) << 16));
	}

	// Token: 0x06002C90 RID: 11408 RVA: 0x000F0A24 File Offset: 0x000EEC24
	private void Awake()
	{
		this.InitData();
	}

	// Token: 0x06002C91 RID: 11409 RVA: 0x000F0A2C File Offset: 0x000EEC2C
	private void InitData()
	{
		GameLightingManager.instance = this;
		this.gameLights = new List<GameLight>(512);
		this.sortKeys = new float[512];
		this.sortValues = new GameLight[512];
		this.lightDataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 50, UnsafeUtility.SizeOf<GameLightingManager.LightDataPacked>());
		this.lightData = new NativeArray<GameLightingManager.LightDataPacked>(50, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.lightDataBufferLegacy = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 50, UnsafeUtility.SizeOf<GameLightingManager.LightDataLegacy>());
		this.lightDataLegacy = new NativeArray<GameLightingManager.LightDataLegacy>(50, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.nextLightUpdate = 0;
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
		this.SetMaxLights(20);
		base.StartCoroutine(this.Preheat());
	}

	// Token: 0x06002C92 RID: 11410 RVA: 0x000F0AF7 File Offset: 0x000EECF7
	private IEnumerator Preheat()
	{
		yield return null;
		this.SetCustomDynamicLightingEnabled(true);
		yield return null;
		this.SetCustomDynamicLightingEnabled(false);
		yield break;
	}

	// Token: 0x06002C93 RID: 11411 RVA: 0x000F0B08 File Offset: 0x000EED08
	private void OnDestroy()
	{
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
		GraphicsBuffer graphicsBuffer = this.lightDataBuffer;
		if (graphicsBuffer != null)
		{
			graphicsBuffer.Dispose();
		}
		if (this.lightData.IsCreated)
		{
			this.lightData.Dispose();
		}
		GraphicsBuffer graphicsBuffer2 = this.lightDataBufferLegacy;
		if (graphicsBuffer2 != null)
		{
			graphicsBuffer2.Dispose();
		}
		if (this.lightDataLegacy.IsCreated)
		{
			this.lightDataLegacy.Dispose();
		}
	}

	// Token: 0x06002C94 RID: 11412 RVA: 0x000F0B8B File Offset: 0x000EED8B
	public new void OnEnable()
	{
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002C95 RID: 11413 RVA: 0x000F0B9A File Offset: 0x000EED9A
	public new void OnDisable()
	{
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002C96 RID: 11414 RVA: 0x000F0BAC File Offset: 0x000EEDAC
	public void ZoneEnableCustomDynamicLighting(bool enable)
	{
		if (enable)
		{
			if (this.zoneDynamicLightingEnableCount == 0)
			{
				this.SetCustomDynamicLightingEnabled(true);
			}
			this.zoneDynamicLightingEnableCount++;
			return;
		}
		this.zoneDynamicLightingEnableCount--;
		if (this.zoneDynamicLightingEnableCount == 0)
		{
			this.SetCustomDynamicLightingEnabled(false);
		}
		if (this.zoneDynamicLightingEnableCount < 0)
		{
			Debug.LogErrorFormat("Zone Dynamic Lighting Ref count is {0} and should never be less that 0", new object[] { this.zoneDynamicLightingEnableCount });
			this.zoneDynamicLightingEnableCount = 0;
		}
	}

	// Token: 0x06002C97 RID: 11415 RVA: 0x000F0C25 File Offset: 0x000EEE25
	public void SetCustomDynamicLightingEnabled(bool enable)
	{
		this.customVertexLightingEnabled = enable;
		if (this.customVertexLightingEnabled)
		{
			Shader.EnableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
			return;
		}
		Shader.DisableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
	}

	// Token: 0x06002C98 RID: 11416 RVA: 0x000F0C4B File Offset: 0x000EEE4B
	public void ToggleCustomDynamicLightingEnabled()
	{
		this.SetCustomDynamicLightingEnabled(!this.customVertexLightingEnabled);
	}

	// Token: 0x06002C99 RID: 11417 RVA: 0x000F0C5C File Offset: 0x000EEE5C
	public void SetAmbientLightDynamic(Color color)
	{
		Shader.SetGlobalColor(GameLightingManager._shaderPropId_GameLight_Ambient_Color, color);
	}

	// Token: 0x06002C9A RID: 11418 RVA: 0x000F0C69 File Offset: 0x000EEE69
	public void SetMaxLights(int maxLights)
	{
		maxLights = Mathf.Min(maxLights, 50);
		this.maxUseTestLights = maxLights;
		Shader.SetGlobalInteger(GameLightingManager._shaderPropId_GameLight_UseMaxLights, maxLights);
	}

	// Token: 0x06002C9B RID: 11419 RVA: 0x000F0C87 File Offset: 0x000EEE87
	public void SetDesaturateAndTintEnabled(bool enable, Color tint)
	{
		Shader.SetGlobalColor(GameLightingManager._shaderPropId_DesaturateAndTint_TintColor, tint);
		Shader.SetGlobalFloat(GameLightingManager._shaderPropId_DesaturateAndTint_TintAmount, enable ? 1f : 0f);
		this.desaturateAndTintEnabled = enable;
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x000F0CB4 File Offset: 0x000EEEB4
	public void SliceUpdate()
	{
		if (this.skipNextSlice)
		{
			this.skipNextSlice = false;
			return;
		}
		this.immediateSort = false;
		this.SortLights();
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x000F0CD4 File Offset: 0x000EEED4
	public void SortLights()
	{
		int count = this.gameLights.Count;
		if (count <= this.maxUseTestLights)
		{
			return;
		}
		if (this.mainCameraTransform == null)
		{
			this.mainCameraTransform = Camera.main.transform;
		}
		Vector3 position = this.mainCameraTransform.position;
		if (this.sortKeys == null || this.sortKeys.Length < count)
		{
			int num = Mathf.Max(count, (this.sortKeys != null) ? (this.sortKeys.Length * 2) : 64);
			this.sortKeys = new float[num];
			this.sortValues = new GameLight[num];
		}
		for (int i = 0; i < count; i++)
		{
			GameLight gameLight = this.gameLights[i];
			if (gameLight == null || gameLight.light == null)
			{
				this.sortKeys[i] = float.MaxValue;
			}
			else
			{
				float num2 = Mathf.Clamp(gameLight.cachedColorAndIntensity.x + gameLight.cachedColorAndIntensity.y + gameLight.cachedColorAndIntensity.z, 0.01f, 6f);
				Vector3 vector = position - gameLight.cachedPosition;
				this.sortKeys[i] = (vector.x * vector.x + vector.y * vector.y + vector.z * vector.z) / num2;
			}
			this.sortValues[i] = gameLight;
		}
		Array.Sort<float, GameLight>(this.sortKeys, this.sortValues, 0, count);
		for (int j = 0; j < count; j++)
		{
			this.gameLights[j] = this.sortValues[j];
		}
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x000F0E77 File Offset: 0x000EF077
	public override void Tick()
	{
		this.RefreshLightData();
	}

	// Token: 0x06002C9F RID: 11423 RVA: 0x000F0E80 File Offset: 0x000EF080
	private void RefreshLightData()
	{
		if (this.lightDataBuffer == null)
		{
			return;
		}
		if (this.customVertexLightingEnabled)
		{
			int num = 10;
			if (this.immediateSort)
			{
				this.immediateSort = false;
				this.skipNextSlice = true;
				this.CacheAllLightData();
				this.SortLights();
				num = this.maxUseTestLights;
			}
			else
			{
				int num2 = 5;
				this.CacheLightDataForNonCloseLights(num2);
			}
			this.PullLightData(num);
			int num3 = Mathf.Min(this.gameLights.Count, this.maxUseTestLights);
			if (num3 > 0)
			{
				bool flag = CustomMapLoader.IsMapLoaded();
				this.lightDataBuffer.SetData<GameLightingManager.LightDataPacked>(this.lightData, 0, 0, num3);
				if (flag)
				{
					this.lightDataBufferLegacy.SetData<GameLightingManager.LightDataLegacy>(this.lightDataLegacy);
				}
				Shader.SetGlobalBuffer(GameLightingManager._shaderPropId_GameLight_LightsPacked, this.lightDataBuffer);
				if (flag)
				{
					Shader.SetGlobalBuffer(GameLightingManager._shaderPropId_GameLight_Lights, this.lightDataBufferLegacy);
				}
				Shader.SetGlobalInteger(GameLightingManager._shaderPropId_GameLight_UseMaxLights, num3);
			}
		}
	}

	// Token: 0x06002CA0 RID: 11424 RVA: 0x000F0F54 File Offset: 0x000EF154
	public void CacheAllLightData()
	{
		for (int i = 0; i < this.gameLights.Count; i++)
		{
			GameLight gameLight = this.gameLights[i];
			if (gameLight != null && gameLight.light != null)
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? (-1f) : 1f) * gameLight.light.color;
			}
		}
	}

	// Token: 0x06002CA1 RID: 11425 RVA: 0x000F0FF4 File Offset: 0x000EF1F4
	public void CacheLightDataForNonCloseLights(int numLightsToUpdateCache)
	{
		int num = this.gameLights.Count - this.maxUseTestLights;
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < numLightsToUpdateCache; i++)
		{
			this.nextLightCacheUpdate = (this.nextLightCacheUpdate + 1) % num;
			GameLight gameLight = this.gameLights[this.maxUseTestLights + this.nextLightCacheUpdate];
			if (gameLight != null && gameLight.light != null)
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? (-1f) : 1f) * gameLight.light.color;
			}
		}
	}

	// Token: 0x06002CA2 RID: 11426 RVA: 0x000F10C0 File Offset: 0x000EF2C0
	public void PullLightData(int numLightsToPull)
	{
		for (int i = 0; i < this.maxUseTestLights; i++)
		{
			if (i < this.gameLights.Count && this.gameLights[i] != null && this.gameLights[i].isHighPriorityPlayerLight)
			{
				this.GetFromLight(i, i);
			}
		}
		for (int j = 0; j < numLightsToPull; j++)
		{
			this.nextLightUpdate = (this.nextLightUpdate + 1) % this.maxUseTestLights;
			if (this.nextLightUpdate < this.gameLights.Count)
			{
				this.GetFromLight(this.nextLightUpdate, this.nextLightUpdate);
				if (this.gameLights[this.nextLightUpdate] != null && this.gameLights[this.nextLightUpdate].isHighPriorityPlayerLight)
				{
				}
			}
			else
			{
				this.ResetLight(this.nextLightUpdate);
			}
		}
	}

	// Token: 0x06002CA3 RID: 11427 RVA: 0x000F11A4 File Offset: 0x000EF3A4
	public int AddGameLight(GameLight light, bool ignoreUnityLightDisable = false)
	{
		if (light == null || !light.gameObject.activeInHierarchy || light.light == null || !light.light.enabled)
		{
			return -1;
		}
		if (light.IsRegistered)
		{
			return -1;
		}
		if (!ignoreUnityLightDisable)
		{
			light.light.enabled = false;
		}
		this.gameLights.Add(light);
		this.immediateSort = true;
		return this.gameLights.Count - 1;
	}

	// Token: 0x06002CA4 RID: 11428 RVA: 0x000F1220 File Offset: 0x000EF420
	public void RemoveGameLight(GameLight light)
	{
		if (light != null && light.light != null)
		{
			light.light.enabled = true;
		}
		if (light != null)
		{
			light.lightId = -1;
		}
		int num = this.gameLights.IndexOf(light);
		if (num >= 0)
		{
			this.gameLights.RemoveAt(num);
			if (CustomMapLoader.IsMapLoaded())
			{
				int count = this.gameLights.Count;
				if (count < 50)
				{
					this.lightDataLegacy[count] = default(GameLightingManager.LightDataLegacy);
				}
			}
		}
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x000F12AC File Offset: 0x000EF4AC
	public void ClearGameLights()
	{
		if (this.gameLights != null)
		{
			this.gameLights.Clear();
		}
		if (this.lightDataBuffer == null)
		{
			return;
		}
		for (int i = 0; i < 50; i++)
		{
			this.ResetLight(i);
		}
		this.lightDataBuffer.SetData<GameLightingManager.LightDataPacked>(this.lightData);
		Shader.SetGlobalBuffer(GameLightingManager._shaderPropId_GameLight_LightsPacked, this.lightDataBuffer);
		if (CustomMapLoader.IsMapLoaded())
		{
			this.lightDataBufferLegacy.SetData<GameLightingManager.LightDataLegacy>(this.lightDataLegacy);
			Shader.SetGlobalBuffer(GameLightingManager._shaderPropId_GameLight_Lights, this.lightDataBufferLegacy);
		}
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x000F1334 File Offset: 0x000EF534
	public void GetFromLight(int lightIndex, int gameLightIndex)
	{
		if (this.lightDataBuffer == null)
		{
			return;
		}
		GameLight gameLight = null;
		if (gameLightIndex >= 0 && gameLightIndex < this.gameLights.Count)
		{
			gameLight = this.gameLights[gameLightIndex];
		}
		if (gameLight == null || gameLight.light == null)
		{
			return;
		}
		gameLight.cachedPosition = gameLight.transform.position;
		gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? (-1f) : 1f) * gameLight.light.color;
		if (gameLight.applyRange && gameLight.light.range > 0f)
		{
			gameLight.range = 0.005f / gameLight.light.range;
		}
		Vector3 cachedPosition = gameLight.cachedPosition;
		Vector4 cachedColorAndIntensity = gameLight.cachedColorAndIntensity;
		this.lightData[lightIndex] = new GameLightingManager.LightDataPacked
		{
			posXY = GameLightingManager.PackHalf2(cachedPosition.x, cachedPosition.y),
			posZW = GameLightingManager.PackHalf2(cachedPosition.z, 1f),
			colorRG = GameLightingManager.PackHalf2(cachedColorAndIntensity.x, cachedColorAndIntensity.y),
			colorBA = GameLightingManager.PackHalf2(cachedColorAndIntensity.z, cachedColorAndIntensity.w),
			range = gameLight.range
		};
		this.lightDataLegacy[lightIndex] = new GameLightingManager.LightDataLegacy
		{
			position = new float4(cachedPosition.x, cachedPosition.y, cachedPosition.z, 1f),
			color = new float4(cachedColorAndIntensity.x, cachedColorAndIntensity.y, cachedColorAndIntensity.z, cachedColorAndIntensity.w),
			direction = float4.zero
		};
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x000F1500 File Offset: 0x000EF700
	private void ResetLight(int lightIndex)
	{
		this.lightData[lightIndex] = default(GameLightingManager.LightDataPacked);
		this.lightDataLegacy[lightIndex] = default(GameLightingManager.LightDataLegacy);
	}

	// Token: 0x17000461 RID: 1121
	// (get) Token: 0x06002CA8 RID: 11432 RVA: 0x000F1537 File Offset: 0x000EF737
	public Light GR_NearsightedDimLight
	{
		get
		{
			return this._GR_NearsightedDimLight;
		}
	}

	// Token: 0x0400390E RID: 14606
	[OnEnterPlay_SetNull]
	public static volatile GameLightingManager instance;

	// Token: 0x0400390F RID: 14607
	public const int MAX_VERTEX_LIGHTS = 50;

	// Token: 0x04003910 RID: 14608
	public const int USE_MAX_VERTEX_LIGHTS = 20;

	// Token: 0x04003911 RID: 14609
	public const int MAX_UPDATE_LIGHTS_PER_FRAME = 10;

	// Token: 0x04003912 RID: 14610
	public Transform testLightsCenter;

	// Token: 0x04003913 RID: 14611
	[ColorUsage(true, true)]
	public Color testAmbience = Color.black;

	// Token: 0x04003914 RID: 14612
	[ColorUsage(true, true)]
	public Color testLightColor = Color.white;

	// Token: 0x04003915 RID: 14613
	public float testLightBrightness = 10f;

	// Token: 0x04003916 RID: 14614
	public float testLightRadius = 2f;

	// Token: 0x04003917 RID: 14615
	public int maxUseTestLights = 1;

	// Token: 0x04003918 RID: 14616
	[ReadOnly]
	[SerializeField]
	private List<GameLight> gameLights;

	// Token: 0x04003919 RID: 14617
	private bool customVertexLightingEnabled;

	// Token: 0x0400391A RID: 14618
	private bool desaturateAndTintEnabled;

	// Token: 0x0400391B RID: 14619
	private Transform mainCameraTransform;

	// Token: 0x0400391C RID: 14620
	private int zoneDynamicLightingEnableCount;

	// Token: 0x0400391D RID: 14621
	private float[] sortKeys;

	// Token: 0x0400391E RID: 14622
	private GameLight[] sortValues;

	// Token: 0x0400391F RID: 14623
	private NativeArray<GameLightingManager.LightDataPacked> lightData;

	// Token: 0x04003920 RID: 14624
	private NativeArray<GameLightingManager.LightDataLegacy> lightDataLegacy;

	// Token: 0x04003921 RID: 14625
	private GraphicsBuffer lightDataBuffer;

	// Token: 0x04003922 RID: 14626
	private GraphicsBuffer lightDataBufferLegacy;

	// Token: 0x04003923 RID: 14627
	private bool skipNextSlice;

	// Token: 0x04003924 RID: 14628
	private bool immediateSort;

	// Token: 0x04003925 RID: 14629
	private int nextLightUpdate;

	// Token: 0x04003926 RID: 14630
	private int nextLightCacheUpdate;

	// Token: 0x04003927 RID: 14631
	[SerializeField]
	private Light _GR_NearsightedDimLight;

	// Token: 0x04003928 RID: 14632
	private static readonly int _shaderPropId_GameLight_UseMaxLights = Shader.PropertyToID("_GT_GameLight_UseMaxLights");

	// Token: 0x04003929 RID: 14633
	private static readonly int _shaderPropId_DesaturateAndTint_TintColor = Shader.PropertyToID("_GT_DesaturateAndTint_TintColor");

	// Token: 0x0400392A RID: 14634
	private static readonly int _shaderPropId_DesaturateAndTint_TintAmount = Shader.PropertyToID("_GT_DesaturateAndTint_TintAmount");

	// Token: 0x0400392B RID: 14635
	private static readonly int _shaderPropId_GameLight_Ambient_Color = Shader.PropertyToID("_GT_GameLight_Ambient_Color");

	// Token: 0x0400392C RID: 14636
	private static readonly int _shaderPropId_GameLight_Lights = Shader.PropertyToID("_GT_GameLight_Lights");

	// Token: 0x0400392D RID: 14637
	private static readonly int _shaderPropId_GameLight_LightsPacked = Shader.PropertyToID("_GT_GameLight_LightsPacked");

	// Token: 0x020006EB RID: 1771
	private struct LightInput
	{
		// Token: 0x0400392E RID: 14638
		public Color color;

		// Token: 0x0400392F RID: 14639
		public float intensity;

		// Token: 0x04003930 RID: 14640
		public float intensityMult;
	}

	// Token: 0x020006EC RID: 1772
	private struct LightDataPacked
	{
		// Token: 0x04003931 RID: 14641
		public uint posXY;

		// Token: 0x04003932 RID: 14642
		public uint posZW;

		// Token: 0x04003933 RID: 14643
		public uint colorRG;

		// Token: 0x04003934 RID: 14644
		public uint colorBA;

		// Token: 0x04003935 RID: 14645
		public float range;
	}

	// Token: 0x020006ED RID: 1773
	private struct LightDataLegacy
	{
		// Token: 0x04003936 RID: 14646
		public float4 position;

		// Token: 0x04003937 RID: 14647
		public float4 color;

		// Token: 0x04003938 RID: 14648
		public float4 direction;
	}
}
