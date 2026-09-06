using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200048B RID: 1163
public class NonCosmeticHandItem : MonoBehaviour
{
	// Token: 0x06001C5A RID: 7258 RVA: 0x00099A83 File Offset: 0x00097C83
	public void EnableItem(bool enable)
	{
		if (this.itemPrefab)
		{
			this.itemPrefab.gameObject.SetActive(enable);
		}
	}

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x06001C5B RID: 7259 RVA: 0x00099AA3 File Offset: 0x00097CA3
	public bool IsEnabled
	{
		get
		{
			return this.itemPrefab && this.itemPrefab.gameObject.activeSelf;
		}
	}

	// Token: 0x04002674 RID: 9844
	public CosmeticsController.CosmeticSlots cosmeticSlots;

	// Token: 0x04002675 RID: 9845
	public GameObject itemPrefab;
}
