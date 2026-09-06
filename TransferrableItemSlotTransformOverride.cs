using System;
using System.Collections.Generic;
using System.Linq;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200054D RID: 1357
public class TransferrableItemSlotTransformOverride : MonoBehaviour, IGorillaSliceableSimple, ISpawnable
{
	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x0600223C RID: 8764 RVA: 0x000B74D3 File Offset: 0x000B56D3
	// (set) Token: 0x0600223D RID: 8765 RVA: 0x000B74DB File Offset: 0x000B56DB
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170003A2 RID: 930
	// (get) Token: 0x0600223E RID: 8766 RVA: 0x000B74E4 File Offset: 0x000B56E4
	// (set) Token: 0x0600223F RID: 8767 RVA: 0x000B74EC File Offset: 0x000B56EC
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06002240 RID: 8768 RVA: 0x000B74F8 File Offset: 0x000B56F8
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.defaultPosition = new SlotTransformOverride
		{
			positionState = TransferrableObject.PositionState.None
		};
		this.lastPosition = TransferrableObject.PositionState.None;
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			slotTransformOverride.Initialize(this, this.anchor);
		}
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x000B7568 File Offset: 0x000B5768
	public void AddGripPosition(TransferrableObject.PositionState state, TransferrableObjectGripPosition togp)
	{
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			if (slotTransformOverride.positionState == state)
			{
				slotTransformOverride.AddSubGrabPoint(togp);
				return;
			}
		}
		SlotTransformOverride slotTransformOverride2 = new SlotTransformOverride
		{
			positionState = state
		};
		this.transformOverridesDeprecated.Add(slotTransformOverride2);
		slotTransformOverride2.AddSubGrabPoint(togp);
	}

	// Token: 0x06002243 RID: 8771 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002244 RID: 8772 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x000B75E8 File Offset: 0x000B57E8
	public void SliceUpdate()
	{
		if (this.followingTransferrableObject == null)
		{
			return;
		}
		if (this.followingTransferrableObject.currentState != this.lastPosition)
		{
			SlotTransformOverride slotTransformOverride = this.transformOverridesDeprecated.Find((SlotTransformOverride x) => (x.positionState & this.followingTransferrableObject.currentState) > TransferrableObject.PositionState.None);
			if (slotTransformOverride != null && slotTransformOverride.positionState == TransferrableObject.PositionState.None)
			{
				slotTransformOverride = this.defaultPosition;
			}
		}
		this.lastPosition = this.followingTransferrableObject.currentState;
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x000B7652 File Offset: 0x000B5852
	private void Awake()
	{
		this.GenerateTransformFromPositionState();
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x000B765C File Offset: 0x000B585C
	public void GenerateTransformFromPositionState()
	{
		this.transformFromPosition = new Dictionary<TransferrableObject.PositionState, Transform>();
		if (this.transformOverridesDeprecated.Count > 0)
		{
			this.transformFromPosition[TransferrableObject.PositionState.None] = this.transformOverridesDeprecated[0].overrideTransform;
		}
		foreach (TransferrableObject.PositionState positionState in Enum.GetValues(typeof(TransferrableObject.PositionState)).Cast<TransferrableObject.PositionState>())
		{
			if (positionState == TransferrableObject.PositionState.None)
			{
				this.transformFromPosition[positionState] = null;
			}
			else
			{
				Transform transform = null;
				foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
				{
					if ((slotTransformOverride.positionState & positionState) != TransferrableObject.PositionState.None)
					{
						transform = slotTransformOverride.overrideTransform;
						break;
					}
				}
				this.transformFromPosition[positionState] = transform;
			}
		}
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x000B7758 File Offset: 0x000B5958
	[CanBeNull]
	public Transform GetTransformFromPositionState(TransferrableObject.PositionState currentState)
	{
		if (this.transformFromPosition == null)
		{
			this.GenerateTransformFromPositionState();
		}
		return this.transformFromPosition[currentState];
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000B7774 File Offset: 0x000B5974
	public bool GetTransformFromPositionState(TransferrableObject.PositionState currentState, AdvancedItemState advancedItemState, Transform targetDockXf, out Matrix4x4 matrix4X4)
	{
		if (currentState != TransferrableObject.PositionState.None)
		{
			foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
			{
				if ((slotTransformOverride.positionState & currentState) != TransferrableObject.PositionState.None)
				{
					if (!slotTransformOverride.useAdvancedGrab)
					{
						matrix4X4 = slotTransformOverride.overrideTransformMatrix;
						return true;
					}
					if (advancedItemState.index >= slotTransformOverride.multiPoints.Count)
					{
						matrix4X4 = slotTransformOverride.overrideTransformMatrix;
						return true;
					}
					SubGrabPoint subGrabPoint = slotTransformOverride.multiPoints[advancedItemState.index];
					matrix4X4 = subGrabPoint.GetTransformFromPositionState(advancedItemState, slotTransformOverride, targetDockXf);
					return true;
				}
			}
			matrix4X4 = Matrix4x4.identity;
			return false;
		}
		if (this.transformOverridesDeprecated.Count > 0)
		{
			matrix4X4 = this.transformOverridesDeprecated[0].overrideTransformMatrix;
			return true;
		}
		matrix4X4 = Matrix4x4.identity;
		return false;
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x000B7878 File Offset: 0x000B5A78
	public AdvancedItemState GetAdvancedItemStateFromHand(TransferrableObject.PositionState currentState, Transform handTransform, Transform targetDock)
	{
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			if ((slotTransformOverride.positionState & currentState) != TransferrableObject.PositionState.None && slotTransformOverride.multiPoints.Count != 0)
			{
				SubGrabPoint subGrabPoint = slotTransformOverride.multiPoints[0];
				float num = float.PositiveInfinity;
				int num2 = -1;
				for (int i = 0; i < slotTransformOverride.multiPoints.Count; i++)
				{
					SubGrabPoint subGrabPoint2 = slotTransformOverride.multiPoints[i];
					if (!(subGrabPoint2.gripPoint == null))
					{
						float num3 = subGrabPoint2.EvaluateScore(base.transform, handTransform, targetDock);
						if (num3 < num)
						{
							subGrabPoint = subGrabPoint2;
							num = num3;
							num2 = i;
						}
					}
				}
				AdvancedItemState advancedItemStateFromHand = subGrabPoint.GetAdvancedItemStateFromHand(base.transform, handTransform, targetDock, slotTransformOverride);
				advancedItemStateFromHand.index = num2;
				return advancedItemStateFromHand;
			}
		}
		return new AdvancedItemState();
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x000B7978 File Offset: 0x000B5B78
	public void Edit()
	{
		if (TransferrableItemSlotTransformOverride.OnBringUpWindow != null)
		{
			TransferrableItemSlotTransformOverride.OnBringUpWindow(base.GetComponent<TransferrableObject>());
		}
	}

	// Token: 0x04002D42 RID: 11586
	[FormerlySerializedAs("transformOverridesList")]
	public List<SlotTransformOverride> transformOverridesDeprecated;

	// Token: 0x04002D43 RID: 11587
	[SerializeReference]
	public List<SlotTransformOverride> transformOverrides;

	// Token: 0x04002D44 RID: 11588
	private TransferrableObject.PositionState lastPosition;

	// Token: 0x04002D45 RID: 11589
	[Tooltip("(2024-08-20 MattO) For cosmetics this is almost always assigned to the TransferrableObject component in the same prefab and almost always belonging to the same gameobject as this Component.")]
	public TransferrableObject followingTransferrableObject;

	// Token: 0x04002D46 RID: 11590
	[Tooltip("(2024-08-20 MattO) This is filled in automatically by the cosmetic spawner.")]
	public SlotTransformOverride defaultPosition;

	// Token: 0x04002D47 RID: 11591
	[Obsolete("(2024-08-2024) This used to be assigned to `defaultPosition.overrideTransform` before, but was there ever an instance where it wasn't null? Keeping it serialized just in case there is a reason for it.")]
	public Transform defaultTransform;

	// Token: 0x04002D48 RID: 11592
	public Transform anchor;

	// Token: 0x04002D4B RID: 11595
	public Dictionary<TransferrableObject.PositionState, Transform> transformFromPosition;

	// Token: 0x04002D4C RID: 11596
	public static Action<TransferrableObject> OnBringUpWindow;
}
