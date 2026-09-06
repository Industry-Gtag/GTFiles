using System;
using UnityEngine;

// Token: 0x020005A2 RID: 1442
public class SpitballEvents : SubEmitterListener
{
	// Token: 0x06002484 RID: 9348 RVA: 0x000C3FEC File Offset: 0x000C21EC
	protected override void OnSubEmit()
	{
		base.OnSubEmit();
		if (this._audioSource && this._sfxHit)
		{
			this._audioSource.GTPlayOneShot(this._sfxHit, 1f);
		}
	}

	// Token: 0x04002FE7 RID: 12263
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x04002FE8 RID: 12264
	[SerializeField]
	private AudioClip _sfxHit;
}
