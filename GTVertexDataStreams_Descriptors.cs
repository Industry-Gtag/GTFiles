using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200038E RID: 910
public static class GTVertexDataStreams_Descriptors
{
	// Token: 0x06001612 RID: 5650 RVA: 0x0007D53C File Offset: 0x0007B73C
	public static void DoSetVertexBufferParams(ref Mesh.MeshData writeData, int totalVertexCount)
	{
		NativeArray<VertexAttributeDescriptor> nativeArray = new NativeArray<VertexAttributeDescriptor>(6, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
		int num = 0;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.position;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.color;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.uv1;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.lightmapUv;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.normal;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.tangent;
		writeData.SetVertexBufferParams(totalVertexCount, nativeArray);
		nativeArray.Dispose();
	}

	// Token: 0x04001C87 RID: 7303
	public static readonly VertexAttributeDescriptor position = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0);

	// Token: 0x04001C88 RID: 7304
	public static readonly VertexAttributeDescriptor color = new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, 0);

	// Token: 0x04001C89 RID: 7305
	public static readonly VertexAttributeDescriptor uv1 = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 4, 0);

	// Token: 0x04001C8A RID: 7306
	public static readonly VertexAttributeDescriptor lightmapUv = new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float16, 2, 0);

	// Token: 0x04001C8B RID: 7307
	public static readonly VertexAttributeDescriptor normal = new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 1);

	// Token: 0x04001C8C RID: 7308
	public static readonly VertexAttributeDescriptor tangent = new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.SNorm8, 4, 1);
}
