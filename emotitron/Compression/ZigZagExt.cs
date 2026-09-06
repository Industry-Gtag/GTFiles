using System;

namespace emotitron.Compression
{
	// Token: 0x020013E0 RID: 5088
	public static class ZigZagExt
	{
		// Token: 0x0600803B RID: 32827 RVA: 0x0029C189 File Offset: 0x0029A389
		public static ulong ZigZag(this long s)
		{
			return (ulong)((s << 1) ^ (s >> 63));
		}

		// Token: 0x0600803C RID: 32828 RVA: 0x0029C193 File Offset: 0x0029A393
		public static long UnZigZag(this ulong u)
		{
			return (long)((u >> 1) ^ -(long)(u & 1UL));
		}

		// Token: 0x0600803D RID: 32829 RVA: 0x0029C19E File Offset: 0x0029A39E
		public static uint ZigZag(this int s)
		{
			return (uint)((s << 1) ^ (s >> 31));
		}

		// Token: 0x0600803E RID: 32830 RVA: 0x0029C1A8 File Offset: 0x0029A3A8
		public static int UnZigZag(this uint u)
		{
			return (int)((ulong)(u >> 1) ^ (ulong)((long)(-(long)(u & 1U))));
		}

		// Token: 0x0600803F RID: 32831 RVA: 0x0029C1B5 File Offset: 0x0029A3B5
		public static ushort ZigZag(this short s)
		{
			return (ushort)(((int)s << 1) ^ (s >> 15));
		}

		// Token: 0x06008040 RID: 32832 RVA: 0x0029C1C0 File Offset: 0x0029A3C0
		public static short UnZigZag(this ushort u)
		{
			return (short)((u >> 1) ^ (int)(-(int)((short)(u & 1))));
		}

		// Token: 0x06008041 RID: 32833 RVA: 0x0029C1CC File Offset: 0x0029A3CC
		public static byte ZigZag(this sbyte s)
		{
			return (byte)(((int)s << 1) ^ (s >> 7));
		}

		// Token: 0x06008042 RID: 32834 RVA: 0x0029C1D6 File Offset: 0x0029A3D6
		public static sbyte UnZigZag(this byte u)
		{
			return (sbyte)((u >> 1) ^ (int)(-(int)((sbyte)(u & 1))));
		}
	}
}
