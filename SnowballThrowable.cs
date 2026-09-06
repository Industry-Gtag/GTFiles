using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200020A RID: 522
public class SnowballThrowable : HoldableObject, IHeldItem
{
	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000DBC RID: 3516 RVA: 0x0004B585 File Offset: 0x00049785
	// (set) Token: 0x06000DBD RID: 3517 RVA: 0x0004B58D File Offset: 0x0004978D
	public XformOffset SpawnOffset
	{
		get
		{
			return this.spawnOffset;
		}
		set
		{
			this.spawnOffset = value;
		}
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000DBE RID: 3518 RVA: 0x0004B598 File Offset: 0x00049798
	internal int ProjectileHash
	{
		get
		{
			return PoolUtils.GameObjHashCode((this.randomModelSelection && this.localModels != null && this.randModelIndex >= 0 && this.randModelIndex <= this.localModels.Count && this.localModels[this.randModelIndex] != null) ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab);
		}
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x0004B610 File Offset: 0x00049810
	protected virtual void Awake()
	{
		if (this.awakeHasBeenCalled)
		{
			return;
		}
		this.awakeHasBeenCalled = true;
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = this.targetRig != null && this.targetRig.isOfflineVRRig;
		this.renderers = base.GetComponentsInChildren<Renderer>();
		this.randModelIndex = -1;
		foreach (RandomProjectileThrowable randomProjectileThrowable in this.localModels)
		{
			if (randomProjectileThrowable != null)
			{
				RandomProjectileThrowable randomProjectileThrowable2 = randomProjectileThrowable;
				randomProjectileThrowable2.OnDestroyRandomProjectile = (UnityAction<bool>)Delegate.Combine(randomProjectileThrowable2.OnDestroyRandomProjectile, new UnityAction<bool>(this.HandleOnDestroyRandomProjectile));
			}
		}
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x0004B6DC File Offset: 0x000498DC
	public bool IsMine()
	{
		return this.targetRig != null && this.targetRig.isOfflineVRRig;
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x0004B6F9 File Offset: 0x000498F9
	bool IHeldItem.InLeftHand()
	{
		return this.isLeftHanded && base.gameObject.activeSelf;
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x0004B710 File Offset: 0x00049910
	bool IHeldItem.InHand()
	{
		return base.gameObject.activeSelf;
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x0004B71D File Offset: 0x0004991D
	bool IHeldItem.IsMyItem()
	{
		return this.IsMine();
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x0004B728 File Offset: 0x00049928
	public virtual void OnEnable()
	{
		if (this.targetRig == null)
		{
			Debug.LogError("SnowballThrowable: targetRig is null! Deactivating.");
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.targetRig.isOfflineVRRig)
		{
			if (this.targetRig.netView != null && this.targetRig.netView.IsMine)
			{
				base.gameObject.SetActive(false);
				return;
			}
			Color32 throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
			this.ApplyColor(throwableProjectileColor);
			if (this.randomModelSelection)
			{
				foreach (RandomProjectileThrowable randomProjectileThrowable in this.localModels)
				{
					randomProjectileThrowable.gameObject.SetActive(false);
				}
				this.randModelIndex = this.targetRig.GetRandomThrowableModelIndex();
				this.EnableRandomModel(this.randModelIndex, true);
			}
		}
		this.AnchorToHand();
		this.OnEnableHasBeenCalled = true;
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnDisable()
	{
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected new virtual void OnDestroy()
	{
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x0004B838 File Offset: 0x00049A38
	public void SetSnowballActiveLocal(bool enabled)
	{
		if (!this.awakeHasBeenCalled)
		{
			this.Awake();
		}
		if (!this.OnEnableHasBeenCalled)
		{
			this.OnEnable();
		}
		if (this.isLeftHanded)
		{
			this.targetRig.LeftThrowableProjectileIndex = (enabled ? this.throwableMakerIndex : (-1));
		}
		else
		{
			this.targetRig.RightThrowableProjectileIndex = (enabled ? this.throwableMakerIndex : (-1));
		}
		bool flag = !base.gameObject.activeSelf && enabled;
		base.gameObject.SetActive(enabled);
		if (flag && this.pickupSoundBankPlayer != null)
		{
			this.pickupSoundBankPlayer.Play();
			if (this.playHapticsOnPickup)
			{
				GorillaTagger.Instance.StartVibration(this.isLeftHanded, (this.pickupHapticStrength > 0f) ? this.pickupHapticStrength : GorillaTagger.Instance.tapHapticStrength, (this.pickupHapticDuration > 0f) ? this.pickupHapticDuration : GorillaTagger.Instance.tapHapticDuration);
			}
		}
		if (this.randomModelSelection)
		{
			if (enabled)
			{
				this.EnableRandomModel(this.GetRandomModelIndex(), true);
			}
			else
			{
				this.EnableRandomModel(this.randModelIndex, false);
			}
			this.targetRig.SetRandomThrowableModelIndex(this.randModelIndex);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(enabled ? this : null, this.isLeftHanded);
		if (this.randomizeColor)
		{
			Color color = (enabled ? GTColor.RandomHSV(this.randomColorHSVRanges) : Color.white);
			this.targetRig.SetThrowableProjectileColor(this.isLeftHanded, color);
			this.ApplyColor(color);
		}
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x0004B9B4 File Offset: 0x00049BB4
	private int GetRandomModelIndex()
	{
		if (this.localModels.Count == 0)
		{
			return -1;
		}
		this.randModelIndex = Random.Range(0, this.localModels.Count);
		if ((float)Random.Range(1, 100) <= this.localModels[this.randModelIndex].spawnChance * 100f)
		{
			return this.randModelIndex;
		}
		return this.GetRandomModelIndex();
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x0004BA1C File Offset: 0x00049C1C
	private void EnableRandomModel(int index, bool enable)
	{
		if (this.randModelIndex >= 0 && this.randModelIndex < this.localModels.Count)
		{
			this.localModels[this.randModelIndex].gameObject.SetActive(enable);
			if (enable && this.localModels[this.randModelIndex].autoDestroyAfterSeconds > 0f)
			{
				this.destroyTimer = 0f;
			}
			return;
		}
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x0004BA90 File Offset: 0x00049C90
	protected virtual void LateUpdateLocal()
	{
		if (this.randomModelSelection && this.randModelIndex > -1 && this.localModels[this.randModelIndex].ForceDestroy)
		{
			this.localModels[this.randModelIndex].ForceDestroy = false;
			if (this.localModels[this.randModelIndex].gameObject.activeSelf)
			{
				this.PerformSnowballThrowAuthority();
			}
		}
		if (this.randomModelSelection && this.randModelIndex > -1 && this.localModels[this.randModelIndex].autoDestroyAfterSeconds > 0f)
		{
			this.destroyTimer += Time.deltaTime;
			if (this.destroyTimer > this.localModels[this.randModelIndex].autoDestroyAfterSeconds)
			{
				if (this.localModels[this.randModelIndex].gameObject.activeSelf)
				{
					this.PerformSnowballThrowAuthority();
				}
				this.destroyTimer = -1f;
			}
		}
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected void LateUpdateReplicated()
	{
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected void LateUpdateShared()
	{
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x0004BB8F File Offset: 0x00049D8F
	private Transform Anchor()
	{
		return base.transform.parent;
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x0004BB9C File Offset: 0x00049D9C
	private void AnchorToHand()
	{
		BodyDockPositions myBodyDockPositions = this.targetRig.myBodyDockPositions;
		Transform transform = this.Anchor();
		if (this.isLeftHanded)
		{
			transform.parent = myBodyDockPositions.leftHandTransform;
		}
		else
		{
			transform.parent = myBodyDockPositions.rightHandTransform;
		}
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		base.transform.localPosition = this.spawnOffset.pos;
		base.transform.localRotation = this.spawnOffset.rot;
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x0004BC20 File Offset: 0x00049E20
	protected void LateUpdate()
	{
		if (this.IsMine())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x0004BC3E File Offset: 0x00049E3E
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.OnSnowballRelease();
		return true;
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x0004BC53 File Offset: 0x00049E53
	protected virtual void OnSnowballRelease()
	{
		this.PerformSnowballThrowAuthority();
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x0004BC5C File Offset: 0x00049E5C
	protected virtual void PerformSnowballThrowAuthority()
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Rigidbody component = GorillaTagger.Instance.GetComponent<Rigidbody>();
		if (component != null)
		{
			vector = component.linearVelocity;
		}
		Vector3 vector2 = this.velocityEstimator.linearVelocity - vector;
		float magnitude = vector2.magnitude;
		if (magnitude > 0.001f)
		{
			float num = Mathf.Clamp(magnitude * this.linSpeedMultiplier, 0f, this.maxLinSpeed);
			vector2 *= num / magnitude;
		}
		Vector3 vector3 = vector2 + vector;
		Color32 throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		SlingshotProjectile slingshotProjectile = this.LaunchSnowballLocal(position, vector3, x, this.randomizeColor, throwableProjectileColor);
		this.SetSnowballActiveLocal(false);
		if (this.randModelIndex > -1 && this.randModelIndex < this.localModels.Count)
		{
			if (this.localModels[this.randModelIndex].ForceDestroy || this.localModels[this.randModelIndex].destroyAfterRelease)
			{
				slingshotProjectile.DestroyAfterRelease();
			}
			else if (this.localModels[this.randModelIndex].moveOverPassedLifeTime)
			{
				float num2 = Time.time - this.localModels[this.randModelIndex].TimeEnabled;
				float remainingLifeTime = slingshotProjectile.GetRemainingLifeTime();
				if (remainingLifeTime > num2)
				{
					float num3 = remainingLifeTime - num2;
					slingshotProjectile.UpdateRemainingLifeTime(num3);
				}
				else
				{
					slingshotProjectile.UpdateRemainingLifeTime(0f);
				}
			}
		}
		if (NetworkSystem.Instance.InRoom)
		{
			RoomSystem.SendLaunchProjectile(position, vector3, this.isLeftHanded ? RoomSystem.ProjectileSource.LeftHand : RoomSystem.ProjectileSource.RightHand, slingshotProjectile.myProjectileCount, this.randomizeColor, throwableProjectileColor.r, throwableProjectileColor.g, throwableProjectileColor.b, throwableProjectileColor.a);
		}
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x0004BE64 File Offset: 0x0004A064
	protected virtual SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale, bool randomColour, Color colour)
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
		int num = ProjectileTracker.AddAndIncrementLocalProjectile(component, velocity, location, scale);
		component.Launch(location, velocity, NetworkSystem.Instance.LocalPlayer, false, false, num, scale, randomColour, colour);
		GorillaTagger.Instance.StartVibration(this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		component.OnImpact += this.OnProjectileImpact;
		return component;
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x0004BF38 File Offset: 0x0004A138
	protected virtual SlingshotProjectile SpawnProjectile()
	{
		return ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x0004BF70 File Offset: 0x0004A170
	protected virtual void OnProjectileImpact(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer)
	{
		if (hitPlayer != null)
		{
			ScienceExperimentManager instance = ScienceExperimentManager.instance;
			if (instance != null && this.projectilePrefab != null && this.projectilePrefab == instance.waterBalloonPrefab)
			{
				instance.OnWaterBalloonHitPlayer(hitPlayer);
			}
			if (hitPlayer.IsLocal)
			{
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			}
		}
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x0004C01C File Offset: 0x0004A21C
	private void ApplyColor(Color newColor)
	{
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer)
			{
				foreach (Material material in renderer.materials)
				{
					if (!(material == null))
					{
						if (material.HasProperty(ShaderProps._BaseColor))
						{
							material.SetColor(ShaderProps._BaseColor, newColor);
						}
						if (material.HasProperty(ShaderProps._Color))
						{
							material.SetColor(ShaderProps._Color, newColor);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x0004C0A9 File Offset: 0x0004A2A9
	public override void DropItemCleanup()
	{
		if (base.gameObject.activeSelf)
		{
			this.OnSnowballRelease();
		}
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x0004C0BE File Offset: 0x0004A2BE
	private void HandleOnDestroyRandomProjectile(bool enable)
	{
		this.SetSnowballActiveLocal(enable);
	}

	// Token: 0x04001061 RID: 4193
	[GorillaSoundLookup]
	public List<int> matDataIndexes = new List<int> { 32 };

	// Token: 0x04001062 RID: 4194
	[Tooltip("prefab to spawn from global object pools when thrown")]
	public GameObject projectilePrefab;

	// Token: 0x04001063 RID: 4195
	public SoundBankPlayer pickupSoundBankPlayer;

	// Token: 0x04001064 RID: 4196
	[Tooltip("If true, plays a haptic pulse on the grabbing hand when the snowball is picked up.")]
	public bool playHapticsOnPickup = true;

	// Token: 0x04001065 RID: 4197
	[Tooltip("Strength of the haptic pulse on pickup. Defaults to tapHapticStrength if left at 0.")]
	public float pickupHapticStrength;

	// Token: 0x04001066 RID: 4198
	[Tooltip("Duration of the haptic pulse on pickup. Defaults to tapHapticDuration if left at 0.")]
	public float pickupHapticDuration;

	// Token: 0x04001067 RID: 4199
	public bool isLeftHanded;

	// Token: 0x04001068 RID: 4200
	[Tooltip("This needs to match the index of the projectilePrefab on the Local Gorilla Player's BodyDockPositions LeftHandThrowables or RightHandThrowables list\nCheck the array in play mode to find the index")]
	public int throwableMakerIndex;

	// Token: 0x04001069 RID: 4201
	[Tooltip("Multiplier is applied to hand speed to get launch speed of the projectile")]
	public float linSpeedMultiplier = 1f;

	// Token: 0x0400106A RID: 4202
	[Tooltip("Maximum launch speed of the projectile")]
	public float maxLinSpeed = 12f;

	// Token: 0x0400106B RID: 4203
	[Space]
	[FormerlySerializedAs("shouldColorize")]
	public bool randomizeColor;

	// Token: 0x0400106C RID: 4204
	public GTColor.HSVRanges randomColorHSVRanges = new GTColor.HSVRanges(0f, 1f, 0.7f, 1f, 1f, 1f);

	// Token: 0x0400106D RID: 4205
	[Tooltip("Check this part only if we want to randomize the prefab meshes and projectile")]
	public bool randomModelSelection;

	// Token: 0x0400106E RID: 4206
	public List<RandomProjectileThrowable> localModels;

	// Token: 0x0400106F RID: 4207
	[Tooltip("projectile identifier sent out by the PlayerGameEvents.LaunchedProjectile event. Uses prefab name if empty")]
	public string throwEventName;

	// Token: 0x04001070 RID: 4208
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001071 RID: 4209
	protected VRRig targetRig;

	// Token: 0x04001072 RID: 4210
	protected bool isOfflineRig;

	// Token: 0x04001073 RID: 4211
	private bool awakeHasBeenCalled;

	// Token: 0x04001074 RID: 4212
	private bool OnEnableHasBeenCalled;

	// Token: 0x04001075 RID: 4213
	private Renderer[] renderers;

	// Token: 0x04001076 RID: 4214
	protected int randModelIndex;

	// Token: 0x04001077 RID: 4215
	private float destroyTimer = -1f;

	// Token: 0x04001078 RID: 4216
	private XformOffset spawnOffset;
}
