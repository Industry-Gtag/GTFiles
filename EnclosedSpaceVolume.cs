using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000695 RID: 1685
public class EnclosedSpaceVolume : GorillaTriggerBox
{
	// Token: 0x06002A07 RID: 10759 RVA: 0x000E29BE File Offset: 0x000E0BBE
	private void Awake()
	{
		this.audioSourceInside.volume = this.quietVolume;
		this.audioSourceOutside.volume = this.loudVolume;
	}

	// Token: 0x06002A08 RID: 10760 RVA: 0x000E29E2 File Offset: 0x000E0BE2
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<GTPlayer>() != null)
		{
			this.audioSourceInside.volume = this.loudVolume;
			this.audioSourceOutside.volume = this.quietVolume;
		}
	}

	// Token: 0x06002A09 RID: 10761 RVA: 0x000E2A19 File Offset: 0x000E0C19
	private void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<GTPlayer>() != null)
		{
			this.audioSourceInside.volume = this.quietVolume;
			this.audioSourceOutside.volume = this.loudVolume;
		}
	}

	// Token: 0x040036AA RID: 13994
	public AudioSource audioSourceInside;

	// Token: 0x040036AB RID: 13995
	public AudioSource audioSourceOutside;

	// Token: 0x040036AC RID: 13996
	public float loudVolume;

	// Token: 0x040036AD RID: 13997
	public float quietVolume;
}
