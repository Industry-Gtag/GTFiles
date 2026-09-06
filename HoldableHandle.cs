using System;
using UnityEngine;

// Token: 0x0200052D RID: 1325
public class HoldableHandle : InteractionPoint
{
	// Token: 0x17000394 RID: 916
	// (get) Token: 0x0600213F RID: 8511 RVA: 0x000B1EFB File Offset: 0x000B00FB
	public new HoldableObject Holdable
	{
		get
		{
			return this.holdable;
		}
	}

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x06002140 RID: 8512 RVA: 0x000B1F03 File Offset: 0x000B0103
	public CapsuleCollider Capsule
	{
		get
		{
			return this.handleCapsuleTrigger;
		}
	}

	// Token: 0x04002C16 RID: 11286
	[SerializeField]
	private HoldableObject holdable;

	// Token: 0x04002C17 RID: 11287
	[SerializeField]
	private CapsuleCollider handleCapsuleTrigger;
}
