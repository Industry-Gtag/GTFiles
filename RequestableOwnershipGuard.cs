using System;
using System.Collections;
using System.Collections.Generic;
using GorillaExtensions;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000396 RID: 918
[RequireComponent(typeof(NetworkView))]
public class RequestableOwnershipGuard : MonoBehaviourPunCallbacks, ISelfValidator
{
	// Token: 0x06001640 RID: 5696 RVA: 0x0008144B File Offset: 0x0007F64B
	private void SetViewToRequest()
	{
		base.GetComponent<NetworkView>().OwnershipTransfer = OwnershipOption.Request;
	}

	// Token: 0x1700022F RID: 559
	// (get) Token: 0x06001641 RID: 5697 RVA: 0x00081459 File Offset: 0x0007F659
	private NetworkView netView
	{
		get
		{
			if (this.netViews == null)
			{
				return null;
			}
			if (this.netViews.Length == 0)
			{
				return null;
			}
			return this.netViews[0];
		}
	}

	// Token: 0x17000230 RID: 560
	// (get) Token: 0x06001642 RID: 5698 RVA: 0x00081478 File Offset: 0x0007F678
	[DevInspectorShow]
	public bool isTrulyMine
	{
		get
		{
			return object.Equals(this.actualOwner, NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x17000231 RID: 561
	// (get) Token: 0x06001643 RID: 5699 RVA: 0x0008148F File Offset: 0x0007F68F
	public bool isMine
	{
		get
		{
			return object.Equals(this.currentOwner, NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x06001644 RID: 5700 RVA: 0x000814A6 File Offset: 0x0007F6A6
	private void BindNetworkViews()
	{
		this.netViews = base.GetComponents<NetworkView>();
	}

	// Token: 0x06001645 RID: 5701 RVA: 0x000814B4 File Offset: 0x0007F6B4
	public override void OnDisable()
	{
		base.OnDisable();
		RequestableOwnershipGuardHandler.RemoveViews(this.netViews, this);
		NetworkSystem.Instance.OnPlayerJoined -= this.PlayerEnteredRoom;
		NetworkSystem.Instance.OnPlayerLeft -= this.PlayerLeftRoom;
		NetworkSystem.Instance.OnJoinedRoomEvent -= this.JoinedRoom;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.MasterClientSwitch;
		this.currentMasterClient = null;
		this.currentOwner = null;
		this.actualOwner = null;
		this.creator = NetworkSystem.Instance.LocalPlayer;
		this.currentState = NetworkingState.IsOwner;
	}

	// Token: 0x06001646 RID: 5702 RVA: 0x00081584 File Offset: 0x0007F784
	public override void OnEnable()
	{
		base.OnEnable();
		if (this.autoRegister)
		{
			this.BindNetworkViews();
		}
		if (this.netViews == null)
		{
			return;
		}
		RequestableOwnershipGuardHandler.RegisterViews(this.netViews, this);
		NetworkSystem.Instance.OnPlayerJoined += this.PlayerEnteredRoom;
		NetworkSystem.Instance.OnPlayerLeft += this.PlayerLeftRoom;
		NetworkSystem.Instance.OnJoinedRoomEvent += this.JoinedRoom;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.MasterClientSwitch;
		NetworkSystem instance = NetworkSystem.Instance;
		if (instance == null || !instance.InRoom)
		{
			GorillaTagger.OnPlayerSpawned(delegate
			{
				this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			});
			return;
		}
		this.currentMasterClient = NetworkSystem.Instance.MasterClient;
		int creatorActorNr = this.netView.GetView.CreatorActorNr;
		NetPlayer netPlayer = this.currentMasterClient;
		int? num = ((netPlayer != null) ? new int?(netPlayer.ActorNumber) : null);
		if (!((creatorActorNr == num.GetValueOrDefault()) & (num != null)))
		{
			this.SetOwnership(NetworkSystem.Instance.GetPlayer(this.netView.GetView.CreatorActorNr), false, false);
			return;
		}
		if (this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			this.currentState = NetworkingState.IsOwner;
			return;
		}
		this.currentState = NetworkingState.IsBlindClient;
		this.SetOwnership(null, false, false);
		this.RequestTheCurrentOwnerFromAuthority();
	}

	// Token: 0x06001647 RID: 5703 RVA: 0x00081720 File Offset: 0x0007F920
	private void PlayerEnteredRoom(NetPlayer player)
	{
		try
		{
			if (!player.IsLocal)
			{
				if (NetworkSystem.Instance.InRoom && this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
				{
					this.netView.SendRPC("SetOwnershipFromMasterClient", player, new object[] { this.currentOwner.GetPlayerRef() });
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x00081794 File Offset: 0x0007F994
	public override void OnPreLeavingRoom()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		case NetworkingState.IsClient:
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
			{
				callback.OnMyOwnerLeft();
			});
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
	}

	// Token: 0x06001649 RID: 5705 RVA: 0x00081818 File Offset: 0x0007FA18
	private void JoinedRoom()
	{
		this.currentMasterClient = NetworkSystem.Instance.MasterClient;
		if (this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			this.currentState = NetworkingState.IsOwner;
			return;
		}
		this.currentState = NetworkingState.IsBlindClient;
		this.SetOwnership(null, false, false);
	}

	// Token: 0x0600164A RID: 5706 RVA: 0x00081874 File Offset: 0x0007FA74
	private void PlayerLeftRoom(NetPlayer otherPlayer)
	{
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		case NetworkingState.IsBlindClient:
			if (this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
			{
				this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
				return;
			}
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		case NetworkingState.IsClient:
			if (this.creator != null && object.Equals(this.creator, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyCreatorLeft();
				});
			}
			if (object.Equals(this.actualOwner, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyOwnerLeft();
				});
				if (this.fallbackOwner != null)
				{
					this.SetOwnership(this.fallbackOwner, false, false);
					return;
				}
				this.SetOwnership(this.currentMasterClient, false, false);
				return;
			}
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			if (this.creator != null && object.Equals(this.creator, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyCreatorLeft();
				});
			}
			if (this.currentState == NetworkingState.ForcefullyTakingOver && object.Equals(this.currentOwner, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyOwnerLeft();
				});
			}
			if (object.Equals(this.actualOwner, otherPlayer))
			{
				if (this.fallbackOwner != null)
				{
					this.SetOwnership(this.fallbackOwner, false, false);
					if (object.Equals(this.fallbackOwner, PhotonNetwork.LocalPlayer))
					{
						Action action = this.ownershipRequestAccepted;
						if (action == null)
						{
							return;
						}
						action();
						return;
					}
					else
					{
						Action action2 = this.ownershipDenied;
						if (action2 == null)
						{
							return;
						}
						action2();
						return;
					}
				}
				else if (object.Equals(this.currentMasterClient, PhotonNetwork.LocalPlayer))
				{
					Action action3 = this.ownershipRequestAccepted;
					if (action3 == null)
					{
						return;
					}
					action3();
					return;
				}
				else
				{
					Action action4 = this.ownershipDenied;
					if (action4 == null)
					{
						return;
					}
					action4();
					return;
				}
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600164B RID: 5707 RVA: 0x00081A94 File Offset: 0x0007FC94
	private void MasterClientSwitch(NetPlayer newMaster)
	{
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsClient:
			if (this.actualOwner == null && this.currentMasterClient == null)
			{
				this.SetOwnership(newMaster, false, false);
			}
			break;
		case NetworkingState.IsBlindClient:
			if (object.Equals(newMaster, NetworkSystem.Instance.LocalPlayer))
			{
				this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			}
			else
			{
				this.RequestTheCurrentOwnerFromAuthority();
			}
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this.currentMasterClient = newMaster;
	}

	// Token: 0x0600164C RID: 5708 RVA: 0x00081B24 File Offset: 0x0007FD24
	[PunRPC]
	public void RequestCurrentOwnerFromAuthorityRPC(PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		MonkeAgent.IncrementRPCCall(info, "RequestCurrentOwnerFromAuthorityRPC");
		if (!this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 22, info.SentServerTime))
		{
			return;
		}
		this.netView.SendRPC("SetOwnershipFromMasterClient", player, new object[] { this.actualOwner.GetPlayerRef() });
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x00081BB0 File Offset: 0x0007FDB0
	[PunRPC]
	public void TransferOwnershipFromToRPC([CanBeNull] Player nextplayer, string nonce, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "TransferOwnershipFromToRPC");
		if (nextplayer == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(nextplayer);
		NetPlayer player2 = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (!this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer) && base.photonView.OwnerActorNr != info.Sender.ActorNumber)
		{
			NetPlayer netPlayer = this.currentOwner;
			int? num = ((netPlayer != null) ? new int?(netPlayer.ActorNumber) : null);
			int num2 = info.Sender.ActorNumber;
			if (!((num.GetValueOrDefault() == num2) & (num != null)))
			{
				NetPlayer netPlayer2 = this.actualOwner;
				num = ((netPlayer2 != null) ? new int?(netPlayer2.ActorNumber) : null);
				num2 = info.Sender.ActorNumber;
				if (!((num.GetValueOrDefault() == num2) & (num != null)))
				{
					return;
				}
			}
		}
		if (this.currentOwner == null)
		{
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(player2, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 22, info.SentServerTime))
			{
				return;
			}
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		}
		else
		{
			if (this.currentOwner.ActorNumber != base.photonView.OwnerActorNr)
			{
				return;
			}
			if (this.actualOwner.ActorNumber == player.ActorNumber)
			{
				return;
			}
			switch (this.currentState)
			{
			case NetworkingState.IsClient:
				this.SetOwnership(player, false, false);
				return;
			case NetworkingState.ForcefullyTakingOver:
			case NetworkingState.RequestingOwnership:
				if (this.ownershipRequestNonce == nonce)
				{
					this.ownershipRequestNonce = "";
					this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
					return;
				}
				this.actualOwner = player;
				return;
			case NetworkingState.RequestingOwnershipWaitingForSight:
			case NetworkingState.ForcefullyTakingOverWaitingForSight:
				this.RequestTheCurrentOwnerFromAuthority();
				return;
			}
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x00081D7C File Offset: 0x0007FF7C
	[PunRPC]
	public void SetOwnershipFromMasterClient([CanBeNull] Player nextMaster, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "SetOwnershipFromMasterClient");
		if (nextMaster == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(nextMaster);
		NetPlayer player2 = NetworkSystem.Instance.GetPlayer(info.Sender);
		this.SetOwnershipFromMasterClient(player, player2);
	}

	// Token: 0x0600164F RID: 5711 RVA: 0x00081DC0 File Offset: 0x0007FFC0
	public void SetOwnershipFromMasterClient([CanBeNull] NetPlayer nextMaster, NetPlayer sender)
	{
		if (nextMaster == null)
		{
			return;
		}
		if (!this.PlayerHasAuthority(sender))
		{
			MonkeAgent.instance.SendReport("Sent an SetOwnershipFromMasterClient when they weren't the master client", sender.UserId, sender.NickName);
			return;
		}
		NetworkingState networkingState;
		if (this.currentOwner == null)
		{
			networkingState = this.currentState;
			if (networkingState != NetworkingState.IsBlindClient)
			{
				int num = networkingState - NetworkingState.RequestingOwnershipWaitingForSight;
			}
		}
		networkingState = this.currentState;
		if (networkingState - NetworkingState.ForcefullyTakingOver <= 3 && object.Equals(nextMaster, PhotonNetwork.LocalPlayer))
		{
			Action action = this.ownershipRequestAccepted;
			if (action != null)
			{
				action();
			}
			this.SetOwnership(nextMaster, false, false);
			return;
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			this.SetOwnership(nextMaster, false, false);
			return;
		case NetworkingState.ForcefullyTakingOver:
			this.actualOwner = nextMaster;
			this.currentState = NetworkingState.ForcefullyTakingOver;
			return;
		case NetworkingState.RequestingOwnership:
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			this.currentState = NetworkingState.RequestingOwnership;
			return;
		case NetworkingState.RequestingOwnershipWaitingForSight:
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			this.currentState = NetworkingState.RequestingOwnership;
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[] { this.ownershipRequestNonce });
			return;
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.actualOwner = nextMaster;
			this.currentState = NetworkingState.ForcefullyTakingOver;
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[] { this.ownershipRequestNonce });
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001650 RID: 5712 RVA: 0x00081F54 File Offset: 0x00080154
	[PunRPC]
	public void OwnershipRequested(string nonce, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		MonkeAgent.IncrementRPCCall(info, "OwnershipRequested");
		if (nonce != null && nonce.Length > 68)
		{
			return;
		}
		if (info.Sender == PhotonNetwork.LocalPlayer)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[8].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		bool flag = true;
		using (List<IRequestableOwnershipGuardCallbacks>.Enumerator enumerator = this.callbacksList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.OnOwnershipRequest(player))
				{
					flag = false;
				}
			}
		}
		if (!flag)
		{
			this.netView.SendRPC("OwnershipRequestDenied", player, new object[] { nonce });
			return;
		}
		this.TransferOwnership(player, nonce);
	}

	// Token: 0x06001651 RID: 5713 RVA: 0x00082040 File Offset: 0x00080240
	private void TransferOwnershipWithID(int id)
	{
		this.TransferOwnership(NetworkSystem.Instance.GetPlayer(id), "");
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x00082058 File Offset: 0x00080258
	public void TransferOwnership(NetPlayer player, string Nonce = "")
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.SetOwnership(player, false, false);
			return;
		}
		if (base.photonView.IsMine)
		{
			this.SetOwnership(player, false, false);
			this.netView.SendRPC("TransferOwnershipFromToRPC", RpcTarget.Others, new object[]
			{
				player.GetPlayerRef(),
				Nonce
			});
			return;
		}
		if (this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			this.SetOwnership(player, false, false);
			this.netView.SendRPC("SetOwnershipFromMasterClient", RpcTarget.Others, new object[] { player.GetPlayerRef() });
			return;
		}
		Debug.LogError("Tried to transfer ownership when im not the owner or a master client");
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x000820FF File Offset: 0x000802FF
	public void RequestTheCurrentOwnerFromAuthority()
	{
		this.netView.SendRPC("RequestCurrentOwnerFromAuthorityRPC", this.GetAuthoritativePlayer(), Array.Empty<object>());
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x0008211C File Offset: 0x0008031C
	protected void SetCurrentOwner(NetPlayer player)
	{
		if (player == null)
		{
			this.currentOwner = null;
		}
		else
		{
			this.currentOwner = player;
		}
		foreach (NetworkView networkView in this.netViews)
		{
			if (player == null)
			{
				networkView.OwnerActorNr = -1;
				networkView.ControllerActorNr = -1;
			}
			else
			{
				networkView.OwnerActorNr = player.ActorNumber;
				networkView.ControllerActorNr = player.ActorNumber;
			}
		}
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x00082180 File Offset: 0x00080380
	protected internal void SetOwnership(NetPlayer player, bool isLocalOnly = false, bool dontPropigate = false)
	{
		if (!object.Equals(player, this.currentOwner) && !dontPropigate)
		{
			this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks actualOwner)
			{
				actualOwner.OnOwnershipTransferred(player, this.currentOwner);
			});
		}
		this.SetCurrentOwner(player);
		if (isLocalOnly)
		{
			return;
		}
		this.actualOwner = player;
		if (player == null)
		{
			return;
		}
		if (player.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.currentState = NetworkingState.IsOwner;
			return;
		}
		this.currentState = NetworkingState.IsClient;
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x0008221E File Offset: 0x0008041E
	public NetPlayer GetAuthoritativePlayer()
	{
		if (this.giveCreatorAbsoluteAuthority)
		{
			return this.creator;
		}
		return NetworkSystem.Instance.MasterClient;
	}

	// Token: 0x06001657 RID: 5719 RVA: 0x0008223C File Offset: 0x0008043C
	[PunRPC]
	public void OwnershipRequestDenied(string nonce, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		MonkeAgent.IncrementRPCCall(info, "OwnershipRequestDenied");
		int actorNumber = info.Sender.ActorNumber;
		NetPlayer netPlayer = this.actualOwner;
		int? num = ((netPlayer != null) ? new int?(netPlayer.ActorNumber) : null);
		if (!((actorNumber == num.GetValueOrDefault()) & (num != null)) && !this.PlayerHasAuthority(player))
		{
			return;
		}
		Action action = this.ownershipDenied;
		if (action != null)
		{
			action();
		}
		this.ownershipDenied = null;
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.currentState = NetworkingState.IsClient;
			this.SetOwnership(this.actualOwner, false, false);
			return;
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[] { this.ownershipRequestNonce });
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001658 RID: 5720 RVA: 0x00082332 File Offset: 0x00080532
	public IEnumerator RequestTimeout()
	{
		Debug.Log(string.Format("Timeout request started...  {0} ", this.currentState));
		yield return new WaitForSecondsRealtime(2f);
		Debug.Log(string.Format("Timeout request ended! {0} ", this.currentState));
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.currentState = NetworkingState.IsClient;
			this.SetOwnership(this.actualOwner, false, false);
			break;
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[] { this.ownershipRequestNonce });
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		yield break;
	}

	// Token: 0x06001659 RID: 5721 RVA: 0x00082344 File Offset: 0x00080544
	public void RequestOwnership(Action onRequestSuccess, Action onRequestFailed)
	{
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
			return;
		case NetworkingState.IsBlindClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.currentState = NetworkingState.RequestingOwnershipWaitingForSight;
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.IsClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.currentState = NetworkingState.RequestingOwnership;
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[] { this.ownershipRequestNonce });
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			base.StartCoroutine("RequestTimeout");
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600165A RID: 5722 RVA: 0x00082440 File Offset: 0x00080640
	public void RequestOwnershipImmediately(Action onRequestFailed)
	{
		Debug.Log("WorldShareable RequestOwnershipImmediately");
		if (this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			this.RequestOwnershipImmediatelyWithGuaranteedAuthority();
			return;
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		{
			bool inRoom = NetworkSystem.Instance.InRoom;
			return;
		}
		case NetworkingState.IsBlindClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.currentState = NetworkingState.ForcefullyTakingOverWaitingForSight;
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, true, false);
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		case NetworkingState.IsClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.currentState = NetworkingState.ForcefullyTakingOver;
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, true, false);
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[] { this.ownershipRequestNonce });
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.currentState = NetworkingState.ForcefullyTakingOver;
			base.StartCoroutine("RequestTimeout");
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x00082584 File Offset: 0x00080784
	public void RequestOwnershipImmediatelyWithGuaranteedAuthority()
	{
		Debug.Log("WorldShareable RequestOwnershipImmediatelyWithGuaranteedAuthority");
		if (!this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			Debug.LogError("Tried to request ownership immediately with guaranteed authority without acutely having authority ");
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
			return;
		case NetworkingState.IsBlindClient:
			this.currentState = NetworkingState.ForcefullyTakingOverWaitingForSight;
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, true, false);
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		case NetworkingState.IsClient:
			this.currentState = NetworkingState.ForcefullyTakingOver;
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, true, false);
			this.netView.SendRPC("SetOwnershipFromMasterClient", RpcTarget.All, new object[] { PhotonNetwork.LocalPlayer });
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.currentState = NetworkingState.ForcefullyTakingOver;
			base.StartCoroutine("RequestTimeout");
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600165C RID: 5724 RVA: 0x00082659 File Offset: 0x00080859
	public void AddCallbackTarget(IRequestableOwnershipGuardCallbacks callbackObject)
	{
		if (!this.callbacksList.Contains(callbackObject))
		{
			this.callbacksList.Add(callbackObject);
			if (this.currentOwner != null)
			{
				callbackObject.OnOwnershipTransferred(this.currentOwner, null);
			}
		}
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x0008268A File Offset: 0x0008088A
	public void RemoveCallbackTarget(IRequestableOwnershipGuardCallbacks callbackObject)
	{
		if (this.callbacksList.Contains(callbackObject))
		{
			this.callbacksList.Remove(callbackObject);
			if (this.currentOwner != null)
			{
				callbackObject.OnOwnershipTransferred(null, this.currentOwner);
			}
		}
	}

	// Token: 0x0600165E RID: 5726 RVA: 0x000826BC File Offset: 0x000808BC
	public void SetCreator(NetPlayer player)
	{
		this.creator = player;
	}

	// Token: 0x17000232 RID: 562
	// (get) Token: 0x0600165F RID: 5727 RVA: 0x000826C5 File Offset: 0x000808C5
	private NetworkingState EdCurrentState
	{
		get
		{
			return this.currentState;
		}
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Validate(SelfValidationResult result)
	{
	}

	// Token: 0x06001661 RID: 5729 RVA: 0x000826CD File Offset: 0x000808CD
	public bool PlayerHasAuthority(NetPlayer player)
	{
		return object.Equals(this.GetAuthoritativePlayer(), player);
	}

	// Token: 0x0400207E RID: 8318
	[DevInspectorShow]
	[DevInspectorColor("#ff5")]
	public NetworkingState currentState;

	// Token: 0x0400207F RID: 8319
	[FormerlySerializedAs("NetworkView")]
	[SerializeField]
	private NetworkView[] netViews;

	// Token: 0x04002080 RID: 8320
	[DevInspectorHide]
	[SerializeField]
	private bool autoRegister = true;

	// Token: 0x04002081 RID: 8321
	[DevInspectorShow]
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	public NetPlayer currentOwner;

	// Token: 0x04002082 RID: 8322
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	private NetPlayer currentMasterClient;

	// Token: 0x04002083 RID: 8323
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	private NetPlayer fallbackOwner;

	// Token: 0x04002084 RID: 8324
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	public NetPlayer creator;

	// Token: 0x04002085 RID: 8325
	public bool giveCreatorAbsoluteAuthority;

	// Token: 0x04002086 RID: 8326
	public bool attemptMasterAssistedTakeoverOnDeny;

	// Token: 0x04002087 RID: 8327
	private Action ownershipDenied;

	// Token: 0x04002088 RID: 8328
	private Action ownershipRequestAccepted;

	// Token: 0x04002089 RID: 8329
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	[DevInspectorShow]
	public NetPlayer actualOwner;

	// Token: 0x0400208A RID: 8330
	public string ownershipRequestNonce;

	// Token: 0x0400208B RID: 8331
	public List<IRequestableOwnershipGuardCallbacks> callbacksList = new List<IRequestableOwnershipGuardCallbacks>();
}
