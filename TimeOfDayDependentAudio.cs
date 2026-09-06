using System;
using UnityEngine;

// Token: 0x020003A9 RID: 937
public class TimeOfDayDependentAudio : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x060016B6 RID: 5814 RVA: 0x00083778 File Offset: 0x00081978
	private void Awake()
	{
		this.stepTime = 1f;
		if (this.myParticleSystem != null)
		{
			this.myEmissionModule = this.myParticleSystem.emission;
			this.startingEmissionRate = this.myEmissionModule.rateOverTime.constant;
		}
		this.positionMultiplier = (this.isModified ? this.positionMultiplierSet : 1f);
		if (this.volumes == null)
		{
			this.volumes = new float[10];
		}
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x000837F8 File Offset: 0x000819F8
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x00083801 File Offset: 0x00081A01
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x0008380B File Offset: 0x00081A0B
	public void SliceUpdate()
	{
		this.isModified = false;
		this.UpdateTimeOfDay();
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x0008381C File Offset: 0x00081A1C
	private void UpdateTimeOfDay()
	{
		if (BetterDayNightManager.instance == null)
		{
			return;
		}
		BetterDayNightManager.WeatherType weatherType = BetterDayNightManager.instance.CurrentWeather();
		BetterDayNightManager.WeatherType weatherType2 = BetterDayNightManager.instance.NextWeather();
		bool flag = this.myWeather == BetterDayNightManager.WeatherType.All || this.myWeather == weatherType || this.myWeather == weatherType2;
		bool flag2 = this.myWeather != BetterDayNightManager.WeatherType.All && weatherType != weatherType2;
		int currentTimeIndex = BetterDayNightManager.instance.currentTimeIndex;
		int num = (currentTimeIndex + 1) % BetterDayNightManager.instance.timeOfDayRange.Length;
		int num2 = (currentTimeIndex - 1) % BetterDayNightManager.instance.timeOfDayRange.Length;
		if (num2 < 0)
		{
			num2 = BetterDayNightManager.instance.timeOfDayRange.Length - 1;
		}
		float currentLerp = BetterDayNightManager.instance.currentLerp;
		if (!flag)
		{
			if (this.dependentStuff.activeSelf)
			{
				this.dependentStuff.SetActive(false);
			}
			return;
		}
		if (!this.dependentStuff.activeSelf && (!this.includesAudio || this.dependentStuff != this.timeOfDayDependent))
		{
			this.dependentStuff.SetActive(true);
		}
		if (this.includesAudio && this.timeOfDayDependent != null)
		{
			bool flag3 = this.volumes[currentTimeIndex] != 0f;
			if (this.timeOfDayDependent.activeSelf != flag3)
			{
				this.timeOfDayDependent.SetActive(flag3);
			}
		}
		if (!flag2)
		{
			this.newRate = this.startingEmissionRate;
			this.currentVolume = Mathf.Lerp(this.volumes[num2], this.volumes[currentTimeIndex], Mathf.Clamp(currentLerp * 20f, 0f, 1f));
		}
		else if (this.myWeather == weatherType2)
		{
			float num3 = Mathf.Clamp(currentLerp * 2f - 1f, 0f, 1f);
			this.newRate = Mathf.Lerp(0f, this.startingEmissionRate, num3);
			this.currentVolume = Mathf.Lerp(0f, this.volumes[num], currentLerp);
		}
		else
		{
			float num4 = Mathf.Clamp(currentLerp * 2f, 0f, 1f);
			this.newRate = Mathf.Lerp(this.startingEmissionRate, 0f, num4);
			this.currentVolume = Mathf.Lerp(this.volumes[currentTimeIndex], 0f, currentLerp);
		}
		if (this.myParticleSystem != null)
		{
			this.myEmissionModule = this.myParticleSystem.emission;
			this.myEmissionModule.rateOverTime = this.newRate;
			bool flag4 = this.newRate != 0f;
			if (this.myParticleSystem.gameObject.activeSelf != flag4)
			{
				this.myParticleSystem.gameObject.SetActive(flag4);
			}
		}
		if (this.includesAudio)
		{
			for (int i = 0; i < this.audioSources.Length; i++)
			{
				MusicSource component = this.audioSources[i].gameObject.GetComponent<MusicSource>();
				if (!(component != null) || !component.VolumeOverridden)
				{
					this.audioSources[i].volume = this.currentVolume * this.positionMultiplier;
					this.audioSources[i].enabled = this.currentVolume != 0f;
				}
			}
		}
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x00083B54 File Offset: 0x00081D54
	public bool BuildValidationCheck()
	{
		for (int i = 0; i < this.audioSources.Length; i++)
		{
			if (this.audioSources[i] == null)
			{
				Debug.LogError("audio source array contains null references", this);
				return false;
			}
		}
		if (this.volumes.Length != 10)
		{
			Debug.LogError("volumes array is the wrong length! you'll pay for this!", this);
			return false;
		}
		return true;
	}

	// Token: 0x040020CD RID: 8397
	public AudioSource[] audioSources;

	// Token: 0x040020CE RID: 8398
	public float[] volumes;

	// Token: 0x040020CF RID: 8399
	public float currentVolume;

	// Token: 0x040020D0 RID: 8400
	public float stepTime;

	// Token: 0x040020D1 RID: 8401
	public BetterDayNightManager.WeatherType myWeather;

	// Token: 0x040020D2 RID: 8402
	public GameObject dependentStuff;

	// Token: 0x040020D3 RID: 8403
	public GameObject timeOfDayDependent;

	// Token: 0x040020D4 RID: 8404
	public bool includesAudio;

	// Token: 0x040020D5 RID: 8405
	public ParticleSystem myParticleSystem;

	// Token: 0x040020D6 RID: 8406
	private float startingEmissionRate;

	// Token: 0x040020D7 RID: 8407
	private ParticleSystem.MinMaxCurve newCurve;

	// Token: 0x040020D8 RID: 8408
	private ParticleSystem.EmissionModule myEmissionModule;

	// Token: 0x040020D9 RID: 8409
	private float newRate;

	// Token: 0x040020DA RID: 8410
	public float positionMultiplierSet;

	// Token: 0x040020DB RID: 8411
	public float positionMultiplier = 1f;

	// Token: 0x040020DC RID: 8412
	public bool isModified;
}
