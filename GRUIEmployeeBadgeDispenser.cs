using System;
using TMPro;
using UnityEngine;

// Token: 0x02000831 RID: 2097
public class GRUIEmployeeBadgeDispenser : MonoBehaviour
{
	// Token: 0x060035DE RID: 13790 RVA: 0x0012984E File Offset: 0x00127A4E
	public void Setup(GhostReactor reactor, int employeeIndex)
	{
		this.reactor = reactor;
	}

	// Token: 0x060035DF RID: 13791 RVA: 0x00129858 File Offset: 0x00127A58
	public void Refresh()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.actorNr);
		if (player != null && player.InRoom)
		{
			this.playerName.text = player.SanitizedNickName;
			if (this.idBadge != null)
			{
				this.idBadge.RefreshText(player);
				return;
			}
		}
		else
		{
			this.playerName.text = "";
		}
	}

	// Token: 0x060035E0 RID: 13792 RVA: 0x001298C0 File Offset: 0x00127AC0
	public void CreateBadge(NetPlayer player, GameEntityManager entityManager)
	{
		if (entityManager.IsAuthority())
		{
			entityManager.RequestCreateItem(this.idBadgePrefab.name.GetStaticHash(), this.spawnLocation.position, this.spawnLocation.rotation, (long)(player.ActorNumber * 100 + this.index));
		}
	}

	// Token: 0x060035E1 RID: 13793 RVA: 0x00129913 File Offset: 0x00127B13
	public Transform GetSpawnMarker()
	{
		return this.spawnLocation;
	}

	// Token: 0x060035E2 RID: 13794 RVA: 0x0012991B File Offset: 0x00127B1B
	public bool IsDispenserForBadge(GRBadge badge)
	{
		return badge == this.idBadge;
	}

	// Token: 0x060035E3 RID: 13795 RVA: 0x00129929 File Offset: 0x00127B29
	public Vector3 GetSpawnPosition()
	{
		return this.spawnLocation.position;
	}

	// Token: 0x060035E4 RID: 13796 RVA: 0x00129936 File Offset: 0x00127B36
	public Quaternion GetSpawnRotation()
	{
		return this.spawnLocation.rotation;
	}

	// Token: 0x060035E5 RID: 13797 RVA: 0x00129943 File Offset: 0x00127B43
	public void ClearBadge()
	{
		this.actorNr = -1;
		this.idBadge = null;
	}

	// Token: 0x060035E6 RID: 13798 RVA: 0x00129954 File Offset: 0x00127B54
	public void AttachIDBadge(GRBadge linkedBadge, NetPlayer _player)
	{
		this.actorNr = ((_player == null) ? (-1) : _player.ActorNumber);
		this.idBadge = linkedBadge;
		this.playerName.text = ((_player == null) ? null : _player.SanitizedNickName);
		this.idBadge.Setup(_player, this.index);
	}

	// Token: 0x04004679 RID: 18041
	[SerializeField]
	private TMP_Text msg;

	// Token: 0x0400467A RID: 18042
	[SerializeField]
	private TMP_Text playerName;

	// Token: 0x0400467B RID: 18043
	[SerializeField]
	private Transform spawnLocation;

	// Token: 0x0400467C RID: 18044
	[SerializeField]
	private GameEntity idBadgePrefab;

	// Token: 0x0400467D RID: 18045
	[SerializeField]
	private LayerMask badgeLayerMask;

	// Token: 0x0400467E RID: 18046
	public int index;

	// Token: 0x0400467F RID: 18047
	public int actorNr;

	// Token: 0x04004680 RID: 18048
	public GRBadge idBadge;

	// Token: 0x04004681 RID: 18049
	private GhostReactor reactor;

	// Token: 0x04004682 RID: 18050
	private Coroutine getSpawnedBadgeCoroutine;

	// Token: 0x04004683 RID: 18051
	private static Collider[] overlapColliders = new Collider[10];

	// Token: 0x04004684 RID: 18052
	private bool isEmployee;

	// Token: 0x04004685 RID: 18053
	private const string GR_DATA_KEY = "GRData";
}
