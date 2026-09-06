using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200095D RID: 2397
public class PlayerAudioManager : MonoBehaviour
{
	// Token: 0x06003EFE RID: 16126 RVA: 0x00152E9E File Offset: 0x0015109E
	public void SetMixerSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 0.1f)
	{
		snapshot.TransitionTo(transitionTime);
	}

	// Token: 0x06003EFF RID: 16127 RVA: 0x00152EA7 File Offset: 0x001510A7
	public void UnsetMixerSnapshot(float transitionTime = 0.1f)
	{
		this.defaultSnapshot.TransitionTo(transitionTime);
	}

	// Token: 0x04004F49 RID: 20297
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x04004F4A RID: 20298
	public AudioMixerSnapshot underwaterSnapshot;
}
