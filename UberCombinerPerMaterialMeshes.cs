using System;
using UnityEngine;

// Token: 0x02000E70 RID: 3696
public class UberCombinerPerMaterialMeshes : MonoBehaviour
{
	// Token: 0x04006A92 RID: 27282
	public GameObject rootObject;

	// Token: 0x04006A93 RID: 27283
	public bool deleteSelfOnPrefabBake;

	// Token: 0x04006A94 RID: 27284
	[Space]
	public GameObject[] objects = new GameObject[0];

	// Token: 0x04006A95 RID: 27285
	public MeshRenderer[] renderers = new MeshRenderer[0];

	// Token: 0x04006A96 RID: 27286
	public MeshFilter[] filters = new MeshFilter[0];

	// Token: 0x04006A97 RID: 27287
	public Material[] materials = new Material[0];
}
