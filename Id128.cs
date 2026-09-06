using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// Token: 0x02000AFE RID: 2814
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct Id128 : IEquatable<Id128>, IComparable<Id128>, IEquatable<Guid>, IEquatable<Hash128>
{
	// Token: 0x060047F5 RID: 18421 RVA: 0x00184698 File Offset: 0x00182898
	public Id128(int a, int b, int c, int d)
	{
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = (this.y = 0L);
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}

	// Token: 0x060047F6 RID: 18422 RVA: 0x001846EC File Offset: 0x001828EC
	public Id128(long x, long y)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = x;
		this.y = y;
	}

	// Token: 0x060047F7 RID: 18423 RVA: 0x00184740 File Offset: 0x00182940
	public Id128(Hash128 hash)
	{
		this.x = (this.y = 0L);
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = hash;
	}

	// Token: 0x060047F8 RID: 18424 RVA: 0x00184794 File Offset: 0x00182994
	public Id128(Guid guid)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = guid;
	}

	// Token: 0x060047F9 RID: 18425 RVA: 0x001847E8 File Offset: 0x001829E8
	public Id128(string guid)
	{
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = Guid.Parse(guid);
	}

	// Token: 0x060047FA RID: 18426 RVA: 0x00184854 File Offset: 0x00182A54
	public Id128(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (bytes.Length != 16)
		{
			throw new ArgumentException("Input buffer must be exactly 16 bytes", "bytes");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = new Guid(bytes);
	}

	// Token: 0x060047FB RID: 18427 RVA: 0x001848D1 File Offset: 0x00182AD1
	[return: TupleElementNames(new string[] { "l1", "l2" })]
	public ValueTuple<long, long> ToLongs()
	{
		return new ValueTuple<long, long>(this.x, this.y);
	}

	// Token: 0x060047FC RID: 18428 RVA: 0x001848E4 File Offset: 0x00182AE4
	[return: TupleElementNames(new string[] { "i1", "i2", "i3", "i4" })]
	public ValueTuple<int, int, int, int> ToInts()
	{
		return new ValueTuple<int, int, int, int>(this.a, this.b, this.c, this.d);
	}

	// Token: 0x060047FD RID: 18429 RVA: 0x00184903 File Offset: 0x00182B03
	public byte[] ToByteArray()
	{
		return this.guid.ToByteArray();
	}

	// Token: 0x060047FE RID: 18430 RVA: 0x00184910 File Offset: 0x00182B10
	public bool Equals(Id128 id)
	{
		return this.x == id.x && this.y == id.y;
	}

	// Token: 0x060047FF RID: 18431 RVA: 0x00184930 File Offset: 0x00182B30
	public bool Equals(Guid g)
	{
		return this.guid == g;
	}

	// Token: 0x06004800 RID: 18432 RVA: 0x0018493E File Offset: 0x00182B3E
	public bool Equals(Hash128 h)
	{
		return this.h128 == h;
	}

	// Token: 0x06004801 RID: 18433 RVA: 0x0018494C File Offset: 0x00182B4C
	public override bool Equals(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.Equals(id);
		}
		if (obj is Guid)
		{
			Guid guid = (Guid)obj;
			return this.Equals(guid);
		}
		if (obj is Hash128)
		{
			Hash128 hash = (Hash128)obj;
			return this.Equals(hash);
		}
		return false;
	}

	// Token: 0x06004802 RID: 18434 RVA: 0x0018499F File Offset: 0x00182B9F
	public override string ToString()
	{
		return this.guid.ToString();
	}

	// Token: 0x06004803 RID: 18435 RVA: 0x001849B2 File Offset: 0x00182BB2
	public override int GetHashCode()
	{
		return StaticHash.Compute(this.a, this.b, this.c, this.d);
	}

	// Token: 0x06004804 RID: 18436 RVA: 0x001849D4 File Offset: 0x00182BD4
	public int CompareTo(Id128 id)
	{
		int num = this.x.CompareTo(id.x);
		if (num == 0)
		{
			num = this.y.CompareTo(id.y);
		}
		return num;
	}

	// Token: 0x06004805 RID: 18437 RVA: 0x00184A0C File Offset: 0x00182C0C
	public int CompareTo(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.CompareTo(id);
		}
		if (obj is Guid)
		{
			Guid guid = (Guid)obj;
			return this.guid.CompareTo(guid);
		}
		if (obj is Hash128)
		{
			Hash128 hash = (Hash128)obj;
			return this.h128.CompareTo(hash);
		}
		throw new ArgumentException("Object must be of type Id128 or Guid");
	}

	// Token: 0x06004806 RID: 18438 RVA: 0x00184A72 File Offset: 0x00182C72
	public static Id128 NewId()
	{
		return new Id128(Guid.NewGuid());
	}

	// Token: 0x06004807 RID: 18439 RVA: 0x00184A80 File Offset: 0x00182C80
	public static Id128 ComputeMD5(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		Id128 id;
		using (MD5 md = MD5.Create())
		{
			id = new Guid(md.ComputeHash(Encoding.UTF8.GetBytes(s)));
		}
		return id;
	}

	// Token: 0x06004808 RID: 18440 RVA: 0x00184ADC File Offset: 0x00182CDC
	public static Id128 ComputeSHV2(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		return Hash128.Compute(s);
	}

	// Token: 0x06004809 RID: 18441 RVA: 0x00184AF7 File Offset: 0x00182CF7
	public static bool operator ==(Id128 j, Id128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x0600480A RID: 18442 RVA: 0x00184B01 File Offset: 0x00182D01
	public static bool operator !=(Id128 j, Id128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x0600480B RID: 18443 RVA: 0x00184B0E File Offset: 0x00182D0E
	public static bool operator ==(Id128 j, Guid k)
	{
		return j.Equals(k);
	}

	// Token: 0x0600480C RID: 18444 RVA: 0x00184B18 File Offset: 0x00182D18
	public static bool operator !=(Id128 j, Guid k)
	{
		return !j.Equals(k);
	}

	// Token: 0x0600480D RID: 18445 RVA: 0x00184B25 File Offset: 0x00182D25
	public static bool operator ==(Guid j, Id128 k)
	{
		return j.Equals(k.guid);
	}

	// Token: 0x0600480E RID: 18446 RVA: 0x00184B34 File Offset: 0x00182D34
	public static bool operator !=(Guid j, Id128 k)
	{
		return !j.Equals(k.guid);
	}

	// Token: 0x0600480F RID: 18447 RVA: 0x00184B46 File Offset: 0x00182D46
	public static bool operator ==(Id128 j, Hash128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x06004810 RID: 18448 RVA: 0x00184B50 File Offset: 0x00182D50
	public static bool operator !=(Id128 j, Hash128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06004811 RID: 18449 RVA: 0x00184B5D File Offset: 0x00182D5D
	public static bool operator ==(Hash128 j, Id128 k)
	{
		return j.Equals(k.h128);
	}

	// Token: 0x06004812 RID: 18450 RVA: 0x00184B6C File Offset: 0x00182D6C
	public static bool operator !=(Hash128 j, Id128 k)
	{
		return !j.Equals(k.h128);
	}

	// Token: 0x06004813 RID: 18451 RVA: 0x00184B7E File Offset: 0x00182D7E
	public static bool operator <(Id128 j, Id128 k)
	{
		return j.CompareTo(k) < 0;
	}

	// Token: 0x06004814 RID: 18452 RVA: 0x00184B8B File Offset: 0x00182D8B
	public static bool operator >(Id128 j, Id128 k)
	{
		return j.CompareTo(k) > 0;
	}

	// Token: 0x06004815 RID: 18453 RVA: 0x00184B98 File Offset: 0x00182D98
	public static bool operator <=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) <= 0;
	}

	// Token: 0x06004816 RID: 18454 RVA: 0x00184BA8 File Offset: 0x00182DA8
	public static bool operator >=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) >= 0;
	}

	// Token: 0x06004817 RID: 18455 RVA: 0x00184BB8 File Offset: 0x00182DB8
	public static implicit operator Guid(Id128 id)
	{
		return id.guid;
	}

	// Token: 0x06004818 RID: 18456 RVA: 0x00184BC0 File Offset: 0x00182DC0
	public static implicit operator Id128(Guid guid)
	{
		return new Id128(guid);
	}

	// Token: 0x06004819 RID: 18457 RVA: 0x00184BC8 File Offset: 0x00182DC8
	public static implicit operator Id128(Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x0600481A RID: 18458 RVA: 0x00184BD0 File Offset: 0x00182DD0
	public static implicit operator Hash128(Id128 id)
	{
		return id.h128;
	}

	// Token: 0x0600481B RID: 18459 RVA: 0x00184BD8 File Offset: 0x00182DD8
	public static explicit operator Id128(string s)
	{
		return Id128.ComputeMD5(s);
	}

	// Token: 0x04005A8B RID: 23179
	[SerializeField]
	[FieldOffset(0)]
	public long x;

	// Token: 0x04005A8C RID: 23180
	[SerializeField]
	[FieldOffset(8)]
	public long y;

	// Token: 0x04005A8D RID: 23181
	[NonSerialized]
	[FieldOffset(0)]
	public int a;

	// Token: 0x04005A8E RID: 23182
	[NonSerialized]
	[FieldOffset(4)]
	public int b;

	// Token: 0x04005A8F RID: 23183
	[NonSerialized]
	[FieldOffset(8)]
	public int c;

	// Token: 0x04005A90 RID: 23184
	[NonSerialized]
	[FieldOffset(12)]
	public int d;

	// Token: 0x04005A91 RID: 23185
	[NonSerialized]
	[FieldOffset(0)]
	public Guid guid;

	// Token: 0x04005A92 RID: 23186
	[NonSerialized]
	[FieldOffset(0)]
	public Hash128 h128;

	// Token: 0x04005A93 RID: 23187
	public static readonly Id128 Empty;
}
