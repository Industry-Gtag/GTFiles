using System;
using UnityEngine;

// Token: 0x020007FC RID: 2044
public class GRSpringMovement
{
	// Token: 0x06003448 RID: 13384 RVA: 0x0011F5A8 File Offset: 0x0011D7A8
	public GRSpringMovement(float _tension, float _dampening)
	{
		this.tension = _tension;
		this.dampening = _dampening;
	}

	// Token: 0x06003449 RID: 13385 RVA: 0x0011F5DB File Offset: 0x0011D7DB
	public void Reset()
	{
		this.pos = 0f;
		this.target = 0f;
		this.speed = 0f;
		this.wasAlreadyAtTargetLastUpdate = false;
	}

	// Token: 0x0600344A RID: 13386 RVA: 0x0011F605 File Offset: 0x0011D805
	public void SetHardStopAtTarget(bool _hardStopAtTarget)
	{
		if (this.hardStopAtTarget == _hardStopAtTarget)
		{
			return;
		}
		this.hardStopAtTarget = _hardStopAtTarget;
		this.speed = 0f;
	}

	// Token: 0x0600344B RID: 13387 RVA: 0x0011F624 File Offset: 0x0011D824
	public void Update()
	{
		this.wasAlreadyAtTargetLastUpdate = this.pos == this.target && this.speed == 0f;
		float num = this.pos;
		float num2 = 0.001f;
		float num3 = Mathf.Min(Time.deltaTime, 0.05f);
		float num4 = 6.2832f / this.tension;
		float num5 = num4 * num4 * (this.target - this.pos) - 2f * this.dampening * num4 * this.speed;
		this.speed += num5 * num3;
		this.pos += this.speed * num3;
		if (this.hardStopAtTarget)
		{
			if ((num <= this.pos && this.pos + num2 >= this.target) || (num >= this.pos && this.pos - num2 <= this.target))
			{
				this.speed = 0f;
				this.pos = this.target;
				return;
			}
		}
		else if (Mathf.Abs(num - this.target) < num2 && Mathf.Abs(this.speed) < num2)
		{
			this.speed = 0f;
			this.pos = this.target;
		}
	}

	// Token: 0x0600344C RID: 13388 RVA: 0x0011F755 File Offset: 0x0011D955
	public bool HitTargetLastUpdate()
	{
		return this.IsAtTarget() && !this.wasAlreadyAtTargetLastUpdate;
	}

	// Token: 0x0600344D RID: 13389 RVA: 0x0011F76A File Offset: 0x0011D96A
	public bool IsAtTarget()
	{
		return this.pos == this.target && this.speed == 0f;
	}

	// Token: 0x04004412 RID: 17426
	public float tension = 1f;

	// Token: 0x04004413 RID: 17427
	public float dampening = 0.7f;

	// Token: 0x04004414 RID: 17428
	public float target;

	// Token: 0x04004415 RID: 17429
	public bool hardStopAtTarget = true;

	// Token: 0x04004416 RID: 17430
	public float pos;

	// Token: 0x04004417 RID: 17431
	public float speed;

	// Token: 0x04004418 RID: 17432
	private bool wasAlreadyAtTargetLastUpdate;
}
