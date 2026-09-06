using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200073B RID: 1851
[Serializable]
public class GRAbilityChase : GRAbilityBase
{
	// Token: 0x06002F05 RID: 12037 RVA: 0x00100894 File Offset: 0x000FEA94
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.targetPlayer = null;
		this.lastSeenTargetTime = 0.0;
		this.lastSeenTargetPosition = Vector3.zero;
		if (GRAbilityChase.targetOffsets == null)
		{
			int num = 8;
			GRAbilityChase.targetOffsets = new List<Vector3>(num);
			float num2 = 1f;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = new Vector3(num2, 0f, 0f);
				vector = Quaternion.Euler(0f, (float)i / (float)num * 360f, 0f) * vector;
				GRAbilityChase.targetOffsets.Add(vector);
			}
			Random random = new Random();
			List<Vector3> list = GRAbilityChase.targetOffsets.OrderBy((Vector3 x) => random.Next()).ToList<Vector3>();
			GRAbilityChase.targetOffsets.Clear();
			GRAbilityChase.targetOffsets.AddRange(list);
		}
		if (this.attributes && this.chaseSpeed == 0f)
		{
			this.chaseSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.ChaseSpeed);
		}
	}

	// Token: 0x06002F06 RID: 12038 RVA: 0x001009B0 File Offset: 0x000FEBB0
	protected override void OnStart()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.agent.SetSpeed(this.chaseSpeed);
		this.lastSeenTargetTime = Time.timeAsDouble;
		this.movementSound.Play(null);
		this.agent.ClearLastRequestedDestination();
	}

	// Token: 0x06002F07 RID: 12039 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnStop()
	{
	}

	// Token: 0x06002F08 RID: 12040 RVA: 0x00100A07 File Offset: 0x000FEC07
	public override bool IsDone()
	{
		return this.targetPlayer == null || Time.timeAsDouble - this.lastSeenTargetTime >= (double)this.giveUpDelay;
	}

	// Token: 0x06002F09 RID: 12041 RVA: 0x00100A2C File Offset: 0x000FEC2C
	protected override void OnThink(float dt)
	{
		GRPlayer grplayer = GRPlayer.Get(this.targetPlayer);
		if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
		{
			Vector3 vector = grplayer.transform.position;
			vector += GRAbilityChase.GetMoveTargetOffset(vector, this.entity);
			if (this.lineOfSight.HasLineOfSight(this.head.position, vector))
			{
				this.lastSeenTargetTime = Time.timeAsDouble;
			}
			if ((float)(Time.timeAsDouble - this.lastSeenTargetTime) <= this.loseVisibilityDelay)
			{
				this.lastSeenTargetPosition = vector;
			}
		}
		this.agent.RequestDestination(this.lastSeenTargetPosition);
	}

	// Token: 0x06002F0A RID: 12042 RVA: 0x00100AC8 File Offset: 0x000FECC8
	protected override void OnUpdateShared(float dt)
	{
		GameAgent.UpdateFacing(this.root, this.agent.navAgent, this.targetPlayer, this.maxTurnSpeed);
	}

	// Token: 0x06002F0B RID: 12043 RVA: 0x00100AEC File Offset: 0x000FECEC
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.targetPlayer = targetPlayer;
	}

	// Token: 0x06002F0C RID: 12044 RVA: 0x00100AF8 File Offset: 0x000FECF8
	public static Vector3 GetMoveTargetOffset(Vector3 targetPos, GameEntity attackingEntity)
	{
		int num = attackingEntity.id.index % GRAbilityChase.targetOffsets.Count;
		return GRAbilityChase.targetOffsets[num];
	}

	// Token: 0x04003C2E RID: 15406
	public float chaseSpeed;

	// Token: 0x04003C2F RID: 15407
	public string animName;

	// Token: 0x04003C30 RID: 15408
	public float animSpeed;

	// Token: 0x04003C31 RID: 15409
	public float maxTurnSpeed;

	// Token: 0x04003C32 RID: 15410
	public float loseVisibilityDelay;

	// Token: 0x04003C33 RID: 15411
	public float giveUpDelay;

	// Token: 0x04003C34 RID: 15412
	public AbilitySound movementSound;

	// Token: 0x04003C35 RID: 15413
	private NetPlayer targetPlayer;

	// Token: 0x04003C36 RID: 15414
	private double lastSeenTargetTime;

	// Token: 0x04003C37 RID: 15415
	private Vector3 lastSeenTargetPosition;

	// Token: 0x04003C38 RID: 15416
	private static List<Vector3> targetOffsets;
}
