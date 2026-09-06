using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000E20 RID: 3616
public class TimeEvent : MonoBehaviour
{
	// Token: 0x06005886 RID: 22662 RVA: 0x001CBE83 File Offset: 0x001CA083
	protected void StartEvent()
	{
		this._ongoing = true;
		UnityEvent unityEvent = this.onEventStart;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06005887 RID: 22663 RVA: 0x001CBE9C File Offset: 0x001CA09C
	protected void StopEvent()
	{
		this._ongoing = false;
		UnityEvent unityEvent = this.onEventStop;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x040068B6 RID: 26806
	public UnityEvent onEventStart;

	// Token: 0x040068B7 RID: 26807
	public UnityEvent onEventStop;

	// Token: 0x040068B8 RID: 26808
	[SerializeField]
	protected bool _ongoing;
}
