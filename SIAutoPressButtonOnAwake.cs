using System;
using UnityEngine;

// Token: 0x02000139 RID: 313
public class SIAutoPressButtonOnAwake : MonoBehaviour
{
	// Token: 0x060007CE RID: 1998 RVA: 0x0002AC0A File Offset: 0x00028E0A
	private void Awake()
	{
		this.button = base.GetComponent<SITouchscreenButton>();
		this.terminalParent = this.button.GetComponentInParent<SICombinedTerminal>();
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0002AC29 File Offset: 0x00028E29
	private void OnEnable()
	{
		if (this.button == null)
		{
			return;
		}
		this.awakeTime = Time.time;
		this.buttonPressed = false;
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0002AC4C File Offset: 0x00028E4C
	private void Update()
	{
		if (this.buttonPressed || Time.time < this.awakeTime + this.delay)
		{
			return;
		}
		if (this.terminalParent.activePlayer.ActorNr == SIPlayer.LocalPlayer.ActorNr)
		{
			this.button.PressButton();
		}
		this.buttonPressed = true;
	}

	// Token: 0x040009DD RID: 2525
	private SICombinedTerminal terminalParent;

	// Token: 0x040009DE RID: 2526
	private SITouchscreenButton button;

	// Token: 0x040009DF RID: 2527
	private float awakeTime;

	// Token: 0x040009E0 RID: 2528
	private bool buttonPressed;

	// Token: 0x040009E1 RID: 2529
	public float delay = 2f;
}
