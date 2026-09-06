using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200068F RID: 1679
public class Decelerate : MonoBehaviour
{
	// Token: 0x060029FA RID: 10746 RVA: 0x000E26A8 File Offset: 0x000E08A8
	public void Restart()
	{
		base.enabled = true;
	}

	// Token: 0x060029FB RID: 10747 RVA: 0x000E26B4 File Offset: 0x000E08B4
	private void Update()
	{
		if (!this._rigidbody)
		{
			return;
		}
		Vector3 vector = this._rigidbody.linearVelocity;
		vector *= this._friction;
		if (vector.Approx0(0.001f))
		{
			this._rigidbody.linearVelocity = Vector3.zero;
			UnityEvent unityEvent = this.onStop;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			base.enabled = false;
		}
		else
		{
			this._rigidbody.linearVelocity = vector;
		}
		if (this._resetOrientationOnRelease && !this._rigidbody.rotation.Approx(Quaternion.identity, 1E-06f))
		{
			this._rigidbody.rotation = Quaternion.identity;
		}
	}

	// Token: 0x04003689 RID: 13961
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x0400368A RID: 13962
	[SerializeField]
	private float _friction = 0.875f;

	// Token: 0x0400368B RID: 13963
	[SerializeField]
	private bool _resetOrientationOnRelease;

	// Token: 0x0400368C RID: 13964
	public UnityEvent onStop;
}
