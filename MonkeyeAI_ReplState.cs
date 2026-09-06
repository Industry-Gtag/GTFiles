using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001A8 RID: 424
[NetworkBehaviourWeaved(42)]
public class MonkeyeAI_ReplState : NetworkComponent
{
	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000B7F RID: 2943 RVA: 0x0003E30A File Offset: 0x0003C50A
	// (set) Token: 0x06000B80 RID: 2944 RVA: 0x0003E334 File Offset: 0x0003C534
	[Networked]
	[NetworkedWeaved(0, 42)]
	private unsafe MonkeyeAI_ReplState.MonkeyeAI_RepStateData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MonkeyeAI_ReplState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(MonkeyeAI_ReplState.MonkeyeAI_RepStateData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MonkeyeAI_ReplState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(MonkeyeAI_ReplState.MonkeyeAI_RepStateData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x0003E360 File Offset: 0x0003C560
	public override void WriteDataFusion()
	{
		MonkeyeAI_ReplState.MonkeyeAI_RepStateData monkeyeAI_RepStateData = new MonkeyeAI_ReplState.MonkeyeAI_RepStateData(this.userId, this.attackPos, this.timer, this.floorEnabled, this.portalEnabled, this.freezePlayer, this.alpha, this.state);
		this.Data = monkeyeAI_RepStateData;
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x0003E3AC File Offset: 0x0003C5AC
	public override void ReadDataFusion()
	{
		this.userId = this.Data.UserId.Value;
		this.attackPos = this.Data.AttackPos;
		this.timer = this.Data.Timer;
		this.floorEnabled = this.Data.FloorEnabled;
		this.portalEnabled = this.Data.PortalEnabled;
		this.freezePlayer = this.Data.FreezePlayer;
		this.alpha = this.Data.Alpha;
		this.state = this.Data.State;
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x0003E470 File Offset: 0x0003C670
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.userId);
		stream.SendNext(this.attackPos);
		stream.SendNext(this.timer);
		stream.SendNext(this.floorEnabled);
		stream.SendNext(this.portalEnabled);
		stream.SendNext(this.freezePlayer);
		stream.SendNext(this.alpha);
		stream.SendNext(this.state);
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x0003E500 File Offset: 0x0003C700
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.photonView.Owner == null)
		{
			return;
		}
		if (info.Sender.ActorNumber != info.photonView.Owner.ActorNumber)
		{
			return;
		}
		this.userId = (string)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		(ref this.attackPos).SetValueSafe(in vector);
		this.timer = (float)stream.ReceiveNext();
		this.floorEnabled = (bool)stream.ReceiveNext();
		this.portalEnabled = (bool)stream.ReceiveNext();
		this.freezePlayer = (bool)stream.ReceiveNext();
		this.alpha = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
		this.state = (MonkeyeAI_ReplState.EStates)stream.ReceiveNext();
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x0003E5D8 File Offset: 0x0003C7D8
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x0003E5F0 File Offset: 0x0003C7F0
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04000DE7 RID: 3559
	public MonkeyeAI_ReplState.EStates state;

	// Token: 0x04000DE8 RID: 3560
	public string userId;

	// Token: 0x04000DE9 RID: 3561
	public Vector3 attackPos;

	// Token: 0x04000DEA RID: 3562
	public float timer;

	// Token: 0x04000DEB RID: 3563
	public bool floorEnabled;

	// Token: 0x04000DEC RID: 3564
	public bool portalEnabled;

	// Token: 0x04000DED RID: 3565
	public bool freezePlayer;

	// Token: 0x04000DEE RID: 3566
	public float alpha;

	// Token: 0x04000DEF RID: 3567
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 42)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private MonkeyeAI_ReplState.MonkeyeAI_RepStateData _Data;

	// Token: 0x020001A9 RID: 425
	public enum EStates
	{
		// Token: 0x04000DF1 RID: 3569
		Sleeping,
		// Token: 0x04000DF2 RID: 3570
		Patrolling,
		// Token: 0x04000DF3 RID: 3571
		Chasing,
		// Token: 0x04000DF4 RID: 3572
		ReturnToSleepPt,
		// Token: 0x04000DF5 RID: 3573
		GoToSleep,
		// Token: 0x04000DF6 RID: 3574
		BeginAttack,
		// Token: 0x04000DF7 RID: 3575
		OpenFloor,
		// Token: 0x04000DF8 RID: 3576
		DropPlayer,
		// Token: 0x04000DF9 RID: 3577
		CloseFloor
	}

	// Token: 0x020001AA RID: 426
	[NetworkStructWeaved(42)]
	[StructLayout(LayoutKind.Explicit, Size = 168)]
	public struct MonkeyeAI_RepStateData : INetworkStruct
	{
		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000B88 RID: 2952 RVA: 0x0003E604 File Offset: 0x0003C804
		// (set) Token: 0x06000B89 RID: 2953 RVA: 0x0003E616 File Offset: 0x0003C816
		[Networked]
		[NetworkedWeaved(0, 33)]
		public unsafe NetworkString<_32> UserId
		{
			readonly get
			{
				return *(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._UserId);
			}
			set
			{
				*(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._UserId) = value;
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000B8A RID: 2954 RVA: 0x0003E629 File Offset: 0x0003C829
		// (set) Token: 0x06000B8B RID: 2955 RVA: 0x0003E63B File Offset: 0x0003C83B
		[Networked]
		[NetworkedWeaved(33, 3)]
		public unsafe Vector3 AttackPos
		{
			readonly get
			{
				return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._AttackPos);
			}
			set
			{
				*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._AttackPos) = value;
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000B8C RID: 2956 RVA: 0x0003E64E File Offset: 0x0003C84E
		// (set) Token: 0x06000B8D RID: 2957 RVA: 0x0003E65C File Offset: 0x0003C85C
		[Networked]
		[NetworkedWeaved(36, 1)]
		public unsafe float Timer
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Timer);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Timer) = value;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000B8E RID: 2958 RVA: 0x0003E66B File Offset: 0x0003C86B
		// (set) Token: 0x06000B8F RID: 2959 RVA: 0x0003E673 File Offset: 0x0003C873
		public NetworkBool FloorEnabled { readonly get; set; }

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000B90 RID: 2960 RVA: 0x0003E67C File Offset: 0x0003C87C
		// (set) Token: 0x06000B91 RID: 2961 RVA: 0x0003E684 File Offset: 0x0003C884
		public NetworkBool PortalEnabled { readonly get; set; }

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000B92 RID: 2962 RVA: 0x0003E68D File Offset: 0x0003C88D
		// (set) Token: 0x06000B93 RID: 2963 RVA: 0x0003E695 File Offset: 0x0003C895
		public NetworkBool FreezePlayer { readonly get; set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000B94 RID: 2964 RVA: 0x0003E69E File Offset: 0x0003C89E
		// (set) Token: 0x06000B95 RID: 2965 RVA: 0x0003E6AC File Offset: 0x0003C8AC
		[Networked]
		[NetworkedWeaved(40, 1)]
		public unsafe float Alpha
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Alpha);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Alpha) = value;
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000B96 RID: 2966 RVA: 0x0003E6BB File Offset: 0x0003C8BB
		// (set) Token: 0x06000B97 RID: 2967 RVA: 0x0003E6C3 File Offset: 0x0003C8C3
		public MonkeyeAI_ReplState.EStates State { readonly get; set; }

		// Token: 0x06000B98 RID: 2968 RVA: 0x0003E6CC File Offset: 0x0003C8CC
		public MonkeyeAI_RepStateData(string id, Vector3 atPos, float timer, bool floorOn, bool portalOn, bool freezePlayer, float alpha, MonkeyeAI_ReplState.EStates state)
		{
			this.UserId = id;
			this.AttackPos = atPos;
			this.Timer = timer;
			this.FloorEnabled = floorOn;
			this.PortalEnabled = portalOn;
			this.FreezePlayer = freezePlayer;
			this.Alpha = alpha;
			this.State = state;
		}

		// Token: 0x04000DFA RID: 3578
		[FixedBufferProperty(typeof(NetworkString<_32>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@33 _UserId;

		// Token: 0x04000DFB RID: 3579
		[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(132)]
		private FixedStorage@3 _AttackPos;

		// Token: 0x04000DFC RID: 3580
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(144)]
		private FixedStorage@1 _Timer;

		// Token: 0x04000E00 RID: 3584
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(160)]
		private FixedStorage@1 _Alpha;
	}
}
