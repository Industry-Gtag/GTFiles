using System;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class PlayerGameEventLocationTrigger : MonoBehaviour
{
	// Token: 0x06000FE8 RID: 4072 RVA: 0x0005641A File Offset: 0x0005461A
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			PlayerGameEvents.TriggerEnterLocation(this.locationName);
		}
	}

	// Token: 0x04001324 RID: 4900
	[SerializeField]
	private string locationName;
}
