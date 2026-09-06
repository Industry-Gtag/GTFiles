using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000957 RID: 2391
public class MusicManager : MonoBehaviour
{
	// Token: 0x06003ED1 RID: 16081 RVA: 0x001527DF File Offset: 0x001509DF
	private void Awake()
	{
		if (MusicManager.Instance == null)
		{
			MusicManager.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06003ED2 RID: 16082 RVA: 0x001527FF File Offset: 0x001509FF
	public void RegisterMusicSource(MusicSource musicSource)
	{
		if (!this.activeSources.Contains(musicSource))
		{
			this.activeSources.Add(musicSource);
		}
	}

	// Token: 0x06003ED3 RID: 16083 RVA: 0x0015281C File Offset: 0x00150A1C
	public void UnregisterMusicSource(MusicSource musicSource)
	{
		if (this.activeSources.Contains(musicSource))
		{
			this.activeSources.Remove(musicSource);
			musicSource.UnsetVolumeOverride();
		}
	}

	// Token: 0x06003ED4 RID: 16084 RVA: 0x00152840 File Offset: 0x00150A40
	public void FadeOutMusic(float duration = 3f)
	{
		base.StopAllCoroutines();
		if (duration > 0f)
		{
			base.StartCoroutine(this.FadeOutVolumeCoroutine(duration));
			return;
		}
		foreach (MusicSource musicSource in this.activeSources)
		{
			musicSource.SetVolumeOverride(0f);
		}
	}

	// Token: 0x06003ED5 RID: 16085 RVA: 0x001528B4 File Offset: 0x00150AB4
	public void FadeInMusic(float duration = 3f)
	{
		base.StopAllCoroutines();
		if (duration > 0f)
		{
			base.StartCoroutine(this.FadeInVolumeCoroutine(duration));
			return;
		}
		foreach (MusicSource musicSource in this.activeSources)
		{
			musicSource.UnsetVolumeOverride();
		}
	}

	// Token: 0x06003ED6 RID: 16086 RVA: 0x00152924 File Offset: 0x00150B24
	private IEnumerator FadeInVolumeCoroutine(float duration)
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			float deltaTime = Time.deltaTime;
			foreach (MusicSource musicSource in this.activeSources)
			{
				float num = musicSource.DefaultVolume / duration;
				float num2 = Mathf.MoveTowards(musicSource.AudioSource.volume, musicSource.DefaultVolume, num * deltaTime);
				musicSource.SetVolumeOverride(num2);
				if (musicSource.AudioSource.volume != musicSource.DefaultVolume)
				{
					complete = false;
				}
			}
			yield return null;
		}
		using (HashSet<MusicSource>.Enumerator enumerator = this.activeSources.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MusicSource musicSource2 = enumerator.Current;
				musicSource2.UnsetVolumeOverride();
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x06003ED7 RID: 16087 RVA: 0x0015293A File Offset: 0x00150B3A
	private IEnumerator FadeOutVolumeCoroutine(float duration)
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			float deltaTime = Time.deltaTime;
			foreach (MusicSource musicSource in this.activeSources)
			{
				float num = musicSource.DefaultVolume / duration;
				float num2 = Mathf.MoveTowards(musicSource.AudioSource.volume, 0f, num * deltaTime);
				musicSource.SetVolumeOverride(num2);
				if (musicSource.AudioSource.volume != 0f)
				{
					complete = false;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06003ED8 RID: 16088 RVA: 0x00152950 File Offset: 0x00150B50
	public static void StopAllMusic()
	{
		MusicManager.StopAllMusic(null);
	}

	// Token: 0x06003ED9 RID: 16089 RVA: 0x00152958 File Offset: 0x00150B58
	public static void StopAllMusic(AudioClip clip)
	{
		if (MusicManager.Instance == null)
		{
			return;
		}
		MusicManager.Instance.StopAllCoroutines();
		foreach (MusicSource musicSource in MusicManager.Instance.activeSources)
		{
			musicSource.UnsetVolumeOverride();
			musicSource.AudioSource.Stop();
			if (clip != null)
			{
				musicSource.AudioSource.PlayOneShot(clip);
			}
		}
	}

	// Token: 0x04004F33 RID: 20275
	[OnEnterPlay_SetNull]
	public static volatile MusicManager Instance;

	// Token: 0x04004F34 RID: 20276
	private HashSet<MusicSource> activeSources = new HashSet<MusicSource>();
}
