using System;
using TMPro;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020013CA RID: 5066
	[RequireComponent(typeof(GameEntity))]
	public class VoxelSpawnableDeposit : MonoBehaviour, IGameEntityComponent
	{
		// Token: 0x06007EC2 RID: 32450 RVA: 0x00298858 File Offset: 0x00296A58
		private void Reset()
		{
			this.entity = base.GetComponent<GameEntity>();
			this.fx = base.GetComponentInChildren<ParticleSystem>();
			if (this.fx)
			{
				this.fx.gameObject.SetActive(false);
			}
		}

		// Token: 0x06007EC3 RID: 32451 RVA: 0x00298890 File Offset: 0x00296A90
		private void Start()
		{
			this.SetCounter(0);
		}

		// Token: 0x06007EC4 RID: 32452 RVA: 0x00298899 File Offset: 0x00296A99
		private void OnEnable()
		{
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		}

		// Token: 0x06007EC5 RID: 32453 RVA: 0x002988B6 File Offset: 0x00296AB6
		private void OnDisable()
		{
			RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
		}

		// Token: 0x06007EC6 RID: 32454 RVA: 0x002988D3 File Offset: 0x00296AD3
		private void OnLeftRoom()
		{
			this.entity.SetState(0L);
		}

		// Token: 0x06007EC7 RID: 32455 RVA: 0x002988E4 File Offset: 0x00296AE4
		private void OnTriggerEnter(Collider other)
		{
			if (!this.entity.IsAuthority())
			{
				return;
			}
			VoxelSpawnable componentInParent = other.GetComponentInParent<VoxelSpawnable>();
			if (componentInParent == null)
			{
				return;
			}
			this.entity.manager.RequestDestroyItem(componentInParent.entity.id);
			this.entity.RequestState(this.entity.GetState() + 1L);
		}

		// Token: 0x06007EC8 RID: 32456 RVA: 0x0029893E File Offset: 0x00296B3E
		private void SetCounter(int count)
		{
			this.text.text = count.ToString();
		}

		// Token: 0x06007EC9 RID: 32457 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnEntityInit()
		{
		}

		// Token: 0x06007ECA RID: 32458 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnEntityDestroy()
		{
		}

		// Token: 0x06007ECB RID: 32459 RVA: 0x00298952 File Offset: 0x00296B52
		public void OnEntityStateChange(long prevState, long newState)
		{
			this.SetCounter(Mathf.Max(0, (int)newState));
			if (newState > prevState && newState > 0L && this.fx)
			{
				this.fx.gameObject.SetActive(true);
			}
		}

		// Token: 0x0400913B RID: 37179
		[SerializeField]
		private GameEntity entity;

		// Token: 0x0400913C RID: 37180
		[SerializeField]
		private TMP_Text text;

		// Token: 0x0400913D RID: 37181
		[SerializeField]
		private ParticleSystem fx;
	}
}
