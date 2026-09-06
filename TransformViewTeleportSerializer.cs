using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000CE2 RID: 3298
[NetworkBehaviourWeaved(1)]
public class TransformViewTeleportSerializer : NetworkComponent
{
	// Token: 0x06005191 RID: 20881 RVA: 0x001AFF9D File Offset: 0x001AE19D
	protected override void Start()
	{
		base.Start();
		this.transformView = base.GetComponent<GorillaNetworkTransform>();
	}

	// Token: 0x06005192 RID: 20882 RVA: 0x001AFFB1 File Offset: 0x001AE1B1
	public void SetWillTeleport()
	{
		this.willTeleport = true;
	}

	// Token: 0x170007BC RID: 1980
	// (get) Token: 0x06005193 RID: 20883 RVA: 0x001AFFBA File Offset: 0x001AE1BA
	// (set) Token: 0x06005194 RID: 20884 RVA: 0x001AFFE4 File Offset: 0x001AE1E4
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe NetworkBool Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TransformViewTeleportSerializer.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TransformViewTeleportSerializer.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06005195 RID: 20885 RVA: 0x001B000F File Offset: 0x001AE20F
	public override void WriteDataFusion()
	{
		this.Data = this.willTeleport;
		this.willTeleport = false;
	}

	// Token: 0x06005196 RID: 20886 RVA: 0x001B0029 File Offset: 0x001AE229
	public override void ReadDataFusion()
	{
		if (this.Data)
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x06005197 RID: 20887 RVA: 0x001B0043 File Offset: 0x001AE243
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		stream.SendNext(this.willTeleport);
		this.willTeleport = false;
	}

	// Token: 0x06005198 RID: 20888 RVA: 0x001B007E File Offset: 0x001AE27E
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		if ((bool)stream.ReceiveNext())
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x0600519A RID: 20890 RVA: 0x001B00B9 File Offset: 0x001AE2B9
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x0600519B RID: 20891 RVA: 0x001B00D1 File Offset: 0x001AE2D1
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040063A2 RID: 25506
	private bool willTeleport;

	// Token: 0x040063A3 RID: 25507
	private GorillaNetworkTransform transformView;

	// Token: 0x040063A4 RID: 25508
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkBool _Data;
}
