using System;

namespace Voxels
{
	// Token: 0x0200137C RID: 4988
	public enum ChunkState
	{
		// Token: 0x04008FB0 RID: 36784
		UNINITIALIZED,
		// Token: 0x04008FB1 RID: 36785
		Created,
		// Token: 0x04008FB2 RID: 36786
		VoxelDataGenerated,
		// Token: 0x04008FB3 RID: 36787
		MeshDataGenerated,
		// Token: 0x04008FB4 RID: 36788
		MeshCreated,
		// Token: 0x04008FB5 RID: 36789
		CollisionBaked,
		// Token: 0x04008FB6 RID: 36790
		MeshAssigned
	}
}
