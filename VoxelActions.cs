using System;
using PlayFab.Internal;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001F1 RID: 497
public class VoxelActions : SingletonMonoBehaviour<VoxelActions>
{
	// Token: 0x06000D03 RID: 3331 RVA: 0x000475D8 File Offset: 0x000457D8
	public void PlayDigFX(Vector3 position, Vector3 normal, int dirtAmount, int stoneAmount)
	{
		if (dirtAmount > 0)
		{
			Object.Instantiate<GameObject>((dirtAmount >= 20) ? this._dirtDigBigFX : this._dirtDigFX, position, Quaternion.LookRotation(normal));
		}
		if (stoneAmount > 0)
		{
			Object.Instantiate<GameObject>((dirtAmount >= 20) ? this._stoneDigBigFX : this._stoneDigFX, position, Quaternion.LookRotation(normal));
		}
	}

	// Token: 0x04000FB0 RID: 4016
	[SerializeField]
	private GameObject _hitFX;

	// Token: 0x04000FB1 RID: 4017
	[FormerlySerializedAs("_digFX")]
	[SerializeField]
	private GameObject _dirtDigFX;

	// Token: 0x04000FB2 RID: 4018
	[FormerlySerializedAs("_bigDigFX")]
	[SerializeField]
	private GameObject _dirtDigBigFX;

	// Token: 0x04000FB3 RID: 4019
	[SerializeField]
	private GameObject _stoneDigFX;

	// Token: 0x04000FB4 RID: 4020
	[SerializeField]
	private GameObject _stoneDigBigFX;
}
