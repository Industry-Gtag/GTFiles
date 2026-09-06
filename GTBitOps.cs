using System;
using System.Runtime.CompilerServices;

// Token: 0x02000345 RID: 837
public static class GTBitOps
{
	// Token: 0x0600148D RID: 5261 RVA: 0x0006E35D File Offset: 0x0006C55D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetValueMask(int count)
	{
		return (1 << count) - 1;
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x0006E367 File Offset: 0x0006C567
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMask(int index, int valueMask)
	{
		return ~(valueMask << index);
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x0006E370 File Offset: 0x0006C570
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMaskByCount(int index, int count)
	{
		return ~((1 << count) - 1 << index);
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x0006E380 File Offset: 0x0006C580
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, int index, int valueMask)
	{
		return (bits >> index) & valueMask;
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x0006E38A File Offset: 0x0006C58A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, GTBitOps.BitWriteInfo info)
	{
		return (bits >> info.index) & info.valueMask;
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x0006E39E File Offset: 0x0006C59E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBitsByCount(int bits, int index, int count)
	{
		return (bits >> index) & ((1 << count) - 1);
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x0006E3AF File Offset: 0x0006C5AF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ReadBit(int bits, int index)
	{
		return ((bits >> index) & 1) == 1;
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x0006E3BC File Offset: 0x0006C5BC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, GTBitOps.BitWriteInfo info, int value)
	{
		bits = (bits & info.clearMask) | ((value & info.valueMask) << info.index);
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x0006E3DC File Offset: 0x0006C5DC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, GTBitOps.BitWriteInfo info, int value)
	{
		GTBitOps.WriteBits(ref bits, info, value);
		return bits;
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x0006E3E8 File Offset: 0x0006C5E8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, int index, int valueMask, int clearMask, int value)
	{
		bits = (bits & clearMask) | ((value & valueMask) << index);
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x0006E3FA File Offset: 0x0006C5FA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, int index, int valueMask, int clearMask, int value)
	{
		GTBitOps.WriteBits(ref bits, index, valueMask, clearMask, value);
		return bits;
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x0006E409 File Offset: 0x0006C609
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBitsByCount(ref int bits, int index, int count, int value)
	{
		bits = (bits & ~((1 << count) - 1 << index)) | ((value & ((1 << count) - 1)) << index);
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x0006E42E File Offset: 0x0006C62E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBitsByCount(int bits, int index, int count, int value)
	{
		GTBitOps.WriteBitsByCount(ref bits, index, count, value);
		return bits;
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x0006E43B File Offset: 0x0006C63B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBit(ref int bits, int index, bool value)
	{
		bits = (bits & ~(1 << index)) | ((value ? 1 : 0) << index);
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x0006E456 File Offset: 0x0006C656
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBit(int bits, int index, bool value)
	{
		GTBitOps.WriteBit(ref bits, index, value);
		return bits;
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x0006E462 File Offset: 0x0006C662
	public static string ToBinaryString(int number)
	{
		return Convert.ToString(number, 2).PadLeft(32, '0');
	}

	// Token: 0x02000346 RID: 838
	public readonly struct BitWriteInfo
	{
		// Token: 0x0600149D RID: 5277 RVA: 0x0006E474 File Offset: 0x0006C674
		public BitWriteInfo(int index, int count)
		{
			this.index = index;
			this.valueMask = GTBitOps.GetValueMask(count);
			this.clearMask = GTBitOps.GetClearMask(index, this.valueMask);
		}

		// Token: 0x0400195F RID: 6495
		public readonly int index;

		// Token: 0x04001960 RID: 6496
		public readonly int valueMask;

		// Token: 0x04001961 RID: 6497
		public readonly int clearMask;
	}
}
