using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000E51 RID: 3665
[NetworkBehaviourWeaved(3)]
public class ThrowableBugReliableState : NetworkComponent, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x17000883 RID: 2179
	// (get) Token: 0x0600597C RID: 22908 RVA: 0x001D0F25 File Offset: 0x001CF125
	// (set) Token: 0x0600597D RID: 22909 RVA: 0x001D0F4F File Offset: 0x001CF14F
	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe ThrowableBugReliableState.BugData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ThrowableBugReliableState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(ThrowableBugReliableState.BugData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ThrowableBugReliableState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(ThrowableBugReliableState.BugData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600597E RID: 22910 RVA: 0x001D0F7A File Offset: 0x001CF17A
	public override void WriteDataFusion()
	{
		this.Data = new ThrowableBugReliableState.BugData(this.travelingDirection);
	}

	// Token: 0x0600597F RID: 22911 RVA: 0x001D0F90 File Offset: 0x001CF190
	public override void ReadDataFusion()
	{
		this.travelingDirection = this.Data.tDirection;
	}

	// Token: 0x06005980 RID: 22912 RVA: 0x001D0FB1 File Offset: 0x001CF1B1
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.travelingDirection);
	}

	// Token: 0x06005981 RID: 22913 RVA: 0x001D0FC4 File Offset: 0x001CF1C4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		Vector3 vector = (Vector3)stream.ReceiveNext();
		(ref this.travelingDirection).SetValueSafe(in vector);
	}

	// Token: 0x06005982 RID: 22914 RVA: 0x00002E60 File Offset: 0x00001060
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06005983 RID: 22915 RVA: 0x00002E60 File Offset: 0x00001060
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06005984 RID: 22916 RVA: 0x00002E60 File Offset: 0x00001060
	public void OnMyOwnerLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06005985 RID: 22917 RVA: 0x00002E60 File Offset: 0x00001060
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06005986 RID: 22918 RVA: 0x00002E60 File Offset: 0x00001060
	public void OnMyCreatorLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06005988 RID: 22920 RVA: 0x001D0FFD File Offset: 0x001CF1FD
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06005989 RID: 22921 RVA: 0x001D1015 File Offset: 0x001CF215
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040069D0 RID: 27088
	public Vector3 travelingDirection = Vector3.zero;

	// Token: 0x040069D1 RID: 27089
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private ThrowableBugReliableState.BugData _Data;

	// Token: 0x02000E52 RID: 3666
	[NetworkStructWeaved(3)]
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	public struct BugData : INetworkStruct
	{
		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x0600598A RID: 22922 RVA: 0x001D1029 File Offset: 0x001CF229
		// (set) Token: 0x0600598B RID: 22923 RVA: 0x001D103B File Offset: 0x001CF23B
		[Networked]
		[NetworkedWeaved(0, 3)]
		public unsafe Vector3 tDirection
		{
			readonly get
			{
				return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._tDirection);
			}
			set
			{
				*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._tDirection) = value;
			}
		}

		// Token: 0x0600598C RID: 22924 RVA: 0x001D104E File Offset: 0x001CF24E
		public BugData(Vector3 dir)
		{
			this.tDirection = dir;
		}

		// Token: 0x040069D2 RID: 27090
		[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@3 _tDirection;
	}
}
