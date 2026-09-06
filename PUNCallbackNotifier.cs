using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000485 RID: 1157
public class PUNCallbackNotifier : MonoBehaviourPunCallbacks, IOnEventCallback
{
	// Token: 0x06001C30 RID: 7216 RVA: 0x000997EE File Offset: 0x000979EE
	private void Start()
	{
		this.parentSystem = base.GetComponent<NetworkSystemPUN>();
	}

	// Token: 0x06001C31 RID: 7217 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Update()
	{
	}

	// Token: 0x06001C32 RID: 7218 RVA: 0x000997FC File Offset: 0x000979FC
	public override void OnConnectedToMaster()
	{
		this.parentSystem.OnConnectedtoMaster();
	}

	// Token: 0x06001C33 RID: 7219 RVA: 0x00099809 File Offset: 0x00097A09
	public override void OnJoinedRoom()
	{
		this.parentSystem.OnJoinedRoom();
	}

	// Token: 0x06001C34 RID: 7220 RVA: 0x00099816 File Offset: 0x00097A16
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		this.parentSystem.OnJoinRoomFailed(returnCode, message);
	}

	// Token: 0x06001C35 RID: 7221 RVA: 0x00099816 File Offset: 0x00097A16
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		this.parentSystem.OnJoinRoomFailed(returnCode, message);
	}

	// Token: 0x06001C36 RID: 7222 RVA: 0x00099825 File Offset: 0x00097A25
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		this.parentSystem.OnCreateRoomFailed(returnCode, message);
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x00099834 File Offset: 0x00097A34
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		this.parentSystem.OnPlayerEnteredRoom(newPlayer);
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x00099842 File Offset: 0x00097A42
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.parentSystem.OnPlayerLeftRoom(otherPlayer);
	}

	// Token: 0x06001C39 RID: 7225 RVA: 0x00099850 File Offset: 0x00097A50
	public override void OnDisconnected(DisconnectCause cause)
	{
		this.parentSystem.OnDisconnected(cause);
	}

	// Token: 0x06001C3A RID: 7226 RVA: 0x0009985E File Offset: 0x00097A5E
	public void OnEvent(EventData photonEvent)
	{
		this.parentSystem.RaiseEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
	}

	// Token: 0x06001C3B RID: 7227 RVA: 0x0009987D File Offset: 0x00097A7D
	public override void OnPreLeavingRoom()
	{
		this.parentSystem.PreLeavingRoom();
	}

	// Token: 0x06001C3C RID: 7228 RVA: 0x0009988A File Offset: 0x00097A8A
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		this.parentSystem.OnMasterClientSwitched(newMasterClient);
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x00099898 File Offset: 0x00097A98
	public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
	{
		this.parentSystem.CustomAuthenticationResponse(data);
	}

	// Token: 0x06001C3E RID: 7230 RVA: 0x000998A6 File Offset: 0x00097AA6
	public override void OnCustomAuthenticationFailed(string debugMessage)
	{
		this.parentSystem.CustomAuthenticationFailed(debugMessage);
	}

	// Token: 0x04002668 RID: 9832
	private NetworkSystemPUN parentSystem;
}
