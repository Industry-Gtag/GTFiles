using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000CEC RID: 3308
public class PersonalGravityZoneEvents : MonoBehaviour
{
	// Token: 0x060051E6 RID: 20966 RVA: 0x001B3601 File Offset: 0x001B1801
	public void SetLocalPlayerGravityDirection(Vector3 direction)
	{
		GTPlayerTransform.Instance.SetPersonalGravityDirection(direction);
	}

	// Token: 0x060051E7 RID: 20967 RVA: 0x001B360E File Offset: 0x001B180E
	public void SetLocalPlayerGravityDirection(Transform referenceDir)
	{
		GTPlayerTransform.Instance.SetPersonalGravityDirection(referenceDir);
	}
}
