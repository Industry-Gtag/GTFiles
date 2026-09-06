using System;
using Fusion;

// Token: 0x020005CC RID: 1484
[NetworkBehaviourWeaved(1)]
public class CasualGameModeData : FusionGameModeData
{
	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x0600255F RID: 9567 RVA: 0x000C7DAB File Offset: 0x000C5FAB
	// (set) Token: 0x06002560 RID: 9568 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override object Data
	{
		get
		{
			return this.casualData;
		}
		set
		{
		}
	}

	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x06002561 RID: 9569 RVA: 0x000C7DB8 File Offset: 0x000C5FB8
	// (set) Token: 0x06002562 RID: 9570 RVA: 0x000C7DE2 File Offset: 0x000C5FE2
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe CasualData casualData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CasualGameModeData.casualData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(CasualData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CasualGameModeData.casualData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(CasualData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06002564 RID: 9572 RVA: 0x000C7E0D File Offset: 0x000C600D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.casualData = this._casualData;
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x000C7E25 File Offset: 0x000C6025
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._casualData = this.casualData;
	}

	// Token: 0x040030E3 RID: 12515
	[WeaverGenerated]
	[DefaultForProperty("casualData", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private CasualData _casualData;
}
