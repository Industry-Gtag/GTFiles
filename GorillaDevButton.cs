using System;
using UnityEngine;

// Token: 0x02000A25 RID: 2597
public class GorillaDevButton : GorillaPressableButton
{
	// Token: 0x17000651 RID: 1617
	// (get) Token: 0x0600428A RID: 17034 RVA: 0x00162A71 File Offset: 0x00160C71
	// (set) Token: 0x0600428B RID: 17035 RVA: 0x00162A79 File Offset: 0x00160C79
	public bool on
	{
		get
		{
			return this.isOn;
		}
		set
		{
			if (this.isOn != value)
			{
				this.isOn = value;
				this.UpdateColor();
			}
		}
	}

	// Token: 0x0600428C RID: 17036 RVA: 0x00162A91 File Offset: 0x00160C91
	public new void OnEnable()
	{
		this.UpdateColor();
	}

	// Token: 0x04005442 RID: 21570
	public DevButtonType Type;

	// Token: 0x04005443 RID: 21571
	public LogType levelType;

	// Token: 0x04005444 RID: 21572
	public DevConsoleInstance targetConsole;

	// Token: 0x04005445 RID: 21573
	public int lineNumber;

	// Token: 0x04005446 RID: 21574
	public bool repeatIfHeld;

	// Token: 0x04005447 RID: 21575
	public float holdForSeconds;

	// Token: 0x04005448 RID: 21576
	private Coroutine pressCoroutine;
}
