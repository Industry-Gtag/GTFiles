using System;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020013C9 RID: 5065
	[RequireComponent(typeof(GameEntity))]
	public class VoxelSpawnable : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06007EB9 RID: 32441 RVA: 0x002986C2 File Offset: 0x002968C2
		private void Reset()
		{
			this.entity = base.GetComponent<GameEntity>();
		}

		// Token: 0x06007EBA RID: 32442 RVA: 0x002986D0 File Offset: 0x002968D0
		private void Start()
		{
			if (!this.entity)
			{
				this.entity = base.GetComponent<GameEntity>();
			}
			if (this.lifespan > 0f)
			{
				GameEntity gameEntity = this.entity;
				gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
				GameEntity gameEntity2 = this.entity;
				gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
				this.StartCountdown();
			}
		}

		// Token: 0x06007EBB RID: 32443 RVA: 0x00298758 File Offset: 0x00296958
		private void OnDestroy()
		{
			if (this.lifespan > 0f)
			{
				GameEntity gameEntity = this.entity;
				gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
				GameEntity gameEntity2 = this.entity;
				gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.OnReleased));
				this.StopCountdown();
			}
		}

		// Token: 0x06007EBC RID: 32444 RVA: 0x002987C6 File Offset: 0x002969C6
		private void OnGrabbed()
		{
			this._held = true;
		}

		// Token: 0x06007EBD RID: 32445 RVA: 0x002987CF File Offset: 0x002969CF
		private void OnReleased()
		{
			this._expireTime = Time.time + this.lifespan;
			this._held = false;
		}

		// Token: 0x06007EBE RID: 32446 RVA: 0x002987EA File Offset: 0x002969EA
		private void StartCountdown()
		{
			this._expireTime = Time.time + this.lifespan;
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06007EBF RID: 32447 RVA: 0x00012134 File Offset: 0x00010334
		private void StopCountdown()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06007EC0 RID: 32448 RVA: 0x00298805 File Offset: 0x00296A05
		public void SliceUpdate()
		{
			if (this._held || !this.entity.IsAuthority())
			{
				return;
			}
			if (Time.time >= this._expireTime)
			{
				this.entity.manager.RequestDestroyItem(this.entity.id);
			}
		}

		// Token: 0x04009136 RID: 37174
		public GameEntity entity;

		// Token: 0x04009137 RID: 37175
		[Tooltip("Lifespan in seconds.  If zero, object will not expire.")]
		public float lifespan = 300f;

		// Token: 0x04009138 RID: 37176
		public string type;

		// Token: 0x04009139 RID: 37177
		private float _expireTime;

		// Token: 0x0400913A RID: 37178
		private bool _held;
	}
}
