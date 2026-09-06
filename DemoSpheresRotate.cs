using System;
using PerformanceSystems;
using UnityEngine;

// Token: 0x02000D77 RID: 3447
public class DemoSpheresRotate : TimeSliceLodBehaviour
{
	// Token: 0x060054FE RID: 21758 RVA: 0x001BDE79 File Offset: 0x001BC079
	public void OnLod0Enter()
	{
		this._renderer.material = this._red;
		this.SwapToTimeSlicer(0);
		base.gameObject.SetActive(true);
	}

	// Token: 0x060054FF RID: 21759 RVA: 0x001BDE9F File Offset: 0x001BC09F
	public void OnLod1Enter()
	{
		this._renderer.material = this._green;
		this.SwapToTimeSlicer(1);
		base.gameObject.SetActive(true);
	}

	// Token: 0x06005500 RID: 21760 RVA: 0x001BDEC5 File Offset: 0x001BC0C5
	public void OnLod2Enter()
	{
		this._renderer.material = this._black;
		this.SwapToTimeSlicer(2);
		base.gameObject.SetActive(true);
	}

	// Token: 0x06005501 RID: 21761 RVA: 0x00044B04 File Offset: 0x00042D04
	public void OnLodExit()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06005502 RID: 21762 RVA: 0x001BDEEB File Offset: 0x001BC0EB
	public override void SliceUpdate(float deltaTime)
	{
		base.transform.Rotate(Vector3.up * this._rotationSpeed * deltaTime);
	}

	// Token: 0x06005503 RID: 21763 RVA: 0x001BDF0E File Offset: 0x001BC10E
	private void SwapToTimeSlicer(int index)
	{
		if (this._timeSliceControllerAssets[index] == this._timeSliceControllerAsset)
		{
			return;
		}
		this._timeSliceControllerAsset.RemoveTimeSliceBehaviour(this);
		this._timeSliceControllerAsset = this._timeSliceControllerAssets[index];
		this._timeSliceControllerAsset.AddTimeSliceBehaviour(this);
	}

	// Token: 0x040066AF RID: 26287
	[SerializeField]
	private TimeSliceControllerAsset[] _timeSliceControllerAssets;

	// Token: 0x040066B0 RID: 26288
	[SerializeField]
	private float _rotationSpeed = 10f;

	// Token: 0x040066B1 RID: 26289
	[SerializeField]
	private Material _red;

	// Token: 0x040066B2 RID: 26290
	[SerializeField]
	private Material _green;

	// Token: 0x040066B3 RID: 26291
	[SerializeField]
	private Material _black;

	// Token: 0x040066B4 RID: 26292
	[SerializeField]
	private Renderer _renderer;
}
