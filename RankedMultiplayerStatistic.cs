using System;

// Token: 0x0200093E RID: 2366
public abstract class RankedMultiplayerStatistic
{
	// Token: 0x06003DF2 RID: 15858 RVA: 0x0005B805 File Offset: 0x00059A05
	public override string ToString()
	{
		return string.Empty;
	}

	// Token: 0x06003DF3 RID: 15859
	public abstract void Load();

	// Token: 0x06003DF4 RID: 15860
	protected abstract void Save();

	// Token: 0x06003DF5 RID: 15861
	public abstract bool TrySetValue(string valAsString);

	// Token: 0x06003DF6 RID: 15862 RVA: 0x0014FF07 File Offset: 0x0014E107
	public virtual string WriteToJson()
	{
		return string.Format("{{{0}:\"{1}\"}}", this.name, this.ToString());
	}

	// Token: 0x170005B2 RID: 1458
	// (get) Token: 0x06003DF7 RID: 15863 RVA: 0x0014FF1F File Offset: 0x0014E11F
	// (set) Token: 0x06003DF8 RID: 15864 RVA: 0x0014FF27 File Offset: 0x0014E127
	public bool IsValid { get; protected set; }

	// Token: 0x06003DF9 RID: 15865 RVA: 0x0014FF30 File Offset: 0x0014E130
	public RankedMultiplayerStatistic(string n, RankedMultiplayerStatistic.SerializationType sType = RankedMultiplayerStatistic.SerializationType.Mothership)
	{
		this.serializationType = sType;
		this.name = n;
		this.IsValid = this.serializationType != RankedMultiplayerStatistic.SerializationType.Mothership;
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
	}

	// Token: 0x06003DFA RID: 15866 RVA: 0x0014FF68 File Offset: 0x0014E168
	protected virtual void HandleUserDataSetSuccess(string keyName)
	{
		if (keyName == this.name)
		{
			this.IsValid = true;
		}
	}

	// Token: 0x06003DFB RID: 15867 RVA: 0x0014FF7F File Offset: 0x0014E17F
	protected virtual void HandleUserDataGetSuccess(string keyName, string value)
	{
		if (keyName == this.name)
		{
			if (this.TrySetValue(value))
			{
				this.IsValid = true;
				return;
			}
			this.Save();
		}
	}

	// Token: 0x06003DFC RID: 15868 RVA: 0x0014FFA6 File Offset: 0x0014E1A6
	protected void HandleUserDataGetFailure(string keyName)
	{
		if (keyName == this.name)
		{
			this.Save();
			this.IsValid = true;
		}
	}

	// Token: 0x06003DFD RID: 15869 RVA: 0x0014FFC3 File Offset: 0x0014E1C3
	protected void HandleUserDataSetFailure(string keyName)
	{
		if (keyName == this.name)
		{
			this.Save();
		}
	}

	// Token: 0x04004EA7 RID: 20135
	protected RankedMultiplayerStatistic.SerializationType serializationType = RankedMultiplayerStatistic.SerializationType.Mothership;

	// Token: 0x04004EA8 RID: 20136
	public string name;

	// Token: 0x0200093F RID: 2367
	public enum SerializationType
	{
		// Token: 0x04004EAB RID: 20139
		None,
		// Token: 0x04004EAC RID: 20140
		Mothership,
		// Token: 0x04004EAD RID: 20141
		PlayerPrefs
	}
}
