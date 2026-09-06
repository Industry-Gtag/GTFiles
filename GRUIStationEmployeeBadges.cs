using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000838 RID: 2104
public class GRUIStationEmployeeBadges : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600361B RID: 13851 RVA: 0x0012AEF8 File Offset: 0x001290F8
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].Setup(reactor, i);
		}
	}

	// Token: 0x0600361C RID: 13852 RVA: 0x0012AF38 File Offset: 0x00129138
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.registeredBadges = new List<GRBadge>();
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].index = i;
			this.badgeDispensers[i].actorNr = -1;
		}
		this.dispenserForActorNr = new Dictionary<int, int>();
		VRRigCache.OnRigActivated += this.UpdateRigs;
		VRRigCache.OnRigDeactivated += this.UpdateRigs;
		RoomSystem.JoinedRoomEvent += new Action(this.UpdateRigs);
		this.UpdateRigs();
	}

	// Token: 0x0600361D RID: 13853 RVA: 0x0012AFE0 File Offset: 0x001291E0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		VRRigCache.OnRigActivated -= this.UpdateRigs;
		VRRigCache.OnRigDeactivated -= this.UpdateRigs;
		RoomSystem.JoinedRoomEvent -= new Action(this.UpdateRigs);
	}

	// Token: 0x0600361E RID: 13854 RVA: 0x0012B032 File Offset: 0x00129232
	public void UpdateRigs(RigContainer container)
	{
		this.UpdateRigs();
	}

	// Token: 0x0600361F RID: 13855 RVA: 0x0012B03A File Offset: 0x0012923A
	public void UpdateRigs()
	{
		GRUIStationEmployeeBadges.tempRigs.Clear();
		GRUIStationEmployeeBadges.tempRigs.Add(VRRig.LocalRig);
		if (VRRigCache.Instance != null)
		{
			VRRigCache.Instance.GetAllUsedRigs(GRUIStationEmployeeBadges.tempRigs);
		}
	}

	// Token: 0x06003620 RID: 13856 RVA: 0x0012B074 File Offset: 0x00129274
	public void RefreshBadgesAuthority()
	{
		for (int i = 0; i < GRUIStationEmployeeBadges.tempRigs.Count; i++)
		{
			NetPlayer netPlayer = (GRUIStationEmployeeBadges.tempRigs[i].isOfflineVRRig ? NetworkSystem.Instance.LocalPlayer : GRUIStationEmployeeBadges.tempRigs[i].OwningNetPlayer);
			int num;
			if (netPlayer != null && netPlayer.ActorNumber != -1 && !this.dispenserForActorNr.TryGetValue(netPlayer.ActorNumber, out num))
			{
				for (int j = 0; j < this.badgeDispensers.Count; j++)
				{
					if (this.badgeDispensers[j].actorNr == -1)
					{
						this.badgeDispensers[j].CreateBadge(netPlayer, this.reactor.grManager.gameEntityManager);
						break;
					}
				}
			}
		}
		for (int k = this.registeredBadges.Count - 1; k >= 0; k--)
		{
			int num2;
			if (NetworkSystem.Instance.GetNetPlayerByID(this.registeredBadges[k].actorNr) == null || !this.dispenserForActorNr.TryGetValue(this.registeredBadges[k].actorNr, out num2) || num2 != this.registeredBadges[k].dispenserIndex)
			{
				this.reactor.grManager.gameEntityManager.RequestDestroyItem(this.registeredBadges[k].GetComponent<GameEntity>().id);
			}
		}
	}

	// Token: 0x06003621 RID: 13857 RVA: 0x0012B1E0 File Offset: 0x001293E0
	public void SliceUpdate()
	{
		if (this.reactor == null || this.reactor.grManager == null)
		{
			return;
		}
		if (!this.reactor.grManager.IsZoneActive())
		{
			return;
		}
		if (this.reactor.grManager.gameEntityManager.IsAuthority())
		{
			this.RefreshBadgesAuthority();
		}
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].Refresh();
		}
	}

	// Token: 0x06003622 RID: 13858 RVA: 0x0012B268 File Offset: 0x00129468
	public void RemoveBadge(GRBadge badge)
	{
		if (this.registeredBadges.Contains(badge))
		{
			this.registeredBadges.Remove(badge);
		}
		if (this.badgeDispensers[badge.dispenserIndex].idBadge == badge)
		{
			this.dispenserForActorNr.Remove(badge.actorNr);
			this.badgeDispensers[badge.dispenserIndex].ClearBadge();
		}
	}

	// Token: 0x06003623 RID: 13859 RVA: 0x0012B2D8 File Offset: 0x001294D8
	public void LinkBadgeToDispenser(GRBadge badge, long createData)
	{
		if (!this.registeredBadges.Contains(badge))
		{
			this.registeredBadges.Add(badge);
		}
		int num = (int)(createData % 100L);
		if (num > this.badgeDispensers.Count)
		{
			return;
		}
		NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID((int)(createData / 100L));
		if (netPlayerByID != null)
		{
			this.dispenserForActorNr[netPlayerByID.ActorNumber] = num;
			this.badgeDispensers[num].AttachIDBadge(badge, netPlayerByID);
		}
	}

	// Token: 0x06003624 RID: 13860 RVA: 0x0012B350 File Offset: 0x00129550
	public GRUIEmployeeBadgeDispenser GetDispenserForPlayer(int actorNumber)
	{
		int num;
		if (!this.dispenserForActorNr.TryGetValue(actorNumber, out num))
		{
			return null;
		}
		return this.badgeDispensers[num];
	}

	// Token: 0x040046C6 RID: 18118
	[SerializeField]
	public List<GRUIEmployeeBadgeDispenser> badgeDispensers;

	// Token: 0x040046C7 RID: 18119
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x040046C8 RID: 18120
	public Dictionary<int, int> dispenserForActorNr;

	// Token: 0x040046C9 RID: 18121
	public List<GRBadge> registeredBadges;

	// Token: 0x040046CA RID: 18122
	private GhostReactor reactor;
}
