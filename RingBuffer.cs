using System;
using System.Collections.Generic;

// Token: 0x02000B14 RID: 2836
public class RingBuffer<T>
{
	// Token: 0x170006E2 RID: 1762
	// (get) Token: 0x0600488E RID: 18574 RVA: 0x00186116 File Offset: 0x00184316
	public int Size
	{
		get
		{
			return this._size;
		}
	}

	// Token: 0x170006E3 RID: 1763
	// (get) Token: 0x0600488F RID: 18575 RVA: 0x0018611E File Offset: 0x0018431E
	public int Capacity
	{
		get
		{
			return this._capacity;
		}
	}

	// Token: 0x170006E4 RID: 1764
	// (get) Token: 0x06004890 RID: 18576 RVA: 0x00186126 File Offset: 0x00184326
	public bool IsFull
	{
		get
		{
			return this._size == this._capacity;
		}
	}

	// Token: 0x170006E5 RID: 1765
	// (get) Token: 0x06004891 RID: 18577 RVA: 0x00186136 File Offset: 0x00184336
	public bool IsEmpty
	{
		get
		{
			return this._size == 0;
		}
	}

	// Token: 0x06004892 RID: 18578 RVA: 0x00186141 File Offset: 0x00184341
	public RingBuffer(int capacity)
	{
		if (capacity < 1)
		{
			throw new ArgumentException("Can't be zero or negative", "capacity");
		}
		this._size = 0;
		this._capacity = capacity;
		this._items = new T[capacity];
	}

	// Token: 0x06004893 RID: 18579 RVA: 0x00186177 File Offset: 0x00184377
	public RingBuffer(IList<T> list)
		: this(list.Count)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		list.CopyTo(this._items, 0);
	}

	// Token: 0x06004894 RID: 18580 RVA: 0x001861A0 File Offset: 0x001843A0
	public ref T PeekFirst()
	{
		return ref this._items[this._head];
	}

	// Token: 0x06004895 RID: 18581 RVA: 0x001861B3 File Offset: 0x001843B3
	public ref T PeekLast()
	{
		return ref this._items[this._tail];
	}

	// Token: 0x06004896 RID: 18582 RVA: 0x001861C8 File Offset: 0x001843C8
	public bool Push(T item)
	{
		if (this._size == this._capacity)
		{
			return false;
		}
		this._items[this._tail] = item;
		this._tail = (this._tail + 1) % this._capacity;
		this._size++;
		return true;
	}

	// Token: 0x06004897 RID: 18583 RVA: 0x0018621C File Offset: 0x0018441C
	public T Pop()
	{
		if (this._size == 0)
		{
			return default(T);
		}
		T t = this._items[this._head];
		this._head = (this._head + 1) % this._capacity;
		this._size--;
		return t;
	}

	// Token: 0x06004898 RID: 18584 RVA: 0x00186270 File Offset: 0x00184470
	public bool TryPop(out T item)
	{
		if (this._size == 0)
		{
			item = default(T);
			return false;
		}
		item = this._items[this._head];
		this._head = (this._head + 1) % this._capacity;
		this._size--;
		return true;
	}

	// Token: 0x06004899 RID: 18585 RVA: 0x001862C9 File Offset: 0x001844C9
	public void Clear()
	{
		this._head = 0;
		this._tail = 0;
		this._size = 0;
		Array.Clear(this._items, 0, this._capacity);
	}

	// Token: 0x0600489A RID: 18586 RVA: 0x001862F2 File Offset: 0x001844F2
	public bool TryGet(int i, out T item)
	{
		if (this._size == 0)
		{
			item = default(T);
			return false;
		}
		item = this._items[this._head + i % this._size];
		return true;
	}

	// Token: 0x0600489B RID: 18587 RVA: 0x00186326 File Offset: 0x00184526
	public ArraySegment<T> AsSegment()
	{
		return new ArraySegment<T>(this._items);
	}

	// Token: 0x04005B19 RID: 23321
	private T[] _items;

	// Token: 0x04005B1A RID: 23322
	private int _head;

	// Token: 0x04005B1B RID: 23323
	private int _tail;

	// Token: 0x04005B1C RID: 23324
	private int _size;

	// Token: 0x04005B1D RID: 23325
	private readonly int _capacity;
}
