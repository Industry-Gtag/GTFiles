using System;

namespace GorillaTag.Shared.Scripts.Utilities
{
	// Token: 0x02001293 RID: 4755
	public sealed class GTBitArray
	{
		// Token: 0x17000BAC RID: 2988
		public bool this[int idx]
		{
			get
			{
				if (idx < 0 || idx >= this.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				int num = idx / 32;
				int num2 = idx % 32;
				return ((ulong)this._data[num] & (ulong)(1L << (num2 & 31))) > 0UL;
			}
			set
			{
				if (idx < 0 || idx >= this.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				int num = idx / 32;
				int num2 = idx % 32;
				if (value)
				{
					this._data[num] |= 1U << num2;
					return;
				}
				this._data[num] &= ~(1U << num2);
			}
		}

		// Token: 0x060077B8 RID: 30648 RVA: 0x0026CA38 File Offset: 0x0026AC38
		public GTBitArray(int length)
		{
			this.Length = length;
			this._data = ((length % 32 == 0) ? new uint[length / 32] : new uint[length / 32 + 1]);
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = 0U;
			}
		}

		// Token: 0x060077B9 RID: 30649 RVA: 0x0026CA90 File Offset: 0x0026AC90
		public void Clear()
		{
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = 0U;
			}
		}

		// Token: 0x060077BA RID: 30650 RVA: 0x0026CABC File Offset: 0x0026ACBC
		public void CopyFrom(GTBitArray other)
		{
			if (this.Length != other.Length)
			{
				throw new ArgumentException("Can only copy bit arrays of the same length.");
			}
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = other._data[i];
			}
		}

		// Token: 0x040087FF RID: 34815
		public readonly int Length;

		// Token: 0x04008800 RID: 34816
		private readonly uint[] _data;
	}
}
