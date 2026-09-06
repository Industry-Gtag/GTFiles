using System;
using UnityEngine;

// Token: 0x02000616 RID: 1558
public class MonkeBallTeamZoneSelector : MonoBehaviour
{
	// Token: 0x060026ED RID: 9965 RVA: 0x000CE020 File Offset: 0x000CC220
	private void OnTriggerEnter(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.IsLocalPlayer() && gamePlayer.teamId != this.teamId)
		{
			MonkeBallGame.Instance.RequestSetTeam(this.teamId);
		}
	}

	// Token: 0x04003262 RID: 12898
	public int teamId;
}
