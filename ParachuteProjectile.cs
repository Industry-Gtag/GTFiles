using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020002EB RID: 747
public class ParachuteProjectile : MonoBehaviour, IProjectile, ITickSystemTick
{
	// Token: 0x06001300 RID: 4864 RVA: 0x000651AA File Offset: 0x000633AA
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x000651B8 File Offset: 0x000633B8
	private void OnEnable()
	{
		this.launched = false;
		this.landTime = 0f;
		this.launchedTime = 0f;
		this.peakTime = float.MaxValue;
		this.monkeMeshFilter.mesh = this.launchMesh;
		this.parachute.SetActive(false);
		if (!this.TickRunning)
		{
			TickSystem<object>.AddCallbackTarget(this);
		}
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x00065218 File Offset: 0x00063418
	private void OnDisable()
	{
		this.launched = false;
		if (this.TickRunning)
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x00065230 File Offset: 0x00063430
	public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
	{
		this.parachuteDeployed = false;
		this.landed = false;
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		this.rb.position = startPosition;
		this.rb.rotation = startRotation;
		this.ChangeUp(Vector3.up);
		this.rb.freezeRotation = true;
		if (ownerRig == null)
		{
			base.transform.localScale = Vector3.one;
		}
		else
		{
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
		}
		this.rb.isKinematic = false;
		this.rb.linearVelocity = velocity;
		this.rb.linearDamping = this.initialDrag;
		this.rb.angularDamping = this.initialAngularDrag;
		this.launchedTime = Time.time;
		this.monkeMeshFilter.mesh = this.launchMesh;
		this.parachute.SetActive(false);
		if (velocity.y > 0f)
		{
			this.peakTime = velocity.y / (-1f * Physics.gravity.y);
		}
		else
		{
			this.peakTime = 0f;
		}
		this.launched = true;
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x0006536C File Offset: 0x0006356C
	private void OnPeakReached()
	{
		this.parachuteDeployed = true;
		this.parachute.SetActive(true);
		this.monkeMeshFilter.mesh = this.parachutingMesh;
		this.ChangeUp(Vector3.up);
		this.rb.linearDamping = this.parachuteDrag;
		this.rb.angularDamping = this.parachuteAngularDrag;
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x000653CC File Offset: 0x000635CC
	private void OnLanded(Collision collision)
	{
		this.landTime = Time.time;
		this.landed = true;
		ContactPoint contact = collision.GetContact(0);
		this.rb.isKinematic = true;
		this.rb.position = contact.point + contact.normal * (this.groundOffset * base.transform.localScale.x);
		this.ChangeUp(contact.normal);
		this.monkeMeshFilter.mesh = this.landedMesh;
		this.parachute.SetActive(false);
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x00065464 File Offset: 0x00063664
	private void ChangeUp(Vector3 newUp)
	{
		Vector3 vector = Vector3.Cross(this.rb.transform.right, newUp);
		if (vector.sqrMagnitude < 1E-45f)
		{
			vector = Vector3.Cross(Vector3.Cross(newUp, this.rb.transform.forward), newUp);
		}
		this.rb.rotation = Quaternion.LookRotation(vector, newUp);
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x000654C8 File Offset: 0x000636C8
	private void PlayImpactEffects(Vector3 position, Vector3 normal)
	{
		if (this.impactEffect != null)
		{
			Vector3 vector = position + this.impactEffectOffset * normal;
			GameObject gameObject = ObjectPools.instance.Instantiate(this.impactEffect, vector, true);
			gameObject.transform.localScale = base.transform.localScale * this.impactEffectScaleMultiplier;
			gameObject.transform.up = normal;
		}
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x00065544 File Offset: 0x00063744
	public void OnTriggerEvent(bool isLeft, Collider col)
	{
		if (this.parachuteDeployed)
		{
			this.PlayImpactEffects(base.transform.position, Vector3.up);
			GorillaTriggerColliderHandIndicator componentInParent = col.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent != null)
			{
				float num = GorillaTagger.Instance.tapHapticStrength / 2f;
				float fixedDeltaTime = Time.fixedDeltaTime;
				GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, num, fixedDeltaTime);
			}
		}
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x000655A8 File Offset: 0x000637A8
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.launched || this.landed)
		{
			return;
		}
		ContactPoint contact = collision.GetContact(0);
		if (collision.collider.attachedRigidbody != null)
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaThrowable))
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (!this.parachuteDeployed)
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (Vector3.Angle(contact.normal, Vector3.up) < this.groudUpThreshold)
		{
			this.OnLanded(collision);
			return;
		}
		this.PlayImpactEffects(contact.point, contact.normal);
	}

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x0600130A RID: 4874 RVA: 0x00065671 File Offset: 0x00063871
	// (set) Token: 0x0600130B RID: 4875 RVA: 0x00065679 File Offset: 0x00063879
	public bool TickRunning { get; set; }

	// Token: 0x0600130C RID: 4876 RVA: 0x00065684 File Offset: 0x00063884
	public void Tick()
	{
		if (!this.parachuteDeployed && Time.time > this.launchedTime + this.parachuteDeployDelay && Time.time >= this.launchedTime + this.peakTime)
		{
			this.OnPeakReached();
		}
		if (this.landed && Time.time > this.landTime + this.destroyOnLandDelay)
		{
			this.PlayImpactEffects(base.transform.position, base.transform.up);
		}
	}

	// Token: 0x0400173B RID: 5947
	[SerializeField]
	private MeshFilter monkeMeshFilter;

	// Token: 0x0400173C RID: 5948
	[SerializeField]
	private GameObject parachute;

	// Token: 0x0400173D RID: 5949
	[SerializeField]
	private Mesh launchMesh;

	// Token: 0x0400173E RID: 5950
	[SerializeField]
	private Mesh parachutingMesh;

	// Token: 0x0400173F RID: 5951
	[SerializeField]
	private Mesh landedMesh;

	// Token: 0x04001740 RID: 5952
	[Tooltip("time to wait after launch before deploying the parachute")]
	[SerializeField]
	private float parachuteDeployDelay = 1f;

	// Token: 0x04001741 RID: 5953
	[Tooltip("time to wait after landing before destroying")]
	[SerializeField]
	private float destroyOnLandDelay = 3f;

	// Token: 0x04001742 RID: 5954
	[Tooltip("How far from the collision point should the projectile sit when landed")]
	[SerializeField]
	private float groundOffset;

	// Token: 0x04001743 RID: 5955
	[Tooltip("Acceptable angle in degrees of surface from world up to be considered the ground")]
	[SerializeField]
	private float groudUpThreshold = 45f;

	// Token: 0x04001744 RID: 5956
	[Tooltip("Drag before the parachute is deployed.")]
	[SerializeField]
	private float initialDrag;

	// Token: 0x04001745 RID: 5957
	[Tooltip("Drag before the parachute is deployed.")]
	[SerializeField]
	private float initialAngularDrag = 0.05f;

	// Token: 0x04001746 RID: 5958
	[Tooltip("Drag after the parachute is deployed.")]
	[SerializeField]
	private float parachuteDrag = 5f;

	// Token: 0x04001747 RID: 5959
	[Tooltip("Drag after the parachute is deployed.")]
	[SerializeField]
	private float parachuteAngularDrag = 10f;

	// Token: 0x04001748 RID: 5960
	[SerializeField]
	private GameObject impactEffect;

	// Token: 0x04001749 RID: 5961
	[SerializeField]
	private float impactEffectScaleMultiplier = 1f;

	// Token: 0x0400174A RID: 5962
	[Tooltip("Distance from the surface that the particle should spawn.")]
	[SerializeField]
	private float impactEffectOffset;

	// Token: 0x0400174B RID: 5963
	private Rigidbody rb;

	// Token: 0x0400174C RID: 5964
	private bool launched;

	// Token: 0x0400174D RID: 5965
	private float launchedTime;

	// Token: 0x0400174E RID: 5966
	private float landTime;

	// Token: 0x0400174F RID: 5967
	private float peakTime = float.MaxValue;

	// Token: 0x04001750 RID: 5968
	private bool parachuteDeployed;

	// Token: 0x04001751 RID: 5969
	private bool landed;
}
