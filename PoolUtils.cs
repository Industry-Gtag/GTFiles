using System;
using UnityEngine;

// Token: 0x02000DBC RID: 3516
public static class PoolUtils
{
	// Token: 0x06005658 RID: 22104 RVA: 0x001C3918 File Offset: 0x001C1B18
	public static int GameObjHashCode(GameObject obj)
	{
		return obj.tag.GetHashCode();
	}
}
