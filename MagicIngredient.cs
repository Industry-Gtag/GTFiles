using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200091C RID: 2332
[Obsolete("replaced with ThrowableSetDressing.cs")]
public class MagicIngredient : TransferrableObject
{
	// Token: 0x06003D0E RID: 15630 RVA: 0x0014C55C File Offset: 0x0014A75C
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.item = this.worldShareableInstance;
		this.grabPtInitParent = this.anchor.transform.parent;
	}

	// Token: 0x06003D0F RID: 15631 RVA: 0x0014C588 File Offset: 0x0014A788
	private void ReParent()
	{
		Transform transform = this.anchor.transform;
		base.gameObject.transform.parent = transform;
		transform.parent = this.grabPtInitParent;
	}

	// Token: 0x06003D10 RID: 15632 RVA: 0x0014C5BE File Offset: 0x0014A7BE
	public void Disable()
	{
		this.DropItem();
		base.OnDisable();
		if (this.item)
		{
			this.item.OnDisable();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04004DC7 RID: 19911
	[FormerlySerializedAs("IngredientType")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04004DC8 RID: 19912
	public Transform rootParent;

	// Token: 0x04004DC9 RID: 19913
	private WorldShareableItem item;

	// Token: 0x04004DCA RID: 19914
	private Transform grabPtInitParent;
}
