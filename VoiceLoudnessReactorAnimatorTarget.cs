using System;
using UnityEngine;

// Token: 0x02000E87 RID: 3719
[Serializable]
public class VoiceLoudnessReactorAnimatorTarget
{
	// Token: 0x04006B7A RID: 27514
	public Animator animator;

	// Token: 0x04006B7B RID: 27515
	public bool useSmoothedLoudness;

	// Token: 0x04006B7C RID: 27516
	public float animatorSpeedToLoudness = 1f;
}
