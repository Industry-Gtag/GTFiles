using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02001029 RID: 4137
	public sealed class SoakTaskHitEnemy : IGhostReactorSoakTask
	{
		// Token: 0x170009DC RID: 2524
		// (get) Token: 0x060066E6 RID: 26342 RVA: 0x00210ADD File Offset: 0x0020ECDD
		// (set) Token: 0x060066E7 RID: 26343 RVA: 0x00210AE5 File Offset: 0x0020ECE5
		public bool Complete { get; private set; }

		// Token: 0x060066E8 RID: 26344 RVA: 0x00210AEE File Offset: 0x0020ECEE
		public SoakTaskHitEnemy(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x060066E9 RID: 26345 RVA: 0x00210B00 File Offset: 0x0020ED00
		public bool Update()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._enemy != null && !SoakTaskHitEnemy.IsLivingEnemy(this._enemy))
			{
				Debug.Log(string.Format("soak enemy {0} is dead", this._enemy.id.index));
				this.Complete = true;
				return true;
			}
			if (this._enemy == null)
			{
				foreach (GameEntity gameEntity in managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>())
				{
					if (!(gameEntity == null) && !gameEntity.IsHeld() && !(gameEntity.GetComponent<GameAgent>() == null) && !(gameEntity.GetComponent<GameHittable>() == null) && SoakTaskHitEnemy.IsEnemy(gameEntity))
					{
						this._enemy = gameEntity;
						this._nextHitTime = new float?(Time.time + 0.1f);
						break;
					}
				}
				return this._enemy != null;
			}
			if (this._nextHitTime == null)
			{
				throw new Exception("Invalid state in HitEnemySoakTask.");
			}
			if (Time.time < this._nextHitTime.Value)
			{
				return true;
			}
			Debug.Log(string.Format("soak hitting enemy {0}", this._enemy.id.index));
			GameEntity randomTool = this.GetRandomTool();
			if (randomTool == null)
			{
				Debug.LogError("No club found for soak task hit enemy.");
				return false;
			}
			GameHitData gameHitData = new GameHitData
			{
				hitEntityId = this._enemy.id,
				hitByEntityId = randomTool.id,
				hitTypeId = 0,
				hitEntityPosition = Vector3.zero,
				hitPosition = Vector3.zero,
				hitImpulse = Vector3.zero,
				hitAmount = 1
			};
			managerForZone.RequestHit(gameHitData);
			this._nextHitTime = new float?(Time.time + 0.1f);
			return true;
		}

		// Token: 0x060066EA RID: 26346 RVA: 0x00210D20 File Offset: 0x0020EF20
		public void Reset()
		{
			this._enemy = null;
			this._nextHitTime = null;
			this.Complete = false;
		}

		// Token: 0x060066EB RID: 26347 RVA: 0x00210D3C File Offset: 0x0020EF3C
		private GameEntity GetRandomTool()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return null;
			}
			foreach (GameEntity gameEntity in managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>())
			{
				if (!(gameEntity == null))
				{
					GRTool component = gameEntity.GetComponent<GRTool>();
					if (component != null)
					{
						GRTool.GRToolType toolType = component.toolType;
						if (toolType == GRTool.GRToolType.Club || toolType == GRTool.GRToolType.HockeyStick)
						{
							return gameEntity;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x060066EC RID: 26348 RVA: 0x00210DE8 File Offset: 0x0020EFE8
		private static bool IsEnemy(GameEntity entity)
		{
			return entity.GetComponent<GREnemyChaser>() != null || entity.GetComponent<GREnemyPest>() != null || entity.GetComponent<GREnemyRanged>() != null || entity.GetComponent<GREnemySummoner>() != null || entity.GetComponent<GREnemyMonkeye>() != null;
		}

		// Token: 0x060066ED RID: 26349 RVA: 0x00210E3C File Offset: 0x0020F03C
		private static bool IsLivingEnemy(GameEntity entity)
		{
			if (SoakTaskHitEnemy.IsEnemy(entity))
			{
				GREnemyChaser component = entity.GetComponent<GREnemyChaser>();
				if (component == null || component.hp <= 0)
				{
					GREnemyPest component2 = entity.GetComponent<GREnemyPest>();
					if (component2 == null || component2.hp <= 0)
					{
						GREnemyRanged component3 = entity.GetComponent<GREnemyRanged>();
						if (component3 == null || component3.hp <= 0)
						{
							GREnemySummoner component4 = entity.GetComponent<GREnemySummoner>();
							if (component4 == null || component4.hp <= 0)
							{
								GREnemyMonkeye component5 = entity.GetComponent<GREnemyMonkeye>();
								return component5 != null && component5.hp > 0;
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x040075CB RID: 30155
		public const float TIME_BETWEEN_HITS = 0.1f;

		// Token: 0x040075CC RID: 30156
		private readonly GRPlayer _grPlayer;

		// Token: 0x040075CD RID: 30157
		private GameEntity _enemy;

		// Token: 0x040075CE RID: 30158
		private float? _nextHitTime;
	}
}
