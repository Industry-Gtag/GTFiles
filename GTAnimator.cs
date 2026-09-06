using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002A0 RID: 672
public class GTAnimator : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x170001BA RID: 442
	// (get) Token: 0x060011B3 RID: 4531 RVA: 0x0005F273 File Offset: 0x0005D473
	public Animation animationComponent
	{
		get
		{
			return this.m_animationComponent;
		}
	}

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x060011B4 RID: 4532 RVA: 0x0005F27B File Offset: 0x0005D47B
	// (set) Token: 0x060011B5 RID: 4533 RVA: 0x0005F283 File Offset: 0x0005D483
	public bool hasAnimationComponent { get; private set; }

	// Token: 0x060011B6 RID: 4534 RVA: 0x0005F28C File Offset: 0x0005D48C
	protected void Awake()
	{
		this.Init();
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x0005F294 File Offset: 0x0005D494
	public void Init()
	{
		if (this._wasInitCalled)
		{
			return;
		}
		this._wasInitCalled = true;
		this.hasAnimationComponent = this.m_animationComponent != null;
		bool hasAnimationComponent = this.hasAnimationComponent;
		this.m_animationMap.Init();
		foreach (GTAnimator.AnimClipAndGObjs animClipAndGObjs in this.m_animationMap.Values)
		{
			this._allStaticGobjs.UnionWith(animClipAndGObjs.endStaticGameObjects);
		}
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEnable()
	{
	}

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x060011B9 RID: 4537 RVA: 0x0005F324 File Offset: 0x0005D524
	public bool IsPlaying
	{
		get
		{
			return this.m_animationComponent.isPlaying;
		}
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x0005F331 File Offset: 0x0005D531
	public void SetState(long enumValueAsLong)
	{
		if (!this._wasInitCalled)
		{
			this.Init();
		}
		if (this._currentStateAsLong != enumValueAsLong)
		{
			this.TryPlay(enumValueAsLong);
		}
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0005F354 File Offset: 0x0005D554
	public bool TryPlay(long enumValueAsLong)
	{
		GTAnimator.AnimClipAndGObjs animClipAndGObjs;
		if (!this.hasAnimationComponent || !this.m_animationMap.TryGet(enumValueAsLong, out animClipAndGObjs))
		{
			return false;
		}
		foreach (GameObject gameObject in this._allStaticGobjs)
		{
			gameObject.SetActive(false);
		}
		GameObject[] animatedGameObjects = this.m_animatedGameObjects;
		for (int i = 0; i < animatedGameObjects.Length; i++)
		{
			animatedGameObjects[i].SetActive(true);
		}
		this._currentStateAsLong = enumValueAsLong;
		this.m_animationComponent.clip = animClipAndGObjs.animClip;
		this.m_animationComponent.Play();
		if (animClipAndGObjs.soundBankToPlayOnStart)
		{
			animClipAndGObjs.soundBankToPlayOnStart.Play();
		}
		if (!animClipAndGObjs.animClip.isLooping)
		{
			this._frameCountWhenLastPlayed = Time.frameCount;
			GTDelayedExec.Add(this, animClipAndGObjs.animClip.length, this._frameCountWhenLastPlayed);
		}
		return true;
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x0005F44C File Offset: 0x0005D64C
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		if (!base.enabled || this._frameCountWhenLastPlayed != contextId)
		{
			return;
		}
		this.m_animationComponent.Stop();
		for (int i = 0; i < this.m_animatedGameObjects.Length; i++)
		{
			if (this.m_animatedGameObjects[i] != null)
			{
				this.m_animatedGameObjects[i].SetActive(false);
			}
		}
		GTAnimator.AnimClipAndGObjs animClipAndGObjs;
		GameObject[] array;
		if (this.m_animationMap.TryGet(this._currentStateAsLong, out animClipAndGObjs) && animClipAndGObjs.endStaticGameObjects != null && animClipAndGObjs.endStaticGameObjects.Length != 0)
		{
			array = animClipAndGObjs.endStaticGameObjects;
		}
		else
		{
			array = this.m_defaultStaticGameObjects;
		}
		if (array != null)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != null)
				{
					array[j].SetActive(true);
				}
			}
		}
		if (this._queuedStateAsLong != -9223372036854775808L)
		{
			long queuedStateAsLong = this._queuedStateAsLong;
			this._queuedStateAsLong = long.MinValue;
			this.TryPlay(queuedStateAsLong);
		}
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x0005F533 File Offset: 0x0005D733
	public void Stop()
	{
		if (this.m_animationComponent != null)
		{
			this.m_animationComponent.Stop();
		}
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x0005F550 File Offset: 0x0005D750
	public void QueueState(long enumValueAsLong)
	{
		if (!this._wasInitCalled)
		{
			this.Init();
		}
		if (this._queuedStateAsLong == enumValueAsLong || this._currentStateAsLong == enumValueAsLong)
		{
			return;
		}
		if (!this.IsPlaying || this._IsCurrentClipLoopable())
		{
			this.TryPlay(enumValueAsLong);
			return;
		}
		this._queuedStateAsLong = enumValueAsLong;
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x0005F5A0 File Offset: 0x0005D7A0
	private bool _IsCurrentClipLoopable()
	{
		if (this.m_animationComponent == null)
		{
			return false;
		}
		AnimationClip clip = this.m_animationComponent.clip;
		if (clip == null)
		{
			return false;
		}
		WrapMode wrapMode = clip.wrapMode;
		return wrapMode == WrapMode.Loop || wrapMode == WrapMode.PingPong;
	}

	// Token: 0x04001535 RID: 5429
	private const string preLog = "[GTAnimator]  ";

	// Token: 0x04001536 RID: 5430
	private const string preErr = "[GTAnimator]  ERROR!!!  ";

	// Token: 0x04001537 RID: 5431
	private const string preErrBeta = "[GTAnimator]  ERROR!!!  (beta only log)  ";

	// Token: 0x04001538 RID: 5432
	[Tooltip("Assign a unity Animation component (not to be confused with less performant Animator Component).")]
	[SerializeField]
	private Animation m_animationComponent;

	// Token: 0x0400153A RID: 5434
	[Tooltip("These will be activated when animation starts playing and deactivated when any anim finishes playing.")]
	[SerializeField]
	private GameObject[] m_animatedGameObjects;

	// Token: 0x0400153B RID: 5435
	[Tooltip("If an enum map value is not defined then these will be activated.")]
	[SerializeField]
	private GameObject[] m_defaultStaticGameObjects;

	// Token: 0x0400153C RID: 5436
	[Header("Enum To Animation Mapping")]
	[Tooltip("Map an enum's values to specific AnimationClips.")]
	[SerializeField]
	internal GTEnumValueMap<GTAnimator.AnimClipAndGObjs> m_animationMap;

	// Token: 0x0400153D RID: 5437
	private readonly HashSet<GameObject> _allStaticGobjs = new HashSet<GameObject>();

	// Token: 0x0400153E RID: 5438
	private const long _k_invalidState = -9223372036854775808L;

	// Token: 0x0400153F RID: 5439
	private long _currentStateAsLong = long.MinValue;

	// Token: 0x04001540 RID: 5440
	private int _frameCountWhenLastPlayed;

	// Token: 0x04001541 RID: 5441
	private bool _wasInitCalled;

	// Token: 0x04001542 RID: 5442
	private long _queuedStateAsLong = long.MinValue;

	// Token: 0x020002A1 RID: 673
	[Serializable]
	public struct AnimClipAndGObjs
	{
		// Token: 0x04001543 RID: 5443
		public AnimationClip animClip;

		// Token: 0x04001544 RID: 5444
		public SoundBankPlayer soundBankToPlayOnStart;

		// Token: 0x04001545 RID: 5445
		[Tooltip("These GameObjects will be activated when the animation clip finishes playing.")]
		public GameObject[] endStaticGameObjects;
	}
}
