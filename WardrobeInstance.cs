using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200058A RID: 1418
public class WardrobeInstance : MonoBehaviour
{
	// Token: 0x06002403 RID: 9219 RVA: 0x000C200F File Offset: 0x000C020F
	public void Start()
	{
		CosmeticsController.instance.AddWardrobeInstance(this);
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x000C201E File Offset: 0x000C021E
	public void OnDestroy()
	{
		CosmeticsController.instance.RemoveWardrobeInstance(this);
	}

	// Token: 0x04002F46 RID: 12102
	public WardrobeItemButton[] wardrobeItemButtons;

	// Token: 0x04002F47 RID: 12103
	public HeadModel selfDoll;
}
