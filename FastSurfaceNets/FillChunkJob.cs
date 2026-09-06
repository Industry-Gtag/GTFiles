using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Voxels;

namespace FastSurfaceNets
{
	// Token: 0x02001379 RID: 4985
	internal struct FillChunkJob : IJobParallelFor
	{
		// Token: 0x06007CD7 RID: 31959 RVA: 0x0028D48C File Offset: 0x0028B68C
		public void Execute(int index)
		{
			int num = index / this.strideZ;
			int num2 = index % this.strideZ / this.strideY;
			int num3 = index % this.strideY;
			float3 @float = (this.chunkPosition + new int3(num3, num2, num)).ToFloat3();
			float num4 = noise.snoise(@float * this.noiseScale) - @float.y / this.heightScale;
			this.sdf[index] = num4.ToByte();
		}

		// Token: 0x04008F96 RID: 36758
		[WriteOnly]
		public NativeArray<byte> sdf;

		// Token: 0x04008F97 RID: 36759
		[ReadOnly]
		public int3 shape;

		// Token: 0x04008F98 RID: 36760
		[ReadOnly]
		public int3 chunkPosition;

		// Token: 0x04008F99 RID: 36761
		[ReadOnly]
		public int3 shapeMin;

		// Token: 0x04008F9A RID: 36762
		[ReadOnly]
		public int3 shapeMax;

		// Token: 0x04008F9B RID: 36763
		[ReadOnly]
		public float noiseScale;

		// Token: 0x04008F9C RID: 36764
		[ReadOnly]
		public float heightScale;

		// Token: 0x04008F9D RID: 36765
		[ReadOnly]
		public int3 min;

		// Token: 0x04008F9E RID: 36766
		[ReadOnly]
		public int3 max;

		// Token: 0x04008F9F RID: 36767
		[ReadOnly]
		public int strideY;

		// Token: 0x04008FA0 RID: 36768
		[ReadOnly]
		public int strideZ;
	}
}
