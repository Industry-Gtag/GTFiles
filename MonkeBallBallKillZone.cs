using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000609 RID: 1545
public class MonkeBallBallKillZone : MonoBehaviour
{
	// Token: 0x06002684 RID: 9860 RVA: 0x000CBF28 File Offset: 0x000CA128
	private void OnTriggerEnter(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.RequestResetBall(component.id, -1);
				return;
			}
			GameBallManager.Instance.RequestSetBallPosition(component.id);
		}
	}
}
