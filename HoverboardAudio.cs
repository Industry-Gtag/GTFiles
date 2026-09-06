using System;
using UnityEngine;

// Token: 0x02000907 RID: 2311
public class HoverboardAudio : MonoBehaviour
{
	// Token: 0x06003C9A RID: 15514 RVA: 0x0014ABDA File Offset: 0x00148DDA
	private void Start()
	{
		this.Stop();
	}

	// Token: 0x06003C9B RID: 15515 RVA: 0x0014ABE2 File Offset: 0x00148DE2
	public void PlayTurnSound(float angle)
	{
		if (Time.time > this.turnSoundCooldownUntilTimestamp && angle > this.minAngleDeltaForTurnSound)
		{
			this.turnSoundCooldownUntilTimestamp = Time.time + this.turnSoundCooldownDuration;
			this.turnSounds.Play();
		}
	}

	// Token: 0x06003C9C RID: 15516 RVA: 0x0014AC18 File Offset: 0x00148E18
	public void UpdateAudioLoop(float speed, float airspeed, float strainLevel, float grindLevel)
	{
		this.motorAnimator.UpdateValue(speed, false);
		this.windRushAnimator.UpdateValue(airspeed, false);
		if (grindLevel > 0f)
		{
			this.grindAnimator.UpdatePitchAndVolume(speed, grindLevel + 0.5f, false);
		}
		else
		{
			this.grindAnimator.UpdatePitchAndVolume(0f, 0f, false);
		}
		strainLevel = Mathf.Clamp01(strainLevel * 10f);
		if (!this.didInitHum1BaseVolume)
		{
			this.hum1BaseVolume = this.hum1.volume;
			this.didInitHum1BaseVolume = true;
		}
		this.hum1.volume = Mathf.MoveTowards(this.hum1.volume, this.hum1BaseVolume * strainLevel, this.fadeSpeed * Time.deltaTime);
	}

	// Token: 0x06003C9D RID: 15517 RVA: 0x0014ACD4 File Offset: 0x00148ED4
	public void Stop()
	{
		if (!this.didInitHum1BaseVolume)
		{
			this.hum1BaseVolume = this.hum1.volume;
			this.didInitHum1BaseVolume = true;
		}
		this.hum1.volume = 0f;
		this.windRushAnimator.UpdateValue(0f, true);
		this.motorAnimator.UpdateValue(0f, true);
		this.grindAnimator.UpdateValue(0f, true);
	}

	// Token: 0x04004D4E RID: 19790
	[SerializeField]
	private AudioSource hum1;

	// Token: 0x04004D4F RID: 19791
	[SerializeField]
	private SoundBankPlayer turnSounds;

	// Token: 0x04004D50 RID: 19792
	private bool didInitHum1BaseVolume;

	// Token: 0x04004D51 RID: 19793
	private float hum1BaseVolume;

	// Token: 0x04004D52 RID: 19794
	[SerializeField]
	private float fadeSpeed;

	// Token: 0x04004D53 RID: 19795
	[SerializeField]
	private AudioAnimator windRushAnimator;

	// Token: 0x04004D54 RID: 19796
	[SerializeField]
	private AudioAnimator motorAnimator;

	// Token: 0x04004D55 RID: 19797
	[SerializeField]
	private AudioAnimator grindAnimator;

	// Token: 0x04004D56 RID: 19798
	[SerializeField]
	private float turnSoundCooldownDuration;

	// Token: 0x04004D57 RID: 19799
	[SerializeField]
	private float minAngleDeltaForTurnSound;

	// Token: 0x04004D58 RID: 19800
	private float turnSoundCooldownUntilTimestamp;
}
