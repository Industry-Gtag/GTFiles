using System;

// Token: 0x0200039A RID: 922
public interface IRequestableOwnershipGuardCallbacks
{
	// Token: 0x06001673 RID: 5747
	void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer);

	// Token: 0x06001674 RID: 5748
	bool OnOwnershipRequest(NetPlayer fromPlayer);

	// Token: 0x06001675 RID: 5749
	void OnMyOwnerLeft();

	// Token: 0x06001676 RID: 5750
	bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer);

	// Token: 0x06001677 RID: 5751
	void OnMyCreatorLeft();
}
