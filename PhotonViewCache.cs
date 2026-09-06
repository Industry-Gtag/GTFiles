using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000851 RID: 2129
public class PhotonViewCache : MonoBehaviour, IPunInstantiateMagicCallback
{
	// Token: 0x170004CF RID: 1231
	// (get) Token: 0x060036CC RID: 14028 RVA: 0x0012E088 File Offset: 0x0012C288
	// (set) Token: 0x060036CD RID: 14029 RVA: 0x0012E090 File Offset: 0x0012C290
	public bool Initialized { get; private set; }

	// Token: 0x060036CE RID: 14030 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x0400475C RID: 18268
	private PhotonView[] m_photonViews;

	// Token: 0x0400475D RID: 18269
	[SerializeField]
	private bool m_isRoomObject;
}
