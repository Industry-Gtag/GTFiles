using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020007F8 RID: 2040
[Serializable]
public class GRShuttleUI
{
	// Token: 0x06003410 RID: 13328 RVA: 0x0011E301 File Offset: 0x0011C501
	public void Setup(GhostReactor reactor, NetPlayer player)
	{
		this.reactor = reactor;
		this.player = player;
		this.RefreshUI();
	}

	// Token: 0x06003411 RID: 13329 RVA: 0x0011E318 File Offset: 0x0011C518
	public void RefreshUI()
	{
		if (this.playerName != null)
		{
			this.playerName.text = ((this.player == null) ? null : this.player.SanitizedNickName);
		}
		if (this.playerTitle != null)
		{
			GRPlayer grplayer = ((this.player == null) ? null : GRPlayer.Get(this.player.ActorNumber));
			if (grplayer != null)
			{
				this.playerTitle.text = GhostReactorProgression.GetTitleName(grplayer.CurrentProgression.redeemedPoints);
			}
			else
			{
				this.playerTitle.text = null;
			}
		}
		if (this.shuttle != null)
		{
			int targetFloor = this.shuttle.GetTargetFloor();
			if (this.destFloorText != null)
			{
				if (targetFloor == -1)
				{
					this.destFloorText.text = "HQ";
				}
				else
				{
					this.destFloorText.text = (targetFloor + 1).ToString();
				}
			}
			bool flag = targetFloor <= this.shuttle.GetMaxDropFloor();
			this.validScreen.SetActive(flag);
			this.invalidScreen.SetActive(!flag);
			if (flag)
			{
				this.infoText.text = "READY!\n\nDROP TO LEVEL";
				return;
			}
			this.infoText.text = "UNSAFE!\n\nUPGRADE DROP CHASSIS";
		}
	}

	// Token: 0x040043DC RID: 17372
	public TMP_Text playerName;

	// Token: 0x040043DD RID: 17373
	public TMP_Text playerTitle;

	// Token: 0x040043DE RID: 17374
	public TMP_Text destFloorText;

	// Token: 0x040043DF RID: 17375
	public TMP_Text infoText;

	// Token: 0x040043E0 RID: 17376
	public GameObject validScreen;

	// Token: 0x040043E1 RID: 17377
	public GameObject invalidScreen;

	// Token: 0x040043E2 RID: 17378
	public GRShuttle shuttle;

	// Token: 0x040043E3 RID: 17379
	private NetPlayer player;

	// Token: 0x040043E4 RID: 17380
	private GhostReactor reactor;
}
