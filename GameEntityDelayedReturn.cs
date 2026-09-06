using System;
using GorillaTag.Audio;
using UnityEngine;

// Token: 0x020006C8 RID: 1736
public class GameEntityDelayedReturn : MonoBehaviour, IGameEntityComponent, IDelayedExecListener
{
	// Token: 0x06002B6F RID: 11119 RVA: 0x000E8284 File Offset: 0x000E6484
	public void OnEntityInit()
	{
		Transform transform = base.transform;
		this.initialPosition = transform.position;
		this.initialRotation = transform.rotation;
		this.initialScale = transform.localScale;
		Rigidbody componentInParent = base.GetComponentInParent<Rigidbody>();
		this.initialIsKinematic = componentInParent != null && componentInParent.isKinematic;
		this.initialized = true;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnInteractionStarted));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnInteractionStarted));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnAttached = (Action)Delegate.Combine(gameEntity3.OnAttached, new Action(this.OnInteractionStarted));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnReleased = (Action)Delegate.Combine(gameEntity4.OnReleased, new Action(this.OnInteractionEnded));
		GameEntity gameEntity5 = this.gameEntity;
		gameEntity5.OnUnsnapped = (Action)Delegate.Combine(gameEntity5.OnUnsnapped, new Action(this.OnInteractionEnded));
		GameEntity gameEntity6 = this.gameEntity;
		gameEntity6.OnDetached = (Action)Delegate.Combine(gameEntity6.OnDetached, new Action(this.OnInteractionEnded));
		if (!this.IsCurrentlyInteracting())
		{
			this.StartTimer();
		}
	}

	// Token: 0x06002B70 RID: 11120 RVA: 0x000E83DC File Offset: 0x000E65DC
	public void OnEntityDestroy()
	{
		if (!this.initialized)
		{
			return;
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnInteractionStarted));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Remove(gameEntity2.OnSnapped, new Action(this.OnInteractionStarted));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnAttached = (Action)Delegate.Remove(gameEntity3.OnAttached, new Action(this.OnInteractionStarted));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnReleased = (Action)Delegate.Remove(gameEntity4.OnReleased, new Action(this.OnInteractionEnded));
		GameEntity gameEntity5 = this.gameEntity;
		gameEntity5.OnUnsnapped = (Action)Delegate.Remove(gameEntity5.OnUnsnapped, new Action(this.OnInteractionEnded));
		GameEntity gameEntity6 = this.gameEntity;
		gameEntity6.OnDetached = (Action)Delegate.Remove(gameEntity6.OnDetached, new Action(this.OnInteractionEnded));
		this.CancelTimer();
	}

	// Token: 0x06002B71 RID: 11121 RVA: 0x000E84E2 File Offset: 0x000E66E2
	public void OnEntityStateChange(long prevState, long newState)
	{
		this.RestartTimer();
	}

	// Token: 0x06002B72 RID: 11122 RVA: 0x000E84EA File Offset: 0x000E66EA
	private void OnDestroy()
	{
		this.CancelDelayedFx();
	}

	// Token: 0x06002B73 RID: 11123 RVA: 0x000E84F2 File Offset: 0x000E66F2
	private void OnInteractionStarted()
	{
		this.CancelTimer();
	}

	// Token: 0x06002B74 RID: 11124 RVA: 0x000E84FA File Offset: 0x000E66FA
	private void OnInteractionEnded()
	{
		if (!this.IsCurrentlyInteracting())
		{
			this.StartTimer();
		}
	}

	// Token: 0x06002B75 RID: 11125 RVA: 0x000E850C File Offset: 0x000E670C
	public void StartTimer()
	{
		this.CancelDelayedFx();
		this._callGenerationId++;
		int callGenerationId = this._callGenerationId;
		GameEntityDelayedReturn.Options options = this.m_options;
		GTDelayedExec.Add(this, options.delay, (callGenerationId << 2) | 1);
		GTDelayedExec.Add(this, options.delay + options.reappearDelay, (callGenerationId << 2) | 2);
		this._timerRunning = true;
		if (options.disappearSound != null)
		{
			this._delayedDisappearAudioIndex = GTAudioOneShot.PlayDelayed(options.disappearSound, base.transform.parent, base.transform.localPosition, options.delay, options.disappearVolume, 1f);
		}
		else
		{
			this._delayedDisappearAudioIndex = -1;
		}
		if (options.pooledDisappearPrefab != null)
		{
			this._delayedDisappearPoolIndex = ObjectPools.InstantiateDelayed(options.pooledDisappearPrefab, base.transform.parent, base.transform.localPosition, options.delay);
		}
		else
		{
			this._delayedDisappearPoolIndex = -1;
		}
		if (options.beepSound == null || options.beepPhases == null || options.beepPhases.Length == 0)
		{
			return;
		}
		int num = (callGenerationId << 2) | 0;
		for (int i = 0; i < options.beepPhases.Length; i++)
		{
			float interval = options.beepPhases[i].interval;
			if (interval > 0f)
			{
				float num2 = ((i + 1 < options.beepPhases.Length) ? options.beepPhases[i + 1].timeRemaining : 0f);
				float num3 = Mathf.Min(options.beepPhases[i].timeRemaining, options.delay);
				if (num3 > num2)
				{
					float num4 = options.delay - num3;
					float num5 = options.delay - num2;
					for (float num6 = num4; num6 < num5; num6 += interval)
					{
						GTDelayedExec.Add(this, num6, num);
					}
				}
			}
		}
	}

	// Token: 0x06002B76 RID: 11126 RVA: 0x000E84FA File Offset: 0x000E66FA
	public void RestartTimer()
	{
		if (!this.IsCurrentlyInteracting())
		{
			this.StartTimer();
		}
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x000E86D4 File Offset: 0x000E68D4
	public void CancelTimer()
	{
		if (!this._timerRunning)
		{
			return;
		}
		this._callGenerationId++;
		this._timerRunning = false;
		this.CancelDelayedFx();
		if (!this.gameEntity.gameObject.activeSelf)
		{
			this.gameEntity.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002B78 RID: 11128 RVA: 0x000E8728 File Offset: 0x000E6928
	private void CancelDelayedFx()
	{
		if (this._delayedDisappearAudioIndex >= 0)
		{
			GTAudioOneShot.CancelDelayed(this._delayedDisappearAudioIndex);
			this._delayedDisappearAudioIndex = -1;
		}
		if (this._delayedDisappearPoolIndex >= 0)
		{
			ObjectPools.CancelDelayedInstantiate(this._delayedDisappearPoolIndex);
			this._delayedDisappearPoolIndex = -1;
		}
	}

	// Token: 0x06002B79 RID: 11129 RVA: 0x000E8760 File Offset: 0x000E6960
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		if (contextId >> 2 != this._callGenerationId)
		{
			return;
		}
		if (this.gameEntity == null)
		{
			return;
		}
		int num = contextId & 3;
		GameEntityDelayedReturn.Options options = this.m_options;
		switch (num)
		{
		case 0:
			if (this._delayedDisappearAudioIndex >= 0)
			{
				GTAudioOneShot.UpdateDelayed(this._delayedDisappearAudioIndex, base.transform.parent, base.transform.localPosition);
			}
			if (this._delayedDisappearPoolIndex >= 0)
			{
				ObjectPools.UpdateDelayedInstantiate(this._delayedDisappearPoolIndex, base.transform.parent, base.transform.localPosition);
			}
			if (options.beepSound != null)
			{
				GTAudioOneShot.Play(options.beepSound, base.transform.position, options.beepVolume, 1f);
				return;
			}
			break;
		case 1:
			this.Disappear();
			return;
		case 2:
			this.Reappear();
			break;
		default:
			return;
		}
	}

	// Token: 0x06002B7A RID: 11130 RVA: 0x000E8837 File Offset: 0x000E6A37
	private bool IsCurrentlyInteracting()
	{
		return this.gameEntity.IsHeld() || this.gameEntity.snappedByActorNumber != -1 || this.gameEntity.attachedToEntityId != GameEntityId.Invalid;
	}

	// Token: 0x06002B7B RID: 11131 RVA: 0x000E886B File Offset: 0x000E6A6B
	private void Disappear()
	{
		this.gameEntity.gameObject.SetActive(false);
	}

	// Token: 0x06002B7C RID: 11132 RVA: 0x000E8880 File Offset: 0x000E6A80
	private void Reappear()
	{
		this._timerRunning = false;
		if (this.resetTarget != null)
		{
			base.transform.SetPositionAndRotation(this.resetTarget.position, this.resetTarget.rotation);
			base.transform.localScale = this.resetTarget.localScale;
		}
		else
		{
			base.transform.SetPositionAndRotation(this.initialPosition, this.initialRotation);
			base.transform.localScale = this.initialScale;
		}
		Rigidbody componentInParent = base.GetComponentInParent<Rigidbody>(true);
		if (componentInParent != null)
		{
			componentInParent.linearVelocity = Vector3.zero;
			componentInParent.angularVelocity = Vector3.zero;
			componentInParent.isKinematic = this.forceKinematicOnReset || this.initialIsKinematic;
		}
		this.gameEntity.gameObject.SetActive(true);
		Vector3 position = base.transform.position;
		GameEntityDelayedReturn.Options options = this.m_options;
		if (options.reappearSound != null)
		{
			GTAudioOneShot.Play(options.reappearSound, position, options.reappearVolume, 1f);
		}
		if (options.pooledReappearPrefab != null)
		{
			ObjectPools.instance.Instantiate(options.pooledReappearPrefab, position, true);
		}
	}

	// Token: 0x06002B7D RID: 11133 RVA: 0x000E89AB File Offset: 0x000E6BAB
	public void ReturnNow()
	{
		this.CancelTimer();
		this.Disappear();
		this.Reappear();
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x000E89BF File Offset: 0x000E6BBF
	public void SetResetTarget(Transform target)
	{
		this.resetTarget = target;
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x000E89C8 File Offset: 0x000E6BC8
	internal void Configure(GameEntityDelayedReturn.Options options)
	{
		this.m_options = options;
	}

	// Token: 0x0400382F RID: 14383
	private const int k_actionBits = 2;

	// Token: 0x04003830 RID: 14384
	private const int k_actionMask = 3;

	// Token: 0x04003831 RID: 14385
	private const int k_actionBeep = 0;

	// Token: 0x04003832 RID: 14386
	private const int k_actionDisappear = 1;

	// Token: 0x04003833 RID: 14387
	private const int k_actionReappear = 2;

	// Token: 0x04003834 RID: 14388
	public GameEntity gameEntity;

	// Token: 0x04003835 RID: 14389
	[SerializeField]
	private GameEntityDelayedReturn.Options m_options = new GameEntityDelayedReturn.Options
	{
		delay = 30f,
		reappearDelay = 0.5f,
		disappearSound = null,
		disappearVolume = 1f,
		pooledDisappearPrefab = null,
		reappearSound = null,
		reappearVolume = 1f,
		pooledReappearPrefab = null,
		beepSound = null,
		beepVolume = 1f,
		beepPhases = null
	};

	// Token: 0x04003836 RID: 14390
	[Tooltip("If set, the entity teleports here instead of its initial position.")]
	public Transform resetTarget;

	// Token: 0x04003837 RID: 14391
	[Tooltip("If true, the Rigidbody is forced kinematic after return regardless of its initial state.")]
	public bool forceKinematicOnReset;

	// Token: 0x04003838 RID: 14392
	private Vector3 initialPosition;

	// Token: 0x04003839 RID: 14393
	private Quaternion initialRotation;

	// Token: 0x0400383A RID: 14394
	private Vector3 initialScale;

	// Token: 0x0400383B RID: 14395
	private bool initialIsKinematic;

	// Token: 0x0400383C RID: 14396
	private bool initialized;

	// Token: 0x0400383D RID: 14397
	private int _callGenerationId;

	// Token: 0x0400383E RID: 14398
	private int _delayedDisappearAudioIndex = -1;

	// Token: 0x0400383F RID: 14399
	private int _delayedDisappearPoolIndex = -1;

	// Token: 0x04003840 RID: 14400
	private bool _timerRunning;

	// Token: 0x020006C9 RID: 1737
	[Serializable]
	public struct Options
	{
		// Token: 0x04003841 RID: 14401
		public float delay;

		// Token: 0x04003842 RID: 14402
		[Tooltip("Seconds the entity stays hidden between disappear and reappear.")]
		public float reappearDelay;

		// Token: 0x04003843 RID: 14403
		public AudioClip disappearSound;

		// Token: 0x04003844 RID: 14404
		public float disappearVolume;

		// Token: 0x04003845 RID: 14405
		public GameObject pooledDisappearPrefab;

		// Token: 0x04003846 RID: 14406
		public AudioClip reappearSound;

		// Token: 0x04003847 RID: 14407
		public float reappearVolume;

		// Token: 0x04003848 RID: 14408
		public GameObject pooledReappearPrefab;

		// Token: 0x04003849 RID: 14409
		public AudioClip beepSound;

		// Token: 0x0400384A RID: 14410
		public float beepVolume;

		// Token: 0x0400384B RID: 14411
		[Tooltip("Beep phases keyed by seconds remaining. Must be ordered from most to least time remaining.")]
		public GameEntityDelayedReturn.BeepPhase[] beepPhases;
	}

	// Token: 0x020006CA RID: 1738
	[Serializable]
	public struct BeepPhase
	{
		// Token: 0x0400384C RID: 14412
		[Tooltip("Beeping starts when this many seconds remain.")]
		public float timeRemaining;

		// Token: 0x0400384D RID: 14413
		[Tooltip("Seconds between beeps during this phase.")]
		public float interval;
	}
}
