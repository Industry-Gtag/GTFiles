using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000F1E RID: 3870
	public class DoubleAverages : AverageCalculator<double>
	{
		// Token: 0x06005EF5 RID: 24309 RVA: 0x001E369E File Offset: 0x001E189E
		public DoubleAverages(int sampleCount)
			: base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x06005EF6 RID: 24310 RVA: 0x001E36AD File Offset: 0x001E18AD
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double PlusEquals(double value, double sample)
		{
			return value + sample;
		}

		// Token: 0x06005EF7 RID: 24311 RVA: 0x001E36B2 File Offset: 0x001E18B2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double MinusEquals(double value, double sample)
		{
			return value - sample;
		}

		// Token: 0x06005EF8 RID: 24312 RVA: 0x001E36B7 File Offset: 0x001E18B7
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double Divide(double value, int sampleCount)
		{
			return value / (double)sampleCount;
		}

		// Token: 0x06005EF9 RID: 24313 RVA: 0x001E36BD File Offset: 0x001E18BD
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double Multiply(double value, int sampleCount)
		{
			return value * (double)sampleCount;
		}
	}
}
