using System;
using UnityEngine;

// Token: 0x020006A4 RID: 1700
public class FortuneResults : ScriptableObject
{
	// Token: 0x06002A6E RID: 10862 RVA: 0x000E4F1C File Offset: 0x000E311C
	private void OnValidate()
	{
		this.totalChance = 0f;
		for (int i = 0; i < this.fortuneResults.Length; i++)
		{
			this.totalChance += this.fortuneResults[i].weightedChance;
		}
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x000E4F68 File Offset: 0x000E3168
	public FortuneResults.FortuneResult GetResult()
	{
		float num = Random.Range(0f, this.totalChance);
		int i = 0;
		while (i < this.fortuneResults.Length)
		{
			FortuneResults.FortuneCategory fortuneCategory = this.fortuneResults[i];
			if (num <= fortuneCategory.weightedChance)
			{
				if (fortuneCategory.textResults.Length == 0)
				{
					return new FortuneResults.FortuneResult(FortuneResults.FortuneCategoryType.Invalid, -1);
				}
				int num2 = Random.Range(0, fortuneCategory.textResults.Length);
				return new FortuneResults.FortuneResult(fortuneCategory.fortuneType, num2);
			}
			else
			{
				num -= fortuneCategory.weightedChance;
				i++;
			}
		}
		return new FortuneResults.FortuneResult(FortuneResults.FortuneCategoryType.Invalid, -1);
	}

	// Token: 0x06002A70 RID: 10864 RVA: 0x000E4FEC File Offset: 0x000E31EC
	public string GetResultText(FortuneResults.FortuneResult result)
	{
		for (int i = 0; i < this.fortuneResults.Length; i++)
		{
			if (this.fortuneResults[i].fortuneType == result.fortuneType && result.resultIndex >= 0 && result.resultIndex < this.fortuneResults[i].textResults.Length)
			{
				return this.fortuneResults[i].textResults[result.resultIndex];
			}
		}
		return "!! Invalid Fortune !!";
	}

	// Token: 0x0400373A RID: 14138
	[SerializeField]
	private FortuneResults.FortuneCategory[] fortuneResults;

	// Token: 0x0400373B RID: 14139
	[SerializeField]
	private float totalChance;

	// Token: 0x020006A5 RID: 1701
	public enum FortuneCategoryType
	{
		// Token: 0x0400373D RID: 14141
		Invalid,
		// Token: 0x0400373E RID: 14142
		Positive,
		// Token: 0x0400373F RID: 14143
		Neutral,
		// Token: 0x04003740 RID: 14144
		Negative,
		// Token: 0x04003741 RID: 14145
		Seasonal
	}

	// Token: 0x020006A6 RID: 1702
	[Serializable]
	public struct FortuneCategory
	{
		// Token: 0x04003742 RID: 14146
		public FortuneResults.FortuneCategoryType fortuneType;

		// Token: 0x04003743 RID: 14147
		public float weightedChance;

		// Token: 0x04003744 RID: 14148
		public string[] textResults;
	}

	// Token: 0x020006A7 RID: 1703
	public struct FortuneResult
	{
		// Token: 0x06002A72 RID: 10866 RVA: 0x000E5067 File Offset: 0x000E3267
		public FortuneResult(FortuneResults.FortuneCategoryType fortuneType, int resultIndex)
		{
			this.fortuneType = fortuneType;
			this.resultIndex = resultIndex;
		}

		// Token: 0x04003745 RID: 14149
		public FortuneResults.FortuneCategoryType fortuneType;

		// Token: 0x04003746 RID: 14150
		public int resultIndex;
	}
}
