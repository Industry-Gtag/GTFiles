using System;
using Fusion;
using Photon.Pun;

// Token: 0x02000849 RID: 2121
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaSerializerMasterOnly : GorillaWrappedSerializer
{
	// Token: 0x0600368D RID: 13965 RVA: 0x0012D747 File Offset: 0x0012B947
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == PhotonNetwork.MasterClient;
	}

	// Token: 0x0600368F RID: 13967 RVA: 0x0012D761 File Offset: 0x0012B961
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06003690 RID: 13968 RVA: 0x0012D76D File Offset: 0x0012B96D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
