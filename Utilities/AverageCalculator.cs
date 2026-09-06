using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000F1D RID: 3869
	public abstract class AverageCalculator<T> where T : struct
	{
		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x06005EEC RID: 24300 RVA: 0x001E357D File Offset: 0x001E177D
		public T Average
		{
			get
			{
				return this.m_average;
			}
		}

		// Token: 0x06005EED RID: 24301 RVA: 0x001E3585 File Offset: 0x001E1785
		public AverageCalculator(int sampleCount)
		{
			this.m_samples = new T[sampleCount];
		}

		// Token: 0x06005EEE RID: 24302 RVA: 0x001E359C File Offset: 0x001E179C
		public virtual void AddSample(T sample)
		{
			T t = this.m_samples[this.m_index];
			this.m_total = this.MinusEquals(this.m_total, t);
			this.m_total = this.PlusEquals(this.m_total, sample);
			this.m_average = this.Divide(this.m_total, this.m_samples.Length);
			this.m_samples[this.m_index] = sample;
			int num = this.m_index + 1;
			this.m_index = num;
			this.m_index = num % this.m_samples.Length;
		}

		// Token: 0x06005EEF RID: 24303 RVA: 0x001E3630 File Offset: 0x001E1830
		public virtual void Reset()
		{
			T t = this.DefaultTypeValue();
			for (int i = 0; i < this.m_samples.Length; i++)
			{
				this.m_samples[i] = t;
			}
			this.m_index = 0;
			this.m_average = t;
			this.m_total = this.Multiply(t, this.m_samples.Length);
		}

		// Token: 0x06005EF0 RID: 24304 RVA: 0x001E3688 File Offset: 0x001E1888
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual T DefaultTypeValue()
		{
			return default(T);
		}

		// Token: 0x06005EF1 RID: 24305
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T PlusEquals(T value, T sample);

		// Token: 0x06005EF2 RID: 24306
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T MinusEquals(T value, T sample);

		// Token: 0x06005EF3 RID: 24307
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T Divide(T value, int sampleCount);

		// Token: 0x06005EF4 RID: 24308
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T Multiply(T value, int sampleCount);

		// Token: 0x04006D93 RID: 28051
		private T[] m_samples;

		// Token: 0x04006D94 RID: 28052
		private T m_average;

		// Token: 0x04006D95 RID: 28053
		private T m_total;

		// Token: 0x04006D96 RID: 28054
		private int m_index;
	}
}
