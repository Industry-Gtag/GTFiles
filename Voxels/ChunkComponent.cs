using System;
using UnityEngine;

namespace Voxels
{
	// Token: 0x0200137E RID: 4990
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshCollider))]
	public class ChunkComponent : MonoBehaviour
	{
		// Token: 0x17000C35 RID: 3125
		// (get) Token: 0x06007CFB RID: 31995 RVA: 0x0028DE4E File Offset: 0x0028C04E
		// (set) Token: 0x06007CFC RID: 31996 RVA: 0x0028DE56 File Offset: 0x0028C056
		public VoxelWorld World { get; set; }

		// Token: 0x06007CFD RID: 31997 RVA: 0x0028DE5F File Offset: 0x0028C05F
		private void Reset()
		{
			this.meshFilter = base.GetComponent<MeshFilter>();
			this.meshRenderer = base.GetComponent<MeshRenderer>();
			this.meshCollider = base.GetComponent<MeshCollider>();
		}

		// Token: 0x04008FD1 RID: 36817
		public MeshFilter meshFilter;

		// Token: 0x04008FD2 RID: 36818
		public MeshRenderer meshRenderer;

		// Token: 0x04008FD3 RID: 36819
		public MeshCollider meshCollider;
	}
}
