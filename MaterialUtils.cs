using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000800 RID: 2048
public static class MaterialUtils
{
	// Token: 0x0600345C RID: 13404 RVA: 0x0011FA72 File Offset: 0x0011DC72
	public static string GetTrimmedMaterialName(Material material)
	{
		return material.name.Replace(" (Instance)", "").Trim();
	}

	// Token: 0x0600345D RID: 13405 RVA: 0x0011FA90 File Offset: 0x0011DC90
	public static void SwapMaterial(MeshAndMaterials meshAndMaterial, bool isOnToOff)
	{
		List<Material> list;
		using (ListPool<Material>.Get(out list))
		{
			meshAndMaterial.meshRenderer.GetSharedMaterials(list);
			for (int i = 0; i < list.Count; i++)
			{
				string trimmedMaterialName = MaterialUtils.GetTrimmedMaterialName(list[i]);
				string text = (isOnToOff ? ((meshAndMaterial.onMaterial != null) ? MaterialUtils.GetTrimmedMaterialName(meshAndMaterial.onMaterial) : null) : ((meshAndMaterial.offMaterial != null) ? MaterialUtils.GetTrimmedMaterialName(meshAndMaterial.offMaterial) : null));
				if (text != null && trimmedMaterialName == text)
				{
					list[i] = (isOnToOff ? meshAndMaterial.offMaterial : meshAndMaterial.onMaterial);
				}
			}
			meshAndMaterial.meshRenderer.SetSharedMaterials(list);
		}
	}
}
