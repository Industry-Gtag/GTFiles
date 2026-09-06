using System;
using Fusion;

// Token: 0x020005CF RID: 1487
[NetworkBehaviourWeaved(43)]
public class HuntGameModeData : FusionGameModeData
{
	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x0600256D RID: 9581 RVA: 0x000C7E8B File Offset: 0x000C608B
	// (set) Token: 0x0600256E RID: 9582 RVA: 0x000C7E98 File Offset: 0x000C6098
	public override object Data
	{
		get
		{
			return this.huntdata;
		}
		set
		{
			this.huntdata = (HuntData)value;
		}
	}

	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x0600256F RID: 9583 RVA: 0x000C7EA6 File Offset: 0x000C60A6
	// (set) Token: 0x06002570 RID: 9584 RVA: 0x000C7ED0 File Offset: 0x000C60D0
	[Networked]
	[NetworkedWeaved(0, 43)]
	private unsafe HuntData huntdata
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HuntGameModeData.huntdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(HuntData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HuntGameModeData.huntdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(HuntData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x000C7EFB File Offset: 0x000C60FB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.huntdata = this._huntdata;
	}

	// Token: 0x06002573 RID: 9587 RVA: 0x000C7F13 File Offset: 0x000C6113
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._huntdata = this.huntdata;
	}

	// Token: 0x040030EA RID: 12522
	[WeaverGenerated]
	[DefaultForProperty("huntdata", 0, 43)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private HuntData _huntdata;
}
