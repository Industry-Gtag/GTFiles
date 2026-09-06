using System;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x02001297 RID: 4759
	[RequireComponent(typeof(Collider))]
	public class CosmeticExclusionZone : MonoBehaviour
	{
		// Token: 0x060077C1 RID: 30657 RVA: 0x0026CB9C File Offset: 0x0026AD9C
		private void Awake()
		{
			this.zoneCollider = base.GetComponent<Collider>();
			this.zoneCollider.isTrigger = true;
			CosmeticExclusionZoneRegistryUtility.RegisterZone(this.zoneCollider);
		}

		// Token: 0x060077C2 RID: 30658 RVA: 0x0026CBC1 File Offset: 0x0026ADC1
		private void OnDestroy()
		{
			CosmeticExclusionZoneRegistryUtility.UnregisterZone(this.zoneCollider);
		}

		// Token: 0x060077C3 RID: 30659 RVA: 0x0026CBD0 File Offset: 0x0026ADD0
		private void OnTriggerEnter(Collider other)
		{
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				CosmeticExclusionZoneRegistry.Enter(componentInParent);
			}
		}

		// Token: 0x060077C4 RID: 30660 RVA: 0x0026CBF4 File Offset: 0x0026ADF4
		private void OnTriggerExit(Collider other)
		{
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				CosmeticExclusionZoneRegistry.Exit(componentInParent);
			}
		}

		// Token: 0x04008805 RID: 34821
		private Collider zoneCollider;
	}
}
