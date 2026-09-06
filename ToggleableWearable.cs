using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020003AA RID: 938
public class ToggleableWearable : MonoBehaviour
{
	// Token: 0x060016BD RID: 5821 RVA: 0x00083BC0 File Offset: 0x00081DC0
	protected void Awake()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
		if (this.ownerRig == null)
		{
			GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
			if (componentInParent != null)
			{
				this.ownerRig = componentInParent.offlineVRRig;
				this.ownerIsLocal = this.ownerRig != null;
			}
		}
		if (this.ownerRig == null)
		{
			Debug.LogError("TriggerToggler: Disabling cannot find VRRig.");
			base.enabled = false;
			return;
		}
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer == null)
			{
				Debug.LogError("TriggerToggler: Disabling because a renderer is null.");
				base.enabled = false;
				break;
			}
			renderer.enabled = this.startOn;
		}
		this.hasAudioSource = this.audioSource != null;
		this.assignedSlotBitIndex = (int)this.assignedSlot;
		if (this.oneShot)
		{
			this.toggleCooldownRange.x = this.toggleCooldownRange.x + this.animationTransitionDuration;
			this.toggleCooldownRange.y = this.toggleCooldownRange.y + this.animationTransitionDuration;
		}
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x00083CC8 File Offset: 0x00081EC8
	protected void LateUpdate()
	{
		if (this.ownerIsLocal)
		{
			this.toggleCooldownTimer -= Time.deltaTime;
			Transform transform = base.transform;
			if (Physics.OverlapSphereNonAlloc(transform.TransformPoint(this.triggerOffset), this.triggerRadius * transform.lossyScale.x, this.colliders, this.layerMask) > 0 && this.toggleCooldownTimer < 0f)
			{
				XRController componentInParent = this.colliders[0].GetComponentInParent<XRController>();
				if (componentInParent != null)
				{
					this.LocalToggle(componentInParent.controllerNode == XRNode.LeftHand, true, true);
				}
				this.toggleCooldownTimer = Random.Range(this.toggleCooldownRange.x, this.toggleCooldownRange.y);
				this.toggleTimer = 0f;
			}
			if (this.resetTimer > 0f)
			{
				this.toggleTimer += Time.deltaTime;
				if (this.toggleTimer > this.resetTimer && this.startOn != this.isOn)
				{
					this.LocalToggle(false, true, false);
					this.toggleTimer = 0f;
				}
			}
		}
		else
		{
			bool flag = (this.ownerRig.WearablePackedStates & (1 << this.assignedSlotBitIndex)) != 0;
			if (this.isOn != flag)
			{
				this.SharedSetState(flag, true);
			}
		}
		if (this.oneShot)
		{
			if (this.isOn)
			{
				this.progress = Mathf.MoveTowards(this.progress, 1f, Time.deltaTime / this.animationTransitionDuration);
				if (this.progress == 1f)
				{
					if (this.ownerIsLocal)
					{
						this.LocalToggle(false, false, false);
					}
					else
					{
						this.SharedSetState(false, false);
					}
					this.progress = 0f;
				}
			}
		}
		else
		{
			this.progress = Mathf.MoveTowards(this.progress, this.isOn ? 1f : 0f, Time.deltaTime / this.animationTransitionDuration);
		}
		Animator[] array = this.animators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(ToggleableWearable.animParam_Progress, this.progress);
		}
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x00083ED8 File Offset: 0x000820D8
	private void LocalToggle(bool isLeftHand, bool playAudio, bool playHaptics)
	{
		this.ownerRig.WearablePackedStates ^= 1 << this.assignedSlotBitIndex;
		this.SharedSetState((this.ownerRig.WearablePackedStates & (1 << this.assignedSlotBitIndex)) != 0, playAudio);
		if (playHaptics && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.isOn ? this.turnOnVibrationDuration : this.turnOffVibrationDuration, this.isOn ? this.turnOnVibrationStrength : this.turnOffVibrationStrength);
		}
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x00083F6C File Offset: 0x0008216C
	private void SharedSetState(bool state, bool playAudio)
	{
		this.isOn = state;
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.isOn;
		}
		if (!playAudio || !this.hasAudioSource)
		{
			return;
		}
		AudioClip audioClip = (this.isOn ? this.toggleOnSound : this.toggleOffSound);
		if (audioClip == null)
		{
			return;
		}
		if (this.oneShot)
		{
			this.audioSource.clip = audioClip;
			this.audioSource.GTPlay();
			return;
		}
		this.audioSource.GTPlayOneShot(audioClip, 1f);
	}

	// Token: 0x040020DD RID: 8413
	public Renderer[] renderers;

	// Token: 0x040020DE RID: 8414
	public Animator[] animators;

	// Token: 0x040020DF RID: 8415
	public float animationTransitionDuration = 1f;

	// Token: 0x040020E0 RID: 8416
	[Tooltip("Whether the wearable state is toggled on by default.")]
	public bool startOn;

	// Token: 0x040020E1 RID: 8417
	[Tooltip("AudioSource to play toggle sounds.")]
	public AudioSource audioSource;

	// Token: 0x040020E2 RID: 8418
	[Tooltip("Sound to play when toggled on.")]
	public AudioClip toggleOnSound;

	// Token: 0x040020E3 RID: 8419
	[Tooltip("Sound to play when toggled off.")]
	public AudioClip toggleOffSound;

	// Token: 0x040020E4 RID: 8420
	[Tooltip("Layer to check for trigger sphere collisions.")]
	public LayerMask layerMask;

	// Token: 0x040020E5 RID: 8421
	[Tooltip("Radius of the trigger sphere.")]
	public float triggerRadius = 0.2f;

	// Token: 0x040020E6 RID: 8422
	[Tooltip("Position in local space to move the trigger sphere.")]
	public Vector3 triggerOffset = Vector3.zero;

	// Token: 0x040020E7 RID: 8423
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	public VRRig.WearablePackedStateSlots assignedSlot;

	// Token: 0x040020E8 RID: 8424
	[Header("Vibration")]
	public float turnOnVibrationDuration = 0.05f;

	// Token: 0x040020E9 RID: 8425
	public float turnOnVibrationStrength = 0.2f;

	// Token: 0x040020EA RID: 8426
	public float turnOffVibrationDuration = 0.05f;

	// Token: 0x040020EB RID: 8427
	public float turnOffVibrationStrength = 0.2f;

	// Token: 0x040020EC RID: 8428
	private VRRig ownerRig;

	// Token: 0x040020ED RID: 8429
	private bool ownerIsLocal;

	// Token: 0x040020EE RID: 8430
	private bool isOn;

	// Token: 0x040020EF RID: 8431
	[SerializeField]
	private Vector2 toggleCooldownRange = new Vector2(0.2f, 0.2f);

	// Token: 0x040020F0 RID: 8432
	private bool hasAudioSource;

	// Token: 0x040020F1 RID: 8433
	private readonly Collider[] colliders = new Collider[1];

	// Token: 0x040020F2 RID: 8434
	private int framesSinceCooldownAndExitingVolume;

	// Token: 0x040020F3 RID: 8435
	private float toggleCooldownTimer;

	// Token: 0x040020F4 RID: 8436
	private int assignedSlotBitIndex;

	// Token: 0x040020F5 RID: 8437
	private static readonly int animParam_Progress = Animator.StringToHash("Progress");

	// Token: 0x040020F6 RID: 8438
	private float progress;

	// Token: 0x040020F7 RID: 8439
	[SerializeField]
	private bool oneShot;

	// Token: 0x040020F8 RID: 8440
	[SerializeField]
	[Tooltip("Seconds before reverting to its default state, as defined by 'Start On.' A value of 0 or less means never.")]
	private float resetTimer;

	// Token: 0x040020F9 RID: 8441
	private float toggleTimer;
}
