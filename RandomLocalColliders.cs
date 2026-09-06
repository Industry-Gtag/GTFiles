using System;
using UnityEngine;

// Token: 0x02000E40 RID: 3648
public class RandomLocalColliders : MonoBehaviour
{
	// Token: 0x0600591F RID: 22815 RVA: 0x001CF553 File Offset: 0x001CD753
	private void Start()
	{
		this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x06005920 RID: 22816 RVA: 0x001CF574 File Offset: 0x001CD774
	private void Update()
	{
		if (this.colliderFound == null)
		{
			return;
		}
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x06005921 RID: 22817 RVA: 0x001CF5D8 File Offset: 0x001CD7D8
	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		int num2 = Physics.RaycastNonAlloc(base.transform.position, RandomLocalColliders.rand.NextPointOnSphere(1f), this.raycastHits, this.maxRadias * num);
		if (num2 <= 0)
		{
			return;
		}
		int num3 = RandomLocalColliders.rand.NextInt(num2);
		for (int i = 0; i < num2; i++)
		{
			if (this.raycastHits[(i + num3) % num2].distance >= this.minRadias * num)
			{
				this.colliderFound.Invoke(base.transform.position, this.raycastHits[(i + num3) % num2].point);
				return;
			}
		}
	}

	// Token: 0x0400694D RID: 26957
	private static SRand rand = new SRand("RandomLocalColliders");

	// Token: 0x0400694E RID: 26958
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x0400694F RID: 26959
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04006950 RID: 26960
	[SerializeField]
	private float minRadias = 1f;

	// Token: 0x04006951 RID: 26961
	[SerializeField]
	private float maxRadias = 10f;

	// Token: 0x04006952 RID: 26962
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04006953 RID: 26963
	private float timeSinceSeek;

	// Token: 0x04006954 RID: 26964
	private float seekFreq;

	// Token: 0x04006955 RID: 26965
	private RaycastHit[] raycastHits = new RaycastHit[100];
}
