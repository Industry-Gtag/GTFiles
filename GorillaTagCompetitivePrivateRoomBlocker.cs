using System;
using UnityEngine;

// Token: 0x020008AC RID: 2220
public class GorillaTagCompetitivePrivateRoomBlocker : MonoBehaviour
{
	// Token: 0x06003A49 RID: 14921 RVA: 0x0013D1CB File Offset: 0x0013B3CB
	private void Update()
	{
		this.blocker.SetActive(NetworkSystem.Instance.SessionIsPrivate);
	}

	// Token: 0x04004A3E RID: 19006
	[SerializeField]
	private GameObject blocker;
}
