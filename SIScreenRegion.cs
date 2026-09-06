using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class SIScreenRegion : MonoBehaviour
{
	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000955 RID: 2389 RVA: 0x00032573 File Offset: 0x00030773
	public bool HasPressedButton
	{
		get
		{
			return this._hasPressedButton;
		}
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x0003257C File Offset: 0x0003077C
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent != null)
		{
			this.handIndicators.Add(componentInParent);
		}
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x000325A0 File Offset: 0x000307A0
	private void OnTriggerExit(Collider other)
	{
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent != null)
		{
			this.handIndicators.Remove(componentInParent);
			if (this.handIndicators.Count == 0)
			{
				this.ClearPressedIndicator();
			}
		}
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x000325D7 File Offset: 0x000307D7
	public void RegisterButtonPress()
	{
		if (this.handIndicators.Count > 0)
		{
			this._hasPressedButton = true;
		}
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x000325EE File Offset: 0x000307EE
	private void ClearPressedIndicator()
	{
		this._hasPressedButton = false;
	}

	// Token: 0x04000B6B RID: 2923
	private HashSet<GorillaTriggerColliderHandIndicator> handIndicators = new HashSet<GorillaTriggerColliderHandIndicator>();

	// Token: 0x04000B6C RID: 2924
	private bool _hasPressedButton;
}
