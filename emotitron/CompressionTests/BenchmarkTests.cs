using System;
using System.Diagnostics;
using emotitron.Compression;
using UnityEngine;

namespace emotitron.CompressionTests
{
	// Token: 0x020013E5 RID: 5093
	public class BenchmarkTests : MonoBehaviour
	{
		// Token: 0x0600808A RID: 32906 RVA: 0x0029CD49 File Offset: 0x0029AF49
		private void Start()
		{
			BenchmarkTests.TestWriterIntegrity();
			BenchmarkTests.ArrayCopy();
			BenchmarkTests.ArrayCopySafe();
		}

		// Token: 0x0600808B RID: 32907 RVA: 0x0029CD5C File Offset: 0x0029AF5C
		public static void TestWriterIntegrity()
		{
			int num = 1;
			int num2 = 1;
			BenchmarkTests.ubuffer.Write(ulong.MaxValue, ref num, 64);
			if (BenchmarkTests.ubuffer.Read(ref num2, 64) != 18446744073709551615UL)
			{
				Debug.Log("Error writing with maxulong");
			}
			for (int i = 0; i < 3000; i++)
			{
				num = Random.Range(0, 200);
				num2 = num;
				int num3 = num;
				int num4 = Random.Range(1, 64);
				int num5 = Random.Range(-(1 << num4 - 1), (1 << num4 - 1) - 1);
				BenchmarkTests.ubuffer.WriteSigned(num5, ref num, num4);
				BenchmarkTests.ubuffer.WriteSigned(num5, ref num, num4);
				BenchmarkTests.ubuffer.WriteSigned(num5, ref num, num4);
				if (BenchmarkTests.ubuffer.ReadSigned(ref num2, num4) != num5)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num5.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
				if (BenchmarkTests.ubuffer.ReadSigned(ref num2, num4) != num5)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num5.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
				if (BenchmarkTests.ubuffer.ReadSigned(ref num2, num4) != num5)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num5.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
				ulong num6 = (ulong)Random.Range(0f, (1L << num4) - 1L);
				BenchmarkTests.ubuffer.Write(num6, ref num, num4);
				BenchmarkTests.ubuffer.Write(num6, ref num, num4);
				BenchmarkTests.ubuffer.Write(num6, ref num, num4);
				if (BenchmarkTests.ubuffer.Read(ref num2, num4) != num6)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num6.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
				if (BenchmarkTests.ubuffer.Read(ref num2, num4) != num6)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num6.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
				if (BenchmarkTests.ubuffer.Read(ref num2, num4) != num6)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num6.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
			}
			Debug.Log("Integrity check complete.");
		}

		// Token: 0x0600808C RID: 32908 RVA: 0x0029D07C File Offset: 0x0029B27C
		private static void TestLog2()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (uint num = 0U; num <= 4294967295U; num += 3000U)
			{
				num.UsedBitCount();
				num.UsedBitCount();
				num.UsedBitCount();
				num.UsedBitCount();
				num.UsedBitCount();
				if (4294967295U - num < 4000U)
				{
					break;
				}
			}
			stopwatch.Stop();
			Debug.Log("Log2 nifty: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x0600808D RID: 32909 RVA: 0x0029D0F4 File Offset: 0x0029B2F4
		private static void ArrayCopy()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				int num = 0;
				BenchmarkTests.ubuffer.ReadOutUnsafe(0, BenchmarkTests.buffer, ref num, 960);
			}
			stopwatch.Stop();
			Debug.Log("Array Copy Unsafe: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x0600808E RID: 32910 RVA: 0x0029D158 File Offset: 0x0029B358
		private static void ArrayCopySafe()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				int num = 0;
				BenchmarkTests.ubuffer.ReadOutSafe(0, BenchmarkTests.buffer, ref num, 960);
			}
			stopwatch.Stop();
			Debug.Log("Array Copy Safe: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x0600808F RID: 32911 RVA: 0x0029D1BC File Offset: 0x0029B3BC
		public static void ByteForByteWrite()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				BasicWriter.Reset();
				for (int j = 0; j < 128; j++)
				{
					BasicWriter.BasicWrite(BenchmarkTests.buffer, byte.MaxValue);
				}
				BasicWriter.Reset();
				for (int k = 0; k < 128; k++)
				{
					BasicWriter.BasicRead(BenchmarkTests.buffer);
				}
			}
			stopwatch.Stop();
			Debug.Log("Byte For Byte: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x06008090 RID: 32912 RVA: 0x0029D250 File Offset: 0x0029B450
		public static void BitpackBytesEven()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				int num = 0;
				for (int j = 0; j < 128; j++)
				{
					BenchmarkTests.buffer.Write(255UL, ref num, 8);
				}
				num = 0;
				for (int k = 0; k < 127; k++)
				{
					BenchmarkTests.buffer.Read(ref num, 8);
				}
			}
			stopwatch.Stop();
			Debug.Log("Even Bitpack byte: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x06008091 RID: 32913 RVA: 0x0029D2E4 File Offset: 0x0029B4E4
		public static void BitpackBytesToULongUneven()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				int num = 0;
				BenchmarkTests.ubuffer.Write(1UL, ref num, 1);
				for (int j = 0; j < 127; j++)
				{
					BenchmarkTests.ubuffer.Write(255UL, ref num, 33);
				}
				num = 0;
				BenchmarkTests.ubuffer.Read(ref num, 1);
				for (int k = 0; k < 127; k++)
				{
					BenchmarkTests.ubuffer.Read(ref num, 33);
				}
			}
			stopwatch.Stop();
			Debug.Log("Uneven Bitpack ulong[]: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x06008092 RID: 32914 RVA: 0x0029D394 File Offset: 0x0029B594
		public static void BitpackBytesUnEven()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				int num = 0;
				BenchmarkTests.buffer.Write(1UL, ref num, 1);
				for (int j = 0; j < 127; j++)
				{
					BenchmarkTests.buffer.Write(255UL, ref num, 8);
				}
				num = 0;
				BenchmarkTests.buffer.Read(ref num, 1);
				for (int k = 0; k < 127; k++)
				{
					BenchmarkTests.buffer.Read(ref num, 8);
				}
			}
			stopwatch.Stop();
			Debug.Log("Uneven Bitpack byte: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x040091A6 RID: 37286
		public const int BYTE_CNT = 128;

		// Token: 0x040091A7 RID: 37287
		public const int LOOP = 1000000;

		// Token: 0x040091A8 RID: 37288
		public static byte[] buffer = new byte[4800];

		// Token: 0x040091A9 RID: 37289
		public static uint[] ibuffer = new uint[128];

		// Token: 0x040091AA RID: 37290
		public static ulong[] ubuffer = new ulong[128];

		// Token: 0x040091AB RID: 37291
		public static ulong[] ubuffer2 = new ulong[128];
	}
}
