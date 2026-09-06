using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200069F RID: 1695
public class FlockingUpdateManager : MonoBehaviour
{
	// Token: 0x06002A4A RID: 10826 RVA: 0x000E45D4 File Offset: 0x000E27D4
	protected void Awake()
	{
		if (FlockingUpdateManager.hasInstance && FlockingUpdateManager.instance != null && FlockingUpdateManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		FlockingUpdateManager.SetInstance(this);
	}

	// Token: 0x06002A4B RID: 10827 RVA: 0x000E4604 File Offset: 0x000E2804
	public static void CreateManager()
	{
		FlockingUpdateManager.SetInstance(new GameObject("FlockingUpdateManager").AddComponent<FlockingUpdateManager>());
	}

	// Token: 0x06002A4C RID: 10828 RVA: 0x000E461A File Offset: 0x000E281A
	private static void SetInstance(FlockingUpdateManager manager)
	{
		FlockingUpdateManager.instance = manager;
		FlockingUpdateManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06002A4D RID: 10829 RVA: 0x000E4635 File Offset: 0x000E2835
	public static void RegisterFlocking(Flocking flocking)
	{
		if (!FlockingUpdateManager.hasInstance)
		{
			FlockingUpdateManager.CreateManager();
		}
		if (!FlockingUpdateManager.allFlockings.Contains(flocking))
		{
			FlockingUpdateManager.allFlockings.Add(flocking);
		}
	}

	// Token: 0x06002A4E RID: 10830 RVA: 0x000E465B File Offset: 0x000E285B
	public static void UnregisterFlocking(Flocking flocking)
	{
		if (!FlockingUpdateManager.hasInstance)
		{
			FlockingUpdateManager.CreateManager();
		}
		if (FlockingUpdateManager.allFlockings.Contains(flocking))
		{
			FlockingUpdateManager.allFlockings.Remove(flocking);
		}
	}

	// Token: 0x06002A4F RID: 10831 RVA: 0x000E4684 File Offset: 0x000E2884
	public void Update()
	{
		for (int i = 0; i < FlockingUpdateManager.allFlockings.Count; i++)
		{
			FlockingUpdateManager.allFlockings[i].InvokeUpdate();
		}
	}

	// Token: 0x04003721 RID: 14113
	public static FlockingUpdateManager instance;

	// Token: 0x04003722 RID: 14114
	public static bool hasInstance = false;

	// Token: 0x04003723 RID: 14115
	public static List<Flocking> allFlockings = new List<Flocking>();
}
