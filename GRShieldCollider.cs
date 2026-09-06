using System;
using UnityEngine;

// Token: 0x020007F2 RID: 2034
public class GRShieldCollider : MonoBehaviour
{
	// Token: 0x170004B8 RID: 1208
	// (get) Token: 0x060033FC RID: 13308 RVA: 0x0011DDF6 File Offset: 0x0011BFF6
	public float KnockbackVelocity
	{
		get
		{
			return this.knockbackVelocity;
		}
	}

	// Token: 0x170004B9 RID: 1209
	// (get) Token: 0x060033FD RID: 13309 RVA: 0x0011DDFE File Offset: 0x0011BFFE
	public GRToolDirectionalShield ShieldTool
	{
		get
		{
			return this.shieldTool;
		}
	}

	// Token: 0x060033FE RID: 13310 RVA: 0x0011DE06 File Offset: 0x0011C006
	private void Awake()
	{
		this.lastBlockHittableEntityId = GameEntityId.Invalid;
		this.lastBlockHittableTime = 0.0;
	}

	// Token: 0x060033FF RID: 13311 RVA: 0x0011DE22 File Offset: 0x0011C022
	public void OnEnemyBlocked(Vector3 enemyPosition)
	{
		if (this.shieldTool != null)
		{
			this.shieldTool.OnEnemyBlocked(enemyPosition);
		}
	}

	// Token: 0x06003400 RID: 13312 RVA: 0x0011DE40 File Offset: 0x0011C040
	public void BlockHittable(Vector3 enemyPosition, Vector3 enemyAttackDirection, GameHittable hittable)
	{
		if (this.shieldTool != null)
		{
			double timeAsDouble = Time.timeAsDouble;
			if (timeAsDouble - this.lastBlockHittableTime >= 1.0 || !(hittable.gameEntity.id == this.lastBlockHittableEntityId))
			{
				this.lastBlockHittableEntityId = hittable.gameEntity.id;
				this.lastBlockHittableTime = timeAsDouble;
				this.shieldTool.BlockHittable(enemyPosition, enemyAttackDirection, hittable, this);
			}
		}
	}

	// Token: 0x040043BE RID: 17342
	[SerializeField]
	private float knockbackVelocity = 3f;

	// Token: 0x040043BF RID: 17343
	[SerializeField]
	private GRToolDirectionalShield shieldTool;

	// Token: 0x040043C0 RID: 17344
	private const float BLOCK_SAME_HITTABLE_COOLDOWN = 1f;

	// Token: 0x040043C1 RID: 17345
	private GameEntityId lastBlockHittableEntityId;

	// Token: 0x040043C2 RID: 17346
	private double lastBlockHittableTime;
}
