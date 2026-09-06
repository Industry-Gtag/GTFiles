using System;
using Photon.Voice;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012A5 RID: 4773
	internal class ProcessVoiceDataToLoudness : IProcessor<float>, IDisposable
	{
		// Token: 0x060077EF RID: 30703 RVA: 0x0026D20E File Offset: 0x0026B40E
		public ProcessVoiceDataToLoudness(VoiceToLoudness voiceToLoudness)
		{
			this._voiceToLoudness = voiceToLoudness;
		}

		// Token: 0x060077F0 RID: 30704 RVA: 0x0026D220 File Offset: 0x0026B420
		public float[] Process(float[] buf)
		{
			float num = 0f;
			for (int i = 0; i < buf.Length; i++)
			{
				num += Mathf.Abs(buf[i]);
			}
			this._voiceToLoudness.Loudness = num / (float)buf.Length;
			return buf;
		}

		// Token: 0x060077F1 RID: 30705 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void Dispose()
		{
		}

		// Token: 0x04008824 RID: 34852
		private VoiceToLoudness _voiceToLoudness;
	}
}
