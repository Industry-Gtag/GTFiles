using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020005A5 RID: 1445
public class StopwatchCosmetic : TransferrableObject
{
	// Token: 0x170003D5 RID: 981
	// (get) Token: 0x06002491 RID: 9361 RVA: 0x000C461B File Offset: 0x000C281B
	public bool isActivating
	{
		get
		{
			return this._isActivating;
		}
	}

	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x06002492 RID: 9362 RVA: 0x000C4623 File Offset: 0x000C2823
	public float activeTimeElapsed
	{
		get
		{
			return this._activeTimeElapsed;
		}
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x000C462C File Offset: 0x000C282C
	protected override void Awake()
	{
		base.Awake();
		if (StopwatchCosmetic.gWatchToggleRPC == null)
		{
			StopwatchCosmetic.gWatchToggleRPC = new PhotonEvent(StaticHash.Compute("StopwatchCosmetic", "WatchToggle"));
		}
		if (StopwatchCosmetic.gWatchResetRPC == null)
		{
			StopwatchCosmetic.gWatchResetRPC = new PhotonEvent(StaticHash.Compute("StopwatchCosmetic", "WatchReset"));
		}
		this._watchToggle = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchToggle);
		this._watchReset = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchReset);
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x000C46B0 File Offset: 0x000C28B0
	internal override void OnEnable()
	{
		base.OnEnable();
		int num;
		if (!this.FetchMyViewID(out num))
		{
			this._photonID = -1;
			return;
		}
		StopwatchCosmetic.gWatchResetRPC += this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC += this._watchToggle;
		this._photonID = num.GetStaticHash();
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000C470B File Offset: 0x000C290B
	internal override void OnDisable()
	{
		base.OnDisable();
		StopwatchCosmetic.gWatchResetRPC -= this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC -= this._watchToggle;
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x000C4740 File Offset: 0x000C2940
	private void OnWatchToggle(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "OnWatchToggle");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		bool flag = (bool)args[1];
		int num = (int)args[2];
		this._watchFace.SetMillisElapsed(num, true);
		this._watchFace.WatchToggle();
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x000C47C0 File Offset: 0x000C29C0
	private void OnWatchReset(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "OnWatchReset");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		this._watchFace.WatchReset();
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x000C4820 File Offset: 0x000C2A20
	private bool FetchMyViewID(out int viewID)
	{
		viewID = -1;
		NetPlayer netPlayer = ((base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
		if (netPlayer == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
		{
			return false;
		}
		if (rigContainer.Rig.netView == null)
		{
			return false;
		}
		viewID = rigContainer.Rig.netView.ViewID;
		return true;
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x000C48BF File Offset: 0x000C2ABF
	public bool PollActivated()
	{
		if (!this._activated)
		{
			return false;
		}
		this._activated = false;
		return true;
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000C48D4 File Offset: 0x000C2AD4
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (this._isActivating)
		{
			this._activeTimeElapsed += Time.deltaTime;
		}
		if (this._isActivating && this._activeTimeElapsed > 1f)
		{
			this._isActivating = false;
			this._watchFace.WatchReset(true);
			StopwatchCosmetic.gWatchResetRPC.RaiseOthers(new object[] { this._photonID });
		}
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x000C4947 File Offset: 0x000C2B47
	public override void OnActivate()
	{
		if (!this.CanActivate())
		{
			return;
		}
		base.OnActivate();
		if (this.IsMyItem())
		{
			this._activeTimeElapsed = 0f;
			this._isActivating = true;
		}
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x000C4974 File Offset: 0x000C2B74
	public override void OnDeactivate()
	{
		if (!this.CanDeactivate())
		{
			return;
		}
		base.OnDeactivate();
		if (!this.IsMyItem())
		{
			return;
		}
		this._isActivating = false;
		this._activated = true;
		this._watchFace.WatchToggle();
		StopwatchCosmetic.gWatchToggleRPC.RaiseOthers(new object[]
		{
			this._photonID,
			this._watchFace.watchActive,
			this._watchFace.millisElapsed
		});
		this._activated = false;
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x000C49FD File Offset: 0x000C2BFD
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x0600249E RID: 9374 RVA: 0x000C4A08 File Offset: 0x000C2C08
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04003007 RID: 12295
	[SerializeField]
	private StopwatchFace _watchFace;

	// Token: 0x04003008 RID: 12296
	[Space]
	[NonSerialized]
	private bool _isActivating;

	// Token: 0x04003009 RID: 12297
	[NonSerialized]
	private float _activeTimeElapsed;

	// Token: 0x0400300A RID: 12298
	[NonSerialized]
	private bool _activated;

	// Token: 0x0400300B RID: 12299
	[Space]
	[NonSerialized]
	private int _photonID = -1;

	// Token: 0x0400300C RID: 12300
	private static PhotonEvent gWatchToggleRPC;

	// Token: 0x0400300D RID: 12301
	private static PhotonEvent gWatchResetRPC;

	// Token: 0x0400300E RID: 12302
	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchToggle;

	// Token: 0x0400300F RID: 12303
	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchReset;

	// Token: 0x04003010 RID: 12304
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04003011 RID: 12305
	[DebugOption]
	public bool disableDeactivation;
}
