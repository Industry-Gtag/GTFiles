using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E88 RID: 3720
public class ZoneBasedGameObjectActivator : MonoBehaviour
{
	// Token: 0x06005A5C RID: 23132 RVA: 0x001D567E File Offset: 0x001D387E
	private void OnEnable()
	{
		ZoneManagement.OnZoneChange += this.ZoneManagement_OnZoneChange;
	}

	// Token: 0x06005A5D RID: 23133 RVA: 0x001D5691 File Offset: 0x001D3891
	private void OnDisable()
	{
		ZoneManagement.OnZoneChange -= this.ZoneManagement_OnZoneChange;
	}

	// Token: 0x06005A5E RID: 23134 RVA: 0x001D56A4 File Offset: 0x001D38A4
	private void ZoneManagement_OnZoneChange(ZoneData[] zoneData)
	{
		HashSet<GTZone> hashSet = new HashSet<GTZone>(this.zones);
		bool flag = false;
		for (int i = 0; i < zoneData.Length; i++)
		{
			flag |= zoneData[i].active && hashSet.Contains(zoneData[i].zone);
		}
		for (int j = 0; j < this.gameObjects.Length; j++)
		{
			this.gameObjects[j].SetActive(flag);
		}
	}

	// Token: 0x04006B7D RID: 27517
	[SerializeField]
	private GTZone[] zones;

	// Token: 0x04006B7E RID: 27518
	[SerializeField]
	private GameObject[] gameObjects;
}
