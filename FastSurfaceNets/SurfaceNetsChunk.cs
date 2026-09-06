using System;
using Cysharp.Threading.Tasks;
using GorillaExtensions;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Voxels;

namespace FastSurfaceNets
{
	// Token: 0x02001377 RID: 4983
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class SurfaceNetsChunk : MonoBehaviour
	{
		// Token: 0x06007CCF RID: 31951 RVA: 0x0028CC5A File Offset: 0x0028AE5A
		private void Awake()
		{
			if (this.autoGenerate)
			{
				this.BuildChunk();
			}
		}

		// Token: 0x06007CD0 RID: 31952 RVA: 0x0028CC6A File Offset: 0x0028AE6A
		private void OnDestroy()
		{
			if (this.sdf.IsCreated)
			{
				this.sdf.Dispose();
				this.sdf = default(NativeArray<byte>);
			}
		}

		// Token: 0x06007CD1 RID: 31953 RVA: 0x0028CC90 File Offset: 0x0028AE90
		public async void BuildChunk()
		{
			this.chunkPosition = this.Id * 32;
			this.shape = new int3(34);
			int num = this.shape.x * this.shape.y * this.shape.z;
			if (this.sdf.IsCreated)
			{
				this.sdf.Dispose();
				this.sdf = default(NativeArray<byte>);
			}
			this.sdf = new NativeArray<byte>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.FillChunk();
			SurfaceNetsBuffer buffer = new SurfaceNetsBuffer(32768, 65536, num, Allocator.TempJob);
			this.min = new int3(0);
			this.max = this.shape - 1;
			JobHandle handle = SurfaceNets.Generate(this.sdf, this.shape, this.min, this.max, buffer, default(JobHandle));
			while (!handle.IsCompleted)
			{
				await UniTask.Yield();
			}
			handle.Complete();
			Debug.Log(string.Format("{0} generated {1} vertices, {2} normals, {3}({4}) triangles.", new object[]
			{
				base.name,
				buffer.Vertices.Length,
				buffer.Normals.Length,
				buffer.Triangles.Length / 3,
				buffer.Triangles.Length
			}), this);
			if (buffer.Triangles.Length != 0)
			{
				if (this.parameters.customNormals)
				{
					MeshUtilities.MeshData meshData = MeshUtilities.SplitByAngle(buffer.Vertices.AsArray(), buffer.Triangles.AsArray(), this.parameters.normalThreshold, this.parameters.areaWeightedNormals, Allocator.TempJob);
					ref NativeList<float3> ptr = ref buffer.Vertices;
					NativeList<float3> nativeList = meshData.Vertices;
					NativeList<float3> nativeList2 = buffer.Vertices;
					ptr = nativeList;
					meshData.Vertices = nativeList2;
					ptr = ref buffer.Normals;
					nativeList2 = meshData.Normals;
					nativeList = buffer.Normals;
					ptr = nativeList2;
					meshData.Normals = nativeList;
					ref NativeList<int> ptr2 = ref buffer.Triangles;
					NativeList<int> triangles = meshData.Triangles;
					NativeList<int> triangles2 = buffer.Triangles;
					ptr2 = triangles;
					meshData.Triangles = triangles2;
					meshData.Dispose();
				}
				this.mesh = new Mesh
				{
					indexFormat = ((buffer.Vertices.Length > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16)
				};
				this.mesh.SetVertices<float3>(buffer.Vertices.AsArray());
				this.mesh.SetTriangles(buffer.Triangles.AsArray().ToArray(), 0, false);
				if (this.parameters.recalculateNormals)
				{
					this.mesh.RecalculateNormals();
				}
				else
				{
					this.mesh.SetNormals<float3>(buffer.Normals.AsArray());
				}
				this.mesh.RecalculateBounds();
				buffer.Dispose();
				base.GetComponent<MeshFilter>().sharedMesh = this.mesh;
				base.gameObject.GetOrAddComponent<MeshCollider>().sharedMesh = this.mesh;
			}
		}

		// Token: 0x06007CD2 RID: 31954 RVA: 0x0028CCC8 File Offset: 0x0028AEC8
		private void FillChunk()
		{
			if (this.parameters.generateShape)
			{
				int x = this.shape.x;
				int num = this.shape.x * this.shape.y;
				int3 @int = this.parameters.shapeMin - this.chunkPosition + this.min;
				int3 int2 = this.parameters.shapeMax - this.chunkPosition + this.min;
				for (int i = 0; i < this.shape.z; i++)
				{
					for (int j = 0; j < this.shape.y; j++)
					{
						int num2 = i * num + j * x;
						for (int k = 0; k < this.shape.x; k++)
						{
							float num3;
							if (this.parameters.generateShape)
							{
								num3 = ((k >= @int.x && k <= int2.x && j >= @int.y && j <= int2.y && i >= @int.z && i <= int2.z) ? 1f : (-1f));
							}
							else
							{
								float3 @float = (this.chunkPosition + new int3(k, j, i)).ToFloat3();
								num3 = noise.snoise(@float * this.parameters.noiseScale) - @float.y / this.parameters.heightScale;
							}
							this.sdf[num2 + k] = num3.ToByte();
						}
					}
				}
				return;
			}
			new FillChunkJob
			{
				sdf = this.sdf,
				shape = this.shape,
				chunkPosition = this.chunkPosition,
				shapeMin = this.parameters.shapeMin,
				shapeMax = this.parameters.shapeMax,
				noiseScale = this.parameters.noiseScale,
				heightScale = this.parameters.heightScale,
				min = this.min,
				max = this.max,
				strideY = this.shape.x,
				strideZ = this.shape.x * this.shape.y
			}.Schedule(this.shape.x * this.shape.y * this.shape.z, 64, default(JobHandle)).Complete();
		}

		// Token: 0x06007CD3 RID: 31955 RVA: 0x0028CF7C File Offset: 0x0028B17C
		private void OnDrawGizmosSelected()
		{
			if (!this.mesh || this.mesh.vertexCount < 3)
			{
				return;
			}
			Gizmos.color = Color.green;
			int vertexCount = this.mesh.vertexCount;
			Vector3[] vertices = this.mesh.vertices;
			Vector3[] normals = this.mesh.normals;
			for (int i = 0; i < vertexCount; i++)
			{
				Gizmos.DrawLine(base.transform.position + vertices[i], base.transform.position + vertices[i] + normals[i] * 0.25f);
			}
		}

		// Token: 0x04008F85 RID: 36741
		public int3 Id;

		// Token: 0x04008F86 RID: 36742
		public GenerationParameters parameters;

		// Token: 0x04008F87 RID: 36743
		public const int ChunkSize = 32;

		// Token: 0x04008F88 RID: 36744
		public bool autoGenerate = true;

		// Token: 0x04008F89 RID: 36745
		private const int Pad = 1;

		// Token: 0x04008F8A RID: 36746
		private int3 chunkPosition;

		// Token: 0x04008F8B RID: 36747
		private NativeArray<byte> sdf;

		// Token: 0x04008F8C RID: 36748
		private int3 min;

		// Token: 0x04008F8D RID: 36749
		private int3 max;

		// Token: 0x04008F8E RID: 36750
		private int3 shape;

		// Token: 0x04008F8F RID: 36751
		private Mesh mesh;
	}
}
