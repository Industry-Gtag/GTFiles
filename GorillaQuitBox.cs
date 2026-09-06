using System;
using UnityEngine;

// Token: 0x020005E0 RID: 1504
public class GorillaQuitBox : GorillaTriggerBox
{
	// Token: 0x060025B2 RID: 9650 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Start()
	{
	}

	// Token: 0x060025B3 RID: 9651 RVA: 0x000C87BD File Offset: 0x000C69BD
	public override void OnBoxTriggered()
	{
		Debug.Log("quitbox hit! hopefully you expected this to happen!");
		Application.Quit();
	}
}
