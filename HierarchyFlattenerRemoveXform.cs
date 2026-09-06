using System;
using UnityEngine;

// Token: 0x02000353 RID: 851
[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerRemoveXform : MonoBehaviour
{
	// Token: 0x060014F7 RID: 5367 RVA: 0x0006FFAE File Offset: 0x0006E1AE
	protected void Awake()
	{
		this._DoIt();
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x0006FFB8 File Offset: 0x0006E1B8
	private void _DoIt()
	{
		if (this._didIt)
		{
			return;
		}
		if (base.GetComponentInChildren<HierarchyFlattenerRemoveXform>(true) != null)
		{
			return;
		}
		HierarchyFlattenerRemoveXform componentInParent = base.GetComponentInParent<HierarchyFlattenerRemoveXform>(true);
		this._didIt = true;
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).SetParent(transform.parent, true);
		}
		Object.Destroy(base.gameObject);
		if (componentInParent != null)
		{
			componentInParent._DoIt();
		}
	}

	// Token: 0x040019B1 RID: 6577
	private bool _didIt;
}
