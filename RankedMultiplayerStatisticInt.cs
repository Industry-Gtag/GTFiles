using System;
using UnityEngine;

// Token: 0x02000940 RID: 2368
[Serializable]
public class RankedMultiplayerStatisticInt : RankedMultiplayerStatistic
{
	// Token: 0x06003DFE RID: 15870 RVA: 0x0014FFD9 File Offset: 0x0014E1D9
	public RankedMultiplayerStatisticInt(string n, int val, int min = 0, int max = 2147483647, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None)
		: base(n, s)
	{
		this.intValue = val;
		this.minValue = min;
		this.maxValue = max;
	}

	// Token: 0x06003DFF RID: 15871 RVA: 0x0014FFFA File Offset: 0x0014E1FA
	public static implicit operator int(RankedMultiplayerStatisticInt stat)
	{
		if (stat.IsValid)
		{
			return stat.intValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return 0;
	}

	// Token: 0x06003E00 RID: 15872 RVA: 0x00150021 File Offset: 0x0014E221
	public void Set(int val)
	{
		this.intValue = Mathf.Clamp(val, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003E01 RID: 15873 RVA: 0x00150041 File Offset: 0x0014E241
	public int Get()
	{
		return this.intValue;
	}

	// Token: 0x06003E02 RID: 15874 RVA: 0x0015004C File Offset: 0x0014E24C
	public override bool TrySetValue(string valAsString)
	{
		int num;
		bool flag = int.TryParse(valAsString, out num);
		if (flag)
		{
			this.intValue = Mathf.Clamp(num, this.minValue, this.maxValue);
		}
		return flag;
	}

	// Token: 0x06003E03 RID: 15875 RVA: 0x0015007C File Offset: 0x0014E27C
	public void Increment()
	{
		this.AddTo(1);
	}

	// Token: 0x06003E04 RID: 15876 RVA: 0x00150085 File Offset: 0x0014E285
	public void AddTo(int amount)
	{
		this.intValue += amount;
		this.intValue = Mathf.Clamp(this.intValue, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003E05 RID: 15877 RVA: 0x001500B8 File Offset: 0x0014E2B8
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetInt(this.name, this.intValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06003E06 RID: 15878 RVA: 0x001500EC File Offset: 0x0014E2EC
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.intValue = PlayerPrefs.GetInt(this.name, this.intValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x06003E07 RID: 15879 RVA: 0x0015012E File Offset: 0x0014E32E
	public override string ToString()
	{
		return this.intValue.ToString();
	}

	// Token: 0x04004EAE RID: 20142
	private int intValue;

	// Token: 0x04004EAF RID: 20143
	private int minValue;

	// Token: 0x04004EB0 RID: 20144
	private int maxValue;
}
