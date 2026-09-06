using System;
using GorillaNetworking;
using Photon.Pun;

// Token: 0x020008E9 RID: 2281
public class GroupJoinButton : GorillaPressableButton
{
	// Token: 0x06003BB8 RID: 15288 RVA: 0x00147069 File Offset: 0x00145269
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.inPrivate)
		{
			GorillaComputer.instance.OnGroupJoinButtonPress(this.gameModeIndex, this.friendCollider);
		}
	}

	// Token: 0x06003BB9 RID: 15289 RVA: 0x00147091 File Offset: 0x00145291
	public void Update()
	{
		this.inPrivate = PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible;
		if (!this.inPrivate)
		{
			this.isOn = true;
		}
	}

	// Token: 0x04004C4A RID: 19530
	public int gameModeIndex;

	// Token: 0x04004C4B RID: 19531
	public GorillaFriendCollider friendCollider;

	// Token: 0x04004C4C RID: 19532
	public bool inPrivate;
}
