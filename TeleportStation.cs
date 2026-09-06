using System;
using GorillaNetworking;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000D5F RID: 3423
public class TeleportStation : MonoBehaviour
{
	// Token: 0x060054B3 RID: 21683 RVA: 0x001BC7CC File Offset: 0x001BA9CC
	private void Start()
	{
		if (!this.sourceFriendColliderRef.TryResolve<GorillaFriendCollider>(out this.sourceFriendCollider))
		{
			Debug.LogError(string.Format("Unable to resolve source friend collider: {0}!", this.sourceFriendColliderRef));
		}
		if (!this.destinationFriendColliderRef.TryResolve<GorillaFriendCollider>(out this.destinationFriendCollider))
		{
			Debug.LogError(string.Format("Unable to resolve source friend collider: {0}!", this.destinationFriendCollider));
		}
		if (!this.destinationJoinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this.destinationJoinTrigger))
		{
			Debug.LogError(string.Format("Unable to resolve source friend collider: {0}!", this.destinationJoinTriggerRef));
		}
	}

	// Token: 0x060054B4 RID: 21684 RVA: 0x001BC85C File Offset: 0x001BAA5C
	public void Attempt1PTeleport(PhotonMessageInfoWrapped info)
	{
		TeleportStationManager.Instance.FirstPersonTeleport(this.targetPos, this.targetRot, this.targetSlop, this.teleportToZone, this.sourceFriendCollider, this.destinationFriendCollider, this.destinationJoinTrigger, this.effectTime);
	}

	// Token: 0x060054B5 RID: 21685 RVA: 0x001BC8A4 File Offset: 0x001BAAA4
	public void Attempt3PTeleport(PhotonMessageInfoWrapped info)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			TeleportStationManager.Instance.ThirdPersonTeleport(rigContainer.Rig, this.effectTime);
			if (!NetworkSystem.Instance.SessionIsPrivate && FriendshipGroupDetection.Instance.IsInParty && FriendshipGroupDetection.Instance.IsInMyGroup(info.Sender.UserId))
			{
				NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
				if (this.sourceFriendCollider.playerIDsCurrentlyTouching.Contains(localPlayer.UserId))
				{
					this.Attempt1PTeleport(info);
					return;
				}
				FriendshipGroupDetection.Instance.LeaveParty();
			}
		}
	}

	// Token: 0x060054B6 RID: 21686 RVA: 0x001BC940 File Offset: 0x001BAB40
	private int LowestActorNumberInFriendCollider()
	{
		this.sourceFriendCollider.RefreshPlayersWithinBounds();
		this.destinationFriendCollider.RefreshPlayersWithinBounds();
		int num = int.MaxValue;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			if (num > allNetPlayers[i].ActorNumber && (this.sourceFriendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) || this.destinationFriendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId)))
			{
				num = allNetPlayers[i].ActorNumber;
			}
		}
		return num;
	}

	// Token: 0x060054B7 RID: 21687 RVA: 0x001BC9CB File Offset: 0x001BABCB
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(this.targetPos, 3f);
	}

	// Token: 0x040065F6 RID: 26102
	[SerializeField]
	private Transform target;

	// Token: 0x040065F7 RID: 26103
	[SerializeField]
	private Vector3 targetPos;

	// Token: 0x040065F8 RID: 26104
	[SerializeField]
	private float targetRot;

	// Token: 0x040065F9 RID: 26105
	[SerializeField]
	private Vector3 targetSlop;

	// Token: 0x040065FA RID: 26106
	[SerializeField]
	private GTZone teleportToZone;

	// Token: 0x040065FB RID: 26107
	[SerializeField]
	private XSceneRef sourceFriendColliderRef;

	// Token: 0x040065FC RID: 26108
	[SerializeField]
	private XSceneRef destinationFriendColliderRef;

	// Token: 0x040065FD RID: 26109
	[SerializeField]
	private XSceneRef destinationJoinTriggerRef;

	// Token: 0x040065FE RID: 26110
	private GorillaFriendCollider sourceFriendCollider;

	// Token: 0x040065FF RID: 26111
	private GorillaFriendCollider destinationFriendCollider;

	// Token: 0x04006600 RID: 26112
	private GorillaNetworkJoinTrigger destinationJoinTrigger;

	// Token: 0x04006601 RID: 26113
	[SerializeField]
	private int effectTime;
}
