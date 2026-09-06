using System;
using System.Collections.Generic;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003A6 RID: 934
[DefaultExecutionOrder(-100)]
public class ThermalManager : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060016A3 RID: 5795 RVA: 0x00083468 File Offset: 0x00081668
	public void OnEnable()
	{
		if (ThermalManager.instance != null)
		{
			Debug.LogError("ThermalManager already exists!");
			return;
		}
		ThermalManager.instance = this;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.lastTime = Time.time;
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060016A5 RID: 5797 RVA: 0x0008349C File Offset: 0x0008169C
	public void SliceUpdate()
	{
		float num = Time.time - this.lastTime;
		this.lastTime = Time.time;
		for (int i = 0; i < ThermalManager.receivers.Count; i++)
		{
			ThermalReceiver thermalReceiver = ThermalManager.receivers[i];
			Transform transform = thermalReceiver.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num2 = 20f;
			for (int j = 0; j < ThermalManager.sources.Count; j++)
			{
				ThermalSourceVolume thermalSourceVolume = ThermalManager.sources[j];
				if ((thermalSourceVolume.exclusionReceivers.Count <= 0 || !thermalSourceVolume.exclusionReceivers.Contains(thermalReceiver)) && (thermalReceiver.exclusionSources.Count <= 0 || !thermalReceiver.exclusionSources.Contains(thermalSourceVolume)))
				{
					Transform transform2 = thermalSourceVolume.transform;
					float x2 = transform2.lossyScale.x;
					float num3 = Vector3.Distance(transform2.position, position);
					float num4 = 1f - Mathf.InverseLerp(thermalSourceVolume.innerRadius * x2, thermalSourceVolume.outerRadius * x2, num3 - thermalReceiver.radius * x);
					num2 += thermalSourceVolume.celsius * num4;
				}
			}
			thermalReceiver.celsius = Mathf.Lerp(thermalReceiver.celsius, num2, num * thermalReceiver.conductivity);
			ContinuousPropertyArray continuousProperties = thermalReceiver.continuousProperties;
			if (continuousProperties != null)
			{
				continuousProperties.ApplyAll(thermalReceiver.celsius);
			}
			if (!thermalReceiver.wasAboveThreshold && thermalReceiver.celsius > thermalReceiver.temperatureThreshold)
			{
				thermalReceiver.wasAboveThreshold = true;
				UnityEvent onAboveThreshold = thermalReceiver.OnAboveThreshold;
				if (onAboveThreshold != null)
				{
					onAboveThreshold.Invoke();
				}
			}
			else if (thermalReceiver.wasAboveThreshold && thermalReceiver.celsius < thermalReceiver.temperatureThreshold)
			{
				thermalReceiver.wasAboveThreshold = false;
				UnityEvent onBelowThreshold = thermalReceiver.OnBelowThreshold;
				if (onBelowThreshold != null)
				{
					onBelowThreshold.Invoke();
				}
			}
		}
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x0008365D File Offset: 0x0008185D
	public static void Register(ThermalSourceVolume source)
	{
		ThermalManager.sources.Add(source);
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x0008366A File Offset: 0x0008186A
	public static void Unregister(ThermalSourceVolume source)
	{
		ThermalManager.sources.Remove(source);
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x00083678 File Offset: 0x00081878
	public static void Register(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Add(receiver);
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x00083685 File Offset: 0x00081885
	public static void Unregister(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Remove(receiver);
	}

	// Token: 0x040020BB RID: 8379
	public static readonly List<ThermalSourceVolume> sources = new List<ThermalSourceVolume>(256);

	// Token: 0x040020BC RID: 8380
	public static readonly List<ThermalReceiver> receivers = new List<ThermalReceiver>(256);

	// Token: 0x040020BD RID: 8381
	[NonSerialized]
	public static ThermalManager instance;

	// Token: 0x040020BE RID: 8382
	private float lastTime;
}
