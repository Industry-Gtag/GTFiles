using System;

// Token: 0x02000894 RID: 2196
public class GorillaModifyDayNightTrigger : GorillaTriggerBox
{
	// Token: 0x06003944 RID: 14660 RVA: 0x001383EC File Offset: 0x001365EC
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (this.clearModifiedTime)
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}
		else
		{
			int num = this.timeOfDayIndex % BetterDayNightManager.instance.timeOfDayRange.Length;
			BetterDayNightManager.instance.SetTimeOfDay(num, false);
			BetterDayNightManager.instance.SetOverrideIndex(num);
		}
		if (this.setFixedWeather)
		{
			BetterDayNightManager.instance.SetFixedWeather(this.fixedWeather, false);
			return;
		}
		BetterDayNightManager.instance.ClearFixedWeather(false);
	}

	// Token: 0x04004958 RID: 18776
	public bool clearModifiedTime;

	// Token: 0x04004959 RID: 18777
	public int timeOfDayIndex;

	// Token: 0x0400495A RID: 18778
	public bool setFixedWeather;

	// Token: 0x0400495B RID: 18779
	public BetterDayNightManager.WeatherType fixedWeather;
}
