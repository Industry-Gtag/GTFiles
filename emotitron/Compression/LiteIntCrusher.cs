using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x020013DF RID: 5087
	[Serializable]
	public class LiteIntCrusher : LiteCrusher<int>
	{
		// Token: 0x06008032 RID: 32818 RVA: 0x0029BEEC File Offset: 0x0029A0EC
		public LiteIntCrusher()
		{
			this.compressType = LiteIntCompressType.PackSigned;
			this.min = -128;
			this.max = 127;
			if (this.compressType == LiteIntCompressType.Range)
			{
				LiteIntCrusher.Recalculate(this.min, this.max, ref this.smallest, ref this.biggest, ref this.bits);
			}
		}

		// Token: 0x06008033 RID: 32819 RVA: 0x0029BF42 File Offset: 0x0029A142
		public LiteIntCrusher(LiteIntCompressType comType = LiteIntCompressType.PackSigned, int min = -128, int max = 127)
		{
			this.compressType = comType;
			this.min = min;
			this.max = max;
			if (this.compressType == LiteIntCompressType.Range)
			{
				LiteIntCrusher.Recalculate(min, max, ref this.smallest, ref this.biggest, ref this.bits);
			}
		}

		// Token: 0x06008034 RID: 32820 RVA: 0x0029BF84 File Offset: 0x0029A184
		public override ulong WriteValue(int val, byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
			{
				uint num = (uint)((val << 1) ^ (val >> 31));
				buffer.WritePackedBytes((ulong)num, ref bitposition, 32);
				return (ulong)num;
			}
			case LiteIntCompressType.PackUnsigned:
				buffer.WritePackedBytes((ulong)val, ref bitposition, 32);
				return (ulong)val;
			case LiteIntCompressType.Range:
			{
				ulong num2 = this.Encode(val);
				buffer.Write(num2, ref bitposition, this.bits);
				return num2;
			}
			default:
				return 0UL;
			}
		}

		// Token: 0x06008035 RID: 32821 RVA: 0x0029BFF0 File Offset: 0x0029A1F0
		public override void WriteCValue(uint cval, byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				buffer.WritePackedBytes((ulong)cval, ref bitposition, 32);
				return;
			case LiteIntCompressType.PackUnsigned:
				buffer.WritePackedBytes((ulong)cval, ref bitposition, 32);
				return;
			case LiteIntCompressType.Range:
				buffer.Write((ulong)cval, ref bitposition, this.bits);
				return;
			default:
				return;
			}
		}

		// Token: 0x06008036 RID: 32822 RVA: 0x0029C040 File Offset: 0x0029A240
		public override int ReadValue(byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				return buffer.ReadSignedPackedBytes(ref bitposition, 32);
			case LiteIntCompressType.PackUnsigned:
				return (int)buffer.ReadPackedBytes(ref bitposition, 32);
			case LiteIntCompressType.Range:
			{
				uint num = (uint)buffer.Read(ref bitposition, this.bits);
				return this.Decode(num);
			}
			default:
				return 0;
			}
		}

		// Token: 0x06008037 RID: 32823 RVA: 0x0029C095 File Offset: 0x0029A295
		public override ulong Encode(int value)
		{
			value = ((value > this.biggest) ? this.biggest : ((value < this.smallest) ? this.smallest : value));
			return (ulong)((long)(value - this.smallest));
		}

		// Token: 0x06008038 RID: 32824 RVA: 0x0029C0C5 File Offset: 0x0029A2C5
		public override int Decode(uint cvalue)
		{
			return (int)((ulong)cvalue + (ulong)((long)this.smallest));
		}

		// Token: 0x06008039 RID: 32825 RVA: 0x0029C0D4 File Offset: 0x0029A2D4
		public static void Recalculate(int min, int max, ref int smallest, ref int biggest, ref int bits)
		{
			if (min < max)
			{
				smallest = min;
				biggest = max;
			}
			else
			{
				smallest = max;
				biggest = min;
			}
			int num = biggest - smallest;
			bits = LiteCrusher.GetBitsForMaxValue((uint)num);
		}

		// Token: 0x0600803A RID: 32826 RVA: 0x0029C104 File Offset: 0x0029A304
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				base.GetType().Name,
				" ",
				this.compressType.ToString(),
				" mn: ",
				this.min.ToString(),
				" mx: ",
				this.max.ToString(),
				" sm: ",
				this.smallest.ToString()
			});
		}

		// Token: 0x04009178 RID: 37240
		[SerializeField]
		public LiteIntCompressType compressType;

		// Token: 0x04009179 RID: 37241
		[SerializeField]
		protected int min;

		// Token: 0x0400917A RID: 37242
		[SerializeField]
		protected int max;

		// Token: 0x0400917B RID: 37243
		[SerializeField]
		private int smallest;

		// Token: 0x0400917C RID: 37244
		[SerializeField]
		private int biggest;
	}
}
