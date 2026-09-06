using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000B2 RID: 178
public class DevWatchButton : MonoBehaviour
{
	// Token: 0x06000446 RID: 1094 RVA: 0x0001901C File Offset: 0x0001721C
	public void OnTriggerEnter(Collider other)
	{
		this.SearchEvent.Invoke();
	}

	// Token: 0x040004B3 RID: 1203
	public UnityEvent SearchEvent = new UnityEvent();
}
