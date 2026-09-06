using System;
using UnityEngine;

// Token: 0x02000DA0 RID: 3488
public class DestroyOnDisabled : MonoBehaviour
{
	// Token: 0x060055D9 RID: 21977 RVA: 0x0006CBFF File Offset: 0x0006ADFF
	private void OnDisable()
	{
		Object.Destroy(base.gameObject);
	}
}
