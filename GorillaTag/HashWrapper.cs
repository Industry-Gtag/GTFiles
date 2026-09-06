using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011F7 RID: 4599
	[Serializable]
	public struct HashWrapper : IEquatable<int>
	{
		// Token: 0x060074B7 RID: 29879 RVA: 0x0025E91F File Offset: 0x0025CB1F
		public HashWrapper(int hash = -1)
		{
			this.hashCode = hash;
		}

		// Token: 0x060074B8 RID: 29880 RVA: 0x0025E928 File Offset: 0x0025CB28
		public override int GetHashCode()
		{
			return this.hashCode;
		}

		// Token: 0x060074B9 RID: 29881 RVA: 0x0025E930 File Offset: 0x0025CB30
		public override bool Equals(object obj)
		{
			return this.hashCode.Equals(obj);
		}

		// Token: 0x060074BA RID: 29882 RVA: 0x0025E93E File Offset: 0x0025CB3E
		public bool Equals(int i)
		{
			return this.hashCode.Equals(i);
		}

		// Token: 0x060074BB RID: 29883 RVA: 0x0025E928 File Offset: 0x0025CB28
		public static implicit operator int(in HashWrapper hash)
		{
			return hash.hashCode;
		}

		// Token: 0x04008469 RID: 33897
		[SerializeField]
		private int hashCode;

		// Token: 0x0400846A RID: 33898
		public const int NULL_HASH = -1;
	}
}
