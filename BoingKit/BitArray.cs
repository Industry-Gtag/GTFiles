using System;

namespace BoingKit
{
	// Token: 0x02001459 RID: 5209
	public struct BitArray
	{
		// Token: 0x17000C9E RID: 3230
		// (get) Token: 0x0600834A RID: 33610 RVA: 0x002AFD38 File Offset: 0x002ADF38
		public int[] Blocks
		{
			get
			{
				return this.m_aBlock;
			}
		}

		// Token: 0x0600834B RID: 33611 RVA: 0x002AFD40 File Offset: 0x002ADF40
		private static int GetBlockIndex(int index)
		{
			return index / 4;
		}

		// Token: 0x0600834C RID: 33612 RVA: 0x002AFD45 File Offset: 0x002ADF45
		private static int GetSubIndex(int index)
		{
			return index % 4;
		}

		// Token: 0x0600834D RID: 33613 RVA: 0x002AFD4C File Offset: 0x002ADF4C
		private static void SetBit(int index, bool value, int[] blocks)
		{
			int blockIndex = BitArray.GetBlockIndex(index);
			int subIndex = BitArray.GetSubIndex(index);
			if (value)
			{
				blocks[blockIndex] |= 1 << subIndex;
				return;
			}
			blocks[blockIndex] &= ~(1 << subIndex);
		}

		// Token: 0x0600834E RID: 33614 RVA: 0x002AFD8E File Offset: 0x002ADF8E
		private static bool IsBitSet(int index, int[] blocks)
		{
			return (blocks[BitArray.GetBlockIndex(index)] & (1 << BitArray.GetSubIndex(index))) != 0;
		}

		// Token: 0x0600834F RID: 33615 RVA: 0x002AFDA8 File Offset: 0x002ADFA8
		public BitArray(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			this.m_aBlock = new int[num];
			this.Clear();
		}

		// Token: 0x06008350 RID: 33616 RVA: 0x002AFDD0 File Offset: 0x002ADFD0
		public void Resize(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			if (num <= this.m_aBlock.Length)
			{
				return;
			}
			int[] array = new int[num];
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				array[i] = this.m_aBlock[i];
				i++;
			}
			this.m_aBlock = array;
		}

		// Token: 0x06008351 RID: 33617 RVA: 0x002AFE1F File Offset: 0x002AE01F
		public void Clear()
		{
			this.SetAllBits(false);
		}

		// Token: 0x06008352 RID: 33618 RVA: 0x002AFE28 File Offset: 0x002AE028
		public void SetAllBits(bool value)
		{
			int num = (value ? (-1) : 1);
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				this.m_aBlock[i] = num;
				i++;
			}
		}

		// Token: 0x06008353 RID: 33619 RVA: 0x002AFE5B File Offset: 0x002AE05B
		public void SetBit(int index, bool value)
		{
			BitArray.SetBit(index, value, this.m_aBlock);
		}

		// Token: 0x06008354 RID: 33620 RVA: 0x002AFE6A File Offset: 0x002AE06A
		public bool IsBitSet(int index)
		{
			return BitArray.IsBitSet(index, this.m_aBlock);
		}

		// Token: 0x0400945B RID: 37979
		private int[] m_aBlock;
	}
}
