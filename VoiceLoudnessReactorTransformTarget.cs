using System;
using UnityEngine;

// Token: 0x02000E83 RID: 3715
[Serializable]
public class VoiceLoudnessReactorTransformTarget
{
	// Token: 0x17000898 RID: 2200
	// (get) Token: 0x06005A4D RID: 23117 RVA: 0x001D55AD File Offset: 0x001D37AD
	// (set) Token: 0x06005A4E RID: 23118 RVA: 0x001D55B5 File Offset: 0x001D37B5
	public Vector3 Initial
	{
		get
		{
			return this.initial;
		}
		set
		{
			this.initial = value;
		}
	}

	// Token: 0x04006B60 RID: 27488
	public Transform transform;

	// Token: 0x04006B61 RID: 27489
	private Vector3 initial;

	// Token: 0x04006B62 RID: 27490
	public Vector3 Max = Vector3.one;

	// Token: 0x04006B63 RID: 27491
	public float Scale = 1f;

	// Token: 0x04006B64 RID: 27492
	public bool UseSmoothedLoudness;
}
