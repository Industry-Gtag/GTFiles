using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002BC RID: 700
public class CrankableToyCarHoldable : TransferrableObject
{
	// Token: 0x0600120E RID: 4622 RVA: 0x00060E1A File Offset: 0x0005F01A
	protected override void Start()
	{
		base.Start();
		this.crank.SetOnCrankedCallback(new Action<float>(this.OnCranked));
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x00060E3C File Offset: 0x0005F03C
	internal override void OnEnable()
	{
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
		}
		NetPlayer netPlayer = ((base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
		if (netPlayer != null && this._events != null)
		{
			this._events.Init(netPlayer);
			this._events.Activate += this.OnDeployRPC;
		}
		else
		{
			Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x00060F27 File Offset: 0x0005F127
	internal override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Dispose();
		}
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x00060F48 File Offset: 0x0005F148
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.deployablePart.activeSelf)
			{
				this.OnCarDeployed();
				return;
			}
		}
		else if (this.deployablePart.activeSelf)
		{
			this.OnCarReturned();
		}
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x00060F9C File Offset: 0x0005F19C
	private void OnCranked(float deltaAngle)
	{
		this.currentCrankStrength += Mathf.Abs(deltaAngle);
		this.currentCrankClickAmount += deltaAngle;
		if (Mathf.Abs(this.currentCrankClickAmount) > this.crankAnglePerClick)
		{
			if (this.currentCrankStrength >= this.maxCrankStrength)
			{
				this.overCrankedSound.Play();
				VRRig ownerRig = this.ownerRig;
				if (ownerRig != null && ownerRig.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.overcrankHapticStrength, this.overcrankHapticDuration);
				}
			}
			else
			{
				float num = Mathf.Lerp(this.minClickPitch, this.maxClickPitch, Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength));
				SoundBankPlayer soundBankPlayer = this.clickSound;
				float? num2 = new float?(num);
				soundBankPlayer.Play(null, num2);
				VRRig ownerRig2 = this.ownerRig;
				if (ownerRig2 != null && ownerRig2.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.crankHapticStrength, this.crankHapticDuration);
				}
			}
			this.currentCrankClickAmount = 0f;
		}
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x000610B0 File Offset: 0x0005F2B0
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
		{
			return false;
		}
		if (this.currentCrankStrength == 0f)
		{
			return true;
		}
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(flag);
		Vector3 vector = base.transform.TransformPoint(Vector3.zero);
		Quaternion rotation = base.transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		float num = Mathf.Lerp(this.minLifetime, this.maxLifetime, Mathf.Clamp01(Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength)));
		this.DeployCarLocal(vector, rotation, averageVelocity, num, false);
		if (PhotonNetwork.InRoom)
		{
			this._events.Activate.RaiseOthers(new object[]
			{
				BitPackUtils.PackWorldPosForNetwork(vector),
				BitPackUtils.PackQuaternionForNetwork(rotation),
				BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f),
				num
			});
		}
		this.currentCrankStrength = 0f;
		return true;
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x000611DF File Offset: 0x0005F3DF
	private void DeployCarLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		if (!this.disabledWhileDeployed.activeSelf)
		{
			return;
		}
		this.deployedCar.Deploy(this, launchPos, launchRot, releaseVel, lifetime, isRemote);
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x00061204 File Offset: 0x0005F404
	private void OnDeployRPC(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (!this || sender != receiver || info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "OnDeployRPC");
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork((long)args[0]);
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork((int)args[1]);
		Vector3 vector2 = BitPackUtils.UnpackWorldPosFromNetwork((long)args[2]) / 100f;
		float num = (float)args[3];
		float num2 = 10000f;
		if ((in vector).IsValid(in num2) && (in quaternion).IsValid())
		{
			float num3 = 10000f;
			if ((in vector2).IsValid(in num3))
			{
				this.DeployCarLocal(vector, quaternion, vector2, num, true);
				return;
			}
		}
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x000612B9 File Offset: 0x0005F4B9
	public void OnCarDeployed()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this.deployablePart.SetActive(true);
		this.disabledWhileDeployed.SetActive(false);
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x000612E1 File Offset: 0x0005F4E1
	public void OnCarReturned()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this.deployablePart.SetActive(false);
		this.disabledWhileDeployed.SetActive(true);
		this.clickSound.RestartSequence();
	}

	// Token: 0x040015CA RID: 5578
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank crank;

	// Token: 0x040015CB RID: 5579
	[SerializeField]
	private CrankableToyCarDeployed deployedCar;

	// Token: 0x040015CC RID: 5580
	[SerializeField]
	private GameObject deployablePart;

	// Token: 0x040015CD RID: 5581
	[SerializeField]
	private GameObject disabledWhileDeployed;

	// Token: 0x040015CE RID: 5582
	[SerializeField]
	private float crankAnglePerClick;

	// Token: 0x040015CF RID: 5583
	[SerializeField]
	private float maxCrankStrength;

	// Token: 0x040015D0 RID: 5584
	[SerializeField]
	private float minClickPitch;

	// Token: 0x040015D1 RID: 5585
	[SerializeField]
	private float maxClickPitch;

	// Token: 0x040015D2 RID: 5586
	[SerializeField]
	private float minLifetime;

	// Token: 0x040015D3 RID: 5587
	[SerializeField]
	private float maxLifetime;

	// Token: 0x040015D4 RID: 5588
	[SerializeField]
	private SoundBankPlayer clickSound;

	// Token: 0x040015D5 RID: 5589
	[SerializeField]
	private SoundBankPlayer overCrankedSound;

	// Token: 0x040015D6 RID: 5590
	[SerializeField]
	private float crankHapticStrength = 0.1f;

	// Token: 0x040015D7 RID: 5591
	[SerializeField]
	private float crankHapticDuration = 0.05f;

	// Token: 0x040015D8 RID: 5592
	[SerializeField]
	private float overcrankHapticStrength = 0.8f;

	// Token: 0x040015D9 RID: 5593
	[SerializeField]
	private float overcrankHapticDuration = 0.05f;

	// Token: 0x040015DA RID: 5594
	private float currentCrankStrength;

	// Token: 0x040015DB RID: 5595
	private float currentCrankClickAmount;

	// Token: 0x040015DC RID: 5596
	private RubberDuckEvents _events;
}
