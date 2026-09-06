using System;
using UnityEngine;

// Token: 0x02000354 RID: 852
[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerReparentXform : MonoBehaviour
{
	// Token: 0x060014FA RID: 5370 RVA: 0x00070032 File Offset: 0x0006E232
	protected void Awake()
	{
		if (base.enabled)
		{
			this._DoIt();
		}
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x00070042 File Offset: 0x0006E242
	protected void OnEnable()
	{
		this._DoIt();
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x0007004A File Offset: 0x0006E24A
	private void _DoIt()
	{
		if (this._didIt)
		{
			return;
		}
		if (this.newParent != null)
		{
			base.transform.SetParent(this.newParent, true);
		}
		Object.Destroy(this);
	}

	// Token: 0x040019B2 RID: 6578
	public Transform newParent;

	// Token: 0x040019B3 RID: 6579
	private bool _didIt;
}
