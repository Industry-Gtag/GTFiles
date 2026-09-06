using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

// Token: 0x0200049A RID: 1178
public class PlayerLoopPruning : MonoBehaviour
{
	// Token: 0x06001C96 RID: 7318 RVA: 0x0009AE1C File Offset: 0x0009901C
	private void Start()
	{
		this.isAndroid = Application.platform == RuntimePlatform.Android;
		PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
		PlayerLoop.SetPlayerLoop(this.RemoveSystem<PreLateUpdate>(in currentPlayerLoop));
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x0009AE4C File Offset: 0x0009904C
	private PlayerLoopSystem RemoveSystem<T>(in PlayerLoopSystem loopSystem) where T : struct
	{
		PlayerLoopSystem playerLoopSystem = new PlayerLoopSystem
		{
			loopConditionFunction = loopSystem.loopConditionFunction,
			type = loopSystem.type,
			updateDelegate = loopSystem.updateDelegate,
			updateFunction = loopSystem.updateFunction
		};
		List<PlayerLoopSystem> list = new List<PlayerLoopSystem>();
		if (loopSystem.subSystemList != null)
		{
			for (int i = 0; i < loopSystem.subSystemList.Length; i++)
			{
				PlayerLoopSystem playerLoopSystem2 = loopSystem.subSystemList[i];
				PlayerLoopSystem playerLoopSystem3 = new PlayerLoopSystem
				{
					loopConditionFunction = playerLoopSystem2.loopConditionFunction,
					type = playerLoopSystem2.type,
					updateDelegate = playerLoopSystem2.updateDelegate,
					updateFunction = playerLoopSystem2.updateFunction
				};
				if (playerLoopSystem2.subSystemList != null)
				{
					List<PlayerLoopSystem> list2 = new List<PlayerLoopSystem>();
					for (int j = 0; j < playerLoopSystem2.subSystemList.Length; j++)
					{
						if (!this.removeSubsystemList.Contains(playerLoopSystem2.subSystemList[j].type.Name) && (!this.isAndroid || !this.androidSubsystemExtras.Contains(playerLoopSystem2.subSystemList[j].type.Name)))
						{
							list2.Add(playerLoopSystem2.subSystemList[j]);
						}
					}
					playerLoopSystem3.subSystemList = list2.ToArray();
				}
				list.Add(playerLoopSystem3);
			}
		}
		PlayerLoopSystem playerLoopSystem4 = new PlayerLoopSystem
		{
			type = typeof(PlayerLoopPruning),
			updateDelegate = new PlayerLoopSystem.UpdateFunction(PlayerLoopPruning.PhaseSyncDestroyer3000Start)
		};
		PlayerLoopSystem playerLoopSystem5 = new PlayerLoopSystem
		{
			type = typeof(PlayerLoopPruning),
			updateDelegate = new PlayerLoopSystem.UpdateFunction(PlayerLoopPruning.PhaseSyncDestroyer3000End)
		};
		list.Insert(0, playerLoopSystem4);
		list.Add(playerLoopSystem5);
		playerLoopSystem.subSystemList = list.ToArray();
		return playerLoopSystem;
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x0009B03C File Offset: 0x0009923C
	private static void PhaseSyncDestroyer3000Start()
	{
		PlayerLoopPruning.slop = (float)PlayerLoopPruning.sw.ElapsedTicks / 10000000f * 0.1f + PlayerLoopPruning.slop * 0.9f;
		PlayerLoopPruning.sw.Restart();
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x0009B070 File Offset: 0x00099270
	private static void PhaseSyncDestroyer3000End()
	{
		long elapsedTicks = PlayerLoopPruning.sw.ElapsedTicks;
		long num = (long)((1f / (float)Application.targetFrameRate - PlayerLoopPruning.slop) * 10000000f);
		long num2 = num - elapsedTicks;
		num2 -= GorillaSimpleBackgroundWorkerManager.DoWork(num2);
		if (num2 < 0L)
		{
			PlayerLoopPruning.sw.Restart();
			return;
		}
		Thread.Sleep((int)(num2 / 10000L));
		while (PlayerLoopPruning.sw.ElapsedTicks < num)
		{
			Thread.Sleep(0);
		}
		PlayerLoopPruning.sw.Restart();
	}

	// Token: 0x040026B1 RID: 9905
	public List<string> removeSubsystemList;

	// Token: 0x040026B2 RID: 9906
	public List<string> androidSubsystemExtras;

	// Token: 0x040026B3 RID: 9907
	private bool isAndroid;

	// Token: 0x040026B4 RID: 9908
	private static Stopwatch sw = new Stopwatch();

	// Token: 0x040026B5 RID: 9909
	private static float slop = 0.0002f;
}
