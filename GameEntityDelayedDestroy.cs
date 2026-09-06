using System;
using GorillaTag.Audio;
using UnityEngine;

// Token: 0x020006C5 RID: 1733
public class GameEntityDelayedDestroy : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x06002B69 RID: 11113 RVA: 0x000E7E64 File Offset: 0x000E6064
	internal void Configure(GameEntityDelayedDestroy.Options options)
	{
		this.m_options = options;
		if ((this.m_options.beepSound != null || this.m_options.explosionSound != null) && this.m_options.audioSource == null)
		{
			this.m_options.audioSource = base.GetComponentInChildren<AudioSource>();
		}
	}

	// Token: 0x06002B6A RID: 11114 RVA: 0x000E7EC2 File Offset: 0x000E60C2
	protected void OnDestroy()
	{
		if (this._delayedExplosionAudioIndex >= 0)
		{
			GTAudioOneShot.CancelDelayed(this._delayedExplosionAudioIndex);
			this._delayedExplosionAudioIndex = -1;
		}
		if (this._delayedExplosionPoolIndex >= 0)
		{
			ObjectPools.CancelDelayedInstantiate(this._delayedExplosionPoolIndex);
			this._delayedExplosionPoolIndex = -1;
		}
	}

	// Token: 0x06002B6B RID: 11115 RVA: 0x000E7EFA File Offset: 0x000E60FA
	protected void Start()
	{
		this._entity = base.GetComponent<GameEntity>();
		if (this._entity == null)
		{
			Debug.LogError("GameEntityDelayedDestroy: No GameEntity found. Must be added to the same GameObject of the GameEntity you are trying to destroy with a delay.");
			return;
		}
		GTDelayedExec.Add(this, 0f, 0);
	}

	// Token: 0x06002B6C RID: 11116 RVA: 0x000E7F30 File Offset: 0x000E6130
	internal void ResetTimer()
	{
		this._callGenerationId++;
		int callGenerationId = this._callGenerationId;
		int num = (callGenerationId << 1) | 1;
		GameEntityDelayedDestroy.Options options = this.m_options;
		GTDelayedExec.Add(this, options.delay, num);
		if (options.explosionSound != null)
		{
			this._delayedExplosionAudioIndex = GTAudioOneShot.PlayDelayed(options.explosionSound, base.transform.parent, base.transform.localPosition, options.delay, options.explosionVolume, 1f);
		}
		else
		{
			this._delayedExplosionAudioIndex = -1;
		}
		if (options.pooledExplosionPrefab != null)
		{
			this._delayedExplosionPoolIndex = ObjectPools.InstantiateDelayed(options.pooledExplosionPrefab, base.transform.parent, base.transform.localPosition, options.delay);
		}
		else
		{
			this._delayedExplosionPoolIndex = -1;
		}
		if (options.beepSound == null || options.beepPhases == null || options.beepPhases.Length == 0)
		{
			return;
		}
		int num2 = callGenerationId << 1;
		for (int i = 0; i < options.beepPhases.Length; i++)
		{
			float interval = options.beepPhases[i].interval;
			if (interval > 0f)
			{
				float num3 = ((i + 1 < options.beepPhases.Length) ? options.beepPhases[i + 1].timeRemaining : 0f);
				float num4 = Mathf.Min(options.beepPhases[i].timeRemaining, options.delay);
				if (num4 > num3)
				{
					float num5 = options.delay - num4;
					float num6 = options.delay - num3;
					for (float num7 = num5; num7 < num6; num7 += interval)
					{
						GTDelayedExec.Add(this, num7, num2);
					}
				}
			}
		}
	}

	// Token: 0x06002B6D RID: 11117 RVA: 0x000E80DC File Offset: 0x000E62DC
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		if (contextId == 0)
		{
			if (this._callGenerationId == 0 && this._entity != null)
			{
				this.ResetTimer();
			}
			return;
		}
		if (contextId >> 1 != this._callGenerationId)
		{
			return;
		}
		if (this._entity == null)
		{
			return;
		}
		GameEntityDelayedDestroy.Options options = this.m_options;
		if ((contextId & 1) != 0)
		{
			this._entity.manager.RequestDestroyItem(this._entity.id);
			return;
		}
		if (this._delayedExplosionAudioIndex >= 0)
		{
			GTAudioOneShot.UpdateDelayed(this._delayedExplosionAudioIndex, base.transform.parent, base.transform.localPosition);
		}
		if (this._delayedExplosionPoolIndex >= 0)
		{
			ObjectPools.UpdateDelayedInstantiate(this._delayedExplosionPoolIndex, base.transform.parent, base.transform.localPosition);
		}
		if (options.beepSound != null)
		{
			if (options.audioSource != null && options.audioSource.isActiveAndEnabled)
			{
				options.audioSource.GTPlayOneShot(options.beepSound, options.beepVolume);
				return;
			}
			GTAudioOneShot.Play(options.beepSound, base.transform.position, options.beepVolume, 1f);
		}
	}

	// Token: 0x0400381F RID: 14367
	[SerializeField]
	private GameEntityDelayedDestroy.Options m_options = new GameEntityDelayedDestroy.Options
	{
		delay = 3f,
		audioSource = null,
		explosionSound = null,
		explosionVolume = 1f,
		pooledExplosionPrefab = null,
		beepSound = null,
		beepVolume = 1f,
		beepPhases = null
	};

	// Token: 0x04003820 RID: 14368
	private GameEntity _entity;

	// Token: 0x04003821 RID: 14369
	private int _callGenerationId;

	// Token: 0x04003822 RID: 14370
	private int _delayedExplosionAudioIndex = -1;

	// Token: 0x04003823 RID: 14371
	private int _delayedExplosionPoolIndex = -1;

	// Token: 0x04003824 RID: 14372
	private const int k_contextId_deferredStart = 0;

	// Token: 0x020006C6 RID: 1734
	[Serializable]
	public struct Options
	{
		// Token: 0x04003825 RID: 14373
		public float delay;

		// Token: 0x04003826 RID: 14374
		[Tooltip("Optional. If not set then a sound will be played at the transforms position. Which if it is a long clip on a transform that moves a lot then it will feel wrong without this set.")]
		public AudioSource audioSource;

		// Token: 0x04003827 RID: 14375
		public AudioClip explosionSound;

		// Token: 0x04003828 RID: 14376
		public float explosionVolume;

		// Token: 0x04003829 RID: 14377
		public GameObject pooledExplosionPrefab;

		// Token: 0x0400382A RID: 14378
		public AudioClip beepSound;

		// Token: 0x0400382B RID: 14379
		public float beepVolume;

		// Token: 0x0400382C RID: 14380
		[Tooltip("Beep phases keyed by seconds remaining. Must be ordered from most to least time remaining.")]
		public GameEntityDelayedDestroy.BeepPhase[] beepPhases;
	}

	// Token: 0x020006C7 RID: 1735
	[Serializable]
	public struct BeepPhase
	{
		// Token: 0x0400382D RID: 14381
		[Tooltip("Beeping starts when this many seconds remain.")]
		public float timeRemaining;

		// Token: 0x0400382E RID: 14382
		[Tooltip("Seconds between beeps during this phase.")]
		public float interval;
	}
}
