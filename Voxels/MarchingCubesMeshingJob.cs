using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x02001387 RID: 4999
	[BurstCompile]
	public struct MarchingCubesMeshingJob : IJob
	{
		// Token: 0x06007D2D RID: 32045 RVA: 0x0028EB74 File Offset: 0x0028CD74
		public void Execute()
		{
			this.dimension = this.chunkSize + 1;
			for (int i = 0; i < this.chunkSize; i++)
			{
				for (int j = 0; j < this.chunkSize; j++)
				{
					for (int k = 0; k < this.chunkSize; k++)
					{
						this.ProcessCube(i, j, k);
					}
				}
			}
		}

		// Token: 0x06007D2E RID: 32046 RVA: 0x0028EBCC File Offset: 0x0028CDCC
		private void ProcessCube(int x, int y, int z)
		{
			int3 @int = new int3(x, y, z);
			this.GetMaterialValue(@int);
			NativeArray<byte> nativeArray = new NativeArray<byte>(8, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < 8; i++)
			{
				int3 int2 = new int3(x, y, z) + (int3)MarchingCubesLookup.CornerOffsets[i];
				nativeArray[i] = this.GetVoxelValue(int2);
			}
			int num = 0;
			for (int j = 0; j < 8; j++)
			{
				if (nativeArray[j] < this.isoLevel)
				{
					num |= 1 << j;
				}
			}
			if (num == 0 || num == 255)
			{
				nativeArray.Dispose();
				return;
			}
			NativeArray<float3> nativeArray2 = new NativeArray<float3>(12, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<float> nativeArray3 = new NativeArray<float>(12, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int k = 0; k < 12; k++)
			{
				if ((MarchingCubesLookup.EdgeTable[num] & (1 << k)) != 0)
				{
					int2 int3 = MarchingCubesLookup.EdgeVertices[k];
					float3 @float = new float3((float)x, (float)y, (float)z) + MarchingCubesLookup.CornerOffsets[int3.x];
					float3 float2 = new float3((float)x, (float)y, (float)z) + MarchingCubesLookup.CornerOffsets[int3.y];
					float num2 = (float)(this.isoLevel - nativeArray[int3.x]) / (float)(nativeArray[int3.y] - nativeArray[int3.x]);
					nativeArray2[k] = math.lerp(@float, float2, num2);
					int materialValue = (int)this.GetMaterialValue((@int + MarchingCubesLookup.CornerOffsets[int3.x]).ToInt3());
					byte materialValue2 = this.GetMaterialValue((@int + MarchingCubesLookup.CornerOffsets[int3.y]).ToInt3());
					int num3 = math.max(materialValue, (int)materialValue2);
					nativeArray3[k] = (float)num3;
				}
			}
			int num4 = 0;
			while (num4 < 16 && MarchingCubesLookup.TriTable[num * 16 + num4] != -1)
			{
				float3 float3 = nativeArray2[MarchingCubesLookup.TriTable[num * 16 + num4]];
				float3 float4 = nativeArray2[MarchingCubesLookup.TriTable[num * 16 + num4 + 1]];
				float3 float5 = nativeArray2[MarchingCubesLookup.TriTable[num * 16 + num4 + 2]];
				float num5 = nativeArray3[MarchingCubesLookup.TriTable[num * 16 + num4]];
				float num6 = nativeArray3[MarchingCubesLookup.TriTable[num * 16 + num4 + 1]];
				float num7 = nativeArray3[MarchingCubesLookup.TriTable[num * 16 + num4 + 2]];
				float4 float6 = new float4(num5, num6, num7, 0f);
				if (!float3.Equals(float4) && !float3.Equals(float5) && !float4.Equals(float5))
				{
					float3 float7 = math.normalize(math.cross(float4 - float3, float5 - float3));
					float3 float8 = math.normalize(math.cross((math.abs(float7.y) < 0.999f) ? new float3(0f, 1f, 0f) : new float3(1f, 0f, 0f), float7));
					float4 float9 = new float4(float8, 1f);
					int num8 = this.triangleCounter.Increment() * 3;
					this.vertexData[num8] = new MeshVertexData(float3, float7, float9, float6, new float4(1f, 0f, 0f, 0f));
					this.triangleData[num8] = (ushort)num8;
					this.vertexData[num8 + 1] = new MeshVertexData(float4, float7, float9, float6, new float4(0f, 1f, 0f, 0f));
					this.triangleData[num8 + 1] = (ushort)(num8 + 1);
					this.vertexData[num8 + 2] = new MeshVertexData(float5, float7, float9, float6, new float4(0f, 0f, 1f, 0f));
					this.triangleData[num8 + 2] = (ushort)(num8 + 2);
				}
				num4 += 3;
			}
			nativeArray.Dispose();
			nativeArray2.Dispose();
			nativeArray3.Dispose();
		}

		// Token: 0x06007D2F RID: 32047 RVA: 0x0028F01A File Offset: 0x0028D21A
		private byte GetMaterialValue(int3 pos)
		{
			return this.materials[pos.x + this.dimension * (pos.y + pos.z * this.dimension)];
		}

		// Token: 0x06007D30 RID: 32048 RVA: 0x0028F04C File Offset: 0x0028D24C
		private byte GetVoxelValue(int3 pos)
		{
			if (pos.x < 0 || pos.y < 0 || pos.z < 0 || pos.x > this.chunkSize || pos.y > this.chunkSize || pos.z > this.chunkSize)
			{
				return 0;
			}
			int num = pos.x + this.dimension * (pos.y + pos.z * this.dimension);
			return this.voxels[num];
		}

		// Token: 0x04008FED RID: 36845
		[ReadOnly]
		public NativeArray<byte> voxels;

		// Token: 0x04008FEE RID: 36846
		[ReadOnly]
		public NativeArray<byte> materials;

		// Token: 0x04008FEF RID: 36847
		[ReadOnly]
		public int chunkSize;

		// Token: 0x04008FF0 RID: 36848
		[ReadOnly]
		public byte isoLevel;

		// Token: 0x04008FF1 RID: 36849
		public NativeCounter triangleCounter;

		// Token: 0x04008FF2 RID: 36850
		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<MeshVertexData> vertexData;

		// Token: 0x04008FF3 RID: 36851
		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<ushort> triangleData;

		// Token: 0x04008FF4 RID: 36852
		private int dimension;
	}
}
