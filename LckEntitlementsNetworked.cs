using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;

// Token: 0x02000404 RID: 1028
public class LckEntitlementsNetworked : MonoBehaviour
{
	// Token: 0x06001864 RID: 6244 RVA: 0x0008AD30 File Offset: 0x00088F30
	public void Awake()
	{
		if (this.m_rigNetworkController.IsNull())
		{
			this.m_rigNetworkController = base.GetComponentInParent<VRRigSerializer>();
		}
		if (this.m_rigNetworkController.IsNull())
		{
			Debug.LogError("LCK: Unable to find VRRigSerializer for LckEntitlementsNetworked.");
			return;
		}
		InDelegateListProcessor<RigContainer, PhotonMessageInfoWrapped> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
		if (succesfullSpawnEvent == null)
		{
			return;
		}
		InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccessfulSpawn);
		succesfullSpawnEvent.Add(in inAction);
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x0008AD94 File Offset: 0x00088F94
	private void OnSuccessfulSpawn(in RigContainer rig, in PhotonMessageInfoWrapped info)
	{
		if (LckEntitlementsManager.Instance == null)
		{
			Debug.LogError("LCK: LckEntitlementsManager.Instance is not available in the scene!");
			return;
		}
		string userId = this.m_rigNetworkController.VRRig.OwningNetPlayer.UserId;
		if (userId.IsNullOrEmpty())
		{
			Debug.LogError("LCK: owningUserId is null on spawn. Cannot process entitlements.");
			return;
		}
		if (rig.Rig.isLocal)
		{
			LckEntitlementsManager.Instance.OnLocalPlayerSpawned(userId);
			return;
		}
		LckEntitlementsManager.Instance.OnRemotePlayerSpawned(userId);
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x0008AE08 File Offset: 0x00089008
	private void OnDestroy()
	{
		if (this.m_rigNetworkController != null && this.m_rigNetworkController.SuccesfullSpawnEvent != null)
		{
			ListProcessor<InAction<RigContainer, PhotonMessageInfoWrapped>> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
			InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccessfulSpawn);
			succesfullSpawnEvent.Remove(in inAction);
		}
	}

	// Token: 0x0400239A RID: 9114
	[SerializeField]
	private VRRigSerializer m_rigNetworkController;
}
