using System;
using UnityEngine;

// Token: 0x020002B7 RID: 695
public class CosmeticFan : MonoBehaviour
{
	// Token: 0x06001200 RID: 4608 RVA: 0x00060AE3 File Offset: 0x0005ECE3
	private void Start()
	{
		this.spinUpRate = this.maxSpeed / this.spinUpDuration;
		this.spinDownRate = this.maxSpeed / this.spinDownDuration;
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x00060B0C File Offset: 0x0005ED0C
	public void Run()
	{
		this.targetSpeed = this.maxSpeed;
		if (this.spinUpDuration > 0f)
		{
			base.enabled = true;
			this.currentAccelRate = this.spinUpRate;
		}
		else
		{
			this.currentSpeed = this.maxSpeed;
		}
		base.enabled = true;
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x00060B5A File Offset: 0x0005ED5A
	public void Stop()
	{
		this.targetSpeed = 0f;
		if (this.spinDownDuration > 0f)
		{
			base.enabled = true;
			this.currentAccelRate = this.spinDownRate;
			return;
		}
		this.currentSpeed = 0f;
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x00060B93 File Offset: 0x0005ED93
	public void InstantStop()
	{
		this.targetSpeed = 0f;
		this.currentSpeed = 0f;
		base.enabled = false;
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x00060BB4 File Offset: 0x0005EDB4
	private void Update()
	{
		this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.targetSpeed, this.currentAccelRate * Time.deltaTime);
		base.transform.localRotation = base.transform.localRotation * Quaternion.AngleAxis(this.currentSpeed * Time.deltaTime, this.axis);
		if (this.currentSpeed == 0f && this.targetSpeed == 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x040015A9 RID: 5545
	[SerializeField]
	private Vector3 axis;

	// Token: 0x040015AA RID: 5546
	[SerializeField]
	private float spinUpDuration = 0.3f;

	// Token: 0x040015AB RID: 5547
	[SerializeField]
	private float spinDownDuration = 0.3f;

	// Token: 0x040015AC RID: 5548
	[SerializeField]
	private float maxSpeed = 360f;

	// Token: 0x040015AD RID: 5549
	private float currentSpeed;

	// Token: 0x040015AE RID: 5550
	private float targetSpeed;

	// Token: 0x040015AF RID: 5551
	private float currentAccelRate;

	// Token: 0x040015B0 RID: 5552
	private float spinUpRate;

	// Token: 0x040015B1 RID: 5553
	private float spinDownRate;
}
