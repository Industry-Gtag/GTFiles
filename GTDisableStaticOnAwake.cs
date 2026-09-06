using System;
using UnityEngine;

// Token: 0x02000348 RID: 840
public class GTDisableStaticOnAwake : MonoBehaviour
{
	// Token: 0x060014B8 RID: 5304 RVA: 0x0006E860 File Offset: 0x0006CA60
	private void Awake()
	{
		base.gameObject.isStatic = false;
		Object.Destroy(this);
	}
}
