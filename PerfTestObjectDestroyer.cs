using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020003D3 RID: 979
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST && !BETA")]
public class PerfTestObjectDestroyer : MonoBehaviour
{
	// Token: 0x06001761 RID: 5985 RVA: 0x000871EA File Offset: 0x000853EA
	private void Start()
	{
		Object.DestroyImmediate(base.gameObject, true);
	}
}
