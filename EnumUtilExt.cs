using System;

// Token: 0x02000AF1 RID: 2801
public static class EnumUtilExt
{
	// Token: 0x060047D1 RID: 18385 RVA: 0x00183CA3 File Offset: 0x00181EA3
	public static string GetName<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x060047D2 RID: 18386 RVA: 0x00183CC7 File Offset: 0x00181EC7
	public static int GetIndex<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x060047D3 RID: 18387 RVA: 0x00183CEB File Offset: 0x00181EEB
	public static long GetLongValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x060047D4 RID: 18388 RVA: 0x00183DEC File Offset: 0x00181FEC
	public static TEnum GetNextValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		return shared.Values[shared.EnumToIndex[e] + 1 % shared.Values.Length];
	}
}
