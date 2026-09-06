using System;
using UnityEngine;

// Token: 0x020001FB RID: 507
public class FingerFlagTwirlTest : MonoBehaviour
{
	// Token: 0x06000D55 RID: 3413 RVA: 0x0004916C File Offset: 0x0004736C
	protected void FixedUpdate()
	{
		this.animTimes += Time.deltaTime * this.rotAnimDurations;
		this.animTimes.x = this.animTimes.x % 1f;
		this.animTimes.y = this.animTimes.y % 1f;
		this.animTimes.z = this.animTimes.z % 1f;
		base.transform.localRotation = Quaternion.Euler(this.rotXAnimCurve.Evaluate(this.animTimes.x) * this.rotAnimAmplitudes.x, this.rotYAnimCurve.Evaluate(this.animTimes.y) * this.rotAnimAmplitudes.y, this.rotZAnimCurve.Evaluate(this.animTimes.z) * this.rotAnimAmplitudes.z);
	}

	// Token: 0x04000FEF RID: 4079
	public Vector3 rotAnimDurations = new Vector3(0.2f, 0.1f, 0.5f);

	// Token: 0x04000FF0 RID: 4080
	public Vector3 rotAnimAmplitudes = Vector3.one * 360f;

	// Token: 0x04000FF1 RID: 4081
	public AnimationCurve rotXAnimCurve;

	// Token: 0x04000FF2 RID: 4082
	public AnimationCurve rotYAnimCurve;

	// Token: 0x04000FF3 RID: 4083
	public AnimationCurve rotZAnimCurve;

	// Token: 0x04000FF4 RID: 4084
	private Vector3 animTimes = Vector3.zero;
}
