using System;
using UnityEngine;

// Token: 0x0200033C RID: 828
[RequireComponent(typeof(PlayerColoredCosmetic))]
public class FXModifierPlayerColorSetter : FXModifier
{
	// Token: 0x06001461 RID: 5217 RVA: 0x0006DC47 File Offset: 0x0006BE47
	public override void UpdateScale(float scale, Color color)
	{
		this.playerColoredCosmetic.UpdateColor(color);
	}

	// Token: 0x04001928 RID: 6440
	[SerializeField]
	private PlayerColoredCosmetic playerColoredCosmetic;
}
