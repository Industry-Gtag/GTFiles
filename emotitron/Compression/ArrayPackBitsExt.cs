using System;

namespace emotitron.Compression
{
	// Token: 0x020013CF RID: 5071
	public static class ArrayPackBitsExt
	{
		// Token: 0x06007EEE RID: 32494 RVA: 0x0029917C File Offset: 0x0029737C
		public unsafe static void WritePackedBits(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int num2 = bits.UsedBitCount();
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, num2);
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, num);
		}

		// Token: 0x06007EEF RID: 32495 RVA: 0x002991B0 File Offset: 0x002973B0
		public static void WritePackedBits(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int num2 = bits.UsedBitCount();
			buffer.Write((ulong)num, ref bitposition, num2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x06007EF0 RID: 32496 RVA: 0x002991E4 File Offset: 0x002973E4
		public static void WritePackedBits(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int num2 = bits.UsedBitCount();
			buffer.Write((ulong)((long)num), ref bitposition, num2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x06007EF1 RID: 32497 RVA: 0x00299218 File Offset: 0x00297418
		public static void WritePackedBits(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			int num = value.UsedBitCount();
			int num2 = bits.UsedBitCount();
			buffer.Write((ulong)num, ref bitposition, num2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x06007EF2 RID: 32498 RVA: 0x00299248 File Offset: 0x00297448
		public unsafe static ulong ReadPackedBits(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = bits.UsedBitCount();
			int num2 = (int)ArraySerializeUnsafe.Read(uPtr, ref bitposition, num);
			return ArraySerializeUnsafe.Read(uPtr, ref bitposition, num2);
		}

		// Token: 0x06007EF3 RID: 32499 RVA: 0x00299274 File Offset: 0x00297474
		public static ulong ReadPackedBits(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06007EF4 RID: 32500 RVA: 0x002992A0 File Offset: 0x002974A0
		public static ulong ReadPackedBits(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06007EF5 RID: 32501 RVA: 0x002992CC File Offset: 0x002974CC
		public static ulong ReadPackedBits(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06007EF6 RID: 32502 RVA: 0x002992F8 File Offset: 0x002974F8
		public unsafe static void WriteSignedPackedBits(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArrayPackBitsExt.WritePackedBits(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007EF7 RID: 32503 RVA: 0x00299318 File Offset: 0x00297518
		public unsafe static int ReadSignedPackedBits(ulong* buffer, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBitsExt.ReadPackedBits(buffer, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007EF8 RID: 32504 RVA: 0x0029933C File Offset: 0x0029753C
		public static void WriteSignedPackedBits(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007EF9 RID: 32505 RVA: 0x0029935C File Offset: 0x0029755C
		public static int ReadSignedPackedBits(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007EFA RID: 32506 RVA: 0x00299380 File Offset: 0x00297580
		public static void WriteSignedPackedBits(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007EFB RID: 32507 RVA: 0x002993A0 File Offset: 0x002975A0
		public static int ReadSignedPackedBits(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007EFC RID: 32508 RVA: 0x002993C4 File Offset: 0x002975C4
		public static void WriteSignedPackedBits(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007EFD RID: 32509 RVA: 0x002993E4 File Offset: 0x002975E4
		public static int ReadSignedPackedBits(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007EFE RID: 32510 RVA: 0x00299408 File Offset: 0x00297608
		public static void WriteSignedPackedBits64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.WritePackedBits(num, ref bitposition, bits);
		}

		// Token: 0x06007EFF RID: 32511 RVA: 0x00299428 File Offset: 0x00297628
		public static long ReadSignedPackedBits64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBits(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}
	}
}
