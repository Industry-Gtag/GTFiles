using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000F1F RID: 3871
	public class FloatAverages : AverageCalculator<float>
	{
		// Token: 0x06005EFA RID: 24314 RVA: 0x001E36C3 File Offset: 0x001E18C3
		public FloatAverages(int sampleCount)
			: base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x06005EFB RID: 24315 RVA: 0x001E36AD File Offset: 0x001E18AD
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float PlusEquals(float value, float sample)
		{
			return value + sample;
		}

		// Token: 0x06005EFC RID: 24316 RVA: 0x001E36B2 File Offset: 0x001E18B2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float MinusEquals(float value, float sample)
		{
			return value - sample;
		}

		// Token: 0x06005EFD RID: 24317 RVA: 0x001E36D2 File Offset: 0x001E18D2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float Divide(float value, int sampleCount)
		{
			return value / (float)sampleCount;
		}

		// Token: 0x06005EFE RID: 24318 RVA: 0x001E36D8 File Offset: 0x001E18D8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float Multiply(float value, int sampleCount)
		{
			return value * (float)sampleCount;
		}
	}
}
