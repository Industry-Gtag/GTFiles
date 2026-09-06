using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020005FA RID: 1530
public class AutoSyncTransforms : MonoBehaviour
{
	// Token: 0x17000400 RID: 1024
	// (get) Token: 0x0600260D RID: 9741 RVA: 0x000C95DE File Offset: 0x000C77DE
	public Transform TargetTransform
	{
		get
		{
			return this.m_transform;
		}
	}

	// Token: 0x17000401 RID: 1025
	// (get) Token: 0x0600260E RID: 9742 RVA: 0x000C95E6 File Offset: 0x000C77E6
	public Rigidbody TargetRigidbody
	{
		get
		{
			return this.m_rigidbody;
		}
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x000C95F0 File Offset: 0x000C77F0
	private void Awake()
	{
		if (this.m_transform.IsNull())
		{
			this.m_transform = base.transform;
		}
		if (this.m_rigidbody.IsNull())
		{
			this.m_rigidbody = base.GetComponent<Rigidbody>();
		}
		if (this.m_transform.IsNull() || this.m_rigidbody.IsNull())
		{
			base.enabled = false;
			Debug.LogError("AutoSyncTransforms: Rigidbody or Transform is null, disabling!! Please add the missing reference or component", this);
			return;
		}
		this.clean = true;
	}

	// Token: 0x06002610 RID: 9744 RVA: 0x000C9663 File Offset: 0x000C7863
	private void OnEnable()
	{
		if (this.clean)
		{
			PostVRRigPhysicsSynch.AddSyncTarget(this);
		}
	}

	// Token: 0x06002611 RID: 9745 RVA: 0x000C9673 File Offset: 0x000C7873
	private void OnDisable()
	{
		if (this.clean)
		{
			PostVRRigPhysicsSynch.RemoveSyncTarget(this);
		}
	}

	// Token: 0x0400319E RID: 12702
	[SerializeField]
	private Transform m_transform;

	// Token: 0x0400319F RID: 12703
	[SerializeField]
	private Rigidbody m_rigidbody;

	// Token: 0x040031A0 RID: 12704
	private bool clean;
}
