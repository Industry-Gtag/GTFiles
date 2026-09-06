using System;
using Fusion;
using Photon.Pun;

// Token: 0x0200084E RID: 2126
internal interface IWrappedSerializable : INetworkStruct
{
	// Token: 0x060036BC RID: 14012
	void OnSerializeRead(object newData);

	// Token: 0x060036BD RID: 14013
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x060036BE RID: 14014
	object OnSerializeWrite();

	// Token: 0x060036BF RID: 14015
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
