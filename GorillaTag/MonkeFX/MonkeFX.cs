using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.MonkeFX
{
	// Token: 0x0200125A RID: 4698
	public class MonkeFX : ITickSystemPost
	{
		// Token: 0x060076F1 RID: 30449 RVA: 0x002695AC File Offset: 0x002677AC
		private static void InitBonesArray()
		{
			MonkeFX._rigs = VRRigCache.Instance.GetAllRigs();
			MonkeFX._bones = new Transform[MonkeFX._rigs.Length * MonkeFX._boneNames.Length];
			for (int i = 0; i < MonkeFX._rigs.Length; i++)
			{
				if (MonkeFX._rigs[i] == null)
				{
					MonkeFX._errorLog_nullVRRigFromVRRigCache.AddOccurrence(i.ToString());
				}
				else
				{
					int num = i * MonkeFX._boneNames.Length;
					if (MonkeFX._rigs[i].mainSkin == null)
					{
						MonkeFX._errorLog_nullMainSkin.AddOccurrence(MonkeFX._rigs[i].transform.GetPath());
						Debug.LogError("(This should never happen) Skipping null `mainSkin` on `VRRig`! Scene path: \n- \"" + MonkeFX._rigs[i].transform.GetPath() + "\"");
					}
					else
					{
						for (int j = 0; j < MonkeFX._rigs[i].mainSkin.bones.Length; j++)
						{
							Transform transform = MonkeFX._rigs[i].mainSkin.bones[j];
							if (transform == null)
							{
								MonkeFX._errorLog_nullBone.AddOccurrence(j.ToString());
							}
							else
							{
								for (int k = 0; k < MonkeFX._boneNames.Length; k++)
								{
									if (MonkeFX._boneNames[k] == transform.name)
									{
										MonkeFX._bones[num + k] = transform;
									}
								}
							}
						}
					}
				}
			}
			MonkeFX._errorLog_nullVRRigFromVRRigCache.LogOccurrences(VRRigCache.Instance, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 106);
			MonkeFX._errorLog_nullMainSkin.LogOccurrences(null, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 107);
			MonkeFX._errorLog_nullBone.LogOccurrences(null, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 108);
		}

		// Token: 0x060076F2 RID: 30450 RVA: 0x00002C2D File Offset: 0x00000E2D
		private static void UpdateBones()
		{
		}

		// Token: 0x060076F3 RID: 30451 RVA: 0x00002C2D File Offset: 0x00000E2D
		private static void UpdateBone()
		{
		}

		// Token: 0x060076F4 RID: 30452 RVA: 0x00269754 File Offset: 0x00267954
		public static void Register(MonkeFXSettingsSO settingsSO)
		{
			MonkeFX.EnsureInstance();
			if (settingsSO == null || !MonkeFX.instance._settingsSOs.Add(settingsSO))
			{
				return;
			}
			int num = MonkeFX.instance._srcMeshId_to_sourceMesh.Count;
			for (int i = 0; i < settingsSO.sourceMeshes.Length; i++)
			{
				Mesh obj = settingsSO.sourceMeshes[i].obj;
				if (!(obj == null) && MonkeFX.instance._srcMeshInst_to_meshId.TryAdd(obj.GetInstanceID(), num))
				{
					MonkeFX.instance._srcMeshId_to_sourceMesh.Add(obj);
					num++;
				}
			}
		}

		// Token: 0x060076F5 RID: 30453 RVA: 0x002697EC File Offset: 0x002679EC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetScaleToFitInBounds(Mesh mesh)
		{
			Bounds bounds = mesh.bounds;
			float num = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
			if (num <= 0f)
			{
				return 0f;
			}
			return 1f / num;
		}

		// Token: 0x060076F6 RID: 30454 RVA: 0x00269844 File Offset: 0x00267A44
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Pack0To1Floats(float x, float y)
		{
			return Mathf.Clamp01(x) * 65536f + Mathf.Clamp01(y);
		}

		// Token: 0x17000B91 RID: 2961
		// (get) Token: 0x060076F7 RID: 30455 RVA: 0x00269859 File Offset: 0x00267A59
		// (set) Token: 0x060076F8 RID: 30456 RVA: 0x00269860 File Offset: 0x00267A60
		public static MonkeFX instance { get; private set; }

		// Token: 0x17000B92 RID: 2962
		// (get) Token: 0x060076F9 RID: 30457 RVA: 0x00269868 File Offset: 0x00267A68
		// (set) Token: 0x060076FA RID: 30458 RVA: 0x0026986F File Offset: 0x00267A6F
		public static bool hasInstance { get; private set; }

		// Token: 0x060076FB RID: 30459 RVA: 0x00269877 File Offset: 0x00267A77
		private static void EnsureInstance()
		{
			if (MonkeFX.hasInstance)
			{
				return;
			}
			MonkeFX.instance = new MonkeFX();
			MonkeFX.hasInstance = true;
		}

		// Token: 0x060076FC RID: 30460 RVA: 0x00269891 File Offset: 0x00267A91
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnAfterFirstSceneLoaded()
		{
			MonkeFX.EnsureInstance();
			TickSystem<object>.AddPostTickCallback(MonkeFX.instance);
		}

		// Token: 0x060076FD RID: 30461 RVA: 0x002698A2 File Offset: 0x00267AA2
		void ITickSystemPost.PostTick()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			MonkeFX.UpdateBones();
		}

		// Token: 0x17000B93 RID: 2963
		// (get) Token: 0x060076FE RID: 30462 RVA: 0x002698B1 File Offset: 0x00267AB1
		// (set) Token: 0x060076FF RID: 30463 RVA: 0x002698B9 File Offset: 0x00267AB9
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06007700 RID: 30464 RVA: 0x002698C2 File Offset: 0x00267AC2
		private static void PauseTick()
		{
			if (!MonkeFX.hasInstance)
			{
				MonkeFX.instance = new MonkeFX();
			}
			TickSystem<object>.RemovePostTickCallback(MonkeFX.instance);
		}

		// Token: 0x06007701 RID: 30465 RVA: 0x002698DF File Offset: 0x00267ADF
		private static void ResumeTick()
		{
			if (!MonkeFX.hasInstance)
			{
				MonkeFX.instance = new MonkeFX();
			}
			TickSystem<object>.AddPostTickCallback(MonkeFX.instance);
		}

		// Token: 0x0400869E RID: 34462
		private static readonly string[] _boneNames = new string[] { "body", "hand.L", "hand.R" };

		// Token: 0x0400869F RID: 34463
		private static VRRig[] _rigs;

		// Token: 0x040086A0 RID: 34464
		private static Transform[] _bones;

		// Token: 0x040086A1 RID: 34465
		private static int _rigsHash;

		// Token: 0x040086A2 RID: 34466
		private static readonly GTLogErrorLimiter _errorLog_nullVRRigFromVRRigCache = new GTLogErrorLimiter("(This should never happen) Skipping null `VRRig` obtained from `VRRigCache`!", 10, "\n- ");

		// Token: 0x040086A3 RID: 34467
		private static GTLogErrorLimiter _errorLog_nullMainSkin = new GTLogErrorLimiter("(This should never happen) Skipping null `mainSkin` on `VRRig`! Scene paths: \n", 10, "\n- ");

		// Token: 0x040086A4 RID: 34468
		private static readonly GTLogErrorLimiter _errorLog_nullBone = new GTLogErrorLimiter("(This should never happen) Skipping null bone obtained from `VRRig.mainSkin.bones`! Index(es): ", 10, "\n- ");

		// Token: 0x040086A5 RID: 34469
		private readonly HashSet<MonkeFXSettingsSO> _settingsSOs = new HashSet<MonkeFXSettingsSO>(8);

		// Token: 0x040086A6 RID: 34470
		private readonly Dictionary<int, int> _srcMeshInst_to_meshId = new Dictionary<int, int>(8);

		// Token: 0x040086A7 RID: 34471
		private readonly List<Mesh> _srcMeshId_to_sourceMesh = new List<Mesh>(8);

		// Token: 0x040086A8 RID: 34472
		private readonly List<MonkeFX.ElementsRange> _srcMeshId_to_elemRange = new List<MonkeFX.ElementsRange>(8);

		// Token: 0x040086A9 RID: 34473
		private readonly Dictionary<int, List<MonkeFXSettingsSO>> _meshId_to_settingsUsers = new Dictionary<int, List<MonkeFXSettingsSO>>();

		// Token: 0x040086AA RID: 34474
		private const float _k16BitFactor = 65536f;

		// Token: 0x0200125B RID: 4699
		private struct ElementsRange
		{
			// Token: 0x040086AE RID: 34478
			public int min;

			// Token: 0x040086AF RID: 34479
			public int max;
		}
	}
}
