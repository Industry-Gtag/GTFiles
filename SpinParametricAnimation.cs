using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020003A2 RID: 930
public class SpinParametricAnimation : MonoBehaviour
{
	// Token: 0x06001689 RID: 5769 RVA: 0x00082BE5 File Offset: 0x00080DE5
	protected void OnEnable()
	{
		this.axis = this.axis.normalized;
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x00082BF8 File Offset: 0x00080DF8
	protected void LateUpdate()
	{
		Transform transform = base.transform;
		this._animationProgress = (this._animationProgress + Time.deltaTime * this.revolutionsPerSecond) % 1f;
		float num = this.timeCurve.Evaluate(this._animationProgress) * 360f;
		float num2 = num - this._oldAngle;
		this._oldAngle = num;
		if (this.WorldSpaceRotation)
		{
			transform.rotation = Quaternion.AngleAxis(num2, this.axis) * transform.rotation;
			return;
		}
		transform.localRotation = Quaternion.AngleAxis(num2, this.axis) * transform.localRotation;
	}

	// Token: 0x040020A9 RID: 8361
	[Tooltip("Axis to rotate around.")]
	public Vector3 axis = Vector3.up;

	// Token: 0x040020AA RID: 8362
	[Tooltip("Whether rotation is in World Space or Local Space")]
	public bool WorldSpaceRotation = true;

	// Token: 0x040020AB RID: 8363
	[FormerlySerializedAs("speed")]
	[Tooltip("Speed of rotation.")]
	public float revolutionsPerSecond = 0.25f;

	// Token: 0x040020AC RID: 8364
	[Tooltip("Affects the progress of the animation over time.")]
	public AnimationCurve timeCurve;

	// Token: 0x040020AD RID: 8365
	private float _animationProgress;

	// Token: 0x040020AE RID: 8366
	private float _oldAngle;
}
