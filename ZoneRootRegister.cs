using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020003C0 RID: 960
public class ZoneRootRegister : MonoBehaviour
{
	// Token: 0x06001721 RID: 5921 RVA: 0x000864F8 File Offset: 0x000846F8
	private void Awake()
	{
		this.watchableSlot.Value = base.gameObject;
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x0008650B File Offset: 0x0008470B
	private void OnDestroy()
	{
		this.watchableSlot.Value = null;
	}

	// Token: 0x04002256 RID: 8790
	[SerializeField]
	private WatchableGameObjectSO watchableSlot;
}
