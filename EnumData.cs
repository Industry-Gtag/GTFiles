using System;
using System.Collections.Generic;

// Token: 0x02000AEF RID: 2799
public class EnumData<TEnum> where TEnum : struct, Enum
{
	// Token: 0x170006CF RID: 1743
	// (get) Token: 0x060047BD RID: 18365 RVA: 0x001839FD File Offset: 0x00181BFD
	public static EnumData<TEnum> Shared { get; } = new EnumData<TEnum>();

	// Token: 0x060047BE RID: 18366 RVA: 0x00183A04 File Offset: 0x00181C04
	private EnumData()
	{
		this.Names = Enum.GetNames(typeof(TEnum));
		this.Values = (TEnum[])Enum.GetValues(typeof(TEnum));
		int num = this.Names.Length;
		this.LongValues = new long[num];
		this.EnumToName = new Dictionary<TEnum, string>(num);
		this.NameToEnum = new Dictionary<string, TEnum>(num * 2);
		this.EnumToIndex = new Dictionary<TEnum, int>(num);
		this.IndexToEnum = new Dictionary<int, TEnum>(num);
		this.EnumToLong = new Dictionary<TEnum, long>(num);
		this.LongToEnum = new Dictionary<long, TEnum>(num);
		long num2 = long.MaxValue;
		long num3 = long.MinValue;
		for (int i = 0; i < this.Names.Length; i++)
		{
			string text = this.Names[i];
			TEnum tenum = this.Values[i];
			long num4 = Convert.ToInt64(tenum);
			this.LongValues[i] = num4;
			this.EnumToName[tenum] = text;
			this.NameToEnum[text] = tenum;
			this.NameToEnum.TryAdd(text.ToLowerInvariant(), tenum);
			this.EnumToIndex[tenum] = i;
			this.IndexToEnum[i] = tenum;
			this.EnumToLong[tenum] = num4;
			this.LongToEnum[num4] = tenum;
			num2 = Math.Min(num4, num2);
			num3 = Math.Max(num4, num3);
		}
		for (int j = 0; j < this.Names.Length; j++)
		{
			string text2 = this.Names[j];
			TEnum tenum2 = this.Values[j];
			this.NameToEnum[text2] = tenum2;
		}
		this.MinValue = this.LongToEnum[num2];
		this.MaxValue = this.LongToEnum[num3];
		this.MinInt = Convert.ToInt32(num2);
		this.MaxInt = Convert.ToInt32(num3);
		this.MinLong = num2;
		this.MaxLong = num3;
		long num5 = 0L;
		bool flag = true;
		foreach (long num6 in this.LongValues)
		{
			if (num6 != 0L && (num6 & (num6 - 1L)) != 0L && (num5 & num6) != num6)
			{
				flag = false;
				break;
			}
			num5 |= num6;
		}
		this.IsBitMaskCompatible = flag;
	}

	// Token: 0x04005A3F RID: 23103
	public readonly string[] Names;

	// Token: 0x04005A40 RID: 23104
	public readonly TEnum[] Values;

	// Token: 0x04005A41 RID: 23105
	public readonly long[] LongValues;

	// Token: 0x04005A42 RID: 23106
	public readonly bool IsBitMaskCompatible;

	// Token: 0x04005A43 RID: 23107
	public readonly Dictionary<TEnum, string> EnumToName;

	// Token: 0x04005A44 RID: 23108
	public readonly Dictionary<string, TEnum> NameToEnum;

	// Token: 0x04005A45 RID: 23109
	public readonly Dictionary<TEnum, int> EnumToIndex;

	// Token: 0x04005A46 RID: 23110
	public readonly Dictionary<int, TEnum> IndexToEnum;

	// Token: 0x04005A47 RID: 23111
	public readonly Dictionary<TEnum, long> EnumToLong;

	// Token: 0x04005A48 RID: 23112
	public readonly Dictionary<long, TEnum> LongToEnum;

	// Token: 0x04005A49 RID: 23113
	public readonly TEnum MinValue;

	// Token: 0x04005A4A RID: 23114
	public readonly TEnum MaxValue;

	// Token: 0x04005A4B RID: 23115
	public readonly int MinInt;

	// Token: 0x04005A4C RID: 23116
	public readonly int MaxInt;

	// Token: 0x04005A4D RID: 23117
	public readonly long MinLong;

	// Token: 0x04005A4E RID: 23118
	public readonly long MaxLong;
}
