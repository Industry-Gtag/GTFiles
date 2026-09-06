using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200131F RID: 4895
	public class CosmeticSwapper : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000C07 RID: 3079
		// (get) Token: 0x06007AE5 RID: 31461 RVA: 0x00281593 File Offset: 0x0027F793
		private int CosmeticStepIndex
		{
			get
			{
				return this.newSwappedCosmetics.Count;
			}
		}

		// Token: 0x06007AE6 RID: 31462 RVA: 0x002815A0 File Offset: 0x0027F7A0
		private void Awake()
		{
			this.controller = CosmeticsController.instance;
		}

		// Token: 0x06007AE7 RID: 31463 RVA: 0x002815AF File Offset: 0x0027F7AF
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticsGlobal(this.cosmeticIDs);
		}

		// Token: 0x06007AE8 RID: 31464 RVA: 0x002815C2 File Offset: 0x0027F7C2
		private void OnDisable()
		{
			PlayerCosmeticsSystem.LockTemporaryCosmeticsGlobal(this.cosmeticIDs);
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007AE9 RID: 31465 RVA: 0x002815D5 File Offset: 0x0027F7D5
		public void SwapInCosmetic(VRRig vrRig)
		{
			this.TriggerSwap(vrRig);
		}

		// Token: 0x06007AEA RID: 31466 RVA: 0x002815DE File Offset: 0x0027F7DE
		private CosmeticSwapper.SwapMode GetCurrentMode()
		{
			return this.swapMode;
		}

		// Token: 0x06007AEB RID: 31467 RVA: 0x002815E6 File Offset: 0x0027F7E6
		private bool ShouldHoldFinalStep()
		{
			return this.holdFinalStep;
		}

		// Token: 0x06007AEC RID: 31468 RVA: 0x002815EE File Offset: 0x0027F7EE
		public int GetCurrentStepIndex(VRRig rig)
		{
			if (rig == null)
			{
				return 0;
			}
			return this.CosmeticStepIndex;
		}

		// Token: 0x06007AED RID: 31469 RVA: 0x00281601 File Offset: 0x0027F801
		public int GetNumberOfSteps()
		{
			if (this.cycleController != null)
			{
				return this.cycleController.Count;
			}
			return this.cosmeticIDs.Count;
		}

		// Token: 0x06007AEE RID: 31470 RVA: 0x00281628 File Offset: 0x0027F828
		private void TriggerSwap(VRRig rig)
		{
			if (GorillaGameManager.instance != null && this.gameModeExclusion.Contains(GorillaGameManager.instance.GameType()))
			{
				return;
			}
			if (rig == null || this.controller == null)
			{
				return;
			}
			if (rig != GorillaTagger.Instance.offlineVRRig)
			{
				return;
			}
			if (this.cycleController != null)
			{
				if (this.swapMode == CosmeticSwapper.SwapMode.Random)
				{
					this.cycleController.CycleRandom();
				}
				string appliedCosmeticID = this.cycleController.GetAppliedCosmeticID();
				if (string.IsNullOrEmpty(appliedCosmeticID))
				{
					return;
				}
				CosmeticSwapper.CosmeticState? cosmeticState = this.SwapInCosmeticWithReturn(appliedCosmeticID, rig);
				if (cosmeticState != null)
				{
					this.AddNewSwappedCosmetic(cosmeticState.Value);
				}
				return;
			}
			else
			{
				if (this.cosmeticIDs.Count == 0)
				{
					return;
				}
				if (this.swapMode == CosmeticSwapper.SwapMode.Random)
				{
					string text = this.cosmeticIDs[Random.Range(0, this.cosmeticIDs.Count)];
					CosmeticSwapper.CosmeticState? cosmeticState2 = this.SwapInCosmeticWithReturn(text, rig);
					if (cosmeticState2 != null)
					{
						this.AddNewSwappedCosmetic(cosmeticState2.Value);
					}
					return;
				}
				if (this.swapMode == CosmeticSwapper.SwapMode.AllAtOnce)
				{
					if (this.newSwappedCosmetics.Count > 0)
					{
						return;
					}
					foreach (string text2 in this.cosmeticIDs)
					{
						CosmeticSwapper.CosmeticState? cosmeticState3 = this.SwapInCosmeticWithReturn(text2, rig);
						if (cosmeticState3 != null)
						{
							this.AddNewSwappedCosmetic(cosmeticState3.Value);
						}
					}
					return;
				}
				else
				{
					int cosmeticStepIndex = this.CosmeticStepIndex;
					if (cosmeticStepIndex < 0 || cosmeticStepIndex >= this.cosmeticIDs.Count)
					{
						return;
					}
					string text3 = this.cosmeticIDs[cosmeticStepIndex];
					CosmeticSwapper.CosmeticState? cosmeticState4 = this.SwapInCosmeticWithReturn(text3, rig);
					if (cosmeticState4 != null)
					{
						this.AddNewSwappedCosmetic(cosmeticState4.Value);
						if (cosmeticStepIndex == this.cosmeticIDs.Count - 1)
						{
							if (this.holdFinalStep)
							{
								this.MarkFinalCosmeticStep();
							}
							if (this.OnSwappingSequenceCompleted != null)
							{
								this.OnSwappingSequenceCompleted.Invoke(rig);
								return;
							}
						}
						else
						{
							this.UnmarkFinalCosmeticStep();
						}
					}
					return;
				}
			}
		}

		// Token: 0x06007AEF RID: 31471 RVA: 0x00281830 File Offset: 0x0027FA30
		private CosmeticSwapper.CosmeticState? SwapInCosmeticWithReturn(string nameOrId, VRRig rig)
		{
			if (this.controller == null)
			{
				return null;
			}
			CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(nameOrId);
			if (cosmeticItem.isNullItem)
			{
				Debug.LogWarning("Cosmetic not found: " + nameOrId);
				return null;
			}
			bool flag;
			CosmeticsController.CosmeticSlots cosmeticSlot = this.GetCosmeticSlot(cosmeticItem, out flag);
			if (cosmeticSlot == CosmeticsController.CosmeticSlots.Count)
			{
				Debug.LogWarning("Could not determine slot for: " + cosmeticItem.displayName);
				return null;
			}
			CosmeticsController.CosmeticItem cosmeticItem2 = this.controller.currentWornSet.items[(int)cosmeticSlot];
			if (!cosmeticItem2.isNullItem && cosmeticItem2.itemName == cosmeticItem.itemName)
			{
				return null;
			}
			this.controller.ApplyCosmeticItemToSet(this.controller.tempUnlockedSet, cosmeticItem, flag, false);
			this.controller.UpdateWornCosmetics(true);
			return new CosmeticSwapper.CosmeticState?(new CosmeticSwapper.CosmeticState
			{
				cosmeticId = nameOrId,
				replacedItem = cosmeticItem2,
				slot = cosmeticSlot,
				isLeftHand = flag
			});
		}

		// Token: 0x06007AF0 RID: 31472 RVA: 0x00281944 File Offset: 0x0027FB44
		private void RestorePreviousCosmetic(CosmeticSwapper.CosmeticState state)
		{
			if (this.controller == null)
			{
				return;
			}
			CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(state.cosmeticId);
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			this.controller.RemoveCosmeticItemFromSet(this.controller.tempUnlockedSet, cosmeticItem.displayName, false);
			if (!state.replacedItem.isNullItem)
			{
				this.controller.ApplyCosmeticItemToSet(this.controller.tempUnlockedSet, state.replacedItem, state.isLeftHand, false);
			}
			this.controller.UpdateWornCosmetics(true);
		}

		// Token: 0x06007AF1 RID: 31473 RVA: 0x002819D0 File Offset: 0x0027FBD0
		private CosmeticsController.CosmeticItem FindItem(string nameOrId)
		{
			CosmeticsController.CosmeticItem cosmeticItem;
			if (this.controller.allCosmeticsDict.TryGetValue(nameOrId, out cosmeticItem))
			{
				return cosmeticItem;
			}
			string text;
			if (this.controller.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(nameOrId, out text))
			{
				return this.controller.GetItemFromDict(text);
			}
			return this.controller.nullItem;
		}

		// Token: 0x06007AF2 RID: 31474 RVA: 0x00281A24 File Offset: 0x0027FC24
		private CosmeticsController.CosmeticSlots GetCosmeticSlot(CosmeticsController.CosmeticItem item, out bool isLeftHand)
		{
			isLeftHand = false;
			if (!item.isHoldable)
			{
				return CosmeticsController.CategoryToNonTransferrableSlot(item.itemCategory);
			}
			CosmeticsController.CosmeticSet currentWornSet = this.controller.currentWornSet;
			CosmeticsController.CosmeticItem cosmeticItem = currentWornSet.items[7];
			CosmeticsController.CosmeticItem cosmeticItem2 = currentWornSet.items[8];
			if (cosmeticItem.isNullItem || (!cosmeticItem2.isNullItem && item.itemName == cosmeticItem.itemName))
			{
				isLeftHand = true;
			}
			if (!isLeftHand)
			{
				return CosmeticsController.CosmeticSlots.HandRight;
			}
			return CosmeticsController.CosmeticSlots.HandLeft;
		}

		// Token: 0x17000C08 RID: 3080
		// (get) Token: 0x06007AF3 RID: 31475 RVA: 0x00281A99 File Offset: 0x0027FC99
		// (set) Token: 0x06007AF4 RID: 31476 RVA: 0x00281AA1 File Offset: 0x0027FCA1
		public bool TickRunning { get; set; }

		// Token: 0x06007AF5 RID: 31477 RVA: 0x00281AAC File Offset: 0x0027FCAC
		public void Tick()
		{
			if (this.newSwappedCosmetics.Count > 0)
			{
				if (this.GetCurrentMode() == CosmeticSwapper.SwapMode.StepByStep)
				{
					if (this.isAtFinalCosmeticStep && this.ShouldHoldFinalStep())
					{
						if (Time.time - this.lastCosmeticSwapTime <= this.stepTimeout)
						{
							return;
						}
						this.isAtFinalCosmeticStep = false;
					}
					if (Time.time - this.lastCosmeticSwapTime > this.stepTimeout)
					{
						while (this.newSwappedCosmetics.Count > 0)
						{
							CosmeticSwapper.CosmeticState cosmeticState = this.newSwappedCosmetics.Pop();
							this.RestorePreviousCosmetic(cosmeticState);
						}
						this.isAtFinalCosmeticStep = false;
						this.lastCosmeticSwapTime = float.PositiveInfinity;
						return;
					}
				}
				else if (this.GetCurrentMode() == CosmeticSwapper.SwapMode.AllAtOnce && Time.time - this.lastCosmeticSwapTime > this.stepTimeout)
				{
					while (this.newSwappedCosmetics.Count > 0)
					{
						CosmeticSwapper.CosmeticState cosmeticState2 = this.newSwappedCosmetics.Pop();
						this.RestorePreviousCosmetic(cosmeticState2);
					}
					this.lastCosmeticSwapTime = float.PositiveInfinity;
					this.isAtFinalCosmeticStep = false;
				}
			}
		}

		// Token: 0x06007AF6 RID: 31478 RVA: 0x00281B9D File Offset: 0x0027FD9D
		private void AddNewSwappedCosmetic(CosmeticSwapper.CosmeticState state)
		{
			this.newSwappedCosmetics.Push(state);
			this.lastCosmeticSwapTime = Time.time;
		}

		// Token: 0x06007AF7 RID: 31479 RVA: 0x00281BB6 File Offset: 0x0027FDB6
		private void MarkFinalCosmeticStep()
		{
			this.isAtFinalCosmeticStep = true;
			this.lastCosmeticSwapTime = Time.time;
		}

		// Token: 0x06007AF8 RID: 31480 RVA: 0x00281BCA File Offset: 0x0027FDCA
		private void UnmarkFinalCosmeticStep()
		{
			this.isAtFinalCosmeticStep = false;
		}

		// Token: 0x04008C76 RID: 35958
		[SerializeField]
		private List<string> cosmeticIDs = new List<string>();

		// Token: 0x04008C77 RID: 35959
		[SerializeField]
		private CosmeticSwapper.SwapMode swapMode = CosmeticSwapper.SwapMode.StepByStep;

		// Token: 0x04008C78 RID: 35960
		[Tooltip("Optional. When assigned, TriggerSwap sources the cosmetic ID from the cycle controller's active sub-item instead of the cosmeticIDs list. Use SwapMode.Random to call CycleRandom() automatically on each hit before reading the active sub-item.")]
		[SerializeField]
		private SubCosmeticCycleController cycleController;

		// Token: 0x04008C79 RID: 35961
		[SerializeField]
		private float stepTimeout = 10f;

		// Token: 0x04008C7A RID: 35962
		[Tooltip("Hold final step as long as the swapper is being called within the timeframe")]
		[SerializeField]
		private bool holdFinalStep = true;

		// Token: 0x04008C7B RID: 35963
		[SerializeField]
		private UnityEvent<VRRig> OnSwappingSequenceCompleted;

		// Token: 0x04008C7C RID: 35964
		[SerializeField]
		private List<GameModeType> gameModeExclusion = new List<GameModeType>();

		// Token: 0x04008C7D RID: 35965
		private CosmeticsController controller;

		// Token: 0x04008C7E RID: 35966
		private Stack<CosmeticSwapper.CosmeticState> newSwappedCosmetics = new Stack<CosmeticSwapper.CosmeticState>();

		// Token: 0x04008C7F RID: 35967
		private float lastCosmeticSwapTime = float.PositiveInfinity;

		// Token: 0x04008C80 RID: 35968
		private bool isAtFinalCosmeticStep;

		// Token: 0x02001320 RID: 4896
		private enum SwapMode
		{
			// Token: 0x04008C83 RID: 35971
			AllAtOnce,
			// Token: 0x04008C84 RID: 35972
			StepByStep,
			// Token: 0x04008C85 RID: 35973
			Random
		}

		// Token: 0x02001321 RID: 4897
		private struct CosmeticState
		{
			// Token: 0x04008C86 RID: 35974
			public string cosmeticId;

			// Token: 0x04008C87 RID: 35975
			public CosmeticsController.CosmeticItem replacedItem;

			// Token: 0x04008C88 RID: 35976
			public CosmeticsController.CosmeticSlots slot;

			// Token: 0x04008C89 RID: 35977
			public bool isLeftHand;
		}
	}
}
