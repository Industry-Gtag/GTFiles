using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020002FC RID: 764
public class SpinWithGorillaSpeed : MonoBehaviour
{
	// Token: 0x0600137C RID: 4988 RVA: 0x00066F1B File Offset: 0x0006511B
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.initialRotation = base.transform.localRotation;
		this.spinAxis = this.initialRotation * this.axisOfRotation * Vector3.forward;
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x00066F5C File Offset: 0x0006515C
	private void Update()
	{
		Vector3 vector = ((this.optionalVelocityEstimator != null) ? this.optionalVelocityEstimator.linearVelocity : this.rig.LatestVelocity());
		vector.y *= this.verticalSpeedInfluence;
		float num = vector.magnitude / this.maxSpeed;
		float num2 = Time.deltaTime * this.degreesPerSecondAtSpeed.Evaluate(num) * (this.clockwise ? (-1f) : 1f);
		this.currentAngle = Mathf.Repeat(this.currentAngle + num2, 360f);
		Quaternion quaternion = this.initialRotation * Quaternion.AngleAxis(this.currentAngle, this.spinAxis);
		base.transform.SetLocalPositionAndRotation(quaternion * this.centerOfRotation, quaternion);
		if (this.tickSound != null && this.tickClips.Length != 0)
		{
			this.tickAngle += num2;
			if (this.tickAngle >= this.tickSoundDegrees)
			{
				this.tickSound.pitch = this.tickPitchAtSpeed.Evaluate(num);
				this.tickSound.volume = this.tickVolumeAtSpeed.Evaluate(num);
				this.tickSound.clip = this.tickClips.GetRandomItem<AudioClip>();
				this.tickSound.GTPlay();
				this.tickAngle = Mathf.Repeat(this.tickAngle, this.tickSoundDegrees);
			}
		}
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x000670C4 File Offset: 0x000652C4
	private void OnDisable()
	{
		this.currentAngle = 0f;
		this.tickAngle = 0f;
	}

	// Token: 0x040017CF RID: 6095
	[Tooltip("Get the velocity from this component when determining the spin speed. If this is unset, it will use the unsmoothed velocity of the parent VRRig component.")]
	[SerializeField]
	private GorillaVelocityEstimator optionalVelocityEstimator;

	// Token: 0x040017D0 RID: 6096
	[SerializeField]
	private Quaternion axisOfRotation = Quaternion.identity;

	// Token: 0x040017D1 RID: 6097
	[SerializeField]
	private Vector3 centerOfRotation = Vector3.zero;

	// Token: 0x040017D2 RID: 6098
	[Tooltip("The reported speed will be divided by this value before being used to sample AnimationCurves, to allow them to be in the range 0-1.")]
	[SerializeField]
	private float maxSpeed;

	// Token: 0x040017D3 RID: 6099
	[SerializeField]
	private AnimationCurve degreesPerSecondAtSpeed;

	// Token: 0x040017D4 RID: 6100
	[SerializeField]
	private bool clockwise;

	// Token: 0x040017D5 RID: 6101
	[Tooltip("The Y component of the reported speed will be multiplied by this value. At 0, falling will have no effect on the rotation speed.")]
	[SerializeField]
	private float verticalSpeedInfluence = 1f;

	// Token: 0x040017D6 RID: 6102
	[Header("Ticking sound")]
	[Tooltip("After this many degrees of rotation, a \"tick\" sound will play.")]
	[SerializeField]
	private float tickSoundDegrees = 360f;

	// Token: 0x040017D7 RID: 6103
	[SerializeField]
	private AnimationCurve tickVolumeAtSpeed;

	// Token: 0x040017D8 RID: 6104
	[SerializeField]
	private AnimationCurve tickPitchAtSpeed;

	// Token: 0x040017D9 RID: 6105
	[SerializeField]
	private AudioSource tickSound;

	// Token: 0x040017DA RID: 6106
	[SerializeField]
	private AudioClip[] tickClips;

	// Token: 0x040017DB RID: 6107
	private VRRig rig;

	// Token: 0x040017DC RID: 6108
	private Quaternion initialRotation;

	// Token: 0x040017DD RID: 6109
	private Vector3 spinAxis;

	// Token: 0x040017DE RID: 6110
	private float currentAngle;

	// Token: 0x040017DF RID: 6111
	private float tickAngle;
}
