using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200073F RID: 1855
[Serializable]
public class GRAbilityWander : GRAbilityBase
{
	// Token: 0x06002F26 RID: 12070 RVA: 0x00101312 File Offset: 0x000FF512
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002F27 RID: 12071 RVA: 0x00101338 File Offset: 0x000FF538
	protected override void OnStart()
	{
		this.moveAbility.Start();
		Vector3 vector = this.PickRandomDestination();
		this.moveAbility.SetTargetPos(vector);
	}

	// Token: 0x06002F28 RID: 12072 RVA: 0x00101363 File Offset: 0x000FF563
	protected override void OnStop()
	{
		this.moveAbility.Stop();
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x00101370 File Offset: 0x000FF570
	protected override void OnThink(float dt)
	{
		if (this.moveAbility.IsDone())
		{
			Vector3 vector = this.PickRandomDestination();
			this.moveAbility.SetTargetPos(vector);
		}
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x001013A0 File Offset: 0x000FF5A0
	private Vector3 PickRandomDestination()
	{
		Vector3 vector = this.agent.transform.position;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(vector, out navMeshHit, 1f, this.walkableArea))
		{
			Vector3 position = navMeshHit.position;
			Vector3 forward = this.agent.transform.forward;
			float num = 0f;
			for (int i = 0; i < GRAbilityWander.rotations.Length; i++)
			{
				Vector3 vector2 = GRAbilityWander.rotations[i] * forward;
				float num2 = 8f;
				if (NavMesh.Raycast(position, position + vector2 * num2, out navMeshHit, this.walkableArea))
				{
					num2 = navMeshHit.distance * 0.95f;
				}
				float num3 = num2 * GRAbilityWander.rotationWeight[i];
				if (num3 > num && NavMesh.SamplePosition(position + vector2 * num2, out navMeshHit, 1f, this.walkableArea))
				{
					num = num3;
					vector = navMeshHit.position;
				}
			}
		}
		return vector;
	}

	// Token: 0x06002F2C RID: 12076 RVA: 0x0010149E File Offset: 0x000FF69E
	protected override void OnUpdateAuthority(float dt)
	{
		this.moveAbility.UpdateAuthority(dt);
	}

	// Token: 0x06002F2D RID: 12077 RVA: 0x001014AC File Offset: 0x000FF6AC
	protected override void OnUpdateRemote(float dt)
	{
		this.moveAbility.UpdateRemote(dt);
	}

	// Token: 0x04003C56 RID: 15446
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x04003C57 RID: 15447
	private static Quaternion[] rotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 45f, 0f),
		Quaternion.Euler(0f, -45f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, -90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, -135f, 0f),
		Quaternion.Euler(0f, 180f, 0f)
	};

	// Token: 0x04003C58 RID: 15448
	private static float[] rotationWeight = new float[] { 1f, 0.75f, 0.75f, 0.5f, 0.5f, 0.2f, 0.2f, 0.2f };
}
