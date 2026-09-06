using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012AF RID: 4783
	public class LoudSpeakerVolume : MonoBehaviour
	{
		// Token: 0x06007833 RID: 30771 RVA: 0x0026E830 File Offset: 0x0026CA30
		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("GorillaPlayer"))
			{
				VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
				if (component != null && component.creator != null)
				{
					if (component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId)
					{
						this._trigger.OnPlayerEnter(component);
						return;
					}
				}
				else
				{
					Debug.LogWarning("LoudSpeakerNetworkVolume :: OnTriggerEnter no colliding rig found!");
				}
			}
		}

		// Token: 0x06007834 RID: 30772 RVA: 0x0026E8A0 File Offset: 0x0026CAA0
		public void OnTriggerExit(Collider other)
		{
			VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				if (component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId)
				{
					this._trigger.OnPlayerExit(component);
					return;
				}
			}
			else
			{
				Debug.LogWarning("LoudSpeakerNetworkVolume :: OnTriggerExit no colliding rig found!");
			}
		}

		// Token: 0x0400885E RID: 34910
		[SerializeField]
		private LoudSpeakerTrigger _trigger;
	}
}
