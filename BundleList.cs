using System;

// Token: 0x0200050B RID: 1291
internal class BundleList
{
	// Token: 0x0600204C RID: 8268 RVA: 0x000ADB98 File Offset: 0x000ABD98
	public void FromJson(string jsonString)
	{
		this.data = JSonHelper.FromJson<BundleData>(jsonString);
		if (this.data.Length == 0)
		{
			return;
		}
		this.activeBundleIdx = 0;
		int num = this.data[0].majorVersion;
		int num2 = this.data[0].minorVersion;
		int num3 = this.data[0].minorVersion2;
		int gameMajorVersion = NetworkSystemConfig.GameMajorVersion;
		int gameMinorVersion = NetworkSystemConfig.GameMinorVersion;
		int gameMinorVersion2 = NetworkSystemConfig.GameMinorVersion2;
		for (int i = 1; i < this.data.Length; i++)
		{
			this.data[i].isActive = false;
			int num4 = gameMajorVersion * 1000000 + gameMinorVersion * 1000 + gameMinorVersion2;
			int num5 = this.data[i].majorVersion * 1000000 + this.data[i].minorVersion * 1000 + this.data[i].minorVersion2;
			if (num4 >= num5 && this.data[i].majorVersion >= num && this.data[i].minorVersion >= num2 && this.data[i].minorVersion2 >= num3)
			{
				this.activeBundleIdx = i;
				num = this.data[i].majorVersion;
				num2 = this.data[i].minorVersion;
				num3 = this.data[i].minorVersion2;
				break;
			}
		}
		this.data[this.activeBundleIdx].isActive = true;
	}

	// Token: 0x17000381 RID: 897
	// (get) Token: 0x0600204D RID: 8269 RVA: 0x000ADD35 File Offset: 0x000ABF35
	public bool IsLoaded
	{
		get
		{
			return this.data != null;
		}
	}

	// Token: 0x0600204E RID: 8270 RVA: 0x000ADD40 File Offset: 0x000ABF40
	public bool HasMothershipRewards(string playFabItemName)
	{
		if (this.data == null || string.IsNullOrEmpty(playFabItemName))
		{
			return false;
		}
		for (int i = 0; i < this.data.Length; i++)
		{
			if (!(this.data[i].playFabItemName != playFabItemName))
			{
				if (this.data[i].mothershipTransactionIds != null && this.data[i].mothershipTransactionIds.Length != 0)
				{
					return true;
				}
				if (this.data[i].progressionNodes != null && this.data[i].progressionNodes.Length != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600204F RID: 8271 RVA: 0x000ADDE0 File Offset: 0x000ABFE0
	public bool HasSku(string skuName, out int idx)
	{
		if (this.data == null)
		{
			idx = -1;
			return false;
		}
		for (int i = 0; i < this.data.Length; i++)
		{
			if (this.data[i].skuName == skuName)
			{
				idx = i;
				return true;
			}
		}
		idx = -1;
		return false;
	}

	// Token: 0x06002050 RID: 8272 RVA: 0x000ADE2F File Offset: 0x000AC02F
	public BundleData ActiveBundle()
	{
		return this.data[this.activeBundleIdx];
	}

	// Token: 0x04002B1E RID: 11038
	private int activeBundleIdx;

	// Token: 0x04002B1F RID: 11039
	public BundleData[] data;
}
