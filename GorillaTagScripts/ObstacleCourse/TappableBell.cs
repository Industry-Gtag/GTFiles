using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02001012 RID: 4114
	public class TappableBell : Tappable
	{
		// Token: 0x140000AC RID: 172
		// (add) Token: 0x06006684 RID: 26244 RVA: 0x0020EFCC File Offset: 0x0020D1CC
		// (remove) Token: 0x06006685 RID: 26245 RVA: 0x0020F004 File Offset: 0x0020D204
		public event TappableBell.ObstacleCourseTriggerEvent OnTapped;

		// Token: 0x06006686 RID: 26246 RVA: 0x0020F03C File Offset: 0x0020D23C
		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
		{
			if (!PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				return;
			}
			if (!this.rpcCooldown.CheckCallTime(Time.time))
			{
				return;
			}
			this.winnerRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (this.winnerRig != null)
			{
				TappableBell.ObstacleCourseTriggerEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(this.winnerRig);
			}
		}

		// Token: 0x0400755D RID: 30045
		private VRRig winnerRig;

		// Token: 0x0400755F RID: 30047
		public CallLimiter rpcCooldown;

		// Token: 0x02001013 RID: 4115
		// (Invoke) Token: 0x06006689 RID: 26249
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
