using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02001026 RID: 4134
	public sealed class SoakTaskBreakable : IGhostReactorSoakTask
	{
		// Token: 0x170009D9 RID: 2521
		// (get) Token: 0x060066D7 RID: 26327 RVA: 0x00210367 File Offset: 0x0020E567
		// (set) Token: 0x060066D8 RID: 26328 RVA: 0x0021036F File Offset: 0x0020E56F
		public bool Complete { get; private set; }

		// Token: 0x060066D9 RID: 26329 RVA: 0x00210378 File Offset: 0x0020E578
		public SoakTaskBreakable(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x060066DA RID: 26330 RVA: 0x00210388 File Offset: 0x0020E588
		public bool Update()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._breakable != null && this._breakable.GetComponent<GRBreakable>().BrokenLocal)
			{
				Debug.Log(string.Format("soak breakable {0} is broken", this._breakable.id.index));
				this._breakable = null;
				this._nextHitTime = null;
				this.Complete = true;
			}
			else
			{
				if (this._breakable == null)
				{
					using (List<GameEntity>.Enumerator enumerator = managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							GameEntity gameEntity = enumerator.Current;
							if (!(gameEntity == null) && !gameEntity.IsHeld())
							{
								GRBreakable component = gameEntity.gameObject.GetComponent<GRBreakable>();
								if (component != null && !component.BrokenLocal)
								{
									this._breakable = gameEntity;
									this._nextHitTime = new float?(Time.time + 0.1f);
									break;
								}
							}
						}
						return true;
					}
				}
				if (this._breakable != null)
				{
					float? nextHitTime = this._nextHitTime;
					if (nextHitTime != null)
					{
						float valueOrDefault = nextHitTime.GetValueOrDefault();
						if (Time.time >= valueOrDefault)
						{
							Debug.Log(string.Format("soak hit breakable {0}", this._breakable.id.index));
							GameHitData gameHitData = new GameHitData
							{
								hitEntityId = this._breakable.id,
								hitByEntityId = this._breakable.id,
								hitTypeId = 0,
								hitEntityPosition = Vector3.zero,
								hitPosition = Vector3.zero,
								hitImpulse = Vector3.zero,
								hitAmount = 1
							};
							managerForZone.RequestHit(gameHitData);
						}
					}
				}
			}
			return true;
		}

		// Token: 0x060066DB RID: 26331 RVA: 0x00210590 File Offset: 0x0020E790
		public void Reset()
		{
			this._breakable = null;
			this._nextHitTime = null;
			this.Complete = false;
		}

		// Token: 0x040075BA RID: 30138
		public const float TIME_BETWEEN_HITS = 0.1f;

		// Token: 0x040075BB RID: 30139
		private readonly GRPlayer _grPlayer;

		// Token: 0x040075BC RID: 30140
		private GameEntity _breakable;

		// Token: 0x040075BD RID: 30141
		private float? _nextHitTime;
	}
}
