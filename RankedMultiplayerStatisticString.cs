using System;
using UnityEngine;

// Token: 0x02000942 RID: 2370
[Serializable]
public class RankedMultiplayerStatisticString : RankedMultiplayerStatistic
{
	// Token: 0x06003E12 RID: 15890 RVA: 0x001502A3 File Offset: 0x0014E4A3
	public RankedMultiplayerStatisticString(string n, string val, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None)
		: base(n, s)
	{
		this.stringValue = val;
	}

	// Token: 0x06003E13 RID: 15891 RVA: 0x001502B4 File Offset: 0x0014E4B4
	public static implicit operator string(RankedMultiplayerStatisticString stat)
	{
		if (stat.IsValid)
		{
			return stat.stringValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return string.Empty;
	}

	// Token: 0x06003E14 RID: 15892 RVA: 0x001502DF File Offset: 0x0014E4DF
	public void Set(string val)
	{
		this.stringValue = val;
		this.Save();
	}

	// Token: 0x06003E15 RID: 15893 RVA: 0x001502EE File Offset: 0x0014E4EE
	public string Get()
	{
		return this.stringValue;
	}

	// Token: 0x06003E16 RID: 15894 RVA: 0x001502F6 File Offset: 0x0014E4F6
	public override bool TrySetValue(string valAsString)
	{
		this.stringValue = valAsString;
		return true;
	}

	// Token: 0x06003E17 RID: 15895 RVA: 0x00150300 File Offset: 0x0014E500
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetString(this.name, this.stringValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06003E18 RID: 15896 RVA: 0x00150334 File Offset: 0x0014E534
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.stringValue = PlayerPrefs.GetString(this.name, this.stringValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x06003E19 RID: 15897 RVA: 0x001502EE File Offset: 0x0014E4EE
	public override string ToString()
	{
		return this.stringValue;
	}

	// Token: 0x04004EB4 RID: 20148
	private string stringValue;
}
