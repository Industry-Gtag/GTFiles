using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003B8 RID: 952
public class LowEffortZone : GorillaTriggerBox
{
	// Token: 0x060016F9 RID: 5881 RVA: 0x00085A8A File Offset: 0x00083C8A
	private void Awake()
	{
		if (this.triggerOnAwake)
		{
			this.OnBoxTriggered();
		}
	}

	// Token: 0x060016FA RID: 5882 RVA: 0x00085A9C File Offset: 0x00083C9C
	public override void OnBoxTriggered()
	{
		for (int i = 0; i < this.objectsToEnable.Length; i++)
		{
			if (this.objectsToEnable[i] != null)
			{
				this.objectsToEnable[i].SetActive(true);
			}
		}
		for (int j = 0; j < this.objectsToDisable.Length; j++)
		{
			if (this.objectsToDisable[j] != null)
			{
				this.objectsToDisable[j].SetActive(false);
			}
		}
		UnityEvent unityEvent = this.onTriggeredEvents;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04002204 RID: 8708
	public GameObject[] objectsToEnable;

	// Token: 0x04002205 RID: 8709
	public GameObject[] objectsToDisable;

	// Token: 0x04002206 RID: 8710
	public bool triggerOnAwake;

	// Token: 0x04002207 RID: 8711
	public UnityEvent onTriggeredEvents;
}
