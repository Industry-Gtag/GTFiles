using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000B02 RID: 2818
public abstract class LerpComponent : MonoBehaviour
{
	// Token: 0x170006D2 RID: 1746
	// (get) Token: 0x06004826 RID: 18470 RVA: 0x00184C8C File Offset: 0x00182E8C
	// (set) Token: 0x06004827 RID: 18471 RVA: 0x00184C94 File Offset: 0x00182E94
	public float Lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (!Mathf.Approximately(this._lerp, num))
			{
				LerpChangedEvent onLerpChanged = this._onLerpChanged;
				if (onLerpChanged != null)
				{
					onLerpChanged.Invoke(num);
				}
			}
			this._lerp = num;
		}
	}

	// Token: 0x170006D3 RID: 1747
	// (get) Token: 0x06004828 RID: 18472 RVA: 0x00184CCF File Offset: 0x00182ECF
	// (set) Token: 0x06004829 RID: 18473 RVA: 0x00184CD7 File Offset: 0x00182ED7
	public float LerpTime
	{
		get
		{
			return this._lerpLength;
		}
		set
		{
			this._lerpLength = ((value < 0f) ? 0f : value);
		}
	}

	// Token: 0x170006D4 RID: 1748
	// (get) Token: 0x0600482A RID: 18474 RVA: 0x00023F0C File Offset: 0x0002210C
	protected virtual bool CanRender
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600482B RID: 18475
	protected abstract void OnLerp(float t);

	// Token: 0x0600482C RID: 18476 RVA: 0x00184CEF File Offset: 0x00182EEF
	protected void RenderLerp()
	{
		this.OnLerp(this._lerp);
	}

	// Token: 0x0600482D RID: 18477 RVA: 0x00184D00 File Offset: 0x00182F00
	protected virtual int GetState()
	{
		return new ValueTuple<float, int>(this._lerp, 779562875).GetHashCode();
	}

	// Token: 0x0600482E RID: 18478 RVA: 0x00184D2B File Offset: 0x00182F2B
	protected virtual void Validate()
	{
		if (this._lerpLength < 0f)
		{
			this._lerpLength = 0f;
		}
	}

	// Token: 0x0600482F RID: 18479 RVA: 0x00002C2D File Offset: 0x00000E2D
	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x06004830 RID: 18480 RVA: 0x00002C2D File Offset: 0x00000E2D
	[Conditional("UNITY_EDITOR")]
	private void TryEditorRender(bool playModeCheck = true)
	{
	}

	// Token: 0x06004831 RID: 18481 RVA: 0x00002C2D File Offset: 0x00000E2D
	[Conditional("UNITY_EDITOR")]
	private void LerpToOne()
	{
	}

	// Token: 0x06004832 RID: 18482 RVA: 0x00002C2D File Offset: 0x00000E2D
	[Conditional("UNITY_EDITOR")]
	private void LerpToZero()
	{
	}

	// Token: 0x06004833 RID: 18483 RVA: 0x00002C2D File Offset: 0x00000E2D
	[Conditional("UNITY_EDITOR")]
	private void StartPreview(float lerpFrom, float lerpTo)
	{
	}

	// Token: 0x04005A95 RID: 23189
	[SerializeField]
	[Range(0f, 1f)]
	protected float _lerp;

	// Token: 0x04005A96 RID: 23190
	[SerializeField]
	protected float _lerpLength = 1f;

	// Token: 0x04005A97 RID: 23191
	[Space]
	[SerializeField]
	protected LerpChangedEvent _onLerpChanged;

	// Token: 0x04005A98 RID: 23192
	[SerializeField]
	protected bool _previewInEditor = true;

	// Token: 0x04005A99 RID: 23193
	[NonSerialized]
	private bool _previewing;

	// Token: 0x04005A9A RID: 23194
	[NonSerialized]
	private bool _cancelPreview;

	// Token: 0x04005A9B RID: 23195
	[NonSerialized]
	private bool _rendering;

	// Token: 0x04005A9C RID: 23196
	[NonSerialized]
	private int _lastState;

	// Token: 0x04005A9D RID: 23197
	[NonSerialized]
	private float _prevLerpFrom;

	// Token: 0x04005A9E RID: 23198
	[NonSerialized]
	private float _prevLerpTo;
}
