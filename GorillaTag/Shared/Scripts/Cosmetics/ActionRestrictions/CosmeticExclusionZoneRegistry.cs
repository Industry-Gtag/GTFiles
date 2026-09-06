using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x02001298 RID: 4760
	public static class CosmeticExclusionZoneRegistry
	{
		// Token: 0x060077C6 RID: 30662 RVA: 0x0026CC17 File Offset: 0x0026AE17
		public static void Enter(VRRig rig)
		{
			if (rig != null)
			{
				CosmeticExclusionZoneRegistry.restrictedRigs.Add(rig);
			}
		}

		// Token: 0x060077C7 RID: 30663 RVA: 0x0026CC2E File Offset: 0x0026AE2E
		public static void Exit(VRRig rig)
		{
			if (rig != null)
			{
				CosmeticExclusionZoneRegistry.restrictedRigs.Remove(rig);
			}
		}

		// Token: 0x060077C8 RID: 30664 RVA: 0x0026CC45 File Offset: 0x0026AE45
		public static bool IsRestricted(VRRig rig)
		{
			return rig != null && CosmeticExclusionZoneRegistry.restrictedRigs.Contains(rig);
		}

		// Token: 0x060077C9 RID: 30665 RVA: 0x0026CC5D File Offset: 0x0026AE5D
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Reset()
		{
			CosmeticExclusionZoneRegistry.restrictedRigs.Clear();
		}

		// Token: 0x04008806 RID: 34822
		private static readonly HashSet<VRRig> restrictedRigs = new HashSet<VRRig>();
	}
}
