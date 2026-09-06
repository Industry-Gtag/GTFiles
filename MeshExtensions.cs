using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020001DF RID: 479
public static class MeshExtensions
{
	// Token: 0x06000CBC RID: 3260 RVA: 0x00045FC4 File Offset: 0x000441C4
	public static void SplitByAngle(this Mesh mesh, float angleDeg)
	{
		float num = Mathf.Cos(angleDeg * 0.017453292f);
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		int num2 = triangles.Length / 3;
		Vector3[] array = new Vector3[num2];
		for (int i = 0; i < num2; i++)
		{
			Vector3 vector = vertices[triangles[i * 3]];
			Vector3 vector2 = vertices[triangles[i * 3 + 1]];
			Vector3 vector3 = vertices[triangles[i * 3 + 2]];
			array[i] = Vector3.Cross(vector2 - vector, vector3 - vector).normalized;
		}
		List<ValueTuple<int, Vector3>>[] array2 = new List<ValueTuple<int, Vector3>>[vertices.Length];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = new List<ValueTuple<int, Vector3>>();
		}
		List<Vector3> list = new List<Vector3>(vertices.Length);
		int[] array3 = new int[triangles.Length];
		for (int k = 0; k < num2; k++)
		{
			for (int l = 0; l < 3; l++)
			{
				int num3 = triangles[k * 3 + l];
				Vector3 vector4 = array[k];
				int num4 = -1;
				foreach (ValueTuple<int, Vector3> valueTuple in array2[num3])
				{
					int item = valueTuple.Item1;
					if (Vector3.Dot(valueTuple.Item2, vector4) >= num)
					{
						num4 = item;
						break;
					}
				}
				if (num4 < 0)
				{
					num4 = list.Count;
					list.Add(vertices[num3]);
					array2[num3].Add(new ValueTuple<int, Vector3>(num4, vector4));
				}
				array3[k * 3 + l] = num4;
			}
		}
		mesh.Clear();
		mesh.SetVertices(list);
		mesh.triangles = array3;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x0004619C File Offset: 0x0004439C
	public static void SplitByAngleBurst(this Mesh mesh, float angleDeg, bool areaWeight = true, Allocator allocator = Allocator.TempJob)
	{
		NativeArray<float3> nativeArray = new NativeArray<float3>(mesh.vertexCount, allocator, NativeArrayOptions.ClearMemory);
		List<Vector3> list = new List<Vector3>(mesh.vertexCount);
		mesh.GetVertices(list);
		for (int i = 0; i < list.Count; i++)
		{
			nativeArray[i] = list[i];
		}
		NativeArray<int> nativeArray2 = new NativeArray<int>(mesh.triangles, allocator);
		NativeArray<float3> nativeArray3 = new NativeArray<float3>(nativeArray2.Length / 3, allocator, NativeArrayOptions.ClearMemory);
		new MeshExtensions.FaceNormalJob
		{
			Verts = nativeArray,
			Tris = nativeArray2,
			FaceN = nativeArray3
		}.Schedule(nativeArray3.Length, 64, default(JobHandle)).Complete();
		NativeList<float3> nativeList = new NativeList<float3>(nativeArray.Length, allocator);
		NativeList<int> nativeList2 = new NativeList<int>(nativeArray2.Length, allocator);
		new MeshExtensions.SplitJob
		{
			CosThresh = math.cos(math.radians(angleDeg)),
			SrcVerts = nativeArray,
			SrcTris = nativeArray2,
			FaceN = nativeArray3,
			DstVerts = nativeList,
			DstTris = nativeList2
		}.Run<MeshExtensions.SplitJob>();
		NativeArray<float3> nativeArray4 = new NativeArray<float3>(nativeList.Length, allocator, NativeArrayOptions.ClearMemory);
		MeshExtensions.RecalcNormalsJobified(nativeList, nativeList2, areaWeight, allocator, ref nativeArray4);
		mesh.Clear();
		List<Vector3> list2 = new List<Vector3>(nativeList.Length);
		for (int j = 0; j < nativeList.Length; j++)
		{
			list2.Add(nativeList[j]);
		}
		mesh.SetVertices(list2);
		mesh.triangles = nativeList2.AsArray().ToArray();
		List<Vector3> list3 = new List<Vector3>(nativeArray4.Length);
		for (int k = 0; k < nativeArray4.Length; k++)
		{
			list3.Add(nativeArray4[k]);
		}
		mesh.SetNormals(list3);
		mesh.RecalculateBounds();
		nativeArray.Dispose();
		nativeArray2.Dispose();
		nativeArray3.Dispose();
		nativeList.Dispose();
		nativeList2.Dispose();
		nativeArray4.Dispose();
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x000463B8 File Offset: 0x000445B8
	private static void RecalcNormalsJobified(NativeList<float3> verts, NativeList<int> tris, bool areaWeight, Allocator alloc, ref NativeArray<float3> outNormals)
	{
		int length = verts.Length;
		int num = tris.Length / 3;
		NativeArray<float3> nativeArray = new NativeArray<float3>(num, alloc, NativeArrayOptions.ClearMemory);
		new MeshExtensions.TriNormalJob
		{
			V = verts.AsArray(),
			T = tris.AsArray(),
			Out = nativeArray,
			AreaWeight = (areaWeight ? 1 : 0)
		}.Schedule(num, 64, default(JobHandle)).Complete();
		NativeParallelMultiHashMap<int, int> nativeParallelMultiHashMap = new NativeParallelMultiHashMap<int, int>(tris.Length, alloc);
		new MeshExtensions.BuildAdjJob
		{
			T = tris.AsArray(),
			MapW = nativeParallelMultiHashMap.AsParallelWriter()
		}.Schedule(num, 64, default(JobHandle)).Complete();
		new MeshExtensions.VertexNormalJob
		{
			AreaWeight = (areaWeight ? 1 : 0),
			TriN = nativeArray,
			V2T = nativeParallelMultiHashMap,
			Out = outNormals
		}.Schedule(length, 64, default(JobHandle)).Complete();
		nativeArray.Dispose();
		nativeParallelMultiHashMap.Dispose();
	}

	// Token: 0x020001E0 RID: 480
	[BurstCompile]
	private struct FaceNormalJob : IJobParallelFor
	{
		// Token: 0x06000CBF RID: 3263 RVA: 0x000464EC File Offset: 0x000446EC
		public void Execute(int index)
		{
			int num = index * 3;
			float3 @float = this.Verts[this.Tris[num]];
			float3 float2 = this.Verts[this.Tris[num + 1]];
			float3 float3 = this.Verts[this.Tris[num + 2]];
			this.FaceN[index] = math.normalize(math.cross(float2 - @float, float3 - @float));
		}

		// Token: 0x04000F73 RID: 3955
		[ReadOnly]
		public NativeArray<float3> Verts;

		// Token: 0x04000F74 RID: 3956
		[ReadOnly]
		public NativeArray<int> Tris;

		// Token: 0x04000F75 RID: 3957
		[WriteOnly]
		public NativeArray<float3> FaceN;
	}

	// Token: 0x020001E1 RID: 481
	[BurstCompile]
	private struct SplitJob : IJob
	{
		// Token: 0x06000CC0 RID: 3264 RVA: 0x00046570 File Offset: 0x00044770
		public void Execute()
		{
			int length = this.SrcVerts.Length;
			int num = this.SrcTris.Length / 3;
			NativeArray<int> nativeArray = new NativeArray<int>(length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < length; i++)
			{
				nativeArray[i] = -1;
			}
			NativeList<MeshExtensions.SplitJob.Bucket> nativeList = new NativeList<MeshExtensions.SplitJob.Bucket>(length, Allocator.Temp);
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
						MeshExtensions.SplitJob.Bucket bucket = nativeList[num4];
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
						MeshExtensions.SplitJob.Bucket bucket2 = new MeshExtensions.SplitJob.Bucket
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

		// Token: 0x04000F76 RID: 3958
		public float CosThresh;

		// Token: 0x04000F77 RID: 3959
		[ReadOnly]
		public NativeArray<float3> SrcVerts;

		// Token: 0x04000F78 RID: 3960
		[ReadOnly]
		public NativeArray<int> SrcTris;

		// Token: 0x04000F79 RID: 3961
		[ReadOnly]
		public NativeArray<float3> FaceN;

		// Token: 0x04000F7A RID: 3962
		public NativeList<float3> DstVerts;

		// Token: 0x04000F7B RID: 3963
		public NativeList<int> DstTris;

		// Token: 0x020001E2 RID: 482
		private struct Bucket
		{
			// Token: 0x04000F7C RID: 3964
			public int next;

			// Token: 0x04000F7D RID: 3965
			public int newIdx;

			// Token: 0x04000F7E RID: 3966
			public float3 repN;
		}
	}

	// Token: 0x020001E3 RID: 483
	[BurstCompile]
	private struct TriNormalJob : IJobParallelFor
	{
		// Token: 0x06000CC1 RID: 3265 RVA: 0x00046724 File Offset: 0x00044924
		public void Execute(int i)
		{
			int num = i * 3;
			float3 @float = this.V[this.T[num]];
			float3 float2 = this.V[this.T[num + 1]];
			float3 float3 = this.V[this.T[num + 2]];
			float3 float4 = math.cross(float2 - @float, float3 - @float);
			this.Out[i] = ((this.AreaWeight == 0) ? math.normalize(float4) : float4);
		}

		// Token: 0x04000F7F RID: 3967
		[ReadOnly]
		public NativeArray<float3> V;

		// Token: 0x04000F80 RID: 3968
		[ReadOnly]
		public NativeArray<int> T;

		// Token: 0x04000F81 RID: 3969
		[WriteOnly]
		public NativeArray<float3> Out;

		// Token: 0x04000F82 RID: 3970
		public int AreaWeight;
	}

	// Token: 0x020001E4 RID: 484
	[BurstCompile]
	private struct BuildAdjJob : IJobParallelFor
	{
		// Token: 0x06000CC2 RID: 3266 RVA: 0x000467B0 File Offset: 0x000449B0
		public void Execute(int triIdx)
		{
			int num = triIdx * 3;
			this.MapW.Add(this.T[num], triIdx);
			this.MapW.Add(this.T[num + 1], triIdx);
			this.MapW.Add(this.T[num + 2], triIdx);
		}

		// Token: 0x04000F83 RID: 3971
		[ReadOnly]
		public NativeArray<int> T;

		// Token: 0x04000F84 RID: 3972
		public NativeParallelMultiHashMap<int, int>.ParallelWriter MapW;
	}

	// Token: 0x020001E5 RID: 485
	[BurstCompile]
	private struct VertexNormalJob : IJobParallelFor
	{
		// Token: 0x06000CC3 RID: 3267 RVA: 0x00046810 File Offset: 0x00044A10
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

		// Token: 0x04000F85 RID: 3973
		public int AreaWeight;

		// Token: 0x04000F86 RID: 3974
		[ReadOnly]
		public NativeArray<float3> TriN;

		// Token: 0x04000F87 RID: 3975
		[ReadOnly]
		public NativeParallelMultiHashMap<int, int> V2T;

		// Token: 0x04000F88 RID: 3976
		[WriteOnly]
		public NativeArray<float3> Out;
	}
}
