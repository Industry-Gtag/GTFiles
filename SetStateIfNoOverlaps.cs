using System;
using UnityEngine;

// Token: 0x020005F1 RID: 1521
public class SetStateIfNoOverlaps : SetStateConditional
{
	// Token: 0x060025E2 RID: 9698 RVA: 0x000C8F4B File Offset: 0x000C714B
	protected override void Setup(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this._volume = animator.GetComponent<VolumeCast>();
	}

	// Token: 0x060025E3 RID: 9699 RVA: 0x000C8F59 File Offset: 0x000C7159
	protected override bool CanSetState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		bool flag = this._volume.CheckOverlaps();
		if (flag)
		{
			this._sinceEnter = 0f;
		}
		return !flag;
	}

	// Token: 0x0400317E RID: 12670
	public VolumeCast _volume;
}
