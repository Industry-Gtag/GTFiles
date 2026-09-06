using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x0200138A RID: 5002
	[Serializable]
	public class PerlinVoxelGenerator : VoxelGenerator
	{
		// Token: 0x06007D39 RID: 32057 RVA: 0x0028F1DC File Offset: 0x0028D3DC
		public override ChunkTask CreateVoxelDataJob(Chunk chunk)
		{
			if (!chunk.Density.IsCreated)
			{
				chunk.Density = new NativeArray<byte>(chunk.VoxelCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			if (!chunk.Material.IsCreated)
			{
				chunk.Material = new NativeArray<byte>(chunk.VoxelCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			JobHandle jobHandle = new PerlinVoxelGenerator.VoxelDataJob
			{
				chunkPosition = chunk.Id,
				chunkSize = chunk.Size.x,
				dimension = chunk.Dimensions.x,
				noiseScale = this.noiseParameters.NoiseScale,
				groundLevel = this.noiseParameters.GroundLevel,
				heightCompensation = this.noiseParameters.HeightCompensation,
				octaves = this.noiseParameters.Octaves,
				persistence = this.noiseParameters.Persistence,
				heightScale = this.noiseParameters.HeightScale,
				seed = this.Seed,
				voxels = chunk.Density,
				materials = chunk.Material
			}.Schedule(chunk.VoxelCount, 64, default(JobHandle));
			Action action = delegate
			{
				chunk.IsDataGenerated = true;
				chunk.IsMeshGenerated = false;
				chunk.IsDirty = true;
			};
			return new ChunkTask(chunk, jobHandle, action);
		}

		// Token: 0x04008FFD RID: 36861
		public PerlinVoxelGenerator.NoiseParameters noiseParameters = new PerlinVoxelGenerator.NoiseParameters
		{
			NoiseScale = 0.01f,
			GroundLevel = 0f,
			HeightScale = 0.01f,
			Octaves = 4,
			Persistence = 0.5f
		};

		// Token: 0x0200138B RID: 5003
		[Serializable]
		public struct NoiseParameters
		{
			// Token: 0x04008FFE RID: 36862
			public float NoiseScale;

			// Token: 0x04008FFF RID: 36863
			public float GroundLevel;

			// Token: 0x04009000 RID: 36864
			public float HeightScale;

			// Token: 0x04009001 RID: 36865
			public float HeightCompensation;

			// Token: 0x04009002 RID: 36866
			public int Octaves;

			// Token: 0x04009003 RID: 36867
			public float Persistence;
		}

		// Token: 0x0200138C RID: 5004
		[BurstCompile]
		public struct VoxelDataJob : IJobParallelFor
		{
			// Token: 0x06007D3B RID: 32059 RVA: 0x0028F3D0 File Offset: 0x0028D5D0
			public void Execute(int index)
			{
				int num = index % this.dimension;
				int num2 = index / this.dimension % this.dimension;
				int num3 = index / (this.dimension * this.dimension);
				float3 @float = new float3((float)(this.chunkPosition.x * this.chunkSize + num), (float)(this.chunkPosition.y * this.chunkSize + num2), (float)(this.chunkPosition.z * this.chunkSize + num3));
				float3 float2 = new float3((float)this.seed * 1.7f, (float)this.seed * 2.3f, (float)this.seed * 3.1f);
				float3 float3 = @float + float2;
				float num4 = noise.snoise((new float3(@float.x, 0f, @float.z) + float2) * this.noiseScale) + (this.groundLevel - @float.y) / this.heightScale;
				num4 = math.clamp(num4 * this.heightCompensation, -1f, 1f);
				float num5 = this.noiseScale;
				float num6 = 1f;
				for (int i = 0; i < this.octaves; i++)
				{
					num5 *= 2f;
					num6 *= this.persistence;
					num4 += noise.snoise(float3 * num5) * num6;
				}
				if (noise.snoise(float3 * 0.05f) > 0.6f && num4 >= 0f)
				{
					this.materials[index] = 1;
				}
				this.voxels[index] = num4.ToByte();
			}

			// Token: 0x04009004 RID: 36868
			public int3 chunkPosition;

			// Token: 0x04009005 RID: 36869
			public int chunkSize;

			// Token: 0x04009006 RID: 36870
			public int dimension;

			// Token: 0x04009007 RID: 36871
			public float noiseScale;

			// Token: 0x04009008 RID: 36872
			public float groundLevel;

			// Token: 0x04009009 RID: 36873
			public float heightScale;

			// Token: 0x0400900A RID: 36874
			public float heightCompensation;

			// Token: 0x0400900B RID: 36875
			public int octaves;

			// Token: 0x0400900C RID: 36876
			public float persistence;

			// Token: 0x0400900D RID: 36877
			public int seed;

			// Token: 0x0400900E RID: 36878
			[WriteOnly]
			public NativeArray<byte> voxels;

			// Token: 0x0400900F RID: 36879
			[WriteOnly]
			public NativeArray<byte> materials;
		}
	}
}
