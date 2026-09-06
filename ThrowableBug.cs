using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000E48 RID: 3656
public class ThrowableBug : TransferrableObject, ITickSystemTick
{
	// Token: 0x1700087E RID: 2174
	// (get) Token: 0x06005942 RID: 22850 RVA: 0x001CFF5E File Offset: 0x001CE15E
	// (set) Token: 0x06005943 RID: 22851 RVA: 0x001CFF66 File Offset: 0x001CE166
	public bool TickRunning { get; set; }

	// Token: 0x06005944 RID: 22852 RVA: 0x001CFF70 File Offset: 0x001CE170
	protected override void Start()
	{
		base.Start();
		float num = Random.Range(0f, 6.2831855f);
		this.targetVelocity = new Vector3(Mathf.Sin(num) * this.maxNaturalSpeed, 0f, Mathf.Cos(num) * this.maxNaturalSpeed);
		this.currentState = TransferrableObject.PositionState.Dropped;
		this.rayCastNonAllocColliders = new RaycastHit[5];
		this.rayCastNonAllocColliders2 = new RaycastHit[5];
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.currentZone = this.startZone;
	}

	// Token: 0x06005945 RID: 22853 RVA: 0x001CFFF8 File Offset: 0x001CE1F8
	internal override void OnEnable()
	{
		base.OnEnable();
		ThrowableBugBeacon.OnCall += this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss += this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock += this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock += this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier += this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06005946 RID: 22854 RVA: 0x001D0068 File Offset: 0x001CE268
	internal override void OnDisable()
	{
		base.OnDisable();
		ThrowableBugBeacon.OnCall -= this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss -= this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock -= this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock -= this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier -= this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06005947 RID: 22855 RVA: 0x001D00D8 File Offset: 0x001CE2D8
	private bool isValid(ThrowableBugBeacon tbb)
	{
		return tbb.BugName == this.bugName && (tbb.Range <= 0f || Vector3.Distance(tbb.transform.position, base.transform.position) <= tbb.Range);
	}

	// Token: 0x06005948 RID: 22856 RVA: 0x001D012A File Offset: 0x001CE32A
	private void ThrowableBugBeacon_OnCall(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
		}
	}

	// Token: 0x06005949 RID: 22857 RVA: 0x001D015C File Offset: 0x001CE35C
	private void ThrowableBugBeacon_OnLock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
			this.lockedTarget = tbb.transform;
			this.locked = true;
		}
	}

	// Token: 0x0600594A RID: 22858 RVA: 0x001D01AB File Offset: 0x001CE3AB
	private void ThrowableBugBeacon_OnDismiss(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = base.transform.position - tbb.transform.position;
			this.locked = false;
		}
	}

	// Token: 0x0600594B RID: 22859 RVA: 0x001D01E3 File Offset: 0x001CE3E3
	private void ThrowableBugBeacon_OnUnlock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.locked = false;
		}
	}

	// Token: 0x0600594C RID: 22860 RVA: 0x001D01F5 File Offset: 0x001CE3F5
	private void ThrowableBugBeacon_OnChangeSpeedMultiplier(ThrowableBugBeacon tbb, float f)
	{
		if (this.isValid(tbb))
		{
			this.speedMultiplier = f;
		}
	}

	// Token: 0x0600594D RID: 22861 RVA: 0x00023F0C File Offset: 0x0002210C
	public override bool ShouldBeKinematic()
	{
		return true;
	}

	// Token: 0x0600594E RID: 22862 RVA: 0x001D0208 File Offset: 0x001CE408
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.raycastFrameCounter = (this.raycastFrameCounter + 1) % this.raycastFramePeriod;
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
		if (this.animator.enabled)
		{
			this.animator.SetBool(ThrowableBug._g_IsHeld, flag);
		}
		this.animator.enabled = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone == this.currentZone;
		if (!this.audioSource)
		{
			return;
		}
		switch (this.currentAudioState)
		{
		case ThrowableBug.AudioState.JustGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			if (this.grabBugAudioClip && this.audioSource.clip != this.grabBugAudioClip)
			{
				this.audioSource.clip = this.grabBugAudioClip;
				this.audioSource.time = 0f;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			else if (!this.audioSource.isPlaying)
			{
				this.currentAudioState = ThrowableBug.AudioState.ContinuallyGrabbed;
				return;
			}
			break;
		case ThrowableBug.AudioState.ContinuallyGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			break;
		case ThrowableBug.AudioState.JustReleased:
			if (!flag)
			{
				if (this.releaseBugAudioClip && this.audioSource.clip != this.releaseBugAudioClip)
				{
					this.audioSource.clip = this.releaseBugAudioClip;
					this.audioSource.time = 0f;
					if (this.audioSource.isActiveAndEnabled)
					{
						this.audioSource.GTPlay();
						return;
					}
				}
				else if (!this.audioSource.isPlaying)
				{
					this.currentAudioState = ThrowableBug.AudioState.NotHeld;
					return;
				}
			}
			else
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
			}
			break;
		case ThrowableBug.AudioState.NotHeld:
			if (flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
				return;
			}
			if (this.flyingBugAudioClip && !this.audioSource.isPlaying)
			{
				this.audioSource.clip = this.flyingBugAudioClip;
				this.audioSource.time = 0f;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600594F RID: 22863 RVA: 0x001D0430 File Offset: 0x001CE630
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this.reliableState)
		{
			return;
		}
		if ((this.currentState & TransferrableObject.PositionState.Dropped) == TransferrableObject.PositionState.None)
		{
			return;
		}
		if (this.locked && Vector3.Distance(this.lockedTarget.position, base.transform.position) > 0.1f)
		{
			this.reliableState.travelingDirection = this.lockedTarget.position - base.transform.position;
		}
		if (this.slowingDownProgress < 1f)
		{
			this.slowingDownProgress += this.slowdownAcceleration * Time.deltaTime;
			this.reliableState.travelingDirection = Vector3.Slerp(this.thrownVeloicity, this.targetVelocity, Mathf.SmoothStep(0f, 1f, this.slowingDownProgress));
		}
		else
		{
			this.reliableState.travelingDirection = this.reliableState.travelingDirection.normalized * this.maxNaturalSpeed;
		}
		this.bobingFrequency = (this.shouldRandomizeFrequency ? this.RandomizeBobingFrequency() : this.bobbingDefaultFrequency);
		float num = this.bobingState + this.bobingSpeed * Time.deltaTime;
		float num2 = Mathf.Sin(num / this.bobingFrequency) - Mathf.Sin(this.bobingState / this.bobingFrequency);
		Vector3 vector = Vector3.up * (num2 * this.bobMagnintude);
		this.bobingState = num;
		if (this.bobingState > 6.2831855f)
		{
			this.bobingState -= 6.2831855f;
		}
		vector += this.reliableState.travelingDirection * Time.deltaTime;
		float num3 = (this.isTooHighTravelingDown ? this.minimumHeightOffOfTheGroundBeforeStoppingDescent : this.maximumHeightOffOfTheGroundBeforeStartingDescent);
		float num4 = (this.isTooLowTravelingUp ? this.maximumHeightOffOfTheGroundBeforeStoppingAscent : this.minimumHeightOffOfTheGroundBeforeStartingAscent);
		if (this.raycastFrameCounter == 0)
		{
			if (Physics.RaycastNonAlloc(base.transform.position, Vector3.down, this.rayCastNonAllocColliders2, num3, this.collisionCheckMask) > 0)
			{
				this.isTooHighTravelingDown = false;
				if (this.descentSlerp > 0f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp - this.descentSlerpRate * Time.deltaTime);
				}
				RaycastHit raycastHit = this.rayCastNonAllocColliders2[0];
				this.isTooLowTravelingUp = raycastHit.distance < num4;
				if (this.isTooLowTravelingUp)
				{
					if (this.ascentSlerp < 1f)
					{
						this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp + this.ascentSlerpRate * Time.deltaTime);
					}
				}
				else if (this.ascentSlerp > 0f)
				{
					this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp - this.ascentSlerpRate * Time.deltaTime);
				}
			}
			else
			{
				this.isTooHighTravelingDown = true;
				if (this.descentSlerp < 1f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp + this.descentSlerpRate * Time.deltaTime);
				}
			}
		}
		vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.descentSlerp) * this.descentRate * Vector3.down;
		vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.ascentSlerp) * this.ascentRate * Vector3.up;
		float num5;
		Vector3 vector2;
		Quaternion.FromToRotation(base.transform.rotation * Vector3.up, Quaternion.identity * Vector3.up).ToAngleAxis(out num5, out vector2);
		Quaternion quaternion = Quaternion.AngleAxis(num5 * 0.02f, vector2);
		float num6;
		Vector3 vector3;
		Quaternion.FromToRotation(base.transform.rotation * Vector3.forward, this.reliableState.travelingDirection.normalized).ToAngleAxis(out num6, out vector3);
		Quaternion quaternion2 = Quaternion.AngleAxis(num6 * 0.005f, vector3);
		quaternion = quaternion2 * quaternion;
		vector = quaternion * quaternion * quaternion * quaternion * vector;
		vector *= this.speedMultiplier;
		this.speedMultiplier = Mathf.MoveTowards(this.speedMultiplier, 1f, Time.deltaTime);
		if (this.raycastFrameCounter == 0)
		{
			if (Physics.SphereCastNonAlloc(base.transform.position, this.collisionHitRadius, vector.normalized, this.rayCastNonAllocColliders, vector.magnitude, this.collisionCheckMask) > 0)
			{
				Vector3 normal = this.rayCastNonAllocColliders[0].normal;
				this.reliableState.travelingDirection = Vector3.Reflect(this.reliableState.travelingDirection, normal).x0z();
				base.transform.position += Vector3.Reflect(vector, normal);
				this.thrownVeloicity = Vector3.Reflect(this.thrownVeloicity, normal);
				this.targetVelocity = Vector3.Reflect(this.targetVelocity, normal).x0z();
			}
			else
			{
				base.transform.position += vector;
			}
		}
		else
		{
			base.transform.position += vector;
		}
		this.bugRotationalVelocity = quaternion * this.bugRotationalVelocity;
		float num7;
		Vector3 vector4;
		this.bugRotationalVelocity.ToAngleAxis(out num7, out vector4);
		this.bugRotationalVelocity = Quaternion.AngleAxis(num7 * 0.9f, vector4);
		base.transform.rotation = this.bugRotationalVelocity * base.transform.rotation;
	}

	// Token: 0x06005950 RID: 22864 RVA: 0x001D09B5 File Offset: 0x001CEBB5
	private float RandomizeBobingFrequency()
	{
		return Random.Range(this.minRandFrequency, this.maxRandFrequency);
	}

	// Token: 0x06005951 RID: 22865 RVA: 0x001D09C8 File Offset: 0x001CEBC8
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.slowingDownProgress = 0f;
		Vector3 linearVelocity = this.velocityEstimator.linearVelocity;
		this.thrownVeloicity = linearVelocity;
		this.reliableState.travelingDirection = linearVelocity;
		this.bugRotationalVelocity = Quaternion.Euler(this.velocityEstimator.angularVelocity);
		this.startingSpeed = linearVelocity.magnitude;
		Vector3 normalized = this.reliableState.travelingDirection.x0z().normalized;
		this.targetVelocity = normalized * this.maxNaturalSpeed;
		return true;
	}

	// Token: 0x06005952 RID: 22866 RVA: 0x001D0A5A File Offset: 0x001CEC5A
	public void OnCollisionEnter(Collision collision)
	{
		this.reliableState.travelingDirection *= -1f;
	}

	// Token: 0x06005953 RID: 22867 RVA: 0x001D0A78 File Offset: 0x001CEC78
	public void Tick()
	{
		if (this.updateMultiplier > 0)
		{
			for (int i = 0; i < this.updateMultiplier; i++)
			{
				this.LateUpdateLocal();
			}
		}
	}

	// Token: 0x0400697F RID: 27007
	public ThrowableBugReliableState reliableState;

	// Token: 0x04006980 RID: 27008
	public float slowingDownProgress;

	// Token: 0x04006981 RID: 27009
	public float startingSpeed;

	// Token: 0x04006982 RID: 27010
	public int raycastFramePeriod = 5;

	// Token: 0x04006983 RID: 27011
	private int raycastFrameCounter;

	// Token: 0x04006984 RID: 27012
	public float bobingSpeed = 1f;

	// Token: 0x04006985 RID: 27013
	public float bobMagnintude = 0.1f;

	// Token: 0x04006986 RID: 27014
	public bool shouldRandomizeFrequency;

	// Token: 0x04006987 RID: 27015
	public float minRandFrequency = 0.008f;

	// Token: 0x04006988 RID: 27016
	public float maxRandFrequency = 1f;

	// Token: 0x04006989 RID: 27017
	public float bobingFrequency = 1f;

	// Token: 0x0400698A RID: 27018
	public float bobingState;

	// Token: 0x0400698B RID: 27019
	public float thrownYVelocity;

	// Token: 0x0400698C RID: 27020
	public float collisionHitRadius;

	// Token: 0x0400698D RID: 27021
	public LayerMask collisionCheckMask;

	// Token: 0x0400698E RID: 27022
	public Vector3 thrownVeloicity;

	// Token: 0x0400698F RID: 27023
	public Vector3 targetVelocity;

	// Token: 0x04006990 RID: 27024
	public Quaternion bugRotationalVelocity;

	// Token: 0x04006991 RID: 27025
	private RaycastHit[] rayCastNonAllocColliders;

	// Token: 0x04006992 RID: 27026
	private RaycastHit[] rayCastNonAllocColliders2;

	// Token: 0x04006993 RID: 27027
	public VRRig followingRig;

	// Token: 0x04006994 RID: 27028
	public bool isTooHighTravelingDown;

	// Token: 0x04006995 RID: 27029
	public float descentSlerp;

	// Token: 0x04006996 RID: 27030
	public float ascentSlerp;

	// Token: 0x04006997 RID: 27031
	public float maxNaturalSpeed;

	// Token: 0x04006998 RID: 27032
	public float slowdownAcceleration;

	// Token: 0x04006999 RID: 27033
	public float maximumHeightOffOfTheGroundBeforeStartingDescent = 5f;

	// Token: 0x0400699A RID: 27034
	public float minimumHeightOffOfTheGroundBeforeStoppingDescent = 3f;

	// Token: 0x0400699B RID: 27035
	public float descentRate = 0.2f;

	// Token: 0x0400699C RID: 27036
	public float descentSlerpRate = 0.2f;

	// Token: 0x0400699D RID: 27037
	public float minimumHeightOffOfTheGroundBeforeStartingAscent = 0.5f;

	// Token: 0x0400699E RID: 27038
	public float maximumHeightOffOfTheGroundBeforeStoppingAscent = 0.75f;

	// Token: 0x0400699F RID: 27039
	public float ascentRate = 0.4f;

	// Token: 0x040069A0 RID: 27040
	public float ascentSlerpRate = 1f;

	// Token: 0x040069A1 RID: 27041
	private bool isTooLowTravelingUp;

	// Token: 0x040069A2 RID: 27042
	public Animator animator;

	// Token: 0x040069A3 RID: 27043
	[FormerlySerializedAs("grabBugAudioSource")]
	public AudioClip grabBugAudioClip;

	// Token: 0x040069A4 RID: 27044
	[FormerlySerializedAs("releaseBugAudioSource")]
	public AudioClip releaseBugAudioClip;

	// Token: 0x040069A5 RID: 27045
	[FormerlySerializedAs("flyingBugAudioSource")]
	public AudioClip flyingBugAudioClip;

	// Token: 0x040069A6 RID: 27046
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040069A7 RID: 27047
	public GTZone startZone;

	// Token: 0x040069A8 RID: 27048
	private GTZone currentZone;

	// Token: 0x040069A9 RID: 27049
	private float bobbingDefaultFrequency = 1f;

	// Token: 0x040069AA RID: 27050
	public int updateMultiplier;

	// Token: 0x040069AB RID: 27051
	private ThrowableBug.AudioState currentAudioState;

	// Token: 0x040069AC RID: 27052
	private float speedMultiplier = 1f;

	// Token: 0x040069AD RID: 27053
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040069AF RID: 27055
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x040069B0 RID: 27056
	private Transform lockedTarget;

	// Token: 0x040069B1 RID: 27057
	private bool locked;

	// Token: 0x040069B2 RID: 27058
	private static readonly int _g_IsHeld = Animator.StringToHash("isHeld");

	// Token: 0x02000E49 RID: 3657
	public enum BugName
	{
		// Token: 0x040069B4 RID: 27060
		NONE,
		// Token: 0x040069B5 RID: 27061
		DougTheBug,
		// Token: 0x040069B6 RID: 27062
		MattTheBat
	}

	// Token: 0x02000E4A RID: 3658
	private enum AudioState
	{
		// Token: 0x040069B8 RID: 27064
		JustGrabbed,
		// Token: 0x040069B9 RID: 27065
		ContinuallyGrabbed,
		// Token: 0x040069BA RID: 27066
		JustReleased,
		// Token: 0x040069BB RID: 27067
		NotHeld
	}
}
