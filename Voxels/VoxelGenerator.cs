using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020013AA RID: 5034
	[Serializable]
	public abstract class VoxelGenerator
	{
		// Token: 0x17000C41 RID: 3137
		// (get) Token: 0x06007D99 RID: 32153 RVA: 0x00292556 File Offset: 0x00290756
		public bool PostProcessMesh
		{
			get
			{
				return this.meshParameters.MeshGenerationMode == MeshGenerationMode.SurfaceNets;
			}
		}

		// Token: 0x06007D9A RID: 32154 RVA: 0x00002C2D File Offset: 0x00000E2D
		public virtual void DrawGizmos(VoxelWorld world)
		{
		}

		// Token: 0x06007D9B RID: 32155 RVA: 0x00292568 File Offset: 0x00290768
		public virtual global::UnityEngine.BoundsInt GetWorldBounds()
		{
			return default(global::UnityEngine.BoundsInt);
		}

		// Token: 0x06007D9C RID: 32156 RVA: 0x00002C2D File Offset: 0x00000E2D
		public virtual void ShiftWorldBounds(Vector3Int worldShift)
		{
		}

		// Token: 0x06007D9D RID: 32157 RVA: 0x00002C2D File Offset: 0x00000E2D
		public virtual void InitWorld(VoxelWorld world)
		{
		}

		// Token: 0x06007D9E RID: 32158
		public abstract ChunkTask CreateVoxelDataJob(Chunk chunk);

		// Token: 0x06007D9F RID: 32159 RVA: 0x00292580 File Offset: 0x00290780
		public ChunkTask CreateMeshDataJob(Chunk chunk)
		{
			VoxelGenerator.<>c__DisplayClass10_0 CS$<>8__locals1 = new VoxelGenerator.<>c__DisplayClass10_0();
			CS$<>8__locals1.chunk = chunk;
			CS$<>8__locals1.triangleCounter = new NativeCounter(Allocator.TempJob);
			CS$<>8__locals1.onComplete = null;
			MeshGenerationMode meshGenerationMode = this.meshParameters.MeshGenerationMode;
			JobHandle jobHandle;
			if (meshGenerationMode != MeshGenerationMode.MarchingCubes)
			{
				if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
				{
					throw new ArgumentOutOfRangeException();
				}
				jobHandle = CS$<>8__locals1.<CreateMeshDataJob>g__CreateSurfaceNetsMeshJob|1();
			}
			else
			{
				jobHandle = CS$<>8__locals1.<CreateMeshDataJob>g__CreateMarchingCubesMeshJob|0();
			}
			JobHandle jobHandle2 = jobHandle;
			return new ChunkTask(CS$<>8__locals1.chunk, jobHandle2, CS$<>8__locals1.onComplete);
		}

		// Token: 0x06007DA0 RID: 32160 RVA: 0x002925F0 File Offset: 0x002907F0
		public ChunkTask CreateMeshPostProcessJob(Chunk chunk)
		{
			if (!this.PostProcessMesh)
			{
				throw new InvalidOperationException(string.Format("{0} does not use Mesh Post-Processing.", this));
			}
			object genericMeshData = chunk.GenericMeshData;
			if (!(genericMeshData is SurfaceNetsBuffer))
			{
				throw new InvalidOperationException(string.Format("{0} GenericMeshData is not a SurfaceNetsBuffer.", chunk));
			}
			SurfaceNetsBuffer surfaceNetsBuffer = (SurfaceNetsBuffer)genericMeshData;
			if (surfaceNetsBuffer.Triangles.Length < 3)
			{
				chunk.IsMeshGenerated = true;
				chunk.IsCollisionBaked = false;
				chunk.IsDirty = true;
				chunk.VertexCount = 0;
				return default(ChunkTask);
			}
			NativeCounter triangleCounter = new NativeCounter(Allocator.TempJob);
			int num = math.min(chunk.VoxelCount * 15, 65535);
			if (!chunk.VertexData.IsCreated)
			{
				chunk.VertexData = new NativeArray<MeshVertexData>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			if (!chunk.TriangleData.IsCreated)
			{
				chunk.TriangleData = new NativeArray<ushort>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
			MeshUtilities.VoxelMeshData voxelMeshData = MeshUtilities.SplitByAngle(surfaceNetsBuffer.Vertices.AsArray(), surfaceNetsBuffer.Materials.AsArray(), surfaceNetsBuffer.Triangles.AsArray(), this.meshParameters.NormalThreshold, this.meshParameters.AreaWeightedNormals, Allocator.TempJob);
			ref NativeList<float3> ptr = ref surfaceNetsBuffer.Vertices;
			NativeList<float3> nativeList = voxelMeshData.Vertices;
			NativeList<float3> nativeList2 = surfaceNetsBuffer.Vertices;
			ptr = nativeList;
			voxelMeshData.Vertices = nativeList2;
			ref NativeList<byte> ptr2 = ref surfaceNetsBuffer.Materials;
			NativeList<byte> materials = voxelMeshData.Materials;
			NativeList<byte> materials2 = surfaceNetsBuffer.Materials;
			ptr2 = materials;
			voxelMeshData.Materials = materials2;
			ptr = ref surfaceNetsBuffer.Normals;
			nativeList2 = voxelMeshData.Normals;
			nativeList = surfaceNetsBuffer.Normals;
			ptr = nativeList2;
			voxelMeshData.Normals = nativeList;
			ref NativeList<int> ptr3 = ref surfaceNetsBuffer.Triangles;
			NativeList<int> triangles = voxelMeshData.Triangles;
			NativeList<int> triangles2 = surfaceNetsBuffer.Triangles;
			ptr3 = triangles;
			voxelMeshData.Triangles = triangles2;
			chunk.GenericMeshData = surfaceNetsBuffer;
			voxelMeshData.Dispose();
			JobHandle jobHandle = new AssembleVertexDataJob
			{
				vertexData = chunk.VertexData,
				triangleData = chunk.TriangleData,
				triangleCounter = triangleCounter,
				srcVerts = surfaceNetsBuffer.Vertices.AsArray(),
				srcMats = surfaceNetsBuffer.Materials.AsArray(),
				srcNorm = surfaceNetsBuffer.Normals.AsArray(),
				srcTris = surfaceNetsBuffer.Triangles.AsArray()
			}.Schedule(default(JobHandle));
			Action action = delegate
			{
				chunk.IsMeshGenerated = true;
				chunk.IsCollisionBaked = false;
				chunk.IsDirty = true;
				chunk.VertexCount = triangleCounter.Count * 3;
				triangleCounter.Dispose();
			};
			return new ChunkTask(chunk, jobHandle, action);
		}

		// Token: 0x0400908E RID: 37006
		public int Seed = 12345;

		// Token: 0x0400908F RID: 37007
		public VoxelGenerator.MeshingParameters meshParameters = new VoxelGenerator.MeshingParameters
		{
			MeshGenerationMode = MeshGenerationMode.MarchingCubes,
			NormalThreshold = 60f,
			AreaWeightedNormals = true
		};

		// Token: 0x020013AB RID: 5035
		[Serializable]
		public struct MeshingParameters
		{
			// Token: 0x04009090 RID: 37008
			public MeshGenerationMode MeshGenerationMode;

			// Token: 0x04009091 RID: 37009
			public float NormalThreshold;

			// Token: 0x04009092 RID: 37010
			public bool AreaWeightedNormals;
		}
	}
}
