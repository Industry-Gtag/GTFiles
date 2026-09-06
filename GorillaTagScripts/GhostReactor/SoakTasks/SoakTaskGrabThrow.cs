using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02001028 RID: 4136
	public sealed class SoakTaskGrabThrow : IGhostReactorSoakTask
	{
		// Token: 0x170009DB RID: 2523
		// (get) Token: 0x060066E1 RID: 26337 RVA: 0x002108C8 File Offset: 0x0020EAC8
		// (set) Token: 0x060066E2 RID: 26338 RVA: 0x002108D0 File Offset: 0x0020EAD0
		public bool Complete { get; private set; }

		// Token: 0x060066E3 RID: 26339 RVA: 0x002108D9 File Offset: 0x0020EAD9
		public SoakTaskGrabThrow(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x060066E4 RID: 26340 RVA: 0x002108E8 File Offset: 0x0020EAE8
		public bool Update()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._dropEntityTime == null || this._heldEntityId == null)
			{
				List<GameEntity> list = managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>();
				GameEntity gameEntity = null;
				foreach (GameEntity gameEntity2 in list)
				{
					if (!(gameEntity2 == null) && !gameEntity2.IsHeld() && gameEntity2.pickupable && !(gameEntity2.gameObject.GetComponent<GameAgent>() != null))
					{
						gameEntity = gameEntity2;
						break;
					}
				}
				if (gameEntity != null)
				{
					Debug.Log(string.Format("Soak grabbing entity {0}", gameEntity.id.index));
					managerForZone.RequestGrabEntity(gameEntity.id, true, Vector3.zero, Quaternion.identity);
					this._heldEntityId = new GameEntityId?(gameEntity.id);
					this._dropEntityTime = new float?(Time.time + 0.1f);
				}
			}
			else if (this._heldEntityId != null)
			{
				float? dropEntityTime = this._dropEntityTime;
				if (dropEntityTime != null)
				{
					float valueOrDefault = dropEntityTime.GetValueOrDefault();
					if (Time.time >= valueOrDefault)
					{
						Debug.Log(string.Format("Soak dropping entity {0}", this._heldEntityId.Value.index));
						managerForZone.RequestThrowEntity(this._heldEntityId.Value, true, Vector3.zero, Vector3.zero, Vector3.zero);
						this._heldEntityId = null;
						this._dropEntityTime = null;
						this.Complete = true;
					}
				}
			}
			return true;
		}

		// Token: 0x060066E5 RID: 26341 RVA: 0x00210ABC File Offset: 0x0020ECBC
		public void Reset()
		{
			this._heldEntityId = null;
			this._dropEntityTime = null;
			this.Complete = false;
		}

		// Token: 0x040075C6 RID: 30150
		public const float TIME_TO_HOLD_ENTITY = 0.1f;

		// Token: 0x040075C7 RID: 30151
		private readonly GRPlayer _grPlayer;

		// Token: 0x040075C8 RID: 30152
		private GameEntityId? _heldEntityId;

		// Token: 0x040075C9 RID: 30153
		private float? _dropEntityTime;
	}
}
