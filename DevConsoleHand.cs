using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000320 RID: 800
public class DevConsoleHand : DevConsoleInstance
{
	// Token: 0x040018C3 RID: 6339
	public List<GameObject> otherButtonsList;

	// Token: 0x040018C4 RID: 6340
	public bool isStillEnabled = true;

	// Token: 0x040018C5 RID: 6341
	public bool isLeftHand;

	// Token: 0x040018C6 RID: 6342
	public ConsoleMode mode;

	// Token: 0x040018C7 RID: 6343
	public double debugScale;

	// Token: 0x040018C8 RID: 6344
	public double inspectorScale;

	// Token: 0x040018C9 RID: 6345
	public double componentInspectorScale;

	// Token: 0x040018CA RID: 6346
	public List<GameObject> consoleButtons;

	// Token: 0x040018CB RID: 6347
	public List<GameObject> inspectorButtons;

	// Token: 0x040018CC RID: 6348
	public List<GameObject> componentInspectorButtons;

	// Token: 0x040018CD RID: 6349
	public GorillaDevButton consoleButton;

	// Token: 0x040018CE RID: 6350
	public GorillaDevButton inspectorButton;

	// Token: 0x040018CF RID: 6351
	public GorillaDevButton componentInspectorButton;

	// Token: 0x040018D0 RID: 6352
	public GorillaDevButton showNonStarItems;

	// Token: 0x040018D1 RID: 6353
	public GorillaDevButton showPrivateItems;

	// Token: 0x040018D2 RID: 6354
	public Text componentInspectionText;

	// Token: 0x040018D3 RID: 6355
	public DevInspector selectedInspector;
}
