using System;
using UnityEngine;

// Token: 0x0200073A RID: 1850
[Serializable]
public class GRAbilityMoveToTarget : GRAbilityBase
{
	// Token: 0x06002EFB RID: 12027 RVA: 0x001006D8 File Offset: 0x000FE8D8
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		this.targetPos = agent.transform.position;
	}

	// Token: 0x06002EFC RID: 12028 RVA: 0x00100704 File Offset: 0x000FE904
	protected override void OnStart()
	{
		this.PlayAnim(this.animName, 0.3f, this.animSpeed);
		if (this.attributes && this.moveSpeed == 0f)
		{
			this.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		}
		this.agent.navAgent.speed = this.moveSpeed;
		this.targetPos = this.agent.transform.position;
		this.movementSound.Play(null);
	}

	// Token: 0x06002EFD RID: 12029 RVA: 0x0010078D File Offset: 0x000FE98D
	protected override void OnStop()
	{
		this.movementSound.Stop();
	}

	// Token: 0x06002EFE RID: 12030 RVA: 0x0010079C File Offset: 0x000FE99C
	public override bool IsDone()
	{
		return (this.targetPos - this.root.position).sqrMagnitude < 0.25f;
	}

	// Token: 0x06002EFF RID: 12031 RVA: 0x001007D0 File Offset: 0x000FE9D0
	protected override void OnUpdateShared(float dt)
	{
		if (this.target != null)
		{
			this.targetPos = this.target.position;
			this.agent.RequestDestination(this.targetPos);
		}
		Transform transform = ((this.lookAtTarget != null) ? this.lookAtTarget : this.target);
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, transform, this.maxTurnSpeed);
	}

	// Token: 0x06002F00 RID: 12032 RVA: 0x00100847 File Offset: 0x000FEA47
	public void SetTarget(Transform transform)
	{
		this.target = transform;
	}

	// Token: 0x06002F01 RID: 12033 RVA: 0x00100850 File Offset: 0x000FEA50
	public void SetTargetPos(Vector3 targetPos)
	{
		this.targetPos = targetPos;
		this.agent.RequestDestination(targetPos);
	}

	// Token: 0x06002F02 RID: 12034 RVA: 0x00100865 File Offset: 0x000FEA65
	public Vector3 GetTargetPos()
	{
		return this.targetPos;
	}

	// Token: 0x06002F03 RID: 12035 RVA: 0x0010086D File Offset: 0x000FEA6D
	public void SetLookAtTarget(Transform transform)
	{
		this.lookAtTarget = transform;
	}

	// Token: 0x04003C26 RID: 15398
	public float moveSpeed;

	// Token: 0x04003C27 RID: 15399
	public string animName;

	// Token: 0x04003C28 RID: 15400
	public float animSpeed = 1f;

	// Token: 0x04003C29 RID: 15401
	public float maxTurnSpeed = 360f;

	// Token: 0x04003C2A RID: 15402
	public AbilitySound movementSound;

	// Token: 0x04003C2B RID: 15403
	private Vector3 targetPos;

	// Token: 0x04003C2C RID: 15404
	private Transform target;

	// Token: 0x04003C2D RID: 15405
	private Transform lookAtTarget;
}
