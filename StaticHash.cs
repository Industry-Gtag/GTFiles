using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02000DE6 RID: 3558
public static class StaticHash
{
	// Token: 0x06005724 RID: 22308 RVA: 0x001C7C30 File Offset: 0x001C5E30
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint ComputeU(uint u)
	{
		uint num = u + 2127912214U + (u << 12);
		num = num ^ 3345072700U ^ (num >> 19);
		num = num + 374761393U + (num << 5);
		num = (num + 3550635116U) ^ (num << 9);
		num = num + 4251993797U + (num << 3);
		return num ^ 3042594569U ^ (num >> 16);
	}

	// Token: 0x06005725 RID: 22309 RVA: 0x001C7C8C File Offset: 0x001C5E8C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint ComputeU(int i)
	{
		return StaticHash.ComputeU((uint)i);
	}

	// Token: 0x06005726 RID: 22310 RVA: 0x001C7C94 File Offset: 0x001C5E94
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(int i)
	{
		return (int)StaticHash.ComputeU(i);
	}

	// Token: 0x06005727 RID: 22311 RVA: 0x001C7C8C File Offset: 0x001C5E8C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(uint u)
	{
		return (int)StaticHash.ComputeU(u);
	}

	// Token: 0x06005728 RID: 22312 RVA: 0x001C7C9C File Offset: 0x001C5E9C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int Compute(float f)
	{
		return StaticHash.Compute(*Unsafe.As<float, uint>(ref f));
	}

	// Token: 0x06005729 RID: 22313 RVA: 0x001C7CAC File Offset: 0x001C5EAC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(float f1, float f2)
	{
		int num = StaticHash.Compute(f1);
		int num2 = StaticHash.Compute(f2);
		return StaticHash.Compute(num, num2);
	}

	// Token: 0x0600572A RID: 22314 RVA: 0x001C7CCC File Offset: 0x001C5ECC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(float f1, float f2, float f3)
	{
		int num = StaticHash.Compute(f1);
		int num2 = StaticHash.Compute(f2);
		int num3 = StaticHash.Compute(f3);
		return StaticHash.Compute(num, num2, num3);
	}

	// Token: 0x0600572B RID: 22315 RVA: 0x001C7CF4 File Offset: 0x001C5EF4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(float f1, float f2, float f3, float f4)
	{
		int num = StaticHash.Compute(f1);
		int num2 = StaticHash.Compute(f2);
		int num3 = StaticHash.Compute(f3);
		int num4 = StaticHash.Compute(f4);
		return StaticHash.Compute(num, num2, num3, num4);
	}

	// Token: 0x0600572C RID: 22316 RVA: 0x001C7D24 File Offset: 0x001C5F24
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong ComputeUL(ulong h)
	{
		h = ~h + (h << 18);
		h ^= h >> 31;
		h *= 21UL;
		h ^= h >> 11;
		h += h << 6;
		h ^= h >> 22;
		return h;
	}

	// Token: 0x0600572D RID: 22317 RVA: 0x001C7D56 File Offset: 0x001C5F56
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(long l)
	{
		return (int)StaticHash.ComputeUL((ulong)l);
	}

	// Token: 0x0600572E RID: 22318 RVA: 0x001C7D60 File Offset: 0x001C5F60
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(long l1, long l2)
	{
		int num = StaticHash.Compute(l1);
		int num2 = StaticHash.Compute(l2);
		return StaticHash.Compute(num, num2);
	}

	// Token: 0x0600572F RID: 22319 RVA: 0x001C7D80 File Offset: 0x001C5F80
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(long l1, long l2, long l3)
	{
		int num = StaticHash.Compute(l1);
		int num2 = StaticHash.Compute(l2);
		int num3 = StaticHash.Compute(l3);
		return StaticHash.Compute(num, num2, num3);
	}

	// Token: 0x06005730 RID: 22320 RVA: 0x001C7DA8 File Offset: 0x001C5FA8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(long l1, long l2, long l3, long l4)
	{
		int num = StaticHash.Compute(l1);
		int num2 = StaticHash.Compute(l2);
		int num3 = StaticHash.Compute(l3);
		int num4 = StaticHash.Compute(l4);
		return StaticHash.Compute(num, num2, num3, num4);
	}

	// Token: 0x06005731 RID: 22321 RVA: 0x001C7DD8 File Offset: 0x001C5FD8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int Compute(double d)
	{
		return StaticHash.Compute(*Unsafe.As<double, long>(ref d));
	}

	// Token: 0x06005732 RID: 22322 RVA: 0x001C7DE8 File Offset: 0x001C5FE8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(double d1, double d2)
	{
		int num = StaticHash.Compute(d1);
		int num2 = StaticHash.Compute(d2);
		return StaticHash.Compute(num, num2);
	}

	// Token: 0x06005733 RID: 22323 RVA: 0x001C7E08 File Offset: 0x001C6008
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(double d1, double d2, double d3)
	{
		int num = StaticHash.Compute(d1);
		int num2 = StaticHash.Compute(d2);
		int num3 = StaticHash.Compute(d3);
		return StaticHash.Compute(num, num2, num3);
	}

	// Token: 0x06005734 RID: 22324 RVA: 0x001C7E30 File Offset: 0x001C6030
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(double d1, double d2, double d3, double d4)
	{
		int num = StaticHash.Compute(d1);
		int num2 = StaticHash.Compute(d2);
		int num3 = StaticHash.Compute(d3);
		int num4 = StaticHash.Compute(d4);
		return StaticHash.Compute(num, num2, num3, num4);
	}

	// Token: 0x06005735 RID: 22325 RVA: 0x001C7E60 File Offset: 0x001C6060
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(bool b)
	{
		if (!b)
		{
			return 1800329511;
		}
		return -1266253386;
	}

	// Token: 0x06005736 RID: 22326 RVA: 0x001C7E70 File Offset: 0x001C6070
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(bool b1, bool b2)
	{
		int num = StaticHash.Compute(b1);
		int num2 = StaticHash.Compute(b2);
		return StaticHash.Compute(num, num2);
	}

	// Token: 0x06005737 RID: 22327 RVA: 0x001C7E90 File Offset: 0x001C6090
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(bool b1, bool b2, bool b3)
	{
		int num = StaticHash.Compute(b1);
		int num2 = StaticHash.Compute(b2);
		int num3 = StaticHash.Compute(b3);
		return StaticHash.Compute(num, num2, num3);
	}

	// Token: 0x06005738 RID: 22328 RVA: 0x001C7EB8 File Offset: 0x001C60B8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(bool b1, bool b2, bool b3, bool b4)
	{
		int num = StaticHash.Compute(b1);
		int num2 = StaticHash.Compute(b2);
		int num3 = StaticHash.Compute(b3);
		int num4 = StaticHash.Compute(b4);
		return StaticHash.Compute(num, num2, num3, num4);
	}

	// Token: 0x06005739 RID: 22329 RVA: 0x001C7EE8 File Offset: 0x001C60E8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(DateTime dt)
	{
		return StaticHash.Compute(dt.ToBinary());
	}

	// Token: 0x0600573A RID: 22330 RVA: 0x001C7EF8 File Offset: 0x001C60F8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(string s)
	{
		if (s == null || s.Length == 0)
		{
			return 0;
		}
		int i = s.Length;
		uint num = (uint)i;
		int num2 = i & 1;
		i >>= 1;
		int num3 = 0;
		while (i > 0)
		{
			num += (uint)s[num3];
			uint num4 = (uint)((uint)s[num3 + 1] << 11) ^ num;
			num = (num << 16) ^ num4;
			num3 += 2;
			num += num >> 11;
			i--;
		}
		if (num2 == 1)
		{
			num += (uint)s[num3];
			num ^= num << 11;
			num += num >> 17;
		}
		num ^= num << 3;
		num += num >> 5;
		num ^= num << 4;
		num += num >> 17;
		num ^= num << 25;
		return (int)(num + (num >> 6));
	}

	// Token: 0x0600573B RID: 22331 RVA: 0x001C7FA0 File Offset: 0x001C61A0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(string s1, string s2)
	{
		int num = StaticHash.Compute(s1);
		int num2 = StaticHash.Compute(s2);
		return StaticHash.Compute(num, num2);
	}

	// Token: 0x0600573C RID: 22332 RVA: 0x001C7FC0 File Offset: 0x001C61C0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(string s1, string s2, string s3)
	{
		int num = StaticHash.Compute(s1);
		int num2 = StaticHash.Compute(s2);
		int num3 = StaticHash.Compute(s3);
		return StaticHash.Compute(num, num2, num3);
	}

	// Token: 0x0600573D RID: 22333 RVA: 0x001C7FE8 File Offset: 0x001C61E8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(string s1, string s2, string s3, string s4)
	{
		int num = StaticHash.Compute(s1);
		int num2 = StaticHash.Compute(s2);
		int num3 = StaticHash.Compute(s3);
		int num4 = StaticHash.Compute(s4);
		return StaticHash.Compute(num, num2, num3, num4);
	}

	// Token: 0x0600573E RID: 22334 RVA: 0x001C8018 File Offset: 0x001C6218
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(byte[] bytes)
	{
		if (bytes == null || bytes.Length == 0)
		{
			return 0;
		}
		int i = bytes.Length;
		uint num = (uint)i;
		int num2 = i & 1;
		i >>= 1;
		int num3 = 0;
		while (i > 0)
		{
			num += (uint)bytes[num3];
			uint num4 = (uint)(((int)bytes[num3 + 1] << 11) ^ (int)num);
			num = (num << 16) ^ num4;
			num3 += 2;
			num += num >> 11;
			i--;
		}
		if (num2 == 1)
		{
			num += (uint)bytes[num3];
			num ^= num << 11;
			num += num >> 17;
		}
		num ^= num << 3;
		num += num >> 5;
		num ^= num << 4;
		num += num >> 17;
		num ^= num << 25;
		return (int)(num + (num >> 6));
	}

	// Token: 0x0600573F RID: 22335 RVA: 0x001C80AC File Offset: 0x001C62AC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(int i1, int i2)
	{
		uint num = 3735928567U;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06005740 RID: 22336 RVA: 0x001C80D8 File Offset: 0x001C62D8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(int i1, int i2, int i3)
	{
		uint num = 3735928571U;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		num3 += (uint)i3;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06005741 RID: 22337 RVA: 0x001C8108 File Offset: 0x001C6308
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(int i1, int i2, int i3, int i4)
	{
		uint num = 3735928575U;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		num3 += (uint)i3;
		StaticHash.Mix(ref num, ref num2, ref num3);
		num += (uint)i4;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06005742 RID: 22338 RVA: 0x001C8148 File Offset: 0x001C6348
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(int[] values)
	{
		if (values == null || values.Length == 0)
		{
			return 224428569;
		}
		int num = values.Length;
		uint num2 = (uint)(-559038737 + (num << 2));
		uint num3 = num2;
		uint num4 = num2;
		int num5 = 0;
		while (num - num5 > 3)
		{
			num2 += (uint)values[num5];
			num3 += (uint)values[num5 + 1];
			num4 += (uint)values[num5 + 2];
			StaticHash.Mix(ref num2, ref num3, ref num4);
			num5 += 3;
		}
		if (num - num5 > 2)
		{
			num4 += (uint)values[num5 + 2];
		}
		if (num - num5 > 1)
		{
			num3 += (uint)values[num5 + 1];
		}
		if (num - num5 > 0)
		{
			num2 += (uint)values[num5];
			StaticHash.Finalize(ref num2, ref num3, ref num4);
		}
		return (int)num4;
	}

	// Token: 0x06005743 RID: 22339 RVA: 0x001C81E4 File Offset: 0x001C63E4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(uint[] values)
	{
		if (values == null || values.Length == 0)
		{
			return 224428569;
		}
		int num = values.Length;
		uint num2 = (uint)(-559038737 + (num << 2));
		uint num3 = num2;
		uint num4 = num2;
		int num5 = 0;
		while (num - num5 > 3)
		{
			num2 += values[num5];
			num3 += values[num5 + 1];
			num4 += values[num5 + 2];
			StaticHash.Mix(ref num2, ref num3, ref num4);
			num5 += 3;
		}
		if (num - num5 > 2)
		{
			num4 += values[num5 + 2];
		}
		if (num - num5 > 1)
		{
			num3 += values[num5 + 1];
		}
		if (num - num5 > 0)
		{
			num2 += values[num5];
			StaticHash.Finalize(ref num2, ref num3, ref num4);
		}
		return (int)num4;
	}

	// Token: 0x06005744 RID: 22340 RVA: 0x001C8280 File Offset: 0x001C6480
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(uint u1, uint u2)
	{
		uint num = 3735928567U;
		uint num2 = num;
		uint num3 = num;
		num += u1;
		num2 += u2;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06005745 RID: 22341 RVA: 0x001C82AC File Offset: 0x001C64AC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(uint u1, uint u2, uint u3)
	{
		uint num = 3735928571U;
		uint num2 = num;
		uint num3 = num;
		num += u1;
		num2 += u2;
		num3 += u3;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06005746 RID: 22342 RVA: 0x001C82DC File Offset: 0x001C64DC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(uint u1, uint u2, uint u3, uint u4)
	{
		uint num = 3735928575U;
		uint num2 = num;
		uint num3 = num;
		num += u1;
		num2 += u2;
		num3 += u3;
		StaticHash.Mix(ref num, ref num2, ref num3);
		num += u4;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06005747 RID: 22343 RVA: 0x001C831C File Offset: 0x001C651C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeOrderAgnostic(int[] values)
	{
		if (values == null || values.Length == 0)
		{
			return 0;
		}
		uint num = (uint)StaticHash.Compute(values[0]);
		if (values.Length == 1)
		{
			return (int)num;
		}
		for (int i = 1; i < values.Length; i++)
		{
			num += (uint)StaticHash.Compute(values[i]);
		}
		return (int)num;
	}

	// Token: 0x06005748 RID: 22344 RVA: 0x001C8360 File Offset: 0x001C6560
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Compute128To64(long a, long b)
	{
		ulong num = (ulong)((b ^ a) * -7070675565921424023L);
		num ^= num >> 47;
		long num2 = (a ^ (long)num) * -7070675565921424023L;
		return (num2 ^ (long)((ulong)num2 >> 47)) * -7070675565921424023L;
	}

	// Token: 0x06005749 RID: 22345 RVA: 0x001C83A0 File Offset: 0x001C65A0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Compute128To64(ulong a, ulong b)
	{
		ulong num = (b ^ a) * 11376068507788127593UL;
		num ^= num >> 47;
		ulong num2 = (a ^ num) * 11376068507788127593UL;
		return (long)((num2 ^ (num2 >> 47)) * 11376068507788127593UL);
	}

	// Token: 0x0600574A RID: 22346 RVA: 0x001C83DE File Offset: 0x001C65DE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeTriple32(int i)
	{
		int num = i + 1;
		int num2 = (num ^ (int)((uint)num >> 17)) * -312814405;
		int num3 = (num2 ^ (int)((uint)num2 >> 11)) * -1404298415;
		int num4 = (num3 ^ (int)((uint)num3 >> 15)) * 830770091;
		return num4 ^ (int)((uint)num4 >> 14);
	}

	// Token: 0x0600574B RID: 22347 RVA: 0x001C840C File Offset: 0x001C660C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReverseTriple32(int i)
	{
		uint num = (uint)(i ^ (int)(((uint)i >> 14) ^ ((uint)i >> 28)));
		num *= 850532099U;
		num ^= (num >> 15) ^ (num >> 30);
		num *= 1184763313U;
		num ^= (num >> 11) ^ (num >> 22);
		num *= 2041073779U;
		num ^= num >> 17;
		return (int)(num - 1U);
	}

	// Token: 0x0600574C RID: 22348 RVA: 0x001C8464 File Offset: 0x001C6664
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Mix(ref uint a, ref uint b, ref uint c)
	{
		a -= c;
		a ^= StaticHash.Rotate(c, 4);
		c += b;
		b -= a;
		b ^= StaticHash.Rotate(a, 6);
		a += c;
		c -= b;
		c ^= StaticHash.Rotate(b, 8);
		b += a;
		a -= c;
		a ^= StaticHash.Rotate(c, 16);
		c += b;
		b -= a;
		b ^= StaticHash.Rotate(a, 19);
		a += c;
		c -= b;
		c ^= StaticHash.Rotate(b, 4);
		b += a;
	}

	// Token: 0x0600574D RID: 22349 RVA: 0x001C8518 File Offset: 0x001C6718
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Finalize(ref uint a, ref uint b, ref uint c)
	{
		c ^= b;
		c -= StaticHash.Rotate(b, 14);
		a ^= c;
		a -= StaticHash.Rotate(c, 11);
		b ^= a;
		b -= StaticHash.Rotate(a, 25);
		c ^= b;
		c -= StaticHash.Rotate(b, 16);
		a ^= c;
		a -= StaticHash.Rotate(c, 4);
		b ^= a;
		b -= StaticHash.Rotate(a, 14);
		c ^= b;
		c -= StaticHash.Rotate(b, 24);
	}

	// Token: 0x0600574E RID: 22350 RVA: 0x001C85B7 File Offset: 0x001C67B7
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Rotate(uint x, int k)
	{
		return (x << k) | (x >> 32 - k);
	}

	// Token: 0x02000DE7 RID: 3559
	[StructLayout(LayoutKind.Explicit)]
	private struct SingleInt32
	{
		// Token: 0x040067F5 RID: 26613
		[FieldOffset(0)]
		public float single;

		// Token: 0x040067F6 RID: 26614
		[FieldOffset(0)]
		public int int32;
	}

	// Token: 0x02000DE8 RID: 3560
	[StructLayout(LayoutKind.Explicit)]
	private struct DoubleInt64
	{
		// Token: 0x040067F7 RID: 26615
		[FieldOffset(0)]
		public double @double;

		// Token: 0x040067F8 RID: 26616
		[FieldOffset(0)]
		public long int64;
	}
}
