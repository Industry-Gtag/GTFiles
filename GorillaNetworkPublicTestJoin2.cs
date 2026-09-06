using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000CCE RID: 3278
public class GorillaNetworkPublicTestJoin2 : GorillaTriggerBox
{
	// Token: 0x0600513D RID: 20797 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Awake()
	{
	}

	// Token: 0x0600513E RID: 20798 RVA: 0x001AED8C File Offset: 0x001ACF8C
	public void LateUpdate()
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

	// Token: 0x0600513F RID: 20799 RVA: 0x001AEEC0 File Offset: 0x001AD0C0
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

	// Token: 0x0400633B RID: 25403
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x0400633C RID: 25404
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x0400633D RID: 25405
	public string gameModeName;

	// Token: 0x0400633E RID: 25406
	public PhotonNetworkController photonNetworkController;

	// Token: 0x0400633F RID: 25407
	public string componentTypeToAdd;

	// Token: 0x04006340 RID: 25408
	public GameObject componentTarget;

	// Token: 0x04006341 RID: 25409
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x04006342 RID: 25410
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x04006343 RID: 25411
	private Transform tosPition;

	// Token: 0x04006344 RID: 25412
	private Transform othsTosPosition;

	// Token: 0x04006345 RID: 25413
	private PhotonView fotVew;

	// Token: 0x04006346 RID: 25414
	private bool waiting;

	// Token: 0x04006347 RID: 25415
	private Vector3 lastPosition;

	// Token: 0x04006348 RID: 25416
	private VRRig tempRig;
}
