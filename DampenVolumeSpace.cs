using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000313 RID: 787
public class DampenVolumeSpace : MonoBehaviour
{
	// Token: 0x060013EC RID: 5100 RVA: 0x0006C5E2 File Offset: 0x0006A7E2
	private void Awake()
	{
		if (this.audioSource == null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x0006C5FC File Offset: 0x0006A7FC
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent != null && componentInParent == GTPlayer.Instance)
		{
			this.audioSource.volume = this.setVolume;
		}
	}

	// Token: 0x0400188A RID: 6282
	public AudioSource audioSource;

	// Token: 0x0400188B RID: 6283
	public float setVolume;
}
