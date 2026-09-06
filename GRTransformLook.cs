using System;
using UnityEngine;

// Token: 0x0200082F RID: 2095
public class GRTransformLook : MonoBehaviour
{
	// Token: 0x060035D7 RID: 13783 RVA: 0x001297B8 File Offset: 0x001279B8
	private void Awake()
	{
		if (this.followPlayer)
		{
			this.lookTarget = Camera.main.transform;
		}
	}

	// Token: 0x060035D8 RID: 13784 RVA: 0x001297D2 File Offset: 0x001279D2
	private void LateUpdate()
	{
		base.transform.LookAt(this.lookTarget);
		base.transform.rotation *= Quaternion.Euler(this.offsetRotation);
	}

	// Token: 0x04004670 RID: 18032
	public bool followPlayer;

	// Token: 0x04004671 RID: 18033
	public Transform lookTarget;

	// Token: 0x04004672 RID: 18034
	public Vector3 offsetRotation;
}
