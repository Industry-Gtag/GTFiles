using System;
using UnityEngine;

// Token: 0x02000858 RID: 2136
public class SafeOwnershipRequestsCallbacks : MonoBehaviour, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x06003712 RID: 14098 RVA: 0x0012EC10 File Offset: 0x0012CE10
	private void Awake()
	{
		this._requestableOwnershipGuard.AddCallbackTarget(this);
	}

	// Token: 0x06003713 RID: 14099 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IRequestableOwnershipGuardCallbacks.OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
	}

	// Token: 0x06003714 RID: 14100 RVA: 0x00002076 File Offset: 0x00000276
	bool IRequestableOwnershipGuardCallbacks.OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x06003715 RID: 14101 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IRequestableOwnershipGuardCallbacks.OnMyOwnerLeft()
	{
	}

	// Token: 0x06003716 RID: 14102 RVA: 0x00002076 File Offset: 0x00000276
	bool IRequestableOwnershipGuardCallbacks.OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x06003717 RID: 14103 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IRequestableOwnershipGuardCallbacks.OnMyCreatorLeft()
	{
	}

	// Token: 0x04004786 RID: 18310
	[SerializeField]
	private RequestableOwnershipGuard _requestableOwnershipGuard;
}
