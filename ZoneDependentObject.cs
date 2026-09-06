using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class ZoneDependentObject : MonoBehaviour
{
	// Token: 0x06000D50 RID: 3408 RVA: 0x000490A2 File Offset: 0x000472A2
	private void Awake()
	{
		ZoneEntityBSP.onPlayerZoneChange += this.OnPlayerZoneChange;
		this.UpdateObjectState();
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x000490BB File Offset: 0x000472BB
	private void OnDestroy()
	{
		ZoneEntityBSP.onPlayerZoneChange -= this.OnPlayerZoneChange;
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x000490D0 File Offset: 0x000472D0
	private void OnPlayerZoneChange(VRRig rig, GTZone fromZone, GTZone toZone)
	{
		string text = "PlayerZoneChange: Player[{0}] {1}->{2}";
		NetPlayer creator = rig.Creator;
		Debug.Log(string.Format(text, (creator != null) ? new int?(creator.ActorNumber) : null, fromZone, toZone));
		this.UpdateObjectState();
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00049122 File Offset: 0x00047322
	private void UpdateObjectState()
	{
		if (this.zones.IsAnyPlayerInZones() != base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(this.zones.IsAnyPlayerInZones());
		}
	}

	// Token: 0x04000FEE RID: 4078
	public List<GTZone> zones = new List<GTZone> { GTZone.forest };
}
