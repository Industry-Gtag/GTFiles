using System;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000459 RID: 1113
[NetworkBehaviourWeaved(0)]
public abstract class NetworkComponent : NetworkView, IPunObservable, IStateAuthorityChanged, IPublicFacingInterface, IOnPhotonViewOwnerChange, IPhotonViewCallback, IInRoomCallbacks, IPunInstantiateMagicCallback
{
	// Token: 0x06001A9F RID: 6815 RVA: 0x0009490C File Offset: 0x00092B0C
	internal virtual void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		this.AddToNetwork();
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x0009491A File Offset: 0x00092B1A
	internal virtual void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x00094928 File Offset: 0x00092B28
	protected override void Start()
	{
		base.Start();
		this.AddToNetwork();
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x00094936 File Offset: 0x00092B36
	private void AddToNetwork()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x0009493E File Offset: 0x00092B3E
	public override void Spawned()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnSpawned();
		}
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x00094952 File Offset: 0x00092B52
	public override void FixedUpdateNetwork()
	{
		this.WriteDataFusion();
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x0009495A File Offset: 0x00092B5A
	public override void Render()
	{
		if (!base.HasStateAuthority)
		{
			this.ReadDataFusion();
		}
	}

	// Token: 0x06001AA6 RID: 6822
	public abstract void WriteDataFusion();

	// Token: 0x06001AA7 RID: 6823
	public abstract void ReadDataFusion();

	// Token: 0x06001AA8 RID: 6824 RVA: 0x0009496A File Offset: 0x00092B6A
	public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		this.OnSpawned();
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x00094972 File Offset: 0x00092B72
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			this.WriteDataPUN(stream, info);
			return;
		}
		if (stream.IsReading)
		{
			this.ReadDataPUN(stream, info);
		}
	}

	// Token: 0x06001AAA RID: 6826
	protected abstract void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06001AAB RID: 6827
	protected abstract void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06001AAC RID: 6828 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnSpawned()
	{
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnOwnerSwitched(NetPlayer newOwningPlayer)
	{
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x00094995 File Offset: 0x00092B95
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		this.OnOwnerSwitched(NetworkSystem.Instance.GetPlayer(newMasterClient));
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x000949A8 File Offset: 0x00092BA8
	public override void StateAuthorityChanged()
	{
		base.StateAuthorityChanged();
		if (base.Object == null)
		{
			return;
		}
		if (base.Object.StateAuthority == default(PlayerRef))
		{
			return;
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnOwnerSwitched(NetworkSystem.Instance.GetPlayer(base.Object.StateAuthority));
			return;
		}
		this.OnOwnerSwitched(NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x00094A1E File Offset: 0x00092C1E
	public void OnMasterClientSwitch(NetPlayer newMaster)
	{
		this.StateAuthorityChanged();
	}

	// Token: 0x06001AB1 RID: 6833 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06001AB2 RID: 6834 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06001AB3 RID: 6835 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06001AB5 RID: 6837 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnOwnerChange(Player newOwner, Player previousOwner)
	{
	}

	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x06001AB6 RID: 6838 RVA: 0x00094A26 File Offset: 0x00092C26
	public bool IsLocallyOwned
	{
		get
		{
			return base.IsMine;
		}
	}

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x06001AB7 RID: 6839 RVA: 0x00094A2E File Offset: 0x00092C2E
	public bool ShouldWriteObjectData
	{
		get
		{
			return NetworkSystem.Instance.ShouldWriteObjectData(base.gameObject);
		}
	}

	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x06001AB8 RID: 6840 RVA: 0x00094A40 File Offset: 0x00092C40
	public bool ShouldUpdateobject
	{
		get
		{
			return NetworkSystem.Instance.ShouldUpdateObject(base.gameObject);
		}
	}

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x06001AB9 RID: 6841 RVA: 0x00094A52 File Offset: 0x00092C52
	public int OwnerID
	{
		get
		{
			return NetworkSystem.Instance.GetOwningPlayerID(base.gameObject);
		}
	}

	// Token: 0x06001ABB RID: 6843 RVA: 0x00094A6C File Offset: 0x00092C6C
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x00094A78 File Offset: 0x00092C78
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
