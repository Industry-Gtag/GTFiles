using System;
using UnityEngine;

// Token: 0x020004B5 RID: 1205
public class KiteDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	// Token: 0x06001D73 RID: 7539 RVA: 0x0009F370 File Offset: 0x0009D570
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtPosition = this.grabPt.position;
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x06001D74 RID: 7540 RVA: 0x0009F3CD File Offset: 0x0009D5CD
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x06001D75 RID: 7541 RVA: 0x0009F3F8 File Offset: 0x0009D5F8
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x0009F448 File Offset: 0x0009D648
	public void EnableDynamics(bool enable, bool collider, bool kinematic)
	{
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = collider;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!enable)
			{
				this.rb.linearVelocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x0009F4B2 File Offset: 0x0009D6B2
	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.rb.useGravity = !enable;
		this.balloonScale = scale;
		this.grabPtPosition = this.grabPt.position;
	}

	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06001D78 RID: 7544 RVA: 0x0009F4DB File Offset: 0x0009D6DB
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x0009F4F8 File Offset: 0x0009D6F8
	private void FixedUpdate()
	{
		if (this.rb.isKinematic || this.rb.useGravity)
		{
			return;
		}
		if (this.enableDynamics)
		{
			Vector3 vector = (this.grabPt.position - this.grabPtPosition) * 100f;
			vector = Matrix4x4.Rotate(this.ctrlRotation).MultiplyVector(vector);
			this.rb.AddForce(vector, ForceMode.Force);
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			this.rb.linearVelocity = linearVelocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
			base.transform.LookAt(base.transform.position - this.rb.linearVelocity);
		}
	}

	// Token: 0x06001D7A RID: 7546 RVA: 0x00002E60 File Offset: 0x00001060
	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001D7B RID: 7547 RVA: 0x0009CDD7 File Offset: 0x0009AFD7
	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	// Token: 0x06001D7C RID: 7548 RVA: 0x0009F5D2 File Offset: 0x0009D7D2
	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		transferOwnership = false;
	}

	// Token: 0x06001D7D RID: 7549 RVA: 0x0009F5D8 File Offset: 0x0009D7D8
	public bool ReturnStep()
	{
		this.rb.isKinematic = true;
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.grabPt.position, Time.deltaTime * this.returnSpeed);
		return base.transform.position == this.grabPt.position;
	}

	// Token: 0x040027AB RID: 10155
	private Rigidbody rb;

	// Token: 0x040027AC RID: 10156
	private Collider balloonCollider;

	// Token: 0x040027AD RID: 10157
	private Bounds bounds;

	// Token: 0x040027AE RID: 10158
	[SerializeField]
	private float bouyancyMinHeight = 10f;

	// Token: 0x040027AF RID: 10159
	[SerializeField]
	private float bouyancyMaxHeight = 20f;

	// Token: 0x040027B0 RID: 10160
	private float bouyancyActualHeight = 20f;

	// Token: 0x040027B1 RID: 10161
	[SerializeField]
	private float airResistance = 0.01f;

	// Token: 0x040027B2 RID: 10162
	public GameObject knot;

	// Token: 0x040027B3 RID: 10163
	private Rigidbody knotRb;

	// Token: 0x040027B4 RID: 10164
	public Transform grabPt;

	// Token: 0x040027B5 RID: 10165
	private Transform grabPtInitParent;

	// Token: 0x040027B6 RID: 10166
	[SerializeField]
	private float maximumVelocity = 2f;

	// Token: 0x040027B7 RID: 10167
	private bool enableDynamics;

	// Token: 0x040027B8 RID: 10168
	[SerializeField]
	private float balloonScale = 1f;

	// Token: 0x040027B9 RID: 10169
	private Vector3 grabPtPosition;

	// Token: 0x040027BA RID: 10170
	[SerializeField]
	private Quaternion ctrlRotation;

	// Token: 0x040027BB RID: 10171
	[SerializeField]
	private float returnSpeed = 50f;
}
