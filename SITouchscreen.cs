using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class SITouchscreen : MonoBehaviour
{
	// Token: 0x060009D1 RID: 2513 RVA: 0x000350B4 File Offset: 0x000332B4
	private void OnTriggerEnter(Collider other)
	{
		this.OnTriggerStay(other);
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x000350C0 File Offset: 0x000332C0
	private void OnTriggerStay(Collider other)
	{
		Transform indicator = this.GetIndicator(other);
		if (indicator != null)
		{
			this.controllingTransform = indicator;
			this.lastTouched = Time.time;
		}
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x000350F0 File Offset: 0x000332F0
	private void OnTriggerExit(Collider other)
	{
		if (this.controllingTransform == null || this.GetIndicator(other) != this.controllingTransform)
		{
			return;
		}
		this.controllingTransform = null;
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x0003511C File Offset: 0x0003331C
	private Transform GetIndicator(Collider other)
	{
		if (this.notFingerTouchDict.Contains(other))
		{
			return null;
		}
		GorillaTriggerColliderHandIndicator componentInParent;
		if (!this.fingerTouchDict.TryGetValue(other, out componentInParent))
		{
			componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				this.notFingerTouchDict.Add(other);
				return null;
			}
			this.fingerTouchDict.Add(other, componentInParent);
		}
		return componentInParent.transform;
	}

	// Token: 0x04000BF5 RID: 3061
	public Transform controllingTransform;

	// Token: 0x04000BF6 RID: 3062
	public float lastTouched;

	// Token: 0x04000BF7 RID: 3063
	public Vector3 lastPosition;

	// Token: 0x04000BF8 RID: 3064
	private Dictionary<Collider, GorillaTriggerColliderHandIndicator> fingerTouchDict = new Dictionary<Collider, GorillaTriggerColliderHandIndicator>();

	// Token: 0x04000BF9 RID: 3065
	private HashSet<Collider> notFingerTouchDict = new HashSet<Collider>();
}
