using System;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001280 RID: 4736
	[Serializable]
	public struct CosmeticInfoV2 : ISerializationCallbackReceiver
	{
		// Token: 0x17000BA1 RID: 2977
		// (get) Token: 0x0600776C RID: 30572 RVA: 0x0026B354 File Offset: 0x00269554
		public bool hasHoldableParts
		{
			get
			{
				CosmeticPart[] array = this.holdableParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000BA2 RID: 2978
		// (get) Token: 0x0600776D RID: 30573 RVA: 0x0026B374 File Offset: 0x00269574
		public bool hasWardrobeParts
		{
			get
			{
				CosmeticPart[] array = this.wardrobeParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000BA3 RID: 2979
		// (get) Token: 0x0600776E RID: 30574 RVA: 0x0026B394 File Offset: 0x00269594
		public bool hasStoreParts
		{
			get
			{
				CosmeticPart[] array = this.storeParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000BA4 RID: 2980
		// (get) Token: 0x0600776F RID: 30575 RVA: 0x0026B3B4 File Offset: 0x002695B4
		public bool hasFunctionalParts
		{
			get
			{
				CosmeticPart[] array = this.functionalParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000BA5 RID: 2981
		// (get) Token: 0x06007770 RID: 30576 RVA: 0x0026B3D4 File Offset: 0x002695D4
		public bool hasFirstPersonViewParts
		{
			get
			{
				CosmeticPart[] array = this.firstPersonViewParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000BA6 RID: 2982
		// (get) Token: 0x06007771 RID: 30577 RVA: 0x0026B3F4 File Offset: 0x002695F4
		public bool hasLocalRigParts
		{
			get
			{
				CosmeticPart[] array = this.localRigParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000BA7 RID: 2983
		// (get) Token: 0x06007772 RID: 30578 RVA: 0x0026B414 File Offset: 0x00269614
		public bool isCollectionSubItem
		{
			get
			{
				CosmeticCollectionParentLink[] array = this.collectionParentLinks;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x06007773 RID: 30579 RVA: 0x0026B434 File Offset: 0x00269634
		public bool IsSubItemOfParent(string parentPlayFabID)
		{
			if (this.collectionParentLinks == null)
			{
				return false;
			}
			for (int i = 0; i < this.collectionParentLinks.Length; i++)
			{
				if (this.collectionParentLinks[i].parentPlayFabID == parentPlayFabID)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007774 RID: 30580 RVA: 0x0026B47C File Offset: 0x0026967C
		public int GetTargetSlotIndexForParent(string parentPlayFabID)
		{
			if (this.collectionParentLinks != null)
			{
				for (int i = 0; i < this.collectionParentLinks.Length; i++)
				{
					if (this.collectionParentLinks[i].parentPlayFabID == parentPlayFabID)
					{
						return this.collectionParentLinks[i].targetSlotIndex;
					}
				}
			}
			return -1;
		}

		// Token: 0x06007775 RID: 30581 RVA: 0x0026B4D0 File Offset: 0x002696D0
		public int GetSeriesIndexForParent(string parentPlayFabID)
		{
			if (this.collectionParentLinks != null)
			{
				for (int i = 0; i < this.collectionParentLinks.Length; i++)
				{
					if (this.collectionParentLinks[i].parentPlayFabID == parentPlayFabID)
					{
						return this.collectionParentLinks[i].seriesIndex;
					}
				}
			}
			return -1;
		}

		// Token: 0x06007776 RID: 30582 RVA: 0x0026B524 File Offset: 0x00269724
		public CosmeticInfoV2(string displayName)
		{
			this.enabled = true;
			this.season = null;
			this.displayName = displayName;
			this.playFabID = "";
			this.category = CosmeticsController.CosmeticCategory.None;
			this.icon = null;
			this.isHoldable = false;
			this.isThrowable = false;
			this.usesBothHandSlots = false;
			this.hideWardrobeMannequin = false;
			this.holdableParts = new CosmeticPart[0];
			this.functionalParts = new CosmeticPart[0];
			this.wardrobeParts = new CosmeticPart[0];
			this.storeParts = new CosmeticPart[0];
			this.firstPersonViewParts = new CosmeticPart[0];
			this.localRigParts = new CosmeticPart[0];
			this.setCosmetics = new CosmeticSO[0];
			this.collectionSlots = Array.Empty<CosmeticCollectionSlotDefinition>();
			this.collectionIsCycling = false;
			this.collectionParentLinks = Array.Empty<CosmeticCollectionParentLink>();
			this.collectionParentPlayFabID = string.Empty;
			this.collectionTargetSlotIndex = -1;
			this.anchorAntiIntersectOffsets = default(CosmeticAnchorAntiIntersectOffsets);
			this.debugCosmeticSOName = "__UNINITIALIZED__";
			this.throwableMaterialGrabIndices = new int[0];
			this.throwableIndex = -1;
			this.collectionUsesIndexTargeting = false;
			this.collectionUsesSeriesOrder = false;
			this.collectionSeriesIndex = -1;
			this.appliedCosmeticPlayFabID = null;
		}

		// Token: 0x06007777 RID: 30583 RVA: 0x00002C2D File Offset: 0x00000E2D
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		// Token: 0x06007778 RID: 30584 RVA: 0x0026B64C File Offset: 0x0026984C
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this._OnAfterDeserialize_InitializePartsArray(ref this.holdableParts, ECosmeticPartType.Holdable);
			this._OnAfterDeserialize_InitializePartsArray(ref this.functionalParts, ECosmeticPartType.Functional);
			this._OnAfterDeserialize_InitializePartsArray(ref this.wardrobeParts, ECosmeticPartType.Wardrobe);
			this._OnAfterDeserialize_InitializePartsArray(ref this.storeParts, ECosmeticPartType.Store);
			this._OnAfterDeserialize_InitializePartsArray(ref this.firstPersonViewParts, ECosmeticPartType.FirstPerson);
			this._OnAfterDeserialize_InitializePartsArray(ref this.localRigParts, ECosmeticPartType.LocalRig);
			if (this.setCosmetics == null)
			{
				this.setCosmetics = Array.Empty<CosmeticSO>();
			}
			if ((this.collectionParentLinks == null || this.collectionParentLinks.Length == 0) && !string.IsNullOrEmpty(this.collectionParentPlayFabID))
			{
				this.collectionParentLinks = new CosmeticCollectionParentLink[]
				{
					new CosmeticCollectionParentLink
					{
						parentPlayFabID = this.collectionParentPlayFabID,
						targetSlotIndex = this.collectionTargetSlotIndex,
						seriesIndex = this.collectionSeriesIndex
					}
				};
			}
			if (this.collectionParentLinks == null)
			{
				this.collectionParentLinks = Array.Empty<CosmeticCollectionParentLink>();
			}
		}

		// Token: 0x06007779 RID: 30585 RVA: 0x0026B730 File Offset: 0x00269930
		private void _OnAfterDeserialize_InitializePartsArray(ref CosmeticPart[] parts, ECosmeticPartType partType)
		{
			for (int i = 0; i < parts.Length; i++)
			{
				parts[i].partType = partType;
				ref CosmeticAttachInfo[] ptr = ref parts[i].attachAnchors;
				if (ptr == null)
				{
					ptr = Array.Empty<CosmeticAttachInfo>();
				}
			}
		}

		// Token: 0x04008714 RID: 34580
		public bool enabled;

		// Token: 0x04008715 RID: 34581
		[Tooltip("// TODO: (2024-09-27 MattO) season will determine what addressables bundle it will be in and wheter it should be active based on release time of season.\n\nThe assigned season will determine what folder the Cosmetic will go in and how it will be listed in the Cosmetic Browser.")]
		[Delayed]
		public SeasonSO season;

		// Token: 0x04008716 RID: 34582
		[Tooltip("Name that is displayed in the store during purchasing.")]
		[Delayed]
		public string displayName;

		// Token: 0x04008717 RID: 34583
		[Tooltip("ID used on the PlayFab servers that must be unique. If this does not exist on the playfab servers then an error will be thrown. In notion search for \"Cosmetics - Adding a PlayFab ID\".")]
		[Delayed]
		public string playFabID;

		// Token: 0x04008718 RID: 34584
		public Sprite icon;

		// Token: 0x04008719 RID: 34585
		[Tooltip("Category determines which category button in the user's wardrobe (which are the two rows of buttons with equivalent names) have to be pressed to access the cosmetic along with others in the same category.")]
		public StringEnum<CosmeticsController.CosmeticCategory> category;

		// Token: 0x0400871A RID: 34586
		[Obsolete("(2024-08-13 MattO) Will be removed after holdables array is fully implemented. Check length of `holdableParts` instead.")]
		[HideInInspector]
		public bool isHoldable;

		// Token: 0x0400871B RID: 34587
		public bool isThrowable;

		// Token: 0x0400871C RID: 34588
		[HideInInspector]
		public int[] throwableMaterialGrabIndices;

		// Token: 0x0400871D RID: 34589
		[HideInInspector]
		public int throwableIndex;

		// Token: 0x0400871E RID: 34590
		public bool usesBothHandSlots;

		// Token: 0x0400871F RID: 34591
		public bool hideWardrobeMannequin;

		// Token: 0x04008720 RID: 34592
		public const string holdableParts_infoBoxShortMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).";

		// Token: 0x04008721 RID: 34593
		public const string holdableParts_infoBoxDetailedMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components. The prefab asset's transform will be moved between the listed \n attach points on \"Gorilla Player Networked.prefab\" when grabbed by the player \n";

		// Token: 0x04008722 RID: 34594
		[Space]
		[Tooltip("\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components. The prefab asset's transform will be moved between the listed \n attach points on \"Gorilla Player Networked.prefab\" when grabbed by the player \n")]
		public CosmeticPart[] holdableParts;

		// Token: 0x04008723 RID: 34595
		public const string functionalParts_infoBoxShortMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.";

		// Token: 0x04008724 RID: 34596
		public const string functionalParts_infoBoxDetailedMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nIf a \"First Person View\" part or \"Local Rig Part\" is set it will be enabled instead of the wearable parts for the local player";

		// Token: 0x04008725 RID: 34597
		[Space]
		[Tooltip("\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nIf a \"First Person View\" part or \"Local Rig Part\" is set it will be enabled instead of the wearable parts for the local player")]
		public CosmeticPart[] functionalParts;

		// Token: 0x04008726 RID: 34598
		public const string wardrobeParts_infoBoxShortMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.";

		// Token: 0x04008727 RID: 34599
		public const string wardrobeParts_infoBoxDetailedMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\nThese prefabs are shown on the satellite wardrobe, and in the store (if \"Store Parts\" is left empty)";

		// Token: 0x04008728 RID: 34600
		[Space]
		[Tooltip("\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\nThese prefabs are shown on the satellite wardrobe, and in the store (if \"Store Parts\" is left empty)")]
		public CosmeticPart[] wardrobeParts;

		// Token: 0x04008729 RID: 34601
		public const string storeParts_infoBoxShortMsg = "\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.";

		// Token: 0x0400872A RID: 34602
		public const string storeParts_infoBoxDetailedMsg = "\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.\nStore parts only need to be specified if the store display should be different than the wardrobe display";

		// Token: 0x0400872B RID: 34603
		[Space]
		[Tooltip("\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.\nStore parts only need to be specified if the store display should be different than the wardrobe display")]
		public CosmeticPart[] storeParts;

		// Token: 0x0400872C RID: 34604
		public const string firstPersonViewParts_infoBoxShortMsg = "\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player";

		// Token: 0x0400872D RID: 34605
		public const string firstPersonViewParts_infoBoxDetailedMsg = "\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player\nThese are used for any peripheral view meshes on the No Mirror layer, usually on HAT or FACE items";

		// Token: 0x0400872E RID: 34606
		[Space]
		[Tooltip("\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player\nThese are used for any peripheral view meshes on the No Mirror layer, usually on HAT or FACE items")]
		public CosmeticPart[] firstPersonViewParts;

		// Token: 0x0400872F RID: 34607
		public const string localRigParts_infoBoxShortMsg = "\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".";

		// Token: 0x04008730 RID: 34608
		public const string localRigParts_infoBoxDetailedMsg = "\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".\nThese objects can be used in addition to first person view parts.\nThese can be used for mirror view meshes (usually HAT or FACE items)\nAny item with GTPosRotConstraints should be parented to the rig and not the camera";

		// Token: 0x04008731 RID: 34609
		[Space]
		[Tooltip("\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".\nThese objects can be used in addition to first person view parts.\nThese can be used for mirror view meshes (usually HAT or FACE items)\nAny item with GTPosRotConstraints should be parented to the rig and not the camera")]
		public CosmeticPart[] localRigParts;

		// Token: 0x04008732 RID: 34610
		[Space]
		[Tooltip("When this cosmetic is equipped, these offsets will be applied to the other objects on the player that are likely to clip\nSHIRT items ususally offset the badge, nametag, and chest items\n PAW items usually offset the hunt computer and builder watch")]
		public CosmeticAnchorAntiIntersectOffsets anchorAntiIntersectOffsets;

		// Token: 0x04008733 RID: 34611
		[Space]
		[Tooltip("TODO COMMENT")]
		public CosmeticSO[] setCosmetics;

		// Token: 0x04008734 RID: 34612
		[Space]
		[Tooltip("For parent (collection) cosmetics: the slots that collectables snap into. Each entry defines the slot type and its local space offset from the cosmetic's root. Edit slot positions visually via the Cosmetic Editor Stage. The slot count is implicit from this array's length.")]
		public CosmeticCollectionSlotDefinition[] collectionSlots;

		// Token: 0x04008735 RID: 34613
		[Tooltip("For parent (collection) cosmetics: when true only one collectable is visible at a time and the player can cycle through them. When false all slots are shown simultaneously")]
		public bool collectionIsCycling;

		// Token: 0x04008736 RID: 34614
		[Tooltip("[Non-cycling collections only] When true, each sub-item is placed at the specific physical slot position declared by its 'Target Slot Index', rather than filling slots in purchase order. Use this when sub-items must always occupy a fixed dedicated position on the parent ")]
		public bool collectionUsesIndexTargeting;

		// Token: 0x04008737 RID: 34615
		[Tooltip("[Cycling collections only] When true, sub-items are cycled through in ascending Series Index order rather than purchase order, regardless of when they were acquired. Gaps in the series are skipped only owned items appear, but always in their correct numbered sequence (e.g. comic reade always cycle as #1 → #2 → #3 even if bought out of order).")]
		public bool collectionUsesSeriesOrder;

		// Token: 0x04008738 RID: 34616
		[Space]
		[Tooltip("For sub-item (collectable) cosmetics: the parent cosmetics this sub-item attaches to. A sub-item can list multiple parents; it is shown on every listed parent that is equipped, and each entry carries its own target slot index, so the same sub-item can occupy a different slot on different parents. At least one listed parent must be owned before this sub-item can be purchased. Leave empty if this is not a sub-item.")]
		public CosmeticCollectionParentLink[] collectionParentLinks;

		// Token: 0x04008739 RID: 34617
		[HideInInspector]
		public string collectionParentPlayFabID;

		// Token: 0x0400873A RID: 34618
		[HideInInspector]
		public int collectionTargetSlotIndex;

		// Token: 0x0400873B RID: 34619
		[HideInInspector]
		public int collectionSeriesIndex;

		// Token: 0x0400873C RID: 34620
		[Tooltip("PlayFab ID of the cosmetic to apply to a hit player via Cosmetic Swapper (e.g. chicken sword) tech. Distinct from this sub-item's own visual.")]
		public string appliedCosmeticPlayFabID;

		// Token: 0x0400873D RID: 34621
		[NonSerialized]
		public string debugCosmeticSOName;
	}
}
