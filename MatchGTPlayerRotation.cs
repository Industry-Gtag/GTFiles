using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200035D RID: 861
public class MatchGTPlayerRotation : MonoBehaviour
{
	// Token: 0x06001514 RID: 5396 RVA: 0x000706A0 File Offset: 0x0006E8A0
	private void LateUpdate()
	{
		if (this.matchPosition)
		{
			base.transform.position = GTPlayer.Instance.mainCamera.transform.position;
		}
		if (this.matchRotation)
		{
			base.transform.rotation = GTPlayer.Instance.transform.rotation;
		}
	}

	// Token: 0x040019F8 RID: 6648
	public bool matchPosition;

	// Token: 0x040019F9 RID: 6649
	public bool matchRotation;
}
