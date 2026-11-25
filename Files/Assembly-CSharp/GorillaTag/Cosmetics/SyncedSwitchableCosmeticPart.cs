using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics;

[AddComponentMenu("GorillaTag/Cosmetics/Synced Switchable Cosmetic Part")]
public class SyncedSwitchableCosmeticPart : MonoBehaviour, ISpawnable
{
	private const string k_slotTooltip = "Slot of VRRig.WearablePackedStates to sync the active state through. The first bit of the slot is used. Pick a slot no other synced component on a co-wearable cosmetic uses (usually the slot matching this cosmetic's category).";

	private const string k_slotInvalidMsg = "This slot has no entry in VRRig.WearablePackedStatesBitWriteInfos.";

	[Tooltip("Slot of VRRig.WearablePackedStates to sync the active state through. The first bit of the slot is used. Pick a slot no other synced component on a co-wearable cosmetic uses (usually the slot matching this cosmetic's category).\n\nUsed when the cosmetic is equipped on the left side, or on Both sides.")]
	[SerializeField]
	private VRRig.WearablePackedStateSlots assignedSlotLeft = VRRig.WearablePackedStateSlots.Arms_Left;

	[Tooltip("Slot of VRRig.WearablePackedStates to sync the active state through. The first bit of the slot is used. Pick a slot no other synced component on a co-wearable cosmetic uses (usually the slot matching this cosmetic's category).\n\nUsed when the cosmetic is equipped on the right side.")]
	[SerializeField]
	private VRRig.WearablePackedStateSlots assignedSlotRight = VRRig.WearablePackedStateSlots.Arms_Right;

	[Tooltip("GameObjects deactivated while active (and activated while inactive). Deactivating an InteractionPoint's GameObject also disables grabbing from it.")]
	[SerializeField]
	private GameObject[] partsToHide = new GameObject[0];

	[Tooltip("GameObjects activated while active (and deactivated while inactive).")]
	[SerializeField]
	private GameObject[] partsToShow = new GameObject[0];

	[Tooltip("Whether this starts out active. The synced bit stores \"different from the starting state\", so a cleared bit (the default for every player) always means this starting state.")]
	[SerializeField]
	private bool startActive;

	[SerializeField]
	private UnityEvent onActivate;

	[SerializeField]
	private UnityEvent onDeactivate;

	private VRRig ownerRig;

	private bool isLocal;

	private GTBitOps.BitWriteInfo bitInfo;

	private bool isActivated;

	private float deactivateAtTime = float.PositiveInfinity;

	public bool IsActivated => isActivated;

	public bool IsSpawned { get; set; }

	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	public void OnSpawn(VRRig rig)
	{
		ownerRig = rig;
		isLocal = rig != null && rig.isLocal;
		VRRig.WearablePackedStateSlots wearablePackedStateSlots = ((CosmeticSelectedSide == ECosmeticSelectSide.Right) ? assignedSlotRight : assignedSlotLeft);
		int num = (int)wearablePackedStateSlots;
		if (num < 0 || num >= VRRig.WearablePackedStatesBitWriteInfos.Length)
		{
			GTDev.LogError(string.Format("{0}: Slot {1} has no bit layout entry, disabling. ", "SyncedSwitchableCosmeticPart", wearablePackedStateSlots) + "Path: \"" + base.transform.GetPath() + "\"", this);
			ownerRig = null;
			base.enabled = false;
		}
		else
		{
			bitInfo = VRRig.WearablePackedStatesBitWriteInfos[num];
		}
	}

	public void OnDespawn()
	{
	}

	public void Activate()
	{
		if (CanWrite())
		{
			deactivateAtTime = float.PositiveInfinity;
			WriteLocalState(activated: true);
		}
	}

	public void ActivateForSeconds(float seconds)
	{
		if (CanWrite())
		{
			deactivateAtTime = ((seconds > 0f) ? (Time.time + seconds) : float.PositiveInfinity);
			WriteLocalState(activated: true);
		}
	}

	public void Deactivate()
	{
		if (CanWrite())
		{
			deactivateAtTime = float.PositiveInfinity;
			WriteLocalState(activated: false);
		}
	}

	protected void OnEnable()
	{
		deactivateAtTime = float.PositiveInfinity;
		if (isActivated != startActive)
		{
			ApplyState(startActive, invokeEvents: false);
		}
	}

	protected void OnDisable()
	{
		if (isLocal && ownerRig != null && isActivated != startActive)
		{
			ownerRig.WearablePackedStates = GTBitOps.WriteBits(ownerRig.WearablePackedStates, bitInfo, 0);
		}
	}

	protected void LateUpdate()
	{
		if (!IsSpawned || ownerRig == null)
		{
			return;
		}
		if (isLocal)
		{
			if (isActivated && Time.time >= deactivateAtTime)
			{
				deactivateAtTime = float.PositiveInfinity;
				WriteLocalState(activated: false);
			}
		}
		else
		{
			bool flag = GTBitOps.ReadBits(ownerRig.WearablePackedStates, bitInfo) != 0 != startActive;
			if (flag != isActivated)
			{
				ApplyState(flag, invokeEvents: true);
			}
		}
	}

	private bool CanWrite()
	{
		if (isLocal && IsSpawned && ownerRig != null)
		{
			return base.isActiveAndEnabled;
		}
		return false;
	}

	private void WriteLocalState(bool activated)
	{
		bool flag = activated != startActive;
		ownerRig.WearablePackedStates = GTBitOps.WriteBits(ownerRig.WearablePackedStates, bitInfo, flag ? 1 : 0);
		if (activated != isActivated)
		{
			ApplyState(activated, invokeEvents: true);
		}
	}

	private void ApplyState(bool activated, bool invokeEvents)
	{
		isActivated = activated;
		SetPartsActive(partsToHide, !activated);
		SetPartsActive(partsToShow, activated);
		if (invokeEvents)
		{
			if (activated)
			{
				onActivate?.Invoke();
			}
			else
			{
				onDeactivate?.Invoke();
			}
		}
	}

	private static void SetPartsActive(GameObject[] parts, bool active)
	{
		if (parts == null)
		{
			return;
		}
		foreach (GameObject gameObject in parts)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(active);
			}
		}
	}
}
