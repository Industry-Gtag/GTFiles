using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020003CF RID: 975
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST && !BETA")]
public class PerfTestGorillaHarness : MonoBehaviour
{
	// Token: 0x06001759 RID: 5977 RVA: 0x00087054 File Offset: 0x00085254
	private void Awake()
	{
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in base.GetComponentsInChildren<PerfTestGorillaSlot>())
		{
			if (perfTestGorillaSlot.slotType == PerfTestGorillaSlot.SlotType.VR_PLAYER)
			{
				this._vrSlot = perfTestGorillaSlot;
			}
			else
			{
				this.dummySlots.Add(perfTestGorillaSlot);
			}
		}
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x00087098 File Offset: 0x00085298
	private void Update()
	{
		if (!this._isRecording)
		{
			return;
		}
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in this.dummySlots)
		{
			float num = perfTestGorillaSlot.localStartPosition.y + Mathf.Sin(Time.time * this.bounceSpeed) * this.bounceAmplitude;
			perfTestGorillaSlot.transform.localPosition = new Vector3(perfTestGorillaSlot.localStartPosition.x, num, perfTestGorillaSlot.localStartPosition.z);
		}
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x0008713C File Offset: 0x0008533C
	public void StartRecording()
	{
		this._isRecording = true;
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x00087148 File Offset: 0x00085348
	public void StopRecording()
	{
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in this.dummySlots)
		{
			perfTestGorillaSlot.transform.localPosition = perfTestGorillaSlot.localStartPosition;
		}
		this._isRecording = false;
	}

	// Token: 0x04002299 RID: 8857
	public PerfTestGorillaSlot _vrSlot;

	// Token: 0x0400229A RID: 8858
	public List<PerfTestGorillaSlot> dummySlots = new List<PerfTestGorillaSlot>(19);

	// Token: 0x0400229B RID: 8859
	private bool _isRecording;

	// Token: 0x0400229C RID: 8860
	private float _nextRandomMoveTime;

	// Token: 0x0400229D RID: 8861
	private float bounceSpeed = 5f;

	// Token: 0x0400229E RID: 8862
	private float bounceAmplitude = 0.5f;
}
