using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000201 RID: 513
public class GorillaVelocityEstimator : MonoBehaviour
{
	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000D74 RID: 3444 RVA: 0x00049B81 File Offset: 0x00047D81
	// (set) Token: 0x06000D75 RID: 3445 RVA: 0x00049B89 File Offset: 0x00047D89
	public Vector3 linearVelocity { get; private set; }

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000D76 RID: 3446 RVA: 0x00049B92 File Offset: 0x00047D92
	// (set) Token: 0x06000D77 RID: 3447 RVA: 0x00049B9A File Offset: 0x00047D9A
	public Vector3 angularVelocity { get; private set; }

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000D78 RID: 3448 RVA: 0x00049BA3 File Offset: 0x00047DA3
	// (set) Token: 0x06000D79 RID: 3449 RVA: 0x00049BAB File Offset: 0x00047DAB
	public Vector3 handPos { get; private set; }

	// Token: 0x06000D7A RID: 3450 RVA: 0x00049BB4 File Offset: 0x00047DB4
	private void Awake()
	{
		this.history = new GorillaVelocityEstimator.VelocityHistorySample[this.numFrames];
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x00049BC8 File Offset: 0x00047DC8
	private void OnEnable()
	{
		this.currentFrame = 0;
		for (int i = 0; i < this.history.Length; i++)
		{
			this.history[i] = default(GorillaVelocityEstimator.VelocityHistorySample);
		}
		this.linearVelocity = Vector3.zero;
		this.angularVelocity = Vector3.zero;
		this.lastPos = base.transform.position;
		this.lastRotation = base.transform.rotation;
		GorillaVelocityEstimatorManager.Register(this);
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00049C3F File Offset: 0x00047E3F
	private void OnDisable()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x00049C3F File Offset: 0x00047E3F
	private void OnDestroy()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x00049C48 File Offset: 0x00047E48
	public void TriggeredLateUpdate()
	{
		Vector3 vector;
		Quaternion quaternion;
		base.transform.GetPositionAndRotation(out vector, out quaternion);
		Vector3 vector2 = Vector3.zero;
		if (!this.useGlobalSpace)
		{
			vector2 = GTPlayer.Instance.InstantaneousVelocity;
		}
		Vector3 vector3 = (vector - this.lastPos) / Time.deltaTime - vector2;
		Vector3 vector4 = (quaternion * Quaternion.Inverse(this.lastRotation)).eulerAngles;
		if (vector4.x > 180f)
		{
			vector4.x -= 360f;
		}
		if (vector4.y > 180f)
		{
			vector4.y -= 360f;
		}
		if (vector4.z > 180f)
		{
			vector4.z -= 360f;
		}
		vector4 *= 0.017453292f / Time.fixedDeltaTime;
		this.linearVelocity += (vector3 - this.history[this.currentFrame].linear) / (float)this.numFrames;
		this.angularVelocity += (vector4 - this.history[this.currentFrame].angular) / (float)this.numFrames;
		this.history[this.currentFrame] = new GorillaVelocityEstimator.VelocityHistorySample
		{
			linear = vector3,
			angular = vector4
		};
		this.handPos = vector;
		this.currentFrame = (this.currentFrame + 1) % this.numFrames;
		this.lastPos = vector;
		this.lastRotation = quaternion;
	}

	// Token: 0x04001024 RID: 4132
	[Min(1f)]
	[SerializeField]
	private int numFrames = 8;

	// Token: 0x04001028 RID: 4136
	private GorillaVelocityEstimator.VelocityHistorySample[] history;

	// Token: 0x04001029 RID: 4137
	private int currentFrame;

	// Token: 0x0400102A RID: 4138
	private Vector3 lastPos;

	// Token: 0x0400102B RID: 4139
	private Quaternion lastRotation;

	// Token: 0x0400102C RID: 4140
	private Vector3 lastRotationVec;

	// Token: 0x0400102D RID: 4141
	public bool useGlobalSpace;

	// Token: 0x02000202 RID: 514
	public struct VelocityHistorySample
	{
		// Token: 0x0400102E RID: 4142
		public Vector3 linear;

		// Token: 0x0400102F RID: 4143
		public Vector3 angular;
	}
}
