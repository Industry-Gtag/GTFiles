using System;
using UnityEngine;

// Token: 0x02000300 RID: 768
public class TasselPhysics : MonoBehaviour
{
	// Token: 0x06001399 RID: 5017 RVA: 0x0006772C File Offset: 0x0006592C
	private void Awake()
	{
		this.centerOfMassLength = this.localCenterOfMass.magnitude;
		if (this.LockXAxis)
		{
			this.rotCorrection = Quaternion.Inverse(Quaternion.LookRotation(Vector3.right, this.localCenterOfMass));
			return;
		}
		this.rotCorrection = Quaternion.Inverse(Quaternion.LookRotation(this.localCenterOfMass));
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x00067784 File Offset: 0x00065984
	private void Update()
	{
		float y = base.transform.lossyScale.y;
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * y * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 vector = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 vector2 = position + (vector - position).normalized * this.centerOfMassLength * y;
		this.velocity = (vector2 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = vector2;
		if (this.LockXAxis)
		{
			foreach (GameObject gameObject in this.tasselInstances)
			{
				gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.right, vector2 - position) * this.rotCorrection;
			}
			return;
		}
		foreach (GameObject gameObject2 in this.tasselInstances)
		{
			gameObject2.transform.rotation = Quaternion.LookRotation(vector2 - position, gameObject2.transform.position - position) * this.rotCorrection;
		}
	}

	// Token: 0x040017FF RID: 6143
	[SerializeField]
	private GameObject[] tasselInstances;

	// Token: 0x04001800 RID: 6144
	[SerializeField]
	private Vector3 localCenterOfMass;

	// Token: 0x04001801 RID: 6145
	[SerializeField]
	private float gravityStrength;

	// Token: 0x04001802 RID: 6146
	[SerializeField]
	private float drag;

	// Token: 0x04001803 RID: 6147
	[SerializeField]
	private bool LockXAxis;

	// Token: 0x04001804 RID: 6148
	private Vector3 lastCenterPos;

	// Token: 0x04001805 RID: 6149
	private Vector3 velocity;

	// Token: 0x04001806 RID: 6150
	private float centerOfMassLength;

	// Token: 0x04001807 RID: 6151
	private Quaternion rotCorrection;
}
