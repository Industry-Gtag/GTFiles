using System;
using GorillaTag;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020005B1 RID: 1457
public class VoiceLoudnessReactor2 : MonoBehaviour, ITickSystemTick, IDynamicFloat
{
	// Token: 0x170003DE RID: 990
	// (get) Token: 0x060024EE RID: 9454 RVA: 0x000C6355 File Offset: 0x000C4555
	private float Loudness
	{
		get
		{
			return this.gsl.Loudness * this.sensitivity;
		}
	}

	// Token: 0x170003DF RID: 991
	// (get) Token: 0x060024EF RID: 9455 RVA: 0x000C6369 File Offset: 0x000C4569
	float IDynamicFloat.floatValue
	{
		get
		{
			return this.Loudness;
		}
	}

	// Token: 0x060024F0 RID: 9456 RVA: 0x000C6374 File Offset: 0x000C4574
	private void OnEnable()
	{
		if (this.continuousProperties.Count == 0)
		{
			return;
		}
		if (this.gsl == null)
		{
			this.gsl = base.GetComponentInParent<GorillaSpeakerLoudness>(true);
			if (this.gsl == null)
			{
				GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
				if (componentInParent != null)
				{
					this.gsl = componentInParent.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
					if (this.gsl == null)
					{
						return;
					}
				}
			}
		}
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060024F1 RID: 9457 RVA: 0x0001A29F File Offset: 0x0001849F
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x170003E0 RID: 992
	// (get) Token: 0x060024F2 RID: 9458 RVA: 0x000C63EE File Offset: 0x000C45EE
	// (set) Token: 0x060024F3 RID: 9459 RVA: 0x000C63F6 File Offset: 0x000C45F6
	public bool TickRunning { get; set; }

	// Token: 0x060024F4 RID: 9460 RVA: 0x000C63FF File Offset: 0x000C45FF
	public void Tick()
	{
		this.continuousProperties.ApplyAll(this.Loudness);
	}

	// Token: 0x04003072 RID: 12402
	[Tooltip("Multiply the microphone input by this value. A good default is 15.")]
	public float sensitivity = 15f;

	// Token: 0x04003073 RID: 12403
	public ContinuousPropertyArray continuousProperties;

	// Token: 0x04003074 RID: 12404
	private GorillaSpeakerLoudness gsl;
}
