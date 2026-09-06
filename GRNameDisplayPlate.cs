using System;
using TMPro;
using UnityEngine;

// Token: 0x020007C7 RID: 1991
public class GRNameDisplayPlate : MonoBehaviour
{
	// Token: 0x060032C1 RID: 12993 RVA: 0x001162C0 File Offset: 0x001144C0
	public void RefreshPlayerName(VRRig vrRig)
	{
		GRPlayer grplayer = GRPlayer.Get(vrRig);
		if (vrRig != null && grplayer != null)
		{
			if (!this.namePlateLabel.text.Equals(vrRig.playerNameVisible))
			{
				this.namePlateLabel.text = vrRig.playerNameVisible;
				return;
			}
		}
		else
		{
			this.namePlateLabel.text = "";
		}
	}

	// Token: 0x060032C2 RID: 12994 RVA: 0x00116320 File Offset: 0x00114520
	public void Clear()
	{
		this.namePlateLabel.text = "";
	}

	// Token: 0x040041D3 RID: 16851
	public TMP_Text namePlateLabel;
}
