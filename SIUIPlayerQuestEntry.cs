using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200017B RID: 379
public class SIUIPlayerQuestEntry : MonoBehaviour
{
	// Token: 0x060009F4 RID: 2548 RVA: 0x00035BA2 File Offset: 0x00033DA2
	private void Awake()
	{
		this.lastQuestId = -1;
		this.lastQuestProgress = -1;
	}

	// Token: 0x04000C42 RID: 3138
	public Image background;

	// Token: 0x04000C43 RID: 3139
	public SIUIProgressBar progress;

	// Token: 0x04000C44 RID: 3140
	public TextMeshProUGUI questDescription;

	// Token: 0x04000C45 RID: 3141
	public GameObject completeOverlay;

	// Token: 0x04000C46 RID: 3142
	public GameObject questInfo;

	// Token: 0x04000C47 RID: 3143
	public GameObject noQuestAvailable;

	// Token: 0x04000C48 RID: 3144
	public GameObject newQuestTag;

	// Token: 0x04000C49 RID: 3145
	public int lastQuestId;

	// Token: 0x04000C4A RID: 3146
	public int lastQuestProgress;
}
