using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200021B RID: 539
public class FingerTorch : MonoBehaviour, ISpawnable
{
	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000E1A RID: 3610 RVA: 0x0004D483 File Offset: 0x0004B683
	// (set) Token: 0x06000E1B RID: 3611 RVA: 0x0004D48B File Offset: 0x0004B68B
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000E1C RID: 3612 RVA: 0x0004D494 File Offset: 0x0004B694
	// (set) Token: 0x06000E1D RID: 3613 RVA: 0x0004D49C File Offset: 0x0004B69C
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000E1E RID: 3614 RVA: 0x0004D4A5 File Offset: 0x0004B6A5
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		if (!this.myRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x0004D4C8 File Offset: 0x0004B6C8
	protected void OnEnable()
	{
		int num = (this.attachedToLeftHand ? 1 : 2);
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected void OnDisable()
	{
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x0004D500 File Offset: 0x0004B700
	private void UpdateLocal()
	{
		int num = (this.attachedToLeftHand ? 4 : 5);
		bool flag = ControllerInputPoller.GripFloat((XRNode)num) > 0.25f;
		bool flag2 = ControllerInputPoller.PrimaryButtonPress((XRNode)num);
		bool flag3 = ControllerInputPoller.SecondaryButtonPress((XRNode)num);
		bool flag4 = flag && (flag2 || flag3);
		this.networkedExtended = flag4;
		if (PhotonNetwork.InRoom && this.myRig)
		{
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.networkedExtended);
		}
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x0004D580 File Offset: 0x0004B780
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
			this.particleFX.SetActive(this.extended);
		}
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x0004D5B4 File Offset: 0x0004B7B4
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x0004D5ED File Offset: 0x0004B7ED
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x0004D60A File Offset: 0x0004B80A
	protected void LateUpdate()
	{
		if (this.IsMyItem())
		{
			this.UpdateLocal();
		}
		else
		{
			this.UpdateReplicated();
		}
		this.UpdateShared();
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x0004D628 File Offset: 0x0004B828
	private void OnExtendStateChanged(bool playAudio)
	{
		this.audioSource.clip = (this.extended ? this.extendAudioClip : this.retractAudioClip);
		if (playAudio)
		{
			this.audioSource.GTPlay();
		}
		if (this.IsMyItem() && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(this.attachedToLeftHand, this.extended ? this.extendVibrationDuration : this.retractVibrationDuration, this.extended ? this.extendVibrationStrength : this.retractVibrationStrength);
		}
	}

	// Token: 0x040010EA RID: 4330
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x040010EB RID: 4331
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x040010EC RID: 4332
	public Transform thumbRingBone;

	// Token: 0x040010ED RID: 4333
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x040010EE RID: 4334
	public AudioClip extendAudioClip;

	// Token: 0x040010EF RID: 4335
	public AudioClip retractAudioClip;

	// Token: 0x040010F0 RID: 4336
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x040010F1 RID: 4337
	public float extendVibrationStrength = 0.2f;

	// Token: 0x040010F2 RID: 4338
	public float retractVibrationDuration = 0.05f;

	// Token: 0x040010F3 RID: 4339
	public float retractVibrationStrength = 0.2f;

	// Token: 0x040010F4 RID: 4340
	[Header("Particle FX")]
	public GameObject particleFX;

	// Token: 0x040010F5 RID: 4341
	private bool networkedExtended;

	// Token: 0x040010F6 RID: 4342
	private bool extended;

	// Token: 0x040010F7 RID: 4343
	private InputDevice inputDevice;

	// Token: 0x040010F8 RID: 4344
	private VRRig myRig;

	// Token: 0x040010F9 RID: 4345
	private int stateBitIndex;
}
