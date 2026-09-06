using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002C4 RID: 708
public class DJScratchtable : MonoBehaviour
{
	// Token: 0x06001248 RID: 4680 RVA: 0x00061FC3 File Offset: 0x000601C3
	public void SetPlaying(bool playing)
	{
		this.isPlaying = playing;
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x00061FCC File Offset: 0x000601CC
	private void OnTriggerStay(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		Vector3 vector = (base.transform.parent.InverseTransformPoint(collider.transform.position) - base.transform.localPosition).WithY(0f);
		float num = Mathf.Atan2(vector.z, vector.x) * 57.29578f;
		if (this.isTouching)
		{
			base.transform.localRotation = Quaternion.LookRotation(vector) * this.firstTouchRotation;
			if (this.isPlaying)
			{
				float num2 = Mathf.DeltaAngle(this.lastScratchSoundAngle, num);
				if (num2 > this.scratchMinAngle)
				{
					if (Time.time > this.cantForwardScratchUntilTimestamp)
					{
						this.scratchPlayer.Play(ScratchSoundType.Forward, this.isLeft);
						this.cantForwardScratchUntilTimestamp = Time.time + this.scratchCooldown;
						this.lastScratchSoundAngle = num;
						GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, this.hapticStrength, this.hapticDuration);
					}
				}
				else if (num2 < -this.scratchMinAngle && Time.time > this.cantBackScratchUntilTimestamp)
				{
					this.scratchPlayer.Play(ScratchSoundType.Back, this.isLeft);
					this.cantBackScratchUntilTimestamp = Time.time + this.scratchCooldown;
					this.lastScratchSoundAngle = num;
					GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, this.hapticStrength, this.hapticDuration);
				}
			}
		}
		else
		{
			this.firstTouchRotation = Quaternion.Inverse(Quaternion.LookRotation(base.transform.InverseTransformPoint(collider.transform.position).WithY(0f)));
			if (this.isPlaying)
			{
				this.PauseTrack();
				this.scratchPlayer.Play(ScratchSoundType.Pause, this.isLeft);
				this.lastScratchSoundAngle = num;
				this.cantForwardScratchUntilTimestamp = Time.time + this.scratchCooldown;
				this.cantBackScratchUntilTimestamp = Time.time + this.scratchCooldown;
			}
		}
		this.isTouching = true;
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x000621D0 File Offset: 0x000603D0
	private void OnTriggerExit(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		if (this.isPlaying)
		{
			this.ResumeTrack();
			this.scratchPlayer.Play(ScratchSoundType.Resume, this.isLeft);
		}
		this.isTouching = false;
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x0006221C File Offset: 0x0006041C
	public void SelectTrack(int track)
	{
		this.lastSelectedTrack = track;
		if (track == 0)
		{
			this.turntableVisual.Stop();
			this.isPlaying = false;
		}
		else
		{
			this.turntableVisual.Run();
			this.isPlaying = true;
		}
		int num = track - 1;
		for (int i = 0; i < this.tracks.Length; i++)
		{
			if (num == i)
			{
				float num2 = (float)(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) % this.trackDuration;
				this.tracks[i].Play();
				this.tracks[i].time = num2;
			}
			else
			{
				this.tracks[i].Stop();
			}
		}
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x000622BC File Offset: 0x000604BC
	public void PauseTrack()
	{
		for (int i = 0; i < this.tracks.Length; i++)
		{
			this.tracks[i].Stop();
		}
		this.pausedUntilTimestamp = Time.time + 1f;
	}

	// Token: 0x0600124D RID: 4685 RVA: 0x000622FA File Offset: 0x000604FA
	public void ResumeTrack()
	{
		this.SelectTrack(this.lastSelectedTrack);
		this.pausedUntilTimestamp = 0f;
	}

	// Token: 0x04001619 RID: 5657
	[SerializeField]
	private bool isLeft;

	// Token: 0x0400161A RID: 5658
	[SerializeField]
	private DJScratchSoundPlayer scratchPlayer;

	// Token: 0x0400161B RID: 5659
	[SerializeField]
	private float scratchCooldown;

	// Token: 0x0400161C RID: 5660
	[SerializeField]
	private float scratchMinAngle;

	// Token: 0x0400161D RID: 5661
	[SerializeField]
	private AudioSource[] tracks;

	// Token: 0x0400161E RID: 5662
	[SerializeField]
	private CosmeticFan turntableVisual;

	// Token: 0x0400161F RID: 5663
	[SerializeField]
	private float trackDuration;

	// Token: 0x04001620 RID: 5664
	[SerializeField]
	private float hapticStrength;

	// Token: 0x04001621 RID: 5665
	[SerializeField]
	private float hapticDuration;

	// Token: 0x04001622 RID: 5666
	private int lastSelectedTrack;

	// Token: 0x04001623 RID: 5667
	private bool isPlaying;

	// Token: 0x04001624 RID: 5668
	private bool isTouching;

	// Token: 0x04001625 RID: 5669
	private Quaternion firstTouchRotation;

	// Token: 0x04001626 RID: 5670
	private float lastScratchSoundAngle;

	// Token: 0x04001627 RID: 5671
	private float cantForwardScratchUntilTimestamp;

	// Token: 0x04001628 RID: 5672
	private float cantBackScratchUntilTimestamp;

	// Token: 0x04001629 RID: 5673
	private float pausedUntilTimestamp;
}
