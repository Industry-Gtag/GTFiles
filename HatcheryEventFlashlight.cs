using System;
using UnityEngine;

// Token: 0x02000289 RID: 649
public class HatcheryEventFlashlight : MonoBehaviourTick, IGorillaSliceableSimple
{
	// Token: 0x06001177 RID: 4471 RVA: 0x0005DE58 File Offset: 0x0005C058
	private void Awake()
	{
		this.parentRig = base.GetComponentInParent<VRRig>();
		this.playerLight = this.parentRig.isOfflineVRRig;
		this.currentEnergy = 10f;
		this.lightComponents = new Light[this.lights.Length];
		this.gameLightComponents = new GameLight[this.lights.Length];
		for (int i = 0; i < this.lights.Length; i++)
		{
			this.lightComponents[i] = this.lights[i].GetComponent<Light>();
			this.gameLightComponents[i] = this.lights[i].GetComponent<GameLight>();
		}
		this.startingBrightness = this.lightComponents[0].intensity;
		this.lightsParent.gameObject.SetActive(false);
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x0005DF14 File Offset: 0x0005C114
	private new void OnEnable()
	{
		base.OnEnable();
		if (!this.playerLight)
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x0005DF2A File Offset: 0x0005C12A
	private new void OnDisable()
	{
		base.OnDisable();
		if (!this.playerLight)
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
		}
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x0005DF41 File Offset: 0x0005C141
	private float MaxEnergy()
	{
		if (NetworkSystem.Instance.CurrentRoom == null)
		{
			return 10f;
		}
		return 10f * (1f / Mathf.Log((float)NetworkSystem.Instance.RoomPlayerCount + 1.72f));
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x0005DF77 File Offset: 0x0005C177
	public override void Tick()
	{
		if (this.playerLight)
		{
			this.SliceUpdate();
		}
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x0005DF88 File Offset: 0x0005C188
	public void SliceUpdate()
	{
		if (GameLightingManager.instance.IsDynamicLightingEnabled != this.flashlight.gameObject.activeSelf)
		{
			this.flashlight.gameObject.SetActive(GameLightingManager.instance.IsDynamicLightingEnabled);
		}
		if (!GameLightingManager.instance.IsDynamicLightingEnabled)
		{
			return;
		}
		float time = Time.time;
		float num = this.MaxEnergy();
		if (this.wasLightEnabled)
		{
			this.currentEnergy -= (time - this.lastUpdated) * 1f;
		}
		else
		{
			this.currentEnergy += (time - this.lastUpdated) * 0.66f;
		}
		this.currentEnergy = Mathf.Clamp(this.currentEnergy, 0f, this.MaxEnergy());
		bool flag = this.parentRig.rightIndex.calcT >= 0.33f;
		bool flag2 = flag && (!this.wasLightSwitchedOn || this.wasLightEnabled) && this.currentEnergy > 0f;
		if (flag2 != this.wasLightEnabled)
		{
			this.lightsParent.gameObject.SetActive(flag2);
			this.clickSource.Play();
		}
		if (flag2)
		{
			this.UpdateLightPositioning();
			this.UpdateLightBrightness(num);
		}
		this.lastUpdated = Time.time;
		this.wasLightSwitchedOn = flag;
		this.wasLightEnabled = flag2;
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x0005E0D4 File Offset: 0x0005C2D4
	private void UpdateLightPositioning()
	{
		int num = Physics.RaycastNonAlloc(this.lightStart.position, this.lightStart.forward, this.hits, 6f, -1, QueryTriggerInteraction.Ignore);
		float num2 = 6f;
		for (int i = 0; i < num; i++)
		{
			if (this.hits[i].distance <= num2)
			{
				num2 = this.hits[i].distance;
			}
		}
		float num3 = ((num2 >= 2f) ? (num2 - 1f) : (num2 / 2f));
		for (int j = 0; j < this.lights.Length; j++)
		{
			this.lights[j].position = this.lightStart.position + this.lightStart.forward * (num3 * (float)(j + 1) / (float)this.lights.Length);
		}
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0005E1B4 File Offset: 0x0005C3B4
	private void UpdateLightBrightness(float _maxEnergy)
	{
		float num = this.startingBrightness / 5f * (1f + 4f * this.currentEnergy / _maxEnergy);
		for (int i = 0; i < this.lightComponents.Length; i++)
		{
			this.lightComponents[i].intensity = num;
			this.gameLightComponents[i].UpdateCachedLightColorAndIntensity();
		}
	}

	// Token: 0x040014CD RID: 5325
	private const float lightMaxDistance = 6f;

	// Token: 0x040014CE RID: 5326
	private const float surfaceOffset = 1f;

	// Token: 0x040014CF RID: 5327
	private const float enableThresholdCurl = 0.33f;

	// Token: 0x040014D0 RID: 5328
	private const float maxEnergy = 10f;

	// Token: 0x040014D1 RID: 5329
	private const float energyUsageRate = 1f;

	// Token: 0x040014D2 RID: 5330
	private const float energyChargeRate = 0.66f;

	// Token: 0x040014D3 RID: 5331
	private RaycastHit[] hits = new RaycastHit[20];

	// Token: 0x040014D4 RID: 5332
	private Light[] lightComponents;

	// Token: 0x040014D5 RID: 5333
	private GameLight[] gameLightComponents;

	// Token: 0x040014D6 RID: 5334
	private VRRig parentRig;

	// Token: 0x040014D7 RID: 5335
	private float currentEnergy;

	// Token: 0x040014D8 RID: 5336
	private float startingBrightness;

	// Token: 0x040014D9 RID: 5337
	private float lastUpdated;

	// Token: 0x040014DA RID: 5338
	private bool playerLight;

	// Token: 0x040014DB RID: 5339
	private bool wasLightEnabled;

	// Token: 0x040014DC RID: 5340
	private bool wasLightSwitchedOn;

	// Token: 0x040014DD RID: 5341
	public Transform lightStart;

	// Token: 0x040014DE RID: 5342
	public Transform lightsParent;

	// Token: 0x040014DF RID: 5343
	public Transform flashlight;

	// Token: 0x040014E0 RID: 5344
	public Transform[] lights;

	// Token: 0x040014E1 RID: 5345
	public AudioSource clickSource;
}
