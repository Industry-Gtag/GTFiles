using System;
using UnityEngine;

// Token: 0x02000D11 RID: 3345
public abstract class RandomContainer<T> : ScriptableObject
{
	// Token: 0x170007D4 RID: 2004
	// (get) Token: 0x06005311 RID: 21265 RVA: 0x001B6B5C File Offset: 0x001B4D5C
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x170007D5 RID: 2005
	// (get) Token: 0x06005312 RID: 21266 RVA: 0x001B6B64 File Offset: 0x001B4D64
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x06005313 RID: 21267 RVA: 0x001B6B6C File Offset: 0x001B4D6C
	public void ResetRandom(int? seedValue = null)
	{
		if (!this.staticSeed)
		{
			this._seed = seedValue ?? StaticHash.Compute(DateTime.UtcNow.Ticks);
		}
		else
		{
			this._seed = this.seed;
		}
		this._rnd = new SRand(this._seed);
	}

	// Token: 0x06005314 RID: 21268 RVA: 0x001B6BCC File Offset: 0x001B4DCC
	public void Reset()
	{
		this.ResetRandom(null);
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x06005315 RID: 21269 RVA: 0x001B6BFB File Offset: 0x001B4DFB
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x06005316 RID: 21270 RVA: 0x001B6C03 File Offset: 0x001B4E03
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x06005317 RID: 21271 RVA: 0x001B6C14 File Offset: 0x001B4E14
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		return t;
	}

	// Token: 0x040064B4 RID: 25780
	public T[] items = new T[0];

	// Token: 0x040064B5 RID: 25781
	public int seed;

	// Token: 0x040064B6 RID: 25782
	public bool staticSeed;

	// Token: 0x040064B7 RID: 25783
	public bool distinct = true;

	// Token: 0x040064B8 RID: 25784
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x040064B9 RID: 25785
	[NonSerialized]
	private T _lastItem;

	// Token: 0x040064BA RID: 25786
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x040064BB RID: 25787
	[NonSerialized]
	private SRand _rnd;
}
