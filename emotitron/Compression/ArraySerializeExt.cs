using System;

namespace emotitron.Compression
{
	// Token: 0x020013D2 RID: 5074
	public static class ArraySerializeExt
	{
		// Token: 0x06007F23 RID: 32547 RVA: 0x00299A1C File Offset: 0x00297C1C
		public static void Zero(this byte[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06007F24 RID: 32548 RVA: 0x00299A3C File Offset: 0x00297C3C
		public static void Zero(this byte[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06007F25 RID: 32549 RVA: 0x00299A60 File Offset: 0x00297C60
		public static void Zero(this byte[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06007F26 RID: 32550 RVA: 0x00299A84 File Offset: 0x00297C84
		public static void Zero(this ushort[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06007F27 RID: 32551 RVA: 0x00299AA4 File Offset: 0x00297CA4
		public static void Zero(this ushort[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06007F28 RID: 32552 RVA: 0x00299AC8 File Offset: 0x00297CC8
		public static void Zero(this ushort[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06007F29 RID: 32553 RVA: 0x00299AEC File Offset: 0x00297CEC
		public static void Zero(this uint[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x06007F2A RID: 32554 RVA: 0x00299B0C File Offset: 0x00297D0C
		public static void Zero(this uint[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x06007F2B RID: 32555 RVA: 0x00299B30 File Offset: 0x00297D30
		public static void Zero(this uint[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x06007F2C RID: 32556 RVA: 0x00299B54 File Offset: 0x00297D54
		public static void Zero(this ulong[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x06007F2D RID: 32557 RVA: 0x00299B74 File Offset: 0x00297D74
		public static void Zero(this ulong[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x06007F2E RID: 32558 RVA: 0x00299B98 File Offset: 0x00297D98
		public static void Zero(this ulong[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x06007F2F RID: 32559 RVA: 0x00299BBC File Offset: 0x00297DBC
		public static void WriteSigned(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F30 RID: 32560 RVA: 0x00299BDC File Offset: 0x00297DDC
		public static void WriteSigned(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F31 RID: 32561 RVA: 0x00299BFC File Offset: 0x00297DFC
		public static void WriteSigned(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F32 RID: 32562 RVA: 0x00299C1C File Offset: 0x00297E1C
		public static void WriteSigned(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.Write(num, ref bitposition, bits);
		}

		// Token: 0x06007F33 RID: 32563 RVA: 0x00299C3C File Offset: 0x00297E3C
		public static void WriteSigned(this uint[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.Write(num, ref bitposition, bits);
		}

		// Token: 0x06007F34 RID: 32564 RVA: 0x00299C5C File Offset: 0x00297E5C
		public static void WriteSigned(this ulong[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.Write(num, ref bitposition, bits);
		}

		// Token: 0x06007F35 RID: 32565 RVA: 0x00299C7C File Offset: 0x00297E7C
		public static int ReadSigned(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007F36 RID: 32566 RVA: 0x00299CA0 File Offset: 0x00297EA0
		public static int ReadSigned(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007F37 RID: 32567 RVA: 0x00299CC4 File Offset: 0x00297EC4
		public static int ReadSigned(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007F38 RID: 32568 RVA: 0x00299CE8 File Offset: 0x00297EE8
		public static long ReadSigned64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}

		// Token: 0x06007F39 RID: 32569 RVA: 0x00299D08 File Offset: 0x00297F08
		public static long ReadSigned64(this uint[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}

		// Token: 0x06007F3A RID: 32570 RVA: 0x00299D28 File Offset: 0x00297F28
		public static long ReadSigned64(this ulong[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}

		// Token: 0x06007F3B RID: 32571 RVA: 0x00299D47 File Offset: 0x00297F47
		public static void WriteFloat(this byte[] buffer, float value, ref int bitposition)
		{
			buffer.Write((ulong)value.uint32, ref bitposition, 32);
		}

		// Token: 0x06007F3C RID: 32572 RVA: 0x00299D5E File Offset: 0x00297F5E
		public static float ReadFloat(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 32);
		}

		// Token: 0x06007F3D RID: 32573 RVA: 0x00299D74 File Offset: 0x00297F74
		public static void Append(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int i = bitposition & 7;
			int num = bitposition >> 3;
			ulong num2 = (1UL << i) - 1UL;
			ulong num3 = ((ulong)buffer[num] & num2) | (value << i);
			buffer[num] = (byte)num3;
			for (i = 8 - i; i < bits; i += 8)
			{
				num++;
				buffer[num] = (byte)(value >> i);
			}
			bitposition += bits;
		}

		// Token: 0x06007F3E RID: 32574 RVA: 0x00299DD0 File Offset: 0x00297FD0
		public static void Append(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int i = bitposition & 31;
			int num = bitposition >> 5;
			ulong num2 = (1UL << i) - 1UL;
			ulong num3 = ((ulong)buffer[num] & num2) | (value << i);
			buffer[num] = (uint)num3;
			for (i = 32 - i; i < bits; i += 32)
			{
				num++;
				buffer[num] = (uint)(value >> i);
			}
			bitposition += bits;
		}

		// Token: 0x06007F3F RID: 32575 RVA: 0x00299E30 File Offset: 0x00298030
		public static void Append(this uint[] buffer, uint value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 31;
			int num2 = bitposition >> 5;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = ((ulong)buffer[num2] & num3) | ((ulong)value << num);
			buffer[num2] = (uint)num4;
			buffer[num2 + 1] = (uint)(num4 >> 32);
			bitposition += bits;
		}

		// Token: 0x06007F40 RID: 32576 RVA: 0x00299E7C File Offset: 0x0029807C
		public static void Append(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = (buffer[num2] & num3) | (value << num);
			buffer[num2] = num4;
			buffer[num2 + 1] = value >> 64 - num;
			bitposition += bits;
		}

		// Token: 0x06007F41 RID: 32577 RVA: 0x00299EC8 File Offset: 0x002980C8
		public static void Write(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 7;
			int num2 = bitposition >> 3;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = (byte)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			num = 8 - num;
			for (i -= 8; i > 8; i -= 8)
			{
				num2++;
				num5 = value >> num;
				buffer[num2] = (byte)num5;
				num += 8;
			}
			if (i > 0)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = (byte)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			}
			bitposition += bits;
		}

		// Token: 0x06007F42 RID: 32578 RVA: 0x00299F6C File Offset: 0x0029816C
		public static void Write(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 31;
			int num2 = bitposition >> 5;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = (uint)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			num = 32 - num;
			for (i -= 32; i > 32; i -= 32)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = (uint)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
				num += 32;
			}
			bitposition += bits;
		}

		// Token: 0x06007F43 RID: 32579 RVA: 0x0029A000 File Offset: 0x00298200
		public static void Write(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = (buffer[num2] & ~num4) | (num5 & num4);
			num = 64 - num;
			for (i -= 64; i > 64; i -= 64)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = (buffer[num2] & ~num4) | (num5 & num4);
				num += 64;
			}
			bitposition += bits;
		}

		// Token: 0x06007F44 RID: 32580 RVA: 0x0029A090 File Offset: 0x00298290
		public static void WriteBool(this ulong[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06007F45 RID: 32581 RVA: 0x0029A0A2 File Offset: 0x002982A2
		public static void WriteBool(this uint[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06007F46 RID: 32582 RVA: 0x0029A0B4 File Offset: 0x002982B4
		public static void WriteBool(this byte[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06007F47 RID: 32583 RVA: 0x0029A0C8 File Offset: 0x002982C8
		public static ulong Read(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 7;
			int num = bitposition >> 3;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = (ulong)buffer[num] >> i;
			for (i = 8 - i; i < bits; i += 8)
			{
				num++;
				num3 |= (ulong)buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06007F48 RID: 32584 RVA: 0x0029A124 File Offset: 0x00298324
		public static ulong Read(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 31;
			int num = bitposition >> 5;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = (ulong)buffer[num] >> i;
			for (i = 32 - i; i < bits; i += 32)
			{
				num++;
				num3 |= (ulong)buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06007F49 RID: 32585 RVA: 0x0029A180 File Offset: 0x00298380
		public static ulong Read(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = buffer[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06007F4A RID: 32586 RVA: 0x0029A1DA File Offset: 0x002983DA
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this byte[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F4B RID: 32587 RVA: 0x0029A1E4 File Offset: 0x002983E4
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this uint[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F4C RID: 32588 RVA: 0x0029A1EE File Offset: 0x002983EE
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this ulong[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F4D RID: 32589 RVA: 0x0029A1F8 File Offset: 0x002983F8
		public static uint ReadUInt32(this byte[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F4E RID: 32590 RVA: 0x0029A203 File Offset: 0x00298403
		public static uint ReadUInt32(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F4F RID: 32591 RVA: 0x0029A20E File Offset: 0x0029840E
		public static uint ReadUInt32(this ulong[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F50 RID: 32592 RVA: 0x0029A219 File Offset: 0x00298419
		public static ushort ReadUInt16(this byte[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F51 RID: 32593 RVA: 0x0029A224 File Offset: 0x00298424
		public static ushort ReadUInt16(this uint[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F52 RID: 32594 RVA: 0x0029A22F File Offset: 0x0029842F
		public static ushort ReadUInt16(this ulong[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F53 RID: 32595 RVA: 0x0029A23A File Offset: 0x0029843A
		public static byte ReadByte(this byte[] buffer, ref int bitposition, int bits = 8)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F54 RID: 32596 RVA: 0x0029A245 File Offset: 0x00298445
		public static byte ReadByte(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F55 RID: 32597 RVA: 0x0029A250 File Offset: 0x00298450
		public static byte ReadByte(this ulong[] buffer, ref int bitposition, int bits)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007F56 RID: 32598 RVA: 0x0029A25B File Offset: 0x0029845B
		public static bool ReadBool(this ulong[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06007F57 RID: 32599 RVA: 0x0029A26C File Offset: 0x0029846C
		public static bool ReadBool(this uint[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06007F58 RID: 32600 RVA: 0x0029A27D File Offset: 0x0029847D
		public static bool ReadBool(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06007F59 RID: 32601 RVA: 0x0029A28E File Offset: 0x0029848E
		public static char ReadChar(this ulong[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06007F5A RID: 32602 RVA: 0x0029A29A File Offset: 0x0029849A
		public static char ReadChar(this uint[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06007F5B RID: 32603 RVA: 0x0029A2A6 File Offset: 0x002984A6
		public static char ReadChar(this byte[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06007F5C RID: 32604 RVA: 0x0029A2B4 File Offset: 0x002984B4
		public static void ReadOutSafe(this ulong[] source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong num3 = source.Read(ref num, num2);
				target.Write(num3, ref bitposition, num2);
			}
			bitposition += bits;
		}

		// Token: 0x06007F5D RID: 32605 RVA: 0x0029A2FC File Offset: 0x002984FC
		public static void ReadOutSafe(this ulong[] source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong num3 = source.Read(ref num, num2);
				target.Write(num3, ref bitposition, num2);
			}
		}

		// Token: 0x06007F5E RID: 32606 RVA: 0x0029A33C File Offset: 0x0029853C
		public static void ReadOutSafe(this byte[] source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 8) ? 8 : i);
				ulong num3 = source.Read(ref num, num2);
				target.Write(num3, ref bitposition, num2);
			}
		}

		// Token: 0x06007F5F RID: 32607 RVA: 0x0029A37C File Offset: 0x0029857C
		public static void ReadOutSafe(this byte[] source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 8) ? 8 : i);
				ulong num3 = source.Read(ref num, num2);
				target.Write(num3, ref bitposition, num2);
			}
		}

		// Token: 0x06007F60 RID: 32608 RVA: 0x0029A3BC File Offset: 0x002985BC
		public static ulong IndexAsUInt64(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (ulong)buffer[num] | ((ulong)buffer[num + 1] << 8) | ((ulong)buffer[num + 2] << 16) | ((ulong)buffer[num + 3] << 24) | ((ulong)buffer[num + 4] << 32) | ((ulong)buffer[num + 5] << 40) | ((ulong)buffer[num + 6] << 48) | ((ulong)buffer[num + 7] << 56);
		}

		// Token: 0x06007F61 RID: 32609 RVA: 0x0029A418 File Offset: 0x00298618
		public static ulong IndexAsUInt64(this uint[] buffer, int index)
		{
			int num = index << 1;
			return (ulong)buffer[num] | ((ulong)buffer[num + 1] << 32);
		}

		// Token: 0x06007F62 RID: 32610 RVA: 0x0029A438 File Offset: 0x00298638
		public static uint IndexAsUInt32(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (uint)((int)buffer[num] | ((int)buffer[num + 1] << 8) | ((int)buffer[num + 2] << 16) | ((int)buffer[num + 3] << 24));
		}

		// Token: 0x06007F63 RID: 32611 RVA: 0x0029A468 File Offset: 0x00298668
		public static uint IndexAsUInt32(this ulong[] buffer, int index)
		{
			int num = index >> 1;
			int num2 = (index & 1) << 5;
			return (uint)((byte)(buffer[num] >> num2));
		}

		// Token: 0x06007F64 RID: 32612 RVA: 0x0029A488 File Offset: 0x00298688
		public static byte IndexAsUInt8(this ulong[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 7) << 3;
			return (byte)(buffer[num] >> num2);
		}

		// Token: 0x06007F65 RID: 32613 RVA: 0x0029A4A8 File Offset: 0x002986A8
		public static byte IndexAsUInt8(this uint[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 3) << 3;
			return (byte)((ulong)buffer[num] >> num2);
		}

		// Token: 0x0400914F RID: 37199
		private const string bufferOverrunMsg = "Byte buffer length exceeded by write or read. Dataloss will occur. Likely due to a Read/Write mismatch.";
	}
}
