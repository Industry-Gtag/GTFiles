using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000341 RID: 833
public class GorillaActionButton : GorillaPressableButton
{
	// Token: 0x0600147A RID: 5242 RVA: 0x0006E077 File Offset: 0x0006C277
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.onPress.Invoke();
	}

	// Token: 0x04001952 RID: 6482
	[SerializeField]
	public UnityEvent onPress;
}
