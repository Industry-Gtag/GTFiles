using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000D63 RID: 3427
public class TeleportStationTappable : Tappable
{
	// Token: 0x060054CB RID: 21707 RVA: 0x001BD18C File Offset: 0x001BB38C
	private void Start()
	{
		this._teleportStationRef.TryResolve<TeleportStation>(out this._teleportStation);
		this._firstPersonEffect.SetActive(false);
		this._thirdPersonEffectStart.SetActive(false);
		this._thirdPersonEffectEnd.SetActive(false);
		TeleportStationManager.Initialize(this._firstPersonEffect, this._thirdPersonEffectStart, this._thirdPersonEffectEnd);
	}

	// Token: 0x060054CC RID: 21708 RVA: 0x001BD1E8 File Offset: 0x001BB3E8
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped sender)
	{
		if (this._teleportStation == null)
		{
			GTDev.LogWarning<string>("TeleportStation is null!", null);
			return;
		}
		if (sender.Sender != null && VRRig.LocalRig.Creator != sender.Sender)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(sender.Sender, out rigContainer) && FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 13, sender.SentServerTime))
			{
				this._teleportStation.Attempt3PTeleport(sender);
				UnityEvent on3PTeleport = this._on3PTeleport;
				if (on3PTeleport == null)
				{
					return;
				}
				on3PTeleport.Invoke();
			}
			return;
		}
		this._teleportStation.Attempt1PTeleport(sender);
		UnityEvent on1PTeleport = this._on1PTeleport;
		if (on1PTeleport == null)
		{
			return;
		}
		on1PTeleport.Invoke();
	}

	// Token: 0x0400661B RID: 26139
	[SerializeField]
	private XSceneRef _teleportStationRef;

	// Token: 0x0400661C RID: 26140
	private TeleportStation _teleportStation;

	// Token: 0x0400661D RID: 26141
	[Space]
	[SerializeField]
	private GameObject _firstPersonEffect;

	// Token: 0x0400661E RID: 26142
	[SerializeField]
	private GameObject _thirdPersonEffectStart;

	// Token: 0x0400661F RID: 26143
	[SerializeField]
	private GameObject _thirdPersonEffectEnd;

	// Token: 0x04006620 RID: 26144
	[SerializeField]
	private UnityEvent _on3PTeleport;

	// Token: 0x04006621 RID: 26145
	[SerializeField]
	private UnityEvent _on1PTeleport;
}
