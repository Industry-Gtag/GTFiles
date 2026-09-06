using System;
using BoingKit;
using UnityEngine;

// Token: 0x020009FD RID: 2557
public class SyncRigidBodyToMovement : MonoBehaviour
{
	// Token: 0x06004198 RID: 16792 RVA: 0x0015D70A File Offset: 0x0015B90A
	private void Awake()
	{
		this.targetParent = this.targetRigidbody.transform.parent;
		this.targetRigidbody.transform.parent = null;
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x06004199 RID: 16793 RVA: 0x0015D744 File Offset: 0x0015B944
	private void OnEnable()
	{
		this.targetRigidbody.gameObject.SetActive(true);
		this.targetRigidbody.transform.position = base.transform.position;
		this.targetRigidbody.transform.rotation = base.transform.rotation;
	}

	// Token: 0x0600419A RID: 16794 RVA: 0x0015D798 File Offset: 0x0015B998
	private void OnDisable()
	{
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x0600419B RID: 16795 RVA: 0x0015D7AC File Offset: 0x0015B9AC
	private void FixedUpdate()
	{
		this.targetRigidbody.linearVelocity = (base.transform.position - this.targetRigidbody.position) / Time.fixedDeltaTime;
		this.targetRigidbody.angularVelocity = QuaternionUtil.ToAngularVector(Quaternion.Inverse(this.targetRigidbody.rotation) * base.transform.rotation) / Time.fixedDeltaTime;
	}

	// Token: 0x04005246 RID: 21062
	[SerializeField]
	private Rigidbody targetRigidbody;

	// Token: 0x04005247 RID: 21063
	private Transform targetParent;
}
