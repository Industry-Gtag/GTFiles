using System;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class AudioSourceEventTargets : MonoBehaviour
{
	// Token: 0x06000053 RID: 83 RVA: 0x00002F21 File Offset: 0x00001121
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.fadeVolume = this.audioSource.volume;
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00002F40 File Offset: 0x00001140
	public void SetFadeSpeed(float arg)
	{
		this.fadeSpeed = Mathf.Max(arg, 0.01f);
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00002F53 File Offset: 0x00001153
	public void StartFade(float arg)
	{
		this.fadeVolume = Mathf.Clamp01(arg);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00002F64 File Offset: 0x00001164
	public void Update()
	{
		if (this.audioSource.volume != this.fadeVolume)
		{
			this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, this.fadeVolume, this.fadeSpeed * Time.deltaTime);
		}
		if (this.lastValueWhenPlayed != this.ExternalTriggerPlay)
		{
			if (!this.lastExternalTriggerPlayMatched)
			{
				this.audioSource.Play();
				this.lastValueWhenPlayed = this.ExternalTriggerPlay;
				this.lastExternalTriggerPlayMatched = true;
			}
			else
			{
				this.ExternalTriggerPlay = this.lastValueWhenPlayed;
				this.lastExternalTriggerPlayMatched = false;
			}
		}
		else
		{
			this.lastExternalTriggerPlayMatched = true;
		}
		if (this.lastValueWhenStopped == this.ExternalTriggerStop)
		{
			this.lastExternalTriggerStopMatched = true;
			return;
		}
		if (!this.lastExternalTriggerStopMatched)
		{
			this.audioSource.Stop();
			this.lastValueWhenStopped = this.ExternalTriggerStop;
			this.lastExternalTriggerStopMatched = true;
			return;
		}
		this.ExternalTriggerStop = this.lastValueWhenStopped;
		this.lastExternalTriggerStopMatched = false;
	}

	// Token: 0x04000034 RID: 52
	private AudioSource audioSource;

	// Token: 0x04000035 RID: 53
	private float fadeVolume;

	// Token: 0x04000036 RID: 54
	private float fadeSpeed;

	// Token: 0x04000037 RID: 55
	[Header("Change Value To Trigger Play (false to true and true to false both work, but value must change the frame you want it played)")]
	public bool ExternalTriggerPlay;

	// Token: 0x04000038 RID: 56
	private bool lastExternalTriggerPlayMatched = true;

	// Token: 0x04000039 RID: 57
	private bool lastValueWhenPlayed;

	// Token: 0x0400003A RID: 58
	[Header("Change Value To Trigger Stop (false to true and true to false both work, but value must change the frame you want it stopped)")]
	public bool ExternalTriggerStop;

	// Token: 0x0400003B RID: 59
	private bool lastExternalTriggerStopMatched = true;

	// Token: 0x0400003C RID: 60
	private bool lastValueWhenStopped;
}
