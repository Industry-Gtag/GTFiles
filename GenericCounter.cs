using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005D5 RID: 1493
public class GenericCounter : MonoBehaviour
{
	// Token: 0x0600258F RID: 9615 RVA: 0x000C82EC File Offset: 0x000C64EC
	public void CountUp()
	{
		this.currentCount++;
		this.DoCallbacks();
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x000C8302 File Offset: 0x000C6502
	public void CountDown()
	{
		this.currentCount--;
		this.DoCallbacks();
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x000C8318 File Offset: 0x000C6518
	private void DoCallbacks()
	{
		if (this.currentCount < this.Threshold)
		{
			this.whenLessThan.Invoke();
			return;
		}
		if (this.currentCount == this.Threshold)
		{
			this.whenEqual.Invoke();
			return;
		}
		this.whenGreaterThan.Invoke();
	}

	// Token: 0x06002592 RID: 9618 RVA: 0x000C8364 File Offset: 0x000C6564
	public void ResetCounter()
	{
		this.currentCount = 0;
	}

	// Token: 0x040030FA RID: 12538
	[SerializeField]
	private int Threshold;

	// Token: 0x040030FB RID: 12539
	[SerializeField]
	private UnityEvent whenLessThan;

	// Token: 0x040030FC RID: 12540
	[SerializeField]
	private UnityEvent whenEqual;

	// Token: 0x040030FD RID: 12541
	[SerializeField]
	private UnityEvent whenGreaterThan;

	// Token: 0x040030FE RID: 12542
	private int currentCount;
}
