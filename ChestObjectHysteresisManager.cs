using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000522 RID: 1314
[DefaultExecutionOrder(2000)]
public class ChestObjectHysteresisManager : MonoBehaviourTick
{
	// Token: 0x060020E6 RID: 8422 RVA: 0x000B0892 File Offset: 0x000AEA92
	protected void Awake()
	{
		if (ChestObjectHysteresisManager.hasInstance && ChestObjectHysteresisManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ChestObjectHysteresisManager.SetInstance(this);
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x000B08B5 File Offset: 0x000AEAB5
	public static void CreateManager()
	{
		ChestObjectHysteresisManager.SetInstance(new GameObject("ChestObjectHysteresisManager").AddComponent<ChestObjectHysteresisManager>());
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x000B08CB File Offset: 0x000AEACB
	private static void SetInstance(ChestObjectHysteresisManager manager)
	{
		ChestObjectHysteresisManager.instance = manager;
		ChestObjectHysteresisManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x000B08E6 File Offset: 0x000AEAE6
	public static void RegisterCH(ChestObjectHysteresis cOH)
	{
		if (!ChestObjectHysteresisManager.hasInstance)
		{
			ChestObjectHysteresisManager.CreateManager();
		}
		if (!ChestObjectHysteresisManager.allChests.Contains(cOH))
		{
			ChestObjectHysteresisManager.allChests.Add(cOH);
		}
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x000B090C File Offset: 0x000AEB0C
	public static void UnregisterCH(ChestObjectHysteresis cOH)
	{
		if (!ChestObjectHysteresisManager.hasInstance)
		{
			ChestObjectHysteresisManager.CreateManager();
		}
		if (ChestObjectHysteresisManager.allChests.Contains(cOH))
		{
			ChestObjectHysteresisManager.allChests.Remove(cOH);
		}
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x000B0934 File Offset: 0x000AEB34
	public override void Tick()
	{
		for (int i = 0; i < ChestObjectHysteresisManager.allChests.Count; i++)
		{
			ChestObjectHysteresisManager.allChests[i].InvokeUpdate();
		}
	}

	// Token: 0x04002BA7 RID: 11175
	public static ChestObjectHysteresisManager instance;

	// Token: 0x04002BA8 RID: 11176
	public static bool hasInstance = false;

	// Token: 0x04002BA9 RID: 11177
	public static List<ChestObjectHysteresis> allChests = new List<ChestObjectHysteresis>();
}
