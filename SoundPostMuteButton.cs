using System;
using UnityEngine;

// Token: 0x02000A48 RID: 2632
public class SoundPostMuteButton : GorillaPressableButton
{
	// Token: 0x06004396 RID: 17302 RVA: 0x00167D60 File Offset: 0x00165F60
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (!this.IsDummyButton)
		{
			SynchedMusicController[] array = this.musicControllers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MuteAudio(this);
			}
			return;
		}
		if (this._targetMuteButton != null)
		{
			this._targetMuteButton.ButtonActivation();
		}
	}

	// Token: 0x04005591 RID: 21905
	public SynchedMusicController[] musicControllers;

	// Token: 0x04005592 RID: 21906
	[Tooltip("If true, then this button will passthrough clicks to a connected SoundPostMuteButton.")]
	public bool IsDummyButton;

	// Token: 0x04005593 RID: 21907
	[SerializeField]
	[Tooltip("The targetted SoundPostMuteButton if this is a dummy button.")]
	private SoundPostMuteButton _targetMuteButton;
}
