using System;
using UnityEngine;

// Token: 0x020002B4 RID: 692
public class AudioFader : MonoBehaviour
{
	// Token: 0x060011F6 RID: 4598 RVA: 0x000603EB File Offset: 0x0005E5EB
	private void Start()
	{
		this.fadeInSpeed = this.maxVolume / this.fadeInDuration;
		this.fadeOutSpeed = this.maxVolume / this.fadeOutDuration;
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x00060414 File Offset: 0x0005E614
	public void FadeIn()
	{
		this.targetVolume = this.maxVolume;
		if (this.fadeInDuration > 0f)
		{
			base.enabled = true;
			this.currentFadeSpeed = this.fadeInSpeed;
		}
		else
		{
			this.currentVolume = this.maxVolume;
		}
		this.audioToFade.volume = this.currentVolume;
		if (!this.audioToFade.isPlaying)
		{
			this.audioToFade.GTPlay();
		}
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x00060484 File Offset: 0x0005E684
	public void FadeOut()
	{
		this.targetVolume = 0f;
		if (this.fadeOutDuration > 0f)
		{
			base.enabled = true;
			this.currentFadeSpeed = this.fadeOutSpeed;
		}
		else
		{
			this.currentVolume = 0f;
			if (this.audioToFade.isPlaying)
			{
				this.audioToFade.Stop();
			}
		}
		if (this.outro != null && this.currentVolume > 0f)
		{
			this.outro.volume = this.currentVolume;
			this.outro.GTPlay();
		}
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x00060518 File Offset: 0x0005E718
	private void Update()
	{
		this.currentVolume = Mathf.MoveTowards(this.currentVolume, this.targetVolume, this.currentFadeSpeed * Time.deltaTime);
		this.audioToFade.volume = this.currentVolume;
		if (this.currentVolume == this.targetVolume)
		{
			base.enabled = false;
			if (this.currentVolume == 0f && this.audioToFade.isPlaying)
			{
				this.audioToFade.Stop();
			}
		}
	}

	// Token: 0x04001586 RID: 5510
	[SerializeField]
	private AudioSource audioToFade;

	// Token: 0x04001587 RID: 5511
	[SerializeField]
	private AudioSource outro;

	// Token: 0x04001588 RID: 5512
	[SerializeField]
	private float fadeInDuration = 0.3f;

	// Token: 0x04001589 RID: 5513
	[SerializeField]
	private float fadeOutDuration = 0.3f;

	// Token: 0x0400158A RID: 5514
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x0400158B RID: 5515
	private float currentVolume;

	// Token: 0x0400158C RID: 5516
	private float targetVolume;

	// Token: 0x0400158D RID: 5517
	private float currentFadeSpeed;

	// Token: 0x0400158E RID: 5518
	private float fadeInSpeed;

	// Token: 0x0400158F RID: 5519
	private float fadeOutSpeed;
}
