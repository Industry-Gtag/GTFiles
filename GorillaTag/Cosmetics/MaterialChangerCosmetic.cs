using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001348 RID: 4936
	public class MaterialChangerCosmetic : MonoBehaviour
	{
		// Token: 0x06007BBC RID: 31676 RVA: 0x00286C44 File Offset: 0x00284E44
		public void ChangeMaterial(Material newMaterial)
		{
			if (this.targetRenderer == null || newMaterial == null || this.materialIndex < 0)
			{
				return;
			}
			Material[] materials = this.targetRenderer.materials;
			if (this.materialIndex >= materials.Length)
			{
				Debug.LogWarning(string.Format("Material index {0} is out of range.", this.materialIndex));
				return;
			}
			materials[this.materialIndex] = newMaterial;
			this.targetRenderer.materials = materials;
		}

		// Token: 0x06007BBD RID: 31677 RVA: 0x00286CBC File Offset: 0x00284EBC
		public void ChangeAllMaterials(Material newMat)
		{
			if (this.targetRenderer == null || newMat == null)
			{
				return;
			}
			Material[] array = new Material[this.targetRenderer.materials.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = newMat;
			}
			this.targetRenderer.materials = array;
		}

		// Token: 0x04008DCD RID: 36301
		[SerializeField]
		private SkinnedMeshRenderer targetRenderer;

		// Token: 0x04008DCE RID: 36302
		[SerializeField]
		private int materialIndex;
	}
}
