using System;
using System.Globalization;

namespace emotitron.Compression.HalfFloat
{
	// Token: 0x020013E2 RID: 5090
	[Serializable]
	public struct Half : IConvertible, IComparable, IComparable<Half>, IEquatable<Half>, IFormattable
	{
		// Token: 0x0600805C RID: 32860 RVA: 0x0029C4F2 File Offset: 0x0029A6F2
		public Half(float value)
		{
			this.value = HalfUtilities.Pack(value);
		}

		// Token: 0x17000C59 RID: 3161
		// (get) Token: 0x0600805D RID: 32861 RVA: 0x0029C500 File Offset: 0x0029A700
		public ushort RawValue
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x0600805E RID: 32862 RVA: 0x0029C508 File Offset: 0x0029A708
		public static float[] ConvertToFloat(Half[] values)
		{
			float[] array = new float[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HalfUtilities.Unpack(values[i].RawValue);
			}
			return array;
		}

		// Token: 0x0600805F RID: 32863 RVA: 0x0029C544 File Offset: 0x0029A744
		public static Half[] ConvertToHalf(float[] values)
		{
			Half[] array = new Half[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Half(values[i]);
			}
			return array;
		}

		// Token: 0x06008060 RID: 32864 RVA: 0x0029C578 File Offset: 0x0029A778
		public static bool IsInfinity(Half half)
		{
			return half == Half.PositiveInfinity || half == Half.NegativeInfinity;
		}

		// Token: 0x06008061 RID: 32865 RVA: 0x0029C594 File Offset: 0x0029A794
		public static bool IsNaN(Half half)
		{
			return half == Half.NaN;
		}

		// Token: 0x06008062 RID: 32866 RVA: 0x0029C5A1 File Offset: 0x0029A7A1
		public static bool IsNegativeInfinity(Half half)
		{
			return half == Half.NegativeInfinity;
		}

		// Token: 0x06008063 RID: 32867 RVA: 0x0029C5AE File Offset: 0x0029A7AE
		public static bool IsPositiveInfinity(Half half)
		{
			return half == Half.PositiveInfinity;
		}

		// Token: 0x06008064 RID: 32868 RVA: 0x0029C5BB File Offset: 0x0029A7BB
		public static bool operator <(Half left, Half right)
		{
			return left < right;
		}

		// Token: 0x06008065 RID: 32869 RVA: 0x0029C5CD File Offset: 0x0029A7CD
		public static bool operator >(Half left, Half right)
		{
			return left > right;
		}

		// Token: 0x06008066 RID: 32870 RVA: 0x0029C5DF File Offset: 0x0029A7DF
		public static bool operator <=(Half left, Half right)
		{
			return left <= right;
		}

		// Token: 0x06008067 RID: 32871 RVA: 0x0029C5F4 File Offset: 0x0029A7F4
		public static bool operator >=(Half left, Half right)
		{
			return left >= right;
		}

		// Token: 0x06008068 RID: 32872 RVA: 0x0029C609 File Offset: 0x0029A809
		public static bool operator ==(Half left, Half right)
		{
			return left.Equals(right);
		}

		// Token: 0x06008069 RID: 32873 RVA: 0x0029C613 File Offset: 0x0029A813
		public static bool operator !=(Half left, Half right)
		{
			return !left.Equals(right);
		}

		// Token: 0x0600806A RID: 32874 RVA: 0x0029C620 File Offset: 0x0029A820
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x0600806B RID: 32875 RVA: 0x0029C628 File Offset: 0x0029A828
		public static implicit operator float(Half value)
		{
			return HalfUtilities.Unpack(value.value);
		}

		// Token: 0x0600806C RID: 32876 RVA: 0x0029C638 File Offset: 0x0029A838
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x0600806D RID: 32877 RVA: 0x0029C668 File Offset: 0x0029A868
		public string ToString(string format)
		{
			if (format == null)
			{
				return this.ToString();
			}
			return string.Format(CultureInfo.CurrentCulture, this.ToString(format, CultureInfo.CurrentCulture), Array.Empty<object>());
		}

		// Token: 0x0600806E RID: 32878 RVA: 0x0029C6B0 File Offset: 0x0029A8B0
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x0600806F RID: 32879 RVA: 0x0029C6DC File Offset: 0x0029A8DC
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				this.ToString(formatProvider);
			}
			return string.Format(formatProvider, this.ToString(format, formatProvider), Array.Empty<object>());
		}

		// Token: 0x06008070 RID: 32880 RVA: 0x0029C715 File Offset: 0x0029A915
		public override int GetHashCode()
		{
			return (int)((this.value * 3 / 2) ^ this.value);
		}

		// Token: 0x06008071 RID: 32881 RVA: 0x0029C728 File Offset: 0x0029A928
		public int CompareTo(Half value)
		{
			if (this < value)
			{
				return -1;
			}
			if (this > value)
			{
				return 1;
			}
			if (this != value)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(value))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06008072 RID: 32882 RVA: 0x0029C780 File Offset: 0x0029A980
		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is Half))
			{
				throw new ArgumentException("The argument value must be a SlimMath.Half.");
			}
			Half half = (Half)value;
			if (this < half)
			{
				return -1;
			}
			if (this > half)
			{
				return 1;
			}
			if (this != half)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(half))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06008073 RID: 32883 RVA: 0x0029C7F4 File Offset: 0x0029A9F4
		public static bool Equals(ref Half value1, ref Half value2)
		{
			return value1.value == value2.value;
		}

		// Token: 0x06008074 RID: 32884 RVA: 0x0029C804 File Offset: 0x0029AA04
		public bool Equals(Half other)
		{
			return other.value == this.value;
		}

		// Token: 0x06008075 RID: 32885 RVA: 0x0029C814 File Offset: 0x0029AA14
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.Equals((Half)obj);
		}

		// Token: 0x06008076 RID: 32886 RVA: 0x0029C846 File Offset: 0x0029AA46
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(typeof(Half));
		}

		// Token: 0x06008077 RID: 32887 RVA: 0x0029C857 File Offset: 0x0029AA57
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x06008078 RID: 32888 RVA: 0x0029C869 File Offset: 0x0029AA69
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x06008079 RID: 32889 RVA: 0x0029C87B File Offset: 0x0029AA7B
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.Char.");
		}

		// Token: 0x0600807A RID: 32890 RVA: 0x0029C887 File Offset: 0x0029AA87
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.DateTime.");
		}

		// Token: 0x0600807B RID: 32891 RVA: 0x0029C893 File Offset: 0x0029AA93
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x0600807C RID: 32892 RVA: 0x0029C8A5 File Offset: 0x0029AAA5
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x0600807D RID: 32893 RVA: 0x0029C8B7 File Offset: 0x0029AAB7
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x0600807E RID: 32894 RVA: 0x0029C8C9 File Offset: 0x0029AAC9
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x0600807F RID: 32895 RVA: 0x0029C8DB File Offset: 0x0029AADB
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x06008080 RID: 32896 RVA: 0x0029C8ED File Offset: 0x0029AAED
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x06008081 RID: 32897 RVA: 0x0029C8FF File Offset: 0x0029AAFF
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x06008082 RID: 32898 RVA: 0x0029C90C File Offset: 0x0029AB0C
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return ((IConvertible)this).ToType(type, provider);
		}

		// Token: 0x06008083 RID: 32899 RVA: 0x0029C926 File Offset: 0x0029AB26
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x06008084 RID: 32900 RVA: 0x0029C938 File Offset: 0x0029AB38
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x06008085 RID: 32901 RVA: 0x0029C94A File Offset: 0x0029AB4A
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		// Token: 0x04009190 RID: 37264
		private ushort value;

		// Token: 0x04009191 RID: 37265
		public const int PrecisionDigits = 3;

		// Token: 0x04009192 RID: 37266
		public const int MantissaBits = 11;

		// Token: 0x04009193 RID: 37267
		public const int MaximumDecimalExponent = 4;

		// Token: 0x04009194 RID: 37268
		public const int MaximumBinaryExponent = 15;

		// Token: 0x04009195 RID: 37269
		public const int MinimumDecimalExponent = -4;

		// Token: 0x04009196 RID: 37270
		public const int MinimumBinaryExponent = -14;

		// Token: 0x04009197 RID: 37271
		public const int ExponentRadix = 2;

		// Token: 0x04009198 RID: 37272
		public const int AdditionRounding = 1;

		// Token: 0x04009199 RID: 37273
		public static readonly Half Epsilon = new Half(0.0004887581f);

		// Token: 0x0400919A RID: 37274
		public static readonly Half MaxValue = new Half(65504f);

		// Token: 0x0400919B RID: 37275
		public static readonly Half MinValue = new Half(6.103516E-05f);

		// Token: 0x0400919C RID: 37276
		public static readonly Half NaN = new Half(float.NaN);

		// Token: 0x0400919D RID: 37277
		public static readonly Half NegativeInfinity = new Half(float.NegativeInfinity);

		// Token: 0x0400919E RID: 37278
		public static readonly Half PositiveInfinity = new Half(float.PositiveInfinity);
	}
}
