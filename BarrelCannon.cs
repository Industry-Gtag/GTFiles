using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001A0 RID: 416
[NetworkBehaviourWeaved(3)]
public class BarrelCannon : NetworkComponent
{
	// Token: 0x06000B30 RID: 2864 RVA: 0x0003BF21 File Offset: 0x0003A121
	private void Update()
	{
		if (base.IsMine)
		{
			this.AuthorityUpdate();
		}
		else
		{
			this.ClientUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x0003BF40 File Offset: 0x0003A140
	private void AuthorityUpdate()
	{
		float time = Time.time;
		this.syncedState.hasAuthorityPassenger = this.localPlayerInside;
		switch (this.syncedState.currentState)
		{
		default:
			if (this.localPlayerInside)
			{
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Loaded;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.Loaded:
			if (time - this.stateStartTime > this.cannonEntryDelayTime)
			{
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.MovingToFirePosition;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.MovingToFirePosition:
			if (this.moveToFiringPositionTime > Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = Mathf.Clamp01((time - this.stateStartTime) / this.moveToFiringPositionTime);
			}
			else
			{
				this.syncedState.firingPositionLerpValue = 1f;
			}
			if (this.syncedState.firingPositionLerpValue >= 1f - Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 1f;
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Firing;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.Firing:
			if (this.localPlayerInside && this.localPlayerRigidbody != null)
			{
				Vector3 vector = base.transform.position - GorillaTagger.Instance.headCollider.transform.position;
				this.localPlayerRigidbody.MovePosition(this.localPlayerRigidbody.position + vector);
			}
			if (time - this.stateStartTime > this.preFiringDelayTime)
			{
				base.transform.localPosition = this.firingPositionOffset;
				base.transform.localRotation = Quaternion.Euler(this.firingRotationOffset);
				this.FireBarrelCannonLocal(base.transform.position, base.transform.up);
				if (PhotonNetwork.InRoom && GorillaGameManager.instance != null)
				{
					base.SendRPC("FireBarrelCannonRPC", RpcTarget.Others, new object[]
					{
						base.transform.position,
						base.transform.up
					});
				}
				Collider[] array = this.colliders;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = false;
				}
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.PostFireCooldown;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.PostFireCooldown:
			if (time - this.stateStartTime > this.postFiringCooldownTime)
			{
				Collider[] array = this.colliders;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = true;
				}
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.ReturningToIdlePosition;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.ReturningToIdlePosition:
			if (this.returnToIdlePositionTime > Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 1f - Mathf.Clamp01((time - this.stateStartTime) / this.returnToIdlePositionTime);
			}
			else
			{
				this.syncedState.firingPositionLerpValue = 0f;
			}
			if (this.syncedState.firingPositionLerpValue <= Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 0f;
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Idle;
			}
			break;
		}
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x0003C244 File Offset: 0x0003A444
	private void ClientUpdate()
	{
		if (!this.syncedState.hasAuthorityPassenger && this.syncedState.currentState == BarrelCannon.BarrelCannonState.Idle && this.localPlayerInside)
		{
			base.RequestOwnership();
		}
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0003C270 File Offset: 0x0003A470
	private void SharedUpdate()
	{
		if (this.syncedState.firingPositionLerpValue != this.localFiringPositionLerpValue)
		{
			this.localFiringPositionLerpValue = this.syncedState.firingPositionLerpValue;
			base.transform.localPosition = Vector3.Lerp(Vector3.zero, this.firingPositionOffset, this.firePositionAnimationCurve.Evaluate(this.localFiringPositionLerpValue));
			base.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, this.firingRotationOffset, this.fireRotationAnimationCurve.Evaluate(this.localFiringPositionLerpValue)));
		}
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void FireBarrelCannonRPC(Vector3 cannonCenter, Vector3 firingDirection)
	{
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x0003C300 File Offset: 0x0003A500
	private void FireBarrelCannonLocal(Vector3 cannonCenter, Vector3 firingDirection)
	{
		if (this.audioSource != null)
		{
			this.audioSource.GTPlay();
		}
		if (this.localPlayerInside && this.localPlayerRigidbody != null)
		{
			Vector3 vector = cannonCenter - GorillaTagger.Instance.headCollider.transform.position;
			this.localPlayerRigidbody.position = this.localPlayerRigidbody.position + vector;
			this.localPlayerRigidbody.linearVelocity = firingDirection * this.firingSpeed;
		}
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0003C38C File Offset: 0x0003A58C
	private void OnTriggerEnter(Collider other)
	{
		Rigidbody rigidbody;
		if (this.LocalPlayerTriggerFilter(other, out rigidbody))
		{
			this.localPlayerInside = true;
			this.localPlayerRigidbody = rigidbody;
		}
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0003C3B4 File Offset: 0x0003A5B4
	private void OnTriggerExit(Collider other)
	{
		Rigidbody rigidbody;
		if (this.LocalPlayerTriggerFilter(other, out rigidbody))
		{
			this.localPlayerInside = false;
			this.localPlayerRigidbody = null;
		}
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x0003C3DA File Offset: 0x0003A5DA
	private bool LocalPlayerTriggerFilter(Collider other, out Rigidbody rb)
	{
		rb = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
		}
		return rb != null;
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0003C410 File Offset: 0x0003A610
	private bool IsLocalPlayerInCannon()
	{
		Vector3 vector;
		Vector3 vector2;
		this.GetCapsulePoints(this.triggerCollider, out vector, out vector2);
		Physics.OverlapCapsuleNonAlloc(vector, vector2, this.triggerCollider.radius, this.triggerOverlapResults);
		for (int i = 0; i < this.triggerOverlapResults.Length; i++)
		{
			Rigidbody rigidbody;
			if (this.LocalPlayerTriggerFilter(this.triggerOverlapResults[i], out rigidbody))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x0003C470 File Offset: 0x0003A670
	private void GetCapsulePoints(CapsuleCollider capsule, out Vector3 pointA, out Vector3 pointB)
	{
		float num = capsule.height * 0.5f - capsule.radius;
		pointA = capsule.transform.position + capsule.transform.up * num;
		pointB = capsule.transform.position - capsule.transform.up * num;
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000B3B RID: 2875 RVA: 0x0003C4DF File Offset: 0x0003A6DF
	// (set) Token: 0x06000B3C RID: 2876 RVA: 0x0003C509 File Offset: 0x0003A709
	[Networked]
	[NetworkedWeaved(0, 3)]
	private unsafe BarrelCannon.BarrelCannonSyncedStateData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BarrelCannon.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BarrelCannon.BarrelCannonSyncedStateData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BarrelCannon.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BarrelCannon.BarrelCannonSyncedStateData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x0003C534 File Offset: 0x0003A734
	public override void WriteDataFusion()
	{
		this.Data = this.syncedState;
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0003C548 File Offset: 0x0003A748
	public override void ReadDataFusion()
	{
		this.syncedState.currentState = this.Data.CurrentState;
		this.syncedState.hasAuthorityPassenger = this.Data.HasAuthorityPassenger;
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x0003C58C File Offset: 0x0003A78C
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.syncedState.currentState);
		stream.SendNext(this.syncedState.hasAuthorityPassenger);
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0003C5BA File Offset: 0x0003A7BA
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.syncedState.currentState = (BarrelCannon.BarrelCannonState)stream.ReceiveNext();
		this.syncedState.hasAuthorityPassenger = (bool)stream.ReceiveNext();
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0003C5E8 File Offset: 0x0003A7E8
	public override void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
		if (!this.localPlayerInside)
		{
			targetView.TransferOwnership(requestingPlayer);
		}
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x0003C6BD File Offset: 0x0003A8BD
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0003C6D5 File Offset: 0x0003A8D5
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04000D75 RID: 3445
	[SerializeField]
	private float firingSpeed = 10f;

	// Token: 0x04000D76 RID: 3446
	[Header("Cannon's Movement Before Firing")]
	[SerializeField]
	private Vector3 firingPositionOffset = Vector3.zero;

	// Token: 0x04000D77 RID: 3447
	[SerializeField]
	private Vector3 firingRotationOffset = Vector3.zero;

	// Token: 0x04000D78 RID: 3448
	[SerializeField]
	private AnimationCurve firePositionAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000D79 RID: 3449
	[SerializeField]
	private AnimationCurve fireRotationAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000D7A RID: 3450
	[Header("Cannon State Change Timing Parameters")]
	[SerializeField]
	private float moveToFiringPositionTime = 0.5f;

	// Token: 0x04000D7B RID: 3451
	[SerializeField]
	[Tooltip("The minimum time to wait after a gorilla enters the cannon before it starts moving into the firing position.")]
	private float cannonEntryDelayTime = 0.25f;

	// Token: 0x04000D7C RID: 3452
	[SerializeField]
	[Tooltip("The minimum time to wait after a gorilla enters the cannon before it starts moving into the firing position.")]
	private float preFiringDelayTime = 0.25f;

	// Token: 0x04000D7D RID: 3453
	[SerializeField]
	[Tooltip("The minimum time to wait after the cannon fires before it starts moving back to the idle position.")]
	private float postFiringCooldownTime = 0.25f;

	// Token: 0x04000D7E RID: 3454
	[SerializeField]
	private float returnToIdlePositionTime = 1f;

	// Token: 0x04000D7F RID: 3455
	[Header("Component References")]
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000D80 RID: 3456
	[SerializeField]
	private CapsuleCollider triggerCollider;

	// Token: 0x04000D81 RID: 3457
	[SerializeField]
	private Collider[] colliders;

	// Token: 0x04000D82 RID: 3458
	private BarrelCannon.BarrelCannonSyncedState syncedState = new BarrelCannon.BarrelCannonSyncedState();

	// Token: 0x04000D83 RID: 3459
	private Collider[] triggerOverlapResults = new Collider[16];

	// Token: 0x04000D84 RID: 3460
	private bool localPlayerInside;

	// Token: 0x04000D85 RID: 3461
	private Rigidbody localPlayerRigidbody;

	// Token: 0x04000D86 RID: 3462
	private float stateStartTime;

	// Token: 0x04000D87 RID: 3463
	private float localFiringPositionLerpValue;

	// Token: 0x04000D88 RID: 3464
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BarrelCannon.BarrelCannonSyncedStateData _Data;

	// Token: 0x020001A1 RID: 417
	private enum BarrelCannonState
	{
		// Token: 0x04000D8A RID: 3466
		Idle,
		// Token: 0x04000D8B RID: 3467
		Loaded,
		// Token: 0x04000D8C RID: 3468
		MovingToFirePosition,
		// Token: 0x04000D8D RID: 3469
		Firing,
		// Token: 0x04000D8E RID: 3470
		PostFireCooldown,
		// Token: 0x04000D8F RID: 3471
		ReturningToIdlePosition
	}

	// Token: 0x020001A2 RID: 418
	private class BarrelCannonSyncedState
	{
		// Token: 0x04000D90 RID: 3472
		public BarrelCannon.BarrelCannonState currentState;

		// Token: 0x04000D91 RID: 3473
		public bool hasAuthorityPassenger;

		// Token: 0x04000D92 RID: 3474
		public float firingPositionLerpValue;
	}

	// Token: 0x020001A3 RID: 419
	[NetworkStructWeaved(3)]
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	private struct BarrelCannonSyncedStateData : INetworkStruct
	{
		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000B46 RID: 2886 RVA: 0x0003C6E9 File Offset: 0x0003A8E9
		// (set) Token: 0x06000B47 RID: 2887 RVA: 0x0003C6FB File Offset: 0x0003A8FB
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe BarrelCannon.BarrelCannonState CurrentState
		{
			readonly get
			{
				return *(BarrelCannon.BarrelCannonState*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentState);
			}
			set
			{
				*(BarrelCannon.BarrelCannonState*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentState) = value;
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000B48 RID: 2888 RVA: 0x0003C70E File Offset: 0x0003A90E
		// (set) Token: 0x06000B49 RID: 2889 RVA: 0x0003C720 File Offset: 0x0003A920
		[Networked]
		[NetworkedWeaved(1, 1)]
		public unsafe NetworkBool HasAuthorityPassenger
		{
			readonly get
			{
				return *(NetworkBool*)Native.ReferenceToPointer<FixedStorage@1>(ref this._HasAuthorityPassenger);
			}
			set
			{
				*(NetworkBool*)Native.ReferenceToPointer<FixedStorage@1>(ref this._HasAuthorityPassenger) = value;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000B4A RID: 2890 RVA: 0x0003C733 File Offset: 0x0003A933
		// (set) Token: 0x06000B4B RID: 2891 RVA: 0x0003C73B File Offset: 0x0003A93B
		public float FiringPositionLerpValue { readonly get; set; }

		// Token: 0x06000B4C RID: 2892 RVA: 0x0003C744 File Offset: 0x0003A944
		public BarrelCannonSyncedStateData(BarrelCannon.BarrelCannonState state, bool hasAuthPassenger, float firingPosLerpVal)
		{
			this.CurrentState = state;
			this.HasAuthorityPassenger = hasAuthPassenger;
			this.FiringPositionLerpValue = firingPosLerpVal;
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x0003C760 File Offset: 0x0003A960
		public static implicit operator BarrelCannon.BarrelCannonSyncedStateData(BarrelCannon.BarrelCannonSyncedState state)
		{
			return new BarrelCannon.BarrelCannonSyncedStateData(state.currentState, state.hasAuthorityPassenger, state.firingPositionLerpValue);
		}

		// Token: 0x04000D93 RID: 3475
		[FixedBufferProperty(typeof(BarrelCannon.BarrelCannonState), typeof(UnityValueSurrogate@ReaderWriter@BarrelCannon__BarrelCannonState), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@1 _CurrentState;

		// Token: 0x04000D94 RID: 3476
		[FixedBufferProperty(typeof(NetworkBool), typeof(UnityValueSurrogate@ElementReaderWriterNetworkBool), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@1 _HasAuthorityPassenger;
	}
}
