using System;
using UnityEngine;

// Token: 0x02000133 RID: 307
public class GrowOnEnable : MonoBehaviour, ITickSystemTick
{
	// Token: 0x060007A2 RID: 1954 RVA: 0x0002A5D3 File Offset: 0x000287D3
	private void Awake()
	{
		this._targetScale = base.transform.localScale;
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0002A5E6 File Offset: 0x000287E6
	private void OnEnable()
	{
		this._lerpVal = 0f;
		this._curve = AnimationCurves.GetCurveForEase(this.easeType);
		this.UpdateScale();
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0002A610 File Offset: 0x00028810
	private void OnDisable()
	{
		base.transform.localScale = this._targetScale;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x060007A5 RID: 1957 RVA: 0x0002A629 File Offset: 0x00028829
	// (set) Token: 0x060007A6 RID: 1958 RVA: 0x0002A631 File Offset: 0x00028831
	public bool TickRunning { get; set; }

	// Token: 0x060007A7 RID: 1959 RVA: 0x0002A63A File Offset: 0x0002883A
	public void Tick()
	{
		this._lerpVal = Mathf.Clamp01(this._lerpVal + Time.deltaTime / this.growDuration);
		this.UpdateScale();
		if (this._lerpVal >= 1f)
		{
			TickSystem<object>.RemoveTickCallback(this);
		}
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0002A673 File Offset: 0x00028873
	private void UpdateScale()
	{
		base.transform.localScale = this._targetScale * this._curve.Evaluate(this._lerpVal);
	}

	// Token: 0x040009C2 RID: 2498
	[SerializeField]
	private float growDuration = 1f;

	// Token: 0x040009C3 RID: 2499
	[SerializeField]
	private AnimationCurves.EaseType easeType = AnimationCurves.EaseType.EaseOutBack;

	// Token: 0x040009C4 RID: 2500
	private AnimationCurve _curve;

	// Token: 0x040009C5 RID: 2501
	private Vector3 _targetScale;

	// Token: 0x040009C6 RID: 2502
	private float _lerpVal;
}
