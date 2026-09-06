using System;
using UnityEngine;

// Token: 0x020006E9 RID: 1769
public class GameLight : MonoBehaviour
{
	// Token: 0x1700045E RID: 1118
	// (get) Token: 0x06002C85 RID: 11397 RVA: 0x000F08A8 File Offset: 0x000EEAA8
	public bool IsRegistered
	{
		get
		{
			return this.lightId != -1;
		}
	}

	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x06002C86 RID: 11398 RVA: 0x000F08B6 File Offset: 0x000EEAB6
	// (set) Token: 0x06002C87 RID: 11399 RVA: 0x000F08BE File Offset: 0x000EEABE
	public float InitialIntensity { get; private set; }

	// Token: 0x06002C88 RID: 11400 RVA: 0x000F08C8 File Offset: 0x000EEAC8
	public void Awake()
	{
		this.intensityMult = 1;
		this.lightId = -1;
		this.light.range = Mathf.Max(this.light.range, 0.01f);
		if (!this.applyRange)
		{
			this.range = 0.005f;
		}
	}

	// Token: 0x06002C89 RID: 11401 RVA: 0x000F0916 File Offset: 0x000EEB16
	protected void OnEnable()
	{
		if (this.initialized)
		{
			this.lightId = GameLightingManager.instance.AddGameLight(this, false);
		}
	}

	// Token: 0x06002C8A RID: 11402 RVA: 0x000F0934 File Offset: 0x000EEB34
	protected void Start()
	{
		this.lightId = GameLightingManager.instance.AddGameLight(this, false);
		this.initialized = true;
	}

	// Token: 0x06002C8B RID: 11403 RVA: 0x000F0951 File Offset: 0x000EEB51
	protected void OnDisable()
	{
		if (this.initialized)
		{
			GameLightingManager.instance.RemoveGameLight(this);
		}
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x000F0968 File Offset: 0x000EEB68
	public void UpdateCachedLightColorAndIntensity()
	{
		this.cachedColorAndIntensity = (float)this.intensityMult * this.light.intensity * (this.negativeLight ? (-1f) : 1f) * this.light.color;
		if (this.applyRange && this.light.range > 0f)
		{
			this.range = 0.005f / this.light.range;
		}
	}

	// Token: 0x04003902 RID: 14594
	public const float DEFAULT_RANGE = 0.005f;

	// Token: 0x04003903 RID: 14595
	public Light light;

	// Token: 0x04003904 RID: 14596
	public bool negativeLight;

	// Token: 0x04003905 RID: 14597
	public bool isHighPriorityPlayerLight;

	// Token: 0x04003906 RID: 14598
	public bool applyRange;

	// Token: 0x04003907 RID: 14599
	public Vector3 cachedPosition;

	// Token: 0x04003908 RID: 14600
	public Vector4 cachedColorAndIntensity;

	// Token: 0x04003909 RID: 14601
	public float range = 0.005f;

	// Token: 0x0400390A RID: 14602
	public int lightId = -1;

	// Token: 0x0400390B RID: 14603
	public int intensityMult = 1;

	// Token: 0x0400390C RID: 14604
	private bool initialized;
}
