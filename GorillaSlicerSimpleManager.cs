using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020008A5 RID: 2213
public class GorillaSlicerSimpleManager : MonoBehaviour
{
	// Token: 0x060039E7 RID: 14823 RVA: 0x0013B5ED File Offset: 0x001397ED
	protected void Awake()
	{
		if (GorillaSlicerSimpleManager.hasInstance && GorillaSlicerSimpleManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaSlicerSimpleManager.SetInstance(this);
	}

	// Token: 0x060039E8 RID: 14824 RVA: 0x0013B610 File Offset: 0x00139810
	public static void CreateManager()
	{
		GorillaSlicerSimpleManager gorillaSlicerSimpleManager = new GameObject("GorillaSlicerSimpleManager").AddComponent<GorillaSlicerSimpleManager>();
		gorillaSlicerSimpleManager.fixedUpdateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.updateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.lateUpdateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.sW = new Stopwatch();
		GorillaSlicerSimpleManager.SetInstance(gorillaSlicerSimpleManager);
	}

	// Token: 0x060039E9 RID: 14825 RVA: 0x0013B65D File Offset: 0x0013985D
	private static void SetInstance(GorillaSlicerSimpleManager manager)
	{
		GorillaSlicerSimpleManager.instance = manager;
		GorillaSlicerSimpleManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060039EA RID: 14826 RVA: 0x00019260 File Offset: 0x00017460
	public static void RegisterSliceable(IGorillaSliceableSimple gSS)
	{
		GorillaSlicerSimpleManager.RegisterSliceable(gSS, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060039EB RID: 14827 RVA: 0x0013B678 File Offset: 0x00139878
	public static void RegisterSliceable(IGorillaSliceableSimple gSS, GorillaSlicerSimpleManager.UpdateStep step)
	{
		if (!GorillaSlicerSimpleManager.hasInstance)
		{
			GorillaSlicerSimpleManager.CreateManager();
		}
		GorillaSlicerSimpleManager.instance.lastRunTicks.TryAdd(gSS, 0L);
		switch (step)
		{
		case GorillaSlicerSimpleManager.UpdateStep.FixedUpdate:
			if (!GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Add(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.Update:
			if (!GorillaSlicerSimpleManager.instance.updateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.updateSlice.Add(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.LateUpdate:
			if (!GorillaSlicerSimpleManager.instance.lateUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.lateUpdateSlice.Add(gSS);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060039EC RID: 14828 RVA: 0x0013B71F File Offset: 0x0013991F
	public static bool UnregisterSliceable(IGorillaSliceableSimple gSS)
	{
		return GorillaSlicerSimpleManager.UnregisterSliceable(gSS, GorillaSlicerSimpleManager.UpdateStep.Update) || GorillaSlicerSimpleManager.UnregisterSliceable(gSS, GorillaSlicerSimpleManager.UpdateStep.LateUpdate) || GorillaSlicerSimpleManager.UnregisterSliceable(gSS, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x060039ED RID: 14829 RVA: 0x0013B740 File Offset: 0x00139940
	public static bool UnregisterSliceable(IGorillaSliceableSimple gSS, GorillaSlicerSimpleManager.UpdateStep step)
	{
		if (!GorillaSlicerSimpleManager.hasInstance)
		{
			GorillaSlicerSimpleManager.CreateManager();
		}
		switch (step)
		{
		case GorillaSlicerSimpleManager.UpdateStep.FixedUpdate:
			if (GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Remove(gSS);
				return true;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.Update:
			if (GorillaSlicerSimpleManager.instance.updateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.updateSlice.Remove(gSS);
				return true;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.LateUpdate:
			if (GorillaSlicerSimpleManager.instance.lateUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.lateUpdateSlice.Remove(gSS);
				return true;
			}
			break;
		}
		return false;
	}

	// Token: 0x060039EE RID: 14830 RVA: 0x0013B7E0 File Offset: 0x001399E0
	public void FixedUpdate()
	{
		this.startingIndex = this.updateIndex;
		if (this.updateIndex < 0 || this.updateIndex >= this.fixedUpdateSlice.Count + this.updateSlice.Count + this.lateUpdateSlice.Count)
		{
			this.updateIndex = 0;
		}
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && this.updateIndex < this.fixedUpdateSlice.Count)
		{
			IGorillaSliceableSimple gorillaSliceableSimple = this.fixedUpdateSlice[this.updateIndex];
			if (this.startingIndex != this.updateIndex && this.ticksThisFrame + this.sW.ElapsedTicks + this.lastRunTicks[gorillaSliceableSimple] >= this.ticksPerFrame)
			{
				this.ticksThisFrame = this.ticksPerFrame;
				break;
			}
			long elapsedTicks = this.sW.ElapsedTicks;
			if (0 <= this.updateIndex && this.updateIndex < this.fixedUpdateSlice.Count)
			{
				MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
				if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
				{
					try
					{
						gorillaSliceableSimple.SliceUpdate();
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}
			this.lastRunTicks[gorillaSliceableSimple] = this.sW.ElapsedTicks - elapsedTicks;
			this.updateIndex++;
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
	}

	// Token: 0x060039EF RID: 14831 RVA: 0x0013B970 File Offset: 0x00139B70
	public void Update()
	{
		int count = this.fixedUpdateSlice.Count;
		int count2 = this.updateSlice.Count;
		int num = count + count2;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && count <= this.updateIndex && this.updateIndex < num)
		{
			IGorillaSliceableSimple gorillaSliceableSimple = this.updateSlice[this.updateIndex - count];
			if (this.startingIndex != this.updateIndex && this.ticksThisFrame + this.sW.ElapsedTicks + this.lastRunTicks[gorillaSliceableSimple] >= this.ticksPerFrame)
			{
				this.ticksThisFrame = this.ticksPerFrame;
				break;
			}
			long elapsedTicks = this.sW.ElapsedTicks;
			if (0 <= this.updateIndex - count && this.updateIndex - count < this.updateSlice.Count)
			{
				MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
				if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
				{
					try
					{
						gorillaSliceableSimple.SliceUpdate();
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}
			this.lastRunTicks[gorillaSliceableSimple] = this.sW.ElapsedTicks - elapsedTicks;
			this.updateIndex++;
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
	}

	// Token: 0x060039F0 RID: 14832 RVA: 0x0013BAE0 File Offset: 0x00139CE0
	public void LateUpdate()
	{
		int count = this.fixedUpdateSlice.Count;
		int count2 = this.updateSlice.Count;
		int count3 = this.lateUpdateSlice.Count;
		int num = count + count2;
		int num2 = num + count3;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && num <= this.updateIndex && this.updateIndex < num2)
		{
			IGorillaSliceableSimple gorillaSliceableSimple = this.lateUpdateSlice[this.updateIndex - num];
			if (this.startingIndex != this.updateIndex && this.ticksThisFrame + this.sW.ElapsedTicks + this.lastRunTicks[gorillaSliceableSimple] >= this.ticksPerFrame)
			{
				this.ticksThisFrame = this.ticksPerFrame;
				break;
			}
			long elapsedTicks = this.sW.ElapsedTicks;
			if (0 <= this.updateIndex - num && this.updateIndex - num < this.lateUpdateSlice.Count)
			{
				MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
				if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
				{
					try
					{
						gorillaSliceableSimple.SliceUpdate();
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}
			this.lastRunTicks[gorillaSliceableSimple] = this.sW.ElapsedTicks - elapsedTicks;
			this.updateIndex++;
		}
		this.sW.Stop();
		if (this.updateIndex >= num2)
		{
			this.updateIndex = -1;
		}
		this.ticksThisFrame = 0L;
	}

	// Token: 0x040049FD RID: 18941
	public static GorillaSlicerSimpleManager instance;

	// Token: 0x040049FE RID: 18942
	public static bool hasInstance;

	// Token: 0x040049FF RID: 18943
	public List<IGorillaSliceableSimple> fixedUpdateSlice;

	// Token: 0x04004A00 RID: 18944
	public List<IGorillaSliceableSimple> updateSlice;

	// Token: 0x04004A01 RID: 18945
	public List<IGorillaSliceableSimple> lateUpdateSlice;

	// Token: 0x04004A02 RID: 18946
	public long ticksPerFrame = 1000L;

	// Token: 0x04004A03 RID: 18947
	public long ticksThisFrame;

	// Token: 0x04004A04 RID: 18948
	public int updateIndex = -1;

	// Token: 0x04004A05 RID: 18949
	public int startingIndex = -1;

	// Token: 0x04004A06 RID: 18950
	public Stopwatch sW;

	// Token: 0x04004A07 RID: 18951
	public Dictionary<IGorillaSliceableSimple, long> lastRunTicks = new Dictionary<IGorillaSliceableSimple, long>();

	// Token: 0x020008A6 RID: 2214
	public enum UpdateStep
	{
		// Token: 0x04004A09 RID: 18953
		FixedUpdate,
		// Token: 0x04004A0A RID: 18954
		Update,
		// Token: 0x04004A0B RID: 18955
		LateUpdate
	}
}
