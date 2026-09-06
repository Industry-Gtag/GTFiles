using System;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000B9D RID: 2973
public class AnimationPauser : StateMachineBehaviour
{
	// Token: 0x06004B18 RID: 19224 RVA: 0x00191468 File Offset: 0x0018F668
	public override async void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		this._animPauseDuration = Random.Range(this._minTimeBetweenAnims, this._maxTimeBetweenAnims);
		await Task.Delay(this._animPauseDuration * 1000);
		animator.SetTrigger(AnimationPauser.Restart_Anim_Name);
	}

	// Token: 0x04005DF3 RID: 24051
	[SerializeField]
	private int _maxTimeBetweenAnims = 5;

	// Token: 0x04005DF4 RID: 24052
	[SerializeField]
	private int _minTimeBetweenAnims = 1;

	// Token: 0x04005DF5 RID: 24053
	private int _animPauseDuration;

	// Token: 0x04005DF6 RID: 24054
	private static readonly string Restart_Anim_Name = "RestartAnim";
}
