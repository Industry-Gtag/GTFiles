using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020002FD RID: 765
public class SpringyWobbler : MonoBehaviour
{
	// Token: 0x06001380 RID: 4992 RVA: 0x00067110 File Offset: 0x00065310
	private void Start()
	{
		int num = 1;
		Transform transform = base.transform;
		while (transform.childCount > 0)
		{
			transform = transform.GetChild(0);
			num++;
		}
		this.children = new Transform[num];
		transform = base.transform;
		this.children[0] = transform;
		int num2 = 1;
		while (transform.childCount > 0)
		{
			transform = transform.GetChild(0);
			this.children[num2] = transform;
			num2++;
		}
		this.lastEndpointWorldPos = this.children[this.children.Length - 1].transform.position;
	}

	// Token: 0x06001381 RID: 4993 RVA: 0x0006719C File Offset: 0x0006539C
	private void Update()
	{
		float x = base.transform.lossyScale.x;
		Vector3 vector = base.transform.TransformPoint(this.idealEndpointLocalPos);
		this.endpointVelocity += (vector - this.lastEndpointWorldPos) * this.stabilizingForce * x * Time.deltaTime;
		Vector3 vector2 = this.lastEndpointWorldPos + this.endpointVelocity * Time.deltaTime;
		float num = this.maxDisplacement * x;
		if ((vector2 - vector).IsLongerThan(num))
		{
			vector2 = vector + (vector2 - vector).normalized * num;
		}
		this.endpointVelocity = (vector2 - this.lastEndpointWorldPos) * (1f - this.drag) / Time.deltaTime;
		Vector3 vector3 = base.transform.TransformPoint(this.rotateToFaceLocalPos);
		Vector3 vector4 = base.transform.TransformDirection(Vector3.up);
		Vector3 position = base.transform.position;
		Vector3 vector5 = position + base.transform.TransformDirection(this.idealEndpointLocalPos) * this.startStiffness * x;
		Vector3 vector6 = vector2;
		Vector3 vector7 = vector6 + (vector3 - vector6).normalized * this.endStiffness * x;
		for (int i = 1; i < this.children.Length; i++)
		{
			float num2 = (float)i / (float)(this.children.Length - 1);
			Vector3 vector8 = BezierUtils.BezierSolve(num2, position, vector5, vector7, vector6);
			Vector3 vector9 = BezierUtils.BezierSolve(num2 + 0.1f, position, vector5, vector7, vector6);
			this.children[i].transform.position = vector8;
			this.children[i].transform.rotation = Quaternion.LookRotation(vector9 - vector8, vector4);
		}
		this.lastIdealEndpointWorldPos = vector;
		this.lastEndpointWorldPos = vector2;
	}

	// Token: 0x040017E0 RID: 6112
	[SerializeField]
	private float stabilizingForce;

	// Token: 0x040017E1 RID: 6113
	[SerializeField]
	private float drag;

	// Token: 0x040017E2 RID: 6114
	[SerializeField]
	private float maxDisplacement;

	// Token: 0x040017E3 RID: 6115
	private Transform[] children;

	// Token: 0x040017E4 RID: 6116
	[SerializeField]
	private Vector3 idealEndpointLocalPos;

	// Token: 0x040017E5 RID: 6117
	[SerializeField]
	private Vector3 rotateToFaceLocalPos;

	// Token: 0x040017E6 RID: 6118
	[SerializeField]
	private float startStiffness;

	// Token: 0x040017E7 RID: 6119
	[SerializeField]
	private float endStiffness;

	// Token: 0x040017E8 RID: 6120
	private Vector3 lastIdealEndpointWorldPos;

	// Token: 0x040017E9 RID: 6121
	private Vector3 lastEndpointWorldPos;

	// Token: 0x040017EA RID: 6122
	private Vector3 endpointVelocity;
}
