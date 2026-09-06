using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200060A RID: 1546
public class MonkeBallBallResetTrigger : MonoBehaviour
{
	// Token: 0x06002686 RID: 9862 RVA: 0x000CBF78 File Offset: 0x000CA178
	private void OnTriggerEnter(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			GameBallPlayer gameBallPlayer = ((component.heldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(component.heldByActorNumber));
			if (gameBallPlayer == null)
			{
				gameBallPlayer = ((component.lastHeldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(component.lastHeldByActorNumber));
				if (gameBallPlayer == null)
				{
					return;
				}
			}
			this._lastBall = component;
			int num = gameBallPlayer.teamId;
			if (num == -1)
			{
				num = component.lastHeldByTeamId;
			}
			if (num >= 0 && num < this.teamMaterials.Length)
			{
				this.trigger.sharedMaterial = this.teamMaterials[num];
			}
			if (PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.ToggleResetButton(true, num);
			}
		}
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x000CC030 File Offset: 0x000CA230
	private void OnTriggerExit(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			if (component == this._lastBall)
			{
				this.trigger.sharedMaterial = this.neutralMaterial;
				this._lastBall = null;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.ToggleResetButton(false, -1);
			}
		}
	}

	// Token: 0x04003203 RID: 12803
	public Renderer trigger;

	// Token: 0x04003204 RID: 12804
	public Material[] teamMaterials;

	// Token: 0x04003205 RID: 12805
	public Material neutralMaterial;

	// Token: 0x04003206 RID: 12806
	private GameBall _lastBall;
}
