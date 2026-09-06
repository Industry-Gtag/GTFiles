using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007DD RID: 2013
public class GRReadyRoom : MonoBehaviour
{
	// Token: 0x0600335C RID: 13148 RVA: 0x00118F04 File Offset: 0x00117104
	public void RefreshRigs(List<VRRig> vrRigs)
	{
		for (int i = 0; i < this.nameDisplayPlates.Count; i++)
		{
			if (this.nameDisplayPlates != null)
			{
				if (i < vrRigs.Count && vrRigs[i] != null && vrRigs[i].OwningNetPlayer != null)
				{
					this.nameDisplayPlates[i].RefreshPlayerName(vrRigs[i]);
				}
				else
				{
					this.nameDisplayPlates[i].Clear();
				}
			}
		}
	}

	// Token: 0x040042A3 RID: 17059
	public List<GRNameDisplayPlate> nameDisplayPlates;
}
