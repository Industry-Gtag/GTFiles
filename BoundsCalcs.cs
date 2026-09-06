using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000E89 RID: 3721
public class BoundsCalcs : MonoBehaviour
{
	// Token: 0x06005A60 RID: 23136 RVA: 0x001D570C File Offset: 0x001D390C
	public void Compute()
	{
		MeshFilter[] array;
		if (this.useRootMeshOnly)
		{
			BoundsCalcs.singleMesh[0] = base.GetComponent<MeshFilter>();
			array = BoundsCalcs.singleMesh;
		}
		else if (this.optionalTargets != null && this.optionalTargets.Length != 0)
		{
			array = base.GetComponentsInChildren<MeshFilter>().Concat(this.optionalTargets).ToArray<MeshFilter>();
		}
		else
		{
			array = base.GetComponentsInChildren<MeshFilter>();
		}
		List<Mesh> list = new List<Mesh>((array.Length + 1) / 2);
		List<Vector3> list2 = new List<Vector3>(array.Length * 512);
		this.elements.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			Matrix4x4 localToWorldMatrix = array[i].transform.localToWorldMatrix;
			Mesh mesh = array[i].sharedMesh;
			if (!mesh.isReadable)
			{
				Mesh mesh2 = mesh.CreateReadableMeshCopy();
				list.Add(mesh2);
				mesh = mesh2;
			}
			Vector3[] vertices = mesh.vertices;
			for (int j = 0; j < vertices.Length; j++)
			{
				vertices[j] = localToWorldMatrix.MultiplyPoint3x4(vertices[j]);
			}
			BoundsInfo boundsInfo = BoundsInfo.ComputeBounds(vertices);
			this.elements.Add(boundsInfo);
			list2.AddRange(vertices);
		}
		this.composite = BoundsInfo.ComputeBounds(list2.ToArray());
		list.ForEach(new Action<Mesh>(Object.DestroyImmediate));
	}

	// Token: 0x04006B7F RID: 27519
	public MeshFilter[] optionalTargets = new MeshFilter[0];

	// Token: 0x04006B80 RID: 27520
	public bool useRootMeshOnly;

	// Token: 0x04006B81 RID: 27521
	[Space]
	public List<BoundsInfo> elements = new List<BoundsInfo>();

	// Token: 0x04006B82 RID: 27522
	[Space]
	public BoundsInfo composite;

	// Token: 0x04006B83 RID: 27523
	[Space]
	private StateHash _state;

	// Token: 0x04006B84 RID: 27524
	private static MeshFilter[] singleMesh = new MeshFilter[1];
}
