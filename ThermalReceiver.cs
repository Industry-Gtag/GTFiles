using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003A7 RID: 935
public class ThermalReceiver : MonoBehaviour, IDynamicFloat, IResettableItem
{
	// Token: 0x17000235 RID: 565
	// (get) Token: 0x060016AC RID: 5804 RVA: 0x000836B3 File Offset: 0x000818B3
	public float Farenheit
	{
		get
		{
			return this.celsius * 1.8f + 32f;
		}
	}

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x060016AD RID: 5805 RVA: 0x000836C7 File Offset: 0x000818C7
	public float floatValue
	{
		get
		{
			return this.celsius;
		}
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x000836CF File Offset: 0x000818CF
	protected void Awake()
	{
		this.defaultCelsius = this.celsius;
		this.wasAboveThreshold = false;
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x000836E4 File Offset: 0x000818E4
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x000836EC File Offset: 0x000818EC
	protected void OnDisable()
	{
		this.wasAboveThreshold = false;
		ThermalManager.Unregister(this);
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x000836FB File Offset: 0x000818FB
	public void ResetToDefaultState()
	{
		this.celsius = this.defaultCelsius;
	}

	// Token: 0x040020BF RID: 8383
	public float radius = 0.2f;

	// Token: 0x040020C0 RID: 8384
	[Tooltip("How fast the temperature should change overtime. 1.0 would be instantly.")]
	public float conductivity = 0.3f;

	// Token: 0x040020C1 RID: 8385
	public ContinuousPropertyArray continuousProperties;

	// Token: 0x040020C2 RID: 8386
	[Tooltip("Optional: Fire events if temperature goes below or above this threshold - Celsius")]
	public float temperatureThreshold;

	// Token: 0x040020C3 RID: 8387
	[Tooltip("Exclude these thermal sources from impacting this receiver")]
	public List<ThermalSourceVolume> exclusionSources = new List<ThermalSourceVolume>();

	// Token: 0x040020C4 RID: 8388
	[Space]
	public UnityEvent OnAboveThreshold;

	// Token: 0x040020C5 RID: 8389
	public UnityEvent OnBelowThreshold;

	// Token: 0x040020C6 RID: 8390
	[DebugOption]
	public float celsius;

	// Token: 0x040020C7 RID: 8391
	public bool wasAboveThreshold;

	// Token: 0x040020C8 RID: 8392
	private float defaultCelsius;
}
