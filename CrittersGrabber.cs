using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000063 RID: 99
public class CrittersGrabber : CrittersActor
{
	// Token: 0x060001ED RID: 493 RVA: 0x0000B72D File Offset: 0x0000992D
	public override void ProcessRemote()
	{
		if (this.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.UpdateAverageSpeed();
		}
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000B747 File Offset: 0x00009947
	public override bool ProcessLocal()
	{
		if (this.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.UpdateAverageSpeed();
		}
		return base.ProcessLocal();
	}

	// Token: 0x04000230 RID: 560
	public Transform grabPosition;

	// Token: 0x04000231 RID: 561
	public bool grabbing;

	// Token: 0x04000232 RID: 562
	public float grabDistance;

	// Token: 0x04000233 RID: 563
	public List<CrittersActor> grabbedActors = new List<CrittersActor>();

	// Token: 0x04000234 RID: 564
	public bool isLeft;
}
