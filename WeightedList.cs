using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000094 RID: 148
public class WeightedList<T>
{
	// Token: 0x17000043 RID: 67
	// (get) Token: 0x0600039E RID: 926 RVA: 0x00014FCA File Offset: 0x000131CA
	public int Count
	{
		get
		{
			return this.items.Count;
		}
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x0600039F RID: 927 RVA: 0x00014FD7 File Offset: 0x000131D7
	public List<T> Items
	{
		get
		{
			return this.items;
		}
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x00014FE0 File Offset: 0x000131E0
	public void Add(T item, float weight)
	{
		if (weight <= 0f)
		{
			throw new ArgumentException("Weight must be greater than zero.");
		}
		this.totalWeight += weight;
		this.items.Add(item);
		this.weights.Add(weight);
		this.cumulativeWeights.Add(this.totalWeight);
	}

	// Token: 0x17000045 RID: 69
	[TupleElementNames(new string[] { "Item", "Weight" })]
	public ValueTuple<T, float> this[int index]
	{
		[return: TupleElementNames(new string[] { "Item", "Weight" })]
		get
		{
			if (index < 0 || index >= this.items.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return new ValueTuple<T, float>(this.items[index], this.weights[index]);
		}
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x0001506E File Offset: 0x0001326E
	public T GetRandomItem()
	{
		return this.items[this.GetRandomIndex()];
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x00015084 File Offset: 0x00013284
	public int GetRandomIndex()
	{
		if (this.items.Count == 0)
		{
			throw new InvalidOperationException("The list is empty.");
		}
		float num = Random.value * this.totalWeight;
		int num2 = this.cumulativeWeights.BinarySearch(num);
		if (num2 < 0)
		{
			num2 = ~num2;
		}
		return num2;
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x000150CC File Offset: 0x000132CC
	public bool Remove(T item)
	{
		int num = this.items.IndexOf(item);
		if (num == -1)
		{
			return false;
		}
		this.RemoveAt(num);
		return true;
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x000150F4 File Offset: 0x000132F4
	public void RemoveAt(int index)
	{
		if (index < 0 || index >= this.items.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		this.totalWeight -= this.weights[index];
		this.items.RemoveAt(index);
		this.weights.RemoveAt(index);
		this.RecalculateCumulativeWeights();
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x00015158 File Offset: 0x00013358
	private void RecalculateCumulativeWeights()
	{
		this.cumulativeWeights.Clear();
		float num = 0f;
		foreach (float num2 in this.weights)
		{
			num += num2;
			this.cumulativeWeights.Add(num);
		}
		this.totalWeight = num;
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x000151CC File Offset: 0x000133CC
	public void Clear()
	{
		this.items.Clear();
		this.weights.Clear();
		this.cumulativeWeights.Clear();
		this.totalWeight = 0f;
	}

	// Token: 0x0400042B RID: 1067
	private List<T> items = new List<T>();

	// Token: 0x0400042C RID: 1068
	private List<float> weights = new List<float>();

	// Token: 0x0400042D RID: 1069
	private List<float> cumulativeWeights = new List<float>();

	// Token: 0x0400042E RID: 1070
	private float totalWeight;
}
