using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000DBE RID: 3518
public class ObjectPools : MonoBehaviour, IBuildValidation
{
	// Token: 0x17000840 RID: 2112
	// (get) Token: 0x06005663 RID: 22115 RVA: 0x001C3B1D File Offset: 0x001C1D1D
	// (set) Token: 0x06005664 RID: 22116 RVA: 0x001C3B25 File Offset: 0x001C1D25
	public bool initialized { get; private set; }

	// Token: 0x06005665 RID: 22117 RVA: 0x001C3B2E File Offset: 0x001C1D2E
	protected void Awake()
	{
		ObjectPools.instance = this;
	}

	// Token: 0x06005666 RID: 22118 RVA: 0x001C3B36 File Offset: 0x001C1D36
	protected void Start()
	{
		this.InitializePools();
	}

	// Token: 0x06005667 RID: 22119 RVA: 0x001C3B40 File Offset: 0x001C1D40
	public void InitializePools()
	{
		if (this.initialized)
		{
			return;
		}
		this.lookUp = new Dictionary<int, SinglePool>();
		foreach (SinglePool singlePool in this.pools)
		{
			singlePool.Initialize(base.gameObject);
			int num = singlePool.PoolGUID();
			if (this.lookUp.ContainsKey(num))
			{
				using (List<SinglePool>.Enumerator enumerator2 = this.pools.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SinglePool singlePool2 = enumerator2.Current;
						if (singlePool2.PoolGUID() == num)
						{
							Debug.LogError("Pools contain more then one instance of the same object\n" + string.Format("First object in question is {0} tag: {1}\n", singlePool2.objectToPool, singlePool2.objectToPool.tag) + string.Format("Second object is {0} tag: {1}", singlePool.objectToPool, singlePool.objectToPool.tag));
							break;
						}
					}
					continue;
				}
			}
			this.lookUp.Add(singlePool.PoolGUID(), singlePool);
		}
		this.initialized = true;
	}

	// Token: 0x06005668 RID: 22120 RVA: 0x001C3C74 File Offset: 0x001C1E74
	public bool DoesPoolExist(GameObject obj)
	{
		return this.DoesPoolExist(PoolUtils.GameObjHashCode(obj));
	}

	// Token: 0x06005669 RID: 22121 RVA: 0x001C3C82 File Offset: 0x001C1E82
	public bool DoesPoolExist(int hash)
	{
		return this.lookUp.ContainsKey(hash);
	}

	// Token: 0x0600566A RID: 22122 RVA: 0x001C3C90 File Offset: 0x001C1E90
	public SinglePool GetPoolByHash(int hash)
	{
		return this.lookUp[hash];
	}

	// Token: 0x0600566B RID: 22123 RVA: 0x001C3CA0 File Offset: 0x001C1EA0
	public SinglePool GetPoolByObjectType(GameObject obj)
	{
		int num = PoolUtils.GameObjHashCode(obj);
		return this.GetPoolByHash(num);
	}

	// Token: 0x0600566C RID: 22124 RVA: 0x001C3CBB File Offset: 0x001C1EBB
	public GameObject Instantiate(GameObject obj, bool setActive = true)
	{
		return this.GetPoolByObjectType(obj).Instantiate(setActive);
	}

	// Token: 0x0600566D RID: 22125 RVA: 0x001C3CCA File Offset: 0x001C1ECA
	public GameObject Instantiate(int hash, bool setActive = true)
	{
		return this.GetPoolByHash(hash).Instantiate(setActive);
	}

	// Token: 0x0600566E RID: 22126 RVA: 0x001C3CD9 File Offset: 0x001C1ED9
	public GameObject Instantiate(int hash, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x0600566F RID: 22127 RVA: 0x001C3CEF File Offset: 0x001C1EEF
	public GameObject Instantiate(int hash, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x06005670 RID: 22128 RVA: 0x001C3D07 File Offset: 0x001C1F07
	public GameObject Instantiate(GameObject obj, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06005671 RID: 22129 RVA: 0x001C3D1D File Offset: 0x001C1F1D
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x06005672 RID: 22130 RVA: 0x001C3D35 File Offset: 0x001C1F35
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, float scale, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = Vector3.one * scale;
		return gameObject;
	}

	// Token: 0x06005673 RID: 22131 RVA: 0x001C3D64 File Offset: 0x001C1F64
	public void Destroy(GameObject obj)
	{
		this.GetPoolByObjectType(obj).Destroy(obj);
	}

	// Token: 0x06005674 RID: 22132 RVA: 0x001C3D74 File Offset: 0x001C1F74
	public bool BuildValidationCheck()
	{
		bool flag = true;
		using (List<SinglePool>.Enumerator enumerator = this.pools.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SinglePool pool = enumerator.Current;
				if (pool.objectToPool == null)
				{
					Debug.Log("GlobalObjectPools contains a nullref. Failing build validation.");
					flag = false;
				}
				else
				{
					DelayedDestroyPooledObj[] componentsInChildren = pool.objectToPool.GetComponentsInChildren<DelayedDestroyPooledObj>(true);
					if (componentsInChildren.Length > 1)
					{
						Debug.LogError(string.Format("Pooled prefab '{0}' has {1} ", pool.objectToPool.name, componentsInChildren.Length) + "DelayedDestroyPooledObj components in its hierarchy. Only the root should have one. Children with their own will try to pool-destroy themselves and spam 'not contained in the activePool' errors. Extra components on:" + string.Concat(Array.ConvertAll<DelayedDestroyPooledObj, string>(componentsInChildren, delegate(DelayedDestroyPooledObj c)
						{
							if (!(c.gameObject == pool.objectToPool))
							{
								return "\n  - " + c.gameObject.name;
							}
							return "";
						})), pool.objectToPool);
						flag = false;
					}
				}
			}
		}
		return flag;
	}

	// Token: 0x06005675 RID: 22133 RVA: 0x001C3E64 File Offset: 0x001C2064
	public static int InstantiateDelayed(GameObject prefab, Vector3 pos, float delay)
	{
		return ObjectPools.InstantiateDelayed(prefab, null, pos, delay);
	}

	// Token: 0x06005676 RID: 22134 RVA: 0x001C3E70 File Offset: 0x001C2070
	public static int InstantiateDelayed(GameObject prefab, Transform xform, Vector3 localPos, float delay)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return -1;
		}
		int num;
		if (ObjectPools._delayedFreeHead >= 0)
		{
			num = ObjectPools._delayedFreeHead;
			ObjectPools._delayedFreeHead = ObjectPools._delayedFreeNext[num];
		}
		else
		{
			if (ObjectPools._delayedHighWater >= ObjectPools._delayedData.Length)
			{
				int num2 = ObjectPools._delayedData.Length * 2;
				Array.Resize<ObjectPools.DelayedSpawnData>(ref ObjectPools._delayedData, num2);
				Array.Resize<int>(ref ObjectPools._delayedFreeNext, num2);
			}
			num = ObjectPools._delayedHighWater++;
		}
		ObjectPools._delayedData[num] = new ObjectPools.DelayedSpawnData
		{
			prefabHash = PoolUtils.GameObjHashCode(prefab),
			xform = xform,
			pos = localPos
		};
		GTDelayedExec.Add(ObjectPools._delayedListener, delay, num);
		return num;
	}

	// Token: 0x06005677 RID: 22135 RVA: 0x001C3F1C File Offset: 0x001C211C
	public static void UpdateDelayedInstantiate(int idx, Transform xform)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools._delayedData[idx].xform = xform;
	}

	// Token: 0x06005678 RID: 22136 RVA: 0x001C3F38 File Offset: 0x001C2138
	public static void UpdateDelayedInstantiate(int idx, Vector3 localPos)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools._delayedData[idx].pos = localPos;
	}

	// Token: 0x06005679 RID: 22137 RVA: 0x001C3F54 File Offset: 0x001C2154
	public static void CancelDelayedInstantiate(int idx)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools._delayedData[idx].prefabHash = 0;
	}

	// Token: 0x0600567A RID: 22138 RVA: 0x001C3F70 File Offset: 0x001C2170
	public static void UpdateDelayedInstantiate(int idx, Transform xform, Vector3 localPos)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools.DelayedSpawnData[] delayedData = ObjectPools._delayedData;
		delayedData[idx].xform = xform;
		delayedData[idx].pos = localPos;
	}

	// Token: 0x0400677E RID: 26494
	public static ObjectPools instance = null;

	// Token: 0x04006780 RID: 26496
	[SerializeField]
	private List<SinglePool> pools;

	// Token: 0x04006781 RID: 26497
	private Dictionary<int, SinglePool> lookUp;

	// Token: 0x04006782 RID: 26498
	private const int k_delayedInitialCount = 16;

	// Token: 0x04006783 RID: 26499
	[OnEnterPlay_Set(0)]
	private static int _delayedHighWater;

	// Token: 0x04006784 RID: 26500
	[OnEnterPlay_Set(-1)]
	private static int _delayedFreeHead = -1;

	// Token: 0x04006785 RID: 26501
	[OnEnterPlay_SetNew]
	private static ObjectPools.DelayedSpawnData[] _delayedData = new ObjectPools.DelayedSpawnData[16];

	// Token: 0x04006786 RID: 26502
	[OnEnterPlay_SetNew]
	private static int[] _delayedFreeNext = new int[16];

	// Token: 0x04006787 RID: 26503
	[OnEnterPlay_SetNew]
	private static readonly ObjectPools.DelayedSpawnListener _delayedListener = new ObjectPools.DelayedSpawnListener();

	// Token: 0x02000DBF RID: 3519
	private struct DelayedSpawnData
	{
		// Token: 0x04006788 RID: 26504
		public int prefabHash;

		// Token: 0x04006789 RID: 26505
		public Transform xform;

		// Token: 0x0400678A RID: 26506
		public Vector3 pos;
	}

	// Token: 0x02000DC0 RID: 3520
	private class DelayedSpawnListener : IDelayedExecListener
	{
		// Token: 0x0600567D RID: 22141 RVA: 0x001C3FC4 File Offset: 0x001C21C4
		public void OnDelayedAction(int contextId)
		{
			if (contextId >= ObjectPools._delayedHighWater)
			{
				return;
			}
			ref ObjectPools.DelayedSpawnData ptr = ref ObjectPools._delayedData[contextId];
			if (ptr.prefabHash != 0 && ObjectPools.instance != null)
			{
				Vector3 vector = ((ptr.xform != null) ? ptr.xform.TransformPoint(ptr.pos) : ptr.pos);
				ObjectPools.instance.Instantiate(ptr.prefabHash, vector, true);
			}
			ptr = default(ObjectPools.DelayedSpawnData);
			ObjectPools._delayedFreeNext[contextId] = ObjectPools._delayedFreeHead;
			ObjectPools._delayedFreeHead = contextId;
		}
	}
}
