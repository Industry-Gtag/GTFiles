using System;
using UnityEngine;

// Token: 0x020003B7 RID: 951
public class GorillaSetZoneTrigger : GorillaTriggerBox
{
	// Token: 0x060016F7 RID: 5879 RVA: 0x00085A5B File Offset: 0x00083C5B
	public override void OnBoxTriggered()
	{
		Debug.Log("Triggered set zone box on gameobject " + base.gameObject.name);
		ZoneManagement.SetActiveZones(this.zones);
	}

	// Token: 0x04002203 RID: 8707
	[SerializeField]
	private GTZone[] zones;
}
