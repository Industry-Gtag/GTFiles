using System;
using GorillaTag.Sports;
using UnityEngine;

// Token: 0x020009F5 RID: 2549
public class SportScoreboardVisuals : MonoBehaviour
{
	// Token: 0x0600416A RID: 16746 RVA: 0x0015C328 File Offset: 0x0015A528
	private void Awake()
	{
		SportScoreboard.Instance.RegisterTeamVisual(this.TeamIndex, this);
	}

	// Token: 0x04005216 RID: 21014
	[SerializeField]
	public MaterialUVOffsetListSetter score1s;

	// Token: 0x04005217 RID: 21015
	[SerializeField]
	public MaterialUVOffsetListSetter score10s;

	// Token: 0x04005218 RID: 21016
	[SerializeField]
	private int TeamIndex;
}
