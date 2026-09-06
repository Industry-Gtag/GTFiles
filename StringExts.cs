using System;

// Token: 0x02000B18 RID: 2840
public static class StringExts
{
	// Token: 0x060048E4 RID: 18660 RVA: 0x00186CF9 File Offset: 0x00184EF9
	public static string EscapeCsv(this string field)
	{
		if (StringExts._escapeChars == null)
		{
			StringExts._escapeChars = new char[] { ',', '"', '\n', '\r' };
		}
		if (field.IndexOfAny(StringExts._escapeChars) != -1)
		{
			return field.Replace("\"", "\"\"");
		}
		return field;
	}

	// Token: 0x04005B27 RID: 23335
	private static char[] _escapeChars;
}
