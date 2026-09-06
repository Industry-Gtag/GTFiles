using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000321 RID: 801
public class DevConsoleInstance : MonoBehaviour
{
	// Token: 0x06001407 RID: 5127 RVA: 0x00044B04 File Offset: 0x00042D04
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x040018D4 RID: 6356
	public GorillaDevButton[] buttons;

	// Token: 0x040018D5 RID: 6357
	public GameObject[] disableWhileActive;

	// Token: 0x040018D6 RID: 6358
	public GameObject[] enableWhileActive;

	// Token: 0x040018D7 RID: 6359
	public float maxHeight;

	// Token: 0x040018D8 RID: 6360
	public float lineHeight;

	// Token: 0x040018D9 RID: 6361
	public int targetLogIndex = -1;

	// Token: 0x040018DA RID: 6362
	public int currentLogIndex;

	// Token: 0x040018DB RID: 6363
	public int expandAmount = 20;

	// Token: 0x040018DC RID: 6364
	public int expandedMessageIndex = -1;

	// Token: 0x040018DD RID: 6365
	public bool canExpand = true;

	// Token: 0x040018DE RID: 6366
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x040018DF RID: 6367
	public HashSet<LogType> selectedLogTypes = new HashSet<LogType>
	{
		LogType.Error,
		LogType.Exception,
		LogType.Log,
		LogType.Warning,
		LogType.Assert
	};

	// Token: 0x040018E0 RID: 6368
	[SerializeField]
	private GorillaDevButton[] logTypeButtons;

	// Token: 0x040018E1 RID: 6369
	[SerializeField]
	private GorillaDevButton BottomButton;

	// Token: 0x040018E2 RID: 6370
	public float lineStartHeight;

	// Token: 0x040018E3 RID: 6371
	public float lineStartZ;

	// Token: 0x040018E4 RID: 6372
	public float textStartHeight;

	// Token: 0x040018E5 RID: 6373
	public float lineStartTextWidth;

	// Token: 0x040018E6 RID: 6374
	public double textScale = 0.5;

	// Token: 0x040018E7 RID: 6375
	public bool isEnabled = true;

	// Token: 0x040018E8 RID: 6376
	[SerializeField]
	private GameObject ConsoleLineExample;
}
