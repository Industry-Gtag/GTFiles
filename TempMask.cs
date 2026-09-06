using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class TempMask : MonoBehaviour
{
	// Token: 0x06000C00 RID: 3072 RVA: 0x00041AA8 File Offset: 0x0003FCA8
	private void Awake()
	{
		this.dayOn = new DateTime(this.year, this.month, this.day);
		this.myRig = base.GetComponentInParent<VRRig>();
		if (this.myRig != null && this.myRig.netView.IsMine && !this.myRig.isOfflineVRRig)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00041B16 File Offset: 0x0003FD16
	private void OnEnable()
	{
		base.StartCoroutine(this.MaskOnDuringDate());
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00005879 File Offset: 0x00003A79
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00041B25 File Offset: 0x0003FD25
	private IEnumerator MaskOnDuringDate()
	{
		for (;;)
		{
			if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.myDate = new DateTime(GorillaComputer.instance.startupMillis * 10000L + (long)(Time.realtimeSinceStartup * 1000f * 10000f)).Subtract(TimeSpan.FromHours(7.0));
				if (this.myDate.DayOfYear == this.dayOn.DayOfYear)
				{
					if (!this.myRenderer.enabled)
					{
						this.myRenderer.enabled = true;
					}
				}
				else if (this.myRenderer.enabled)
				{
					this.myRenderer.enabled = false;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x04000EA6 RID: 3750
	public int year;

	// Token: 0x04000EA7 RID: 3751
	public int month;

	// Token: 0x04000EA8 RID: 3752
	public int day;

	// Token: 0x04000EA9 RID: 3753
	public DateTime dayOn;

	// Token: 0x04000EAA RID: 3754
	public MeshRenderer myRenderer;

	// Token: 0x04000EAB RID: 3755
	private DateTime myDate;

	// Token: 0x04000EAC RID: 3756
	private VRRig myRig;
}
