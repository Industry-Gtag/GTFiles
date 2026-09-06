using System;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class NamedTriggerZone : MonoBehaviour
{
	// Token: 0x06000CD0 RID: 3280 RVA: 0x00046BDE File Offset: 0x00044DDE
	private void Reset()
	{
		this.ConfigureCollider();
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x00046BE8 File Offset: 0x00044DE8
	private void ConfigureCollider()
	{
		Collider collider = base.GetComponent<Collider>();
		if (!collider)
		{
			collider = base.gameObject.AddComponent<BoxCollider>();
		}
		collider.isTrigger = true;
		base.gameObject.layer = LayerMask.NameToLayer("Gorilla Trigger");
	}

	// Token: 0x04000F89 RID: 3977
	public string TriggerName = "Trigger";
}
