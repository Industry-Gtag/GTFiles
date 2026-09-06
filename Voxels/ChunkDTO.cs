using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x0200137F RID: 4991
	public struct ChunkDTO
	{
		// Token: 0x17000C36 RID: 3126
		// (get) Token: 0x06007CFF RID: 31999 RVA: 0x0028DE85 File Offset: 0x0028C085
		public bool IsValid
		{
			get
			{
				return !this.Size.Equals(int3.zero) && this.Density.IsCreated && this.Material.IsCreated;
			}
		}

		// Token: 0x06007D00 RID: 32000 RVA: 0x0028DEB4 File Offset: 0x0028C0B4
		public ChunkDTO(Chunk chunk)
		{
			this.WorldId = chunk.World.Id;
			this.Id = chunk.Id;
			this.Size = chunk.Size;
			this.Dimensions = chunk.Dimensions;
			this.Density = chunk.Density;
			this.Material = chunk.Material;
		}

		// Token: 0x04008FD5 RID: 36821
		public int WorldId;

		// Token: 0x04008FD6 RID: 36822
		public int3 Id;

		// Token: 0x04008FD7 RID: 36823
		public int3 Size;

		// Token: 0x04008FD8 RID: 36824
		public int3 Dimensions;

		// Token: 0x04008FD9 RID: 36825
		public NativeArray<byte> Density;

		// Token: 0x04008FDA RID: 36826
		public NativeArray<byte> Material;
	}
}
