using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x02001397 RID: 5015
	public static class MeshUtilities
	{
		// Token: 0x06007D5A RID: 32090 RVA: 0x002904B8 File Offset: 0x0028E6B8
		public static void SplitByAngle(this Mesh mesh, float angleDeg, bool areaWeight = true, Allocator allocator = Allocator.TempJob)
		{
			NativeArray<float3> nativeArray = new NativeArray<float3>(mesh.vertexCount, allocator, NativeArrayOptions.ClearMemory);
			List<Vector3> list = new List<Vector3>(mesh.vertexCount);
			mesh.GetVertices(list);
			for (int i = 0; i < list.Count; i++)
			{
				nativeArray[i] = list[i];
			}
			new NativeArray<byte>(mesh.vertexCount, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<int> nativeArray2 = new NativeArray<int>(mesh.triangles, allocator);
			MeshUtilities.MeshData meshData = MeshUtilities.SplitByAngle(nativeArray, nativeArray2, angleDeg, areaWeight, allocator);
			mesh.Clear();
			mesh.SetVertices<float3>(meshData.Vertices.AsArray());
			mesh.SetTriangles(meshData.Triangles.AsArray().ToArray(), 0, false);
			mesh.SetNormals<float3>(meshData.Normals.AsArray());
			mesh.RecalculateBounds();
			nativeArray.Dispose();
			nativeArray2.Dispose();
			meshData.Dispose();
		}

		// Token: 0x06007D5B RID: 32091 RVA: 0x0029059C File Offset: 0x0028E79C
		public static MeshUtilities.MeshData SplitByAngle(NativeArray<float3> srcVerts, NativeArray<int> srcTris, float angleDeg, bool areaWeight = true, Allocator allocator = Allocator.TempJob)
		{
			NativeArray<float3> nativeArray = new NativeArray<float3>(srcTris.Length / 3, allocator, NativeArrayOptions.ClearMemory);
			new MeshUtilities.FaceNormalJob
			{
				Verts = srcVerts,
				Tris = srcTris,
				FaceN = nativeArray
			}.Schedule(nativeArray.Length, 64, default(JobHandle)).Complete();
			NativeList<float3> nativeList = new NativeList<float3>(srcVerts.Length, allocator);
			NativeList<int> nativeList2 = new NativeList<int>(srcTris.Length, allocator);
			new MeshUtilities.SplitJob
			{
				CosThresh = math.cos(math.radians(angleDeg)),
				SrcVerts = srcVerts,
				SrcTris = srcTris,
				FaceN = nativeArray,
				DstVerts = nativeList,
				DstTris = nativeList2
			}.Run<MeshUtilities.SplitJob>();
			NativeList<float3> nativeList3 = new NativeList<float3>(nativeList.Length, allocator);
			nativeList3.ResizeUninitialized(nativeList.Length);
			MeshUtilities.RecalcNormalsJobified(nativeList, nativeList2, areaWeight, allocator, ref nativeList3);
			nativeArray.Dispose();
			return new MeshUtilities.MeshData
			{
				Vertices = nativeList,
				Triangles = nativeList2,
				Normals = nativeList3
			};
		}

		// Token: 0x06007D5C RID: 32092 RVA: 0x002906CC File Offset: 0x0028E8CC
		public static MeshUtilities.VoxelMeshData SplitByAngle(NativeArray<float3> srcVerts, NativeArray<byte> srcMats, NativeArray<int> srcTris, float angleDeg, bool areaWeight = true, Allocator allocator = Allocator.TempJob)
		{
			NativeArray<float3> nativeArray = new NativeArray<float3>(srcTris.Length / 3, allocator, NativeArrayOptions.ClearMemory);
			new MeshUtilities.FaceNormalJob
			{
				Verts = srcVerts,
				Tris = srcTris,
				FaceN = nativeArray
			}.Schedule(nativeArray.Length, 64, default(JobHandle)).Complete();
			NativeList<float3> nativeList = new NativeList<float3>(srcVerts.Length, allocator);
			NativeList<byte> nativeList2 = new NativeList<byte>(srcVerts.Length, allocator);
			NativeList<int> nativeList3 = new NativeList<int>(srcTris.Length, allocator);
			new MeshUtilities.SplitVoxelMeshJob
			{
				CosThresh = math.cos(math.radians(angleDeg)),
				SrcVerts = srcVerts,
				SrcMats = srcMats,
				SrcTris = srcTris,
				FaceN = nativeArray,
				DstVerts = nativeList,
				DstMats = nativeList2,
				DstTris = nativeList3
			}.Run<MeshUtilities.SplitVoxelMeshJob>();
			NativeList<float3> nativeList4 = new NativeList<float3>(nativeList.Length, allocator);
			nativeList4.ResizeUninitialized(nativeList.Length);
			MeshUtilities.RecalcNormalsJobified(nativeList, nativeList3, areaWeight, allocator, ref nativeList4);
			nativeArray.Dispose();
			return new MeshUtilities.VoxelMeshData
			{
				Vertices = nativeList,
				Materials = nativeList2,
				Triangles = nativeList3,
				Normals = nativeList4
			};
		}

		// Token: 0x06007D5D RID: 32093 RVA: 0x0029082C File Offset: 0x0028EA2C
		private static void RecalcNormalsJobified(NativeList<float3> verts, NativeList<int> tris, bool areaWeight, Allocator alloc, ref NativeList<float3> outNormals)
		{
			int length = verts.Length;
			int num = tris.Length / 3;
			NativeArray<float3> nativeArray = new NativeArray<float3>(num, alloc, NativeArrayOptions.ClearMemory);
			new MeshUtilities.TriNormalJob
			{
				V = verts.AsArray(),
				T = tris.AsArray(),
				Out = nativeArray,
				AreaWeight = (areaWeight ? 1 : 0)
			}.Schedule(num, 64, default(JobHandle)).Complete();
			NativeParallelMultiHashMap<int, int> nativeParallelMultiHashMap = new NativeParallelMultiHashMap<int, int>(tris.Length, alloc);
			new MeshUtilities.BuildAdjJob
			{
				T = tris.AsArray(),
				MapW = nativeParallelMultiHashMap.AsParallelWriter()
			}.Schedule(num, 64, default(JobHandle)).Complete();
			new MeshUtilities.VertexNormalJob
			{
				AreaWeight = (areaWeight ? 1 : 0),
				TriN = nativeArray,
				V2T = nativeParallelMultiHashMap,
				Out = outNormals.AsArray()
			}.Schedule(length, 64, default(JobHandle)).Complete();
			nativeArray.Dispose();
			nativeParallelMultiHashMap.Dispose();
		}

		// Token: 0x02001398 RID: 5016
		public struct MeshData : IDisposable
		{
			// Token: 0x06007D5E RID: 32094 RVA: 0x00290960 File Offset: 0x0028EB60
			public void Dispose()
			{
				this.Vertices.Dispose();
				this.Triangles.Dispose();
				this.Normals.Dispose();
			}

			// Token: 0x04009042 RID: 36930
			public NativeList<float3> Vertices;

			// Token: 0x04009043 RID: 36931
			public NativeList<int> Triangles;

			// Token: 0x04009044 RID: 36932
			public NativeList<float3> Normals;
		}

		// Token: 0x02001399 RID: 5017
		public struct VoxelMeshData : IDisposable
		{
			// Token: 0x06007D5F RID: 32095 RVA: 0x00290983 File Offset: 0x0028EB83
			public void Dispose()
			{
				this.Vertices.Dispose();
				this.Materials.Dispose();
				this.Triangles.Dispose();
				this.Normals.Dispose();
			}

			// Token: 0x04009045 RID: 36933
			public NativeList<float3> Vertices;

			// Token: 0x04009046 RID: 36934
			public NativeList<byte> Materials;

			// Token: 0x04009047 RID: 36935
			public NativeList<int> Triangles;

			// Token: 0x04009048 RID: 36936
			public NativeList<float3> Normals;
		}

		// Token: 0x0200139A RID: 5018
		[BurstCompile]
		public struct FaceNormalJob : IJobParallelFor
		{
			// Token: 0x06007D60 RID: 32096 RVA: 0x002909B4 File Offset: 0x0028EBB4
			public void Execute(int index)
			{
				int num = index * 3;
				float3 @float = this.Verts[this.Tris[num]];
				float3 float2 = this.Verts[this.Tris[num + 1]];
				float3 float3 = this.Verts[this.Tris[num + 2]];
				this.FaceN[index] = math.normalize(math.cross(float2 - @float, float3 - @float));
			}

			// Token: 0x04009049 RID: 36937
			[ReadOnly]
			public NativeArray<float3> Verts;

			// Token: 0x0400904A RID: 36938
			[ReadOnly]
			public NativeArray<int> Tris;

			// Token: 0x0400904B RID: 36939
			[WriteOnly]
			public NativeArray<float3> FaceN;
		}

		// Token: 0x0200139B RID: 5019
		[BurstCompile]
		public struct SplitJob : IJob
		{
			// Token: 0x06007D61 RID: 32097 RVA: 0x00290A38 File Offset: 0x0028EC38
			public void Execute()
			{
				int length = this.SrcVerts.Length;
				int num = this.SrcTris.Length / 3;
				NativeArray<int> nativeArray = new NativeArray<int>(length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				for (int i = 0; i < length; i++)
				{
					nativeArray[i] = -1;
				}
				NativeList<MeshUtilities.SplitJob.Bucket> nativeList = new NativeList<MeshUtilities.SplitJob.Bucket>(length, Allocator.Temp);
				this.DstVerts.Clear();
				this.DstTris.ResizeUninitialized(this.SrcTris.Length);
				for (int j = 0; j < num; j++)
				{
					float3 @float = this.FaceN[j];
					for (int k = 0; k < 3; k++)
					{
						int num2 = this.SrcTris[j * 3 + k];
						int num3 = -1;
						for (int num4 = nativeArray[num2]; num4 != -1; num4 = nativeList[num4].next)
						{
							MeshUtilities.SplitJob.Bucket bucket = nativeList[num4];
							if (math.dot(bucket.repN, @float) >= this.CosThresh)
							{
								num3 = bucket.newIdx;
								break;
							}
						}
						if (num3 == -1)
						{
							num3 = this.DstVerts.Length;
							float3 float2 = this.SrcVerts[num2];
							this.DstVerts.Add(in float2);
							MeshUtilities.SplitJob.Bucket bucket2 = new MeshUtilities.SplitJob.Bucket
							{
								next = nativeArray[num2],
								newIdx = num3,
								repN = @float
							};
							nativeArray[num2] = nativeList.Length;
							nativeList.Add(in bucket2);
						}
						this.DstTris[j * 3 + k] = num3;
					}
				}
				nativeArray.Dispose();
				nativeList.Dispose();
			}

			// Token: 0x0400904C RID: 36940
			public float CosThresh;

			// Token: 0x0400904D RID: 36941
			[ReadOnly]
			public NativeArray<float3> SrcVerts;

			// Token: 0x0400904E RID: 36942
			[ReadOnly]
			public NativeArray<int> SrcTris;

			// Token: 0x0400904F RID: 36943
			[ReadOnly]
			public NativeArray<float3> FaceN;

			// Token: 0x04009050 RID: 36944
			public NativeList<float3> DstVerts;

			// Token: 0x04009051 RID: 36945
			public NativeList<int> DstTris;

			// Token: 0x0200139C RID: 5020
			private struct Bucket
			{
				// Token: 0x04009052 RID: 36946
				public int next;

				// Token: 0x04009053 RID: 36947
				public int newIdx;

				// Token: 0x04009054 RID: 36948
				public float3 repN;
			}
		}

		// Token: 0x0200139D RID: 5021
		[BurstCompile]
		public struct SplitVoxelMeshJob : IJob
		{
			// Token: 0x06007D62 RID: 32098 RVA: 0x00290BEC File Offset: 0x0028EDEC
			public void Execute()
			{
				int length = this.SrcVerts.Length;
				int num = this.SrcTris.Length / 3;
				NativeArray<int> nativeArray = new NativeArray<int>(length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				for (int i = 0; i < length; i++)
				{
					nativeArray[i] = -1;
				}
				NativeList<MeshUtilities.SplitVoxelMeshJob.Bucket> nativeList = new NativeList<MeshUtilities.SplitVoxelMeshJob.Bucket>(length, Allocator.Temp);
				this.DstVerts.Clear();
				this.DstMats.Clear();
				this.DstTris.ResizeUninitialized(this.SrcTris.Length);
				for (int j = 0; j < num; j++)
				{
					float3 @float = this.FaceN[j];
					for (int k = 0; k < 3; k++)
					{
						int num2 = this.SrcTris[j * 3 + k];
						int num3 = -1;
						for (int num4 = nativeArray[num2]; num4 != -1; num4 = nativeList[num4].next)
						{
							MeshUtilities.SplitVoxelMeshJob.Bucket bucket = nativeList[num4];
							if (math.dot(bucket.repN, @float) >= this.CosThresh)
							{
								num3 = bucket.newIdx;
								break;
							}
						}
						if (num3 == -1)
						{
							num3 = this.DstVerts.Length;
							float3 float2 = this.SrcVerts[num2];
							this.DstVerts.Add(in float2);
							byte b = this.SrcMats[num2];
							this.DstMats.Add(in b);
							MeshUtilities.SplitVoxelMeshJob.Bucket bucket2 = new MeshUtilities.SplitVoxelMeshJob.Bucket
							{
								next = nativeArray[num2],
								newIdx = num3,
								repN = @float
							};
							nativeArray[num2] = nativeList.Length;
							nativeList.Add(in bucket2);
						}
						this.DstTris[j * 3 + k] = num3;
					}
				}
				nativeArray.Dispose();
				nativeList.Dispose();
			}

			// Token: 0x04009055 RID: 36949
			public float CosThresh;

			// Token: 0x04009056 RID: 36950
			[ReadOnly]
			public NativeArray<float3> SrcVerts;

			// Token: 0x04009057 RID: 36951
			[ReadOnly]
			public NativeArray<byte> SrcMats;

			// Token: 0x04009058 RID: 36952
			[ReadOnly]
			public NativeArray<int> SrcTris;

			// Token: 0x04009059 RID: 36953
			[ReadOnly]
			public NativeArray<float3> FaceN;

			// Token: 0x0400905A RID: 36954
			public NativeList<float3> DstVerts;

			// Token: 0x0400905B RID: 36955
			public NativeList<byte> DstMats;

			// Token: 0x0400905C RID: 36956
			public NativeList<int> DstTris;

			// Token: 0x0200139E RID: 5022
			private struct Bucket
			{
				// Token: 0x0400905D RID: 36957
				public int next;

				// Token: 0x0400905E RID: 36958
				public int newIdx;

				// Token: 0x0400905F RID: 36959
				public float3 repN;
			}
		}

		// Token: 0x0200139F RID: 5023
		[BurstCompile]
		private struct TriNormalJob : IJobParallelFor
		{
			// Token: 0x06007D63 RID: 32099 RVA: 0x00290DC8 File Offset: 0x0028EFC8
			public void Execute(int i)
			{
				int num = i * 3;
				float3 @float = this.V[this.T[num]];
				float3 float2 = this.V[this.T[num + 1]];
				float3 float3 = this.V[this.T[num + 2]];
				float3 float4 = math.cross(float2 - @float, float3 - @float);
				this.Out[i] = ((this.AreaWeight == 0) ? math.normalize(float4) : float4);
			}

			// Token: 0x04009060 RID: 36960
			[ReadOnly]
			public NativeArray<float3> V;

			// Token: 0x04009061 RID: 36961
			[ReadOnly]
			public NativeArray<int> T;

			// Token: 0x04009062 RID: 36962
			[WriteOnly]
			public NativeArray<float3> Out;

			// Token: 0x04009063 RID: 36963
			public int AreaWeight;
		}

		// Token: 0x020013A0 RID: 5024
		[BurstCompile]
		private struct BuildAdjJob : IJobParallelFor
		{
			// Token: 0x06007D64 RID: 32100 RVA: 0x00290E54 File Offset: 0x0028F054
			public void Execute(int triIdx)
			{
				int num = triIdx * 3;
				this.MapW.Add(this.T[num], triIdx);
				this.MapW.Add(this.T[num + 1], triIdx);
				this.MapW.Add(this.T[num + 2], triIdx);
			}

			// Token: 0x04009064 RID: 36964
			[ReadOnly]
			public NativeArray<int> T;

			// Token: 0x04009065 RID: 36965
			public NativeParallelMultiHashMap<int, int>.ParallelWriter MapW;
		}

		// Token: 0x020013A1 RID: 5025
		[BurstCompile]
		private struct VertexNormalJob : IJobParallelFor
		{
			// Token: 0x06007D65 RID: 32101 RVA: 0x00290EB4 File Offset: 0x0028F0B4
			public void Execute(int v)
			{
				NativeParallelMultiHashMap<int, int>.Enumerator valuesForKey = this.V2T.GetValuesForKey(v);
				if (!valuesForKey.MoveNext())
				{
					this.Out[v] = float3.zero;
					return;
				}
				int num = valuesForKey.Current;
				float3 @float = this.TriN[num];
				float3 float2 = ((this.AreaWeight == 0) ? @float : math.normalize(@float));
				float3 float3 = float3.zero;
				NativeParallelMultiHashMap<int, int>.Enumerator valuesForKey2 = this.V2T.GetValuesForKey(v);
				while (valuesForKey2.MoveNext())
				{
					int num2 = valuesForKey2.Current;
					float3 float4 = this.TriN[num2];
					if (this.AreaWeight != 0)
					{
						math.normalize(float4);
					}
					float3 += float4;
				}
				this.Out[v] = ((math.lengthsq(float3) < 1E-09f) ? float2 : math.normalize(float3));
			}

			// Token: 0x04009066 RID: 36966
			public int AreaWeight;

			// Token: 0x04009067 RID: 36967
			[ReadOnly]
			public NativeArray<float3> TriN;

			// Token: 0x04009068 RID: 36968
			[ReadOnly]
			public NativeParallelMultiHashMap<int, int> V2T;

			// Token: 0x04009069 RID: 36969
			[WriteOnly]
			public NativeArray<float3> Out;
		}
	}
}
