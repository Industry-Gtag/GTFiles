using Cosmetics;
using UnityEngine;

namespace GorillaNetworking.Store;

public class EndCapSpawnPoint : MonoBehaviour
{
	[Tooltip("Where BundleManager spawns this end cap's bundle stand. Defaults to this transform. On timed end caps, point this at an object under objectsToDisableWhenInactive (e.g. EndCap/Cosmetics) so the stand is hidden with it.")]
	[SerializeField]
	private Transform standParent;

	[Tooltip("Show \"GET NOW!\" instead of the IAP price on this end cap's purchase buttons, and hide the fan club (VIM) price label.")]
	[SerializeField]
	private bool hidePurchasePrice;

	[Tooltip("Optional. When set, this end cap is only live while the TitleDataActivation's title data window is active. Use a TitleDataActivation with no children of its own, since it toggles all of its children.")]
	[SerializeField]
	private TitleDataActivation titleDataActivation;

	[Tooltip("Objects turned off while outside the active date range (i.e. the cosmetic stands).")]
	[SerializeField]
	private GameObject[] objectsToDisableWhenInactive;

	[Tooltip("Bundle purchase buttons locked while outside the active date range.")]
	[SerializeField]
	private BundlePurchaseButton[] purchaseButtonsToLockWhenInactive;

	[Tooltip("Also lock any BundlePurchaseButtons parented under this end cap (e.g. on spawned bundle stands).")]
	[SerializeField]
	private bool includeChildPurchaseButtons = true;

	[Tooltip("Other pressable buttons disabled while outside the active date range.")]
	[SerializeField]
	private GorillaPressableButton[] buttonsToDisableWhenInactive;

	[Tooltip("While inside the active date range, put this end cap's bundles in a free slot on the try on stand. Bundles only sold on timed end caps are always hidden from the try on stand while outside it.")]
	[SerializeField]
	private bool addBundlesToTryOnStand = true;

	[Tooltip("Creator code terminal for this end cap. Auto-filled from children (including inactive) when empty.")]
	[SerializeField]
	private CreatorCodeTerminal creatorCodeTerminal;

	[Tooltip("When off, the creator code terminal is kept disabled, bundle stands here purchase without a creator code, and the missing Creator Code Provider warnings are suppressed for them.")]
	[SerializeField]
	private bool useCreatorCodeTerminal = true;

	private bool HasTitleDataGate => titleDataActivation != null;

	public bool IsEndCapActive
	{
		get
		{
			if (HasTitleDataGate)
			{
				return titleDataActivation.IsActive;
			}
			return true;
		}
	}

	public Transform StandParent
	{
		get
		{
			if (!(standParent != null))
			{
				return base.transform;
			}
			return standParent;
		}
	}

	public bool HidesPurchasePrice => hidePurchasePrice;

	public CreatorCodeTerminal CreatorCodeTerminal => creatorCodeTerminal;

	public bool UsesCreatorCodeTerminal => useCreatorCodeTerminal;

	private void OnEnable()
	{
		if (!HasTitleDataGate)
		{
			SetCreatorCodeTerminalActive(isActive: true);
			return;
		}
		titleDataActivation.OnActivationStateChanged += OnTitleDataActivationChanged;
		ApplyActiveState(titleDataActivation.IsActive);
	}

	private void Start()
	{
		if (HasTitleDataGate)
		{
			UpdateTryOnBundles(titleDataActivation.IsActive);
		}
	}

	private void OnDisable()
	{
		if (HasTitleDataGate)
		{
			titleDataActivation.OnActivationStateChanged -= OnTitleDataActivationChanged;
		}
	}

	private void OnTitleDataActivationChanged(bool isActive)
	{
		ApplyActiveState(isActive);
	}

	private void ApplyActiveState(bool isActive)
	{
		SetObjectsActive(isActive);
		SetPurchaseButtonsLocked(!isActive);
		SetButtonsEnabled(isActive);
		SetCreatorCodeTerminalActive(isActive);
		UpdateTryOnBundles(isActive);
		ApplyTextSwaps(isActive);
	}

	private void UpdateTryOnBundles(bool isActive)
	{
		if (!(BundleManager.instance == null))
		{
			BundleStand[] componentsInChildren = GetComponentsInChildren<BundleStand>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				BundleManager.instance.SetTimedTryOnBundle(componentsInChildren[i]._bundleDataReference, this, isActive, addBundlesToTryOnStand);
			}
		}
	}

	private void SetCreatorCodeTerminalActive(bool isActive)
	{
		if (!(creatorCodeTerminal == null))
		{
			creatorCodeTerminal.gameObject.SetActive(isActive && useCreatorCodeTerminal);
		}
	}

	private void SetObjectsActive(bool isActive)
	{
		if (objectsToDisableWhenInactive == null)
		{
			return;
		}
		for (int i = 0; i < objectsToDisableWhenInactive.Length; i++)
		{
			if (objectsToDisableWhenInactive[i] != null)
			{
				objectsToDisableWhenInactive[i].SetActive(isActive);
			}
		}
	}

	private void SetPurchaseButtonsLocked(bool locked)
	{
		if (purchaseButtonsToLockWhenInactive != null)
		{
			for (int i = 0; i < purchaseButtonsToLockWhenInactive.Length; i++)
			{
				if (purchaseButtonsToLockWhenInactive[i] != null)
				{
					purchaseButtonsToLockWhenInactive[i].SetPurchaseLocked(locked);
				}
			}
		}
		if (includeChildPurchaseButtons)
		{
			BundlePurchaseButton[] componentsInChildren = GetComponentsInChildren<BundlePurchaseButton>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].SetPurchaseLocked(locked);
			}
		}
	}

	private void SetButtonsEnabled(bool isEnabled)
	{
		if (buttonsToDisableWhenInactive == null)
		{
			return;
		}
		for (int i = 0; i < buttonsToDisableWhenInactive.Length; i++)
		{
			GorillaPressableButton gorillaPressableButton = buttonsToDisableWhenInactive[i];
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.enabled = isEnabled;
				if (gorillaPressableButton.TryGetComponent<Collider>(out var component))
				{
					component.enabled = isEnabled;
				}
			}
		}
	}

	private void ApplyTextSwaps(bool isActive)
	{
	}
}
