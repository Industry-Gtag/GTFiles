using System;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class SIScannableHand : MonoBehaviour
{
	// Token: 0x06000953 RID: 2387 RVA: 0x00032565 File Offset: 0x00030765
	private void Awake()
	{
		this.parentPlayer = base.GetComponentInParent<SIPlayer>();
	}

	// Token: 0x04000B6A RID: 2922
	public SIPlayer parentPlayer;
}
