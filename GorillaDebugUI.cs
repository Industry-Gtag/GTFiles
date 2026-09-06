using System;
using TMPro;
using UnityEngine;

// Token: 0x020005BC RID: 1468
public class GorillaDebugUI : MonoBehaviour
{
	// Token: 0x0400309A RID: 12442
	private readonly float Delay = 0.5f;

	// Token: 0x0400309B RID: 12443
	public GameObject parentCanvas;

	// Token: 0x0400309C RID: 12444
	public GameObject rayInteractorLeft;

	// Token: 0x0400309D RID: 12445
	public GameObject rayInteractorRight;

	// Token: 0x0400309E RID: 12446
	[SerializeField]
	private TMP_Dropdown playfabIdDropdown;

	// Token: 0x0400309F RID: 12447
	[SerializeField]
	private TMP_Dropdown roomIdDropdown;

	// Token: 0x040030A0 RID: 12448
	[SerializeField]
	private TMP_Dropdown locationDropdown;

	// Token: 0x040030A1 RID: 12449
	[SerializeField]
	private TMP_Dropdown playerNameDropdown;

	// Token: 0x040030A2 RID: 12450
	[SerializeField]
	private TMP_Dropdown gameModeDropdown;

	// Token: 0x040030A3 RID: 12451
	[SerializeField]
	private TMP_Dropdown timeOfDayDropdown;

	// Token: 0x040030A4 RID: 12452
	[SerializeField]
	private TMP_Text networkStateTextBox;

	// Token: 0x040030A5 RID: 12453
	[SerializeField]
	private TMP_Text gameModeTextBox;

	// Token: 0x040030A6 RID: 12454
	[SerializeField]
	private TMP_Text currentRoomTextBox;

	// Token: 0x040030A7 RID: 12455
	[SerializeField]
	private TMP_Text playerCountTextBox;

	// Token: 0x040030A8 RID: 12456
	[SerializeField]
	private TMP_Text roomVisibilityTextBox;

	// Token: 0x040030A9 RID: 12457
	[SerializeField]
	private TMP_Text timeMultiplierTextBox;

	// Token: 0x040030AA RID: 12458
	[SerializeField]
	private TMP_Text versionTextBox;
}
