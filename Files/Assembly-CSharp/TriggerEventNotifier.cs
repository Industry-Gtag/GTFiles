using UnityEngine;

public class TriggerEventNotifier : MonoBehaviour
{
	public delegate void TriggerEvent(TriggerEventNotifier notifier, Collider collider);

	[HideInInspector]
	public int maskIndex;

	public event TriggerEvent TriggerEnterEvent;

	public event TriggerEvent TriggerExitEvent;

	private void OnTriggerEnter(Collider other)
	{
		TriggerEnterEvent?.Invoke(this, other);
	}

	private void OnTriggerExit(Collider other)
	{
		TriggerExitEvent?.Invoke(this, other);
	}
}
