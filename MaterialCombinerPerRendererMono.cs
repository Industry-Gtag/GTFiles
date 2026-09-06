using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000387 RID: 903
public class MaterialCombinerPerRendererMono : MonoBehaviour
{
	// Token: 0x060015FA RID: 5626 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected void Awake()
	{
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x00074D48 File Offset: 0x00072F48
	public void AddEntry(Renderer r, int slot, int sliceIndex, Color baseColor, Material oldMat)
	{
		this.slotData.Add(new MaterialCombinerPerRendererInfo
		{
			renderer = r,
			slotIndex = slot,
			sliceIndex = sliceIndex,
			baseColor = baseColor,
			oldMat = oldMat
		});
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x00074D94 File Offset: 0x00072F94
	public bool TryGetData(Renderer r, int slot, out MaterialCombinerPerRendererInfo data)
	{
		foreach (MaterialCombinerPerRendererInfo materialCombinerPerRendererInfo in this.slotData)
		{
			if (materialCombinerPerRendererInfo.renderer == r && materialCombinerPerRendererInfo.slotIndex == slot)
			{
				data = materialCombinerPerRendererInfo;
				return true;
			}
		}
		data = default(MaterialCombinerPerRendererInfo);
		return false;
	}

	// Token: 0x04001ADB RID: 6875
	public List<MaterialCombinerPerRendererInfo> slotData = new List<MaterialCombinerPerRendererInfo>();
}
