using System;
using System.Collections.Generic;

// Token: 0x020003BF RID: 959
public static class ZoneExtensions
{
	// Token: 0x0600171F RID: 5919 RVA: 0x0008643C File Offset: 0x0008463C
	public static bool IsAnyPlayerInZone(this GTZone zone)
	{
		using (IEnumerator<VRRig> enumerator = VRRigCache.ActiveRigs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.zoneEntity.currentZone == zone)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x00086494 File Offset: 0x00084694
	public static bool IsAnyPlayerInZones(this IList<GTZone> zones)
	{
		if (zones == null)
		{
			return false;
		}
		foreach (VRRig vrrig in VRRigCache.ActiveRigs)
		{
			if (zones.Contains(vrrig.zoneEntity.currentZone))
			{
				return true;
			}
		}
		return false;
	}
}
