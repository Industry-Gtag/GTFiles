using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000890 RID: 2192
public class GorillaJoinTeamBox : GorillaTriggerBox
{
	// Token: 0x0600393A RID: 14650 RVA: 0x00138290 File Offset: 0x00136490
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (GameObject.FindGameObjectWithTag("GorillaGameManager").GetComponent<GorillaGameManager>() != null)
		{
			bool inRoom = PhotonNetwork.InRoom;
		}
	}

	// Token: 0x0400494E RID: 18766
	public bool joinRedTeam;
}
