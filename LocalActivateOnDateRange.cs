using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200035B RID: 859
public class LocalActivateOnDateRange : MonoBehaviour
{
	// Token: 0x0600150D RID: 5389 RVA: 0x00070490 File Offset: 0x0006E690
	private void Awake()
	{
		GameObject[] array = this.gameObjectsToActivate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x000704BB File Offset: 0x0006E6BB
	private void OnEnable()
	{
		this.InitActiveTimes();
	}

	// Token: 0x0600150F RID: 5391 RVA: 0x000704C4 File Offset: 0x0006E6C4
	private void InitActiveTimes()
	{
		this.activationTime = new DateTime(this.activationYear, this.activationMonth, this.activationDay, this.activationHour, this.activationMinute, this.activationSecond, DateTimeKind.Utc);
		this.deactivationTime = new DateTime(this.deactivationYear, this.deactivationMonth, this.deactivationDay, this.deactivationHour, this.deactivationMinute, this.deactivationSecond, DateTimeKind.Utc);
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x00070534 File Offset: 0x0006E734
	private void LateUpdate()
	{
		DateTime utcNow = DateTime.UtcNow;
		this.dbgTimeUntilActivation = (this.activationTime - utcNow).TotalSeconds;
		this.dbgTimeUntilDeactivation = (this.deactivationTime - utcNow).TotalSeconds;
		bool flag = utcNow >= this.activationTime && utcNow <= this.deactivationTime;
		if (flag != this.isActive)
		{
			GameObject[] array = this.gameObjectsToActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
			this.isActive = flag;
		}
	}

	// Token: 0x040019E3 RID: 6627
	[Header("Activation Date and Time (UTC)")]
	public int activationYear = 2023;

	// Token: 0x040019E4 RID: 6628
	public int activationMonth = 4;

	// Token: 0x040019E5 RID: 6629
	public int activationDay = 1;

	// Token: 0x040019E6 RID: 6630
	public int activationHour = 7;

	// Token: 0x040019E7 RID: 6631
	public int activationMinute;

	// Token: 0x040019E8 RID: 6632
	public int activationSecond;

	// Token: 0x040019E9 RID: 6633
	[Header("Deactivation Date and Time (UTC)")]
	public int deactivationYear = 2023;

	// Token: 0x040019EA RID: 6634
	public int deactivationMonth = 4;

	// Token: 0x040019EB RID: 6635
	public int deactivationDay = 2;

	// Token: 0x040019EC RID: 6636
	public int deactivationHour = 7;

	// Token: 0x040019ED RID: 6637
	public int deactivationMinute;

	// Token: 0x040019EE RID: 6638
	public int deactivationSecond;

	// Token: 0x040019EF RID: 6639
	public GameObject[] gameObjectsToActivate;

	// Token: 0x040019F0 RID: 6640
	private bool isActive;

	// Token: 0x040019F1 RID: 6641
	private DateTime activationTime;

	// Token: 0x040019F2 RID: 6642
	private DateTime deactivationTime;

	// Token: 0x040019F3 RID: 6643
	[DebugReadout]
	public double dbgTimeUntilActivation;

	// Token: 0x040019F4 RID: 6644
	[DebugReadout]
	public double dbgTimeUntilDeactivation;
}
