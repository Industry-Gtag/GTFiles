using System;

namespace emotitron.Compression
{
	// Token: 0x020013D6 RID: 5078
	public static class BitCounter
	{
		// Token: 0x06007F99 RID: 32665 RVA: 0x0029ADD0 File Offset: 0x00298FD0
		public static int UsedBitCount(this ulong val)
		{
			val |= val >> 1;
			val |= val >> 2;
			val |= val >> 4;
			val |= val >> 8;
			val |= val >> 16;
			val |= val >> 32;
			checked
			{
				return BitCounter.bitPatternToLog2[(int)((IntPtr)(unchecked(val * 7783611145303519083UL) >> 57))];
			}
		}

		// Token: 0x06007F9A RID: 32666 RVA: 0x0029AE1E File Offset: 0x0029901E
		public static int UsedBitCount(this uint val)
		{
			val |= val >> 1;
			val |= val >> 2;
			val |= val >> 4;
			val |= val >> 8;
			val |= val >> 16;
			checked
			{
				return BitCounter.bitPatternToLog2[(int)((IntPtr)(unchecked((ulong)val * 7783611145303519083UL) >> 57))];
			}
		}

		// Token: 0x06007F9B RID: 32667 RVA: 0x0029AE5A File Offset: 0x0029905A
		public static int UsedBitCount(this int val)
		{
			val |= val >> 1;
			val |= val >> 2;
			val |= val >> 4;
			val |= val >> 8;
			val |= val >> 16;
			checked
			{
				return BitCounter.bitPatternToLog2[(int)((IntPtr)((ulong)(unchecked((long)val * 7783611145303519083L)) >> 57))];
			}
		}

		// Token: 0x06007F9C RID: 32668 RVA: 0x0029AE98 File Offset: 0x00299098
		public static int UsedBitCount(this ushort val)
		{
			uint num = (uint)val | ((uint)val >> 1);
			num |= num >> 2;
			num |= num >> 4;
			num |= num >> 8;
			checked
			{
				return BitCounter.bitPatternToLog2[(int)((IntPtr)(unchecked((ulong)num * 7783611145303519083UL) >> 57))];
			}
		}

		// Token: 0x06007F9D RID: 32669 RVA: 0x0029AED8 File Offset: 0x002990D8
		public static int UsedBitCount(this byte val)
		{
			uint num = (uint)val | ((uint)val >> 1);
			num |= num >> 2;
			num |= num >> 4;
			checked
			{
				return BitCounter.bitPatternToLog2[(int)((IntPtr)(unchecked((ulong)num * 7783611145303519083UL) >> 57))];
			}
		}

		// Token: 0x06007F9E RID: 32670 RVA: 0x0029AF10 File Offset: 0x00299110
		public static int UsedByteCount(this ulong val)
		{
			if (val == 0UL)
			{
				return 0;
			}
			if ((val & 1095216660480UL) != 0UL)
			{
				if ((val & 71776119061217280UL) != 0UL)
				{
					if ((val & 18374686479671623680UL) != 0UL)
					{
						return 8;
					}
					return 7;
				}
				else
				{
					if ((val & 280375465082880UL) != 0UL)
					{
						return 6;
					}
					return 5;
				}
			}
			else if ((val & 16711680UL) != 0UL)
			{
				if ((val & (ulong)(-16777216)) != 0UL)
				{
					return 4;
				}
				return 3;
			}
			else
			{
				if ((val & 65280UL) != 0UL)
				{
					return 2;
				}
				return 1;
			}
		}

		// Token: 0x06007F9F RID: 32671 RVA: 0x0029AF83 File Offset: 0x00299183
		public static int UsedByteCount(this uint val)
		{
			if (val == 0U)
			{
				return 0;
			}
			if ((val & 16711680U) != 0U)
			{
				if ((val & 4278190080U) != 0U)
				{
					return 4;
				}
				return 3;
			}
			else
			{
				if ((val & 65280U) != 0U)
				{
					return 2;
				}
				return 1;
			}
		}

		// Token: 0x06007FA0 RID: 32672 RVA: 0x0029AFAC File Offset: 0x002991AC
		public static int UsedByteCount(this ushort val)
		{
			if (val == 0)
			{
				return 0;
			}
			if ((val & 65280) != 0)
			{
				return 2;
			}
			return 1;
		}

		// Token: 0x0400915B RID: 37211
		public static readonly int[] bitPatternToLog2 = new int[]
		{
			0, 48, -1, -1, 31, -1, 15, 51, -1, 63,
			5, -1, -1, -1, 19, -1, 23, 28, -1, -1,
			-1, 40, 36, 46, -1, 13, -1, -1, -1, 34,
			-1, 58, -1, 60, 2, 43, 55, -1, -1, -1,
			50, 62, 4, -1, 18, 27, -1, 39, 45, -1,
			-1, 33, 57, -1, 1, 54, -1, 49, -1, 17,
			-1, -1, 32, -1, 53, -1, 16, -1, -1, 52,
			-1, -1, -1, 64, 6, 7, 8, -1, 9, -1,
			-1, -1, 20, 10, -1, -1, 24, -1, 29, -1,
			-1, 21, -1, 11, -1, -1, 41, -1, 25, 37,
			-1, 47, -1, 30, 14, -1, -1, -1, -1, 22,
			-1, -1, 35, 12, -1, -1, -1, 59, 42, -1,
			-1, 61, 3, 26, 38, 44, -1, 56
		};

		// Token: 0x0400915C RID: 37212
		public const ulong MULTIPLICATOR = 7783611145303519083UL;
	}
}
