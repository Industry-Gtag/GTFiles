using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000DD6 RID: 3542
public class SoundBankPlayer : MonoBehaviour
{
	// Token: 0x1700084A RID: 2122
	// (get) Token: 0x060056C3 RID: 22211 RVA: 0x001C4834 File Offset: 0x001C2A34
	public bool isPlaying
	{
		get
		{
			return Time.realtimeSinceStartup < this.playEndTime;
		}
	}

	// Token: 0x1700084B RID: 2123
	// (get) Token: 0x060056C4 RID: 22212 RVA: 0x001C4843 File Offset: 0x001C2A43
	public float NormalizedTime
	{
		get
		{
			if (this.clipDuration != 0f)
			{
				return Mathf.Clamp01(this.CurrentTime / this.clipDuration);
			}
			return 1f;
		}
	}

	// Token: 0x1700084C RID: 2124
	// (get) Token: 0x060056C5 RID: 22213 RVA: 0x001C486A File Offset: 0x001C2A6A
	public float CurrentTime
	{
		get
		{
			return Time.realtimeSinceStartup - this.playStartTime;
		}
	}

	// Token: 0x060056C6 RID: 22214 RVA: 0x001C4878 File Offset: 0x001C2A78
	protected void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.gameObject.AddComponent<AudioSource>();
			this.audioSource.outputAudioMixerGroup = this.outputAudioMixerGroup;
			this.audioSource.spatialize = this.spatialize;
			this.audioSource.spatializePostEffects = this.spatializePostEffects;
			this.audioSource.bypassEffects = this.bypassEffects;
			this.audioSource.bypassListenerEffects = this.bypassListenerEffects;
			this.audioSource.bypassReverbZones = this.bypassReverbZones;
			this.audioSource.priority = this.priority;
			this.audioSource.spatialBlend = this.spatialBlend;
			this.audioSource.dopplerLevel = this.dopplerLevel;
			this.audioSource.spread = this.spread;
			this.audioSource.rolloffMode = this.rolloffMode;
			this.audioSource.minDistance = this.minDistance;
			this.audioSource.maxDistance = this.maxDistance;
			this.audioSource.reverbZoneMix = this.reverbZoneMix;
		}
		this.audioSource.volume = 1f;
		this.audioSource.playOnAwake = false;
		if (this.shuffleOrder)
		{
			int[] array = new int[this.soundBank.sounds.Length / 2];
			this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
			for (int i = 0; i < this.playlist.Length; i++)
			{
				int num = 0;
				for (int j = 0; j < 100; j++)
				{
					num = Random.Range(0, this.soundBank.sounds.Length);
					if (Array.IndexOf<int>(array, num) == -1)
					{
						break;
					}
				}
				if (array.Length != 0)
				{
					array[i % array.Length] = num;
				}
				this.playlist[i] = new SoundBankPlayer.PlaylistEntry
				{
					index = num,
					volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
					pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
				};
			}
			return;
		}
		this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
		for (int k = 0; k < this.playlist.Length; k++)
		{
			this.playlist[k] = new SoundBankPlayer.PlaylistEntry
			{
				index = k % this.soundBank.sounds.Length,
				volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
				pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
			};
		}
	}

	// Token: 0x060056C7 RID: 22215 RVA: 0x001C4B71 File Offset: 0x001C2D71
	protected void OnEnable()
	{
		if (this.playOnEnable)
		{
			this.Play();
		}
	}

	// Token: 0x060056C8 RID: 22216 RVA: 0x001C4B84 File Offset: 0x001C2D84
	public void Play()
	{
		this.Play(null, null);
	}

	// Token: 0x060056C9 RID: 22217 RVA: 0x001C4BAC File Offset: 0x001C2DAC
	public void Play(float? volumeOverride = null, float? pitchOverride = null)
	{
		if (!base.enabled || this.soundBank.sounds.Length == 0 || this.playlist == null)
		{
			return;
		}
		SoundBankPlayer.PlaylistEntry playlistEntry = this.playlist[this.nextIndex];
		this.audioSource.pitch = ((pitchOverride != null) ? pitchOverride.Value : playlistEntry.pitch);
		AudioClip audioClip = this.soundBank.sounds[playlistEntry.index];
		if (audioClip != null)
		{
			this.audioSource.GTPlayOneShot(audioClip, (volumeOverride != null) ? volumeOverride.Value : playlistEntry.volume);
			this.clipDuration = audioClip.length;
			this.playStartTime = Time.realtimeSinceStartup;
			this.playEndTime = Mathf.Max(this.playEndTime, this.playStartTime + audioClip.length);
			this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
			return;
		}
		if (this.missingSoundsAreOk)
		{
			this.clipDuration = 0f;
			this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
			return;
		}
		Debug.LogErrorFormat("Sounds bank {0} is missing a clip at {1}", new object[]
		{
			base.gameObject.name,
			playlistEntry.index
		});
	}

	// Token: 0x060056CA RID: 22218 RVA: 0x001C4CF1 File Offset: 0x001C2EF1
	public void RestartSequence()
	{
		this.nextIndex = 0;
	}

	// Token: 0x040067A0 RID: 26528
	[Tooltip("Optional. AudioSource Settings will be used if this is not defined.")]
	public AudioSource audioSource;

	// Token: 0x040067A1 RID: 26529
	public bool playOnEnable = true;

	// Token: 0x040067A2 RID: 26530
	public bool shuffleOrder = true;

	// Token: 0x040067A3 RID: 26531
	public bool missingSoundsAreOk;

	// Token: 0x040067A4 RID: 26532
	public SoundBankSO soundBank;

	// Token: 0x040067A5 RID: 26533
	public AudioMixerGroup outputAudioMixerGroup;

	// Token: 0x040067A6 RID: 26534
	public bool spatialize;

	// Token: 0x040067A7 RID: 26535
	public bool spatializePostEffects;

	// Token: 0x040067A8 RID: 26536
	public bool bypassEffects;

	// Token: 0x040067A9 RID: 26537
	public bool bypassListenerEffects;

	// Token: 0x040067AA RID: 26538
	public bool bypassReverbZones;

	// Token: 0x040067AB RID: 26539
	public int priority = 128;

	// Token: 0x040067AC RID: 26540
	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	// Token: 0x040067AD RID: 26541
	public float reverbZoneMix = 1f;

	// Token: 0x040067AE RID: 26542
	public float dopplerLevel = 1f;

	// Token: 0x040067AF RID: 26543
	public float spread;

	// Token: 0x040067B0 RID: 26544
	public AudioRolloffMode rolloffMode;

	// Token: 0x040067B1 RID: 26545
	public float minDistance = 1f;

	// Token: 0x040067B2 RID: 26546
	public float maxDistance = 100f;

	// Token: 0x040067B3 RID: 26547
	public AnimationCurve customRolloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x040067B4 RID: 26548
	private int nextIndex;

	// Token: 0x040067B5 RID: 26549
	private float playStartTime;

	// Token: 0x040067B6 RID: 26550
	private float playEndTime;

	// Token: 0x040067B7 RID: 26551
	private float clipDuration;

	// Token: 0x040067B8 RID: 26552
	private SoundBankPlayer.PlaylistEntry[] playlist;

	// Token: 0x02000DD7 RID: 3543
	private struct PlaylistEntry
	{
		// Token: 0x040067B9 RID: 26553
		public int index;

		// Token: 0x040067BA RID: 26554
		public float volume;

		// Token: 0x040067BB RID: 26555
		public float pitch;
	}
}
