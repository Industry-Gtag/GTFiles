using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020002E8 RID: 744
public class LongScarfSim : MonoBehaviour
{
	// Token: 0x060012F2 RID: 4850 RVA: 0x00064DB0 File Offset: 0x00062FB0
	private void Start()
	{
		this.clampToPlane.Normalize();
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.baseLocalRotations = new Quaternion[this.gameObjects.Length];
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.baseLocalRotations[i] = this.gameObjects[i].transform.localRotation;
		}
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x00064E18 File Offset: 0x00063018
	private void LateUpdate()
	{
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 vector = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 vector2 = position + (vector - position).normalized * this.centerOfMassLength;
		Vector3 vector3 = base.transform.InverseTransformPoint(vector2);
		float num = Vector3.Dot(vector3, this.clampToPlane);
		if (num < 0f)
		{
			vector3 -= this.clampToPlane * num;
			vector2 = base.transform.TransformPoint(vector3);
		}
		Vector3 vector4 = vector2;
		this.velocity = (vector4 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = vector4;
		float num2 = (float)(this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold) ? 1 : 0);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, num2, this.blendAmountPerSecond * Time.deltaTime);
		Quaternion quaternion = Quaternion.LookRotation(vector4 - position);
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			Quaternion quaternion2 = this.gameObjects[i].transform.parent.rotation * this.baseLocalRotations[i];
			this.gameObjects[i].transform.rotation = Quaternion.Lerp(quaternion2, quaternion, this.currentBlend);
		}
	}

	// Token: 0x04001725 RID: 5925
	[SerializeField]
	private GameObject[] gameObjects;

	// Token: 0x04001726 RID: 5926
	[SerializeField]
	private float speedThreshold = 1f;

	// Token: 0x04001727 RID: 5927
	[SerializeField]
	private float blendAmountPerSecond = 1f;

	// Token: 0x04001728 RID: 5928
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001729 RID: 5929
	private Quaternion[] baseLocalRotations;

	// Token: 0x0400172A RID: 5930
	private float currentBlend;

	// Token: 0x0400172B RID: 5931
	[SerializeField]
	private float centerOfMassLength;

	// Token: 0x0400172C RID: 5932
	[SerializeField]
	private float gravityStrength;

	// Token: 0x0400172D RID: 5933
	[SerializeField]
	private float drag;

	// Token: 0x0400172E RID: 5934
	[SerializeField]
	private Vector3 clampToPlane;

	// Token: 0x0400172F RID: 5935
	private Vector3 lastCenterPos;

	// Token: 0x04001730 RID: 5936
	private Vector3 velocity;
}
