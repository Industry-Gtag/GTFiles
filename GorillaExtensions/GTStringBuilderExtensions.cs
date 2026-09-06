using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using UnityEngine;

namespace GorillaExtensions
{
	// Token: 0x020011C3 RID: 4547
	public static class GTStringBuilderExtensions
	{
		// Token: 0x0600729F RID: 29343 RVA: 0x00255840 File Offset: 0x00253A40
		public unsafe static IEnumerable<ReadOnlyMemory<char>> GetSegmentsOfMem(this Utf16ValueStringBuilder sb, int maxCharsPerSegment = 16300)
		{
			int i = 0;
			List<ReadOnlyMemory<char>> list = new List<ReadOnlyMemory<char>>(64);
			ReadOnlyMemory<char> readOnlyMemory = sb.AsMemory();
			while (i < readOnlyMemory.Length)
			{
				int num = Mathf.Min(i + maxCharsPerSegment, readOnlyMemory.Length);
				if (num < readOnlyMemory.Length)
				{
					int num2 = -1;
					for (int j = num - 1; j >= i; j--)
					{
						if (*readOnlyMemory.Span[j] == 10)
						{
							num2 = j;
							break;
						}
					}
					if (num2 != -1)
					{
						num = num2;
					}
				}
				list.Add(readOnlyMemory.Slice(i, num - i));
				i = num + 1;
			}
			return list;
		}

		// Token: 0x060072A0 RID: 29344 RVA: 0x002558D5 File Offset: 0x00253AD5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTAddPath(this Utf16ValueStringBuilder stringBuilderToAddTo, GameObject gameObject)
		{
			gameObject.transform.GetPathQ(ref stringBuilderToAddTo);
		}

		// Token: 0x060072A1 RID: 29345 RVA: 0x002558E4 File Offset: 0x00253AE4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTAddPath(this Utf16ValueStringBuilder stringBuilderToAddTo, Transform transform)
		{
			transform.GetPathQ(ref stringBuilderToAddTo);
		}

		// Token: 0x060072A2 RID: 29346 RVA: 0x002558EE File Offset: 0x00253AEE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Q(this Utf16ValueStringBuilder sb, string value)
		{
			sb.Append('"');
			sb.Append(value);
			sb.Append('"');
		}

		// Token: 0x060072A3 RID: 29347 RVA: 0x0025590A File Offset: 0x00253B0A
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b)
		{
			sb.Append(a);
			sb.Append(b);
		}

		// Token: 0x060072A4 RID: 29348 RVA: 0x0025591C File Offset: 0x00253B1C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
		}

		// Token: 0x060072A5 RID: 29349 RVA: 0x00255936 File Offset: 0x00253B36
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
		}

		// Token: 0x060072A6 RID: 29350 RVA: 0x00255959 File Offset: 0x00253B59
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
		}

		// Token: 0x060072A7 RID: 29351 RVA: 0x00255985 File Offset: 0x00253B85
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
		}

		// Token: 0x060072A8 RID: 29352 RVA: 0x002559BA File Offset: 0x00253BBA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
		}

		// Token: 0x060072A9 RID: 29353 RVA: 0x002559F8 File Offset: 0x00253BF8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
		}

		// Token: 0x060072AA RID: 29354 RVA: 0x00255A4C File Offset: 0x00253C4C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
			sb.Append(i);
		}

		// Token: 0x060072AB RID: 29355 RVA: 0x00255AA8 File Offset: 0x00253CA8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i, string j)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
			sb.Append(i);
			sb.Append(j);
		}
	}
}
