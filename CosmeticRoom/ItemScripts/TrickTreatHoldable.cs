using System;
using UnityEngine;

namespace CosmeticRoom.ItemScripts
{
	// Token: 0x0200108E RID: 4238
	public class TrickTreatHoldable : TransferrableObject
	{
		// Token: 0x060069D0 RID: 27088 RVA: 0x00220D55 File Offset: 0x0021EF55
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.candyCollider)
			{
				this.candyCollider.enabled = this.IsMyItem() && this.IsHeld();
			}
		}

		// Token: 0x0400798A RID: 31114
		public MeshCollider candyCollider;
	}
}
