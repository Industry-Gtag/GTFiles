using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200142A RID: 5162
	public static class BoingManager
	{
		// Token: 0x17000C83 RID: 3203
		// (get) Token: 0x0600822F RID: 33327 RVA: 0x002A931C File Offset: 0x002A751C
		public static IEnumerable<BoingBehavior> Behaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Values;
			}
		}

		// Token: 0x17000C84 RID: 3204
		// (get) Token: 0x06008230 RID: 33328 RVA: 0x002A9328 File Offset: 0x002A7528
		public static IEnumerable<BoingReactor> Reactors
		{
			get
			{
				return BoingManager.s_reactorMap.Values;
			}
		}

		// Token: 0x17000C85 RID: 3205
		// (get) Token: 0x06008231 RID: 33329 RVA: 0x002A9334 File Offset: 0x002A7534
		public static IEnumerable<BoingEffector> Effectors
		{
			get
			{
				return BoingManager.s_effectorMap.Values;
			}
		}

		// Token: 0x17000C86 RID: 3206
		// (get) Token: 0x06008232 RID: 33330 RVA: 0x002A9340 File Offset: 0x002A7540
		public static IEnumerable<BoingReactorField> ReactorFields
		{
			get
			{
				return BoingManager.s_fieldMap.Values;
			}
		}

		// Token: 0x17000C87 RID: 3207
		// (get) Token: 0x06008233 RID: 33331 RVA: 0x002A934C File Offset: 0x002A754C
		public static IEnumerable<BoingReactorFieldCPUSampler> ReactorFieldCPUSamlers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Values;
			}
		}

		// Token: 0x17000C88 RID: 3208
		// (get) Token: 0x06008234 RID: 33332 RVA: 0x002A9358 File Offset: 0x002A7558
		public static IEnumerable<BoingReactorFieldGPUSampler> ReactorFieldGPUSampler
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Values;
			}
		}

		// Token: 0x17000C89 RID: 3209
		// (get) Token: 0x06008235 RID: 33333 RVA: 0x002A9364 File Offset: 0x002A7564
		public static float DeltaTime
		{
			get
			{
				return BoingManager.s_deltaTime;
			}
		}

		// Token: 0x17000C8A RID: 3210
		// (get) Token: 0x06008236 RID: 33334 RVA: 0x002A936B File Offset: 0x002A756B
		public static float FixedDeltaTime
		{
			get
			{
				return Time.fixedDeltaTime;
			}
		}

		// Token: 0x17000C8B RID: 3211
		// (get) Token: 0x06008237 RID: 33335 RVA: 0x002A9372 File Offset: 0x002A7572
		internal static int NumBehaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Count;
			}
		}

		// Token: 0x17000C8C RID: 3212
		// (get) Token: 0x06008238 RID: 33336 RVA: 0x002A937E File Offset: 0x002A757E
		internal static int NumEffectors
		{
			get
			{
				return BoingManager.s_effectorMap.Count;
			}
		}

		// Token: 0x17000C8D RID: 3213
		// (get) Token: 0x06008239 RID: 33337 RVA: 0x002A938A File Offset: 0x002A758A
		internal static int NumReactors
		{
			get
			{
				return BoingManager.s_reactorMap.Count;
			}
		}

		// Token: 0x17000C8E RID: 3214
		// (get) Token: 0x0600823A RID: 33338 RVA: 0x002A9396 File Offset: 0x002A7596
		internal static int NumFields
		{
			get
			{
				return BoingManager.s_fieldMap.Count;
			}
		}

		// Token: 0x17000C8F RID: 3215
		// (get) Token: 0x0600823B RID: 33339 RVA: 0x002A93A2 File Offset: 0x002A75A2
		internal static int NumCPUFieldSamplers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Count;
			}
		}

		// Token: 0x17000C90 RID: 3216
		// (get) Token: 0x0600823C RID: 33340 RVA: 0x002A93AE File Offset: 0x002A75AE
		internal static int NumGPUFieldSamplers
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Count;
			}
		}

		// Token: 0x0600823D RID: 33341 RVA: 0x002A93BC File Offset: 0x002A75BC
		private static void ValidateManager()
		{
			if (BoingManager.s_managerGo != null)
			{
				return;
			}
			BoingManager.s_managerGo = new GameObject("Boing Kit manager (don't delete)");
			BoingManager.s_managerGo.AddComponent<BoingManagerPreUpdatePump>();
			BoingManager.s_managerGo.AddComponent<BoingManagerPostUpdatePump>();
			Object.DontDestroyOnLoad(BoingManager.s_managerGo);
			BoingManager.s_managerGo.AddComponent<SphereCollider>().enabled = false;
		}

		// Token: 0x17000C91 RID: 3217
		// (get) Token: 0x0600823E RID: 33342 RVA: 0x002A9416 File Offset: 0x002A7616
		internal static SphereCollider SharedSphereCollider
		{
			get
			{
				if (BoingManager.s_managerGo == null)
				{
					return null;
				}
				return BoingManager.s_managerGo.GetComponent<SphereCollider>();
			}
		}

		// Token: 0x0600823F RID: 33343 RVA: 0x002A9431 File Offset: 0x002A7631
		internal static void Register(BoingBehavior behavior)
		{
			BoingManager.PreRegisterBehavior();
			BoingManager.s_behaviorMap.Add(behavior.GetInstanceID(), behavior);
			if (BoingManager.OnBehaviorRegister != null)
			{
				BoingManager.OnBehaviorRegister(behavior);
			}
		}

		// Token: 0x06008240 RID: 33344 RVA: 0x002A945B File Offset: 0x002A765B
		internal static void Unregister(BoingBehavior behavior)
		{
			if (BoingManager.OnBehaviorUnregister != null)
			{
				BoingManager.OnBehaviorUnregister(behavior);
			}
			BoingManager.s_behaviorMap.Remove(behavior.GetInstanceID());
			BoingManager.PostUnregisterBehavior();
		}

		// Token: 0x06008241 RID: 33345 RVA: 0x002A9485 File Offset: 0x002A7685
		internal static void Register(BoingEffector effector)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_effectorMap.Add(effector.GetInstanceID(), effector);
			if (BoingManager.OnEffectorRegister != null)
			{
				BoingManager.OnEffectorRegister(effector);
			}
		}

		// Token: 0x06008242 RID: 33346 RVA: 0x002A94AF File Offset: 0x002A76AF
		internal static void Unregister(BoingEffector effector)
		{
			if (BoingManager.OnEffectorUnregister != null)
			{
				BoingManager.OnEffectorUnregister(effector);
			}
			BoingManager.s_effectorMap.Remove(effector.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06008243 RID: 33347 RVA: 0x002A94D9 File Offset: 0x002A76D9
		internal static void Register(BoingReactor reactor)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_reactorMap.Add(reactor.GetInstanceID(), reactor);
			if (BoingManager.OnReactorRegister != null)
			{
				BoingManager.OnReactorRegister(reactor);
			}
		}

		// Token: 0x06008244 RID: 33348 RVA: 0x002A9503 File Offset: 0x002A7703
		internal static void Unregister(BoingReactor reactor)
		{
			if (BoingManager.OnReactorUnregister != null)
			{
				BoingManager.OnReactorUnregister(reactor);
			}
			BoingManager.s_reactorMap.Remove(reactor.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06008245 RID: 33349 RVA: 0x002A952D File Offset: 0x002A772D
		internal static void Register(BoingReactorField field)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_fieldMap.Add(field.GetInstanceID(), field);
			if (BoingManager.OnReactorFieldRegister != null)
			{
				BoingManager.OnReactorFieldRegister(field);
			}
		}

		// Token: 0x06008246 RID: 33350 RVA: 0x002A9557 File Offset: 0x002A7757
		internal static void Unregister(BoingReactorField field)
		{
			if (BoingManager.OnReactorFieldUnregister != null)
			{
				BoingManager.OnReactorFieldUnregister(field);
			}
			BoingManager.s_fieldMap.Remove(field.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06008247 RID: 33351 RVA: 0x002A9581 File Offset: 0x002A7781
		internal static void Register(BoingReactorFieldCPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_cpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldCPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
		}

		// Token: 0x06008248 RID: 33352 RVA: 0x002A95AB File Offset: 0x002A77AB
		internal static void Unregister(BoingReactorFieldCPUSampler sampler)
		{
			if (BoingManager.OnReactorFieldCPUSamplerUnregister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
			BoingManager.s_cpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06008249 RID: 33353 RVA: 0x002A95D5 File Offset: 0x002A77D5
		internal static void Register(BoingReactorFieldGPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_gpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldGPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldGPUSamplerRegister(sampler);
			}
		}

		// Token: 0x0600824A RID: 33354 RVA: 0x002A95FF File Offset: 0x002A77FF
		internal static void Unregister(BoingReactorFieldGPUSampler sampler)
		{
			if (BoingManager.OnFieldGPUSamplerUnregister != null)
			{
				BoingManager.OnFieldGPUSamplerUnregister(sampler);
			}
			BoingManager.s_gpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x0600824B RID: 33355 RVA: 0x002A9629 File Offset: 0x002A7829
		internal static void Register(BoingBones bones)
		{
			BoingManager.PreRegisterBones();
			BoingManager.s_bonesMap.Add(bones.GetInstanceID(), bones);
			if (BoingManager.OnBonesRegister != null)
			{
				BoingManager.OnBonesRegister(bones);
			}
		}

		// Token: 0x0600824C RID: 33356 RVA: 0x002A9653 File Offset: 0x002A7853
		internal static void Unregister(BoingBones bones)
		{
			if (BoingManager.OnBonesUnregister != null)
			{
				BoingManager.OnBonesUnregister(bones);
			}
			BoingManager.s_bonesMap.Remove(bones.GetInstanceID());
			BoingManager.PostUnregisterBones();
		}

		// Token: 0x0600824D RID: 33357 RVA: 0x002A967D File Offset: 0x002A787D
		private static void PreRegisterBehavior()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x0600824E RID: 33358 RVA: 0x002A9684 File Offset: 0x002A7884
		private static void PostUnregisterBehavior()
		{
			if (BoingManager.s_behaviorMap.Count > 0)
			{
				return;
			}
			BoingWorkAsynchronous.PostUnregisterBehaviorCleanUp();
		}

		// Token: 0x0600824F RID: 33359 RVA: 0x002A969C File Offset: 0x002A789C
		private static void PreRegisterEffectorReactor()
		{
			BoingManager.ValidateManager();
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				BoingManager.s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
			if (BoingManager.s_effectorMap.Count >= BoingManager.s_effectorParamsList.Capacity)
			{
				BoingManager.s_effectorParamsList.Capacity += BoingManager.kEffectorParamsIncrement;
				BoingManager.s_effectorParamsBuffer.Dispose();
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
		}

		// Token: 0x06008250 RID: 33360 RVA: 0x002A972C File Offset: 0x002A792C
		private static void PostUnregisterEffectorReactor()
		{
			if (BoingManager.s_effectorMap.Count > 0 || BoingManager.s_reactorMap.Count > 0 || BoingManager.s_fieldMap.Count > 0 || BoingManager.s_cpuSamplerMap.Count > 0 || BoingManager.s_gpuSamplerMap.Count > 0)
			{
				return;
			}
			BoingManager.s_effectorParamsList = null;
			BoingManager.s_effectorParamsBuffer.Dispose();
			BoingManager.s_effectorParamsBuffer = null;
			BoingWorkAsynchronous.PostUnregisterEffectorReactorCleanUp();
		}

		// Token: 0x06008251 RID: 33361 RVA: 0x002A967D File Offset: 0x002A787D
		private static void PreRegisterBones()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x06008252 RID: 33362 RVA: 0x00002C2D File Offset: 0x00000E2D
		private static void PostUnregisterBones()
		{
		}

		// Token: 0x06008253 RID: 33363 RVA: 0x002A9796 File Offset: 0x002A7996
		internal static void Execute(BoingManager.UpdateMode updateMode)
		{
			if (updateMode == BoingManager.UpdateMode.EarlyUpdate)
			{
				BoingManager.s_deltaTime = Time.deltaTime;
			}
			BoingManager.RefreshEffectorParams();
			BoingManager.ExecuteBones(updateMode);
			BoingManager.ExecuteBehaviors(updateMode);
			BoingManager.ExecuteReactors(updateMode);
		}

		// Token: 0x06008254 RID: 33364 RVA: 0x002A97C0 File Offset: 0x002A79C0
		internal static void ExecuteBehaviors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_behaviorMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				BoingBehavior value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
		}

		// Token: 0x06008255 RID: 33365 RVA: 0x002A9854 File Offset: 0x002A7A54
		internal static void PullBehaviorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
		}

		// Token: 0x06008256 RID: 33366 RVA: 0x002A98BC File Offset: 0x002A7ABC
		internal static void RestoreBehaviors()
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x06008257 RID: 33367 RVA: 0x002A9914 File Offset: 0x002A7B14
		internal static void RefreshEffectorParams()
		{
			if (BoingManager.s_effectorParamsList == null)
			{
				return;
			}
			BoingManager.s_effectorParamsIndexMap.Clear();
			BoingManager.s_effectorParamsList.Clear();
			foreach (KeyValuePair<int, BoingEffector> keyValuePair in BoingManager.s_effectorMap)
			{
				BoingEffector value = keyValuePair.Value;
				BoingManager.s_effectorParamsIndexMap.Add(value.GetInstanceID(), BoingManager.s_effectorParamsList.Count);
				BoingManager.s_effectorParamsList.Add(new BoingEffector.Params(value));
			}
			if (BoingManager.s_aEffectorParams == null || BoingManager.s_aEffectorParams.Length != BoingManager.s_effectorParamsList.Count)
			{
				BoingManager.s_aEffectorParams = BoingManager.s_effectorParamsList.ToArray();
				return;
			}
			BoingManager.s_effectorParamsList.CopyTo(BoingManager.s_aEffectorParams);
		}

		// Token: 0x06008258 RID: 33368 RVA: 0x002A99E8 File Offset: 0x002A7BE8
		internal static void ExecuteReactors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_effectorMap.Count == 0 && BoingManager.s_reactorMap.Count == 0 && BoingManager.s_fieldMap.Count == 0 && BoingManager.s_cpuSamplerMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				BoingReactor value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteReactors(BoingManager.s_effectorMap, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteReactors(BoingManager.s_aEffectorParams, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
		}

		// Token: 0x06008259 RID: 33369 RVA: 0x002A9AC0 File Offset: 0x002A7CC0
		internal static void PullReactorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				if (keyValuePair2.Value.UpdateMode == updateMode)
				{
					keyValuePair2.Value.SampleFromField();
				}
			}
		}

		// Token: 0x0600825A RID: 33370 RVA: 0x002A9B7C File Offset: 0x002A7D7C
		internal static void RestoreReactors()
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				keyValuePair.Value.Restore();
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				keyValuePair2.Value.Restore();
			}
		}

		// Token: 0x0600825B RID: 33371 RVA: 0x002A9C1C File Offset: 0x002A7E1C
		internal static void DispatchReactorFieldCompute()
		{
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				return;
			}
			BoingManager.s_effectorParamsBuffer.SetData(BoingManager.s_aEffectorParams);
			float deltaTime = Time.deltaTime;
			foreach (KeyValuePair<int, BoingReactorField> keyValuePair in BoingManager.s_fieldMap)
			{
				BoingReactorField value = keyValuePair.Value;
				if (value.HardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					value.ExecuteGpu(deltaTime, BoingManager.s_effectorParamsBuffer, BoingManager.s_effectorParamsIndexMap);
				}
			}
		}

		// Token: 0x0600825C RID: 33372 RVA: 0x002A9CA8 File Offset: 0x002A7EA8
		internal static void ExecuteBones(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x0600825D RID: 33373 RVA: 0x002A9D48 File Offset: 0x002A7F48
		internal static void PullBonesResults(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x0600825E RID: 33374 RVA: 0x002A9D80 File Offset: 0x002A7F80
		internal static void RestoreBones()
		{
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x04009356 RID: 37718
		public static BoingManager.BehaviorRegisterDelegate OnBehaviorRegister;

		// Token: 0x04009357 RID: 37719
		public static BoingManager.BehaviorUnregisterDelegate OnBehaviorUnregister;

		// Token: 0x04009358 RID: 37720
		public static BoingManager.EffectorRegisterDelegate OnEffectorRegister;

		// Token: 0x04009359 RID: 37721
		public static BoingManager.EffectorUnregisterDelegate OnEffectorUnregister;

		// Token: 0x0400935A RID: 37722
		public static BoingManager.ReactorRegisterDelegate OnReactorRegister;

		// Token: 0x0400935B RID: 37723
		public static BoingManager.ReactorUnregisterDelegate OnReactorUnregister;

		// Token: 0x0400935C RID: 37724
		public static BoingManager.ReactorFieldRegisterDelegate OnReactorFieldRegister;

		// Token: 0x0400935D RID: 37725
		public static BoingManager.ReactorFieldUnregisterDelegate OnReactorFieldUnregister;

		// Token: 0x0400935E RID: 37726
		public static BoingManager.ReactorFieldCPUSamplerRegisterDelegate OnReactorFieldCPUSamplerRegister;

		// Token: 0x0400935F RID: 37727
		public static BoingManager.ReactorFieldCPUSamplerUnregisterDelegate OnReactorFieldCPUSamplerUnregister;

		// Token: 0x04009360 RID: 37728
		public static BoingManager.ReactorFieldGPUSamplerRegisterDelegate OnReactorFieldGPUSamplerRegister;

		// Token: 0x04009361 RID: 37729
		public static BoingManager.ReactorFieldGPUSamplerUnregisterDelegate OnFieldGPUSamplerUnregister;

		// Token: 0x04009362 RID: 37730
		public static BoingManager.BonesRegisterDelegate OnBonesRegister;

		// Token: 0x04009363 RID: 37731
		public static BoingManager.BonesUnregisterDelegate OnBonesUnregister;

		// Token: 0x04009364 RID: 37732
		private static float s_deltaTime = 0f;

		// Token: 0x04009365 RID: 37733
		private static Dictionary<int, BoingBehavior> s_behaviorMap = new Dictionary<int, BoingBehavior>();

		// Token: 0x04009366 RID: 37734
		private static Dictionary<int, BoingEffector> s_effectorMap = new Dictionary<int, BoingEffector>();

		// Token: 0x04009367 RID: 37735
		private static Dictionary<int, BoingReactor> s_reactorMap = new Dictionary<int, BoingReactor>();

		// Token: 0x04009368 RID: 37736
		private static Dictionary<int, BoingReactorField> s_fieldMap = new Dictionary<int, BoingReactorField>();

		// Token: 0x04009369 RID: 37737
		private static Dictionary<int, BoingReactorFieldCPUSampler> s_cpuSamplerMap = new Dictionary<int, BoingReactorFieldCPUSampler>();

		// Token: 0x0400936A RID: 37738
		private static Dictionary<int, BoingReactorFieldGPUSampler> s_gpuSamplerMap = new Dictionary<int, BoingReactorFieldGPUSampler>();

		// Token: 0x0400936B RID: 37739
		private static Dictionary<int, BoingBones> s_bonesMap = new Dictionary<int, BoingBones>();

		// Token: 0x0400936C RID: 37740
		private static readonly int kEffectorParamsIncrement = 16;

		// Token: 0x0400936D RID: 37741
		private static List<BoingEffector.Params> s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);

		// Token: 0x0400936E RID: 37742
		private static BoingEffector.Params[] s_aEffectorParams;

		// Token: 0x0400936F RID: 37743
		private static ComputeBuffer s_effectorParamsBuffer;

		// Token: 0x04009370 RID: 37744
		private static Dictionary<int, int> s_effectorParamsIndexMap = new Dictionary<int, int>();

		// Token: 0x04009371 RID: 37745
		internal static readonly bool UseAsynchronousJobs = true;

		// Token: 0x04009372 RID: 37746
		internal static GameObject s_managerGo;

		// Token: 0x0200142B RID: 5163
		public enum UpdateMode
		{
			// Token: 0x04009374 RID: 37748
			FixedUpdate,
			// Token: 0x04009375 RID: 37749
			EarlyUpdate,
			// Token: 0x04009376 RID: 37750
			LateUpdate
		}

		// Token: 0x0200142C RID: 5164
		public enum TranslationLockSpace
		{
			// Token: 0x04009378 RID: 37752
			Global,
			// Token: 0x04009379 RID: 37753
			Local
		}

		// Token: 0x0200142D RID: 5165
		// (Invoke) Token: 0x06008261 RID: 33377
		public delegate void BehaviorRegisterDelegate(BoingBehavior behavior);

		// Token: 0x0200142E RID: 5166
		// (Invoke) Token: 0x06008265 RID: 33381
		public delegate void BehaviorUnregisterDelegate(BoingBehavior behavior);

		// Token: 0x0200142F RID: 5167
		// (Invoke) Token: 0x06008269 RID: 33385
		public delegate void EffectorRegisterDelegate(BoingEffector effector);

		// Token: 0x02001430 RID: 5168
		// (Invoke) Token: 0x0600826D RID: 33389
		public delegate void EffectorUnregisterDelegate(BoingEffector effector);

		// Token: 0x02001431 RID: 5169
		// (Invoke) Token: 0x06008271 RID: 33393
		public delegate void ReactorRegisterDelegate(BoingReactor reactor);

		// Token: 0x02001432 RID: 5170
		// (Invoke) Token: 0x06008275 RID: 33397
		public delegate void ReactorUnregisterDelegate(BoingReactor reactor);

		// Token: 0x02001433 RID: 5171
		// (Invoke) Token: 0x06008279 RID: 33401
		public delegate void ReactorFieldRegisterDelegate(BoingReactorField field);

		// Token: 0x02001434 RID: 5172
		// (Invoke) Token: 0x0600827D RID: 33405
		public delegate void ReactorFieldUnregisterDelegate(BoingReactorField field);

		// Token: 0x02001435 RID: 5173
		// (Invoke) Token: 0x06008281 RID: 33409
		public delegate void ReactorFieldCPUSamplerRegisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x02001436 RID: 5174
		// (Invoke) Token: 0x06008285 RID: 33413
		public delegate void ReactorFieldCPUSamplerUnregisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x02001437 RID: 5175
		// (Invoke) Token: 0x06008289 RID: 33417
		public delegate void ReactorFieldGPUSamplerRegisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x02001438 RID: 5176
		// (Invoke) Token: 0x0600828D RID: 33421
		public delegate void ReactorFieldGPUSamplerUnregisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x02001439 RID: 5177
		// (Invoke) Token: 0x06008291 RID: 33425
		public delegate void BonesRegisterDelegate(BoingBones bones);

		// Token: 0x0200143A RID: 5178
		// (Invoke) Token: 0x06008295 RID: 33429
		public delegate void BonesUnregisterDelegate(BoingBones bones);
	}
}
