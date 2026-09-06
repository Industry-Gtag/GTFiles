using System;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x02001296 RID: 4758
	public class CosmeticExclusionSource : MonoBehaviour
	{
		// Token: 0x060077BF RID: 30655 RVA: 0x0026CB8A File Offset: 0x0026AD8A
		public bool IsRestricted()
		{
			return CosmeticExclusionZoneRegistryUtility.IsPositionRestricted(base.transform.position);
		}
	}
}
