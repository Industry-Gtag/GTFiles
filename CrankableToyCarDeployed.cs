using System;
using UnityEngine;

// Token: 0x020002BB RID: 699
public class CrankableToyCarDeployed : MonoBehaviour
{
	// Token: 0x0600120B RID: 4619 RVA: 0x00060CC4 File Offset: 0x0005EEC4
	public void Deploy(CrankableToyCarHoldable holdable, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		this.holdable = holdable;
		holdable.OnCarDeployed();
		base.transform.position = launchPos;
		base.transform.rotation = launchRot;
		base.transform.localScale = holdable.transform.lossyScale;
		this.rb.linearVelocity = releaseVel;
		this.startedAtTimestamp = Time.time;
		this.expiresAtTimestamp = Time.time + lifetime;
		this.isRemote = isRemote;
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x00060D3C File Offset: 0x0005EF3C
	private void Update()
	{
		if (!this.isRemote && Time.time > this.expiresAtTimestamp)
		{
			if (this.holdable != null)
			{
				this.holdable.OnCarReturned();
			}
			return;
		}
		if (!this.wheelDriver.hasCollision)
		{
			this.expiresAtTimestamp -= Time.deltaTime;
			if (!this.offGroundDrivingAudio.isPlaying)
			{
				this.offGroundDrivingAudio.GTPlay();
				this.drivingAudio.Stop();
			}
		}
		else if (!this.drivingAudio.isPlaying)
		{
			this.drivingAudio.GTPlay();
			this.offGroundDrivingAudio.Stop();
		}
		float num = Mathf.InverseLerp(this.startedAtTimestamp, this.expiresAtTimestamp, Time.time);
		float num2 = this.thrustCurve.Evaluate(num);
		this.wheelDriver.SetThrust(this.maxThrust * num2);
	}

	// Token: 0x040015C0 RID: 5568
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x040015C1 RID: 5569
	[SerializeField]
	private FakeWheelDriver wheelDriver;

	// Token: 0x040015C2 RID: 5570
	[SerializeField]
	private Vector3 maxThrust;

	// Token: 0x040015C3 RID: 5571
	[SerializeField]
	private AnimationCurve thrustCurve;

	// Token: 0x040015C4 RID: 5572
	private float startedAtTimestamp;

	// Token: 0x040015C5 RID: 5573
	private float expiresAtTimestamp;

	// Token: 0x040015C6 RID: 5574
	private CrankableToyCarHoldable holdable;

	// Token: 0x040015C7 RID: 5575
	[SerializeField]
	private AudioSource drivingAudio;

	// Token: 0x040015C8 RID: 5576
	[SerializeField]
	private AudioSource offGroundDrivingAudio;

	// Token: 0x040015C9 RID: 5577
	private bool isRemote;
}
