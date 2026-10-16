using System;
using UnityEngine;

public abstract class AnnoyingMonoBehaviour : MonoBehaviour, IAnnoyingFeature
{
	public static event Action<IAnnoyingFeature, bool> AlertActive;

	internal void OnEnable()
	{
		AlertActive?.Invoke(this, arg2: true);
	}

	internal void OnDisable()
	{
		AlertActive?.Invoke(this, arg2: false);
	}

	public abstract void Allowed(bool v);
}
