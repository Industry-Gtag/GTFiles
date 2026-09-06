using System;
using UnityEngine;

// Token: 0x0200063F RID: 1599
[Serializable]
public struct PieceFallbackInfo
{
	// Token: 0x0400337D RID: 13181
	[Tooltip("Check if the piece has Material Options set and the default material is in a starter set")]
	public bool materialSwapThisPrefab;

	// Token: 0x0400337E RID: 13182
	[Tooltip("A piece in a starter set with the same builder attach grid configuration\n(check BuilderSetManager _starterPieceSets for pieces in starter sets)")]
	public BuilderPiece prefab;
}
