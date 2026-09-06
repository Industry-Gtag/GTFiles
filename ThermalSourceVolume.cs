using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003A8 RID: 936
public class ThermalSourceVolume : MonoBehaviour
{
	// Token: 0x060016B3 RID: 5811 RVA: 0x00083732 File Offset: 0x00081932
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x0008373A File Offset: 0x0008193A
	protected void OnDisable()
	{
		ThermalManager.Unregister(this);
	}

	// Token: 0x040020C9 RID: 8393
	[Tooltip("Temperature in celsius. Default is 20 which is room temperature.")]
	public float celsius = 20f;

	// Token: 0x040020CA RID: 8394
	public float innerRadius = 0.1f;

	// Token: 0x040020CB RID: 8395
	public float outerRadius = 1f;

	// Token: 0x040020CC RID: 8396
	[Tooltip("Exclude these thermal receivers from being impacted by this source")]
	public List<ThermalReceiver> exclusionReceivers = new List<ThermalReceiver>();
}
