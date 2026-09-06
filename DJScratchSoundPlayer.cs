using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002C3 RID: 707
public class DJScratchSoundPlayer : MonoBehaviour, ISpawnable
{
	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x0600123C RID: 4668 RVA: 0x00061D31 File Offset: 0x0005FF31
	// (set) Token: 0x0600123D RID: 4669 RVA: 0x00061D39 File Offset: 0x0005FF39
	public bool IsSpawned { get; set; }

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x0600123E RID: 4670 RVA: 0x00061D42 File Offset: 0x0005FF42
	// (set) Token: 0x0600123F RID: 4671 RVA: 0x00061D4A File Offset: 0x0005FF4A
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001240 RID: 4672 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDespawn()
	{
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x00061D54 File Offset: 0x0005FF54
	private void OnEnable()
	{
		if (this._events.IsNull())
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = ((this.myRig != null) ? ((this.myRig.creator != null) ? this.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
		}
		this._events.Activate += this.OnPlayEvent;
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x00061DE8 File Offset: 0x0005FFE8
	private void OnDisable()
	{
		if (this._events.IsNotNull())
		{
			this._events.Activate -= this.OnPlayEvent;
			this._events.Dispose();
			this._events = null;
		}
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x00061E36 File Offset: 0x00060036
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		if (!rig.isLocal)
		{
			this.scratchTableLeft.enabled = false;
			this.scratchTableRight.enabled = false;
		}
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x00061E5F File Offset: 0x0006005F
	public void Play(ScratchSoundType type, bool isLeft)
	{
		if (this.myRig.isLocal)
		{
			this.PlayLocal(type, isLeft);
			this._events.Activate.RaiseOthers(new object[] { (int)(type + (isLeft ? 100 : 0)) });
		}
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x00061EA0 File Offset: 0x000600A0
	public void OnPlayEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.myRig.creator.ActorNumber)
		{
			return;
		}
		if (args.Length != 1)
		{
			Debug.LogError(string.Format("Invalid DJ Scratch Event - expected 1 arg, got {0}", args.Length));
			return;
		}
		int num = (int)args[0];
		bool flag = num >= 100;
		if (flag)
		{
			num -= 100;
		}
		ScratchSoundType scratchSoundType = (ScratchSoundType)num;
		if (scratchSoundType < ScratchSoundType.Pause || scratchSoundType > ScratchSoundType.Back)
		{
			return;
		}
		this.PlayLocal(scratchSoundType, flag);
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x00061F18 File Offset: 0x00060118
	public void PlayLocal(ScratchSoundType type, bool isLeft)
	{
		switch (type)
		{
		case ScratchSoundType.Pause:
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			this.scratchPause.Play();
			return;
		case ScratchSoundType.Resume:
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).ResumeTrack();
			this.scratchResume.Play();
			return;
		case ScratchSoundType.Forward:
			this.scratchForward.Play();
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			return;
		case ScratchSoundType.Back:
			this.scratchBack.Play();
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			return;
		default:
			return;
		}
	}

	// Token: 0x0400160F RID: 5647
	[SerializeField]
	private SoundBankPlayer scratchForward;

	// Token: 0x04001610 RID: 5648
	[SerializeField]
	private SoundBankPlayer scratchBack;

	// Token: 0x04001611 RID: 5649
	[SerializeField]
	private SoundBankPlayer scratchPause;

	// Token: 0x04001612 RID: 5650
	[SerializeField]
	private SoundBankPlayer scratchResume;

	// Token: 0x04001613 RID: 5651
	[SerializeField]
	private DJScratchtable scratchTableLeft;

	// Token: 0x04001614 RID: 5652
	[SerializeField]
	private DJScratchtable scratchTableRight;

	// Token: 0x04001615 RID: 5653
	private RubberDuckEvents _events;

	// Token: 0x04001616 RID: 5654
	private VRRig myRig;
}
