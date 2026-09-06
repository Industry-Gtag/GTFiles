using System;
using UnityEngine;

// Token: 0x02000E84 RID: 3716
[Serializable]
public class VoiceLoudnessReactorTransformRotationTarget
{
	// Token: 0x17000899 RID: 2201
	// (get) Token: 0x06005A50 RID: 23120 RVA: 0x001D55DC File Offset: 0x001D37DC
	// (set) Token: 0x06005A51 RID: 23121 RVA: 0x001D55E4 File Offset: 0x001D37E4
	public Quaternion Initial
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

	// Token: 0x04006B65 RID: 27493
	public Transform transform;

	// Token: 0x04006B66 RID: 27494
	private Quaternion initial;

	// Token: 0x04006B67 RID: 27495
	public Quaternion Max = Quaternion.identity;

	// Token: 0x04006B68 RID: 27496
	public float Scale = 1f;

	// Token: 0x04006B69 RID: 27497
	public bool UseSmoothedLoudness;
}
