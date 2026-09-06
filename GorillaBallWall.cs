using System;
using UnityEngine;

// Token: 0x0200085F RID: 2143
public class GorillaBallWall : MonoBehaviour
{
	// Token: 0x06003790 RID: 14224 RVA: 0x00130C58 File Offset: 0x0012EE58
	private void Awake()
	{
		if (GorillaBallWall.instance == null)
		{
			GorillaBallWall.instance = this;
			return;
		}
		if (GorillaBallWall.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06003791 RID: 14225 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Update()
	{
	}

	// Token: 0x040047B7 RID: 18359
	[OnEnterPlay_SetNull]
	public static volatile GorillaBallWall instance;
}
