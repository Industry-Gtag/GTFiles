using System;
using UnityEngine;

// Token: 0x02000941 RID: 2369
[Serializable]
public class RankedMultiplayerStatisticFloat : RankedMultiplayerStatistic
{
	// Token: 0x06003E08 RID: 15880 RVA: 0x0015013B File Offset: 0x0014E33B
	public RankedMultiplayerStatisticFloat(string n, float val, float min = 0f, float max = 3.4028235E+38f, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None)
		: base(n, s)
	{
		this.floatValue = val;
		this.minValue = min;
		this.maxValue = max;
	}

	// Token: 0x06003E09 RID: 15881 RVA: 0x0015015C File Offset: 0x0014E35C
	public static implicit operator float(RankedMultiplayerStatisticFloat stat)
	{
		if (stat.IsValid)
		{
			return stat.floatValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return 0f;
	}

	// Token: 0x06003E0A RID: 15882 RVA: 0x00150187 File Offset: 0x0014E387
	public void Set(float val)
	{
		this.floatValue = Mathf.Clamp(val, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003E0B RID: 15883 RVA: 0x001501A7 File Offset: 0x0014E3A7
	public float Get()
	{
		return this.floatValue;
	}

	// Token: 0x06003E0C RID: 15884 RVA: 0x001501B0 File Offset: 0x0014E3B0
	public override bool TrySetValue(string valAsString)
	{
		float num;
		bool flag = float.TryParse(valAsString, out num);
		if (flag)
		{
			this.floatValue = Mathf.Clamp(num, this.minValue, this.maxValue);
		}
		return flag;
	}

	// Token: 0x06003E0D RID: 15885 RVA: 0x001501E0 File Offset: 0x0014E3E0
	public void Increment()
	{
		this.AddTo(1f);
	}

	// Token: 0x06003E0E RID: 15886 RVA: 0x001501ED File Offset: 0x0014E3ED
	public void AddTo(float amount)
	{
		this.floatValue += amount;
		this.floatValue = Mathf.Clamp(this.floatValue, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003E0F RID: 15887 RVA: 0x00150220 File Offset: 0x0014E420
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetFloat(this.name, this.floatValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06003E10 RID: 15888 RVA: 0x00150254 File Offset: 0x0014E454
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.floatValue = PlayerPrefs.GetFloat(this.name, this.floatValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x06003E11 RID: 15889 RVA: 0x00150296 File Offset: 0x0014E496
	public override string ToString()
	{
		return this.floatValue.ToString();
	}

	// Token: 0x04004EB1 RID: 20145
	private float floatValue;

	// Token: 0x04004EB2 RID: 20146
	private float minValue;

	// Token: 0x04004EB3 RID: 20147
	private float maxValue;
}
