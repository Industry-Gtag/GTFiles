using System;
using UnityEngine;

// Token: 0x020001D5 RID: 469
public class SkeletonPathingNode : MonoBehaviour
{
	// Token: 0x06000C88 RID: 3208 RVA: 0x00044B04 File Offset: 0x00042D04
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000F2A RID: 3882
	public bool ejectionPoint;

	// Token: 0x04000F2B RID: 3883
	public SkeletonPathingNode[] connectedNodes;

	// Token: 0x04000F2C RID: 3884
	public float distanceToExitNode;
}
