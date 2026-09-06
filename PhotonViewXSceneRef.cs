using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020003C1 RID: 961
public class PhotonViewXSceneRef : MonoBehaviour
{
	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06001724 RID: 5924 RVA: 0x0008651C File Offset: 0x0008471C
	public PhotonView photonView
	{
		get
		{
			PhotonView photonView;
			if (this.reference.TryResolve<PhotonView>(out photonView))
			{
				return photonView;
			}
			return null;
		}
	}

	// Token: 0x04002257 RID: 8791
	[SerializeField]
	private XSceneRef reference;
}
