using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02001027 RID: 4135
	public sealed class SoakTaskDepositCollectibles : IGhostReactorSoakTask
	{
		// Token: 0x170009DA RID: 2522
		// (get) Token: 0x060066DC RID: 26332 RVA: 0x002105AC File Offset: 0x0020E7AC
		// (set) Token: 0x060066DD RID: 26333 RVA: 0x002105B4 File Offset: 0x0020E7B4
		public bool Complete { get; private set; }

		// Token: 0x060066DE RID: 26334 RVA: 0x002105BD File Offset: 0x0020E7BD
		public SoakTaskDepositCollectibles(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x060066DF RID: 26335 RVA: 0x002105CC File Offset: 0x0020E7CC
		public bool Update()
		{
			if (this._coreDepositor == null)
			{
				GhostReactor instance = GhostReactor.instance;
				if (instance != null)
				{
					this._coreDepositor = instance.currencyDepositor;
				}
				if (this._coreDepositor == null)
				{
					return false;
				}
			}
			if (this._seedExtractorTriggerLocation == null)
			{
				GhostReactor instance2 = GhostReactor.instance;
				if (instance2 != null)
				{
					this._seedExtractorTriggerLocation = new Vector3?(instance2.seedExtractor.transform.Find("DepositorTrigger").position);
				}
				if (this._seedExtractorTriggerLocation == null)
				{
					return false;
				}
			}
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._heldEntity == null || this._depositCollectibleTime == null)
			{
				List<GameEntity> list = managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>();
				GameEntity gameEntity = null;
				foreach (GameEntity gameEntity2 in list)
				{
					if (!(gameEntity2 == null) && !gameEntity2.IsHeld())
					{
						GRCollectible component = gameEntity2.gameObject.GetComponent<GRCollectible>();
						if (component != null && (component.type == ProgressionManager.CoreType.Core || component.type == ProgressionManager.CoreType.SuperCore || component.type == ProgressionManager.CoreType.ChaosSeed))
						{
							gameEntity = gameEntity2;
							break;
						}
					}
				}
				if (gameEntity != null)
				{
					Debug.Log(string.Format("Soak grabbing core {0}", gameEntity.id.index));
					managerForZone.RequestGrabEntity(gameEntity.id, true, Vector3.zero, Quaternion.identity);
					this._heldEntity = gameEntity;
					this._depositCollectibleTime = new float?(Time.time + 0.1f);
				}
			}
			else if (this._heldEntity != null)
			{
				float? depositCollectibleTime = this._depositCollectibleTime;
				if (depositCollectibleTime != null)
				{
					float valueOrDefault = depositCollectibleTime.GetValueOrDefault();
					if (Time.time >= valueOrDefault)
					{
						GRCollectible component2 = this._heldEntity.GetComponent<GRCollectible>();
						if (component2 == null)
						{
							return false;
						}
						ProgressionManager.CoreType type = component2.type;
						if (type - ProgressionManager.CoreType.Core > 1)
						{
							if (type == ProgressionManager.CoreType.ChaosSeed)
							{
								Debug.Log(string.Format("Soak depositing chaos seed {0}", this._heldEntity.id.index));
								this._heldEntity.gameObject.transform.position = this._seedExtractorTriggerLocation.Value;
							}
						}
						else
						{
							Debug.Log(string.Format("Soak depositing core {0}", this._heldEntity.id.index));
							this._heldEntity.gameObject.transform.position = this._coreDepositor.gameObject.transform.position;
						}
						this._heldEntity = null;
						this._depositCollectibleTime = null;
						this.Complete = true;
					}
				}
			}
			return true;
		}

		// Token: 0x060066E0 RID: 26336 RVA: 0x002108AC File Offset: 0x0020EAAC
		public void Reset()
		{
			this._heldEntity = null;
			this._depositCollectibleTime = null;
			this.Complete = false;
		}

		// Token: 0x040075BF RID: 30143
		public const float TIME_TO_HOLD_COLLECTIBLE = 0.1f;

		// Token: 0x040075C0 RID: 30144
		private readonly GRPlayer _grPlayer;

		// Token: 0x040075C1 RID: 30145
		private GRCurrencyDepositor _coreDepositor;

		// Token: 0x040075C2 RID: 30146
		private Vector3? _seedExtractorTriggerLocation;

		// Token: 0x040075C3 RID: 30147
		private GameEntity _heldEntity;

		// Token: 0x040075C4 RID: 30148
		private float? _depositCollectibleTime;
	}
}
