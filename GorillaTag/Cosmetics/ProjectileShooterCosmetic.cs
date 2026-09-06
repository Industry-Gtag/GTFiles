using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001356 RID: 4950
	public class ProjectileShooterCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06007C14 RID: 31764 RVA: 0x00288450 File Offset: 0x00286650
		private bool IsMovementShoot()
		{
			return this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold;
		}

		// Token: 0x06007C15 RID: 31765 RVA: 0x0028845B File Offset: 0x0028665B
		private bool IsRigDirection()
		{
			return this.shootDirectionType == ProjectileShooterCosmetic.ShootDirection.LineFromRigToLaunchTransform;
		}

		// Token: 0x17000C22 RID: 3106
		// (get) Token: 0x06007C16 RID: 31766 RVA: 0x00288466 File Offset: 0x00286666
		// (set) Token: 0x06007C17 RID: 31767 RVA: 0x0028846E File Offset: 0x0028666E
		public bool shootingAllowed { get; set; } = true;

		// Token: 0x17000C23 RID: 3107
		// (get) Token: 0x06007C18 RID: 31768 RVA: 0x00288477 File Offset: 0x00286677
		private bool IsCoolingDown
		{
			get
			{
				return this.cooldownRemaining > 0f;
			}
		}

		// Token: 0x06007C19 RID: 31769 RVA: 0x00288488 File Offset: 0x00286688
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			this.rig = ((this.transferrableObject == null) ? base.GetComponentInParent<VRRig>() : this.transferrableObject.ownerRig);
			UnityEvent<int> unityEvent = this.onMovedToNextStep;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.currentStep);
			}
			this.isLocal = (this.transferrableObject != null && this.transferrableObject.IsMyItem()) || (this.rig != null && this.rig == GorillaTagger.Instance.offlineVRRig);
		}

		// Token: 0x17000C24 RID: 3108
		// (get) Token: 0x06007C1A RID: 31770 RVA: 0x00288529 File Offset: 0x00286729
		// (set) Token: 0x06007C1B RID: 31771 RVA: 0x00288531 File Offset: 0x00286731
		public bool TickRunning { get; set; }

		// Token: 0x06007C1C RID: 31772 RVA: 0x0028853C File Offset: 0x0028673C
		public void Tick()
		{
			if (this.IsCoolingDown)
			{
				this.cooldownRemaining -= Time.deltaTime;
				if (this.cooldownRemaining <= 0f)
				{
					this.cooldownRemaining = 0f;
					UnityEvent unityEvent = this.onCooldownFinished;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					if (this.isPressed)
					{
						this.SetPressState(true);
					}
					if (!this.allowCharging && this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
					{
						TickSystem<object>.RemoveTickCallback(this);
					}
				}
			}
			if (!this.IsCoolingDown && this.allowCharging)
			{
				if (this.isPressed)
				{
					if (this.chargeTime < this.maxChargeSeconds)
					{
						this.chargeTime += Time.deltaTime;
						if (this.chargeTime >= this.maxChargeSeconds || this.chargeTime >= this.snapToMaxChargeAt)
						{
							this.chargeTime = this.maxChargeSeconds;
							UnityEvent unityEvent2 = this.onMaxCharge;
							if (unityEvent2 != null)
							{
								unityEvent2.Invoke();
							}
						}
					}
					float chargeFrac = this.GetChargeFrac();
					ContinuousPropertyArray continuousPropertyArray = this.continuousChargingProperties;
					if (continuousPropertyArray != null)
					{
						continuousPropertyArray.ApplyAll(chargeFrac);
					}
					UnityEvent<float> unityEvent3 = this.whileCharging;
					if (unityEvent3 != null)
					{
						unityEvent3.Invoke(chargeFrac);
					}
					this.TryRunHaptics((chargeFrac >= 1f) ? this.maxChargeHapticsIntensity : (chargeFrac * this.chargeHapticsIntensity), Time.deltaTime);
					this.lastStep = this.currentStep;
					this.currentStep = Mathf.Clamp(Mathf.FloorToInt(chargeFrac * (float)this.numberOfProgressSteps), 0, this.numberOfProgressSteps - 1);
					if (this.currentStep >= 0 && this.currentStep != this.lastStep)
					{
						UnityEvent<int> unityEvent4 = this.onMovedToNextStep;
						if (unityEvent4 != null)
						{
							unityEvent4.Invoke(this.currentStep);
						}
						if (this.currentStep == this.numberOfProgressSteps - 1)
						{
							UnityEvent<int> unityEvent5 = this.onReachedLastProgressStep;
							if (unityEvent5 != null)
							{
								unityEvent5.Invoke(this.currentStep);
							}
						}
					}
					if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
					{
						Vector3 linearVelocity = this.velocityEstimator.linearVelocity;
						float num = linearVelocity.magnitude;
						float num2 = Vector3.Dot(linearVelocity / num, this.GetVectorFromBodyToLaunchPosition().normalized);
						num *= Mathf.Ceil(num2 - this.velocityEstimatorMinRigDotProduct);
						if (num >= this.velocityEstimatorStartGestureSpeed)
						{
							this.velocityEstimatorThresholdMet = true;
							return;
						}
						if (this.velocityEstimatorThresholdMet && num < this.velocityEstimatorStopGestureSpeed)
						{
							this.TryShoot();
							return;
						}
					}
				}
				else if (this.chargeTime > 0f)
				{
					this.chargeTime -= Time.deltaTime * this.chargeDecaySpeed;
					if (this.chargeTime <= 0f)
					{
						this.chargeTime = 0f;
						TickSystem<object>.RemoveTickCallback(this);
						ContinuousPropertyArray continuousPropertyArray2 = this.continuousChargingProperties;
						if (continuousPropertyArray2 != null)
						{
							continuousPropertyArray2.ApplyAll(0f);
						}
						UnityEvent<float> unityEvent6 = this.whileCharging;
						if (unityEvent6 == null)
						{
							return;
						}
						unityEvent6.Invoke(0f);
						return;
					}
					else
					{
						float chargeFrac2 = this.GetChargeFrac();
						ContinuousPropertyArray continuousPropertyArray3 = this.continuousChargingProperties;
						if (continuousPropertyArray3 != null)
						{
							continuousPropertyArray3.ApplyAll(chargeFrac2);
						}
						UnityEvent<float> unityEvent7 = this.whileCharging;
						if (unityEvent7 == null)
						{
							return;
						}
						unityEvent7.Invoke(chargeFrac2);
					}
				}
			}
		}

		// Token: 0x06007C1D RID: 31773 RVA: 0x00288819 File Offset: 0x00286A19
		private Vector3 GetVectorFromBodyToLaunchPosition()
		{
			return this.shootFromTransform.position - this.rig.bodyTransform.TransformPoint(this.offsetRigPosition);
		}

		// Token: 0x06007C1E RID: 31774 RVA: 0x00288844 File Offset: 0x00286A44
		private void GetShootPositionAndRotation(out Vector3 position, out Quaternion rotation)
		{
			ProjectileShooterCosmetic.ShootDirection shootDirection = this.shootDirectionType;
			if (shootDirection != ProjectileShooterCosmetic.ShootDirection.LaunchTransformRotation && shootDirection == ProjectileShooterCosmetic.ShootDirection.LineFromRigToLaunchTransform)
			{
				position = this.shootFromTransform.position;
				rotation = Quaternion.LookRotation(position - this.rig.bodyTransform.TransformPoint(this.offsetRigPosition));
				return;
			}
			this.shootFromTransform.GetPositionAndRotation(out position, out rotation);
		}

		// Token: 0x06007C1F RID: 31775 RVA: 0x002888AC File Offset: 0x00286AAC
		private void Shoot()
		{
			float chargeFrac = this.GetChargeFrac();
			float num = Mathf.Lerp(this.shootMinSpeed, this.shootMaxSpeed, this.chargeToShotSpeedCurve.Evaluate(chargeFrac));
			GameObject gameObject = ObjectPools.instance.Instantiate(in this.projectilePrefab, true);
			gameObject.transform.localScale = Vector3.one * this.rig.scaleFactor;
			IProjectile component = gameObject.GetComponent<IProjectile>();
			if (component != null)
			{
				Vector3 vector;
				Quaternion quaternion;
				this.GetShootPositionAndRotation(out vector, out quaternion);
				Vector3 vector2 = quaternion * Vector3.forward * (num * this.rig.scaleFactor);
				component.Launch(vector, quaternion, vector2, chargeFrac, this.rig, this.currentStep);
				if ((in this.projectileTrailPrefab) != -1)
				{
					this.AttachTrail(in this.projectileTrailPrefab, gameObject, vector, false, false);
				}
			}
			UnityEvent<float> unityEvent = this.onShoot;
			if (unityEvent != null)
			{
				unityEvent.Invoke(chargeFrac);
			}
			this.continuousChargingProperties.ApplyAll(0f);
			UnityEvent<float> unityEvent2 = this.whileCharging;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(0f);
			}
			if (this.isLocal)
			{
				UnityEvent<float> unityEvent3 = this.onShootLocal;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(chargeFrac);
				}
			}
			if (this.allowCharging && this.runChargeCancelledEventOnShoot)
			{
				UnityEvent unityEvent4 = this.onChargeCancelled;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke();
				}
			}
			this.TryRunHaptics(chargeFrac * this.shootHapticsIntensity, this.shootHapticsDuration);
			this.SetPressState(false);
			this.cooldownRemaining = this.cooldownSeconds;
			this.chargeTime = 0f;
			this.currentStep = -1;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06007C20 RID: 31776 RVA: 0x00288A38 File Offset: 0x00286C38
		private bool TryShoot()
		{
			if ((!this.IsCoolingDown && this.shootingAllowed && this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge) || (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge && this.chargeTime >= this.maxChargeSeconds))
			{
				this.Shoot();
				return true;
			}
			return false;
		}

		// Token: 0x06007C21 RID: 31777 RVA: 0x00288A74 File Offset: 0x00286C74
		private void TryRunHaptics(float intensity, float duration)
		{
			if (!this.enableHaptics || !this.isLocal || intensity <= 0f)
			{
				return;
			}
			bool flag = this.transferrableObject != null && this.transferrableObject.InLeftHand();
			GorillaTagger.Instance.StartVibration(flag, intensity, duration);
			if (this.hapticsBothHands)
			{
				GorillaTagger.Instance.StartVibration(!flag, intensity, duration);
			}
		}

		// Token: 0x06007C22 RID: 31778 RVA: 0x00288ADC File Offset: 0x00286CDC
		private float GetChargeFrac()
		{
			if (!this.allowCharging)
			{
				return 1f;
			}
			if (this.chargeTime <= 0f)
			{
				return 0f;
			}
			if (this.chargeTime < this.maxChargeSeconds)
			{
				return this.chargeRateCurve.Evaluate(this.chargeTime / this.maxChargeSeconds);
			}
			return 1f;
		}

		// Token: 0x06007C23 RID: 31779 RVA: 0x00288B36 File Offset: 0x00286D36
		private void SetPressState(bool pressed)
		{
			this.isPressed = pressed;
			this.velocityEstimatorThresholdMet = false;
		}

		// Token: 0x06007C24 RID: 31780 RVA: 0x00288B46 File Offset: 0x00286D46
		public void OnButtonPressed()
		{
			this.SetPressState(true);
			if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.ButtonPressed)
			{
				this.TryShoot();
				return;
			}
			if (this.allowCharging || this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06007C25 RID: 31781 RVA: 0x00288B78 File Offset: 0x00286D78
		public void OnButtonReleased()
		{
			if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold && this.velocityEstimatorThresholdMet)
			{
				return;
			}
			ProjectileShooterCosmetic.ShootActivator shootActivator = this.shootActivatorType;
			if ((shootActivator != ProjectileShooterCosmetic.ShootActivator.ButtonReleased && shootActivator != ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge) || !this.TryShoot())
			{
				this.SetPressState(false);
				if (this.allowCharging)
				{
					ContinuousPropertyArray continuousPropertyArray = this.continuousChargingProperties;
					if (continuousPropertyArray != null)
					{
						continuousPropertyArray.ApplyAll(0f);
					}
					UnityEvent<float> unityEvent = this.whileCharging;
					if (unityEvent != null)
					{
						unityEvent.Invoke(0f);
					}
					UnityEvent unityEvent2 = this.onChargeCancelled;
					if (unityEvent2 == null)
					{
						return;
					}
					unityEvent2.Invoke();
				}
			}
		}

		// Token: 0x06007C26 RID: 31782 RVA: 0x00288BFB File Offset: 0x00286DFB
		public void ResetShoot()
		{
			this.isPressed = false;
			this.velocityEstimatorThresholdMet = false;
			this.currentStep = -1;
			this.lastStep = -1;
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007C27 RID: 31783 RVA: 0x00288C20 File Offset: 0x00286E20
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

		// Token: 0x04008E22 RID: 36386
		private const string CHARGE_STR = "allowCharging";

		// Token: 0x04008E23 RID: 36387
		private const string CHARGE_MSG = "only enabled when allowCharging is true.";

		// Token: 0x04008E24 RID: 36388
		private const string HAPTICS_STR = "enableHaptics";

		// Token: 0x04008E25 RID: 36389
		private const string MOVE_STR = "IsMovementShoot";

		// Token: 0x04008E26 RID: 36390
		[SerializeField]
		private HashWrapper projectilePrefab;

		// Token: 0x04008E27 RID: 36391
		[SerializeField]
		private HashWrapper projectileTrailPrefab;

		// Token: 0x04008E28 RID: 36392
		[FormerlySerializedAs("launchActivatorType")]
		[SerializeField]
		private ProjectileShooterCosmetic.ShootActivator shootActivatorType;

		// Token: 0x04008E29 RID: 36393
		[FormerlySerializedAs("launchDirectionType")]
		[SerializeField]
		private ProjectileShooterCosmetic.ShootDirection shootDirectionType;

		// Token: 0x04008E2A RID: 36394
		[SerializeField]
		private Vector3 offsetRigPosition;

		// Token: 0x04008E2B RID: 36395
		[FormerlySerializedAs("launchTransform")]
		[SerializeField]
		private Transform shootFromTransform;

		// Token: 0x04008E2C RID: 36396
		[SerializeField]
		private bool drawShootVector;

		// Token: 0x04008E2D RID: 36397
		[FormerlySerializedAs("cooldown")]
		[SerializeField]
		private float cooldownSeconds;

		// Token: 0x04008E2E RID: 36398
		[Space]
		[SerializeField]
		private bool enableHaptics = true;

		// Token: 0x04008E2F RID: 36399
		[FormerlySerializedAs("hapticsIntensity")]
		[SerializeField]
		private float shootHapticsIntensity = 0.5f;

		// Token: 0x04008E30 RID: 36400
		[FormerlySerializedAs("hapticsDuration")]
		[SerializeField]
		private float shootHapticsDuration = 0.2f;

		// Token: 0x04008E31 RID: 36401
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float chargeHapticsIntensity = 0.3f;

		// Token: 0x04008E32 RID: 36402
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float maxChargeHapticsIntensity = 0.3f;

		// Token: 0x04008E33 RID: 36403
		[SerializeField]
		private bool hapticsBothHands;

		// Token: 0x04008E34 RID: 36404
		[Space]
		[SerializeField]
		private GorillaVelocityEstimator velocityEstimator;

		// Token: 0x04008E35 RID: 36405
		[SerializeField]
		private float velocityEstimatorStartGestureSpeed = 0.5f;

		// Token: 0x04008E36 RID: 36406
		[SerializeField]
		private float velocityEstimatorStopGestureSpeed = 0.2f;

		// Token: 0x04008E37 RID: 36407
		[SerializeField]
		private float velocityEstimatorMinRigDotProduct = 0.5f;

		// Token: 0x04008E38 RID: 36408
		[SerializeField]
		private bool logVelocityEstimatorSpeed;

		// Token: 0x04008E39 RID: 36409
		[FormerlySerializedAs("launchMinSpeed")]
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float shootMinSpeed;

		// Token: 0x04008E3A RID: 36410
		[FormerlySerializedAs("launchMaxSpeed")]
		[SerializeField]
		private float shootMaxSpeed;

		// Token: 0x04008E3B RID: 36411
		[SerializeField]
		private bool allowCharging;

		// Token: 0x04008E3C RID: 36412
		[SerializeField]
		private float maxChargeSeconds = 2f;

		// Token: 0x04008E3D RID: 36413
		[SerializeField]
		private float snapToMaxChargeAt = 9999999f;

		// Token: 0x04008E3E RID: 36414
		[SerializeField]
		private float chargeDecaySpeed = 9999999f;

		// Token: 0x04008E3F RID: 36415
		[SerializeField]
		private bool runChargeCancelledEventOnShoot;

		// Token: 0x04008E40 RID: 36416
		[SerializeField]
		private AnimationCurve chargeRateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008E41 RID: 36417
		[SerializeField]
		private AnimationCurve chargeToShotSpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008E42 RID: 36418
		[FormerlySerializedAs("onReadyToShoot")]
		public UnityEvent onCooldownFinished;

		// Token: 0x04008E43 RID: 36419
		public ContinuousPropertyArray continuousChargingProperties;

		// Token: 0x04008E44 RID: 36420
		public UnityEvent<float> whileCharging;

		// Token: 0x04008E45 RID: 36421
		public UnityEvent onMaxCharge;

		// Token: 0x04008E46 RID: 36422
		public UnityEvent onChargeCancelled;

		// Token: 0x04008E47 RID: 36423
		[FormerlySerializedAs("onLaunchProjectileShared")]
		public UnityEvent<float> onShoot;

		// Token: 0x04008E48 RID: 36424
		[FormerlySerializedAs("onOwnerLaunchProjectile")]
		public UnityEvent<float> onShootLocal;

		// Token: 0x04008E49 RID: 36425
		[SerializeField]
		private int numberOfProgressSteps;

		// Token: 0x04008E4A RID: 36426
		public UnityEvent<int> onMovedToNextStep;

		// Token: 0x04008E4B RID: 36427
		public UnityEvent<int> onReachedLastProgressStep;

		// Token: 0x04008E4C RID: 36428
		private int currentStep = -1;

		// Token: 0x04008E4D RID: 36429
		private int lastStep = -1;

		// Token: 0x04008E4F RID: 36431
		private bool isPressed;

		// Token: 0x04008E50 RID: 36432
		private bool velocityEstimatorThresholdMet;

		// Token: 0x04008E51 RID: 36433
		private float cooldownRemaining;

		// Token: 0x04008E52 RID: 36434
		private float chargeTime;

		// Token: 0x04008E53 RID: 36435
		private TransferrableObject transferrableObject;

		// Token: 0x04008E54 RID: 36436
		private VRRig rig;

		// Token: 0x04008E55 RID: 36437
		private bool isLocal;

		// Token: 0x04008E56 RID: 36438
		private Transform debugShootDirection;

		// Token: 0x02001357 RID: 4951
		private enum ShootActivator
		{
			// Token: 0x04008E59 RID: 36441
			ButtonReleased,
			// Token: 0x04008E5A RID: 36442
			ButtonPressed,
			// Token: 0x04008E5B RID: 36443
			ButtonStayed,
			// Token: 0x04008E5C RID: 36444
			VelocityEstimatorThreshold,
			// Token: 0x04008E5D RID: 36445
			ButtonReleasedFullCharge
		}

		// Token: 0x02001358 RID: 4952
		private enum ShootDirection
		{
			// Token: 0x04008E5F RID: 36447
			LaunchTransformRotation,
			// Token: 0x04008E60 RID: 36448
			LineFromRigToLaunchTransform
		}
	}
}
