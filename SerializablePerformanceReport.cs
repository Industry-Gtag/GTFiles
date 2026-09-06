using System;
using System.Collections.Generic;

// Token: 0x020003CC RID: 972
[Serializable]
public class SerializablePerformanceReport<T>
{
	// Token: 0x0400228D RID: 8845
	public string reportDate;

	// Token: 0x0400228E RID: 8846
	public string version;

	// Token: 0x0400228F RID: 8847
	public List<T> results;
}
