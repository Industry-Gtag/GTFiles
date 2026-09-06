using System;

namespace emotitron.Compression
{
	// Token: 0x020013D1 RID: 5073
	public static class ArraySegmentExt
	{
		// Token: 0x06007F12 RID: 32530 RVA: 0x00299747 File Offset: 0x00297947
		public static ArraySegment<byte> ExtractArraySegment(byte[] buffer, ref int bitposition)
		{
			return new ArraySegment<byte>(buffer, 0, bitposition + 7 >> 3);
		}

		// Token: 0x06007F13 RID: 32531 RVA: 0x00299756 File Offset: 0x00297956
		public static ArraySegment<ushort> ExtractArraySegment(ushort[] buffer, ref int bitposition)
		{
			return new ArraySegment<ushort>(buffer, 0, bitposition + 15 >> 4);
		}

		// Token: 0x06007F14 RID: 32532 RVA: 0x00299766 File Offset: 0x00297966
		public static ArraySegment<uint> ExtractArraySegment(uint[] buffer, ref int bitposition)
		{
			return new ArraySegment<uint>(buffer, 0, bitposition + 31 >> 5);
		}

		// Token: 0x06007F15 RID: 32533 RVA: 0x00299776 File Offset: 0x00297976
		public static ArraySegment<ulong> ExtractArraySegment(ulong[] buffer, ref int bitposition)
		{
			return new ArraySegment<ulong>(buffer, 0, bitposition + 63 >> 6);
		}

		// Token: 0x06007F16 RID: 32534 RVA: 0x00299788 File Offset: 0x00297988
		public static void Append(this ArraySegment<byte> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06007F17 RID: 32535 RVA: 0x002997BC File Offset: 0x002979BC
		public static void Append(this ArraySegment<uint> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06007F18 RID: 32536 RVA: 0x002997F0 File Offset: 0x002979F0
		public static void Append(this ArraySegment<ulong> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06007F19 RID: 32537 RVA: 0x00299824 File Offset: 0x00297A24
		public static void Write(this ArraySegment<byte> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06007F1A RID: 32538 RVA: 0x00299858 File Offset: 0x00297A58
		public static void Write(this ArraySegment<uint> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06007F1B RID: 32539 RVA: 0x0029988C File Offset: 0x00297A8C
		public static void Write(this ArraySegment<ulong> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06007F1C RID: 32540 RVA: 0x002998C0 File Offset: 0x00297AC0
		public static ulong Read(this ArraySegment<byte> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			ulong num2 = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return num2;
		}

		// Token: 0x06007F1D RID: 32541 RVA: 0x002998F4 File Offset: 0x00297AF4
		public static ulong Read(this ArraySegment<uint> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			ulong num2 = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return num2;
		}

		// Token: 0x06007F1E RID: 32542 RVA: 0x00299928 File Offset: 0x00297B28
		public static ulong Read(this ArraySegment<ulong> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			ulong num2 = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return num2;
		}

		// Token: 0x06007F1F RID: 32543 RVA: 0x0029995C File Offset: 0x00297B5C
		public static void ReadOutSafe(this ArraySegment<byte> source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 3;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x06007F20 RID: 32544 RVA: 0x0029998C File Offset: 0x00297B8C
		public static void ReadOutSafe(this ArraySegment<byte> source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 3;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x06007F21 RID: 32545 RVA: 0x002999BC File Offset: 0x00297BBC
		public static void ReadOutSafe(this ArraySegment<ulong> source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 6;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x06007F22 RID: 32546 RVA: 0x002999EC File Offset: 0x00297BEC
		public static void ReadOutSafe(this ArraySegment<ulong> source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 6;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}
	}
}
