using System;
using UnityEngine;

// Token: 0x02000D7B RID: 3451
public static class AnimatorUtils
{
	// Token: 0x06005517 RID: 21783 RVA: 0x001BE328 File Offset: 0x001BC528
	public static void ResetToEntryState(this Animator a)
	{
		if (a == null)
		{
			return;
		}
		a.Rebind();
		a.Update(0f);
	}
}
