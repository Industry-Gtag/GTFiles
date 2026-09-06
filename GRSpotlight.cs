using System;
using UnityEngine;

// Token: 0x020007FB RID: 2043
public class GRSpotlight : MonoBehaviourTick
{
	// Token: 0x06003445 RID: 13381 RVA: 0x0011F488 File Offset: 0x0011D688
	private void Awake()
	{
		this.yStart = base.transform.rotation.eulerAngles.y;
		this.xStart = base.transform.rotation.eulerAngles.x;
		this.timeOffset = Random.value * 360f;
		this.yFrequency += Random.value / 100f;
		this.xFrequency += Random.value / 100f;
	}

	// Token: 0x06003446 RID: 13382 RVA: 0x0011F514 File Offset: 0x0011D714
	public override void Tick()
	{
		base.transform.eulerAngles = new Vector3(this.xStart + this.xAmplitude * Mathf.Sin(Time.time * this.xFrequency), this.yStart + this.yAmplitude * Mathf.Cos(Time.time * this.yFrequency), 0f);
	}

	// Token: 0x0400440B RID: 17419
	public float yAmplitude = 75f;

	// Token: 0x0400440C RID: 17420
	public float xAmplitude = 40f;

	// Token: 0x0400440D RID: 17421
	public float yFrequency = 0.2f;

	// Token: 0x0400440E RID: 17422
	public float xFrequency = 0.3f;

	// Token: 0x0400440F RID: 17423
	private float yStart;

	// Token: 0x04004410 RID: 17424
	private float xStart;

	// Token: 0x04004411 RID: 17425
	private float timeOffset;
}
