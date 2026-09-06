using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000D10 RID: 3344
public abstract class RandomComponent<T> : MonoBehaviour
{
	// Token: 0x170007D2 RID: 2002
	// (get) Token: 0x06005308 RID: 21256 RVA: 0x001B6A01 File Offset: 0x001B4C01
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x170007D3 RID: 2003
	// (get) Token: 0x06005309 RID: 21257 RVA: 0x001B6A09 File Offset: 0x001B4C09
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x0600530A RID: 21258 RVA: 0x001B6A14 File Offset: 0x001B4C14
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

	// Token: 0x0600530B RID: 21259 RVA: 0x001B6A74 File Offset: 0x001B4C74
	public void Reset()
	{
		this.ResetRandom(null);
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x0600530C RID: 21260 RVA: 0x001B6AA3 File Offset: 0x001B4CA3
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x0600530D RID: 21261 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnNextItem(T item)
	{
	}

	// Token: 0x0600530E RID: 21262 RVA: 0x001B6AAB File Offset: 0x001B4CAB
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x0600530F RID: 21263 RVA: 0x001B6ABC File Offset: 0x001B4CBC
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		this.OnNextItem(t);
		UnityEvent<T> unityEvent = this.onNextItem;
		if (unityEvent != null)
		{
			unityEvent.Invoke(t);
		}
		return t;
	}

	// Token: 0x040064AB RID: 25771
	public T[] items = new T[0];

	// Token: 0x040064AC RID: 25772
	public int seed;

	// Token: 0x040064AD RID: 25773
	public bool staticSeed;

	// Token: 0x040064AE RID: 25774
	public bool distinct = true;

	// Token: 0x040064AF RID: 25775
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x040064B0 RID: 25776
	[NonSerialized]
	private T _lastItem;

	// Token: 0x040064B1 RID: 25777
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x040064B2 RID: 25778
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x040064B3 RID: 25779
	public UnityEvent<T> onNextItem;
}
