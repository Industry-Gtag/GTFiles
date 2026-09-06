using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020004D5 RID: 1237
[NetworkBehaviourWeaved(0)]
public class WorldShareableItem : NetworkComponent, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06001E10 RID: 7696 RVA: 0x000A1F58 File Offset: 0x000A0158
	// (set) Token: 0x06001E11 RID: 7697 RVA: 0x000A1F60 File Offset: 0x000A0160
	[DevInspectorShow]
	public TransferrableObject.PositionState transferableObjectState { get; set; }

	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06001E12 RID: 7698 RVA: 0x000A1F69 File Offset: 0x000A0169
	// (set) Token: 0x06001E13 RID: 7699 RVA: 0x000A1F71 File Offset: 0x000A0171
	public TransferrableObject.ItemStates transferableObjectItemState { get; set; }

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06001E14 RID: 7700 RVA: 0x000A1F7A File Offset: 0x000A017A
	// (set) Token: 0x06001E15 RID: 7701 RVA: 0x000A1F82 File Offset: 0x000A0182
	public TransferrableObject.PositionState transferableObjectStateNetworked { get; set; }

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06001E16 RID: 7702 RVA: 0x000A1F8B File Offset: 0x000A018B
	// (set) Token: 0x06001E17 RID: 7703 RVA: 0x000A1F93 File Offset: 0x000A0193
	public TransferrableObject.ItemStates transferableObjectItemStateNetworked { get; set; }

	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06001E18 RID: 7704 RVA: 0x000A1F9C File Offset: 0x000A019C
	// (set) Token: 0x06001E19 RID: 7705 RVA: 0x000A1FA4 File Offset: 0x000A01A4
	[DevInspectorShow]
	public WorldTargetItem target
	{
		get
		{
			return this._target;
		}
		set
		{
			this._target = value;
		}
	}

	// Token: 0x06001E1A RID: 7706 RVA: 0x000A1FAD File Offset: 0x000A01AD
	protected override void Awake()
	{
		base.Awake();
		this.guard = base.GetComponent<RequestableOwnershipGuard>();
		this.teleportSerializer = base.GetComponent<TransformViewTeleportSerializer>();
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x06001E1B RID: 7707 RVA: 0x000A1FDD File Offset: 0x000A01DD
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		if (GTAppState.isQuitting)
		{
			return;
		}
		base.OnEnable();
		this.guard.AddCallbackTarget(this);
		WorldShareableItemManager.Register(this);
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x06001E1C RID: 7708 RVA: 0x000A2018 File Offset: 0x000A0218
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		if (this.target == null || !this.target.transferrableObject.isSceneObject)
		{
			return;
		}
		PhotonView[] components = base.GetComponents<PhotonView>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ViewID = 0;
		}
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
		this.guard.RemoveCallbackTarget(this);
		this.rpcCallBack = null;
		this.onOwnerChangeCb = null;
		WorldShareableItemManager.Unregister(this);
	}

	// Token: 0x06001E1D RID: 7709 RVA: 0x000A2098 File Offset: 0x000A0298
	public void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		WorldShareableItemManager.Unregister(this);
	}

	// Token: 0x06001E1E RID: 7710 RVA: 0x000A20A8 File Offset: 0x000A02A8
	public void SetupSharableViewIDs(NetPlayer player, int slotID)
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		int num = player.ActorNumber * 1000 + 990 + slotID * 2;
		this.guard.giveCreatorAbsoluteAuthority = true;
		if (num != photonView.ViewID)
		{
			photonView.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2;
			photonView2.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2 + 1;
			this.guard.SetCreator(player);
		}
	}

	// Token: 0x06001E1F RID: 7711 RVA: 0x000A2134 File Offset: 0x000A0334
	public void ResetViews()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		photonView.ViewID = 0;
		photonView2.ViewID = 0;
	}

	// Token: 0x06001E20 RID: 7712 RVA: 0x000A2164 File Offset: 0x000A0364
	public void SetupSharableObject(int itemIDx, NetPlayer owner, Transform targetXform)
	{
		if (this.target != null)
		{
			Debug.LogError("ERROR!!!  WorldShareableItem.SetupSharableObject: target is expected to be null before this call. In scene path = \"" + base.transform.GetPathQ() + "\"", this);
			return;
		}
		this.target = WorldTargetItem.GenerateTargetFromPlayerAndID(owner, itemIDx);
		if (this.target.targetObject != targetXform)
		{
			Debug.LogError(string.Format("The target object found a transform that does not match the target transform, this should never happen. owner: {0} itemIDx: {1} targetXformPath: {2}, target.targetObject: {3}", new object[]
			{
				owner,
				itemIDx,
				targetXform.GetPath(),
				this.target.targetObject.GetPath()
			}));
		}
		TransferrableObject component = this.target.targetObject.GetComponent<TransferrableObject>();
		this.validShareable = component.canDrop || component.shareable || component.allowWorldSharableInstance;
		if (!this.validShareable)
		{
			Debug.LogError(string.Format("tried to setup an invalid shareable {0} {1} {2}", owner, itemIDx, targetXform.GetPath()));
			base.gameObject.SetActive(false);
			this.Invalidate();
			return;
		}
		this.guard.AddCallbackTarget(component);
		this.guard.giveCreatorAbsoluteAuthority = true;
		component.SetWorldShareableItem(this);
	}

	// Token: 0x06001E21 RID: 7713 RVA: 0x000A227E File Offset: 0x000A047E
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		base.OnPhotonInstantiate(info);
	}

	// Token: 0x06001E22 RID: 7714 RVA: 0x000A2288 File Offset: 0x000A0488
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (this.onOwnerChangeCb != null)
		{
			NetPlayer player = NetworkSystem.Instance.GetPlayer(newOwner);
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(previousOwner);
			this.onOwnerChangeCb(player, player2);
		}
	}

	// Token: 0x1700032E RID: 814
	// (get) Token: 0x06001E23 RID: 7715 RVA: 0x000A22C2 File Offset: 0x000A04C2
	// (set) Token: 0x06001E24 RID: 7716 RVA: 0x000A22CA File Offset: 0x000A04CA
	[DevInspectorShow]
	public bool EnableRemoteSync
	{
		get
		{
			return this.enableRemoteSync;
		}
		set
		{
			this.enableRemoteSync = value;
		}
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x000A22D4 File Offset: 0x000A04D4
	public void TriggeredUpdate()
	{
		if (!this.IsTargetValid())
		{
			return;
		}
		if (this.guard.isTrulyMine)
		{
			Vector3 vector;
			Quaternion quaternion;
			this.target.targetObject.GetPositionAndRotation(out vector, out quaternion);
			base.transform.SetPositionAndRotation(vector, quaternion);
			return;
		}
		if (!base.IsMine && this.EnableRemoteSync)
		{
			Vector3 vector2;
			Quaternion quaternion2;
			base.transform.GetPositionAndRotation(out vector2, out quaternion2);
			this.target.targetObject.SetPositionAndRotation(vector2, quaternion2);
		}
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x000A234A File Offset: 0x000A054A
	public void SyncToSceneObject(TransferrableObject transferrableObject)
	{
		this.target = WorldTargetItem.GenerateTargetFromWorldSharableItem(null, -2, transferrableObject.transform);
		base.transform.parent = null;
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x000A236C File Offset: 0x000A056C
	public void SetupSceneObjectOnNetwork(NetPlayer owner)
	{
		this.guard.SetOwnership(owner, false, false);
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x000A237C File Offset: 0x000A057C
	public bool IsTargetValid()
	{
		return this.target != null;
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x000A2387 File Offset: 0x000A0587
	public void Invalidate()
	{
		this.target = null;
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001E2A RID: 7722 RVA: 0x000A23A0 File Offset: 0x000A05A0
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == null)
		{
			return;
		}
		WorldShareableItem.CachedData cachedData;
		if (this.cachedDatas.TryGetValue(toPlayer, out cachedData))
		{
			this.transferableObjectState = cachedData.cachedTransferableObjectState;
			this.transferableObjectItemState = cachedData.cachedTransferableObjectItemState;
			this.cachedDatas.Remove(toPlayer);
		}
	}

	// Token: 0x06001E2B RID: 7723 RVA: 0x000A23E6 File Offset: 0x000A05E6
	public override void WriteDataFusion()
	{
		this.transferableObjectItemStateNetworked = this.transferableObjectItemState;
		this.transferableObjectStateNetworked = this.transferableObjectState;
	}

	// Token: 0x06001E2C RID: 7724 RVA: 0x000A2400 File Offset: 0x000A0600
	public override void ReadDataFusion()
	{
		this.transferableObjectItemState = this.transferableObjectItemStateNetworked;
		this.transferableObjectState = this.transferableObjectStateNetworked;
	}

	// Token: 0x06001E2D RID: 7725 RVA: 0x000A241A File Offset: 0x000A061A
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.transferableObjectState);
		stream.SendNext(this.transferableObjectItemState);
	}

	// Token: 0x06001E2E RID: 7726 RVA: 0x000A2440 File Offset: 0x000A0640
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (player != this.guard.actualOwner)
		{
			Debug.Log("Blocking info from non owner");
			this.cachedDatas.AddOrUpdate(player, new WorldShareableItem.CachedData
			{
				cachedTransferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext(),
				cachedTransferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext()
			});
			return;
		}
		this.transferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext();
		this.transferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext();
	}

	// Token: 0x06001E2F RID: 7727 RVA: 0x000A24D2 File Offset: 0x000A06D2
	[PunRPC]
	internal void RPCWorldShareable(PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		MonkeAgent.IncrementRPCCall(info, "RPCWorldShareable");
		if (this.rpcCallBack == null)
		{
			return;
		}
		this.rpcCallBack();
	}

	// Token: 0x06001E30 RID: 7728 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return true;
	}

	// Token: 0x06001E31 RID: 7729 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x06001E32 RID: 7730 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return true;
	}

	// Token: 0x06001E33 RID: 7731 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06001E34 RID: 7732 RVA: 0x000A2504 File Offset: 0x000A0704
	public void SetWillTeleport()
	{
		this.teleportSerializer.SetWillTeleport();
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001E37 RID: 7735 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002877 RID: 10359
	private bool validShareable = true;

	// Token: 0x04002878 RID: 10360
	public RequestableOwnershipGuard guard;

	// Token: 0x04002879 RID: 10361
	private TransformViewTeleportSerializer teleportSerializer;

	// Token: 0x0400287A RID: 10362
	[DevInspectorShow]
	[CanBeNull]
	private WorldTargetItem _target;

	// Token: 0x0400287B RID: 10363
	public WorldShareableItem.OnOwnerChangeDelegate onOwnerChangeCb;

	// Token: 0x0400287C RID: 10364
	public Action rpcCallBack;

	// Token: 0x0400287D RID: 10365
	private bool enableRemoteSync = true;

	// Token: 0x0400287E RID: 10366
	public Dictionary<NetPlayer, WorldShareableItem.CachedData> cachedDatas = new Dictionary<NetPlayer, WorldShareableItem.CachedData>();

	// Token: 0x020004D6 RID: 1238
	// (Invoke) Token: 0x06001E39 RID: 7737
	public delegate void Delegate();

	// Token: 0x020004D7 RID: 1239
	// (Invoke) Token: 0x06001E3D RID: 7741
	public delegate void OnOwnerChangeDelegate(NetPlayer newOwner, NetPlayer prevOwner);

	// Token: 0x020004D8 RID: 1240
	public struct CachedData
	{
		// Token: 0x0400287F RID: 10367
		public TransferrableObject.PositionState cachedTransferableObjectState;

		// Token: 0x04002880 RID: 10368
		public TransferrableObject.ItemStates cachedTransferableObjectItemState;
	}
}
