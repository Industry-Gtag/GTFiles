using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x020013DA RID: 5082
	[Serializable]
	public abstract class LiteCrusher
	{
		// Token: 0x06008021 RID: 32801 RVA: 0x0029BB04 File Offset: 0x00299D04
		public static int GetBitsForMaxValue(uint maxvalue)
		{
			for (int i = 0; i < 32; i++)
			{
				if (maxvalue >> i == 0U)
				{
					return i;
				}
			}
			return 32;
		}

		// Token: 0x0400915E RID: 37214
		[SerializeField]
		protected int bits;
	}
}
