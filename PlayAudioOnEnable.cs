using System;
using UnityEngine;

// Token: 0x02000137 RID: 311
public class PlayAudioOnEnable : MonoBehaviour
{
	// Token: 0x060007C8 RID: 1992 RVA: 0x0002AB1B File Offset: 0x00028D1B
	private void OnEnable()
	{
		this.audioSource.clip = this.audioClips[Random.Range(0, this.audioClips.Length)];
		this.audioSource.GTPlay();
	}

	// Token: 0x040009D7 RID: 2519
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040009D8 RID: 2520
	[SerializeField]
	private AudioClip[] audioClips;
}
