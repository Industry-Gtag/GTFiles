using System;
using UnityEngine;

// Token: 0x020005C4 RID: 1476
public class DisableScreamer : MonoBehaviour
{
	// Token: 0x0600253E RID: 9534 RVA: 0x000C77B9 File Offset: 0x000C59B9
	private void OnDisable()
	{
		Debug.LogError("oh my god i've been disabled! aaag!!! AAAAAAAAA!!!!", base.gameObject);
	}
}
