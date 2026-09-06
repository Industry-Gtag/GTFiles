using System;
using UnityEngine;

// Token: 0x020002EF RID: 751
public class ParticleEffectsPoolStatic<T> : ParticleEffectsPool where T : ParticleEffectsPool
{
	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x06001329 RID: 4905 RVA: 0x00065ADB File Offset: 0x00063CDB
	public static T Instance
	{
		get
		{
			return ParticleEffectsPoolStatic<T>.gInstance;
		}
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x00065AE2 File Offset: 0x00063CE2
	protected override void OnPoolAwake()
	{
		if (ParticleEffectsPoolStatic<T>.gInstance && ParticleEffectsPoolStatic<T>.gInstance != this)
		{
			Object.Destroy(this);
			return;
		}
		ParticleEffectsPoolStatic<T>.gInstance = this as T;
	}

	// Token: 0x04001761 RID: 5985
	protected static T gInstance;
}
