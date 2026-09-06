using System;
using System.Collections.Generic;
using System.Linq;
using GorillaTag;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Pooling
{
	// Token: 0x02000F14 RID: 3860
	public static class Pool
	{
		// Token: 0x06005E9F RID: 24223 RVA: 0x001E2DD9 File Offset: 0x001E0FD9
		static Pool()
		{
			SceneManager.sceneUnloaded += Pool.OnSceneManagerSceneUnload;
		}

		// Token: 0x06005EA0 RID: 24224 RVA: 0x001E2DF6 File Offset: 0x001E0FF6
		private static void OnSceneManagerSceneUnload(Scene scene)
		{
			Pool.OnSceneChange();
		}

		// Token: 0x06005EA1 RID: 24225 RVA: 0x001E2E00 File Offset: 0x001E1000
		private static void OnSceneChange()
		{
			GameObject[] array = Pool.poolDict.Keys.ToArray<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				Pool.DestroyPool(array[i]);
			}
		}

		// Token: 0x06005EA2 RID: 24226 RVA: 0x001E2E34 File Offset: 0x001E1034
		public static IObjectPool<GameObject> CreatePool(GameObject prefab, bool collectionChecks = true, int defaultCapacity = 10, int maxPoolSize = 10000)
		{
			global::UnityEngine.Pool.ObjectPool<GameObject> objectPool = new global::UnityEngine.Pool.ObjectPool<GameObject>(new Func<GameObject>(prefab.CreateInstance), new Action<GameObject>(Pool.OnGet), new Action<GameObject>(Pool.OnRelease), new Action<GameObject>(Pool.OnDestroy), collectionChecks, defaultCapacity, maxPoolSize);
			Pool.poolDict[prefab] = objectPool;
			return objectPool;
		}

		// Token: 0x06005EA3 RID: 24227 RVA: 0x001E2E88 File Offset: 0x001E1088
		public static IObjectPool<GameObject> GetOrCreatePool(GameObject prefab)
		{
			IObjectPool<GameObject> objectPool;
			if (!Pool.poolDict.TryGetValue(prefab, out objectPool))
			{
				objectPool = Pool.CreatePool(prefab, true, 10, 10000);
			}
			return objectPool;
		}

		// Token: 0x06005EA4 RID: 24228 RVA: 0x001E2EB4 File Offset: 0x001E10B4
		public static IObjectPool<GameObject> GetPool(GameObject prefab)
		{
			IObjectPool<GameObject> objectPool;
			Pool.poolDict.TryGetValue(prefab, out objectPool);
			return objectPool;
		}

		// Token: 0x06005EA5 RID: 24229 RVA: 0x001E2C58 File Offset: 0x001E0E58
		public static void ChildToPoolRoot(Transform transform)
		{
			if (transform.parent != null)
			{
				transform.SetParent(null);
			}
		}

		// Token: 0x06005EA6 RID: 24230 RVA: 0x001E2ED0 File Offset: 0x001E10D0
		public static GameObject CreateInstance(this GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(prefab);
			gameObject.gameObject.SetActive(false);
			gameObject.name = "[Pooled] " + prefab.name;
			return gameObject;
		}

		// Token: 0x06005EA7 RID: 24231 RVA: 0x001E2EFA File Offset: 0x001E10FA
		public static GameObject Get(this GameObject prefab)
		{
			return prefab.Get(Vector3.zero, Quaternion.identity);
		}

		// Token: 0x06005EA8 RID: 24232 RVA: 0x001E2F0C File Offset: 0x001E110C
		public static GameObject Get(this GameObject prefab, Transform parent)
		{
			return prefab.Get(parent.position, parent.rotation, parent);
		}

		// Token: 0x06005EA9 RID: 24233 RVA: 0x001E2F24 File Offset: 0x001E1124
		public static GameObject Get(this GameObject prefab, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject;
			Pool.GetOrCreatePool(prefab).Get(out gameObject);
			gameObject.transform.SetPositionAndRotation(position, rotation);
			gameObject.gameObject.SetActive(true);
			return gameObject;
		}

		// Token: 0x06005EAA RID: 24234 RVA: 0x001E2F5C File Offset: 0x001E115C
		public static GameObject Get(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
		{
			GameObject gameObject;
			Pool.GetOrCreatePool(prefab).Get(out gameObject);
			gameObject.transform.SetParent(parent);
			gameObject.transform.SetPositionAndRotation(position, rotation);
			gameObject.gameObject.SetActive(true);
			return gameObject;
		}

		// Token: 0x06005EAB RID: 24235 RVA: 0x001E2F9D File Offset: 0x001E119D
		public static GameObject GetUninstantiated(this GameObject prefab)
		{
			return prefab.GetUninstantiated(Vector3.zero, Quaternion.identity);
		}

		// Token: 0x06005EAC RID: 24236 RVA: 0x001E2FB0 File Offset: 0x001E11B0
		public static GameObject GetUninstantiated(this GameObject prefab, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject;
			Pool.GetOrCreatePool(prefab).Get(out gameObject);
			gameObject.transform.SetPositionAndRotation(position, rotation);
			return gameObject;
		}

		// Token: 0x06005EAD RID: 24237 RVA: 0x001E2FD9 File Offset: 0x001E11D9
		public static GameObject GetUninstantiated(this GameObject prefab, Transform parent)
		{
			return prefab.GetUninstantiated(parent.position, parent.rotation, parent);
		}

		// Token: 0x06005EAE RID: 24238 RVA: 0x001E2FF0 File Offset: 0x001E11F0
		public static GameObject GetUninstantiated(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
		{
			GameObject gameObject;
			Pool.GetOrCreatePool(prefab).Get(out gameObject);
			gameObject.transform.SetParent(parent);
			gameObject.transform.SetPositionAndRotation(position, rotation);
			return gameObject;
		}

		// Token: 0x06005EAF RID: 24239 RVA: 0x001E3028 File Offset: 0x001E1228
		public static void Release(this GameObject prefab, GameObject instance)
		{
			if (!instance)
			{
				return;
			}
			IObjectPool<GameObject> objectPool;
			if (Pool.poolDict.TryGetValue(prefab, out objectPool))
			{
				objectPool.Release(instance);
				instance.transform.localScale = prefab.transform.localScale;
			}
		}

		// Token: 0x06005EB0 RID: 24240 RVA: 0x001E306C File Offset: 0x001E126C
		public static void DestroyPool(GameObject prefab)
		{
			IObjectPool<GameObject> pool = Pool.GetPool(prefab);
			if (pool == null)
			{
				return;
			}
			pool.Clear();
			Pool.poolDict.Remove(prefab);
		}

		// Token: 0x06005EB1 RID: 24241 RVA: 0x00002C2D File Offset: 0x00000E2D
		private static void OnGet(GameObject instance)
		{
		}

		// Token: 0x06005EB2 RID: 24242 RVA: 0x001E3096 File Offset: 0x001E1296
		private static void OnRelease(GameObject instance)
		{
			if (GTAppState.isQuitting)
			{
				return;
			}
			instance.SetActive(false);
			Pool.ChildToPoolRoot(instance.transform);
		}

		// Token: 0x06005EB3 RID: 24243 RVA: 0x001E30B2 File Offset: 0x001E12B2
		private static void OnDestroy(GameObject instance)
		{
			Object.Destroy(instance.gameObject);
		}

		// Token: 0x04006D86 RID: 28038
		private static readonly Dictionary<GameObject, IObjectPool<GameObject>> poolDict = new Dictionary<GameObject, IObjectPool<GameObject>>();

		// Token: 0x04006D87 RID: 28039
		private const Transform PoolRoot = null;
	}
}
