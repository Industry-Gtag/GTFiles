using System;
using UnityEngine;

// Token: 0x020002B8 RID: 696
public class CosmeticRefRegistry : MonoBehaviour
{
	// Token: 0x06001206 RID: 4614 RVA: 0x00060C60 File Offset: 0x0005EE60
	private void Awake()
	{
		foreach (CosmeticRefTarget cosmeticRefTarget in this.builtInRefTargets)
		{
			this.Register(cosmeticRefTarget.id, cosmeticRefTarget.gameObject);
		}
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x00060C98 File Offset: 0x0005EE98
	public void Register(CosmeticRefID partID, GameObject part)
	{
		this.partsTable[(int)partID] = part;
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x00060CA3 File Offset: 0x0005EEA3
	public GameObject Get(CosmeticRefID partID)
	{
		return this.partsTable[(int)partID];
	}

	// Token: 0x040015B2 RID: 5554
	private GameObject[] partsTable = new GameObject[9];

	// Token: 0x040015B3 RID: 5555
	[SerializeField]
	private CosmeticRefTarget[] builtInRefTargets;
}
