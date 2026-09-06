using System;
using UnityEngine;

// Token: 0x02000368 RID: 872
[CreateAssetMenu(fileName = "PlatformTagJoin", menuName = "ScriptableObjects/PlatformTagJoin", order = 0)]
public class PlatformTagJoin : ScriptableObject
{
	// Token: 0x0600155D RID: 5469 RVA: 0x00071972 File Offset: 0x0006FB72
	public override string ToString()
	{
		return this.PlatformTag;
	}

	// Token: 0x04001A36 RID: 6710
	public string PlatformTag = "";
}
