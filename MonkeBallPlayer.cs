using System;
using UnityEngine;

// Token: 0x02000610 RID: 1552
public class MonkeBallPlayer : MonoBehaviour
{
	// Token: 0x060026D0 RID: 9936 RVA: 0x000CDC27 File Offset: 0x000CBE27
	private void Awake()
	{
		if (this.gamePlayer == null)
		{
			this.gamePlayer = base.GetComponent<GameBallPlayer>();
		}
	}

	// Token: 0x0400323F RID: 12863
	public GameBallPlayer gamePlayer;

	// Token: 0x04003240 RID: 12864
	public MonkeBallGoalZone currGoalZone;
}
