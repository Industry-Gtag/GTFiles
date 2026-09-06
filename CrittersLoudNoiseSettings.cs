using System;

// Token: 0x02000066 RID: 102
public class CrittersLoudNoiseSettings : CrittersActorSettings
{
	// Token: 0x06000202 RID: 514 RVA: 0x0000BC44 File Offset: 0x00009E44
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)this.parentActor;
		crittersLoudNoise.soundVolume = this._soundVolume;
		crittersLoudNoise.soundDuration = this._soundDuration;
		crittersLoudNoise.soundEnabled = this._soundEnabled;
		crittersLoudNoise.disableWhenSoundDisabled = this._disableWhenSoundDisabled;
		crittersLoudNoise.volumeFearAttractionMultiplier = this._volumeFearAttractionMultiplier;
	}

	// Token: 0x0400023E RID: 574
	public float _soundVolume;

	// Token: 0x0400023F RID: 575
	public float _soundDuration;

	// Token: 0x04000240 RID: 576
	public bool _soundEnabled;

	// Token: 0x04000241 RID: 577
	public bool _disableWhenSoundDisabled;

	// Token: 0x04000242 RID: 578
	public float _volumeFearAttractionMultiplier = 1f;
}
