using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Text;
using UnityEngine;

// Token: 0x02000DEA RID: 3562
public static class StringUtils
{
	// Token: 0x06005758 RID: 22360 RVA: 0x001C8611 File Offset: 0x001C6811
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty(this string s)
	{
		return string.IsNullOrEmpty(s);
	}

	// Token: 0x06005759 RID: 22361 RVA: 0x001C8619 File Offset: 0x001C6819
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrWhiteSpace(this string s)
	{
		return string.IsNullOrWhiteSpace(s);
	}

	// Token: 0x0600575A RID: 22362 RVA: 0x001C8624 File Offset: 0x001C6824
	public static string ToAlphaNumeric(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return string.Empty;
		}
		string text;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
		{
			foreach (char c in s)
			{
				if (char.IsLetterOrDigit(c))
				{
					utf16ValueStringBuilder.Append(c);
				}
			}
			text = utf16ValueStringBuilder.ToString();
		}
		return text;
	}

	// Token: 0x0600575B RID: 22363 RVA: 0x001C86A0 File Offset: 0x001C68A0
	public static string Capitalize(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		char[] array = s.ToCharArray();
		array[0] = char.ToUpperInvariant(array[0]);
		return new string(array);
	}

	// Token: 0x0600575C RID: 22364 RVA: 0x001C86CF File Offset: 0x001C68CF
	public static string Concat(this IEnumerable<string> source)
	{
		return string.Concat(source);
	}

	// Token: 0x0600575D RID: 22365 RVA: 0x001C86D7 File Offset: 0x001C68D7
	public static string Join(this IEnumerable<string> source, string separator)
	{
		return string.Join(separator, source);
	}

	// Token: 0x0600575E RID: 22366 RVA: 0x001C86E0 File Offset: 0x001C68E0
	public static string Join(this IEnumerable<string> source, char separator)
	{
		return string.Join<string>(separator, source);
	}

	// Token: 0x0600575F RID: 22367 RVA: 0x001C86E9 File Offset: 0x001C68E9
	public static string RemoveAll(this string s, string value, StringComparison mode = StringComparison.OrdinalIgnoreCase)
	{
		if (string.IsNullOrEmpty(s))
		{
			return s;
		}
		return s.Replace(value, string.Empty, mode);
	}

	// Token: 0x06005760 RID: 22368 RVA: 0x001C8702 File Offset: 0x001C6902
	public static string RemoveAll(this string s, char value, StringComparison mode = StringComparison.OrdinalIgnoreCase)
	{
		return s.RemoveAll(value.ToString(), mode);
	}

	// Token: 0x06005761 RID: 22369 RVA: 0x001C8712 File Offset: 0x001C6912
	public static byte[] ToBytesASCII(this string s)
	{
		return Encoding.ASCII.GetBytes(s);
	}

	// Token: 0x06005762 RID: 22370 RVA: 0x001C871F File Offset: 0x001C691F
	public static byte[] ToBytesUTF8(this string s)
	{
		return Encoding.UTF8.GetBytes(s);
	}

	// Token: 0x06005763 RID: 22371 RVA: 0x001C872C File Offset: 0x001C692C
	public static byte[] ToBytesUnicode(this string s)
	{
		return Encoding.Unicode.GetBytes(s);
	}

	// Token: 0x06005764 RID: 22372 RVA: 0x001C873C File Offset: 0x001C693C
	public static string ComputeSHV2(this string s)
	{
		return Hash128.Compute(s).ToString();
	}

	// Token: 0x06005765 RID: 22373 RVA: 0x001C875D File Offset: 0x001C695D
	public static string ToQueryString(this Dictionary<string, string> d)
	{
		if (d == null)
		{
			return null;
		}
		return "?" + string.Join("&", d.Select((KeyValuePair<string, string> x) => x.Key + "=" + x.Value));
	}

	// Token: 0x06005766 RID: 22374 RVA: 0x001C87A0 File Offset: 0x001C69A0
	public static string Combine(string separator, params string[] values)
	{
		if (values == null || values.Length == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = !string.IsNullOrEmpty(separator);
		for (int i = 0; i < values.Length; i++)
		{
			if (flag)
			{
				stringBuilder.Append(separator);
			}
			stringBuilder.Append(values);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06005767 RID: 22375 RVA: 0x001C87F0 File Offset: 0x001C69F0
	public static string ToUpperCamelCase(this string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return string.Empty;
		}
		string[] array = Regex.Split(input, "[^A-Za-z0-9]+");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Length > 0)
			{
				string[] array2 = array;
				int num = i;
				string text = char.ToUpper(array[i][0]).ToString();
				string text2;
				if (array[i].Length <= 1)
				{
					text2 = "";
				}
				else
				{
					string text3 = array[i];
					text2 = text3.Substring(1, text3.Length - 1).ToLower();
				}
				array2[num] = text + text2;
			}
		}
		return string.Join("", array);
	}

	// Token: 0x06005768 RID: 22376 RVA: 0x001C8884 File Offset: 0x001C6A84
	public static string ToUpperCaseFromCamelCase(this string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		input = input.Trim();
		string text;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
		{
			bool flag = true;
			foreach (char c in input)
			{
				if (char.IsUpper(c) && !flag)
				{
					utf16ValueStringBuilder.Append(' ');
				}
				utf16ValueStringBuilder.Append(char.ToUpper(c));
				flag = char.IsUpper(c);
			}
			text = utf16ValueStringBuilder.ToString().Trim();
		}
		return text;
	}

	// Token: 0x06005769 RID: 22377 RVA: 0x001C8924 File Offset: 0x001C6B24
	public static string RemoveStart(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		if (string.IsNullOrEmpty(s) || !s.StartsWith(value, comparison))
		{
			return s;
		}
		return s.Substring(value.Length);
	}

	// Token: 0x0600576A RID: 22378 RVA: 0x001C8946 File Offset: 0x001C6B46
	public static string RemoveEnd(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		if (string.IsNullOrEmpty(s) || !s.EndsWith(value, comparison))
		{
			return s;
		}
		return s.Substring(0, s.Length - value.Length);
	}

	// Token: 0x0600576B RID: 22379 RVA: 0x001C8970 File Offset: 0x001C6B70
	public static string RemoveBothEnds(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		return s.RemoveEnd(value, comparison).RemoveStart(value, comparison);
	}

	// Token: 0x0600576C RID: 22380 RVA: 0x001C8981 File Offset: 0x001C6B81
	public static string TrailingSpace(this string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			Debug.LogError("[STRING::UTILS] Trying to add Space, but string is null or empty");
			return s;
		}
		if (s[s.Length - 1] == ' ')
		{
			return s;
		}
		return s + " ";
	}

	// Token: 0x040067F9 RID: 26617
	public const string kForwardSlash = "/";

	// Token: 0x040067FA RID: 26618
	public const string kBackSlash = "/";

	// Token: 0x040067FB RID: 26619
	public const string kBackTick = "`";

	// Token: 0x040067FC RID: 26620
	public const string kMinusDash = "-";

	// Token: 0x040067FD RID: 26621
	public const string kPeriod = ".";

	// Token: 0x040067FE RID: 26622
	public const string kUnderScore = "_";

	// Token: 0x040067FF RID: 26623
	public const string kColon = ":";
}
