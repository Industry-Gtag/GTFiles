using System;
using Fusion;
using Photon.Pun;

// Token: 0x0200045A RID: 1114
[NetworkBehaviourWeaved(0)]
public class NetworkComponentCallbacks : NetworkComponent
{
	// Token: 0x06001ABD RID: 6845 RVA: 0x00094A80 File Offset: 0x00092C80
	public override void ReadDataFusion()
	{
		this.ReadData();
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x00094A8D File Offset: 0x00092C8D
	public override void WriteDataFusion()
	{
		this.WriteData();
	}

	// Token: 0x06001ABF RID: 6847 RVA: 0x00094A9A File Offset: 0x00092C9A
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.ReadPunData(stream, info);
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x00094AA9 File Offset: 0x00092CA9
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.WritePunData(stream, info);
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001AC3 RID: 6851 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002564 RID: 9572
	public Action ReadData;

	// Token: 0x04002565 RID: 9573
	public Action WriteData;

	// Token: 0x04002566 RID: 9574
	public Action<PhotonStream, PhotonMessageInfo> ReadPunData;

	// Token: 0x04002567 RID: 9575
	public Action<PhotonStream, PhotonMessageInfo> WritePunData;
}
