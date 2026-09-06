using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F7E RID: 3966
	public class DecorativeItem : TransferrableObject
	{
		// Token: 0x06006254 RID: 25172 RVA: 0x001EA564 File Offset: 0x001E8764
		public override bool ShouldBeKinematic()
		{
			return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State4 || base.ShouldBeKinematic();
		}

		// Token: 0x06006255 RID: 25173 RVA: 0x001FA446 File Offset: 0x001F8646
		public override void OnSpawn(VRRig rig)
		{
			base.OnSpawn(rig);
			this.parent = base.transform.parent;
		}

		// Token: 0x06006256 RID: 25174 RVA: 0x001EA5E7 File Offset: 0x001E87E7
		protected override void Start()
		{
			base.Start();
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x06006257 RID: 25175 RVA: 0x001FA460 File Offset: 0x001F8660
		private new void OnStateChanged()
		{
			TransferrableObject.ItemStates itemState = this.itemState;
			if (itemState == TransferrableObject.ItemStates.State2)
			{
				this.SnapItem(this.reliableState.isSnapped, this.reliableState.snapPosition);
				return;
			}
			if (itemState != TransferrableObject.ItemStates.State3)
			{
				return;
			}
			this.Respawn(this.reliableState.respawnPosition, this.reliableState.respawnRotation);
		}

		// Token: 0x06006258 RID: 25176 RVA: 0x001FA4B8 File Offset: 0x001F86B8
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (base.InHand())
			{
				this.itemState = TransferrableObject.ItemStates.State0;
			}
			DecorativeItem.DecorativeItemState itemState = (DecorativeItem.DecorativeItemState)this.itemState;
			if (itemState != this.previousItemState)
			{
				this.OnStateChanged();
			}
			this.previousItemState = itemState;
		}

		// Token: 0x06006259 RID: 25177 RVA: 0x001FA4F7 File Offset: 0x001F86F7
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.itemState == TransferrableObject.ItemStates.State4 && this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine)
			{
				this.InvokeRespawn();
			}
		}

		// Token: 0x0600625A RID: 25178 RVA: 0x001FA52E File Offset: 0x001F872E
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			this.itemState = TransferrableObject.ItemStates.State0;
		}

		// Token: 0x0600625B RID: 25179 RVA: 0x001FA53F File Offset: 0x001F873F
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.itemState = TransferrableObject.ItemStates.State1;
			this.Reparent(null);
			return true;
		}

		// Token: 0x0600625C RID: 25180 RVA: 0x001D1124 File Offset: 0x001CF324
		private void SetWillTeleport()
		{
			this.worldShareableInstance.SetWillTeleport();
		}

		// Token: 0x0600625D RID: 25181 RVA: 0x001FA560 File Offset: 0x001F8760
		public void Respawn(Vector3 randPosition, Quaternion randRotation)
		{
			if (base.InHand())
			{
				return;
			}
			if (this.shatterVFX && this.ShouldPlayFX())
			{
				this.PlayVFX(this.shatterVFX);
			}
			this.itemState = TransferrableObject.ItemStates.State3;
			this.SetWillTeleport();
			Transform transform = base.transform;
			transform.position = randPosition;
			transform.rotation = randRotation;
			if (this.reliableState)
			{
				this.reliableState.respawnPosition = randPosition;
				this.reliableState.respawnRotation = randRotation;
			}
		}

		// Token: 0x0600625E RID: 25182 RVA: 0x000D4992 File Offset: 0x000D2B92
		private void PlayVFX(GameObject vfx)
		{
			ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
		}

		// Token: 0x0600625F RID: 25183 RVA: 0x001FA5DC File Offset: 0x001F87DC
		private bool Reparent(Transform _transform)
		{
			if (!this.allowReparenting)
			{
				return false;
			}
			if (this.parent)
			{
				this.parent.SetParent(_transform);
				base.transform.SetParent(this.parent);
				return true;
			}
			return false;
		}

		// Token: 0x06006260 RID: 25184 RVA: 0x001FA618 File Offset: 0x001F8818
		public void SnapItem(bool snap, Vector3 attachPoint)
		{
			if (!this.reliableState)
			{
				return;
			}
			if (snap)
			{
				AttachPoint currentAttachPointByPosition = DecorativeItemsManager.Instance.getCurrentAttachPointByPosition(attachPoint);
				if (!currentAttachPointByPosition)
				{
					this.reliableState.isSnapped = false;
					this.reliableState.snapPosition = Vector3.zero;
					return;
				}
				Transform attachPoint2 = currentAttachPointByPosition.attachPoint;
				if (!this.Reparent(attachPoint2))
				{
					this.reliableState.isSnapped = false;
					this.reliableState.snapPosition = Vector3.zero;
					return;
				}
				this.itemState = TransferrableObject.ItemStates.State2;
				base.transform.parent.localPosition = Vector3.zero;
				base.transform.localPosition = Vector3.zero;
				this.reliableState.isSnapped = true;
				if (this.audioSource && this.snapAudio && this.ShouldPlayFX())
				{
					this.audioSource.GTPlayOneShot(this.snapAudio, 1f);
				}
				currentAttachPointByPosition.SetIsHook(true);
			}
			else
			{
				this.Reparent(null);
				this.reliableState.isSnapped = false;
			}
			this.reliableState.snapPosition = attachPoint;
		}

		// Token: 0x06006261 RID: 25185 RVA: 0x001FA733 File Offset: 0x001F8933
		private void InvokeRespawn()
		{
			if (this.itemState == TransferrableObject.ItemStates.State2)
			{
				return;
			}
			UnityAction<DecorativeItem> unityAction = this.respawnItem;
			if (unityAction == null)
			{
				return;
			}
			unityAction(this);
		}

		// Token: 0x06006262 RID: 25186 RVA: 0x001FA750 File Offset: 0x001F8950
		private bool ShouldPlayFX()
		{
			return this.previousItemState == DecorativeItem.DecorativeItemState.isHeld || this.previousItemState == DecorativeItem.DecorativeItemState.dropped;
		}

		// Token: 0x06006263 RID: 25187 RVA: 0x001FA767 File Offset: 0x001F8967
		private void OnCollisionEnter(Collision other)
		{
			if (this.breakItemLayerMask != (this.breakItemLayerMask | (1 << other.gameObject.layer)))
			{
				return;
			}
			this.InvokeRespawn();
		}

		// Token: 0x0400711B RID: 28955
		public DecorativeItemReliableState reliableState;

		// Token: 0x0400711C RID: 28956
		public UnityAction<DecorativeItem> respawnItem;

		// Token: 0x0400711D RID: 28957
		public LayerMask breakItemLayerMask;

		// Token: 0x0400711E RID: 28958
		private Coroutine respawnTimer;

		// Token: 0x0400711F RID: 28959
		private Transform parent;

		// Token: 0x04007120 RID: 28960
		private float _respawnTimestamp;

		// Token: 0x04007121 RID: 28961
		private bool isSnapped;

		// Token: 0x04007122 RID: 28962
		private Vector3 currentPosition;

		// Token: 0x04007123 RID: 28963
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007124 RID: 28964
		public AudioClip snapAudio;

		// Token: 0x04007125 RID: 28965
		public GameObject shatterVFX;

		// Token: 0x04007126 RID: 28966
		private new DecorativeItem.DecorativeItemState previousItemState = DecorativeItem.DecorativeItemState.dropped;

		// Token: 0x02000F7F RID: 3967
		private enum DecorativeItemState
		{
			// Token: 0x04007128 RID: 28968
			isHeld = 1,
			// Token: 0x04007129 RID: 28969
			dropped,
			// Token: 0x0400712A RID: 28970
			snapped = 4,
			// Token: 0x0400712B RID: 28971
			respawn = 8,
			// Token: 0x0400712C RID: 28972
			none = 16
		}
	}
}
