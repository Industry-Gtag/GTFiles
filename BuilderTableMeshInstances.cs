using System;
using Unity.Collections;
using UnityEngine.Jobs;

// Token: 0x0200064F RID: 1615
public struct BuilderTableMeshInstances
{
	// Token: 0x04003456 RID: 13398
	public TransformAccessArray transforms;

	// Token: 0x04003457 RID: 13399
	public NativeList<int> texIndex;

	// Token: 0x04003458 RID: 13400
	public NativeList<float> tint;
}
