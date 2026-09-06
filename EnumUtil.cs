using System;
using System.Collections.Generic;

// Token: 0x02000AF0 RID: 2800
public static class EnumUtil
{
	// Token: 0x060047C0 RID: 18368 RVA: 0x00183C70 File Offset: 0x00181E70
	public static string[] GetNames<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<string>(EnumData<TEnum>.Shared.Names);
	}

	// Token: 0x060047C1 RID: 18369 RVA: 0x00183C81 File Offset: 0x00181E81
	public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<TEnum>(EnumData<TEnum>.Shared.Values);
	}

	// Token: 0x060047C2 RID: 18370 RVA: 0x00183C92 File Offset: 0x00181E92
	public static long[] GetLongValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<long>(EnumData<TEnum>.Shared.LongValues);
	}

	// Token: 0x060047C3 RID: 18371 RVA: 0x00183CA3 File Offset: 0x00181EA3
	public static string EnumToName<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x060047C4 RID: 18372 RVA: 0x00183CB5 File Offset: 0x00181EB5
	public static TEnum NameToEnum<TEnum>(string n) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[n];
	}

	// Token: 0x060047C5 RID: 18373 RVA: 0x00183CC7 File Offset: 0x00181EC7
	public static int EnumToIndex<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x060047C6 RID: 18374 RVA: 0x00183CD9 File Offset: 0x00181ED9
	public static TEnum IndexToEnum<TEnum>(int i) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.IndexToEnum[i];
	}

	// Token: 0x060047C7 RID: 18375 RVA: 0x00183CEB File Offset: 0x00181EEB
	public static long EnumToLong<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x060047C8 RID: 18376 RVA: 0x00183CFD File Offset: 0x00181EFD
	public static TEnum LongToEnum<TEnum>(long l) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[l];
	}

	// Token: 0x060047C9 RID: 18377 RVA: 0x00183D0F File Offset: 0x00181F0F
	public static TEnum GetValue<TEnum>(int index) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.Values[index];
	}

	// Token: 0x060047CA RID: 18378 RVA: 0x00183CC7 File Offset: 0x00181EC7
	public static int GetIndex<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[value];
	}

	// Token: 0x060047CB RID: 18379 RVA: 0x00183CA3 File Offset: 0x00181EA3
	public static string GetName<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[value];
	}

	// Token: 0x060047CC RID: 18380 RVA: 0x00183CB5 File Offset: 0x00181EB5
	public static TEnum GetValue<TEnum>(string name) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[name];
	}

	// Token: 0x060047CD RID: 18381 RVA: 0x00183CEB File Offset: 0x00181EEB
	public static long GetLongValue<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[value];
	}

	// Token: 0x060047CE RID: 18382 RVA: 0x00183CFD File Offset: 0x00181EFD
	public static TEnum GetValue<TEnum>(long longValue) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[longValue];
	}

	// Token: 0x060047CF RID: 18383 RVA: 0x00183D21 File Offset: 0x00181F21
	public static TEnum[] SplitBitmask<TEnum>(TEnum bitmask) where TEnum : struct, Enum
	{
		return EnumUtil.SplitBitmask<TEnum>(Convert.ToInt64(bitmask));
	}

	// Token: 0x060047D0 RID: 18384 RVA: 0x00183D34 File Offset: 0x00181F34
	public static TEnum[] SplitBitmask<TEnum>(long bitmaskLong) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (!shared.IsBitMaskCompatible)
		{
			throw new ArgumentException("The enum type " + typeof(TEnum).Name + " is not bitmask-compatible.");
		}
		if (bitmaskLong == 0L)
		{
			return new TEnum[] { (TEnum)((object)Enum.ToObject(typeof(TEnum), 0L)) };
		}
		List<TEnum> list = new List<TEnum>(shared.Values.Length);
		for (int i = 0; i < shared.Values.Length; i++)
		{
			TEnum tenum = shared.Values[i];
			long num = shared.LongValues[i];
			if (num != 0L && (bitmaskLong & num) == num)
			{
				list.Add(tenum);
			}
		}
		return list.ToArray();
	}
}
