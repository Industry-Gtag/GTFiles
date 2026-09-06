using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000592 RID: 1426
public class GumBubble : LerpComponent
{
	// Token: 0x0600241A RID: 9242 RVA: 0x000C22DD File Offset: 0x000C04DD
	private void Awake()
	{
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x000C22F2 File Offset: 0x000C04F2
	public void InflateDelayed()
	{
		this.InflateDelayed(this._delayInflate);
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x000C2300 File Offset: 0x000C0500
	public void InflateDelayed(float delay)
	{
		if (delay < 0f)
		{
			delay = 0f;
		}
		base.Invoke("Inflate", delay);
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x000C2320 File Offset: 0x000C0520
	public void Inflate()
	{
		base.gameObject.SetActive(true);
		base.enabled = true;
		if (this._animating)
		{
			return;
		}
		this._animating = true;
		this._sinceInflate = 0f;
		if (this.audioSource != null && this._sfxInflate != null)
		{
			this.audioSource.GTPlayOneShot(this._sfxInflate, 1f);
		}
		UnityEvent unityEvent = this.onInflate;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000C23A4 File Offset: 0x000C05A4
	public void Pop()
	{
		this._lerp = 0f;
		base.RenderLerp();
		if (this.audioSource != null && this._sfxPop != null)
		{
			this.audioSource.GTPlayOneShot(this._sfxPop, 1f);
		}
		UnityEvent unityEvent = this.onPop;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this._done = false;
		this._animating = false;
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x000C2428 File Offset: 0x000C0628
	private void Update()
	{
		float num = Mathf.Clamp01(this._sinceInflate / this._lerpLength);
		this._lerp = Mathf.Lerp(0f, 1f, num);
		if (this._lerp <= 1f && !this._done)
		{
			base.RenderLerp();
			if (Mathf.Approximately(this._lerp, 1f))
			{
				this._done = true;
			}
		}
		float num2 = this._lerpLength + this._delayPop;
		if (this._sinceInflate >= num2)
		{
			this.Pop();
		}
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x000C24BC File Offset: 0x000C06BC
	protected override void OnLerp(float t)
	{
		if (!this.target)
		{
			return;
		}
		if (this._lerpCurve == null)
		{
			GTDev.LogError<string>("[GumBubble] Missing lerp curve", this, null);
			return;
		}
		this.target.localScale = this.targetScale * this._lerpCurve.Evaluate(t);
	}

	// Token: 0x04002F5A RID: 12122
	public Transform target;

	// Token: 0x04002F5B RID: 12123
	public Vector3 targetScale = Vector3.one;

	// Token: 0x04002F5C RID: 12124
	[SerializeField]
	private AnimationCurve _lerpCurve;

	// Token: 0x04002F5D RID: 12125
	public AudioSource audioSource;

	// Token: 0x04002F5E RID: 12126
	[SerializeField]
	private AudioClip _sfxInflate;

	// Token: 0x04002F5F RID: 12127
	[SerializeField]
	private AudioClip _sfxPop;

	// Token: 0x04002F60 RID: 12128
	[SerializeField]
	private float _delayInflate = 1.16f;

	// Token: 0x04002F61 RID: 12129
	[FormerlySerializedAs("_popDelay")]
	[SerializeField]
	private float _delayPop = 0.5f;

	// Token: 0x04002F62 RID: 12130
	[SerializeField]
	private bool _animating;

	// Token: 0x04002F63 RID: 12131
	public UnityEvent onPop;

	// Token: 0x04002F64 RID: 12132
	public UnityEvent onInflate;

	// Token: 0x04002F65 RID: 12133
	[NonSerialized]
	private bool _done;

	// Token: 0x04002F66 RID: 12134
	[NonSerialized]
	private TimeSince _sinceInflate;
}
