using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000DBD RID: 3517
[Serializable]
public class SinglePool : IGorillaSimpleBackgroundWorker
{
	// Token: 0x06005659 RID: 22105 RVA: 0x001C3928 File Offset: 0x001C1B28
	public void SimpleWork()
	{
		int count = this.inactivePool.Count;
		if (count >= this.initAmountToPool)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.objectToPool, this.gameObject.transform, true);
		gameObject.name = this.objectToPool.name + "(PoolIndex=" + count.ToString() + ")";
		gameObject.SetActive(false);
		this.inactivePool.Push(gameObject);
		this.amountAllocatedToPool++;
		int instanceID = gameObject.GetInstanceID();
		this.pooledObjects.Add(instanceID);
		GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
	}

	// Token: 0x0600565A RID: 22106 RVA: 0x001C39C5 File Offset: 0x001C1BC5
	private void PrivAllocPooledObjects()
	{
		if (this.inactivePool.Count == 0)
		{
			this.SimpleWork();
			return;
		}
		GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
	}

	// Token: 0x0600565B RID: 22107 RVA: 0x001C39E1 File Offset: 0x001C1BE1
	public void Initialize(GameObject gameObject_)
	{
		this.gameObject = gameObject_;
		this.activePool = new Dictionary<int, GameObject>(this.initAmountToPool);
		this.inactivePool = new Stack<GameObject>(this.initAmountToPool);
		this.pooledObjects = new HashSet<int>();
		GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
	}

	// Token: 0x0600565C RID: 22108 RVA: 0x001C3A20 File Offset: 0x001C1C20
	public GameObject Instantiate(bool setActive = true)
	{
		if (this.inactivePool.Count == 0)
		{
			Debug.LogWarning("Pool '" + this.objectToPool.name + "'is expanding consider changing initial pool size");
			this.PrivAllocPooledObjects();
		}
		GameObject gameObject = this.inactivePool.Pop();
		int instanceID = gameObject.GetInstanceID();
		gameObject.SetActive(setActive);
		this.activePool.Add(instanceID, gameObject);
		return gameObject;
	}

	// Token: 0x0600565D RID: 22109 RVA: 0x001C3A88 File Offset: 0x001C1C88
	public void Destroy(GameObject obj)
	{
		int instanceID = obj.GetInstanceID();
		if (!this.activePool.ContainsKey(instanceID))
		{
			return;
		}
		if (!this.pooledObjects.Contains(instanceID))
		{
			return;
		}
		obj.SetActive(false);
		this.inactivePool.Push(obj);
		this.activePool.Remove(instanceID);
	}

	// Token: 0x0600565E RID: 22110 RVA: 0x001C3ADA File Offset: 0x001C1CDA
	public int PoolGUID()
	{
		return PoolUtils.GameObjHashCode(this.objectToPool);
	}

	// Token: 0x0600565F RID: 22111 RVA: 0x001C3AE7 File Offset: 0x001C1CE7
	public int GetTotalCount()
	{
		return this.pooledObjects.Count;
	}

	// Token: 0x06005660 RID: 22112 RVA: 0x001C3AF4 File Offset: 0x001C1CF4
	public int GetActiveCount()
	{
		return this.activePool.Count;
	}

	// Token: 0x06005661 RID: 22113 RVA: 0x001C3B01 File Offset: 0x001C1D01
	public int GetInactiveCount()
	{
		return this.inactivePool.Count;
	}

	// Token: 0x04006777 RID: 26487
	public GameObject objectToPool;

	// Token: 0x04006778 RID: 26488
	public int initAmountToPool = 8;

	// Token: 0x04006779 RID: 26489
	private HashSet<int> pooledObjects;

	// Token: 0x0400677A RID: 26490
	private Stack<GameObject> inactivePool;

	// Token: 0x0400677B RID: 26491
	private Dictionary<int, GameObject> activePool;

	// Token: 0x0400677C RID: 26492
	private GameObject gameObject;

	// Token: 0x0400677D RID: 26493
	private int amountAllocatedToPool;
}
