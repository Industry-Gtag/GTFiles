using System;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace Voxels
{
	// Token: 0x02001388 RID: 5000
	public struct MeshVertexData
	{
		// Token: 0x06007D31 RID: 32049 RVA: 0x0028F0CF File Offset: 0x0028D2CF
		public MeshVertexData(float3 position, float3 normal, float4 tangent, float4 materials, float4 blend)
		{
			this.position = position;
			this.normal = normal;
			this.tangent = tangent;
			this.materials = materials;
			this.blend = blend;
		}

		// Token: 0x06007D32 RID: 32050 RVA: 0x0028F0F6 File Offset: 0x0028D2F6
		public override string ToString()
		{
			return string.Format("({0:F2} x {1:F2})", this.position, this.normal);
		}

		// Token: 0x04008FF5 RID: 36853
		public float3 position;

		// Token: 0x04008FF6 RID: 36854
		public float3 normal;

		// Token: 0x04008FF7 RID: 36855
		public float4 tangent;

		// Token: 0x04008FF8 RID: 36856
		public float4 materials;

		// Token: 0x04008FF9 RID: 36857
		public float4 blend;

		// Token: 0x04008FFA RID: 36858
		public static readonly VertexAttributeDescriptor[] VertexBufferMemoryLayout = new VertexAttributeDescriptor[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float32, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.Float32, 4, 0)
		};
	}
}
