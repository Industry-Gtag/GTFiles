using System;
using emotitron.Compression.HalfFloat;
using emotitron.Compression.Utilities;

namespace emotitron.Compression
{
	// Token: 0x020013D9 RID: 5081
	public static class PrimitiveSerializeExt
	{
		// Token: 0x06007FB6 RID: 32694 RVA: 0x0029B352 File Offset: 0x00299552
		public static void Inject(this ByteConverter value, ref ulong buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FB7 RID: 32695 RVA: 0x0029B362 File Offset: 0x00299562
		public static void Inject(this ByteConverter value, ref uint buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FB8 RID: 32696 RVA: 0x0029B372 File Offset: 0x00299572
		public static void Inject(this ByteConverter value, ref ushort buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FB9 RID: 32697 RVA: 0x0029B382 File Offset: 0x00299582
		public static void Inject(this ByteConverter value, ref byte buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FBA RID: 32698 RVA: 0x0029B394 File Offset: 0x00299594
		public static ulong WriteSigned(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007FBB RID: 32699 RVA: 0x0029B3B4 File Offset: 0x002995B4
		public static void InjectSigned(this long value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FBC RID: 32700 RVA: 0x0029B3C7 File Offset: 0x002995C7
		public static void InjectSigned(this int value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FBD RID: 32701 RVA: 0x0029B3C7 File Offset: 0x002995C7
		public static void InjectSigned(this short value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FBE RID: 32702 RVA: 0x0029B3C7 File Offset: 0x002995C7
		public static void InjectSigned(this sbyte value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FBF RID: 32703 RVA: 0x0029B3DC File Offset: 0x002995DC
		public static int ReadSigned(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007FC0 RID: 32704 RVA: 0x0029B400 File Offset: 0x00299600
		public static uint WriteSigned(this uint buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007FC1 RID: 32705 RVA: 0x0029B420 File Offset: 0x00299620
		public static void InjectSigned(this long value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FC2 RID: 32706 RVA: 0x0029B433 File Offset: 0x00299633
		public static void InjectSigned(this int value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FC3 RID: 32707 RVA: 0x0029B433 File Offset: 0x00299633
		public static void InjectSigned(this short value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FC4 RID: 32708 RVA: 0x0029B433 File Offset: 0x00299633
		public static void InjectSigned(this sbyte value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FC5 RID: 32709 RVA: 0x0029B448 File Offset: 0x00299648
		public static int ReadSigned(this uint buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007FC6 RID: 32710 RVA: 0x0029B46C File Offset: 0x0029966C
		public static ushort WriteSigned(this ushort buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007FC7 RID: 32711 RVA: 0x0029B48C File Offset: 0x0029968C
		public static void InjectSigned(this long value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FC8 RID: 32712 RVA: 0x0029B49F File Offset: 0x0029969F
		public static void InjectSigned(this int value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FC9 RID: 32713 RVA: 0x0029B49F File Offset: 0x0029969F
		public static void InjectSigned(this short value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FCA RID: 32714 RVA: 0x0029B49F File Offset: 0x0029969F
		public static void InjectSigned(this sbyte value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FCB RID: 32715 RVA: 0x0029B4B4 File Offset: 0x002996B4
		public static int ReadSigned(this ushort buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007FCC RID: 32716 RVA: 0x0029B4D8 File Offset: 0x002996D8
		public static byte WriteSigned(this byte buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007FCD RID: 32717 RVA: 0x0029B4F8 File Offset: 0x002996F8
		public static void InjectSigned(this long value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FCE RID: 32718 RVA: 0x0029B50B File Offset: 0x0029970B
		public static void InjectSigned(this int value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FCF RID: 32719 RVA: 0x0029B50B File Offset: 0x0029970B
		public static void InjectSigned(this short value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FD0 RID: 32720 RVA: 0x0029B50B File Offset: 0x0029970B
		public static void InjectSigned(this sbyte value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06007FD1 RID: 32721 RVA: 0x0029B520 File Offset: 0x00299720
		public static int ReadSigned(this byte buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007FD2 RID: 32722 RVA: 0x0029B541 File Offset: 0x00299741
		public static ulong WritetBool(this ulong buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06007FD3 RID: 32723 RVA: 0x0029B553 File Offset: 0x00299753
		public static uint WritetBool(this uint buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06007FD4 RID: 32724 RVA: 0x0029B565 File Offset: 0x00299765
		public static ushort WritetBool(this ushort buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06007FD5 RID: 32725 RVA: 0x0029B577 File Offset: 0x00299777
		public static byte WritetBool(this byte buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06007FD6 RID: 32726 RVA: 0x0029B589 File Offset: 0x00299789
		public static void Inject(this bool value, ref ulong buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x06007FD7 RID: 32727 RVA: 0x0029B59B File Offset: 0x0029979B
		public static void Inject(this bool value, ref uint buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x06007FD8 RID: 32728 RVA: 0x0029B5AD File Offset: 0x002997AD
		public static void Inject(this bool value, ref ushort buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x06007FD9 RID: 32729 RVA: 0x0029B5BF File Offset: 0x002997BF
		public static void Inject(this bool value, ref byte buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x06007FDA RID: 32730 RVA: 0x0029B5D1 File Offset: 0x002997D1
		public static bool ReadBool(this ulong buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0UL;
		}

		// Token: 0x06007FDB RID: 32731 RVA: 0x0029B5E0 File Offset: 0x002997E0
		public static bool ReadtBool(this uint buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x06007FDC RID: 32732 RVA: 0x0029B5EF File Offset: 0x002997EF
		public static bool ReadBool(this ushort buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x06007FDD RID: 32733 RVA: 0x0029B5FE File Offset: 0x002997FE
		public static bool ReadBool(this byte buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x06007FDE RID: 32734 RVA: 0x0029B610 File Offset: 0x00299810
		public static ulong Write(this ulong buffer, ulong value, ref int bitposition, int bits = 64)
		{
			ulong num = value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= num2 & num;
			bitposition += bits;
			return buffer;
		}

		// Token: 0x06007FDF RID: 32735 RVA: 0x0029B64C File Offset: 0x0029984C
		public static uint Write(this uint buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = uint.MaxValue >> 32 - bits << bitposition;
			buffer &= ~num2;
			buffer |= num2 & num;
			bitposition += bits;
			return buffer;
		}

		// Token: 0x06007FE0 RID: 32736 RVA: 0x0029B688 File Offset: 0x00299888
		public static ushort Write(this ushort buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = 65535U >> 16 - bits << bitposition;
			buffer = (ushort)(((uint)buffer & ~num2) | (num2 & num));
			bitposition += bits;
			return buffer;
		}

		// Token: 0x06007FE1 RID: 32737 RVA: 0x0029B6C4 File Offset: 0x002998C4
		public static byte Write(this byte buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = 255U >> 8 - bits << bitposition;
			buffer = (byte)(((uint)buffer & ~num2) | (num2 & num));
			bitposition += bits;
			return buffer;
		}

		// Token: 0x06007FE2 RID: 32738 RVA: 0x0029B6FF File Offset: 0x002998FF
		public static void Inject(this ulong value, ref ulong buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06007FE3 RID: 32739 RVA: 0x0029B710 File Offset: 0x00299910
		public static void Inject(this ulong value, ref ulong buffer, int bitposition, int bits = 64)
		{
			ulong num = value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= num2 & num;
		}

		// Token: 0x06007FE4 RID: 32740 RVA: 0x0029B743 File Offset: 0x00299943
		public static void Inject(this uint value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FE5 RID: 32741 RVA: 0x0029B754 File Offset: 0x00299954
		public static void Inject(this uint value, ref ulong buffer, int bitposition, int bits = 32)
		{
			ulong num = (ulong)value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= num2 & num;
		}

		// Token: 0x06007FE6 RID: 32742 RVA: 0x0029B743 File Offset: 0x00299943
		public static void Inject(this ushort value, ref ulong buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FE7 RID: 32743 RVA: 0x0029B788 File Offset: 0x00299988
		public static void Inject(this ushort value, ref ulong buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FE8 RID: 32744 RVA: 0x0029B743 File Offset: 0x00299943
		public static void Inject(this byte value, ref ulong buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FE9 RID: 32745 RVA: 0x0029B788 File Offset: 0x00299988
		public static void Inject(this byte value, ref ulong buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FEA RID: 32746 RVA: 0x0029B6FF File Offset: 0x002998FF
		public static void InjectUnsigned(this long value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FEB RID: 32747 RVA: 0x0029B798 File Offset: 0x00299998
		public static void InjectUnsigned(this int value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007FEC RID: 32748 RVA: 0x0029B798 File Offset: 0x00299998
		public static void InjectUnsigned(this short value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007FED RID: 32749 RVA: 0x0029B798 File Offset: 0x00299998
		public static void InjectUnsigned(this sbyte value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007FEE RID: 32750 RVA: 0x0029B7A7 File Offset: 0x002999A7
		public static void Inject(this ulong value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06007FEF RID: 32751 RVA: 0x0029B7B5 File Offset: 0x002999B5
		public static void Inject(this ulong value, ref uint buffer, int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06007FF0 RID: 32752 RVA: 0x0029B7C4 File Offset: 0x002999C4
		public static void Inject(this uint value, ref uint buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FF1 RID: 32753 RVA: 0x0029B7D3 File Offset: 0x002999D3
		public static void Inject(this uint value, ref uint buffer, int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FF2 RID: 32754 RVA: 0x0029B7C4 File Offset: 0x002999C4
		public static void Inject(this ushort value, ref uint buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FF3 RID: 32755 RVA: 0x0029B7D3 File Offset: 0x002999D3
		public static void Inject(this ushort value, ref uint buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FF4 RID: 32756 RVA: 0x0029B7C4 File Offset: 0x002999C4
		public static void Inject(this byte value, ref uint buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FF5 RID: 32757 RVA: 0x0029B7D3 File Offset: 0x002999D3
		public static void Inject(this byte value, ref uint buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FF6 RID: 32758 RVA: 0x0029B7A7 File Offset: 0x002999A7
		public static void InjectUnsigned(this long value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FF7 RID: 32759 RVA: 0x0029B7E3 File Offset: 0x002999E3
		public static void InjectUnsigned(this int value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007FF8 RID: 32760 RVA: 0x0029B7E3 File Offset: 0x002999E3
		public static void InjectUnsigned(this short value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007FF9 RID: 32761 RVA: 0x0029B7E3 File Offset: 0x002999E3
		public static void InjectUnsigned(this sbyte value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007FFA RID: 32762 RVA: 0x0029B7F2 File Offset: 0x002999F2
		public static void Inject(this ulong value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06007FFB RID: 32763 RVA: 0x0029B800 File Offset: 0x00299A00
		public static void Inject(this ulong value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06007FFC RID: 32764 RVA: 0x0029B80F File Offset: 0x00299A0F
		public static void Inject(this uint value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FFD RID: 32765 RVA: 0x0029B81E File Offset: 0x00299A1E
		public static void Inject(this uint value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FFE RID: 32766 RVA: 0x0029B80F File Offset: 0x00299A0F
		public static void Inject(this ushort value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007FFF RID: 32767 RVA: 0x0029B81E File Offset: 0x00299A1E
		public static void Inject(this ushort value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06008000 RID: 32768 RVA: 0x0029B80F File Offset: 0x00299A0F
		public static void Inject(this byte value, ref ushort buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06008001 RID: 32769 RVA: 0x0029B81E File Offset: 0x00299A1E
		public static void Inject(this byte value, ref ushort buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06008002 RID: 32770 RVA: 0x0029B82E File Offset: 0x00299A2E
		public static void Inject(this ulong value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06008003 RID: 32771 RVA: 0x0029B83C File Offset: 0x00299A3C
		public static void Inject(this ulong value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06008004 RID: 32772 RVA: 0x0029B84B File Offset: 0x00299A4B
		public static void Inject(this uint value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06008005 RID: 32773 RVA: 0x0029B85A File Offset: 0x00299A5A
		public static void Inject(this uint value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06008006 RID: 32774 RVA: 0x0029B84B File Offset: 0x00299A4B
		public static void Inject(this ushort value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06008007 RID: 32775 RVA: 0x0029B85A File Offset: 0x00299A5A
		public static void Inject(this ushort value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06008008 RID: 32776 RVA: 0x0029B84B File Offset: 0x00299A4B
		public static void Inject(this byte value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06008009 RID: 32777 RVA: 0x0029B85A File Offset: 0x00299A5A
		public static void Inject(this byte value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600800A RID: 32778 RVA: 0x0029B86A File Offset: 0x00299A6A
		[Obsolete("Argument order changed")]
		public static ulong Extract(this ulong value, int bits, ref int bitposition)
		{
			return value.Extract(bits, ref bitposition);
		}

		// Token: 0x0600800B RID: 32779 RVA: 0x0029B874 File Offset: 0x00299A74
		public static ulong Read(this ulong value, ref int bitposition, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			ulong num2 = (value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x0600800C RID: 32780 RVA: 0x0029B89C File Offset: 0x00299A9C
		[Obsolete("Use Read instead.")]
		public static ulong Extract(this ulong value, ref int bitposition, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			ulong num2 = (value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x0600800D RID: 32781 RVA: 0x0029B8C4 File Offset: 0x00299AC4
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static ulong Extract(this ulong value, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			return value & num;
		}

		// Token: 0x0600800E RID: 32782 RVA: 0x0029B8E0 File Offset: 0x00299AE0
		public static uint Read(this uint value, ref int bitposition, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			uint num2 = (value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x0600800F RID: 32783 RVA: 0x0029B908 File Offset: 0x00299B08
		[Obsolete("Use Read instead.")]
		public static uint Extract(this uint value, ref int bitposition, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			uint num2 = (value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x06008010 RID: 32784 RVA: 0x0029B930 File Offset: 0x00299B30
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static uint Extract(this uint value, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			return value & num;
		}

		// Token: 0x06008011 RID: 32785 RVA: 0x0029B94C File Offset: 0x00299B4C
		public static uint Read(this ushort value, ref int bitposition, int bits)
		{
			uint num = 65535U >> 16 - bits;
			uint num2 = ((uint)value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x06008012 RID: 32786 RVA: 0x0029B978 File Offset: 0x00299B78
		[Obsolete("Use Read instead.")]
		public static uint Extract(this ushort value, ref int bitposition, int bits)
		{
			uint num = 65535U >> 16 - bits;
			uint num2 = ((uint)value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x06008013 RID: 32787 RVA: 0x0029B9A4 File Offset: 0x00299BA4
		public static uint Read(this byte value, ref int bitposition, int bits)
		{
			uint num = 255U >> 8 - bits;
			uint num2 = ((uint)value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x06008014 RID: 32788 RVA: 0x0029B9D0 File Offset: 0x00299BD0
		[Obsolete("Use Read instead.")]
		public static uint Extract(this byte value, ref int bitposition, int bits)
		{
			uint num = 255U >> 8 - bits;
			uint num2 = ((uint)value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x06008015 RID: 32789 RVA: 0x0029B9FC File Offset: 0x00299BFC
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static byte Extract(this byte value, int bits)
		{
			uint num = 255U >> 8 - bits;
			return (byte)((uint)value & num);
		}

		// Token: 0x06008016 RID: 32790 RVA: 0x0029BA1A File Offset: 0x00299C1A
		public static void Inject(this float f, ref ulong buffer, ref int bitposition)
		{
			buffer = buffer.Write(f, ref bitposition, 32);
		}

		// Token: 0x06008017 RID: 32791 RVA: 0x0029BA33 File Offset: 0x00299C33
		public static float ReadFloat(this ulong buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 32);
		}

		// Token: 0x06008018 RID: 32792 RVA: 0x0029BA48 File Offset: 0x00299C48
		[Obsolete("Use Read instead.")]
		public static float ExtractFloat(this ulong buffer, ref int bitposition)
		{
			return buffer.Extract(ref bitposition, 32);
		}

		// Token: 0x06008019 RID: 32793 RVA: 0x0029BA60 File Offset: 0x00299C60
		public static ushort InjectAsHalfFloat(this float f, ref ulong buffer, ref int bitposition)
		{
			ushort num = HalfUtilities.Pack(f);
			buffer = buffer.Write((ulong)num, ref bitposition, 16);
			return num;
		}

		// Token: 0x0600801A RID: 32794 RVA: 0x0029BA84 File Offset: 0x00299C84
		public static ushort InjectAsHalfFloat(this float f, ref uint buffer, ref int bitposition)
		{
			ushort num = HalfUtilities.Pack(f);
			buffer = buffer.Write((ulong)num, ref bitposition, 16);
			return num;
		}

		// Token: 0x0600801B RID: 32795 RVA: 0x0029BAA7 File Offset: 0x00299CA7
		public static float ReadHalfFloat(this ulong buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
		}

		// Token: 0x0600801C RID: 32796 RVA: 0x0029BAB8 File Offset: 0x00299CB8
		[Obsolete("Use Read instead.")]
		public static float ExtractHalfFloat(this ulong buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Extract(ref bitposition, 16));
		}

		// Token: 0x0600801D RID: 32797 RVA: 0x0029BAC9 File Offset: 0x00299CC9
		public static float ReadHalfFloat(this uint buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
		}

		// Token: 0x0600801E RID: 32798 RVA: 0x0029BADA File Offset: 0x00299CDA
		[Obsolete("Use Read instead.")]
		public static float ExtractHalfFloat(this uint buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Extract(ref bitposition, 16));
		}

		// Token: 0x0600801F RID: 32799 RVA: 0x0029BAEB File Offset: 0x00299CEB
		[Obsolete("Argument order changed")]
		public static void Inject(this ulong value, ref uint buffer, int bits, ref int bitposition)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06008020 RID: 32800 RVA: 0x0029BAF6 File Offset: 0x00299CF6
		[Obsolete("Argument order changed")]
		public static void Inject(this ulong value, ref ulong buffer, int bits, ref int bitposition)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0400915D RID: 37213
		private const string overrunerror = "Write buffer overrun. writepos + bits exceeds target length. Data loss will occur.";
	}
}
