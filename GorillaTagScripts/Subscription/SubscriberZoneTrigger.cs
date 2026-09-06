using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000FF7 RID: 4087
	public class SubscriberZoneTrigger : MonoBehaviour
	{
		// Token: 0x060065A1 RID: 26017 RVA: 0x0020BAC5 File Offset: 0x00209CC5
		private void OnTriggerEnter(Collider other)
		{
			if (GTPlayer.Instance != null && other == GTPlayer.Instance.bodyCollider && this.parentZone != null)
			{
				this.parentZone.OnZoneEnter(this.isRestrictedZone);
			}
		}

		// Token: 0x060065A2 RID: 26018 RVA: 0x0020BB05 File Offset: 0x00209D05
		private void OnTriggerExit(Collider other)
		{
			if (GTPlayer.Instance != null && other == GTPlayer.Instance.bodyCollider && this.parentZone != null)
			{
				this.parentZone.OnZoneExit(this.isRestrictedZone);
			}
		}

		// Token: 0x040074A3 RID: 29859
		public SubscriberExclusiveZone parentZone;

		// Token: 0x040074A4 RID: 29860
		public bool isRestrictedZone;
	}
}
