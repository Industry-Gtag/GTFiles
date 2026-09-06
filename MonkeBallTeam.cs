using System;
using UnityEngine;

// Token: 0x0200060B RID: 1547
[Serializable]
public class MonkeBallTeam
{
	// Token: 0x04003207 RID: 12807
	public Color color;

	// Token: 0x04003208 RID: 12808
	public int score;

	// Token: 0x04003209 RID: 12809
	public Transform ballStartLocation;

	// Token: 0x0400320A RID: 12810
	public Transform ballLaunchPosition;

	// Token: 0x0400320B RID: 12811
	[Tooltip("The min/max random velocity of the ball when launched.")]
	public Vector2 ballLaunchVelocityRange = new Vector2(8f, 15f);

	// Token: 0x0400320C RID: 12812
	[Tooltip("The min/max random x-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleXRange = new Vector2(0f, 0f);

	// Token: 0x0400320D RID: 12813
	[Tooltip("The min/max random y-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleYRange = new Vector2(0f, 0f);
}
