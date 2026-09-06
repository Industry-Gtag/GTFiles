using System;
using UnityEngine;

// Token: 0x0200087C RID: 2172
public class GorillaHasUITransformFollow : MonoBehaviour
{
	// Token: 0x06003897 RID: 14487 RVA: 0x001343F8 File Offset: 0x001325F8
	private void Awake()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(base.gameObject.activeSelf);
		}
	}

	// Token: 0x06003898 RID: 14488 RVA: 0x00134434 File Offset: 0x00132634
	private void OnDestroy()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}

	// Token: 0x06003899 RID: 14489 RVA: 0x00134464 File Offset: 0x00132664
	private void OnEnable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x0600389A RID: 14490 RVA: 0x00134494 File Offset: 0x00132694
	private void OnDisable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x04004892 RID: 18578
	public GorillaUITransformFollow[] transformFollowers;
}
