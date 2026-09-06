using System;
using System.Collections;
using GorillaExtensions;
using GorillaTag.Gravity;
using UnityEngine;

// Token: 0x02000CEA RID: 3306
public class AprilFoolsGravityFX : MonoBehaviour
{
	// Token: 0x060051DD RID: 20957 RVA: 0x001B3524 File Offset: 0x001B1724
	private void Start()
	{
		BasicGravityZone basicGravityZone = base.gameObject.AddComponent<PersonalGravityZone>();
		MonkeGravityController component = base.GetComponent<MonkeGravityController>();
		basicGravityZone.AddTarget(component);
		component.SetPersonalGravityDirection(Random.insideUnitCircle.x0y().WithY(-0.5f).normalized);
		base.StartCoroutine(this.BackToNormal());
	}

	// Token: 0x060051DE RID: 20958 RVA: 0x001B3578 File Offset: 0x001B1778
	private IEnumerator BackToNormal()
	{
		yield return new WaitForSeconds(180f);
		PersonalGravityZone component = base.GetComponent<PersonalGravityZone>();
		MonkeGravityController component2 = base.GetComponent<MonkeGravityController>();
		component.RemoveTarget(component2);
		yield break;
	}
}
