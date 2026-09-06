using System;
using UnityEngine;

// Token: 0x020003AC RID: 940
public static class UnityLayerExtensions
{
	// Token: 0x060016C4 RID: 5828 RVA: 0x00084096 File Offset: 0x00082296
	public static int ToLayerMask(this UnityLayer self)
	{
		return 1 << (int)self;
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x0008409E File Offset: 0x0008229E
	public static int ToLayerIndex(this UnityLayer self)
	{
		return (int)self;
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x000840A1 File Offset: 0x000822A1
	public static bool IsOnLayer(this GameObject obj, UnityLayer layer)
	{
		return obj.layer == (int)layer;
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x000840AC File Offset: 0x000822AC
	public static void SetLayer(this GameObject obj, UnityLayer layer)
	{
		obj.layer = (int)layer;
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x000840B8 File Offset: 0x000822B8
	public static void SetLayerRecursively(this GameObject obj, UnityLayer layer)
	{
		obj.layer = (int)layer;
		foreach (object obj2 in obj.transform)
		{
			((Transform)obj2).gameObject.SetLayerRecursively(layer);
		}
	}
}
