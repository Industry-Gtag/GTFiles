using System;

namespace emotitron.Compression
{
	// Token: 0x020013D8 RID: 5080
	public static class PrimitivePackBytesExt
	{
		// Token: 0x06007FAE RID: 32686 RVA: 0x0029B1C8 File Offset: 0x002993C8
		public static ulong WritePackedBytes(this ulong buffer, ulong value, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write(value, ref bitposition, num2 << 3);
			return buffer;
		}

		// Token: 0x06007FAF RID: 32687 RVA: 0x0029B204 File Offset: 0x00299404
		public static uint WritePackedBytes(this uint buffer, uint value, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2 << 3);
			return buffer;
		}

		// Token: 0x06007FB0 RID: 32688 RVA: 0x0029B240 File Offset: 0x00299440
		public static void InjectPackedBytes(this ulong value, ref ulong buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write(value, ref bitposition, num2 << 3);
		}

		// Token: 0x06007FB1 RID: 32689 RVA: 0x0029B27C File Offset: 0x0029947C
		public static void InjectPackedBytes(this uint value, ref uint buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2 << 3);
		}

		// Token: 0x06007FB2 RID: 32690 RVA: 0x0029B2B8 File Offset: 0x002994B8
		public static ulong ReadPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2 << 3);
		}

		// Token: 0x06007FB3 RID: 32691 RVA: 0x0029B2E4 File Offset: 0x002994E4
		public static uint ReadPackedBytes(this uint buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2 << 3);
		}

		// Token: 0x06007FB4 RID: 32692 RVA: 0x0029B310 File Offset: 0x00299510
		public static ulong WriteSignedPackedBytes(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007FB5 RID: 32693 RVA: 0x0029B330 File Offset: 0x00299530
		public static int ReadSignedPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}
	}
}
