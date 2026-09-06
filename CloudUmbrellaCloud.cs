using System;
using UnityEngine;

// Token: 0x0200020B RID: 523
public class CloudUmbrellaCloud : MonoBehaviour
{
	// Token: 0x06000DDC RID: 3548 RVA: 0x0004C13F File Offset: 0x0004A33F
	protected void Awake()
	{
		this.umbrellaXform = this.umbrella.transform;
		this.cloudScaleXform = this.cloudRenderer.transform;
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x0004C164 File Offset: 0x0004A364
	protected void LateUpdate()
	{
		float num = Vector3.Dot(this.umbrellaXform.up, Vector3.up);
		float num2 = Mathf.Clamp01(this.scaleCurve.Evaluate(num));
		this.rendererOn = ((num2 > 0.09f && num2 < 0.1f) ? this.rendererOn : (num2 > 0.1f));
		this.cloudRenderer.enabled = this.rendererOn;
		this.cloudScaleXform.localScale = new Vector3(num2, num2, num2);
		this.cloudRotateXform.up = Vector3.up;
	}

	// Token: 0x04001079 RID: 4217
	public UmbrellaItem umbrella;

	// Token: 0x0400107A RID: 4218
	public Transform cloudRotateXform;

	// Token: 0x0400107B RID: 4219
	public Renderer cloudRenderer;

	// Token: 0x0400107C RID: 4220
	public AnimationCurve scaleCurve;

	// Token: 0x0400107D RID: 4221
	private const float kHideAtScale = 0.1f;

	// Token: 0x0400107E RID: 4222
	private const float kHideAtScaleTolerance = 0.01f;

	// Token: 0x0400107F RID: 4223
	private bool rendererOn;

	// Token: 0x04001080 RID: 4224
	private Transform umbrellaXform;

	// Token: 0x04001081 RID: 4225
	private Transform cloudScaleXform;
}
