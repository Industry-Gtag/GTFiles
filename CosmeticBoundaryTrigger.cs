using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000506 RID: 1286
public class CosmeticBoundaryTrigger : GorillaTriggerBox
{
	// Token: 0x06002044 RID: 8260 RVA: 0x000AD9D0 File Offset: 0x000ABBD0
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null)
		{
			return;
		}
		if (CosmeticBoundaryTrigger.sinceLastTryOnEvent.HasElapsed(0.5f, true))
		{
			GorillaTelemetry.PostShopEvent(this.rigRef, GTShopEventType.item_try_on, this.rigRef.tryOnSet.items);
		}
		this.rigRef.inTryOnRoom = true;
		this.rigRef.LocalUpdateCosmeticsWithTryon(this.rigRef.cosmeticSet, this.rigRef.tryOnSet, false);
		this.rigRef.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x06002045 RID: 8261 RVA: 0x000ADA80 File Offset: 0x000ABC80
	public void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null)
		{
			return;
		}
		this.rigRef.inTryOnRoom = false;
		if (this.rigRef.isOfflineVRRig)
		{
			this.rigRef.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			CosmeticsController.instance.ClearCheckout(false);
			CosmeticsController.instance.UpdateShoppingCart();
			CosmeticsController.instance.UpdateWornCosmetics(true);
			CosmeticsController.ClearTryOnCollectable();
		}
		this.rigRef.LocalUpdateCosmeticsWithTryon(this.rigRef.cosmeticSet, this.rigRef.tryOnSet, false);
		this.rigRef.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x04002B10 RID: 11024
	public VRRig rigRef;

	// Token: 0x04002B11 RID: 11025
	private static TimeSince sinceLastTryOnEvent = 0f;
}
