using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000652 RID: 1618
public class BuilderTableDataRenderData
{
	// Token: 0x0400346D RID: 13421
	public const int NUM_SPLIT_MESH_INSTANCE_GROUPS = 1;

	// Token: 0x0400346E RID: 13422
	public int texWidth;

	// Token: 0x0400346F RID: 13423
	public int texHeight;

	// Token: 0x04003470 RID: 13424
	public TextureFormat textureFormat;

	// Token: 0x04003471 RID: 13425
	public Dictionary<Material, int> materialToIndex;

	// Token: 0x04003472 RID: 13426
	public List<Material> materials;

	// Token: 0x04003473 RID: 13427
	public Material sharedMaterial;

	// Token: 0x04003474 RID: 13428
	public Material sharedMaterialIndirect;

	// Token: 0x04003475 RID: 13429
	public Dictionary<Texture2D, int> textureToIndex;

	// Token: 0x04003476 RID: 13430
	public List<Texture2D> textures;

	// Token: 0x04003477 RID: 13431
	public List<Material> perTextureMaterial;

	// Token: 0x04003478 RID: 13432
	public List<MaterialPropertyBlock> perTexturePropertyBlock;

	// Token: 0x04003479 RID: 13433
	public Texture2DArray sharedTexArray;

	// Token: 0x0400347A RID: 13434
	public Dictionary<Mesh, int> meshToIndex;

	// Token: 0x0400347B RID: 13435
	public List<Mesh> meshes;

	// Token: 0x0400347C RID: 13436
	public List<int> meshInstanceCount;

	// Token: 0x0400347D RID: 13437
	public NativeList<BuilderTableSubMesh> subMeshes;

	// Token: 0x0400347E RID: 13438
	public Mesh sharedMesh;

	// Token: 0x0400347F RID: 13439
	public BuilderTableDataRenderIndirectBatch dynamicBatch;

	// Token: 0x04003480 RID: 13440
	public BuilderTableDataRenderIndirectBatch staticBatch;

	// Token: 0x04003481 RID: 13441
	public JobHandle setupInstancesJobs;
}
