using System;
using Unity.Mathematics;

namespace FastSurfaceNets
{
	// Token: 0x0200137A RID: 4986
	[Serializable]
	public class GenerationParameters
	{
		// Token: 0x04008FA1 RID: 36769
		public bool recalculateNormals;

		// Token: 0x04008FA2 RID: 36770
		public bool customNormals = true;

		// Token: 0x04008FA3 RID: 36771
		public bool useBurst = true;

		// Token: 0x04008FA4 RID: 36772
		public float normalThreshold = 60f;

		// Token: 0x04008FA5 RID: 36773
		public bool areaWeightedNormals = true;

		// Token: 0x04008FA6 RID: 36774
		public bool generateShape = true;

		// Token: 0x04008FA7 RID: 36775
		public int3 shapeMin = new int3(1);

		// Token: 0x04008FA8 RID: 36776
		public int3 shapeMax = new int3(15);

		// Token: 0x04008FA9 RID: 36777
		public float noiseScale = 0.01f;

		// Token: 0x04008FAA RID: 36778
		public float baseHeight = 10f;

		// Token: 0x04008FAB RID: 36779
		public float heightScale = 5f;
	}
}
