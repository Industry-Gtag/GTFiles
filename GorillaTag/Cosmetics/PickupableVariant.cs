using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012ED RID: 4845
	public class PickupableVariant : MonoBehaviour
	{
		// Token: 0x0600796B RID: 31083 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected internal virtual void Release(HoldableObject holdable, Vector3 startPosition, Vector3 releaseVelocity, float playerScale)
		{
		}

		// Token: 0x0600796C RID: 31084 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected internal virtual void Pickup(bool isAutoPickup = false)
		{
		}

		// Token: 0x0600796D RID: 31085 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected internal virtual void DelayedPickup()
		{
		}
	}
}
