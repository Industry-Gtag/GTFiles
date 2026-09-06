using System;
using UnityEngine;

// Token: 0x020002AE RID: 686
public class AudioAnimator : MonoBehaviour
{
	// Token: 0x060011DF RID: 4575 RVA: 0x0005FDA6 File Offset: 0x0005DFA6
	private void Start()
	{
		if (!this.didInitBaseVolume)
		{
			this.InitBaseVolume();
		}
	}

	// Token: 0x060011E0 RID: 4576 RVA: 0x0005FDB8 File Offset: 0x0005DFB8
	private void InitBaseVolume()
	{
		for (int i = 0; i < this.targets.Length; i++)
		{
			this.targets[i].baseVolume = this.targets[i].audioSource.volume;
		}
		this.didInitBaseVolume = true;
	}

	// Token: 0x060011E1 RID: 4577 RVA: 0x0005FE06 File Offset: 0x0005E006
	public void UpdateValue(float value, bool ignoreSmoothing = false)
	{
		this.UpdatePitchAndVolume(value, value, ignoreSmoothing);
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x0005FE14 File Offset: 0x0005E014
	public void UpdatePitchAndVolume(float pitchValue, float volumeValue, bool ignoreSmoothing = false)
	{
		if (!this.didInitBaseVolume)
		{
			this.InitBaseVolume();
		}
		for (int i = 0; i < this.targets.Length; i++)
		{
			AudioAnimator.AudioTarget audioTarget = this.targets[i];
			float num = audioTarget.pitchCurve.Evaluate(pitchValue);
			float num2 = Mathf.Pow(1.05946f, num);
			audioTarget.audioSource.pitch = num2;
			float num3 = audioTarget.volumeCurve.Evaluate(volumeValue);
			float volume = audioTarget.audioSource.volume;
			float num4 = audioTarget.baseVolume * num3;
			if (ignoreSmoothing)
			{
				audioTarget.audioSource.volume = num4;
			}
			else if (volume > num4)
			{
				audioTarget.audioSource.volume = Mathf.MoveTowards(audioTarget.audioSource.volume, audioTarget.baseVolume * num3, (1f - audioTarget.lowerSmoothing) * audioTarget.baseVolume * Time.deltaTime * 90f);
			}
			else
			{
				audioTarget.audioSource.volume = Mathf.MoveTowards(audioTarget.audioSource.volume, audioTarget.baseVolume * num3, (1f - audioTarget.riseSmoothing) * audioTarget.baseVolume * Time.deltaTime * 90f);
			}
		}
	}

	// Token: 0x0400156B RID: 5483
	private bool didInitBaseVolume;

	// Token: 0x0400156C RID: 5484
	[SerializeField]
	private AudioAnimator.AudioTarget[] targets;

	// Token: 0x020002AF RID: 687
	[Serializable]
	private struct AudioTarget
	{
		// Token: 0x0400156D RID: 5485
		public AudioSource audioSource;

		// Token: 0x0400156E RID: 5486
		public AnimationCurve pitchCurve;

		// Token: 0x0400156F RID: 5487
		public AnimationCurve volumeCurve;

		// Token: 0x04001570 RID: 5488
		[NonSerialized]
		public float baseVolume;

		// Token: 0x04001571 RID: 5489
		public float riseSmoothing;

		// Token: 0x04001572 RID: 5490
		public float lowerSmoothing;
	}
}
