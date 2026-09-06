using System;
using UnityEngine.Events;

// Token: 0x020005E5 RID: 1509
public class GorillaTriggerBoxEvent : GorillaTriggerBox
{
	// Token: 0x060025BC RID: 9660 RVA: 0x000C8829 File Offset: 0x000C6A29
	public override void OnBoxTriggered()
	{
		UnityEvent unityEvent = this.onBoxTriggered;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060025BD RID: 9661 RVA: 0x000C883B File Offset: 0x000C6A3B
	public override void OnBoxExited()
	{
		UnityEvent unityEvent = this.onBoxExited;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04003158 RID: 12632
	public UnityEvent onBoxTriggered;

	// Token: 0x04003159 RID: 12633
	public UnityEvent onBoxExited;
}
