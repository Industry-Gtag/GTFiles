using System;
using UnityEngine;

namespace Pooling
{
	// Token: 0x02000F16 RID: 3862
	public static class ComponentPoolExts
	{
		// Token: 0x06005EC0 RID: 24256 RVA: 0x001E3247 File Offset: 0x001E1447
		public static T CreateComponentInstance<T>(this T prefab) where T : Component
		{
			T t = Object.Instantiate<T>(prefab);
			t.gameObject.SetActive(false);
			t.name = "[Pooled] " + prefab.name;
			return t;
		}

		// Token: 0x06005EC1 RID: 24257 RVA: 0x001E3280 File Offset: 0x001E1480
		public static T GetInstance<T>(this T prefab) where T : Component
		{
			return prefab.GetInstance(Vector3.zero, Quaternion.identity);
		}

		// Token: 0x06005EC2 RID: 24258 RVA: 0x001E3294 File Offset: 0x001E1494
		public static T GetInstance<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Component
		{
			T t;
			ComponentPool<T>.GetOrCreatePool(prefab).Get(out t);
			t.transform.SetPositionAndRotation(position, rotation);
			t.gameObject.SetActive(true);
			return t;
		}

		// Token: 0x06005EC3 RID: 24259 RVA: 0x001E32D3 File Offset: 0x001E14D3
		public static T GetUninstantiated<T>(this T prefab, Transform parent) where T : Component
		{
			return prefab.GetUninstantiated(parent.position, parent.rotation, parent);
		}

		// Token: 0x06005EC4 RID: 24260 RVA: 0x001E32E8 File Offset: 0x001E14E8
		public static T GetUninstantiated<T>(this T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
		{
			T t;
			ComponentPool<T>.GetOrCreatePool(prefab).Get(out t);
			t.transform.SetParent(parent);
			t.transform.SetPositionAndRotation(position, rotation);
			return t;
		}

		// Token: 0x06005EC5 RID: 24261 RVA: 0x001E3328 File Offset: 0x001E1528
		public static T GetUninstantiated<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Component
		{
			T t;
			ComponentPool<T>.GetOrCreatePool(prefab).Get(out t);
			if (t.gameObject.activeSelf)
			{
				t.gameObject.SetActive(false);
			}
			t.transform.SetPositionAndRotation(position, rotation);
			return t;
		}

		// Token: 0x06005EC6 RID: 24262 RVA: 0x001E3379 File Offset: 0x001E1579
		public static void ReleaseInstance<T>(this T prefab, T instance) where T : Component
		{
			ComponentPool<T>.Release(prefab, instance);
		}
	}
}
