using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x02001294 RID: 4756
	public class CosmeticExclusionEventGate : MonoBehaviour
	{
		// Token: 0x060077BB RID: 30651 RVA: 0x0026CB05 File Offset: 0x0026AD05
		private void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
		}

		// Token: 0x060077BC RID: 30652 RVA: 0x0026CB13 File Offset: 0x0026AD13
		public void InvokeEvent()
		{
			if (CosmeticExclusionQuery.IsRestricted(this.ownerRig, this.effectSource))
			{
				UnityEvent unityEvent = this.onRestricted;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke();
				return;
			}
			else
			{
				UnityEvent unityEvent2 = this.onNormal;
				if (unityEvent2 == null)
				{
					return;
				}
				unityEvent2.Invoke();
				return;
			}
		}

		// Token: 0x04008801 RID: 34817
		[Header("Context")]
		[Tooltip("Optional effect source.\nIf set and has CosmeticExclusionSource, world position will be checked.")]
		[SerializeField]
		private GameObject effectSource;

		// Token: 0x04008802 RID: 34818
		[Header("Forwarded Events")]
		[SerializeField]
		private UnityEvent onNormal;

		// Token: 0x04008803 RID: 34819
		[SerializeField]
		private UnityEvent onRestricted;

		// Token: 0x04008804 RID: 34820
		private VRRig ownerRig;
	}
}
