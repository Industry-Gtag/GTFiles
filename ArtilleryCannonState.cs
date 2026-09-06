using System;
using System.Runtime.InteropServices;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200018E RID: 398
[RequireComponent(typeof(XSceneRefTarget))]
[NetworkBehaviourWeaved(8)]
public class ArtilleryCannonState : NetworkComponent
{
	// Token: 0x14000016 RID: 22
	// (add) Token: 0x06000AAF RID: 2735 RVA: 0x000396DC File Offset: 0x000378DC
	// (remove) Token: 0x06000AB0 RID: 2736 RVA: 0x00039714 File Offset: 0x00037914
	internal event Action onRotationChanged;

	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06000AB1 RID: 2737 RVA: 0x0003974C File Offset: 0x0003794C
	// (remove) Token: 0x06000AB2 RID: 2738 RVA: 0x00039784 File Offset: 0x00037984
	internal event Action onFired;

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000AB3 RID: 2739 RVA: 0x000397B9 File Offset: 0x000379B9
	internal float CurrentPitch
	{
		get
		{
			return this.currentPitch;
		}
	}

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000AB4 RID: 2740 RVA: 0x000397C1 File Offset: 0x000379C1
	internal float CurrentYaw
	{
		get
		{
			return this.currentYaw;
		}
	}

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000AB5 RID: 2741 RVA: 0x000397C9 File Offset: 0x000379C9
	internal float PitchMin
	{
		get
		{
			return this.pitchMin;
		}
	}

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000AB6 RID: 2742 RVA: 0x000397D1 File Offset: 0x000379D1
	internal float PitchMax
	{
		get
		{
			return this.pitchMax;
		}
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000AB7 RID: 2743 RVA: 0x000397D9 File Offset: 0x000379D9
	internal float DegreesPerCrankDegree
	{
		get
		{
			return this.degreesPerCrankDegree;
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000AB8 RID: 2744 RVA: 0x000391DD File Offset: 0x000373DD
	private int LocalActorNr
	{
		get
		{
			if (PhotonNetwork.LocalPlayer == null)
			{
				return -1;
			}
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x000397E1 File Offset: 0x000379E1
	protected override void Awake()
	{
		base.Awake();
		this.pitchCrankSync.holderActorNr = -1;
		this.yawCrankSync.holderActorNr = -1;
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00039804 File Offset: 0x00037A04
	internal void UpdateLocalCrankState(int crankIndex, bool isLeftHand, float angle)
	{
		ref ArtilleryCannonState.CrankSyncState ptr = (ref crankIndex == 0 ? ref this.pitchCrankSync : ref this.yawCrankSync);
		int localActorNr = this.LocalActorNr;
		if (ptr.holderActorNr == localActorNr)
		{
			ptr.isLeftHand = isLeftHand;
			ptr.angle = angle;
		}
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00039844 File Offset: 0x00037A44
	internal static VRRig FindRigForActor(int actorNr)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(actorNr, out rigContainer))
		{
			return rigContainer.Rig;
		}
		return null;
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00039868 File Offset: 0x00037A68
	internal unsafe bool NotifyCrankGrabbed(int crankIndex, bool isLeftHand)
	{
		ref ArtilleryCannonState.CrankSyncState ptr = (ref crankIndex == 0 ? ref this.pitchCrankSync : ref this.yawCrankSync);
		if (ptr.holderActorNr != -1)
		{
			return false;
		}
		ptr.holderActorNr = this.LocalActorNr;
		*(ref crankIndex == 0 ? ref this.pitchPendingGrabTime : ref this.yawPendingGrabTime) = Time.time;
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_ArtilleryMessage", RpcTarget.MasterClient, new object[]
			{
				isLeftHand ? 0 : 1,
				(byte)crankIndex,
				0f
			});
		}
		return true;
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x000398F8 File Offset: 0x00037AF8
	internal unsafe void NotifyCrankReleased(int crankIndex, float finalAngle)
	{
		ref ArtilleryCannonState.CrankSyncState ptr = (ref crankIndex == 0 ? ref this.pitchCrankSync : ref this.yawCrankSync);
		ptr.holderActorNr = -1;
		ptr.angle = finalAngle;
		*(ref crankIndex == 0 ? ref this.pitchPendingGrabTime : ref this.yawPendingGrabTime) = 0f;
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_ArtilleryMessage", RpcTarget.All, new object[]
			{
				2,
				(byte)crankIndex,
				finalAngle
			});
		}
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x00039970 File Offset: 0x00037B70
	internal void NotifyCrankInput(int crankIndex, float degrees)
	{
		if (crankIndex == 0)
		{
			this.currentPitch = Mathf.Clamp(this.currentPitch + degrees * this.degreesPerCrankDegree, this.pitchMin, this.pitchMax);
		}
		else
		{
			this.currentYaw += degrees * this.degreesPerCrankDegree;
		}
		Action action = this.onRotationChanged;
		if (action != null)
		{
			action();
		}
		if (PhotonNetwork.InRoom)
		{
			float num = ((crankIndex == 0) ? this.currentPitch : this.currentYaw);
			base.SendRPC("RPC_ArtilleryMessage", RpcTarget.MasterClient, new object[]
			{
				3,
				(byte)crankIndex,
				num
			});
		}
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x00039A18 File Offset: 0x00037C18
	internal bool TryFire()
	{
		if (Time.time < this.lastFireTime + this.fireCooldown)
		{
			return false;
		}
		this.lastFireTime = Time.time;
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_ArtilleryMessage", RpcTarget.Others, new object[] { 4, 0, 0f });
		}
		return true;
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00039A80 File Offset: 0x00037C80
	[PunRPC]
	public void RPC_ArtilleryMessage(byte msgType, byte crankIndex, float floatParam, PhotonMessageInfo info)
	{
		switch (msgType)
		{
		case 0:
		case 1:
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			ref ArtilleryCannonState.CrankSyncState ptr = (ref crankIndex == 0 ? ref this.pitchCrankSync : ref this.yawCrankSync);
			if (ptr.holderActorNr == -1)
			{
				ptr.holderActorNr = info.Sender.ActorNumber;
				ptr.isLeftHand = msgType == 0;
				return;
			}
			break;
		}
		case 2:
		{
			ref ArtilleryCannonState.CrankSyncState ptr2 = (ref crankIndex == 0 ? ref this.pitchCrankSync : ref this.yawCrankSync);
			if (ptr2.holderActorNr == info.Sender.ActorNumber)
			{
				ptr2.holderActorNr = -1;
				ptr2.angle = floatParam;
				return;
			}
			break;
		}
		case 3:
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if ((ref crankIndex == 0 ? ref this.pitchCrankSync : ref this.yawCrankSync).holderActorNr != info.Sender.ActorNumber)
			{
				return;
			}
			if (crankIndex == 0)
			{
				this.currentPitch = Mathf.Clamp(floatParam, this.pitchMin, this.pitchMax);
			}
			else
			{
				this.currentYaw = floatParam;
			}
			Action action = this.onRotationChanged;
			if (action == null)
			{
				return;
			}
			action();
			return;
		}
		case 4:
		{
			int actorNumber = info.Sender.ActorNumber;
			if (this.pitchCrankSync.holderActorNr != actorNumber && this.yawCrankSync.holderActorNr != actorNumber)
			{
				return;
			}
			Action action2 = this.onFired;
			if (action2 == null)
			{
				return;
			}
			action2();
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x00039BC4 File Offset: 0x00037DC4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.currentPitch);
		stream.SendNext(this.currentYaw);
		stream.SendNext(this.pitchCrankSync.holderActorNr);
		stream.SendNext(this.pitchCrankSync.isLeftHand);
		stream.SendNext(this.pitchCrankSync.angle);
		stream.SendNext(this.yawCrankSync.holderActorNr);
		stream.SendNext(this.yawCrankSync.isLeftHand);
		stream.SendNext(this.yawCrankSync.angle);
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x00039C78 File Offset: 0x00037E78
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		float num = (float)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		int localActorNr = this.LocalActorNr;
		this.ReadCrankSyncPUN(stream, ref this.pitchCrankSync, ref this.pitchPendingGrabTime, localActorNr);
		this.ReadCrankSyncPUN(stream, ref this.yawCrankSync, ref this.yawPendingGrabTime, localActorNr);
		if (this.pitchCrankSync.holderActorNr != localActorNr)
		{
			this.currentPitch = num;
		}
		if (this.yawCrankSync.holderActorNr != localActorNr)
		{
			this.currentYaw = num2;
		}
		Action action = this.onRotationChanged;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x00039D08 File Offset: 0x00037F08
	private void ReadCrankSyncPUN(PhotonStream stream, ref ArtilleryCannonState.CrankSyncState crank, ref float pendingTime, int localActor)
	{
		int num = (int)stream.ReceiveNext();
		bool flag = (bool)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		if (pendingTime > 0f && crank.holderActorNr == localActor)
		{
			if (num == localActor)
			{
				pendingTime = 0f;
			}
			else if (num != -1)
			{
				pendingTime = 0f;
			}
			else
			{
				if (Time.time - pendingTime <= 1f)
				{
					return;
				}
				pendingTime = 0f;
			}
		}
		crank.holderActorNr = num;
		crank.isLeftHand = flag;
		crank.angle = num2;
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000AC4 RID: 2756 RVA: 0x00039D95 File Offset: 0x00037F95
	// (set) Token: 0x06000AC5 RID: 2757 RVA: 0x00039DBF File Offset: 0x00037FBF
	[Networked]
	[NetworkedWeaved(0, 8)]
	private unsafe ArtilleryCannonState.FusionSyncState FusionData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ArtilleryCannonState.FusionData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(ArtilleryCannonState.FusionSyncState*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ArtilleryCannonState.FusionData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(ArtilleryCannonState.FusionSyncState*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00039DEC File Offset: 0x00037FEC
	public override void WriteDataFusion()
	{
		this.FusionData = new ArtilleryCannonState.FusionSyncState
		{
			pitch = this.currentPitch,
			yaw = this.currentYaw,
			pitchHolderActorNr = this.pitchCrankSync.holderActorNr,
			pitchIsLeftHand = this.pitchCrankSync.isLeftHand,
			pitchCrankAngle = this.pitchCrankSync.angle,
			yawHolderActorNr = this.yawCrankSync.holderActorNr,
			yawIsLeftHand = this.yawCrankSync.isLeftHand,
			yawCrankAngle = this.yawCrankSync.angle
		};
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00039E98 File Offset: 0x00038098
	public override void ReadDataFusion()
	{
		ArtilleryCannonState.FusionSyncState fusionData = this.FusionData;
		int localActorNr = this.LocalActorNr;
		this.ReadCrankSyncFusion(ref this.pitchCrankSync, ref this.pitchPendingGrabTime, localActorNr, fusionData.pitchHolderActorNr, fusionData.pitchIsLeftHand, fusionData.pitchCrankAngle);
		this.ReadCrankSyncFusion(ref this.yawCrankSync, ref this.yawPendingGrabTime, localActorNr, fusionData.yawHolderActorNr, fusionData.yawIsLeftHand, fusionData.yawCrankAngle);
		if (this.pitchCrankSync.holderActorNr != localActorNr)
		{
			this.currentPitch = fusionData.pitch;
		}
		if (this.yawCrankSync.holderActorNr != localActorNr)
		{
			this.currentYaw = fusionData.yaw;
		}
		Action action = this.onRotationChanged;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x00039F4C File Offset: 0x0003814C
	private void ReadCrankSyncFusion(ref ArtilleryCannonState.CrankSyncState crank, ref float pendingTime, int localActor, int incomingHolder, bool incomingLeftHand, float incomingAngle)
	{
		if (pendingTime > 0f && crank.holderActorNr == localActor)
		{
			if (incomingHolder == localActor)
			{
				pendingTime = 0f;
			}
			else if (incomingHolder != -1)
			{
				pendingTime = 0f;
			}
			else
			{
				if (Time.time - pendingTime <= 1f)
				{
					return;
				}
				pendingTime = 0f;
			}
		}
		crank.holderActorNr = incomingHolder;
		crank.isLeftHand = incomingLeftHand;
		crank.angle = incomingAngle;
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x00039FEC File Offset: 0x000381EC
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.FusionData = this._FusionData;
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x0003A004 File Offset: 0x00038204
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._FusionData = this.FusionData;
	}

	// Token: 0x04000CED RID: 3309
	internal const int CRANK_PITCH = 0;

	// Token: 0x04000CEE RID: 3310
	internal const int CRANK_YAW = 1;

	// Token: 0x04000CEF RID: 3311
	[Header("Rotation Limits")]
	[SerializeField]
	private float pitchMin = -10f;

	// Token: 0x04000CF0 RID: 3312
	[SerializeField]
	private float pitchMax = 60f;

	// Token: 0x04000CF1 RID: 3313
	[Tooltip("How many degrees the cannon rotates per degree of crank rotation")]
	[SerializeField]
	private float degreesPerCrankDegree = 0.5f;

	// Token: 0x04000CF2 RID: 3314
	[Header("Firing")]
	[SerializeField]
	private float fireCooldown = 2f;

	// Token: 0x04000CF3 RID: 3315
	private float currentPitch;

	// Token: 0x04000CF4 RID: 3316
	private float currentYaw;

	// Token: 0x04000CF5 RID: 3317
	private float lastFireTime;

	// Token: 0x04000CF6 RID: 3318
	internal ArtilleryCannonState.CrankSyncState pitchCrankSync;

	// Token: 0x04000CF7 RID: 3319
	internal ArtilleryCannonState.CrankSyncState yawCrankSync;

	// Token: 0x04000CF8 RID: 3320
	private const float GRAB_GRACE_PERIOD = 1f;

	// Token: 0x04000CF9 RID: 3321
	private float pitchPendingGrabTime;

	// Token: 0x04000CFA RID: 3322
	private float yawPendingGrabTime;

	// Token: 0x04000CFD RID: 3325
	[WeaverGenerated]
	[DefaultForProperty("FusionData", 0, 8)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private ArtilleryCannonState.FusionSyncState _FusionData;

	// Token: 0x0200018F RID: 399
	internal struct CrankSyncState
	{
		// Token: 0x04000CFE RID: 3326
		public int holderActorNr;

		// Token: 0x04000CFF RID: 3327
		public bool isLeftHand;

		// Token: 0x04000D00 RID: 3328
		public float angle;
	}

	// Token: 0x02000190 RID: 400
	private enum ArtilleryMsg : byte
	{
		// Token: 0x04000D02 RID: 3330
		CrankGrabLeft,
		// Token: 0x04000D03 RID: 3331
		CrankGrabRight,
		// Token: 0x04000D04 RID: 3332
		CrankRelease,
		// Token: 0x04000D05 RID: 3333
		CrankInput,
		// Token: 0x04000D06 RID: 3334
		Fire
	}

	// Token: 0x02000191 RID: 401
	[NetworkStructWeaved(8)]
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	private struct FusionSyncState : INetworkStruct
	{
		// Token: 0x04000D07 RID: 3335
		[FieldOffset(0)]
		public float pitch;

		// Token: 0x04000D08 RID: 3336
		[FieldOffset(4)]
		public float yaw;

		// Token: 0x04000D09 RID: 3337
		[FieldOffset(8)]
		public int pitchHolderActorNr;

		// Token: 0x04000D0A RID: 3338
		[FieldOffset(12)]
		public NetworkBool pitchIsLeftHand;

		// Token: 0x04000D0B RID: 3339
		[FieldOffset(16)]
		public float pitchCrankAngle;

		// Token: 0x04000D0C RID: 3340
		[FieldOffset(20)]
		public int yawHolderActorNr;

		// Token: 0x04000D0D RID: 3341
		[FieldOffset(24)]
		public NetworkBool yawIsLeftHand;

		// Token: 0x04000D0E RID: 3342
		[FieldOffset(28)]
		public float yawCrankAngle;
	}
}
