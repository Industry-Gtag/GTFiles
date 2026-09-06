using System;
using UnityEngine;

// Token: 0x020001DD RID: 477
public class BakeBlendShape : MonoBehaviour
{
	// Token: 0x06000CAE RID: 3246 RVA: 0x000458D4 File Offset: 0x00043AD4
	private void Update()
	{
		Mesh mesh = new Mesh();
		MeshCollider component = base.GetComponent<MeshCollider>();
		base.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
		component.sharedMesh = null;
		component.sharedMesh = mesh;
	}
}
