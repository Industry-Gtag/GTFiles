using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000931 RID: 2353
public class TappableGeneric : Tappable
{
	// Token: 0x06003DAA RID: 15786 RVA: 0x0014E53B File Offset: 0x0014C73B
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		UnityEvent onTapped = this.OnTapped;
		if (onTapped == null)
		{
			return;
		}
		onTapped.Invoke();
	}

	// Token: 0x04004E67 RID: 20071
	[Tooltip("Invoked when this object is tapped. Fires on every client by default; if localOnly is true, fires only on the tapping player's client.")]
	[SerializeField]
	protected UnityEvent OnTapped;
}
