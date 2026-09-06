using System;
using Liv.Lck.Cosmetics;
using UnityEngine;

// Token: 0x020003EC RID: 1004
public class GtLckNetworkCosmeticDependantPlayerIdSupplier : MonoBehaviour, ILckCosmeticDependantPlayerIdSupplier
{
	// Token: 0x14000032 RID: 50
	// (add) Token: 0x060017D4 RID: 6100 RVA: 0x00088C04 File Offset: 0x00086E04
	// (remove) Token: 0x060017D5 RID: 6101 RVA: 0x00088C3C File Offset: 0x00086E3C
	public event PlayerIdUpdatedEvent PlayerIdUpdated;

	// Token: 0x060017D6 RID: 6102 RVA: 0x00088C71 File Offset: 0x00086E71
	public string GetPlayerId()
	{
		return this.vrrig.OwningNetPlayer.UserId;
	}

	// Token: 0x060017D7 RID: 6103 RVA: 0x00088C83 File Offset: 0x00086E83
	public void UpdatePlayerId()
	{
		PlayerIdUpdatedEvent playerIdUpdated = this.PlayerIdUpdated;
		if (playerIdUpdated == null)
		{
			return;
		}
		playerIdUpdated();
	}

	// Token: 0x04002316 RID: 8982
	[SerializeField]
	private VRRig vrrig;
}
