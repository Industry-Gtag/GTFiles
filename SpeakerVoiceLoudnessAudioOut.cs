using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020003A1 RID: 929
public class SpeakerVoiceLoudnessAudioOut : UnityAudioOut
{
	// Token: 0x06001687 RID: 5767 RVA: 0x00082B24 File Offset: 0x00080D24
	public SpeakerVoiceLoudnessAudioOut(SpeakerVoiceToLoudness speaker, AudioSource audioSource, AudioOutDelayControl.PlayDelayConfig playDelayConfig, Photon.Voice.ILogger logger, string logPrefix, bool debugInfo)
		: base(audioSource, playDelayConfig, logger, logPrefix, debugInfo)
	{
		this.voiceToLoudness = speaker;
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x00082B3C File Offset: 0x00080D3C
	public override void OutWrite(float[] data, int offsetSamples)
	{
		float num = 0f;
		for (int i = 0; i < data.Length; i++)
		{
			float num2 = data[i];
			if (!float.IsFinite(num2))
			{
				num2 = 0f;
				data[i] = num2;
			}
			else if (num2 > 1f)
			{
				num2 = 1f;
				data[i] = num2;
			}
			else if (num2 < -1f)
			{
				num2 = -1f;
				data[i] = num2;
			}
			num += Mathf.Abs(num2);
		}
		if (num > 0f)
		{
			float num3 = num / (float)data.Length;
			this.voiceToLoudness.loudness = num3;
			if (SpeakerVoiceToLoudnessConfig.EnableLoudnessLimit && num3 > SpeakerVoiceToLoudnessConfig.LoudnessLimitThreshold)
			{
				data = SpeakerVoiceToLoudnessConfig.StaticArrays.GetStaticArray(data.Length);
			}
		}
		base.OutWrite(data, offsetSamples);
	}

	// Token: 0x040020A8 RID: 8360
	private SpeakerVoiceToLoudness voiceToLoudness;
}
