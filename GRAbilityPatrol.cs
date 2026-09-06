using System;
using CjLib;
using Photon.Pun;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000743 RID: 1859
[Serializable]
public class GRAbilityPatrol : GRAbilityBase
{
	// Token: 0x06002F4A RID: 12106 RVA: 0x00101CBC File Offset: 0x000FFEBC
	public bool HasValidPatrolPath()
	{
		return this.patrolPath != null && this.patrolPath.patrolNodes.Count > 1;
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x00101CE4 File Offset: 0x000FFEE4
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.attributes && this.moveAbility.moveSpeed == 0f)
		{
			this.moveAbility.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		}
		this.navMeshAgent = agent.GetComponent<NavMeshAgent>();
		this.InitializeRandoms();
		this.nextPatrolNode = 0;
	}

	// Token: 0x06002F4C RID: 12108 RVA: 0x00101D64 File Offset: 0x000FFF64
	private void InitializeRandoms()
	{
		this.patrolGroanSoundDelayRandom = new Unity.Mathematics.Random((uint)this.entity.GetNetId());
		this.patrolGroanSoundRandom = new Unity.Mathematics.Random((uint)this.entity.GetNetId());
	}

	// Token: 0x06002F4D RID: 12109 RVA: 0x00101D94 File Offset: 0x000FFF94
	protected override void OnStart()
	{
		this.moveAbility.Start();
		this.agent.SetIsPathing(true, true);
		if (this.patrolPath != null)
		{
			this.moveAbility.SetTarget(this.patrolPath.patrolNodes[this.nextPatrolNode]);
		}
		else
		{
			Debug.LogError("Starting patrol ability with no patrol path");
		}
		this.CalculateNextPatrolGroan();
	}

	// Token: 0x06002F4E RID: 12110 RVA: 0x00101DFA File Offset: 0x000FFFFA
	protected override void OnStop()
	{
		this.moveAbility.Stop();
	}

	// Token: 0x06002F4F RID: 12111 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002F50 RID: 12112 RVA: 0x00101E07 File Offset: 0x00100007
	public void SetPatrolPath(GRPatrolPath patrolPath)
	{
		this.patrolPath = patrolPath;
	}

	// Token: 0x06002F51 RID: 12113 RVA: 0x00101E10 File Offset: 0x00100010
	public GRPatrolPath GetPatrolPath()
	{
		return this.patrolPath;
	}

	// Token: 0x06002F52 RID: 12114 RVA: 0x00101E18 File Offset: 0x00100018
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x06002F53 RID: 12115 RVA: 0x00101E21 File Offset: 0x00100021
	public void CalculateNextPatrolGroan()
	{
		this.nextPatrolGroanTime = this.patrolGroanSoundDelayRandom.NextDouble(this.ambientSoundDelayMin, this.ambientSoundDelayMax) + PhotonNetwork.Time;
	}

	// Token: 0x06002F54 RID: 12116 RVA: 0x00101E48 File Offset: 0x00100048
	private void PlayPatrolGroan()
	{
		this.audioSource.clip = this.ambientPatrolSounds[this.patrolGroanSoundRandom.NextInt(this.ambientPatrolSounds.Length - 1)];
		this.audioSource.volume = this.ambientSoundVolume;
		this.audioSource.Play();
		this.CalculateNextPatrolGroan();
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x00101EA0 File Offset: 0x001000A0
	protected override void OnUpdateAuthority(float dt)
	{
		this.moveAbility.UpdateAuthority(dt);
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.green, true);
		}
		if (this.moveAbility.IsDone())
		{
			this.nextPatrolNode = (this.nextPatrolNode + 1) % this.patrolPath.patrolNodes.Count;
			this.moveAbility.SetTarget(this.patrolPath.patrolNodes[this.nextPatrolNode]);
		}
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x00101F44 File Offset: 0x00100144
	protected override void OnUpdateRemote(float dt)
	{
		this.moveAbility.SetTarget(null);
		this.moveAbility.SetTargetPos(this.agent.navAgent.destination);
		this.moveAbility.UpdateRemote(dt);
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.green, true);
		}
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x04003C64 RID: 15460
	private NavMeshAgent navMeshAgent;

	// Token: 0x04003C65 RID: 15461
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x04003C66 RID: 15462
	private GRPatrolPath patrolPath;

	// Token: 0x04003C67 RID: 15463
	public double lastStateChange;

	// Token: 0x04003C68 RID: 15464
	public float ambientSoundVolume = 0.5f;

	// Token: 0x04003C69 RID: 15465
	public double ambientSoundDelayMin = 5.0;

	// Token: 0x04003C6A RID: 15466
	public double ambientSoundDelayMax = 10.0;

	// Token: 0x04003C6B RID: 15467
	public AudioClip[] ambientPatrolSounds;

	// Token: 0x04003C6C RID: 15468
	private double lastPartrolAmbientSoundTime;

	// Token: 0x04003C6D RID: 15469
	private double nextPatrolGroanTime;

	// Token: 0x04003C6E RID: 15470
	private Unity.Mathematics.Random patrolGroanSoundDelayRandom;

	// Token: 0x04003C6F RID: 15471
	private Unity.Mathematics.Random patrolGroanSoundRandom;

	// Token: 0x04003C70 RID: 15472
	[ReadOnly]
	public int nextPatrolNode;
}
