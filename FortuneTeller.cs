using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x020006A8 RID: 1704
public class FortuneTeller : MonoBehaviourPunCallbacks
{
	// Token: 0x06002A73 RID: 10867 RVA: 0x000E5078 File Offset: 0x000E3278
	private void Awake()
	{
		if (this.changeMaterialsInGreyZone && GreyZoneManager.Instance != null)
		{
			GreyZoneManager instance = GreyZoneManager.Instance;
			instance.OnGreyZoneActivated = (Action)Delegate.Combine(instance.OnGreyZoneActivated, new Action(this.GreyZoneActivated));
			GreyZoneManager instance2 = GreyZoneManager.Instance;
			instance2.OnGreyZoneDeactivated = (Action)Delegate.Combine(instance2.OnGreyZoneDeactivated, new Action(this.GreyZoneDeactivated));
		}
	}

	// Token: 0x06002A74 RID: 10868 RVA: 0x000E50EC File Offset: 0x000E32EC
	private void OnDestroy()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager instance = GreyZoneManager.Instance;
			instance.OnGreyZoneActivated = (Action)Delegate.Remove(instance.OnGreyZoneActivated, new Action(this.GreyZoneActivated));
			GreyZoneManager instance2 = GreyZoneManager.Instance;
			instance2.OnGreyZoneDeactivated = (Action)Delegate.Remove(instance2.OnGreyZoneDeactivated, new Action(this.GreyZoneDeactivated));
		}
	}

	// Token: 0x06002A75 RID: 10869 RVA: 0x000E5158 File Offset: 0x000E3358
	public override void OnEnable()
	{
		base.OnEnable();
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
		if (this.button)
		{
			this.button.onPressed += this.HandlePressedButton;
		}
	}

	// Token: 0x06002A76 RID: 10870 RVA: 0x000E5196 File Offset: 0x000E3396
	public override void OnDisable()
	{
		base.OnDisable();
		if (this.button)
		{
			this.button.onPressed -= this.HandlePressedButton;
		}
	}

	// Token: 0x06002A77 RID: 10871 RVA: 0x000E51C2 File Offset: 0x000E33C2
	private void GreyZoneActivated()
	{
		this.boothRenderer.material = this.boothGreyZoneMaterial;
		this.beardRenderer.material = this.beardGreyZoneMaterial;
		this.tellerRenderer.SetMaterials(this.tellerGreyZoneMaterials);
	}

	// Token: 0x06002A78 RID: 10872 RVA: 0x000E51F7 File Offset: 0x000E33F7
	private void GreyZoneDeactivated()
	{
		this.boothRenderer.material = this.boothDefaultMaterial;
		this.beardRenderer.material = this.beardDefaultMaterial;
		this.tellerRenderer.SetMaterials(this.tellerDefaultMaterials);
	}

	// Token: 0x06002A79 RID: 10873 RVA: 0x000E522C File Offset: 0x000E342C
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			base.photonView.RPC("TriggerUpdateFortuneRPC", newPlayer, new object[]
			{
				(int)this.latestFortune.fortuneType,
				this.latestFortune.resultIndex
			});
		}
	}

	// Token: 0x06002A7A RID: 10874 RVA: 0x000E5290 File Offset: 0x000E3490
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.StartAttractModeMonitor();
		}
	}

	// Token: 0x06002A7B RID: 10875 RVA: 0x000E5290 File Offset: 0x000E3490
	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.StartAttractModeMonitor();
		}
	}

	// Token: 0x06002A7C RID: 10876 RVA: 0x000E529F File Offset: 0x000E349F
	private void HandlePressedButton(GorillaPressableButton button, bool isLeft)
	{
		if (base.photonView.IsMine)
		{
			this.SendNewFortune();
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("RequestFortuneRPC", RpcTarget.MasterClient, Array.Empty<object>());
		}
	}

	// Token: 0x06002A7D RID: 10877 RVA: 0x000E52D4 File Offset: 0x000E34D4
	[PunRPC]
	private void RequestFortuneRPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestFortune");
		RigContainer rigContainer;
		if (NetworkSystem.Instance.IsMasterClient && info.Sender != null && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			CallLimitType<CallLimiter> callLimitType = rigContainer.Rig.fxSettings.callSettings[(int)this.limiterType];
			if (callLimitType.UseNetWorkTime ? callLimitType.CallLimitSettings.CheckCallServerTime(info.SentServerTime) : callLimitType.CallLimitSettings.CheckCallTime(Time.time))
			{
				this.SendNewFortune();
			}
		}
	}

	// Token: 0x06002A7E RID: 10878 RVA: 0x000E5360 File Offset: 0x000E3560
	private void SendNewFortune()
	{
		if (this.playable.time > 0.0 && this.playable.time < this.playable.duration)
		{
			return;
		}
		this.latestFortune = this.results.GetResult();
		this.UpdateFortune(this.latestFortune, true);
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("TriggerNewFortuneRPC", RpcTarget.Others, new object[]
			{
				(int)this.latestFortune.fortuneType,
				this.latestFortune.resultIndex
			});
		}
	}

	// Token: 0x06002A7F RID: 10879 RVA: 0x000E5400 File Offset: 0x000E3600
	[PunRPC]
	private void TriggerUpdateFortuneRPC(int fortuneType, int resultIndex, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "TriggerUpdateFortune");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			MonkeAgent.instance.SendReport("Sent TriggerUpdateFortune when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!this.triggerUpdateFortuneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.latestFortune = new FortuneResults.FortuneResult((FortuneResults.FortuneCategoryType)fortuneType, resultIndex);
		this.UpdateFortune(this.latestFortune, false);
	}

	// Token: 0x06002A80 RID: 10880 RVA: 0x000E547C File Offset: 0x000E367C
	[PunRPC]
	private void TriggerNewFortuneRPC(int fortuneType, int resultIndex, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "TriggerNewFortune");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			MonkeAgent.instance.SendReport("Sent TriggerNewFortune when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!this.triggerNewFortuneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.latestFortune = new FortuneResults.FortuneResult((FortuneResults.FortuneCategoryType)fortuneType, resultIndex);
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
		this.UpdateFortune(this.latestFortune, true);
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x000E5508 File Offset: 0x000E3708
	private void StartAttractModeMonitor()
	{
		if (this.attractModeMonitor == null)
		{
			this.attractModeMonitor = base.StartCoroutine(this.AttractModeMonitor());
		}
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x000E5524 File Offset: 0x000E3724
	private IEnumerator AttractModeMonitor()
	{
		while (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			if (Time.time >= this.nextAttractAnimTimestamp)
			{
				this.SendAttractAnim();
			}
			yield return new WaitForSeconds(this.nextAttractAnimTimestamp - Time.time);
		}
		this.attractModeMonitor = null;
		yield break;
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x000E5533 File Offset: 0x000E3733
	private void SendAttractAnim()
	{
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
		{
			base.photonView.RPC("TriggerAttractAnimRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x000E555C File Offset: 0x000E375C
	[PunRPC]
	private void TriggerAttractAnimRPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "TriggerAttractAnim");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			MonkeAgent.instance.SendReport("Sent TriggerAttractAnim when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		this.animator.SetTrigger(this.trigger_attract);
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
	}

	// Token: 0x06002A85 RID: 10885 RVA: 0x000E55D4 File Offset: 0x000E37D4
	private void UpdateFortune(FortuneResults.FortuneResult result, bool newFortune)
	{
		if (this.results)
		{
			PlayableAsset resultFanfare = this.GetResultFanfare(result.fortuneType);
			if (resultFanfare)
			{
				this.playable.initialTime = (newFortune ? 0.0 : resultFanfare.duration);
				this.playable.Play(resultFanfare, DirectorWrapMode.Hold);
				this.animator.SetTrigger(this.trigger_prediction);
				this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
			}
		}
	}

	// Token: 0x06002A86 RID: 10886 RVA: 0x000E5657 File Offset: 0x000E3857
	public void ApplyFortuneText()
	{
		this.text.text = this.results.GetResultText(this.latestFortune).ToUpper();
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x000E567C File Offset: 0x000E387C
	private PlayableAsset GetResultFanfare(FortuneResults.FortuneCategoryType fortuneType)
	{
		foreach (FortuneTeller.FortuneTellerResultFanfare fortuneTellerResultFanfare in this.resultFanfares)
		{
			if (fortuneTellerResultFanfare.type == fortuneType)
			{
				return fortuneTellerResultFanfare.fanfare;
			}
		}
		return null;
	}

	// Token: 0x04003747 RID: 14151
	[SerializeField]
	private FXType limiterType;

	// Token: 0x04003748 RID: 14152
	[SerializeField]
	private FortuneTellerButton button;

	// Token: 0x04003749 RID: 14153
	[SerializeField]
	private TextMeshPro text;

	// Token: 0x0400374A RID: 14154
	[SerializeField]
	private FortuneResults results;

	// Token: 0x0400374B RID: 14155
	[SerializeField]
	private PlayableDirector playable;

	// Token: 0x0400374C RID: 14156
	[SerializeField]
	private Animator animator;

	// Token: 0x0400374D RID: 14157
	[SerializeField]
	private float waitDurationBeforeAttractAnim;

	// Token: 0x0400374E RID: 14158
	[SerializeField]
	private FortuneTeller.FortuneTellerResultFanfare[] resultFanfares;

	// Token: 0x0400374F RID: 14159
	[Header("Grey Zone Visuals")]
	[SerializeField]
	private bool changeMaterialsInGreyZone;

	// Token: 0x04003750 RID: 14160
	[SerializeField]
	private MeshRenderer boothRenderer;

	// Token: 0x04003751 RID: 14161
	[SerializeField]
	private Material boothDefaultMaterial;

	// Token: 0x04003752 RID: 14162
	[SerializeField]
	private Material boothGreyZoneMaterial;

	// Token: 0x04003753 RID: 14163
	[SerializeField]
	private MeshRenderer beardRenderer;

	// Token: 0x04003754 RID: 14164
	[SerializeField]
	private Material beardDefaultMaterial;

	// Token: 0x04003755 RID: 14165
	[SerializeField]
	private Material beardGreyZoneMaterial;

	// Token: 0x04003756 RID: 14166
	[SerializeField]
	private SkinnedMeshRenderer tellerRenderer;

	// Token: 0x04003757 RID: 14167
	[SerializeField]
	private List<Material> tellerDefaultMaterials;

	// Token: 0x04003758 RID: 14168
	[SerializeField]
	private List<Material> tellerGreyZoneMaterials;

	// Token: 0x04003759 RID: 14169
	private FortuneResults.FortuneResult latestFortune;

	// Token: 0x0400375A RID: 14170
	private CallLimiter triggerNewFortuneLimiter = new CallLimiter(10, 1f, 0.5f);

	// Token: 0x0400375B RID: 14171
	private CallLimiter triggerUpdateFortuneLimiter = new CallLimiter(10, 1f, 0.5f);

	// Token: 0x0400375C RID: 14172
	private AnimHashId trigger_attract = "Attract";

	// Token: 0x0400375D RID: 14173
	private AnimHashId trigger_prediction = "Prediction";

	// Token: 0x0400375E RID: 14174
	private float nextAttractAnimTimestamp;

	// Token: 0x0400375F RID: 14175
	private Coroutine attractModeMonitor;

	// Token: 0x020006A9 RID: 1705
	[Serializable]
	public struct FortuneTellerResultFanfare
	{
		// Token: 0x04003760 RID: 14176
		public FortuneResults.FortuneCategoryType type;

		// Token: 0x04003761 RID: 14177
		public PlayableAsset fanfare;
	}
}
