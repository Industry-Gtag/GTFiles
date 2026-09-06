using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200092E RID: 2350
public class SizeManagerManager : MonoBehaviour
{
	// Token: 0x06003D8E RID: 15758 RVA: 0x0014E158 File Offset: 0x0014C358
	protected void Awake()
	{
		if (SizeManagerManager.hasInstance && SizeManagerManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		SizeManagerManager.SetInstance(this);
	}

	// Token: 0x06003D8F RID: 15759 RVA: 0x0014E17B File Offset: 0x0014C37B
	public static void CreateManager()
	{
		SizeManagerManager.SetInstance(new GameObject("SizeManagerManager").AddComponent<SizeManagerManager>());
	}

	// Token: 0x06003D90 RID: 15760 RVA: 0x0014E191 File Offset: 0x0014C391
	private static void SetInstance(SizeManagerManager manager)
	{
		SizeManagerManager.instance = manager;
		SizeManagerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06003D91 RID: 15761 RVA: 0x0014E1AC File Offset: 0x0014C3AC
	public static void RegisterSM(SizeManager sM)
	{
		if (!SizeManagerManager.hasInstance)
		{
			SizeManagerManager.CreateManager();
		}
		if (!SizeManagerManager.allSM.Contains(sM))
		{
			SizeManagerManager.allSM.Add(sM);
		}
	}

	// Token: 0x06003D92 RID: 15762 RVA: 0x0014E1D2 File Offset: 0x0014C3D2
	public static void UnregisterSM(SizeManager sM)
	{
		if (!SizeManagerManager.hasInstance)
		{
			SizeManagerManager.CreateManager();
		}
		if (SizeManagerManager.allSM.Contains(sM))
		{
			SizeManagerManager.allSM.Remove(sM);
		}
	}

	// Token: 0x06003D93 RID: 15763 RVA: 0x0014E1FC File Offset: 0x0014C3FC
	public void FixedUpdate()
	{
		for (int i = 0; i < SizeManagerManager.allSM.Count; i++)
		{
			SizeManagerManager.allSM[i].InvokeFixedUpdate();
		}
	}

	// Token: 0x04004E5A RID: 20058
	[OnEnterPlay_SetNull]
	public static SizeManagerManager instance;

	// Token: 0x04004E5B RID: 20059
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x04004E5C RID: 20060
	[OnEnterPlay_Clear]
	public static List<SizeManager> allSM = new List<SizeManager>();
}
