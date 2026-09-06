using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200092C RID: 2348
public class SizeManager : MonoBehaviour
{
	// Token: 0x170005AA RID: 1450
	// (get) Token: 0x06003D80 RID: 15744 RVA: 0x0014DACC File Offset: 0x0014BCCC
	public float currentScale
	{
		get
		{
			if (this.targetRig != null)
			{
				return this.targetRig.ScaleMultiplier;
			}
			if (this.targetPlayer != null)
			{
				return this.targetPlayer.ScaleMultiplier;
			}
			return 1f;
		}
	}

	// Token: 0x170005AB RID: 1451
	// (get) Token: 0x06003D81 RID: 15745 RVA: 0x0014DB07 File Offset: 0x0014BD07
	// (set) Token: 0x06003D82 RID: 15746 RVA: 0x0014DB3C File Offset: 0x0014BD3C
	public int currentSizeLayerMaskValue
	{
		get
		{
			if (this.targetPlayer)
			{
				return this.targetPlayer.sizeLayerMask;
			}
			if (this.targetRig)
			{
				return this.targetRig.SizeLayerMask;
			}
			return 1;
		}
		set
		{
			if (this.targetPlayer)
			{
				this.targetPlayer.sizeLayerMask = value;
				if (this.targetRig != null)
				{
					this.targetRig.SizeLayerMask = value;
					return;
				}
			}
			else if (this.targetRig)
			{
				this.targetRig.SizeLayerMask = value;
			}
		}
	}

	// Token: 0x06003D83 RID: 15747 RVA: 0x0014DB96 File Offset: 0x0014BD96
	private void OnDisable()
	{
		this.touchingChangers.Clear();
		this.currentSizeLayerMaskValue = 1;
		SizeManagerManager.UnregisterSM(this);
	}

	// Token: 0x06003D84 RID: 15748 RVA: 0x0014DBB0 File Offset: 0x0014BDB0
	private void OnEnable()
	{
		SizeManagerManager.RegisterSM(this);
	}

	// Token: 0x06003D85 RID: 15749 RVA: 0x0014DBB8 File Offset: 0x0014BDB8
	private void CollectLineRenderers(GameObject obj)
	{
		this.lineRenderers = obj.GetComponentsInChildren<LineRenderer>(true);
		int num = this.lineRenderers.Length;
		foreach (LineRenderer lineRenderer in this.lineRenderers)
		{
			this.initLineScalar.Add(lineRenderer.widthMultiplier);
		}
	}

	// Token: 0x06003D86 RID: 15750 RVA: 0x0014DC08 File Offset: 0x0014BE08
	public void BuildInitialize()
	{
		this.rate = 650f;
		if (this.targetRig != null)
		{
			this.CollectLineRenderers(this.targetRig.gameObject);
		}
		else if (this.targetPlayer != null)
		{
			this.CollectLineRenderers(GorillaTagger.Instance.offlineVRRig.gameObject);
		}
		this.mainCameraTransform = Camera.main.transform;
		if (this.targetPlayer != null)
		{
			this.myType = SizeManager.SizeChangerType.LocalOffline;
		}
		else if (this.targetRig != null && !this.targetRig.isOfflineVRRig && this.targetRig.netView != null && this.targetRig.netView.Owner != NetworkSystem.Instance.LocalPlayer)
		{
			this.myType = SizeManager.SizeChangerType.OtherOnline;
		}
		else
		{
			this.myType = SizeManager.SizeChangerType.LocalOnline;
		}
		this.buildInitialized = true;
	}

	// Token: 0x06003D87 RID: 15751 RVA: 0x0014DCEC File Offset: 0x0014BEEC
	private void Awake()
	{
		if (!this.buildInitialized)
		{
			this.BuildInitialize();
		}
		SizeManagerManager.RegisterSM(this);
	}

	// Token: 0x06003D88 RID: 15752 RVA: 0x0014DD04 File Offset: 0x0014BF04
	public void InvokeFixedUpdate()
	{
		float num = 1f;
		CustomGameMode customGameMode = GorillaGameManager.instance as CustomGameMode;
		if (customGameMode != null)
		{
			num = customGameMode.GetRigScale(this.targetRig);
		}
		SizeChanger sizeChanger = this.ControllingChanger(this.targetRig.transform);
		switch (this.myType)
		{
		case SizeManager.SizeChangerType.LocalOffline:
			num *= this.ScaleFromChanger(sizeChanger, this.mainCameraTransform, Time.fixedDeltaTime);
			this.targetPlayer.SetScaleMultiplier((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		case SizeManager.SizeChangerType.LocalOnline:
			num *= this.ScaleFromChanger(sizeChanger, this.targetRig.transform, Time.fixedDeltaTime);
			this.targetRig.ScaleMultiplier = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		case SizeManager.SizeChangerType.OtherOnline:
			num *= this.ScaleFromChanger(sizeChanger, this.targetRig.transform, Time.fixedDeltaTime);
			this.targetRig.ScaleMultiplier = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		}
		if (num != this.lastScale)
		{
			for (int i = 0; i < this.lineRenderers.Length; i++)
			{
				this.lineRenderers[i].widthMultiplier = num * this.initLineScalar[i];
			}
			Vector3 vector;
			if (sizeChanger != null && sizeChanger.TryGetScaleCenterPoint(out vector))
			{
				if (this.myType == SizeManager.SizeChangerType.LocalOffline)
				{
					this.targetPlayer.ScaleAwayFromPoint(this.lastScale, num, vector);
				}
				else if (this.myType == SizeManager.SizeChangerType.LocalOnline)
				{
					GTPlayer.Instance.ScaleAwayFromPoint(this.lastScale, num, vector);
				}
			}
			if (this.myType == SizeManager.SizeChangerType.LocalOffline)
			{
				this.CheckSizeChangeEvents(num);
			}
		}
		this.lastScale = num;
	}

	// Token: 0x06003D89 RID: 15753 RVA: 0x0014DEC8 File Offset: 0x0014C0C8
	private SizeChanger ControllingChanger(Transform t)
	{
		for (int i = this.touchingChangers.Count - 1; i >= 0; i--)
		{
			SizeChanger sizeChanger = this.touchingChangers[i];
			if (!(sizeChanger == null) && sizeChanger.gameObject.activeInHierarchy && (sizeChanger.SizeLayerMask & this.currentSizeLayerMaskValue) != 0 && (sizeChanger.alwaysControlWhenEntered || (sizeChanger.ClosestPoint(t.position) - t.position).magnitude < this.magnitudeThreshold))
			{
				return sizeChanger;
			}
		}
		return null;
	}

	// Token: 0x06003D8A RID: 15754 RVA: 0x0014DF54 File Offset: 0x0014C154
	private float ScaleFromChanger(SizeChanger sC, Transform t, float deltaTime)
	{
		if (sC == null)
		{
			return 1f;
		}
		switch (sC.MyType)
		{
		case SizeChanger.ChangerType.Static:
			return this.SizeOverTime(sC.MinScale, sC.StaticEasing, deltaTime);
		case SizeChanger.ChangerType.Continuous:
		{
			Vector3 vector = Vector3.Project(t.position - sC.StartPos.position, sC.EndPos.position - sC.StartPos.position);
			return Mathf.Clamp(sC.MaxScale - vector.magnitude / (sC.StartPos.position - sC.EndPos.position).magnitude * (sC.MaxScale - sC.MinScale), sC.MinScale, sC.MaxScale);
		}
		case SizeChanger.ChangerType.Radius:
		{
			float num = Vector3.Distance(t.position, sC.StartPos.position);
			float num2 = Mathf.InverseLerp(sC.startRadius, sC.endRadius, num);
			return Mathf.Lerp(sC.MinScale, sC.MaxScale, num2);
		}
		default:
			return 1f;
		}
	}

	// Token: 0x06003D8B RID: 15755 RVA: 0x0014E06E File Offset: 0x0014C26E
	private float SizeOverTime(float targetSize, float easing, float deltaTime)
	{
		if (easing <= 0f || Mathf.Abs(this.targetRig.ScaleMultiplier - targetSize) < 0.05f)
		{
			return targetSize;
		}
		return Mathf.MoveTowards(this.targetRig.ScaleMultiplier, targetSize, deltaTime / easing);
	}

	// Token: 0x06003D8C RID: 15756 RVA: 0x0014E0A8 File Offset: 0x0014C2A8
	private void CheckSizeChangeEvents(float newSize)
	{
		if (newSize < this.smallThreshold)
		{
			if (!this.isSmall)
			{
				this.isSmall = true;
				this.isLarge = false;
				PlayerGameEvents.MiscEvent("SizeSmall", 1);
				return;
			}
		}
		else if (newSize > this.largeThreshold)
		{
			if (!this.isLarge)
			{
				this.isLarge = true;
				this.isSmall = false;
				PlayerGameEvents.MiscEvent("SizeLarge", 1);
				return;
			}
		}
		else
		{
			this.isLarge = false;
			this.isSmall = false;
		}
	}

	// Token: 0x04004E46 RID: 20038
	public List<SizeChanger> touchingChangers;

	// Token: 0x04004E47 RID: 20039
	private LineRenderer[] lineRenderers;

	// Token: 0x04004E48 RID: 20040
	private List<float> initLineScalar = new List<float>();

	// Token: 0x04004E49 RID: 20041
	public VRRig targetRig;

	// Token: 0x04004E4A RID: 20042
	public GTPlayer targetPlayer;

	// Token: 0x04004E4B RID: 20043
	public float magnitudeThreshold = 0.01f;

	// Token: 0x04004E4C RID: 20044
	public float rate = 650f;

	// Token: 0x04004E4D RID: 20045
	public Transform mainCameraTransform;

	// Token: 0x04004E4E RID: 20046
	public SizeManager.SizeChangerType myType;

	// Token: 0x04004E4F RID: 20047
	public float lastScale;

	// Token: 0x04004E50 RID: 20048
	private bool buildInitialized;

	// Token: 0x04004E51 RID: 20049
	private const float returnToNormalEasing = 0.33f;

	// Token: 0x04004E52 RID: 20050
	private float smallThreshold = 0.6f;

	// Token: 0x04004E53 RID: 20051
	private float largeThreshold = 1.5f;

	// Token: 0x04004E54 RID: 20052
	private bool isSmall;

	// Token: 0x04004E55 RID: 20053
	private bool isLarge;

	// Token: 0x0200092D RID: 2349
	public enum SizeChangerType
	{
		// Token: 0x04004E57 RID: 20055
		LocalOffline,
		// Token: 0x04004E58 RID: 20056
		LocalOnline,
		// Token: 0x04004E59 RID: 20057
		OtherOnline
	}
}
