using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F98 RID: 3992
	public class GorillaTimerManager : MonoBehaviour
	{
		// Token: 0x0600635A RID: 25434 RVA: 0x001FF2E3 File Offset: 0x001FD4E3
		protected void Awake()
		{
			if (GorillaTimerManager.hasInstance && GorillaTimerManager.instance != null && GorillaTimerManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaTimerManager.SetInstance(this);
		}

		// Token: 0x0600635B RID: 25435 RVA: 0x001FF313 File Offset: 0x001FD513
		public static void CreateManager()
		{
			GorillaTimerManager.SetInstance(new GameObject("GorillaTimerManager").AddComponent<GorillaTimerManager>());
		}

		// Token: 0x0600635C RID: 25436 RVA: 0x001FF329 File Offset: 0x001FD529
		private static void SetInstance(GorillaTimerManager manager)
		{
			GorillaTimerManager.instance = manager;
			GorillaTimerManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x0600635D RID: 25437 RVA: 0x001FF344 File Offset: 0x001FD544
		public static void RegisterGorillaTimer(GorillaTimer gTimer)
		{
			if (!GorillaTimerManager.hasInstance)
			{
				GorillaTimerManager.CreateManager();
			}
			if (!GorillaTimerManager.allTimers.Contains(gTimer))
			{
				GorillaTimerManager.allTimers.Add(gTimer);
			}
		}

		// Token: 0x0600635E RID: 25438 RVA: 0x001FF36A File Offset: 0x001FD56A
		public static void UnregisterGorillaTimer(GorillaTimer gTimer)
		{
			if (!GorillaTimerManager.hasInstance)
			{
				GorillaTimerManager.CreateManager();
			}
			if (GorillaTimerManager.allTimers.Contains(gTimer))
			{
				GorillaTimerManager.allTimers.Remove(gTimer);
			}
		}

		// Token: 0x0600635F RID: 25439 RVA: 0x001FF394 File Offset: 0x001FD594
		public void Update()
		{
			for (int i = 0; i < GorillaTimerManager.allTimers.Count; i++)
			{
				GorillaTimerManager.allTimers[i].InvokeUpdate();
			}
		}

		// Token: 0x040071FA RID: 29178
		public static GorillaTimerManager instance;

		// Token: 0x040071FB RID: 29179
		public static bool hasInstance = false;

		// Token: 0x040071FC RID: 29180
		public static List<GorillaTimer> allTimers = new List<GorillaTimer>();
	}
}
