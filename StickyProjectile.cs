using System;
using GorillaLocomotion.Swimming;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005A3 RID: 1443
public class StickyProjectile : MonoBehaviour, IProjectile, ITickSystemTick
{
	// Token: 0x06002486 RID: 9350 RVA: 0x000C402C File Offset: 0x000C222C
	private void Awake()
	{
		this.stickyPart.GetLocalPositionAndRotation(out this.stickyPartLocalPosition, out this.stickyPartLocalRotation);
		this.stickyPartLocalScale = this.stickyPart.localScale;
		this.headZoneInversePosition = this.INVERSE_HEAD_ROTATION * this.headZonePosition;
		this.headZoneInverseLocalPosition = this.INVERSE_HEAD_ROTATION * this.localHeadZonePosition;
		this.rb = base.GetComponent<Rigidbody>();
		this.rbwi = base.GetComponent<RigidbodyWaterInteraction>();
		this.collider = base.GetComponent<Collider>();
		this.pcc = base.GetComponent<PlayerColoredCosmetic>();
		this.triggerLayer = LayerMask.NameToLayer("Gorilla Tag Collider");
		UnityEvent onReset = this.OnReset;
		if (onReset == null)
		{
			return;
		}
		onReset.Invoke();
	}

	// Token: 0x06002487 RID: 9351 RVA: 0x000C40E0 File Offset: 0x000C22E0
	public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
	{
		UnityEvent onLaunch = this.OnLaunch;
		if (onLaunch != null)
		{
			onLaunch.Invoke();
		}
		this.stickyPart.SetParent(base.transform, false);
		this.stickyPart.SetLocalPositionAndRotation(this.stickyPartLocalPosition, this.stickyPartLocalRotation);
		this.stickyPart.localScale = this.stickyPartLocalScale;
		base.transform.SetPositionAndRotation(startPosition, startRotation);
		base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
		this.rb.isKinematic = false;
		this.rb.position = startPosition;
		this.rb.rotation = startRotation;
		this.rb.linearVelocity = velocity;
		if (this.faceVelocityWhileAirborne)
		{
			TickSystem<object>.AddTickCallback(this);
			this.rb.angularVelocity = Vector3.zero;
		}
		else
		{
			this.rb.angularVelocity = Random.onUnitSphere * Random.Range(this.launchRandomSpinSpeedMinMax.x, this.launchRandomSpinSpeedMinMax.y);
		}
		this.rbwi.enabled = true;
		this.collider.enabled = true;
		if (this.pcc != null)
		{
			this.pcc.UpdateColor(ownerRig.playerColor);
		}
	}

	// Token: 0x06002488 RID: 9352 RVA: 0x000C421C File Offset: 0x000C241C
	private void StickTo(Transform otherTransform, Vector3 position, Quaternion rotation)
	{
		this.stickyPart.parent = otherTransform;
		this.stickyPart.SetPositionAndRotation(position + rotation * this.stickyPartLocalPosition, rotation * this.stickyPartLocalRotation);
		this.rb.isKinematic = true;
		this.rbwi.enabled = false;
		this.collider.enabled = false;
	}

	// Token: 0x06002489 RID: 9353 RVA: 0x000C4284 File Offset: 0x000C2484
	private void OnCollisionEnter(Collision collision)
	{
		TickSystem<object>.RemoveTickCallback(this);
		ContactPoint contact = collision.GetContact(0);
		this.StickTo(collision.transform, contact.point, this.alignToHitNormal ? Quaternion.LookRotation(contact.normal, Random.onUnitSphere) : base.transform.rotation);
		this.stickEvents.InvokeAll(StickyProjectile.StickFlags.Wall, false);
	}

	// Token: 0x0600248A RID: 9354 RVA: 0x000C42E8 File Offset: 0x000C24E8
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != this.triggerLayer)
		{
			return;
		}
		TickSystem<object>.RemoveTickCallback(this);
		Vector3 vector = Time.fixedDeltaTime * 2f * this.rb.linearVelocity;
		Vector3 vector2 = base.transform.position - vector;
		Vector3 vector3;
		Quaternion quaternion;
		if (this.alignToHitNormal)
		{
			float magnitude = vector.magnitude;
			RaycastHit raycastHit;
			if (other.Raycast(new Ray(vector2, vector / magnitude), out raycastHit, 2f * magnitude))
			{
				vector3 = raycastHit.point;
			}
			else
			{
				vector3 = base.transform.position;
			}
			quaternion = Quaternion.LookRotation(raycastHit.normal, Random.onUnitSphere);
		}
		else
		{
			vector3 = other.ClosestPoint(vector2);
			quaternion = base.transform.rotation;
		}
		VRRig componentInParent = other.GetComponentInParent<VRRig>();
		if (componentInParent != null)
		{
			if (this.headZoneRadius > 0f && string.Equals(other.name, "SpeakerHeadCollider"))
			{
				Vector3 vector4;
				Quaternion quaternion2;
				other.transform.GetPositionAndRotation(out vector4, out quaternion2);
				Vector3 vector5 = quaternion2 * this.headZoneInversePosition + vector4;
				if ((vector3 - vector5).magnitude <= this.headZoneRadius * componentInParent.scaleFactor)
				{
					if (componentInParent.isOfflineVRRig)
					{
						this.StickTo(other.transform, quaternion2 * this.headZoneInverseLocalPosition + vector4, quaternion2 * this.INVERSE_HEAD_ROTATION);
						this.stickyPart.localScale *= this.scaleOnLocalHeadZone;
						this.stickEvents.InvokeAll(StickyProjectile.StickFlags.LocalHeadZone, false);
						return;
					}
					this.StickTo(other.transform, vector5, quaternion2 * this.INVERSE_HEAD_ROTATION);
					this.stickEvents.InvokeAll(StickyProjectile.StickFlags.RemoteHeadZone, false);
					return;
				}
				else if (componentInParent.isOfflineVRRig)
				{
					this.stickyPart.localScale *= this.scaleOnLocalHead;
				}
			}
			this.stickEvents.InvokeAll(componentInParent.isOfflineVRRig ? StickyProjectile.StickFlags.LocalPlayer : StickyProjectile.StickFlags.RemotePlayer, false);
		}
		else
		{
			this.stickEvents.InvokeAll(StickyProjectile.StickFlags.Wall, false);
		}
		this.StickTo(other.transform, vector3, quaternion);
	}

	// Token: 0x0600248B RID: 9355 RVA: 0x000C4514 File Offset: 0x000C2714
	private void OnEnable()
	{
		this.stickyPart.gameObject.SetActive(true);
	}

	// Token: 0x0600248C RID: 9356 RVA: 0x000C4527 File Offset: 0x000C2727
	private void OnDisable()
	{
		this.stickyPart.gameObject.SetActive(false);
		UnityEvent onReset = this.OnReset;
		if (onReset == null)
		{
			return;
		}
		onReset.Invoke();
	}

	// Token: 0x170003D4 RID: 980
	// (get) Token: 0x0600248D RID: 9357 RVA: 0x000C454A File Offset: 0x000C274A
	// (set) Token: 0x0600248E RID: 9358 RVA: 0x000C4552 File Offset: 0x000C2752
	public bool TickRunning { get; set; }

	// Token: 0x0600248F RID: 9359 RVA: 0x000C455B File Offset: 0x000C275B
	public void Tick()
	{
		this.rb.rotation = Quaternion.LookRotation(this.rb.linearVelocity);
	}

	// Token: 0x04002FE9 RID: 12265
	[SerializeField]
	private Transform stickyPart;

	// Token: 0x04002FEA RID: 12266
	[Tooltip("Align the positive Z direction of this object to the rigidbody's velocity.")]
	[SerializeField]
	private bool faceVelocityWhileAirborne;

	// Token: 0x04002FEB RID: 12267
	[Tooltip("Set the rigidbody's angular velocity to a random unit Vector3, multiplied by a random value in this range.")]
	[SerializeField]
	private Vector2 launchRandomSpinSpeedMinMax = new Vector2(90f, 360f);

	// Token: 0x04002FEC RID: 12268
	[Tooltip("When enabled, the positive Z direction will face away from whatever surface the projectile hit. When disabled, it will keep its original rotation.")]
	[SerializeField]
	private bool alignToHitNormal = true;

	// Token: 0x04002FED RID: 12269
	[Space]
	[SerializeField]
	public UnityEvent OnReset;

	// Token: 0x04002FEE RID: 12270
	[SerializeField]
	public UnityEvent OnLaunch;

	// Token: 0x04002FEF RID: 12271
	[Tooltip("Scale the 'Sticky Part' by this value when hitting the local player's head. Usually used to prevent things from obscuring your vision too much.")]
	[SerializeField]
	private float scaleOnLocalHead = 0.7f;

	// Token: 0x04002FF0 RID: 12272
	[Tooltip("The radius of the head zone. Can be set to 0 to disable head zone functionality.")]
	[SerializeField]
	private float headZoneRadius = 0.15f;

	// Token: 0x04002FF1 RID: 12273
	[Tooltip("The local origin of the head zone, relative to the player rig's head transform. When a shot hits inside the zone, the 'Sticky Part' will be moved to this position relative to the hit player's head.")]
	[SerializeField]
	private Vector3 headZonePosition = new Vector3(0f, 0.02f, 0.17f);

	// Token: 0x04002FF2 RID: 12274
	[Tooltip("Scale the 'Sticky Part' by this value when hitting the local player's head zone. Can override 'Scale On Local Head' in case you want it to appear larger for emphasis.")]
	[SerializeField]
	private float scaleOnLocalHeadZone = 1f;

	// Token: 0x04002FF3 RID: 12275
	[Tooltip("When a shot hits inside a remote player's head zone, it will be moved to the 'Head Zone Relative Position'. For the local player, it will instead be moved here. This DOES NOT AFFECT the actual origin of the head zone for hit-detection purposes, it is purely visual after-the-fact.")]
	[SerializeField]
	private Vector3 localHeadZonePosition = new Vector3(0f, 0.05f, 0.2f);

	// Token: 0x04002FF4 RID: 12276
	[SerializeField]
	private FlagEvents<StickyProjectile.StickFlags> stickEvents;

	// Token: 0x04002FF5 RID: 12277
	private readonly Quaternion INVERSE_HEAD_ROTATION = Quaternion.Inverse(Quaternion.Euler(0f, 270f, 252.3229f));

	// Token: 0x04002FF6 RID: 12278
	private Vector3 headZoneInversePosition;

	// Token: 0x04002FF7 RID: 12279
	private Vector3 headZoneInverseLocalPosition;

	// Token: 0x04002FF8 RID: 12280
	private Vector3 stickyPartLocalPosition;

	// Token: 0x04002FF9 RID: 12281
	private Quaternion stickyPartLocalRotation;

	// Token: 0x04002FFA RID: 12282
	private Vector3 stickyPartLocalScale;

	// Token: 0x04002FFB RID: 12283
	private Rigidbody rb;

	// Token: 0x04002FFC RID: 12284
	private RigidbodyWaterInteraction rbwi;

	// Token: 0x04002FFD RID: 12285
	private Collider collider;

	// Token: 0x04002FFE RID: 12286
	private PlayerColoredCosmetic pcc;

	// Token: 0x04002FFF RID: 12287
	private int triggerLayer;

	// Token: 0x020005A4 RID: 1444
	[Flags]
	public enum StickFlags
	{
		// Token: 0x04003002 RID: 12290
		Wall = 1,
		// Token: 0x04003003 RID: 12291
		LocalPlayer = 2,
		// Token: 0x04003004 RID: 12292
		RemotePlayer = 4,
		// Token: 0x04003005 RID: 12293
		LocalHeadZone = 8,
		// Token: 0x04003006 RID: 12294
		RemoteHeadZone = 16
	}
}
