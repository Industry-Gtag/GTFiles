using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000F20 RID: 3872
	public class NetTimeAverages : DoubleAverages
	{
		// Token: 0x06005EFF RID: 24319 RVA: 0x001E36DE File Offset: 0x001E18DE
		public NetTimeAverages(int sampleCount)
			: base(sampleCount)
		{
		}

		// Token: 0x06005F00 RID: 24320 RVA: 0x001E36E7 File Offset: 0x001E18E7
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double DefaultTypeValue()
		{
			return 1.0;
		}
	}
}
