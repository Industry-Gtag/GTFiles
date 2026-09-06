using System;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x02001295 RID: 4757
	public static class CosmeticExclusionQuery
	{
		// Token: 0x060077BE RID: 30654 RVA: 0x0026CB4C File Offset: 0x0026AD4C
		public static bool IsRestricted(VRRig ownerRig = null, GameObject effectSource = null)
		{
			CosmeticExclusionSource cosmeticExclusionSource;
			return (ownerRig != null && CosmeticExclusionZoneRegistry.IsRestricted(ownerRig)) || (effectSource != null && effectSource.TryGetComponent<CosmeticExclusionSource>(out cosmeticExclusionSource) && cosmeticExclusionSource.IsRestricted());
		}
	}
}
