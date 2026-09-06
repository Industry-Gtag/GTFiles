using System;
using UnityEngine;

// Token: 0x020003C5 RID: 965
public class XSceneInvoker_Send : MonoBehaviour
{
	// Token: 0x06001732 RID: 5938 RVA: 0x00086774 File Offset: 0x00084974
	public void Invoke()
	{
		XSceneInvoker_Receive xsceneInvoker_Receive;
		if (this.target.TryResolve<XSceneInvoker_Receive>(out xsceneInvoker_Receive))
		{
			xsceneInvoker_Receive.Invoke();
		}
	}

	// Token: 0x04002276 RID: 8822
	[SerializeField]
	private XSceneRef target;
}
