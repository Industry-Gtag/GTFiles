using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x020013A2 RID: 5026
	public struct SurfaceNetsBuffer : IDisposable
	{
		// Token: 0x06007D66 RID: 32102 RVA: 0x00290F84 File Offset: 0x0028F184
		public SurfaceNetsBuffer(int vertexCap, int indexCap, int strideCount, Allocator alloc = Allocator.TempJob)
		{
			this.Vertices = new NativeList<float3>(vertexCap, alloc);
			this.Normals = new NativeList<float3>(vertexCap, alloc);
			this.Materials = new NativeList<byte>(vertexCap, alloc);
			this.Triangles = new NativeList<int>(indexCap, alloc);
			this.SurfacePoints = new NativeList<int3>(vertexCap, alloc);
			this.SurfaceStrides = new NativeList<int>(vertexCap, alloc);
			this.StrideToIndex = new NativeArray<int>(strideCount, alloc, NativeArrayOptions.UninitializedMemory);
			this.Reset(strideCount);
		}

		// Token: 0x06007D67 RID: 32103 RVA: 0x0029101C File Offset: 0x0028F21C
		public void Reset(int strideCount)
		{
			this.Vertices.Clear();
			this.Normals.Clear();
			this.Triangles.Clear();
			this.SurfacePoints.Clear();
			this.SurfaceStrides.Clear();
			if (this.StrideToIndex.Length < strideCount)
			{
				this.StrideToIndex.Dispose();
			}
			if (!this.StrideToIndex.IsCreated || this.StrideToIndex.Length != strideCount)
			{
				this.StrideToIndex = new NativeArray<int>(strideCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			}
			for (int i = 0; i < strideCount; i++)
			{
				this.StrideToIndex[i] = int.MaxValue;
			}
		}

		// Token: 0x06007D68 RID: 32104 RVA: 0x002910C0 File Offset: 0x0028F2C0
		public void Dispose()
		{
			this.Vertices.Dispose();
			this.Normals.Dispose();
			this.Materials.Dispose();
			this.Triangles.Dispose();
			this.SurfacePoints.Dispose();
			this.SurfaceStrides.Dispose();
			this.StrideToIndex.Dispose();
		}

		// Token: 0x0400906A RID: 36970
		public NativeList<float3> Vertices;

		// Token: 0x0400906B RID: 36971
		public NativeList<float3> Normals;

		// Token: 0x0400906C RID: 36972
		public NativeList<byte> Materials;

		// Token: 0x0400906D RID: 36973
		public NativeList<int> Triangles;

		// Token: 0x0400906E RID: 36974
		public NativeList<int3> SurfacePoints;

		// Token: 0x0400906F RID: 36975
		public NativeList<int> SurfaceStrides;

		// Token: 0x04009070 RID: 36976
		public NativeArray<int> StrideToIndex;

		// Token: 0x04009071 RID: 36977
		public const int NullVertex = 2147483647;
	}
}
