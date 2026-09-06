using System;
using UnityEngine;

// Token: 0x020008E3 RID: 2275
public class GorillaUIParent : MonoBehaviour
{
	// Token: 0x06003B99 RID: 15257 RVA: 0x00146A61 File Offset: 0x00144C61
	private void Awake()
	{
		if (GorillaUIParent.instance == null)
		{
			GorillaUIParent.instance = this;
			return;
		}
		if (GorillaUIParent.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04004C32 RID: 19506
	[OnEnterPlay_SetNull]
	public static volatile GorillaUIParent instance;
}
