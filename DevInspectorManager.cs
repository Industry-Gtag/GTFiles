using System;
using UnityEngine;

// Token: 0x0200032A RID: 810
public class DevInspectorManager : MonoBehaviour
{
	// Token: 0x17000205 RID: 517
	// (get) Token: 0x06001417 RID: 5143 RVA: 0x0006CC1B File Offset: 0x0006AE1B
	public static DevInspectorManager instance
	{
		get
		{
			if (DevInspectorManager._instance == null)
			{
				DevInspectorManager._instance = Object.FindAnyObjectByType<DevInspectorManager>();
			}
			return DevInspectorManager._instance;
		}
	}

	// Token: 0x040018FC RID: 6396
	private static DevInspectorManager _instance;
}
