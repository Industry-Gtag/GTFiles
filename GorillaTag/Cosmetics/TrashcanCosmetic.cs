using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012F7 RID: 4855
	public class TrashcanCosmetic : MonoBehaviour
	{
		// Token: 0x060079BC RID: 31164 RVA: 0x0027B6CC File Offset: 0x002798CC
		public void OnBasket(bool isLeftHand, Collider other)
		{
			SlingshotProjectile slingshotProjectile;
			if (other.TryGetComponent<SlingshotProjectile>(out slingshotProjectile) && slingshotProjectile.GetDistanceTraveled() >= this.minScoringDistance)
			{
				UnityEvent onScored = this.OnScored;
				if (onScored != null)
				{
					onScored.Invoke();
				}
				slingshotProjectile.DestroyAfterRelease();
			}
		}

		// Token: 0x04008B05 RID: 35589
		public float minScoringDistance = 2f;

		// Token: 0x04008B06 RID: 35590
		public UnityEvent OnScored;
	}
}
