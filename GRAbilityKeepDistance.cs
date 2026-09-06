using System;
using CjLib;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000742 RID: 1858
[Serializable]
public class GRAbilityKeepDistance : GRAbilityBase
{
	// Token: 0x06002F3E RID: 12094 RVA: 0x001016C8 File Offset: 0x000FF8C8
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.navMeshAgent = agent.GetComponent<NavMeshAgent>();
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.attributes && this.moveAbility.moveSpeed == 0f)
		{
			this.moveAbility.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.BackupSpeed);
		}
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x0010173C File Offset: 0x000FF93C
	protected override void OnStart()
	{
		if (this.target != null)
		{
			Vector3 vector = this.agent.transform.position - this.target.position;
			if (this.maxDistanceFromTarget > 0f && vector.magnitude > this.maxDistanceFromTarget)
			{
				this.agent.SetStopped(true);
				this.PlayAnim(this.idleAnimName, 0.5f, 1f);
				this.idleSound.Play(null);
			}
			else
			{
				this.moveAbility.Start();
			}
		}
		else
		{
			this.moveAbility.Start();
		}
		this.agent.SetIsPathing(true, true);
		Vector3 vector2 = this.PickBackupDestination();
		this.moveAbility.SetTargetPos(vector2);
		if (this.navMeshAgent != null)
		{
			this.defaultUpdateRotation = this.navMeshAgent.updateRotation;
			this.navMeshAgent.updateRotation = false;
		}
	}

	// Token: 0x06002F40 RID: 12096 RVA: 0x00101828 File Offset: 0x000FFA28
	protected override void OnStop()
	{
		this.moveAbility.Stop();
		this.idleSound.Stop();
		if (this.navMeshAgent != null)
		{
			this.navMeshAgent.updateRotation = this.defaultUpdateRotation;
		}
		this.agent.SetStopped(false);
	}

	// Token: 0x06002F41 RID: 12097 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002F42 RID: 12098 RVA: 0x00101878 File Offset: 0x000FFA78
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
				this.moveAbility.SetLookAtTarget(this.target);
			}
		}
	}

	// Token: 0x06002F43 RID: 12099 RVA: 0x001018CC File Offset: 0x000FFACC
	protected override void OnThink(float dt)
	{
		Vector3 vector = this.agent.transform.position - this.target.position;
		if (this.moveAbility.IsDone())
		{
			if (this.maxDistanceFromTarget < 0f || vector.magnitude < this.maxDistanceFromTarget)
			{
				if (this.navMeshAgent != null && this.navMeshAgent.isOnNavMesh && this.navMeshAgent.isStopped)
				{
					this.idleSound.Stop();
					this.moveAbility.Start();
				}
				Vector3 vector2 = this.PickBackupDestination();
				this.moveAbility.SetTargetPos(vector2);
				return;
			}
		}
		else if (this.maxDistanceFromTarget > 0f && vector.magnitude > this.maxDistanceFromTarget)
		{
			this.moveAbility.SetTargetPos(this.root.position);
			this.moveAbility.Stop();
			this.agent.SetStopped(true);
			this.PlayAnim(this.idleAnimName, 0.5f, 1f);
			this.idleSound.Play(null);
		}
	}

	// Token: 0x06002F44 RID: 12100 RVA: 0x001019E4 File Offset: 0x000FFBE4
	private Vector3 PickBackupDestination()
	{
		Vector3 position = this.agent.transform.position;
		if (this.target == null)
		{
			return position;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(position, out navMeshHit, 1f, this.walkableArea))
		{
			Vector3 position2 = navMeshHit.position;
			Vector3 vector = this.agent.transform.position - this.target.position;
			vector.y = 0f;
			Vector3 normalized = vector.normalized;
			int i = 0;
			while (i < GRAbilityKeepDistance.rotations.Length)
			{
				Vector3 vector2 = GRAbilityKeepDistance.rotations[i] * normalized;
				float num = 2f;
				Vector3 vector3 = position2 + vector2 * num;
				NavMeshHit navMeshHit2;
				if (!NavMesh.Raycast(position2, vector3, out navMeshHit2, this.walkableArea))
				{
					goto IL_00D6;
				}
				if (navMeshHit2.distance >= this.minBackupSpaceRequired)
				{
					vector3 = navMeshHit2.position;
					goto IL_00D6;
				}
				IL_0128:
				i++;
				continue;
				IL_00D6:
				NavMeshHit navMeshHit3;
				if (!NavMesh.SamplePosition(vector3, out navMeshHit3, 1f, this.walkableArea))
				{
					goto IL_0128;
				}
				Vector3 position3 = navMeshHit3.position;
				Vector3 vector4 = position3 - this.target.position;
				vector4.y = 0f;
				if (vector4.sqrMagnitude > vector.sqrMagnitude)
				{
					return position3;
				}
				goto IL_0128;
			}
		}
		return position;
	}

	// Token: 0x06002F45 RID: 12101 RVA: 0x00101B2E File Offset: 0x000FFD2E
	protected override void OnUpdateShared(float dt)
	{
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.magenta, true);
		}
	}

	// Token: 0x06002F46 RID: 12102 RVA: 0x00101B58 File Offset: 0x000FFD58
	protected override void OnUpdateAuthority(float dt)
	{
		this.moveAbility.UpdateAuthority(dt);
	}

	// Token: 0x06002F47 RID: 12103 RVA: 0x00101B66 File Offset: 0x000FFD66
	protected override void OnUpdateRemote(float dt)
	{
		this.moveAbility.UpdateRemote(dt);
	}

	// Token: 0x04003C5B RID: 15451
	private NavMeshAgent navMeshAgent;

	// Token: 0x04003C5C RID: 15452
	private Transform target;

	// Token: 0x04003C5D RID: 15453
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x04003C5E RID: 15454
	public string idleAnimName;

	// Token: 0x04003C5F RID: 15455
	public AbilitySound idleSound;

	// Token: 0x04003C60 RID: 15456
	public float minBackupSpaceRequired = 0.5f;

	// Token: 0x04003C61 RID: 15457
	public float maxDistanceFromTarget = -1f;

	// Token: 0x04003C62 RID: 15458
	private bool defaultUpdateRotation;

	// Token: 0x04003C63 RID: 15459
	private static Quaternion[] rotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 30f, 0f),
		Quaternion.Euler(0f, -30f, 0f),
		Quaternion.Euler(0f, 60f, 0f),
		Quaternion.Euler(0f, -60f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, -90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, -135f, 0f),
		Quaternion.Euler(0f, 180f, 0f)
	};
}
