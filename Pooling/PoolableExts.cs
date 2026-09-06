using System;
using UnityEngine;

namespace Pooling
{
	// Token: 0x02000F1B RID: 3867
	public static class PoolableExts
	{
		// Token: 0x06005EDB RID: 24283 RVA: 0x001E343C File Offset: 0x001E163C
		public static T CreateInstance<T>(this T prefab) where T : Component, IPoolable<T>
		{
			T t = Object.Instantiate<T>(prefab);
			t.gameObject.SetActive(false);
			t.name = "[Pooled] " + prefab.name;
			t.Pool = Pool<T>.GetOrCreatePool(prefab);
			t.OnCreate();
			return t;
		}

		// Token: 0x06005EDC RID: 24284 RVA: 0x001E349C File Offset: 0x001E169C
		public static T Get<T>(this T prefab) where T : Component, IPoolable<T>
		{
			return prefab.Get(Vector3.zero, Quaternion.identity, null);
		}

		// Token: 0x06005EDD RID: 24285 RVA: 0x001E34AF File Offset: 0x001E16AF
		public static T Get<T>(this T prefab, Transform parent) where T : Component, IPoolable<T>
		{
			return prefab.Get(parent.position, parent.rotation, parent, null);
		}

		// Token: 0x06005EDE RID: 24286 RVA: 0x001E34C5 File Offset: 0x001E16C5
		public static T Get<T>(this T prefab, Vector3 position, Quaternion rotation, Action<T> beforeEnable = null) where T : Component, IPoolable<T>
		{
			return Pool<T>.Get(prefab, position, rotation, beforeEnable);
		}

		// Token: 0x06005EDF RID: 24287 RVA: 0x001E34D0 File Offset: 0x001E16D0
		public static T Get<T>(this T prefab, Vector3 position, Quaternion rotation, Transform parent, Action<T> beforeEnable = null) where T : Component, IPoolable<T>
		{
			return Pool<T>.Get(prefab, position, rotation, parent, beforeEnable);
		}

		// Token: 0x06005EE0 RID: 24288 RVA: 0x001E34DD File Offset: 0x001E16DD
		public static void Release<T>(this T poolable) where T : Component, IPoolable<T>
		{
			poolable.Pool.Release(poolable);
		}

		// Token: 0x06005EE1 RID: 24289 RVA: 0x001E34F0 File Offset: 0x001E16F0
		public static void DestroyPool<T>(this T prefab) where T : Component, IPoolable<T>
		{
			Pool<T>.DestroyPool(prefab);
		}
	}
}
