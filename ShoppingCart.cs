using System;
using UnityEngine;

// Token: 0x02000568 RID: 1384
public class ShoppingCart : MonoBehaviour
{
	// Token: 0x0600232A RID: 9002 RVA: 0x000BCCFD File Offset: 0x000BAEFD
	public void Awake()
	{
		if (ShoppingCart.instance == null)
		{
			ShoppingCart.instance = this;
			return;
		}
		if (ShoppingCart.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Start()
	{
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Update()
	{
	}

	// Token: 0x04002E42 RID: 11842
	[OnEnterPlay_SetNull]
	public static volatile ShoppingCart instance;
}
