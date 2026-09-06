using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000651 RID: 1617
public class BuilderTableDataRenderIndirectBatch
{
	// Token: 0x0400345C RID: 13404
	public int totalInstances;

	// Token: 0x0400345D RID: 13405
	public TransformAccessArray instanceTransform;

	// Token: 0x0400345E RID: 13406
	public NativeArray<int> instanceTransformIndexToDataIndex;

	// Token: 0x0400345F RID: 13407
	public List<int> pieceIDPerTransform;

	// Token: 0x04003460 RID: 13408
	public NativeArray<Matrix4x4> instanceObjectToWorld;

	// Token: 0x04003461 RID: 13409
	public NativeArray<int> instanceTexIndex;

	// Token: 0x04003462 RID: 13410
	public NativeArray<float> instanceTint;

	// Token: 0x04003463 RID: 13411
	public NativeArray<int> instanceLodLevel;

	// Token: 0x04003464 RID: 13412
	public NativeArray<int> instanceLodLevelDirty;

	// Token: 0x04003465 RID: 13413
	public NativeList<BuilderTableMeshInstances> renderMeshes;

	// Token: 0x04003466 RID: 13414
	public GraphicsBuffer commandBuf;

	// Token: 0x04003467 RID: 13415
	public GraphicsBuffer matrixBuf;

	// Token: 0x04003468 RID: 13416
	public GraphicsBuffer texIndexBuf;

	// Token: 0x04003469 RID: 13417
	public GraphicsBuffer tintBuf;

	// Token: 0x0400346A RID: 13418
	public NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs> commandData;

	// Token: 0x0400346B RID: 13419
	public int commandCount;

	// Token: 0x0400346C RID: 13420
	public RenderParams rp;
}
