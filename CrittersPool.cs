using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class CrittersPool : MonoBehaviour
{
	// Token: 0x060002D9 RID: 729 RVA: 0x000114FC File Offset: 0x0000F6FC
	public static GameObject GetPooled(GameObject prefab)
	{
		CrittersPool crittersPool = CrittersPool.instance;
		if (crittersPool == null)
		{
			return null;
		}
		return crittersPool.GetInstance(prefab);
	}

	// Token: 0x060002DA RID: 730 RVA: 0x0001150F File Offset: 0x0000F70F
	public static void Return(GameObject pooledGO)
	{
		CrittersPool crittersPool = CrittersPool.instance;
		if (crittersPool == null)
		{
			return;
		}
		crittersPool.ReturnInstance(pooledGO);
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00011521 File Offset: 0x0000F721
	private void Awake()
	{
		if (CrittersPool.instance != null)
		{
			Object.Destroy(this);
			return;
		}
		CrittersPool.instance = this;
		this.SetupPools();
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00011544 File Offset: 0x0000F744
	private void SetupPools()
	{
		this.pools = new Dictionary<GameObject, List<GameObject>>();
		this.poolParent = new GameObject("CrittersPool")
		{
			transform = 
			{
				parent = base.transform
			}
		}.transform;
		for (int i = 0; i < this.eventEffects.Length; i++)
		{
			CrittersPool.CrittersPoolSettings crittersPoolSettings = this.eventEffects[i];
			if (crittersPoolSettings.poolObject == null || crittersPoolSettings.poolSize <= 0)
			{
				GTDev.Log<string>("CrittersPool.SetupPools Failed. Pool has no poolObject or has size 0.", null);
			}
			else
			{
				List<GameObject> list = new List<GameObject>();
				for (int j = 0; j < crittersPoolSettings.poolSize; j++)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(crittersPoolSettings.poolObject);
					gameObject.transform.SetParent(this.poolParent);
					GameObject gameObject2 = gameObject;
					gameObject2.name += j.ToString();
					gameObject.SetActive(false);
					list.Add(gameObject);
				}
				this.pools.Add(crittersPoolSettings.poolObject, list);
			}
		}
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00011640 File Offset: 0x0000F840
	private GameObject GetInstance(GameObject prefab)
	{
		List<GameObject> list;
		if (this.pools.TryGetValue(prefab, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null && !list[i].activeSelf)
				{
					list[i].SetActive(true);
					return list[i];
				}
			}
			GTDev.Log<string>("CrittersPool.GetInstance Failed. No available instance.", null);
			return null;
		}
		GTDev.LogError<string>("CrittersPool.GetInstance Failed. Prefab doesn't have a valid pool setup.", null);
		return null;
	}

	// Token: 0x060002DE RID: 734 RVA: 0x000116B9 File Offset: 0x0000F8B9
	private void ReturnInstance(GameObject instance)
	{
		instance.transform.SetParent(this.poolParent);
		instance.SetActive(false);
	}

	// Token: 0x0400034C RID: 844
	private static CrittersPool instance;

	// Token: 0x0400034D RID: 845
	public CrittersPool.CrittersPoolSettings[] eventEffects;

	// Token: 0x0400034E RID: 846
	private Dictionary<GameObject, List<GameObject>> pools;

	// Token: 0x0400034F RID: 847
	public Transform poolParent;

	// Token: 0x02000076 RID: 118
	[Serializable]
	public class CrittersPoolSettings
	{
		// Token: 0x04000350 RID: 848
		public GameObject poolObject;

		// Token: 0x04000351 RID: 849
		public int poolSize = 20;
	}
}
