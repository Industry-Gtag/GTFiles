using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class ColliderSpinner : MonoBehaviour
{
	// Token: 0x06000066 RID: 102 RVA: 0x00003840 File Offset: 0x00001A40
	private void Start()
	{
		this.m_targetOffset = ((this.Target != null) ? (base.transform.position - this.Target.position) : Vector3.zero);
		this.m_spring.Reset(base.transform.position);
	}

	// Token: 0x06000067 RID: 103 RVA: 0x0000389C File Offset: 0x00001A9C
	private void FixedUpdate()
	{
		Vector3 vector = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(vector, 0.02f, Time.fixedDeltaTime);
	}

	// Token: 0x04000058 RID: 88
	public Transform Target;

	// Token: 0x04000059 RID: 89
	private Vector3 m_targetOffset;

	// Token: 0x0400005A RID: 90
	private Vector3Spring m_spring;
}
