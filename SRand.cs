using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000B15 RID: 2837
[Serializable]
public struct SRand
{
	// Token: 0x0600489C RID: 18588 RVA: 0x00186333 File Offset: 0x00184533
	public SRand(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x0600489D RID: 18589 RVA: 0x00186333 File Offset: 0x00184533
	public SRand(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x0600489E RID: 18590 RVA: 0x00186348 File Offset: 0x00184548
	public SRand(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x0600489F RID: 18591 RVA: 0x00186362 File Offset: 0x00184562
	public SRand(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060048A0 RID: 18592 RVA: 0x0018637C File Offset: 0x0018457C
	public SRand(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060048A1 RID: 18593 RVA: 0x001863AE File Offset: 0x001845AE
	public SRand(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060048A2 RID: 18594 RVA: 0x001863DF File Offset: 0x001845DF
	public double NextDouble()
	{
		return this.NextState() % 268435457U * 3.725290298461914E-09;
	}

	// Token: 0x060048A3 RID: 18595 RVA: 0x001863F9 File Offset: 0x001845F9
	public double NextDouble(double max)
	{
		if (max < 0.0)
		{
			return 0.0;
		}
		return this.NextDouble() * max;
	}

	// Token: 0x060048A4 RID: 18596 RVA: 0x0018641C File Offset: 0x0018461C
	public double NextDouble(double min, double max)
	{
		double num = max - min;
		if (num <= 0.0)
		{
			return min;
		}
		double num2 = this.NextDouble() * num;
		return min + num2;
	}

	// Token: 0x060048A5 RID: 18597 RVA: 0x00186447 File Offset: 0x00184647
	public float NextFloat()
	{
		return (float)this.NextDouble();
	}

	// Token: 0x060048A6 RID: 18598 RVA: 0x00186450 File Offset: 0x00184650
	public float NextFloat(float max)
	{
		return (float)this.NextDouble((double)max);
	}

	// Token: 0x060048A7 RID: 18599 RVA: 0x0018645B File Offset: 0x0018465B
	public float NextFloat(float min, float max)
	{
		return (float)this.NextDouble((double)min, (double)max);
	}

	// Token: 0x060048A8 RID: 18600 RVA: 0x00186468 File Offset: 0x00184668
	public bool NextBool()
	{
		return this.NextState() % 2U == 1U;
	}

	// Token: 0x060048A9 RID: 18601 RVA: 0x00186475 File Offset: 0x00184675
	public uint NextUInt()
	{
		return this.NextState();
	}

	// Token: 0x060048AA RID: 18602 RVA: 0x00186475 File Offset: 0x00184675
	public int NextInt()
	{
		return (int)this.NextState();
	}

	// Token: 0x060048AB RID: 18603 RVA: 0x0018647D File Offset: 0x0018467D
	public int NextInt(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)((ulong)this.NextState() % (ulong)((long)max));
	}

	// Token: 0x060048AC RID: 18604 RVA: 0x00186490 File Offset: 0x00184690
	public int NextInt(int min, int max)
	{
		int num = max - min;
		if (num <= 0)
		{
			return min;
		}
		return min + this.NextInt(num);
	}

	// Token: 0x060048AD RID: 18605 RVA: 0x001864B0 File Offset: 0x001846B0
	public int NextIntWithExclusion(int min, int max, int exclude)
	{
		int num = max - min - 1;
		if (num <= 0)
		{
			return min;
		}
		int num2 = min + 1 + this.NextInt(num);
		if (num2 > exclude)
		{
			return num2;
		}
		return num2 - 1;
	}

	// Token: 0x060048AE RID: 18606 RVA: 0x001864E0 File Offset: 0x001846E0
	public int NextIntWithExclusion2(int min, int max, int exclude, int exclude2)
	{
		if (exclude == exclude2)
		{
			return this.NextIntWithExclusion(min, max, exclude);
		}
		int num = max - min - 2;
		if (num <= 0)
		{
			return min;
		}
		int num2 = min + 2 + this.NextInt(num);
		int num3;
		int num4;
		if (exclude >= exclude2)
		{
			num3 = exclude2 + 1;
			num4 = exclude;
		}
		else
		{
			num3 = exclude + 1;
			num4 = exclude2;
		}
		if (num2 <= num3)
		{
			return num2 - 2;
		}
		if (num2 <= num4)
		{
			return num2 - 1;
		}
		return num2;
	}

	// Token: 0x060048AF RID: 18607 RVA: 0x00186546 File Offset: 0x00184746
	public byte NextByte()
	{
		return (byte)(this.NextState() & 255U);
	}

	// Token: 0x060048B0 RID: 18608 RVA: 0x00186558 File Offset: 0x00184758
	public Color32 NextColor32()
	{
		byte b = this.NextByte();
		byte b2 = this.NextByte();
		byte b3 = this.NextByte();
		return new Color32(b, b2, b3, byte.MaxValue);
	}

	// Token: 0x060048B1 RID: 18609 RVA: 0x00186588 File Offset: 0x00184788
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 NextPointInsideSphere(float radius)
	{
		float num = this.NextFloat() * 2f - 1f;
		float num2 = this.NextFloat() * 2f - 1f;
		float num3 = this.NextFloat() * 2f - 1f;
		float num4 = MathF.Pow(this.NextFloat(), 0.33333334f);
		float num5 = 1f / MathF.Sqrt(num * num + num2 * num2 + num3 * num3);
		return new Vector3(num * num5 * num4 * radius, num2 * num5 * num4 * radius, num3 * num5 * num4 * radius);
	}

	// Token: 0x060048B2 RID: 18610 RVA: 0x00186614 File Offset: 0x00184814
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 NextPointOnSphere(float radius)
	{
		float num = this.NextFloat() * 2f - 1f;
		float num2 = this.NextFloat() * 2f - 1f;
		float num3 = this.NextFloat() * 2f - 1f;
		float num4 = 1f / MathF.Sqrt(num * num + num2 * num2 + num3 * num3);
		return new Vector3(num * num4 * radius, num2 * num4 * radius, num3 * num4 * radius);
	}

	// Token: 0x060048B3 RID: 18611 RVA: 0x00186688 File Offset: 0x00184888
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 NextPointInsideBox(Vector3 extents)
	{
		float num = this.NextFloat() - 0.5f;
		float num2 = this.NextFloat() - 0.5f;
		float num3 = this.NextFloat() - 0.5f;
		return new Vector3(num * extents.x, num2 * extents.y, num3 * extents.z);
	}

	// Token: 0x060048B4 RID: 18612 RVA: 0x001866D8 File Offset: 0x001848D8
	public Color NextColor()
	{
		float num = this.NextFloat();
		float num2 = this.NextFloat();
		float num3 = this.NextFloat();
		return new Color(num, num2, num3, 1f);
	}

	// Token: 0x060048B5 RID: 18613 RVA: 0x00186708 File Offset: 0x00184908
	public void Shuffle<T>(T[] array)
	{
		int i = array.Length;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			int num2 = i;
			int num3 = num;
			T t = array[num];
			T t2 = array[i];
			array[num2] = t;
			array[num3] = t2;
		}
	}

	// Token: 0x060048B6 RID: 18614 RVA: 0x00186758 File Offset: 0x00184958
	public void Shuffle<T>(List<T> list)
	{
		int i = list.Count;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			int num2 = i;
			int num3 = num;
			T t = list[num];
			T t2 = list[i];
			list[num2] = t;
			list[num3] = t2;
		}
	}

	// Token: 0x060048B7 RID: 18615 RVA: 0x001867B0 File Offset: 0x001849B0
	public void Reset()
	{
		this._state = this._seed;
	}

	// Token: 0x060048B8 RID: 18616 RVA: 0x00186333 File Offset: 0x00184533
	public void Reset(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x060048B9 RID: 18617 RVA: 0x00186333 File Offset: 0x00184533
	public void Reset(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x060048BA RID: 18618 RVA: 0x00186348 File Offset: 0x00184548
	public void Reset(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060048BB RID: 18619 RVA: 0x00186362 File Offset: 0x00184562
	public void Reset(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060048BC RID: 18620 RVA: 0x0018637C File Offset: 0x0018457C
	public void Reset(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060048BD RID: 18621 RVA: 0x001863AE File Offset: 0x001845AE
	public void Reset(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060048BE RID: 18622 RVA: 0x001867C0 File Offset: 0x001849C0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint NextState()
	{
		return this._state = this.Mix(this._state + 184402071U);
	}

	// Token: 0x060048BF RID: 18623 RVA: 0x001867E8 File Offset: 0x001849E8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint Mix(uint x)
	{
		x = ((x >> 17) ^ x) * 3982152891U;
		x = ((x >> 11) ^ x) * 2890668881U;
		x = ((x >> 15) ^ x) * 830770091U;
		x = (x >> 14) ^ x;
		return x;
	}

	// Token: 0x060048C0 RID: 18624 RVA: 0x0018681D File Offset: 0x00184A1D
	public override int GetHashCode()
	{
		return StaticHash.Compute((int)this._seed, (int)this._state);
	}

	// Token: 0x060048C1 RID: 18625 RVA: 0x00186830 File Offset: 0x00184A30
	public override string ToString()
	{
		return string.Format("{0} {{ {1}: {2:X8} {3}: {4:X8} }}", new object[] { "SRand", "_seed", this._seed, "_state", this._state });
	}

	// Token: 0x060048C2 RID: 18626 RVA: 0x00186881 File Offset: 0x00184A81
	public static SRand New()
	{
		return new SRand(DateTime.UtcNow);
	}

	// Token: 0x060048C3 RID: 18627 RVA: 0x0018688D File Offset: 0x00184A8D
	public static explicit operator SRand(int seed)
	{
		return new SRand(seed);
	}

	// Token: 0x060048C4 RID: 18628 RVA: 0x00186895 File Offset: 0x00184A95
	public static explicit operator SRand(uint seed)
	{
		return new SRand(seed);
	}

	// Token: 0x060048C5 RID: 18629 RVA: 0x0018689D File Offset: 0x00184A9D
	public static explicit operator SRand(long seed)
	{
		return new SRand(seed);
	}

	// Token: 0x060048C6 RID: 18630 RVA: 0x001868A5 File Offset: 0x00184AA5
	public static explicit operator SRand(string seed)
	{
		return new SRand(seed);
	}

	// Token: 0x060048C7 RID: 18631 RVA: 0x001868AD File Offset: 0x00184AAD
	public static explicit operator SRand(byte[] seed)
	{
		return new SRand(seed);
	}

	// Token: 0x060048C8 RID: 18632 RVA: 0x001868B5 File Offset: 0x00184AB5
	public static explicit operator SRand(DateTime seed)
	{
		return new SRand(seed);
	}

	// Token: 0x04005B1E RID: 23326
	[SerializeField]
	private uint _seed;

	// Token: 0x04005B1F RID: 23327
	[SerializeField]
	private uint _state;

	// Token: 0x04005B20 RID: 23328
	private const double MAX_AS_DOUBLE = 268435456.0;

	// Token: 0x04005B21 RID: 23329
	private const uint MAX_PLUS_ONE = 268435457U;

	// Token: 0x04005B22 RID: 23330
	private const double STEP_SIZE = 3.725290298461914E-09;

	// Token: 0x04005B23 RID: 23331
	private const float ONE_THIRD = 0.33333334f;
}
