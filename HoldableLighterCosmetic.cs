using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000C3 RID: 195
public class HoldableLighterCosmetic : MonoBehaviour
{
	// Token: 0x060004C5 RID: 1221 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnEnable()
	{
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x0001AB19 File Offset: 0x00018D19
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0001AB33 File Offset: 0x00018D33
	private bool IsMyItem()
	{
		return this.rig != null && this.rig.isOfflineVRRig;
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x0001AB50 File Offset: 0x00018D50
	private void DebugPull()
	{
		this.TriggerPulled();
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0001AB58 File Offset: 0x00018D58
	private void DebugRelease()
	{
		this.TriggerReleased();
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0001AB60 File Offset: 0x00018D60
	public void TriggerPulled()
	{
		this.triggerHeld = true;
		if (this.OwnerID == 0)
		{
			this.TrySetID();
		}
		double time = PhotonNetwork.Time;
		switch (this.GetResultAtTime(time, this.OwnerID))
		{
		case HoldableLighterCosmetic.LighterResult.Flicker:
		{
			UnityEvent onFlicker = this.OnFlicker;
			if (onFlicker != null)
			{
				onFlicker.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.1f, 0.1f);
				return;
			}
			break;
		}
		case HoldableLighterCosmetic.LighterResult.Light:
		{
			UnityEvent onLight = this.OnLight;
			if (onLight != null)
			{
				onLight.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.1f, 0.1f);
				return;
			}
			break;
		}
		case HoldableLighterCosmetic.LighterResult.Explode:
		{
			UnityEvent onExplode = this.OnExplode;
			if (onExplode != null)
			{
				onExplode.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.75f, 0.5f);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0001AC68 File Offset: 0x00018E68
	private HoldableLighterCosmetic.LighterResult GetResultAtTime(double photonTime, int seed)
	{
		int num = (int)Math.Floor(photonTime);
		float num2 = (float)new Random(seed ^ num).NextDouble();
		if (num2 < this.explodeWeight)
		{
			return HoldableLighterCosmetic.LighterResult.Explode;
		}
		if (num2 < this.explodeWeight + this.lightWeight)
		{
			return HoldableLighterCosmetic.LighterResult.Light;
		}
		return HoldableLighterCosmetic.LighterResult.Flicker;
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0001ACAA File Offset: 0x00018EAA
	public void TriggerReleased()
	{
		this.triggerHeld = false;
		UnityEvent onTriggerRelease = this.OnTriggerRelease;
		if (onTriggerRelease == null)
		{
			return;
		}
		onTriggerRelease.Invoke();
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0001ACC4 File Offset: 0x00018EC4
	private void TrySetID()
	{
		if (this.parentTransferable.IsLocalObject())
		{
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (instance != null)
			{
				string playFabPlayerId = instance.GetPlayFabPlayerId();
				Type type = base.GetType();
				this.OwnerID = (playFabPlayerId + ((type != null) ? type.ToString() : null)).GetStaticHash();
				return;
			}
		}
		else if (this.parentTransferable.targetRig != null && this.parentTransferable.targetRig.creator != null)
		{
			string userId = this.parentTransferable.targetRig.creator.UserId;
			Type type2 = base.GetType();
			this.OwnerID = (userId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
		}
	}

	// Token: 0x04000543 RID: 1347
	private int OwnerID;

	// Token: 0x04000544 RID: 1348
	[Header("Weights (0 to 1 total)")]
	[Range(0f, 1f)]
	public float flickerWeight = 0.5f;

	// Token: 0x04000545 RID: 1349
	[Range(0f, 1f)]
	public float lightWeight = 0.3f;

	// Token: 0x04000546 RID: 1350
	[Range(0f, 1f)]
	public float explodeWeight = 0.2f;

	// Token: 0x04000547 RID: 1351
	[Header("Unity Events")]
	public UnityEvent OnFlicker;

	// Token: 0x04000548 RID: 1352
	public UnityEvent OnLight;

	// Token: 0x04000549 RID: 1353
	public UnityEvent OnExplode;

	// Token: 0x0400054A RID: 1354
	public UnityEvent OnTriggerRelease;

	// Token: 0x0400054B RID: 1355
	private HoldableLighterCosmetic.LighterResult[] resultTimeline;

	// Token: 0x0400054C RID: 1356
	private bool triggerHeld;

	// Token: 0x0400054D RID: 1357
	private float lastCheckTime;

	// Token: 0x0400054E RID: 1358
	private VRRig rig;

	// Token: 0x0400054F RID: 1359
	private TransferrableObject parentTransferable;

	// Token: 0x020000C4 RID: 196
	public enum LighterResult
	{
		// Token: 0x04000551 RID: 1361
		Flicker,
		// Token: 0x04000552 RID: 1362
		Light,
		// Token: 0x04000553 RID: 1363
		Explode
	}
}
