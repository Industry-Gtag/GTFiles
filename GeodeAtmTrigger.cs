using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000576 RID: 1398
public class GeodeAtmTrigger : MonoBehaviour
{
	// Token: 0x06002381 RID: 9089 RVA: 0x000BF133 File Offset: 0x000BD333
	private void OnTriggerEnter(Collider other)
	{
		if (this.OnTrigger != null)
		{
			this.OnTrigger.Invoke();
		}
	}

	// Token: 0x04002EBF RID: 11967
	[SerializeField]
	private UnityEvent OnTrigger;
}
