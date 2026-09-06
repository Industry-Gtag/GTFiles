using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000821 RID: 2081
public class GRToolShieldGun : MonoBehaviour
{
	// Token: 0x06003564 RID: 13668 RVA: 0x001256DD File Offset: 0x001238DD
	private void Awake()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x06003565 RID: 13669 RVA: 0x00125710 File Offset: 0x00123910
	private void OnToolUpgraded(GRTool tool)
	{
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength1))
		{
			this.firingSound = this.upgrade1FiringSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength2))
		{
			this.firingSound = this.upgrade2FiringSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength3))
		{
			this.firingSound = this.upgrade3FiringSound;
		}
	}

	// Token: 0x06003566 RID: 13670 RVA: 0x00125761 File Offset: 0x00123961
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06003567 RID: 13671 RVA: 0x0012577C File Offset: 0x0012397C
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06003568 RID: 13672 RVA: 0x001257B0 File Offset: 0x001239B0
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolShieldGun.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolShieldGun.State.Charging);
				this.activatedLocally = true;
				return;
			}
			break;
		case GRToolShieldGun.State.Charging:
		{
			bool flag = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Firing);
				return;
			}
			if (!flag)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolShieldGun.State.Firing:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Cooldown);
				return;
			}
			break;
		case GRToolShieldGun.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f && !this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolShieldGun.State.Idle);
				this.activatedLocally = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003569 RID: 13673 RVA: 0x00125898 File Offset: 0x00123A98
	private void OnUpdateRemote(float dt)
	{
		GRToolShieldGun.State state = (GRToolShieldGun.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetStateAuthority(state);
		}
	}

	// Token: 0x0600356A RID: 13674 RVA: 0x001258C2 File Offset: 0x00123AC2
	private void SetStateAuthority(GRToolShieldGun.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x0600356B RID: 13675 RVA: 0x001258E4 File Offset: 0x00123AE4
	private void SetState(GRToolShieldGun.State newState)
	{
		if (newState == this.state || !this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case GRToolShieldGun.State.Idle:
			this.stateTimeRemaining = -1f;
			return;
		case GRToolShieldGun.State.Charging:
			this.StartCharge();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolShieldGun.State.Firing:
			this.StartFiring();
			this.stateTimeRemaining = this.flashDuration;
			return;
		case GRToolShieldGun.State.Cooldown:
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x0600356C RID: 13676 RVA: 0x00125968 File Offset: 0x00123B68
	private void StartCharge()
	{
		if (this.chargeSound != null)
		{
			this.audioSource.PlayOneShot(this.chargeSound, this.chargeSoundVolume);
		}
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x0600356D RID: 13677 RVA: 0x001259B8 File Offset: 0x00123BB8
	private void StartFiring()
	{
		if (this.firingSound != null)
		{
			this.audioSource.PlayOneShot(this.firingSound, this.firingSoundVolume);
		}
		this.timeLastFired = Time.time;
		this.tool.UseEnergy();
		Vector3 position = this.firingTransform.position;
		Vector3 vector = this.firingTransform.forward * this.projectileSpeed;
		float scale = GTPlayer.Instance.scale;
		int num = PoolUtils.GameObjHashCode(this.projectilePrefab);
		this.firedProjectile = ObjectPools.instance.Instantiate(num, true).GetComponent<SlingshotProjectile>();
		this.firedProjectile.transform.localScale = Vector3.one * scale;
		if (this.projectileTrailPrefab != null)
		{
			int num2 = PoolUtils.GameObjHashCode(this.projectileTrailPrefab);
			this.AttachTrail(num2, this.firedProjectile.gameObject, position, false, false);
		}
		Collider component = this.firedProjectile.gameObject.GetComponent<Collider>();
		if (component != null)
		{
			for (int i = 0; i < this.colliders.Count; i++)
			{
				Physics.IgnoreCollision(this.colliders[i], component);
			}
		}
		if (this.IsHeldLocal())
		{
			this.firedProjectile.OnImpact += this.OnProjectileImpact;
		}
		this.onHaptic.PlayIfHeldLocal(this.gameEntity);
		this.firedProjectile.Launch(position, vector, NetworkSystem.Instance.LocalPlayer, false, false, 1, scale, true, this.projectileColor);
	}

	// Token: 0x0600356E RID: 13678 RVA: 0x00125B3C File Offset: 0x00123D3C
	private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(trailHash, true);
		SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
		if (component.IsNull())
		{
			ObjectPools.instance.Destroy(gameObject);
		}
		newProjectile.transform.position = location;
		component.AttachTrail(newProjectile, blueTeam, orangeTeam, false, default(Color));
	}

	// Token: 0x0600356F RID: 13679 RVA: 0x00125B90 File Offset: 0x00123D90
	private void OnProjectileImpact(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer)
	{
		projectile.OnImpact -= this.OnProjectileImpact;
		GRPlayer grplayer = null;
		RigContainer rigContainer;
		if (hitPlayer != null && VRRigCache.Instance.TryGetVrrig(hitPlayer, out rigContainer) && rigContainer.Rig != null)
		{
			grplayer = rigContainer.Rig.GetComponent<GRPlayer>();
		}
		else if (this.allowAoeHits)
		{
			GRToolShieldGun.vrRigs.Clear();
			GRToolShieldGun.vrRigs.Add(VRRig.LocalRig);
			VRRigCache.Instance.GetAllUsedRigs(GRToolShieldGun.vrRigs);
			VRRig vrrig = null;
			float num = float.MaxValue;
			for (int i = 0; i < GRToolShieldGun.vrRigs.Count; i++)
			{
				float sqrMagnitude = (GRToolShieldGun.vrRigs[i].bodyTransform.position - impactPos).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					vrrig = GRToolShieldGun.vrRigs[i];
				}
			}
			if (vrrig != null)
			{
				grplayer = vrrig.GetComponent<GRPlayer>();
			}
		}
		if (grplayer != null)
		{
			int num2 = 0;
			if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength1))
			{
				num2 |= 1;
			}
			if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength2))
			{
				num2 |= 2;
			}
			if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength3))
			{
				num2 |= 4;
			}
			this.gameEntity.manager.ghostReactorManager.RequestGrantPlayerShield(grplayer, this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ShieldSize), num2);
		}
	}

	// Token: 0x06003570 RID: 13680 RVA: 0x00125CF4 File Offset: 0x00123EF4
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x06003571 RID: 13681 RVA: 0x00125D54 File Offset: 0x00123F54
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x06003572 RID: 13682 RVA: 0x00125DA8 File Offset: 0x00123FA8
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L && ((int)newStateIndex != 2 || Time.time > this.timeLastFired + this.cooldownMinimum);
	}

	// Token: 0x040045A0 RID: 17824
	public GameEntity gameEntity;

	// Token: 0x040045A1 RID: 17825
	public GRTool tool;

	// Token: 0x040045A2 RID: 17826
	public GRAttributes attributes;

	// Token: 0x040045A3 RID: 17827
	public GameObject projectilePrefab;

	// Token: 0x040045A4 RID: 17828
	public GameObject projectileTrailPrefab;

	// Token: 0x040045A5 RID: 17829
	public Transform firingTransform;

	// Token: 0x040045A6 RID: 17830
	public List<Collider> colliders;

	// Token: 0x040045A7 RID: 17831
	public float projectileSpeed = 25f;

	// Token: 0x040045A8 RID: 17832
	public Color projectileColor = new Color(0.25f, 0.25f, 1f);

	// Token: 0x040045A9 RID: 17833
	public bool allowAoeHits;

	// Token: 0x040045AA RID: 17834
	public float aeoHitRadius = 0.5f;

	// Token: 0x040045AB RID: 17835
	public float chargeDuration = 0.75f;

	// Token: 0x040045AC RID: 17836
	public float flashDuration = 0.1f;

	// Token: 0x040045AD RID: 17837
	public float cooldownDuration;

	// Token: 0x040045AE RID: 17838
	public AudioSource audioSource;

	// Token: 0x040045AF RID: 17839
	public AudioClip chargeSound;

	// Token: 0x040045B0 RID: 17840
	public float chargeSoundVolume = 0.5f;

	// Token: 0x040045B1 RID: 17841
	public AudioClip firingSound;

	// Token: 0x040045B2 RID: 17842
	public float firingSoundVolume = 0.5f;

	// Token: 0x040045B3 RID: 17843
	public AudioClip upgrade1FiringSound;

	// Token: 0x040045B4 RID: 17844
	public AudioClip upgrade2FiringSound;

	// Token: 0x040045B5 RID: 17845
	public AudioClip upgrade3FiringSound;

	// Token: 0x040045B6 RID: 17846
	[Header("Haptic")]
	public AbilityHaptic onHaptic;

	// Token: 0x040045B7 RID: 17847
	private GRToolShieldGun.State state;

	// Token: 0x040045B8 RID: 17848
	private float stateTimeRemaining;

	// Token: 0x040045B9 RID: 17849
	private bool activatedLocally;

	// Token: 0x040045BA RID: 17850
	private bool waitingForButtonRelease;

	// Token: 0x040045BB RID: 17851
	private float timeLastFired;

	// Token: 0x040045BC RID: 17852
	private float cooldownMinimum = 0.35f;

	// Token: 0x040045BD RID: 17853
	private SlingshotProjectile firedProjectile;

	// Token: 0x040045BE RID: 17854
	private static List<VRRig> vrRigs = new List<VRRig>(10);

	// Token: 0x02000822 RID: 2082
	private enum State
	{
		// Token: 0x040045C0 RID: 17856
		Idle,
		// Token: 0x040045C1 RID: 17857
		Charging,
		// Token: 0x040045C2 RID: 17858
		Firing,
		// Token: 0x040045C3 RID: 17859
		Cooldown,
		// Token: 0x040045C4 RID: 17860
		Count
	}
}
