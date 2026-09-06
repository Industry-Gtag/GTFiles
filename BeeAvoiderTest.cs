using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class BeeAvoiderTest : MonoBehaviour
{
	// Token: 0x06000E5C RID: 3676 RVA: 0x0004F498 File Offset: 0x0004D698
	public void Update()
	{
		Vector3 position = this.patrolPoints[this.nextPatrolPoint].transform.position;
		Vector3 position2 = base.transform.position;
		Vector3 vector = (position - position2).normalized * this.speed;
		this.velocity = Vector3.MoveTowards(this.velocity * this.drag, vector, this.acceleration);
		if ((position2 - position).IsLongerThan(this.instabilityOffRadius))
		{
			this.velocity += Random.insideUnitSphere * this.instability * Time.deltaTime;
		}
		Vector3 vector2 = position2 + this.velocity * Time.deltaTime;
		GameObject[] array = this.avoidancePoints;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 position3 = array[i].transform.position;
			if ((vector2 - position3).IsShorterThan(this.avoidRadius))
			{
				Vector3 normalized = Vector3.Cross(position3 - vector2, position - vector2).normalized;
				Vector3 normalized2 = (position - position3).normalized;
				float num = Vector3.Dot(vector2 - position3, normalized);
				Vector3 vector3 = (this.avoidRadius - num) * normalized;
				vector2 += vector3;
				this.velocity += vector3;
			}
		}
		base.transform.position = vector2;
		base.transform.rotation = Quaternion.LookRotation(position - vector2);
		if ((vector2 - position).IsShorterThan(this.patrolArrivedRadius))
		{
			this.nextPatrolPoint = (this.nextPatrolPoint + 1) % this.patrolPoints.Length;
		}
	}

	// Token: 0x0400115B RID: 4443
	public GameObject[] patrolPoints;

	// Token: 0x0400115C RID: 4444
	public GameObject[] avoidancePoints;

	// Token: 0x0400115D RID: 4445
	public float speed;

	// Token: 0x0400115E RID: 4446
	public float acceleration;

	// Token: 0x0400115F RID: 4447
	public float instability;

	// Token: 0x04001160 RID: 4448
	public float instabilityOffRadius;

	// Token: 0x04001161 RID: 4449
	public float drag;

	// Token: 0x04001162 RID: 4450
	public float avoidRadius;

	// Token: 0x04001163 RID: 4451
	public float patrolArrivedRadius;

	// Token: 0x04001164 RID: 4452
	private int nextPatrolPoint;

	// Token: 0x04001165 RID: 4453
	private Vector3 velocity;
}
