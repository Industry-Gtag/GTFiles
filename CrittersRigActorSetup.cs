using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class CrittersRigActorSetup : MonoBehaviour
{
	// Token: 0x060002F0 RID: 752 RVA: 0x000118F3 File Offset: 0x0000FAF3
	public void OnEnable()
	{
		CrittersManager.RegisterRigActorSetup(this);
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x000118FC File Offset: 0x0000FAFC
	public void OnDisable()
	{
		for (int i = 0; i < this.rigActors.Length; i++)
		{
			this.rigActors[i].actorSet = null;
		}
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00011930 File Offset: 0x0000FB30
	private CrittersActor RefreshActorForIndex(int index)
	{
		CrittersRigActorSetup.RigActor rigActor = this.rigActors[index];
		if (rigActor.actorSet.IsNotNull())
		{
			rigActor.actorSet.gameObject.SetActive(false);
		}
		CrittersActor crittersActor = CrittersManager.instance.SpawnActor(rigActor.type, rigActor.subIndex);
		if (crittersActor.IsNull())
		{
			return null;
		}
		crittersActor.isOnPlayer = true;
		crittersActor.rigIndex = index;
		crittersActor.rigPlayerId = this.myRig.Creator.ActorNumber;
		if (crittersActor.rigPlayerId == -1 && PhotonNetwork.InRoom)
		{
			crittersActor.rigPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
		}
		crittersActor.PlacePlayerCrittersActor();
		return crittersActor;
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x000119D8 File Offset: 0x0000FBD8
	public void CheckUpdate(ref List<object> refActorData, bool forceCheck = false)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		for (int i = 0; i < this.rigActors.Length; i++)
		{
			CrittersRigActorSetup.RigActor rigActor = this.rigActors[i];
			RigContainer rigContainer;
			if (forceCheck || rigActor.actorSet == null || (rigActor.actorSet.rigPlayerId != this.myRig.Creator.ActorNumber && VRRigCache.Instance.TryGetVrrig(this.myRig.Creator, out rigContainer) && CrittersManager.instance.rigSetupByRig.ContainsKey(this.myRig)))
			{
				CrittersActor crittersActor = this.RefreshActorForIndex(i);
				if (crittersActor != null)
				{
					crittersActor.AddPlayerCrittersActorDataToList(ref refActorData);
				}
			}
		}
	}

	// Token: 0x04000359 RID: 857
	public CrittersRigActorSetup.RigActor[] rigActors;

	// Token: 0x0400035A RID: 858
	public List<object> rigActorData = new List<object>();

	// Token: 0x0400035B RID: 859
	public VRRig myRig;

	// Token: 0x02000079 RID: 121
	[Serializable]
	public struct RigActor
	{
		// Token: 0x0400035C RID: 860
		public Transform location;

		// Token: 0x0400035D RID: 861
		public CrittersActor.CrittersActorType type;

		// Token: 0x0400035E RID: 862
		public int subIndex;

		// Token: 0x0400035F RID: 863
		public CrittersActor actorSet;
	}
}
