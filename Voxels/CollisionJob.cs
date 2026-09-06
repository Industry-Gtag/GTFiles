using System;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace Voxels
{
	// Token: 0x02001385 RID: 4997
	[BurstCompile]
	public struct CollisionJob : IJob
	{
		// Token: 0x06007D2B RID: 32043 RVA: 0x0028E981 File Offset: 0x0028CB81
		public void Execute()
		{
			Physics.BakeMesh(this.MeshId, false, MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase);
		}

		// Token: 0x04008FE7 RID: 36839
		public const MeshColliderCookingOptions CookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase;

		// Token: 0x04008FE8 RID: 36840
		public EntityId MeshId;
	}
}
