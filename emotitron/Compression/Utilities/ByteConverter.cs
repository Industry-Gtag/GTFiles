using System;
using System.Runtime.InteropServices;

namespace emotitron.Compression.Utilities
{
	// Token: 0x020013E1 RID: 5089
	[StructLayout(LayoutKind.Explicit)]
	public struct ByteConverter
	{
		// Token: 0x17000C58 RID: 3160
		public byte this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.byte0;
				case 1:
					return this.byte1;
				case 2:
					return this.byte2;
				case 3:
					return this.byte3;
				case 4:
					return this.byte4;
				case 5:
					return this.byte5;
				case 6:
					return this.byte6;
				case 7:
					return this.byte7;
				default:
					return 0;
				}
			}
		}

		// Token: 0x06008044 RID: 32836 RVA: 0x0029C254 File Offset: 0x0029A454
		public static implicit operator ByteConverter(byte[] bytes)
		{
			ByteConverter byteConverter = default(ByteConverter);
			int num = bytes.Length;
			byteConverter.byte0 = bytes[0];
			if (num > 0)
			{
				byteConverter.byte1 = bytes[1];
			}
			if (num > 1)
			{
				byteConverter.byte2 = bytes[2];
			}
			if (num > 2)
			{
				byteConverter.byte3 = bytes[3];
			}
			if (num > 3)
			{
				byteConverter.byte4 = bytes[4];
			}
			if (num > 4)
			{
				byteConverter.byte5 = bytes[5];
			}
			if (num > 5)
			{
				byteConverter.byte6 = bytes[3];
			}
			if (num > 6)
			{
				byteConverter.byte7 = bytes[7];
			}
			return byteConverter;
		}

		// Token: 0x06008045 RID: 32837 RVA: 0x0029C2D8 File Offset: 0x0029A4D8
		public static implicit operator ByteConverter(byte val)
		{
			return new ByteConverter
			{
				byte0 = val
			};
		}

		// Token: 0x06008046 RID: 32838 RVA: 0x0029C2F8 File Offset: 0x0029A4F8
		public static implicit operator ByteConverter(sbyte val)
		{
			return new ByteConverter
			{
				int8 = val
			};
		}

		// Token: 0x06008047 RID: 32839 RVA: 0x0029C318 File Offset: 0x0029A518
		public static implicit operator ByteConverter(char val)
		{
			return new ByteConverter
			{
				character = val
			};
		}

		// Token: 0x06008048 RID: 32840 RVA: 0x0029C338 File Offset: 0x0029A538
		public static implicit operator ByteConverter(uint val)
		{
			return new ByteConverter
			{
				uint32 = val
			};
		}

		// Token: 0x06008049 RID: 32841 RVA: 0x0029C358 File Offset: 0x0029A558
		public static implicit operator ByteConverter(int val)
		{
			return new ByteConverter
			{
				int32 = val
			};
		}

		// Token: 0x0600804A RID: 32842 RVA: 0x0029C378 File Offset: 0x0029A578
		public static implicit operator ByteConverter(ulong val)
		{
			return new ByteConverter
			{
				uint64 = val
			};
		}

		// Token: 0x0600804B RID: 32843 RVA: 0x0029C398 File Offset: 0x0029A598
		public static implicit operator ByteConverter(long val)
		{
			return new ByteConverter
			{
				int64 = val
			};
		}

		// Token: 0x0600804C RID: 32844 RVA: 0x0029C3B8 File Offset: 0x0029A5B8
		public static implicit operator ByteConverter(float val)
		{
			return new ByteConverter
			{
				float32 = val
			};
		}

		// Token: 0x0600804D RID: 32845 RVA: 0x0029C3D8 File Offset: 0x0029A5D8
		public static implicit operator ByteConverter(double val)
		{
			return new ByteConverter
			{
				float64 = val
			};
		}

		// Token: 0x0600804E RID: 32846 RVA: 0x0029C3F8 File Offset: 0x0029A5F8
		public static implicit operator ByteConverter(bool val)
		{
			return new ByteConverter
			{
				int32 = (val ? 1 : 0)
			};
		}

		// Token: 0x0600804F RID: 32847 RVA: 0x0029C41C File Offset: 0x0029A61C
		public void ExtractByteArray(byte[] targetArray)
		{
			int num = targetArray.Length;
			targetArray[0] = this.byte0;
			if (num > 0)
			{
				targetArray[1] = this.byte1;
			}
			if (num > 1)
			{
				targetArray[2] = this.byte2;
			}
			if (num > 2)
			{
				targetArray[3] = this.byte3;
			}
			if (num > 3)
			{
				targetArray[4] = this.byte4;
			}
			if (num > 4)
			{
				targetArray[5] = this.byte5;
			}
			if (num > 5)
			{
				targetArray[6] = this.byte6;
			}
			if (num > 6)
			{
				targetArray[7] = this.byte7;
			}
		}

		// Token: 0x06008050 RID: 32848 RVA: 0x0029C48F File Offset: 0x0029A68F
		public static implicit operator byte(ByteConverter bc)
		{
			return bc.byte0;
		}

		// Token: 0x06008051 RID: 32849 RVA: 0x0029C497 File Offset: 0x0029A697
		public static implicit operator sbyte(ByteConverter bc)
		{
			return bc.int8;
		}

		// Token: 0x06008052 RID: 32850 RVA: 0x0029C49F File Offset: 0x0029A69F
		public static implicit operator char(ByteConverter bc)
		{
			return bc.character;
		}

		// Token: 0x06008053 RID: 32851 RVA: 0x0029C4A7 File Offset: 0x0029A6A7
		public static implicit operator ushort(ByteConverter bc)
		{
			return bc.uint16;
		}

		// Token: 0x06008054 RID: 32852 RVA: 0x0029C4AF File Offset: 0x0029A6AF
		public static implicit operator short(ByteConverter bc)
		{
			return bc.int16;
		}

		// Token: 0x06008055 RID: 32853 RVA: 0x0029C4B7 File Offset: 0x0029A6B7
		public static implicit operator uint(ByteConverter bc)
		{
			return bc.uint32;
		}

		// Token: 0x06008056 RID: 32854 RVA: 0x0029C4BF File Offset: 0x0029A6BF
		public static implicit operator int(ByteConverter bc)
		{
			return bc.int32;
		}

		// Token: 0x06008057 RID: 32855 RVA: 0x0029C4C7 File Offset: 0x0029A6C7
		public static implicit operator ulong(ByteConverter bc)
		{
			return bc.uint64;
		}

		// Token: 0x06008058 RID: 32856 RVA: 0x0029C4CF File Offset: 0x0029A6CF
		public static implicit operator long(ByteConverter bc)
		{
			return bc.int64;
		}

		// Token: 0x06008059 RID: 32857 RVA: 0x0029C4D7 File Offset: 0x0029A6D7
		public static implicit operator float(ByteConverter bc)
		{
			return bc.float32;
		}

		// Token: 0x0600805A RID: 32858 RVA: 0x0029C4DF File Offset: 0x0029A6DF
		public static implicit operator double(ByteConverter bc)
		{
			return bc.float64;
		}

		// Token: 0x0600805B RID: 32859 RVA: 0x0029C4E7 File Offset: 0x0029A6E7
		public static implicit operator bool(ByteConverter bc)
		{
			return bc.int32 != 0;
		}

		// Token: 0x0400917D RID: 37245
		[FieldOffset(0)]
		public float float32;

		// Token: 0x0400917E RID: 37246
		[FieldOffset(0)]
		public double float64;

		// Token: 0x0400917F RID: 37247
		[FieldOffset(0)]
		public sbyte int8;

		// Token: 0x04009180 RID: 37248
		[FieldOffset(0)]
		public short int16;

		// Token: 0x04009181 RID: 37249
		[FieldOffset(0)]
		public ushort uint16;

		// Token: 0x04009182 RID: 37250
		[FieldOffset(0)]
		public char character;

		// Token: 0x04009183 RID: 37251
		[FieldOffset(0)]
		public int int32;

		// Token: 0x04009184 RID: 37252
		[FieldOffset(0)]
		public uint uint32;

		// Token: 0x04009185 RID: 37253
		[FieldOffset(0)]
		public long int64;

		// Token: 0x04009186 RID: 37254
		[FieldOffset(0)]
		public ulong uint64;

		// Token: 0x04009187 RID: 37255
		[FieldOffset(0)]
		public byte byte0;

		// Token: 0x04009188 RID: 37256
		[FieldOffset(1)]
		public byte byte1;

		// Token: 0x04009189 RID: 37257
		[FieldOffset(2)]
		public byte byte2;

		// Token: 0x0400918A RID: 37258
		[FieldOffset(3)]
		public byte byte3;

		// Token: 0x0400918B RID: 37259
		[FieldOffset(4)]
		public byte byte4;

		// Token: 0x0400918C RID: 37260
		[FieldOffset(5)]
		public byte byte5;

		// Token: 0x0400918D RID: 37261
		[FieldOffset(6)]
		public byte byte6;

		// Token: 0x0400918E RID: 37262
		[FieldOffset(7)]
		public byte byte7;

		// Token: 0x0400918F RID: 37263
		[FieldOffset(4)]
		public uint uint16_B;
	}
}
