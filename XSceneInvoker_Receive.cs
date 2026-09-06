using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003C4 RID: 964
public class XSceneInvoker_Receive : MonoBehaviour
{
	// Token: 0x06001730 RID: 5936 RVA: 0x00086766 File Offset: 0x00084966
	public void Invoke()
	{
		this.evt.Invoke();
	}

	// Token: 0x04002275 RID: 8821
	[SerializeField]
	private UnityEvent evt;
}
