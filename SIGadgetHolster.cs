using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200010D RID: 269
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
public class SIGadgetHolster : SIGadget, I_SIDisruptable
{
	// Token: 0x06000661 RID: 1633 RVA: 0x00023B33 File Offset: 0x00021D33
	private void Start()
	{
		this.gtPlayer = GTPlayer.Instance;
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Disrupt(float disruptTime)
	{
	}

	// Token: 0x040007C7 RID: 1991
	[SerializeField]
	private Image imageMask;

	// Token: 0x040007C8 RID: 1992
	public List<SuperInfectionSnapPoint> snapPoints;

	// Token: 0x040007C9 RID: 1993
	private SIGadgetHolster.State state;

	// Token: 0x040007CA RID: 1994
	private GTPlayer gtPlayer;

	// Token: 0x0200010E RID: 270
	private enum State
	{
		// Token: 0x040007CC RID: 1996
		Unequipped,
		// Token: 0x040007CD RID: 1997
		Equipped
	}
}
