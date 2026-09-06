using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010F5 RID: 4341
	public class GorillaNetworkLobbyJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x04007D39 RID: 32057
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x04007D3A RID: 32058
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04007D3B RID: 32059
		public string gameModeName;

		// Token: 0x04007D3C RID: 32060
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04007D3D RID: 32061
		public string componentTypeToRemove;

		// Token: 0x04007D3E RID: 32062
		public GameObject componentRemoveTarget;

		// Token: 0x04007D3F RID: 32063
		public string componentTypeToAdd;

		// Token: 0x04007D40 RID: 32064
		public GameObject componentAddTarget;

		// Token: 0x04007D41 RID: 32065
		public GameObject gorillaParent;

		// Token: 0x04007D42 RID: 32066
		public GameObject joinFailedBlock;
	}
}
