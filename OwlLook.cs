using System;
using UnityEngine;

// Token: 0x0200053E RID: 1342
public class OwlLook : MonoBehaviour
{
	// Token: 0x060021E7 RID: 8679 RVA: 0x000B4F5F File Offset: 0x000B315F
	private void Awake()
	{
		this.overlapRigs = new VRRig[20];
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
	}

	// Token: 0x060021E8 RID: 8680 RVA: 0x000B4F88 File Offset: 0x000B3188
	private void LateUpdate()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.rigs.Length != NetworkSystem.Instance.RoomPlayerCount)
			{
				this.rigs = VRRigCache.Instance.GetAllRigs();
			}
		}
		else if (this.rigs.Length != 1)
		{
			this.rigs = new VRRig[1];
			this.rigs[0] = VRRig.LocalRig;
		}
		float num = -1f;
		float num2 = Mathf.Cos(this.lookAtAngleDegrees / 180f * 3.1415927f);
		int num3 = 0;
		for (int i = 0; i < this.rigs.Length; i++)
		{
			if (!(this.rigs[i] == this.myRig))
			{
				Vector3 vector = this.rigs[i].tagSound.transform.position - base.transform.position;
				if (vector.magnitude <= this.lookRadius)
				{
					float num4 = Vector3.Dot(-base.transform.up, vector.normalized);
					if (num4 > num2)
					{
						this.overlapRigs[num3++] = this.rigs[i];
					}
				}
			}
		}
		this.lookTarget = null;
		for (int j = 0; j < num3; j++)
		{
			Vector3 vector = (this.overlapRigs[j].tagSound.transform.position - base.transform.position).normalized;
			float num4 = Vector3.Dot(base.transform.forward, vector);
			if (num4 > num)
			{
				num = num4;
				this.lookTarget = this.overlapRigs[j].tagSound.transform;
			}
		}
		Vector3 vector2 = this.neck.forward;
		if (this.lookTarget != null)
		{
			vector2 = (this.lookTarget.position - this.head.position).normalized;
		}
		Vector3 vector3 = this.neck.InverseTransformDirection(vector2);
		vector3.y = Mathf.Clamp(vector3.y, this.minNeckY, this.maxNeckY);
		vector2 = this.neck.TransformDirection(vector3.normalized);
		Vector3 vector4 = Vector3.RotateTowards(this.head.forward, vector2, this.rotSpeed * 0.017453292f * Time.deltaTime, 0f);
		this.head.rotation = Quaternion.LookRotation(vector4, this.neck.up);
	}

	// Token: 0x04002CC4 RID: 11460
	public Transform head;

	// Token: 0x04002CC5 RID: 11461
	public Transform lookTarget;

	// Token: 0x04002CC6 RID: 11462
	public Transform neck;

	// Token: 0x04002CC7 RID: 11463
	public float lookRadius = 0.5f;

	// Token: 0x04002CC8 RID: 11464
	public Collider[] overlapColliders;

	// Token: 0x04002CC9 RID: 11465
	public VRRig[] rigs = new VRRig[20];

	// Token: 0x04002CCA RID: 11466
	public VRRig[] overlapRigs;

	// Token: 0x04002CCB RID: 11467
	public float rotSpeed = 1f;

	// Token: 0x04002CCC RID: 11468
	public float lookAtAngleDegrees = 60f;

	// Token: 0x04002CCD RID: 11469
	public float maxNeckY;

	// Token: 0x04002CCE RID: 11470
	public float minNeckY;

	// Token: 0x04002CCF RID: 11471
	public VRRig myRig;
}
