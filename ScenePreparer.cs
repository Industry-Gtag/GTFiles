using System;
using UnityEngine;

// Token: 0x0200039C RID: 924
[DefaultExecutionOrder(-9999)]
public class ScenePreparer : MonoBehaviour
{
	// Token: 0x0600167C RID: 5756 RVA: 0x00082930 File Offset: 0x00080B30
	protected void Awake()
	{
		bool flag = false;
		GameObject[] array = this.betaEnableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		array = this.betaDisableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!flag);
		}
	}

	// Token: 0x0400209A RID: 8346
	public OVRManager ovrManager;

	// Token: 0x0400209B RID: 8347
	public GameObject[] betaDisableObjects;

	// Token: 0x0400209C RID: 8348
	public GameObject[] betaEnableObjects;
}
