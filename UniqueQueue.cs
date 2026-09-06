using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000DF4 RID: 3572
public class UniqueQueue<T> : IEnumerable<T>, IEnumerable
{
	// Token: 0x17000850 RID: 2128
	// (get) Token: 0x06005789 RID: 22409 RVA: 0x001C90CB File Offset: 0x001C72CB
	public int Count
	{
		get
		{
			return this.queue.Count;
		}
	}

	// Token: 0x0600578A RID: 22410 RVA: 0x001C90D8 File Offset: 0x001C72D8
	public UniqueQueue()
	{
		this.queuedItems = new HashSet<T>();
		this.queue = new Queue<T>();
	}

	// Token: 0x0600578B RID: 22411 RVA: 0x001C90F6 File Offset: 0x001C72F6
	public UniqueQueue(int capacity)
	{
		this.queuedItems = new HashSet<T>(capacity);
		this.queue = new Queue<T>(capacity);
	}

	// Token: 0x0600578C RID: 22412 RVA: 0x001C9116 File Offset: 0x001C7316
	public void Clear()
	{
		this.queuedItems.Clear();
		this.queue.Clear();
	}

	// Token: 0x0600578D RID: 22413 RVA: 0x001C912E File Offset: 0x001C732E
	public bool Enqueue(T item)
	{
		if (!this.queuedItems.Add(item))
		{
			return false;
		}
		this.queue.Enqueue(item);
		return true;
	}

	// Token: 0x0600578E RID: 22414 RVA: 0x001C9150 File Offset: 0x001C7350
	public T Dequeue()
	{
		T t = this.queue.Dequeue();
		this.queuedItems.Remove(t);
		return t;
	}

	// Token: 0x0600578F RID: 22415 RVA: 0x001C9177 File Offset: 0x001C7377
	public bool TryDequeue(out T item)
	{
		if (this.queue.Count < 1)
		{
			item = default(T);
			return false;
		}
		item = this.Dequeue();
		return true;
	}

	// Token: 0x06005790 RID: 22416 RVA: 0x001C919D File Offset: 0x001C739D
	public T Peek()
	{
		return this.queue.Peek();
	}

	// Token: 0x06005791 RID: 22417 RVA: 0x001C91AA File Offset: 0x001C73AA
	public bool Contains(T item)
	{
		return this.queuedItems.Contains(item);
	}

	// Token: 0x06005792 RID: 22418 RVA: 0x001C91B8 File Offset: 0x001C73B8
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x06005793 RID: 22419 RVA: 0x001C91B8 File Offset: 0x001C73B8
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x0400681A RID: 26650
	private HashSet<T> queuedItems;

	// Token: 0x0400681B RID: 26651
	private Queue<T> queue;
}
