using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x02001394 RID: 5012
	[BurstCompile]
	public struct SortChunksJob : IJob
	{
		// Token: 0x06007D4C RID: 32076 RVA: 0x0028FE00 File Offset: 0x0028E000
		public void Execute()
		{
			int count = this.ChunkSet.Count;
			this.SortedChunks.ResizeUninitialized(count);
			NativeArray<int3> nativeArray = new NativeArray<int3>(count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int num = 0;
			foreach (int3 @int in this.ChunkSet)
			{
				nativeArray[num++] = @int;
			}
			NativeArray<SortChunksJob.SortKey> nativeArray2 = new NativeArray<SortChunksJob.SortKey>(count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < count; i++)
			{
				uint num2 = (uint)math.distancesq(nativeArray[i], this.TargetPos);
				nativeArray2[i] = new SortChunksJob.SortKey(((ulong)num2 << 32) | (ulong)i);
			}
			nativeArray2.Sort<SortChunksJob.SortKey>();
			for (int j = 0; j < count; j++)
			{
				int num3 = (int)(nativeArray2[j].value & (ulong)(-1));
				this.SortedChunks[j] = nativeArray[num3];
			}
			nativeArray2.Dispose();
			nativeArray.Dispose();
		}

		// Token: 0x04009033 RID: 36915
		[ReadOnly]
		public NativeHashSet<int3> ChunkSet;

		// Token: 0x04009034 RID: 36916
		[ReadOnly]
		public int3 TargetPos;

		// Token: 0x04009035 RID: 36917
		public NativeList<int3> SortedChunks;

		// Token: 0x02001395 RID: 5013
		private struct SortKey : IComparable<SortChunksJob.SortKey>
		{
			// Token: 0x06007D4D RID: 32077 RVA: 0x0028FF24 File Offset: 0x0028E124
			public SortKey(ulong val)
			{
				this.value = val;
			}

			// Token: 0x06007D4E RID: 32078 RVA: 0x0028FF2D File Offset: 0x0028E12D
			public int CompareTo(SortChunksJob.SortKey other)
			{
				return this.value.CompareTo(other.value);
			}

			// Token: 0x04009036 RID: 36918
			public ulong value;
		}
	}
}
