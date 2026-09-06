using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000E6B RID: 3691
public class GTMeshData
{
	// Token: 0x06005A0A RID: 23050 RVA: 0x001D317C File Offset: 0x001D137C
	public GTMeshData(Mesh m)
	{
		this.mesh = m;
		this.subMeshCount = m.subMeshCount;
		this.vertices = m.vertices;
		this.triangles = m.triangles;
		this.normals = m.normals;
		this.tangents = m.tangents;
		this.colors32 = m.colors32;
		this.boneWeights = m.boneWeights;
		this.uv = m.uv;
		this.uv2 = m.uv2;
		this.uv3 = m.uv3;
		this.uv4 = m.uv4;
		this.uv5 = m.uv5;
		this.uv6 = m.uv6;
		this.uv7 = m.uv7;
		this.uv8 = m.uv8;
	}

	// Token: 0x06005A0B RID: 23051 RVA: 0x001D324C File Offset: 0x001D144C
	public Mesh ExtractSubmesh(int subMeshIndex, bool optimize = false)
	{
		if (subMeshIndex < 0 || subMeshIndex >= this.subMeshCount)
		{
			throw new IndexOutOfRangeException("subMeshIndex");
		}
		SubMeshDescriptor subMesh = this.mesh.GetSubMesh(subMeshIndex);
		int firstVertex = subMesh.firstVertex;
		int vertexCount = subMesh.vertexCount;
		MeshTopology topology = subMesh.topology;
		int[] indices = this.mesh.GetIndices(subMeshIndex, false);
		for (int i = 0; i < indices.Length; i++)
		{
			indices[i] -= firstVertex;
		}
		Mesh mesh = new Mesh();
		mesh.indexFormat = ((vertexCount > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
		mesh.SetVertices(this.vertices, firstVertex, vertexCount);
		mesh.SetIndices(indices, topology, 0);
		mesh.SetNormals(this.normals, firstVertex, vertexCount);
		mesh.SetTangents(this.tangents, firstVertex, vertexCount);
		if (!this.uv.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(0, this.uv, firstVertex, vertexCount);
		}
		if (!this.uv2.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(1, this.uv2, firstVertex, vertexCount);
		}
		if (!this.uv3.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(2, this.uv3, firstVertex, vertexCount);
		}
		if (!this.uv4.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(3, this.uv4, firstVertex, vertexCount);
		}
		if (!this.uv5.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(4, this.uv5, firstVertex, vertexCount);
		}
		if (!this.uv6.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(5, this.uv6, firstVertex, vertexCount);
		}
		if (!this.uv7.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(6, this.uv7, firstVertex, vertexCount);
		}
		if (!this.uv8.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(7, this.uv8, firstVertex, vertexCount);
		}
		if (optimize)
		{
			mesh.Optimize();
			mesh.OptimizeIndexBuffers();
		}
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x06005A0C RID: 23052 RVA: 0x001D341A File Offset: 0x001D161A
	public static GTMeshData Parse(Mesh mesh)
	{
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		return new GTMeshData(mesh);
	}

	// Token: 0x04006A5D RID: 27229
	public Mesh mesh;

	// Token: 0x04006A5E RID: 27230
	public Vector3[] vertices;

	// Token: 0x04006A5F RID: 27231
	public Vector3[] normals;

	// Token: 0x04006A60 RID: 27232
	public Vector4[] tangents;

	// Token: 0x04006A61 RID: 27233
	public Color32[] colors32;

	// Token: 0x04006A62 RID: 27234
	public int[] triangles;

	// Token: 0x04006A63 RID: 27235
	public BoneWeight[] boneWeights;

	// Token: 0x04006A64 RID: 27236
	public Vector2[] uv;

	// Token: 0x04006A65 RID: 27237
	public Vector2[] uv2;

	// Token: 0x04006A66 RID: 27238
	public Vector2[] uv3;

	// Token: 0x04006A67 RID: 27239
	public Vector2[] uv4;

	// Token: 0x04006A68 RID: 27240
	public Vector2[] uv5;

	// Token: 0x04006A69 RID: 27241
	public Vector2[] uv6;

	// Token: 0x04006A6A RID: 27242
	public Vector2[] uv7;

	// Token: 0x04006A6B RID: 27243
	public Vector2[] uv8;

	// Token: 0x04006A6C RID: 27244
	public int subMeshCount;
}
