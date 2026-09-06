using System;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x0200129A RID: 4762
	public class ZoneStateEventBase
	{
		// Token: 0x060077CF RID: 30671 RVA: 0x0026CCFF File Offset: 0x0026AEFF
		protected bool IsRestricted(VRRig vrRig)
		{
			return CosmeticExclusionZoneRegistry.IsRestricted(vrRig);
		}
	}
}
