using System;
using Fusion;

// Token: 0x020005D1 RID: 1489
[NetworkBehaviourWeaved(22)]
public class TagGameModeData : FusionGameModeData
{
	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x06002577 RID: 9591 RVA: 0x000C7F60 File Offset: 0x000C6160
	// (set) Token: 0x06002578 RID: 9592 RVA: 0x000C7F6D File Offset: 0x000C616D
	public override object Data
	{
		get
		{
			return this.tagData;
		}
		set
		{
			this.tagData = (TagData)value;
		}
	}

	// Token: 0x170003F6 RID: 1014
	// (get) Token: 0x06002579 RID: 9593 RVA: 0x000C7F7B File Offset: 0x000C617B
	// (set) Token: 0x0600257A RID: 9594 RVA: 0x000C7FA5 File Offset: 0x000C61A5
	[Networked]
	[NetworkedWeaved(0, 22)]
	private unsafe TagData tagData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TagGameModeData.tagData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(TagData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TagGameModeData.tagData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(TagData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600257C RID: 9596 RVA: 0x000C7FD0 File Offset: 0x000C61D0
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.tagData = this._tagData;
	}

	// Token: 0x0600257D RID: 9597 RVA: 0x000C7FE8 File Offset: 0x000C61E8
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._tagData = this.tagData;
	}

	// Token: 0x040030EE RID: 12526
	[WeaverGenerated]
	[DefaultForProperty("tagData", 0, 22)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private TagData _tagData;
}
