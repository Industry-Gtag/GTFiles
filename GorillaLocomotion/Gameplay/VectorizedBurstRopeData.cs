using System;
using Unity.Collections;
using Unity.Mathematics;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011B5 RID: 4533
	public struct VectorizedBurstRopeData
	{
		// Token: 0x04008330 RID: 33584
		public NativeArray<float4> posX;

		// Token: 0x04008331 RID: 33585
		public NativeArray<float4> posY;

		// Token: 0x04008332 RID: 33586
		public NativeArray<float4> posZ;

		// Token: 0x04008333 RID: 33587
		public NativeArray<int4> validNodes;

		// Token: 0x04008334 RID: 33588
		public NativeArray<float4> lastPosX;

		// Token: 0x04008335 RID: 33589
		public NativeArray<float4> lastPosY;

		// Token: 0x04008336 RID: 33590
		public NativeArray<float4> lastPosZ;

		// Token: 0x04008337 RID: 33591
		public NativeArray<float3> ropeRoots;

		// Token: 0x04008338 RID: 33592
		public NativeArray<float4> nodeMass;
	}
}
