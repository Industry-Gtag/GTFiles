using System;
using UnityEngine;

// Token: 0x02000B04 RID: 2820
public class LerpScale : LerpComponent
{
	// Token: 0x06004836 RID: 18486 RVA: 0x00184D68 File Offset: 0x00182F68
	protected override void OnLerp(float t)
	{
		this.current = Vector3.Lerp(this.start, this.end, this.scaleCurve.Evaluate(t));
		if (this.target)
		{
			this.target.localScale = this.current;
		}
	}

	// Token: 0x04005A9F RID: 23199
	[Space]
	public Transform target;

	// Token: 0x04005AA0 RID: 23200
	[Space]
	public Vector3 start = Vector3.one;

	// Token: 0x04005AA1 RID: 23201
	public Vector3 end = Vector3.one;

	// Token: 0x04005AA2 RID: 23202
	public Vector3 current;

	// Token: 0x04005AA3 RID: 23203
	[SerializeField]
	private AnimationCurve scaleCurve = AnimationCurves.EaseInOutBounce;
}
