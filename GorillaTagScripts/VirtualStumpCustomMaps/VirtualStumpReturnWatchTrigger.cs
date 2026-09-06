using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FDE RID: 4062
	public class VirtualStumpReturnWatchTrigger : MonoBehaviour
	{
		// Token: 0x06006518 RID: 25880 RVA: 0x00208B1F File Offset: 0x00206D1F
		public void OnTriggerEnter(Collider other)
		{
			if (other == GTPlayer.Instance.headCollider)
			{
				VRRig.LocalRig.EnableVStumpReturnWatch(false);
			}
		}

		// Token: 0x06006519 RID: 25881 RVA: 0x00208B3E File Offset: 0x00206D3E
		public void OnTriggerExit(Collider other)
		{
			if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				VRRig.LocalRig.EnableVStumpReturnWatch(true);
			}
		}
	}
}
