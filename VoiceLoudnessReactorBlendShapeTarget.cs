using System;
using UnityEngine;

// Token: 0x02000E82 RID: 3714
[Serializable]
public class VoiceLoudnessReactorBlendShapeTarget
{
	// Token: 0x04006B5B RID: 27483
	public SkinnedMeshRenderer SkinnedMeshRenderer;

	// Token: 0x04006B5C RID: 27484
	public int BlendShapeIndex;

	// Token: 0x04006B5D RID: 27485
	[Tooltip("Blend shape weight at minimum loudness ")]
	public float minValue;

	// Token: 0x04006B5E RID: 27486
	[Tooltip("Blend shape weight at maximum loudness (use 100 for full weighting)\nA number higher than 100 can be used to have full weighting at lower voice loudness")]
	public float maxValue = 1f;

	// Token: 0x04006B5F RID: 27487
	public bool UseSmoothedLoudness;
}
