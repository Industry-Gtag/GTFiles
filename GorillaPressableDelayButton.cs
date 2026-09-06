using System;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class GorillaPressableDelayButton : GorillaPressableButton, IGorillaSliceableSimple
{
	// Token: 0x1400000B RID: 11
	// (add) Token: 0x0600044B RID: 1099 RVA: 0x00019090 File Offset: 0x00017290
	// (remove) Token: 0x0600044C RID: 1100 RVA: 0x000190C8 File Offset: 0x000172C8
	public event Action onPressBegin;

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x0600044D RID: 1101 RVA: 0x00019100 File Offset: 0x00017300
	// (remove) Token: 0x0600044E RID: 1102 RVA: 0x00019138 File Offset: 0x00017338
	public event Action onPressAbort;

	// Token: 0x0600044F RID: 1103 RVA: 0x00019170 File Offset: 0x00017370
	private void Awake()
	{
		if (this.fillBar == null)
		{
			return;
		}
		this.fillBarScale = (this.fillbarStartingScale = this.fillBar.localScale);
		this.UpdateFillBar();
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x000191AC File Offset: 0x000173AC
	private new void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (this.touching)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.touching = collider;
		this.pressStartTime = Time.unscaledTime;
		this.progress = 0f;
		this.UpdateFillBar();
		Action action = this.onPressBegin;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00019227 File Offset: 0x00017427
	private void OnTriggerExit(Collider other)
	{
		if (other != this.touching)
		{
			return;
		}
		this.touching = null;
		this.progress = 0f;
		this.UpdateFillBar();
		Action action = this.onPressAbort;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00019260 File Offset: 0x00017460
	public new void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00019269 File Offset: 0x00017469
	public new void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00019274 File Offset: 0x00017474
	public void SliceUpdate()
	{
		if (this.touching == null)
		{
			return;
		}
		float num = Time.unscaledTime - this.pressStartTime;
		this.progress = ((this.delayTime > 0f) ? Mathf.Clamp01(num / this.delayTime) : ((num > 0f) ? 1f : 0f));
		if (num > this.delayTime)
		{
			base.OnTriggerEnter(this.touching);
			this.touching = null;
			this.progress = 0f;
		}
		this.UpdateFillBar();
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00019300 File Offset: 0x00017500
	public void SetFillBar(Transform newFillBar)
	{
		this.fillBar = newFillBar;
		if (this.fillBar == null)
		{
			return;
		}
		this.fillBarScale = (this.fillbarStartingScale = this.fillBar.localScale);
		this.UpdateFillBar();
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00019343 File Offset: 0x00017543
	private void UpdateFillBar()
	{
		if (this.fillBar == null)
		{
			return;
		}
		this.fillBarScale.x = this.fillbarStartingScale.x * this.progress;
		this.fillBar.localScale = this.fillBarScale;
	}

	// Token: 0x040004B8 RID: 1208
	private Collider touching;

	// Token: 0x040004B9 RID: 1209
	private float pressStartTime;

	// Token: 0x040004BA RID: 1210
	private float progress;

	// Token: 0x040004BB RID: 1211
	[SerializeField]
	[Range(0.01f, 5f)]
	public float delayTime = 1f;

	// Token: 0x040004BC RID: 1212
	[SerializeField]
	private Transform fillBar;

	// Token: 0x040004BD RID: 1213
	private Vector3 fillbarStartingScale;

	// Token: 0x040004BE RID: 1214
	private Vector3 fillBarScale;
}
