using System;
using UnityEngine;

// Token: 0x020005A8 RID: 1448
public class KinematicWhenTargetInactive : MonoBehaviour
{
	// Token: 0x060024AC RID: 9388 RVA: 0x000C50D8 File Offset: 0x000C32D8
	private void LateUpdate()
	{
		if (!this.target.activeSelf)
		{
			foreach (Rigidbody rigidbody in this.rigidBodies)
			{
				if (!rigidbody.isKinematic)
				{
					rigidbody.isKinematic = true;
				}
			}
			return;
		}
		foreach (Rigidbody rigidbody2 in this.rigidBodies)
		{
			if (rigidbody2.isKinematic)
			{
				rigidbody2.isKinematic = false;
			}
		}
	}

	// Token: 0x0400302E RID: 12334
	public Rigidbody[] rigidBodies;

	// Token: 0x0400302F RID: 12335
	public GameObject target;
}
