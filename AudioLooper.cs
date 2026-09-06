using System;
using UnityEngine;

// Token: 0x02000D7F RID: 3455
[RequireComponent(typeof(AudioSource))]
public class AudioLooper : MonoBehaviour
{
	// Token: 0x06005531 RID: 21809 RVA: 0x001BE5D0 File Offset: 0x001BC7D0
	protected virtual void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06005532 RID: 21810 RVA: 0x001BE5E0 File Offset: 0x001BC7E0
	private void Update()
	{
		if (!this.audioSource.isPlaying)
		{
			if (this.audioSource.clip == this.loopClip && this.interjectionClips.Length != 0 && Random.value < this.interjectionLikelyhood)
			{
				this.audioSource.clip = this.interjectionClips[Random.Range(0, this.interjectionClips.Length)];
			}
			else
			{
				this.audioSource.clip = this.loopClip;
			}
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x040066C4 RID: 26308
	private AudioSource audioSource;

	// Token: 0x040066C5 RID: 26309
	[SerializeField]
	private AudioClip loopClip;

	// Token: 0x040066C6 RID: 26310
	[SerializeField]
	private AudioClip[] interjectionClips;

	// Token: 0x040066C7 RID: 26311
	[SerializeField]
	private float interjectionLikelyhood = 0.5f;
}
