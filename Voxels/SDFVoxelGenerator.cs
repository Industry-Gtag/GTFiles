using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x0200138E RID: 5006
	public class SDFVoxelGenerator : VoxelGenerator
	{
		// Token: 0x06007D3E RID: 32062 RVA: 0x0028F598 File Offset: 0x0028D798
		public override ChunkTask CreateVoxelDataJob(Chunk chunk)
		{
			NativeArray<SDFVoxelGenerator.SDFPrimitive> opBuffer = new NativeArray<SDFVoxelGenerator.SDFPrimitive>(this.Primitives, Allocator.TempJob);
			if (!chunk.Density.IsCreated)
			{
				chunk.Density = new NativeArray<byte>(chunk.VoxelCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			if (!chunk.Material.IsCreated)
			{
				chunk.Material = new NativeArray<byte>(chunk.VoxelCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			JobHandle jobHandle = new SDFVoxelGenerator.VoxelDataJob
			{
				chunkPosition = chunk.Id,
				chunkSize = chunk.Size.x,
				dimension = chunk.Dimensions.x,
				blocky = this.Blocky,
				seed = this.Seed,
				voxels = chunk.Density,
				materials = chunk.Material,
				fill = this.fill,
				operations = opBuffer,
				heightScale = this.NoiseScale,
				noiseScale = this.Frequency,
				octaves = this.Octaves,
				persistence = this.Persistence
			}.Schedule(chunk.VoxelCount, 64, default(JobHandle));
			Action action = delegate
			{
				chunk.IsDataGenerated = true;
				chunk.IsMeshGenerated = false;
				chunk.IsDirty = true;
				opBuffer.Dispose();
			};
			return new ChunkTask(chunk, jobHandle, action);
		}

		// Token: 0x06007D3F RID: 32063 RVA: 0x0028F730 File Offset: 0x0028D930
		public override void DrawGizmos(VoxelWorld world)
		{
			Transform root = world.Root;
			Gizmos.matrix = Matrix4x4.TRS(root.position, root.rotation, root.lossyScale * world.Scale);
			foreach (SDFVoxelGenerator.SDFPrimitive sdfprimitive in this.Primitives)
			{
				Gizmos.color = ((sdfprimitive.Operation == SDFVoxelGenerator.Operation.Add) ? Color.green : Color.red);
				SDFVoxelGenerator.Shape shape = sdfprimitive.Shape;
				if (shape != SDFVoxelGenerator.Shape.Sphere)
				{
					if (shape != SDFVoxelGenerator.Shape.Cube)
					{
						throw new ArgumentOutOfRangeException();
					}
					Gizmos.DrawWireCube(sdfprimitive.Position, sdfprimitive.Size);
					Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.1f);
					Gizmos.DrawCube(sdfprimitive.Position, sdfprimitive.Size);
				}
				else
				{
					Gizmos.DrawWireSphere(sdfprimitive.Position, sdfprimitive.Radius);
				}
			}
		}

		// Token: 0x06007D40 RID: 32064 RVA: 0x0028F840 File Offset: 0x0028DA40
		public override global::UnityEngine.BoundsInt GetWorldBounds()
		{
			if (this.Primitives.Length < 1)
			{
				return default(global::UnityEngine.BoundsInt);
			}
			Bounds bounds = this.Primitives[0].GetBounds();
			foreach (SDFVoxelGenerator.SDFPrimitive sdfprimitive in this.Primitives)
			{
				bounds.Encapsulate(sdfprimitive.GetBounds());
			}
			bounds.size += Vector3.one * (2f + math.max(this.NoiseScale * 2f, 0f));
			return new global::UnityEngine.BoundsInt(bounds.min.FloorToVectorInt(), bounds.size.CeilToVectorInt());
		}

		// Token: 0x06007D41 RID: 32065 RVA: 0x0028F8F8 File Offset: 0x0028DAF8
		public override void ShiftWorldBounds(Vector3Int worldShift)
		{
			float3 @float = new float3((float)worldShift.x, (float)worldShift.y, (float)worldShift.z);
			for (int i = 0; i < this.Primitives.Length; i++)
			{
				SDFVoxelGenerator.SDFPrimitive[] primitives = this.Primitives;
				int num = i;
				primitives[num].Position = primitives[num].Position + @float;
			}
		}

		// Token: 0x06007D42 RID: 32066 RVA: 0x0028F959 File Offset: 0x0028DB59
		public override void InitWorld(VoxelWorld world)
		{
			world.SetWorldBounds(this.GetWorldBounds());
		}

		// Token: 0x06007D43 RID: 32067 RVA: 0x0028F968 File Offset: 0x0028DB68
		public void SetPrimitive(global::UnityEngine.BoundsInt worldBounds, byte material = 0)
		{
			Vector3 vector = (worldBounds.min + worldBounds.max) / 2f;
			this.Primitives = new SDFVoxelGenerator.SDFPrimitive[1];
			this.Primitives[0] = new SDFVoxelGenerator.SDFPrimitive
			{
				Operation = SDFVoxelGenerator.Operation.Add,
				Shape = SDFVoxelGenerator.Shape.Cube,
				Position = vector,
				Size = worldBounds.size,
				Material = material
			};
		}

		// Token: 0x04009011 RID: 36881
		public byte fill;

		// Token: 0x04009012 RID: 36882
		public bool Blocky;

		// Token: 0x04009013 RID: 36883
		[Header("Noise")]
		public float NoiseScale;

		// Token: 0x04009014 RID: 36884
		public float Frequency = 0.1f;

		// Token: 0x04009015 RID: 36885
		public int Octaves = 1;

		// Token: 0x04009016 RID: 36886
		public float Persistence = 0.5f;

		// Token: 0x04009017 RID: 36887
		public SDFVoxelGenerator.SDFPrimitive[] Primitives;

		// Token: 0x0200138F RID: 5007
		public enum Operation
		{
			// Token: 0x04009019 RID: 36889
			Add,
			// Token: 0x0400901A RID: 36890
			Subtract
		}

		// Token: 0x02001390 RID: 5008
		public enum Shape
		{
			// Token: 0x0400901C RID: 36892
			Sphere,
			// Token: 0x0400901D RID: 36893
			Cube
		}

		// Token: 0x02001391 RID: 5009
		[Serializable]
		public struct SDFPrimitive
		{
			// Token: 0x17000C3D RID: 3133
			// (get) Token: 0x06007D45 RID: 32069 RVA: 0x0028FA20 File Offset: 0x0028DC20
			private bool ShowRadius
			{
				get
				{
					return this.Shape == SDFVoxelGenerator.Shape.Sphere;
				}
			}

			// Token: 0x17000C3E RID: 3134
			// (get) Token: 0x06007D46 RID: 32070 RVA: 0x0028FA2B File Offset: 0x0028DC2B
			private bool ShowSize
			{
				get
				{
					return !this.ShowRadius;
				}
			}

			// Token: 0x06007D47 RID: 32071 RVA: 0x0028FA38 File Offset: 0x0028DC38
			public Bounds GetBounds()
			{
				SDFVoxelGenerator.Shape shape = this.Shape;
				Vector3 vector;
				if (shape != SDFVoxelGenerator.Shape.Sphere)
				{
					if (shape != SDFVoxelGenerator.Shape.Cube)
					{
						vector = Vector3.zero;
					}
					else
					{
						vector = this.Size;
					}
				}
				else
				{
					vector = new Vector3(this.Radius * 2f, this.Radius * 2f, this.Radius * 2f);
				}
				Vector3 vector2 = vector;
				return new Bounds(this.Position, vector2);
			}

			// Token: 0x0400901E RID: 36894
			public SDFVoxelGenerator.Operation Operation;

			// Token: 0x0400901F RID: 36895
			public SDFVoxelGenerator.Shape Shape;

			// Token: 0x04009020 RID: 36896
			public float3 Position;

			// Token: 0x04009021 RID: 36897
			public float Radius;

			// Token: 0x04009022 RID: 36898
			public float3 Size;

			// Token: 0x04009023 RID: 36899
			public byte Material;
		}

		// Token: 0x02001392 RID: 5010
		[BurstCompile]
		public struct VoxelDataJob : IJobParallelFor
		{
			// Token: 0x06007D48 RID: 32072 RVA: 0x0028FAA8 File Offset: 0x0028DCA8
			public void Execute(int index)
			{
				int num = index % this.dimension;
				int num2 = index / this.dimension % this.dimension;
				int num3 = index / (this.dimension * this.dimension);
				float3 @float = new float3((float)(this.chunkPosition.x * this.chunkSize + num), (float)(this.chunkPosition.y * this.chunkSize + num2), (float)(this.chunkPosition.z * this.chunkSize + num3));
				byte b = this.fill;
				float num4 = -1f;
				float num5 = float.MaxValue;
				float num6 = float.MaxValue;
				byte material = this.fill;
				SDFVoxelGenerator.SDFPrimitive sdfprimitive = default(SDFVoxelGenerator.SDFPrimitive);
				foreach (SDFVoxelGenerator.SDFPrimitive sdfprimitive2 in this.operations)
				{
					float distance = SDFVoxelGenerator.VoxelDataJob.GetDistance(sdfprimitive2, @float);
					if ((sdfprimitive2.Operation != SDFVoxelGenerator.Operation.Subtract || (num5 <= 0f && distance <= 0f)) && (distance <= 0f || distance < num5))
					{
						sdfprimitive = sdfprimitive2;
						num5 = distance;
						if (sdfprimitive2.Operation == SDFVoxelGenerator.Operation.Add)
						{
							num6 = num5;
							material = sdfprimitive2.Material;
						}
					}
				}
				if (num5 < 3.4028235E+38f)
				{
					SDFVoxelGenerator.Operation operation = sdfprimitive.Operation;
					if (operation != SDFVoxelGenerator.Operation.Add)
					{
						if (operation != SDFVoxelGenerator.Operation.Subtract)
						{
							throw new ArgumentOutOfRangeException();
						}
						num4 = num5;
					}
					else
					{
						num4 = -num5;
					}
					if (this.heightScale > 0f)
					{
						float3 float2 = new float3((float)this.seed * 1.7f, (float)this.seed * 2.3f, (float)this.seed * 3.1f);
						float3 float3 = @float + float2;
						float num7 = noise.snoise(float3 * this.noiseScale) * this.heightScale;
						float num8 = this.noiseScale;
						float num9 = 1f;
						for (int i = 0; i < this.octaves; i++)
						{
							num8 *= 2f;
							num9 *= this.persistence;
							num7 += noise.snoise(float3 * num8) * num9 * this.heightScale;
						}
						num4 += num7;
					}
					if (num4 > -1.75f && num6 < 3.4028235E+38f)
					{
						b = material;
					}
				}
				this.materials[index] = b;
				byte b2 = num4.ToByte();
				if (this.blocky)
				{
					b2 = (b2.IsSolid() ? byte.MaxValue : 0);
				}
				this.voxels[index] = b2;
			}

			// Token: 0x06007D49 RID: 32073 RVA: 0x0028FD30 File Offset: 0x0028DF30
			private static float GetDistance(SDFVoxelGenerator.SDFPrimitive primitive, float3 position)
			{
				SDFVoxelGenerator.Shape shape = primitive.Shape;
				if (shape == SDFVoxelGenerator.Shape.Sphere)
				{
					return math.distance(primitive.Position, position) - primitive.Radius;
				}
				if (shape != SDFVoxelGenerator.Shape.Cube)
				{
					return float.MaxValue;
				}
				float3 @float = primitive.Size * 0.5f;
				float3 float2 = math.abs(position - primitive.Position) - @float;
				float num = math.length(math.max(float2, 0f));
				float num2 = math.min(math.max(float2.x, math.max(float2.y, float2.z)), 0f);
				return num + num2;
			}

			// Token: 0x04009024 RID: 36900
			public int3 chunkPosition;

			// Token: 0x04009025 RID: 36901
			public int chunkSize;

			// Token: 0x04009026 RID: 36902
			public int dimension;

			// Token: 0x04009027 RID: 36903
			public bool blocky;

			// Token: 0x04009028 RID: 36904
			public float noiseScale;

			// Token: 0x04009029 RID: 36905
			public float heightScale;

			// Token: 0x0400902A RID: 36906
			public int octaves;

			// Token: 0x0400902B RID: 36907
			public float persistence;

			// Token: 0x0400902C RID: 36908
			public int seed;

			// Token: 0x0400902D RID: 36909
			[WriteOnly]
			public NativeArray<byte> voxels;

			// Token: 0x0400902E RID: 36910
			[WriteOnly]
			public NativeArray<byte> materials;

			// Token: 0x0400902F RID: 36911
			public byte fill;

			// Token: 0x04009030 RID: 36912
			[ReadOnly]
			public NativeArray<SDFVoxelGenerator.SDFPrimitive> operations;
		}
	}
}
