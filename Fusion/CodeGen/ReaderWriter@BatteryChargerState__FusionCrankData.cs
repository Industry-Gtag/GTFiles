using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x020014AB RID: 5291
	[WeaverGenerated]
	internal struct ReaderWriter@BatteryChargerState__FusionCrankData : IElementReaderWriter<BatteryChargerState.FusionCrankData>
	{
		// Token: 0x0600841C RID: 33820 RVA: 0x002B2159 File Offset: 0x002B0359
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe BatteryChargerState.FusionCrankData Read(byte* data, int index)
		{
			return *(BatteryChargerState.FusionCrankData*)(data + index * 12);
		}

		// Token: 0x0600841D RID: 33821 RVA: 0x002B2169 File Offset: 0x002B0369
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe ref BatteryChargerState.FusionCrankData ReadRef(byte* data, int index)
		{
			return ref *(BatteryChargerState.FusionCrankData*)(data + index * 12);
		}

		// Token: 0x0600841E RID: 33822 RVA: 0x002B2174 File Offset: 0x002B0374
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, BatteryChargerState.FusionCrankData val)
		{
			*(BatteryChargerState.FusionCrankData*)(data + index * 12) = val;
		}

		// Token: 0x0600841F RID: 33823 RVA: 0x00138C59 File Offset: 0x00136E59
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 3;
		}

		// Token: 0x06008420 RID: 33824 RVA: 0x002B2188 File Offset: 0x002B0388
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementHashCode(BatteryChargerState.FusionCrankData val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06008421 RID: 33825 RVA: 0x002B21A4 File Offset: 0x002B03A4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<BatteryChargerState.FusionCrankData> GetInstance()
		{
			if (ReaderWriter@BatteryChargerState__FusionCrankData.Instance == null)
			{
				ReaderWriter@BatteryChargerState__FusionCrankData.Instance = default(ReaderWriter@BatteryChargerState__FusionCrankData);
			}
			return ReaderWriter@BatteryChargerState__FusionCrankData.Instance;
		}

		// Token: 0x04009818 RID: 38936
		[WeaverGenerated]
		public static IElementReaderWriter<BatteryChargerState.FusionCrankData> Instance;
	}
}
