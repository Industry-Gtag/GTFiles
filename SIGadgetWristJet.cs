using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000119 RID: 281
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetWristJet : SIGadget, I_SIDisruptable, IEnergyGadget
{
	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000708 RID: 1800 RVA: 0x00028114 File Offset: 0x00026314
	private bool CanRecharge
	{
		get
		{
			return (!this.rechargeRequiresFloorTouch || this._floorTouched) && this.state == SIGadgetWristJet.State.Unactive;
		}
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x00028134 File Offset: 0x00026334
	private void Awake()
	{
		this._maxSqrHorizontalSpeed = this.maxHorizontalSpeed * this.maxHorizontalSpeed;
		this._hasThrustLoopAudioSource = this.m_thrustLoopAudioSource != null;
		this.m_warnFuelLowThreshold = ((this.m_warnFuelLowSound != null) ? this.m_warnFuelLowThreshold : (-1f));
		this._hasInactiveStateVisual = this.inactiveStateVisual != null;
		this._hasActiveStateVisual = this.activeStateVisual != null;
		this._gaugeMatPropBlock = new MaterialPropertyBlock();
		this._baseFuelSpendRate = this.fuelSpendRate;
		this._baseJetForce = this.jetForce;
		this._baseMaxVerticalSpeed = this.maxVerticalSpeed;
		this._baseMaxHorizontalSpeed = this.maxHorizontalSpeed;
		if (this.m_gaugeMatSlots == null)
		{
			this.m_gaugeMatSlots = Array.Empty<GTRendererMatSlot>();
		}
		int num = 0;
		for (int i = 0; i < this.m_gaugeMatSlots.Length; i++)
		{
			if (this.m_gaugeMatSlots[i].TryInitialize())
			{
				this.m_gaugeMatSlots[num] = this.m_gaugeMatSlots[i];
				num++;
			}
		}
		if (num != this.m_gaugeMatSlots.Length)
		{
			Array.Resize<GTRendererMatSlot>(ref this.m_gaugeMatSlots, num);
		}
		this.throttleFlapInitialRots = ((this.m_throttleFlapXforms != null) ? new Quaternion[this.m_throttleFlapXforms.Length] : Array.Empty<Quaternion>());
		for (int j = 0; j < this.throttleFlapInitialRots.Length; j++)
		{
			if (this.m_throttleFlapXforms[j] == null)
			{
				this.throttleFlapInitialRots = Array.Empty<Quaternion>();
				Debug.LogError("[SIGadgetWristJet]  ERROR!!!  Awake: Throttle indicator flaps will not animate because entry is null in " + string.Format("array at `{0}[{1}]`. Path={2}", "m_throttleFlapXforms", j, base.transform.GetPathQ()), this);
				return;
			}
			this.throttleFlapInitialRots[j] = this.m_throttleFlapXforms[j].localRotation;
		}
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x000282F0 File Offset: 0x000264F0
	private void Start()
	{
		this.gtPlayer = GTPlayer.Instance;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnReleased = (Action)Delegate.Combine(gameEntity.OnReleased, new Action(this.HandleStopInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Combine(gameEntity2.OnUnsnapped, new Action(this.HandleStopInteraction));
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x0002836D File Offset: 0x0002656D
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.m_warnFuelLowThreshold > 0f)
		{
			this.m_warnFuelLowSound.LoadAudioData();
		}
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x0002838E File Offset: 0x0002658E
	protected override void OnDisable()
	{
		if (this.m_warnFuelLowThreshold > 0f && this.m_warnFuelLowSound.loadState != AudioDataLoadState.Unloaded)
		{
			this.m_warnFuelLowSound.UnloadAudioData();
		}
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x000283B8 File Offset: 0x000265B8
	protected override void Update()
	{
		base.Update();
		if (this._hasThrustLoopAudioSource)
		{
			float num = ((this.state == SIGadgetWristJet.State.Active) ? this.m_thrustLoopSoundVolume : 0f);
			float num2 = ((this.state == SIGadgetWristJet.State.Active) ? this.m_thrustLoopAudioFadeInTime : this.m_thrustLoopAudioFadeOutTime);
			this.m_thrustLoopAudioSource.volume = Mathf.MoveTowards(this.m_thrustLoopAudioSource.volume, num, 1f / num2 * Time.unscaledDeltaTime);
		}
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x0002842C File Offset: 0x0002662C
	private void FixedUpdate()
	{
		if (!this.IsEquippedLocal() && !this.activatedLocally)
		{
			return;
		}
		if (this.state == SIGadgetWristJet.State.Active && this.currentFuel > 0f && this.buttonActivatable.CheckInput(0.25f) && !base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			this.gtPlayer.AddForce(-Physics.gravity * (this.gtPlayer.scale * this.gravityNegationPercent), ForceMode.Acceleration);
			this._ApplyClampedThrust();
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x000284AE File Offset: 0x000266AE
	private void HandleStopInteraction()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		this.SetStateAuthority(SIGadgetWristJet.State.Unactive);
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x000284C8 File Offset: 0x000266C8
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		bool flag = this.buttonActivatable.CheckInput(0.25f);
		if (!this._floorTouched)
		{
			this._floorTouched = this.gtPlayer.IsGroundedButt || this.gtPlayer.IsGroundedHand;
		}
		if (this._throttleControl)
		{
			Vector2 joystickInput = base.GetJoystickInput();
			if (Mathf.Abs(joystickInput.y) > 0.75f && Mathf.Abs(joystickInput.x) < 0.5f)
			{
				this._throttle = Mathf.Clamp01(this._throttle + joystickInput.y * this.throttleChangeSpeed * Time.deltaTime);
				this._currentBurnRate = Mathf.Lerp(this.minimumBurnRate, 1f, this._throttle);
				this.UpdateThrottleIndicator();
			}
		}
		switch (this.state)
		{
		case SIGadgetWristJet.State.Unactive:
			if (flag && !base.IsBlocked(SIExclusionType.AffectsLocalMovement))
			{
				this.SetStateAuthority(SIGadgetWristJet.State.Active);
			}
			break;
		case SIGadgetWristJet.State.Active:
			this.currentFuel = Mathf.Clamp(this.currentFuel - dt * this.fuelSpendRate * this._currentBurnRate, 0f, this.fuelSize);
			this._floorTouched = false;
			this.gtPlayer.ThrusterActiveAtFrame = Time.frameCount;
			if (flag && this.m_warnFuelLowThreshold > 0f)
			{
				float num = this.currentFuel / this.fuelSize;
				if (this._warnFuelLowSoundWasPlayed && num > this.m_warnFuelLowThreshold)
				{
					this._warnFuelLowSoundWasPlayed = false;
				}
				else if (!this._warnFuelLowSoundWasPlayed && num <= this.m_warnFuelLowThreshold)
				{
					this._warnFuelLowSoundWasPlayed = true;
					this.gameEntity.audioSource.GTPlayOneShot(this.m_warnFuelLowSound, this.m_warnFuelLowSoundVolume);
				}
			}
			if (!flag || this.currentFuel <= 0f)
			{
				this.SetStateAuthority(SIGadgetWristJet.State.OutOfFuel);
			}
			break;
		case SIGadgetWristJet.State.OutOfFuel:
			if (!flag)
			{
				this.emptiedCooldownResetProgress += dt;
			}
			else if (this.currentFuel > 0f)
			{
				this.SetStateAuthority(SIGadgetWristJet.State.Active);
			}
			if (this.emptiedCooldownResetProgress > this.emptiedCooldown)
			{
				this.emptiedCooldownResetProgress = 0f;
				this.SetStateAuthority(SIGadgetWristJet.State.Unactive);
			}
			break;
		}
		float num2 = this.currentFuel / this.fuelSize;
		for (int i = 0; i < this.m_gaugeMatSlots.Length; i++)
		{
			this._gaugeMatPropBlock.SetFloat(ShaderProps._EmissionDissolveProgress, num2);
			this.m_gaugeMatSlots[i].renderer.SetPropertyBlock(this._gaugeMatPropBlock, this.m_gaugeMatSlots[i].slot);
		}
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00028744 File Offset: 0x00026944
	private void UpdateThrottleIndicator()
	{
		for (int i = 0; i < this.throttleFlapInitialRots.Length; i++)
		{
			Quaternion quaternion = this.throttleFlapInitialRots[i] * this.m_throttleFlapMaxRotOffset;
			this.m_throttleFlapXforms[i].localRotation = Quaternion.Lerp(this.throttleFlapInitialRots[i], quaternion, this._throttle);
		}
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x000287A4 File Offset: 0x000269A4
	private void _ApplyClampedThrust()
	{
		Vector3 rigidbodyVelocity = this.gtPlayer.RigidbodyVelocity;
		float num = this.jetForce * this._currentBurnRate;
		Vector3 vector = rigidbodyVelocity + base.transform.forward * (num * Time.fixedDeltaTime);
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		if (vector2.sqrMagnitude > this._maxSqrHorizontalSpeed)
		{
			float magnitude = new Vector3(rigidbodyVelocity.x, 0f, rigidbodyVelocity.z).magnitude;
			vector2 = Vector3.ClampMagnitude(vector2, Mathf.Max(this.maxHorizontalSpeed, magnitude));
		}
		Vector3 vector3 = vector2;
		vector3.y = ((vector.y > this.maxVerticalSpeed) ? Mathf.Max(this.maxVerticalSpeed, rigidbodyVelocity.y) : vector.y);
		this.gtPlayer.AddForce(vector3 - rigidbodyVelocity, ForceMode.VelocityChange);
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x0002888C File Offset: 0x00026A8C
	private void OnEntityStateChanged(long oldState, long newState)
	{
		SIGadgetWristJet.State state = (SIGadgetWristJet.State)oldState;
		SIGadgetWristJet.State state2 = (SIGadgetWristJet.State)newState;
		if (state != state2)
		{
			this.SetState(state2);
		}
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x000288A8 File Offset: 0x00026AA8
	private void SetStateAuthority(SIGadgetWristJet.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x000288CC File Offset: 0x00026ACC
	private void SetState(SIGadgetWristJet.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetWristJet.State.Unactive:
			if (this._hasInactiveStateVisual)
			{
				this.inactiveStateVisual.SetActive(true);
			}
			if (this._hasActiveStateVisual)
			{
				this.activeStateVisual.SetActive(false);
				return;
			}
			break;
		case SIGadgetWristJet.State.Active:
			if (this._hasInactiveStateVisual)
			{
				this.inactiveStateVisual.SetActive(false);
			}
			if (this._hasActiveStateVisual)
			{
				this.activeStateVisual.SetActive(true);
				return;
			}
			break;
		case SIGadgetWristJet.State.OutOfFuel:
			if (this._hasInactiveStateVisual)
			{
				this.inactiveStateVisual.SetActive(true);
			}
			if (this._hasActiveStateVisual)
			{
				this.activeStateVisual.SetActive(false);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x00028980 File Offset: 0x00026B80
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._throttleControl = withUpgrades.Contains(SIUpgradeType.Thruster_Throttle_Control);
		if (this._throttleControl)
		{
			this.UpdateThrottleIndicator();
		}
		switch (this.jetType)
		{
		case SIGadgetWristJet.WristJetType.Jet:
			this.fuelSpendRate = this._baseFuelSpendRate * (withUpgrades.Contains(SIUpgradeType.Thruster_Jet_Duration) ? 0.8f : 1f);
			this.jetForce = this._baseJetForce * (withUpgrades.Contains(SIUpgradeType.Thruster_Jet_Accel) ? 1.2f : 1f);
			break;
		case SIGadgetWristJet.WristJetType.Propellor:
			this.fuelSpendRate = this._baseFuelSpendRate * (withUpgrades.Contains(SIUpgradeType.Thruster_Prop_Duration) ? 0.8f : 1f);
			this.maxVerticalSpeed = this._baseMaxVerticalSpeed * (withUpgrades.Contains(SIUpgradeType.Thruster_Prop_Speed) ? 1.2f : 1f);
			this.maxHorizontalSpeed = this._baseMaxHorizontalSpeed * (withUpgrades.Contains(SIUpgradeType.Thruster_Prop_Speed) ? 1.2f : 1f);
			break;
		}
		AudioClip audioClip;
		if (this._hasThrustLoopAudioSource && this.m_thrustLoopSoundByUpgrade.TryGetActiveValue(withUpgrades, out audioClip))
		{
			this.m_thrustLoopAudioSource.clip = audioClip;
			this.m_thrustLoopAudioSource.Play();
		}
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00028AA8 File Offset: 0x00026CA8
	public void Disrupt(float disruptTime)
	{
		this.emptiedCooldownResetProgress = -disruptTime;
		this.SetState(SIGadgetWristJet.State.OutOfFuel);
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00028ABC File Offset: 0x00026CBC
	public override void OnEntityInit()
	{
		this.emptiedCooldownResetProgress = 0f;
		if (this._hasInactiveStateVisual)
		{
			this.inactiveStateVisual.SetActive(true);
		}
		if (this._hasActiveStateVisual)
		{
			this.activeStateVisual.SetActive(false);
		}
		this.currentFuel = (this.fuelSize = 10f);
		this._throttle = (this._currentBurnRate = 1f);
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000719 RID: 1817 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool UsesEnergy
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x0600071A RID: 1818 RVA: 0x00028B24 File Offset: 0x00026D24
	public bool IsFull
	{
		get
		{
			return this.currentFuel >= this.fuelSize;
		}
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00028B37 File Offset: 0x00026D37
	public void UpdateRecharge(float dt)
	{
		if (this.CanRecharge)
		{
			this.currentFuel = Mathf.Clamp(this.currentFuel + dt * this.fuelGainRate, 0f, this.fuelSize);
		}
	}

	// Token: 0x040008C5 RID: 2245
	private const string preLog = "[SIGadgetWristJet]  ";

	// Token: 0x040008C6 RID: 2246
	private const string preErr = "[SIGadgetWristJet]  ERROR!!!  ";

	// Token: 0x040008C7 RID: 2247
	private const string preErrBeta = "[SIGadgetWristJet]  ERROR!!!  (beta only log)  ";

	// Token: 0x040008C8 RID: 2248
	[SerializeField]
	private AudioSource m_thrustLoopAudioSource;

	// Token: 0x040008C9 RID: 2249
	private bool _hasThrustLoopAudioSource;

	// Token: 0x040008CA RID: 2250
	[SerializeField]
	private SIUpgradeBasedGeneric<AudioClip> m_thrustLoopSoundByUpgrade;

	// Token: 0x040008CB RID: 2251
	[SerializeField]
	private float m_thrustLoopAudioFadeInTime = 0.1f;

	// Token: 0x040008CC RID: 2252
	[SerializeField]
	private float m_thrustLoopAudioFadeOutTime = 0.5f;

	// Token: 0x040008CD RID: 2253
	[SerializeField]
	private float m_thrustLoopSoundVolume = 0.33f;

	// Token: 0x040008CE RID: 2254
	[SerializeField]
	private AudioClip m_warnFuelLowSound;

	// Token: 0x040008CF RID: 2255
	[SerializeField]
	private float m_warnFuelLowThreshold = 0.5f;

	// Token: 0x040008D0 RID: 2256
	[SerializeField]
	private float m_warnFuelLowSoundVolume = 0.05f;

	// Token: 0x040008D1 RID: 2257
	private bool _warnFuelLowSoundWasPlayed;

	// Token: 0x040008D2 RID: 2258
	[Tooltip("This renderer's material will have the `_EmissionDissolveProgress` property changed to visually communicate current fuel amount.")]
	[SerializeField]
	private GTRendererMatSlot[] m_gaugeMatSlots;

	// Token: 0x040008D3 RID: 2259
	public SIGadgetWristJet.WristJetType jetType;

	// Token: 0x040008D4 RID: 2260
	public GameButtonActivatable buttonActivatable;

	// Token: 0x040008D5 RID: 2261
	public GameObject inactiveStateVisual;

	// Token: 0x040008D6 RID: 2262
	private bool _hasInactiveStateVisual;

	// Token: 0x040008D7 RID: 2263
	[FormerlySerializedAs("jetFlame")]
	public GameObject activeStateVisual;

	// Token: 0x040008D8 RID: 2264
	private bool _hasActiveStateVisual;

	// Token: 0x040008D9 RID: 2265
	public float jetForce;

	// Token: 0x040008DA RID: 2266
	public float fuelGainRate;

	// Token: 0x040008DB RID: 2267
	public float fuelSpendRate;

	// Token: 0x040008DC RID: 2268
	public float emptiedCooldown;

	// Token: 0x040008DD RID: 2269
	public float gravityNegationPercent;

	// Token: 0x040008DE RID: 2270
	public float maxVerticalSpeed;

	// Token: 0x040008DF RID: 2271
	public float maxHorizontalSpeed;

	// Token: 0x040008E0 RID: 2272
	[SerializeField]
	private bool rechargeRequiresFloorTouch;

	// Token: 0x040008E1 RID: 2273
	[SerializeField]
	private float throttleChangeSpeed = 2f;

	// Token: 0x040008E2 RID: 2274
	[SerializeField]
	[Tooltip("Minimum proportion of thrust allowed with throttle control.")]
	[Range(0f, 1f)]
	private float minimumBurnRate = 0.33f;

	// Token: 0x040008E3 RID: 2275
	[SerializeField]
	private Transform[] m_throttleFlapXforms;

	// Token: 0x040008E4 RID: 2276
	private Quaternion[] throttleFlapInitialRots;

	// Token: 0x040008E5 RID: 2277
	[SerializeField]
	private Quaternion m_throttleFlapMaxRotOffset = Quaternion.Euler(45f, 0f, 0f);

	// Token: 0x040008E6 RID: 2278
	private float fuelSize;

	// Token: 0x040008E7 RID: 2279
	private float currentFuel;

	// Token: 0x040008E8 RID: 2280
	private SIGadgetWristJet.State state;

	// Token: 0x040008E9 RID: 2281
	private GTPlayer gtPlayer;

	// Token: 0x040008EA RID: 2282
	private float emptiedCooldownResetProgress;

	// Token: 0x040008EB RID: 2283
	private bool _floorTouched;

	// Token: 0x040008EC RID: 2284
	private float _maxSqrHorizontalSpeed;

	// Token: 0x040008ED RID: 2285
	private const float kFUEL_CAPACITY = 10f;

	// Token: 0x040008EE RID: 2286
	private MaterialPropertyBlock _gaugeMatPropBlock;

	// Token: 0x040008EF RID: 2287
	private bool _throttleControl;

	// Token: 0x040008F0 RID: 2288
	private float _throttle;

	// Token: 0x040008F1 RID: 2289
	private float _currentBurnRate;

	// Token: 0x040008F2 RID: 2290
	private float _baseFuelSpendRate;

	// Token: 0x040008F3 RID: 2291
	private float _baseJetForce;

	// Token: 0x040008F4 RID: 2292
	private float _baseMaxVerticalSpeed;

	// Token: 0x040008F5 RID: 2293
	private float _baseMaxHorizontalSpeed;

	// Token: 0x0200011A RID: 282
	private enum State
	{
		// Token: 0x040008F7 RID: 2295
		Unactive,
		// Token: 0x040008F8 RID: 2296
		Active,
		// Token: 0x040008F9 RID: 2297
		OutOfFuel
	}

	// Token: 0x0200011B RID: 283
	public enum WristJetType
	{
		// Token: 0x040008FB RID: 2299
		Basic,
		// Token: 0x040008FC RID: 2300
		Jet,
		// Token: 0x040008FD RID: 2301
		Propellor
	}
}
