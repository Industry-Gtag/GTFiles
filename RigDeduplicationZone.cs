using System;
using UnityEngine;

// Token: 0x020004DB RID: 1243
public class RigDeduplicationZone : RigDisplacementZone
{
	// Token: 0x06001E4C RID: 7756 RVA: 0x000A26C7 File Offset: 0x000A08C7
	public override bool IsDisplacingRig(VRRig rig)
	{
		return rig.portalShenanigansBit != VRRig.LocalRig.portalShenanigansBit;
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x000A26DE File Offset: 0x000A08DE
	public override Vector3 GetDisplacementForRig(VRRig rig, Vector3 undisplacedPosition)
	{
		if (!this.IsDisplacingRig(rig))
		{
			return Vector3.zero;
		}
		return -undisplacedPosition;
	}
}
