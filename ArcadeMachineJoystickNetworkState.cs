using System;
using Fusion;
using Photon.Pun;

// Token: 0x02000011 RID: 17
[NetworkBehaviourWeaved(0)]
public class ArcadeMachineJoystickNetworkState : NetworkComponent
{
	// Token: 0x06000047 RID: 71 RVA: 0x00002E52 File Offset: 0x00001052
	private new void Awake()
	{
		this.joystick = base.GetComponent<ArcadeMachineJoystick>();
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00002E60 File Offset: 0x00001060
	public override void ReadDataFusion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00002E60 File Offset: 0x00001060
	public override void WriteDataFusion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04000030 RID: 48
	private ArcadeMachineJoystick joystick;
}
