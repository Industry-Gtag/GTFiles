using System;
using UnityEngine;

// Token: 0x020005DA RID: 1498
public class GorillaCameraTriggerIndex : MonoBehaviour
{
	// Token: 0x0600259E RID: 9630 RVA: 0x000C856E File Offset: 0x000C676E
	private void Start()
	{
		this.parentTrigger = base.GetComponentInParent<GorillaCameraSceneTrigger>();
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x000C857C File Offset: 0x000C677C
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.mostRecentSceneTrigger = this;
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x000C85A8 File Offset: 0x000C67A8
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x04003111 RID: 12561
	public int sceneTriggerIndex;

	// Token: 0x04003112 RID: 12562
	public GorillaCameraSceneTrigger parentTrigger;
}
