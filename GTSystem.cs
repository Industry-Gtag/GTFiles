using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000921 RID: 2337
[DisallowMultipleComponent]
public abstract class GTSystem<T> : MonoBehaviour, IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : MonoBehaviour
{
	// Token: 0x1700059D RID: 1437
	// (get) Token: 0x06003D22 RID: 15650 RVA: 0x0014C9AE File Offset: 0x0014ABAE
	public PhotonView photonView
	{
		get
		{
			return this._photonView;
		}
	}

	// Token: 0x06003D23 RID: 15651 RVA: 0x0014C9B6 File Offset: 0x0014ABB6
	protected virtual void Awake()
	{
		GTSystem<T>.SetSingleton(this);
	}

	// Token: 0x06003D24 RID: 15652 RVA: 0x0014C9C0 File Offset: 0x0014ABC0
	protected virtual void Tick()
	{
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < this._instances.Count; i++)
		{
			T t = this._instances[i];
			if (t)
			{
				this.OnTick(deltaTime, t);
			}
		}
	}

	// Token: 0x06003D25 RID: 15653 RVA: 0x0014CA0B File Offset: 0x0014AC0B
	protected virtual void OnApplicationQuit()
	{
		GTSystem<T>.gAppQuitting = true;
	}

	// Token: 0x06003D26 RID: 15654 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnTick(float dt, T instance)
	{
	}

	// Token: 0x06003D27 RID: 15655 RVA: 0x0014CA14 File Offset: 0x0014AC14
	private bool RegisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError<string>("[" + base.GetType().Name + "::Register] Instance is null.", null);
			return false;
		}
		if (this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Add(instance);
		this.OnRegister(instance);
		return true;
	}

	// Token: 0x06003D28 RID: 15656 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnRegister(T instance)
	{
	}

	// Token: 0x06003D29 RID: 15657 RVA: 0x0014CA78 File Offset: 0x0014AC78
	private bool UnregisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError<string>("[" + base.GetType().Name + "::Unregister] Instance is null.", null);
			return false;
		}
		if (!this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Remove(instance);
		this.OnUnregister(instance);
		return true;
	}

	// Token: 0x06003D2A RID: 15658 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnUnregister(T instance)
	{
	}

	// Token: 0x06003D2B RID: 15659 RVA: 0x0014CADA File Offset: 0x0014ACDA
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return ((IEnumerable<T>)this._instances).GetEnumerator();
	}

	// Token: 0x06003D2C RID: 15660 RVA: 0x0014CADA File Offset: 0x0014ACDA
	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<T>)this._instances).GetEnumerator();
	}

	// Token: 0x1700059E RID: 1438
	// (get) Token: 0x06003D2D RID: 15661 RVA: 0x0014CAE7 File Offset: 0x0014ACE7
	int IReadOnlyCollection<T>.Count
	{
		get
		{
			return this._instances.Count;
		}
	}

	// Token: 0x1700059F RID: 1439
	T IReadOnlyList<T>.this[int index]
	{
		get
		{
			return this._instances[index];
		}
	}

	// Token: 0x170005A0 RID: 1440
	// (get) Token: 0x06003D2F RID: 15663 RVA: 0x0014CB02 File Offset: 0x0014AD02
	public static PhotonView PhotonView
	{
		get
		{
			return GTSystem<T>.gSingleton._photonView;
		}
	}

	// Token: 0x06003D30 RID: 15664 RVA: 0x0014CB10 File Offset: 0x0014AD10
	protected static void SetSingleton(GTSystem<T> system)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (GTSystem<T>.gSingleton != null && GTSystem<T>.gSingleton != system)
		{
			Object.Destroy(system);
			GTDev.LogWarning<string>("Singleton of type " + GTSystem<T>.gSingleton.GetType().Name + " already exists.", null);
			return;
		}
		GTSystem<T>.gSingleton = system;
		if (!GTSystem<T>.gInitializing)
		{
			return;
		}
		GTSystem<T>.gSingleton._instances.Clear();
		T[] array = GTSystem<T>.gQueueRegister.Where((T x) => x != null).ToArray<T>();
		GTSystem<T>.gSingleton._instances.AddRange(array);
		GTSystem<T>.gQueueRegister.Clear();
		PhotonView component = GTSystem<T>.gSingleton.GetComponent<PhotonView>();
		if (component != null)
		{
			GTSystem<T>.gSingleton._photonView = component;
			GTSystem<T>.gSingleton._networked = true;
		}
		GTSystem<T>.gInitializing = false;
	}

	// Token: 0x06003D31 RID: 15665 RVA: 0x0014CC00 File Offset: 0x0014AE00
	public static void Register(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		GTSystem<T>.gSingleton.RegisterInstance(instance);
	}

	// Token: 0x06003D32 RID: 15666 RVA: 0x0014CC6C File Offset: 0x0014AE6C
	public static void Unregister(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		GTSystem<T>.gSingleton.UnregisterInstance(instance);
	}

	// Token: 0x04004DDE RID: 19934
	[SerializeField]
	protected List<T> _instances = new List<T>();

	// Token: 0x04004DDF RID: 19935
	[SerializeField]
	private bool _networked;

	// Token: 0x04004DE0 RID: 19936
	[SerializeField]
	private PhotonView _photonView;

	// Token: 0x04004DE1 RID: 19937
	private static GTSystem<T> gSingleton;

	// Token: 0x04004DE2 RID: 19938
	private static bool gInitializing = false;

	// Token: 0x04004DE3 RID: 19939
	private static bool gAppQuitting = false;

	// Token: 0x04004DE4 RID: 19940
	private static HashSet<T> gQueueRegister = new HashSet<T>();
}
