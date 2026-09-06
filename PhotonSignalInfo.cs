using System;
using Photon.Pun;

// Token: 0x02000D01 RID: 3329
[Serializable]
public struct PhotonSignalInfo
{
	// Token: 0x06005254 RID: 21076 RVA: 0x001B41E0 File Offset: 0x001B23E0
	public PhotonSignalInfo(NetPlayer sender, int timestamp)
	{
		this.sender = sender;
		this.timestamp = timestamp;
	}

	// Token: 0x170007C7 RID: 1991
	// (get) Token: 0x06005255 RID: 21077 RVA: 0x001B41F0 File Offset: 0x001B23F0
	public double sentServerTime
	{
		get
		{
			return this.timestamp / 1000.0;
		}
	}

	// Token: 0x06005256 RID: 21078 RVA: 0x001B4204 File Offset: 0x001B2404
	public override string ToString()
	{
		return string.Format("[{0}: Sender = '{1}' sentTime = {2}]", "PhotonSignalInfo", this.sender.ActorNumber, this.sentServerTime);
	}

	// Token: 0x06005257 RID: 21079 RVA: 0x001B4230 File Offset: 0x001B2430
	public static implicit operator PhotonMessageInfo(PhotonSignalInfo psi)
	{
		return new PhotonMessageInfo(psi.sender.GetPlayerRef(), psi.timestamp, null);
	}

	// Token: 0x04006469 RID: 25705
	public readonly int timestamp;

	// Token: 0x0400646A RID: 25706
	public readonly NetPlayer sender;
}
