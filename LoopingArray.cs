using System;
using GorillaTag;

// Token: 0x02000DB5 RID: 3509
public class LoopingArray<T> : ObjectPoolEvents
{
	// Token: 0x1700083C RID: 2108
	// (get) Token: 0x0600561C RID: 22044 RVA: 0x001C2450 File Offset: 0x001C0650
	public int Length
	{
		get
		{
			return this.m_length;
		}
	}

	// Token: 0x1700083D RID: 2109
	// (get) Token: 0x0600561D RID: 22045 RVA: 0x001C2458 File Offset: 0x001C0658
	public int CurrentIndex
	{
		get
		{
			return this.m_currentIndex;
		}
	}

	// Token: 0x1700083E RID: 2110
	public T this[int index]
	{
		get
		{
			return this.m_array[index];
		}
		set
		{
			this.m_array[index] = value;
		}
	}

	// Token: 0x06005620 RID: 22048 RVA: 0x001C247D File Offset: 0x001C067D
	public LoopingArray()
		: this(0)
	{
	}

	// Token: 0x06005621 RID: 22049 RVA: 0x001C2486 File Offset: 0x001C0686
	public LoopingArray(int capicity)
	{
		this.m_length = capicity;
		this.m_array = new T[capicity];
		this.Clear();
	}

	// Token: 0x06005622 RID: 22050 RVA: 0x001C24A7 File Offset: 0x001C06A7
	public int AddAndIncrement(in T value)
	{
		int currentIndex = this.m_currentIndex;
		this.m_array[this.m_currentIndex] = value;
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		return currentIndex;
	}

	// Token: 0x06005623 RID: 22051 RVA: 0x001C24DB File Offset: 0x001C06DB
	public int IncrementAndAdd(in T value)
	{
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		this.m_array[this.m_currentIndex] = value;
		return this.m_currentIndex;
	}

	// Token: 0x06005624 RID: 22052 RVA: 0x001C2510 File Offset: 0x001C0710
	public void Clear()
	{
		this.m_currentIndex = 0;
		for (int i = 0; i < this.m_array.Length; i++)
		{
			this.m_array[i] = default(T);
		}
	}

	// Token: 0x06005625 RID: 22053 RVA: 0x001C254C File Offset: 0x001C074C
	void ObjectPoolEvents.OnTaken()
	{
		this.Clear();
	}

	// Token: 0x06005626 RID: 22054 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ObjectPoolEvents.OnReturned()
	{
	}

	// Token: 0x0400676B RID: 26475
	private int m_length;

	// Token: 0x0400676C RID: 26476
	private int m_currentIndex;

	// Token: 0x0400676D RID: 26477
	private T[] m_array;

	// Token: 0x02000DB6 RID: 3510
	public class Pool : ObjectPool<LoopingArray<T>>
	{
		// Token: 0x06005627 RID: 22055 RVA: 0x001C2554 File Offset: 0x001C0754
		private Pool(int amount)
			: base(amount)
		{
		}

		// Token: 0x06005628 RID: 22056 RVA: 0x001C255D File Offset: 0x001C075D
		public Pool(int size, int amount)
			: this(size, amount, amount)
		{
		}

		// Token: 0x06005629 RID: 22057 RVA: 0x001C2568 File Offset: 0x001C0768
		public Pool(int size, int initialAmount, int maxAmount)
		{
			this.m_size = size;
			base.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x0600562A RID: 22058 RVA: 0x001C257F File Offset: 0x001C077F
		public override LoopingArray<T> CreateInstance()
		{
			return new LoopingArray<T>(this.m_size);
		}

		// Token: 0x0400676E RID: 26478
		private readonly int m_size;
	}
}
