using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020008A3 RID: 2211
public class GorillaSimpleBackgroundWorkerManager : MonoBehaviour
{
	// Token: 0x060039DE RID: 14814 RVA: 0x0013B4B6 File Offset: 0x001396B6
	protected void Awake()
	{
		if (GorillaSimpleBackgroundWorkerManager.hasInstance && GorillaSimpleBackgroundWorkerManager._instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaSimpleBackgroundWorkerManager.SetInstance(this);
	}

	// Token: 0x060039DF RID: 14815 RVA: 0x0013B4D9 File Offset: 0x001396D9
	private static void SetInstance(GorillaSimpleBackgroundWorkerManager manager)
	{
		GorillaSimpleBackgroundWorkerManager._instance = manager;
		GorillaSimpleBackgroundWorkerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060039E0 RID: 14816 RVA: 0x0013B4F4 File Offset: 0x001396F4
	public static void CreateManager()
	{
		GameObject gameObject = new GameObject("GorillaSimpleBackgroundWorkerManager");
		GorillaSimpleBackgroundWorkerManager gorillaSimpleBackgroundWorkerManager = gameObject.AddComponent<GorillaSimpleBackgroundWorkerManager>();
		Object.DontDestroyOnLoad(gameObject);
		GorillaSimpleBackgroundWorkerManager.SetInstance(gorillaSimpleBackgroundWorkerManager);
	}

	// Token: 0x060039E1 RID: 14817 RVA: 0x0013B51D File Offset: 0x0013971D
	public static long DoWork(long ticksOfWork)
	{
		if (!GorillaSimpleBackgroundWorkerManager.hasInstance)
		{
			GorillaSimpleBackgroundWorkerManager.CreateManager();
		}
		return GorillaSimpleBackgroundWorkerManager._instance._DoWork(ticksOfWork);
	}

	// Token: 0x060039E2 RID: 14818 RVA: 0x0013B538 File Offset: 0x00139738
	public long _DoWork(long ticksOfWork)
	{
		this.stopwatch.Restart();
		if (ticksOfWork < GorillaSimpleBackgroundWorkerManager.MINIMUM_TICKS_OF_WORK)
		{
			ticksOfWork = GorillaSimpleBackgroundWorkerManager.MINIMUM_TICKS_OF_WORK;
		}
		while (this.stopwatch.ElapsedTicks < ticksOfWork && this.workerSignups.Count > 0)
		{
			IGorillaSimpleBackgroundWorker gorillaSimpleBackgroundWorker = this.workerSignups.Dequeue();
			if (gorillaSimpleBackgroundWorker != null)
			{
				gorillaSimpleBackgroundWorker.SimpleWork();
			}
		}
		return this.stopwatch.ElapsedTicks;
	}

	// Token: 0x060039E3 RID: 14819 RVA: 0x0013B59E File Offset: 0x0013979E
	public static void WorkerSignup(IGorillaSimpleBackgroundWorker worker)
	{
		if (!GorillaSimpleBackgroundWorkerManager.hasInstance)
		{
			GorillaSimpleBackgroundWorkerManager.CreateManager();
		}
		GorillaSimpleBackgroundWorkerManager._instance.workerSignups.Enqueue(worker);
	}

	// Token: 0x040049F8 RID: 18936
	private static GorillaSimpleBackgroundWorkerManager _instance;

	// Token: 0x040049F9 RID: 18937
	private static bool hasInstance = false;

	// Token: 0x040049FA RID: 18938
	private static long MINIMUM_TICKS_OF_WORK = 10000L;

	// Token: 0x040049FB RID: 18939
	public Queue<IGorillaSimpleBackgroundWorker> workerSignups = new Queue<IGorillaSimpleBackgroundWorker>();

	// Token: 0x040049FC RID: 18940
	private Stopwatch stopwatch = new Stopwatch();
}
