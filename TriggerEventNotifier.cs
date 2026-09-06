using System;
using UnityEngine;

// Token: 0x02000DF0 RID: 3568
public class TriggerEventNotifier : MonoBehaviour
{
	// Token: 0x1400009D RID: 157
	// (add) Token: 0x0600577A RID: 22394 RVA: 0x001C8EC0 File Offset: 0x001C70C0
	// (remove) Token: 0x0600577B RID: 22395 RVA: 0x001C8EF8 File Offset: 0x001C70F8
	public event TriggerEventNotifier.TriggerEvent TriggerEnterEvent;

	// Token: 0x1400009E RID: 158
	// (add) Token: 0x0600577C RID: 22396 RVA: 0x001C8F30 File Offset: 0x001C7130
	// (remove) Token: 0x0600577D RID: 22397 RVA: 0x001C8F68 File Offset: 0x001C7168
	public event TriggerEventNotifier.TriggerEvent TriggerExitEvent;

	// Token: 0x0600577E RID: 22398 RVA: 0x001C8F9D File Offset: 0x001C719D
	private void OnTriggerEnter(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerEnterEvent = this.TriggerEnterEvent;
		if (triggerEnterEvent == null)
		{
			return;
		}
		triggerEnterEvent(this, other);
	}

	// Token: 0x0600577F RID: 22399 RVA: 0x001C8FB1 File Offset: 0x001C71B1
	private void OnTriggerExit(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerExitEvent = this.TriggerExitEvent;
		if (triggerExitEvent == null)
		{
			return;
		}
		triggerExitEvent(this, other);
	}

	// Token: 0x04006812 RID: 26642
	[HideInInspector]
	public int maskIndex;

	// Token: 0x02000DF1 RID: 3569
	// (Invoke) Token: 0x06005782 RID: 22402
	public delegate void TriggerEvent(TriggerEventNotifier notifier, Collider collider);
}
