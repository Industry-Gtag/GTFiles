using System;
using GorillaTagScripts.Builder;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class ArtilleryCannon : MonoBehaviour
{
	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000A9B RID: 2715 RVA: 0x000391DD File Offset: 0x000373DD
	private int LocalActorNr
	{
		get
		{
			if (PhotonNetwork.LocalPlayer == null)
			{
				return -1;
			}
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x000391F2 File Offset: 0x000373F2
	private void Awake()
	{
		if (this.projectilePrefab != null)
		{
			this.projectileHash = PoolUtils.GameObjHashCode(this.projectilePrefab);
		}
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x00039214 File Offset: 0x00037414
	private void OnEnable()
	{
		if (this.fireHitNotifier != null)
		{
			this.fireHitNotifier.OnProjectileHit += this.OnFireProjectileHit;
		}
		ArtilleryCannonState artilleryCannonState;
		if (this.stateRef.TryResolve<ArtilleryCannonState>(out artilleryCannonState))
		{
			this.Bind(artilleryCannonState);
			return;
		}
		this.stateRef.AddCallbackOnLoad(new Action(this.OnStateSceneLoaded));
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x00039274 File Offset: 0x00037474
	private void OnDisable()
	{
		if (this.fireHitNotifier != null)
		{
			this.fireHitNotifier.OnProjectileHit -= this.OnFireProjectileHit;
		}
		this.stateRef.RemoveCallbackOnLoad(new Action(this.OnStateSceneLoaded));
		this.Unbind();
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x000392C4 File Offset: 0x000374C4
	private void OnStateSceneLoaded()
	{
		ArtilleryCannonState artilleryCannonState;
		if (this.stateRef.TryResolve<ArtilleryCannonState>(out artilleryCannonState))
		{
			this.Bind(artilleryCannonState);
		}
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x000392E8 File Offset: 0x000374E8
	private void Bind(ArtilleryCannonState newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.Unbind();
		this.state = newState;
		if (this.state == null)
		{
			return;
		}
		this.state.onRotationChanged += this.OnRotationChanged;
		this.state.onFired += this.OnFiredRemote;
		this.ApplyRotation();
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x00039354 File Offset: 0x00037554
	private void Unbind()
	{
		if (this.state == null)
		{
			return;
		}
		this.state.onRotationChanged -= this.OnRotationChanged;
		this.state.onFired -= this.OnFiredRemote;
		this.state = null;
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x000393A8 File Offset: 0x000375A8
	private void LateUpdate()
	{
		if (this.state == null)
		{
			return;
		}
		int localActorNr = this.LocalActorNr;
		if (this.pitchCrank != null && this.state.pitchCrankSync.holderActorNr == localActorNr)
		{
			this.state.UpdateLocalCrankState(0, this.pitchCrank.IsHeldLeftHand, this.pitchCrank.CurrentAngle);
		}
		if (this.yawCrank != null && this.state.yawCrankSync.holderActorNr == localActorNr)
		{
			this.state.UpdateLocalCrankState(1, this.yawCrank.IsHeldLeftHand, this.yawCrank.CurrentAngle);
		}
		this.UpdateRemoteCrankVisual(this.pitchCrank, this.state.pitchCrankSync, localActorNr);
		this.UpdateRemoteCrankVisual(this.yawCrank, this.state.yawCrankSync, localActorNr);
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x00039484 File Offset: 0x00037684
	private void UpdateRemoteCrankVisual(ArtilleryCrank crank, ArtilleryCannonState.CrankSyncState syncState, int localActor)
	{
		if (crank == null || syncState.holderActorNr == localActor)
		{
			return;
		}
		if (syncState.holderActorNr != -1)
		{
			VRRig vrrig = ArtilleryCannonState.FindRigForActor(syncState.holderActorNr);
			if (vrrig != null)
			{
				crank.UpdateFromRemoteHand(vrrig, syncState.isLeftHand);
				return;
			}
		}
		crank.SetVisualAngle(syncState.angle);
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x000394DC File Offset: 0x000376DC
	internal bool IsCrankHeldLocally(int crankIndex)
	{
		return (ref crankIndex == 0 ? ref this.state.pitchCrankSync : ref this.state.yawCrankSync).holderActorNr == this.LocalActorNr;
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00039506 File Offset: 0x00037706
	internal bool OnCrankGrabbed(int crankIndex, bool isLeftHand)
	{
		return this.state.NotifyCrankGrabbed(crankIndex, isLeftHand);
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00039515 File Offset: 0x00037715
	internal void OnCrankReleased(int crankIndex, float finalAngle)
	{
		this.state.NotifyCrankReleased(crankIndex, finalAngle);
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00039524 File Offset: 0x00037724
	internal void OnCrankInput(int crankIndex, float degrees)
	{
		this.state.NotifyCrankInput(crankIndex, degrees);
		this.ApplyRotation();
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x00039539 File Offset: 0x00037739
	private void OnRotationChanged()
	{
		this.ApplyRotation();
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x00039544 File Offset: 0x00037744
	private void ApplyRotation()
	{
		if (this.state == null)
		{
			return;
		}
		if (this.yawTransform != null)
		{
			this.yawTransform.localRotation = Quaternion.Euler(0f, this.state.CurrentYaw, 0f);
		}
		if (this.pitchTransform != null)
		{
			this.pitchTransform.localRotation = Quaternion.Euler(-this.state.CurrentPitch, 0f, 0f);
		}
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x000395C7 File Offset: 0x000377C7
	public void Fire()
	{
		if (this.state == null)
		{
			return;
		}
		if (this.state.TryFire())
		{
			this.FireLocal();
		}
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x000395EB File Offset: 0x000377EB
	private void OnFireProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		this.Fire();
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x000395F3 File Offset: 0x000377F3
	private void OnFiredRemote()
	{
		this.FireLocal();
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x000395FC File Offset: 0x000377FC
	private void FireLocal()
	{
		if (this.projectilePrefab == null || this.muzzle == null)
		{
			return;
		}
		Vector3 position = this.muzzle.position;
		Vector3 forward = this.muzzle.forward;
		GameObject gameObject = ObjectPools.instance.Instantiate(this.projectileHash, true);
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.LookRotation(forward);
		BuilderProjectile component = gameObject.GetComponent<BuilderProjectile>();
		if (component != null)
		{
			component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(this.knockbackConfig);
		}
		Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
		if (component2 != null)
		{
			component2.linearVelocity = forward * this.launchSpeed;
		}
		if (this.fireSound != null)
		{
			this.fireSound.GTPlay();
		}
	}

	// Token: 0x04000CE0 RID: 3296
	[Header("Network State")]
	[SerializeField]
	private XSceneRef stateRef;

	// Token: 0x04000CE1 RID: 3297
	[Header("Cranks")]
	[SerializeField]
	private ArtilleryCrank pitchCrank;

	// Token: 0x04000CE2 RID: 3298
	[SerializeField]
	private ArtilleryCrank yawCrank;

	// Token: 0x04000CE3 RID: 3299
	[Header("Rotation")]
	[SerializeField]
	private Transform yawTransform;

	// Token: 0x04000CE4 RID: 3300
	[SerializeField]
	private Transform pitchTransform;

	// Token: 0x04000CE5 RID: 3301
	[Header("Firing")]
	[SerializeField]
	private Transform muzzle;

	// Token: 0x04000CE6 RID: 3302
	[SerializeField]
	private GameObject projectilePrefab;

	// Token: 0x04000CE7 RID: 3303
	[SerializeField]
	private float launchSpeed = 30f;

	// Token: 0x04000CE8 RID: 3304
	[SerializeField]
	private AudioSource fireSound;

	// Token: 0x04000CE9 RID: 3305
	[SerializeField]
	private SlingshotProjectile.AOEKnockbackConfig knockbackConfig;

	// Token: 0x04000CEA RID: 3306
	[Header("Fire Trigger")]
	[Tooltip("When a projectile hits this notifier, the cannon fires.")]
	[SerializeField]
	private SlingshotProjectileHitNotifier fireHitNotifier;

	// Token: 0x04000CEB RID: 3307
	private ArtilleryCannonState state;

	// Token: 0x04000CEC RID: 3308
	private int projectileHash;
}
