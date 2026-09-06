using System;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class AnimationScrubber : MonoBehaviour
{
	// Token: 0x06001180 RID: 4480 RVA: 0x0005E228 File Offset: 0x0005C428
	private void LateUpdate()
	{
		if (!this.scrubberActive)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = this.targetAnimator.GetCurrentAnimatorStateInfo(0);
		AnimatorClipInfo[] currentAnimatorClipInfo = this.targetAnimator.GetCurrentAnimatorClipInfo(0);
		this.targetAnimator.Play(currentAnimatorClipInfo[0].clip.name, 0, this.animationPlaybackTime / currentAnimatorStateInfo.length);
	}

	// Token: 0x040014E2 RID: 5346
	public bool scrubberActive;

	// Token: 0x040014E3 RID: 5347
	public float animationPlaybackTime;

	// Token: 0x040014E4 RID: 5348
	public Animator targetAnimator;
}
