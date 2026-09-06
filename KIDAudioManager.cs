using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000B5E RID: 2910
[DefaultExecutionOrder(0)]
public class KIDAudioManager : MonoBehaviour
{
	// Token: 0x17000714 RID: 1812
	// (get) Token: 0x060049EB RID: 18923 RVA: 0x0018A07D File Offset: 0x0018827D
	public static KIDAudioManager Instance
	{
		get
		{
			if (!KIDAudioManager._instance)
			{
				if (!ApplicationQuittingState.IsQuitting)
				{
					Debug.LogError("No KIDAudioManager instance found in scene!");
				}
				return null;
			}
			return KIDAudioManager._instance;
		}
	}

	// Token: 0x060049EC RID: 18924 RVA: 0x0018A0A4 File Offset: 0x001882A4
	private void Awake()
	{
		if (KIDAudioManager._instance == null)
		{
			KIDAudioManager._instance = this;
			base.transform.parent = null;
			Object.DontDestroyOnLoad(base.gameObject);
			this.ConfigureAudioSource();
			this.InitializeSoundClips();
			this.mainMixer.GetFloat("Game_Volume", out this.cachedGameVolume);
			return;
		}
		if (KIDAudioManager._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060049ED RID: 18925 RVA: 0x0018A118 File Offset: 0x00188318
	private void ConfigureAudioSource()
	{
		if (this.audioSource != null)
		{
			this.audioSource.outputAudioMixerGroup = this.kidUIGroup;
			this.audioSource.playOnAwake = false;
			this.audioSource.spatialBlend = 0f;
			this.audioSource.volume = 1f;
			this.audioSource.enabled = true;
		}
		if (this.loopingAudioSource != null)
		{
			this.loopingAudioSource.outputAudioMixerGroup = this.kidUIGroup;
			this.loopingAudioSource.playOnAwake = false;
			this.loopingAudioSource.spatialBlend = 0f;
			this.loopingAudioSource.volume = 1f;
			this.loopingAudioSource.loop = true;
			this.loopingAudioSource.enabled = true;
		}
	}

	// Token: 0x060049EE RID: 18926 RVA: 0x0018A1E0 File Offset: 0x001883E0
	private void InitializeSoundClips()
	{
		this.soundClips = new Dictionary<KIDAudioManager.KIDSoundType, AudioClip>
		{
			{
				KIDAudioManager.KIDSoundType.ButtonClick,
				this.buttonClickSound
			},
			{
				KIDAudioManager.KIDSoundType.Denied,
				this.deniedSound
			},
			{
				KIDAudioManager.KIDSoundType.Success,
				this.successSound
			},
			{
				KIDAudioManager.KIDSoundType.Hover,
				this.buttonHoverSound
			},
			{
				KIDAudioManager.KIDSoundType.ButtonHeld,
				this.buttonHeldSound
			},
			{
				KIDAudioManager.KIDSoundType.PageTransition,
				this.pageTransitionSound
			},
			{
				KIDAudioManager.KIDSoundType.InputBack,
				this.inputBackSound
			},
			{
				KIDAudioManager.KIDSoundType.TurnOffPermission,
				this.turnOffPermissionSound
			}
		};
	}

	// Token: 0x060049EF RID: 18927 RVA: 0x0018A260 File Offset: 0x00188460
	public void SetKIDUIAudioActive(bool active)
	{
		if (!this.IsInstanceValid() || this.isKIDUIActive == active)
		{
			return;
		}
		this.isKIDUIActive = active;
		if (!active)
		{
			this.StopButtonHeldSound();
		}
		if (active)
		{
			this.KIDSnapshot.TransitionTo(0f);
			return;
		}
		this.normalSnapshot.TransitionTo(0f);
	}

	// Token: 0x060049F0 RID: 18928 RVA: 0x0018A2B4 File Offset: 0x001884B4
	public void PlaySound(KIDAudioManager.KIDSoundType soundType)
	{
		if (!this.IsInstanceValid())
		{
			return;
		}
		if (soundType == KIDAudioManager.KIDSoundType.ButtonHeld)
		{
			Debug.LogWarning("[KIDAudioManager] Button held sound is already playing, skipping delayed sound.");
			return;
		}
		AudioClip audioClip;
		if (this.soundClips.TryGetValue(soundType, out audioClip) && audioClip != null)
		{
			this.audioSource.PlayOneShot(audioClip);
			return;
		}
		Debug.LogWarning(string.Format("[KIDAudioManager] Sound clip for {0} is null or not found!", soundType));
	}

	// Token: 0x060049F1 RID: 18929 RVA: 0x0018A314 File Offset: 0x00188514
	public void StartButtonHeldSound()
	{
		if (!this.IsInstanceValid() || this.buttonHeldSound == null || this.isHoldSoundPlaying)
		{
			return;
		}
		this.loopingAudioSource.clip = this.buttonHeldSound;
		this.loopingAudioSource.Play();
		this.isHoldSoundPlaying = true;
	}

	// Token: 0x060049F2 RID: 18930 RVA: 0x0018A363 File Offset: 0x00188563
	public void StopButtonHeldSound()
	{
		if (!this.IsInstanceValid() || !this.isHoldSoundPlaying)
		{
			return;
		}
		if (this.loopingAudioSource.clip == this.buttonHeldSound)
		{
			this.loopingAudioSource.Stop();
		}
		this.isHoldSoundPlaying = false;
	}

	// Token: 0x060049F3 RID: 18931 RVA: 0x0018A3A0 File Offset: 0x001885A0
	private bool IsInstanceValid()
	{
		return !(KIDAudioManager._instance == null) && !(KIDAudioManager._instance != this) && !(this.audioSource == null) && !(this.loopingAudioSource == null);
	}

	// Token: 0x060049F4 RID: 18932 RVA: 0x0018A3DB File Offset: 0x001885DB
	public bool IsKIDUIActive()
	{
		return this.isKIDUIActive;
	}

	// Token: 0x060049F5 RID: 18933 RVA: 0x0018A3E3 File Offset: 0x001885E3
	public void PlaySoundWithDelay(KIDAudioManager.KIDSoundType soundType)
	{
		base.StartCoroutine(this.PlayDelayedSound(soundType, 0.05f));
	}

	// Token: 0x060049F6 RID: 18934 RVA: 0x0018A3F8 File Offset: 0x001885F8
	private IEnumerator PlayDelayedSound(KIDAudioManager.KIDSoundType soundType, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.PlaySound(soundType);
		yield break;
	}

	// Token: 0x04005C62 RID: 23650
	private static KIDAudioManager _instance;

	// Token: 0x04005C63 RID: 23651
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04005C64 RID: 23652
	[SerializeField]
	private AudioSource loopingAudioSource;

	// Token: 0x04005C65 RID: 23653
	[SerializeField]
	private AudioMixer mainMixer;

	// Token: 0x04005C66 RID: 23654
	[SerializeField]
	private AudioMixerSnapshot KIDSnapshot;

	// Token: 0x04005C67 RID: 23655
	[SerializeField]
	private AudioMixerSnapshot normalSnapshot;

	// Token: 0x04005C68 RID: 23656
	[SerializeField]
	private AudioMixerGroup kidUIGroup;

	// Token: 0x04005C69 RID: 23657
	[SerializeField]
	private AudioClip buttonClickSound;

	// Token: 0x04005C6A RID: 23658
	[SerializeField]
	private AudioClip deniedSound;

	// Token: 0x04005C6B RID: 23659
	[SerializeField]
	private AudioClip successSound;

	// Token: 0x04005C6C RID: 23660
	[SerializeField]
	private AudioClip buttonHoverSound;

	// Token: 0x04005C6D RID: 23661
	[SerializeField]
	private AudioClip buttonHeldSound;

	// Token: 0x04005C6E RID: 23662
	[SerializeField]
	private AudioClip pageTransitionSound;

	// Token: 0x04005C6F RID: 23663
	[SerializeField]
	private AudioClip inputBackSound;

	// Token: 0x04005C70 RID: 23664
	[SerializeField]
	private AudioClip turnOffPermissionSound;

	// Token: 0x04005C71 RID: 23665
	private const string GAME_VOLUME = "Game_Volume";

	// Token: 0x04005C72 RID: 23666
	private const string KID_VOLUME = "KID_UI_Volume";

	// Token: 0x04005C73 RID: 23667
	private const float MUTED_VALUE = -80f;

	// Token: 0x04005C74 RID: 23668
	private const float UNMUTED_VALUE = 0f;

	// Token: 0x04005C75 RID: 23669
	private bool isKIDUIActive;

	// Token: 0x04005C76 RID: 23670
	private float cachedGameVolume;

	// Token: 0x04005C77 RID: 23671
	private bool isHoldSoundPlaying;

	// Token: 0x04005C78 RID: 23672
	private Dictionary<KIDAudioManager.KIDSoundType, AudioClip> soundClips;

	// Token: 0x02000B5F RID: 2911
	public enum KIDSoundType
	{
		// Token: 0x04005C7A RID: 23674
		ButtonClick,
		// Token: 0x04005C7B RID: 23675
		Hover,
		// Token: 0x04005C7C RID: 23676
		Success,
		// Token: 0x04005C7D RID: 23677
		Denied,
		// Token: 0x04005C7E RID: 23678
		InputBack,
		// Token: 0x04005C7F RID: 23679
		TurnOffPermission,
		// Token: 0x04005C80 RID: 23680
		PageTransition,
		// Token: 0x04005C81 RID: 23681
		ButtonHeld
	}
}
