using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009DB RID: 2523
public class RandomizeTest : MonoBehaviour
{
	// Token: 0x060040B8 RID: 16568 RVA: 0x00158BB8 File Offset: 0x00156DB8
	private void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			this.testList.Add(i);
		}
		for (int j = 0; j < 10; j++)
		{
			this.testListArray[j] = 0;
		}
		for (int k = 0; k < this.testList.Count; k++)
		{
			this.testListArray[k] = this.testList[k];
		}
		this.RandomizeList(ref this.testList);
		for (int l = 0; l < 10; l++)
		{
			this.testListArray[l] = 0;
		}
		for (int m = 0; m < this.testList.Count; m++)
		{
			this.testListArray[m] = this.testList[m];
		}
	}

	// Token: 0x060040B9 RID: 16569 RVA: 0x00158C70 File Offset: 0x00156E70
	public void RandomizeList(ref List<int> listToRandomize)
	{
		this.randomIterator = 0;
		while (this.randomIterator < listToRandomize.Count)
		{
			this.tempRandIndex = Random.Range(this.randomIterator, listToRandomize.Count);
			this.tempRandValue = listToRandomize[this.randomIterator];
			listToRandomize[this.randomIterator] = listToRandomize[this.tempRandIndex];
			listToRandomize[this.tempRandIndex] = this.tempRandValue;
			this.randomIterator++;
		}
	}

	// Token: 0x04005143 RID: 20803
	public List<int> testList = new List<int>();

	// Token: 0x04005144 RID: 20804
	public int[] testListArray = new int[10];

	// Token: 0x04005145 RID: 20805
	public int randomIterator;

	// Token: 0x04005146 RID: 20806
	public int tempRandIndex;

	// Token: 0x04005147 RID: 20807
	public int tempRandValue;
}
