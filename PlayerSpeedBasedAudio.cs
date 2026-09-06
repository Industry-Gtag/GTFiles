using System;
using UnityEngine;

// Token: 0x020002B0 RID: 688
public class PlayerSpeedBasedAudio : MonoBehaviour
{
	// Token: 0x060011E4 RID: 4580 RVA: 0x0005FF43 File Offset: 0x0005E143
	private void Start()
	{
		this.fadeRate = 1f / this.fadeTime;
		this.baseVolume = this.audioSource.volume;
		this.localPlayerVelocityEstimator.TryResolve<GorillaVelocityEstimator>(out this.velocityEstimator);
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x0005FF7C File Offset: 0x0005E17C
	private void Update()
	{
		this.currentFadeLevel = Mathf.MoveTowards(this.currentFadeLevel, Mathf.InverseLerp(this.minVolumeSpeed, this.fullVolumeSpeed, this.velocityEstimator.linearVelocity.magnitude), this.fadeRate * Time.deltaTime);
		if (this.baseVolume == 0f || this.currentFadeLevel == 0f)
		{
			this.audioSource.volume = 0.0001f;
			return;
		}
		this.audioSource.volume = this.baseVolume * this.currentFadeLevel;
	}

	// Token: 0x04001573 RID: 5491
	[SerializeField]
	private float minVolumeSpeed;

	// Token: 0x04001574 RID: 5492
	[SerializeField]
	private float fullVolumeSpeed;

	// Token: 0x04001575 RID: 5493
	[SerializeField]
	private float fadeTime;

	// Token: 0x04001576 RID: 5494
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001577 RID: 5495
	[SerializeField]
	private XSceneRef localPlayerVelocityEstimator;

	// Token: 0x04001578 RID: 5496
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001579 RID: 5497
	private float baseVolume;

	// Token: 0x0400157A RID: 5498
	private float fadeRate;

	// Token: 0x0400157B RID: 5499
	private float currentFadeLevel;
}
