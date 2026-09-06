using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000692 RID: 1682
public class ElderGorilla : MonoBehaviour
{
	// Token: 0x060029FE RID: 10750 RVA: 0x000E2790 File Offset: 0x000E0990
	private void Update()
	{
		if (GTPlayer.Instance == null)
		{
			return;
		}
		if (GTPlayer.Instance.inOverlay || !GTPlayer.Instance.isUserPresent)
		{
			return;
		}
		this.tHMD = GTPlayer.Instance.headCollider.transform;
		this.tLeftHand = GTPlayer.Instance.GetControllerTransform(true);
		this.tRightHand = GTPlayer.Instance.GetControllerTransform(false);
		if (Time.time - this.timeLastValidArmDist > 1f)
		{
			this.CheckHandDistance(this.tLeftHand);
			this.CheckHandDistance(this.tRightHand);
		}
		this.CheckHeight();
		this.CheckMicVolume();
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x000E2834 File Offset: 0x000E0A34
	private void CheckHandDistance(Transform hand)
	{
		float num = Vector3.Distance(hand.localPosition, this.tHMD.localPosition);
		if (num >= 1f)
		{
			return;
		}
		if (num >= 0.75f)
		{
			this.countValidArmDists++;
			this.timeLastValidArmDist = Time.time;
		}
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x000E2884 File Offset: 0x000E0A84
	private void CheckHeight()
	{
		float y = this.tHMD.localPosition.y;
		if (!this.trackingHeadHeight)
		{
			this.trackedHeadHeight = y - 0.05f;
			this.timerTrackedHeadHeight = 0f;
		}
		else if (this.trackedHeadHeight < y)
		{
			this.trackingHeadHeight = false;
		}
		if (this.trackingHeadHeight)
		{
			if (this.timerTrackedHeadHeight >= 1f)
			{
				this.savedHeadHeight = y;
				this.trackingHeadHeight = false;
				return;
			}
			this.timerTrackedHeadHeight += Time.deltaTime;
		}
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x000E290A File Offset: 0x000E0B0A
	private void CheckMicVolume()
	{
		float currentPeakAmp = GorillaTagger.Instance.myRecorder.LevelMeter.CurrentPeakAmp;
	}

	// Token: 0x04003695 RID: 13973
	private const float MAX_HAND_DIST = 1f;

	// Token: 0x04003696 RID: 13974
	private const float COOLDOWN_HAND_DIST = 1f;

	// Token: 0x04003697 RID: 13975
	private const float VALID_HAND_DIST = 0.75f;

	// Token: 0x04003698 RID: 13976
	private const float TIME_VALID_HEAD_HEIGHT = 1f;

	// Token: 0x04003699 RID: 13977
	private Transform tHMD;

	// Token: 0x0400369A RID: 13978
	private Transform tLeftHand;

	// Token: 0x0400369B RID: 13979
	private Transform tRightHand;

	// Token: 0x0400369C RID: 13980
	private int countValidArmDists;

	// Token: 0x0400369D RID: 13981
	private float timeLastValidArmDist;

	// Token: 0x0400369E RID: 13982
	private bool trackingHeadHeight;

	// Token: 0x0400369F RID: 13983
	private float trackedHeadHeight;

	// Token: 0x040036A0 RID: 13984
	private float timerTrackedHeadHeight;

	// Token: 0x040036A1 RID: 13985
	private float savedHeadHeight = 1.5f;
}
