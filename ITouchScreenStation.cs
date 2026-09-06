using System;
using UnityEngine;

// Token: 0x02000178 RID: 376
public interface ITouchScreenStation
{
	// Token: 0x170000DC RID: 220
	// (get) Token: 0x060009DF RID: 2527
	GameObject gameObject { get; }

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x060009E0 RID: 2528
	SIScreenRegion ScreenRegion { get; }

	// Token: 0x060009E1 RID: 2529
	void AddButton(SITouchscreenButton button, bool isPopupButton = false);

	// Token: 0x060009E2 RID: 2530
	void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr);

	// Token: 0x060009E3 RID: 2531
	void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn);
}
