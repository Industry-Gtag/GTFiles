using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012A0 RID: 4768
	internal static class GTAudioOneShot
	{
		// Token: 0x17000BAD RID: 2989
		// (get) Token: 0x060077D9 RID: 30681 RVA: 0x0026CDA8 File Offset: 0x0026AFA8
		// (set) Token: 0x060077DA RID: 30682 RVA: 0x0026CDAF File Offset: 0x0026AFAF
		internal static bool isInitialized { get; private set; }

		// Token: 0x060077DB RID: 30683 RVA: 0x0026CDB8 File Offset: 0x0026AFB8
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (GTAudioOneShot.isInitialized)
			{
				return;
			}
			AudioSource audioSource = Resources.Load<AudioSource>("AudioSourceSingleton_Prefab");
			if (audioSource == null)
			{
				Debug.LogError("GTAudioOneShot: Failed to load AudioSourceSingleton_Prefab from resources!!!");
				return;
			}
			GTAudioOneShot.audioSource = Object.Instantiate<AudioSource>(audioSource);
			GTAudioOneShot.defaultCurve = GTAudioOneShot.audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
			Object.DontDestroyOnLoad(GTAudioOneShot.audioSource);
			GTAudioOneShot.isInitialized = true;
		}

		// Token: 0x060077DC RID: 30684 RVA: 0x0026CE17 File Offset: 0x0026B017
		internal static void Play(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.pitch = pitch;
			GTAudioOneShot.audioSource.transform.position = position;
			GTAudioOneShot.audioSource.GTPlayOneShot(clip, volume);
		}

		// Token: 0x060077DD RID: 30685 RVA: 0x0026CE4F File Offset: 0x0026B04F
		internal static void Play(AudioClip clip, Vector3 position, AnimationCurve curve, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
			GTAudioOneShot.Play(clip, position, volume, pitch);
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, GTAudioOneShot.defaultCurve);
		}

		// Token: 0x060077DE RID: 30686 RVA: 0x0026CE86 File Offset: 0x0026B086
		internal static int PlayDelayed(AudioClip sound, Vector3 pos, float delay, float volume = 1f, float pitch = 1f)
		{
			return GTAudioOneShot.PlayDelayed(sound, null, pos, delay, volume, pitch);
		}

		// Token: 0x060077DF RID: 30687 RVA: 0x0026CE94 File Offset: 0x0026B094
		internal static int PlayDelayed(AudioClip sound, Transform xform, Vector3 pos, float delay, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return -1;
			}
			int num;
			if (GTAudioOneShot._delayedFreeHead >= 0)
			{
				num = GTAudioOneShot._delayedFreeHead;
				GTAudioOneShot._delayedFreeHead = GTAudioOneShot._delayedFreeNext[num];
			}
			else
			{
				if (GTAudioOneShot._delayedHighWater >= GTAudioOneShot._delayedData.Length)
				{
					int num2 = GTAudioOneShot._delayedData.Length * 2;
					Array.Resize<GTAudioOneShot.DelayedPlayData>(ref GTAudioOneShot._delayedData, num2);
					Array.Resize<int>(ref GTAudioOneShot._delayedFreeNext, num2);
				}
				num = GTAudioOneShot._delayedHighWater++;
			}
			GTAudioOneShot._delayedData[num] = new GTAudioOneShot.DelayedPlayData
			{
				sound = sound,
				xform = xform,
				pos = pos,
				volume = volume,
				pitch = pitch
			};
			GTDelayedExec.Add(GTAudioOneShot._delayedListener, delay, num);
			return num;
		}

		// Token: 0x060077E0 RID: 30688 RVA: 0x0026CF54 File Offset: 0x0026B154
		internal static void CancelDelayed(int idx)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			GTAudioOneShot._delayedData[idx].sound = null;
		}

		// Token: 0x060077E1 RID: 30689 RVA: 0x0026CF70 File Offset: 0x0026B170
		internal static void UpdateDelayed(int idx, Transform xform)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[idx];
			if (ptr.sound == null)
			{
				return;
			}
			ptr.xform = xform;
		}

		// Token: 0x060077E2 RID: 30690 RVA: 0x0026CFA8 File Offset: 0x0026B1A8
		internal static void UpdateDelayed(int idx, Vector3 pos)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[idx];
			if (ptr.sound == null)
			{
				return;
			}
			ptr.pos = pos;
		}

		// Token: 0x060077E3 RID: 30691 RVA: 0x0026CFE0 File Offset: 0x0026B1E0
		internal static void UpdateDelayed(int idx, Transform xform, Vector3 pos)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[idx];
			if (ptr.sound == null)
			{
				return;
			}
			ptr.xform = xform;
			ptr.pos = pos;
		}

		// Token: 0x0400880F RID: 34831
		[OnEnterPlay_SetNull]
		internal static AudioSource audioSource;

		// Token: 0x04008810 RID: 34832
		[OnEnterPlay_SetNull]
		internal static AnimationCurve defaultCurve;

		// Token: 0x04008811 RID: 34833
		private const int k_initialDelayedCount = 32;

		// Token: 0x04008812 RID: 34834
		[OnEnterPlay_Set(0)]
		private static int _delayedHighWater;

		// Token: 0x04008813 RID: 34835
		[OnEnterPlay_Set(-1)]
		private static int _delayedFreeHead = -1;

		// Token: 0x04008814 RID: 34836
		[OnEnterPlay_SetNew]
		private static GTAudioOneShot.DelayedPlayData[] _delayedData = new GTAudioOneShot.DelayedPlayData[32];

		// Token: 0x04008815 RID: 34837
		[OnEnterPlay_SetNew]
		private static int[] _delayedFreeNext = new int[32];

		// Token: 0x04008816 RID: 34838
		[OnEnterPlay_SetNew]
		private static readonly GTAudioOneShot.DelayedPlayListener _delayedListener = new GTAudioOneShot.DelayedPlayListener();

		// Token: 0x020012A1 RID: 4769
		private struct DelayedPlayData
		{
			// Token: 0x04008817 RID: 34839
			public AudioClip sound;

			// Token: 0x04008818 RID: 34840
			public Transform xform;

			// Token: 0x04008819 RID: 34841
			public Vector3 pos;

			// Token: 0x0400881A RID: 34842
			public float volume;

			// Token: 0x0400881B RID: 34843
			public float pitch;
		}

		// Token: 0x020012A2 RID: 4770
		private class DelayedPlayListener : IDelayedExecListener
		{
			// Token: 0x060077E5 RID: 30693 RVA: 0x0026D04C File Offset: 0x0026B24C
			public void OnDelayedAction(int contextId)
			{
				if (contextId >= GTAudioOneShot._delayedHighWater)
				{
					return;
				}
				ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[contextId];
				if (ptr.sound != null)
				{
					Vector3 vector = ((ptr.xform != null) ? ptr.xform.TransformPoint(ptr.pos) : ptr.pos);
					GTAudioOneShot.Play(ptr.sound, vector, ptr.volume, ptr.pitch);
				}
				ptr = default(GTAudioOneShot.DelayedPlayData);
				GTAudioOneShot._delayedFreeNext[contextId] = GTAudioOneShot._delayedFreeHead;
				GTAudioOneShot._delayedFreeHead = contextId;
			}
		}
	}
}
