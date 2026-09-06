using System;
using GameObjectScheduling;
using UnityEngine;

// Token: 0x02000421 RID: 1057
public class MeshMaterialReplacer : MonoBehaviour
{
	// Token: 0x06001913 RID: 6419 RVA: 0x0008E164 File Offset: 0x0008C364
	private void Start()
	{
		MeshRenderer meshRenderer;
		if (base.TryGetComponent<MeshRenderer>(out meshRenderer))
		{
			base.GetComponent<MeshFilter>().mesh = this.meshMaterialReplacement.mesh;
			meshRenderer.materials = this.meshMaterialReplacement.materials;
			return;
		}
		SkinnedMeshRenderer skinnedMeshRenderer;
		if (base.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
		{
			skinnedMeshRenderer.sharedMesh = this.meshMaterialReplacement.mesh;
			skinnedMeshRenderer.materials = this.meshMaterialReplacement.materials;
		}
	}

	// Token: 0x04002441 RID: 9281
	[SerializeField]
	private MeshMaterialReplacement meshMaterialReplacement;
}
