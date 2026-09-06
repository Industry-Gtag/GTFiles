using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class JellyfishUFOCamera : MonoBehaviour
{
	// Token: 0x0600005A RID: 90 RVA: 0x00003192 File Offset: 0x00001392
	private void Start()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_spring.Reset(this.Target.transform.position);
	}

	// Token: 0x0600005B RID: 91 RVA: 0x000031C0 File Offset: 0x000013C0
	private void FixedUpdate()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_spring.TrackExponential(this.Target.transform.position, 0.5f, Time.fixedDeltaTime);
		Vector3 normalized = (this.m_spring.Value - base.transform.position).normalized;
		base.transform.rotation = Quaternion.LookRotation(normalized);
	}

	// Token: 0x04000040 RID: 64
	public Transform Target;

	// Token: 0x04000041 RID: 65
	private Vector3Spring m_spring;
}
