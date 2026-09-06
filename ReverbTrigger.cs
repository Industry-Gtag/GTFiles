using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020004DA RID: 1242
public class ReverbTrigger : MonoBehaviour
{
	// Token: 0x06001E49 RID: 7753 RVA: 0x000A2672 File Offset: 0x000A0872
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			this.targetSnapshot.TransitionTo(this.transitionTime);
		}
	}

	// Token: 0x06001E4A RID: 7754 RVA: 0x000A2693 File Offset: 0x000A0893
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			this.normalSnapshot.TransitionTo(this.transitionTime);
		}
	}

	// Token: 0x04002884 RID: 10372
	[SerializeField]
	private AudioMixer mixer;

	// Token: 0x04002885 RID: 10373
	[SerializeField]
	private AudioMixerSnapshot targetSnapshot;

	// Token: 0x04002886 RID: 10374
	[SerializeField]
	private AudioMixerSnapshot normalSnapshot;

	// Token: 0x04002887 RID: 10375
	[SerializeField]
	private Collider reverbTrigger;

	// Token: 0x04002888 RID: 10376
	[SerializeField]
	private float transitionTime = 1f;
}
