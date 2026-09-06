using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000379 RID: 889
[Serializable]
public struct GTRendererMatSlot
{
	// Token: 0x1700022D RID: 557
	// (get) Token: 0x060015D2 RID: 5586 RVA: 0x0007361B File Offset: 0x0007181B
	// (set) Token: 0x060015D3 RID: 5587 RVA: 0x00073623 File Offset: 0x00071823
	public bool isValid { readonly get; private set; }

	// Token: 0x060015D4 RID: 5588 RVA: 0x0007362C File Offset: 0x0007182C
	public bool TryInitialize()
	{
		this.isValid = this.renderer != null;
		if (!this.isValid)
		{
			return false;
		}
		List<Material> list;
		bool isValid;
		using (ListPool<Material>.Get(out list))
		{
			this.renderer.GetSharedMaterials(list);
			this.isValid = this.slot >= 0 && this.slot < list.Count && list[this.slot] != null;
			isValid = this.isValid;
		}
		return isValid;
	}

	// Token: 0x04001A9F RID: 6815
	public Renderer renderer;

	// Token: 0x04001AA0 RID: 6816
	public int slot;
}
