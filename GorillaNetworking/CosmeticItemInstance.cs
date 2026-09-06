using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010A8 RID: 4264
	public class CosmeticItemInstance
	{
		// Token: 0x17000A21 RID: 2593
		// (get) Token: 0x06006A45 RID: 27205 RVA: 0x002221C0 File Offset: 0x002203C0
		public CosmeticsController.CosmeticSlots ActiveSlot
		{
			get
			{
				return this._activeSlot;
			}
		}

		// Token: 0x06006A46 RID: 27206 RVA: 0x002221C8 File Offset: 0x002203C8
		private void EnableItem(GameObject obj, bool enable)
		{
			try
			{
				obj.SetActive(enable);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Exception while enabling cosmetic: {0}", ex));
			}
		}

		// Token: 0x06006A47 RID: 27207 RVA: 0x00222204 File Offset: 0x00220404
		private void ApplyClippingOffsets(bool itemEnabled)
		{
			if (this._bodyDockPositions == null)
			{
				return;
			}
			if (this._anchorOverrides != null)
			{
				if (this.clippingOffsets.nameTag.enabled)
				{
					this._anchorOverrides.UpdateNameTagOffset(itemEnabled ? this.clippingOffsets.nameTag.offset : XformOffset.Identity, itemEnabled, this._activeSlot);
				}
				if (this.clippingOffsets.leftArm.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnLeftArm, this.clippingOffsets.leftArm.offset, itemEnabled, this._bodyDockPositions.leftArmTransform);
				}
				if (this.clippingOffsets.rightArm.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnRightArm, this.clippingOffsets.rightArm.offset, itemEnabled, this._bodyDockPositions.rightArmTransform);
				}
				if (this.clippingOffsets.chest.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnChest, this.clippingOffsets.chest.offset, itemEnabled, this._anchorOverrides.chestDefaultTransform);
				}
				if (this.clippingOffsets.huntComputer.enabled)
				{
					this._anchorOverrides.UpdateHuntWatchOffset(this.clippingOffsets.huntComputer.offset, itemEnabled);
				}
				if (this.clippingOffsets.badge.enabled)
				{
					this._anchorOverrides.UpdateBadgeOffset(itemEnabled ? this.clippingOffsets.badge.offset : XformOffset.Identity, itemEnabled, this._activeSlot);
				}
				if (this.clippingOffsets.builderWatch.enabled)
				{
					this._anchorOverrides.UpdateBuilderWatchOffset(this.clippingOffsets.builderWatch.offset, itemEnabled);
				}
				if (this.clippingOffsets.friendshipBraceletLeft.enabled)
				{
					this._anchorOverrides.UpdateFriendshipBraceletOffset(this.clippingOffsets.friendshipBraceletLeft.offset, true, itemEnabled);
				}
				if (this.clippingOffsets.friendshipBraceletRight.enabled)
				{
					this._anchorOverrides.UpdateFriendshipBraceletOffset(this.clippingOffsets.friendshipBraceletRight.offset, false, itemEnabled);
				}
			}
		}

		// Token: 0x06006A48 RID: 27208 RVA: 0x00222418 File Offset: 0x00220618
		public void DisableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject gameObject in this.objects)
			{
				this.EnableItem(gameObject, false);
			}
			if (flag)
			{
				foreach (GameObject gameObject2 in this.leftObjects)
				{
					this.EnableItem(gameObject2, false);
				}
			}
			if (flag2)
			{
				foreach (GameObject gameObject3 in this.rightObjects)
				{
					this.EnableItem(gameObject3, false);
				}
			}
			this.ApplyClippingOffsets(false);
		}

		// Token: 0x06006A49 RID: 27209 RVA: 0x00222514 File Offset: 0x00220714
		public void EnableItem(CosmeticsController.CosmeticSlots cosmeticSlot, VRRig rig)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			this._activeSlot = cosmeticSlot;
			if (rig != null && this._anchorOverrides == null)
			{
				this._anchorOverrides = rig.gameObject.GetComponent<VRRigAnchorOverrides>();
				this._bodyDockPositions = rig.GetComponent<BodyDockPositions>();
			}
			foreach (GameObject gameObject in this.objects)
			{
				this.EnableItem(gameObject, true);
				if (cosmeticSlot == CosmeticsController.CosmeticSlots.Badge)
				{
					if (this.objects.Count > 1)
					{
						GTHardCodedBones.EBone ebone;
						Transform transform;
						if (GTHardCodedBones.TryGetFirstBoneInParents(gameObject.transform, out ebone, out transform) && ebone == GTHardCodedBones.EBone.body)
						{
							this._anchorOverrides.CurrentBadgeTransform = gameObject.transform;
						}
					}
					else
					{
						this._anchorOverrides.CurrentBadgeTransform = gameObject.transform;
					}
				}
			}
			if (flag)
			{
				foreach (GameObject gameObject2 in this.leftObjects)
				{
					this.EnableItem(gameObject2, true);
				}
			}
			if (flag2)
			{
				foreach (GameObject gameObject3 in this.rightObjects)
				{
					this.EnableItem(gameObject3, true);
				}
			}
			this.ApplyClippingOffsets(true);
		}

		// Token: 0x06006A4A RID: 27210 RVA: 0x00222694 File Offset: 0x00220894
		public void ToggleRenderers(bool enabled)
		{
			for (int i = 0; i < this.allRenderers.Count; i++)
			{
				this.allRenderers[i].enabled = enabled;
			}
		}

		// Token: 0x06006A4B RID: 27211 RVA: 0x002226CC File Offset: 0x002208CC
		public void ToggleParticles(bool enabled)
		{
			for (int i = 0; i < this.allParticles.Count; i++)
			{
				this.allParticles[i].emission.enabled = enabled;
			}
		}

		// Token: 0x040079D7 RID: 31191
		public List<GameObject> leftObjects = new List<GameObject>();

		// Token: 0x040079D8 RID: 31192
		public List<GameObject> rightObjects = new List<GameObject>();

		// Token: 0x040079D9 RID: 31193
		public List<GameObject> objects = new List<GameObject>();

		// Token: 0x040079DA RID: 31194
		public List<GameObject> holdableObjects = new List<GameObject>();

		// Token: 0x040079DB RID: 31195
		public List<Renderer> allRenderers = new List<Renderer>();

		// Token: 0x040079DC RID: 31196
		public List<ParticleSystem> allParticles = new List<ParticleSystem>();

		// Token: 0x040079DD RID: 31197
		public CosmeticAnchorAntiIntersectOffsets clippingOffsets;

		// Token: 0x040079DE RID: 31198
		public bool isHoldableItem;

		// Token: 0x040079DF RID: 31199
		public string dbgname;

		// Token: 0x040079E0 RID: 31200
		private BodyDockPositions _bodyDockPositions;

		// Token: 0x040079E1 RID: 31201
		private VRRigAnchorOverrides _anchorOverrides;

		// Token: 0x040079E2 RID: 31202
		private CosmeticsController.CosmeticSlots _activeSlot;
	}
}
