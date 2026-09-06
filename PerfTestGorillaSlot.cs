using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020003D0 RID: 976
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST && !BETA")]
public class PerfTestGorillaSlot : MonoBehaviour
{
	// Token: 0x0600175E RID: 5982 RVA: 0x000871D7 File Offset: 0x000853D7
	private void Start()
	{
		this.localStartPosition = base.transform.localPosition;
	}

	// Token: 0x0400229F RID: 8863
	public PerfTestGorillaSlot.SlotType slotType;

	// Token: 0x040022A0 RID: 8864
	public Vector3 localStartPosition;

	// Token: 0x020003D1 RID: 977
	public enum SlotType
	{
		// Token: 0x040022A2 RID: 8866
		VR_PLAYER,
		// Token: 0x040022A3 RID: 8867
		DUMMY
	}
}
