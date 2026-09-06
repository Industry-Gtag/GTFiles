using System;
using System.Collections.Generic;
using CjLib;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000231 RID: 561
public class MoonController : MonoBehaviour
{
	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000ED9 RID: 3801 RVA: 0x00051313 File Offset: 0x0004F513
	public float Distance
	{
		get
		{
			return this.distance;
		}
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000EDA RID: 3802 RVA: 0x0005131B File Offset: 0x0004F51B
	private float TimeOfDay
	{
		get
		{
			if (this.debugOverrideTimeOfDay)
			{
				return Mathf.Repeat(this.timeOfDayOverride, 1f);
			}
			if (!(BetterDayNightManager.instance != null))
			{
				return 1f;
			}
			return BetterDayNightManager.instance.NormalizedTimeOfDay;
		}
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x00051357 File Offset: 0x0004F557
	public void SetEyeOpenAnimation()
	{
		this.openMoonAnimator.SetBool(this.eyeOpenHash, true);
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x0005136B File Offset: 0x0004F56B
	public void StartEyeCloseAnimation()
	{
		this.openMoonAnimator.SetBool(this.eyeOpenHash, false);
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x00051380 File Offset: 0x0004F580
	private void Start()
	{
		this.eyeOpenHash = Animator.StringToHash("EyeOpen");
		this.zoneToSceneMapping.Add(GTZone.forest, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.city, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.basement, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.canyon, MoonController.Scenes.Canyon);
		this.zoneToSceneMapping.Add(GTZone.beach, MoonController.Scenes.Beach);
		this.zoneToSceneMapping.Add(GTZone.mountain, MoonController.Scenes.Mountain);
		this.zoneToSceneMapping.Add(GTZone.skyJungle, MoonController.Scenes.Clouds);
		this.zoneToSceneMapping.Add(GTZone.cave, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.cityWithSkyJungle, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.tutorial, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.rotating, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.none, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.Metropolis, MoonController.Scenes.Metropolis);
		this.zoneToSceneMapping.Add(GTZone.cityNoBuildings, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.attic, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.arcade, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.bayou, MoonController.Scenes.Bayou);
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.RegisterMoon(this);
		}
		this.crackStartDayOfYear = new DateTime(2024, 10, 4).DayOfYear;
		this.crackEndDayOfYear = new DateTime(2024, 10, 25).DayOfYear;
		if (this.crackRenderer != null)
		{
			this.currentlySetCrackProgress = 1f;
			this.crackMaterialPropertyBlock = new MaterialPropertyBlock();
			this.crackRenderer.GetPropertyBlock(this.crackMaterialPropertyBlock);
			this.crackMaterialPropertyBlock.SetFloat(ShaderProps._Progress, this.currentlySetCrackProgress);
			this.crackRenderer.SetPropertyBlock(this.crackMaterialPropertyBlock);
		}
		this.orbitAngle = 0f;
		this.UpdateCrack();
		this.UpdatePlacement();
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x0005157B File Offset: 0x0004F77B
	private void OnDestroy()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.UnregisterMoon(this);
		}
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x0005159C File Offset: 0x0004F79C
	private void OnZoneChanged()
	{
		ZoneManagement instance = ZoneManagement.instance;
		MoonController.Scenes scenes = MoonController.Scenes.Forest;
		for (int i = 0; i < instance.activeZones.Count; i++)
		{
			MoonController.Scenes scenes2;
			if (this.zoneToSceneMapping.TryGetValue(instance.activeZones[i], out scenes2) && scenes2 > scenes)
			{
				scenes = scenes2;
			}
		}
		this.UpdateActiveScene(scenes);
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x000515EF File Offset: 0x0004F7EF
	private void UpdateActiveScene(MoonController.Scenes nextScene)
	{
		this.activeScene = nextScene;
		this.UpdateCrack();
		this.UpdatePlacement();
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x00051604 File Offset: 0x0004F804
	private void Update()
	{
		this.UpdateCrack();
		if (!this.alwaysInTheSky)
		{
			float timeOfDay = this.TimeOfDay;
			bool flag = timeOfDay > 0.53999996f && timeOfDay < 0.6733333f;
			bool flag2 = timeOfDay > 0.086666666f && timeOfDay < 0.22f;
			bool flag3 = timeOfDay <= 0.086666666f || timeOfDay >= 0.6733333f;
			if (timeOfDay >= 0.22f)
			{
				bool flag4 = timeOfDay <= 0.53999996f;
			}
			float num = this.orbitAngle;
			if (flag)
			{
				num = Mathf.Lerp(3.1415927f, 0f, (timeOfDay - 0.53999996f) / 0.13333333f);
			}
			else if (flag2)
			{
				num = Mathf.Lerp(0f, -3.1415927f, (timeOfDay - 0.086666666f) / 0.13333333f);
			}
			else if (flag3)
			{
				num = 0f;
			}
			else
			{
				num = 3.1415927f;
			}
			if (this.orbitAngle != num)
			{
				this.orbitAngle = num;
				this.UpdateCrack();
				this.UpdatePlacement();
			}
		}
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x000516F5 File Offset: 0x0004F8F5
	public void UpdateDistance(float nextDistance)
	{
		this.distance = nextDistance;
		this.UpdateVisualState();
		this.UpdatePlacement();
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x0005170C File Offset: 0x0004F90C
	public void UpdateVisualState()
	{
		bool flag = false;
		if (GreyZoneManager.Instance != null)
		{
			flag = GreyZoneManager.Instance.GreyZoneActive;
		}
		if (flag && this.openEyeModelEnabled && this.distance < this.eyeOpenDistThreshold && !this.openMoonAnimator.GetBool(this.eyeOpenHash))
		{
			this.openMoonAnimator.SetBool(this.eyeOpenHash, true);
			return;
		}
		if (!flag && this.distance > this.eyeCloseDistThreshold && this.openMoonAnimator.GetBool(this.eyeOpenHash))
		{
			this.openMoonAnimator.SetBool(this.eyeOpenHash, false);
		}
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x000517AC File Offset: 0x0004F9AC
	public void UpdatePlacement()
	{
		if (this.alwaysInTheSky)
		{
			this.UpdatePlacementSimple();
			return;
		}
		this.UpdatePlacementOrbit();
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x000517C4 File Offset: 0x0004F9C4
	private void UpdatePlacementSimple()
	{
		MoonController.SceneData sceneData = this.scenes[(int)this.activeScene];
		Transform referencePoint = sceneData.referencePoint;
		MoonController.Placement placement = (sceneData.overridePlacement ? sceneData.PlacementOverride : this.defaultPlacement);
		float num = Mathf.Lerp(placement.heightRange.x, placement.heightRange.y, this.distance);
		float num2 = Mathf.Lerp(placement.radiusRange.x, placement.radiusRange.y, this.distance);
		float num3 = Mathf.Lerp(placement.scaleRange.x, placement.scaleRange.y, this.distance);
		float restAngle = placement.restAngle;
		Vector3 position = referencePoint.position;
		position.y += num;
		position.x += num2 * Mathf.Cos(restAngle * 0.017453292f);
		position.z += num2 * Mathf.Sin(restAngle * 0.017453292f);
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(referencePoint.position - base.transform.position);
		base.transform.localScale = Vector3.one * num3;
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x00051908 File Offset: 0x0004FB08
	public void UpdatePlacementOrbit()
	{
		MoonController.SceneData sceneData = this.scenes[(int)this.activeScene];
		Transform referencePoint = sceneData.referencePoint;
		MoonController.Placement placement = (sceneData.overridePlacement ? sceneData.PlacementOverride : this.defaultPlacement);
		float y = placement.heightRange.y;
		float y2 = placement.radiusRange.y;
		Vector3 position = referencePoint.position;
		position.y += y;
		position.x += y2 * Mathf.Cos(placement.restAngle * 0.017453292f);
		position.z += y2 * Mathf.Sin(placement.restAngle * 0.017453292f);
		float num = Mathf.Sqrt(y * y + y2 * y2);
		float num2 = Mathf.Atan2(y, y2);
		Quaternion quaternion = Quaternion.AngleAxis(57.29578f * num2, Vector3.Cross(position - referencePoint.position, Vector3.up));
		float num3 = placement.restAngle * 0.017453292f + this.orbitAngle;
		Vector3 vector = referencePoint.position + quaternion * new Vector3(Mathf.Cos(num3), 0f, Mathf.Sin(num3)) * num;
		if (this.distance < 1f)
		{
			Vector3 position2 = referencePoint.position;
			position2.y += placement.heightRange.x;
			position2.x += placement.radiusRange.x * Mathf.Cos(placement.restAngle * 0.017453292f);
			position2.z += placement.radiusRange.x * Mathf.Sin(placement.restAngle * 0.017453292f);
			if (Mathf.Abs(this.orbitAngle) < 0.9424779f)
			{
				vector = Vector3.Lerp(position2, vector, this.distance);
			}
			else
			{
				vector = Vector3.Lerp(position2, position, this.distance);
			}
		}
		base.transform.position = vector;
		base.transform.rotation = Quaternion.LookRotation(referencePoint.position - base.transform.position);
		base.transform.localScale = Vector3.one * Mathf.Lerp(placement.scaleRange.x, placement.scaleRange.y, this.distance);
		if (this.debugDrawOrbit)
		{
			int num4 = 32;
			float timeOfDay = this.TimeOfDay;
			float num5 = 0.086666666f;
			float num6 = 0.24666667f;
			float num7 = 0.6333333f;
			float num8 = 0.76f;
			bool flag = timeOfDay > num5 && timeOfDay < num6;
			bool flag2 = timeOfDay > num7 && timeOfDay < num8;
			bool flag3 = timeOfDay <= num5 || timeOfDay >= num8;
			if (timeOfDay >= num6)
			{
				bool flag4 = timeOfDay <= num7;
			}
			Color color = (flag2 ? Color.red : (flag3 ? Color.green : (flag ? Color.yellow : Color.blue)));
			Vector3 vector2 = referencePoint.position + quaternion * new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * num;
			for (int i = 1; i <= num4; i++)
			{
				float num9 = (float)i / (float)num4;
				Vector3 vector3 = referencePoint.position + quaternion * new Vector3(Mathf.Cos(6.2831855f * num9), 0f, Mathf.Sin(6.2831855f * num9)) * num;
				DebugUtil.DrawLine(vector2, vector3, color, false);
				vector2 = vector3;
			}
		}
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x00051C98 File Offset: 0x0004FE98
	private void UpdateCrack()
	{
		bool flag = GreyZoneManager.Instance != null && GreyZoneManager.Instance.GreyZoneAvailable;
		if (flag && !this.openEyeModelEnabled)
		{
			this.openEyeModelEnabled = true;
			this.defaultMoon.gameObject.SetActive(false);
			this.openMoon.gameObject.SetActive(true);
		}
		else if (!flag && this.openEyeModelEnabled)
		{
			this.openEyeModelEnabled = false;
			this.defaultMoon.gameObject.SetActive(true);
			this.openMoon.gameObject.SetActive(false);
		}
		if (!flag && GorillaComputer.instance != null)
		{
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			if (this.debugOverrideCrackDayInOctober)
			{
				serverTime = new DateTime(2024, 10, Mathf.Clamp(this.crackDayInOctoberOverride, 1, 31));
			}
			float num = Mathf.InverseLerp((float)this.crackStartDayOfYear, (float)this.crackEndDayOfYear, (float)serverTime.DayOfYear);
			if (this.debugOverrideCrackProgress)
			{
				num = this.crackProgress;
			}
			float num2 = 1f - Mathf.Clamp01(num);
			if (this.crackRenderer != null && Mathf.Abs(num2 - this.currentlySetCrackProgress) > Mathf.Epsilon)
			{
				this.currentlySetCrackProgress = num2;
				this.crackMaterialPropertyBlock.SetFloat(ShaderProps._Progress, this.currentlySetCrackProgress);
				this.crackRenderer.SetPropertyBlock(this.crackMaterialPropertyBlock);
			}
		}
	}

	// Token: 0x040011E5 RID: 4581
	[SerializeField]
	private List<MoonController.SceneData> scenes = new List<MoonController.SceneData>();

	// Token: 0x040011E6 RID: 4582
	[SerializeField]
	private MoonController.Scenes activeScene;

	// Token: 0x040011E7 RID: 4583
	[SerializeField]
	private MoonController.Placement defaultPlacement;

	// Token: 0x040011E8 RID: 4584
	[SerializeField]
	[Range(0f, 1f)]
	private float distance;

	// Token: 0x040011E9 RID: 4585
	[SerializeField]
	private bool alwaysInTheSky;

	// Token: 0x040011EA RID: 4586
	[Header("Model Swap")]
	[SerializeField]
	private Transform defaultMoon;

	// Token: 0x040011EB RID: 4587
	[SerializeField]
	private Transform openMoon;

	// Token: 0x040011EC RID: 4588
	[Header("Animation")]
	[SerializeField]
	private Animator openMoonAnimator;

	// Token: 0x040011ED RID: 4589
	[SerializeField]
	private float eyeOpenDistThreshold = 0.9f;

	// Token: 0x040011EE RID: 4590
	[SerializeField]
	private float eyeCloseDistThreshold = 0.05f;

	// Token: 0x040011EF RID: 4591
	[Header("Debug")]
	[SerializeField]
	private bool debugOverrideTimeOfDay;

	// Token: 0x040011F0 RID: 4592
	[SerializeField]
	[Range(0f, 4f)]
	private float timeOfDayOverride;

	// Token: 0x040011F1 RID: 4593
	[SerializeField]
	private bool debugOverrideCrackProgress;

	// Token: 0x040011F2 RID: 4594
	[SerializeField]
	[Range(0f, 1f)]
	private float crackProgress;

	// Token: 0x040011F3 RID: 4595
	[SerializeField]
	private bool debugOverrideCrackDayInOctober;

	// Token: 0x040011F4 RID: 4596
	[SerializeField]
	[Range(1f, 31f)]
	private int crackDayInOctoberOverride = 4;

	// Token: 0x040011F5 RID: 4597
	[SerializeField]
	private MeshRenderer crackRenderer;

	// Token: 0x040011F6 RID: 4598
	private int crackStartDayOfYear;

	// Token: 0x040011F7 RID: 4599
	private int crackEndDayOfYear;

	// Token: 0x040011F8 RID: 4600
	private float orbitAngle;

	// Token: 0x040011F9 RID: 4601
	private int eyeOpenHash;

	// Token: 0x040011FA RID: 4602
	private bool openEyeModelEnabled;

	// Token: 0x040011FB RID: 4603
	private float currentlySetCrackProgress;

	// Token: 0x040011FC RID: 4604
	private MaterialPropertyBlock crackMaterialPropertyBlock;

	// Token: 0x040011FD RID: 4605
	private bool debugDrawOrbit;

	// Token: 0x040011FE RID: 4606
	private Dictionary<GTZone, MoonController.Scenes> zoneToSceneMapping = new Dictionary<GTZone, MoonController.Scenes>();

	// Token: 0x040011FF RID: 4607
	private const float moonFallStart = 0.086666666f;

	// Token: 0x04001200 RID: 4608
	private const float moonFallEnd = 0.22f;

	// Token: 0x04001201 RID: 4609
	private const float moonRiseStart = 0.53999996f;

	// Token: 0x04001202 RID: 4610
	private const float moonRiseEnd = 0.6733333f;

	// Token: 0x02000232 RID: 562
	public enum Scenes
	{
		// Token: 0x04001204 RID: 4612
		Forest,
		// Token: 0x04001205 RID: 4613
		Bayou,
		// Token: 0x04001206 RID: 4614
		Beach,
		// Token: 0x04001207 RID: 4615
		Canyon,
		// Token: 0x04001208 RID: 4616
		Clouds,
		// Token: 0x04001209 RID: 4617
		City,
		// Token: 0x0400120A RID: 4618
		Metropolis,
		// Token: 0x0400120B RID: 4619
		Mountain
	}

	// Token: 0x02000233 RID: 563
	[Serializable]
	public struct SceneData
	{
		// Token: 0x0400120C RID: 4620
		public MoonController.Scenes scene;

		// Token: 0x0400120D RID: 4621
		public Transform referencePoint;

		// Token: 0x0400120E RID: 4622
		public bool overridePlacement;

		// Token: 0x0400120F RID: 4623
		public MoonController.Placement PlacementOverride;
	}

	// Token: 0x02000234 RID: 564
	[Serializable]
	public struct Placement
	{
		// Token: 0x04001210 RID: 4624
		public Vector2 radiusRange;

		// Token: 0x04001211 RID: 4625
		public Vector2 heightRange;

		// Token: 0x04001212 RID: 4626
		public Vector2 scaleRange;

		// Token: 0x04001213 RID: 4627
		public float restAngle;
	}
}
