using System;
using UnityEngine;

// Token: 0x020005BB RID: 1467
public class DestroyIfNotQA : MonoBehaviour
{
	// Token: 0x06002523 RID: 9507 RVA: 0x0006CBFF File Offset: 0x0006ADFF
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
