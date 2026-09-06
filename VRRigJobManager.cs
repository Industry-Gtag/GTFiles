using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000AD8 RID: 2776
[DefaultExecutionOrder(0)]
public class VRRigJobManager : MonoBehaviour
{
	// Token: 0x170006AA RID: 1706
	// (get) Token: 0x06004747 RID: 18247 RVA: 0x00180933 File Offset: 0x0017EB33
	public static VRRigJobManager Instance
	{
		get
		{
			return VRRigJobManager._instance;
		}
	}

	// Token: 0x06004748 RID: 18248 RVA: 0x0018093A File Offset: 0x0017EB3A
	private void Awake()
	{
		VRRigJobManager._instance = this;
		this.cachedInput = new NativeArray<VRRigJobManager.VRRigTransformInput>(19, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.tAA = new TransformAccessArray(19, 2);
		this.job = default(VRRigJobManager.VRRigTransformJob);
	}

	// Token: 0x06004749 RID: 18249 RVA: 0x0018096B File Offset: 0x0017EB6B
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.cachedInput.Dispose();
		this.tAA.Dispose();
	}

	// Token: 0x0600474A RID: 18250 RVA: 0x0018098E File Offset: 0x0017EB8E
	public void RegisterVRRig(VRRig rig)
	{
		this.rigList.Add(rig);
		this.tAA.Add(rig.transform);
		this.actualListSz++;
	}

	// Token: 0x0600474B RID: 18251 RVA: 0x001809BC File Offset: 0x0017EBBC
	public void DeregisterVRRig(VRRig rig)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.rigList.Remove(rig);
		for (int i = this.actualListSz - 1; i >= 0; i--)
		{
			if (this.tAA[i] == rig.transform)
			{
				this.tAA.RemoveAtSwapBack(i);
				break;
			}
		}
		this.actualListSz--;
	}

	// Token: 0x0600474C RID: 18252 RVA: 0x00180A28 File Offset: 0x0017EC28
	private void CopyInput()
	{
		for (int i = 0; i < this.actualListSz; i++)
		{
			this.cachedInput[i] = new VRRigJobManager.VRRigTransformInput
			{
				rigPosition = this.rigList[i].jobPos,
				rigRotaton = this.rigList[i].jobRotation
			};
			this.tAA[i] = this.rigList[i].transform;
		}
	}

	// Token: 0x0600474D RID: 18253 RVA: 0x00180AA8 File Offset: 0x0017ECA8
	public void Update()
	{
		this.jobHandle.Complete();
		for (int i = 0; i < this.rigList.Count; i++)
		{
			this.rigList[i].RemoteRigUpdate();
		}
		this.CopyInput();
		this.job.input = this.cachedInput;
		this.jobHandle = this.job.Schedule(this.tAA, default(JobHandle));
	}

	// Token: 0x040059C7 RID: 22983
	[OnEnterPlay_SetNull]
	private static VRRigJobManager _instance;

	// Token: 0x040059C8 RID: 22984
	private const int MaxSize = 19;

	// Token: 0x040059C9 RID: 22985
	private const int questJobThreads = 2;

	// Token: 0x040059CA RID: 22986
	private List<VRRig> rigList = new List<VRRig>(19);

	// Token: 0x040059CB RID: 22987
	private NativeArray<VRRigJobManager.VRRigTransformInput> cachedInput;

	// Token: 0x040059CC RID: 22988
	private TransformAccessArray tAA;

	// Token: 0x040059CD RID: 22989
	private int actualListSz;

	// Token: 0x040059CE RID: 22990
	private JobHandle jobHandle;

	// Token: 0x040059CF RID: 22991
	private VRRigJobManager.VRRigTransformJob job;

	// Token: 0x02000AD9 RID: 2777
	private struct VRRigTransformInput
	{
		// Token: 0x040059D0 RID: 22992
		public Vector3 rigPosition;

		// Token: 0x040059D1 RID: 22993
		public Quaternion rigRotaton;
	}

	// Token: 0x02000ADA RID: 2778
	[BurstCompile]
	private struct VRRigTransformJob : IJobParallelForTransform
	{
		// Token: 0x0600474F RID: 18255 RVA: 0x00180B33 File Offset: 0x0017ED33
		public void Execute(int i, TransformAccess tA)
		{
			if (i < this.input.Length)
			{
				tA.position = this.input[i].rigPosition;
				tA.rotation = this.input[i].rigRotaton;
			}
		}

		// Token: 0x040059D2 RID: 22994
		[ReadOnly]
		public NativeArray<VRRigJobManager.VRRigTransformInput> input;
	}
}
