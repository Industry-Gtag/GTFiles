using System;
using UnityEngine;

// Token: 0x020000BA RID: 186
[Serializable]
public struct KeyValueStringPair
{
	// Token: 0x06000486 RID: 1158 RVA: 0x00019D05 File Offset: 0x00017F05
	public KeyValueStringPair(string key, string value)
	{
		this.Key = key;
		this.Value = value;
	}

	// Token: 0x040004E5 RID: 1253
	public string Key;

	// Token: 0x040004E6 RID: 1254
	[Multiline]
	public string Value;
}
