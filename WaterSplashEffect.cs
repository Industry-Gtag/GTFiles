using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x020003B6 RID: 950
public class WaterSplashEffect : MonoBehaviour
{
	// Token: 0x060016ED RID: 5869 RVA: 0x0008562F File Offset: 0x0008382F
	private void OnEnable()
	{
		this.startTime = Time.time;
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x0008563C File Offset: 0x0008383C
	public void Destroy()
	{
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.DeactivateParticleSystems(this.smallSplashParticleSystems);
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x060016EF RID: 5871 RVA: 0x00085670 File Offset: 0x00083870
	public void PlayEffect(bool isBigSplash, bool isEntry, float scale, WaterVolume volume = null)
	{
		this.waterVolume = volume;
		if (isBigSplash)
		{
			this.DeactivateParticleSystems(this.smallSplashParticleSystems);
			this.SetParticleEffectParameters(this.bigSplashParticleSystems, scale, this.bigSplashBaseGravityMultiplier, this.bigSplashBaseStartSpeed, this.bigSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.bigSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.bigSplashAudioClips, ref WaterSplashEffect.lastPlayedBigSplashAudioClipIndex);
			return;
		}
		if (isEntry)
		{
			this.DeactivateParticleSystems(this.bigSplashParticleSystems);
			this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.smallSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.smallSplashEntryAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashEntryAudioClipIndex);
			return;
		}
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
		this.PlayParticleEffects(this.smallSplashParticleSystems);
		this.PlayRandomAudioClipWithoutRepeats(this.smallSplashExitAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashExitAudioClipIndex);
	}

	// Token: 0x060016F0 RID: 5872 RVA: 0x00085778 File Offset: 0x00083978
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 vector = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - vector;
		}
		if ((Time.time - this.startTime) / this.lifeTime >= 1f)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x060016F1 RID: 5873 RVA: 0x00085840 File Offset: 0x00083A40
	private void DeactivateParticleSystems(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060016F2 RID: 5874 RVA: 0x0008586C File Offset: 0x00083A6C
	private void PlayParticleEffects(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(true);
				particleSystems[i].Play();
			}
		}
	}

	// Token: 0x060016F3 RID: 5875 RVA: 0x000858A0 File Offset: 0x00083AA0
	private void SetParticleEffectParameters(ParticleSystem[] particleSystems, float scale, float baseGravMultiplier, float baseStartSpeed, float baseSimulationSpeed, WaterVolume waterVolume = null)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				ParticleSystem.MainModule main = particleSystems[i].main;
				main.startSpeed = baseStartSpeed;
				main.gravityModifier = baseGravMultiplier;
				if (scale < 0.99f)
				{
					main.startSpeed = baseStartSpeed * scale * 2f;
					main.gravityModifier = baseGravMultiplier * scale * 0.5f;
				}
				if (waterVolume != null && waterVolume.Parameters != null)
				{
					particleSystems[i].colorBySpeed.color = waterVolume.Parameters.splashColorBySpeedGradient;
				}
			}
		}
	}

	// Token: 0x060016F4 RID: 5876 RVA: 0x00085958 File Offset: 0x00083B58
	private void PlayRandomAudioClipWithoutRepeats(AudioClip[] audioClips, ref int lastPlayedAudioClipIndex)
	{
		if (this.audioSource != null && audioClips != null && audioClips.Length != 0)
		{
			int num = 0;
			if (audioClips.Length > 1)
			{
				int num2 = Random.Range(0, audioClips.Length);
				if (num2 == lastPlayedAudioClipIndex)
				{
					num2 = ((Random.Range(0f, 1f) > 0.5f) ? ((num2 + 1) % audioClips.Length) : (num2 - 1));
					if (num2 < 0)
					{
						num2 = audioClips.Length - 1;
					}
				}
				num = num2;
			}
			lastPlayedAudioClipIndex = num;
			this.audioSource.clip = audioClips[num];
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x040021F1 RID: 8689
	private static int lastPlayedBigSplashAudioClipIndex = -1;

	// Token: 0x040021F2 RID: 8690
	private static int lastPlayedSmallSplashEntryAudioClipIndex = -1;

	// Token: 0x040021F3 RID: 8691
	private static int lastPlayedSmallSplashExitAudioClipIndex = -1;

	// Token: 0x040021F4 RID: 8692
	public ParticleSystem[] bigSplashParticleSystems;

	// Token: 0x040021F5 RID: 8693
	public ParticleSystem[] smallSplashParticleSystems;

	// Token: 0x040021F6 RID: 8694
	public float bigSplashBaseGravityMultiplier = 0.9f;

	// Token: 0x040021F7 RID: 8695
	public float bigSplashBaseStartSpeed = 1.9f;

	// Token: 0x040021F8 RID: 8696
	public float bigSplashBaseSimulationSpeed = 0.9f;

	// Token: 0x040021F9 RID: 8697
	public float smallSplashBaseGravityMultiplier = 0.6f;

	// Token: 0x040021FA RID: 8698
	public float smallSplashBaseStartSpeed = 0.6f;

	// Token: 0x040021FB RID: 8699
	public float smallSplashBaseSimulationSpeed = 0.6f;

	// Token: 0x040021FC RID: 8700
	public float lifeTime = 1f;

	// Token: 0x040021FD RID: 8701
	private float startTime = -1f;

	// Token: 0x040021FE RID: 8702
	public AudioSource audioSource;

	// Token: 0x040021FF RID: 8703
	public AudioClip[] bigSplashAudioClips;

	// Token: 0x04002200 RID: 8704
	public AudioClip[] smallSplashEntryAudioClips;

	// Token: 0x04002201 RID: 8705
	public AudioClip[] smallSplashExitAudioClips;

	// Token: 0x04002202 RID: 8706
	private WaterVolume waterVolume;
}
