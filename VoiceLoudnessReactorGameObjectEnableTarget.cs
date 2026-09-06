using System;
using UnityEngine;

// Token: 0x02000E86 RID: 3718
[Serializable]
public class VoiceLoudnessReactorGameObjectEnableTarget
{
	// Token: 0x04006B75 RID: 27509
	public GameObject GameObject;

	// Token: 0x04006B76 RID: 27510
	public float Threshold;

	// Token: 0x04006B77 RID: 27511
	public bool TurnOnAtThreshhold = true;

	// Token: 0x04006B78 RID: 27512
	public bool UseSmoothedLoudness;

	// Token: 0x04006B79 RID: 27513
	public float Scale = 1f;
}
