using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class VODTarget : ObservableBehavior, IBuildValidation
{
	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000BD6 RID: 3030 RVA: 0x00040F12 File Offset: 0x0003F112
	public VODTarget.VODTargetAudioSettings AudioSettings
	{
		get
		{
			return this.audioSettings;
		}
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000BD7 RID: 3031 RVA: 0x00040F1A File Offset: 0x0003F11A
	public Renderer Renderer
	{
		get
		{
			return this.targetRenderer;
		}
	}

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000BD8 RID: 3032 RVA: 0x00040F22 File Offset: 0x0003F122
	public Material StandbyOverride
	{
		get
		{
			return this.standbyOverride;
		}
	}

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x00040F2A File Offset: 0x0003F12A
	public VODPlayer.VODStream.VODStreamChannel[] Channel
	{
		get
		{
			if (this.channel.Length != 0)
			{
				return this.channel;
			}
			return new VODPlayer.VODStream.VODStreamChannel[1];
		}
	}

	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000BDA RID: 3034 RVA: 0x00040F42 File Offset: 0x0003F142
	public bool Unmutable
	{
		get
		{
			return this.unmutable;
		}
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x00040F4A File Offset: 0x0003F14A
	public void SetNext(VODPlayer.VODNextStreamData data)
	{
		this.upNextData = data;
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x00040F53 File Offset: 0x0003F153
	public void ClearNext()
	{
		this.upNextData = default(VODPlayer.VODNextStreamData);
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x00040F64 File Offset: 0x0003F164
	public bool VerifyChannel(VODPlayer.VODStream.VODStreamChannel ch)
	{
		if (this.channel.Length == 0 && ch == VODPlayer.VODStream.VODStreamChannel.DEFAULT)
		{
			return true;
		}
		for (int i = 0; i < this.channel.Length; i++)
		{
			if (this.channel[i] == ch)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x00040FA0 File Offset: 0x0003F1A0
	protected override void OnLostObservable()
	{
		if (!this.staticScreen.activeInHierarchy && VODTarget.AlertDisabled != null)
		{
			VODTarget.AlertDisabled(this);
		}
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x00040FC1 File Offset: 0x0003F1C1
	protected override void OnBecameObservable()
	{
		if (!this.staticScreen.activeInHierarchy && VODTarget.AlertEnabled != null)
		{
			VODTarget.AlertEnabled(this);
		}
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x00040FE2 File Offset: 0x0003F1E2
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.targetRenderer == null)
		{
			Debug.LogError("VODTarget " + base.name + " must set a Target Renderer");
			return false;
		}
		return true;
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x0004100F File Offset: 0x0003F20F
	private void Start()
	{
		this.targetRenderer.material = ((this.standbyOverride == null) ? VODPlayer.StandbyMaterial : this.standbyOverride);
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x00041037 File Offset: 0x0003F237
	protected override void UnityOnEnable()
	{
		VODPlayer.OnCrash = (Action)Delegate.Combine(VODPlayer.OnCrash, new Action(this.VODPlayer_OnCrash));
		if (VODPlayer.state == VODPlayer.State.CRASHED)
		{
			this.staticScreen.SetActive(true);
		}
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x0004106D File Offset: 0x0003F26D
	protected override void UnityOnDisable()
	{
		VODPlayer.OnCrash = (Action)Delegate.Remove(VODPlayer.OnCrash, new Action(this.VODPlayer_OnCrash));
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x0004106D File Offset: 0x0003F26D
	private void OnDestroy()
	{
		VODPlayer.OnCrash = (Action)Delegate.Remove(VODPlayer.OnCrash, new Action(this.VODPlayer_OnCrash));
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x0004108F File Offset: 0x0003F28F
	private void VODPlayer_OnCrash()
	{
		this.staticScreen.SetActive(true);
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x000410A0 File Offset: 0x0003F2A0
	protected override void ObservableSliceUpdate()
	{
		if (this.upNextData.Title.IsNullOrEmpty())
		{
			if (this.upNext.text.Length > 0)
			{
				this.upNext.text = string.Empty;
			}
			return;
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		TimeSpan timeSpan = this.upNextData.StartTime - GorillaComputer.instance.GetServerTime();
		this.upNext.text = string.Format("next: {0} - {1:00}:{2:00}", this.upNextData.Title, timeSpan.Minutes, timeSpan.Seconds);
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x00041148 File Offset: 0x0003F348
	public void ShowStatic(bool on)
	{
		this.staticScreen.SetActive(on);
		if (on)
		{
			if (this.observable && VODTarget.AlertDisabled != null)
			{
				VODTarget.AlertDisabled(this);
				return;
			}
		}
		else if (this.observable && VODTarget.AlertEnabled != null)
		{
			VODTarget.AlertEnabled(this);
		}
	}

	// Token: 0x04000E6B RID: 3691
	[SerializeField]
	private Renderer targetRenderer;

	// Token: 0x04000E6C RID: 3692
	[SerializeField]
	private Material standbyOverride;

	// Token: 0x04000E6D RID: 3693
	[SerializeField]
	private VODTarget.VODTargetAudioSettings audioSettings;

	// Token: 0x04000E6E RID: 3694
	[SerializeField]
	private TMP_Text upNext;

	// Token: 0x04000E6F RID: 3695
	[SerializeField]
	private VODPlayer.VODStream.VODStreamChannel[] channel;

	// Token: 0x04000E70 RID: 3696
	[SerializeField]
	private GameObject staticScreen;

	// Token: 0x04000E71 RID: 3697
	[SerializeField]
	private bool unmutable;

	// Token: 0x04000E72 RID: 3698
	public static Action<VODTarget> AlertEnabled;

	// Token: 0x04000E73 RID: 3699
	public static Action<VODTarget> AlertDisabled;

	// Token: 0x04000E74 RID: 3700
	private VODPlayer.VODNextStreamData upNextData;

	// Token: 0x020001BC RID: 444
	[Serializable]
	public class VODTargetAudioSettings
	{
		// Token: 0x04000E75 RID: 3701
		[Range(0f, 1f)]
		public float volume;

		// Token: 0x04000E76 RID: 3702
		[Range(0f, 5f)]
		public float dopplerLevel = 1f;

		// Token: 0x04000E77 RID: 3703
		[Range(0f, 360f)]
		public float spread;

		// Token: 0x04000E78 RID: 3704
		public AudioRolloffMode rolloffMode;

		// Token: 0x04000E79 RID: 3705
		public float minDistance = 0.5f;

		// Token: 0x04000E7A RID: 3706
		public float maxDistance = 5f;
	}
}
