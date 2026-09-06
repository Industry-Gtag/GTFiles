using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000A6C RID: 2668
public class CustomMapTelemetryTrigger : MonoBehaviour
{
	// Token: 0x060044A0 RID: 17568 RVA: 0x0016F0AB File Offset: 0x0016D2AB
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			CustomMapTelemetry.OnPlayerLeftMap();
			if (CustomMapTelemetry.IsActive)
			{
				CustomMapTelemetry.EndMapTracking();
			}
		}
	}

	// Token: 0x060044A1 RID: 17569 RVA: 0x0016F0D0 File Offset: 0x0016D2D0
	public void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump())
		{
			CustomMapTelemetry.OnPlayerEnteredMap();
			if (!CustomMapTelemetry.IsActive)
			{
				CustomMapTelemetry.StartMapTracking();
			}
		}
	}
}
