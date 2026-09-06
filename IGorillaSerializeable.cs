using System;
using Photon.Pun;

// Token: 0x0200084C RID: 2124
public interface IGorillaSerializeable
{
	// Token: 0x060036B7 RID: 14007
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x060036B8 RID: 14008
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
