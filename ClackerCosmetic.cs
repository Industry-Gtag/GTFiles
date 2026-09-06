using System;
using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020002B5 RID: 693
public class ClackerCosmetic : MonoBehaviour
{
	// Token: 0x060011FB RID: 4603 RVA: 0x000605BC File Offset: 0x0005E7BC
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.arm1.parent = this;
		this.arm2.parent = this;
		this.arm1.transform = this.clackerArm1;
		this.arm2.transform = this.clackerArm2;
		this.arm1.lastWorldPosition = this.clackerArm1.transform.TransformPoint(this.LocalCenterOfMass);
		this.arm2.lastWorldPosition = this.clackerArm2.transform.TransformPoint(this.LocalCenterOfMass);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x00060678 File Offset: 0x0005E878
	private void Update()
	{
		Vector3 lastWorldPosition = this.arm1.lastWorldPosition;
		this.arm1.UpdateArm();
		this.arm2.UpdateArm();
		ref Vector3 eulerAngles = this.clackerArm1.transform.eulerAngles;
		Vector3 eulerAngles2 = this.clackerArm2.transform.eulerAngles;
		Mathf.DeltaAngle(eulerAngles.y, eulerAngles2.y);
		if ((this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).IsShorterThan(this.collisionDistance))
		{
			float sqrMagnitude = (this.arm1.velocity - this.arm2.velocity).sqrMagnitude;
			if (this.parentHoldable.InHand())
			{
				if (sqrMagnitude > this.heavyClackSpeed * this.heavyClackSpeed)
				{
					this.heavyClackAudio.Play();
				}
				else if (sqrMagnitude > this.mediumClackSpeed * this.mediumClackSpeed)
				{
					this.mediumClackAudio.Play();
				}
				else if (sqrMagnitude > this.minimumClackSpeed * this.minimumClackSpeed)
				{
					this.lightClackAudio.Play();
				}
			}
			Vector3 vector = (this.arm1.lastWorldPosition + this.arm2.lastWorldPosition) / 2f;
			Vector3 vector2 = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * (this.collisionDistance + 0.001f) / 2f;
			Vector3 vector3 = vector + vector2;
			Vector3 vector4 = vector - vector2;
			if ((lastWorldPosition - vector3).IsLongerThan(lastWorldPosition - vector4))
			{
				vector2 = -vector2;
			}
			this.arm1.SetPosition(vector + vector2);
			this.arm2.SetPosition(vector - vector2);
			ref Vector3 ptr = ref this.arm1.velocity;
			Vector3 velocity = this.arm2.velocity;
			Vector3 velocity2 = this.arm1.velocity;
			ptr = velocity;
			this.arm2.velocity = velocity2;
			Vector3 vector5 = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * this.pushApartStrength * Mathf.Sqrt(sqrMagnitude);
			this.arm1.velocity = this.arm1.velocity + vector5;
			this.arm2.velocity = this.arm2.velocity - vector5;
		}
	}

	// Token: 0x04001590 RID: 5520
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x04001591 RID: 5521
	[SerializeField]
	private Transform clackerArm1;

	// Token: 0x04001592 RID: 5522
	[SerializeField]
	private Transform clackerArm2;

	// Token: 0x04001593 RID: 5523
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	// Token: 0x04001594 RID: 5524
	[SerializeField]
	private Vector3 LocalRotationAxis;

	// Token: 0x04001595 RID: 5525
	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	// Token: 0x04001596 RID: 5526
	[SerializeField]
	private float drag;

	// Token: 0x04001597 RID: 5527
	[SerializeField]
	private float gravity;

	// Token: 0x04001598 RID: 5528
	[SerializeField]
	private float localFriction;

	// Token: 0x04001599 RID: 5529
	[SerializeField]
	private float minimumClackSpeed;

	// Token: 0x0400159A RID: 5530
	[SerializeField]
	private SoundBankPlayer lightClackAudio;

	// Token: 0x0400159B RID: 5531
	[SerializeField]
	private float mediumClackSpeed;

	// Token: 0x0400159C RID: 5532
	[SerializeField]
	private SoundBankPlayer mediumClackAudio;

	// Token: 0x0400159D RID: 5533
	[SerializeField]
	private float heavyClackSpeed;

	// Token: 0x0400159E RID: 5534
	[SerializeField]
	private SoundBankPlayer heavyClackAudio;

	// Token: 0x0400159F RID: 5535
	[SerializeField]
	private float collisionDistance;

	// Token: 0x040015A0 RID: 5536
	private float centerOfMassRadius;

	// Token: 0x040015A1 RID: 5537
	[SerializeField]
	private float pushApartStrength;

	// Token: 0x040015A2 RID: 5538
	private ClackerCosmetic.PerArmData arm1;

	// Token: 0x040015A3 RID: 5539
	private ClackerCosmetic.PerArmData arm2;

	// Token: 0x040015A4 RID: 5540
	private Quaternion RotationCorrection;

	// Token: 0x020002B6 RID: 694
	private struct PerArmData
	{
		// Token: 0x060011FE RID: 4606 RVA: 0x00060904 File Offset: 0x0005EB04
		public void UpdateArm()
		{
			Vector3 vector = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
			Vector3 vector2 = this.lastWorldPosition + this.velocity * Time.deltaTime * this.parent.drag;
			Vector3 vector3 = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			Vector3 vector4 = this.transform.position + (vector2 - this.transform.position).ProjectOntoPlane(vector3).normalized * this.parent.centerOfMassRadius;
			vector4 = Vector3.MoveTowards(vector4, vector, this.parent.localFriction * Time.deltaTime);
			this.velocity = (vector4 - this.lastWorldPosition) / Time.deltaTime;
			this.velocity += Vector3.down * this.parent.gravity * Time.deltaTime;
			this.lastWorldPosition = vector4;
			this.transform.rotation = Quaternion.LookRotation(vector3, vector4 - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x00060A6C File Offset: 0x0005EC6C
		public void SetPosition(Vector3 newPosition)
		{
			Vector3 vector = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			this.transform.rotation = Quaternion.LookRotation(vector, newPosition - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		// Token: 0x040015A5 RID: 5541
		public ClackerCosmetic parent;

		// Token: 0x040015A6 RID: 5542
		public Transform transform;

		// Token: 0x040015A7 RID: 5543
		public Vector3 velocity;

		// Token: 0x040015A8 RID: 5544
		public Vector3 lastWorldPosition;
	}
}
