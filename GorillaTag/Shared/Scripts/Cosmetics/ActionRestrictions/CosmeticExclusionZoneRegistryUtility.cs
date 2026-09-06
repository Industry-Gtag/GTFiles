using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x02001299 RID: 4761
	public static class CosmeticExclusionZoneRegistryUtility
	{
		// Token: 0x060077CB RID: 30667 RVA: 0x0026CC75 File Offset: 0x0026AE75
		public static void RegisterZone(Collider zone)
		{
			if (zone != null && !CosmeticExclusionZoneRegistryUtility.exclusionZones.Contains(zone))
			{
				CosmeticExclusionZoneRegistryUtility.exclusionZones.Add(zone);
			}
		}

		// Token: 0x060077CC RID: 30668 RVA: 0x0026CC98 File Offset: 0x0026AE98
		public static void UnregisterZone(Collider zone)
		{
			CosmeticExclusionZoneRegistryUtility.exclusionZones.Remove(zone);
		}

		// Token: 0x060077CD RID: 30669 RVA: 0x0026CCA8 File Offset: 0x0026AEA8
		public static bool IsPositionRestricted(Vector3 worldPos)
		{
			for (int i = 0; i < CosmeticExclusionZoneRegistryUtility.exclusionZones.Count; i++)
			{
				Collider collider = CosmeticExclusionZoneRegistryUtility.exclusionZones[i];
				if (collider != null && collider.bounds.Contains(worldPos))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04008807 RID: 34823
		private static readonly List<Collider> exclusionZones = new List<Collider>();
	}
}
