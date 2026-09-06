using UnityEngine;

public class ThrowableBugBeacon : MonoBehaviour
{
	public delegate void ThrowableBugBeaconEvent(ThrowableBugBeacon tbb);

	public delegate void ThrowableBugBeaconFloatEvent(ThrowableBugBeacon tbb, float f);

	[SerializeField]
	private float range;

	[SerializeField]
	private ThrowableBug.BugName bugName;

	public ThrowableBug.BugName BugName => bugName;

	public float Range => range;

	public static event ThrowableBugBeaconEvent OnCall;

	public static event ThrowableBugBeaconEvent OnDismiss;

	public static event ThrowableBugBeaconEvent OnLock;

	public static event ThrowableBugBeaconEvent OnUnlock;

	public static event ThrowableBugBeaconFloatEvent OnChangeSpeedMultiplier;

	public void Call()
	{
		if (OnCall != null)
		{
			OnCall(this);
		}
	}

	public void Dismiss()
	{
		if (OnDismiss != null)
		{
			OnDismiss(this);
		}
	}

	public void Lock()
	{
		if (OnLock != null)
		{
			OnLock(this);
		}
	}

	public void Unlock()
	{
		if (OnUnlock != null)
		{
			OnUnlock(this);
		}
	}

	public void ChangeSpeedMultiplier(float f)
	{
		if (OnChangeSpeedMultiplier != null)
		{
			OnChangeSpeedMultiplier(this, f);
		}
	}

	private void OnDisable()
	{
		if (OnUnlock != null)
		{
			OnUnlock(this);
		}
	}
}
