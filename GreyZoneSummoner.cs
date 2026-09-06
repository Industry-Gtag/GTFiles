using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000238 RID: 568
public class GreyZoneSummoner : MonoBehaviour
{
	// Token: 0x1700017B RID: 379
	// (get) Token: 0x06000F1E RID: 3870 RVA: 0x00052E88 File Offset: 0x00051088
	public Vector3 SummoningFocusPoint
	{
		get
		{
			return this.summoningFocusPoint.position;
		}
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x06000F1F RID: 3871 RVA: 0x00052E95 File Offset: 0x00051095
	public float SummonerMaxDistance
	{
		get
		{
			return this.areaTriggerCollider.radius + 1f;
		}
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x00052EA8 File Offset: 0x000510A8
	private void OnEnable()
	{
		this.greyZoneManager = GreyZoneManager.Instance;
		if (this.greyZoneManager == null)
		{
			return;
		}
		this.greyZoneManager.RegisterSummoner(this);
		this.areaTriggerNotifier.TriggerEnterEvent += this.ColliderEnteredArea;
		this.areaTriggerNotifier.TriggerExitEvent += this.ColliderExitedArea;
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x00052F0C File Offset: 0x0005110C
	private void OnDisable()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.DeregisterSummoner(this);
		}
		this.areaTriggerNotifier.TriggerEnterEvent -= this.ColliderEnteredArea;
		this.areaTriggerNotifier.TriggerExitEvent -= this.ColliderExitedArea;
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x00052F64 File Offset: 0x00051164
	public void UpdateProgressFeedback(bool greyZoneAvailable)
	{
		if (this.greyZoneManager == null)
		{
			return;
		}
		if (greyZoneAvailable && !this.candlesParent.gameObject.activeSelf)
		{
			this.candlesParent.gameObject.SetActive(true);
		}
		this.candlesTimeline.time = (double)Mathf.Clamp01(this.greyZoneManager.SummoningProgress) * this.candlesTimeline.duration;
		this.candlesTimeline.Evaluate();
		if (!this.greyZoneManager.GreyZoneActive)
		{
			float num = (float)this.summoningTones.Count * this.greyZoneManager.SummoningProgress;
			for (int i = 0; i < this.summoningTones.Count; i++)
			{
				float num2 = Mathf.InverseLerp((float)i, (float)i + 1f + this.summoningTonesFadeOverlap, num);
				this.summoningTones[i].volume = num2 * this.summoningTonesMaxVolume;
			}
		}
		this.greyZoneActivationButton.isOn = this.greyZoneManager.GreyZoneActive;
		this.greyZoneActivationButton.UpdateColor();
		for (int j = 0; j < this.greyZoneGravityFactorButtons.Count; j++)
		{
			this.greyZoneGravityFactorButtons[j].isOn = this.greyZoneManager.GravityFactorSelection == j;
			this.greyZoneGravityFactorButtons[j].UpdateColor();
		}
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x000530AD File Offset: 0x000512AD
	public void OnGreyZoneActivated()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeOutSummoningTones());
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x000530C2 File Offset: 0x000512C2
	private IEnumerator FadeOutSummoningTones()
	{
		float fadeStartTime = Time.time;
		float fadeRate = 1f / this.summoningTonesFadeTime;
		while (Time.time < fadeStartTime + this.summoningTonesFadeTime)
		{
			for (int i = 0; i < this.summoningTones.Count; i++)
			{
				this.summoningTones[i].volume = Mathf.MoveTowards(this.summoningTones[i].volume, 0f, this.summoningTonesMaxVolume * fadeRate * Time.deltaTime);
			}
			yield return null;
		}
		for (int j = 0; j < this.summoningTones.Count; j++)
		{
			this.summoningTones[j].volume = 0f;
		}
		yield break;
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x000530D4 File Offset: 0x000512D4
	public void ColliderEnteredArea(TriggerEventNotifier notifier, Collider other)
	{
		ZoneEntityBSP component = other.GetComponent<ZoneEntityBSP>();
		VRRig vrrig = ((component != null) ? component.entityRig : null);
		if (vrrig != null && this.greyZoneManager != null)
		{
			this.greyZoneManager.VRRigEnteredSummonerProximity(vrrig, this);
		}
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x00053120 File Offset: 0x00051320
	public void ColliderExitedArea(TriggerEventNotifier notifier, Collider other)
	{
		ZoneEntityBSP component = other.GetComponent<ZoneEntityBSP>();
		VRRig vrrig = ((component != null) ? component.entityRig : null);
		if (vrrig != null && this.greyZoneManager != null)
		{
			this.greyZoneManager.VRRigExitedSummonerProximity(vrrig, this);
		}
	}

	// Token: 0x04001245 RID: 4677
	[SerializeField]
	private Transform summoningFocusPoint;

	// Token: 0x04001246 RID: 4678
	[SerializeField]
	private Transform candlesParent;

	// Token: 0x04001247 RID: 4679
	[SerializeField]
	private PlayableDirector candlesTimeline;

	// Token: 0x04001248 RID: 4680
	[SerializeField]
	private TriggerEventNotifier areaTriggerNotifier;

	// Token: 0x04001249 RID: 4681
	[SerializeField]
	private SphereCollider areaTriggerCollider;

	// Token: 0x0400124A RID: 4682
	[SerializeField]
	private GorillaPressableButton greyZoneActivationButton;

	// Token: 0x0400124B RID: 4683
	[SerializeField]
	private List<AudioSource> summoningTones = new List<AudioSource>();

	// Token: 0x0400124C RID: 4684
	[SerializeField]
	private float summoningTonesMaxVolume = 1f;

	// Token: 0x0400124D RID: 4685
	[SerializeField]
	private float summoningTonesFadeOverlap = 0.5f;

	// Token: 0x0400124E RID: 4686
	[SerializeField]
	private float summoningTonesFadeTime = 4f;

	// Token: 0x0400124F RID: 4687
	[SerializeField]
	private List<GorillaPressableButton> greyZoneGravityFactorButtons = new List<GorillaPressableButton>();

	// Token: 0x04001250 RID: 4688
	private GreyZoneManager greyZoneManager;
}
