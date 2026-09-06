using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004EA RID: 1258
public class AnimEventsGeneric : MonoBehaviour
{
	// Token: 0x06001E98 RID: 7832 RVA: 0x000A3A5B File Offset: 0x000A1C5B
	public void Event1()
	{
		this.event1.Invoke();
	}

	// Token: 0x06001E99 RID: 7833 RVA: 0x000A3A68 File Offset: 0x000A1C68
	public void Event2()
	{
		this.event2.Invoke();
	}

	// Token: 0x06001E9A RID: 7834 RVA: 0x000A3A75 File Offset: 0x000A1C75
	public void Event3()
	{
		this.event3.Invoke();
	}

	// Token: 0x06001E9B RID: 7835 RVA: 0x000A3A82 File Offset: 0x000A1C82
	public void Event4()
	{
		this.event4.Invoke();
	}

	// Token: 0x06001E9C RID: 7836 RVA: 0x000A3A8F File Offset: 0x000A1C8F
	public void Event5()
	{
		this.event5.Invoke();
	}

	// Token: 0x06001E9D RID: 7837 RVA: 0x000A3A9C File Offset: 0x000A1C9C
	public void Event6()
	{
		this.event6.Invoke();
	}

	// Token: 0x06001E9E RID: 7838 RVA: 0x000A3AA9 File Offset: 0x000A1CA9
	public void Event7()
	{
		this.event7.Invoke();
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x000A3AB6 File Offset: 0x000A1CB6
	public void Event8()
	{
		this.event8.Invoke();
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x000A3AC3 File Offset: 0x000A1CC3
	public void Event9()
	{
		this.event9.Invoke();
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x000A3AD0 File Offset: 0x000A1CD0
	public void Event10()
	{
		this.event10.Invoke();
	}

	// Token: 0x040028D0 RID: 10448
	[SerializeField]
	private UnityEvent event1;

	// Token: 0x040028D1 RID: 10449
	[SerializeField]
	private UnityEvent event2;

	// Token: 0x040028D2 RID: 10450
	[SerializeField]
	private UnityEvent event3;

	// Token: 0x040028D3 RID: 10451
	[SerializeField]
	private UnityEvent event4;

	// Token: 0x040028D4 RID: 10452
	[SerializeField]
	private UnityEvent event5;

	// Token: 0x040028D5 RID: 10453
	[SerializeField]
	private UnityEvent event6;

	// Token: 0x040028D6 RID: 10454
	[SerializeField]
	private UnityEvent event7;

	// Token: 0x040028D7 RID: 10455
	[SerializeField]
	private UnityEvent event8;

	// Token: 0x040028D8 RID: 10456
	[SerializeField]
	private UnityEvent event9;

	// Token: 0x040028D9 RID: 10457
	[SerializeField]
	private UnityEvent event10;
}
