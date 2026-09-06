using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GorillaNetworking
{
	// Token: 0x020010A5 RID: 4261
	public class CosmeticCollectionDisplay : MonoBehaviour
	{
		// Token: 0x17000A19 RID: 2585
		// (get) Token: 0x06006A1C RID: 27164 RVA: 0x002211F4 File Offset: 0x0021F3F4
		// (set) Token: 0x06006A1D RID: 27165 RVA: 0x002211FC File Offset: 0x0021F3FC
		public string ParentPlayFabID { get; private set; }

		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x06006A1E RID: 27166 RVA: 0x00221205 File Offset: 0x0021F405
		public int ActiveIndex
		{
			get
			{
				return this.activeIndex;
			}
		}

		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x06006A1F RID: 27167 RVA: 0x0022120D File Offset: 0x0021F40D
		public int Count
		{
			get
			{
				return this.spawnedAnchors.Count;
			}
		}

		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x06006A20 RID: 27168 RVA: 0x0022121A File Offset: 0x0021F41A
		public int VisibleMask
		{
			get
			{
				return this.visibleMask;
			}
		}

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x06006A21 RID: 27169 RVA: 0x00221222 File Offset: 0x0021F422
		public bool IsLocal
		{
			get
			{
				return this.isLocal;
			}
		}

		// Token: 0x06006A22 RID: 27170 RVA: 0x0022122C File Offset: 0x0021F42C
		public static void Register(VRRig rig, string parentID, CosmeticCollectionDisplay display, bool isLocal)
		{
			display.registeredRig = rig;
			display.registeredParentID = parentID;
			display.ParentPlayFabID = parentID;
			display.isLocal = isLocal;
			CosmeticCollectionDisplay cosmeticCollectionDisplay;
			if (CosmeticCollectionDisplay.Registered.TryGetValue(new ValueTuple<VRRig, string>(rig, parentID), out cosmeticCollectionDisplay) && cosmeticCollectionDisplay != null && cosmeticCollectionDisplay != display)
			{
				Object.Destroy(cosmeticCollectionDisplay);
			}
			CosmeticCollectionDisplay.Registered[new ValueTuple<VRRig, string>(rig, parentID)] = display;
			if (!CosmeticCollectionDisplay.AllDisplays.Contains(display))
			{
				CosmeticCollectionDisplay.AllDisplays.Add(display);
			}
		}

		// Token: 0x06006A23 RID: 27171 RVA: 0x002212AC File Offset: 0x0021F4AC
		public static CosmeticCollectionDisplay FindForRig(VRRig rig, string parentID)
		{
			CosmeticCollectionDisplay cosmeticCollectionDisplay;
			CosmeticCollectionDisplay.Registered.TryGetValue(new ValueTuple<VRRig, string>(rig, parentID), out cosmeticCollectionDisplay);
			return cosmeticCollectionDisplay;
		}

		// Token: 0x06006A24 RID: 27172 RVA: 0x002212D0 File Offset: 0x0021F4D0
		public static void GetAllForParent(VRRig rig, string parentID, List<CosmeticCollectionDisplay> result)
		{
			result.Clear();
			for (int i = 0; i < CosmeticCollectionDisplay.AllDisplays.Count; i++)
			{
				CosmeticCollectionDisplay cosmeticCollectionDisplay = CosmeticCollectionDisplay.AllDisplays[i];
				if (!(cosmeticCollectionDisplay == null) && cosmeticCollectionDisplay.registeredRig == rig && cosmeticCollectionDisplay.registeredParentID == parentID)
				{
					result.Add(cosmeticCollectionDisplay);
				}
			}
		}

		// Token: 0x06006A25 RID: 27173 RVA: 0x00221330 File Offset: 0x0021F530
		public static void DestroyAllForParentExcept(VRRig rig, string parentID, GameObject host)
		{
			for (int i = CosmeticCollectionDisplay.AllDisplays.Count - 1; i >= 0; i--)
			{
				CosmeticCollectionDisplay cosmeticCollectionDisplay = CosmeticCollectionDisplay.AllDisplays[i];
				if (cosmeticCollectionDisplay == null)
				{
					CosmeticCollectionDisplay.AllDisplays.RemoveAt(i);
				}
				else if (!(cosmeticCollectionDisplay.registeredRig != rig) && !(cosmeticCollectionDisplay.registeredParentID != parentID) && !(cosmeticCollectionDisplay.gameObject == host))
				{
					Object.Destroy(cosmeticCollectionDisplay);
				}
			}
		}

		// Token: 0x06006A26 RID: 27174 RVA: 0x002213A8 File Offset: 0x0021F5A8
		public static void GetDisplaysForRig(VRRig rig, List<CosmeticCollectionDisplay> result)
		{
			result.Clear();
			foreach (KeyValuePair<ValueTuple<VRRig, string>, CosmeticCollectionDisplay> keyValuePair in CosmeticCollectionDisplay.Registered)
			{
				if (keyValuePair.Key.Item1 == rig)
				{
					result.Add(keyValuePair.Value);
				}
			}
		}

		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x06006A27 RID: 27175 RVA: 0x0022141C File Offset: 0x0021F61C
		public CosmeticsController.CosmeticItem? ActiveCollectable
		{
			get
			{
				if (this.placedCollectables.Count <= 0)
				{
					return null;
				}
				return new CosmeticsController.CosmeticItem?(this.placedCollectables[this.activeIndex]);
			}
		}

		// Token: 0x06006A28 RID: 27176 RVA: 0x00221458 File Offset: 0x0021F658
		public CosmeticsController.CosmeticItem? GetCollectableAt(int index)
		{
			if (index < 0 || index >= this.placedCollectables.Count)
			{
				return null;
			}
			return new CosmeticsController.CosmeticItem?(this.placedCollectables[index]);
		}

		// Token: 0x06006A29 RID: 27177 RVA: 0x00221494 File Offset: 0x0021F694
		public bool ContentMatches(IReadOnlyList<CosmeticsController.CosmeticItem> items)
		{
			if (this.placedCollectables.Count != items.Count)
			{
				return false;
			}
			for (int i = 0; i < this.placedCollectables.Count; i++)
			{
				if (this.placedCollectables[i].itemName != items[i].itemName)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06006A2A RID: 27178 RVA: 0x002214F4 File Offset: 0x0021F6F4
		public void Populate(IReadOnlyList<CosmeticsController.CosmeticItem> ownedCollectables, CosmeticInfoV2 parentInfo, Transform rootXform)
		{
			this.ClearSpawnedAnchors();
			this.placedCollectables.Clear();
			this.canonicalIndices.Clear();
			this.isCycling = parentInfo.collectionIsCycling;
			bool collectionUsesIndexTargeting = parentInfo.collectionUsesIndexTargeting;
			if (this.isCycling)
			{
				CosmeticCollectionSlotDefinition cosmeticCollectionSlotDefinition = parentInfo.collectionSlots[0];
				Vector3 vector = cosmeticCollectionSlotDefinition.offset.scale;
				if (Mathf.Abs(vector.x) < 0.001f || Mathf.Abs(vector.y) < 0.001f || Mathf.Abs(vector.z) < 0.001f)
				{
					vector = Vector3.one;
				}
				for (int i = 0; i < ownedCollectables.Count; i++)
				{
					GameObject gameObject = new GameObject(string.Format("CollectionSlot_{0}", i));
					gameObject.transform.SetParent(rootXform, false);
					gameObject.transform.localPosition = cosmeticCollectionSlotDefinition.offset.pos;
					gameObject.transform.localRotation = cosmeticCollectionSlotDefinition.offset.rot;
					gameObject.transform.localScale = vector;
					this.spawnedAnchors.Add(gameObject);
					this.placedCollectables.Add(ownedCollectables[i]);
					this.canonicalIndices.Add(CosmeticCollectionDisplay.ResolveCanonicalIndex(parentInfo.playFabID, ownedCollectables[i].itemName));
					this.InstantiateIntoAnchor(ownedCollectables[i], gameObject.transform);
				}
			}
			else
			{
				int num = 0;
				for (int j = 0; j < parentInfo.collectionSlots.Length; j++)
				{
					CosmeticCollectionSlotDefinition cosmeticCollectionSlotDefinition2 = parentInfo.collectionSlots[j];
					CosmeticsController.CosmeticItem? cosmeticItem = null;
					if (collectionUsesIndexTargeting)
					{
						for (int k = 0; k < ownedCollectables.Count; k++)
						{
							if (ownedCollectables[k].GetTargetSlotIndexForParent(parentInfo.playFabID) == j)
							{
								cosmeticItem = new CosmeticsController.CosmeticItem?(ownedCollectables[k]);
								break;
							}
						}
					}
					else if (num < ownedCollectables.Count)
					{
						cosmeticItem = new CosmeticsController.CosmeticItem?(ownedCollectables[num++]);
					}
					if (cosmeticItem != null)
					{
						Vector3 vector2 = cosmeticCollectionSlotDefinition2.offset.scale;
						if (Mathf.Abs(vector2.x) < 0.001f || Mathf.Abs(vector2.y) < 0.001f || Mathf.Abs(vector2.z) < 0.001f)
						{
							vector2 = Vector3.one;
						}
						GameObject gameObject2 = new GameObject(string.Format("CollectionSlot_{0}", j));
						gameObject2.transform.SetParent(rootXform, false);
						gameObject2.transform.localPosition = cosmeticCollectionSlotDefinition2.offset.pos;
						gameObject2.transform.localRotation = cosmeticCollectionSlotDefinition2.offset.rot;
						gameObject2.transform.localScale = vector2;
						this.spawnedAnchors.Add(gameObject2);
						this.placedCollectables.Add(cosmeticItem.Value);
						this.canonicalIndices.Add(CosmeticCollectionDisplay.ResolveCanonicalIndex(parentInfo.playFabID, cosmeticItem.Value.itemName));
						this.InstantiateIntoAnchor(cosmeticItem.Value, gameObject2.transform);
					}
				}
			}
			this.activeIndex = 0;
			this.visibleMask = 0;
			for (int l = 0; l < this.canonicalIndices.Count; l++)
			{
				int num2 = this.canonicalIndices[l];
				if (num2 >= 0 && num2 < 32)
				{
					this.visibleMask |= 1 << num2;
				}
			}
			this.ApplyCyclingVisibility();
		}

		// Token: 0x06006A2B RID: 27179 RVA: 0x0022186C File Offset: 0x0021FA6C
		private static int ResolveCanonicalIndex(string parentPlayFabID, string itemName)
		{
			if (!CosmeticsController.hasInstance)
			{
				return -1;
			}
			return CosmeticsController.instance.GetCanonicalCollectableIndex(parentPlayFabID, itemName);
		}

		// Token: 0x06006A2C RID: 27180 RVA: 0x00221885 File Offset: 0x0021FA85
		public void SetActiveIndex(int index)
		{
			if (this.spawnedAnchors.Count == 0)
			{
				return;
			}
			this.activeIndex = Mathf.Clamp(index, 0, this.spawnedAnchors.Count - 1);
			this.RefreshAnchorVisibility();
			this.PersistLocalState();
		}

		// Token: 0x06006A2D RID: 27181 RVA: 0x002218BB File Offset: 0x0021FABB
		public void SetVisibleMask(int mask)
		{
			this.visibleMask = mask;
			this.RefreshAnchorVisibility();
			this.PersistLocalState();
		}

		// Token: 0x06006A2E RID: 27182 RVA: 0x002218D0 File Offset: 0x0021FAD0
		public bool SetEquippedAtCanonical(int canonicalIndex, bool equipped)
		{
			if (canonicalIndex < 0 || canonicalIndex >= 32)
			{
				return false;
			}
			int num = 1 << canonicalIndex;
			int num2 = (equipped ? (this.visibleMask | num) : (this.visibleMask & ~num));
			if (num2 == this.visibleMask)
			{
				return false;
			}
			this.visibleMask = num2;
			this.RefreshAnchorVisibility();
			this.PersistLocalState();
			return true;
		}

		// Token: 0x06006A2F RID: 27183 RVA: 0x00221925 File Offset: 0x0021FB25
		public bool IsEquippedAtCanonical(int canonicalIndex)
		{
			return canonicalIndex < 0 || canonicalIndex >= 32 || (this.visibleMask & (1 << canonicalIndex)) != 0;
		}

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x06006A30 RID: 27184 RVA: 0x00221942 File Offset: 0x0021FB42
		public int ActiveCanonicalIndex
		{
			get
			{
				if (this.placedCollectables.Count == 0 || this.activeIndex < 0 || this.activeIndex >= this.canonicalIndices.Count)
				{
					return -1;
				}
				return this.canonicalIndices[this.activeIndex];
			}
		}

		// Token: 0x06006A31 RID: 27185 RVA: 0x00221980 File Offset: 0x0021FB80
		public void PersistLocalState()
		{
			if (!this.isLocal || string.IsNullOrEmpty(this.registeredParentID) || !CosmeticsController.hasInstance)
			{
				return;
			}
			CosmeticsController.instance.localCycleStates[new ValueTuple<VRRig, string>(this.registeredRig, this.registeredParentID)] = new CosmeticsController.CollectionState
			{
				activeIndex = this.activeIndex,
				visibleMask = this.visibleMask
			};
		}

		// Token: 0x06006A32 RID: 27186 RVA: 0x002219F0 File Offset: 0x0021FBF0
		public void CycleActive(int direction)
		{
			if (!this.isCycling || this.spawnedAnchors.Count == 0)
			{
				return;
			}
			this.activeIndex = (this.activeIndex + direction + this.spawnedAnchors.Count) % this.spawnedAnchors.Count;
			this.RefreshAnchorVisibility();
		}

		// Token: 0x06006A33 RID: 27187 RVA: 0x00221A3F File Offset: 0x0021FC3F
		public void SetVisible(bool visible)
		{
			this.isVisible = visible;
			this.RefreshAnchorVisibility();
		}

		// Token: 0x06006A34 RID: 27188 RVA: 0x00221A50 File Offset: 0x0021FC50
		private void InstantiateIntoAnchor(CosmeticsController.CosmeticItem collectable, Transform anchor)
		{
			CosmeticInfoV2 cosmeticInfoV;
			if (!CosmeticsController.instance.TryGetCosmeticInfoV2(collectable.itemName, out cosmeticInfoV))
			{
				return;
			}
			CosmeticPart[] array = (cosmeticInfoV.hasStoreParts ? cosmeticInfoV.storeParts : cosmeticInfoV.functionalParts);
			if (array == null || array.Length == 0)
			{
				return;
			}
			GTAssetRef<GameObject> prefabAssetRef = array[0].prefabAssetRef;
			if (prefabAssetRef == null || !prefabAssetRef.RuntimeKeyIsValid())
			{
				return;
			}
			Vector3 attachScale = Vector3.one;
			CosmeticPart[] functionalParts = cosmeticInfoV.functionalParts;
			if (functionalParts != null && functionalParts.Length != 0)
			{
				CosmeticAttachInfo[] attachAnchors = functionalParts[0].attachAnchors;
				if (attachAnchors != null && attachAnchors.Length != 0)
				{
					Vector3 scale = attachAnchors[0].offset.scale;
					if (Mathf.Abs(scale.x) >= 0.001f && Mathf.Abs(scale.y) >= 0.001f && Mathf.Abs(scale.z) >= 0.001f)
					{
						attachScale = scale;
					}
				}
			}
			AsyncOperationHandle<GameObject> asyncOperationHandle = prefabAssetRef.InstantiateAsync(anchor, false);
			this.loadOps.Add(asyncOperationHandle);
			asyncOperationHandle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
			{
				if (handle.Status != AsyncOperationStatus.Succeeded)
				{
					return;
				}
				if (anchor == null || handle.Result == null)
				{
					Addressables.ReleaseInstance(handle);
					return;
				}
				handle.Result.transform.localPosition = Vector3.zero;
				handle.Result.transform.localRotation = Quaternion.identity;
				handle.Result.transform.localScale = attachScale;
			};
		}

		// Token: 0x06006A35 RID: 27189 RVA: 0x00221B72 File Offset: 0x0021FD72
		private void ApplyCyclingVisibility()
		{
			this.RefreshAnchorVisibility();
		}

		// Token: 0x06006A36 RID: 27190 RVA: 0x00221B7C File Offset: 0x0021FD7C
		private void RefreshAnchorVisibility()
		{
			for (int i = 0; i < this.spawnedAnchors.Count; i++)
			{
				if (!(this.spawnedAnchors[i] == null))
				{
					int num = ((i < this.canonicalIndices.Count) ? this.canonicalIndices[i] : i);
					bool flag = num < 0 || num >= 32 || (this.visibleMask & (1 << num)) != 0;
					bool flag2 = this.isVisible && flag && (!this.isCycling || i == this.activeIndex);
					this.spawnedAnchors[i].SetActive(flag2);
				}
			}
		}

		// Token: 0x06006A37 RID: 27191 RVA: 0x00221C28 File Offset: 0x0021FE28
		private void ClearSpawnedAnchors()
		{
			for (int i = 0; i < this.loadOps.Count; i++)
			{
				if (this.loadOps[i].IsValid())
				{
					Addressables.ReleaseInstance(this.loadOps[i]);
				}
			}
			this.loadOps.Clear();
			for (int j = 0; j < this.spawnedAnchors.Count; j++)
			{
				if (this.spawnedAnchors[j] != null)
				{
					Object.Destroy(this.spawnedAnchors[j]);
				}
			}
			this.spawnedAnchors.Clear();
			this.placedCollectables.Clear();
			this.canonicalIndices.Clear();
		}

		// Token: 0x06006A38 RID: 27192 RVA: 0x00221CDC File Offset: 0x0021FEDC
		private void UnregisterIfOwner()
		{
			ValueTuple<VRRig, string> valueTuple = new ValueTuple<VRRig, string>(this.registeredRig, this.registeredParentID);
			CosmeticCollectionDisplay cosmeticCollectionDisplay;
			if (CosmeticCollectionDisplay.Registered.TryGetValue(valueTuple, out cosmeticCollectionDisplay) && cosmeticCollectionDisplay == this)
			{
				CosmeticCollectionDisplay.Registered.Remove(valueTuple);
			}
		}

		// Token: 0x06006A39 RID: 27193 RVA: 0x00221D1F File Offset: 0x0021FF1F
		private void OnDisable()
		{
			this.UnregisterIfOwner();
		}

		// Token: 0x06006A3A RID: 27194 RVA: 0x00221D28 File Offset: 0x0021FF28
		private void OnEnable()
		{
			if (!string.IsNullOrEmpty(this.registeredParentID))
			{
				ValueTuple<VRRig, string> valueTuple = new ValueTuple<VRRig, string>(this.registeredRig, this.registeredParentID);
				CosmeticCollectionDisplay cosmeticCollectionDisplay;
				if (!CosmeticCollectionDisplay.Registered.TryGetValue(valueTuple, out cosmeticCollectionDisplay) || cosmeticCollectionDisplay == null || cosmeticCollectionDisplay == this)
				{
					CosmeticCollectionDisplay.Registered[valueTuple] = this;
				}
				CosmeticsController.CollectionState collectionState;
				if (this.isLocal && CosmeticsController.hasInstance && CosmeticsController.instance.localCycleStates.TryGetValue(new ValueTuple<VRRig, string>(this.registeredRig, this.registeredParentID), out collectionState))
				{
					this.visibleMask = collectionState.visibleMask;
					this.SetActiveIndex(collectionState.activeIndex);
				}
			}
		}

		// Token: 0x06006A3B RID: 27195 RVA: 0x00221DD1 File Offset: 0x0021FFD1
		private void OnDestroy()
		{
			this.UnregisterIfOwner();
			CosmeticCollectionDisplay.AllDisplays.Remove(this);
			this.ClearSpawnedAnchors();
		}

		// Token: 0x040079C3 RID: 31171
		private static readonly Dictionary<ValueTuple<VRRig, string>, CosmeticCollectionDisplay> Registered = new Dictionary<ValueTuple<VRRig, string>, CosmeticCollectionDisplay>();

		// Token: 0x040079C4 RID: 31172
		private static readonly List<CosmeticCollectionDisplay> AllDisplays = new List<CosmeticCollectionDisplay>();

		// Token: 0x040079C5 RID: 31173
		private bool isCycling;

		// Token: 0x040079C6 RID: 31174
		private bool isVisible = true;

		// Token: 0x040079C7 RID: 31175
		private bool isLocal;

		// Token: 0x040079C8 RID: 31176
		private int activeIndex;

		// Token: 0x040079C9 RID: 31177
		private int visibleMask;

		// Token: 0x040079CA RID: 31178
		private VRRig registeredRig;

		// Token: 0x040079CB RID: 31179
		private string registeredParentID;

		// Token: 0x040079CC RID: 31180
		private readonly List<GameObject> spawnedAnchors = new List<GameObject>();

		// Token: 0x040079CD RID: 31181
		private readonly List<AsyncOperationHandle<GameObject>> loadOps = new List<AsyncOperationHandle<GameObject>>();

		// Token: 0x040079CE RID: 31182
		private readonly List<CosmeticsController.CosmeticItem> placedCollectables = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x040079CF RID: 31183
		private readonly List<int> canonicalIndices = new List<int>();
	}
}
