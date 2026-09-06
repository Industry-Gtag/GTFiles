using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag.Gravity;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004BE RID: 1214
public class SlingshotProjectile : MonoBehaviour
{
	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06001DA9 RID: 7593 RVA: 0x000A04B8 File Offset: 0x0009E6B8
	// (set) Token: 0x06001DAA RID: 7594 RVA: 0x000A04C0 File Offset: 0x0009E6C0
	public Vector3 launchPosition { get; private set; }

	// Token: 0x1400003B RID: 59
	// (add) Token: 0x06001DAB RID: 7595 RVA: 0x000A04CC File Offset: 0x0009E6CC
	// (remove) Token: 0x06001DAC RID: 7596 RVA: 0x000A0504 File Offset: 0x0009E704
	public event SlingshotProjectile.ProjectileImpactEvent OnImpact;

	// Token: 0x06001DAD RID: 7597 RVA: 0x000A053C File Offset: 0x0009E73C
	public void Launch(Vector3 position, Vector3 velocity, NetPlayer player, bool blueTeam, bool orangeTeam, int projectileCount, float scale, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (this.launchSoundBankPlayer != null)
		{
			this.launchSoundBankPlayer.Play();
		}
		this.particleLaunched = true;
		this.timeCreated = Time.time;
		this.launchPosition = position;
		Transform transform = base.transform;
		transform.position = position;
		transform.localScale = Vector3.one * scale;
		base.GetComponent<Collider>().contactOffset = 0.01f * scale;
		RigidbodyWaterInteraction component = base.GetComponent<RigidbodyWaterInteraction>();
		if (component != null)
		{
			component.objectRadiusForWaterCollision = 0.02f * scale;
		}
		this.gravityController.GravityMultiplier = this.gravityMultiplier * ((scale < 1f) ? scale : 1f);
		this.projectileRigidbody.isKinematic = false;
		this.projectileRigidbody.useGravity = false;
		this.projectileRigidbody.linearVelocity = velocity;
		this.projectileOwner = player;
		this.myProjectileCount = projectileCount;
		this.projectileRigidbody.position = position;
		this.ApplyTeamModelAndColor(blueTeam, orangeTeam, shouldOverrideColor, overrideColor);
		this.remainingLifeTime = this.lifeTime;
		if (this.useForwardForce && this.forceComponent)
		{
			this.forceComponent.enabled = true;
			this.forceComponent.force = this.projectileRigidbody.linearVelocity.normalized * this.forwardForceMultiplier;
		}
		this.isSettled = false;
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			this.gravityController.SetPersonalGravityDirection(rigContainer.Rig.transform.up);
		}
		UnityEvent<NetPlayer> onLaunch = this.OnLaunch;
		if (onLaunch == null)
		{
			return;
		}
		onLaunch.Invoke(this.projectileOwner);
	}

	// Token: 0x06001DAE RID: 7598 RVA: 0x000A06D8 File Offset: 0x0009E8D8
	protected void Awake()
	{
		if (this.playerImpactEffectPrefab == null)
		{
			this.playerImpactEffectPrefab = this.surfaceImpactEffectPrefab;
		}
		this.projectileRigidbody = base.GetComponent<Rigidbody>();
		this.forceComponent = base.GetComponent<ConstantForce>();
		this.initialScale = base.transform.localScale.x;
		this.matPropBlock = new MaterialPropertyBlock();
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
		this.remainingLifeTime = this.lifeTime;
		this.gravityController = base.GetComponent<MonkeGravityController>();
		if (this.gravityController == null)
		{
			this.gravityController = base.gameObject.AddComponent<MonkeGravityController>();
		}
	}

	// Token: 0x06001DAF RID: 7599 RVA: 0x000A077C File Offset: 0x0009E97C
	public void Deactivate()
	{
		base.transform.localScale = Vector3.one * this.initialScale;
		this.projectileRigidbody.useGravity = true;
		if (this.forceComponent)
		{
			this.forceComponent.force = Vector3.zero;
		}
		this.OnImpact = null;
		this.aoeKnockbackConfig = null;
		this.impactSoundVolumeOverride = null;
		this.impactSoundPitchOverride = null;
		this.impactEffectScaleMultiplier = 1f;
		this.projectileRigidbody.isKinematic = false;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06001DB0 RID: 7600 RVA: 0x000A0820 File Offset: 0x0009EA20
	private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
	{
		if (prefab == null)
		{
			return;
		}
		Vector3 vector = position + normal * this.impactEffectOffset;
		GameObject gameObject = ObjectPools.instance.Instantiate(prefab, vector, true);
		Vector3 localScale = base.transform.localScale;
		gameObject.transform.localScale = localScale * this.impactEffectScaleMultiplier;
		gameObject.transform.up = normal;
		GorillaColorizableBase component = gameObject.GetComponent<GorillaColorizableBase>();
		if (component != null)
		{
			component.SetColor(this.teamColor);
		}
		SurfaceImpactFX component2 = gameObject.GetComponent<SurfaceImpactFX>();
		if (component2 != null)
		{
			component2.SetScale(localScale.x * this.impactEffectScaleMultiplier);
		}
		SoundBankPlayer component3 = gameObject.GetComponent<SoundBankPlayer>();
		if (component3 != null && !component3.playOnEnable)
		{
			component3.Play(this.impactSoundVolumeOverride, this.impactSoundPitchOverride);
		}
		if (this.spawnWorldEffects != null)
		{
			this.spawnWorldEffects.RequestSpawn(position, normal);
		}
		UnityEvent<Vector3> onImapctEvent = this.OnImapctEvent;
		if (onImapctEvent == null)
		{
			return;
		}
		onImapctEvent.Invoke(position);
	}

	// Token: 0x06001DB1 RID: 7601 RVA: 0x000A0920 File Offset: 0x0009EB20
	public void CheckForAOEKnockback(Vector3 impactPosition, float impactSpeed)
	{
		if (this.aoeKnockbackConfig != null && this.aoeKnockbackConfig.Value.applyAOEKnockback)
		{
			Vector3 vector = GTPlayer.Instance.HeadCenterPosition - impactPosition;
			if (vector.sqrMagnitude < this.aoeKnockbackConfig.Value.aeoOuterRadius * this.aoeKnockbackConfig.Value.aeoOuterRadius)
			{
				float magnitude = vector.magnitude;
				Vector3 vector2 = ((magnitude > 0.001f) ? (vector / magnitude) : Vector3.up);
				float num = Mathf.InverseLerp(this.aoeKnockbackConfig.Value.aeoOuterRadius, this.aoeKnockbackConfig.Value.aeoInnerRadius, magnitude);
				float num2 = Mathf.InverseLerp(0f, this.aoeKnockbackConfig.Value.impactVelocityThreshold, impactSpeed);
				GTPlayer.Instance.ApplyKnockback(vector2, this.aoeKnockbackConfig.Value.knockbackVelocity * num * num2, false);
				this.impactEffectScaleMultiplier = Mathf.Lerp(1f, this.impactEffectScaleMultiplier, num2);
				if (this.impactSoundVolumeOverride != null)
				{
					this.impactSoundVolumeOverride = new float?(Mathf.Lerp(this.impactSoundVolumeOverride.Value * 0.5f, this.impactSoundVolumeOverride.Value, num2));
				}
				float num3 = Mathf.Lerp(this.aoeKnockbackConfig.Value.aeoInnerRadius, this.aoeKnockbackConfig.Value.aeoOuterRadius, 0.25f);
				if (this.aoeKnockbackConfig.Value.playerProximityEffect != PlayerEffect.NONE && vector.sqrMagnitude < num3 * num3)
				{
					RoomSystem.SendPlayerEffect(PlayerEffect.SNOWBALL_IMPACT, NetworkSystem.Instance.LocalPlayer);
				}
			}
		}
	}

	// Token: 0x06001DB2 RID: 7602 RVA: 0x000A0AC4 File Offset: 0x0009ECC4
	public void ApplyTeamModelAndColor(bool blueTeam, bool orangeTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (shouldOverrideColor)
		{
			this.teamColor = overrideColor;
		}
		else
		{
			this.teamColor = (blueTeam ? this.blueColor : (orangeTeam ? this.orangeColor : this.defaultColor));
		}
		this.blueBall.enabled = blueTeam;
		this.orangeBall.enabled = orangeTeam;
		this.defaultBall.enabled = !blueTeam && !orangeTeam;
		this.teamRenderer = (blueTeam ? this.blueBall : (orangeTeam ? this.orangeBall : this.defaultBall));
		this.ApplyColor(this.teamRenderer, (this.colorizeBalls || shouldOverrideColor) ? this.teamColor : Color.white);
	}

	// Token: 0x06001DB3 RID: 7603 RVA: 0x000A0B72 File Offset: 0x0009ED72
	protected void OnEnable()
	{
		this.timeCreated = 0f;
		this.particleLaunched = false;
		SlingshotProjectileManager.RegisterSP(this);
	}

	// Token: 0x06001DB4 RID: 7604 RVA: 0x000A0B8C File Offset: 0x0009ED8C
	protected void OnDisable()
	{
		this.particleLaunched = false;
		SlingshotProjectileManager.UnregisterSP(this);
	}

	// Token: 0x06001DB5 RID: 7605 RVA: 0x000A0B9C File Offset: 0x0009ED9C
	public void InvokeUpdate()
	{
		if (this.particleLaunched || this.dontDestroyOnHit)
		{
			if (Time.time > this.timeCreated + this.GetRemainingLifeTime())
			{
				this.DestroyAfterRelease();
			}
			if (this.faceDirectionOfTravel)
			{
				Transform transform = base.transform;
				Vector3 position = transform.position;
				Vector3 vector = position - this.previousPosition;
				transform.rotation = ((vector.sqrMagnitude > 0f) ? Quaternion.LookRotation(vector) : transform.rotation);
				this.previousPosition = position;
			}
		}
		if (this.dontDestroyOnHit)
		{
			this.SettleProjectile();
		}
	}

	// Token: 0x06001DB6 RID: 7606 RVA: 0x000A0C2D File Offset: 0x0009EE2D
	public void DestroyAfterRelease()
	{
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, base.transform.position, Vector3.up);
		this.Deactivate();
	}

	// Token: 0x06001DB7 RID: 7607 RVA: 0x000A0C51 File Offset: 0x0009EE51
	public float GetRemainingLifeTime()
	{
		return this.remainingLifeTime;
	}

	// Token: 0x06001DB8 RID: 7608 RVA: 0x000A0C59 File Offset: 0x0009EE59
	public void UpdateRemainingLifeTime(float newLifeTime)
	{
		this.remainingLifeTime = newLifeTime;
	}

	// Token: 0x06001DB9 RID: 7609 RVA: 0x000A0C64 File Offset: 0x0009EE64
	public float GetDistanceTraveled()
	{
		return (base.transform.position - this.launchPosition).magnitude;
	}

	// Token: 0x06001DBA RID: 7610 RVA: 0x000A0C90 File Offset: 0x0009EE90
	private void SettleProjectile()
	{
		if (!this.isSettled)
		{
			int value = this.floorLayerMask.value;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 0.1f, value, QueryTriggerInteraction.Ignore) && Vector3.Angle(raycastHit.normal, Vector3.up) < 40f)
			{
				if (this.forceComponent)
				{
					this.forceComponent.force = Vector3.zero;
				}
				this.projectileRigidbody.angularVelocity = Vector3.zero;
				this.projectileRigidbody.linearVelocity = Vector3.zero;
				this.projectileRigidbody.isKinematic = true;
				base.transform.position = raycastHit.point + Vector3.up * this.placementOffset;
				this.isSettled = true;
				return;
			}
		}
		else if (this.keepRotationUpright)
		{
			Quaternion quaternion = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.up, Vector3.up).normalized, Vector3.up);
			base.transform.rotation = quaternion;
		}
	}

	// Token: 0x06001DBB RID: 7611 RVA: 0x000A0DA8 File Offset: 0x0009EFA8
	protected void OnCollisionEnter(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		if (this.dontDestroyOnHit)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.collider.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeHit(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.CheckForAOEKnockback(contact.point, collision.relativeVelocity.magnitude);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
		if (onImpact != null)
		{
			onImpact(this, contact.point, null);
		}
		this.Deactivate();
	}

	// Token: 0x06001DBC RID: 7612 RVA: 0x000A0E40 File Offset: 0x0009F040
	protected void OnCollisionStay(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		if (this.dontDestroyOnHit)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeCollisionStay(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.CheckForAOEKnockback(contact.point, collision.relativeVelocity.magnitude);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
		if (onImpact != null)
		{
			onImpact(this, contact.point, null);
		}
		this.Deactivate();
	}

	// Token: 0x06001DBD RID: 7613 RVA: 0x000A0ED4 File Offset: 0x0009F0D4
	protected void OnTriggerExit(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (other.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeTriggerExit(this, other);
		}
	}

	// Token: 0x06001DBE RID: 7614 RVA: 0x000A0F04 File Offset: 0x0009F104
	protected void OnTriggerEnter(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (other.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeTriggerEnter(this, other);
		}
		if (this.projectileOwner == NetworkSystem.Instance.LocalPlayer)
		{
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			GorillaPaintbrawlManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaSlingshotCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			NetPlayer netPlayer = ((componentInParent != null) ? componentInParent.creator : null);
			if (netPlayer == null)
			{
				return;
			}
			SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, base.transform.position, netPlayer);
			}
			if (NetworkSystem.Instance.LocalPlayer == netPlayer)
			{
				return;
			}
			if (component && !component.LocalCanHit(NetworkSystem.Instance.LocalPlayer, netPlayer))
			{
				return;
			}
			if (component && GameMode.ActiveNetworkHandler)
			{
				GameMode.ActiveNetworkHandler.SendRPC("RPC_ReportSlingshotHit", false, new object[]
				{
					(netPlayer as PunNetPlayer).PlayerRef,
					base.transform.position,
					this.myProjectileCount
				});
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
			if (this.m_sendNetworkedImpact)
			{
				RoomSystem.SendImpactEffect(base.transform.position, this.teamColor.r, this.teamColor.g, this.teamColor.b, this.teamColor.a, this.myProjectileCount);
			}
			this.Deactivate();
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		VRRig vrrig;
		if (attachedRigidbody.IsNotNull() && attachedRigidbody.gameObject.TryGetComponent<VRRig>(out vrrig))
		{
			UnityEvent<VRRig> onHitPlayer = this.OnHitPlayer;
			if (onHitPlayer == null)
			{
				return;
			}
			onHitPlayer.Invoke(vrrig);
		}
	}

	// Token: 0x06001DBF RID: 7615 RVA: 0x000A10D1 File Offset: 0x0009F2D1
	private void ApplyColor(Renderer rend, Color color)
	{
		if (!rend)
		{
			return;
		}
		this.matPropBlock.SetColor(ShaderProps._BaseColor, color);
		this.matPropBlock.SetColor(ShaderProps._Color, color);
		rend.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x04002802 RID: 10242
	public NetPlayer projectileOwner;

	// Token: 0x04002803 RID: 10243
	[Tooltip("Rotates to point along the Y axis after spawn.")]
	public GameObject surfaceImpactEffectPrefab;

	// Token: 0x04002804 RID: 10244
	[Tooltip("if left empty, the default player impact that is set in Room System Setting will be played")]
	public GameObject playerImpactEffectPrefab;

	// Token: 0x04002805 RID: 10245
	[Tooltip("Distance from the surface that the particle should spawn.")]
	[SerializeField]
	private float impactEffectOffset;

	// Token: 0x04002806 RID: 10246
	[SerializeField]
	private SoundBankPlayer launchSoundBankPlayer;

	// Token: 0x04002807 RID: 10247
	[SerializeField]
	private bool dontDestroyOnHit;

	// Token: 0x04002808 RID: 10248
	[SerializeField]
	private LayerMask floorLayerMask;

	// Token: 0x04002809 RID: 10249
	[SerializeField]
	private float placementOffset = 0.01f;

	// Token: 0x0400280A RID: 10250
	[SerializeField]
	private bool keepRotationUpright = true;

	// Token: 0x0400280B RID: 10251
	public float lifeTime = 20f;

	// Token: 0x0400280C RID: 10252
	public float gravityMultiplier = 1f;

	// Token: 0x0400280D RID: 10253
	public bool useForwardForce;

	// Token: 0x0400280E RID: 10254
	public float forwardForceMultiplier = 0.1f;

	// Token: 0x0400280F RID: 10255
	public Color defaultColor = Color.white;

	// Token: 0x04002810 RID: 10256
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x04002811 RID: 10257
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x04002812 RID: 10258
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer defaultBall;

	// Token: 0x04002813 RID: 10259
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer orangeBall;

	// Token: 0x04002814 RID: 10260
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer blueBall;

	// Token: 0x04002815 RID: 10261
	public bool colorizeBalls;

	// Token: 0x04002816 RID: 10262
	public bool faceDirectionOfTravel = true;

	// Token: 0x04002817 RID: 10263
	private bool particleLaunched;

	// Token: 0x04002818 RID: 10264
	private float timeCreated;

	// Token: 0x0400281A RID: 10266
	private Rigidbody projectileRigidbody;

	// Token: 0x0400281B RID: 10267
	private Color teamColor = Color.white;

	// Token: 0x0400281C RID: 10268
	private Renderer teamRenderer;

	// Token: 0x0400281D RID: 10269
	public int myProjectileCount;

	// Token: 0x0400281E RID: 10270
	private float initialScale;

	// Token: 0x0400281F RID: 10271
	private Vector3 previousPosition;

	// Token: 0x04002820 RID: 10272
	[HideInInspector]
	public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

	// Token: 0x04002821 RID: 10273
	[HideInInspector]
	public float? impactSoundVolumeOverride;

	// Token: 0x04002822 RID: 10274
	[HideInInspector]
	public float? impactSoundPitchOverride;

	// Token: 0x04002823 RID: 10275
	[HideInInspector]
	public float impactEffectScaleMultiplier = 1f;

	// Token: 0x04002824 RID: 10276
	private ConstantForce forceComponent;

	// Token: 0x04002825 RID: 10277
	public bool m_sendNetworkedImpact = true;

	// Token: 0x04002827 RID: 10279
	public UnityEvent<NetPlayer> OnLaunch;

	// Token: 0x04002828 RID: 10280
	public UnityEvent<Vector3> OnImapctEvent;

	// Token: 0x04002829 RID: 10281
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x0400282A RID: 10282
	private SpawnWorldEffects spawnWorldEffects;

	// Token: 0x0400282B RID: 10283
	public UnityEvent<VRRig> OnHitPlayer;

	// Token: 0x0400282C RID: 10284
	private float remainingLifeTime;

	// Token: 0x0400282D RID: 10285
	private bool isSettled;

	// Token: 0x0400282E RID: 10286
	private float distanceTraveled;

	// Token: 0x0400282F RID: 10287
	private MonkeGravityController gravityController;

	// Token: 0x020004BF RID: 1215
	[Serializable]
	public struct AOEKnockbackConfig
	{
		// Token: 0x04002830 RID: 10288
		public bool applyAOEKnockback;

		// Token: 0x04002831 RID: 10289
		[Tooltip("Full knockback velocity is imparted within the inner radius")]
		public float aeoInnerRadius;

		// Token: 0x04002832 RID: 10290
		[Tooltip("Partial knockback velocity is imparted between the inner and outer radius")]
		public float aeoOuterRadius;

		// Token: 0x04002833 RID: 10291
		public float knockbackVelocity;

		// Token: 0x04002834 RID: 10292
		[Tooltip("The required impact velocity to achieve full knockback velocity")]
		public float impactVelocityThreshold;

		// Token: 0x04002835 RID: 10293
		[SerializeField]
		public PlayerEffect playerProximityEffect;
	}

	// Token: 0x020004C0 RID: 1216
	// (Invoke) Token: 0x06001DC2 RID: 7618
	public delegate void ProjectileImpactEvent(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
}
