using System;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

namespace FastSurfaceNets
{
	// Token: 0x0200137B RID: 4987
	public class SurfaceNetsWorld : MonoBehaviour
	{
		// Token: 0x06007CD9 RID: 31961 RVA: 0x0028D57C File Offset: 0x0028B77C
		private void Awake()
		{
			this.Generate();
		}

		// Token: 0x06007CDA RID: 31962 RVA: 0x0028D584 File Offset: 0x0028B784
		private void Generate()
		{
			this.DestroyChildren();
			for (int i = -this.radius.x; i <= this.radius.x; i++)
			{
				for (int j = -this.radius.y; j <= this.radius.y; j++)
				{
					for (int k = -this.radius.z; k <= this.radius.z; k++)
					{
						int3 @int = new int3(i, j, k);
						SurfaceNetsChunk surfaceNetsChunk = Object.Instantiate<SurfaceNetsChunk>(this.chunkPrefab, base.transform);
						surfaceNetsChunk.Id = @int;
						surfaceNetsChunk.parameters = this.parameters;
						surfaceNetsChunk.name = string.Format("SurfaceNetsChunk_{0}_{1}_{2}", @int.x, @int.y, @int.z);
						surfaceNetsChunk.transform.localPosition = @int.ToFloat3() * 32f;
						surfaceNetsChunk.BuildChunk();
					}
				}
			}
		}

		// Token: 0x06007CDB RID: 31963 RVA: 0x0028D690 File Offset: 0x0028B890
		private void DestroyChildren()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				Transform child = base.transform.GetChild(i);
				if (child != null)
				{
					JamUtil.Destroy(child.gameObject);
				}
			}
		}

		// Token: 0x04008FAC RID: 36780
		public SurfaceNetsChunk chunkPrefab;

		// Token: 0x04008FAD RID: 36781
		public int3 radius;

		// Token: 0x04008FAE RID: 36782
		public GenerationParameters parameters;
	}
}
