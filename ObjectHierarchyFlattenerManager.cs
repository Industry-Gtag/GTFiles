using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200035F RID: 863
public class ObjectHierarchyFlattenerManager : MonoBehaviourPostTick
{
	// Token: 0x0600151F RID: 5407 RVA: 0x000709D1 File Offset: 0x0006EBD1
	protected void Awake()
	{
		if (ObjectHierarchyFlattenerManager.hasInstance && ObjectHierarchyFlattenerManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ObjectHierarchyFlattenerManager.SetInstance(this);
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x000709F4 File Offset: 0x0006EBF4
	public static void CreateManager()
	{
		ObjectHierarchyFlattenerManager.SetInstance(new GameObject("ObjectHierarchyFlattenerManager").AddComponent<ObjectHierarchyFlattenerManager>());
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x00070A0A File Offset: 0x0006EC0A
	private static void SetInstance(ObjectHierarchyFlattenerManager manager)
	{
		ObjectHierarchyFlattenerManager.instance = manager;
		ObjectHierarchyFlattenerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x00070A25 File Offset: 0x0006EC25
	public static void RegisterOHF(ObjectHierarchyFlattener rbWI)
	{
		if (!ObjectHierarchyFlattenerManager.hasInstance)
		{
			ObjectHierarchyFlattenerManager.CreateManager();
		}
		if (!ObjectHierarchyFlattenerManager.alloHF.Contains(rbWI))
		{
			ObjectHierarchyFlattenerManager.alloHF.Add(rbWI);
		}
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x00070A4B File Offset: 0x0006EC4B
	public static void UnregisterOHF(ObjectHierarchyFlattener rbWI)
	{
		if (!ObjectHierarchyFlattenerManager.hasInstance)
		{
			ObjectHierarchyFlattenerManager.CreateManager();
		}
		if (ObjectHierarchyFlattenerManager.alloHF.Contains(rbWI))
		{
			ObjectHierarchyFlattenerManager.alloHF.Remove(rbWI);
		}
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x00070A74 File Offset: 0x0006EC74
	public override void PostTick()
	{
		for (int i = 0; i < ObjectHierarchyFlattenerManager.alloHF.Count; i++)
		{
			ObjectHierarchyFlattenerManager.alloHF[i].InvokeLateUpdate();
		}
	}

	// Token: 0x04001A09 RID: 6665
	public static ObjectHierarchyFlattenerManager instance;

	// Token: 0x04001A0A RID: 6666
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x04001A0B RID: 6667
	public static List<ObjectHierarchyFlattener> alloHF = new List<ObjectHierarchyFlattener>();
}
