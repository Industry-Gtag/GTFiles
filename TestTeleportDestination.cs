using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020003D4 RID: 980
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST && !BETA")]
public class TestTeleportDestination : MonoBehaviour
{
	// Token: 0x06001763 RID: 5987 RVA: 0x000871F8 File Offset: 0x000853F8
	private void OnDrawGizmosSelected()
	{
		Debug.DrawRay(base.transform.position, base.transform.forward * 2f, Color.magenta);
	}

	// Token: 0x040022A4 RID: 8868
	public GTZone[] zones;

	// Token: 0x040022A5 RID: 8869
	public GameObject teleportTransform;
}
