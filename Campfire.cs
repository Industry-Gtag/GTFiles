using System;
using UnityEngine;

// Token: 0x02000670 RID: 1648
public class Campfire : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002911 RID: 10513 RVA: 0x000DEF7C File Offset: 0x000DD17C
	private void Start()
	{
		this.lastAngleBottom = 0f;
		this.lastAngleMiddle = 0f;
		this.lastAngleTop = 0f;
		this.perlinBottom = (float)Random.Range(0, 100);
		this.perlinMiddle = (float)Random.Range(200, 300);
		this.perlinTop = (float)Random.Range(400, 500);
		this.startingRotationBottom = this.baseFire.localEulerAngles.x;
		this.startingRotationMiddle = this.middleFire.localEulerAngles.x;
		this.startingRotationTop = this.topFire.localEulerAngles.x;
		this.tempVec = new Vector3(0f, 0f, 0f);
		this.mergedBottom = false;
		this.mergedMiddle = false;
		this.mergedTop = false;
		this.wasActive = false;
		this.lastTime = Time.time;
	}

	// Token: 0x06002912 RID: 10514 RVA: 0x0001212B File Offset: 0x0001032B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002913 RID: 10515 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002914 RID: 10516 RVA: 0x000DF068 File Offset: 0x000DD268
	public void SliceUpdate()
	{
		if (BetterDayNightManager.instance == null)
		{
			return;
		}
		if ((this.isActive[BetterDayNightManager.instance.currentTimeIndex] && (this.playDuringRain || BetterDayNightManager.instance.CurrentWeather() != BetterDayNightManager.WeatherType.Raining)) || this.overrideDayNight == 1)
		{
			if (!this.wasActive)
			{
				this.wasActive = true;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, out this.h, out this.s, out this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 1f);
			}
			this.Flap(ref this.perlinBottom, this.perlinStepBottom, ref this.lastAngleBottom, ref this.baseFire, this.bottomRange, this.baseMultiplier, ref this.mergedBottom);
			this.Flap(ref this.perlinMiddle, this.perlinStepMiddle, ref this.lastAngleMiddle, ref this.middleFire, this.middleRange, this.middleMultiplier, ref this.mergedMiddle);
			this.Flap(ref this.perlinTop, this.perlinStepTop, ref this.lastAngleTop, ref this.topFire, this.topRange, this.topMultiplier, ref this.mergedTop);
		}
		else
		{
			if (this.wasActive)
			{
				this.wasActive = false;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, out this.h, out this.s, out this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 0.25f);
			}
			this.ReturnToOff(ref this.baseFire, this.startingRotationBottom, ref this.mergedBottom);
			this.ReturnToOff(ref this.middleFire, this.startingRotationMiddle, ref this.mergedMiddle);
			this.ReturnToOff(ref this.topFire, this.startingRotationTop, ref this.mergedTop);
		}
		this.lastTime = Time.time;
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x000DF274 File Offset: 0x000DD474
	private void Flap(ref float perlinValue, float perlinStep, ref float lastAngle, ref Transform flameTransform, float range, float multiplier, ref bool isMerged)
	{
		perlinValue += perlinStep;
		lastAngle += (Time.time - this.lastTime) * Mathf.PerlinNoise(perlinValue, 0f);
		this.tempVec.x = range * Mathf.Sin(lastAngle * multiplier);
		if (Mathf.Abs(this.tempVec.x - flameTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > flameTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (isMerged)
		{
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		if (Mathf.Abs(flameTransform.localEulerAngles.x - this.tempVec.x) < 1f)
		{
			isMerged = true;
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		this.tempVec.x = (this.tempVec.x - flameTransform.localEulerAngles.x) * this.slerp + flameTransform.localEulerAngles.x;
		flameTransform.localEulerAngles = this.tempVec;
	}

	// Token: 0x06002916 RID: 10518 RVA: 0x000DF3BC File Offset: 0x000DD5BC
	private void ReturnToOff(ref Transform startTransform, float targetAngle, ref bool isMerged)
	{
		this.tempVec.x = targetAngle;
		if (Mathf.Abs(this.tempVec.x - startTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > startTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (!isMerged)
		{
			if (Mathf.Abs(startTransform.localEulerAngles.x - targetAngle) < 1f)
			{
				isMerged = true;
				return;
			}
			this.tempVec.x = (this.tempVec.x - startTransform.localEulerAngles.x) * this.slerp + startTransform.localEulerAngles.x;
			startTransform.localEulerAngles = this.tempVec;
		}
	}

	// Token: 0x0400357D RID: 13693
	public Transform baseFire;

	// Token: 0x0400357E RID: 13694
	public Transform middleFire;

	// Token: 0x0400357F RID: 13695
	public Transform topFire;

	// Token: 0x04003580 RID: 13696
	public float baseMultiplier;

	// Token: 0x04003581 RID: 13697
	public float middleMultiplier;

	// Token: 0x04003582 RID: 13698
	public float topMultiplier;

	// Token: 0x04003583 RID: 13699
	public float bottomRange;

	// Token: 0x04003584 RID: 13700
	public float middleRange;

	// Token: 0x04003585 RID: 13701
	public float topRange;

	// Token: 0x04003586 RID: 13702
	private float lastAngleBottom;

	// Token: 0x04003587 RID: 13703
	private float lastAngleMiddle;

	// Token: 0x04003588 RID: 13704
	private float lastAngleTop;

	// Token: 0x04003589 RID: 13705
	public float perlinStepBottom;

	// Token: 0x0400358A RID: 13706
	public float perlinStepMiddle;

	// Token: 0x0400358B RID: 13707
	public float perlinStepTop;

	// Token: 0x0400358C RID: 13708
	private float perlinBottom;

	// Token: 0x0400358D RID: 13709
	private float perlinMiddle;

	// Token: 0x0400358E RID: 13710
	private float perlinTop;

	// Token: 0x0400358F RID: 13711
	public float startingRotationBottom;

	// Token: 0x04003590 RID: 13712
	public float startingRotationMiddle;

	// Token: 0x04003591 RID: 13713
	public float startingRotationTop;

	// Token: 0x04003592 RID: 13714
	public float slerp = 0.01f;

	// Token: 0x04003593 RID: 13715
	private bool mergedBottom;

	// Token: 0x04003594 RID: 13716
	private bool mergedMiddle;

	// Token: 0x04003595 RID: 13717
	private bool mergedTop;

	// Token: 0x04003596 RID: 13718
	public string lastTimeOfDay;

	// Token: 0x04003597 RID: 13719
	public Material mat;

	// Token: 0x04003598 RID: 13720
	private float h;

	// Token: 0x04003599 RID: 13721
	private float s;

	// Token: 0x0400359A RID: 13722
	private float v;

	// Token: 0x0400359B RID: 13723
	public int overrideDayNight;

	// Token: 0x0400359C RID: 13724
	private Vector3 tempVec;

	// Token: 0x0400359D RID: 13725
	public bool[] isActive;

	// Token: 0x0400359E RID: 13726
	public bool wasActive;

	// Token: 0x0400359F RID: 13727
	private float lastTime;

	// Token: 0x040035A0 RID: 13728
	public bool playDuringRain;
}
