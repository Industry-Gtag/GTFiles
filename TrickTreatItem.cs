using System;
using UnityEngine;

// Token: 0x02000D78 RID: 3448
public class TrickTreatItem : RandomComponent<MeshRenderer>
{
	// Token: 0x06005505 RID: 21765 RVA: 0x001BDF60 File Offset: 0x001BC160
	protected override void OnNextItem(MeshRenderer item)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			MeshRenderer meshRenderer = this.items[i];
			meshRenderer.enabled = meshRenderer == item;
		}
	}

	// Token: 0x06005506 RID: 21766 RVA: 0x001BDF94 File Offset: 0x001BC194
	public void Randomize()
	{
		this.NextItem();
	}
}
