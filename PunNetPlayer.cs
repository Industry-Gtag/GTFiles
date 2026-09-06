using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000486 RID: 1158
[Serializable]
public class PunNetPlayer : NetPlayer
{
	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06001C40 RID: 7232 RVA: 0x000998B4 File Offset: 0x00097AB4
	// (set) Token: 0x06001C41 RID: 7233 RVA: 0x000998BC File Offset: 0x00097ABC
	public Player PlayerRef { get; private set; }

	// Token: 0x06001C43 RID: 7235 RVA: 0x000998CD File Offset: 0x00097ACD
	public void InitPlayer(Player playerRef)
	{
		this.PlayerRef = playerRef;
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06001C44 RID: 7236 RVA: 0x000998D6 File Offset: 0x00097AD6
	public override bool IsValid
	{
		get
		{
			return !this.PlayerRef.IsInactive;
		}
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06001C45 RID: 7237 RVA: 0x000998E6 File Offset: 0x00097AE6
	public override int ActorNumber
	{
		get
		{
			Player playerRef = this.PlayerRef;
			if (playerRef == null)
			{
				return -1;
			}
			return playerRef.ActorNumber;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06001C46 RID: 7238 RVA: 0x000998F9 File Offset: 0x00097AF9
	public override string UserId
	{
		get
		{
			return this.PlayerRef.UserId;
		}
	}

	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06001C47 RID: 7239 RVA: 0x00099906 File Offset: 0x00097B06
	public override bool IsMasterClient
	{
		get
		{
			return this.PlayerRef.IsMasterClient;
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06001C48 RID: 7240 RVA: 0x00099913 File Offset: 0x00097B13
	public override bool IsLocal
	{
		get
		{
			return this.PlayerRef == PhotonNetwork.LocalPlayer;
		}
	}

	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06001C49 RID: 7241 RVA: 0x00099922 File Offset: 0x00097B22
	public override bool IsNull
	{
		get
		{
			return this.PlayerRef == null;
		}
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06001C4A RID: 7242 RVA: 0x0009992D File Offset: 0x00097B2D
	public override string NickName
	{
		get
		{
			return this.PlayerRef.NickName;
		}
	}

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x06001C4B RID: 7243 RVA: 0x0009993A File Offset: 0x00097B3A
	public override string DefaultName
	{
		get
		{
			return this.PlayerRef.DefaultName;
		}
	}

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x06001C4C RID: 7244 RVA: 0x00099947 File Offset: 0x00097B47
	public override bool InRoom
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && currentRoom.Players.ContainsValue(this.PlayerRef);
		}
	}

	// Token: 0x06001C4D RID: 7245 RVA: 0x00099964 File Offset: 0x00097B64
	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((PunNetPlayer)myPlayer).PlayerRef.Equals(((PunNetPlayer)other).PlayerRef);
	}

	// Token: 0x06001C4E RID: 7246 RVA: 0x00099989 File Offset: 0x00097B89
	public override void OnReturned()
	{
		base.OnReturned();
	}

	// Token: 0x06001C4F RID: 7247 RVA: 0x00099991 File Offset: 0x00097B91
	public override void OnTaken()
	{
		base.OnTaken();
		this.PlayerRef = null;
	}
}
