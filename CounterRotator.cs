using System;
using GorillaTag.Gravity;
using UnityEngine;

// Token: 0x02000312 RID: 786
public class CounterRotator : MonoBehaviour
{
	// Token: 0x060013E9 RID: 5097 RVA: 0x0006C50E File Offset: 0x0006A70E
	private void Start()
	{
		this.startingPosition = this.stabilizedObject.transform.position;
		this.startingRotation = this.stabilizedObject.transform.rotation;
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x0006C53C File Offset: 0x0006A73C
	private void LateUpdate()
	{
		Quaternion quaternion = this.startingRotation * Quaternion.Inverse(this.stabilizedObject.transform.rotation);
		base.transform.rotation = quaternion * base.transform.rotation;
		Vector3 vector = this.startingPosition - this.stabilizedObject.transform.position;
		base.transform.position += vector;
		if (this.gravityCompensator != null)
		{
			this.gravityCompensator.SetGravityDirection(-base.transform.up);
		}
	}

	// Token: 0x04001886 RID: 6278
	[SerializeField]
	private GameObject stabilizedObject;

	// Token: 0x04001887 RID: 6279
	[SerializeField]
	private ChangingBasicGravityZone gravityCompensator;

	// Token: 0x04001888 RID: 6280
	private Vector3 startingPosition;

	// Token: 0x04001889 RID: 6281
	private Quaternion startingRotation;
}
