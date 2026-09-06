using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000A6F RID: 2671
public class CustomMapsChaseBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x060044AE RID: 17582 RVA: 0x0016F710 File Offset: 0x0016D910
	public CustomMapsChaseBehaviour(CustomMapsAIBehaviourController AIController, AIAgent agentSettings)
	{
		this.sightOffset = agentSettings.sightOffset;
		this.rememberLoseSightPos = agentSettings.rememberLoseSightPosition;
		this.loseSightDist = agentSettings.loseSightDist;
		this.loseSightDistSq = this.loseSightDist * this.loseSightDist;
		this.stopDistSq = agentSettings.stopDist * agentSettings.stopDist;
		this.controller = AIController;
	}

	// Token: 0x060044AF RID: 17583 RVA: 0x0016F774 File Offset: 0x0016D974
	public override bool CanExecute()
	{
		return !this.controller.IsNull() && !this.controller.TargetPlayer.IsNull();
	}

	// Token: 0x060044B0 RID: 17584 RVA: 0x0016F79C File Offset: 0x0016D99C
	public override bool CanContinueExecuting()
	{
		if (!this.CanExecute())
		{
			return false;
		}
		bool flag;
		if (this.IsTargetInChaseRange(out flag))
		{
			return !flag;
		}
		if (!this.controller.IsTargetable(this.controller.TargetPlayer))
		{
			this.controller.StopMoving();
		}
		this.controller.ClearTarget();
		return false;
	}

	// Token: 0x060044B1 RID: 17585 RVA: 0x0016F7F4 File Offset: 0x0016D9F4
	public override void Execute()
	{
		bool flag;
		if (!this.IsTargetInChaseRange(out flag))
		{
			this.controller.ClearTarget();
			this.isChasing = false;
			if (!this.rememberLoseSightPos)
			{
				this.controller.StopMoving();
			}
			return;
		}
		if (!this.IsTargetVisible())
		{
			this.controller.ClearTarget();
			this.isChasing = false;
			if (!this.rememberLoseSightPos)
			{
				this.controller.StopMoving();
			}
			return;
		}
		if (flag && this.isChasing)
		{
			this.isChasing = false;
			this.controller.StopMoving();
			return;
		}
		this.isChasing = true;
		this.controller.RequestDestination(this.controller.TargetPlayer.transform.position);
	}

	// Token: 0x060044B2 RID: 17586 RVA: 0x0016F8A4 File Offset: 0x0016DAA4
	private bool IsTargetVisible()
	{
		Vector3 vector = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
		return this.controller.IsTargetVisible(vector, this.controller.TargetPlayer, this.loseSightDist);
	}

	// Token: 0x060044B3 RID: 17587 RVA: 0x0016F8FC File Offset: 0x0016DAFC
	private bool IsTargetInChaseRange(out bool withinStopDist)
	{
		withinStopDist = false;
		Vector3 vector;
		if (!this.controller.IsTargetInRange(this.controller.transform.position, this.controller.TargetPlayer, this.loseSightDistSq, out vector))
		{
			return false;
		}
		if (vector.sqrMagnitude < this.stopDistSq)
		{
			withinStopDist = true;
		}
		return true;
	}

	// Token: 0x060044B4 RID: 17588 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void NetExecute()
	{
	}

	// Token: 0x060044B5 RID: 17589 RVA: 0x0016F951 File Offset: 0x0016DB51
	public override void ResetBehavior()
	{
		this.isChasing = false;
	}

	// Token: 0x060044B6 RID: 17590 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnTriggerEnter(Collider otherCollider)
	{
	}

	// Token: 0x040056C5 RID: 22213
	private NavMeshAgent navMeshAgent;

	// Token: 0x040056C6 RID: 22214
	private CustomMapsAIBehaviourController controller;

	// Token: 0x040056C7 RID: 22215
	private float loseSightDist;

	// Token: 0x040056C8 RID: 22216
	private float loseSightDistSq;

	// Token: 0x040056C9 RID: 22217
	private Vector3 sightOffset;

	// Token: 0x040056CA RID: 22218
	private bool rememberLoseSightPos;

	// Token: 0x040056CB RID: 22219
	private float stopDistSq;

	// Token: 0x040056CC RID: 22220
	private bool isChasing;
}
