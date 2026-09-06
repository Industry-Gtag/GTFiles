using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000F21 RID: 3873
	public class IntAverages : AverageCalculator<int>
	{
		// Token: 0x06005F01 RID: 24321 RVA: 0x001E36F2 File Offset: 0x001E18F2
		public IntAverages(int sampleCount)
			: base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x06005F02 RID: 24322 RVA: 0x001E36AD File Offset: 0x001E18AD
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override int PlusEquals(int value, int samples)
		{
			return value + samples;
		}

		// Token: 0x06005F03 RID: 24323 RVA: 0x001E36B2 File Offset: 0x001E18B2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override int MinusEquals(int value, int samples)
		{
			return value - samples;
		}

		// Token: 0x06005F04 RID: 24324 RVA: 0x001E3701 File Offset: 0x001E1901
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override int Divide(int value, int samples)
		{
			return value / samples;
		}

		// Token: 0x06005F05 RID: 24325 RVA: 0x001E3706 File Offset: 0x001E1906
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override int Multiply(int value, int samples)
		{
			return value * samples;
		}
	}
}
