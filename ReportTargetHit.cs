using System;
using NetSynchrony;
using UnityEngine;

// Token: 0x02000E42 RID: 3650
public class ReportTargetHit : MonoBehaviour
{
	// Token: 0x0600592C RID: 22828 RVA: 0x001CF8E4 File Offset: 0x001CDAE4
	private void Start()
	{
		this.seekFreq = ReportTargetHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x0600592D RID: 22829 RVA: 0x001CF902 File Offset: 0x001CDB02
	private void OnEnable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch += this.NsRand_Dispatch;
		}
	}

	// Token: 0x0600592E RID: 22830 RVA: 0x001CF929 File Offset: 0x001CDB29
	private void OnDisable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch -= this.NsRand_Dispatch;
		}
	}

	// Token: 0x0600592F RID: 22831 RVA: 0x001CF950 File Offset: 0x001CDB50
	private void NsRand_Dispatch(RandomDispatcher randomDispatcher)
	{
		this.seek();
	}

	// Token: 0x06005930 RID: 22832 RVA: 0x001CF958 File Offset: 0x001CDB58
	private void Update()
	{
		if (this.nsRand != null)
		{
			return;
		}
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = ReportTargetHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x06005931 RID: 22833 RVA: 0x001CF9C4 File Offset: 0x001CDBC4
	private void seek()
	{
		if (this.targets.Length != 0)
		{
			Vector3 vector = this.targets[ReportTargetHit.rand.NextInt(this.targets.Length)].position - base.transform.position;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, vector, out raycastHit) && this.colliderFound != null)
			{
				this.colliderFound.Invoke(base.transform.position, raycastHit.point);
			}
		}
	}

	// Token: 0x0400695F RID: 26975
	private static SRand rand = new SRand("ReportForwardHit");

	// Token: 0x04006960 RID: 26976
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x04006961 RID: 26977
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04006962 RID: 26978
	[SerializeField]
	private Transform[] targets;

	// Token: 0x04006963 RID: 26979
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04006964 RID: 26980
	private float timeSinceSeek;

	// Token: 0x04006965 RID: 26981
	private float seekFreq;

	// Token: 0x04006966 RID: 26982
	[SerializeField]
	private RandomDispatcher nsRand;
}
