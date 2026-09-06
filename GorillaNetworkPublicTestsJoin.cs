using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000CD0 RID: 3280
public class GorillaNetworkPublicTestsJoin : GorillaTriggerBox, ITickSystemPost
{
	// Token: 0x170007B9 RID: 1977
	// (get) Token: 0x06005147 RID: 20807 RVA: 0x001AF0A4 File Offset: 0x001AD2A4
	// (set) Token: 0x06005148 RID: 20808 RVA: 0x001AF0AC File Offset: 0x001AD2AC
	public bool PostTickRunning { get; set; }

	// Token: 0x06005149 RID: 20809 RVA: 0x001AF0B5 File Offset: 0x001AD2B5
	public void Awake()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x0600514A RID: 20810 RVA: 0x001AF0C0 File Offset: 0x001AD2C0
	public void PostTick()
	{
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (GTPlayer.Instance.GetComponent<Rigidbody>().isKinematic && !this.waiting && !MonkeAgent.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				if ((GTPlayer.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || GTPlayer.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f) && !this.waiting && !MonkeAgent.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				float magnitude = (GTPlayer.Instance.transform.position - this.lastPosition).magnitude;
			}
			this.lastPosition = GTPlayer.Instance.transform.position;
		}
		catch
		{
		}
	}

	// Token: 0x0600514B RID: 20811 RVA: 0x001AF1F4 File Offset: 0x001AD3F4
	private IEnumerator GracePeriod()
	{
		this.waiting = true;
		yield return new WaitForSeconds(30f);
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (GTPlayer.Instance.GetComponent<Rigidbody>().isKinematic)
				{
					MonkeAgent.instance.SendReport("gorvity bisdabled", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GTPlayer.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || GTPlayer.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f)
				{
					MonkeAgent.instance.SendReport(string.Concat(new string[]
					{
						"jimp 2mcuh.",
						GTPlayer.Instance.jumpMultiplier.ToString(),
						".",
						GTPlayer.Instance.maxJumpSpeed.ToString(),
						"."
					}), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GorillaTagger.Instance.sphereCastRadius > 0.04f)
				{
					MonkeAgent.instance.SendReport("wack rad. " + GorillaTagger.Instance.sphereCastRadius.ToString(), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
			}
			this.waiting = false;
			yield break;
		}
		catch
		{
			yield break;
		}
		yield break;
	}

	// Token: 0x0400634C RID: 25420
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x0400634D RID: 25421
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x0400634E RID: 25422
	public string gameModeName;

	// Token: 0x0400634F RID: 25423
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04006350 RID: 25424
	public string componentTypeToAdd;

	// Token: 0x04006351 RID: 25425
	public GameObject componentTarget;

	// Token: 0x04006352 RID: 25426
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x04006353 RID: 25427
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x04006354 RID: 25428
	private Transform tosPition;

	// Token: 0x04006355 RID: 25429
	private Transform othsTosPosition;

	// Token: 0x04006356 RID: 25430
	private PhotonView fotVew;

	// Token: 0x04006357 RID: 25431
	private bool waiting;

	// Token: 0x04006358 RID: 25432
	private Vector3 lastPosition;

	// Token: 0x04006359 RID: 25433
	private VRRig tempRig;
}
