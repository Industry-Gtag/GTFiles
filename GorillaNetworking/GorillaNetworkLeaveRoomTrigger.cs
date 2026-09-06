using System;
using System.Threading.Tasks;
using GorillaTagScripts;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010F3 RID: 4339
	public class GorillaNetworkLeaveRoomTrigger : GorillaTriggerBox
	{
		// Token: 0x06006CE7 RID: 27879 RVA: 0x00233570 File Offset: 0x00231770
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (NetworkSystem.Instance.InRoom && (!this.excludePrivateRooms || !NetworkSystem.Instance.SessionIsPrivate))
			{
				if (FriendshipGroupDetection.Instance.IsInParty)
				{
					FriendshipGroupDetection.Instance.LeaveParty();
					this.DisconnectAfterDelay(1f);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
		}

		// Token: 0x06006CE8 RID: 27880 RVA: 0x002335D0 File Offset: 0x002317D0
		private async void DisconnectAfterDelay(float seconds)
		{
			await Task.Delay((int)(1000f * seconds));
			await NetworkSystem.Instance.ReturnToSinglePlayer();
		}

		// Token: 0x04007D34 RID: 32052
		[SerializeField]
		private bool excludePrivateRooms;
	}
}
