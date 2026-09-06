using System;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020013CB RID: 5067
	[RequireComponent(typeof(GameEntity))]
	public class VoxelSpawnHandler : MonoBehaviour, IGameEntityComponent
	{
		// Token: 0x06007ECD RID: 32461 RVA: 0x00298989 File Offset: 0x00296B89
		private void Reset()
		{
			this.entity = base.GetComponent<GameEntity>();
		}

		// Token: 0x06007ECE RID: 32462 RVA: 0x00298997 File Offset: 0x00296B97
		private void Awake()
		{
			this.RegisterSpawnables();
		}

		// Token: 0x06007ECF RID: 32463 RVA: 0x0029899F File Offset: 0x00296B9F
		private void OnDisable()
		{
			this.UpdateListeningState();
		}

		// Token: 0x06007ED0 RID: 32464 RVA: 0x002989A8 File Offset: 0x00296BA8
		private void RegisterSpawnables()
		{
			if (this._spawnablesRegistered)
			{
				return;
			}
			GameEntityManager gameEntityManager = this.entity.manager;
			if (!gameEntityManager)
			{
				foreach (GameEntityManager gameEntityManager2 in GameEntityManager.managersByZone.Values)
				{
					if (gameEntityManager2.GetZoneSceneName() == base.gameObject.scene.name)
					{
						gameEntityManager = gameEntityManager2;
						break;
					}
				}
			}
			foreach (VoxelSpawnHandler.SpawnableSet spawnableSet in this.spawnables)
			{
				gameEntityManager.AddToFactory(spawnableSet.prefabs);
			}
			this._spawnablesRegistered = true;
		}

		// Token: 0x06007ED1 RID: 32465 RVA: 0x00298A70 File Offset: 0x00296C70
		public void OnEntityInit()
		{
			this._counts = new int[this.materialSet.Materials.Length];
			this.RegisterSpawnables();
			this.entity.manager.OnAuthorityChanged += this.OnAuthorityChanged;
			this.entity.manager.OnZoneActiveChanged += this.OnZoneActiveChanged;
			this.SetIsAuthority(this.entity.manager.IsAuthority());
			this.SetZoneActive(this.entity.manager.IsZoneActive());
		}

		// Token: 0x06007ED2 RID: 32466 RVA: 0x00298AFF File Offset: 0x00296CFF
		public void OnEntityDestroy()
		{
			this.entity.manager.OnAuthorityChanged -= this.OnAuthorityChanged;
			this.entity.manager.OnZoneActiveChanged += this.OnZoneActiveChanged;
		}

		// Token: 0x06007ED3 RID: 32467 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnEntityStateChange(long prevState, long newState)
		{
		}

		// Token: 0x06007ED4 RID: 32468 RVA: 0x00298B39 File Offset: 0x00296D39
		private void OnAuthorityChanged(NetPlayer fromPlayer, NetPlayer toPlayer)
		{
			this.SetIsAuthority(toPlayer != null && toPlayer.IsLocal);
		}

		// Token: 0x06007ED5 RID: 32469 RVA: 0x00298B4D File Offset: 0x00296D4D
		private void OnZoneActiveChanged(bool zoneActive)
		{
			this.SetZoneActive(zoneActive);
		}

		// Token: 0x06007ED6 RID: 32470 RVA: 0x00298B56 File Offset: 0x00296D56
		private void SetIsAuthority(bool newAuthority)
		{
			if (newAuthority == this._managerIsAuthority)
			{
				return;
			}
			this._managerIsAuthority = newAuthority;
			this.UpdateListeningState();
		}

		// Token: 0x06007ED7 RID: 32471 RVA: 0x00298B6F File Offset: 0x00296D6F
		private void SetZoneActive(bool newActive)
		{
			if (newActive == this._zoneIsActive)
			{
				return;
			}
			this._zoneIsActive = newActive;
			this.UpdateListeningState();
		}

		// Token: 0x06007ED8 RID: 32472 RVA: 0x00298B88 File Offset: 0x00296D88
		private void UpdateListeningState()
		{
			bool flag = this._managerIsAuthority && this._zoneIsActive;
			if (flag == this._isListening)
			{
				return;
			}
			if (flag)
			{
				VoxelEvents.OnResourcesMined += this.OnResourcesMined;
			}
			else
			{
				VoxelEvents.OnResourcesMined -= this.OnResourcesMined;
			}
			this._isListening = flag;
		}

		// Token: 0x06007ED9 RID: 32473 RVA: 0x00298BE0 File Offset: 0x00296DE0
		private void OnResourcesMined(VoxelWorld world, Vector3 hitPoint, Vector3 hitNormal, int[] amounts)
		{
			if (world.MaterialSet != this.materialSet)
			{
				return;
			}
			ZoneDef zoneDef = ZoneGraphBSP.Instance.FindZoneAtPoint(hitPoint);
			if (((zoneDef != null) ? zoneDef.zoneId : GTZone.none) != this.entity.manager.zone)
			{
				return;
			}
			for (int i = 0; i < amounts.Length; i++)
			{
				this._counts[i] += amounts[i];
				while (this._counts[i] >= this.spawnables[i].interval)
				{
					this._counts[i] -= this.spawnables[i].interval;
					if (Random.value < this.spawnables[i].chance)
					{
						this.SpawnItem(this.spawnables[i], hitPoint, hitNormal);
					}
				}
			}
		}

		// Token: 0x06007EDA RID: 32474 RVA: 0x00298CAC File Offset: 0x00296EAC
		private void SpawnItem(VoxelSpawnHandler.SpawnableSet spawns, Vector3 hitPoint, Vector3 hitNormal)
		{
			GameEntity gameEntity = spawns.prefabs[Random.Range(0, spawns.prefabs.Length)];
			Quaternion quaternion = Quaternion.LookRotation(hitNormal) * Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
			GameEntityId gameEntityId = this.entity.manager.RequestCreateItem(gameEntity.name.GetStaticHash(), hitPoint - hitNormal * 0.1f, quaternion, 0L);
			GameEntity gameEntity2 = this.entity.manager.GetGameEntity(gameEntityId);
			if (gameEntity2)
			{
				Rigidbody component = gameEntity2.GetComponent<Rigidbody>();
				component.linearVelocity = hitNormal * 3f;
				component.angularVelocity = Random.insideUnitSphere * 32f;
				gameEntity2.PlayThrowFx();
			}
		}

		// Token: 0x0400913E RID: 37182
		[SerializeField]
		private GameEntity entity;

		// Token: 0x0400913F RID: 37183
		[SerializeField]
		private VoxelMaterialSet materialSet;

		// Token: 0x04009140 RID: 37184
		[SerializeField]
		private VoxelSpawnHandler.SpawnableSet[] spawnables;

		// Token: 0x04009141 RID: 37185
		private bool _managerIsAuthority;

		// Token: 0x04009142 RID: 37186
		private bool _zoneIsActive;

		// Token: 0x04009143 RID: 37187
		private bool _isListening;

		// Token: 0x04009144 RID: 37188
		private int[] _counts;

		// Token: 0x04009145 RID: 37189
		private bool _spawnablesRegistered;

		// Token: 0x020013CC RID: 5068
		[Serializable]
		private class SpawnableSet
		{
			// Token: 0x04009146 RID: 37190
			public int interval = 100;

			// Token: 0x04009147 RID: 37191
			public float chance = 0.1f;

			// Token: 0x04009148 RID: 37192
			public GameEntity[] prefabs;
		}
	}
}
