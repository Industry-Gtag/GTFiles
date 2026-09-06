using System;
using System.Collections;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020009E8 RID: 2536
public class VolcanoEffects : MonoBehaviour
{
	// Token: 0x06004111 RID: 16657 RVA: 0x0015AC9C File Offset: 0x00158E9C
	private void Awake()
	{
		if (this.RemoveNullsFromArray<ParticleSystem>(ref this.lavaSpewParticleSystems))
		{
			this.LogNullsFoundInArray("lavaSpewParticleSystems");
		}
		if (this.RemoveNullsFromArray<ParticleSystem>(ref this.smokeParticleSystems))
		{
			this.LogNullsFoundInArray("smokeParticleSystems");
		}
		this.hasVolcanoAudioSrc = this.volcanoAudioSource != null;
		this.hasForestSpeakerAudioSrc = this.forestSpeakerAudioSrc != null;
		this.lavaSpewEmissionModules = new ParticleSystem.EmissionModule[this.lavaSpewParticleSystems.Length];
		this.lavaSpewEmissionDefaultRateMultipliers = new float[this.lavaSpewParticleSystems.Length];
		this.lavaSpewDefaultEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
		this.lavaSpewAdjustedEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
		for (int i = 0; i < this.lavaSpewParticleSystems.Length; i++)
		{
			ParticleSystem.EmissionModule emission = this.lavaSpewParticleSystems[i].emission;
			this.lavaSpewEmissionDefaultRateMultipliers[i] = emission.rateOverTimeMultiplier;
			this.lavaSpewDefaultEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
			this.lavaSpewAdjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
			for (int j = 0; j < emission.burstCount; j++)
			{
				ParticleSystem.Burst burst = emission.GetBurst(j);
				this.lavaSpewDefaultEmitBursts[i][j] = burst;
				this.lavaSpewAdjustedEmitBursts[i][j] = new ParticleSystem.Burst(burst.time, burst.minCount, burst.maxCount, burst.cycleCount, burst.repeatInterval);
				this.lavaSpewAdjustedEmitBursts[i][j].count = burst.count;
			}
			this.lavaSpewEmissionModules[i] = emission;
		}
		this.smokeMainModules = new ParticleSystem.MainModule[this.smokeParticleSystems.Length];
		this.smokeEmissionModules = new ParticleSystem.EmissionModule[this.smokeParticleSystems.Length];
		this.smokeEmissionDefaultRateMultipliers = new float[this.smokeParticleSystems.Length];
		for (int k = 0; k < this.smokeParticleSystems.Length; k++)
		{
			this.smokeMainModules[k] = this.smokeParticleSystems[k].main;
			this.smokeEmissionModules[k] = this.smokeParticleSystems[k].emission;
			this.smokeEmissionDefaultRateMultipliers[k] = this.smokeEmissionModules[k].rateOverTimeMultiplier;
		}
		this.InitState(this.drainedStateFX);
		this.InitState(this.eruptingStateFX);
		this.InitState(this.risingStateFX);
		this.InitState(this.fullStateFX);
		this.InitState(this.drainingStateFX);
		this.currentStateFX = this.drainedStateFX;
		this.UpdateDrainedState(0f);
	}

	// Token: 0x06004112 RID: 16658 RVA: 0x0015AF24 File Offset: 0x00159124
	public void PreloadAssets()
	{
		VolcanoEffects.PreloadClip(this.warnVolcanoBellyEmptied);
		VolcanoEffects.PreloadClip(this.volcanoAcceptStone);
		VolcanoEffects.PreloadClip(this.volcanoAcceptLastStone);
		VolcanoEffects.PreloadStateFXClips(this.drainedStateFX);
		VolcanoEffects.PreloadStateFXClips(this.eruptingStateFX);
		VolcanoEffects.PreloadStateFXClips(this.risingStateFX);
		VolcanoEffects.PreloadStateFXClips(this.fullStateFX);
		VolcanoEffects.PreloadStateFXClips(this.drainingStateFX);
		VolcanoEffects.WarmUpAudioSourceGO(this.forestSpeakerAudioSrc);
		VolcanoEffects.WarmUpAudioSourceGO(this.volcanoAudioSource);
		VolcanoEffects.WarmUpStateFXSources(this.drainedStateFX);
		VolcanoEffects.WarmUpStateFXSources(this.eruptingStateFX);
		VolcanoEffects.WarmUpStateFXSources(this.risingStateFX);
		VolcanoEffects.WarmUpStateFXSources(this.fullStateFX);
		VolcanoEffects.WarmUpStateFXSources(this.drainingStateFX);
		for (int i = 0; i < this.lavaSurfaceAudioSrcs.Length; i++)
		{
			VolcanoEffects.WarmUpAudioSourceGO(this.lavaSurfaceAudioSrcs[i]);
		}
		if (this.prewarmCoroutine != null)
		{
			base.StopCoroutine(this.prewarmCoroutine);
		}
		this.prewarmCoroutine = base.StartCoroutine(this._PrewarmLavaSpewRenderers());
	}

	// Token: 0x06004113 RID: 16659 RVA: 0x0015B01C File Offset: 0x0015921C
	private static void PreloadClip(AudioClip clip)
	{
		if (clip != null && clip.loadState != AudioDataLoadState.Loaded)
		{
			clip.LoadAudioData();
		}
	}

	// Token: 0x06004114 RID: 16660 RVA: 0x0015B038 File Offset: 0x00159238
	private static void PreloadStateFXClips(VolcanoEffects.LavaStateFX fx)
	{
		VolcanoEffects.PreloadClip(fx.startSound);
		VolcanoEffects.PreloadClip(fx.endSound);
		if (fx.loop1AudioSrc != null && fx.loop1AudioSrc.clip != null)
		{
			VolcanoEffects.PreloadClip(fx.loop1AudioSrc.clip);
		}
		if (fx.loop2AudioSrc != null && fx.loop2AudioSrc.clip != null)
		{
			VolcanoEffects.PreloadClip(fx.loop2AudioSrc.clip);
		}
	}

	// Token: 0x06004115 RID: 16661 RVA: 0x0015B0C0 File Offset: 0x001592C0
	private static void WarmUpAudioSourceGO(AudioSource src)
	{
		if (src == null)
		{
			return;
		}
		GameObject gameObject = src.gameObject;
		if (gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(true);
		gameObject.SetActive(false);
	}

	// Token: 0x06004116 RID: 16662 RVA: 0x0015B0F8 File Offset: 0x001592F8
	private static void WarmUpStateFXSources(VolcanoEffects.LavaStateFX fx)
	{
		if (fx.startSoundExists)
		{
			VolcanoEffects.WarmUpAudioSourceGO(fx.startSoundAudioSrc);
		}
		if (fx.endSoundExists)
		{
			VolcanoEffects.WarmUpAudioSourceGO(fx.endSoundAudioSrc);
		}
		if (fx.loop1Exists)
		{
			VolcanoEffects.WarmUpAudioSourceGO(fx.loop1AudioSrc);
		}
		if (fx.loop2Exists)
		{
			VolcanoEffects.WarmUpAudioSourceGO(fx.loop2AudioSrc);
		}
	}

	// Token: 0x06004117 RID: 16663 RVA: 0x0015B151 File Offset: 0x00159351
	private IEnumerator _PrewarmLavaSpewRenderers()
	{
		for (int i = 0; i < this.lavaSpewParticleSystems.Length; i++)
		{
			this.lavaSpewParticleSystems[i].Emit(1);
		}
		yield return null;
		for (int j = 0; j < this.lavaSpewParticleSystems.Length; j++)
		{
			this.lavaSpewParticleSystems[j].Clear(true);
		}
		this.prewarmCoroutine = null;
		yield break;
	}

	// Token: 0x06004118 RID: 16664 RVA: 0x0015B160 File Offset: 0x00159360
	private void OnDisable()
	{
		if (this.prewarmCoroutine != null)
		{
			base.StopCoroutine(this.prewarmCoroutine);
			this.prewarmCoroutine = null;
		}
	}

	// Token: 0x06004119 RID: 16665 RVA: 0x0015B180 File Offset: 0x00159380
	public void OnVolcanoBellyEmpty()
	{
		if (!this.hasForestSpeakerAudioSrc)
		{
			return;
		}
		if (Time.time - this.timeVolcanoBellyWasLastEmpty < this.warnVolcanoBellyEmptied.length)
		{
			return;
		}
		this.forestSpeakerAudioSrc.gameObject.SetActive(true);
		this.forestSpeakerAudioSrc.GTPlayOneShot(this.warnVolcanoBellyEmptied, 1f);
	}

	// Token: 0x0600411A RID: 16666 RVA: 0x0015B1D8 File Offset: 0x001593D8
	public void OnStoneAccepted(float activationProgress)
	{
		if (!this.hasVolcanoAudioSrc)
		{
			return;
		}
		this.volcanoAudioSource.gameObject.SetActive(true);
		if (activationProgress > 1f)
		{
			this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptLastStone, 1f);
			return;
		}
		this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptStone, 1f);
	}

	// Token: 0x0600411B RID: 16667 RVA: 0x0015B234 File Offset: 0x00159434
	private void InitState(VolcanoEffects.LavaStateFX fx)
	{
		fx.startSoundExists = fx.startSound != null;
		fx.endSoundExists = fx.endSound != null;
		fx.loop1Exists = fx.loop1AudioSrc != null;
		fx.loop2Exists = fx.loop2AudioSrc != null;
		if (fx.loop1Exists)
		{
			fx.loop1DefaultVolume = fx.loop1AudioSrc.volume;
			fx.loop1AudioSrc.volume = 0f;
		}
		if (fx.loop2Exists)
		{
			fx.loop2DefaultVolume = fx.loop2AudioSrc.volume;
			fx.loop2AudioSrc.volume = 0f;
		}
	}

	// Token: 0x0600411C RID: 16668 RVA: 0x0015B2DC File Offset: 0x001594DC
	private void SetLavaAudioEnabled(bool toEnable)
	{
		AudioSource[] array = this.lavaSurfaceAudioSrcs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(toEnable);
		}
	}

	// Token: 0x0600411D RID: 16669 RVA: 0x0015B30C File Offset: 0x0015950C
	private void SetLavaAudioEnabled(bool toEnable, float volume)
	{
		foreach (AudioSource audioSource in this.lavaSurfaceAudioSrcs)
		{
			audioSource.volume = volume;
			audioSource.gameObject.SetActive(toEnable);
		}
	}

	// Token: 0x0600411E RID: 16670 RVA: 0x0015B344 File Offset: 0x00159544
	private void ResetState()
	{
		if (this.currentStateFX == null)
		{
			return;
		}
		this.currentStateFX.startSoundPlayed = false;
		this.currentStateFX.endSoundPlayed = false;
		if (this.currentStateFX.startSoundExists)
		{
			this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(false);
		}
		if (this.currentStateFX.endSoundExists)
		{
			this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(false);
		}
		if (this.currentStateFX.loop1Exists)
		{
			this.currentStateFX.loop1AudioSrc.gameObject.SetActive(false);
		}
		if (this.currentStateFX.loop2Exists)
		{
			this.currentStateFX.loop2AudioSrc.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600411F RID: 16671 RVA: 0x0015B400 File Offset: 0x00159600
	private void UpdateState(float time, float timeRemaining, float progress)
	{
		if (this.currentStateFX == null)
		{
			return;
		}
		if (this.currentStateFX.startSoundExists && !this.currentStateFX.startSoundPlayed && time >= this.currentStateFX.startSoundDelay)
		{
			this.currentStateFX.startSoundPlayed = true;
			this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(true);
			this.currentStateFX.startSoundAudioSrc.GTPlayOneShot(this.currentStateFX.startSound, this.currentStateFX.startSoundVol);
		}
		if (this.currentStateFX.endSoundExists && !this.currentStateFX.endSoundPlayed && timeRemaining <= this.currentStateFX.endSound.length + this.currentStateFX.endSoundPadTime)
		{
			this.currentStateFX.endSoundPlayed = true;
			this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(true);
			this.currentStateFX.endSoundAudioSrc.GTPlayOneShot(this.currentStateFX.endSound, this.currentStateFX.endSoundVol);
		}
		if (this.currentStateFX.loop1Exists)
		{
			this.currentStateFX.loop1AudioSrc.volume = this.currentStateFX.loop1VolAnim.Evaluate(progress) * this.currentStateFX.loop1DefaultVolume;
			if (!this.currentStateFX.loop1AudioSrc.isPlaying)
			{
				this.currentStateFX.loop1AudioSrc.gameObject.SetActive(true);
				this.currentStateFX.loop1AudioSrc.GTPlay();
			}
		}
		if (this.currentStateFX.loop2Exists)
		{
			this.currentStateFX.loop2AudioSrc.volume = this.currentStateFX.loop2VolAnim.Evaluate(progress) * this.currentStateFX.loop2DefaultVolume;
			if (!this.currentStateFX.loop2AudioSrc.isPlaying)
			{
				this.currentStateFX.loop2AudioSrc.gameObject.SetActive(true);
				this.currentStateFX.loop2AudioSrc.GTPlay();
			}
		}
		for (int i = 0; i < this.smokeMainModules.Length; i++)
		{
			this.smokeMainModules[i].startColor = this.currentStateFX.smokeStartColorAnim.Evaluate(progress);
			this.smokeEmissionModules[i].rateOverTimeMultiplier = this.currentStateFX.smokeEmissionAnim.Evaluate(progress) * this.smokeEmissionDefaultRateMultipliers[i];
		}
		this.SetParticleEmissionRateAndBurst(this.currentStateFX.lavaSpewEmissionAnim.Evaluate(progress), this.lavaSpewEmissionModules, this.lavaSpewEmissionDefaultRateMultipliers, this.lavaSpewDefaultEmitBursts, this.lavaSpewAdjustedEmitBursts);
		if (this.applyShaderGlobals)
		{
			Shader.SetGlobalColor(this.shaderProp_ZoneLiquidLightColor, this.currentStateFX.lavaLightColor.Evaluate(progress) * this.currentStateFX.lavaLightIntensityAnim.Evaluate(progress));
			Shader.SetGlobalFloat(this.shaderProp_ZoneLiquidLightDistScale, this.currentStateFX.lavaLightAttenuationAnim.Evaluate(progress));
		}
	}

	// Token: 0x06004120 RID: 16672 RVA: 0x0015B6D5 File Offset: 0x001598D5
	public void SetDrainedState()
	{
		this.ResetState();
		this.SetLavaAudioEnabled(false);
		this.currentStateFX = this.drainedStateFX;
	}

	// Token: 0x06004121 RID: 16673 RVA: 0x0015B6F0 File Offset: 0x001598F0
	public void UpdateDrainedState(float time)
	{
		this.UpdateState(time, float.MaxValue, float.MinValue);
	}

	// Token: 0x06004122 RID: 16674 RVA: 0x0015B703 File Offset: 0x00159903
	public void SetEruptingState()
	{
		this.ResetState();
		this.SetLavaAudioEnabled(false, 0f);
		this.currentStateFX = this.eruptingStateFX;
	}

	// Token: 0x06004123 RID: 16675 RVA: 0x0015B723 File Offset: 0x00159923
	public void UpdateEruptingState(float time, float timeRemaining, float progress)
	{
		this.UpdateState(time, timeRemaining, progress);
	}

	// Token: 0x06004124 RID: 16676 RVA: 0x0015B72E File Offset: 0x0015992E
	public void SetRisingState()
	{
		this.ResetState();
		this.SetLavaAudioEnabled(true, 0f);
		this.currentStateFX = this.risingStateFX;
	}

	// Token: 0x06004125 RID: 16677 RVA: 0x0015B750 File Offset: 0x00159950
	public void UpdateRisingState(float time, float timeRemaining, float progress)
	{
		this.UpdateState(time, timeRemaining, progress);
		AudioSource[] array = this.lavaSurfaceAudioSrcs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].volume = Mathf.Lerp(0f, 1f, Mathf.Clamp01(time));
		}
	}

	// Token: 0x06004126 RID: 16678 RVA: 0x0015B798 File Offset: 0x00159998
	public void SetFullState()
	{
		this.ResetState();
		this.SetLavaAudioEnabled(true, 1f);
		this.currentStateFX = this.fullStateFX;
	}

	// Token: 0x06004127 RID: 16679 RVA: 0x0015B723 File Offset: 0x00159923
	public void UpdateFullState(float time, float timeRemaining, float progress)
	{
		this.UpdateState(time, timeRemaining, progress);
	}

	// Token: 0x06004128 RID: 16680 RVA: 0x0015B7B8 File Offset: 0x001599B8
	public void SetDrainingState()
	{
		this.ResetState();
		this.SetLavaAudioEnabled(true, 1f);
		this.currentStateFX = this.drainingStateFX;
	}

	// Token: 0x06004129 RID: 16681 RVA: 0x0015B7D8 File Offset: 0x001599D8
	public void UpdateDrainingState(float time, float timeRemaining, float progress)
	{
		this.UpdateState(time, timeRemaining, progress);
		AudioSource[] array = this.lavaSurfaceAudioSrcs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].volume = Mathf.Lerp(1f, 0f, progress);
		}
	}

	// Token: 0x0600412A RID: 16682 RVA: 0x0015B81C File Offset: 0x00159A1C
	private void SetParticleEmissionRateAndBurst(float multiplier, ParticleSystem.EmissionModule[] emissionModules, float[] defaultRateMultipliers, ParticleSystem.Burst[][] defaultEmitBursts, ParticleSystem.Burst[][] adjustedEmitBursts)
	{
		for (int i = 0; i < emissionModules.Length; i++)
		{
			emissionModules[i].rateOverTimeMultiplier = multiplier * defaultRateMultipliers[i];
			int num = Mathf.Min(emissionModules[i].burstCount, defaultEmitBursts[i].Length);
			for (int j = 0; j < num; j++)
			{
				adjustedEmitBursts[i][j].probability = defaultEmitBursts[i][j].probability * multiplier;
			}
			emissionModules[i].SetBursts(adjustedEmitBursts[i]);
		}
	}

	// Token: 0x0600412B RID: 16683 RVA: 0x0015B89C File Offset: 0x00159A9C
	private bool RemoveNullsFromArray<T>(ref T[] array) where T : Object
	{
		List<T> list = new List<T>(array.Length);
		foreach (T t in array)
		{
			if (t != null)
			{
				list.Add(t);
			}
		}
		int num = array.Length;
		array = list.ToArray();
		return num != array.Length;
	}

	// Token: 0x0600412C RID: 16684 RVA: 0x0015B8F6 File Offset: 0x00159AF6
	private void LogNullsFoundInArray(string nameOfArray)
	{
		Debug.LogError(string.Concat(new string[]
		{
			"Null reference found in ",
			nameOfArray,
			" array of component: \"",
			this.GetComponentPath(int.MaxValue),
			"\""
		}), this);
	}

	// Token: 0x040051AA RID: 20906
	[Tooltip("Only one VolcanoEffects should change shader globals in the scene (lava color, lava light) at a time.")]
	[SerializeField]
	private bool applyShaderGlobals = true;

	// Token: 0x040051AB RID: 20907
	[Tooltip("Game trigger notification sounds will play through this.")]
	[SerializeField]
	private AudioSource forestSpeakerAudioSrc;

	// Token: 0x040051AC RID: 20908
	[Tooltip("The accumulator value of rocks being thrown into the volcano has been reset.")]
	[SerializeField]
	private AudioClip warnVolcanoBellyEmptied;

	// Token: 0x040051AD RID: 20909
	[Tooltip("Accept stone sounds will play through here.")]
	[SerializeField]
	private AudioSource volcanoAudioSource;

	// Token: 0x040051AE RID: 20910
	[Tooltip("volcano ate rock but needs more.")]
	[SerializeField]
	private AudioClip volcanoAcceptStone;

	// Token: 0x040051AF RID: 20911
	[Tooltip("volcano ate last needed rock.")]
	[SerializeField]
	private AudioClip volcanoAcceptLastStone;

	// Token: 0x040051B0 RID: 20912
	[Tooltip("This will be faded in while lava is rising.")]
	[SerializeField]
	private AudioSource[] lavaSurfaceAudioSrcs;

	// Token: 0x040051B1 RID: 20913
	[Tooltip("Emission will be adjusted for these particles during eruption.")]
	[SerializeField]
	private ParticleSystem[] lavaSpewParticleSystems;

	// Token: 0x040051B2 RID: 20914
	[Tooltip("Smoke emits during all states but it's intensity and color will change when erupting/idling.")]
	[SerializeField]
	private ParticleSystem[] smokeParticleSystems;

	// Token: 0x040051B3 RID: 20915
	[SerializeField]
	private VolcanoEffects.LavaStateFX drainedStateFX;

	// Token: 0x040051B4 RID: 20916
	[SerializeField]
	private VolcanoEffects.LavaStateFX eruptingStateFX;

	// Token: 0x040051B5 RID: 20917
	[SerializeField]
	private VolcanoEffects.LavaStateFX risingStateFX;

	// Token: 0x040051B6 RID: 20918
	[SerializeField]
	private VolcanoEffects.LavaStateFX fullStateFX;

	// Token: 0x040051B7 RID: 20919
	[SerializeField]
	private VolcanoEffects.LavaStateFX drainingStateFX;

	// Token: 0x040051B8 RID: 20920
	private VolcanoEffects.LavaStateFX currentStateFX;

	// Token: 0x040051B9 RID: 20921
	private ParticleSystem.EmissionModule[] lavaSpewEmissionModules;

	// Token: 0x040051BA RID: 20922
	private float[] lavaSpewEmissionDefaultRateMultipliers;

	// Token: 0x040051BB RID: 20923
	private ParticleSystem.Burst[][] lavaSpewDefaultEmitBursts;

	// Token: 0x040051BC RID: 20924
	private ParticleSystem.Burst[][] lavaSpewAdjustedEmitBursts;

	// Token: 0x040051BD RID: 20925
	private ParticleSystem.MainModule[] smokeMainModules;

	// Token: 0x040051BE RID: 20926
	private ParticleSystem.EmissionModule[] smokeEmissionModules;

	// Token: 0x040051BF RID: 20927
	private float[] smokeEmissionDefaultRateMultipliers;

	// Token: 0x040051C0 RID: 20928
	private readonly int shaderProp_ZoneLiquidLightColor = Shader.PropertyToID("_ZoneLiquidLightColor");

	// Token: 0x040051C1 RID: 20929
	private readonly int shaderProp_ZoneLiquidLightDistScale = Shader.PropertyToID("_ZoneLiquidLightDistScale");

	// Token: 0x040051C2 RID: 20930
	private float timeVolcanoBellyWasLastEmpty;

	// Token: 0x040051C3 RID: 20931
	private bool hasVolcanoAudioSrc;

	// Token: 0x040051C4 RID: 20932
	private bool hasForestSpeakerAudioSrc;

	// Token: 0x040051C5 RID: 20933
	private Coroutine prewarmCoroutine;

	// Token: 0x020009E9 RID: 2537
	[Serializable]
	public class LavaStateFX
	{
		// Token: 0x040051C6 RID: 20934
		public AudioClip startSound;

		// Token: 0x040051C7 RID: 20935
		public AudioSource startSoundAudioSrc;

		// Token: 0x040051C8 RID: 20936
		[Tooltip("Multiplied by the AudioSource's volume.")]
		public float startSoundVol = 1f;

		// Token: 0x040051C9 RID: 20937
		[FormerlySerializedAs("startSoundPad")]
		public float startSoundDelay;

		// Token: 0x040051CA RID: 20938
		public AudioClip endSound;

		// Token: 0x040051CB RID: 20939
		public AudioSource endSoundAudioSrc;

		// Token: 0x040051CC RID: 20940
		[Tooltip("Multiplied by the AudioSource's volume.")]
		public float endSoundVol = 1f;

		// Token: 0x040051CD RID: 20941
		[Tooltip("How much time should there be between the end of the clip playing and the end of the state.")]
		public float endSoundPadTime;

		// Token: 0x040051CE RID: 20942
		public AudioSource loop1AudioSrc;

		// Token: 0x040051CF RID: 20943
		public AnimationCurve loop1VolAnim;

		// Token: 0x040051D0 RID: 20944
		public AudioSource loop2AudioSrc;

		// Token: 0x040051D1 RID: 20945
		public AnimationCurve loop2VolAnim;

		// Token: 0x040051D2 RID: 20946
		public AnimationCurve lavaSpewEmissionAnim;

		// Token: 0x040051D3 RID: 20947
		public AnimationCurve smokeEmissionAnim;

		// Token: 0x040051D4 RID: 20948
		public Gradient smokeStartColorAnim;

		// Token: 0x040051D5 RID: 20949
		public Gradient lavaLightColor;

		// Token: 0x040051D6 RID: 20950
		public AnimationCurve lavaLightIntensityAnim = AnimationCurve.Constant(0f, 1f, 60f);

		// Token: 0x040051D7 RID: 20951
		public AnimationCurve lavaLightAttenuationAnim = AnimationCurve.Constant(0f, 1f, 0.1f);

		// Token: 0x040051D8 RID: 20952
		[NonSerialized]
		public bool startSoundExists;

		// Token: 0x040051D9 RID: 20953
		[NonSerialized]
		public bool startSoundPlayed;

		// Token: 0x040051DA RID: 20954
		[NonSerialized]
		public bool endSoundExists;

		// Token: 0x040051DB RID: 20955
		[NonSerialized]
		public bool endSoundPlayed;

		// Token: 0x040051DC RID: 20956
		[NonSerialized]
		public bool loop1Exists;

		// Token: 0x040051DD RID: 20957
		[NonSerialized]
		public float loop1DefaultVolume;

		// Token: 0x040051DE RID: 20958
		[NonSerialized]
		public bool loop2Exists;

		// Token: 0x040051DF RID: 20959
		[NonSerialized]
		public float loop2DefaultVolume;
	}
}
