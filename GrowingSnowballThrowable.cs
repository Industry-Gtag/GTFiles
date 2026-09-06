using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000204 RID: 516
public class GrowingSnowballThrowable : SnowballThrowable
{
	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000D89 RID: 3465 RVA: 0x00049F3E File Offset: 0x0004813E
	public int SizeLevel
	{
		get
		{
			return this.sizeLevel;
		}
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000D8A RID: 3466 RVA: 0x00049F46 File Offset: 0x00048146
	public int MaxSizeLevel
	{
		get
		{
			return Mathf.Max(this.snowballSizeLevels.Count - 1, 0);
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000D8B RID: 3467 RVA: 0x00049F5B File Offset: 0x0004815B
	public static bool IsAOEEnabled
	{
		get
		{
			return GrowingSnowballThrowable.ForceAOEEnabled || GrowingSnowballThrowable.k_useAOE;
		}
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x00049F6B File Offset: 0x0004816B
	public static void NotifyEnableKnockbackIntent(SnowballKnockbackEnabler source)
	{
		if (source.IsNotNull() && !GrowingSnowballThrowable.s_KnockSources.Contains(source))
		{
			GrowingSnowballThrowable.s_KnockSources.Add(source);
		}
		if (GrowingSnowballThrowable.s_KnockSources.Count > 0)
		{
			GrowingSnowballThrowable.k_useAOE = true;
		}
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x00049FA0 File Offset: 0x000481A0
	public static void NotifyDisableKnockbackIntent(SnowballKnockbackEnabler source)
	{
		if (source.IsNotNull() && GrowingSnowballThrowable.s_KnockSources.Remove(source) && GrowingSnowballThrowable.s_KnockSources.Count < 1)
		{
			GrowingSnowballThrowable.k_useAOE = false;
		}
	}

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000D8E RID: 3470 RVA: 0x00049FCC File Offset: 0x000481CC
	public float CurrentSnowballRadius
	{
		get
		{
			if (this.snowballSizeLevels.Count > 0 && this.sizeLevel > -1 && this.sizeLevel < this.snowballSizeLevels.Count)
			{
				return this.snowballSizeLevels[this.sizeLevel].snowballScale * this.modelRadius * base.transform.lossyScale.x;
			}
			return this.modelRadius * base.transform.lossyScale.x;
		}
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x0004A04C File Offset: 0x0004824C
	protected override void Awake()
	{
		base.Awake();
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted += this.StartedMultiplayerSession;
		}
		else
		{
			Debug.LogError("NetworkSystem.Instance was null in SnowballThrowable Awake");
		}
		VRRigCache.OnRigActivated += this.VRRigActivated;
		VRRigCache.OnRigDeactivated += this.VRRigDeactivated;
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x0004A0BC File Offset: 0x000482BC
	public override void OnEnable()
	{
		base.OnEnable();
		this.snowballModelParentTransform.localPosition = this.modelParentOffset;
		this.snowballModelTransform.localPosition = this.modelOffset;
		this.otherHandSnowball = (this.isLeftHanded ? (EquipmentInteractor.instance.rightHandHeldEquipment as GrowingSnowballThrowable) : (EquipmentInteractor.instance.leftHandHeldEquipment as GrowingSnowballThrowable));
		if (Time.time > this.maintainSizeLevelUntilLocalTime)
		{
			this.SetSizeLevelLocal(0);
		}
		this.CreatePhotonEventsIfNull();
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x0004A13D File Offset: 0x0004833D
	protected override void OnDestroy()
	{
		this.DestroyPhotonEvents();
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x0004A148 File Offset: 0x00048348
	private void VRRigActivated(RigContainer rigContainer)
	{
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = this.targetRig != null && this.targetRig.isOfflineVRRig;
		if (rigContainer.Rig == this.targetRig)
		{
			this.CreatePhotonEventsIfNull();
		}
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x0004A19D File Offset: 0x0004839D
	private void VRRigDeactivated(RigContainer rigContainer)
	{
		if (rigContainer.Rig == this.targetRig)
		{
			this.DestroyPhotonEvents();
		}
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x0004A1B8 File Offset: 0x000483B8
	private void StartedMultiplayerSession()
	{
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = this.targetRig != null && this.targetRig.isOfflineVRRig;
		if (this.isOfflineRig)
		{
			this.DestroyPhotonEvents();
			this.CreatePhotonEventsIfNull();
		}
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x0004A208 File Offset: 0x00048408
	private void CreatePhotonEventsIfNull()
	{
		if (this.targetRig == null)
		{
			this.targetRig = base.GetComponentInParent<VRRig>(true);
			this.isOfflineRig = this.targetRig != null && this.targetRig.isOfflineVRRig;
		}
		if (this.targetRig == null || this.targetRig.netView == null)
		{
			return;
		}
		if (this.changeSizeEvent == null)
		{
			"SnowballThrowable" + base.gameObject.name + (this.isLeftHanded ? "ChangeSizeEventLeft" : "ChangeSizeEventRight") + this.targetRig.netView.ViewID.ToString();
			int num = StaticHash.Compute("SnowballThrowable", base.gameObject.name, this.isLeftHanded ? "ChangeSizeEventLeft" : "ChangeSizeEventRight", this.targetRig.netView.ViewID.ToString());
			this.changeSizeEvent = new PhotonEvent(num);
			this.changeSizeEvent.reliable = true;
			this.changeSizeEvent += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ChangeSizeEventReceiver);
		}
		if (this.snowballThrowEvent == null)
		{
			"SnowballThrowable" + base.gameObject.name + (this.isLeftHanded ? "SnowballThrowEventLeft" : "SnowballThrowEventRight") + this.targetRig.netView.ViewID.ToString();
			int num2 = StaticHash.Compute("SnowballThrowable", base.gameObject.name, this.isLeftHanded ? "SnowballThrowEventLeft" : "SnowballThrowEventRight", this.targetRig.netView.ViewID.ToString());
			this.snowballThrowEvent = new PhotonEvent(num2);
			this.snowballThrowEvent.reliable = true;
			this.snowballThrowEvent += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SnowballThrowEventReceiver);
		}
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x0004A408 File Offset: 0x00048608
	private void DestroyPhotonEvents()
	{
		if (this.changeSizeEvent != null)
		{
			this.changeSizeEvent -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ChangeSizeEventReceiver);
			this.changeSizeEvent.Dispose();
			this.changeSizeEvent = null;
		}
		if (this.snowballThrowEvent != null)
		{
			this.snowballThrowEvent -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SnowballThrowEventReceiver);
			this.snowballThrowEvent.Dispose();
			this.snowballThrowEvent = null;
		}
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x0004A48F File Offset: 0x0004868F
	public void IncreaseSize(int increase)
	{
		this.SetSizeLevelAuthority(this.sizeLevel + increase);
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x0004A4A0 File Offset: 0x000486A0
	private void SetSizeLevelAuthority(int sizeLevel)
	{
		if (this.targetRig != null && this.targetRig.creator != null && this.targetRig.creator.IsLocal)
		{
			int validSizeLevel = this.GetValidSizeLevel(sizeLevel);
			if (validSizeLevel > this.sizeLevel)
			{
				this.sizeIncreaseSoundBankPlayer.Play();
			}
			this.SetSizeLevelLocal(validSizeLevel);
			PhotonEvent photonEvent = this.changeSizeEvent;
			if (photonEvent == null)
			{
				return;
			}
			photonEvent.RaiseOthers(new object[] { validSizeLevel });
		}
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x0004A51C File Offset: 0x0004871C
	private int GetValidSizeLevel(int inputSizeLevel)
	{
		int num = Mathf.Max(this.snowballSizeLevels.Count - 1, 0);
		return Mathf.Clamp(inputSizeLevel, 0, num);
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x0004A548 File Offset: 0x00048748
	private void SetSizeLevelLocal(int sizeLevel)
	{
		int validSizeLevel = this.GetValidSizeLevel(sizeLevel);
		if (validSizeLevel >= 0 && validSizeLevel != this.sizeLevel)
		{
			this.sizeLevel = validSizeLevel;
			this.snowballModelParentTransform.localScale = Vector3.one * this.snowballSizeLevels[this.sizeLevel].snowballScale;
		}
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x0004A59C File Offset: 0x0004879C
	private void ChangeSizeEventReceiver(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != receiver)
		{
			return;
		}
		if (args == null || args.Length < 1)
		{
			return;
		}
		int num = ((this.targetRig != null && this.targetRig.gameObject.activeInHierarchy && this.targetRig.netView != null && this.targetRig.netView.Owner != null) ? this.targetRig.netView.Owner.ActorNumber : (-1));
		if (info.senderID != num)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "ChangeSizeEventReceiver");
		int num2 = (int)args[0];
		if (this.GetValidSizeLevel(num2) > this.sizeLevel && this.sizeIncreaseSoundBankPlayer.gameObject.activeInHierarchy)
		{
			this.sizeIncreaseSoundBankPlayer.Play();
		}
		this.SetSizeLevelLocal(num2);
		if (!base.gameObject.activeSelf)
		{
			this.maintainSizeLevelUntilLocalTime = Time.time + 0.1f;
		}
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x0004A688 File Offset: 0x00048888
	private void SnowballThrowEventReceiver(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != receiver)
		{
			return;
		}
		if (args == null || args.Length < 3)
		{
			return;
		}
		if (this.targetRig.IsNull() || !this.targetRig.gameObject.activeSelf)
		{
			return;
		}
		NetPlayer creator = this.targetRig.creator;
		if (info.senderID != this.targetRig.creator.ActorNumber)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "SnowballThrowEventReceiver");
		if (!FXSystem.CheckCallSpam(this.targetRig.fxSettings, 4, info.SentServerTime))
		{
			return;
		}
		object obj = args[0];
		if (obj is Vector3)
		{
			Vector3 vector = (Vector3)obj;
			obj = args[1];
			if (obj is Vector3)
			{
				Vector3 vector2 = (Vector3)obj;
				obj = args[2];
				if (obj is int)
				{
					int num = (int)obj;
					Vector3 vector3 = this.targetRig.ClampVelocityRelativeToPlayerSafe(vector2, 50f, 100f);
					float x = this.snowballModelTransform.lossyScale.x;
					float num2 = 10000f;
					if (!(in vector).IsValid(in num2) || !this.targetRig.IsPositionInRange(vector, 4f))
					{
						return;
					}
					this.LaunchSnowballRemote(vector, vector3, x, num, info);
					return;
				}
			}
		}
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x0004A7B4 File Offset: 0x000489B4
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (GrowingSnowballThrowable.twoHandedSnowballGrowing)
		{
			if (this.otherHandSnowball != null && this.otherHandSnowball.isActiveAndEnabled)
			{
				IHoldableObject holdableObject = (this.isLeftHanded ? EquipmentInteractor.instance.rightHandHeldEquipment : EquipmentInteractor.instance.leftHandHeldEquipment);
				if (holdableObject != null && this.otherHandSnowball != (GrowingSnowballThrowable)holdableObject)
				{
					this.otherHandSnowball = null;
					return;
				}
				float num = this.otherHandSnowball.CurrentSnowballRadius + this.CurrentSnowballRadius;
				if (this.SizeLevel < this.MaxSizeLevel && this.otherHandSnowball.SizeLevel < this.otherHandSnowball.MaxSizeLevel && (this.otherHandSnowball.snowballModelTransform.position - this.snowballModelTransform.position).sqrMagnitude < num * num)
				{
					int num2 = this.SizeLevel - this.otherHandSnowball.SizeLevel;
					float magnitude = this.velocityEstimator.linearVelocity.magnitude;
					float magnitude2 = this.otherHandSnowball.velocityEstimator.linearVelocity.magnitude;
					bool flag;
					if (Mathf.Abs(magnitude - magnitude2) > this.combineBasedOnSpeedThreshold || num2 == 0)
					{
						flag = magnitude > magnitude2;
					}
					else
					{
						flag = num2 < 0;
					}
					if (flag)
					{
						this.otherHandSnowball.IncreaseSize(this.sizeLevel + 1);
						GorillaTagger.Instance.StartVibration(!this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
						base.SetSnowballActiveLocal(false);
						return;
					}
					this.IncreaseSize(this.otherHandSnowball.SizeLevel + 1);
					GorillaTagger.Instance.StartVibration(this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
					this.otherHandSnowball.SetSnowballActiveLocal(false);
					return;
				}
			}
			else
			{
				this.otherHandSnowball = null;
			}
		}
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x0004A9BA File Offset: 0x00048BBA
	protected override void OnSnowballRelease()
	{
		if (base.isActiveAndEnabled)
		{
			this.PerformSnowballThrowAuthority();
		}
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x0004A9CC File Offset: 0x00048BCC
	protected override void PerformSnowballThrowAuthority()
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
		this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		Transform transform = this.snowballModelTransform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		SlingshotProjectile slingshotProjectile = this.LaunchSnowballLocal(position, vector3, x);
		base.SetSnowballActiveLocal(false);
		if (this.randModelIndex > -1 && this.randModelIndex < this.localModels.Count && this.localModels[this.randModelIndex].destroyAfterRelease)
		{
			slingshotProjectile.DestroyAfterRelease();
		}
		PhotonEvent photonEvent = this.snowballThrowEvent;
		if (photonEvent == null)
		{
			return;
		}
		photonEvent.RaiseOthers(new object[] { position, vector3, slingshotProjectile.myProjectileCount });
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x0004AB30 File Offset: 0x00048D30
	protected virtual SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale)
	{
		return this.LaunchSnowballLocal(location, velocity, scale, false, Color.white);
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x0004AB44 File Offset: 0x00048D44
	protected override SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale, bool randomizeColour, Color colour)
	{
		SlingshotProjectile slingshotProjectile = this.SpawnGrowingSnowball(ref velocity, scale);
		int num = ProjectileTracker.AddAndIncrementLocalProjectile(slingshotProjectile, velocity, location, scale);
		slingshotProjectile.Launch(location, velocity, NetworkSystem.Instance.LocalPlayer, false, false, num, scale, randomizeColour, colour);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		slingshotProjectile.OnImpact += this.OnProjectileImpact;
		return slingshotProjectile;
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x0004ABBB File Offset: 0x00048DBB
	protected virtual SlingshotProjectile LaunchSnowballRemote(Vector3 location, Vector3 velocity, float scale, int index, PhotonMessageInfoWrapped info)
	{
		return this.LaunchSnowballRemote(location, velocity, scale, index, false, Color.white, info);
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x0004ABD0 File Offset: 0x00048DD0
	protected virtual SlingshotProjectile LaunchSnowballRemote(Vector3 location, Vector3 velocity, float scale, int index, bool randomizeColour, Color colour, PhotonMessageInfoWrapped info)
	{
		SlingshotProjectile slingshotProjectile = this.SpawnGrowingSnowball(ref velocity, scale);
		ProjectileTracker.AddRemotePlayerProjectile(info.Sender, slingshotProjectile, index, info.SentServerTime, velocity, location, scale);
		slingshotProjectile.Launch(location, velocity, info.Sender, false, false, index, scale, randomizeColour, Color.white);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		slingshotProjectile.OnImpact += this.OnProjectileImpact;
		return slingshotProjectile;
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x0004AC5C File Offset: 0x00048E5C
	private SlingshotProjectile SpawnGrowingSnowball(ref Vector3 velocity, float scale)
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].projectilePrefab : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
		if (GrowingSnowballThrowable.IsAOEEnabled && this.snowballSizeLevels.Count > 0 && this.sizeLevel >= 0 && this.sizeLevel < this.snowballSizeLevels.Count)
		{
			float num = scale / this.snowballSizeLevels[this.sizeLevel].snowballScale;
			SlingshotProjectile.AOEKnockbackConfig aoeKnockbackConfig = this.snowballSizeLevels[this.sizeLevel].aoeKnockbackConfig;
			aoeKnockbackConfig.aeoInnerRadius *= num;
			aoeKnockbackConfig.aeoOuterRadius *= num;
			aoeKnockbackConfig.knockbackVelocity *= num;
			aoeKnockbackConfig.impactVelocityThreshold *= num;
			velocity *= this.snowballSizeLevels[this.sizeLevel].throwSpeedMultiplier;
			component.gravityMultiplier = this.snowballSizeLevels[this.sizeLevel].gravityMultiplier;
			component.impactEffectScaleMultiplier = this.snowballSizeLevels[this.sizeLevel].impactEffectScale;
			component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(aoeKnockbackConfig);
			component.impactSoundVolumeOverride = new float?(this.snowballSizeLevels[this.sizeLevel].impactSoundVolume);
			component.impactSoundPitchOverride = new float?(this.snowballSizeLevels[this.sizeLevel].impactSoundPitch);
		}
		return component;
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x0004ADEC File Offset: 0x00048FEC
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		SnowballThrowable snowballThrowable;
		if (((this.isLeftHanded && grabbingHand == EquipmentInteractor.instance.rightHand && EquipmentInteractor.instance.rightHandHeldEquipment == null) || (!this.isLeftHanded && grabbingHand == EquipmentInteractor.instance.leftHand && EquipmentInteractor.instance.leftHandHeldEquipment == null)) && (this.isLeftHanded ? SnowballMaker.rightHandInstance : SnowballMaker.leftHandInstance).TryCreateSnowball(this.matDataIndexes[0], out snowballThrowable))
		{
			GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable as GrowingSnowballThrowable;
			if (growingSnowballThrowable != null)
			{
				growingSnowballThrowable.IncreaseSize(this.sizeLevel);
				GorillaTagger.Instance.StartVibration(!this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				base.SetSnowballActiveLocal(false);
			}
		}
	}

	// Token: 0x04001033 RID: 4147
	public Transform snowballModelParentTransform;

	// Token: 0x04001034 RID: 4148
	public Transform snowballModelTransform;

	// Token: 0x04001035 RID: 4149
	public Vector3 modelParentOffset = Vector3.zero;

	// Token: 0x04001036 RID: 4150
	public Vector3 modelOffset = Vector3.zero;

	// Token: 0x04001037 RID: 4151
	public float modelRadius = 0.055f;

	// Token: 0x04001038 RID: 4152
	[Tooltip("Snowballs will combine into the larger snowball unless they are moving faster than this threshold.Then the faster moving snowball will go in to the more stationary hand")]
	public float combineBasedOnSpeedThreshold = 0.5f;

	// Token: 0x04001039 RID: 4153
	public SoundBankPlayer sizeIncreaseSoundBankPlayer;

	// Token: 0x0400103A RID: 4154
	public List<GrowingSnowballThrowable.SizeParameters> snowballSizeLevels = new List<GrowingSnowballThrowable.SizeParameters>();

	// Token: 0x0400103B RID: 4155
	private int sizeLevel;

	// Token: 0x0400103C RID: 4156
	private float maintainSizeLevelUntilLocalTime;

	// Token: 0x0400103D RID: 4157
	private PhotonEvent changeSizeEvent;

	// Token: 0x0400103E RID: 4158
	private PhotonEvent snowballThrowEvent;

	// Token: 0x0400103F RID: 4159
	[HideInInspector]
	public static bool debugDrawAOERange = false;

	// Token: 0x04001040 RID: 4160
	[HideInInspector]
	public static bool twoHandedSnowballGrowing = true;

	// Token: 0x04001041 RID: 4161
	private static bool k_useAOE = false;

	// Token: 0x04001042 RID: 4162
	public static bool ForceAOEEnabled = false;

	// Token: 0x04001043 RID: 4163
	private static readonly List<SnowballKnockbackEnabler> s_KnockSources = new List<SnowballKnockbackEnabler>(3);

	// Token: 0x04001044 RID: 4164
	private Queue<GrowingSnowballThrowable.AOERangeDebugDraw> aoeRangeDebugDrawQueue = new Queue<GrowingSnowballThrowable.AOERangeDebugDraw>();

	// Token: 0x04001045 RID: 4165
	private GrowingSnowballThrowable otherHandSnowball;

	// Token: 0x04001046 RID: 4166
	private float debugDrawAOERangeTime = 1.5f;

	// Token: 0x02000205 RID: 517
	[Serializable]
	public struct SizeParameters
	{
		// Token: 0x04001047 RID: 4167
		public float snowballScale;

		// Token: 0x04001048 RID: 4168
		public float impactEffectScale;

		// Token: 0x04001049 RID: 4169
		public float impactSoundVolume;

		// Token: 0x0400104A RID: 4170
		public float impactSoundPitch;

		// Token: 0x0400104B RID: 4171
		public float throwSpeedMultiplier;

		// Token: 0x0400104C RID: 4172
		public float gravityMultiplier;

		// Token: 0x0400104D RID: 4173
		public SlingshotProjectile.AOEKnockbackConfig aoeKnockbackConfig;
	}

	// Token: 0x02000206 RID: 518
	private struct AOERangeDebugDraw
	{
		// Token: 0x0400104E RID: 4174
		public float impactTime;

		// Token: 0x0400104F RID: 4175
		public Vector3 position;

		// Token: 0x04001050 RID: 4176
		public float innerRadius;

		// Token: 0x04001051 RID: 4177
		public float outerRadius;
	}
}
