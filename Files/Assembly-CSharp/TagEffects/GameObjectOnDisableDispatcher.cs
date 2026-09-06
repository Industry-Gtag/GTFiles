using UnityEngine;

namespace TagEffects;

public class GameObjectOnDisableDispatcher : MonoBehaviour
{
	public delegate void OnDisabledEvent(GameObjectOnDisableDispatcher me);

	public event OnDisabledEvent OnDisabled;

	private void OnDisable()
	{
		if (OnDisabled != null)
		{
			OnDisabled(this);
		}
	}
}
