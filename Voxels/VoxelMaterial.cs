using System;
using Pooling;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020013C4 RID: 5060
	[Serializable]
	public struct VoxelMaterial
	{
		// Token: 0x0400911A RID: 37146
		public string name;

		// Token: 0x0400911B RID: 37147
		public Texture2D texture;

		// Token: 0x0400911C RID: 37148
		public int hardness;

		// Token: 0x0400911D RID: 37149
		public PoolableFX digFX;

		// Token: 0x0400911E RID: 37150
		public PoolableFX digBigFX;
	}
}
