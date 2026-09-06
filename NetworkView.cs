using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200046D RID: 1133
[RequireComponent(typeof(PhotonView), typeof(NetworkObject))]
[NetworkBehaviourWeaved(0)]
public class NetworkView : NetworkBehaviour, IStateAuthorityChanged, IPublicFacingInterface, IPunOwnershipCallbacks
{
	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06001B7C RID: 7036 RVA: 0x0009589C File Offset: 0x00093A9C
	public bool IsMine
	{
		get
		{
			return this.punView != null && this.punView.IsMine;
		}
	}

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x06001B7D RID: 7037 RVA: 0x000958B9 File Offset: 0x00093AB9
	public bool IsValid
	{
		get
		{
			return this.punView != null;
		}
	}

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06001B7E RID: 7038 RVA: 0x000958B9 File Offset: 0x00093AB9
	public bool HasView
	{
		get
		{
			return this.punView != null;
		}
	}

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06001B7F RID: 7039 RVA: 0x000958C7 File Offset: 0x00093AC7
	public bool IsRoomView
	{
		get
		{
			return this.punView.IsRoomView;
		}
	}

	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x06001B80 RID: 7040 RVA: 0x000958D4 File Offset: 0x00093AD4
	public PhotonView GetView
	{
		get
		{
			return this.punView;
		}
	}

	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x06001B81 RID: 7041 RVA: 0x000958DC File Offset: 0x00093ADC
	public NetPlayer Owner
	{
		get
		{
			return NetworkSystem.Instance.GetPlayer(this.punView.Owner);
		}
	}

	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06001B82 RID: 7042 RVA: 0x000958F3 File Offset: 0x00093AF3
	public int ViewID
	{
		get
		{
			return this.punView.ViewID;
		}
	}

	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06001B83 RID: 7043 RVA: 0x00095900 File Offset: 0x00093B00
	// (set) Token: 0x06001B84 RID: 7044 RVA: 0x0009590D File Offset: 0x00093B0D
	internal OwnershipOption OwnershipTransfer
	{
		get
		{
			return this.punView.OwnershipTransfer;
		}
		set
		{
			this.punView.OwnershipTransfer = value;
			if (this.reliableView != null)
			{
				this.reliableView.OwnershipTransfer = value;
			}
		}
	}

	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06001B85 RID: 7045 RVA: 0x00095935 File Offset: 0x00093B35
	// (set) Token: 0x06001B86 RID: 7046 RVA: 0x00095942 File Offset: 0x00093B42
	public int OwnerActorNr
	{
		get
		{
			return this.punView.OwnerActorNr;
		}
		set
		{
			this.punView.OwnerActorNr = value;
			if (this.reliableView != null)
			{
				this.reliableView.OwnerActorNr = value;
			}
		}
	}

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06001B87 RID: 7047 RVA: 0x0009596A File Offset: 0x00093B6A
	// (set) Token: 0x06001B88 RID: 7048 RVA: 0x00095977 File Offset: 0x00093B77
	public int ControllerActorNr
	{
		get
		{
			return this.punView.ControllerActorNr;
		}
		set
		{
			this.punView.ControllerActorNr = value;
			if (this.reliableView != null)
			{
				this.reliableView.ControllerActorNr = value;
			}
		}
	}

	// Token: 0x06001B89 RID: 7049 RVA: 0x000959A0 File Offset: 0x00093BA0
	private void GetViews()
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		if (components.Length > 1)
		{
			if (components[0].Synchronization == ViewSynchronization.UnreliableOnChange)
			{
				this.punView = components[0];
				this.reliableView = components[1];
			}
			else if (components[0].Synchronization == ViewSynchronization.ReliableDeltaCompressed)
			{
				this.reliableView = components[0];
				this.punView = components[1];
			}
		}
		else
		{
			this.punView = components[0];
		}
		if (this.punView == null)
		{
			this.punView = base.GetComponent<PhotonView>();
		}
		if (this.fusionView == null)
		{
			this.fusionView = base.GetComponent<NetworkObject>();
		}
	}

	// Token: 0x06001B8A RID: 7050 RVA: 0x00095A35 File Offset: 0x00093C35
	protected virtual void Awake()
	{
		this.GetViews();
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x00095A3D File Offset: 0x00093C3D
	protected virtual void Start()
	{
		if (this._sceneObject)
		{
			NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
		}
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x00095A58 File Offset: 0x00093C58
	public void SendRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
		Player playerRef = (targetPlayer as PunNetPlayer).PlayerRef;
		this.punView.RPC(method, playerRef, parameters);
	}

	// Token: 0x06001B8D RID: 7053 RVA: 0x00095A7F File Offset: 0x00093C7F
	public void SendRPC(string method, RpcTarget target, params object[] parameters)
	{
		this.punView.RPC(method, target, parameters);
	}

	// Token: 0x06001B8E RID: 7054 RVA: 0x00095A90 File Offset: 0x00093C90
	public void SendRPC(string method, int target, params object[] parameters)
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom == null || !currentRoom.Players.ContainsKey(target))
		{
			return;
		}
		this.punView.RPC(method, currentRoom.Players[target], parameters);
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x00095ACE File Offset: 0x00093CCE
	public override void Spawned()
	{
		base.Spawned();
		this._spawned = true;
	}

	// Token: 0x06001B90 RID: 7056 RVA: 0x00095ADD File Offset: 0x00093CDD
	public void RequestOwnership()
	{
		this.GetView.RequestOwnership();
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x00095AEA File Offset: 0x00093CEA
	public void ReleaseOwnership()
	{
		this.changingStatAuth = true;
		base.Object.ReleaseStateAuthority();
	}

	// Token: 0x06001B92 RID: 7058 RVA: 0x00095AFE File Offset: 0x00093CFE
	public virtual void StateAuthorityChanged()
	{
		if (this.changingStatAuth)
		{
			this.changingStatAuth = false;
		}
	}

	// Token: 0x06001B93 RID: 7059 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06001B94 RID: 7060 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x00002C2D File Offset: 0x00000E2D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x00002C2D File Offset: 0x00000E2D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x040025B6 RID: 9654
	[SerializeField]
	private PhotonView punView;

	// Token: 0x040025B7 RID: 9655
	[SerializeField]
	private PhotonView reliableView;

	// Token: 0x040025B8 RID: 9656
	[SerializeField]
	internal NetworkObject fusionView;

	// Token: 0x040025B9 RID: 9657
	[SerializeField]
	protected bool _sceneObject;

	// Token: 0x040025BA RID: 9658
	private bool _spawned;

	// Token: 0x040025BB RID: 9659
	private bool changingStatAuth;
}
