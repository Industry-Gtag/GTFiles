using System;
using UnityEngine;

// Token: 0x02000D30 RID: 3376
public class GTDelayedExec : ITickSystemTick
{
	// Token: 0x170007E5 RID: 2021
	// (get) Token: 0x0600537D RID: 21373 RVA: 0x001B7F69 File Offset: 0x001B6169
	// (set) Token: 0x0600537E RID: 21374 RVA: 0x001B7F70 File Offset: 0x001B6170
	public static GTDelayedExec instance { get; private set; }

	// Token: 0x170007E6 RID: 2022
	// (get) Token: 0x0600537F RID: 21375 RVA: 0x001B7F78 File Offset: 0x001B6178
	// (set) Token: 0x06005380 RID: 21376 RVA: 0x001B7F7F File Offset: 0x001B617F
	public static int listenerCount { get; private set; }

	// Token: 0x06005381 RID: 21377 RVA: 0x001B7F87 File Offset: 0x001B6187
	[OnEnterPlay_Run]
	private static void EdReInit()
	{
		GTDelayedExec._listenerDelays = new float[1024];
		GTDelayedExec._listeners = new GTDelayedExec.Listener[1024];
	}

	// Token: 0x06005382 RID: 21378 RVA: 0x001B7FA7 File Offset: 0x001B61A7
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void InitializeAfterAssemblies()
	{
		GTDelayedExec.listenerCount = 0;
		GTDelayedExec.instance = new GTDelayedExec();
		TickSystem<object>.AddTickCallback(GTDelayedExec.instance);
	}

	// Token: 0x06005383 RID: 21379 RVA: 0x001B7FC4 File Offset: 0x001B61C4
	internal static void Add(IDelayedExecListener listener, float delay, int contextId)
	{
		if (GTDelayedExec.listenerCount >= GTDelayedExec.maxListenersCount)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"ERROR!!!  GTDelayedExec: Recovering from default maximum number of delayed listeners ",
				1024.ToString(),
				" reached. Please set the k_defaultMaxListenersCount value to ",
				(GTDelayedExec.maxListenersCount * 2).ToString(),
				"."
			}));
			GTDelayedExec.maxListenersCount *= 2;
			Array.Resize<float>(ref GTDelayedExec._listenerDelays, GTDelayedExec.maxListenersCount);
			Array.Resize<GTDelayedExec.Listener>(ref GTDelayedExec._listeners, GTDelayedExec.maxListenersCount);
		}
		GTDelayedExec._listenerDelays[GTDelayedExec.listenerCount] = Time.unscaledTime + delay;
		GTDelayedExec._listeners[GTDelayedExec.listenerCount] = new GTDelayedExec.Listener(listener, contextId);
		GTDelayedExec.listenerCount++;
	}

	// Token: 0x170007E7 RID: 2023
	// (get) Token: 0x06005384 RID: 21380 RVA: 0x001B8085 File Offset: 0x001B6285
	// (set) Token: 0x06005385 RID: 21381 RVA: 0x001B808D File Offset: 0x001B628D
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x06005386 RID: 21382 RVA: 0x001B8098 File Offset: 0x001B6298
	void ITickSystemTick.Tick()
	{
		for (int i = 0; i < GTDelayedExec.listenerCount; i++)
		{
			if (Time.unscaledTime >= GTDelayedExec._listenerDelays[i])
			{
				try
				{
					GTDelayedExec._listeners[i].listener.OnDelayedAction(GTDelayedExec._listeners[i].contextId);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
				GTDelayedExec.listenerCount--;
				GTDelayedExec._listenerDelays[i] = GTDelayedExec._listenerDelays[GTDelayedExec.listenerCount];
				GTDelayedExec._listeners[i] = GTDelayedExec._listeners[GTDelayedExec.listenerCount];
				i--;
			}
		}
	}

	// Token: 0x04006516 RID: 25878
	public const int k_defaultMaxListenersCount = 1024;

	// Token: 0x04006517 RID: 25879
	public static int maxListenersCount = 1024;

	// Token: 0x04006519 RID: 25881
	private static float[] _listenerDelays = new float[1024];

	// Token: 0x0400651A RID: 25882
	private static GTDelayedExec.Listener[] _listeners = new GTDelayedExec.Listener[1024];

	// Token: 0x02000D31 RID: 3377
	private struct Listener
	{
		// Token: 0x06005389 RID: 21385 RVA: 0x001B816E File Offset: 0x001B636E
		public Listener(IDelayedExecListener listener, int contextId)
		{
			this.listener = listener;
			this.contextId = contextId;
		}

		// Token: 0x0400651C RID: 25884
		public readonly IDelayedExecListener listener;

		// Token: 0x0400651D RID: 25885
		public readonly int contextId;
	}
}
