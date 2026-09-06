using System;
using UnityEngine;

// Token: 0x020005F0 RID: 1520
public class SetStateConditional : StateMachineBehaviour
{
	// Token: 0x060025DC RID: 9692 RVA: 0x000C8EAB File Offset: 0x000C70AB
	private void OnValidate()
	{
		this._setToID = this.setToState;
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x000C8EBE File Offset: 0x000C70BE
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!this._didSetup)
		{
			this.parentAnimator = animator;
			this.Setup(animator, stateInfo, layerIndex);
			this._didSetup = true;
		}
		this._sinceEnter = TimeSince.Now();
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x000C8EEC File Offset: 0x000C70EC
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.delay > 0f && !this._sinceEnter.HasElapsed(this.delay, true))
		{
			return;
		}
		if (!this.CanSetState(animator, stateInfo, layerIndex))
		{
			return;
		}
		animator.Play(this._setToID);
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void Setup(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x060025E0 RID: 9696 RVA: 0x00023F0C File Offset: 0x0002210C
	protected virtual bool CanSetState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		return true;
	}

	// Token: 0x04003178 RID: 12664
	public Animator parentAnimator;

	// Token: 0x04003179 RID: 12665
	public string setToState;

	// Token: 0x0400317A RID: 12666
	[SerializeField]
	private AnimStateHash _setToID;

	// Token: 0x0400317B RID: 12667
	public float delay = 1f;

	// Token: 0x0400317C RID: 12668
	protected TimeSince _sinceEnter;

	// Token: 0x0400317D RID: 12669
	[NonSerialized]
	private bool _didSetup;
}
