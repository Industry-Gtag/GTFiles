using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008E6 RID: 2278
public class GRAdaptiveMusicController : MonoBehaviour
{
	// Token: 0x06003BA0 RID: 15264 RVA: 0x00146B36 File Offset: 0x00144D36
	private void Start()
	{
		this.cachedSourcePosition = this.AudioSources[0].transform.position;
	}

	// Token: 0x06003BA1 RID: 15265 RVA: 0x00146B54 File Offset: 0x00144D54
	private void PlayCurrentTrack()
	{
		if (this.trackIndex < 0 || this.trackIndex >= this.Tracks.Count)
		{
			return;
		}
		GRAdaptiveMusicController.SingleTrack singleTrack = this.Tracks[this.trackIndex];
		AudioSource currentAudioSource = this.GetCurrentAudioSource();
		currentAudioSource.clip = singleTrack.IntroClip;
		currentAudioSource.Play();
		AudioSource nextAudioSource = this.GetNextAudioSource();
		nextAudioSource.clip = singleTrack.LoopedClip;
		nextAudioSource.loop = true;
		double num = AudioSettings.dspTime + (double)singleTrack.IntroClip.length;
		currentAudioSource.SetScheduledEndTime(num);
		nextAudioSource.PlayScheduled(num);
		this.currentAudioSourceIndex = this.NextAudioSourceIndex;
		this.CurrentTrack = singleTrack;
	}

	// Token: 0x06003BA2 RID: 15266 RVA: 0x00146BF5 File Offset: 0x00144DF5
	[ContextMenu("Transition Next Track")]
	public void TransitionToNextTrack()
	{
		this.GoToTrack(this.trackIndex + 1, false);
	}

	// Token: 0x06003BA3 RID: 15267 RVA: 0x00146C06 File Offset: 0x00144E06
	public void TransitionToLastTrack()
	{
		this.GoToTrack(this.Tracks.Count - 1, false);
	}

	// Token: 0x06003BA4 RID: 15268 RVA: 0x00146C1C File Offset: 0x00144E1C
	public void GoToTrack(int nextIndex, bool force = false)
	{
		if (!force && (nextIndex < 0 || nextIndex >= this.Tracks.Count || this.trackIndex == nextIndex))
		{
			return;
		}
		Debug.Log(string.Format("GRAdaptiveMusicController - Going to track {0}.", nextIndex));
		GRAdaptiveMusicController.SingleTrack singleTrack = this.Tracks[nextIndex];
		AudioSource audioSource = this.GetCurrentAudioSource();
		AudioSource audioSource2 = this.GetNextAudioSource();
		double num = (double)audioSource.timeSamples / (double)audioSource.clip.frequency % GRAdaptiveMusicController.BAR_DURATION;
		double num2 = AudioSettings.dspTime + (GRAdaptiveMusicController.BAR_DURATION - num);
		audioSource2.Stop();
		audioSource2.clip = singleTrack.IntroClip;
		audioSource2.loop = false;
		audioSource.SetScheduledEndTime(num2);
		audioSource2.PlayScheduled(num2);
		this.currentAudioSourceIndex = this.NextAudioSourceIndex;
		if (singleTrack.LoopedClip != null)
		{
			audioSource = audioSource2;
			audioSource2 = this.GetNextAudioSource();
			audioSource2.clip = singleTrack.LoopedClip;
			audioSource2.loop = true;
			double num3 = num2 + (double)singleTrack.IntroClip.length;
			audioSource.SetScheduledEndTime(num3);
			audioSource2.PlayScheduled(num3);
			this.currentAudioSourceIndex = this.NextAudioSourceIndex;
		}
		else
		{
			this.Finish(singleTrack.IntroClip.length + 1f);
		}
		this.trackIndex = nextIndex;
		this.CurrentTrack = singleTrack;
	}

	// Token: 0x06003BA5 RID: 15269 RVA: 0x00146D58 File Offset: 0x00144F58
	[ContextMenu("Restart")]
	public void Restart()
	{
		Debug.Log("Restarting AdaptiveMusicController.");
		this.cachedSourceVolume = this.AudioSources[0].volume;
		this.synchedMusicController.enabled = false;
		this.StopAllAudioSources();
		this.UpdateAudioSourcesVolume(this.AdjustedSourceVolume);
		if (this.RepositionAudioSourcePoint != null)
		{
			this.UpdateAudioSourcesPosition(this.RepositionAudioSourcePoint.position);
		}
		this.trackIndex = 0;
		this.currentAudioSourceIndex = 0;
		this.PlayCurrentTrack();
	}

	// Token: 0x06003BA6 RID: 15270 RVA: 0x00146DD8 File Offset: 0x00144FD8
	public void RestartAt(int index)
	{
		Debug.Log(string.Format("Restarting AdaptiveMusicController at index {0}.", index));
		this.cachedSourceVolume = this.AudioSources[0].volume;
		this.synchedMusicController.enabled = false;
		this.StopAllAudioSources();
		this.UpdateAudioSourcesVolume(this.AdjustedSourceVolume);
		if (this.RepositionAudioSourcePoint != null)
		{
			this.UpdateAudioSourcesPosition(this.RepositionAudioSourcePoint.position);
		}
		this.trackIndex = index;
		this.currentAudioSourceIndex = 0;
		this.GoToTrack(this.trackIndex, true);
	}

	// Token: 0x06003BA7 RID: 15271 RVA: 0x00146E69 File Offset: 0x00145069
	private AudioSource GetCurrentAudioSource()
	{
		return this.AudioSources[this.currentAudioSourceIndex];
	}

	// Token: 0x06003BA8 RID: 15272 RVA: 0x00146E7C File Offset: 0x0014507C
	private AudioSource GetNextAudioSource()
	{
		return this.AudioSources[this.NextAudioSourceIndex];
	}

	// Token: 0x1700055C RID: 1372
	// (get) Token: 0x06003BA9 RID: 15273 RVA: 0x00146E8F File Offset: 0x0014508F
	private int NextAudioSourceIndex
	{
		get
		{
			return (this.currentAudioSourceIndex + 1) % this.AudioSources.Count;
		}
	}

	// Token: 0x06003BAA RID: 15274 RVA: 0x00146EA8 File Offset: 0x001450A8
	private void StopAllAudioSources()
	{
		for (int i = 0; i < this.AudioSources.Count; i++)
		{
			this.AudioSources[i].Stop();
		}
	}

	// Token: 0x06003BAB RID: 15275 RVA: 0x00146EDC File Offset: 0x001450DC
	private void UpdateAudioSourcesVolume(float volume)
	{
		for (int i = 0; i < this.AudioSources.Count; i++)
		{
			this.AudioSources[i].mute = false;
			this.AudioSources[i].volume = volume;
		}
	}

	// Token: 0x06003BAC RID: 15276 RVA: 0x00146F24 File Offset: 0x00145124
	private void UpdateAudioSourcesPosition(Vector3 position)
	{
		for (int i = 0; i < this.AudioSources.Count; i++)
		{
			this.AudioSources[i].transform.position = position;
		}
	}

	// Token: 0x06003BAD RID: 15277 RVA: 0x00146F5E File Offset: 0x0014515E
	private void Finish(float delay)
	{
		if (this.finishCoroutine != null)
		{
			return;
		}
		this.finishCoroutine = base.StartCoroutine(this.TryFinish(delay));
	}

	// Token: 0x06003BAE RID: 15278 RVA: 0x00146F7C File Offset: 0x0014517C
	private IEnumerator TryFinish(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.StopAllAudioSources();
		this.UpdateAudioSourcesVolume(this.cachedSourceVolume);
		if (this.RepositionAudioSourcePoint != null)
		{
			this.UpdateAudioSourcesPosition(this.cachedSourcePosition);
		}
		this.synchedMusicController.enabled = true;
		yield break;
	}

	// Token: 0x04004C38 RID: 19512
	private static double BAR_DURATION = 1.6551724137931034;

	// Token: 0x04004C39 RID: 19513
	public List<GRAdaptiveMusicController.SingleTrack> Tracks;

	// Token: 0x04004C3A RID: 19514
	public GRAdaptiveMusicController.SingleTrack CurrentTrack;

	// Token: 0x04004C3B RID: 19515
	[SerializeField]
	private int trackIndex;

	// Token: 0x04004C3C RID: 19516
	public List<AudioSource> AudioSources;

	// Token: 0x04004C3D RID: 19517
	public Transform RepositionAudioSourcePoint;

	// Token: 0x04004C3E RID: 19518
	public float AdjustedSourceVolume = 0.035f;

	// Token: 0x04004C3F RID: 19519
	private int currentAudioSourceIndex;

	// Token: 0x04004C40 RID: 19520
	private float cachedSourceVolume = 0.1f;

	// Token: 0x04004C41 RID: 19521
	private Vector3 cachedSourcePosition = Vector3.zero;

	// Token: 0x04004C42 RID: 19522
	[SerializeField]
	private SynchedMusicController synchedMusicController;

	// Token: 0x04004C43 RID: 19523
	private Coroutine finishCoroutine;

	// Token: 0x020008E7 RID: 2279
	[Serializable]
	public class SingleTrack
	{
		// Token: 0x04004C44 RID: 19524
		public AudioClip IntroClip;

		// Token: 0x04004C45 RID: 19525
		public AudioClip LoopedClip;
	}
}
