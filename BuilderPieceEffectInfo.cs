using System;
using UnityEngine;

// Token: 0x0200061B RID: 1563
[CreateAssetMenu(fileName = "BuilderPieceEffectInfo", menuName = "Gorilla Tag/Builder/EffectInfo", order = 0)]
public class BuilderPieceEffectInfo : ScriptableObject
{
	// Token: 0x04003297 RID: 12951
	public GameObject placeVFX;

	// Token: 0x04003298 RID: 12952
	public GameObject disconnectVFX;

	// Token: 0x04003299 RID: 12953
	public GameObject grabbedVFX;

	// Token: 0x0400329A RID: 12954
	public GameObject locationLockVFX;

	// Token: 0x0400329B RID: 12955
	public GameObject recycleVFX;

	// Token: 0x0400329C RID: 12956
	public GameObject tooHeavyVFX;
}
