using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("GorillaTag/Seasonal/Snowball Grab Zone")]
public class SnowballGrabZone : HoldableObject, ISpawnable
{
	[GorillaSoundLookup]
	public int materialIndex;

	[Tooltip("When this grab zone is part of a cosmetic, only the player wearing the cosmetic can grab from it. Has no effect on grab zones that aren't spawned as part of a cosmetic (e.g. scene snow piles).")]
	[SerializeField]
	private bool onlyGrabbableByOwner;

	[Tooltip("Invoked locally (on the grabbing player's client only) after a snowball is successfully created in the grabbing hand. To make the effect visible to other players, route it through a synced component such as SyncedSwitchableCosmeticPart.")]
	[SerializeField]
	private UnityEvent onSnowballGrabbed;

	private VRRig ownerRig;

	public bool CanLocalPlayerGrab
	{
		get
		{
			if (onlyGrabbableByOwner && !(ownerRig == null))
			{
				return ownerRig.isLocal;
			}
			return true;
		}
	}

	bool ISpawnable.IsSpawned { get; set; }

	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	void ISpawnable.OnSpawn(VRRig rig)
	{
		ownerRig = rig;
		if (CanLocalPlayerGrab)
		{
			return;
		}
		InteractionPoint[] componentsInChildren = GetComponentsInChildren<InteractionPoint>(includeInactive: true);
		foreach (InteractionPoint interactionPoint in componentsInChildren)
		{
			if (!(interactionPoint.parentHoldableObject != base.gameObject))
			{
				interactionPoint.enabled = false;
				Collider component = interactionPoint.GetComponent<Collider>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
		}
	}

	void ISpawnable.OnDespawn()
	{
	}

	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	public override void DropItemCleanup()
	{
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (CanLocalPlayerGrab)
		{
			bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
			if (!(flag ? EquipmentInteractor.instance.disableLeftGrab : EquipmentInteractor.instance.disableRightGrab) && (flag ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(materialIndex, out var _))
			{
				onSnowballGrabbed?.Invoke();
			}
		}
	}
}
