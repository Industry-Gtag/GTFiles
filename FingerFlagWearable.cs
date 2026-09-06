using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020001FC RID: 508
public class FingerFlagWearable : MonoBehaviour, ISpawnable
{
	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000D57 RID: 3415 RVA: 0x00049299 File Offset: 0x00047499
	// (set) Token: 0x06000D58 RID: 3416 RVA: 0x000492A1 File Offset: 0x000474A1
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000D59 RID: 3417 RVA: 0x000492AA File Offset: 0x000474AA
	// (set) Token: 0x06000D5A RID: 3418 RVA: 0x000492B2 File Offset: 0x000474B2
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000D5B RID: 3419 RVA: 0x000492BB File Offset: 0x000474BB
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = base.GetComponentInParent<VRRig>(true);
		if (!this.myRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x000492E4 File Offset: 0x000474E4
	protected void OnEnable()
	{
		int num = (this.attachedToLeftHand ? 1 : 2);
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x0004931C File Offset: 0x0004751C
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

	// Token: 0x06000D5F RID: 3423 RVA: 0x0004939C File Offset: 0x0004759C
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
		}
		bool flag = this.fullyRetracted;
		this.fullyRetracted = this.extended && this.retractExtendTime <= 0f;
		if (flag != this.fullyRetracted)
		{
			Transform[] array = this.clothRigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(!this.fullyRetracted);
			}
		}
		this.UpdateAnimation();
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x0004942A File Offset: 0x0004762A
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x00049463 File Offset: 0x00047663
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x00049480 File Offset: 0x00047680
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

	// Token: 0x06000D63 RID: 3427 RVA: 0x000494A0 File Offset: 0x000476A0
	private void UpdateAnimation()
	{
		float num = (this.extended ? this.extendSpeed : (-this.retractSpeed));
		this.retractExtendTime = Mathf.Clamp01(this.retractExtendTime + Time.deltaTime * num);
		this.animator.SetFloat(this.retractExtendTimeAnimParam, this.retractExtendTime);
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x000494F8 File Offset: 0x000476F8
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

	// Token: 0x04000FF5 RID: 4085
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x04000FF6 RID: 4086
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x04000FF7 RID: 4087
	public Transform thumbRingBone;

	// Token: 0x04000FF8 RID: 4088
	public Transform[] clothBones;

	// Token: 0x04000FF9 RID: 4089
	public Transform[] clothRigidbodies;

	// Token: 0x04000FFA RID: 4090
	[Header("Animation")]
	public Animator animator;

	// Token: 0x04000FFB RID: 4091
	public float extendSpeed = 1.5f;

	// Token: 0x04000FFC RID: 4092
	public float retractSpeed = 2.25f;

	// Token: 0x04000FFD RID: 4093
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x04000FFE RID: 4094
	public AudioClip extendAudioClip;

	// Token: 0x04000FFF RID: 4095
	public AudioClip retractAudioClip;

	// Token: 0x04001000 RID: 4096
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x04001001 RID: 4097
	public float extendVibrationStrength = 0.2f;

	// Token: 0x04001002 RID: 4098
	public float retractVibrationDuration = 0.05f;

	// Token: 0x04001003 RID: 4099
	public float retractVibrationStrength = 0.2f;

	// Token: 0x04001004 RID: 4100
	private readonly int retractExtendTimeAnimParam = Animator.StringToHash("retractExtendTime");

	// Token: 0x04001005 RID: 4101
	private bool networkedExtended;

	// Token: 0x04001006 RID: 4102
	private bool extended;

	// Token: 0x04001007 RID: 4103
	private bool fullyRetracted;

	// Token: 0x04001008 RID: 4104
	private float retractExtendTime;

	// Token: 0x04001009 RID: 4105
	private InputDevice inputDevice;

	// Token: 0x0400100A RID: 4106
	private VRRig myRig;

	// Token: 0x0400100B RID: 4107
	private int stateBitIndex;
}
