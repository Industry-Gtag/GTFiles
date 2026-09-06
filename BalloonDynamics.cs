using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020004A6 RID: 1190
public class BalloonDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	// Token: 0x06001CE0 RID: 7392 RVA: 0x0009C988 File Offset: 0x0009AB88
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x0009C9D4 File Offset: 0x0009ABD4
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x06001CE2 RID: 7394 RVA: 0x0009CA00 File Offset: 0x0009AC00
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x06001CE3 RID: 7395 RVA: 0x0009CA50 File Offset: 0x0009AC50
	private void ApplyBouyancyForce()
	{
		float num = this.bouyancyActualHeight + Mathf.Sin(Time.time) * this.varianceMaxheight;
		float num2 = (num - base.transform.position.y) / num;
		float num3 = this.bouyancyForce * num2 * this.balloonScale;
		this.rb.AddForce(new Vector3(0f, num3, 0f) * this.rb.mass, ForceMode.Force);
	}

	// Token: 0x06001CE4 RID: 7396 RVA: 0x0009CAC8 File Offset: 0x0009ACC8
	private void ApplyUpRightForce()
	{
		Vector3 vector = Vector3.Cross(base.transform.up, Vector3.up) * this.upRightTorque * this.balloonScale;
		this.rb.AddTorque(vector);
	}

	// Token: 0x06001CE5 RID: 7397 RVA: 0x0009CB10 File Offset: 0x0009AD10
	private void ApplyAntiSpinForce()
	{
		Vector3 vector = this.rb.transform.InverseTransformDirection(this.rb.angularVelocity);
		this.rb.AddRelativeTorque(0f, -vector.y * this.antiSpinTorque, 0f);
	}

	// Token: 0x06001CE6 RID: 7398 RVA: 0x0009CB5C File Offset: 0x0009AD5C
	private void ApplyAirResistance()
	{
		this.rb.linearVelocity *= 1f - this.airResistance;
	}

	// Token: 0x06001CE7 RID: 7399 RVA: 0x0009CB80 File Offset: 0x0009AD80
	private void ApplyDistanceConstraint()
	{
		this.knot.transform.position - base.transform.position;
		Vector3 vector = this.grabPt.transform.position - this.knot.transform.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		float num = this.stringLength * this.balloonScale;
		if (magnitude > num)
		{
			Vector3 vector2 = Vector3.Dot(this.knotRb.linearVelocity, normalized) * normalized;
			float num2 = magnitude - num;
			float num3 = num2 / Time.fixedDeltaTime;
			if (vector2.magnitude < num3)
			{
				float num4 = num3 - vector2.magnitude;
				float num5 = Mathf.Clamp01(num2 / this.stringStretch);
				Vector3 vector3 = Mathf.Lerp(0f, num4, num5 * num5) * normalized * this.stringStrength;
				this.rb.AddForceAtPosition(vector3 * this.rb.mass, this.knot.transform.position, ForceMode.Impulse);
			}
		}
	}

	// Token: 0x06001CE8 RID: 7400 RVA: 0x0009CC9C File Offset: 0x0009AE9C
	public void EnableDynamics(bool enable, bool collider, bool kinematic)
	{
		bool flag = !this.enableDynamics && enable;
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = collider;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!kinematic && flag)
			{
				this.rb.linearVelocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001CE9 RID: 7401 RVA: 0x0009CD17 File Offset: 0x0009AF17
	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.enableDistanceConstraints = enable;
		this.balloonScale = scale;
	}

	// Token: 0x17000313 RID: 787
	// (get) Token: 0x06001CEA RID: 7402 RVA: 0x0009CD27 File Offset: 0x0009AF27
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x06001CEB RID: 7403 RVA: 0x0009CD44 File Offset: 0x0009AF44
	private void FixedUpdate()
	{
		if (this.enableDynamics && !this.rb.isKinematic)
		{
			this.ApplyBouyancyForce();
			if (this.antiSpinTorque > 0f)
			{
				this.ApplyAntiSpinForce();
			}
			this.ApplyUpRightForce();
			this.ApplyAirResistance();
			if (this.enableDistanceConstraints)
			{
				this.ApplyDistanceConstraint();
			}
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			this.rb.linearVelocity = linearVelocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
		}
	}

	// Token: 0x06001CEC RID: 7404 RVA: 0x00002E60 File Offset: 0x00001060
	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001CED RID: 7405 RVA: 0x0009CDD7 File Offset: 0x0009AFD7
	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	// Token: 0x06001CEE RID: 7406 RVA: 0x0009CDE0 File Offset: 0x0009AFE0
	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		if (!other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
		{
			return;
		}
		if (!this.rb)
		{
			return;
		}
		transferOwnership = true;
		TransformFollow component = other.gameObject.GetComponent<TransformFollow>();
		if (!component)
		{
			return;
		}
		Vector3 vector = (component.transform.position - component.prevPos) / Time.deltaTime;
		force = vector * this.bopSpeed;
		force = Mathf.Min(this.maximumVelocity, force.magnitude) * force.normalized * this.balloonScale;
		if (this.bopSpeedCap > 0f && force.IsLongerThan(this.bopSpeedCap))
		{
			force = force.normalized * this.bopSpeedCap;
		}
		collisionPt = other.ClosestPointOnBounds(base.transform.position);
		this.rb.AddForceAtPosition(force * this.rb.mass, collisionPt, ForceMode.Impulse);
		if (this.balloonBopSource != null)
		{
			this.balloonBopSource.GTPlay();
		}
		GorillaTriggerColliderHandIndicator component2 = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component2 != null)
		{
			float num = GorillaTagger.Instance.tapHapticStrength / 4f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			GorillaTagger.Instance.StartVibration(component2.isLeftHand, num, fixedDeltaTime);
		}
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool ReturnStep()
	{
		return true;
	}

	// Token: 0x0400270F RID: 9999
	private Rigidbody rb;

	// Token: 0x04002710 RID: 10000
	private Collider balloonCollider;

	// Token: 0x04002711 RID: 10001
	private Bounds bounds;

	// Token: 0x04002712 RID: 10002
	public float bouyancyForce = 1f;

	// Token: 0x04002713 RID: 10003
	public float bouyancyMinHeight = 10f;

	// Token: 0x04002714 RID: 10004
	public float bouyancyMaxHeight = 20f;

	// Token: 0x04002715 RID: 10005
	private float bouyancyActualHeight = 20f;

	// Token: 0x04002716 RID: 10006
	public float varianceMaxheight = 5f;

	// Token: 0x04002717 RID: 10007
	public float airResistance = 0.01f;

	// Token: 0x04002718 RID: 10008
	public GameObject knot;

	// Token: 0x04002719 RID: 10009
	private Rigidbody knotRb;

	// Token: 0x0400271A RID: 10010
	public Transform grabPt;

	// Token: 0x0400271B RID: 10011
	private Transform grabPtInitParent;

	// Token: 0x0400271C RID: 10012
	public float stringLength = 2f;

	// Token: 0x0400271D RID: 10013
	public float stringStrength = 0.9f;

	// Token: 0x0400271E RID: 10014
	public float stringStretch = 0.1f;

	// Token: 0x0400271F RID: 10015
	public float maximumVelocity = 2f;

	// Token: 0x04002720 RID: 10016
	public float upRightTorque = 1f;

	// Token: 0x04002721 RID: 10017
	public float antiSpinTorque;

	// Token: 0x04002722 RID: 10018
	private bool enableDynamics;

	// Token: 0x04002723 RID: 10019
	private bool enableDistanceConstraints;

	// Token: 0x04002724 RID: 10020
	public float balloonScale = 1f;

	// Token: 0x04002725 RID: 10021
	public float bopSpeed = 1f;

	// Token: 0x04002726 RID: 10022
	public float bopSpeedCap;

	// Token: 0x04002727 RID: 10023
	[SerializeField]
	private AudioSource balloonBopSource;
}
