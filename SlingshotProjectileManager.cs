using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004C6 RID: 1222
public class SlingshotProjectileManager : MonoBehaviourTick
{
	// Token: 0x06001DE3 RID: 7651 RVA: 0x000A14A5 File Offset: 0x0009F6A5
	protected void Awake()
	{
		if (SlingshotProjectileManager.hasInstance && SlingshotProjectileManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		SlingshotProjectileManager.SetInstance(this);
	}

	// Token: 0x06001DE4 RID: 7652 RVA: 0x000A14C8 File Offset: 0x0009F6C8
	public static void CreateManager()
	{
		SlingshotProjectileManager.SetInstance(new GameObject("SlingshotProjectileManager").AddComponent<SlingshotProjectileManager>());
	}

	// Token: 0x06001DE5 RID: 7653 RVA: 0x000A14DE File Offset: 0x0009F6DE
	private static void SetInstance(SlingshotProjectileManager manager)
	{
		SlingshotProjectileManager.instance = manager;
		SlingshotProjectileManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001DE6 RID: 7654 RVA: 0x000A14F9 File Offset: 0x0009F6F9
	public static void RegisterSP(SlingshotProjectile sP)
	{
		if (!SlingshotProjectileManager.hasInstance)
		{
			SlingshotProjectileManager.CreateManager();
		}
		if (!SlingshotProjectileManager.allsP.Contains(sP))
		{
			SlingshotProjectileManager.allsP.Add(sP);
		}
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x000A151F File Offset: 0x0009F71F
	public static void UnregisterSP(SlingshotProjectile sP)
	{
		if (!SlingshotProjectileManager.hasInstance)
		{
			SlingshotProjectileManager.CreateManager();
		}
		if (SlingshotProjectileManager.allsP.Contains(sP))
		{
			SlingshotProjectileManager.allsP.Remove(sP);
		}
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x000A1548 File Offset: 0x0009F748
	public override void Tick()
	{
		for (int i = 0; i < SlingshotProjectileManager.allsP.Count; i++)
		{
			SlingshotProjectileManager.allsP[i].InvokeUpdate();
		}
	}

	// Token: 0x0400283C RID: 10300
	public static SlingshotProjectileManager instance;

	// Token: 0x0400283D RID: 10301
	public static bool hasInstance = false;

	// Token: 0x0400283E RID: 10302
	public static List<SlingshotProjectile> allsP = new List<SlingshotProjectile>();
}
