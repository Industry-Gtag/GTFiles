using System;
using UnityEngine;

// Token: 0x0200055C RID: 1372
public class YorickLook : MonoBehaviour
{
	// Token: 0x060022FE RID: 8958 RVA: 0x000BC323 File Offset: 0x000BA523
	private void Awake()
	{
		this.overlapRigs = new VRRig[20];
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x000BC334 File Offset: 0x000BA534
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
		Vector3 vector2 = -base.transform.up;
		Vector3 vector3 = -base.transform.up;
		if (this.lookTarget != null)
		{
			vector2 = (this.lookTarget.position - this.leftEye.position).normalized;
			vector3 = (this.lookTarget.position - this.rightEye.position).normalized;
		}
		Vector3 vector4 = Vector3.RotateTowards(this.leftEye.rotation * Vector3.forward, vector2, this.rotSpeed * 3.1415927f, 0f);
		Vector3 vector5 = Vector3.RotateTowards(this.rightEye.rotation * Vector3.forward, vector3, this.rotSpeed * 3.1415927f, 0f);
		this.leftEye.rotation = Quaternion.LookRotation(vector4);
		this.rightEye.rotation = Quaternion.LookRotation(vector5);
	}

	// Token: 0x04002E15 RID: 11797
	public Transform leftEye;

	// Token: 0x04002E16 RID: 11798
	public Transform rightEye;

	// Token: 0x04002E17 RID: 11799
	public Transform lookTarget;

	// Token: 0x04002E18 RID: 11800
	public float lookRadius = 0.5f;

	// Token: 0x04002E19 RID: 11801
	public VRRig[] rigs = new VRRig[20];

	// Token: 0x04002E1A RID: 11802
	public VRRig[] overlapRigs;

	// Token: 0x04002E1B RID: 11803
	public float rotSpeed = 1f;

	// Token: 0x04002E1C RID: 11804
	public float lookAtAngleDegrees = 60f;
}
