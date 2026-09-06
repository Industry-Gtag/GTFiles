using System;
using UnityEngine;

// Token: 0x02000ADF RID: 2783
public class ZoneBasedObject : MonoBehaviour
{
	// Token: 0x06004769 RID: 18281 RVA: 0x00181350 File Offset: 0x0017F550
	public bool IsLocalPlayerInZone()
	{
		GTZone[] array = this.zones;
		for (int i = 0; i < array.Length; i++)
		{
			if (ZoneManagement.IsInZone(array[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600476A RID: 18282 RVA: 0x00181380 File Offset: 0x0017F580
	public static ZoneBasedObject SelectRandomEligible(ZoneBasedObject[] objects, string overrideChoice = "")
	{
		if (overrideChoice != "")
		{
			foreach (ZoneBasedObject zoneBasedObject in objects)
			{
				if (zoneBasedObject.gameObject.name == overrideChoice)
				{
					return zoneBasedObject;
				}
			}
		}
		ZoneBasedObject zoneBasedObject2 = null;
		int num = 0;
		foreach (ZoneBasedObject zoneBasedObject3 in objects)
		{
			if (zoneBasedObject3.gameObject.activeInHierarchy)
			{
				GTZone[] array = zoneBasedObject3.zones;
				for (int j = 0; j < array.Length; j++)
				{
					if (ZoneManagement.IsInZone(array[j]))
					{
						if (Random.Range(0, num) == 0)
						{
							zoneBasedObject2 = zoneBasedObject3;
						}
						num++;
						break;
					}
				}
			}
		}
		return zoneBasedObject2;
	}

	// Token: 0x040059F8 RID: 23032
	public GTZone[] zones;
}
