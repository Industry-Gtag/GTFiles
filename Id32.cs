using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000B00 RID: 2816
[Serializable]
public struct Id32
{
	// Token: 0x0600481E RID: 18462 RVA: 0x00184BE0 File Offset: 0x00182DE0
	public Id32(string idString)
	{
		if (idString == null)
		{
			throw new ArgumentNullException("idString");
		}
		if (string.IsNullOrWhiteSpace(idString.Trim()))
		{
			throw new ArgumentNullException("idString");
		}
		this._id = XXHash32.Compute(idString, 0U);
	}

	// Token: 0x0600481F RID: 18463 RVA: 0x00184C15 File Offset: 0x00182E15
	public unsafe static implicit operator int(Id32 i32)
	{
		return *Unsafe.As<Id32, int>(ref i32);
	}

	// Token: 0x06004820 RID: 18464 RVA: 0x00184C1F File Offset: 0x00182E1F
	public static implicit operator Id32(string s)
	{
		return Id32.ComputeID(s);
	}

	// Token: 0x06004821 RID: 18465 RVA: 0x00184C28 File Offset: 0x00182E28
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Id32 ComputeID(string s)
	{
		int num = Id32.ComputeHash(s);
		return *Unsafe.As<int, Id32>(ref num);
	}

	// Token: 0x06004822 RID: 18466 RVA: 0x00184C48 File Offset: 0x00182E48
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeHash(string s)
	{
		if (s == null)
		{
			return 0;
		}
		s = s.Trim();
		if (string.IsNullOrWhiteSpace(s))
		{
			return 0;
		}
		return XXHash32.Compute(s, 0U);
	}

	// Token: 0x06004823 RID: 18467 RVA: 0x00184C68 File Offset: 0x00182E68
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06004824 RID: 18468 RVA: 0x00184C70 File Offset: 0x00182E70
	public override string ToString()
	{
		return string.Format("{{ {0} : {1} }}", "Id32", this._id);
	}

	// Token: 0x04005A94 RID: 23188
	[SerializeField]
	private int _id;
}
