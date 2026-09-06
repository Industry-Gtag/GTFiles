using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F93 RID: 3987
	public class GorillaIntervalTimerManager : MonoBehaviour
	{
		// Token: 0x06006329 RID: 25385 RVA: 0x001FEC5A File Offset: 0x001FCE5A
		protected void Awake()
		{
			if (GorillaIntervalTimerManager.hasInstance && GorillaIntervalTimerManager.instance != null && GorillaIntervalTimerManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaIntervalTimerManager.SetInstance(this);
		}

		// Token: 0x0600632A RID: 25386 RVA: 0x001FEC8A File Offset: 0x001FCE8A
		private static void CreateManager()
		{
			GorillaIntervalTimerManager.SetInstance(new GameObject("GorillaIntervalTimerManager").AddComponent<GorillaIntervalTimerManager>());
		}

		// Token: 0x0600632B RID: 25387 RVA: 0x001FECA0 File Offset: 0x001FCEA0
		private static void SetInstance(GorillaIntervalTimerManager manager)
		{
			GorillaIntervalTimerManager.instance = manager;
			GorillaIntervalTimerManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x0600632C RID: 25388 RVA: 0x001FECBB File Offset: 0x001FCEBB
		public static void RegisterGorillaTimer(GorillaIntervalTimer gTimer)
		{
			if (!GorillaIntervalTimerManager.hasInstance)
			{
				GorillaIntervalTimerManager.CreateManager();
			}
			if (!GorillaIntervalTimerManager.allTimers.Contains(gTimer))
			{
				GorillaIntervalTimerManager.allTimers.Add(gTimer);
			}
		}

		// Token: 0x0600632D RID: 25389 RVA: 0x001FECE1 File Offset: 0x001FCEE1
		public static void UnregisterGorillaTimer(GorillaIntervalTimer gTimer)
		{
			if (!GorillaIntervalTimerManager.hasInstance)
			{
				GorillaIntervalTimerManager.CreateManager();
			}
			if (GorillaIntervalTimerManager.allTimers.Contains(gTimer))
			{
				GorillaIntervalTimerManager.allTimers.Remove(gTimer);
			}
		}

		// Token: 0x0600632E RID: 25390 RVA: 0x001FED08 File Offset: 0x001FCF08
		private void Update()
		{
			for (int i = 0; i < GorillaIntervalTimerManager.allTimers.Count; i++)
			{
				GorillaIntervalTimerManager.allTimers[i].InvokeUpdate();
			}
		}

		// Token: 0x040071DE RID: 29150
		private static GorillaIntervalTimerManager instance;

		// Token: 0x040071DF RID: 29151
		private static bool hasInstance = false;

		// Token: 0x040071E0 RID: 29152
		private static List<GorillaIntervalTimer> allTimers = new List<GorillaIntervalTimer>();
	}
}
