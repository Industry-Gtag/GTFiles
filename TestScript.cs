using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000D74 RID: 3444
[GTStripGameObjectFromBuild("!QATESTING")]
public class TestScript : MonoBehaviour
{
	// Token: 0x17000825 RID: 2085
	// (get) Token: 0x060054F4 RID: 21748 RVA: 0x00002076 File Offset: 0x00000276
	public int callbackOrder
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x17000826 RID: 2086
	// (get) Token: 0x060054F5 RID: 21749 RVA: 0x00002076 File Offset: 0x00000276
	public static bool IsUIOpen
	{
		get
		{
			return false;
		}
	}

	// Token: 0x040066AA RID: 26282
	public GameObject testDelete;
}
