using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200028C RID: 652
public class GrabbyTentacleNetworking : MonoBehaviourPunCallbacks
{
	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x06001189 RID: 4489 RVA: 0x0005E622 File Offset: 0x0005C822
	// (set) Token: 0x0600118A RID: 4490 RVA: 0x0005E629 File Offset: 0x0005C829
	public static GrabbyTentacleNetworking Instance { get; private set; }

	// Token: 0x0600118B RID: 4491 RVA: 0x0005E634 File Offset: 0x0005C834
	private void Awake()
	{
		if (GrabbyTentacleNetworking.Instance != null && GrabbyTentacleNetworking.Instance != this)
		{
			Debug.LogWarning("[GrabbyTentacleNetworking] duplicate instance on " + base.name);
			return;
		}
		GrabbyTentacleNetworking.Instance = this;
		if (this.tablePhotonView == null)
		{
			this.tablePhotonView = base.GetComponent<PhotonView>();
		}
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x0005E691 File Offset: 0x0005C891
	private void OnDestroy()
	{
		if (GrabbyTentacleNetworking.Instance == this)
		{
			GrabbyTentacleNetworking.Instance = null;
		}
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x0005E6A6 File Offset: 0x0005C8A6
	public void Register(GrabbyTentacleController controller)
	{
		this.registeredController = controller;
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x0005E6AF File Offset: 0x0005C8AF
	public void Unregister(GrabbyTentacleController controller)
	{
		if (this.registeredController == controller)
		{
			this.registeredController = null;
		}
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x0005E6C6 File Offset: 0x0005C8C6
	public void SendGrab(int tentacleIndex, Player targetPlayer)
	{
		if (!PhotonNetwork.IsMasterClient || this.tablePhotonView == null || targetPlayer == null)
		{
			return;
		}
		this.tablePhotonView.RPC("ApplyTargetRPC", RpcTarget.All, new object[] { tentacleIndex, targetPlayer });
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x0005E708 File Offset: 0x0005C908
	[PunRPC]
	public void ApplyTargetRPC(int tentacleIndex, Player targetPlayer, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "ApplyTargetRPC");
		if (info.Sender == null || !info.Sender.IsMasterClient)
		{
			return;
		}
		if (targetPlayer == null || this.registeredController == null)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(targetPlayer, out rigContainer) || rigContainer == null)
		{
			return;
		}
		bool flag = targetPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		this.registeredController.OnGrabReceived(tentacleIndex, rigContainer.Rig, flag);
	}

	// Token: 0x040014EE RID: 5358
	[SerializeField]
	private PhotonView tablePhotonView;

	// Token: 0x040014EF RID: 5359
	private GrabbyTentacleController registeredController;
}
