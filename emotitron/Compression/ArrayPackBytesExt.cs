using System;

namespace emotitron.Compression
{
	// Token: 0x020013D0 RID: 5072
	public static class ArrayPackBytesExt
	{
		// Token: 0x06007F00 RID: 32512 RVA: 0x00299448 File Offset: 0x00297648
		public unsafe static void WritePackedBytes(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			ArraySerializeUnsafe.Write(uPtr, (ulong)num2, ref bitposition, num);
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, num2 << 3);
		}

		// Token: 0x06007F01 RID: 32513 RVA: 0x00299480 File Offset: 0x00297680
		public static void WritePackedBytes(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer.Write((ulong)num2, ref bitposition, num);
			buffer.Write(value, ref bitposition, num2 << 3);
		}

		// Token: 0x06007F02 RID: 32514 RVA: 0x002994B8 File Offset: 0x002976B8
		public static void WritePackedBytes(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer.Write((ulong)num2, ref bitposition, num);
			buffer.Write(value, ref bitposition, num2 << 3);
		}

		// Token: 0x06007F03 RID: 32515 RVA: 0x002994F0 File Offset: 0x002976F0
		public static void WritePackedBytes(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer.Write((ulong)num2, ref bitposition, num);
			buffer.Write(value, ref bitposition, num2 << 3);
		}

		// Token: 0x06007F04 RID: 32516 RVA: 0x00299528 File Offset: 0x00297728
		public unsafe static ulong ReadPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)ArraySerializeUnsafe.Read(uPtr, ref bitposition, num) << 3;
			return ArraySerializeUnsafe.Read(uPtr, ref bitposition, num2);
		}

		// Token: 0x06007F05 RID: 32517 RVA: 0x0029955C File Offset: 0x0029775C
		public static ulong ReadPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num) << 3;
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06007F06 RID: 32518 RVA: 0x00299590 File Offset: 0x00297790
		public static ulong ReadPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num) << 3;
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06007F07 RID: 32519 RVA: 0x002995C4 File Offset: 0x002977C4
		public static ulong ReadPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num) << 3;
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06007F08 RID: 32520 RVA: 0x002995F8 File Offset: 0x002977F8
		public unsafe static void WriteSignedPackedBytes(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArrayPackBytesExt.WritePackedBytes(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F09 RID: 32521 RVA: 0x00299618 File Offset: 0x00297818
		public unsafe static int ReadSignedPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBytesExt.ReadPackedBytes(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007F0A RID: 32522 RVA: 0x0029963C File Offset: 0x0029783C
		public static void WriteSignedPackedBytes(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F0B RID: 32523 RVA: 0x0029965C File Offset: 0x0029785C
		public static int ReadSignedPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007F0C RID: 32524 RVA: 0x00299680 File Offset: 0x00297880
		public static void WriteSignedPackedBytes(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F0D RID: 32525 RVA: 0x002996A0 File Offset: 0x002978A0
		public static int ReadSignedPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007F0E RID: 32526 RVA: 0x002996C4 File Offset: 0x002978C4
		public static void WriteSignedPackedBytes(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F0F RID: 32527 RVA: 0x002996E4 File Offset: 0x002978E4
		public static int ReadSignedPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007F10 RID: 32528 RVA: 0x00299708 File Offset: 0x00297908
		public static void WriteSignedPackedBytes64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.WritePackedBytes(num, ref bitposition, bits);
		}

		// Token: 0x06007F11 RID: 32529 RVA: 0x00299728 File Offset: 0x00297928
		public static long ReadSignedPackedBytes64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBytes(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}
	}
}
