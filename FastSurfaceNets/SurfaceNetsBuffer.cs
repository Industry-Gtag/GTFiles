using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace FastSurfaceNets
{
	// Token: 0x02001375 RID: 4981
	public class SurfaceNetsBuffer
	{
		// Token: 0x06007CC5 RID: 31941 RVA: 0x0028C1E0 File Offset: 0x0028A3E0
		internal void Reset(int arraySize)
		{
			this.Positions.Clear();
			this.Normals.Clear();
			this.Indices.Clear();
			this.SurfacePoints.Clear();
			this.SurfaceStrides.Clear();
			if (this.StrideToIndex.Length < arraySize)
			{
				Array.Resize<int>(ref this.StrideToIndex, arraySize);
			}
			for (int i = 0; i < arraySize; i++)
			{
				this.StrideToIndex[i] = int.MaxValue;
			}
		}

		// Token: 0x04008F7B RID: 36731
		public readonly List<float3> Positions = new List<float3>();

		// Token: 0x04008F7C RID: 36732
		public readonly List<float3> Normals = new List<float3>();

		// Token: 0x04008F7D RID: 36733
		public readonly List<int> Indices = new List<int>();

		// Token: 0x04008F7E RID: 36734
		internal readonly List<int3> SurfacePoints = new List<int3>();

		// Token: 0x04008F7F RID: 36735
		internal readonly List<int> SurfaceStrides = new List<int>();

		// Token: 0x04008F80 RID: 36736
		internal int[] StrideToIndex = Array.Empty<int>();

		// Token: 0x04008F81 RID: 36737
		public const int NullVertex = 2147483647;
	}
}
