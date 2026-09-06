using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010F0 RID: 4336
	public class GorillaNetworkDisconnectTrigger : GorillaTriggerBox
	{
		// Token: 0x06006CCE RID: 27854 RVA: 0x00232CD8 File Offset: 0x00230ED8
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (this.makeSureThisIsEnabled != null)
			{
				this.makeSureThisIsEnabled.SetActive(true);
			}
			GameObject[] array = this.makeSureTheseAreEnabled;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.componentTypeToRemove != "" && this.componentTarget.GetComponent(this.componentTypeToRemove) != null)
				{
					Object.Destroy(this.componentTarget.GetComponent(this.componentTypeToRemove));
				}
				PhotonNetwork.Disconnect();
				SkinnedMeshRenderer[] array2 = this.photonNetworkController.offlineVRRig;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].enabled = true;
				}
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		// Token: 0x04007D1E RID: 32030
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04007D1F RID: 32031
		public GameObject offlineVRRig;

		// Token: 0x04007D20 RID: 32032
		public GameObject makeSureThisIsEnabled;

		// Token: 0x04007D21 RID: 32033
		public GameObject[] makeSureTheseAreEnabled;

		// Token: 0x04007D22 RID: 32034
		public string componentTypeToRemove;

		// Token: 0x04007D23 RID: 32035
		public GameObject componentTarget;
	}
}
