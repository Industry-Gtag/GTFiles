using Liv.Lck.Cosmetics;
using UnityEngine;

public class GtLckNetworkCosmeticDependantPlayerIdSupplier : MonoBehaviour, ILckCosmeticDependantPlayerIdSupplier
{
	[SerializeField]
	private VRRig vrrig;

	public event PlayerIdUpdatedEvent PlayerIdUpdated;

	public string GetPlayerId()
	{
		return vrrig.OwningNetPlayer.UserId;
	}

	public void UpdatePlayerId()
	{
		PlayerIdUpdated?.Invoke();
	}
}
