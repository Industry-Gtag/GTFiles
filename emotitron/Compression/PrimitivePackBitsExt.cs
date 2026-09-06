using System;

namespace emotitron.Compression
{
	// Token: 0x020013D7 RID: 5079
	public static class PrimitivePackBitsExt
	{
		// Token: 0x06007FA2 RID: 32674 RVA: 0x0029AFDC File Offset: 0x002991DC
		public static ulong WritePackedBits(this ulong buffer, uint value, ref int bitposition, int bits)
		{
			int num = ((uint)bits).UsedBitCount();
			int num2 = value.UsedBitCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2);
			return buffer;
		}

		// Token: 0x06007FA3 RID: 32675 RVA: 0x0029B010 File Offset: 0x00299210
		public static uint WritePackedBits(this uint buffer, ushort value, ref int bitposition, int bits)
		{
			int num = ((uint)bits).UsedBitCount();
			int num2 = value.UsedBitCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2);
			return buffer;
		}

		// Token: 0x06007FA4 RID: 32676 RVA: 0x0029B044 File Offset: 0x00299244
		public static ushort WritePackedBits(this ushort buffer, byte value, ref int bitposition, int bits)
		{
			int num = ((uint)bits).UsedBitCount();
			int num2 = value.UsedBitCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2);
			return buffer;
		}

		// Token: 0x06007FA5 RID: 32677 RVA: 0x0029B078 File Offset: 0x00299278
		public static ulong ReadPackedBits(this ulong buffer, ref int bitposition, int bits)
		{
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06007FA6 RID: 32678 RVA: 0x0029B0A0 File Offset: 0x002992A0
		public static ulong ReadPackedBits(this uint buffer, ref int bitposition, int bits)
		{
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return (ulong)buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06007FA7 RID: 32679 RVA: 0x0029B0C8 File Offset: 0x002992C8
		public static ulong ReadPackedBits(this ushort buffer, ref int bitposition, int bits)
		{
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return (ulong)buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06007FA8 RID: 32680 RVA: 0x0029B0F0 File Offset: 0x002992F0
		public static ulong WriteSignedPackedBits(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer = buffer.WritePackedBits(num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06007FA9 RID: 32681 RVA: 0x0029B114 File Offset: 0x00299314
		public static uint WriteSignedPackedBits(this uint buffer, short value, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			buffer = buffer.WritePackedBits((ushort)num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06007FAA RID: 32682 RVA: 0x0029B138 File Offset: 0x00299338
		public static ushort WriteSignedPackedBits(this ushort buffer, sbyte value, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			buffer = buffer.WritePackedBits((byte)num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06007FAB RID: 32683 RVA: 0x0029B15C File Offset: 0x0029935C
		public static int ReadSignedPackedBits(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007FAC RID: 32684 RVA: 0x0029B180 File Offset: 0x00299380
		public static short ReadSignedPackedBits(this uint buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (short)((int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U)))));
		}

		// Token: 0x06007FAD RID: 32685 RVA: 0x0029B1A4 File Offset: 0x002993A4
		public static sbyte ReadSignedPackedBits(this ushort buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (sbyte)((int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U)))));
		}
	}
}
