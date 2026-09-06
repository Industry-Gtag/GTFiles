using System;
using UnityEngine;

// Token: 0x02000B17 RID: 2839
[Serializable]
public struct StringEnum<TEnum> where TEnum : struct, Enum
{
	// Token: 0x170006E6 RID: 1766
	// (get) Token: 0x060048DC RID: 18652 RVA: 0x00186C34 File Offset: 0x00184E34
	public TEnum Value
	{
		get
		{
			return this.m_EnumValue;
		}
	}

	// Token: 0x060048DD RID: 18653 RVA: 0x00186C3C File Offset: 0x00184E3C
	public static implicit operator StringEnum<TEnum>(TEnum e)
	{
		return new StringEnum<TEnum>
		{
			m_EnumValue = e
		};
	}

	// Token: 0x060048DE RID: 18654 RVA: 0x00186C34 File Offset: 0x00184E34
	public static implicit operator TEnum(StringEnum<TEnum> se)
	{
		return se.m_EnumValue;
	}

	// Token: 0x060048DF RID: 18655 RVA: 0x00186C5A File Offset: 0x00184E5A
	public static bool operator ==(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return left.m_EnumValue.Equals(right.m_EnumValue);
	}

	// Token: 0x060048E0 RID: 18656 RVA: 0x00186C79 File Offset: 0x00184E79
	public static bool operator !=(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return !(left == right);
	}

	// Token: 0x060048E1 RID: 18657 RVA: 0x00186C88 File Offset: 0x00184E88
	public override bool Equals(object obj)
	{
		if (obj is StringEnum<TEnum>)
		{
			StringEnum<TEnum> stringEnum = (StringEnum<TEnum>)obj;
			return this.m_EnumValue.Equals(stringEnum.m_EnumValue);
		}
		return false;
	}

	// Token: 0x060048E2 RID: 18658 RVA: 0x00186CC2 File Offset: 0x00184EC2
	public override int GetHashCode()
	{
		return this.m_EnumValue.GetHashCode();
	}

	// Token: 0x060048E3 RID: 18659 RVA: 0x00186CD8 File Offset: 0x00184ED8
	public override string ToString()
	{
		TEnum value = this.Value;
		return value.ToString();
	}

	// Token: 0x04005B26 RID: 23334
	[SerializeField]
	private TEnum m_EnumValue;
}
