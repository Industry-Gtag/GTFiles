using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002C5 RID: 709
public class ElfLauncher : MonoBehaviour
{
	// Token: 0x0600124F RID: 4687 RVA: 0x00062314 File Offset: 0x00060514
	private void OnEnable()
	{
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = ((this.parentHoldable.myOnlineRig != null) ? this.parentHoldable.myOnlineRig.creator : ((this.parentHoldable.myRig != null) ? ((this.parentHoldable.myRig.creator != null) ? this.parentHoldable.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
			if (netPlayer != null)
			{
				this.m_player = netPlayer;
				this._events.Init(netPlayer);
			}
			else
			{
				Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
			}
		}
		if (this._events != null)
		{
			this._events.Activate += this.ShootShared;
		}
	}

	// Token: 0x06001250 RID: 4688 RVA: 0x00062400 File Offset: 0x00060600
	private void OnDisable()
	{
		if (this._events != null)
		{
			this._events.Activate -= this.ShootShared;
			this._events.Dispose();
			this._events = null;
			this.m_player = null;
		}
	}

	// Token: 0x06001251 RID: 4689 RVA: 0x00062458 File Offset: 0x00060658
	private void Awake()
	{
		this._events = base.GetComponent<RubberDuckEvents>();
		this.elfProjectileHash = PoolUtils.GameObjHashCode(this.elfProjectilePrefab);
		TransferrableObjectHoldablePart_Crank[] array = this.cranks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOnCrankedCallback(new Action<float>(this.OnCranked));
		}
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x000624AC File Offset: 0x000606AC
	private void OnCranked(float deltaAngle)
	{
		this.currentShootCrankAmount += deltaAngle;
		if (Mathf.Abs(this.currentShootCrankAmount) > this.crankShootThreshold)
		{
			this.currentShootCrankAmount = 0f;
			this.Shoot();
		}
		this.currentClickCrankAmount += deltaAngle;
		if (Mathf.Abs(this.currentClickCrankAmount) > this.crankClickThreshold)
		{
			this.currentClickCrankAmount = 0f;
			this.crankClickAudio.Play();
		}
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x00062524 File Offset: 0x00060724
	private void Shoot()
	{
		if (this.parentHoldable.IsLocalObject())
		{
			GorillaTagger.Instance.StartVibration(true, this.shootHapticStrength, this.shootHapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.shootHapticStrength, this.shootHapticDuration);
			if (PhotonNetwork.InRoom)
			{
				this._events.Activate.RaiseAll(new object[]
				{
					this.muzzle.transform.position,
					this.muzzle.transform.forward
				});
				return;
			}
			this.ShootShared(this.muzzle.transform.position, this.muzzle.transform.forward);
		}
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x000625E4 File Offset: 0x000607E4
	private void ShootShared(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (args.Length != 2)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		VRRig ownerRig = this.parentHoldable.ownerRig;
		if (info.senderID != ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (args.Length == 2)
		{
			object obj = args[0];
			if (obj is Vector3)
			{
				Vector3 vector = (Vector3)obj;
				obj = args[1];
				if (obj is Vector3)
				{
					Vector3 vector2 = (Vector3)obj;
					float num = 10000f;
					if ((in vector).IsValid(in num))
					{
						float num2 = 10000f;
						if ((in vector2).IsValid(in num2))
						{
							if (!FXSystem.CheckCallSpam(ownerRig.fxSettings, 4, info.SentServerTime) || !ownerRig.IsPositionInRange(vector, 6f))
							{
								return;
							}
							this.ShootShared(vector, vector2);
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x0006269C File Offset: 0x0006089C
	protected virtual void ShootShared(Vector3 origin, Vector3 direction)
	{
		this.shootAudio.Play();
		Vector3 lossyScale = base.transform.lossyScale;
		GameObject gameObject = ObjectPools.instance.Instantiate(this.elfProjectileHash, true);
		gameObject.transform.position = origin;
		gameObject.transform.rotation = Quaternion.LookRotation(direction);
		gameObject.transform.localScale = lossyScale;
		gameObject.GetComponent<Rigidbody>().linearVelocity = direction * this.muzzleVelocity * lossyScale.x;
	}

	// Token: 0x0400162A RID: 5674
	[SerializeField]
	protected TransferrableObject parentHoldable;

	// Token: 0x0400162B RID: 5675
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank[] cranks;

	// Token: 0x0400162C RID: 5676
	[SerializeField]
	private float crankShootThreshold = 360f;

	// Token: 0x0400162D RID: 5677
	[SerializeField]
	private float crankClickThreshold = 30f;

	// Token: 0x0400162E RID: 5678
	[SerializeField]
	private Transform muzzle;

	// Token: 0x0400162F RID: 5679
	[SerializeField]
	private GameObject elfProjectilePrefab;

	// Token: 0x04001630 RID: 5680
	protected int elfProjectileHash;

	// Token: 0x04001631 RID: 5681
	[SerializeField]
	protected float muzzleVelocity = 10f;

	// Token: 0x04001632 RID: 5682
	[SerializeField]
	private SoundBankPlayer crankClickAudio;

	// Token: 0x04001633 RID: 5683
	[SerializeField]
	protected SoundBankPlayer shootAudio;

	// Token: 0x04001634 RID: 5684
	[SerializeField]
	private float shootHapticStrength;

	// Token: 0x04001635 RID: 5685
	[SerializeField]
	private float shootHapticDuration;

	// Token: 0x04001636 RID: 5686
	private RubberDuckEvents _events;

	// Token: 0x04001637 RID: 5687
	private float currentShootCrankAmount;

	// Token: 0x04001638 RID: 5688
	private float currentClickCrankAmount;

	// Token: 0x04001639 RID: 5689
	private NetPlayer m_player;
}
