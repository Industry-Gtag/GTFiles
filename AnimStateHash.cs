using System;
using UnityEngine;

// Token: 0x02000AE6 RID: 2790
[Serializable]
public struct AnimStateHash
{
	// Token: 0x060047A8 RID: 18344 RVA: 0x00183508 File Offset: 0x00181708
	public static implicit operator AnimStateHash(string s)
	{
		return new AnimStateHash
		{
			_hash = Animator.StringToHash(s)
		};
	}

	// Token: 0x060047A9 RID: 18345 RVA: 0x0018352B File Offset: 0x0018172B
	public static implicit operator int(AnimStateHash ash)
	{
		return ash._hash;
	}

	// Token: 0x04005A32 RID: 23090
	[SerializeField]
	private int _hash;
}
