using System;
using UnityEngine;

// Token: 0x0200052F RID: 1327
public interface IHoldableObject
{
	// Token: 0x17000397 RID: 919
	// (get) Token: 0x0600214C RID: 8524
	GameObject gameObject { get; }

	// Token: 0x17000398 RID: 920
	// (get) Token: 0x0600214D RID: 8525
	// (set) Token: 0x0600214E RID: 8526
	string name { get; set; }

	// Token: 0x17000399 RID: 921
	// (get) Token: 0x0600214F RID: 8527
	bool TwoHanded { get; }

	// Token: 0x06002150 RID: 8528
	void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06002151 RID: 8529
	void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x06002152 RID: 8530
	bool OnRelease(DropZone zoneReleased, GameObject releasingHand);

	// Token: 0x06002153 RID: 8531
	void DropItemCleanup();
}
