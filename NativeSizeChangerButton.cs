using System;
using UnityEngine;

// Token: 0x0200042B RID: 1067
public class NativeSizeChangerButton : GorillaPressableButton
{
	// Token: 0x0600195B RID: 6491 RVA: 0x0008E9E9 File Offset: 0x0008CBE9
	public override void ButtonActivation()
	{
		this.nativeSizeChanger.Activate(this.settings);
	}

	// Token: 0x04002468 RID: 9320
	[SerializeField]
	private NativeSizeChanger nativeSizeChanger;

	// Token: 0x04002469 RID: 9321
	[SerializeField]
	private NativeSizeChangerSettings settings;
}
