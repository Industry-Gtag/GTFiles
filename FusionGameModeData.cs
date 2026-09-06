using System;
using Fusion;

// Token: 0x020005CD RID: 1485
[NetworkBehaviourWeaved(0)]
public abstract class FusionGameModeData : NetworkBehaviour
{
	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x06002566 RID: 9574
	// (set) Token: 0x06002567 RID: 9575
	public abstract object Data { get; set; }

	// Token: 0x06002569 RID: 9577 RVA: 0x00002C2D File Offset: 0x00000E2D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x0600256A RID: 9578 RVA: 0x00002C2D File Offset: 0x00000E2D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x040030E4 RID: 12516
	protected INetworkStruct data;
}
