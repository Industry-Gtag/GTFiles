using System;
using emotitron.Compression.HalfFloat;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x020013DD RID: 5085
	[Serializable]
	public class LiteFloatCrusher : LiteCrusher<float>
	{
		// Token: 0x06008029 RID: 32809 RVA: 0x0029BB34 File Offset: 0x00299D34
		public LiteFloatCrusher()
		{
			this.compressType = LiteFloatCompressType.Half16;
			this.min = 0f;
			this.max = 1f;
			this.accurateCenter = true;
			LiteFloatCrusher.Recalculate(this.compressType, this.min, this.max, this.accurateCenter, ref this.bits, ref this.encoder, ref this.decoder, ref this.maxCVal);
		}

		// Token: 0x0600802A RID: 32810 RVA: 0x0029BBB0 File Offset: 0x00299DB0
		public LiteFloatCrusher(LiteFloatCompressType compressType, float min, float max, bool accurateCenter)
		{
			this.compressType = compressType;
			this.min = min;
			this.max = max;
			this.accurateCenter = accurateCenter;
			LiteFloatCrusher.Recalculate(compressType, min, max, accurateCenter, ref this.bits, ref this.encoder, ref this.decoder, ref this.maxCVal);
		}

		// Token: 0x0600802B RID: 32811 RVA: 0x0029BC14 File Offset: 0x00299E14
		public static void Recalculate(LiteFloatCompressType compressType, float min, float max, bool accurateCenter, ref int bits, ref float encoder, ref float decoder, ref ulong maxCVal)
		{
			bits = (int)compressType;
			float num = max - min;
			ulong num2 = ((bits == 64) ? ulong.MaxValue : ((1UL << bits) - 1UL));
			if (accurateCenter && num2 != 0UL)
			{
				num2 -= 1UL;
			}
			encoder = ((num == 0f) ? 0f : (num2 / num));
			decoder = ((num2 == 0UL) ? 0f : (num / num2));
			maxCVal = num2;
		}

		// Token: 0x0600802C RID: 32812 RVA: 0x0029BC7C File Offset: 0x00299E7C
		public override ulong Encode(float val)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return (ulong)HalfUtilities.Pack(val);
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return (ulong)val.uint32;
			}
			float num = (val - this.min) * this.encoder + 0.5f;
			if (num < 0f)
			{
				return 0UL;
			}
			ulong num2 = (ulong)num;
			if (num2 <= this.maxCVal)
			{
				return num2;
			}
			return this.maxCVal;
		}

		// Token: 0x0600802D RID: 32813 RVA: 0x0029BCE8 File Offset: 0x00299EE8
		public override float Decode(uint cval)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return HalfUtilities.Unpack((ushort)cval);
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return cval.float32;
			}
			if (cval == 0U)
			{
				return this.min;
			}
			if ((ulong)cval == this.maxCVal)
			{
				return this.max;
			}
			return cval * this.decoder + this.min;
		}

		// Token: 0x0600802E RID: 32814 RVA: 0x0029BD4C File Offset: 0x00299F4C
		public override ulong WriteValue(float val, byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				ulong num = (ulong)HalfUtilities.Pack(val);
				buffer.Write(num, ref bitposition, 16);
				return num;
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				ulong num2 = (ulong)val.uint32;
				buffer.Write(num2, ref bitposition, 32);
				return num2;
			}
			ulong num3 = this.Encode(val);
			buffer.Write(num3, ref bitposition, (int)this.compressType);
			return num3;
		}

		// Token: 0x0600802F RID: 32815 RVA: 0x0029BDB1 File Offset: 0x00299FB1
		public override void WriteCValue(uint cval, byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				buffer.Write((ulong)cval, ref bitposition, 16);
				return;
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				buffer.Write((ulong)cval, ref bitposition, 32);
				return;
			}
			buffer.Write((ulong)cval, ref bitposition, (int)this.compressType);
		}

		// Token: 0x06008030 RID: 32816 RVA: 0x0029BDF0 File Offset: 0x00299FF0
		public override float ReadValue(byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return ((uint)buffer.Read(ref bitposition, 32)).float32;
			}
			uint num = (uint)buffer.Read(ref bitposition, (int)this.compressType);
			return this.Decode(num);
		}

		// Token: 0x06008031 RID: 32817 RVA: 0x0029BE4C File Offset: 0x0029A04C
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
				" e: ",
				this.encoder.ToString(),
				" d: ",
				this.decoder.ToString()
			});
		}

		// Token: 0x0400916D RID: 37229
		[SerializeField]
		protected float min;

		// Token: 0x0400916E RID: 37230
		[SerializeField]
		protected float max;

		// Token: 0x0400916F RID: 37231
		[SerializeField]
		public LiteFloatCompressType compressType = LiteFloatCompressType.Half16;

		// Token: 0x04009170 RID: 37232
		[SerializeField]
		private bool accurateCenter = true;

		// Token: 0x04009171 RID: 37233
		[SerializeField]
		private float encoder;

		// Token: 0x04009172 RID: 37234
		[SerializeField]
		private float decoder;

		// Token: 0x04009173 RID: 37235
		[SerializeField]
		private ulong maxCVal;
	}
}
