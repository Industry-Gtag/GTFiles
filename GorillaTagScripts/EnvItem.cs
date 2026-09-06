using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F82 RID: 3970
	public class EnvItem : MonoBehaviour, IPunInstantiateMagicCallback
	{
		// Token: 0x0600627D RID: 25213 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnEnable()
		{
		}

		// Token: 0x0600627E RID: 25214 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnDisable()
		{
		}

		// Token: 0x0600627F RID: 25215 RVA: 0x001FB22C File Offset: 0x001F942C
		public void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			object[] instantiationData = info.photonView.InstantiationData;
			this.spawnedByPhotonViewId = (int)instantiationData[0];
		}

		// Token: 0x0400713F RID: 28991
		public int spawnedByPhotonViewId;
	}
}
