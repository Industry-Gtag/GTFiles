using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics;

public class CosmeticSwapForcedState : MonoBehaviour, ISpawnable, ITickSystemTick
{
	[Tooltip("Play Fab ID (or item name) of the cosmetic this is on, matched against CosmeticSwapper.cosmeticIDs.")]
	[SerializeField]
	private string cosmeticId;

	[SerializeField]
	private NetworkedWearable wearable;

	private static readonly Dictionary<VRRig, List<CosmeticSwapForcedState>> s_byRig = new Dictionary<VRRig, List<CosmeticSwapForcedState>>();

	private VRRig rig;

	private bool isLocal;

	private bool registered;

	private float forcedUntil = float.NegativeInfinity;

	private bool requestedState;

	public string CosmeticId => cosmeticId;

	public bool IsStateOn
	{
		get
		{
			if (wearable != null)
			{
				return wearable.IsStateOn;
			}
			return false;
		}
	}

	private bool IsForced => Time.time < forcedUntil;

	public bool TickRunning { get; set; }

	public bool IsSpawned { get; set; }

	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void StaticReset()
	{
		s_byRig.Clear();
	}

	public static List<CosmeticSwapForcedState> GetForRig(VRRig vrRig)
	{
		if (!(vrRig != null) || !s_byRig.TryGetValue(vrRig, out var value))
		{
			return null;
		}
		return value;
	}

	public void SetState(bool state)
	{
		if (IsForced)
		{
			requestedState = state;
		}
		else if (wearable != null)
		{
			wearable.SetWearableStateBool(state);
		}
	}

	public void Force(float duration)
	{
		if (isLocal && !(wearable == null))
		{
			if (!IsForced)
			{
				requestedState = wearable.IsStateOn;
			}
			forcedUntil = Time.time + duration;
			wearable.SetWearableStateBool(newState: true);
			TickSystem<object>.AddTickCallback(this);
		}
	}

	public void Tick()
	{
		if (!IsForced)
		{
			TickSystem<object>.RemoveTickCallback(this);
			if (wearable != null)
			{
				wearable.SetWearableStateBool(requestedState);
			}
		}
	}

	private void OnEnable()
	{
		if (IsSpawned)
		{
			Register();
		}
	}

	private void OnDisable()
	{
		Unregister();
		TickSystem<object>.RemoveTickCallback(this);
		forcedUntil = float.NegativeInfinity;
	}

	private void Register()
	{
		if (!registered && !(rig == null))
		{
			if (!s_byRig.TryGetValue(rig, out var value))
			{
				value = new List<CosmeticSwapForcedState>();
				s_byRig[rig] = value;
			}
			value.Add(this);
			registered = true;
		}
	}

	private void Unregister()
	{
		if (!registered)
		{
			return;
		}
		registered = false;
		if (rig != null && s_byRig.TryGetValue(rig, out var value))
		{
			value.Remove(this);
			if (value.Count == 0)
			{
				s_byRig.Remove(rig);
			}
		}
	}

	public void OnSpawn(VRRig vrRig)
	{
		rig = vrRig;
		isLocal = vrRig != null && vrRig.isLocal;
		if (base.isActiveAndEnabled)
		{
			Register();
		}
	}

	public void OnDespawn()
	{
		Unregister();
	}
}
