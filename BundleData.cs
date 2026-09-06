using System;

// Token: 0x0200050A RID: 1290
[Serializable]
internal struct BundleData
{
	// Token: 0x04002B15 RID: 11029
	public string skuName;

	// Token: 0x04002B16 RID: 11030
	public string playFabItemName;

	// Token: 0x04002B17 RID: 11031
	public int shinyRocks;

	// Token: 0x04002B18 RID: 11032
	public int majorVersion;

	// Token: 0x04002B19 RID: 11033
	public int minorVersion;

	// Token: 0x04002B1A RID: 11034
	public int minorVersion2;

	// Token: 0x04002B1B RID: 11035
	public bool isActive;

	// Token: 0x04002B1C RID: 11036
	public string[] mothershipTransactionIds;

	// Token: 0x04002B1D RID: 11037
	public MothershipProgressionNodeRef[] progressionNodes;
}
