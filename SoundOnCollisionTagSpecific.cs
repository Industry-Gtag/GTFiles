using System;
using UnityEngine;

// Token: 0x0200039E RID: 926
public class SoundOnCollisionTagSpecific : MonoBehaviour
{
	// Token: 0x06001680 RID: 5760 RVA: 0x00082998 File Offset: 0x00080B98
	private void OnTriggerEnter(Collider collider)
	{
		if (Time.time > this.nextSound && collider.gameObject.CompareTag(this.tagName))
		{
			this.nextSound = Time.time + this.noiseCooldown;
			this.audioSource.GTPlayOneShot(this.collisionSounds[Random.Range(0, this.collisionSounds.Length)], 0.5f);
		}
	}

	// Token: 0x0400209E RID: 8350
	public string tagName;

	// Token: 0x0400209F RID: 8351
	public float noiseCooldown = 1f;

	// Token: 0x040020A0 RID: 8352
	private float nextSound;

	// Token: 0x040020A1 RID: 8353
	public AudioSource audioSource;

	// Token: 0x040020A2 RID: 8354
	public AudioClip[] collisionSounds;
}
