using System;

// Token: 0x02000D94 RID: 3476
internal class CircularBuffer<T>
{
	// Token: 0x17000831 RID: 2097
	// (get) Token: 0x0600557E RID: 21886 RVA: 0x001BF977 File Offset: 0x001BDB77
	// (set) Token: 0x0600557F RID: 21887 RVA: 0x001BF97F File Offset: 0x001BDB7F
	public int Count { get; private set; }

	// Token: 0x17000832 RID: 2098
	// (get) Token: 0x06005580 RID: 21888 RVA: 0x001BF988 File Offset: 0x001BDB88
	// (set) Token: 0x06005581 RID: 21889 RVA: 0x001BF990 File Offset: 0x001BDB90
	public int Capacity { get; private set; }

	// Token: 0x06005582 RID: 21890 RVA: 0x001BF999 File Offset: 0x001BDB99
	public CircularBuffer(int capacity)
	{
		this.backingArray = new T[capacity];
		this.Capacity = capacity;
		this.Count = 0;
	}

	// Token: 0x06005583 RID: 21891 RVA: 0x001BF9BC File Offset: 0x001BDBBC
	public void Add(T value)
	{
		this.backingArray[this.nextWriteIdx] = value;
		this.lastWriteIdx = this.nextWriteIdx;
		this.nextWriteIdx = (this.nextWriteIdx + 1) % this.Capacity;
		if (this.Count < this.Capacity)
		{
			int count = this.Count;
			this.Count = count + 1;
		}
	}

	// Token: 0x06005584 RID: 21892 RVA: 0x001BFA1A File Offset: 0x001BDC1A
	public void Clear()
	{
		this.Count = 0;
	}

	// Token: 0x06005585 RID: 21893 RVA: 0x001BFA23 File Offset: 0x001BDC23
	public T Last()
	{
		return this.backingArray[this.lastWriteIdx];
	}

	// Token: 0x17000833 RID: 2099
	public T this[int logicalIdx]
	{
		get
		{
			if (logicalIdx < 0 || logicalIdx >= this.Count)
			{
				throw new ArgumentOutOfRangeException("logicalIdx", logicalIdx, string.Format("Out of bounds index {0} into CircularBuffer with length {1}", logicalIdx, this.Count));
			}
			int num = (this.lastWriteIdx + this.Capacity - logicalIdx) % this.Capacity;
			return this.backingArray[num];
		}
	}

	// Token: 0x040066F7 RID: 26359
	private T[] backingArray;

	// Token: 0x040066FA RID: 26362
	private int nextWriteIdx;

	// Token: 0x040066FB RID: 26363
	private int lastWriteIdx;
}
