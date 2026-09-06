using System;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class RotateXform : MonoBehaviour
{
	// Token: 0x06000BFE RID: 3070 RVA: 0x000419D8 File Offset: 0x0003FBD8
	private void Update()
	{
		if (!this.xform)
		{
			return;
		}
		Vector3 vector = ((this.mode == RotateXform.Mode.Local) ? this.xform.localEulerAngles : this.xform.eulerAngles);
		float num = Time.deltaTime * this.speedFactor;
		vector.x += this.speed.x * num;
		vector.y += this.speed.y * num;
		vector.z += this.speed.z * num;
		if (this.mode == RotateXform.Mode.Local)
		{
			this.xform.localEulerAngles = vector;
			return;
		}
		this.xform.eulerAngles = vector;
	}

	// Token: 0x04000E9F RID: 3743
	public Transform xform;

	// Token: 0x04000EA0 RID: 3744
	public Vector3 speed = Vector3.zero;

	// Token: 0x04000EA1 RID: 3745
	public RotateXform.Mode mode;

	// Token: 0x04000EA2 RID: 3746
	public float speedFactor = 0.0625f;

	// Token: 0x020001C2 RID: 450
	public enum Mode
	{
		// Token: 0x04000EA4 RID: 3748
		Local,
		// Token: 0x04000EA5 RID: 3749
		World
	}
}
