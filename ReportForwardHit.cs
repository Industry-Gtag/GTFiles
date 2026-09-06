using System;
using NetSynchrony;
using UnityEngine;

// Token: 0x02000E41 RID: 3649
public class ReportForwardHit : MonoBehaviour
{
	// Token: 0x06005924 RID: 22820 RVA: 0x001CF71D File Offset: 0x001CD91D
	private void Start()
	{
		this.seekFreq = ReportForwardHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x06005925 RID: 22821 RVA: 0x001CF73B File Offset: 0x001CD93B
	private void OnEnable()
	{
		if (this.seekOnEnable)
		{
			this.seek();
		}
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch += this.NsRand_Dispatch;
		}
	}

	// Token: 0x06005926 RID: 22822 RVA: 0x001CF770 File Offset: 0x001CD970
	private void OnDisable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch -= this.NsRand_Dispatch;
		}
	}

	// Token: 0x06005927 RID: 22823 RVA: 0x001CF797 File Offset: 0x001CD997
	private void NsRand_Dispatch(RandomDispatcher randomDispatcher)
	{
		this.seek();
	}

	// Token: 0x06005928 RID: 22824 RVA: 0x001CF7A0 File Offset: 0x001CD9A0
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
			this.seekFreq = ReportForwardHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x06005929 RID: 22825 RVA: 0x001CF80C File Offset: 0x001CDA0C
	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, this.maxRadias * num) && this.colliderFound != null)
		{
			this.colliderFound.Invoke(base.transform.position, raycastHit.point);
		}
	}

	// Token: 0x04006956 RID: 26966
	private static SRand rand = new SRand("ReportForwardHit");

	// Token: 0x04006957 RID: 26967
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x04006958 RID: 26968
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04006959 RID: 26969
	[SerializeField]
	private float maxRadias = 10f;

	// Token: 0x0400695A RID: 26970
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x0400695B RID: 26971
	[SerializeField]
	private RandomDispatcher nsRand;

	// Token: 0x0400695C RID: 26972
	private float timeSinceSeek;

	// Token: 0x0400695D RID: 26973
	private float seekFreq;

	// Token: 0x0400695E RID: 26974
	[SerializeField]
	private bool seekOnEnable;
}
