using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001458 RID: 5208
	[Serializable]
	public struct Bits32
	{
		// Token: 0x17000C9D RID: 3229
		// (get) Token: 0x06008345 RID: 33605 RVA: 0x002AFCDF File Offset: 0x002ADEDF
		public int IntValue
		{
			get
			{
				return this.m_bits;
			}
		}

		// Token: 0x06008346 RID: 33606 RVA: 0x002AFCE7 File Offset: 0x002ADEE7
		public Bits32(int bits = 0)
		{
			this.m_bits = bits;
		}

		// Token: 0x06008347 RID: 33607 RVA: 0x002AFCF0 File Offset: 0x002ADEF0
		public void Clear()
		{
			this.m_bits = 0;
		}

		// Token: 0x06008348 RID: 33608 RVA: 0x002AFCF9 File Offset: 0x002ADEF9
		public void SetBit(int index, bool value)
		{
			if (value)
			{
				this.m_bits |= 1 << index;
				return;
			}
			this.m_bits &= ~(1 << index);
		}

		// Token: 0x06008349 RID: 33609 RVA: 0x002AFD26 File Offset: 0x002ADF26
		public bool IsBitSet(int index)
		{
			return (this.m_bits & (1 << index)) != 0;
		}

		// Token: 0x0400945A RID: 37978
		[SerializeField]
		private int m_bits;
	}
}
