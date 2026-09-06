using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A11 RID: 2577
public class BetterDayNightManager : MonoBehaviour, IGorillaSliceableSimple, ITimeOfDaySystem
{
	// Token: 0x0600421D RID: 16925 RVA: 0x00160730 File Offset: 0x0015E930
	public static void Register(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Add(data);
	}

	// Token: 0x0600421E RID: 16926 RVA: 0x0016073D File Offset: 0x0015E93D
	public static void Unregister(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Remove(data);
	}

	// Token: 0x17000642 RID: 1602
	// (get) Token: 0x0600421F RID: 16927 RVA: 0x0016074B File Offset: 0x0015E94B
	public double[] timeOfDayRange
	{
		get
		{
			if (this.currentSeason == BetterDayNightManager.Season.Winter)
			{
				return this.winterTimeOfDayRange;
			}
			return this.summerTimeOfDayRange;
		}
	}

	// Token: 0x17000643 RID: 1603
	// (get) Token: 0x06004220 RID: 16928 RVA: 0x00160762 File Offset: 0x0015E962
	// (set) Token: 0x06004221 RID: 16929 RVA: 0x0016076A File Offset: 0x0015E96A
	public string currentTimeOfDay { get; private set; }

	// Token: 0x17000644 RID: 1604
	// (get) Token: 0x06004222 RID: 16930 RVA: 0x00160773 File Offset: 0x0015E973
	public float NormalizedTimeOfDay
	{
		get
		{
			return Mathf.Clamp01((float)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds / this.totalSeconds));
		}
	}

	// Token: 0x17000645 RID: 1605
	// (get) Token: 0x06004223 RID: 16931 RVA: 0x0016079D File Offset: 0x0015E99D
	double ITimeOfDaySystem.currentTimeInSeconds
	{
		get
		{
			return this.currentTime;
		}
	}

	// Token: 0x17000646 RID: 1606
	// (get) Token: 0x06004224 RID: 16932 RVA: 0x001607A5 File Offset: 0x0015E9A5
	double ITimeOfDaySystem.totalTimeInSeconds
	{
		get
		{
			return this.totalSeconds;
		}
	}

	// Token: 0x06004225 RID: 16933 RVA: 0x001607B0 File Offset: 0x0015E9B0
	private void Awake()
	{
		RoomSystem.JoinedRoomEvent += new Action(this.OnRoomJoin);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoined);
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.OnMasterClientSwitched;
		this.m_fixedDataCache.Reset();
		this.m_setTimeDataCache.Reset();
		SubscriptionManager.OnSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnSubscriptionData, new Action(this.OnSubscrptionData));
	}

	// Token: 0x06004226 RID: 16934 RVA: 0x0016084C File Offset: 0x0015EA4C
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (BetterDayNightManager.instance == null)
		{
			BetterDayNightManager.instance = this;
		}
		else if (BetterDayNightManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.currentLerp = 0f;
		this.totalHours = 0.0;
		for (int i = 0; i < this.timeOfDayRange.Length; i++)
		{
			this.totalHours += this.timeOfDayRange[i];
		}
		this.totalSeconds = this.totalHours * 60.0 * 60.0;
		this.currentTimeIndex = 0;
		this.baseSeconds = 0.0;
		this.computerInit = false;
		this.randomNumberGenerator = new Random(this.mySeed);
		this.GenerateWeatherEventTimes();
		this.ChangeMaps(0, 1);
		base.StartCoroutine(this.InitialUpdate());
	}

	// Token: 0x06004227 RID: 16935 RVA: 0x00019269 File Offset: 0x00017469
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004228 RID: 16936 RVA: 0x00160940 File Offset: 0x0015EB40
	public void UpdateTimeOfDay(bool forceUpdate = false)
	{
		if (!forceUpdate && Time.time < this.lastTimeChecked + this.currentTimestep)
		{
			return;
		}
		this.lastTimeChecked = Time.time;
		if (this.animatingLightFlash != null)
		{
			return;
		}
		try
		{
			if (!this.computerInit && GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.computerInit = true;
				this.initialDayCycles = (long)(TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier / this.totalSeconds);
				this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
				this.baseSeconds = TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier % this.totalSeconds;
				this.currentTime = (this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds;
				this.FindTimeOfDayIndex();
				this.currentWeatherIndex += this.currentTimeIndex;
			}
			else if (!this.computerInit && this.baseSeconds == 0.0)
			{
				this.initialDayCycles = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier / this.totalSeconds);
				this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
				this.baseSeconds = TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier % this.totalSeconds;
				this.currentTime = this.baseSeconds % this.totalSeconds;
				this.FindTimeOfDayIndex();
				this.currentWeatherIndex += this.currentTimeIndex - 1;
				if (this.currentWeatherIndex < 0)
				{
					this.currentWeatherIndex = this.weatherCycle.Length - 1;
				}
			}
			this.currentTime = ((this.currentSetting == TimeSettings.Normal) ? ((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds) : this.currentTime);
			this.FindTimeOfDayIndex();
			if (this.timeIndexOverrideFunc != null)
			{
				this.currentTimeIndex = this.timeIndexOverrideFunc(this.currentTimeIndex);
			}
			if (this.currentTimeIndex != this.lastIndex)
			{
				this.currentWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
				this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
			}
			this.currentLerp = (float)(1.0 - (this.currentIndexSeconds - this.currentTime) / (this.timeOfDayRange[this.currentTimeIndex] * 3600.0));
			Shader.SetGlobalFloat(this._GT_DayCycleTimeProgress, this.NormalizedTimeOfDay);
			this.ChangeLerps(this.currentLerp);
			this.lastIndex = this.currentTimeIndex;
			this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
		}
		catch (Exception ex)
		{
			string text = "Error in BetterDayNightManager: ";
			Exception ex2 = ex;
			Debug.LogError(text + ((ex2 != null) ? ex2.ToString() : null), this);
		}
		this.gameEpochDay = (long)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) / this.totalSeconds + (double)this.initialDayCycles);
		foreach (BetterDayNightManager.ScheduledEvent scheduledEvent in BetterDayNightManager.scheduledEvents.Values)
		{
			if (scheduledEvent.lastDayCalled != this.gameEpochDay && scheduledEvent.hour == this.currentTimeIndex)
			{
				scheduledEvent.lastDayCalled = this.gameEpochDay;
				scheduledEvent.action();
			}
		}
	}

	// Token: 0x06004229 RID: 16937 RVA: 0x00160D4C File Offset: 0x0015EF4C
	private void FindTimeOfDayIndex()
	{
		this.currentIndexSeconds = 0.0;
		for (int i = 0; i < this.timeOfDayRange.Length; i++)
		{
			this.currentIndexSeconds += this.timeOfDayRange[i] * 3600.0;
			if (this.currentIndexSeconds > this.currentTime)
			{
				this.currentTimeIndex = i;
				return;
			}
		}
	}

	// Token: 0x0600422A RID: 16938 RVA: 0x00160DB0 File Offset: 0x0015EFB0
	private void ChangeLerps(float newLerp)
	{
		Shader.SetGlobalFloat(this._GlobalDayNightLerpValue, newLerp);
		Shader.SetGlobalFloat(this._GT_DayCycleBrightnessOption1_Id, Mathf.Lerp(this.colorFrom, this.colorTo, newLerp));
		Shader.SetGlobalFloat(this._GT_DayCycleBrightnessOption2_Id, Mathf.Lerp(this.colorFromDarker, this.colorToDarker, newLerp));
	}

	// Token: 0x0600422B RID: 16939 RVA: 0x00160E14 File Offset: 0x0015F014
	private void ChangeMaps(int fromIndex, int toIndex)
	{
		this.fromWeatherIndex = this.currentWeatherIndex;
		this.toWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining && this.currentSetting != TimeSettings.Static)
		{
			this.fromSky = this.dayNightWeatherSkyboxTextures[fromIndex];
		}
		else
		{
			this.fromSky = this.dayNightSkyboxTextures[fromIndex];
		}
		this.fromSky2 = this.cloudsDayNightSkyboxTextures[fromIndex];
		this.fromSky3 = this.beachDayNightSkyboxTextures[fromIndex];
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining && this.currentSetting != TimeSettings.Static)
		{
			this.toSky = this.dayNightWeatherSkyboxTextures[toIndex];
		}
		else
		{
			this.toSky = this.dayNightSkyboxTextures[toIndex];
		}
		this.toSky2 = this.cloudsDayNightSkyboxTextures[toIndex];
		this.toSky3 = this.beachDayNightSkyboxTextures[toIndex];
		this.PopulateAllLightmaps(fromIndex, toIndex);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex1, this.fromSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex2, this.toSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex1, this.fromSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex2, this.toSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex1, this.fromSky3);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex2, this.toSky3);
		this.colorFrom = this.standardUnlitColor[fromIndex];
		this.colorTo = this.standardUnlitColor[toIndex];
		this.colorFromDarker = this.standardUnlitColorWithPremadeColorDarker[fromIndex];
		this.colorToDarker = this.standardUnlitColorWithPremadeColorDarker[toIndex];
	}

	// Token: 0x0600422C RID: 16940 RVA: 0x00160FAC File Offset: 0x0015F1AC
	public void SliceUpdate()
	{
		if (!this.shouldRepopulate)
		{
			using (List<PerSceneRenderData>.Enumerator enumerator = BetterDayNightManager.allScenesRenderData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CheckShouldRepopulate())
					{
						this.shouldRepopulate = true;
						break;
					}
				}
			}
		}
		if (this.shouldRepopulate)
		{
			this.PopulateAllLightmaps();
			this.shouldRepopulate = false;
		}
		this.UpdateTimeOfDay(false);
	}

	// Token: 0x0600422D RID: 16941 RVA: 0x0016102C File Offset: 0x0015F22C
	private IEnumerator InitialUpdate()
	{
		yield return null;
		this.SliceUpdate();
		yield break;
	}

	// Token: 0x0600422E RID: 16942 RVA: 0x0016103B File Offset: 0x0015F23B
	public void RequestRepopulateLightmaps()
	{
		this.shouldRepopulate = true;
	}

	// Token: 0x0600422F RID: 16943 RVA: 0x00161044 File Offset: 0x0015F244
	public void PopulateAllLightmaps()
	{
		this.PopulateAllLightmaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x06004230 RID: 16944 RVA: 0x00161064 File Offset: 0x0015F264
	public void PopulateAllLightmaps(int fromIndex, int toIndex)
	{
		string text;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			text = this.dayNightWeatherLightmapNames[fromIndex];
		}
		else
		{
			text = this.dayNightLightmapNames[fromIndex];
		}
		string text2;
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			text2 = this.dayNightWeatherLightmapNames[toIndex];
		}
		else
		{
			text2 = this.dayNightLightmapNames[toIndex];
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		foreach (PerSceneRenderData perSceneRenderData in BetterDayNightManager.allScenesRenderData)
		{
			perSceneRenderData.PopulateLightmaps(text, text2, lightmaps);
		}
		LightmapSettings.lightmaps = lightmaps;
	}

	// Token: 0x06004231 RID: 16945 RVA: 0x0016110C File Offset: 0x0015F30C
	public BetterDayNightManager.WeatherType CurrentWeather()
	{
		if (this.overrideWeather)
		{
			return this.overrideWeatherType;
		}
		if (this.currentSetting == TimeSettings.Static)
		{
			return BetterDayNightManager.WeatherType.None;
		}
		return this.weatherCycle[this.currentWeatherIndex];
	}

	// Token: 0x06004232 RID: 16946 RVA: 0x00161134 File Offset: 0x0015F334
	public BetterDayNightManager.WeatherType NextWeather()
	{
		if (this.overrideWeather)
		{
			return this.overrideWeatherType;
		}
		if (this.currentSetting == TimeSettings.Static)
		{
			return BetterDayNightManager.WeatherType.None;
		}
		return this.weatherCycle[(this.currentWeatherIndex + 1) % this.weatherCycle.Length];
	}

	// Token: 0x06004233 RID: 16947 RVA: 0x00161167 File Offset: 0x0015F367
	public BetterDayNightManager.WeatherType LastWeather()
	{
		if (this.overrideWeather)
		{
			return this.overrideWeatherType;
		}
		if (this.currentSetting == TimeSettings.Static)
		{
			return BetterDayNightManager.WeatherType.None;
		}
		return this.weatherCycle[(this.currentWeatherIndex - 1) % this.weatherCycle.Length];
	}

	// Token: 0x06004234 RID: 16948 RVA: 0x0016119C File Offset: 0x0015F39C
	private void GenerateWeatherEventTimes()
	{
		this.weatherCycle = new BetterDayNightManager.WeatherType[100 * this.dayNightLightmapNames.Length];
		this.rainChance = this.rainChance * 2f / (float)this.maxRainDuration;
		for (int i = 1; i < this.weatherCycle.Length; i++)
		{
			this.weatherCycle[i] = (((float)this.randomNumberGenerator.Next(100) < this.rainChance * 100f) ? BetterDayNightManager.WeatherType.Raining : BetterDayNightManager.WeatherType.None);
			if (this.weatherCycle[i] == BetterDayNightManager.WeatherType.Raining)
			{
				this.rainDuration = this.randomNumberGenerator.Next(1, this.maxRainDuration + 1);
				for (int j = 1; j < this.rainDuration; j++)
				{
					if (i + j < this.weatherCycle.Length)
					{
						this.weatherCycle[i + j] = BetterDayNightManager.WeatherType.Raining;
					}
				}
				i += this.rainDuration - 1;
			}
		}
	}

	// Token: 0x06004235 RID: 16949 RVA: 0x00161274 File Offset: 0x0015F474
	public static int RegisterScheduledEvent(int hour, Action action)
	{
		int num = (int)(DateTime.Now.Ticks % 2147483647L);
		while (BetterDayNightManager.scheduledEvents.ContainsKey(num))
		{
			num++;
		}
		BetterDayNightManager.scheduledEvents.Add(num, new BetterDayNightManager.ScheduledEvent
		{
			lastDayCalled = -1L,
			hour = hour,
			action = action
		});
		return num;
	}

	// Token: 0x06004236 RID: 16950 RVA: 0x001612D1 File Offset: 0x0015F4D1
	public static void UnregisterScheduledEvent(int id)
	{
		BetterDayNightManager.scheduledEvents.Remove(id);
	}

	// Token: 0x06004237 RID: 16951 RVA: 0x001612DF File Offset: 0x0015F4DF
	public void SetTimeIndexOverrideFunction(Func<int, int> overrideFunction)
	{
		this.timeIndexOverrideFunc = overrideFunction;
	}

	// Token: 0x06004238 RID: 16952 RVA: 0x001612E8 File Offset: 0x0015F4E8
	public void UnsetTimeIndexOverrideFunction()
	{
		this.timeIndexOverrideFunc = null;
	}

	// Token: 0x06004239 RID: 16953 RVA: 0x001612F4 File Offset: 0x0015F4F4
	public void SetOverrideIndex(int index)
	{
		this.overrideIndex = index;
		this.currentWeatherIndex = this.overrideIndex;
		this.currentTimeIndex = this.overrideIndex;
		this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x0600423A RID: 16954 RVA: 0x00161350 File Offset: 0x0015F550
	public void AnimateLightFlash(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		if (this.animatingLightFlash != null)
		{
			base.StopCoroutine(this.animatingLightFlash);
		}
		this.animatingLightFlash = base.StartCoroutine(this.AnimateLightFlashCo(index, fadeInDuration, holdDuration, fadeOutDuration));
	}

	// Token: 0x0600423B RID: 16955 RVA: 0x0016137D File Offset: 0x0015F57D
	private IEnumerator AnimateLightFlashCo(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		int startMap = ((this.currentLerp < 0.5f) ? this.currentTimeIndex : ((this.currentTimeIndex + 1) % this.timeOfDayRange.Length));
		this.ChangeMaps(startMap, index);
		float endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(index, index);
		this.ChangeLerps(0f);
		endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			yield return null;
		}
		this.ChangeMaps(index, startMap);
		endTimestamp = Time.time + fadeOutDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
		this.ChangeLerps(this.currentLerp);
		this.animatingLightFlash = null;
		yield break;
	}

	// Token: 0x0600423C RID: 16956 RVA: 0x001613A4 File Offset: 0x0015F5A4
	public void SetTimeOfDay(int timeIndex, bool forceUpdate = false)
	{
		this.lastSentTimeIndex = timeIndex;
		double num = 0.0;
		for (int i = 0; i < timeIndex; i++)
		{
			num += this.timeOfDayRange[i];
		}
		this.currentTime = num * 3600.0;
		this.currentSetting = TimeSettings.Static;
		if (forceUpdate)
		{
			this.FindTimeOfDayIndex();
			this.UpdateTimeOfDay(true);
		}
	}

	// Token: 0x0600423D RID: 16957 RVA: 0x00161404 File Offset: 0x0015F604
	public void IncrementTimeOfDay(int change)
	{
		int num = (this.currentTimeIndex + this.timeOfDayRange.Length + change) % this.timeOfDayRange.Length;
		this.SetTimeOfDayIndex(num);
	}

	// Token: 0x0600423E RID: 16958 RVA: 0x00161434 File Offset: 0x0015F634
	private void SetTimeOfDayIndex(int newIndex)
	{
		if (this.lastSentTimeIndex != -1 && this.lastSentTimeIndex == newIndex)
		{
			return;
		}
		BetterDayNightManager.WeatherType weatherType = this.overrideWeatherType;
		this.SetTimeOfDay(newIndex, false);
		if (this.overrideWeather)
		{
			this.SetFixedWeather(weatherType, false);
		}
		this.SetOverrideIndex(newIndex);
	}

	// Token: 0x0600423F RID: 16959 RVA: 0x0016147A File Offset: 0x0015F67A
	public void FastForward(float seconds)
	{
		this.baseSeconds += (double)seconds;
	}

	// Token: 0x06004240 RID: 16960 RVA: 0x0016148B File Offset: 0x0015F68B
	public void ClearTimeOfDay(bool forceUpdate = false)
	{
		this.currentSetting = TimeSettings.Normal;
		this.lastSentTimeIndex = -1;
		if (forceUpdate)
		{
			this.UpdateTimeOfDay(true);
		}
	}

	// Token: 0x06004241 RID: 16961 RVA: 0x001614A8 File Offset: 0x0015F6A8
	public string GetTimeOfDayString()
	{
		if (this.currentSetting == TimeSettings.Normal)
		{
			return "DEFAULT";
		}
		string text = this.currentTimeOfDay;
		if (this.currentTimeIndex >= 0 && this.currentTimeIndex < this.dayNightLightmapNames.Length)
		{
			text = this.dayNightLightmapNames[this.currentTimeIndex];
		}
		if (this.lastSentTimeIndex == 3 && !text.Equals("10am", StringComparison.OrdinalIgnoreCase))
		{
			text = "10am";
		}
		return text;
	}

	// Token: 0x06004242 RID: 16962 RVA: 0x00161510 File Offset: 0x0015F710
	public void SetFixedWeather(BetterDayNightManager.WeatherType weather, bool forceUpdate = false)
	{
		this.overrideWeather = true;
		this.overrideWeatherType = weather;
		if (forceUpdate)
		{
			this.UpdateTimeOfDay(true);
		}
	}

	// Token: 0x06004243 RID: 16963 RVA: 0x0016152A File Offset: 0x0015F72A
	public void ClearFixedWeather(bool forceUpdate = false)
	{
		this.overrideWeather = false;
		if (forceUpdate)
		{
			this.UpdateTimeOfDay(true);
		}
	}

	// Token: 0x06004244 RID: 16964 RVA: 0x0016153D File Offset: 0x0015F73D
	public string GetWeatherString()
	{
		if (!this.overrideWeather)
		{
			return "DEFAULT";
		}
		if (this.overrideWeatherType == BetterDayNightManager.WeatherType.Raining)
		{
			return "RAINING";
		}
		return "DEFAULT";
	}

	// Token: 0x06004245 RID: 16965 RVA: 0x00161561 File Offset: 0x0015F761
	public void SetFixedWeatherNetworked(BetterDayNightManager.WeatherType weather)
	{
		if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient || !SubscriptionManager.IsLocalSubscribed())
		{
			return;
		}
		this.HandleFixedWeather(weather);
		this.photonView.RPC("ChangeFixedWeatherRPC", RpcTarget.Others, new object[] { weather });
	}

	// Token: 0x06004246 RID: 16966 RVA: 0x001615A0 File Offset: 0x0015F7A0
	public void SetTimeOfDayNetworked(int timeIndex)
	{
		if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient || !SubscriptionManager.IsLocalSubscribed())
		{
			return;
		}
		this.HandleTimeOfDay(timeIndex);
		this.photonView.RPC("ChangeTimeOfDayRPC", RpcTarget.Others, new object[] { timeIndex });
	}

	// Token: 0x06004247 RID: 16967 RVA: 0x001615E0 File Offset: 0x0015F7E0
	[PunRPC]
	private void ChangeFixedWeatherRPC(int weather, PhotonMessageInfo info)
	{
		if (info.Sender == null || !info.Sender.IsMasterClient || (!RoomSystem.WasRoomPrivate && !RoomSystem.WasRoomSubscription))
		{
			return;
		}
		if (this.rpcSpamChecks.IsSpamming(BetterDayNightManager.RPC.ChangeFixedWeather))
		{
			return;
		}
		if (weather < 0 || weather > 2)
		{
			return;
		}
		if (RoomSystem.WasRoomPrivate && !SubscriptionManager.IsPlayerSubscribed(info.Sender))
		{
			this.m_fixedDataCache.Pending = true;
			this.m_fixedDataCache.Value = weather;
			return;
		}
		this.m_fixedDataCache.Reset();
		this.HandleFixedWeather((BetterDayNightManager.WeatherType)weather);
	}

	// Token: 0x06004248 RID: 16968 RVA: 0x0016166D File Offset: 0x0015F86D
	private void HandleFixedWeather(BetterDayNightManager.WeatherType weather)
	{
		if (weather < BetterDayNightManager.WeatherType.None || weather > BetterDayNightManager.WeatherType.All)
		{
			return;
		}
		if (weather == BetterDayNightManager.WeatherType.None)
		{
			this.ClearFixedWeather(true);
			GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
			return;
		}
		this.SetFixedWeather(weather, true);
		GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
	}

	// Token: 0x06004249 RID: 16969 RVA: 0x001616A0 File Offset: 0x0015F8A0
	[PunRPC]
	private void ChangeTimeOfDayRPC(int timeIndex, PhotonMessageInfo info)
	{
		if (info.Sender == null || !info.Sender.IsMasterClient || (!RoomSystem.WasRoomPrivate && !RoomSystem.WasRoomSubscription))
		{
			return;
		}
		if (this.rpcSpamChecks.IsSpamming(BetterDayNightManager.RPC.ChangeTimeOfDay))
		{
			return;
		}
		if (timeIndex < -1 || timeIndex >= this.timeOfDayRange.Length)
		{
			return;
		}
		if (RoomSystem.WasRoomPrivate && !SubscriptionManager.IsPlayerSubscribed(info.Sender))
		{
			this.m_setTimeDataCache.Pending = true;
			this.m_setTimeDataCache.Value = timeIndex;
			return;
		}
		this.m_setTimeDataCache.Reset();
		this.HandleTimeOfDay(timeIndex);
	}

	// Token: 0x0600424A RID: 16970 RVA: 0x00161734 File Offset: 0x0015F934
	private void HandleTimeOfDay(int timeIndex)
	{
		if (timeIndex < -1 || timeIndex >= this.timeOfDayRange.Length)
		{
			return;
		}
		if (timeIndex == -1)
		{
			this.ClearTimeOfDay(true);
			GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
			return;
		}
		this.SetTimeOfDay(timeIndex, true);
		GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
	}

	// Token: 0x0600424B RID: 16971 RVA: 0x0016176E File Offset: 0x0015F96E
	private void OnRoomJoin()
	{
		this.ClearTimeOfDay(false);
		this.ClearFixedWeather(false);
		this.m_fixedDataCache.Reset();
		this.m_setTimeDataCache.Reset();
	}

	// Token: 0x0600424C RID: 16972 RVA: 0x00161794 File Offset: 0x0015F994
	private void OnPlayerJoined(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient || !SubscriptionManager.IsLocalSubscribed())
		{
			return;
		}
		if (!PhotonNetwork.IsMasterClient || !SubscriptionManager.IsLocalSubscribed())
		{
			return;
		}
		if (this.overrideWeather)
		{
			this.photonView.RPC("ChangeFixedWeatherRPC", RpcTarget.Others, new object[] { this.overrideWeatherType });
		}
		if (this.currentSetting == TimeSettings.Static)
		{
			this.photonView.RPC("ChangeTimeOfDayRPC", RpcTarget.Others, new object[] { this.lastSentTimeIndex });
		}
	}

	// Token: 0x0600424D RID: 16973 RVA: 0x00161820 File Offset: 0x0015FA20
	private void OnMasterClientSwitched(NetPlayer newMasterClient)
	{
		this.m_fixedDataCache.Reset();
		this.m_setTimeDataCache.Reset();
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!SubscriptionManager.IsLocalSubscribed())
		{
			this.ClearTimeOfDay(false);
			this.ClearFixedWeather(true);
			this.photonView.RPC("ChangeTimeOfDayRPC", RpcTarget.Others, new object[] { -1 });
			this.photonView.RPC("ChangeFixedWeatherRPC", RpcTarget.Others, new object[] { BetterDayNightManager.WeatherType.None });
		}
	}

	// Token: 0x0600424E RID: 16974 RVA: 0x001618A0 File Offset: 0x0015FAA0
	private void OnSubscrptionData()
	{
		if (!this.m_fixedDataCache.Pending || !this.m_setTimeDataCache.Pending)
		{
			return;
		}
		if (!RoomSystem.JoinedRoom)
		{
			this.m_fixedDataCache.Reset();
			this.m_setTimeDataCache.Reset();
			return;
		}
		if (!SubscriptionManager.IsPlayerSubscribed(NetworkSystem.Instance.MasterClient))
		{
			return;
		}
		if (this.m_fixedDataCache.Pending)
		{
			this.HandleFixedWeather((BetterDayNightManager.WeatherType)this.m_fixedDataCache.Value);
			this.m_fixedDataCache.Reset();
		}
		if (this.m_setTimeDataCache.Pending)
		{
			this.HandleTimeOfDay(this.m_setTimeDataCache.Value);
			this.m_setTimeDataCache.Reset();
		}
	}

	// Token: 0x04005382 RID: 21378
	private BetterDayNightManager.RPCDataCache m_fixedDataCache;

	// Token: 0x04005383 RID: 21379
	private BetterDayNightManager.RPCDataCache m_setTimeDataCache;

	// Token: 0x04005384 RID: 21380
	public const int TIME_OF_DAY_COUNT = 10;

	// Token: 0x04005385 RID: 21381
	[OnEnterPlay_SetNull]
	public static volatile BetterDayNightManager instance;

	// Token: 0x04005386 RID: 21382
	[OnEnterPlay_Clear]
	public static List<PerSceneRenderData> allScenesRenderData = new List<PerSceneRenderData>();

	// Token: 0x04005387 RID: 21383
	public PhotonView photonView;

	// Token: 0x04005388 RID: 21384
	public Shader standard;

	// Token: 0x04005389 RID: 21385
	public Shader standardCutout;

	// Token: 0x0400538A RID: 21386
	public Shader gorillaUnlit;

	// Token: 0x0400538B RID: 21387
	public Shader gorillaUnlitCutout;

	// Token: 0x0400538C RID: 21388
	public Material[] dayNightSupportedMaterials;

	// Token: 0x0400538D RID: 21389
	public Material[] dayNightSupportedMaterialsCutout;

	// Token: 0x0400538E RID: 21390
	public string[] dayNightLightmapNames;

	// Token: 0x0400538F RID: 21391
	public string[] dayNightWeatherLightmapNames;

	// Token: 0x04005390 RID: 21392
	public Texture2D[] dayNightSkyboxTextures;

	// Token: 0x04005391 RID: 21393
	public Texture2D[] cloudsDayNightSkyboxTextures;

	// Token: 0x04005392 RID: 21394
	public Texture2D[] beachDayNightSkyboxTextures;

	// Token: 0x04005393 RID: 21395
	public Texture2D[] dayNightWeatherSkyboxTextures;

	// Token: 0x04005394 RID: 21396
	public float[] standardUnlitColor;

	// Token: 0x04005395 RID: 21397
	public float[] standardUnlitColorWithPremadeColorDarker;

	// Token: 0x04005396 RID: 21398
	public float currentLerp;

	// Token: 0x04005397 RID: 21399
	public float currentTimestep;

	// Token: 0x04005398 RID: 21400
	public BetterDayNightManager.Season currentSeason;

	// Token: 0x04005399 RID: 21401
	public double[] summerTimeOfDayRange;

	// Token: 0x0400539A RID: 21402
	public double[] winterTimeOfDayRange;

	// Token: 0x0400539B RID: 21403
	public double timeMultiplier;

	// Token: 0x0400539C RID: 21404
	private float lastTime;

	// Token: 0x0400539D RID: 21405
	private double currentTime;

	// Token: 0x0400539E RID: 21406
	private double totalHours;

	// Token: 0x0400539F RID: 21407
	private double totalSeconds;

	// Token: 0x040053A0 RID: 21408
	private float colorFrom;

	// Token: 0x040053A1 RID: 21409
	private float colorTo;

	// Token: 0x040053A2 RID: 21410
	private float colorFromDarker;

	// Token: 0x040053A3 RID: 21411
	private float colorToDarker;

	// Token: 0x040053A4 RID: 21412
	public int currentTimeIndex;

	// Token: 0x040053A5 RID: 21413
	public int currentWeatherIndex;

	// Token: 0x040053A6 RID: 21414
	private int lastIndex;

	// Token: 0x040053A7 RID: 21415
	private double currentIndexSeconds;

	// Token: 0x040053A8 RID: 21416
	private double baseSeconds;

	// Token: 0x040053A9 RID: 21417
	private bool computerInit;

	// Token: 0x040053AA RID: 21418
	public int mySeed;

	// Token: 0x040053AB RID: 21419
	public Random randomNumberGenerator;

	// Token: 0x040053AC RID: 21420
	public BetterDayNightManager.WeatherType[] weatherCycle;

	// Token: 0x040053AD RID: 21421
	public bool overrideWeather;

	// Token: 0x040053AE RID: 21422
	public BetterDayNightManager.WeatherType overrideWeatherType;

	// Token: 0x040053B0 RID: 21424
	public float rainChance = 0.3f;

	// Token: 0x040053B1 RID: 21425
	public int maxRainDuration = 5;

	// Token: 0x040053B2 RID: 21426
	private int rainDuration;

	// Token: 0x040053B3 RID: 21427
	private float remainingSeconds;

	// Token: 0x040053B4 RID: 21428
	private long initialDayCycles;

	// Token: 0x040053B5 RID: 21429
	private long gameEpochDay;

	// Token: 0x040053B6 RID: 21430
	private int currentWeatherCycle;

	// Token: 0x040053B7 RID: 21431
	private int fromWeatherIndex;

	// Token: 0x040053B8 RID: 21432
	private int toWeatherIndex;

	// Token: 0x040053B9 RID: 21433
	private Texture2D fromSky;

	// Token: 0x040053BA RID: 21434
	private Texture2D fromSky2;

	// Token: 0x040053BB RID: 21435
	private Texture2D fromSky3;

	// Token: 0x040053BC RID: 21436
	private Texture2D toSky;

	// Token: 0x040053BD RID: 21437
	private Texture2D toSky2;

	// Token: 0x040053BE RID: 21438
	private Texture2D toSky3;

	// Token: 0x040053BF RID: 21439
	public AddCollidersToParticleSystemTriggers[] weatherSystems;

	// Token: 0x040053C0 RID: 21440
	public List<Collider> collidersToAddToWeatherSystems = new List<Collider>();

	// Token: 0x040053C1 RID: 21441
	private float lastTimeChecked;

	// Token: 0x040053C2 RID: 21442
	private Func<int, int> timeIndexOverrideFunc;

	// Token: 0x040053C3 RID: 21443
	private int lastSentTimeIndex = -1;

	// Token: 0x040053C4 RID: 21444
	public CallLimitersList<CallLimiter, BetterDayNightManager.RPC> rpcSpamChecks = new CallLimitersList<CallLimiter, BetterDayNightManager.RPC>();

	// Token: 0x040053C5 RID: 21445
	public int overrideIndex = -1;

	// Token: 0x040053C6 RID: 21446
	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, BetterDayNightManager.ScheduledEvent> scheduledEvents = new Dictionary<int, BetterDayNightManager.ScheduledEvent>(256);

	// Token: 0x040053C7 RID: 21447
	public TimeSettings currentSetting;

	// Token: 0x040053C8 RID: 21448
	private ShaderHashId _GT_DayCycleTimeProgress = "_GT_DayCycleTimeProgress";

	// Token: 0x040053C9 RID: 21449
	private ShaderHashId _GT_DayCycleBrightnessOption1_Id = "_GT_DayCycleBrightnessOption1";

	// Token: 0x040053CA RID: 21450
	private ShaderHashId _GT_DayCycleBrightnessOption2_Id = "_GT_DayCycleBrightnessOption2";

	// Token: 0x040053CB RID: 21451
	private ShaderHashId _GlobalDayNightLerpValue = "_GlobalDayNightLerpValue";

	// Token: 0x040053CC RID: 21452
	private ShaderHashId _GlobalDayNightSkyTex1 = "_GlobalDayNightSkyTex1";

	// Token: 0x040053CD RID: 21453
	private ShaderHashId _GlobalDayNightSkyTex2 = "_GlobalDayNightSkyTex2";

	// Token: 0x040053CE RID: 21454
	private ShaderHashId _GlobalDayNightSky2Tex1 = "_GlobalDayNightSky2Tex1";

	// Token: 0x040053CF RID: 21455
	private ShaderHashId _GlobalDayNightSky2Tex2 = "_GlobalDayNightSky2Tex2";

	// Token: 0x040053D0 RID: 21456
	private ShaderHashId _GlobalDayNightSky3Tex1 = "_GlobalDayNightSky3Tex1";

	// Token: 0x040053D1 RID: 21457
	private ShaderHashId _GlobalDayNightSky3Tex2 = "_GlobalDayNightSky3Tex2";

	// Token: 0x040053D2 RID: 21458
	private bool shouldRepopulate;

	// Token: 0x040053D3 RID: 21459
	private Coroutine animatingLightFlash;

	// Token: 0x02000A12 RID: 2578
	private struct RPCDataCache
	{
		// Token: 0x06004251 RID: 16977 RVA: 0x00161A50 File Offset: 0x0015FC50
		public void Reset()
		{
			this.Pending = false;
			this.Value = 0;
		}

		// Token: 0x040053D4 RID: 21460
		public bool Pending;

		// Token: 0x040053D5 RID: 21461
		public int Value;
	}

	// Token: 0x02000A13 RID: 2579
	public enum Season
	{
		// Token: 0x040053D7 RID: 21463
		Winter,
		// Token: 0x040053D8 RID: 21464
		Spring,
		// Token: 0x040053D9 RID: 21465
		Summer,
		// Token: 0x040053DA RID: 21466
		Fall
	}

	// Token: 0x02000A14 RID: 2580
	public enum WeatherType
	{
		// Token: 0x040053DC RID: 21468
		None,
		// Token: 0x040053DD RID: 21469
		Raining,
		// Token: 0x040053DE RID: 21470
		All
	}

	// Token: 0x02000A15 RID: 2581
	public enum RPC
	{
		// Token: 0x040053E0 RID: 21472
		ChangeFixedWeather,
		// Token: 0x040053E1 RID: 21473
		ChangeTimeOfDay
	}

	// Token: 0x02000A16 RID: 2582
	private class ScheduledEvent
	{
		// Token: 0x040053E2 RID: 21474
		public long lastDayCalled;

		// Token: 0x040053E3 RID: 21475
		public int hour;

		// Token: 0x040053E4 RID: 21476
		public Action action;
	}
}
