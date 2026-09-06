using System;
using CjLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000738 RID: 1848
[Serializable]
public class GRAbilityJump : GRAbilityBase
{
	// Token: 0x06002EEA RID: 12010 RVA: 0x001002AA File Offset: 0x000FE4AA
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.isActive = false;
	}

	// Token: 0x06002EEB RID: 12011 RVA: 0x001002C4 File Offset: 0x000FE4C4
	public void SetupJump(Vector3 start, Vector3 end, float heightScale = 1f, float speedScale = 1f)
	{
		this.elapsedTime = 0f;
		this.startPos = start;
		this.endPos = end;
		float magnitude = (this.endPos - this.startPos).magnitude;
		this.controlPoint = (this.startPos + this.endPos) / 2f + new Vector3(0f, magnitude * heightScale, 0f);
		this.jumpTime = magnitude / (this.jumpSpeed * speedScale);
	}

	// Token: 0x06002EEC RID: 12012 RVA: 0x00100350 File Offset: 0x000FE550
	public void SetupJumpFromLinkData(OffMeshLinkData linkData)
	{
		if ((this.root.position - linkData.startPos).sqrMagnitude < (this.root.position - linkData.endPos).sqrMagnitude)
		{
			this.SetupJump(linkData.startPos, linkData.endPos, 1f, 1f);
			return;
		}
		this.SetupJump(linkData.endPos, linkData.startPos, 1f, 1f);
	}

	// Token: 0x06002EED RID: 12013 RVA: 0x001003DC File Offset: 0x000FE5DC
	protected override void OnStart()
	{
		this.elapsedTime = 0f;
		this.isActive = true;
		this.PlayAnim(this.animationData.animName, 0.05f, this.animationData.speed);
		this.agent.SetStopped(true);
		this.agent.SetDisableNetworkSync(true);
		this.agent.pauseEntityThink = true;
		this.soundJump.Play(this.audioSource);
	}

	// Token: 0x06002EEE RID: 12014 RVA: 0x00100454 File Offset: 0x000FE654
	protected override void OnStop()
	{
		this.agent.navAgent.Warp(this.endPos);
		this.agent.navAgent.CompleteOffMeshLink();
		this.agent.SetStopped(false);
		this.isActive = false;
		this.agent.SetDisableNetworkSync(false);
		this.agent.pauseEntityThink = false;
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x001004B3 File Offset: 0x000FE6B3
	public override bool IsDone()
	{
		return this.elapsedTime >= this.jumpTime;
	}

	// Token: 0x06002EF0 RID: 12016 RVA: 0x001004C6 File Offset: 0x000FE6C6
	public bool IsActive()
	{
		return this.isActive;
	}

	// Token: 0x06002EF1 RID: 12017 RVA: 0x001004D0 File Offset: 0x000FE6D0
	protected override void OnUpdateShared(float dt)
	{
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.startPos, this.controlPoint, Color.green, true);
			DebugUtil.DrawLine(this.endPos, this.controlPoint, Color.green, true);
		}
		float num = ((this.jumpTime > 0f) ? Math.Clamp(this.elapsedTime / this.jumpTime, 0f, 1f) : 1f);
		Vector3 vector = GRAbilityJump.EvaluateQuadratic(this.startPos, this.controlPoint, this.endPos, num);
		this.root.position = vector;
		if (this.rb != null)
		{
			this.rb.position = vector;
		}
		this.elapsedTime += dt;
	}

	// Token: 0x06002EF2 RID: 12018 RVA: 0x00100590 File Offset: 0x000FE790
	public static Vector3 EvaluateQuadratic(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		Vector3 vector = Vector3.Lerp(p0, p1, t);
		Vector3 vector2 = Vector3.Lerp(p1, p2, t);
		return Vector3.Lerp(vector, vector2, t);
	}

	// Token: 0x04003C17 RID: 15383
	private Vector3 startPos;

	// Token: 0x04003C18 RID: 15384
	private Vector3 endPos;

	// Token: 0x04003C19 RID: 15385
	private Vector3 controlPoint;

	// Token: 0x04003C1A RID: 15386
	[ReadOnly]
	public float jumpTime;

	// Token: 0x04003C1B RID: 15387
	[ReadOnly]
	public float elapsedTime;

	// Token: 0x04003C1C RID: 15388
	private bool isActive;

	// Token: 0x04003C1D RID: 15389
	public AnimationData animationData;

	// Token: 0x04003C1E RID: 15390
	public float jumpSpeed = 3f;

	// Token: 0x04003C1F RID: 15391
	public AbilitySound soundJump;
}
