using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200072F RID: 1839
[Serializable]
public class AbilitySound
{
	// Token: 0x06002EB7 RID: 11959 RVA: 0x000FF8DF File Offset: 0x000FDADF
	public bool IsValid()
	{
		return this.sounds != null && this.sounds.Count > 0;
	}

	// Token: 0x06002EB8 RID: 11960 RVA: 0x000FF8FC File Offset: 0x000FDAFC
	public void UpdateNextSound()
	{
		AbilitySound.SoundSelectMode soundSelectMode = this.soundSelectMode;
		if (soundSelectMode == AbilitySound.SoundSelectMode.Sequential)
		{
			this.nextSound = (this.nextSound + 1) % this.sounds.Count;
			return;
		}
		if (soundSelectMode != AbilitySound.SoundSelectMode.Random)
		{
			return;
		}
		this.nextSound = Random.Range(0, this.sounds.Count);
	}

	// Token: 0x06002EB9 RID: 11961 RVA: 0x000FF94C File Offset: 0x000FDB4C
	public void Play(AudioSource audioSourceIn)
	{
		this.usedAudioSource = ((audioSourceIn != null) ? audioSourceIn : this.audioSource);
		if (this.sounds != null && this.sounds.Count > 0 && this.usedAudioSource != null)
		{
			if (this.nextSound < 0)
			{
				this.UpdateNextSound();
			}
			AudioClip audioClip = this.sounds[this.nextSound];
			this.UpdateNextSound();
			if (audioClip != null)
			{
				this.usedAudioSource.clip = audioClip;
				this.usedAudioSource.volume = this.volume;
				this.usedAudioSource.pitch = this.pitch;
				this.usedAudioSource.loop = this.loop;
				if (this.delay <= 0f)
				{
					this.usedAudioSource.Play();
				}
				else
				{
					this.usedAudioSource.PlayDelayed(this.delay);
				}
				this.currentSound = audioClip;
			}
		}
	}

	// Token: 0x06002EBA RID: 11962 RVA: 0x000FFA40 File Offset: 0x000FDC40
	public void Stop()
	{
		if (this.usedAudioSource != null && this.usedAudioSource.clip == this.currentSound)
		{
			this.usedAudioSource.Stop();
			this.currentSound = null;
			this.usedAudioSource = null;
		}
	}

	// Token: 0x04003BDE RID: 15326
	public float volume = 1f;

	// Token: 0x04003BDF RID: 15327
	public float pitch = 1f;

	// Token: 0x04003BE0 RID: 15328
	public bool loop;

	// Token: 0x04003BE1 RID: 15329
	public float delay;

	// Token: 0x04003BE2 RID: 15330
	public List<AudioClip> sounds;

	// Token: 0x04003BE3 RID: 15331
	private AudioClip currentSound;

	// Token: 0x04003BE4 RID: 15332
	public AudioSource audioSource;

	// Token: 0x04003BE5 RID: 15333
	private AudioSource usedAudioSource;

	// Token: 0x04003BE6 RID: 15334
	private int nextSound = -1;

	// Token: 0x04003BE7 RID: 15335
	public AbilitySound.SoundSelectMode soundSelectMode;

	// Token: 0x02000730 RID: 1840
	public enum SoundSelectMode
	{
		// Token: 0x04003BE9 RID: 15337
		Sequential,
		// Token: 0x04003BEA RID: 15338
		Random
	}
}
