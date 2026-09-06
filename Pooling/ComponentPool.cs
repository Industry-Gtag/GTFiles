using System;
using System.Collections.Generic;
using System.Linq;
using GorillaTag;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Pooling
{
	// Token: 0x02000F15 RID: 3861
	public static class ComponentPool<T> where T : Component
	{
		// Token: 0x06005EB4 RID: 24244 RVA: 0x001E30BF File Offset: 0x001E12BF
		static ComponentPool()
		{
			SceneManager.sceneUnloaded += ComponentPool<T>.OnSceneManagerSceneUnload;
		}

		// Token: 0x06005EB5 RID: 24245 RVA: 0x001E30DC File Offset: 0x001E12DC
		private static void OnSceneManagerSceneUnload(Scene scene)
		{
			ComponentPool<T>.OnSceneWillChange();
		}

		// Token: 0x06005EB6 RID: 24246 RVA: 0x001E30E4 File Offset: 0x001E12E4
		private static void OnSceneWillChange()
		{
			T[] array = ComponentPool<T>.poolDict.Keys.ToArray<T>();
			for (int i = 0; i < array.Length; i++)
			{
				ComponentPool<T>.DestroyPool(array[i]);
			}
		}

		// Token: 0x06005EB7 RID: 24247 RVA: 0x001E311C File Offset: 0x001E131C
		public static IObjectPool<T> CreatePool(T prefab, bool collectionChecks = true, int defaultCapacity = 10, int maxPoolSize = 10000)
		{
			global::UnityEngine.Pool.ObjectPool<T> objectPool = new global::UnityEngine.Pool.ObjectPool<T>(new Func<T>(prefab.CreateComponentInstance<T>), new Action<T>(ComponentPool<T>.OnGet), new Action<T>(ComponentPool<T>.OnRelease), new Action<T>(ComponentPool<T>.OnDestroy), collectionChecks, defaultCapacity, maxPoolSize);
			ComponentPool<T>.poolDict[prefab] = objectPool;
			return objectPool;
		}

		// Token: 0x06005EB8 RID: 24248 RVA: 0x001E3174 File Offset: 0x001E1374
		public static IObjectPool<T> GetOrCreatePool(T prefab)
		{
			IObjectPool<T> objectPool;
			if (!ComponentPool<T>.poolDict.TryGetValue(prefab, out objectPool))
			{
				objectPool = ComponentPool<T>.CreatePool(prefab, true, 10, 10000);
			}
			return objectPool;
		}

		// Token: 0x06005EB9 RID: 24249 RVA: 0x001E31A0 File Offset: 0x001E13A0
		public static IObjectPool<T> GetPool(T prefab)
		{
			IObjectPool<T> objectPool;
			ComponentPool<T>.poolDict.TryGetValue(prefab, out objectPool);
			return objectPool;
		}

		// Token: 0x06005EBA RID: 24250 RVA: 0x001E2C58 File Offset: 0x001E0E58
		public static void ChildToPoolRoot(Transform transform)
		{
			if (transform.parent != null)
			{
				transform.SetParent(null);
			}
		}

		// Token: 0x06005EBB RID: 24251 RVA: 0x001E31BC File Offset: 0x001E13BC
		public static void Release(T prefab, T instance)
		{
			IObjectPool<T> objectPool;
			if (ComponentPool<T>.poolDict.TryGetValue(prefab, out objectPool))
			{
				objectPool.Release(instance);
			}
		}

		// Token: 0x06005EBC RID: 24252 RVA: 0x001E31E0 File Offset: 0x001E13E0
		public static void DestroyPool(T prefab)
		{
			IObjectPool<T> pool = ComponentPool<T>.GetPool(prefab);
			if (pool == null)
			{
				return;
			}
			pool.Clear();
			ComponentPool<T>.poolDict.Remove(prefab);
		}

		// Token: 0x06005EBD RID: 24253 RVA: 0x00002C2D File Offset: 0x00000E2D
		private static void OnGet(T instance)
		{
		}

		// Token: 0x06005EBE RID: 24254 RVA: 0x001E320A File Offset: 0x001E140A
		private static void OnRelease(T instance)
		{
			if (GTAppState.isQuitting)
			{
				return;
			}
			instance.gameObject.SetActive(false);
			ComponentPool<T>.ChildToPoolRoot(instance.transform);
		}

		// Token: 0x06005EBF RID: 24255 RVA: 0x001E3235 File Offset: 0x001E1435
		private static void OnDestroy(T instance)
		{
			Object.Destroy(instance.gameObject);
		}

		// Token: 0x04006D88 RID: 28040
		private static readonly Dictionary<T, IObjectPool<T>> poolDict = new Dictionary<T, IObjectPool<T>>();

		// Token: 0x04006D89 RID: 28041
		private const Transform PoolRoot = null;
	}
}
