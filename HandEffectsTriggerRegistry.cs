using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.Shared.Scripts.Utilities;
using TagEffects;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x020003DD RID: 989
[DefaultExecutionOrder(10000)]
public class HandEffectsTriggerRegistry : MonoBehaviour, ITickSystemTick, ITickSystemPost
{
	// Token: 0x1700024B RID: 587
	// (get) Token: 0x0600178D RID: 6029 RVA: 0x0008770F File Offset: 0x0008590F
	// (set) Token: 0x0600178E RID: 6030 RVA: 0x00087717 File Offset: 0x00085917
	public bool TickRunning { get; set; }

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x0600178F RID: 6031 RVA: 0x00087720 File Offset: 0x00085920
	// (set) Token: 0x06001790 RID: 6032 RVA: 0x00087728 File Offset: 0x00085928
	public bool PostTickRunning { get; set; }

	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06001791 RID: 6033 RVA: 0x00087731 File Offset: 0x00085931
	// (set) Token: 0x06001792 RID: 6034 RVA: 0x00087738 File Offset: 0x00085938
	public static HandEffectsTriggerRegistry Instance { get; private set; }

	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06001793 RID: 6035 RVA: 0x00087740 File Offset: 0x00085940
	// (set) Token: 0x06001794 RID: 6036 RVA: 0x00087747 File Offset: 0x00085947
	public static bool HasInstance { get; private set; }

	// Token: 0x06001795 RID: 6037 RVA: 0x0008774F File Offset: 0x0008594F
	public static void FindInstance()
	{
		HandEffectsTriggerRegistry.Instance = Object.FindAnyObjectByType<HandEffectsTriggerRegistry>();
		HandEffectsTriggerRegistry.HasInstance = true;
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x00087764 File Offset: 0x00085964
	private void Awake()
	{
		HandEffectsTriggerRegistry.Instance = this;
		HandEffectsTriggerRegistry.HasInstance = true;
		this.job = new HandEffectsTriggerRegistry.HandEffectsJob
		{
			positionInput = new NativeArray<Vector3>(50, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			closeOutput = new NativeArray<bool>(2500, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			actualListSize = this.actualListSz
		};
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x000877BC File Offset: 0x000859BC
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x000877CA File Offset: 0x000859CA
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06001799 RID: 6041 RVA: 0x000877D8 File Offset: 0x000859D8
	public void Register(IHandEffectsTrigger trigger)
	{
		if (this.triggers.Count < 50)
		{
			this.actualListSz++;
			this.triggers.Add(trigger);
		}
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x00087804 File Offset: 0x00085A04
	public void Unregister(IHandEffectsTrigger trigger)
	{
		int num = this.triggers.IndexOf(trigger);
		if (num >= 0)
		{
			this.actualListSz--;
			this.triggers.RemoveAt(num);
		}
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x0008783C File Offset: 0x00085A3C
	private void OnDestroy()
	{
		if (!this.jobHandle.IsCompleted)
		{
			this.jobHandle.Complete();
		}
		this.job.Dispose();
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x00087864 File Offset: 0x00085A64
	public void Tick()
	{
		this.CopyInput();
		this.jobHandle = this.job.Schedule(this.actualListSz, 20, default(JobHandle));
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x00087899 File Offset: 0x00085A99
	public void PostTick()
	{
		this.jobHandle.Complete();
		this.CheckForHandEffectOnProcessedOutput();
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x000878AC File Offset: 0x00085AAC
	public void CheckForHandEffectOnProcessedOutput()
	{
		this.newCollisionBits.Clear();
		for (int i = 0; i < this.triggers.Count; i++)
		{
			IHandEffectsTrigger handEffectsTrigger = this.triggers[i];
			int num = i * 50;
			for (int j = i + 1; j < this.triggers.Count; j++)
			{
				if (this.job.closeOutput[i * 50 + j])
				{
					IHandEffectsTrigger handEffectsTrigger2 = this.triggers[j];
					if (handEffectsTrigger.InTriggerZone(handEffectsTrigger2) || handEffectsTrigger2.InTriggerZone(handEffectsTrigger))
					{
						int num2 = num + j;
						this.newCollisionBits[num2] = true;
						if (!this.existingCollisionBits[num2] && Time.time - this.triggerTimes[i] > 0.5f && Time.time - this.triggerTimes[j] > 0.5f)
						{
							handEffectsTrigger.OnTriggerEntered(handEffectsTrigger2);
							handEffectsTrigger2.OnTriggerEntered(handEffectsTrigger);
							this.triggerTimes[i] = (this.triggerTimes[j] = Time.time);
						}
					}
				}
			}
		}
		this.existingCollisionBits.CopyFrom(this.newCollisionBits);
	}

	// Token: 0x0600179F RID: 6047 RVA: 0x000879D4 File Offset: 0x00085BD4
	private void CopyInput()
	{
		for (int i = 0; i < this.actualListSz; i++)
		{
			this.job.positionInput[i] = this.triggers[i].Transform.position;
		}
		if (this.job.actualListSize != this.actualListSz)
		{
			this.job.actualListSize = this.actualListSz;
		}
	}

	// Token: 0x040022CB RID: 8907
	private const int MAX_TRIGGERS = 50;

	// Token: 0x040022CC RID: 8908
	private const int BIT_ARRAY_SIZE = 2500;

	// Token: 0x040022CD RID: 8909
	private const float COOLDOWN_TIME = 0.5f;

	// Token: 0x040022CE RID: 8910
	private const float DEFAULT_RADIUS = 0.5f;

	// Token: 0x040022CF RID: 8911
	private readonly List<IHandEffectsTrigger> triggers = new List<IHandEffectsTrigger>();

	// Token: 0x040022D0 RID: 8912
	private readonly float[] triggerTimes = new float[50];

	// Token: 0x040022D1 RID: 8913
	private readonly GTBitArray existingCollisionBits = new GTBitArray(2500);

	// Token: 0x040022D2 RID: 8914
	private readonly GTBitArray newCollisionBits = new GTBitArray(2500);

	// Token: 0x040022D3 RID: 8915
	private int actualListSz;

	// Token: 0x040022D4 RID: 8916
	private JobHandle jobHandle;

	// Token: 0x040022D5 RID: 8917
	private HandEffectsTriggerRegistry.HandEffectsJob job;

	// Token: 0x020003DE RID: 990
	[BurstCompile]
	private struct HandEffectsJob : IJobParallelFor, IDisposable
	{
		// Token: 0x060017A1 RID: 6049 RVA: 0x00087A80 File Offset: 0x00085C80
		public void Execute(int i)
		{
			for (int j = i + 1; j < this.actualListSize; j++)
			{
				this.closeOutput[i * 50 + j] = (this.positionInput[i] - this.positionInput[j]).IsShorterThan(0.5f);
			}
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x00087AD8 File Offset: 0x00085CD8
		public void Dispose()
		{
			this.positionInput.Dispose();
			this.closeOutput.Dispose();
		}

		// Token: 0x040022DA RID: 8922
		[NativeDisableParallelForRestriction]
		public NativeArray<Vector3> positionInput;

		// Token: 0x040022DB RID: 8923
		[NativeDisableParallelForRestriction]
		public NativeArray<bool> closeOutput;

		// Token: 0x040022DC RID: 8924
		public int actualListSize;
	}
}
