using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class StiltRBHandFollower : MonoBehaviour
{
	// Token: 0x06000A0E RID: 2574 RVA: 0x00036159 File Offset: 0x00034359
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.maxAngularVelocity = this.angularSpeedLimit;
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00036178 File Offset: 0x00034378
	private void FixedUpdate()
	{
		Vector3 vector = this.targetHand.TransformPoint(this.handOffset);
		float num;
		Vector3 vector2;
		(this.targetHand.TransformRotation(this.handRotOffset) * Quaternion.Inverse(this.rb.transform.rotation)).ToAngleAxis(out num, out vector2);
		this.rb.linearVelocity = (vector - this.rb.transform.position) / Time.fixedDeltaTime;
		this.rb.angularVelocity = vector2 * num * 0.017453292f / Time.fixedDeltaTime;
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x0003621F File Offset: 0x0003441F
	private void OnCollisionEnter(Collision collision)
	{
		this.collisions[collision.collider] = collision.contacts[0].point;
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x0003621F File Offset: 0x0003441F
	private void OnCollisionStay(Collision collision)
	{
		this.collisions[collision.collider] = collision.contacts[0].point;
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x00036243 File Offset: 0x00034443
	private void OnCollisionExit(Collision collision)
	{
		this.collisions.Remove(collision.collider);
	}

	// Token: 0x04000C5B RID: 3163
	private Rigidbody rb;

	// Token: 0x04000C5C RID: 3164
	[SerializeField]
	private Transform targetHand;

	// Token: 0x04000C5D RID: 3165
	[SerializeField]
	private Vector3 handOffset;

	// Token: 0x04000C5E RID: 3166
	[SerializeField]
	private Quaternion handRotOffset = Quaternion.identity;

	// Token: 0x04000C5F RID: 3167
	[SerializeField]
	private float angularSpeedLimit;

	// Token: 0x04000C60 RID: 3168
	private Dictionary<Collider, Vector3> collisions = new Dictionary<Collider, Vector3>();
}
