using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200136E RID: 4974
	public class WormInApple : MonoBehaviour
	{
		// Token: 0x06007C9B RID: 31899 RVA: 0x0028B4D8 File Offset: 0x002896D8
		public void OnHandTap()
		{
			if (this.blendShapeCosmetic && this.blendShapeCosmetic.GetBlendValue() > 0.5f)
			{
				UnityEvent onHandTapped = this.OnHandTapped;
				if (onHandTapped == null)
				{
					return;
				}
				onHandTapped.Invoke();
			}
		}

		// Token: 0x04008F34 RID: 36660
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04008F35 RID: 36661
		public UnityEvent OnHandTapped;
	}
}
