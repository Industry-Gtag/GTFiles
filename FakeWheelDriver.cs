using System;
using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020002C9 RID: 713
public class FakeWheelDriver : MonoBehaviour
{
	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06001266 RID: 4710 RVA: 0x000628F4 File Offset: 0x00060AF4
	// (set) Token: 0x06001267 RID: 4711 RVA: 0x000628FC File Offset: 0x00060AFC
	public bool hasCollision { get; private set; }

	// Token: 0x06001268 RID: 4712 RVA: 0x00062905 File Offset: 0x00060B05
	public void SetThrust(Vector3 thrust)
	{
		this.thrust = thrust;
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x00062910 File Offset: 0x00060B10
	private void OnCollisionStay(Collision collision)
	{
		int num = 0;
		Vector3 vector = Vector3.zero;
		foreach (ContactPoint contactPoint in collision.contacts)
		{
			if (contactPoint.thisCollider == this.wheelCollider)
			{
				vector += contactPoint.point;
				num++;
			}
		}
		if (num > 0)
		{
			this.collisionNormal = collision.contacts[0].normal;
			this.collisionPoint = vector / (float)num;
			this.hasCollision = true;
		}
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x0006299C File Offset: 0x00060B9C
	private void FixedUpdate()
	{
		if (this.hasCollision)
		{
			Vector3 vector = base.transform.rotation * this.thrust;
			if (this.myRigidBody.linearVelocity.IsShorterThan(this.maxSpeed))
			{
				vector = vector.ProjectOntoPlane(this.collisionNormal).normalized * this.thrust.magnitude;
				this.myRigidBody.AddForceAtPosition(vector, this.collisionPoint);
			}
			Vector3 vector2 = this.myRigidBody.linearVelocity.ProjectOntoPlane(this.collisionNormal).ProjectOntoPlane(vector.normalized);
			if (vector2.IsLongerThan(this.lateralFrictionForce))
			{
				this.myRigidBody.AddForceAtPosition(-vector2.normalized * this.lateralFrictionForce, this.collisionPoint);
			}
			else
			{
				this.myRigidBody.AddForceAtPosition(-vector2, this.collisionPoint);
			}
		}
		this.hasCollision = false;
	}

	// Token: 0x0400164B RID: 5707
	[SerializeField]
	private Rigidbody myRigidBody;

	// Token: 0x0400164C RID: 5708
	[SerializeField]
	private Vector3 thrust;

	// Token: 0x0400164D RID: 5709
	[SerializeField]
	private Collider wheelCollider;

	// Token: 0x0400164E RID: 5710
	[SerializeField]
	private float maxSpeed;

	// Token: 0x0400164F RID: 5711
	[SerializeField]
	private float lateralFrictionForce;

	// Token: 0x04001651 RID: 5713
	private Vector3 collisionPoint;

	// Token: 0x04001652 RID: 5714
	private Vector3 collisionNormal;
}
