using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200048E RID: 1166
public class OnTapHandler : Tappable
{
	// Token: 0x06001C5F RID: 7263 RVA: 0x00099B22 File Offset: 0x00097D22
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped sender)
	{
		UnityEvent onTapEvents = this.OnTapEvents;
		if (onTapEvents == null)
		{
			return;
		}
		onTapEvents.Invoke();
	}

	// Token: 0x06001C60 RID: 7264 RVA: 0x00099B34 File Offset: 0x00097D34
	public override void OnGrabLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
		UnityEvent onGrabEvents = this.OnGrabEvents;
		if (onGrabEvents == null)
		{
			return;
		}
		onGrabEvents.Invoke();
	}

	// Token: 0x06001C61 RID: 7265 RVA: 0x00099B46 File Offset: 0x00097D46
	public override void OnReleaseLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
		UnityEvent onReleaseEvents = this.OnReleaseEvents;
		if (onReleaseEvents == null)
		{
			return;
		}
		onReleaseEvents.Invoke();
	}

	// Token: 0x0400267C RID: 9852
	[SerializeField]
	private UnityEvent OnTapEvents;

	// Token: 0x0400267D RID: 9853
	[SerializeField]
	private UnityEvent OnGrabEvents;

	// Token: 0x0400267E RID: 9854
	[SerializeField]
	private UnityEvent OnReleaseEvents;
}
