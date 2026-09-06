using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012A7 RID: 4775
	public class GTMicWrapper : MicWrapper
	{
		// Token: 0x060077F7 RID: 30711 RVA: 0x0026D33C File Offset: 0x0026B53C
		public GTMicWrapper(string device, int suggestedFrequency, bool allowPitchAdjustment, float pitchAdjustment, bool allowVolumeAdjustment, float volumeAdjustment, Photon.Voice.ILogger logger)
			: base(device, suggestedFrequency, logger)
		{
			this.UpdatePitchAdjustment(allowPitchAdjustment, pitchAdjustment);
			this.UpdateVolumeAdjustment(allowVolumeAdjustment, volumeAdjustment);
		}

		// Token: 0x060077F8 RID: 30712 RVA: 0x0026D428 File Offset: 0x0026B628
		public void UpdateWrapper(bool allowPitchAdjustment, float pitchAdjustment, bool allowVolumeAdjustment, float volumeAdjustment)
		{
			this.UpdatePitchAdjustment(allowPitchAdjustment, pitchAdjustment);
			this.UpdateVolumeAdjustment(allowVolumeAdjustment, volumeAdjustment);
		}

		// Token: 0x060077F9 RID: 30713 RVA: 0x0026D43B File Offset: 0x0026B63B
		public void UpdatePitchAdjustment(bool allow, float pitchAdjustment)
		{
			this._allowPitchAdjustment = allow;
			this._pitchAdjustment = pitchAdjustment;
		}

		// Token: 0x060077FA RID: 30714 RVA: 0x0026D44B File Offset: 0x0026B64B
		public void UpdateVolumeAdjustment(bool allow, float volumeAdjustment)
		{
			this._allowVolumeAdjustment = allow;
			this._volumeAdjustment = volumeAdjustment;
		}

		// Token: 0x060077FB RID: 30715 RVA: 0x0026D45C File Offset: 0x0026B65C
		public override bool Read(float[] buffer)
		{
			if (base.Error != null)
			{
				return false;
			}
			int position = UnityMicrophone.GetPosition(this.device);
			if (position < this.micPrevPos)
			{
				this.micLoopCnt++;
			}
			this.micPrevPos = position;
			int num = this.micLoopCnt * this.mic.samples + position;
			if (this.mic.channels == 0)
			{
				base.Error = "Number of channels is 0 in Read()";
				this.logger.LogError("[PV] MicWrapper: " + base.Error, Array.Empty<object>());
				return false;
			}
			int num2 = buffer.Length / this.mic.channels;
			int num3 = this.readAbsPos + num2;
			if (num3 < num)
			{
				this.mic.GetData(buffer, this.readAbsPos % this.mic.samples);
				this.readAbsPos = num3;
				float num4 = Mathf.Clamp(this._pitchAdjustment, 0.5f, 2f);
				if (this._allowPitchAdjustment && !Mathf.Approximately(num4, 1f))
				{
					this.PitchShift(num4, (long)num2, (float)base.SamplingRate, buffer);
				}
				if (this._allowVolumeAdjustment && !Mathf.Approximately(this._volumeAdjustment, 1f))
				{
					for (int i = 0; i < buffer.Length; i++)
					{
						buffer[i] = Mathf.Clamp(buffer[i] * this._volumeAdjustment, float.MinValue, float.MaxValue);
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x060077FC RID: 30716 RVA: 0x0026D5BE File Offset: 0x0026B7BE
		private void PitchShift(float pitchShift, long numSampsToProcess, float sampleRate, float[] indata)
		{
			this.PitchShift(pitchShift, numSampsToProcess, 2048L, 10L, sampleRate, indata);
		}

		// Token: 0x060077FD RID: 30717 RVA: 0x0026D5D4 File Offset: 0x0026B7D4
		public void PitchShift(float pitchShift, long numSampsToProcess, long fftFrameSize, long osamp, float sampleRate, float[] indata)
		{
			long num = fftFrameSize / 2L;
			long num2 = fftFrameSize / osamp;
			double num3 = (double)sampleRate / (double)fftFrameSize;
			double num4 = 6.283185307179586 * (double)num2 / (double)fftFrameSize;
			long num5 = fftFrameSize - num2;
			if (this._gRover == 0L)
			{
				this._gRover = num5;
			}
			for (long num6 = 0L; num6 < numSampsToProcess; num6 += 1L)
			{
				checked
				{
					this.InFifo[(int)((IntPtr)this._gRover)] = indata[(int)((IntPtr)num6)];
					indata[(int)((IntPtr)num6)] = this.OutFifo[(int)((IntPtr)(unchecked(this._gRover - num5)))];
				}
				this._gRover += 1L;
				if (this._gRover >= fftFrameSize)
				{
					this._gRover = num5;
					for (long num7 = 0L; num7 < fftFrameSize; num7 += 1L)
					{
						double num8 = -0.5 * Math.Cos(6.283185307179586 * (double)num7 / (double)fftFrameSize) + 0.5;
						this.FfTworksp[(int)(checked((IntPtr)(unchecked(2L * num7))))] = (float)((double)this.InFifo[(int)(checked((IntPtr)num7))] * num8);
						checked
						{
							this.FfTworksp[(int)((IntPtr)(unchecked(2L * num7 + 1L)))] = 0f;
						}
					}
					this.ShortTimeFourierTransform(this.FfTworksp, fftFrameSize, -1L);
					for (long num7 = 0L; num7 <= num; num7 += 1L)
					{
						double num9;
						double num10;
						checked
						{
							num9 = (double)this.FfTworksp[(int)((IntPtr)(unchecked(2L * num7)))];
							num10 = (double)this.FfTworksp[(int)((IntPtr)(unchecked(2L * num7 + 1L)))];
						}
						double num11 = 2.0 * Math.Sqrt(num9 * num9 + num10 * num10);
						double num12 = Math.Atan2(num10, num9);
						double num13 = num12 - (double)this.LastPhase[(int)(checked((IntPtr)num7))];
						this.LastPhase[(int)(checked((IntPtr)num7))] = (float)num12;
						num13 -= (double)num7 * num4;
						long num14 = (long)(num13 / 3.141592653589793);
						if (num14 >= 0L)
						{
							num14 += num14 & 1L;
						}
						else
						{
							num14 -= num14 & 1L;
						}
						num13 -= 3.141592653589793 * (double)num14;
						num13 = (double)osamp * num13 / 6.283185307179586;
						num13 = (double)num7 * num3 + num13 * num3;
						checked
						{
							this.AnaMagn[(int)((IntPtr)num7)] = (float)num11;
							this.AnaFreq[(int)((IntPtr)num7)] = (float)num13;
						}
					}
					int num15 = 0;
					while ((long)num15 < fftFrameSize)
					{
						this.SynMagn[num15] = 0f;
						this.SynFreq[num15] = 0f;
						num15++;
					}
					for (long num7 = 0L; num7 <= num; num7 += 1L)
					{
						long num16 = (long)((float)num7 * pitchShift);
						if (num16 <= num)
						{
							this.SynMagn[(int)(checked((IntPtr)num16))] += this.AnaMagn[(int)(checked((IntPtr)num7))];
							this.SynFreq[(int)(checked((IntPtr)num16))] = this.AnaFreq[(int)(checked((IntPtr)num7))] * pitchShift;
						}
					}
					for (long num7 = 0L; num7 <= num; num7 += 1L)
					{
						double num11;
						double num13;
						checked
						{
							num11 = (double)this.SynMagn[(int)((IntPtr)num7)];
							num13 = (double)this.SynFreq[(int)((IntPtr)num7)];
						}
						num13 -= (double)num7 * num3;
						num13 /= num3;
						num13 = 6.283185307179586 * num13 / (double)osamp;
						num13 += (double)num7 * num4;
						this.SumPhase[(int)(checked((IntPtr)num7))] += (float)num13;
						checked
						{
							double num12 = (double)this.SumPhase[(int)((IntPtr)num7)];
							this.FfTworksp[(int)((IntPtr)(unchecked(2L * num7)))] = (float)(unchecked(num11 * Math.Cos(num12)));
							this.FfTworksp[(int)((IntPtr)(unchecked(2L * num7 + 1L)))] = (float)(unchecked(num11 * Math.Sin(num12)));
						}
					}
					for (long num7 = fftFrameSize + 2L; num7 < 2L * fftFrameSize; num7 += 1L)
					{
						this.FfTworksp[(int)(checked((IntPtr)num7))] = 0f;
					}
					this.ShortTimeFourierTransform(this.FfTworksp, fftFrameSize, 1L);
					for (long num7 = 0L; num7 < fftFrameSize; num7 += 1L)
					{
						double num8 = -0.5 * Math.Cos(6.283185307179586 * (double)num7 / (double)fftFrameSize) + 0.5;
						this.OutputAccum[(int)(checked((IntPtr)num7))] += (float)(2.0 * num8 * (double)this.FfTworksp[(int)(checked((IntPtr)(unchecked(2L * num7))))] / (double)(num * osamp));
					}
					for (long num7 = 0L; num7 < num2; num7 += 1L)
					{
						checked
						{
							this.OutFifo[(int)((IntPtr)num7)] = this.OutputAccum[(int)((IntPtr)num7)];
						}
					}
					for (long num7 = 0L; num7 < fftFrameSize; num7 += 1L)
					{
						checked
						{
							this.OutputAccum[(int)((IntPtr)num7)] = this.OutputAccum[(int)((IntPtr)(unchecked(num7 + num2)))];
						}
					}
					for (long num7 = 0L; num7 < num5; num7 += 1L)
					{
						checked
						{
							this.InFifo[(int)((IntPtr)num7)] = this.InFifo[(int)((IntPtr)(unchecked(num7 + num2)))];
						}
					}
				}
			}
		}

		// Token: 0x060077FE RID: 30718 RVA: 0x0026DA4C File Offset: 0x0026BC4C
		public void ShortTimeFourierTransform(float[] fftBuffer, long fftFrameSize, long sign)
		{
			for (long num = 2L; num < 2L * fftFrameSize - 2L; num += 2L)
			{
				long num2 = 2L;
				long num3 = 0L;
				while (num2 < 2L * fftFrameSize)
				{
					if ((num & num2) != 0L)
					{
						num3 += 1L;
					}
					num3 <<= 1;
					num2 <<= 1;
				}
				checked
				{
					if (num < num3)
					{
						float num4 = fftBuffer[(int)((IntPtr)num)];
						fftBuffer[(int)((IntPtr)num)] = fftBuffer[(int)((IntPtr)num3)];
						fftBuffer[(int)((IntPtr)num3)] = num4;
						num4 = fftBuffer[(int)((IntPtr)(unchecked(num + 1L)))];
						fftBuffer[(int)((IntPtr)(unchecked(num + 1L)))] = fftBuffer[(int)((IntPtr)(unchecked(num3 + 1L)))];
						fftBuffer[(int)((IntPtr)(unchecked(num3 + 1L)))] = num4;
					}
				}
			}
			long num5 = (long)(Math.Log((double)fftFrameSize) / Math.Log(2.0) + 0.5);
			long num6 = 0L;
			long num7 = 2L;
			while (num6 < num5)
			{
				num7 <<= 1;
				long num8 = num7 >> 1;
				float num9 = 1f;
				float num10 = 0f;
				float num11 = 3.1415927f / (float)(num8 >> 1);
				float num12 = (float)Math.Cos((double)num11);
				float num13 = (float)((double)sign * Math.Sin((double)num11));
				for (long num3 = 0L; num3 < num8; num3 += 2L)
				{
					float num14;
					for (long num = num3; num < 2L * fftFrameSize; num += num7)
					{
						num14 = fftBuffer[(int)(checked((IntPtr)(unchecked(num + num8))))] * num9 - fftBuffer[(int)(checked((IntPtr)(unchecked(num + num8 + 1L))))] * num10;
						float num15 = fftBuffer[(int)(checked((IntPtr)(unchecked(num + num8))))] * num10 + fftBuffer[(int)(checked((IntPtr)(unchecked(num + num8 + 1L))))] * num9;
						fftBuffer[(int)(checked((IntPtr)(unchecked(num + num8))))] = fftBuffer[(int)(checked((IntPtr)num))] - num14;
						fftBuffer[(int)(checked((IntPtr)(unchecked(num + num8 + 1L))))] = fftBuffer[(int)(checked((IntPtr)(unchecked(num + 1L))))] - num15;
						fftBuffer[(int)(checked((IntPtr)num))] += num14;
						fftBuffer[(int)(checked((IntPtr)(unchecked(num + 1L))))] += num15;
					}
					num14 = num9 * num12 - num10 * num13;
					num10 = num9 * num13 + num10 * num12;
					num9 = num14;
				}
				num6 += 1L;
			}
		}

		// Token: 0x04008828 RID: 34856
		private bool _allowPitchAdjustment;

		// Token: 0x04008829 RID: 34857
		private float _pitchAdjustment = 1f;

		// Token: 0x0400882A RID: 34858
		private bool _allowVolumeAdjustment;

		// Token: 0x0400882B RID: 34859
		private float _volumeAdjustment = 1f;

		// Token: 0x0400882C RID: 34860
		private static readonly int MaxFrameLength = 16000;

		// Token: 0x0400882D RID: 34861
		private readonly float[] InFifo = new float[GTMicWrapper.MaxFrameLength];

		// Token: 0x0400882E RID: 34862
		private readonly float[] OutFifo = new float[GTMicWrapper.MaxFrameLength];

		// Token: 0x0400882F RID: 34863
		private readonly float[] FfTworksp = new float[2 * GTMicWrapper.MaxFrameLength];

		// Token: 0x04008830 RID: 34864
		private readonly float[] LastPhase = new float[GTMicWrapper.MaxFrameLength / 2 + 1];

		// Token: 0x04008831 RID: 34865
		private readonly float[] SumPhase = new float[GTMicWrapper.MaxFrameLength / 2 + 1];

		// Token: 0x04008832 RID: 34866
		private readonly float[] OutputAccum = new float[2 * GTMicWrapper.MaxFrameLength];

		// Token: 0x04008833 RID: 34867
		private readonly float[] AnaFreq = new float[GTMicWrapper.MaxFrameLength];

		// Token: 0x04008834 RID: 34868
		private readonly float[] AnaMagn = new float[GTMicWrapper.MaxFrameLength];

		// Token: 0x04008835 RID: 34869
		private readonly float[] SynFreq = new float[GTMicWrapper.MaxFrameLength];

		// Token: 0x04008836 RID: 34870
		private readonly float[] SynMagn = new float[GTMicWrapper.MaxFrameLength];

		// Token: 0x04008837 RID: 34871
		private long _gRover;
	}
}
