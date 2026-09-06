using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000B19 RID: 2841
public class SubEmitterListener : MonoBehaviour
{
	// Token: 0x060048E5 RID: 18661 RVA: 0x00186D38 File Offset: 0x00184F38
	private void OnEnable()
	{
		if (this.target == null)
		{
			this.Disable();
			return;
		}
		ParticleSystem.SubEmittersModule subEmitters = this.target.subEmitters;
		if (this.subEmitterIndex < 0)
		{
			this.subEmitterIndex = 0;
		}
		this._canListen = subEmitters.subEmittersCount > 0 && this.subEmitterIndex <= subEmitters.subEmittersCount - 1;
		if (!this._canListen)
		{
			this.Disable();
			return;
		}
		this.subEmitter = this.target.subEmitters.GetSubEmitterSystem(this.subEmitterIndex);
		ParticleSystem.MainModule main = this.subEmitter.main;
		this.interval = main.startLifetime.constantMax * main.startLifetimeMultiplier;
	}

	// Token: 0x060048E6 RID: 18662 RVA: 0x00186DF4 File Offset: 0x00184FF4
	private void OnDisable()
	{
		this._listenOnce = false;
		this._listening = false;
	}

	// Token: 0x060048E7 RID: 18663 RVA: 0x00186E04 File Offset: 0x00185004
	public void ListenStart()
	{
		if (this._listening)
		{
			return;
		}
		if (this._canListen)
		{
			this.Enable();
			this._listening = true;
		}
	}

	// Token: 0x060048E8 RID: 18664 RVA: 0x00186E24 File Offset: 0x00185024
	public void ListenStop()
	{
		this.Disable();
	}

	// Token: 0x060048E9 RID: 18665 RVA: 0x00186E2C File Offset: 0x0018502C
	public void ListenOnce()
	{
		if (this._listening)
		{
			return;
		}
		this.Enable();
		if (this._canListen)
		{
			this.Enable();
			this._listenOnce = true;
			this._listening = true;
		}
	}

	// Token: 0x060048EA RID: 18666 RVA: 0x00186E5C File Offset: 0x0018505C
	private void Update()
	{
		if (!this._canListen)
		{
			return;
		}
		if (!this._listening)
		{
			return;
		}
		if (this.subEmitter.particleCount > 0 && this._sinceLastEmit >= this.interval * this.intervalScale)
		{
			this._sinceLastEmit = 0f;
			this.OnSubEmit();
			if (this._listenOnce)
			{
				this.Disable();
			}
		}
	}

	// Token: 0x060048EB RID: 18667 RVA: 0x00186EC7 File Offset: 0x001850C7
	protected virtual void OnSubEmit()
	{
		UnityEvent unityEvent = this.onSubEmit;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060048EC RID: 18668 RVA: 0x00186ED9 File Offset: 0x001850D9
	public void Enable()
	{
		if (!base.enabled)
		{
			base.enabled = true;
		}
	}

	// Token: 0x060048ED RID: 18669 RVA: 0x00186EEA File Offset: 0x001850EA
	public void Disable()
	{
		if (base.enabled)
		{
			base.enabled = false;
		}
	}

	// Token: 0x04005B28 RID: 23336
	public ParticleSystem target;

	// Token: 0x04005B29 RID: 23337
	public ParticleSystem subEmitter;

	// Token: 0x04005B2A RID: 23338
	public int subEmitterIndex;

	// Token: 0x04005B2B RID: 23339
	public UnityEvent onSubEmit;

	// Token: 0x04005B2C RID: 23340
	public float intervalScale = 1f;

	// Token: 0x04005B2D RID: 23341
	public float interval;

	// Token: 0x04005B2E RID: 23342
	[NonSerialized]
	private bool _canListen;

	// Token: 0x04005B2F RID: 23343
	[NonSerialized]
	private bool _listening;

	// Token: 0x04005B30 RID: 23344
	[NonSerialized]
	private bool _listenOnce;

	// Token: 0x04005B31 RID: 23345
	[NonSerialized]
	private TimeSince _sinceLastEmit;
}
