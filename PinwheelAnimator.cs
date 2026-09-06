using System;
using UnityEngine;

// Token: 0x0200020E RID: 526
public class PinwheelAnimator : MonoBehaviour
{
	// Token: 0x06000DE7 RID: 3559 RVA: 0x0004C501 File Offset: 0x0004A701
	protected void OnEnable()
	{
		this.oldPos = this.spinnerTransform.position;
		this.spinSpeed = 0f;
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x0004C520 File Offset: 0x0004A720
	protected void LateUpdate()
	{
		Vector3 position = this.spinnerTransform.position;
		Vector3 forward = base.transform.forward;
		Vector3 vector = position - this.oldPos;
		float num = Mathf.Clamp(vector.magnitude / Time.deltaTime * Vector3.Dot(vector.normalized, forward) * this.spinSpeedMultiplier, -this.maxSpinSpeed, this.maxSpinSpeed);
		this.spinSpeed = Mathf.Lerp(this.spinSpeed, num, Time.deltaTime * this.damping);
		this.spinnerTransform.Rotate(Vector3.forward, this.spinSpeed * 360f * Time.deltaTime);
		this.oldPos = position;
	}

	// Token: 0x04001090 RID: 4240
	public Transform spinnerTransform;

	// Token: 0x04001091 RID: 4241
	[Tooltip("In revolutions per second.")]
	public float maxSpinSpeed = 4f;

	// Token: 0x04001092 RID: 4242
	public float spinSpeedMultiplier = 5f;

	// Token: 0x04001093 RID: 4243
	public float damping = 0.5f;

	// Token: 0x04001094 RID: 4244
	private Vector3 oldPos;

	// Token: 0x04001095 RID: 4245
	private float spinSpeed;
}
