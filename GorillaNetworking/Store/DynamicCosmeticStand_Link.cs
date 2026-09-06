using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02001147 RID: 4423
	public class DynamicCosmeticStand_Link : MonoBehaviour
	{
		// Token: 0x06006F09 RID: 28425 RVA: 0x0023C6E0 File Offset: 0x0023A8E0
		public void SetStandType(HeadModel_CosmeticStand.BustType type)
		{
			this.stand.SetStandType(type);
		}

		// Token: 0x06006F0A RID: 28426 RVA: 0x0023C6EE File Offset: 0x0023A8EE
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.stand.SpawnItemOntoStand(PlayFabID);
		}

		// Token: 0x06006F0B RID: 28427 RVA: 0x0023C6FC File Offset: 0x0023A8FC
		public void SaveCosmeticMountPosition()
		{
			this.stand.UpdateCosmeticsMountPositions();
		}

		// Token: 0x06006F0C RID: 28428 RVA: 0x0023C709 File Offset: 0x0023A909
		public void ClearCosmeticItems()
		{
			this.stand.ClearCosmetics();
		}

		// Token: 0x04007F17 RID: 32535
		public DynamicCosmeticStand stand;
	}
}
