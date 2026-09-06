using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000294 RID: 660
public class GameStateFx : MonoBehaviour, IGameStateReceiver, IDelayedExecListener
{
	// Token: 0x060011A4 RID: 4516 RVA: 0x0005EBD0 File Offset: 0x0005CDD0
	protected void Awake()
	{
		IGameStateProvider gameStateProvider = this.m_stateProvider as IGameStateProvider;
		if (gameStateProvider == null)
		{
			GTDev.LogError<string>("[GT/GameStateFx]  ERROR!!!  Awake: The supplied State Provider is not type `IGameStateProvider`. Path=" + base.transform.GetPathQ(), null);
			this._isValid = false;
			base.enabled = false;
			return;
		}
		this._stateProvider = gameStateProvider;
		if (!this._IsAllValid())
		{
			return;
		}
		foreach (GameStateFx.StateReaction[] array in this.m_stateMap.Values)
		{
			if (array != null)
			{
				Array.Sort<GameStateFx.StateReaction>(array, new Comparison<GameStateFx.StateReaction>(GameStateFx._DelaySortCompare));
			}
		}
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x0005EC7C File Offset: 0x0005CE7C
	private static int _DelaySortCompare(GameStateFx.StateReaction a, GameStateFx.StateReaction b)
	{
		return a.delay.CompareTo(b.delay);
	}

	// Token: 0x060011A6 RID: 4518 RVA: 0x0005EC8F File Offset: 0x0005CE8F
	protected void OnEnable()
	{
		if (!this._isValid || ApplicationQuittingState.IsQuitting)
		{
			base.enabled = false;
			return;
		}
		this._stateProvider.GameStateReceiverRegister(this);
	}

	// Token: 0x060011A7 RID: 4519 RVA: 0x0005ECB4 File Offset: 0x0005CEB4
	protected void OnDisable()
	{
		if (!this._isValid || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._stateProvider.GameStateReceiverUnregister(this);
	}

	// Token: 0x060011A8 RID: 4520 RVA: 0x0005ECD4 File Offset: 0x0005CED4
	void IGameStateReceiver.GameStateReceiverOnStateChanged(long oldState, long newState)
	{
		GameStateFx.StateReaction[] array;
		if (!this.m_stateMap.TryGet(newState, out array))
		{
			return;
		}
		this._delayedExecContextFrameNum = Time.frameCount;
		this._reactionQueue.Clear();
		foreach (GameStateFx.StateReaction stateReaction in array)
		{
			if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Delay) != (GameStateFx.StateReaction.EOptions)0)
			{
				this._reactionQueue.Enqueue(stateReaction);
				GTDelayedExec.Add(this, stateReaction.delay, Time.frameCount);
			}
			else
			{
				GameStateFx._PerformReactions(stateReaction);
			}
		}
	}

	// Token: 0x060011A9 RID: 4521 RVA: 0x0005ED4B File Offset: 0x0005CF4B
	void IDelayedExecListener.OnDelayedAction(int contextFrameNum)
	{
		if (contextFrameNum != this._delayedExecContextFrameNum || !base.isActiveAndEnabled)
		{
			return;
		}
		GameStateFx._PerformReactions(this._reactionQueue.Dequeue());
	}

	// Token: 0x060011AA RID: 4522 RVA: 0x0005ED70 File Offset: 0x0005CF70
	private static void _PerformReactions(GameStateFx.StateReaction reaction)
	{
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Sound) != (GameStateFx.StateReaction.EOptions)0)
		{
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Sound) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.resource = reaction.soundInfo.sound;
			}
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Volume) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.volume = reaction.soundInfo.volume;
			}
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Pitch) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.pitch = reaction.soundInfo.pitch;
			}
			reaction.soundInfo.source.GTPlay();
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.GameObjects) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.GameObjectInfo gameObjectInfo in reaction.gameObjectInfos)
			{
				gameObjectInfo.gameObject.SetActive(gameObjectInfo.activate);
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Behaviours) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.BehaviourInfo behaviourInfo in reaction.behaviourInfos)
			{
				behaviourInfo.behaviour.enabled = behaviourInfo.enable;
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Renderers) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.RenderInfo renderInfo in reaction.renderers)
			{
				renderInfo.renderer.enabled = renderInfo.enable;
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Materials) != (GameStateFx.StateReaction.EOptions)0)
		{
			GameStateFx.MaterialInfo[] materialInfos = reaction.materialInfos;
			for (int i = 0; i < materialInfos.Length; i++)
			{
				foreach (GameStateFx.MaterialInfo.Entry entry in materialInfos[i].entries)
				{
					entry.slotInfo.renderer.GetSharedMaterials(GameStateFx._g_materialsCache);
					if (entry.slotInfo.slot >= 0 && entry.slotInfo.slot < GameStateFx._g_materialsCache.Count)
					{
						GameStateFx._g_materialsCache[entry.slotInfo.slot] = entry.material;
						entry.slotInfo.renderer.SetSharedMaterials(GameStateFx._g_materialsCache);
					}
				}
			}
		}
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x0005EF98 File Offset: 0x0005D198
	private bool _IsAllValid()
	{
		this._isValid = true;
		bool flag = false;
		this._hasDefaultAudioSource = this.m_defaultAudioSource != null;
		foreach (GameStateFx.StateReaction[] array in this.m_stateMap.Values)
		{
			foreach (GameStateFx.StateReaction stateReaction in array)
			{
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Sound) != (GameStateFx.StateReaction.EOptions)0)
				{
					if ((stateReaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Source) != (GameStateFx.SoundEntry.EOptions)0)
					{
						if (!this._IsOneValid(stateReaction.soundInfo.source != null, "an AudioSource is unassigned."))
						{
							return false;
						}
					}
					else
					{
						flag = true;
						stateReaction.soundInfo.source = this.m_defaultAudioSource;
					}
					if (!this._IsOneValid(stateReaction.soundInfo.sound != null, "A sound is unassigned."))
					{
						return false;
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.GameObjects) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.GameObjectInfo gameObjectInfo in stateReaction.gameObjectInfos)
					{
						if (!this._IsOneValid(gameObjectInfo.gameObject != null, "A GameObject is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Behaviours) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.BehaviourInfo behaviourInfo in stateReaction.behaviourInfos)
					{
						if (!this._IsOneValid(behaviourInfo.behaviour != null, "A Behaviour is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Renderers) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.RenderInfo renderInfo in stateReaction.renderers)
					{
						if (!this._IsOneValid(renderInfo.renderer != null, "A Renderer is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Materials) != (GameStateFx.StateReaction.EOptions)0)
				{
					GameStateFx.MaterialInfo[] materialInfos = stateReaction.materialInfos;
					for (int j = 0; j < materialInfos.Length; j++)
					{
						foreach (GameStateFx.MaterialInfo.Entry entry in materialInfos[j].entries)
						{
							if (!this._IsOneValid(entry.slotInfo.renderer != null, "A mat swap Renderer is unassigned"))
							{
								return false;
							}
						}
					}
				}
			}
		}
		if (flag && !this._hasDefaultAudioSource)
		{
			base.enabled = false;
			this._isValid = false;
			return false;
		}
		return true;
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x0005F23C File Offset: 0x0005D43C
	private bool _IsOneValid(bool isValidCondition, string msgFailReason)
	{
		if (isValidCondition)
		{
			return true;
		}
		this._isValid = false;
		base.enabled = false;
		return false;
	}

	// Token: 0x04001509 RID: 5385
	private const string preLog = "[GT/GameStateFx]  ";

	// Token: 0x0400150A RID: 5386
	private const string preErr = "[GT/GameStateFx]  ERROR!!!  ";

	// Token: 0x0400150B RID: 5387
	private bool _isValid;

	// Token: 0x0400150C RID: 5388
	[SerializeField]
	private MonoBehaviour m_stateProvider;

	// Token: 0x0400150D RID: 5389
	private IGameStateProvider _stateProvider;

	// Token: 0x0400150E RID: 5390
	[SerializeField]
	private AudioSource m_defaultAudioSource;

	// Token: 0x0400150F RID: 5391
	private bool _hasDefaultAudioSource;

	// Token: 0x04001510 RID: 5392
	[SerializeField]
	private GTEnumValueMap<GameStateFx.StateReaction[]> m_stateMap;

	// Token: 0x04001511 RID: 5393
	private int _delayedExecContextFrameNum;

	// Token: 0x04001512 RID: 5394
	private Queue<GameStateFx.StateReaction> _reactionQueue = new Queue<GameStateFx.StateReaction>(4);

	// Token: 0x04001513 RID: 5395
	private static readonly List<Material> _g_materialsCache = new List<Material>(8);

	// Token: 0x02000295 RID: 661
	[Serializable]
	internal class StateReaction
	{
		// Token: 0x04001514 RID: 5396
		[Tooltip("Options for what this reaction should do.")]
		public GameStateFx.StateReaction.EOptions options;

		// Token: 0x04001515 RID: 5397
		public float delay;

		// Token: 0x04001516 RID: 5398
		public GameStateFx.SoundEntry soundInfo;

		// Token: 0x04001517 RID: 5399
		public GameStateFx.GameObjectInfo[] gameObjectInfos;

		// Token: 0x04001518 RID: 5400
		public GameStateFx.BehaviourInfo[] behaviourInfos;

		// Token: 0x04001519 RID: 5401
		public GameStateFx.RenderInfo[] renderers;

		// Token: 0x0400151A RID: 5402
		public GameStateFx.MaterialInfo[] materialInfos;

		// Token: 0x02000296 RID: 662
		[Flags]
		public enum EOptions
		{
			// Token: 0x0400151C RID: 5404
			Delay = 1,
			// Token: 0x0400151D RID: 5405
			Sound = 2,
			// Token: 0x0400151E RID: 5406
			GameObjects = 4,
			// Token: 0x0400151F RID: 5407
			Behaviours = 8,
			// Token: 0x04001520 RID: 5408
			Renderers = 16,
			// Token: 0x04001521 RID: 5409
			Materials = 32
		}
	}

	// Token: 0x02000297 RID: 663
	[Serializable]
	public struct SoundEntry
	{
		// Token: 0x04001522 RID: 5410
		public GameStateFx.SoundEntry.EOptions options;

		// Token: 0x04001523 RID: 5411
		public AudioSource source;

		// Token: 0x04001524 RID: 5412
		public AudioResource sound;

		// Token: 0x04001525 RID: 5413
		public float volume;

		// Token: 0x04001526 RID: 5414
		public float pitch;

		// Token: 0x02000298 RID: 664
		[Flags]
		public enum EOptions
		{
			// Token: 0x04001528 RID: 5416
			Source = 1,
			// Token: 0x04001529 RID: 5417
			Sound = 2,
			// Token: 0x0400152A RID: 5418
			Volume = 4,
			// Token: 0x0400152B RID: 5419
			Pitch = 8
		}
	}

	// Token: 0x02000299 RID: 665
	[Serializable]
	internal struct GameObjectInfo
	{
		// Token: 0x0400152C RID: 5420
		public bool activate;

		// Token: 0x0400152D RID: 5421
		public GameObject gameObject;
	}

	// Token: 0x0200029A RID: 666
	[Serializable]
	internal struct BehaviourInfo
	{
		// Token: 0x0400152E RID: 5422
		public bool enable;

		// Token: 0x0400152F RID: 5423
		public Behaviour behaviour;
	}

	// Token: 0x0200029B RID: 667
	[Serializable]
	internal struct RenderInfo
	{
		// Token: 0x04001530 RID: 5424
		public bool enable;

		// Token: 0x04001531 RID: 5425
		public Renderer renderer;
	}

	// Token: 0x0200029C RID: 668
	[Serializable]
	internal struct MaterialInfo
	{
		// Token: 0x04001532 RID: 5426
		public GameStateFx.MaterialInfo.Entry[] entries;

		// Token: 0x0200029D RID: 669
		[Serializable]
		internal struct Entry
		{
			// Token: 0x04001533 RID: 5427
			public GTRendererMatSlot slotInfo;

			// Token: 0x04001534 RID: 5428
			public Material material;
		}
	}
}
