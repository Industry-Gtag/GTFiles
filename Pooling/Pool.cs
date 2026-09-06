using System;
using System.Collections.Generic;
using System.Linq;
using GorillaTag;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Pooling
{
	// Token: 0x02000F13 RID: 3859
	public static class Pool<T> where T : Component, IPoolable<T>
	{
		// Token: 0x06005E92 RID: 24210 RVA: 0x001E2B5A File Offset: 0x001E0D5A
		static Pool()
		{
			SceneManager.sceneUnloaded += Pool<T>.OnSceneManagerSceneUnload;
		}

		// Token: 0x06005E93 RID: 24211 RVA: 0x001E2B77 File Offset: 0x001E0D77
		private static void OnSceneManagerSceneUnload(Scene scene)
		{
			Pool<T>.OnSceneChange();
		}

		// Token: 0x06005E94 RID: 24212 RVA: 0x001E2B80 File Offset: 0x001E0D80
		private static void OnSceneChange()
		{
			T[] array = Pool<T>.poolDict.Keys.ToArray<T>();
			for (int i = 0; i < array.Length; i++)
			{
				Pool<T>.DestroyPool(array[i]);
			}
		}

		// Token: 0x06005E95 RID: 24213 RVA: 0x001E2BB8 File Offset: 0x001E0DB8
		public static IObjectPool<T> CreatePool(T prefab, bool collectionChecks = true, int defaultCapacity = 10, int maxPoolSize = 10000)
		{
			global::UnityEngine.Pool.ObjectPool<T> objectPool = new global::UnityEngine.Pool.ObjectPool<T>(new Func<T>(prefab.CreateInstance<T>), new Action<T>(Pool<T>.OnGet), new Action<T>(Pool<T>.OnRelease), new Action<T>(Pool<T>.OnDestroy), collectionChecks, defaultCapacity, maxPoolSize);
			Pool<T>.poolDict[prefab] = objectPool;
			return objectPool;
		}

		// Token: 0x06005E96 RID: 24214 RVA: 0x001E2C10 File Offset: 0x001E0E10
		public static IObjectPool<T> GetOrCreatePool(T prefab)
		{
			IObjectPool<T> objectPool;
			if (!Pool<T>.poolDict.TryGetValue(prefab, out objectPool))
			{
				objectPool = Pool<T>.CreatePool(prefab, true, 10, 10000);
			}
			return objectPool;
		}

		// Token: 0x06005E97 RID: 24215 RVA: 0x001E2C3C File Offset: 0x001E0E3C
		public static IObjectPool<T> GetPool(T prefab)
		{
			IObjectPool<T> objectPool;
			Pool<T>.poolDict.TryGetValue(prefab, out objectPool);
			return objectPool;
		}

		// Token: 0x06005E98 RID: 24216 RVA: 0x001E2C58 File Offset: 0x001E0E58
		public static void ChildToPoolRoot(Transform transform)
		{
			if (transform.parent != null)
			{
				transform.SetParent(null);
			}
		}

		// Token: 0x06005E99 RID: 24217 RVA: 0x001E2C70 File Offset: 0x001E0E70
		public static T Get(T prefab, Vector3 position, Quaternion rotation, Action<T> beforeEnable = null)
		{
			T t;
			Pool<T>.GetOrCreatePool(prefab).Get(out t);
			t.transform.SetPositionAndRotation(position, rotation);
			if (beforeEnable != null)
			{
				beforeEnable(t);
			}
			t.gameObject.SetActive(true);
			t.OnPostGet();
			return t;
		}

		// Token: 0x06005E9A RID: 24218 RVA: 0x001E2CC4 File Offset: 0x001E0EC4
		public static T Get(T prefab, Vector3 position, Quaternion rotation, Transform parent, Action<T> beforeEnable = null)
		{
			T t;
			Pool<T>.GetOrCreatePool(prefab).Get(out t);
			t.transform.SetParent(parent);
			t.transform.SetPositionAndRotation(position, rotation);
			t.transform.localScale = prefab.transform.localScale;
			if (beforeEnable != null)
			{
				beforeEnable(t);
			}
			t.gameObject.SetActive(true);
			t.OnPostGet();
			return t;
		}

		// Token: 0x06005E9B RID: 24219 RVA: 0x001E2D4C File Offset: 0x001E0F4C
		public static void DestroyPool(T prefab)
		{
			IObjectPool<T> pool = Pool<T>.GetPool(prefab);
			if (pool == null)
			{
				return;
			}
			pool.Clear();
			Pool<T>.poolDict.Remove(prefab);
		}

		// Token: 0x06005E9C RID: 24220 RVA: 0x001E2D76 File Offset: 0x001E0F76
		private static void OnGet(T instance)
		{
			instance.OnPreGet();
		}

		// Token: 0x06005E9D RID: 24221 RVA: 0x001E2D83 File Offset: 0x001E0F83
		private static void OnRelease(T instance)
		{
			if (GTAppState.isQuitting)
			{
				return;
			}
			instance.gameObject.SetActive(false);
			instance.OnRelease();
			Pool<T>.ChildToPoolRoot(instance.transform);
		}

		// Token: 0x06005E9E RID: 24222 RVA: 0x001E2DB9 File Offset: 0x001E0FB9
		private static void OnDestroy(T instance)
		{
			if (!instance)
			{
				return;
			}
			Object.Destroy(instance.gameObject);
		}

		// Token: 0x04006D84 RID: 28036
		private static readonly Dictionary<T, IObjectPool<T>> poolDict = new Dictionary<T, IObjectPool<T>>();

		// Token: 0x04006D85 RID: 28037
		private const Transform PoolRoot = null;
	}
}
