using System;

namespace emotitron.Compression
{
	// Token: 0x020013D3 RID: 5075
	public static class ArraySerializeUnsafe
	{
		// Token: 0x06007F66 RID: 32614 RVA: 0x0029A4CC File Offset: 0x002986CC
		public unsafe static void WriteSigned(ulong* buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(buffer, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F67 RID: 32615 RVA: 0x0029A4EC File Offset: 0x002986EC
		public unsafe static void AppendSigned(ulong* buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Append(buffer, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F68 RID: 32616 RVA: 0x0029A50C File Offset: 0x0029870C
		public unsafe static void AddSigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F69 RID: 32617 RVA: 0x0029A52C File Offset: 0x0029872C
		public unsafe static void AddSigned(this short value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F6A RID: 32618 RVA: 0x0029A54C File Offset: 0x0029874C
		public unsafe static void AddSigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F6B RID: 32619 RVA: 0x0029A56C File Offset: 0x0029876C
		public unsafe static void InjectSigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F6C RID: 32620 RVA: 0x0029A58C File Offset: 0x0029878C
		public unsafe static void InjectSigned(this short value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F6D RID: 32621 RVA: 0x0029A5AC File Offset: 0x002987AC
		public unsafe static void InjectSigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F6E RID: 32622 RVA: 0x0029A5CC File Offset: 0x002987CC
		public unsafe static void PokeSigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F6F RID: 32623 RVA: 0x0029A5F0 File Offset: 0x002987F0
		public unsafe static void PokeSigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F70 RID: 32624 RVA: 0x0029A614 File Offset: 0x00298814
		public unsafe static void PokeSigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007F71 RID: 32625 RVA: 0x0029A638 File Offset: 0x00298838
		public unsafe static int ReadSigned(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007F72 RID: 32626 RVA: 0x0029A65C File Offset: 0x0029885C
		public unsafe static int PeekSigned(ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007F73 RID: 32627 RVA: 0x0029A680 File Offset: 0x00298880
		public unsafe static void Append(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = (uPtr[num2] & num3) | (value << num);
			uPtr[num2] = num4;
			uPtr[num2 + 1] = num4 >> 64 - num;
			bitposition += bits;
		}

		// Token: 0x06007F74 RID: 32628 RVA: 0x0029A6D8 File Offset: 0x002988D8
		public unsafe static void Write(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			uPtr[num2] = (uPtr[num2] & ~num4) | (num5 & num4);
			num = 64 - num;
			if (num < bits)
			{
				num4 = num3 >> num;
				num5 = value >> num;
				num2++;
				uPtr[num2] = (uPtr[num2] & ~num4) | (num5 & num4);
			}
			bitposition += bits;
		}

		// Token: 0x06007F75 RID: 32629 RVA: 0x0029A75C File Offset: 0x0029895C
		public unsafe static ulong Read(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = uPtr[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= uPtr[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06007F76 RID: 32630 RVA: 0x0029A7C0 File Offset: 0x002989C0
		public unsafe static ulong Read(ulong* uPtr, int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = uPtr[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= uPtr[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06007F77 RID: 32631 RVA: 0x0029A81F File Offset: 0x00298A1F
		public unsafe static void Add(this ulong value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06007F78 RID: 32632 RVA: 0x0029A82B File Offset: 0x00298A2B
		public unsafe static void Add(this uint value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F79 RID: 32633 RVA: 0x0029A82B File Offset: 0x00298A2B
		public unsafe static void Add(this ushort value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F7A RID: 32634 RVA: 0x0029A82B File Offset: 0x00298A2B
		public unsafe static void Add(this byte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F7B RID: 32635 RVA: 0x0029A81F File Offset: 0x00298A1F
		public unsafe static void AddUnsigned(this long value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F7C RID: 32636 RVA: 0x0029A838 File Offset: 0x00298A38
		public unsafe static void AddUnsigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007F7D RID: 32637 RVA: 0x0029A838 File Offset: 0x00298A38
		public unsafe static void AddUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007F7E RID: 32638 RVA: 0x0029A838 File Offset: 0x00298A38
		public unsafe static void AddUnsigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007F7F RID: 32639 RVA: 0x0029A845 File Offset: 0x00298A45
		public unsafe static void Inject(this ulong value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06007F80 RID: 32640 RVA: 0x0029A850 File Offset: 0x00298A50
		public unsafe static void Inject(this uint value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F81 RID: 32641 RVA: 0x0029A850 File Offset: 0x00298A50
		public unsafe static void Inject(this ushort value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F82 RID: 32642 RVA: 0x0029A850 File Offset: 0x00298A50
		public unsafe static void Inject(this byte value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F83 RID: 32643 RVA: 0x0029A845 File Offset: 0x00298A45
		public unsafe static void InjectUnsigned(this long value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F84 RID: 32644 RVA: 0x0029A85C File Offset: 0x00298A5C
		public unsafe static void InjectUnsigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007F85 RID: 32645 RVA: 0x0029A868 File Offset: 0x00298A68
		public unsafe static void InjectUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007F86 RID: 32646 RVA: 0x0029A85C File Offset: 0x00298A5C
		public unsafe static void InjectUnsigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007F87 RID: 32647 RVA: 0x0029A875 File Offset: 0x00298A75
		public unsafe static void Poke(this ulong value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06007F88 RID: 32648 RVA: 0x0029A881 File Offset: 0x00298A81
		public unsafe static void Poke(this uint value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F89 RID: 32649 RVA: 0x0029A881 File Offset: 0x00298A81
		public unsafe static void Poke(this ushort value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F8A RID: 32650 RVA: 0x0029A881 File Offset: 0x00298A81
		public unsafe static void Poke(this byte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F8B RID: 32651 RVA: 0x0029A875 File Offset: 0x00298A75
		public unsafe static void InjectUnsigned(this long value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06007F8C RID: 32652 RVA: 0x0029A868 File Offset: 0x00298A68
		public unsafe static void InjectUnsigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007F8D RID: 32653 RVA: 0x0029A868 File Offset: 0x00298A68
		public unsafe static void PokeUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007F8E RID: 32654 RVA: 0x0029A868 File Offset: 0x00298A68
		public unsafe static void PokeUnsigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06007F8F RID: 32655 RVA: 0x0029A890 File Offset: 0x00298A90
		public unsafe static void ReadOutUnsafe(ulong* sourcePtr, int sourcePos, ulong* targetPtr, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong num3 = ArraySerializeUnsafe.Read(sourcePtr, ref num, num2);
				ArraySerializeUnsafe.Write(targetPtr, num3, ref targetPos, num2);
			}
			targetPos += bits;
		}

		// Token: 0x06007F90 RID: 32656 RVA: 0x0029A8D8 File Offset: 0x00298AD8
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr, ref num, num2);
						ArraySerializeUnsafe.Write(ptr3, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06007F91 RID: 32657 RVA: 0x0029A964 File Offset: 0x00298B64
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr, ref num, num2);
						ArraySerializeUnsafe.Write(ptr3, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06007F92 RID: 32658 RVA: 0x0029A9F0 File Offset: 0x00298BF0
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr, ref num, num2);
						ArraySerializeUnsafe.Write(ptr2, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06007F93 RID: 32659 RVA: 0x0029AA78 File Offset: 0x00298C78
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					ulong* ptr4 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr4, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06007F94 RID: 32660 RVA: 0x0029AB08 File Offset: 0x00298D08
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					ulong* ptr4 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr4, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06007F95 RID: 32661 RVA: 0x0029AB98 File Offset: 0x00298D98
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr2, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06007F96 RID: 32662 RVA: 0x0029AC24 File Offset: 0x00298E24
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr2, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06007F97 RID: 32663 RVA: 0x0029ACB0 File Offset: 0x00298EB0
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					ulong* ptr4 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr4, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06007F98 RID: 32664 RVA: 0x0029AD40 File Offset: 0x00298F40
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					ulong* ptr4 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr4, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x04009150 RID: 37200
		private const string bufferOverrunMsg = "Byte buffer overrun. Dataloss will occur.";
	}
}
