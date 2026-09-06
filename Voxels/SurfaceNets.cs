using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x020013A6 RID: 5030
	public static class SurfaceNets
	{
		// Token: 0x06007D76 RID: 32118 RVA: 0x00291EBC File Offset: 0x002900BC
		public static JobHandle Generate(NativeArray<byte> sdf, int3 shape, int3 min, int3 max, SurfaceNetsBuffer buffer, JobHandle dependency = default(JobHandle))
		{
			if (sdf.Length != shape.x * shape.y * shape.z)
			{
				throw new ArgumentException("SDF size does not match shape dimensions.");
			}
			return new SurfaceNetsJob
			{
				sdf = sdf,
				shape = shape,
				min = min,
				max = max,
				buffer = buffer,
				isoLevel = 0f.ToByte()
			}.Schedule(dependency);
		}
	}
}
