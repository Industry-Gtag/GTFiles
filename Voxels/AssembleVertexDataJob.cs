using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x020013A4 RID: 5028
	public struct AssembleVertexDataJob : IJob
	{
		// Token: 0x06007D70 RID: 32112 RVA: 0x00291BB2 File Offset: 0x0028FDB2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int3 Sort3(int a, int b, int c)
		{
			if (a > b)
			{
				int num = a;
				a = b;
				b = num;
			}
			if (b > c)
			{
				int num2 = b;
				b = c;
				c = num2;
			}
			if (a > b)
			{
				int num3 = a;
				a = b;
				b = num3;
			}
			return new int3(a, b, c);
		}

		// Token: 0x06007D71 RID: 32113 RVA: 0x00291BDA File Offset: 0x0028FDDA
		private static int4 MakeMatSet(byte m0, byte m1, byte m2)
		{
			return new int4((int)m0, (int)m1, (int)m2, 255);
		}

		// Token: 0x06007D72 RID: 32114 RVA: 0x00291BEC File Offset: 0x0028FDEC
		public void Execute()
		{
			NativeParallelHashMap<AssembleVertexDataJob.Key, int> nativeParallelHashMap = new NativeParallelHashMap<AssembleVertexDataJob.Key, int>(this.srcVerts.Length * 2, Allocator.Temp);
			NativeList<MeshVertexData> nativeList = new NativeList<MeshVertexData>(this.srcVerts.Length * 2, Allocator.Temp);
			NativeList<ushort> nativeList2 = new NativeList<ushort>(this.srcTris.Length, Allocator.Temp);
			int num = this.srcTris.Length / 3;
			for (int i = 0; i < num; i++)
			{
				int num2 = this.srcTris[i * 3];
				int num3 = this.srcTris[i * 3 + 1];
				int num4 = this.srcTris[i * 3 + 2];
				int4 @int = AssembleVertexDataJob.MakeMatSet(this.srcMats[num2], this.srcMats[num3], this.srcMats[num4]);
				ushort num5 = this.GetOrCreate(num2, @int, new float4(1f, 0f, 0f, 0f), ref nativeParallelHashMap, ref nativeList);
				nativeList2.Add(in num5);
				num5 = this.GetOrCreate(num3, @int, new float4(0f, 1f, 0f, 0f), ref nativeParallelHashMap, ref nativeList);
				nativeList2.Add(in num5);
				num5 = this.GetOrCreate(num4, @int, new float4(0f, 0f, 1f, 0f), ref nativeParallelHashMap, ref nativeList);
				nativeList2.Add(in num5);
			}
			this.triangleCounter.Count = num;
			int length = nativeList.Length;
			int length2 = nativeList2.Length;
			NativeArray<MeshVertexData>.Copy(nativeList.AsArray(), this.vertexData, length);
			NativeArray<ushort>.Copy(nativeList2.AsArray(), this.triangleData, length2);
		}

		// Token: 0x06007D73 RID: 32115 RVA: 0x00291DAC File Offset: 0x0028FFAC
		private ushort GetOrCreate(int srcIdx, int4 mats, float4 blend, ref NativeParallelHashMap<AssembleVertexDataJob.Key, int> map, ref NativeList<MeshVertexData> vertsOut)
		{
			AssembleVertexDataJob.Key key = new AssembleVertexDataJob.Key
			{
				srcIdx = srcIdx,
				mats = mats
			};
			int length;
			if (!map.TryGetValue(key, out length))
			{
				length = vertsOut.Length;
				map.Add(key, length);
				float3 @float = this.srcNorm[srcIdx];
				float3 float2 = math.normalize(math.cross((math.abs(@float.y) < 0.999f) ? new float3(0f, 1f, 0f) : new float3(1f, 0f, 0f), @float));
				MeshVertexData meshVertexData = new MeshVertexData(this.srcVerts[srcIdx], @float, new float4(float2, 1f), mats, blend);
				vertsOut.Add(in meshVertexData);
			}
			return (ushort)length;
		}

		// Token: 0x0400907C RID: 36988
		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<MeshVertexData> vertexData;

		// Token: 0x0400907D RID: 36989
		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<ushort> triangleData;

		// Token: 0x0400907E RID: 36990
		[ReadOnly]
		public NativeArray<float3> srcVerts;

		// Token: 0x0400907F RID: 36991
		[ReadOnly]
		public NativeArray<byte> srcMats;

		// Token: 0x04009080 RID: 36992
		[ReadOnly]
		public NativeArray<float3> srcNorm;

		// Token: 0x04009081 RID: 36993
		[ReadOnly]
		public NativeArray<int> srcTris;

		// Token: 0x04009082 RID: 36994
		public NativeCounter triangleCounter;

		// Token: 0x020013A5 RID: 5029
		private struct Key : IEquatable<AssembleVertexDataJob.Key>
		{
			// Token: 0x06007D74 RID: 32116 RVA: 0x00291E7B File Offset: 0x0029007B
			public bool Equals(AssembleVertexDataJob.Key other)
			{
				return this.srcIdx == other.srcIdx && this.mats.Equals(other.mats);
			}

			// Token: 0x06007D75 RID: 32117 RVA: 0x00291E9E File Offset: 0x0029009E
			public override int GetHashCode()
			{
				return (int)math.hash(new int4(this.srcIdx, this.mats.xyz));
			}

			// Token: 0x04009083 RID: 36995
			public int srcIdx;

			// Token: 0x04009084 RID: 36996
			public int4 mats;
		}
	}
}
