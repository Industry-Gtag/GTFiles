using System;
using UnityEngine;

// Token: 0x0200095A RID: 2394
[RequireComponent(typeof(AudioSource))]
public class MusicSource : MonoBehaviour
{
	// Token: 0x170005D0 RID: 1488
	// (get) Token: 0x06003EE7 RID: 16103 RVA: 0x00152C60 File Offset: 0x00150E60
	public AudioSource AudioSource
	{
		get
		{
			return this.audioSource;
		}
	}

	// Token: 0x170005D1 RID: 1489
	// (get) Token: 0x06003EE8 RID: 16104 RVA: 0x00152C68 File Offset: 0x00150E68
	public float DefaultVolume
	{
		get
		{
			return this.defaultVolume;
		}
	}

	// Token: 0x170005D2 RID: 1490
	// (get) Token: 0x06003EE9 RID: 16105 RVA: 0x00152C70 File Offset: 0x00150E70
	public bool VolumeOverridden
	{
		get
		{
			return this.volumeOverride != null;
		}
	}

	// Token: 0x06003EEA RID: 16106 RVA: 0x00152C7D File Offset: 0x00150E7D
	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		if (this.setDefaultVolumeFromAudioSourceOnAwake)
		{
			this.defaultVolume = this.audioSource.volume;
		}
	}

	// Token: 0x06003EEB RID: 16107 RVA: 0x00152CB2 File Offset: 0x00150EB2
	private void OnEnable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.RegisterMusicSource(this);
		}
	}

	// Token: 0x06003EEC RID: 16108 RVA: 0x00152CD0 File Offset: 0x00150ED0
	private void OnDisable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.UnregisterMusicSource(this);
		}
	}

	// Token: 0x06003EED RID: 16109 RVA: 0x00152CEE File Offset: 0x00150EEE
	public void SetVolumeOverride(float volume)
	{
		this.volumeOverride = new float?(volume);
		this.audioSource.volume = this.volumeOverride.Value;
	}

	// Token: 0x06003EEE RID: 16110 RVA: 0x00152D12 File Offset: 0x00150F12
	public void UnsetVolumeOverride()
	{
		this.volumeOverride = null;
		this.audioSource.volume = this.defaultVolume;
	}

	// Token: 0x04004F3F RID: 20287
	[SerializeField]
	private float defaultVolume = 1f;

	// Token: 0x04004F40 RID: 20288
	[SerializeField]
	private bool setDefaultVolumeFromAudioSourceOnAwake = true;

	// Token: 0x04004F41 RID: 20289
	private AudioSource audioSource;

	// Token: 0x04004F42 RID: 20290
	private float? volumeOverride;
}
